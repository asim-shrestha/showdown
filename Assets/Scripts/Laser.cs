using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour {
	public LineRenderer lineRenderer;

	// Start is called before the first frame update
	void Start() {
		EnableLaser();
	}

	// Update is called once per frame
	void FixedUpdate() {
		transform.Rotate(new Vector3(0, 0, 1));
	}

	public void DisableLaser() {
		lineRenderer.enabled = false;
	}

	public void EnableLaser() {
		lineRenderer.enabled = true;
	}
}