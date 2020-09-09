using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour
{
	private BoxCollider2D wallCheckBoxCollider;
	public bool hasTouchedWall;
	public bool hasPushedOffWall;

	void Start() {
		wallCheckBoxCollider = GetComponent<BoxCollider2D>();
		hasTouchedWall = false;
		hasPushedOffWall = false;
	}

	public bool GetHasTouchedWall() {
		return hasTouchedWall;
	}

	public void SetHasTouchedWall(bool b) {
		hasTouchedWall = b;
	}

	public bool GetHasPushedOffWall() {
		return hasPushedOffWall;
	}

	public void SetHasPushedOffWall(bool b) {
		hasPushedOffWall = b;
	}


	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.layer == LayerMask.NameToLayer ("Ground")) {
			hasTouchedWall = true;
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.layer == LayerMask.NameToLayer ("Ground")) {
			hasTouchedWall = false;
			hasPushedOffWall = true;
		}
	}
}
