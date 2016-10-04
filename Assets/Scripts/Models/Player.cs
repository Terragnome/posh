using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	[SerializeField]
	float walkSpeed = 3f;
	[SerializeField]
	float turnSpeed = 7f;
	[SerializeField]
	float dashSpeed = 6f;
	[SerializeField]
	float dashDuration = 0.3f;
	[SerializeField]
	float dashTimeLeft = 0f;
	bool isDashing = false;

	[SerializeField]
	float liftDistanceSquared = 1.5f*1.5f;
	float liftAngle = 45f;
	[SerializeField]
	Liftable liftTarget = null;

	[SerializeField]
	float useDistanceSquared = 1.5f*1.5f;
	float useAngle = 45f;
	[SerializeField]
	float useLiftedDistanceSquared = 2.5f*2.5f;
	
	[SerializeField]
	Usable useTarget = null;
	bool isUsing = false;

	public GameObject avatar {
	    get { return transform.GetChild(0).gameObject; }
	}

	public Vector3 avatarLiftPosition {
		get { return avatar.transform.position-avatar.transform.forward+Vector3.up; }
	}

  	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		float dT = Time.deltaTime;

		UpdateUse(dT);
		if(isUsing) return;

		UpdateLift(dT);
		UpdateMovement(dT);
	}

	void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.GetComponent<Usable>() != null) {
        	collision.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        }
	}

	Vector2 ToVector2(Vector3 v) {
		return new Vector2(v.x, v.z);
	}

	float DistanceSquared(Vector3 targetPosition) {
		Vector2 distVector = ToVector2(avatar.transform.position) - ToVector2(targetPosition);
		Debug.Log(distVector.sqrMagnitude);
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
				!curUsable.isFilled
				&& DistanceSquared(curUsable.transform.position) <= useDistanceSquared
				&& IsFacing(curUsable.transform.position, useAngle)
			){
				closestUsable = curUsable;
			}
        }
        return closestUsable;
	}

	Liftable GetClosestLiftable(float liftDistance) {
		Liftable[] liftables = Object.FindObjectsOfType<Liftable>();
		Liftable closestLiftable = null;
		foreach(Liftable curLiftable in liftables){
			if(
				DistanceSquared(curLiftable.transform.position) <= liftDistanceSquared
				&& IsFacing(curLiftable.transform.position, liftAngle)
			){
				closestLiftable = curLiftable;
			}
        }
        return closestLiftable;
	}

	void UpdateLookAt(Vector3 lookVector, float dT, float lookSpeed=1f) {
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
				Usable closestUsable = GetClosestUsable(useLiftedDistanceSquared);
				if(closestUsable != null){
					closestUsable.FillWith(liftTarget);
					liftTarget.DropOn(closestUsable);
				}else{
					liftTarget.Drop();
				}
				liftTarget = null;
			}else{
				Liftable curTarget = GetClosestLiftable(liftDistanceSquared);
				if(curTarget){
					curTarget.Lift();
					liftTarget = curTarget;
				}
			}
		}

		if(liftTarget != null){
			liftTarget.transform.position = avatarLiftPosition;
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

			transform.Translate(moveVector*curMoveSpeed*dT);
		}
	}
}