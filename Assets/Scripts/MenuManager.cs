using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour {
	[SerializeField] Menu[] menus;
	[SerializeField] Menu openMenu;
	public static MenuManager Instance;

	private void Awake() {
		Instance = this;
	}

	private void Start() {
		OpenMenu(openMenu);
	}

	public void OpenMenu(string menuName) {
		foreach (Menu menu in menus) {
			if (menu.menuName == menuName) {
				OpenMenu(menu);
				return;
			}
		}

		Debug.LogError("Could not find menu");
	}

	public void OpenMenu(Menu menu) {
		CloseMenu(openMenu);
		openMenu = menu;
		openMenu.Open();
	}

	public void CloseMenu(Menu menu) {
		menu.Close();
	}
}