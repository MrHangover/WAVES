using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaveManager : MonoBehaviour {

    //Constants
    const float PILLAR_VISUAL_WIDTH = 0.5f;
    const float PILLAR_COLLIDER_WIDTH = 0.1f;
    const float PILLAR_START_POS = -10f;
    const float PILLAR_END_POS = 10f;
    const float GRASS_START_POS = -15f;
    const float GRASS_END_POS = 15f;
    const float GRASS_WIDTH = 6.35f;
    const float CURVE_WIDTH = 1.5f;

    //Public statics
    public static WaveManager instance = null;

    //Publics
    [Range(-8f, 0f)]
    public float grassYPosition = -8f;
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
    public GameObject[] grassTypes;
    public bool isScrolling = true;
    public bool isMakingWaves = true;

	public float amplitudeModifier = 2f;
	public float mapMin = -25f;
	public float mapMax = 10f;

    //Privates
    List<WaveObject> colliderPillars;
    List<WaveObject> visualPillars;
    List<GameObject> grass;
	List<Vector2> previousColliderPillarPositions;
    List<Vector3> previousVisualPillarPositions;

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
        Vector3 grassPos = grass[0].transform.position;
        if (scrollSpeed >= 0f)
        {
            //Debug.Log("Scrolling left!");
            visualPillars[0].transform.position -= Vector3.right * scrollSpeed * Time.fixedDeltaTime;
            grass[0].transform.position -= Vector3.right * scrollSpeed * Time.fixedDeltaTime;
            pillarPos = visualPillars[0].transform.position;
            grassPos = grass[0].transform.position;
        }
        else
        {
            //Debug.Log("Scrolling right!");
            visualPillars[visualPillars.Count - 1].transform.position -= Vector3.right * scrollSpeed * Time.fixedDeltaTime;
            grass[grass.Count - 1].transform.position -= Vector3.right * scrollSpeed * Time.fixedDeltaTime;
            pillarPos = visualPillars[visualPillars.Count - 1].transform.position;
            grassPos = grass[grass.Count - 1].transform.position;
        }

        float freq = 0f;
        float amp = 0f;
        if (frequencyAndAmp.Count > 0 && isMakingWaves)
        {
			if (FrequencyAnalysis.instance != null) {
				//freq = MapToCalibration((FrequencyAnalysis.instance.MapToCalibration(frequencyAndAmp[0].Key)));
				freq = TotalMap(FrequencyAnalysis.instance.avgFreq);
				amp = Mathf.Clamp (frequencyAndAmp [0].Value * amplitudeModifier, 0, 10f);
			}

        }
		if (FrequencyAnalysis.instance != null) {
			//float hejmartin = FrequencyAnalysis.instance.MapToCalibration(frequencyAndAmp[0].Key);
			Debug.Log("FREQ: " +frequencyAndAmp[0].Key +"    "+ freq+"   AMP: "+amp);
		}

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

                        colliderPillars[i].body.position = Vector2.Lerp(previousColliderPillarPositions[i],
																new Vector2(colliderPillars[i].body.position.x, colliderPillars[i].startYPos + Gaussian(colliderPillars[i].body.position.x, amp, freq)),
																Time.fixedDeltaTime*2f);
                    }
                    else
                    {
                        colliderPillars[i].body.position = Vector2.Lerp(previousColliderPillarPositions[i],
																new Vector2(colliderPillars[i].body.position.x, colliderPillars[i].startYPos + Gaussian(colliderPillars[i].body.position.x, amp, freq)),
																Time.fixedDeltaTime*2f);
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

				for (int j = 0; j < colliderPillars.Count; j++) {
                    previousColliderPillarPositions[j] = colliderPillars[j].body.position;
				}

                //Add new pillars at the start or end if some pillars reached the edge.
                if (scrollSpeed >= 0f && colliderPillars[i].body.position.x < PILLAR_START_POS)
                {
                    Destroy(colliderPillars[i].gameObject);
                    GameObject newPillar = Instantiate(colliderPillar, new Vector3(PILLAR_END_POS, pillarYPosition, pillarLayer), Quaternion.identity);
                    colliderPillars.Add(newPillar.GetComponent<WaveObject>());
                    previousColliderPillarPositions.Add (newPillar.GetComponent<WaveObject> ().body.position);
                    //Don't remove the reference here, as it will create an infinite loop!
                }
                else if (scrollSpeed < 0f && colliderPillars[i].body.position.x > PILLAR_END_POS)
                {
                    float difference = colliderPillars[i].body.position.x - PILLAR_END_POS;
                    Destroy(colliderPillars[i].gameObject);
                    GameObject newPillar = Instantiate(colliderPillar, new Vector3(PILLAR_START_POS + difference, pillarYPosition, pillarLayer), Quaternion.identity);
                    colliderPillars.Insert(0, newPillar.GetComponent<WaveObject>());
                    previousColliderPillarPositions.Insert (0,newPillar.GetComponent<WaveObject> ().body.position);
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
                        float visualToColliderPoint = (visualPillars[i].transform.position.x + 10f) / 20f;
                        visualToColliderPoint *= colliderPillars.Count;
                        if (visualToColliderPoint <= 0f || visualToColliderPoint >= colliderPillars.Count - 1)
                        {
                            visualPillars[i].transform.position = new Vector3(pillarPos.x + PILLAR_VISUAL_WIDTH * i,
                                                                  visualPillars[i].startYPos,
                                                                  pillarLayer);
                        }
                        else if (visualToColliderPoint % 1f == 0f)
                        {
                            visualPillars[i].transform.position = new Vector3(pillarPos.x + PILLAR_VISUAL_WIDTH * i,
                                                                  colliderPillars[(int)visualToColliderPoint].body.position.y,
                                                                  pillarLayer);
                        }
                        else
                        {
                            int first = Mathf.FloorToInt(visualToColliderPoint);
                            float firstHeight = colliderPillars[first].body.position.y;
                            float secondHeight = colliderPillars[first + 1].body.position.y;
                            float interpHeight = firstHeight - (firstHeight - secondHeight) * (first - visualToColliderPoint);

                            visualPillars[i].transform.position = new Vector3(pillarPos.x + PILLAR_VISUAL_WIDTH * i,
                                                                  interpHeight,
                                                                  pillarLayer);
                        }
                    }
                    else
                    {
                        float visualToColliderPoint = (visualPillars[i].transform.position.x + 10f) / 20f;
                        visualToColliderPoint *= colliderPillars.Count;
                        if (visualToColliderPoint <= 0f || visualToColliderPoint >= colliderPillars.Count - 1)
                        {
                            visualPillars[i].transform.position = new Vector3(pillarPos.x - PILLAR_VISUAL_WIDTH * ((visualPillars.Count - 1) - i),
                                                                  visualPillars[i].startYPos,
                                                                  pillarLayer);
                        }
                        else if (visualToColliderPoint % 1f == 0f)
                        {
                            visualPillars[i].transform.position = new Vector3(pillarPos.x - PILLAR_VISUAL_WIDTH * ((visualPillars.Count - 1) - i),
                                                                  colliderPillars[(int)visualToColliderPoint].body.position.y,
                                                                  pillarLayer);
                        }
                        else
                        {
                            int first = Mathf.FloorToInt(visualToColliderPoint);
                            float firstHeight = colliderPillars[first].body.position.y;
                            float secondHeight = colliderPillars[first + 1].body.position.y;
                            float interpHeight = firstHeight - (firstHeight - secondHeight) * (first - visualToColliderPoint);

                            visualPillars[i].transform.position = new Vector3(pillarPos.x - PILLAR_VISUAL_WIDTH * ((visualPillars.Count - 1) - i),
                                                                  interpHeight,
                                                                  pillarLayer);
                        }
                    }
                }

                else
                {
                    float sins = 0;
                    foreach (KeyValuePair<float, float> faa in frequencyAndAmp)
                    {
                        float freq2 = (faa.Key / 684.8f) * 4f;
                        sins += (Mathf.Sin(visualPillars[i].transform.position.x * freq2 * faa.Value * FrequencyAnalysis.instance.micVolumeScale + FrequencyAnalysis.instance.noiseLevel));
                    }


                    if (scrollSpeed >= 0f)
                    {
                        visualPillars[i].transform.position = new Vector3(pillarPos.x + PILLAR_VISUAL_WIDTH * i, visualPillars[i].startYPos + sins * amplitude);
                    }
                    else
                    {
                        visualPillars[i].transform.position = new Vector3(pillarPos.x - PILLAR_VISUAL_WIDTH * ((visualPillars.Count - 1) - i), visualPillars[i].startYPos + sins * amplitude);
                    }
                }

                for (int j = 0; j < visualPillars.Count; j++)
                {
                    previousVisualPillarPositions[j] = visualPillars[j].transform.position;
                }

                //Add new pillars at the start or end if some pillars reached the edge.
                if (scrollSpeed >= 0f && visualPillars[i].transform.position.x < PILLAR_START_POS)
                {
                    Destroy(visualPillars[i].gameObject);
                    GameObject newPillar = Instantiate(visualPillar[Random.Range(0, visualPillar.Length)], new Vector3(PILLAR_END_POS, pillarYPosition, pillarLayer), Quaternion.identity);
                    visualPillars.Add(newPillar.GetComponent<WaveObject>());
                    previousVisualPillarPositions.Add(newPillar.transform.position);
                    //Don't remove the reference here, as it will create an infinite loop!
                }
                else if (scrollSpeed < 0f && visualPillars[i].transform.position.x > PILLAR_END_POS)
                {
                    float difference = visualPillars[i].transform.position.x - PILLAR_END_POS;
                    Destroy(visualPillars[i].gameObject);
                    GameObject newPillar = Instantiate(visualPillar[Random.Range(0, visualPillar.Length)], new Vector3(PILLAR_START_POS + difference, pillarYPosition, pillarLayer), Quaternion.identity);
                    visualPillars.Insert(0, newPillar.GetComponent<WaveObject>());
                    previousVisualPillarPositions.Insert(0, newPillar.transform.position);
                    i++; //skip the (now null) game object as we just checked it
                }
            }
        }
        #endregion

        //Moving the grass
        #region grass
        for (int i = 0; i < grass.Count; i++)
        {
            if (grass[i] != null)
            {

                //Move all grass based on a single grass at the start or end.
                if (scrollSpeed >= 0f)
                {
                    grass[i].transform.position = new Vector3(grassPos.x + i * GRASS_WIDTH,
                                                              grassYPosition,
                                                              pillarLayer);
                }
                else
                {
                    grass[i].transform.position = new Vector3(grassPos.x - GRASS_WIDTH * ((grass.Count - 1) - i),
                                                              grassYPosition,
                                                              pillarLayer);
                }

                //Add new grass at the start or end if some grass reached the edge.
                if (scrollSpeed >= 0f && grass[i].transform.position.x < GRASS_START_POS)
                {
                    Destroy(grass[i]);
                    GameObject newGrass = Instantiate(grassTypes[Random.Range(0, grassTypes.Length)], new Vector3(GRASS_END_POS, grassYPosition, pillarLayer), Quaternion.identity);
                    grass.Add(newGrass);
                    //Don't remove the reference here, as it will create an infinite loop!
                }
                else if (scrollSpeed < 0f && grass[i].transform.position.x > GRASS_END_POS)
                {
                    float difference = grass[i].transform.position.x - GRASS_END_POS;
                    Destroy(grass[i]);
                    GameObject newGrass = Instantiate(grassTypes[Random.Range(0, grassTypes.Length)], new Vector3(GRASS_START_POS + difference, grassYPosition, pillarLayer), Quaternion.identity);
                    grass.Insert(0, newGrass);
                    i++; //skip the (now null) game object as we just checked it
                }
            }
        }
        #endregion
    }

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
				previousVisualPillarPositions.RemoveAt (i);
                i--;
            }
        }

        for (int i = 0; i < grass.Count; i++)
        {
            if (grass[i] == null)
            {
                grass.RemoveAt(i);
                i--;
            }
        }
    }

    void SpawnColliderPillars()
    {
        float width = PILLAR_END_POS - PILLAR_START_POS;
        int numOfPillars = (int)(width / PILLAR_COLLIDER_WIDTH);
        colliderPillars = new List<WaveObject>();
        previousColliderPillarPositions = new List<Vector2>();

        int count = 0;
        for (float x = PILLAR_START_POS; x < PILLAR_END_POS; x += PILLAR_COLLIDER_WIDTH)
        {
            GameObject pInstance = Instantiate(colliderPillar, new Vector3(x, pillarYPosition, pillarLayer), Quaternion.identity);
            colliderPillars.Add(pInstance.GetComponent<WaveObject>());
            previousColliderPillarPositions.Add(pInstance.GetComponent<WaveObject>().body.position);
            count++;
        }
    }

    void SpawnVisualPillars()
    {
        float width = PILLAR_END_POS - PILLAR_START_POS;
        int numOfPillars = (int)(width / PILLAR_VISUAL_WIDTH);
        visualPillars = new List<WaveObject>();
        previousVisualPillarPositions = new List<Vector3> ();

        int count = 0;
        for (float x = PILLAR_START_POS; x < PILLAR_END_POS; x += PILLAR_VISUAL_WIDTH)
        {
            GameObject pInstance = Instantiate(visualPillar[Random.Range(0, visualPillar.Length)], new Vector3(x, pillarYPosition, pillarLayer), Quaternion.identity);
            visualPillars.Add(pInstance.GetComponent<WaveObject>());
            previousVisualPillarPositions.Add (pInstance.transform.position);
            count++;
        }
    }

    void SpawnGrass()
    {
        float width = GRASS_END_POS - GRASS_START_POS;
        int numOfGrass = (int)(width / GRASS_WIDTH);
        grass = new List<GameObject>();

        int count = 0;
        for (float x = GRASS_START_POS; x < GRASS_END_POS; x += GRASS_WIDTH)
        {
            GameObject grassInstance = Instantiate(grassTypes[Random.Range(0, grassTypes.Length)], new Vector3(x, grassYPosition, pillarLayer), Quaternion.identity);
            grass.Add(grassInstance);
            count++;
        }
    }

    public void Init()
    {
        SpawnColliderPillars();
        SpawnVisualPillars();
        SpawnGrass();
    }


	public float MapToCalibration(float freqtoMap){
		float mapmax = 10; float mapmin = -15;
		return (((freqtoMap - mapmin) * (mapmax - mapmin)) / (FrequencyAnalysis.instance.calibrationFreqMax - FrequencyAnalysis.instance.calibrationFreqMin)) + mapmin;
	}

	public float TotalMap(float freqtoMap){
		float mapmax = mapMax; float mapmin = mapMin;
		return (((freqtoMap - mapmin) * (mapmax - mapmin)) / (684.8f - 0)) + mapmin;
	}

}
