using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Laser : NetworkBehaviour {
	[SerializeField] ParticleSystem[] LaserParticles;
	[SerializeField] float secondsTillDisable = 5;
	[SerializeField] int numRuns = 0;
	[SerializeField] int direction = 1;
	[SerializeField][Range(0, 100)] int directionFlipPercent = 20; // Percent chance to change direction on disable

	private BoxCollider2D boxCollider;
	private Animator animator;

	void Start() {
		boxCollider = GetComponent<BoxCollider2D>();
		animator = GetComponent<Animator>();
		DisableLaser();
	}

	// Called via animation. After the laser has been disabled, the opening animation will again play
	public void EnableLaser() {
		numRuns++;
		EnableParticles();
		boxCollider.enabled = true;
		StartCoroutine(WaitAndDisableLaser());
	}

	IEnumerator WaitAndDisableLaser() {
		yield return new WaitForSeconds(secondsTillDisable);
		numRuns++;
		animator.SetTrigger("DisableLaser");
		DisableLaser();
	}

	// Called via the start animation
	public void DisableLaser() {
		DisableParticles();
		boxCollider.enabled = false;
		RandomlyFlipDirection();
	}

	private void DisableParticles() {
		foreach (ParticleSystem particleSystem in LaserParticles) {
			particleSystem.Stop();
		}
	}

	private void EnableParticles() {
		foreach (ParticleSystem particleSystem in LaserParticles) {
			particleSystem.Play();
		}
	}

	public int GetNumRuns() {
		return numRuns;
	}

	public int GetDirection() {
		return direction;
	}

	public void RandomlyFlipDirection() {
		if (directionFlipPercent >= Random.Range(0, 100)) {
			direction *= -1;
		}
	}

	public void SetDirection(int direction) {
		this.direction = direction;
	}
}