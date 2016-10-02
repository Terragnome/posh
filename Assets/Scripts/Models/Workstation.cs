using UnityEngine;
using System.Collections;

public class Workstation : MonoBehaviour {
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void Use(float dT) {
		UpdateProgress(dT);
	}

	void UpdateProgress(float dT){
		Debug.Log("Using!");
	}
}
