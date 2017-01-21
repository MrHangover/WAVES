using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    public Level level = Level.TestLvl;
    
    public enum Level
    {
        Loader,
        MainMenu,
        TestLvl,
		TheLevel
    }

    void Awake()
    {
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

	// Use this for initialization
	void Start () {
        if(SceneManager.GetActiveScene().name != level.ToString())
        {
            SceneManager.LoadScene(level.ToString());
        }
	}

    public void Init()
    {

    }
}
