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
	float liftDistance = 2.5f;
	[SerializeField]
	Liftable liftTarget = null;

	[SerializeField]
	float useDistance = 1.5f;
	[SerializeField]
	float useLiftedDistance = 2.5f;
	
	[SerializeField]
	Usable useTarget = null;
	bool isUsing = false;

	public GameObject avatar {
	    get { return this.gameObject.transform.GetChild(0).gameObject; }
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

	bool CloseEnough(Vector3 position, float distance) {
		Vector3 distVector = this.gameObject.transform.position-position;
		return distVector.sqrMagnitude <= distance*distance;
	}

	Usable GetClosestUsable(float useDistance) {
		GameObject[] usables = GameObject.FindGameObjectsWithTag(TagType.USABLE);
		foreach(GameObject curGO in usables){
			if(CloseEnough(curGO.transform.position, useDistance)){
				return curGO.GetComponent<Usable>();
			}
        }
        return null;
	}

	Liftable GetClosestLiftable(float liftDistance) {
		GameObject[] liftables = GameObject.FindGameObjectsWithTag(TagType.LIFTABLE);
		foreach(GameObject curGO in liftables){
			if(CloseEnough(curGO.transform.position, liftDistance)){
				return curGO.GetComponent<Liftable>();
			}
        }
        return null;
	}

	void UpdateLookAt(Vector3 lookVector, float dT, float lookSpeed=1f) {
		avatar.transform.rotation = Quaternion.Lerp(
			avatar.transform.rotation,
			Quaternion.LookRotation(lookVector*-1, Vector3.up),
			lookSpeed*dT
		);
	}

	void UpdateLookAtPosition(Vector3 position, float dT, float lookSpeed=1f) {
		Vector3 lookVector = Vector3.Normalize(position-this.gameObject.transform.position);
		lookVector.y = 0f;
		UpdateLookAt(lookVector, dT, lookSpeed);
	}

	void UpdateUse(float dT) {
		bool checkUse = Input.GetKey(KeyCode.F);
		if(checkUse){
			if( useTarget != null ){				
				if(CloseEnough(useTarget.transform.position, useDistance)){
					useTarget.Use(dT);
					UpdateLookAtPosition(useTarget.transform.position, dT, turnSpeed);
				}else{
					useTarget = null;					
				}
			}

			if( useTarget == null){
				Usable curTarget = GetClosestUsable(useDistance);
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
				Usable closestUsable = GetClosestUsable(useLiftedDistance);
				if(closestUsable != null && !closestUsable.IsFilled){
					closestUsable.FillWith(liftTarget);
					liftTarget.DropOn(closestUsable);
				}else{
					liftTarget.Drop();
				}
				liftTarget = null;
			}else{
				Liftable curTarget = GetClosestLiftable(liftDistance);
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

			this.gameObject.transform.Translate(moveVector*curMoveSpeed*dT);
		}
	}
}