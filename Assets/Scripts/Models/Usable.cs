using UnityEngine;
using System.Collections;

public class Usable : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

	Collider collider {
		get {
			BoxCollider boxCollider = GetComponent<BoxCollider>();
			if(boxCollider == null){
				boxCollider = gameObject.AddComponent<BoxCollider>();
				boxCollider.size = new Vector3(1f, 1f, 1f);
			}
			return boxCollider;
		}
	}

	public void Use(float dT) {
		UpdateProgress(dT);
	}

	void UpdateProgress(float dT){
		Debug.Log("Using!");
	}
}
