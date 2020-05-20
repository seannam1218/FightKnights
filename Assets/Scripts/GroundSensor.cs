using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GroundSensor : MonoBehaviourPunCallbacks {

	public Transform model;
	public MasterController masterController;

	void Start()
	{
		model = this.transform.root.Find("Model");
		masterController = model.GetComponent<MasterController>();
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (photonView.IsMine) {
			if (other.CompareTag("Ground") || other.CompareTag("Block") || other.CompareTag("Player")) {
				masterController.isGrounded = true;
				if (!masterController.CheckIsAnimActive("Attack") && !masterController.CheckIsAnimActive("Roll")) {
					photonView.RPC("playAnimPassFunc", RpcTarget.All, "Idle");
				}
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (photonView.IsMine) {
			if (other.CompareTag("Ground") || other.CompareTag("Block") || other.CompareTag("Player")) {
				masterController.isGrounded = false;
				if (!masterController.CheckIsAnimActive("Attack") && !masterController.CheckIsAnimActive("Roll")) {
					photonView.RPC("playAnimPassFunc", RpcTarget.All, "Jump");
				}
			}
		}
	}

	[PunRPC]
	public void playAnimPassFunc(string action) {
		masterController.playAnim(action);
	}
}
