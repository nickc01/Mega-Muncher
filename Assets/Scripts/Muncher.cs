using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using static Direction;

public enum Direction
{
    Up,
    Down,
    Left,
    Right,
    None
}

public class Muncher : GameEventHandler
{
    public static Muncher MainMuncher { get; private set; } //the main muncher in the level
    public static Direction MuncherWantingDirection => MainMuncher.wantingDirection; //The public interface for accessing the direction the muncher WANTS to travel in
    public float Speed => MovementSpeed; //The public interface for the muncher's speed
    [SerializeField] float MovementSpeed = 3f; //The movement speed of the muncher
    [Range(0f,0.5f)]
    [SerializeField] float MovementFlexibility = 0.15f; //How easily the muncher is able to turn around corners.
    [SerializeField] Direction wantingDirection = Right; //The current direction the muncher WANTS to travel in
    Direction directionInternal = Right; //The direction the muncher IS traveling in
    Direction Direction //The public interface for the direction the muncher IS traveling in
    {
        get => directionInternal;
        set
        {
            directionInternal = value;
            //Update the rotation of the muncher based on the new value
            UpdateRotation(value);
        }
    }

    [SerializeField] int lives = 3; //The starting amount of lives for the muncher

    public static int Lives //The public interface for accessing the lives
    {
        get => MainMuncher.lives;
        set
        {
            //Update the lives couunter as well
            LivesCounter.Lives = value;
            MainMuncher.lives = value;
        }
    }


    Vector3 LastPosition; //The last position the player was at
    Vector3 CurrentPosition; //The current position the muncher is at now
    Vector3? NextPosition; //The next position the player is traveling towards
    Animator animator; //The animator component for the muncher
    new SpriteRenderer renderer; //The sprite renderer component of the muncher
    float movementCounter = 0; //The movement counter for interpolating between the last position and the next position

    //When the level starts
    protected override void OnGameStart()
    {
        //Enable the player
        enabled = true;
        //Enable the player's animator
        animator.enabled = true;
    }

    //When the game is paused
    protected override void OnGamePause()
    {
        //Disable the player
        enabled = false;
        //Disable the player's animator
        animator.enabled = false;
    }

    //When the player looses
    protected override void OnLose()
    {
        //Hide the player
        renderer.enabled = false;
        //Play an explosion effect
        Explosion.PlayExplosion(transform.position);
    }

    //Called when the game wants to reset the objects to their starting positions
    protected override void OnLevelReset()
    {
        //Show the player
        renderer.enabled = true;
        //Set the camera to be at the muncher
        CameraManager.SetTargetForceful(gameObject);
        //Update the last, current, and next positions
        LastPosition = Level.SpawnPoint;
        CurrentPosition = LastPosition;
        NextPosition = LastPosition + wantingDirection.DirToVector();
        //If the next position is invalid
        if (Level.Map.HasTile(NextPosition.Value.ToInt()))
        {
            //Set it to null
            NextPosition = null;
        }
        //Set the camera's target to be the player
        CameraManager.Target = gameObject;
        Direction = wantingDirection;
        //Update the player's actual position
        transform.position = Level.SpawnPoint + new Vector3(0.5f, 0.5f);
    }

    public void OnMuncherSpawn()
    {
        //Start the Game Event Handler
        StartEvents();
        //Get the muncher's sprite renderer
        renderer = GetComponent<SpriteRenderer>();
        //Get the player's animator controller
        animator = GetComponent<Animator>();
        //Disable the animation
        animator.enabled = false;
        //Disable the muncher
        enabled = false;
        //Set the main muncher to be this
        MainMuncher = this;
        //Set the lives counter
        LivesCounter.Lives = Lives;
        //Reset the muncher to it's starting position
        OnLevelReset();
    }

    private void Update()
    {
        //If there is a next position to move to
        if (NextPosition != null)
        {
            //Increase the counter
            movementCounter += Time.deltaTime * MovementSpeed;
            //Clamp the counter to be under 1
            if (movementCounter > 1)
            {
                movementCounter = 1;
            }
            //Update the current position by linearly interpolating from the last position to the next position
            CurrentPosition = Vector3.Lerp(LastPosition, NextPosition.Value, movementCounter);
            //If the movement counter == 1
            //OR the muncher is close enough to the destination to turn and the player is not making a 180 degree turn
            if (movementCounter == 1 || (movementCounter >= 1f - MovementFlexibility && Direction != wantingDirection && !wantingDirection.AreOpposites(Direction)))
            {
                //Rest the movement counter
                movementCounter = 0;
                //Update the Current, Last, and next position
                CurrentPosition = NextPosition.Value;
                LastPosition = NextPosition.Value;
                NextPosition = LastPosition + wantingDirection.DirToVector();
                //If there is a teleporter here
                var teleporter = Teleporter.GetTeleporter(CurrentPosition.ToInt());
                if (teleporter != null)
                {
                    //Teleport!!!
                    var telePosition = teleporter.LinkedTeleporter.Position;
                    //Update the last, current, next, and actual position accordingly
                    LastPosition = telePosition;
                    CurrentPosition = telePosition;
                    NextPosition = telePosition + wantingDirection.DirToVector();
                    transform.position = NextPosition.Value + new Vector3(0.5f, 0.5f);
                }

                //If the next position is invalid
                if (Level.Map.HasTile(NextPosition.Value.ToInt()))
                {
                    //Use the previous direction the player was traveling, instead of the direction the player wants to go in
                    NextPosition = LastPosition + Direction.DirToVector();
                    //If it is still invalid
                    if (Level.Map.HasTile(NextPosition.Value.ToInt()))
                    {
                        //Set the next position to null
                        NextPosition = null;
                    }
                }
                //If it is valid
                else
                {
                    //Then set the direction to the direction the player wants to move in
                    Direction = wantingDirection;
                }
            }
            else
            {
                //If the player is making a 180 degree turn
                if (wantingDirection.AreOpposites(Direction))
                {
                    //Reverse the direction
                    Direction = wantingDirection;
                    //Swap the last and next position and invert the movement counter
                    var cache = LastPosition;
                    LastPosition = NextPosition.Value;
                    NextPosition = cache;
                    movementCounter = 1 - movementCounter;
                }
            }
        }
        //If there is no direction to move in right now
        else
        {
            movementCounter = 0;
            //Get the next position based on the direction the player wants to go in
            NextPosition = LastPosition + wantingDirection.DirToVector();
            //If the next position is invalid
            if (Level.Map.HasTile(NextPosition.Value.ToInt()))
            {
                //Set the next position to null
                NextPosition = null;
            }
            Direction = wantingDirection;
        }
        //Update the player's actual position
        transform.position = CurrentPosition + new Vector3(0.5f, 0.5f);
        //Change the direction the player wants to go based on the arrow key inputs
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            wantingDirection = Left;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            wantingDirection = Right;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            wantingDirection = Up;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            wantingDirection = Down;
        }
    }

    //Updates the player's actual rotatation based on the direction
    private void UpdateRotation(Direction direction)
    {
        switch (direction)
        {
            case Up:
                transform.rotation = Quaternion.Euler(0, 0, 90f);
                break;
            case Down:
                transform.rotation = Quaternion.Euler(0, 0, -90f);
                break;
            case Left:
                transform.rotation = Quaternion.Euler(0, 0, 180f);
                break;
            case Right:
                transform.rotation = Quaternion.identity;
                break;
            default:
                break;
        }
    }

    //When the player collides with something
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //If the collided object is eatable
        var eatable = collision.GetComponent<IEatable>();
        if (eatable != null)
        {
            //Eat it
            eatable.OnEat(this);
        }
    }
}
