using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// public class HitCheck : MonoBehaviourPunCallbacks

public class HitCheck : MonoBehaviour
{

	public Transform BloodParticleEffect;
	// Update is called once per frame
	void Update()
	{
	}

	private void OnTriggerStay2D(Collider2D collider) {

		// if (isRolling == true) {
		// 	Debug.Log("collision is ignored!");	
		// 	Physics.IgnoreCollision((Collider2D)collider, ObjectCollidingWithPlayer.GetComponent<Collider2D>());
		// }
	
		if (collider.gameObject.tag == "AttackHitBox" && collider.gameObject.tag != "LocalPlayer") {
			BloodParticleEffect.GetComponent<ParticleSystem>().Play();
		}
	}

}

