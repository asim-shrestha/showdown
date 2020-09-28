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

	// Update is called once per frame
	void Update()
	{
	}

	public void HandleLandingAnim() {
		animator.Play("PlayerIdle");
	}

	public void HandleGroundDustAnim() {
		groundDustParticles.GetComponent<ParticleSystem>().Play();
	}

	public void HandleFallingAnim() {
		animator.Play("PlayerFall");
	}

	public void HandleMovementAnim() {
		if (Input.GetAxisRaw("Horizontal") != 0) {
			if (!isAnimInAction()) {
				animator.Play("PlayerWalk");
			}
		}
		else {
			if (!isAnimInAction()) {
				animator.Play("PlayerIdle");
			}
		}
	}

	private bool isAnimInAction() {
		return animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerJump") || 
			animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerWallSlide") ||
			animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerFall");
			
		
	}

	public void HandleJumpAnim() {
		animator.Play("PlayerJump");
	}

	public void HandleWallSlideAnim() {
		wallDustParticles.GetComponent<ParticleSystem>().Play();
		animator.Play("PlayerWallSlide");
	}
 
}

