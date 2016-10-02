using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	float walkSpeed = 3f;
	float turnSpeed = 7f;
	float dashSpeed = 6f;
	float dashDuration = 0.3f;
	float dashTimeLeft = 0f;
	bool isWalking = false;
	bool isDashing = false;

	float liftDistance = 1.25f;
	Liftable liftTarget = null;

	float useDistance = 1.25f;
	Usable useTarget = null;
	bool isUsing = false;

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

	GameObject GetAvatar() {
		return this.gameObject.transform.GetChild(0).gameObject;
	}

	void UpdateLookAt(Vector3 lookVector, float dT, float lookSpeed=1f) {
		GameObject avatar = GetAvatar();
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
				GameObject[] usables = GameObject.FindGameObjectsWithTag(TagType.USABLE);

				foreach(GameObject curGO in usables){
					if(CloseEnough(curGO.transform.position, useDistance)){
						Usable curTarget = curGO.GetComponent(typeof(Usable)) as Usable;
						useTarget = curTarget;
		            	break;
					}
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
				Debug.Log("Drop!");
				liftTarget.transform.parent = null;
				liftTarget = null;
			}else{
				GameObject[] liftables = GameObject.FindGameObjectsWithTag(TagType.LIFTABLE);

				foreach(GameObject curGO in liftables){
					if(CloseEnough(curGO.transform.position, liftDistance)){
						Debug.Log("Lift!");

						GameObject avatar = GetAvatar();
						curGO.transform.parent = avatar.transform;

						Liftable curTarget = curGO.GetComponent(typeof(Liftable)) as Liftable;
						liftTarget = curTarget;
		            	break;
					}
		        }
			}
		}
	}

	void UpdateMovement(float dT) {
		float curMoveSpeed = walkSpeed;

		bool isWalkForward = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
		bool isWalkBack = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
		bool isWalkLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
		bool isWalkRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);

		isWalking = isWalkForward || isWalkBack || isWalkLeft || isWalkRight;

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

			Debug.DrawRay(
				this.gameObject.transform.position,
				moveVector*10,
				Color.red
			);
		}
	}
}