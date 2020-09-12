using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour
{
	private BoxCollider2D wallCheckBoxCollider;
	public bool isTouchingWall;
	public bool hasPushedOffWall;

	void Start() {
		wallCheckBoxCollider = GetComponent<BoxCollider2D>();
		isTouchingWall = false;
		hasPushedOffWall = false;
	}

	public bool GetIsTouchingWall() {
		return isTouchingWall;
	}

	public void SetIsTouchingWall(bool b) {
		isTouchingWall = b;
	}

	public bool GetHasPushedOffWall() {
		return hasPushedOffWall;
	}

	public void SetHasPushedOffWall(bool b) {
		hasPushedOffWall = b;
	}


	void OnTriggerStay2D(Collider2D other) {
		if (other.gameObject.layer == LayerMask.NameToLayer ("Ground")) {
			isTouchingWall = true;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.layer == LayerMask.NameToLayer ("Ground")) {
			isTouchingWall = false;
			hasPushedOffWall = true;
		}
	}
}
