using UnityEngine;
using System.Collections;

public class Portable : MonoBehaviour {
	public Container container = null;

	void Start () {
		Rigidbody initRigidbody = rb;
		Collider initCollider = col;
	}

	void Update () {}

	Collider col {
		get {
			SphereCollider sphereCollider = GetComponent<SphereCollider>();
			if(sphereCollider == null){
				sphereCollider = gameObject.AddComponent<SphereCollider>();
				sphereCollider.radius = 0.25f;
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

	public Portable Lift() {
		if(container){
			container.Clear();
			container = null;
		}
		rb.isKinematic = false;
		rb.useGravity = false;
		return this;
	}

	public void DropOn(Container targetContainer) {
		container = targetContainer;
		rb.isKinematic = true;
		Vector3 newPosition = new Vector3(
			container.transform.position.x,
			container.transform.position.y+container.transform.lossyScale.y,
			container.transform.position.z
		);
		rb.MovePosition(newPosition);
	}

	public void Drop() {
		rb.velocity = Vector3.zero;
		rb.useGravity = true;
	}
}
