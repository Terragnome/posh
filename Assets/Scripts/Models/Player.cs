using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	const float GRAVITY = 9.8f;

	public float walkSpeed = 3f;
	public float turnSpeed = 7f;
	public float dashSpeed = 6f;
	public float dashDuration = 0.3f;
	float dashTimeLeft = 0f;
	bool isDashing = false;

	public float pushForce = 5f;

	public float liftDistanceSquared = Mathf.Pow(2f, 2);
	public float liftAngle = 60f;
	Portable liftTarget = null;
	bool isLifting = false;

	public float dropDistanceSquared = Mathf.Pow(3f, 2);

	public float useDistanceSquared = Mathf.Pow(2f, 2);
	public float useAngle = 60f;
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
				!curContainer.isFull
				&& DistanceSquared(curContainer.transform.position) <= dropDistanceSquared
				&& IsFacing(curContainer.transform.position, useAngle)
			){
				closestContainer = curContainer;
			}
        }
        return closestContainer;
	}

	Portable GetClosestPortable(float liftDistance) {
		Generator[] generators = Object.FindObjectsOfType<Generator>();
		Generator closestGenerator = null;
		float closestGeneratorDistSquared = liftDistanceSquared;
		foreach(Generator curGenerator in generators){
			float curGeneratorDistSquared = DistanceSquared(curGenerator.transform.position);
			if(
				curGeneratorDistSquared <= closestGeneratorDistSquared
				&& IsFacing(curGenerator.transform.position, liftAngle)
			){
				closestGenerator = curGenerator;
			}
		}

		Portable[] portables = Object.FindObjectsOfType<Portable>();
		Portable closestPortable = null;
		float closestPortableDistSquared = liftDistanceSquared;
		foreach(Portable curPortable in portables){
			float curPortableDistSquared = DistanceSquared(curPortable.transform.position);
			if(
				curPortableDistSquared <= closestPortableDistSquared
				&& IsFacing(curPortable.transform.position, liftAngle)
			){
				closestPortable = curPortable;
			}
    }

    if(
    	(
    		closestPortable == null
    		|| closestGeneratorDistSquared < closestPortableDistSquared
    	)
    	&& closestGenerator.CanGenerate
    ){
    	Portable newGenerated = closestGenerator.Generate();
    	if(newGenerated != null){
    		return newGenerated;
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
				Container closestContainer = GetClosestContainer(dropDistanceSquared);
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
					liftTarget = curTarget.Lift();
					isLifting = true;
				}
			}
		}

		if(liftTarget != null){
			Rigidbody liftRb = liftTarget.GetComponent<Rigidbody>();

			Quaternion rotationA = avatar.transform.rotation * Quaternion.Euler(0, 90, 0);
			float angleA = Quaternion.Angle(liftRb.transform.rotation, rotationA);

			Quaternion rotationB = avatar.transform.rotation * Quaternion.Euler(0, -90, 0);
			float angleB = Quaternion.Angle(liftRb.transform.rotation, rotationB);

			Quaternion targetRotation = angleA < angleB ? rotationA : rotationB;
			liftRb.transform.rotation = targetRotation;
			liftRb.MovePosition(avatarLiftPosition);
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
		fullMoveVector += Vector3.down*GRAVITY;

		GetComponent<CharacterController>().Move(fullMoveVector*dT);
	}
}