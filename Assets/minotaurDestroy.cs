﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minotaurDestroy : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter2D(Collision2D other)
    {
        Destroy(gameObject);
    }
}
