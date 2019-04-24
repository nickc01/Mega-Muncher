using Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using static Direction;

public enum TargetingState
{
    Random, //Chooses a random direction to go in
    Targeting //Moves towards the target
}

public abstract class Ghost : GameEventHandler, IEatable
{
    protected static List<Ghost> SpawnedGhosts = new List<Ghost>(); //A list of spawned ghosts in the level
    public static ReadOnlyCollection<Ghost> Ghosts => SpawnedGhosts.AsReadOnly(); //The public interface for accessing the ghosts in the level
    public static bool AllVulnerable //Sets all the ghosts to be vulnerable or not
    {
        set => SpawnedGhosts.ForEach(ghost => ghost.Vulnerable = value);
    }
    public static bool AllVulnerableVisual //Sets all the ghosts to appear vulnerable or not
    {
        set => SpawnedGhosts.ForEach(ghost => ghost.VulnerableVisual = value);
    }

    public TargetingState CurrentState { get; protected set; } = TargetingState.Targeting; //The current targeting state of the ghost
    public Color SpriteColor { get; protected set; } = Color.white; //The normal color of the ghost
    public Color VulnerableColor { get; protected set; } = new Color32(140, 20, 252, 255); //The color of the ghost when it's vulnerable
    public bool AllowReversing { get; protected set; } = false; //Determines if the ghost can make 180 degree turns
    public bool Enabled { get; protected set; } = false; //Determines whether the ghost should be moving or not
    public Vector3Int SpawnPoint { get; private set; } //The spawnpoint for the ghost
    public virtual Vector3 Target => Muncher.MainMuncher.transform.position; //The target the ghost should move towards
    private bool vulnerableInternal = false; //Whether the ghost is vulnerable or not
    private bool vulnerableVisualInternal = false; //Whether the ghost appears vulnerable or not
    public bool Vulnerable //The public interface for the vulnerability of the ghost
    {
        get => vulnerableInternal;
        set
        {
            vulnerableInternal = value;
            VulnerableVisual = value;
        }
    }
    public bool VulnerableVisual //The public interface for the apperance of the ghost
    {
        get => vulnerableVisualInternal;
        set
        {
            vulnerableVisualInternal = value;
            if (value)
            {
                //Make the ghost appear vulnerable
                renderer.color = VulnerableColor;
            }
            else
            {
                //Make the ghost appear normal
                renderer.color = SpriteColor;
            }
            //Update the ghost's eye direction
            SetEyeDirection(CurrentDirection);
        }
    }

    private bool deadInternal = false; //Whether the ghost is dead or not
    public bool Dead //The public interface for whether the ghost is dead or not
    {
        get => deadInternal;
        set
        {
            deadInternal = value;
            if (value)
            {
                //If the ghost is dead, trigger any events that are called when a ghost dies
                OnDead?.Invoke(this);
            }
            //Make the ghost invisible if dead, and visible otherwise
            renderer.enabled = !value;
            //The ghost is no longer vulnerable
            Vulnerable = false;
        }
    }

    public event Action<Ghost> OnDead; //The event that is called when the ghost dies

    protected Vector3Int PreviousPosition { get; private set; } //The position the ghost was just previously at
    protected Direction CurrentDirection { get; private set; } = None; //The current direction of the ghost
    protected Vector3Int NextPosition { get; private set; } //The position the ghost is traveling towards
    protected virtual float Speed => 4f; //The speed the ghost travels normally
    protected virtual float VulnerableSpeed => Speed / 2f; //The speed the ghost travels when vulnerable
    protected virtual float DeadSpeed => Speed * 2f; //The speed the ghost travels when it is dead
    private float CurrentSpeed => Dead ? DeadSpeed : Vulnerable ? VulnerableSpeed : Speed; //Gets the speed the ghost is currently
    private bool MovingToTile = false; //Whether the ghost is moving towards a tile or not. If this is false, the the ghost will calculate the next tile it should move to
    private float MoveCounter = 0f; //Used to interpolate from the PreviousPosition to the NextPosition
    private Coroutine UnStuckerRoutine; //Used to get the ghost unstuck if the ghost cannot find it's spawnpoint when it is dead

    private Animator eyeAnimator; //The animator for the eyes of the ghost
    private new SpriteRenderer renderer; //The sprite renderer of the ghost

    /*private static bool AreOpposites(Direction A, Direction B)
    {
        return B == OppositeOf(A);
    }
    protected static Direction OppositeOf(Direction A)
    {
        switch (A)
        {
            case Up:
                return Down;
            case Down:
                return Up;
            case Left:
                return Right;
            case Right:
                return Left;
            default:
                return default;
        }
    }
    protected static Vector3Int DirToVector(Direction A)
    {
        switch (A)
        {
            case Up:
                return Vector3Int.up;
            case Down:
                return Vector3Int.down;
            case Left:
                return Vector3Int.left;
            case Right:
                return Vector3Int.right;
            case None:
                return Vector3Int.zero;
        }
        return default;
    }*/


    protected virtual Direction PickDirection(Vector3Int PreviousPosition,Vector3Int CurrentPosition, Direction PreviousDirection)
    {
        //If the targeting is random
        if (CurrentState == TargetingState.Random)
        {
            //A list of valid direction to travel in
            List<Direction> validDirections = new List<Direction>();
            //Add the valid directions the ghost can travel in
            if (PreviousDirection != Down && !Level.Map.HasTile(CurrentPosition + Vector3Int.up))
            {
                validDirections.Add(Up);
            }
            if (PreviousDirection != Up && !Level.Map.HasTile(CurrentPosition + Vector3Int.down))
            {
                validDirections.Add(Down);
            }
            if (PreviousDirection != Right && !Level.Map.HasTile(CurrentPosition + Vector3Int.left))
            {
                validDirections.Add(Left);
            }
            if (PreviousDirection != Left && !Level.Map.HasTile(CurrentPosition + Vector3Int.right))
            {
                validDirections.Add(Right);
            }
            //If there are no valid directions, then take reversing 180 degrees into account
            if (validDirections.Count == 0 && !Level.Map.HasTile(CurrentPosition + PreviousDirection.Opposite().ToVector()))
            {
                validDirections.Add(PreviousDirection.Opposite());
            }
            //Returns a random direction from the list
            return RandomDirection(validDirections);
        }
        else
        {
            //Get the target. If it is dead, the go to the spawnpoint. If not, then go to the target
            var target = Dead ? SpawnPoint : Target;
            //Get one unit above the current position and get it's distance from the player
            var UpTarget = Vector3.Distance(CurrentPosition + Vector3.up, target);
            //Get one unit below the current position and get it's distance from the player
            var DownTarget = Vector3.Distance(CurrentPosition + Vector3.down, target);
            //Get one unit to the left the current position and get it's distance from the player
            var LeftTarget = Vector3.Distance(CurrentPosition + Vector3.left, target);
            //Get one unit to the right the current position and get it's distance from the player
            var RightTarget = Vector3.Distance(CurrentPosition + Vector3.right, target);
            //Initializes the best direction to travel in.
            //If the ghost is vulnerable, find the best direction away from the target
            //If not, then find the best direction towards the target
            (float distance, Direction direction) BestDirection = (Vulnerable && !Dead ? float.NegativeInfinity : float.PositiveInfinity, None);
            //For each direction one unit from the ghost, get the distance to the muncher.
            //Then, if reversing is enabled, make sure that the ghost does not go in the opposite direction
            //Finally, make sure that there is no tile occupied in the direction
            //If these conditions are met, then it will update the best direction
            if (DistanceChecker(UpTarget, BestDirection.distance) && ((PreviousDirection != Down && !AllowReversing) || AllowReversing) && !Level.Map.HasTile(CurrentPosition + Vector3Int.up))
            {
                BestDirection = (UpTarget, Up);
            }
            if (DistanceChecker(DownTarget, BestDirection.distance) && ((PreviousDirection != Up && !AllowReversing) || AllowReversing) && !Level.Map.HasTile(CurrentPosition + Vector3Int.down))
            {
                BestDirection = (DownTarget, Down);
            }
            if (DistanceChecker(LeftTarget, BestDirection.distance) && ((PreviousDirection != Right && !AllowReversing) || AllowReversing) && !Level.Map.HasTile(CurrentPosition + Vector3Int.left))
            {
                BestDirection = (LeftTarget, Left);
            }
            if (DistanceChecker(RightTarget, BestDirection.distance) && ((PreviousDirection != Left && !AllowReversing) || AllowReversing) && !Level.Map.HasTile(CurrentPosition + Vector3Int.right))
            {
                BestDirection = (RightTarget, Right);
            }
            //If there is no suitable direction to take, then take reversing into account
            if (BestDirection.direction == None && !Level.Map.HasTile(CurrentPosition + PreviousDirection.Opposite().ToVector()))
            {
                BestDirection = (BestDirection.distance, PreviousDirection.Opposite());
            }
            //Return the best direction to take
            return BestDirection.direction;
        }
    }

    //Chooses the larger distance when vulnerable to move away from the target
    //Chooses the shorter distance when normal to move towards the target
    private bool DistanceChecker(float MuncherDistance, float CurrentBestDistance)
    {
        return Vulnerable && !Dead ? MuncherDistance > CurrentBestDistance : MuncherDistance < CurrentBestDistance;
    }

    
    protected virtual void OnLevelFinish()
    {
        
    }

    //Called when the level is finished
    protected override void OnLevelEnd()
    {
        //Remove this ghost from the list of spawned ghosts
        SpawnedGhosts.Remove(this);
        //Call the base function
        base.OnLevelEnd();
    }

    protected override void OnLevelReset()
    {
        //Reset the move counter and MoveToTile variable
        MoveCounter = 0;
        MovingToTile = false;
        //Make the ghost no longer dead
        Dead = false;
        //Set it's position the the spawnpoint
        transform.position = SpawnPoint + new Vector3(0.5f, 0.5f);
        //Reset the previous position to the spawnpoint
        PreviousPosition = SpawnPoint;
        if (UnStuckerRoutine != null)
        {
            StopCoroutine(UnStuckerRoutine);
            UnStuckerRoutine = null;
        }
        //Run the PickDirection Function to pick the best direction to travel from here
        var direction = PickDirection(PreviousPosition, PreviousPosition, None);
        var result = direction.ToVector();
        //If a direction is chosen and there is no tile occupied in that space
        if (result != Vector3Int.zero && !Level.Map.HasTile(PreviousPosition + result))
        {
            //The ghost is moving towards a tile
            MovingToTile = true;
            //Set the next tile to move to
            NextPosition = PreviousPosition + result;
            //Set the current direction
            CurrentDirection = direction;
        }
        else
        {
            //If the direction is not valid, then set the next position to the previous position
            NextPosition = PreviousPosition;
        }
    }

    //Called when the game starts
    protected override void OnGameStart()
    {
        //Enable the ghost so it can start moving
        Enabled = true;
    }

    //When the game wants to stop all objects from moving
    protected override void OnGamePause()
    {
        //Disable the ghost from moving
        Enabled = false;
        //If the unstucker routine is running, then stop it
        if (UnStuckerRoutine != null)
        {
            StopCoroutine(UnStuckerRoutine);
            UnStuckerRoutine = null;
        }
    }

    //When the ghost spawns
    public virtual void OnGhostSpawn(Vector3Int spawnPoint)
    {
        //Start the Game Event Handler
        StartEvents();
        //Set the spawnpoint
        SpawnPoint = spawnPoint;
        //Add this ghost to a list if spawned objects
        SpawnedGhosts.Add(this);
        //Get the eye animator for the ghost
        eyeAnimator = transform.GetChild(0).GetComponent<Animator>();
        //Get the sprite renderer of the ghost
        renderer = GetComponent<SpriteRenderer>();
        //Get the sprite color
        SpriteColor = renderer.color;
        //Reset the ghost to the spawn point
        OnLevelReset();
        //Start the ghost update function
        StartCoroutine(GhostUpdate());
    }

    //Sets where the eyes are looking
    private void SetEyeDirection(Direction direction)
    {
        //Reset the values
        eyeAnimator.SetBool("Up", false);
        eyeAnimator.SetBool("Down", false);
        eyeAnimator.SetBool("Left", false);
        eyeAnimator.SetBool("Right", false);
        //If the ghost is not vulerable
        if (!Vulnerable)
        {
            //Set the eye direction based on the direction set
            switch (direction)
            {
                case Direction.Up:
                    eyeAnimator.SetBool("Up", true);
                    break;
                case Direction.Down:
                    eyeAnimator.SetBool("Down", true);
                    break;
                case Direction.Left:
                    eyeAnimator.SetBool("Left", true);
                    break;
                case Direction.Right:
                    eyeAnimator.SetBool("Right", true);
                    break;
                default:
                    break;
            }
        }
    }

    [RuntimeInitializeOnLoadMethod]
    //This method is called when the play button is pressed due to the "RuntimeInitializeOnLoadMethod" attribute
    public static void LoadInitialization()
    {
        //Add an event to clear the spawned ghosts list when the level is closed
        GameManager.OnLevelUnload += () => SpawnedGhosts.Clear();
    }

    /*private static Vector3Int DirectionToVector(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                return Vector3Int.up;
            case Direction.Down:
                return Vector3Int.down;
            case Direction.Left:
                return Vector3Int.left;
            case Direction.Right:
                return Vector3Int.right;
            case Direction.None:
                return Vector3Int.zero;
            default:
                return default;
        }
    }*/

    //Picks a random direction from a list of directions
    private static Direction RandomDirection(List<Direction> directions)
    {
        return directions[UnityEngine.Random.Range(0,directions.Count)];
    }

    //The function called every frame to move the ghost
    private IEnumerator GhostUpdate()
    {
        while (true)
        {
            //If the ghost is enabled
            if (Enabled)
            {
                //If the ghost is currently not moving towards a tile, then it needs a new tile to move to
                if (MovingToTile == false)
                {
                    //If the ghost is dead and it is at the spawnpoitn
                    if (Dead && NextPosition == SpawnPoint)
                    {
                        //The ghost can now respawn
                        Dead = false;
                        //If the unstucker routine is running, then stop it
                        if (UnStuckerRoutine != null)
                        {
                            StopCoroutine(UnStuckerRoutine);
                            UnStuckerRoutine = null;
                        }
                    }
                    //Pick the best direction to travel to
                    var direction = PickDirection(PreviousPosition, NextPosition, CurrentDirection);
                    var result = direction.ToVector();
                    //If the direction is valid and there is no tile occupied there
                    if (result != Vector3Int.zero && !Level.Map.HasTile(NextPosition + result))
                    {
                        //Set the ghost to move to the tile
                        MovingToTile = true;
                        //Reset the move counter
                        MoveCounter = 0f;
                        //Update the previous position
                        PreviousPosition = NextPosition;
                        //Set the next position to travel to
                        NextPosition = NextPosition + result;
                        //Update the current direction
                        CurrentDirection = direction;
                        //Update the eye direction
                        SetEyeDirection(direction);
                    }
                }
                //If the ghost is moving towards a tile
                else
                {
                    //Increase the counter
                    MoveCounter += Time.deltaTime * CurrentSpeed;
                    //Clamp the counter down to 1 if it is greater
                    if (MoveCounter > 1)
                    {
                        MoveCounter = 1;
                    }
                    //Linearly interpolate from the previous position to the next position based on the move counter
                    var CurrentPosition = Vector3.Lerp(PreviousPosition, NextPosition, MoveCounter);
                    //Update the ghost's actual position
                    transform.position = CurrentPosition + new Vector3(0.5f, 0.5f);
                    //If the ghost has reached the next tile
                    if (MoveCounter == 1)
                    {
                        //If the tile the ghost has landed on is a teleporter
                        var teleporter = Teleporter.GetTeleporter(NextPosition);
                        if (teleporter != null)
                        {
                            //Teleport!!!
                            var telePosition = teleporter.LinkedTeleporter.Position;
                            //Update the prevous position, the next position, and the actual position
                            PreviousPosition = telePosition;
                            NextPosition = telePosition;
                            transform.position = NextPosition + new Vector3(0.5f, 0.5f);
                        }
                        //Ghost is no longer moving to a new tile, and needs a new tile to move to
                        MovingToTile = false;
                        continue;
                    }
                }
            }
            //Wait a frame
            yield return null;
        }
    }

    //Used in case if the ghost gets stuck when trying to move back to the spawnpoint
    IEnumerator UnStucker()
    {
        //Wait for 8 seconds
        yield return new WaitForSeconds(8f);
        //Teleport back to the spawn
        OnLevelReset();
    }

    //Called when the ghost is eaten by the player
    public void OnEat(Muncher muncher)
    {
        //If the ghost is already dead, then don't do anything
        if (Dead)
        {
            return;
        }
        //If the ghost is vulnerable
        if (Vulnerable)
        {
            //Increase the score
            ScoreCounter.Score += 10;
            //Play the "Eat Ghost" Sound
            AudioSource.PlayClipAtPoint(GameManager.Game.EatGhostSound, CameraManager.Main.transform.position);
            //Make the ghost dead
            Dead = true;
            //Start the unstucking routine
            UnStuckerRoutine = StartCoroutine(UnStucker());
        }
        //If the ghost is normal
        else
        {
            //Cause the player to loose a life
            GameManager.CurrentGameState = GameState.Lose;
        }
    }
}
