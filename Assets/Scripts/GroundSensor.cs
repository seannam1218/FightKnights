using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GroundSensor : MonoBehaviour {

	public MasterController m_root;
	public PhotonView photonView;

	void Start()
	{
		m_root = this.transform.root.GetComponent<MasterController>();
		photonView = m_root.GetComponent<PhotonView>();
	}
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if (photonView.IsMine) {
			if (other.CompareTag("Ground") || other.CompareTag("Block") || other.CompareTag("Player")) {
				m_root.isGrounded = true;
				if (!m_root.CheckIsAnimActive("Attack") && !m_root.CheckIsAnimActive("Roll")) {
					photonView.RPC("playAnim", RpcTarget.All, "Idle");
				}
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if (photonView.IsMine) {
			m_root.isGrounded = false;
			if (!m_root.CheckIsAnimActive("Attack") && !m_root.CheckIsAnimActive("Roll")) {
				photonView.RPC("playAnim", RpcTarget.All, "Jump");
			}
		}
	}

	[PunRPC]
	void playAnim(string action) {
		m_root.playAnim(action);
	}

}
