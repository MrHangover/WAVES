using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerLoader : MonoBehaviour {

    public GameObject waveManager;

	// Use this for initialization
	void Awake () {
		if(WaveManager.instance == null)
        {
            Instantiate(waveManager);
        }
	}
}
