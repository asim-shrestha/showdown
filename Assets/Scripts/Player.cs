using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour {
	[SerializeField] float movementSpeed = 7f;
	[SerializeField] float jumpSpeed = 13f;
	[SerializeField] int maxJumps = 2;
	[SerializeField] int jumpsAvailable;
	[SerializeField] float jumpDecrementThresholdVelocity = -0.05f; //when falling off a cliff, gives a buffer time before jump count will decrement
	[SerializeField] float wallSlideTriggerVelocity = 0.1f; //upward direction
	[SerializeField] float wallSlideVelocity = -0.1f; //downward direction
	bool isFalling;
	bool isGrounded;
	float dirFacing;

	// [SerializeField] private GameObject groundDustParticles;
	// [SerializeField] private GameObject wallDustParticles;

	// Ground and wall checks are performed with the following gameobjects.
	[SerializeField] private GameObject GroundCheck;
	private GroundCheck groundCheckScript;
	[SerializeField] private GameObject WallCheck;
	private WallCheck wallCheckScript;


	[SerializeField] private GameObject Animation;
	private PlayerAnimation animationScript;

	private Rigidbody2D rb;
	private BoxCollider2D boxCollider;
		

	// Start is called before the first frame update
	void Start() {
		rb = GetComponent<Rigidbody2D>();
		boxCollider = GetComponent<BoxCollider2D>();

		jumpsAvailable = maxJumps;
		isFalling = true;
		dirFacing = 1;

		//load scripts from groundCheck and WallCheck gameobjects
		groundCheckScript = GroundCheck.GetComponent<GroundCheck>();
		wallCheckScript = WallCheck.GetComponent<WallCheck>();
		animationScript = Animation.GetComponent<PlayerAnimation>();
		
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
			detectLanding();
			detectFalling();
			handleJump();
			handleWallSlide();
			handleMovement();
		}
	}

	private void detectLanding() {
		if (GetIsTouchingGround()) {
			if (!isGrounded) {
				CmdHandleGroundDustAnim();
				CmdHandleLandingAnim();
			}
			jumpsAvailable = maxJumps;
			isFalling = false;
			isGrounded = true;
			
		} 
	}

	private void detectFalling() {
		if ((!isFalling && GetHasLeftGround() && rb.velocity.y < jumpDecrementThresholdVelocity) ||
			(!isFalling && GetHasPushedOffWall() && rb.velocity.y < jumpDecrementThresholdVelocity+wallSlideVelocity)) {
			isFalling = true;
			isGrounded = false;
			SetHasLeftGround(false);
			SetHasPushedOffWall(false);
		}
		if (isFalling) {
			CmdHandleFallingAnim();
		}
	}

	private void handleMovement() {
		if (Input.GetAxisRaw("Horizontal") != 0) {
			Vector2 movementVector = new Vector2(Input.GetAxisRaw("Horizontal") * movementSpeed, rb.velocity.y);
			rb.velocity = movementVector;
			turnPlayer();
		}
		else {
			rb.velocity = new Vector2(0, rb.velocity.y);
		}
		CmdHandleMovementAnim();
	}

	private void turnPlayer() {
		dirFacing = Input.GetAxisRaw("Horizontal");
		transform.localScale = new Vector3(dirFacing, 1f, 1f);
	}

	private void handleJump() {
		if (jumpsAvailable <= 0) {
			return;
		}
		if (Input.GetButtonDown("Jump")) {
			Vector2 jumpVelocity = new Vector2(rb.velocity.x, jumpSpeed);
			rb.velocity = jumpVelocity;
			isGrounded = false;
			if (isFalling) {
				jumpsAvailable--;
				isFalling = false;
				if (jumpsAvailable <= 0) {
					return;
				}
			}
			jumpsAvailable--;
			CmdHandleJumpAnim();
			// animator.Play("PlayerJump");
		}
	}

	private void handleWallSlide() {
		// if player is moving  onto a  wall or moving right onto a right wall, wall sliding will activate.
		if (rb.velocity.y < wallSlideTriggerVelocity && !isGrounded) {
			if (GetIsTouchingWall() && Input.GetAxisRaw("Horizontal") == dirFacing) {
				// CmdPlayWallDustParticles();
				CmdHandleWallSlideAnim();
				isFalling = false;
				jumpsAvailable = 1;
				if (rb.velocity.y < wallSlideVelocity) {
					Vector2 slideVelocity = new Vector2(rb.velocity.x, wallSlideVelocity);
					rb.velocity = slideVelocity;
				}
			}
		}
	}

	[Command]
	private void CmdHandleLandingAnim() {
		ClientHandleLandingAnim();
	}

	[ClientRpc]
	private void ClientHandleLandingAnim() {
		animationScript.HandleLandingAnim();
	}

	[Command]
	private void CmdHandleGroundDustAnim() {
		ClientHandleGroundDustAnim();
	}

	[ClientRpc]
	private void ClientHandleGroundDustAnim() {
		animationScript.HandleGroundDustAnim();
	}

	[Command]
	private void CmdHandleFallingAnim() {
		ClientHandleFallingAnim();
	}

	[ClientRpc]
	private void ClientHandleFallingAnim() {
		animationScript.HandleFallingAnim();
	}

	[Command]
	private void CmdHandleMovementAnim() {
		ClientHandleMovementAnim();
	}

	[ClientRpc]
	private void ClientHandleMovementAnim() {
		animationScript.HandleMovementAnim();
	}

	[Command]
	private void CmdHandleJumpAnim() {
		ClientHandleJumpAnim();
	}

	[ClientRpc]
	private void ClientHandleJumpAnim() {
		animationScript.HandleJumpAnim();
	}

	[Command]
	private void CmdHandleWallSlideAnim() {
		ClientHandleWallSlideAnim();
	}

	[ClientRpc]
	private void ClientHandleWallSlideAnim() {
		animationScript.HandleWallSlideAnim();
	}


	// checks if the small groundcheck collision box below player is triggered.
	private bool GetIsTouchingGround() {
		return groundCheckScript.GetIsTouchingGround();
	}
	
	private void SetIsTouchingGround(bool b) {
		groundCheckScript.SetIsTouchingGround(b);
	}



	// private bool GetHasLanded() {
	// 	return groundCheckScript.GetHasLanded();
	// }
	
	// private void SetHasLanded(bool b) {
	// 	groundCheckScript.SetHasLanded(b);
	// }

	private bool GetHasLeftGround() {
		return groundCheckScript.GetHasLeftGround();
	}

	private void SetHasLeftGround(bool b) {
		groundCheckScript.SetHasLeftGround(b);
	}

	private bool GetIsTouchingWall() {
		return wallCheckScript.GetIsTouchingWall();
	}

	private void SetIsTouchingWall(bool b) {
		wallCheckScript.SetIsTouchingWall(b);
	}

	private bool GetHasPushedOffWall() {
		return wallCheckScript.GetHasPushedOffWall();
	}

	private void SetHasPushedOffWall(bool b) {
		wallCheckScript.SetHasPushedOffWall(b);
	}
}
