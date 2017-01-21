using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTrigger : MonoBehaviour {

    public GameObject LightningBolt;

    void OnTriggerEnter (Collider other)
    {
        Instantiate(LightningBolt);
    }


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
