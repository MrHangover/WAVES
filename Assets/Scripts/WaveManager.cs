using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour {

    //Constants
    const float PILLAR_WIDTH = 1f;
    const float PILLAR_START_POS = -10f;
    const float PILLAR_END_POS = 10f;

    //Public statics
    public static WaveManager instance = null;

    //Publics
    [Range(-6f, 6f)]
    public float pillarYPosition = 0f;
    [Range(-10, 10)]
    public int pillarLayer = 0;
    [Range(-20f, 20f)]
    public float scrollSpeed = 5f;
    public GameObject pillar;

    //Privates
    List<GameObject> pillars;

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

    }

	// Update is called once per frame
	void Update () {

        CleanPillarList();
        pillars[0].transform.position -= Vector3.right * scrollSpeed * Time.deltaTime;
        Vector3 pillarPos = pillars[0].transform.position;

        for (int i = 0; i < pillars.Count; i++)
        {
            if (pillars[i] != null)
            {
                pillars[i].transform.position = new Vector3(pillarPos.x + PILLAR_WIDTH * i, pillars[i].transform.position.y, pillarLayer);

                if (pillars[i].transform.position.x < PILLAR_START_POS)
                {
                    Destroy(pillars[i]);
                    GameObject newPillar = Instantiate(pillar, new Vector3(PILLAR_END_POS, pillarYPosition, pillarLayer), Quaternion.identity);
                    pillars.Add(newPillar);
                    //Don't remove the reference here, as it will create an infinite loop!
                }
                else if (pillars[i].transform.position.x > PILLAR_END_POS)
                {
                    float difference = pillars[i].transform.position.x - PILLAR_END_POS;
                    Destroy(pillars[i]);
                    GameObject newPillar = Instantiate(pillar, new Vector3(PILLAR_START_POS + difference, pillarYPosition, pillarLayer), Quaternion.identity);
                    pillars.Insert(0, newPillar);
                    i++;
                }
            }
        }
	}

    //public void AddWaveObject(int layer, GameObject wave)
    //{
    //    while(waves.Count < layer)
    //    {
    //        waves.Add(new List<GameObject>());
    //    }

    //    waves[layer - 1].Add(wave);
    //}

    void CleanPillarList()
    {
        for(int i = 0; i < pillars.Count; i++)
        {
            if(pillars[i] == null)
            {
                pillars.RemoveAt(i);
                i--;
            }
        }
    }

    void SpawnPillars()
    {
        float width = PILLAR_END_POS - PILLAR_START_POS;
        int numOfPillars = (int)(width / PILLAR_WIDTH);
        pillars = new List<GameObject>();

        int count = 0;
        for (float x = PILLAR_START_POS; x < PILLAR_END_POS; x += PILLAR_WIDTH)
        {
            GameObject pInstance = Instantiate(pillar, new Vector3(x, pillarYPosition, pillarLayer), Quaternion.identity);
            pillars.Add(pInstance);
            count++;
        }
    }

    public void Init()
    {
        SpawnPillars();
    }
}
