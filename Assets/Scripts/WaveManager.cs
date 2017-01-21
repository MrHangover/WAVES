using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour {

    //Constants
    const float PILLAR_VISUAL_WIDTH = 0.5f;
    const float PILLAR_COLLIDER_WIDTH = 0.1f;
    const float PILLAR_START_POS = -10f;
    const float PILLAR_END_POS = 10f;
    const float CURVE_WIDTH = 1.5f;

    //Public statics
    public static WaveManager instance = null;

    //Publics
    [Range(-6f, 6f)]
    public float pillarYPosition = 0f;
    [Range(-10, 10)]
    public int pillarLayer = 0;
    [Range(-20f, 20f)]
    public float scrollSpeed = 5f;
    public bool useGaussian = true;

    //Knowing the amplitude of each frequency is important because if you get like 5 samples maybe only the first 2 are very loud. So it's important to scale each individual frequency's sine wave by its specific amplitude.
	public List<KeyValuePair<float, float>> frequencyAndAmp = new List<KeyValuePair<float, float>>();
    public float amplitude = 1f;
    public float singleFrequency = 0f;
    public GameObject colliderPillar;
    public GameObject[] visualPillar;

    //Privates
    List<WaveObject> colliderPillars;
    List<WaveObject> visualPillars;

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

        //Move the start or end pillar based on scrollspeed
        Vector3 pillarPos = visualPillars[0].transform.position;
        if (scrollSpeed >= 0f)
        {
            //Debug.Log("Scrolling left!");
            visualPillars[0].transform.position -= Vector3.right * scrollSpeed * Time.fixedDeltaTime;

            pillarPos = visualPillars[0].transform.position;
        }
        else
        {
            //Debug.Log("Scrolling right!");
            visualPillars[visualPillars.Count - 1].transform.position -= Vector3.right * scrollSpeed * Time.fixedDeltaTime;
            pillarPos = visualPillars[visualPillars.Count - 1].transform.position;
        }

        float freq = 0f;
        float amp = 0f;
        if (frequencyAndAmp.Count > 0)
        {
            freq = frequencyAndAmp[0].Key / 25f - 15f;
            amp = frequencyAndAmp[0].Value * 2f;
        }
        Debug.Log("Freq: " + freq);

        //Moving the physical pillars
        #region physicalPillars
        for (int i = 0; i < colliderPillars.Count; i++)
        {
            if (colliderPillars[i] != null)
            {
				
                if (useGaussian)
                {

                    //Move all pillars based on a single pillar at the start or end, and the frequency and amplitude.
                    if (scrollSpeed >= 0f)
                    {

                        colliderPillars[i].body.position = new Vector2(colliderPillars[i].body.position.x,
                                                               colliderPillars[i].startYPos + Gaussian(colliderPillars[i].body.position.x, amp, freq));
                    }
                    else
                    {

                        colliderPillars[i].body.position = new Vector2(colliderPillars[i].body.position.x,
                                                               colliderPillars[i].startYPos + Gaussian(colliderPillars[i].body.position.x, amp, freq));
                    }
                }

                else
                {
					float sins = 0;
					foreach(KeyValuePair<float,float> faa in frequencyAndAmp){
						float freq2 = (faa.Key / 684.8f) * 4f;
						sins += (Mathf.Sin(colliderPillars[i].body.position.x * freq2 * faa.Value * FrequencyAnalysis.instance.micVolumeScale + FrequencyAnalysis.instance.noiseLevel));
					}


                    if (scrollSpeed >= 0f)
                    {
                        colliderPillars[i].body.position = new Vector3(pillarPos.x + PILLAR_COLLIDER_WIDTH * i, colliderPillars[i].startYPos + sins * amplitude);
                    }
                    else
                    {
                        colliderPillars[i].body.position = new Vector3(pillarPos.x - PILLAR_COLLIDER_WIDTH * ((colliderPillars.Count - 1) - i), colliderPillars[i].startYPos + sins * amplitude);
                    }
                }

                //Add new pillars at the start or end if some pillars reached the edge.
                if (scrollSpeed >= 0f && colliderPillars[i].body.position.x < PILLAR_START_POS)
                {
                    Destroy(colliderPillars[i].gameObject);
                    GameObject newPillar = Instantiate(colliderPillar, new Vector3(PILLAR_END_POS, pillarYPosition, pillarLayer), Quaternion.identity);
                    colliderPillars.Add(newPillar.GetComponent<WaveObject>());
                    //Don't remove the reference here, as it will create an infinite loop!
                }
                else if (scrollSpeed < 0f && colliderPillars[i].body.position.x > PILLAR_END_POS)
                {
                    float difference = colliderPillars[i].body.position.x - PILLAR_END_POS;
                    Destroy(colliderPillars[i].gameObject);
                    GameObject newPillar = Instantiate(colliderPillar, new Vector3(PILLAR_START_POS + difference, pillarYPosition, pillarLayer), Quaternion.identity);
                    colliderPillars.Insert(0, newPillar.GetComponent<WaveObject>());
                    i++; //skip the (now null) game object as we just checked it
                }
            }
        }
        #endregion

        //Moving the visual pillars
        #region visualPillars
        for (int i = 0; i < visualPillars.Count; i++)
        {
            if (visualPillars[i] != null)
            {

                if (useGaussian)
                {
                    
                    //Move all pillars based on a single pillar at the start or end, and the frequency and amplitude.
                    if (scrollSpeed >= 0f)
                    {

                        visualPillars[i].transform.position = new Vector3(pillarPos.x + PILLAR_VISUAL_WIDTH * i,
                                                               visualPillars[i].startYPos + Gaussian(visualPillars[i].transform.position.x, amp, freq),
                                                               pillarLayer);
                    }
                    else
                    {

                        visualPillars[i].transform.position = new Vector3(pillarPos.x - PILLAR_VISUAL_WIDTH * ((visualPillars.Count - 1) - i),
                                                               visualPillars[i].startYPos + Gaussian(visualPillars[i].transform.position.x, amp, freq),
                                                               pillarLayer);
                    }
                }

                else
                {
                    float sins = 0;
                    foreach (KeyValuePair<float, float> faa in frequencyAndAmp)
                    {
                        float freq2 = (faa.Key / 684.8f) * 4f;
                        sins += (Mathf.Sin(visualPillars[i].body.position.x * freq2 * faa.Value * FrequencyAnalysis.instance.micVolumeScale + FrequencyAnalysis.instance.noiseLevel));
                    }


                    if (scrollSpeed >= 0f)
                    {
                        visualPillars[i].body.position = new Vector3(pillarPos.x + PILLAR_VISUAL_WIDTH * i, visualPillars[i].startYPos + sins * amplitude);
                    }
                    else
                    {
                        visualPillars[i].body.position = new Vector3(pillarPos.x - PILLAR_VISUAL_WIDTH * ((visualPillars.Count - 1) - i), visualPillars[i].startYPos + sins * amplitude);
                    }
                }

                //Add new pillars at the start or end if some pillars reached the edge.
                if (scrollSpeed >= 0f && visualPillars[i].transform.position.x < PILLAR_START_POS)
                {
                    Destroy(visualPillars[i].gameObject);
                    GameObject newPillar = Instantiate(visualPillar[Random.Range(0, visualPillar.Length)], new Vector3(PILLAR_END_POS, pillarYPosition, pillarLayer), Quaternion.identity);
                    visualPillars.Add(newPillar.GetComponent<WaveObject>());
                    //Don't remove the reference here, as it will create an infinite loop!
                }
                else if (scrollSpeed < 0f && visualPillars[i].transform.position.x > PILLAR_END_POS)
                {
                    float difference = visualPillars[i].transform.position.x - PILLAR_END_POS;
                    Destroy(visualPillars[i].gameObject);
                    GameObject newPillar = Instantiate(visualPillar[Random.Range(0, visualPillar.Length)], new Vector3(PILLAR_START_POS + difference, pillarYPosition, pillarLayer), Quaternion.identity);
                    visualPillars.Insert(0, newPillar.GetComponent<WaveObject>());
                    i++; //skip the (now null) game object as we just checked it
                }
            }
        }
        #endregion
    }

    //public void AddWaveObject(int layer, GameObject wave)
    //{
    //    while(waves.Count < layer)
    //    {
    //        waves.Add(new List<GameObject>());
    //    }

    //    waves[layer - 1].Add(wave);
    //}

    //Hopefully this is a gaussian function
    float Gaussian(float xPos, float amplitude, float frequency)
    {
        float val = amplitude * Mathf.Exp(-(Mathf.Pow(xPos - frequency, 2f) / (2 * Mathf.Pow(CURVE_WIDTH, 2))));
        //Debug.Log(val);
        return val;
    }

    void CleanPillarList()
    {
        for(int i = 0; i < visualPillars.Count; i++)
        {
            if(visualPillars[i] == null)
            {
                visualPillars.RemoveAt(i);
                i--;
            }
        }
    }

    void SpawnColliderPillars()
    {
        float width = PILLAR_END_POS - PILLAR_START_POS;
        int numOfPillars = (int)(width / PILLAR_COLLIDER_WIDTH);
        colliderPillars = new List<WaveObject>();

        int count = 0;
        for (float x = PILLAR_START_POS; x < PILLAR_END_POS; x += PILLAR_COLLIDER_WIDTH)
        {
            GameObject pInstance = Instantiate(colliderPillar, new Vector3(x, pillarYPosition, pillarLayer), Quaternion.identity);
            colliderPillars.Add(pInstance.GetComponent<WaveObject>());
            count++;
        }
    }

    void SpawnVisualPillars()
    {
        float width = PILLAR_END_POS - PILLAR_START_POS;
        int numOfPillars = (int)(width / PILLAR_VISUAL_WIDTH);
        visualPillars = new List<WaveObject>();

        int count = 0;
        for (float x = PILLAR_START_POS; x < PILLAR_END_POS; x += PILLAR_VISUAL_WIDTH)
        {
            GameObject pInstance = Instantiate(visualPillar[Random.Range(0, visualPillar.Length)], new Vector3(x, pillarYPosition, pillarLayer), Quaternion.identity);
            visualPillars.Add(pInstance.GetComponent<WaveObject>());
            count++;
        }
    }

    public void Init()
    {
        SpawnColliderPillars();
        SpawnVisualPillars();
    }
}
