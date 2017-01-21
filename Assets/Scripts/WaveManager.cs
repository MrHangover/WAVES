using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour {

    //Constants
    const float PILLAR_WIDTH = 0.2f;
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

    //Knowing the amplitude of each frequency is important because if you get like 5 samples maybe only the first 2 are very loud. So it's important to scale each individual frequency's sine wave by its specific amplitude.
	public List<KeyValuePair<float, float>> frequencyAndAmp = new List<KeyValuePair<float, float>>();
    public float amplitude = 1f;
    public GameObject pillar;

    //Privates
    List<WaveObject> pillars;

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
	void FixedUpdate () {

        CleanPillarList();

        Vector2 pillarPos = pillars[0].body.position;
        if (scrollSpeed >= 0f)
        {
            Debug.Log("Scrolling left!");
            pillars[0].body.position -= Vector2.right * scrollSpeed * Time.fixedDeltaTime;
            pillarPos = pillars[0].body.position;
        }
        else
        {
            Debug.Log("Scrolling right!");
            pillars[pillars.Count - 1].body.position -= Vector2.right * scrollSpeed * Time.fixedDeltaTime;
            pillarPos = pillars[pillars.Count - 1].body.position;
        }

        for (int i = 0; i < pillars.Count; i++)
        {
            if (pillars[i] != null)
            {
				float sins = 0;
				foreach(KeyValuePair<float,float> faa in frequencyAndAmp){
					sins += (Mathf.Sin(pillars[i].body.position.x * faa.Key * faa.Value));
				}

                if (scrollSpeed >= 0f)
                {
                    pillars[i].body.position = new Vector3(pillarPos.x + PILLAR_WIDTH * i, pillars[i].startYPos +  sins * amplitude);
                }
                else
                {
                    pillars[i].body.position = new Vector3(pillarPos.x - PILLAR_WIDTH * ((pillars.Count - 1) - i), pillars[i].startYPos + sins * amplitude);
                }

                if (scrollSpeed >= 0f && pillars[i].body.position.x < PILLAR_START_POS)
                {
                    Destroy(pillars[i].gameObject);
                    GameObject newPillar = Instantiate(pillar, new Vector3(PILLAR_END_POS, pillarYPosition, pillarLayer), Quaternion.identity);
                    pillars.Add(newPillar.GetComponent<WaveObject>());
                    //Don't remove the reference here, as it will create an infinite loop!
                }
                else if (scrollSpeed < 0f && pillars[i].body.position.x > PILLAR_END_POS)
                {
                    float difference = pillars[i].body.position.x - PILLAR_END_POS;
                    Destroy(pillars[i].gameObject);
                    GameObject newPillar = Instantiate(pillar, new Vector3(PILLAR_START_POS + difference, pillarYPosition, pillarLayer), Quaternion.identity);
                    pillars.Insert(0, newPillar.GetComponent<WaveObject>());
                    i++; //skip the (now null) game object as we just checked it
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
        pillars = new List<WaveObject>();

        int count = 0;
        for (float x = PILLAR_START_POS; x < PILLAR_END_POS; x += PILLAR_WIDTH)
        {
            GameObject pInstance = Instantiate(pillar, new Vector3(x, pillarYPosition, pillarLayer), Quaternion.identity);
            pInstance.name = pillars.Count.ToString();
            pillars.Add(pInstance.GetComponent<WaveObject>());
            count++;
        }
    }

    public void Init()
    {
        SpawnPillars();
    }
}
