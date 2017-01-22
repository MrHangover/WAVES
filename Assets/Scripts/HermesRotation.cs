using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HermesRotation : MonoBehaviour {

    const float ROTATION_SPEED = 35f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (WaveManager.instance.isScrolling)
        {
            transform.Rotate(0f, 0f, -WaveManager.instance.scrollSpeed * ROTATION_SPEED * Time.deltaTime);
        }
	}
}
