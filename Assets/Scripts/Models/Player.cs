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

	float useDistance = 100f;
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

	void UpdateUse(float dT){
		isUsing = Input.GetKey(KeyCode.Space);
		if(isUsing){
			if( useTarget == null){
				Debug.Log("Find Target");

			    Collider[] hitColliders = Physics.OverlapSphere(
			    	this.gameObject.transform.position,
			    	useDistance
			    );

		        for(int i =0; i<hitColliders.Length; i++) {
		        	Workstation curStation = hitColliders[i].gameObject.GetComponent(typeof(Workstation)) as Workstation;
		            Debug.Log( curStation );
		            i++;
		        }
			}else{

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