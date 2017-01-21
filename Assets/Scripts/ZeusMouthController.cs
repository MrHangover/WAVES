using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZeusMouthController : MonoBehaviour {

    const float MOVEMENT_SCALE = 0.15f;
    const float CLAMP_Y = 1f;

	// Use this for initialization
	void Awake () {

	}
	
	// Update is called once per frame
	void Update () {
        Vector3 parentPos = transform.parent.position;

        float amp = 0f;
        if (FrequencyAnalysis.instance.frequencyAndAmp.Count > 0)
        {
            amp = FrequencyAnalysis.instance.frequencyAndAmp[0].Value;
        }

        transform.position = Vector3.Lerp(transform.position, parentPos + Vector3.down * amp * MOVEMENT_SCALE, Time.deltaTime * 6f);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, parentPos.y - CLAMP_Y, parentPos.y), transform.position.z);
	}
}
