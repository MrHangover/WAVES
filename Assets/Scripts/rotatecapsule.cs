using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotatecapsule : MonoBehaviour {

    [Range(-10f,20f)]
    public float rotationsPerMinute = 10f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, 0, 6f * rotationsPerMinute * Time.deltaTime);
    }
}
