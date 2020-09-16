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
	private float EPSILON = 0.1;
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
			
			float addVeloctyX = CalculateAdditionalVelocities(playerRigidbody.velocity.x);
			float addVeloctyY = CalculateAdditionalVelocities(playerRigidbody.velocity.y);

			float x = kickAngle.x * dirFacing + addVeloctyX;
			float y = kickAngle.y + addVeloctyY;
			Vector2 kickDirection = new Vector2(x, y);
			other.gameObject.GetComponent<Rigidbody2D>().AddForce(kickDirection * kickStrengh, ForceMode2D.Impulse);
		}
	}

	private float CalculateAdditionalVelocities(float directionalVeloctiy) {
		float addVelocty = 0;
		if (directionalVeloctiy > EPSILON) {
			float addVelocty = 1;
		} 
		else if (directionalVeloctiy < -EPSILON) {
			float addVelocty = -1;
		}
	}
}
