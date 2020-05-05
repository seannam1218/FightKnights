using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSensor : MonoBehaviour {

	public MasterController m_root;

	void Start()
	{
		m_root = this.transform.root.GetComponent<MasterController>();
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Ground") || other.CompareTag("Block") || other.CompareTag("Player")) {
			m_root.isGrounded = true;
			if (!m_root.CheckIsAnimActive("Attack") && !m_root.CheckIsAnimActive("Roll")) {
				m_root.playerAnim.Play("Idle");
			}
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		m_root.isGrounded = false;
		if (!m_root.CheckIsAnimActive("Attack") && !m_root.CheckIsAnimActive("Roll")) {
			m_root.playerAnim.Play("Jump");
		}
	}

}
