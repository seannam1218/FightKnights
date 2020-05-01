using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Launcher : MonoBehaviourPunCallbacks
{
 
    public void Awake() {
    	PhotonNetwork.AutomaticallySyncScene = true;
    	Connect();
    }

    //gets called when a player connects to photon server
    public override void OnConnectedToMaster() {
    	Join();
    	base.OnConnectedToMaster();
    }

    public override void OnJoinedRoom() {
    	StartGame();
    	base.OnJoinedRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message) {
    	Create();
    	base.OnJoinRandomFailed(returnCode, message);
    }

    public void Connect() {
    	PhotonNetwork.GameVersion = "0.0.0";
 		PhotonNetwork.ConnectUsingSettings();
    }

    public void Join() {
    	PhotonNetwork.JoinRandomRoom();
    }

    public void Create() {
    	PhotonNetwork.CreateRoom("");
    }

    public void StartGame() {
    	if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
    		PhotonNetwork.LoadLevel(1); //1 is for the game scene
    	}
    }

}
