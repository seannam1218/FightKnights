using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Photon.Pun;

// public class HitCheck : MonoBehaviourPunCallbacks
public class HitCheck : MonoBehaviour

{
	// [SerializeField]
	// private GameObject ObjectGettingHit;

	// private GameObject Collider;

    // Start is called before the first frame update
    void Start()
    {
        // Collider = ObjectGettingHit.CapsuleCollider2D;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(Weapon.SpriteRenderer.color);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        //TODO: something wrong here! 
        //  if (!photonView.IsMine) {
        //     return;
        // }
        // Debug.Log("triggered....");	
    	if (collider.gameObject.tag == "AttackHitBox" && collider.gameObject != this.gameObject) {
    		Debug.Log("tag OUCH!");
    	}
    }
}

