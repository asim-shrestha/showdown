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
	private float spawnMaximumX = 16;
	private float spawnY = 7f;

	
	// Start is called before the first frame update
	void Start() {
		spawnTimeLeft = spawnInterval;
	}

	// Update is called once per frame
	void Update() {
		spawnTimeLeft -= Time.deltaTime;
		// when time left expires, spawn grenade from top of the map at random x location.
		if (spawnTimeLeft <= 0) {
			float spawnX = Random.Range(spawnMinimumX, spawnMaximumX);
			if (isServer) {
				CmdSpawnGrenades(spawnX, spawnY);
			}
			spawnTimeLeft = spawnInterval;
		}
	}

	[Command(ignoreAuthority = true)]
	private void CmdSpawnGrenades(float spawnX, float spawnY) {
		Vector2 spawnPosition = new Vector2(spawnX, spawnY);
		GameObject grenadeInstance = Instantiate(grenade, spawnPosition, Quaternion.identity);
		NetworkServer.Spawn(grenadeInstance); // spawns grenade for every player in the network
	}
}