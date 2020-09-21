using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class TranslationLaser : Laser {
	[SerializeField] float initialTranslationSpeed = 0.01f;
	[SerializeField] float translationMultiplier = 0.001f;
	[SerializeField] Collider2D endPoints;

	void FixedUpdate() {
		if(isServer) {
			TranslateLaser();
		}
	}

	private void TranslateLaser() {
		transform.Translate(
			new Vector3(initialTranslationSpeed + numRuns * translationMultiplier, 0, 0) * direction
		);
	}
}