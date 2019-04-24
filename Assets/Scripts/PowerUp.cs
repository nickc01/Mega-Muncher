using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : Pellet ,IEatable
{
    Muncher muncher;

    [SerializeField] protected float WaitTime = 10f;
    [SerializeField] protected float WarningTime = 7f;
    protected virtual bool OneTypeAtATime => true; //if true, will interrupt any other currently running powerups of the same type
    protected virtual bool OneAnyAtATime => true; //If true, will intterupt all other powerups currently running
    protected sealed override bool DestroyOnEat => false; //Used to not immediately destroy the powerup when eaten

    private static Dictionary<Type, PowerUp> Dominants = new Dictionary<Type, PowerUp>();
    private static Dictionary<Type, Action> TypeBasedInterupts = new Dictionary<Type, Action>(); //Stores all the interupt functions for powerups if similar types

    public bool IsDominant => Dominants.ContainsKey(GetType()) && Dominants[GetType()] == this;

    private Coroutine WaitCoroutine;
    private Type thisType;

    [RuntimeInitializeOnLoadMethod]
    public static void LoadInit()
    {
        GameManager.OnLevelEnd += () =>
        {
            Dominants.Clear();
            TypeBasedInterupts.Clear();
        };
    }

    public override void OnEat(Muncher muncher)
    {
        AudioSource.PlayClipAtPoint(GameManager.Game.PowerUpSound, CameraManager.Main.transform.position,1f);
        GameManager.OnGamePause += OnPowerUpInterrupt;
        this.muncher = muncher;
        thisType = GetType();
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
        OnPowerUpActivate(muncher);
        WaitCoroutine = StartCoroutine(Waiter());
        base.OnEat(muncher);
    }

    public static bool IsRunning<T>() where T : PowerUp
    {
        if (TypeBasedInterupts.ContainsKey(typeof(T)))
        {
            return TypeBasedInterupts[typeof(T)] != null;
        }

        return false;
    }

    public static bool IsAnyRunning()
    {
        return TypeBasedInterupts.Count > 0;
    }

    public static void InterruptAll()
    {
        foreach (var pair in TypeBasedInterupts)
        {
            pair.Value?.Invoke();
        }
    }

    public static void InterruptAllOfType<T>() where T : PowerUp
    {
        if (TypeBasedInterupts.ContainsKey(typeof(T)))
        {
            TypeBasedInterupts[typeof(T)]?.Invoke();
        }
    }

    IEnumerator Waiter()
    {
        yield return new WaitForSeconds(WarningTime);
        OnPowerUpWarning();
        yield return new WaitForSeconds(WaitTime - WarningTime);
        OnPowerUpDeactivate();
    }

    //Usually called when the game ends while the powerup is still running
    //If an interrupt is called, the warning and deactivate functions will not call if not called already
    protected virtual void OnPowerUpInterrupt()
    {
        StopCoroutine(WaitCoroutine);
        GameManager.OnGamePause -= OnPowerUpInterrupt;
        TypeBasedInterupts[thisType] -= OnPowerUpInterrupt;
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
        GameManager.OnGamePause -= OnPowerUpInterrupt;
        TypeBasedInterupts[thisType] -= OnPowerUpInterrupt;
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
