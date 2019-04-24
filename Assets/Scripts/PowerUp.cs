using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : Pellet ,IEatable
{

    [SerializeField] protected float WaitTime = 10f; //How long the powerup will last
    [SerializeField] protected float WarningTime = 7f; //How long the powerup will run before the warning is triggered
    protected virtual bool OneTypeAtATime => true; //if true, will interrupt any other currently running powerups of the same type
    protected virtual bool OneAnyAtATime => true; //If true, will intterupt all other powerups currently running
    protected sealed override bool DestroyOnEat => false; //Used to not immediately destroy the powerup when eaten

    private static Dictionary<Type, PowerUp> Dominants = new Dictionary<Type, PowerUp>(); //Stores the dominant powerup for specific types
    private static Dictionary<Type, Action> TypeBasedInterupts = new Dictionary<Type, Action>(); //Stores all the interupt functions for powerups if similar types

    public bool IsDominant => Dominants.ContainsKey(GetType()) && Dominants[GetType()] == this; //Determines whether the powerup is the dominant one

    private Coroutine WaitCoroutine; //Used to wait for the warning and end triggers
    private Type thisType; //The type of this powerup

    [RuntimeInitializeOnLoadMethod]
    public static void LoadInit()
    {
        //Clear the dominant lists when the level unloads
        GameManager.OnLevelUnload += () =>
        {
            Dominants.Clear();
            TypeBasedInterupts.Clear();
        };
    }

    //When the powerup is eaten
    public override void OnEat(Muncher muncher)
    {
        //Play the powerup sound
        AudioSource.PlayClipAtPoint(GameManager.Game.PowerUpSound, CameraManager.Main.transform.position,1f);
        //Add an event that interupts the powerup when the game pauses
        GameManager.OnGamePause += OnPowerUpInterrupt;
        //Get the type of this powerup
        thisType = GetType();
        //Setup the dominants so that only one powerup can run at a time
        if (!Dominants.ContainsKey(thisType))
        {
            Dominants.Add(thisType, this);
        }
        else
        {
            if (OneTypeAtATime)
            {
                Dominants[thisType]?.OnPowerUpInterrupt();
            }
            Dominants[thisType] = this;
        }
        if (OneAnyAtATime)
        {
            foreach (var pair in TypeBasedInterupts)
            {
                pair.Value?.Invoke();
            }
        }
        if (!TypeBasedInterupts.ContainsKey(thisType))
        {
            TypeBasedInterupts.Add(thisType, null);
        }
        TypeBasedInterupts[thisType] += OnPowerUpInterrupt;
        //Activates the powerup
        OnPowerUpActivate(muncher);
        //Starts the waiting coroutine
        WaitCoroutine = StartCoroutine(Waiter());
        //Run the base function
        base.OnEat(muncher);
    }

    //Determines if the powerup type is already running
    public static bool IsRunning<T>() where T : PowerUp
    {
        if (TypeBasedInterupts.ContainsKey(typeof(T)))
        {
            return TypeBasedInterupts[typeof(T)] != null;
        }

        return false;
    }

    //Determines if any powerup is running at the moment
    public static bool IsAnyRunning()
    {
        return TypeBasedInterupts.Count > 0;
    }

    //Interrupts all running powerups
    public static void InterruptAll()
    {
        foreach (var pair in TypeBasedInterupts)
        {
            pair.Value?.Invoke();
        }
    }

    //Interrupts all powerups of a certain type
    public static void InterruptAllOfType<T>() where T : PowerUp
    {
        if (TypeBasedInterupts.ContainsKey(typeof(T)))
        {
            TypeBasedInterupts[typeof(T)]?.Invoke();
        }
    }

    //Waits to trigger the warning and end of the powerup
    IEnumerator Waiter()
    {
        yield return new WaitForSeconds(WarningTime);
        //Trigger the warning
        OnPowerUpWarning();
        yield return new WaitForSeconds(WaitTime - WarningTime);
        //Deactivate the powerup
        OnPowerUpDeactivate();
    }

    //Usually called when the game ends while the powerup is still running
    //If an interrupt is called, the warning and deactivate functions will not call if not called already
    protected virtual void OnPowerUpInterrupt()
    {
        //Stop the wait coroutine
        StopCoroutine(WaitCoroutine);
        //Remove the events
        GameManager.OnGamePause -= OnPowerUpInterrupt;
        TypeBasedInterupts[thisType] -= OnPowerUpInterrupt;
        //Update the dominants
        if (TypeBasedInterupts[thisType] == null)
        {
            TypeBasedInterupts.Remove(thisType);
        }
        Destroy(gameObject);
        if (Dominants[thisType] == this)
        {
            Dominants.Remove(thisType);
        }
    }

    //Called when the muncher eats the powerup
    //The base behaviour will disable it's spriteRender and 2D Collider
    //The return value is the time before it's deactivated and the time before a warning will flash to indicate the power up is almost done
    //The base behaviour will return 10 seconds and 7 seconds respectively
    protected virtual void OnPowerUpActivate(Muncher muncher)
    {
        //Hides the powerup from the level
        GetComponent<Collider2D>().enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }

    //Called when the power-up is warning the player that the power up is about to run out
    //Base does nothing by default
    protected virtual void OnPowerUpWarning()
    {
        
    }

    //Called when the power-up is deactivated
    //Destroys the power-up by default and removes it from the pellet list
    protected virtual void OnPowerUpDeactivate()
    {
        //Remove the events
        GameManager.OnGamePause -= OnPowerUpInterrupt;
        TypeBasedInterupts[thisType] -= OnPowerUpInterrupt;
        //Update the dominants
        if (TypeBasedInterupts[thisType] == null)
        {
            TypeBasedInterupts.Remove(thisType);
        }
        Destroy(gameObject);
        if (Dominants[thisType] == this)
        {
            Dominants.Remove(thisType);
        }
    }
}
