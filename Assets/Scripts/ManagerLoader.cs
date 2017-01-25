using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerLoader : MonoBehaviour {

    public GameObject waveManager;
    public GameObject gameManager;
    public GameObject freqAnalysis;


	// Use this for initialization
	void Awake () {
		if(WaveManager.instance == null)
        {
            Instantiate(waveManager);
        }
        if(GameManager.instance == null)
        {
            Instantiate(gameManager);
        }

        if (FrequencyAnalysis.instance == null)
        {
            Instantiate(freqAnalysis);
        }
    }

    void Start()
    {
  
        WaveManager.instance.Init();

        GameManager.instance.Init();

        FrequencyAnalysis.instance.Init();
    }
}
