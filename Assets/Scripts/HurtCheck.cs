using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HurtCheck : MonoBehaviour
{
	public Transform BloodParticleEffect;
	public PhotonView photonView;


	// Update is called once per frame
	void Start()
	{
		photonView = gameObject.GetComponent<PhotonView>();
	}

	private void OnTriggerEnter2D(Collider2D other) {
		if (PhotonNetwork.LocalPlayer.IsLocal) {
			Debug.Log(gameObject.name + " took damage!");
			if (other.gameObject.tag == "Weapon") {
				// photonView.RPC("Bleed", RpcTarget.All);
				Bleed();
			}
			// 
			// 
		}
	}

	[PunRPC]
	private void Bleed() {
		BloodParticleEffect.GetComponent<ParticleSystem>().Play();
	}
}

