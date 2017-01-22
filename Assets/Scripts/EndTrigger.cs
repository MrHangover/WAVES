using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTrigger : MonoBehaviour {

    public GameObject LightningBolt;
	public Transform lightningSpot;
	GameObject lightInst;

    public float animationDuration = 2;

    const string playerTag = "Player";

    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.tag != playerTag)
            return;

		print ("HELLO!");
		lightInst = (GameObject)Instantiate(LightningBolt,lightningSpot.position,lightningSpot.rotation);
		//lightInst.transform.SetParent (lightningSpot.parent);
		lightInst.transform.Rotate (new Vector3 (0, 0, 90f));

		lightInst.GetComponent<LightningScript>().SetValues(lightningSpot.position , Quaternion.Euler(0,0,70), animationDuration);
		//WaveManager.instance.isMakingWaves = false;
		//WaveManager.instance.isScrolling = false;
		//Destroy (gameObject);
        //gameObject.SetActive(false);
        GetComponent<BoxCollider2D>().enabled = false;
    }


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
