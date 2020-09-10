using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
	[SerializeField] float timeToExplode;
	[SerializeField] float randomBounceThrust;
	[SerializeField] float randomBounceStopSpeedThreshold;
	private float currentThrust;
	private bool isBlinking;

	private Rigidbody2D rb;
	private SpriteRenderer sr;

	Color grenadeColor;
	Color warningColor;

	// Start is called before the first frame update
	void Start()
	{
		grenadeColor = new Color(25, 145, 0);
		warningColor = new Color(168, 9, 2);

		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
		Vector2 direction = new Vector2(Random.Range(-3f,3f), 0);
		rb.AddForce(direction * 4f, ForceMode2D.Impulse);
	}

	// Update is called once per frame
	void Update()
	{
		if (Mathf.Abs(rb.velocity.y) <= randomBounceStopSpeedThreshold) {
			currentThrust = 0;
		} else {
			currentThrust = randomBounceThrust;
		}

		timeToExplode -= Time.deltaTime;
		if (timeToExplode <= 0) { 
			Destroy(gameObject);
		}
		if (timeToExplode < 1 && !isBlinking) {
			StartCoroutine(ChangeColor(0.25f));
			return;
		}
		if (timeToExplode < 3 && !isBlinking) {
			StartCoroutine(ChangeColor(0.5f));
			return;
		}
	}

	IEnumerator ChangeColor(float interval) {
		// Your code here…
		sr.color = warningColor;
		yield return new WaitForSeconds(interval);
		sr.color = grenadeColor;
		yield return new WaitForSeconds(interval);
	}


	void OnCollisionEnter2D(Collision2D other) {
		Vector2 direction = new Vector2(Random.Range(-3f,3f), Random.Range(0f,1f));
		rb.AddForce(direction * currentThrust, ForceMode2D.Impulse);
	}
}
