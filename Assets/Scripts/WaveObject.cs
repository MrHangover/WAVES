using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveObject : MonoBehaviour {

    [Range(1, 1000)]
    public int waveLayer = 1;
    public float startYPos;
    public Rigidbody2D body;

    Vector3 previousPos;

	// Use this for initialization
	void Awake () {
        //WaveManager.instance.AddWaveObject(waveLayer, gameObject);
        startYPos = transform.position.y;
        previousPos = transform.position;
    }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        body.velocity = (transform.position - previousPos) * 5f;
        previousPos = transform.position;
    }
}
