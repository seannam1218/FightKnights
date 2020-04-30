using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MainMenu : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable() {
    	PhotonNetwork.AutomaticallySyncScene = true;
    	Connect();
    }

    void Connect() {
    	PhotonNetwork.GameVersion = "0.0.0";
 		PhotonNetwork.ConnectUsingSettings();
    }

    void Join() {

    }

    void StartGame() {

    }

}
