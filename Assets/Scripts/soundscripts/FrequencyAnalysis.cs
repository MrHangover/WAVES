using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Linq;

public class FrequencyAnalysis : MonoBehaviour {

	string micstring = "Built-in Microphone";

	public AudioSource aso;

	public int numSamples = 2048;
	public int samplesToTake = 64;
	public GameObject abar;
	public bool bars = false;

	// Private Variables
	float[] numberleft;
	float[] volumeSamples;
	float volumenumber;
	public GameObject[] thebarsleft;
	float spacing;
	float width;

	public float pitch;
	float threshold = 0.02f;


	[Range(0,100)]
	public float volumeScale = 10;
	float volumeRef = 0.1f;
	float specScale = 20f;
	float prevVolume;
	float outputVolume;

	public float noiseLevel = 0.7f;
	public float lerpSpeed = 1.5f;

	void Start() {
		aso.clip = Microphone.Start (micstring, true, 1, 44100);
		while (!(Microphone.GetPosition(null) > 0)){}
		aso.Play ();
		aso.loop = true;

		numberleft = new float[numSamples];
		volumeSamples = new float[numSamples];

		if (bars) {
			thebarsleft = new GameObject[samplesToTake];
			volumenumber = 0;
			spacing = 0.4f - (samplesToTake * 0.001f);
			width = 0.3f - (samplesToTake * 0.001f);
			for (int i = 0; i < samplesToTake; i++) {
				float xpos = i * spacing - 8.0f;
				Vector3 positionleft = new Vector3 (xpos, 3, 0);
				thebarsleft [i] = (GameObject)Instantiate (abar, positionleft, Quaternion.identity) as GameObject;
				thebarsleft [i].transform.localScale = new Vector3 (width, 1, 0.2f);
			}
		}
	}

	// Update is called once per frame
	void Update () {
		numberleft = new float[numSamples];

		aso.GetSpectrumData (numberleft, 0,FFTWindow.BlackmanHarris);

		//	22050/samplenumber = 10.7
		//	TO FIND ELEMENT IN ARRAY: 
		//	FrequencyYouWant / 21(result from last)
		// 441 Hz -> [21]. that's then numberleft[21]


		float specLeft;
		//print (numberleft [1]);
		numberleft = numberleft.Take (samplesToTake).ToArray();
		//print (numberleft [1]);

		for (int i = 0; i < samplesToTake; i++) {
			if (float.IsInfinity (numberleft [i]) || float.IsNaN (numberleft [i])) {
			} else {

				//if(maxN > 0 && maxN < numSamples - 1){ //interpolate index using neighbors
				//		float dl = numberleft[maxN - 1] / numberleft[maxN];
				//	float dr = numberleft[maxN + 1] / numberleft[maxN];
				//	freq = 0.5f * (dr*dr-dl*dl);

				specLeft = numberleft [i];
				if (specLeft != 0) {
					specLeft *= 10f;
					specLeft = Mathf.Log10 (specLeft);
					specLeft = 1/specLeft;
				}

				specLeft = Mathf.Abs (specLeft);

				//print (numberleft[i]+" "+numberright[i]);

				if (bars) {
					thebarsleft [i].transform.localScale = new Vector3 (width, specLeft, 0.2f);
				}
			}
		}

		aso.GetOutputData (volumeSamples, 0);

		prevVolume = outputVolume;

		volumenumber = 0f;
		for (int j = 0; j < numSamples; j++) {
			volumenumber += volumeSamples [j] * volumeSamples [j]; //sum squared samples.
		}

		volumenumber = Mathf.Sqrt (volumenumber / numSamples); //rms = square root of average
		volumenumber = (1 / Mathf.Abs (20 * Mathf.Log10 (volumenumber / volumeRef))); //convert to dB

		//transform.localScale = new Vector3 (transform.localScale.x, (volumenumber) * volumeScale, 1); 
		outputVolume = Mathf.Lerp(prevVolume, volumenumber * volumeScale - noiseLevel,Time.deltaTime*lerpSpeed);


		if (outputVolume > 4f) {
			outputVolume = 4f;
		}

		if (WaveManager.instance != null) {
			WaveManager.instance.pillarYPosition = outputVolume*5 -0.9F;
            WaveManager.instance.amplitude = outputVolume * 5 - 0.9F;
            WaveManager.instance.frequency = outputVolume * 5 - 0.9F;
        }



	}


}