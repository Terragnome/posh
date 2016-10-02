using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	float walkSpeed = 2f;

	float dashSpeed = 7f;
	float dashTime = 0.3f;
	bool isDashing = false;
	float dashTimeLeft = 0f;

  // Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
		float dT = Time.deltaTime;
		float curWalkSpeed = walkSpeed;

		bool isWalkForward = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
		bool isWalkBack = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);
		bool isWalkLeft = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
		bool isWalkRight = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);

		if( !isDashing ){
			bool hasDashed = Input.GetKeyDown(KeyCode.Space);
			if(hasDashed){
				isDashing = true;
				dashTimeLeft = dashTime;
			}
		}else{
			curWalkSpeed += dashSpeed;
			if(dashTimeLeft > 0){
				dashTimeLeft -= dT;
				if(dashTimeLeft <= 0){
					isDashing = false;
					dashTimeLeft = 0f;
				}
			}
		}

		if(isWalkBack && isWalkForward){
		}else if(isWalkBack){
			this.gameObject.transform.Translate(Vector3.back*dT*curWalkSpeed);
		}else if(isWalkForward){
			this.gameObject.transform.Translate(Vector3.forward*dT*curWalkSpeed);
		}

		if(isWalkLeft && isWalkRight){
		}else if(isWalkLeft){
			this.gameObject.transform.Translate(Vector3.left*dT*curWalkSpeed);
		}else if(isWalkRight){
			this.gameObject.transform.Translate(Vector3.right*dT*curWalkSpeed);
		}
	}
}