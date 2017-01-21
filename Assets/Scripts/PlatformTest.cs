using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformTest : MonoBehaviour {

    Rigidbody2D body;
    public float speed = 5f;

	// Use this for initialization
	void Start () {
        body = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
        body.MovePosition(new Vector2(1f, 1f));
    }

    void FixedUpdate()
    {
    }
}
