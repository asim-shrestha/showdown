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
	[SerializeField] float jumpDecrementThresholdSpeed = -0.05f; //gives a buffer time when falling off a cliff before jump count will decrement
	[SerializeField] float wallSlideTriggerSpeed = 0.1f; //upward direction
	[SerializeField] float wallSlideSpeed = -0.1f; //downward direction
	public enum JumpState {
		LANDED,
		ONGROUND,
		INAIR,
		FALLING
	}
	[SerializeField] public JumpState playerJumpState;


	// GameObject instead of particle system to pass into a server command
	[SerializeField] private GameObject dustParticles;
	
	// Ground and wall checks are preformed with the following gameobjects.
	[SerializeField] private GameObject groundCheck;
	private GroundCheck groundCheckScript;
	[SerializeField] private GameObject LeftWallCheck;
	private WallCheck leftWallCheckScript;
	[SerializeField] private GameObject RightWallCheck;
	private WallCheck rightWallCheckScript;

	

	private Rigidbody2D rb;
	private BoxCollider2D boxCollider;

	// Start is called before the first frame update
	void Start() {
		rb = GetComponent<Rigidbody2D>();
		boxCollider = GetComponent<BoxCollider2D>();
		jumpsAvailable = maxJumps;
		playerJumpState = JumpState.FALLING;

		//load scripts from groundCheck and LeftWallCheck gameobjects
		groundCheckScript = groundCheck.GetComponent<GroundCheck>();
		leftWallCheckScript = LeftWallCheck.GetComponent<WallCheck>();
		rightWallCheckScript = RightWallCheck.GetComponent<WallCheck>();

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
			Debug.Log(playerJumpState);
			// the order of these operations is important due to them changing the "JumpState" of the player.
			// checkLanding();
			handleLanding(); // player can land either on the ground or the wall - both activate the same behaviour
			handleJump();
			handleWallSlide();
			handleMovement();
		}
	}

	private void handleLanding() {
		if (GetHasLanded()) {
			playerJumpState = JumpState.LANDED;
			SetHasLanded(false);
		} 
		if (playerJumpState == JumpState.LANDED) {
			// As soon as landing has been detected, it should perform the following lines once
			// and then immediately change the JumpState to ONGROUND, so these line won't run again.
			jumpsAvailable = maxJumps;
			CmdPlayDustParticles();
			playerJumpState = JumpState.ONGROUND;
		}
	}

	private void handleMovement() {
		Vector2 movementVector = new Vector2(Input.GetAxisRaw("Horizontal") * movementSpeed, rb.velocity.y);
		rb.velocity = movementVector;
	}

	private void handleJump() {
		if (jumpsAvailable <= 0) {
			return;
		}
		if (GetHasLeftGround()) {
			// If GroundCheck detects that player is no longer on firm ground and the velocity is decreasing,
			// then the player is falling rather than jumping.
			if (rb.velocity.y < jumpDecrementThresholdSpeed) {
				playerJumpState = JumpState.FALLING;
			}
			else {
				playerJumpState = JumpState.INAIR;
			}
			SetHasLeftGround(false);
		}
		if (GetHasPushedOffWall()) {
			if (rb.velocity.y < jumpDecrementThresholdSpeed + wallSlideSpeed) {
				playerJumpState = JumpState.FALLING;
			}
			else {
				playerJumpState = JumpState.INAIR;
			}
			SetHasPushedOffWall(false);
		}
		if (Input.GetButtonDown("Jump")) {
			Vector2 jumpVelocity = new Vector2(rb.velocity.x, jumpSpeed);
			rb.velocity = jumpVelocity;
			jumpsAvailable--;
			// The following lines ensure that when the player falls off the edge and tries to jump, his available jumps decrements twice.
			if (playerJumpState == JumpState.FALLING) {
				jumpsAvailable--;
				playerJumpState = JumpState.INAIR;
			}
		}
	}

	private void handleWallSlide() {
		// if player is moving left onto a left wall or moving right onto a right wall, wall sliding will activate.
		if ((GetHasTouchedLeftWall() && Input.GetAxisRaw("Horizontal") < 0) ||
			(GetHasTouchedRightWall() && Input.GetAxisRaw("Horizontal") > 0)) {
			if (rb.velocity.y < wallSlideTriggerSpeed) {
				playerJumpState = JumpState.LANDED;
				Debug.Log("Landing occurred...");
				CmdPlayDustParticles();
				if (rb.velocity.y < wallSlideSpeed) {
					Vector2 slideVelocity = new Vector2(rb.velocity.x, wallSlideSpeed);
					rb.velocity = slideVelocity;
				}
			}
		}
		// else if (GetHasTouchedLeftWall() || GetHasTouchedRightWall()) {
		// 	playerJumpState = JumpState.ONGROUND;
		// }

		// if (GetHasPushedOffWall() && rb.velocity.y < jumpDecrementThresholdSpeed) {
		// 	playerJumpState = JumpState.FALLING;
		// 	SetHasPushedOffWall(false);
		// }
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

	// checks if the small groundcheck collision box below player is triggered.
	private bool GetHasLanded() {
		return groundCheckScript.GetHasLanded();
	}
	
	private void SetHasLanded(bool b) {
		groundCheckScript.SetHasLanded(b);
	}

	private bool GetHasLeftGround() {
		return groundCheckScript.GetHasLeftGround();
	}

	private void SetHasLeftGround(bool b) {
		groundCheckScript.SetHasLeftGround(b);
	}

	private bool GetHasTouchedLeftWall() {
		return leftWallCheckScript.GetHasTouchedWall();
	}

	private bool GetHasTouchedRightWall() {
		return rightWallCheckScript.GetHasTouchedWall();
	}

	private void SetHasTouchedWall(bool b) {
		leftWallCheckScript.SetHasTouchedWall(b);
		rightWallCheckScript.SetHasTouchedWall(b);
	}

	private bool GetHasPushedOffWall() {
		return leftWallCheckScript.GetHasTouchedWall() || rightWallCheckScript.GetHasTouchedWall();
	}

	private void SetHasPushedOffWall(bool b) {
		rightWallCheckScript.SetHasPushedOffWall(b);
		leftWallCheckScript.SetHasPushedOffWall(b);
	}

}