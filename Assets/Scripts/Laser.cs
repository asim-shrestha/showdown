using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Laser : NetworkBehaviour {
	public LineRenderer lineRenderer;
	public ParticleSystem[] LaserParticles;
	private BoxCollider2D boxCollider;

	// Disable on start. Will be enabled via animation
	void Start() {
		boxCollider = GetComponent<BoxCollider2D>();
	}

	// Update is called once per frame
	void FixedUpdate() {
		transform.Rotate(new Vector3(0, 0, 1));
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

	// Called via animations
	public void DisableLaser() {
		DisableParticles();
		boxCollider.enabled = false;
	}

	public void EnableLaser() {
		EnableParticles();
		boxCollider.enabled = true;
	}
}