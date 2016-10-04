using UnityEngine;
using System.Collections;

public class Portable : MonoBehaviour {
	Container container = null;

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
		// rb.isKinematic = true;
	}

	public void DropOn(Container targetContainer) {
		container = targetContainer;
		GetComponent<Rigidbody>().MovePosition(container.transform.position+Vector3.up*container.transform.lossyScale.y);
	}

	public void Drop() {
		rb.velocity = Vector3.zero;
		// rb.isKinematic = false;
		rb.useGravity = true;
	}
}
