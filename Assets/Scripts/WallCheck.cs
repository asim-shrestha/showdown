using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallCheck : MonoBehaviour
{
	private BoxCollider2D wallCheckBoxCollider;
	public bool isOnWall;

	void Start() {
		wallCheckBoxCollider = GetComponent<BoxCollider2D>();
		isOnWall = false;
	}

	public bool GetIsOnWall() {
		return isOnWall;
	}

	void OnTriggerEnter2D(Collider2D col) {
		isOnWall = true;
	}

	void OnTriggerExit2D(Collider2D col) {
		isOnWall = false;
	}
}
