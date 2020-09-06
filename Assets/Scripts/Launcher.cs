using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class Launcher : MonoBehaviourPunCallbacks {
	[SerializeField] TMP_InputField roomNameInputField;
	// Start is called before the first frame update
	void Start() {
		PhotonNetwork.ConnectUsingSettings();
	}

	public override void OnConnectedToMaster() {
		Debug.Log("Connecting to master");
		PhotonNetwork.JoinLobby();
	}

	public override void OnJoinedLobby() {
		Debug.Log("Joined lobby");
		MenuManager.Instance.OpenMenu("title");
		base.OnJoinedLobby();
	}

	public void CreateRoom() {
		if (string.IsNullOrEmpty(roomNameInputField.text)) {
			Debug.LogError("No room name");
		}
		PhotonNetwork.CreateRoom(roomNameInputField.text);
		MenuManager.Instance.OpenMenu("loading");
	}

	public override void OnJoinedRoom() {

	}

	public override void OnCreateRoomFailed(short returnCode, string message) {
		base.OnCreateRoomFailed(returnCode, message);
	}
}