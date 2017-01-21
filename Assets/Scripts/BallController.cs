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

        constrainBallToCameraAirspace();
    }


    /// <summary>
    /// Makes sure ball doesn't get thrown outside the screen, and doesn't fall through the ground.
    /// It can however move outside the screen vertically, but makes sure it always falls back down.
    /// </summary>
    void constrainBallToCameraAirspace()
    {
        Bounds cameraBounds = Camera.main.OrthographicBounds();
        Vector3 cameraPos = Camera.main.transform.position;
        Collider2D sphereCollider = GetComponent<CircleCollider2D>();
        Rigidbody2D rigidbody = GetComponent<Rigidbody2D>();

        float topBound = (cameraPos.y + cameraBounds.extents.y) * 2;
        float bottomBound = (cameraPos.y - cameraBounds.extents.y) - sphereCollider.bounds.extents.y*0.8f;
        float leftBound = cameraPos.x - cameraBounds.extents.x + sphereCollider.bounds.extents.y;
        float rightBound = cameraPos.x + cameraBounds.extents.x - sphereCollider.bounds.extents.y;

        //Check to see if it overlaps a ground tile. If it does, teleport it to the top of that tile.
        int layermask = 1 << 25;//only pillars
        //Collider2D foundCol = Physics2D.OverlapCircle(transform.position, sphereCollider.bounds.extents.y*0.8f, layermask,Camera.main.transform.position.z);
        Collider2D foundCol = Physics2D.OverlapPoint(transform.position, layermask, Camera.main.transform.position.z, 1000);

        if (foundCol)
        {
            Vector3 pos = transform.position;
            //pos.y = foundCol.bounds.extents.y + foundCol.transform.position.y + 0.01f;
            pos.y = foundCol.transform.position.y + 0.01f;
            Debug.Log("Player pos: " + pos + "; foundCol.transform.position: " + foundCol.transform.position);
            transform.position = pos;
        }
        


        Vector3 newPos = transform.position;
        Vector3 newVel = rigidbody.velocity;
        if (transform.position.x < leftBound)
        {
            newPos.x = leftBound;
            newVel.x = 0;
        }
        else if(transform.position.x > rightBound)
        {
            newPos.x = rightBound;
            newVel.x = 0;
        }

        if(transform.position.y < bottomBound)
        {
            newPos.y = bottomBound;
            newVel.y = 0;
        }
        else if(transform.position.y > topBound)
        {
            newPos.y = topBound;
            newVel.y = 0;
        }

        transform.position = newPos;
        rigidbody.velocity = newVel;

    }

  

}
