using UnityEngine;
using System.Collections;

public class Portable : MonoBehaviour {
	Container container = null;

	// Use this for initialization
	void Start () {
		Rigidbody rigidBody = rb;
		Collider sphereCollider = collider;
	}

	// Update is called once per frame
	void Update () {
	}

	Collider collider {
		get {
			SphereCollider sphereCollider = GetComponent<SphereCollider>();
			if(sphereCollider == null){
				sphereCollider = gameObject.AddComponent<SphereCollider>();
				sphereCollider.radius = 0.5f;
			}
			return sphereCollider;
		}
	}

	Rigidbody rb {
		get {
			Rigidbody rigidbody = GetComponent<Rigidbody>();
			if(rigidbody == null){
				rigidbody = gameObject.AddComponent<Rigidbody>();
				rigidbody.angularDrag = 5;
				rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
				rigidbody.drag = 5;
				rigidbody.mass = 1;
				rigidbody.useGravity = true;
			}
			return rigidbody;
		}
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
