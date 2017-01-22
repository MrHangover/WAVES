using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Menu : MonoBehaviour {

	public List<Button> mics = new List<Button>();

	float micVolumeScale = 5;
	float noiseLevel = 0;

	[SerializeField] Text volumeText;
	[SerializeField] Text noiseText;
	[SerializeField] Text freqMinText;
	[SerializeField] Text freqMaxText;
	[SerializeField] GameObject menuPanel;

	[SerializeField] GameObject calibration;

	[SerializeField] GameObject barParent;
	[SerializeField] GameObject barText;
	[SerializeField] Toggle avgToggle;



	// Use this for initialization
	void Start () {
		for (int i = 0; i < Microphone.devices.Length; i++) {
			if (Microphone.devices [i] != null) {
				mics [i].GetComponentInChildren<Text> ().text = Microphone.devices [i];
			}
		}
		volumeText.text = micVolumeScale.ToString ();
		noiseText.text = noiseLevel.ToString();
		freqMaxText.text = WaveManager.instance.mapMax.ToString();
		freqMinText.text = WaveManager.instance.mapMin.ToString ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			OpenCloseMenu ();
		}

		if (Input.GetKeyDown (KeyCode.F)) {
			ShowHideBars ();
		}

		if (Input.GetKeyDown (KeyCode.A)) {
			WaveManager.instance.useAvg = !WaveManager.instance.useAvg;
			avgToggle.isOn = WaveManager.instance.useAvg;
		}
		
	}

	public void OpenCloseMenu(){
		menuPanel.SetActive (!menuPanel.activeInHierarchy);
		if (menuPanel.activeInHierarchy) {
			Time.timeScale = 0;
		} else {
			Time.timeScale = 1;
		}
	}


	public void ChangeMicrophone(Button b){
		if (Microphone.devices.Length > mics.IndexOf(b)) {
			if (FrequencyAnalysis.instance != null) {
				FrequencyAnalysis.instance.ResetMicrophone (Microphone.devices.ToList ().IndexOf (b.GetComponentInChildren<Text> ().text));
			}
		}
	}

	public void OnSliderChange(Slider sl){
		micVolumeScale = sl.value;
		if (WaveManager.instance != null) {
			WaveManager.instance.amplitudeModifier = micVolumeScale;
		}
		volumeText.text = micVolumeScale.ToString ();

	}

	public void OnNoiseChange(Slider sl){
		noiseLevel = sl.value;
		if (FrequencyAnalysis.instance != null) {
			FrequencyAnalysis.instance.noiseLevel = noiseLevel;
		}
		noiseText.text = noiseLevel.ToString();
	}

	public void OnMaxChange(Slider sl){
		if (WaveManager.instance != null) {
			WaveManager.instance.mapMax = sl.value;
		}
		freqMaxText.text = sl.value.ToString ();
	}

	public void OnMinChange(Slider sl){
		if (WaveManager.instance != null) {
			WaveManager.instance.mapMin = sl.value;
		}
		freqMinText.text = sl.value.ToString ();
	}


	public void StartCalibration(){
		calibration.SetActive (true);
	}

	void ShowHideBars(){
		barParent.SetActive (!barParent.activeInHierarchy);
		barText.SetActive (!barText.activeInHierarchy);
	}

}
