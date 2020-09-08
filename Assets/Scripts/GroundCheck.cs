using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
	private BoxCollider2D groundCheckBoxCollider;
	public bool hasLanded;
	public bool isOnGround;
	public enum JumpState {
		LANDED,
		ONGROUND,
		INAIR,
		FALLING
	}
	public JumpState playerJumpState;

	void Start() {
		groundCheckBoxCollider = GetComponent<BoxCollider2D>();
		playerJumpState = JumpState.INAIR;
	}

	public JumpState GetJumpState() {
		return playerJumpState;
	}

	public void SetJumpState(JumpState js) {
		playerJumpState = js;
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.layer == LayerMask.NameToLayer ("Ground")) {
			SetJumpState(JumpState.LANDED);
		}
	}

	void OnTriggerExit2D(Collider2D other) {
		if (other.gameObject.layer == LayerMask.NameToLayer ("Ground")) {
			SetJumpState(JumpState.FALLING);
		}
	}
}
