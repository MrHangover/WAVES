using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveObject : MonoBehaviour {

    [Range(1, 1000)]
    public int waveLayer = 1;
    public float startYPos;

	// Use this for initialization
	void Awake () {
        //WaveManager.instance.AddWaveObject(waveLayer, gameObject);
        startYPos = transform.position.y;
    }

    void Start()
    {

    }
}
