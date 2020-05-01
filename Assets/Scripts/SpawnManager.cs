using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class SpawnManager : MonoBehaviour
{

	public string playerPrefab;
	public Transform spawnPoint;

    // Start is called before the first frame update
    void Start() {
    	Spawn();
    }

    public void Spawn() {
    	PhotonNetwork.Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
    }

}
