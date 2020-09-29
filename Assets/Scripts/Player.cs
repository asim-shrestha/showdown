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
			fixMovementAnim();
			detectLanding();
			detectFalling();
			handleJump();
			handleWallSlide();
			handleMovement();
		}
	}

	// There is a bug when an arrow key and jump is pressed simultaneously, the animation overrides to walking when it should be jumping. 
	// The following code fixes this issue to a large extent.
	private void fixMovementAnim() {
		if (!GetIsTouchingGround()) {
			CmdFixMovementAnim();
		}
	}

	// On every frame that player is touching the ground, reset the available jumps to max, set isFalling flag to false, and set isGrounded flag to true.
	// isGrounded flag is checked to play the landing animation on the first frame that player lands on ground. The flag is then immediately set to false to avoid replaying the animation.
	private void detectLanding() {
		if (GetIsTouchingGround()) {
			if (!isGrounded) {
				CmdHandleLandingAnim();
			}
			jumpsAvailable = maxJumps;
			isFalling = false;
			isGrounded = true;	
		} 
	}

	private void detectFalling() {
		// Two different animations visually signals to the player whether he/she has any jumps left.
		if (isFalling && jumpsAvailable >= 1) {
			CmdHandleJumpingAnim();
		} 
		else if (isFalling && jumpsAvailable < 1) {
			CmdHandleFallingAnim();
		}

		// Checks whether the isFalling flag should be set to true. IsFalling flag is checked while jumping to determine if available jumps need to be decremented.
		if ((!isFalling && GetHasLeftGround() && rb.velocity.y < jumpDecrementThresholdVelocity) ||
			(!isFalling && GetHasPushedOffWall() && rb.velocity.y < jumpDecrementThresholdVelocity+wallSlideVelocity)) {
			isFalling = true;
			isGrounded = false;
			SetHasLeftGround(false);
			SetHasPushedOffWall(false);
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

			// check if player is falling, if so, decrement available jumps and play flipping animation, then set isFalling to false.
			if (isFalling) {
				CmdHandleFlippingAnim();
				jumpsAvailable--;
				isFalling = false;
				if (jumpsAvailable <= 0) {
					return;
				}
			}
			
			// Choose which animation to play depending on how many jumps are available.
			if (jumpsAvailable == 2) {
				CmdHandleJumpingAnim();
			}
			else if (jumpsAvailable == 1) {
				CmdHandleFlippingAnim();
			}

			jumpsAvailable--;
		}
	}

	private void handleWallSlide() {
		// if player is moving into a wall, wall sliding will activate.
		if (rb.velocity.y < wallSlideTriggerVelocity && !isGrounded) {
			if (GetIsTouchingWall() && Input.GetAxisRaw("Horizontal") == dirFacing) {
				CmdHandleWallSlidingAnim();
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
	private void CmdFixMovementAnim() {
		ClientFixMovementAnim();
	}
	[ClientRpc]
	private void ClientFixMovementAnim() {
		animationScript.FixMovementAnim();
	}

	[Command]
	private void CmdHandleJumpingAnim() {
		ClientHandleJumpingAnim();
	}
	[ClientRpc]
	private void ClientHandleJumpingAnim() {
		animationScript.HandleJumpingAnim();
	}

	[Command]
	private void CmdHandleFlippingAnim() {
		ClientHandleFlippingAnim();
	}
	[ClientRpc]
	private void ClientHandleFlippingAnim() {
		animationScript.HandleFlippingAnim();
	}

	[Command]
	private void CmdHandleWallSlidingAnim() {
		ClientHandleWallSlidingAnim();
	}
	[ClientRpc]
	private void ClientHandleWallSlidingAnim() {
		animationScript.HandleWallSlidingAnim();
	}


	// checks if the small groundcheck collision box below player is triggered.
	private bool GetIsTouchingGround() {
		return groundCheckScript.GetIsTouchingGround();
	}
	
	private void SetIsTouchingGround(bool b) {
		groundCheckScript.SetIsTouchingGround(b);
	}

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
