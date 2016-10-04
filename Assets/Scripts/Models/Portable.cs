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
		rb.isKinematic = false;
		rb.useGravity = false;
	}

	public void DropOn(Container targetContainer) {
		container = targetContainer;
		rb.isKinematic = true;
		rb.MovePosition(container.transform.position+Vector3.up*container.transform.lossyScale.y);
	}

	public void Drop() {
		rb.velocity = Vector3.zero;
		rb.useGravity = true;
	}
}
