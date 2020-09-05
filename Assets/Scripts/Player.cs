using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	[SerializeField] float movementSpeed = 7f;
	[SerializeField] float jumpSpeed = 7f;

	private Rigidbody2D rb;
	// Start is called before the first frame update
	void Start() {
		rb = GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update() {
		handleMovement();
		handleJump();
	}

	private void handleMovement() {
		Vector2 movementVector = new Vector2(Input.GetAxisRaw("Horizontal") * movementSpeed, rb.velocity.y);
		rb.velocity = movementVector;
	}

	private void handleJump() {
		if (Input.GetButtonDown("Jump")) {
			Vector2 jumpVelocity = new Vector2(rb.velocity.x, jumpSpeed);
			rb.velocity = jumpVelocity;
		}
	}
}