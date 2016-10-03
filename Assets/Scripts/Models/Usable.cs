using UnityEngine;
using System.Collections;

public class Usable : MonoBehaviour {
	Liftable contents = null;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
	}

	public bool IsFilled {
		get { return contents != null; }
	}

	public void Use(float dT) {
		UpdateProgress(dT);
	}

	public void FillWith(Liftable liftable) {
		contents = liftable;
		Debug.Log("Filled");
	}

	public void Clear() {
		contents = null;
	}

	void UpdateProgress(float dT){
		Debug.Log("Using!");
	}
}
