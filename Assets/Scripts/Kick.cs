using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The Kick gameobject should be inactive at start of game.
public class Kick : MonoBehaviour
{
	private BoxCollider2D kickBoxCollider;
	[SerializeField] Vector2 kickAngle;
	[SerializeField] float kickStrengh;
	private float dirFacing;
	private Player player;
	private Rigidbody2D playerRigidbody;


	void Start() {
		kickBoxCollider = GetComponent<BoxCollider2D>();
		player = GetComponentInParent<Player>();
		playerRigidbody = player.GetComponent<Rigidbody2D>();
	}

	void OnTriggerEnter2D(Collider2D other) {
		if (other.gameObject.tag == "Grenade") {
			dirFacing = player.dirFacing;
			float x = kickAngle.x * dirFacing + playerRigidbody.velocity.x;
			Vector2 kickDirection = new Vector2(x, kickAngle.y + playerRigidbody.velocity.y);
			Debug.Log(kickDirection);
			other.gameObject.GetComponent<Rigidbody2D>().AddForce(kickDirection * kickStrengh, ForceMode2D.Impulse);
		}
	}
}
