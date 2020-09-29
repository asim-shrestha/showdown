using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
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

	public void HandleLandingAnim() {
		animator.Play("PlayerIdle");
		groundDustParticles.GetComponent<ParticleSystem>().Play();
	}

	public void HandleFallingAnim() {
		if (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerJump") &&
			!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerFlip")) {
			animator.Play("PlayerFall");
		}
	}

	public void HandleMovementAnim() {
		if (!isAnimInAction()) {
			if (Input.GetAxisRaw("Horizontal") != 0) {
				animator.Play("PlayerWalk");
			}
			else {
				animator.Play("PlayerIdle");
			}
		}
	}

	public void FixMovementAnim() {
		if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerWalk")) {
			animator.Play("PlayerJump");
		}
	}

	private bool isAnimInAction() {
		return animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerJump") || 
			animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerWallSlide") ||
			animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerFall") ||
			animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerFlip");
	}

	public void HandleJumpingAnim() {
		animator.Play("PlayerJump");
	}

	public void HandleFlippingAnim() {
		animator.Play("PlayerFlip");
	}

	public void HandleWallSlidingAnim() {
		wallDustParticles.GetComponent<ParticleSystem>().Play();
		animator.Play("PlayerWallSlide");
	}
 
}

