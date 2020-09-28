using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerAnimation : NetworkBehaviour
{
	// GameObject instead of particle system to pass into a server command
	[SerializeField] private GameObject groundDustParticles;
	[SerializeField] private GameObject wallDustParticles;

	private Animator animator;

	// Start is called before the first frame update
	void Start()
	{
		animator = GetComponent<Animator>();
		animator.Play("PlayerIdle");
	}

	// Update is called once per frame
	void Update()
	{
		
	}

	public void HandleLandingAnim() {
		if (hasAuthority) {
			CmdPlayGroundDustParticles();
			animator.Play("PlayerIdle");
		}
	}

	public void HandleMovementAnim() {
		if (Input.GetAxisRaw("Horizontal") != 0) {
			if (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerJump")) {
				animator.Play("PlayerWalk");
			}
		}
		else {
			if (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerJump")) {
				animator.Play("PlayerIdle");
			}
		}
	}

	public void HandleJumpAnim() {
		animator.Play("PlayerJump");
	}

	public void HandleWallSlideAnim() {
		if (hasAuthority) {
			CmdPlayWallDustParticles();
		}
	}
 
 
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

}

