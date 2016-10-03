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

	public Rigidbody rigidbody {
		get { return GetComponent<Rigidbody>(); }
	}

	public void Lift() {
		if(container){
			container.Clear();
			container = null;
		}
		rigidbody.useGravity = false;
		rigidbody.isKinematic = true;
	}

	public void DropOn(Usable usable) {
		container = usable;
		this.gameObject.transform.position = container.gameObject.transform.position+Vector3.up;
	}

	public void Drop() {
		rigidbody.isKinematic = false;
		rigidbody.useGravity = true;
	}
}
