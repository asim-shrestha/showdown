using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {
	public string menuName;

	// Close menues by default. Menu manager will handle opening and closing menus
	private void Awake() {
		Close();
	}

	public void Open() {
		gameObject.SetActive(true);

	}

	public void Close() {
		gameObject.SetActive(false);
	}
}