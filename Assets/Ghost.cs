using Extensions;
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

public abstract class Ghost : MonoBehaviour, IEatable
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
    public Color VulnerableColor { get; protected set; } = new Color(140, 20, 252, 255);
    public virtual Vector3 Target => Muncher.MainMuncher.transform.position;
    private bool vulnerableInternal = false;
    private bool vulnerableVisualInternal = false;
    public bool Vulnerable
    {
        get => vulnerableInternal;
        set
        {
            Debug.Log("VULERABILITY = " + value);
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
            if (value == true)
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

    protected Vector3Int PreviousPosition { get; private set; }
    protected Direction PreviousDirection { get; private set; } = None;
    protected Vector3Int NextPosition { get; private set; }
    protected virtual float Speed => 4f;
    protected virtual float VulnerableSpeed => Speed / 2f;
    private float CurrentSpeed => Vulnerable ? VulnerableSpeed : Speed;
    private bool Moving = false;
    private float MoveCounter = 0f;

    private Animator eyeAnimator;
    private new SpriteRenderer renderer;


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
            return RandomDirection(validDirections);
        }
        else
        {
            var UpTarget = Vector3.Distance(CurrentPosition + Vector3.up, Target);
            var DownTarget = Vector3.Distance(CurrentPosition + Vector3.down, Target);
            var LeftTarget = Vector3.Distance(CurrentPosition + Vector3.left, Target);
            var RightTarget = Vector3.Distance(CurrentPosition + Vector3.right, Target);
            (float distance, Direction direction) Distance = (Vulnerable ? float.NegativeInfinity : float.PositiveInfinity, None);
            if (DistanceChecker(UpTarget, Distance.distance) && PreviousDirection != Down && !Level.Map.HasTile(CurrentPosition + Vector3Int.up))
            {
                Distance = (UpTarget, Up);
            }
            if (DistanceChecker(DownTarget, Distance.distance) && PreviousDirection != Up && !Level.Map.HasTile(CurrentPosition + Vector3Int.down))
            {
                Distance = (DownTarget, Down);
            }
            if (DistanceChecker(LeftTarget, Distance.distance) && PreviousDirection != Right && !Level.Map.HasTile(CurrentPosition + Vector3Int.left))
            {
                Distance = (LeftTarget, Left);
            }
            if (DistanceChecker(RightTarget, Distance.distance) && PreviousDirection != Left && !Level.Map.HasTile(CurrentPosition + Vector3Int.right))
            {
                Distance = (RightTarget, Right);
            }
            return Distance.direction;
        }
    }

    //Chooses the larger distance when vulnerable to move away from the muncher
    //Chooses the shorter distance when normal to move towards the muncher
    private bool DistanceChecker(float MuncherDistance, float CurrentBestDistance)
    {
        return Vulnerable ? MuncherDistance > CurrentBestDistance : MuncherDistance < CurrentBestDistance;
        //return MuncherDistance < CurrentBestDistance;
    }

    protected virtual void OnLevelFinish()
    {
        SpawnedGhosts.Remove(this);
        GameManager.OnLevelEnd -= OnLevelFinish;
    }

    public virtual void OnGhostSpawn(Vector3Int SpawnPoint)
    {
        SpawnedGhosts.Add(this);
        GameManager.OnLevelEnd += OnLevelFinish;
        eyeAnimator = transform.GetChild(0).GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
        SpriteColor = renderer.color;
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
        return directions[Random.Range(0,directions.Count)];
    }

    private IEnumerator GhostUpdate()
    {
        while (true)
        {
            if (Moving == false)
            {
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
                    Moving = false;
                    continue;
                }
            }
            yield return null;
        }
    }

    public void OnEat(Muncher muncher)
    {
        
    }
}
