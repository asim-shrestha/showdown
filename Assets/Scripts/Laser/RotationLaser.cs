using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationLaser : MonoBehaviour {
	[SerializeField] float initialRotationSpeed = 0.4f;
	[SerializeField] float rotationMultiplier = 0.05f;

	private Laser laser;

	private void Start() {
		laser = GetComponent<Laser>();
	}

	void FixedUpdate() {
		transform.Rotate(
			new Vector3(0, 0, initialRotationSpeed + laser.GetNumRuns() * rotationMultiplier) * laser.GetDirection()
		);
	}
}