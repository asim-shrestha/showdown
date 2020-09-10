using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslationLaserBound : MonoBehaviour {
	[SerializeField] int nextLaserDirection = 1;
	// Flip the direction of translation lasers when they hit this bound
	private void OnTriggerEnter2D(Collider2D other) {
		if (other.GetComponent<TranslationLaser>()) {
			other.GetComponent<Laser>().SetDirection(nextLaserDirection);
		}
	}
}