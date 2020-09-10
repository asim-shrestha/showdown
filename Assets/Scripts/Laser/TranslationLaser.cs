using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslationLaser : MonoBehaviour {
	[SerializeField] float initialTranslationSpeed = 0.01f;
	[SerializeField] float translationMultiplier = 0.001f;

	[SerializeField] Collider2D endPoints;

	private Laser laser;

	private void Start() {
		laser = GetComponent<Laser>();
	}

	void FixedUpdate() {
		TranslateLaser();
	}

	private void TranslateLaser() {
		transform.Translate(
			new Vector3(initialTranslationSpeed + laser.GetNumRuns() * translationMultiplier, 0, 0) * laser.GetDirection()
		);
	}
}