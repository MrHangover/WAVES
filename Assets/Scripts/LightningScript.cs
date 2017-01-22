using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningScript : MonoBehaviour {

    public float minPitch;

    public float maxPitch;

    public GameObject Minotaur;

    [Range(20f,700f)]
    public float pitchValue = 20;

    public float timerMax = 5f;
	float timer;

    public float minPos = -7;
    public float maxPos = 7;
	public float xOffset = -10;

	Vector2 prevPos = Vector2.zero;
	Quaternion origRot;
	//[SerializeField]
	Vector3 finalRotationV3 = new Vector3(0,0,56);
	Quaternion finalRotation;

    // Use this for initialization
    void Start () {

        Minotaur = GameObject.FindGameObjectWithTag("Minotaur");
		finalRotation = Quaternion.Euler(finalRotationV3);
		timer = timerMax;
    }
	
	// Update is called once per frame
	void Update () {

        float posDif = maxPos - minPos;

        float pitchDif = 680;

        float temp = 5f;
        if (WaveManager.instance != null)
        {
			pitchValue = FrequencyAnalysis.instance.avgFreq;
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;

			gameObject.transform.rotation = Quaternion.Lerp(finalRotation, origRot, timer/timerMax);
			gameObject.transform.localScale = Vector3.Lerp(Vector3.one, new Vector3(0.3f,0.3f,0.3f), timer/timerMax);

			//+xOffset
			gameObject.transform.localPosition = Vector2.Lerp (prevPos, new Vector2 ((pitchValue * posDif / pitchDif) * 2 + minPos , transform.localPosition.y), Time.deltaTime * 1.5f);
			prevPos = gameObject.transform.localPosition;
        }
        else
        {
            /*
			RaycastHit2D hit = Physics2D.Raycast(transform.localPosition, Vector2.down);
			if (hit && hit.collider.tag == "Minotaur")
            {
                Debug.Log("test");
                Destroy(Minotaur);
            }
            timer = temp;
            */
            Debug.Log("test");
            Destroy(Minotaur);
            gameObject.SetActive(false);
        }
 
    }

	public void SetValues(Vector3 newPrevPos, Quaternion origRot, float animationDuration)
    {

		this.prevPos = newPrevPos;
		this.origRot = origRot;
        this.timerMax = animationDuration;
        timer = timerMax;
    }

}
