using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
	[SerializeField] float timeToExplode;
	[SerializeField] float blinkDuration;
	[SerializeField] float randomBounceThrust;
	[SerializeField] float randomBounceStopSpeedThreshold;
	
	float timeLeftToExplode;
	float blinkInterval;
	private float currentThrust;
	private bool isBlinking;
	private float EPSILON = 0.05f;

	private Rigidbody2D rb;
	private SpriteRenderer sr;

	Color grenadeColor;
	Color warningColor;
	Color flashColor;

	// Start is called before the first frame update
	void Start()
	{
		grenadeColor = new Color(0.1f, 0.6f, 0f, 1f);
		warningColor = new Color(0.7f, 0.1f, 0f, 1f);
		flashColor = new Color(1f, 1f, 1f, 1f);

		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
		Vector2 direction = new Vector2(Random.Range(-3f,3f), 0);
		rb.AddForce(direction * 4f, ForceMode2D.Impulse);

		sr.color = grenadeColor;
		timeLeftToExplode = timeToExplode;
		blinkInterval = timeToExplode;
	}

	// Update is called once per frame
	void Update()
	{
		if (Mathf.Abs(rb.velocity.y) <= randomBounceStopSpeedThreshold) {
			currentThrust = 0;
		} else {
			currentThrust = randomBounceThrust;
		}

		// Destroy game object when time is up. Proceed to exploding.
		timeLeftToExplode -= Time.deltaTime;
		if (timeLeftToExplode <= 0) { 
			Destroy(gameObject);
		}

		changeBlinkInterval(5f, 0.7f);
		changeBlinkInterval(2f, 0.15f);
		
		if (timeLeftToExplode % blinkInterval > -EPSILON && timeLeftToExplode % blinkInterval < EPSILON) {
			StartCoroutine(Blink());
		}
	}

	void changeBlinkInterval(float timeLeft, float interval) {
		if (timeLeftToExplode < timeLeft) {
			blinkInterval = interval;
		}
	}

	IEnumerator Blink() {
		sr.color = warningColor;
		yield return new WaitForSeconds(blinkDuration);
		sr.color = grenadeColor;
	}

	void OnCollisionEnter2D(Collision2D other) {
		Vector2 direction = new Vector2(Random.Range(-3f,3f), Random.Range(0f,1f));
		rb.AddForce(direction * currentThrust, ForceMode2D.Impulse);
	}
}
