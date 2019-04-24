using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPellet : PowerUp
{
    [SerializeField] float FlashRate = 5f; //How many times the powerup will flash per second
    [SerializeField] float GhostFlickerRate = 5f; //How fast the ghosts flicker

    new SpriteRenderer renderer; //The sprite renderer of the Power Pellet
    Coroutine ClockRoutine; //The clock routine for flickering the powerup
    Coroutine FlickerRoutine; //The flicker routine for the ghosts
    List<Ghost> VulnerableGhosts; //A list of vulnerable ghosts that the power-up affects

    //When the power-up is placed in the level
    public override void Start()
    {
        //Run the base method
        base.Start();
        //Get the sprite renderer
        renderer = GetComponent<SpriteRenderer>();
        //Start the clock routine
        ClockRoutine = StartCoroutine(Clock());
    }

    //When a ghost dies
    private void OnGhostDie(Ghost ghost)
    {
        //Remove it from the list of vulnerable ghosts
        VulnerableGhosts.Remove(ghost);
        //Set the ghost to no longer be vulnerable
        ghost.Vulnerable = false;
    }

    //When the powerup is activated
    protected override void OnPowerUpActivate(Muncher muncher)
    {
        //Clear the vulnerable list and add all the ghosts that are not dead to the list
        VulnerableGhosts = new List<Ghost>();
        foreach (var ghost in Ghost.Ghosts)
        {
            if (!ghost.Dead)
            {
                VulnerableGhosts.Add(ghost);
                ghost.Vulnerable = true;
                ghost.OnDead += OnGhostDie;
            }
        }
        //Stop the clock routine
        StopCoroutine(ClockRoutine);
        //Run the base method
        base.OnPowerUpActivate(muncher);
    }

    //If the powerup is interrupted by another powerup collected by the player
    protected override void OnPowerUpInterrupt()
    {
        //Stop the flicker routine
        if (FlickerRoutine != null)
        {
            StopCoroutine(FlickerRoutine);
        }
        //Set all the ghosts to no longer be vulnerable
        foreach (var ghost in VulnerableGhosts)
        {
            ghost.Vulnerable = false;
            ghost.OnDead -= OnGhostDie;
        }
        //Run the base method
        base.OnPowerUpInterrupt();
    }

    //When the powerup is about to run out
    protected override void OnPowerUpWarning()
    {
        //Start the flicker routine for flickering the ghosts
        FlickerRoutine = StartCoroutine(Flicker());
        //Run the base method
        base.OnPowerUpWarning();
    }

    //When the power up has run out
    protected override void OnPowerUpDeactivate()
    {
        //Stop the ghost flicker routine
        StopCoroutine(FlickerRoutine);
        //Set all the ghosts to no longer be vulnerable
        VulnerableGhosts.ForEach(ghost => ghost.Vulnerable = false);
        //Run the base method
        base.OnPowerUpDeactivate();
    }

    //The clock routine for flickering the powerup
    IEnumerator Clock()
    {
        while (true)
        {
            //Wait a little bit
            yield return new WaitForSeconds(1 / FlashRate);
            //Show the sprite
            renderer.enabled = false;
            //Wait a little bit
            yield return new WaitForSeconds(1 / FlashRate);
            //Hide the sprite
            renderer.enabled = true;
        }
    }
    
    //The flicker routine for flickering the ghosts
    IEnumerator Flicker()
    {
        while (true)
        {
            //Wait a little bit
            yield return new WaitForSeconds(1f / GhostFlickerRate);
            //Make the ghosts appear non-vulnerable
            VulnerableGhosts.ForEach(ghost => ghost.VulnerableVisual = false);
            //Wait a little bit
            yield return new WaitForSeconds(1f / GhostFlickerRate);
            //Make the ghosts appear vulnerable
            VulnerableGhosts.ForEach(ghost => ghost.VulnerableVisual = true);
        }
    }
}
