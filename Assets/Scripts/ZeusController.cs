using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeusController : MonoBehaviour {

    const float MOVEMENT_RADIUS_Y = 2f;
    const float MOVEMENT_RADIUS_X = 0.5f;
    const float TIME_TO_CHANGE = 0.5f;

    Vector3 startPos;
    float xPos = 0f;
    float yPos = 0f;

	// Use this for initialization
	void Awake () {
        startPos = transform.position;
	}

    void Start()
    {
        Invoke("ChangePositions", 0f);
    }
	
	// Update is called once per frame
	void Update () {
        transform.position = Vector3.Lerp(transform.position, new Vector3(startPos.x + xPos, startPos.y + yPos, startPos.z), Time.deltaTime * 0.5f);
	}

    void ChangePositions()
    {
        xPos = Random.Range(-MOVEMENT_RADIUS_X, MOVEMENT_RADIUS_X);
        yPos = Random.Range(-MOVEMENT_RADIUS_Y, MOVEMENT_RADIUS_Y);
        Invoke("ChangePositions", TIME_TO_CHANGE);
    }
}
