using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour {
	[SerializeField] float movementSpeed = 7f;
	[SerializeField] float jumpSpeed = 7f;
	[SerializeField] int maxJumps = 2;
	[SerializeField] int jumpsAvailable;
	// GameObject instead of particle system to pass into a server command
	[SerializeField] private GameObject dustParticles;

	private Rigidbody2D rb;
	private BoxCollider2D boxCollider;

	// Start is called before the first frame update
	void Start() {
		rb = GetComponent<Rigidbody2D>();
		boxCollider = GetComponent<BoxCollider2D>();
		jumpsAvailable = maxJumps;
		ConnectClientToCamera();
		DisablePhysicsIfOtherPlayer();
	}

	private void ConnectClientToCamera() {
		// Ensure the player belongs to this client
		if (!hasAuthority) { return; }

		CinemachineStateDrivenCamera camera = FindObjectOfType<CinemachineStateDrivenCamera>();
		camera.Follow = transform;
		camera.LookAt = transform;
	}

	private void DisablePhysicsIfOtherPlayer() {
		// Kinematic rigidbodys do not undergo physics calculations
		// This disables physics if the player belongs to a different client
		// Doing this will remove jittery players but also remove basic player - player collision
		rb.isKinematic = !hasAuthority;
	}

	// Update is called once per frame
	void Update() {
		if (hasAuthority) {
			handleResetJumpsAvailable();
			handleJump();
			handleMovement();
		}
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
		float extraHeight = 0.1f; // Used so that the boxcast slightly extends past the plaer
		Debug.DrawRay(boxCollider.bounds.center, boxCollider.bounds.size / 2 * Vector2.down, Color.black);
		RaycastHit2D raycast = Physics2D.BoxCast(
			boxCollider.bounds.center,
			boxCollider.bounds.size,
			0f,
			Vector2.down,
			extraHeight,
			LayerMask.GetMask("Ground")
		);
		float velocityEpsilon = 0.01f;
		return raycast.collider != null && Mathf.Abs(rb.velocity.y) <= velocityEpsilon;
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
			if (jumpsAvailable == maxJumps) {
				CmdPlayDustParticles();
			}
			jumpsAvailable--;
		}
	}

	// Run on server so every player can see the dust particles
	[Command]
	private void CmdPlayDustParticles() {
		ClientPlayDustParticles();
	}

	[ClientRpc]
	private void ClientPlayDustParticles() {
		dustParticles.GetComponent<ParticleSystem>().Play();
	}
}