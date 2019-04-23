using Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using static Direction;

public enum GhostState
{
    Random,
    Targeting
}

public abstract class Ghost : GameEventHandler, IEatable
{
    protected static List<Ghost> SpawnedGhosts = new List<Ghost>();
    public static ReadOnlyCollection<Ghost> Ghosts => SpawnedGhosts.AsReadOnly();
    public static bool AllVulnerable
    {
        set => SpawnedGhosts.ForEach(ghost => ghost.Vulnerable = value);
    }
    public static bool AllVulnerableVisual
    {
        set => SpawnedGhosts.ForEach(ghost => ghost.VulnerableVisual = value);
    }

    public GhostState CurrentState { get; protected set; } = GhostState.Targeting;
    public Color SpriteColor { get; protected set; } = Color.white;
    public Color VulnerableColor { get; protected set; } = new Color32(140, 20, 252, 255);
    public bool AllowReversing { get; protected set; } = false;
    public bool Enabled { get; protected set; } = false;
    public Vector3Int SpawnPoint { get; private set; }
    public virtual Vector3 Target => Muncher.MainMuncher.transform.position;
    private bool vulnerableInternal = false;
    private bool vulnerableVisualInternal = false;
    public bool Vulnerable
    {
        get => vulnerableInternal;
        set
        {
            vulnerableInternal = value;
            VulnerableVisual = value;
        }
    }
    public bool VulnerableVisual
    {
        get => vulnerableVisualInternal;
        set
        {
            vulnerableVisualInternal = value;
            if (value)
            {
                renderer.color = VulnerableColor;
            }
            else
            {
                renderer.color = SpriteColor;
            }
            SetEyeDirection(PreviousDirection);
        }
    }

    private bool deadInternal = false;
    public bool Dead
    {
        get => deadInternal;
        set
        {
            deadInternal = value;
            if (value)
            {
                OnDead?.Invoke(this);
            }
            renderer.enabled = !value;
            Vulnerable = false;
        }
    }

    public event Action<Ghost> OnDead;

    protected Vector3Int PreviousPosition { get; private set; }
    protected Direction PreviousDirection { get; private set; } = None;
    protected Vector3Int NextPosition { get; private set; }
    protected virtual float Speed => 4f;
    protected virtual float VulnerableSpeed => Speed / 2f;
    protected virtual float DeadSpeed => Speed * 2f;
    private float CurrentSpeed => Dead ? DeadSpeed : Vulnerable ? VulnerableSpeed : Speed;
    private bool Moving = false;
    private float MoveCounter = 0f;

    private Animator eyeAnimator;
    private new SpriteRenderer renderer;

    private static bool AreOpposites(Direction A, Direction B)
    {
        return B == OppositeOf(A);
    }
    private static Direction OppositeOf(Direction A)
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
    private static Vector3Int DirToVector(Direction A)
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
    }


    protected virtual Direction PickDirection(Vector3Int PreviousPosition,Vector3Int CurrentPosition, Direction PreviousDirection)
    {
        if (CurrentState == GhostState.Random)
        {
            List<Direction> validDirections = new List<Direction>();
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
            if (validDirections.Count == 0 && !Level.Map.HasTile(CurrentPosition + DirToVector(OppositeOf(PreviousDirection))))
            {
                validDirections.Add(OppositeOf(PreviousDirection));
            }
            return RandomDirection(validDirections);
        }
        else
        {
            //ADD REVERSING CHECK
            var target = Dead ? SpawnPoint : Target;
            var UpTarget = Vector3.Distance(CurrentPosition + Vector3.up, target);
            var DownTarget = Vector3.Distance(CurrentPosition + Vector3.down, target);
            var LeftTarget = Vector3.Distance(CurrentPosition + Vector3.left, target);
            var RightTarget = Vector3.Distance(CurrentPosition + Vector3.right, target);
            (float distance, Direction direction) Distance = (Vulnerable && !Dead ? float.NegativeInfinity : float.PositiveInfinity, None);
            if (DistanceChecker(UpTarget, Distance.distance) && ((PreviousDirection != Down && !AllowReversing) || AllowReversing) && !Level.Map.HasTile(CurrentPosition + Vector3Int.up))
            {
                Distance = (UpTarget, Up);
            }
            if (DistanceChecker(DownTarget, Distance.distance) && ((PreviousDirection != Up && !AllowReversing) || AllowReversing) && !Level.Map.HasTile(CurrentPosition + Vector3Int.down))
            {
                Distance = (DownTarget, Down);
            }
            if (DistanceChecker(LeftTarget, Distance.distance) && ((PreviousDirection != Right && !AllowReversing) || AllowReversing) && !Level.Map.HasTile(CurrentPosition + Vector3Int.left))
            {
                Distance = (LeftTarget, Left);
            }
            if (DistanceChecker(RightTarget, Distance.distance) && ((PreviousDirection != Left && !AllowReversing) || AllowReversing) && !Level.Map.HasTile(CurrentPosition + Vector3Int.right))
            {
                Distance = (RightTarget, Right);
            }
            if (Distance.direction == None && !Level.Map.HasTile(CurrentPosition + DirToVector(OppositeOf(PreviousDirection))))
            {
                Distance = (Distance.distance, OppositeOf(PreviousDirection));
            }
            return Distance.direction;
        }
    }

    //Chooses the larger distance when vulnerable to move away from the muncher
    //Chooses the shorter distance when normal to move towards the muncher
    private bool DistanceChecker(float MuncherDistance, float CurrentBestDistance)
    {
        return Vulnerable && !Dead ? MuncherDistance > CurrentBestDistance : MuncherDistance < CurrentBestDistance;
        //return MuncherDistance < CurrentBestDistance;
    }

    protected virtual void OnLevelFinish()
    {
        SpawnedGhosts.Remove(this);
        GameManager.OnLevelEnd -= OnLevelFinish;
    }

    protected override void OnLevelReset()
    {
        MoveCounter = 0;
        Dead = false;
        transform.position = SpawnPoint + new Vector3(0.5f, 0.5f);
        PreviousPosition = SpawnPoint;
        var direction = PickDirection(PreviousPosition, PreviousPosition, None);
        var result = DirectionToVector(direction);
        if (result != Vector3Int.zero && !Level.Map.HasTile(PreviousPosition + result))
        {
            Moving = true;
            NextPosition = PreviousPosition + result;
            PreviousDirection = direction;
        }
        else
        {
            NextPosition = PreviousPosition;
        }
    }

    protected override void OnGameStart()
    {
        Enabled = true;
    }

    protected override void OnGamePause()
    {
       Enabled = false;
    }

    public virtual void OnGhostSpawn(Vector3Int spawnPoint)
    {
        StartEvents();
        SpawnPoint = spawnPoint;
        SpawnedGhosts.Add(this);
        GameManager.OnLevelEnd += OnLevelFinish;
        eyeAnimator = transform.GetChild(0).GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
        SpriteColor = renderer.color;
        OnLevelReset();
        StartCoroutine(GhostUpdate());
    }

    private void SetEyeDirection(Direction direction)
    {
        eyeAnimator.SetBool("Up", false);
        eyeAnimator.SetBool("Down", false);
        eyeAnimator.SetBool("Left", false);
        eyeAnimator.SetBool("Right", false);
        if (!Vulnerable)
        {
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
    public static void LoadInitialization()
    {
        GameManager.OnLevelEnd += () => SpawnedGhosts.Clear();
    }

    private static Vector3Int DirectionToVector(Direction direction)
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
    }

    private static Direction RandomDirection(List<Direction> directions)
    {
        return directions[UnityEngine.Random.Range(0,directions.Count)];
    }

    private IEnumerator GhostUpdate()
    {
        while (true)
        {
            if (Enabled)
            {
                if (Moving == false)
                {
                    if (Dead && NextPosition == SpawnPoint)
                    {
                        Dead = false;
                    }
                    //Debug.Log("Move = " + Test);
                    var direction = PickDirection(PreviousPosition, NextPosition, PreviousDirection);
                    var result = DirectionToVector(direction);
                    if (result != Vector3Int.zero && !Level.Map.HasTile(NextPosition + result))
                    {
                        Moving = true;
                        MoveCounter = 0f;
                        PreviousPosition = NextPosition;
                        NextPosition = NextPosition + result;
                        PreviousDirection = direction;
                        SetEyeDirection(direction);
                    }
                }
                else
                {
                    MoveCounter += Time.deltaTime * CurrentSpeed;
                    if (MoveCounter > 1)
                    {
                        MoveCounter = 1;
                    }
                    var CurrentPosition = Vector3.Lerp(PreviousPosition, NextPosition, MoveCounter);
                    transform.position = CurrentPosition + new Vector3(0.5f, 0.5f);
                    if (MoveCounter == 1)
                    {
                        var teleporter = Teleporter.GetTeleporter(NextPosition);
                        if (teleporter != null)
                        {
                            var telePosition = teleporter.LinkedTeleporter.Position;
                            PreviousPosition = telePosition;
                            NextPosition = telePosition;
                            transform.position = NextPosition + new Vector3(0.5f, 0.5f);
                        }
                        Moving = false;
                        continue;
                    }
                }
            }
            yield return null;
        }
    }

    //Used in case if the ghost gets stuck when trying to move back to the spawnpoint
    /*IEnumerator UnStucker()
    {
        float Multiplier = 1f;
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(3f);
            AllowReversing = false;
            yield return new WaitForSeconds(1.5f * Multiplier);
            AllowReversing = true;
            Multiplier += 1.3f;
        }
        //Teleport back to the spawn
        Moving = false;
        transform.position = SpawnPoint + new Vector3(0.5f, 0.5f);
        PreviousPosition = SpawnPoint;
        var direction = PickDirection(PreviousPosition, PreviousPosition, None);
        var result = DirectionToVector(direction);
        if (result != Vector3Int.zero && !Level.Map.HasTile(PreviousPosition + result))
        {
            NextPosition = PreviousPosition + result;
            PreviousDirection = direction;
        }
        else
        {
            NextPosition = PreviousPosition;
        }
    }*/

    public void OnEat(Muncher muncher)
    {
        if (Dead)
        {
            return;
        }
        if (Vulnerable)
        {
            ScoreCounter.Score += 10;
            Dead = true;
        }
        else
        {
            GameManager.CurrentGameState = GameState.Lose;
        }
    }
}
