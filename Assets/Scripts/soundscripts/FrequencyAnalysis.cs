using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Linq;

public class FrequencyAnalysis : MonoBehaviour {

    const string barParentGO = "BarParent";

    //Public statics
    public static FrequencyAnalysis instance = null;

	//string micstring = "Built-in Microphone";

	public AudioSource aso;

    [Header("BarParent is assigned from script.")]
    //[Space(5, order = 1)]
    [Header("You must have a 'BarParent' GameObject in the scene,")]
    [Space(5, order = 1)]

    [Tooltip("BarParent is assigned from script. You must have a 'BarParent' GameObject in the scene.")]
    [SerializeField]
    Transform _barParent;
    public Transform barParent { 
        get { return _barParent; }
        protected set { _barParent = value; } 
    }
    

    public int numSamples = 2048;
	public int samplesToTake = 64;
	public GameObject abar;
	public bool bars = true;

	// Private Variables
	float[] numberleft;
	float[] volumeSamples;
	float volumenumber;
	public GameObject[] thebarsleft;
	float spacing;
	float width;

	public float pitch;
	//float threshold = 0.02f;

	[SerializeField] float volumeScale = 5;
	float volumeRef = 0.1f;
	//float specScale = 20f;
	float prevVolume;


	public float noiseLevel = 0.7f;
	public float lerpSpeed = 1.5f;


    public int numOvertoneSamples = 3;
    SortedDictionary<float, int> localMaximums;
    KeyValuePair<float, int> currentLocalMaximum;
    bool canSaveLM = true;


	int microphoneNr = 0;

	[SerializeField] float freqAmpThreshold = 0.5f;

	//Public Variables
	public float outputVolume;
	public float micVolumeScale = 1;
	public List<KeyValuePair<float, float>> frequencyAndAmp = new List<KeyValuePair<float, float>>();//why do we need this public variable if we're already sending it to WaveManager.instance.frequencyAndAmp?
	public float avgFreq = 0;

	public float calibrationFreqMin = 0f, calibrationFreqMax = 684.8f;

	void Awake () {
		if(instance == null)
		{
			instance = this;
		}
		else if(instance != this)
		{
			Destroy(gameObject);
		}
		DontDestroyOnLoad(gameObject);
	}

    void Start() {

        print(Microphone.devices.Length);
        aso.clip = Microphone.Start(Microphone.devices[microphoneNr], true, 1, 44100);
        while (!(Microphone.GetPosition(null) > 0)) { }

        numberleft = new float[numSamples];
		volumeSamples = new float[numSamples];
        localMaximums = new SortedDictionary<float, int>();
        currentLocalMaximum = new KeyValuePair<float, int>(-1,-1);
    }

    public void Init()
    {
        //print(Microphone.devices.Length);
        //aso.clip = Microphone.Start(Microphone.devices[microphoneNr], true, 1, 44100);
        //while (!(Microphone.GetPosition(null) > 0)) { }

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0 ||
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings-1
            )
        {
            aso.loop = false;
            aso.Stop();
        }
        else
        {
            aso.loop = true;
            aso.Play();
        }

        GameObject bpgo = GameObject.Find(barParentGO);
        if (bpgo == null)
        {
            Debug.LogError("You don't have a GameObject (parented to the Camera) named 'BarParent'.");
        }
        else
        {
            barParent = bpgo.transform;

            if (bars)
            {
                thebarsleft = new GameObject[samplesToTake];
                volumenumber = 0;
                spacing = 0.4f - (samplesToTake * 0.001f);
                width = 0.3f - (samplesToTake * 0.001f);
                for (int i = 0; i < samplesToTake; i++)
                {
                    float xpos = i * spacing - 8.0f;
                    Vector3 positionleft = new Vector3(xpos, 3, 0);
                    thebarsleft[i] = (GameObject)Instantiate(abar, positionleft, Quaternion.identity) as GameObject;
                    thebarsleft[i].transform.SetParent(barParent, false);
                    thebarsleft[i].transform.localScale = new Vector3(width, 1, 0.2f);
                }
            }
        }
    }

    // Update is called once per frame
    void Update () {

        if (thebarsleft.Length == 0 || thebarsleft[0] == null)
            return;

            numberleft = new float[numSamples];
		aso.GetSpectrumData (numberleft, 0,FFTWindow.BlackmanHarris);
		//	22050/samplenumber = 10.7
		//	TO FIND ELEMENT IN ARRAY: 
		//	FrequencyYouWant / 10.7(result from last)
		// 441 Hz -> [21]. that's then numberleft[21]


		float specLeft;
		//print (numberleft [1]);
		numberleft = numberleft.Take (samplesToTake).ToArray();
		//print (numberleft [1]);

		for (int i = 0; i < samplesToTake; i++) {
			if (float.IsInfinity (numberleft [i]) || float.IsNaN (numberleft [i])) {
			} else {
				if (i > 3) {
					//if(maxN > 0 && maxN < numSamples - 1){ //interpolate index using neighbors
					//		float dl = numberleft[maxN - 1] / numberleft[maxN];
					//	float dr = numberleft[maxN + 1] / numberleft[maxN];
					//	freq = 0.5f * (dr*dr-dl*dl);

					specLeft = numberleft [i];
					if (specLeft != 0) {
						specLeft *= 10f;
						specLeft = Mathf.Log10 (specLeft);
						specLeft = 1 / specLeft;
					}

					specLeft = Mathf.Abs (specLeft);

					//print (numberleft[i]+" "+numberright[i]);

               
					if (bars) {
						thebarsleft [i].transform.localScale = new Vector3 (width, Mathf.Clamp(specLeft,0,2), 0.2f);
					}

					/*
                //testing
                if (i % 10 == 0)
                {
                    specLeft = 0.6f + i/60 + Random.Range(0,0.4f);
                }
                else
                    specLeft = 0;
                */
					//specLeft is the amplitude registered for the current frequency range

					registerLocalMaximums (specLeft, i, ref currentLocalMaximum, ref canSaveLM, localMaximums);
				} else 
                {
                    thebarsleft[i].transform.localScale = Vector3.zero;
				}
               

            }
        }

		frequencyAndAmp = chooseTopOvertoneSampleIndices(numOvertoneSamples, localMaximums, 10.7f);
        if (WaveManager.instance != null)
        { 
			WaveManager.instance.frequencyAndAmp = frequencyAndAmp;
        }
        else //otherwise it doesn't run in soundtestscene, because that does not have a WaveManager
        {
			//frequencyAndAmp = chooseTopOvertoneSampleIndices(numOvertoneSamples, localMaximums, 10.7f);
        }



		/// VOLUME -----------

        aso.GetOutputData(volumeSamples, 0);

        prevVolume = outputVolume;


		volumenumber = 0f;
		for (int j = 0; j < numSamples; j++) {
			volumenumber += volumeSamples [j] * volumeSamples [j]; //sum squared samples.
		}

		volumenumber = Mathf.Sqrt (volumenumber / numSamples); //rms = square root of average
		volumenumber = (1 / Mathf.Abs (20 * Mathf.Log10 (volumenumber / volumeRef))); //convert to dB

		//transform.localScale = new Vector3 (transform.localScale.x, (volumenumber) * volumeScale, 1); 
		outputVolume = Mathf.Lerp(prevVolume, ((volumenumber * volumeScale)*micVolumeScale) + noiseLevel,Time.deltaTime*lerpSpeed);


		if (outputVolume > 4f) {
			outputVolume = 4f;
		}

		if (WaveManager.instance != null) {
			WaveManager.instance.amplitude = outputVolume;
		}



		if (Input.GetKey (KeyCode.Alpha1)) {
			if (Microphone.devices.Length > 0) {
				ResetMicrophone (0);
			}
		}
		if (Input.GetKey (KeyCode.Alpha2)) {
			if (Microphone.devices.Length > 1) {
				ResetMicrophone (1);
			}
		}
		if (Input.GetKey (KeyCode.Alpha3)) {
			if (Microphone.devices.Length > 2) {
				
				ResetMicrophone (2);
			}
		}
		avgFreq = 0;
		foreach(KeyValuePair<float,float> kvp in frequencyAndAmp){
			avgFreq += kvp.Key;
		}
		avgFreq /= frequencyAndAmp.Count;
		//print ("----------------------------");
		//print (avgFreq);

	}



    void registerLocalMaximums(float input, int position, ref KeyValuePair<float, int> currentLocalMaximum, ref bool canSaveLM, SortedDictionary<float,int> savedLocalMaximums)
    {
        position++;//avoids zero indexing
        
        if ( input >= currentLocalMaximum.Key 
            )
        {
            currentLocalMaximum = new KeyValuePair<float, int>(input, position);
            canSaveLM = true;

        }
        else
        {
            if (canSaveLM && !savedLocalMaximums.ContainsKey(currentLocalMaximum.Key))
            {
                savedLocalMaximums.Add(currentLocalMaximum.Key, currentLocalMaximum.Value);
                //currentLocalMaximum = new KeyValuePair<float, int>(-1, -1);
                canSaveLM = false;
            }

            currentLocalMaximum = new KeyValuePair<float, int>(input, position);
        }

    }

    /// <summary>
    /// Returns a list of key value pairs of the n-th strongest oversamples. Key is the frequency in Hertz and Value is the amplitude of that frequency.
    /// Knowing the amplitude of each frequency is important because if you get like 5 samples maybe only the first 2 are very loud. Or maybe they are all very silent.
    /// </summary>
    /// <param name="numOvertoneSamples"></param>
    /// <param name="savedLocalMaximums"></param>
    /// <param name="indexScaler"></param>
    /// <returns></returns>
	List<KeyValuePair<float, float>> chooseTopOvertoneSampleIndices(int numOvertoneSamples, SortedDictionary<float, int> savedLocalMaximums, float indexScaler)
    {
		SortedDictionary<float, float> overtonesFreqAndAmp = new SortedDictionary<float, float>();
        currentLocalMaximum = new KeyValuePair<float, int>(-1, -1);

		float max = savedLocalMaximums.Last().Key;

        //sort samples by size;
        for (int i = 0; i < numOvertoneSamples; i++)
        {
            
            if (savedLocalMaximums.Count < 1)
            {
                break;
            }
            KeyValuePair<float,int> kvp = savedLocalMaximums.Last();

			// remove low amp outliers among the max values

			if (kvp.Key < max / 2) {
				break;
			}

			if (kvp.Value > freqAmpThreshold) {
				overtonesFreqAndAmp.Add (kvp.Value * indexScaler, kvp.Key);
				savedLocalMaximums.Remove (kvp.Key);
			}
            
        //    Debug.Log("freq value: "+ kvp.Key+ "; freq index: "+ kvp.Value + "; * indexScaler "+ indexScaler);
        }
       // Debug.Log("______________________________________________________");



		if (overtonesFreqAndAmp.Count == 0) {
			overtonesFreqAndAmp.Add (1, 0);
		}

        savedLocalMaximums.Clear();
		return overtonesFreqAndAmp.ToList();
    }

 



	public void ResetMicrophone(int nr){
		microphoneNr = nr;
		aso.clip = Microphone.Start (Microphone.devices[microphoneNr], true, 1, 44100);
		while (!(Microphone.GetPosition(null) > 0)){}
		aso.Play ();
	}

	public void SetCalibrationFrequencies(float min, float max){
		calibrationFreqMin = min;
		calibrationFreqMax = max;
	}

	public float MapToCalibration(float freqtoMap){
		return (((freqtoMap - calibrationFreqMin) * (calibrationFreqMax - calibrationFreqMin)) / (684.8f - 0f)) + calibrationFreqMin;
	}

}