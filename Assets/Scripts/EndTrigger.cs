using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTrigger : MonoBehaviour {

    public GameObject LightningBolt;
	public Transform lightningSpot;
	GameObject lightInst;

    void OnTriggerEnter2D (Collider2D other)
    {
		print ("HELLO!");
		lightInst = (GameObject)Instantiate(LightningBolt,lightningSpot.position,lightningSpot.rotation);
		//lightInst.transform.SetParent (lightningSpot.parent);
		lightInst.transform.Rotate (new Vector3 (0, 0, 90f));

		lightInst.GetComponent<LightningScript>().SetValues(lightningSpot.position , Quaternion.Euler(0,0,70));
		WaveManager.instance.isMakingWaves = false;
		WaveManager.instance.isScrolling = false;
		Destroy (gameObject);
    }


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}


}
