using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class to handle GameObject dissolving
// Not an animation so that objects can dissolve in any state
public class Dissolve : MonoBehaviour {
	[SerializeField] bool isDissolving = false;
	private float fade = 1;
	private Material material;

	private void Start() {
		material = GetComponentInChildren<SpriteRenderer>().material;
	}

	private void Update() {
		if (isDissolving) {
			fade -= Time.deltaTime;
			if (fade <= 0f) {
				fade = 0f;
				isDissolving = false;
			}
			material.SetFloat("_Fade", fade);
		}
	}

	public void DissolveObject() {
		isDissolving = true;
	}
}