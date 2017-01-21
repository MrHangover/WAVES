using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

    [SerializeField]
    float cameraShakeAmplitude = 1f;
    [SerializeField]
    Vector2 clampMinMaxVolume = new Vector2(1f,4f);
    [SerializeField]
    AnimationCurve amplitudeContribution;

    [SerializeField] GameObject ball;
	Vector3 pos;
	[SerializeField] float mboundy = -4, boundy = 2f;

    Vector3 origCamPos;
    CameraShake cameraShake;

    // Use this for initialization
    void Start () {
        origCamPos = transform.position;
        cameraShake = transform.GetComponent<CameraShake>();
    }

    
	// Update is called once per frame
	void Update () {

		pos = transform.position;
		pos.y = Mathf.Clamp (Mathf.Lerp (pos.y, ball.transform.position.y, Time.deltaTime * 2f), mboundy, boundy);
		transform.position = pos;


        
        Vector2 vel2d = ball.GetComponent<Rigidbody2D>().velocity;
        float noise = 0;
        foreach(KeyValuePair<float, float> kvp in FrequencyAnalysis.instance.frequencyAndAmp)
        {
            //Debug.Log(kvp.Value);
            //noise.x += (Mathf.PerlinNoise(Time.time * noiseInputScaler, Time.time * noiseInputScaler) - 0.5f)*2;
            //noise.y += (Mathf.PerlinNoise(Time.time * noiseInputScaler + secondSampleOffset, Time.time * noiseInputScaler + secondSampleOffset) - 0.5f)*2;
            noise += kvp.Value;
        }

        noise *= cameraShakeAmplitude;
        //Debug.Log(noise);
        if (noise > clampMinMaxVolume.y)
            noise = clampMinMaxVolume.y;
        else if (noise < clampMinMaxVolume.x)
            noise = 0;

        noise *= amplitudeContribution.Evaluate(noise / 5);//TODO: CALIBRATION GOES HERE INSTEAD OF 5
        //Debug.Log(noise);
        //Debug.Log("____________________________");
       


        cameraShake.ShakeCamera(noise, 0.1f);
        

        
    }
}
