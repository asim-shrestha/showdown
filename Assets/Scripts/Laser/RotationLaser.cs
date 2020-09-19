using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RotationLaser : Laser {
	[SerializeField] float initialRotationSpeed = 0.4f;
	[SerializeField] float rotationMultiplier = 0.05f;

	void FixedUpdate() {
		RotateLaser();
	}

	private void RotateLaser() {
		transform.Rotate(
			new Vector3(0, 0, initialRotationSpeed + numRuns * rotationMultiplier) * direction
		);
	}
}