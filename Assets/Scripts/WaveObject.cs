using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveObject : MonoBehaviour {

    [Range(1, 1000)]
    public int waveLayer = 1;

	// Use this for initialization
	void Awake () {
        WaveManager.instance.AddWaveObject(waveLayer, gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
