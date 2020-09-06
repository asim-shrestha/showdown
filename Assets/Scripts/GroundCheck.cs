using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
	[SerializeField] private ParticleSystem dust;
	private bool notYetLanded;

	// Start is called before the first frame update
	void Start()
	{
		notYetLanded = true;
	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnTriggerEnter(Collider other) {
		Debug.Log("An object entered.");
	}
}
