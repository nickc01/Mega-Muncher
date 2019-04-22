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
    List<Ghost> VulnerableGhosts;

    public override void Start()
    {
        base.Start();
        renderer = GetComponent<SpriteRenderer>();
        ClockRoutine = StartCoroutine(Clock());
    }


    private void OnGhostDie(Ghost ghost)
    {
        VulnerableGhosts.Remove(ghost);
        ghost.Vulnerable = false;
    }

    protected override void OnPowerUpActivate(Muncher muncher)
    {
        VulnerableGhosts = new List<Ghost>();
        foreach (var ghost in Ghost.Ghosts)
        {
            VulnerableGhosts.Add(ghost);
            ghost.Vulnerable = true;
            ghost.OnDead += OnGhostDie;
        }
        StopCoroutine(ClockRoutine);
        base.OnPowerUpActivate(muncher);
    }

    protected override void OnPowerUpInterrupt()
    {
        if (FlickerRoutine != null)
        {
            StopCoroutine(FlickerRoutine);
        }
        foreach (var ghost in VulnerableGhosts)
        {
            ghost.Vulnerable = false;
            ghost.OnDead -= OnGhostDie;
        }
        /*VulnerableGhosts.ForEach(ghost => {
            ghost.Vulnerable = false;
        });*/
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
        VulnerableGhosts.ForEach(ghost => ghost.Vulnerable = false);
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
            VulnerableGhosts.ForEach(ghost => ghost.VulnerableVisual = false);
            yield return new WaitForSeconds(1f / GhostFlickerRate);
            VulnerableGhosts.ForEach(ghost => ghost.VulnerableVisual = true);
        }
    }
}
