using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
	public bool isTouchingGround;
	public bool hasLeftGround;
	
	void Start() {
		isTouchingGround = false;
		hasLeftGround = false;
	}

	public bool GetIsTouchingGround() {
		return isTouchingGround;
	}

	public void SetIsTouchingGround(bool b) {
		isTouchingGround = b;
	}

	public bool GetHasLeftGround() {
		return hasLeftGround;
	}

	public void SetHasLeftGround(bool b) {
		hasLeftGround = b;
	}

	void OnTriggerStay2D(Collider2D other) {
		if (other.gameObject.layer == LayerMask.NameToLayer ("Ground")) {
			isTouchingGround = true;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.layer == LayerMask.NameToLayer ("Ground")) {
			isTouchingGround = false;
			hasLeftGround = true;
		}
	}

	
}
