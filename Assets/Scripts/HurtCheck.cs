using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// public class HurtCheck : MonoBehaviourPunCallbacks

public class HurtCheck : MonoBehaviour
{

	public Transform BloodParticleEffect;
	// Update is called once per frame
	void Update()
	{
	}

	private void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.tag == "Weapon") {
			Debug.Log(gameObject.name + " took damage!");
			BloodParticleEffect.GetComponent<ParticleSystem>().Play();
		}
	}

}

