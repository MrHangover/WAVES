using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {

	[SerializeField] GameObject ball;
	Vector3 pos;
	[SerializeField] float mboundy, boundy;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		pos = transform.position;
		pos.y = Mathf.Clamp (Mathf.Lerp (pos.y, ball.transform.position.y, Time.deltaTime * 2f), mboundy, boundy);
		transform.position = pos;
		
	}
}
