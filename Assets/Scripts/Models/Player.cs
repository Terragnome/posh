using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	float walkSpeed = 3f;
	bool isWalking = false;

	float turnSpeed = 7f;

	float dashSpeed = 6f;
	float dashDuration = 0.3f;
	float dashTimeLeft = 0f;
	bool isDashing = false;

	float useDistance = 1.25f;
	Workstation useTarget = null;
	bool isUsing = false;

  	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		float dT = Time.deltaTime;
		UpdateUse(dT);

		if(!isUsing){
			UpdateMovement(dT);
		}
	}

	bool CloseEnough(Vector3 position, float distance){
		Vector3 distVector = this.gameObject.transform.position-position;
		return distVector.sqrMagnitude <= distance*distance;
	}

	void UpdateUse(float dT){
		bool checkUse = Input.GetKey(KeyCode.Space);
		if(checkUse){
			if( useTarget != null ){				
				if(CloseEnough(useTarget.transform.position, useDistance)){
					useTarget.Use(dT);
				}else{
					useTarget = null;					
				}
			}

			if( useTarget == null){
				GameObject[] workstations = GameObject.FindGameObjectsWithTag("Workstation");

				foreach(GameObject curGO in workstations){
					if(CloseEnough(curGO.transform.position, useDistance)){
						Workstation curTarget = curGO.GetComponent(typeof(Workstation)) as Workstation;
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
			GameObject avatar = this.gameObject.transform.GetChild(0).gameObject;
			avatar.transform.rotation = Quaternion.Lerp(
				avatar.transform.rotation,
				Quaternion.LookRotation(moveVector*-1, Vector3.up),
				turnSpeed*dT
			);

			this.gameObject.transform.Translate(moveVector*curMoveSpeed*dT);

			Debug.DrawRay(
				this.gameObject.transform.position,
				moveVector*10,
				Color.red
			);
		}
	}
}