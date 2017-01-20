using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour {

    public static WaveManager instance = null;

    List<List<GameObject>> waves = new List<List<GameObject>>();

	// Use this for initialization
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
	
    void Start()
    {
        Debug.Log(waves[0].Count);
    }

	// Update is called once per frame
	void Update () {
		
	}

    public void AddWaveObject(int layer, GameObject wave)
    {
        while(waves.Count < layer)
        {
            waves.Add(new List<GameObject>());
        }

        waves[layer - 1].Add(wave);
    }
}
