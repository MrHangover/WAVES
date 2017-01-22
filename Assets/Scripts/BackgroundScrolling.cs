using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScrolling : MonoBehaviour {

    const float SCROLL_SCALE = 0.25f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (WaveManager.instance.isScrolling)
        {
            transform.position -= Vector3.right * WaveManager.instance.scrollSpeed * Time.deltaTime * SCROLL_SCALE;
        }
    }
}
