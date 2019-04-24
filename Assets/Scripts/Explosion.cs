using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float AnimationSpeed = 0.5f; //How long the explosion animation will play
    public List<Sprite> explosionSprites; //The sprites used to animate the explosion
    public List<AudioClip> explosionSounds; //A list of sounds the object will randomly use

    private float Clock = 0f; //A clock to keep track of the animation
    private int currentIndex = 0; //The current sprite index to use for the sprite renderer
    private float frameTime; //How long a frame will be displayed
    private new SpriteRenderer renderer; //The spriteRenderer on the current object
    private new AudioSource audio; //The Audio Source on the current object
    private bool update = true; //Determines whether to run update or not

    private void Start()
    {
        //Get the Sprite Renderer
        renderer = GetComponent<SpriteRenderer>();
        //Get the audio source
        audio = GetComponent<AudioSource>();
        //Play a random explosion sound from the sounds list
        audio.clip = explosionSounds[Random.Range(0, explosionSounds.Count)];
        audio.Play();
        //Set the time for each frame
        frameTime = AnimationSpeed / explosionSprites.Count;
        //Set the spriteRenderer to the current frame
        renderer.sprite = explosionSprites[currentIndex];
    }

    private void Update()
    {
        //If the update variable is true
        if (update)
        {
            //Increment the clock
            Clock += Time.deltaTime;
            //If the clock is greater than the frame time
            if (Clock > frameTime)
            {
                //Reset the clock
                Clock = 0f;
                //Increment the index
                currentIndex++;
                //If the current index is greater than or equal to the list count
                if (currentIndex >= explosionSprites.Count)
                {
                    //Set update to false
                    update = false;
                    //Run the Finish function
                    _ = Finish();
                }
                else
                {
                    //Set the renderer sprite to the next index
                    renderer.sprite = explosionSprites[currentIndex];
                }
            }
        }
    }

    //Waits until the audio source is done playing before deleting the object
    private async Task Finish()
    {
        //Hide the sprite renderer
        renderer.enabled = false;
        //If the audio source is still playing, wait untill it is done
        while (audio.isPlaying)
        {
            await Wait(50);
        }
        //Destroy the explosion sprite
        Destroy(gameObject);
    }

    //Waits a set amount of time
    private async Task Wait(int milliseconds)
    {
        await Task.Run(() => Thread.Sleep(milliseconds));
    }

    //Plays an explosion at the set position
    public static void PlayExplosion(Vector3 position)
    {
        GameObject.Instantiate(GameManager.Game.ExplosionPrefab, position, Quaternion.identity);
    }
}
