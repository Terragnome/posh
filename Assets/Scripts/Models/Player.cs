using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	float gravity = 9.8f;

	public float walkSpeed = 3f;
	public float turnSpeed = 7f;
	public float dashSpeed = 6f;
	public float dashDuration = 0.3f;
	float dashTimeLeft = 0f;
	bool isDashing = false;

	public float pushForce = 5f;

	public float liftDistanceSquared = 1.5f*1.5f;
	public float liftAngle = 45f;
	Portable liftTarget = null;

	public float useDistanceSquared = 1.5f*1.5f;
	public float useAngle = 45f;
	public float useLiftedDistanceSquared = 2.5f*2.5f;
	
	public Usable useTarget = null;
	bool isUsing = false;

	public GameObject avatar {
	    get { return transform.GetChild(0).gameObject; }
	}

	public Vector3 avatarLiftPosition {
		get { return avatar.transform.position-avatar.transform.forward*1.2f+Vector3.up; }
	}

  	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void FixedUpdate () {
		float dT = Time.deltaTime;

		UpdateUse(dT);
		if(isUsing) return;

		UpdateLift(dT);
		UpdateMovement(dT);
	}

	void OnControllerColliderHit(ControllerColliderHit hit) {
		Rigidbody body = hit.collider.attachedRigidbody;
        if(body == null || body.isKinematic) return;

        if(hit.moveDirection.y < -0.3F) return;

        Vector3 pushDir = new Vector3(hit.moveDirection.x, 0, hit.moveDirection.z);
        body.velocity = pushDir*pushForce;
	}

	Vector2 ToVector2(Vector3 v) {
		return new Vector2(v.x, v.z);
	}

	float DistanceSquared(Vector3 targetPosition) {
		Vector2 distVector = ToVector2(avatar.transform.position) - ToVector2(targetPosition);
		return distVector.sqrMagnitude;
	}

	bool IsFacing(Vector3 position, float maxAngle){
		float angle = Vector2.Angle(
			ToVector2(avatar.transform.position) - ToVector2(position),
			ToVector2(avatar.transform.forward)
		);
		return angle <= maxAngle;
	}

	Usable GetClosestUsable(float useDistance) {
		Usable[] usables = Object.FindObjectsOfType<Usable>();
		Usable closestUsable = null;
		foreach(Usable curUsable in usables){
			if(
				DistanceSquared(curUsable.transform.position) <= useDistanceSquared
				&& IsFacing(curUsable.transform.position, useAngle)
			){
				closestUsable = curUsable;
			}
        }
        return closestUsable;
	}

	Container GetClosestContainer(float useDistance) {
		Container[] containers = Object.FindObjectsOfType<Container>();
		Container closestContainer = null;
		foreach(Container curContainer in containers){
			if(
				!curContainer.isFilled
				&& DistanceSquared(curContainer.transform.position) <= useDistanceSquared
				&& IsFacing(curContainer.transform.position, useAngle)
			){
				closestContainer = curContainer;
			}
        }
        return closestContainer;
	}

	Portable GetClosestPortable(float liftDistance) {
		Portable[] portables = Object.FindObjectsOfType<Portable>();
		Portable closestPortable = null;
		foreach(Portable curPortable in portables){
			if(
				DistanceSquared(curPortable.transform.position) <= liftDistanceSquared
				&& IsFacing(curPortable.transform.position, liftAngle)
			){
				closestPortable = curPortable;
			}
        }
        return closestPortable;
	}

	void UpdateLookAt(Vector3 lookVector, float dT, float lookSpeed=1f) {
		// GetComponent<Rigidbody>().MoveRotation(
		avatar.transform.rotation = Quaternion.Lerp(
			avatar.transform.rotation,
			Quaternion.LookRotation(lookVector*-1, Vector3.up),
			lookSpeed*dT
		);
	}

	void UpdateLookAtPosition(Vector3 position, float dT, float lookSpeed=1f) {
		Vector3 lookVector = Vector3.Normalize(position-avatar.transform.position);
		lookVector.y = 0f;
		UpdateLookAt(lookVector, dT, lookSpeed);
	}

	void UpdateUse(float dT) {
		bool checkUse = Input.GetKey(KeyCode.F);
		if(checkUse){
			if( useTarget != null ){				
				if(
					DistanceSquared(useTarget.transform.position) < useDistanceSquared
					&& IsFacing(useTarget.transform.position, useAngle)
				){
					useTarget.Use(dT);
					UpdateLookAtPosition(useTarget.transform.position, dT, turnSpeed);
				}else{
					useTarget = null;					
				}
			}

			if( useTarget == null){
				Usable curTarget = GetClosestUsable(useDistanceSquared);
				if(curTarget){
					useTarget = curTarget;
				}
			}
		}else{
			useTarget = null;
		}

		isUsing = useTarget != null;
	}

	void UpdateLift(float dT) {
		bool checkLift = Input.GetKeyDown(KeyCode.Space);
		if(checkLift){
			if( liftTarget != null ){
				Container closestContainer = GetClosestContainer(useLiftedDistanceSquared);
				if(closestContainer != null){
					closestContainer.FillWith(liftTarget);
					liftTarget.DropOn(closestContainer);
				}else{
					liftTarget.Drop();
				}
				liftTarget = null;
			}else{
				Portable curTarget = GetClosestPortable(liftDistanceSquared);
				if(curTarget){
					curTarget.Lift();
					liftTarget = curTarget;
				}
			}
		}

		if(liftTarget != null){
			liftTarget.GetComponent<Rigidbody>().MovePosition(avatarLiftPosition);
		}
	}

	void UpdateMovement(float dT) {
		float curMoveSpeed = walkSpeed;

		bool isWalkForward = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
		bool isWalkBack = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
		bool isWalkLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
		bool isWalkRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);

		if( !isDashing ){
			bool hasDashed = Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
			if(hasDashed){
				isDashing = true;
				dashTimeLeft = dashDuration;
			}
		}else{
			curMoveSpeed = dashSpeed;
			if(dashTimeLeft > 0){
				dashTimeLeft -= dT;
				if(dashTimeLeft <= 0){
					isDashing = false;
					dashTimeLeft = 0f;
				}
			}
		}

		Vector3 moveVector = Vector3.zero;
		if(isWalkBack) moveVector += Vector3.back;
		if(isWalkForward) moveVector += Vector3.forward;
		if(isWalkLeft) moveVector += Vector3.left;
		if(isWalkRight) moveVector += Vector3.right;
		moveVector = Vector3.Normalize(moveVector);

		if(moveVector != Vector3.zero){
			UpdateLookAt(moveVector, dT, turnSpeed);
		}

		Vector3 fullMoveVector = moveVector*curMoveSpeed;
		fullMoveVector += Vector3.down*gravity;

		// GetComponent<Rigidbody>().MovePosition(transform.position+moveVector*curMoveSpeed*dT);
		GetComponent<CharacterController>().Move(fullMoveVector*dT);
	}
}