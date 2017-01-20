using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class microphonetest : MonoBehaviour {

	public AudioSource aus;
	string MicString = "Built-in Microphone";
	float[] samples;
	int numSamples = 64;
	// Use this for initialization
	void Start () {
		samples = new float[numSamples];

		aus = GetComponent<AudioSource> ();
		aus.clip = Microphone.Start (MicString, true, 1, 44100);
		aus.Play ();
		aus.loop = true;
	}
	
	// Update is called once per frame
	void Update () {
		print (aus.volume);

		aus.GetSpectrumData (samples, 0,FFTWindow.BlackmanHarris);
		print (samples[21]);
	}
}
