using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour {

    public float leftMoveLimit = -7f;
    public float rightMoveLimit = 0f;
    public float centerLimit = -3f;
    public float rightMaxSpeed = 5f;
    public float leftMaxSpeed = -5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(transform.position.x < centerLimit && transform.position.x >= leftMoveLimit)
        {
            WaveManager.instance.scrollSpeed = ((centerLimit - transform.position.x) / (centerLimit - leftMoveLimit)) * leftMaxSpeed;
            //Debug.Log("Left speed: " + ((centerLimit - transform.position.x) / (centerLimit - leftMaxSpeed)) * leftMaxSpeed);
        }
        else if(transform.position.x > centerLimit && transform.position.x < rightMoveLimit)
        {
            WaveManager.instance.scrollSpeed = ((transform.position.x - centerLimit) / (rightMoveLimit - centerLimit)) * rightMaxSpeed;
            //Debug.Log("Right speed: " + ((transform.position.x - centerLimit) / (rightMoveLimit - centerLimit)) * rightMaxSpeed);
        }
        else if(transform.position.x < leftMoveLimit)
        {
            WaveManager.instance.scrollSpeed = leftMaxSpeed;
        }
        else if(transform.position.x > rightMoveLimit)
        {
            WaveManager.instance.scrollSpeed = rightMaxSpeed;
        }
        else
        {
            WaveManager.instance.scrollSpeed = 0f;
        }
	}
}
