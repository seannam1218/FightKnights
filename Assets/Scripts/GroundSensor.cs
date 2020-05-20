using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GroundSensor : MonoBehaviourPunCallbacks {

	public MasterController m_root;

	void Start()
	{
		m_root = this.transform.root.GetComponent<MasterController>();
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		Debug.Log("trigger entered!");
		if (photonView.IsMine) {
			if (other.CompareTag("Ground") || other.CompareTag("Block") || other.CompareTag("Player")) {
				m_root.isGrounded = true;
				if (!m_root.CheckIsAnimActive("Attack") && !m_root.CheckIsAnimActive("Roll")) {
					photonView.RPC("playAnimPassFunc", RpcTarget.All, "Idle");
				}
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		Debug.Log("trigger exited!");
		if (photonView.IsMine) {
			if (other.CompareTag("Ground") || other.CompareTag("Block") || other.CompareTag("Player")) {
				m_root.isGrounded = false;
				if (!m_root.CheckIsAnimActive("Attack") && !m_root.CheckIsAnimActive("Roll")) {
					photonView.RPC("playAnimPassFunc", RpcTarget.All, "Jump");
				}
			}
		}
	}

	[PunRPC]
	public void playAnimPassFunc(string action) {
		m_root.playAnim(action);
	}
}
