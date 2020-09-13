using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class GrenadeSpawner : NetworkBehaviour
{
	[SerializeField] GameObject grenade;
	[SerializeField] float spawnInterval;
	private float spawnTimeLeft;
	private float spawnMinimumX = -9;
	private float spawnMaximumX = 27;
	private float spawnY = 7f;

	
	// Start is called before the first frame update
	void Start() {
		spawnTimeLeft = spawnInterval;
	}

	// Update is called once per frame
	void Update() {
		spawnTimeLeft -= Time.deltaTime;
		// when time left expires, spawn grenade from top of the map at random x location.
		if (hasAuthority && spawnTimeLeft <= 0) {
			CmdMakeGrenades();
		}
	}

	[Command]
	private void CmdMakeGrenades() {
		ClientMakeGrenades();
	}

	[ClientRpc]
	private void ClientMakeGrenades() {
		float spawnX = Random.Range(spawnMinimumX, spawnMaximumX);
		Vector2 spawnPosition = new Vector2(spawnX, spawnY);
		Instantiate(grenade, spawnPosition, Quaternion.identity);
		spawnTimeLeft = spawnInterval;
	}
}
