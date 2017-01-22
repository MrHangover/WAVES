using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSoundPlayer : MonoBehaviour
{
    public AudioClip[] sounds;

    public float minTime = 1f;
    public float maxTime = 10f;

    float timer = 3f;
    AudioSource audio;


    void Start()
    {
        audio = gameObject.GetComponent<AudioSource>();
    }
	
	void Update ()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            audio.clip = sounds[Random.Range(0, sounds.Length)];
            audio.Play();
            timer = Random.Range(minTime, maxTime);
            
            //Debug.Log("win");
        }
		
	}
}
