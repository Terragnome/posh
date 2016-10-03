using UnityEngine;
using System.Collections;

public class Liftable : MonoBehaviour {
	Usable container = null;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	Rigidbody rb {
		get { return GetComponent<Rigidbody>(); }
	}

	public void Lift() {
		if(container){
			container.Clear();
			container = null;
		}
		rb.useGravity = false;
		rb.isKinematic = true;
	}

	public void DropOn(Usable usable) {
		container = usable;
		transform.position = container.transform.position+Vector3.up;
	}

	public void Drop() {
		rb.isKinematic = false;
		rb.useGravity = true;
	}
}
