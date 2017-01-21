using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningScript : MonoBehaviour {

    public float minPitch;

    public float maxPitch;

    public GameObject Minotaur;

    [Range(20f,700f)]
    public float pitchValue = 20;

    public float timer = 5f;

    public float minPos = -7;
    public float maxPos = 7;




    // Use this for initialization
    void Start () {


    }
	
	// Update is called once per frame
	void Update () {

        float posDif = maxPos - minPos;

        float pitchDif = 680;

        float temp = 5f;
        if (WaveManager.instance != null)
        {
           pitchValue = FrequencyAnalysis.instance.frequencyAndAmp[0].Key;
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
            gameObject.transform.position = new Vector2((pitchValue * posDif / pitchDif) * 2 + minPos, transform.position.y);
        }

        else
        {
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down);
            if (hit.collider.tag == "Minotaur")
            {
                Debug.Log("test");
                Destroy(Minotaur);
            }
            timer = temp;
        }
 
    }
}
