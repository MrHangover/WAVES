using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelCompleteScript : MonoBehaviour {

    const string playerTag = "Player";

    public BoxCollider2D minotaurFightTrigger;
    public float delay = 1.3f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag != playerTag )
            return;

        if (minotaurFightTrigger.isActiveAndEnabled == true)
            return;

        StartCoroutine(LoadCredits());
        GetComponent<BoxCollider2D>().enabled = false;
    }

    IEnumerator LoadCredits()
    {
        yield return new WaitForSeconds(delay);

        print("Game Finished!");
        SceneManager.LoadScene("Credits");
    }
}
