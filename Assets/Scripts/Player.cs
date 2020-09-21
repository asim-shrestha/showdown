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
	public float dirFacing;
	bool kickIsActive;

	// GameObject instead of particle system to pass into a server command
	[SerializeField] private GameObject groundDustParticles;
	[SerializeField] private GameObject wallDustParticles;
	
	// Ground and wall checks are performed with the following gameobjects.
	[SerializeField] private GameObject GroundCheck;
	private GroundCheck groundCheckScript;
	[SerializeField] private GameObject WallCheck;
	private WallCheck wallCheckScript;
	[SerializeField] private GameObject Kick;

	private Rigidbody2D rb;
	private BoxCollider2D boxCollider;

	// Start is called before the first frame update
	void Start() {
		rb = GetComponent<Rigidbody2D>();
		boxCollider = GetComponent<BoxCollider2D>();
		jumpsAvailable = maxJumps;
		isFalling = true;
		dirFacing = 1;
		kickIsActive = false;

		//load scripts from groundCheck and WallCheck gameobjects
		groundCheckScript = GroundCheck.GetComponent<GroundCheck>();
		wallCheckScript = WallCheck.GetComponent<WallCheck>();

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
			handleClick();
		}
	}

	private void detectLanding() {
		if (GetHasLanded()) {
			jumpsAvailable = maxJumps;
			CmdPlayGroundDustParticles();
			SetHasLanded(false);
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
	}

	private void handleMovement() {
		Vector2 movementVector = new Vector2(Input.GetAxisRaw("Horizontal") * movementSpeed, rb.velocity.y);
		rb.velocity = movementVector;
		turnPlayer();
	}

	private void turnPlayer() {
		if (Input.GetAxisRaw("Horizontal") == 1f) {
			dirFacing = 1f;
		} 
		else if (Input.GetAxisRaw("Horizontal") == -1f) {
			dirFacing = -1f;
		}
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
			isGrounded = false;
			jumpsAvailable--;
		}
	}

	private void handleWallSlide() {
		// if player is moving  onto a  wall or moving right onto a right wall, wall sliding will activate.
		if (rb.velocity.y < wallSlideTriggerVelocity && !isGrounded) {
			if (GetIsTouchingWall() && Input.GetAxisRaw("Horizontal") == dirFacing) {
				CmdPlayWallDustParticles();
				isFalling = false;
				jumpsAvailable = 1;
				if (rb.velocity.y < wallSlideVelocity) {
					Vector2 slideVelocity = new Vector2(rb.velocity.x, wallSlideVelocity);
					rb.velocity = slideVelocity;
				}
			}
		}
	}

	private void handleClick() {
		if (Input.GetMouseButtonDown(0) && !kickIsActive) {
			kickIsActive = true;
			Kick.SetActive(true);
			StartCoroutine(KickReset());
		}
	}

	IEnumerator KickReset() {
		yield return new WaitForSeconds(0.1f);
		kickIsActive = false;
		Kick.SetActive(false);
	}


	// Run on server so every player can see the dust particles
	[Command]
	private void CmdPlayGroundDustParticles() {
		ClientPlayGroundDustParticles();
	}

	[ClientRpc]
	private void ClientPlayGroundDustParticles() {
		groundDustParticles.GetComponent<ParticleSystem>().Play();
	}

	[Command]
	private void CmdPlayWallDustParticles() {
		ClientPlayWallDustParticles();
	}

	[ClientRpc]
	private void ClientPlayWallDustParticles() {
		wallDustParticles.GetComponent<ParticleSystem>().Play();
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
