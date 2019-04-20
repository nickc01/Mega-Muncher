﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using static Direction;

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public class Muncher : MonoBehaviour
{
    [SerializeField] float MovementSpeed = 3f;
    [Range(0f,0.5f)]
    [SerializeField] float MovementFlexibility = 0.15f;
    [SerializeField] Direction direction = Right;
    Direction prevDirectionInternal = Right;
    Direction previousDirection
    {
        get => prevDirectionInternal;
        set
        {
            prevDirectionInternal = value;
            UpdateRotation(value);
        }
    }


    Vector3 LastTilePosition;
    Vector3 CurrentPosition;
    Vector3? NextTilePosition;
    float movementCounter = 0;

    private void Start()
    {
        LastTilePosition = Level.SpawnPoint;
        CurrentPosition = LastTilePosition;
        Debug.Log("Spawn Position = " + LastTilePosition);
        NextTilePosition = LastTilePosition + DirToVector(direction);
        if (Level.Map.HasTile(NextTilePosition.Value.ToInt()))
        {
            NextTilePosition = null;
        }
        previousDirection = direction;
    }

    private void Update()
    {
        if (NextTilePosition != null)
        {
            movementCounter += Time.deltaTime * MovementSpeed;
            if (movementCounter > 1)
            {
                movementCounter = 1;
            }
            CurrentPosition = Vector3.Lerp(LastTilePosition, NextTilePosition.Value, movementCounter);
            if (movementCounter == 1 || (movementCounter >= 1f - MovementFlexibility && previousDirection != direction && !AreOpposites(previousDirection,direction)))
            {
                CurrentPosition = NextTilePosition.Value;
                movementCounter = 0;
                LastTilePosition = NextTilePosition.Value;
                NextTilePosition = LastTilePosition + DirToVector(direction);
                if (Level.Map.HasTile(NextTilePosition.Value.ToInt()))
                {
                    NextTilePosition = LastTilePosition + DirToVector(previousDirection);
                    if (Level.Map.HasTile(NextTilePosition.Value.ToInt()))
                    {
                        NextTilePosition = null;
                    }
                }
                else
                {
                    previousDirection = direction;
                }
            }
            else
            {
                if (AreOpposites(direction,previousDirection))
                {
                    previousDirection = direction;
                    var cache = LastTilePosition;
                    LastTilePosition = NextTilePosition.Value;
                    NextTilePosition = cache;
                    movementCounter = 1 - movementCounter;
                }
            }
        }
        else
        {
            movementCounter = 0;
            NextTilePosition = LastTilePosition + DirToVector(direction);
            if (Level.Map.HasTile(NextTilePosition.Value.ToInt()))
            {
                NextTilePosition = null;
            }
            previousDirection = direction;
        }

        //Debug.Log("Current Position = " + CurrentPosition);
        transform.position = CurrentPosition + new Vector3(0.5f, 0.5f);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = Left;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = Right;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = Up;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = Down;
        }
    }

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

    private static Vector3Int DirToVector(Direction direction)
    {
        switch (direction)
        {
            case Up:
                return new Vector3Int(0, 1, 0);
            case Down:
                return new Vector3Int(0, -1, 0);
            case Left:
                return new Vector3Int(-1, 0, 0);
            case Right:
                return new Vector3Int(1, 0, 0);
        }
        return default;
    }

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var eatable = collision.GetComponent<IEatable>();
        if (eatable != null)
        {
            eatable.OnEat(this);
        }
    }
}