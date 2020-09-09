using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
	private BoxCollider2D groundCheckBoxCollider;
	public bool hasLanded;
	public bool hasLeftGround;
	
	void Start() {
		groundCheckBoxCollider = GetComponent<BoxCollider2D>();
		hasLanded = false;
		hasLeftGround = false;
	}

	public bool GetHasLanded() {
		return hasLanded;
	}

	public void SetHasLanded(bool b) {
		hasLanded = b;
	}

	public bool GetHasLeftGround() {
		return hasLeftGround;
	}

	public void SetHasLeftGround(bool b) {
		hasLeftGround = b;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.layer == LayerMask.NameToLayer ("Ground")) {
			hasLanded = true;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.layer == LayerMask.NameToLayer ("Ground")) {
			hasLeftGround = true;
		}
	}

	
}
