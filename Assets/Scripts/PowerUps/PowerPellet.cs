using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPellet : PowerUp
{
    [SerializeField] float FlashRate = 5f; //How many times the powerup will flash per second
    [SerializeField] float GhostFlickerRate = 5f; //How fast the ghosts flicker

    new SpriteRenderer renderer;
    Coroutine ClockRoutine;
    Coroutine FlickerRoutine;

    public override void Start()
    {
        base.Start();
        renderer = GetComponent<SpriteRenderer>();
        ClockRoutine = StartCoroutine(Clock());
    }

    protected override void OnPowerUpActivate(Muncher muncher)
    {
        Ghost.AllVulnerable = true;
        StopCoroutine(ClockRoutine);
        base.OnPowerUpActivate(muncher);
    }

    protected override void OnPowerUpInterrupt()
    {
        if (FlickerRoutine != null)
        {
            StopCoroutine(FlickerRoutine);
            Ghost.AllVulnerableVisual = false;
        }
        Ghost.AllVulnerable = false;
        base.OnPowerUpInterrupt();
    }

    protected override void OnPowerUpWarning()
    {
        FlickerRoutine = StartCoroutine(Flicker());
        base.OnPowerUpWarning();
    }

    protected override void OnPowerUpDeactivate()
    {
        StopCoroutine(FlickerRoutine);
        Ghost.AllVulnerable = false;
        base.OnPowerUpDeactivate();
    }

    IEnumerator Clock()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / FlashRate);
            renderer.enabled = false;
            yield return new WaitForSeconds(1 / FlashRate);
            renderer.enabled = true;
        }
    }

    IEnumerator Flicker()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f / GhostFlickerRate);
            Ghost.AllVulnerableVisual = false;
            yield return new WaitForSeconds(1f / GhostFlickerRate);
            Ghost.AllVulnerableVisual = true;
        }
    }
}
