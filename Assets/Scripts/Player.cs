using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	[SerializeField] float movementSpeed = 7f;
	[SerializeField] float jumpSpeed = 7f;
	[SerializeField] int maxJumps = 2;
	[SerializeField] int jumpsAvailable;

	private Rigidbody2D rb;
	private BoxCollider2D boxCollider;

	// Start is called before the first frame update
	void Start() {
		rb = GetComponent<Rigidbody2D>();
		boxCollider = GetComponent<BoxCollider2D>();
		jumpsAvailable = maxJumps;
	}

	// Update is called once per frame
	void Update() {
		handleResetJumpsAvailable();
		handleJump();
		handleMovement();
	}

	private void handleResetJumpsAvailable() {
		if (isGrounded()) {
			jumpsAvailable = maxJumps;
		}
	}

	// Draws a ray from the origin of the player downwards
	// Returns whether or not the ray hits the ground and the player is not moving upward
	private bool isGrounded() {
		// TODO remove debug
		Debug.DrawRay(boxCollider.bounds.center, boxCollider.bounds.size / 2 * Vector2.down, Color.black);
		RaycastHit2D raycast = Physics2D.BoxCast(
			boxCollider.bounds.center,
			boxCollider.bounds.size / 2,
			0f,
			Vector2.down,
			1f,
			LayerMask.GetMask("Ground")
		);
		return raycast.collider != null && rb.velocity.y <= 0;
	}

	private void handleMovement() {
		Vector2 movementVector = new Vector2(Input.GetAxisRaw("Horizontal") * movementSpeed, rb.velocity.y);
		rb.velocity = movementVector;
	}

	private void handleJump() {
		if (jumpsAvailable <= 0) {
			return;
		} else if (Input.GetButtonDown("Jump")) {
			Vector2 jumpVelocity = new Vector2(rb.velocity.x, jumpSpeed);
			rb.velocity = jumpVelocity;
			jumpsAvailable--;
		}
	}
}