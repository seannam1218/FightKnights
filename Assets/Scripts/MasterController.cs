using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MasterController : MonoBehaviourPunCallbacks
{
	public Animator playerAnim;
	private Rigidbody2D playerRigidbody;
	private CapsuleCollider2D playerCapsuleCollider;
	public GameObject weapon;
	public GameObject weaponCollider;
	public GameObject camera;
	public GameObject crosshair;
	public Transform BloodParticleEffect;

	private float playerMoveX;
	private float savedPlayerVelocity;
	public bool isGrounded;
	public bool isRolling;
	public bool isJumping;
	private float dirFacing; 

	private static float offsetHalfGameScreen = 0.5f;
	private static float attackDelay = 0.32f;
	private static float attackDuration = 0.2f;
	private static float rollDelay = 0.1f;
	private static float rollDuration = 0.3f;
	private static float jumpAdditionWindow = 0.3f;

	[Header("[Setting]")]
	public float moveSpeed;
	public float walkBackSlowDownFactor;
	public float initialJumpForce;
	public float additionalJumpForce;
	
	public float rollSpeedMultiplier;

	//Layers
	private int PLAYER = 8;
	private int LOCALPLAYER = 11;
	private int WEAPON = 12;
	private int PLAYERBACK = 13;

	void Start()
	{
		playerCapsuleCollider  = this.GetComponent<CapsuleCollider2D>();
		playerAnim = this.GetComponent<Animator>();
		playerRigidbody = this.GetComponent<Rigidbody2D>();
		weaponCollider = GameObject.Find("WeaponCollider");

		if (photonView.IsMine) {
			camera.SetActive(true);
			crosshair.SetActive(true);
			photonView.RPC("WeaponColliderSetActive", RpcTarget.All, false);

			SwitchLayerTo(LOCALPLAYER);
			Transform[] allChildren = GetComponentsInChildren<Transform>();
			foreach (Transform child in allChildren)
			{
				child.gameObject.layer = LOCALPLAYER;
			}
		}	
	}

	void Update() 
	{
		if (photonView.IsMine) {
			photonView.RPC("FaceCorrectDirection", RpcTarget.All);
			AnimUpdate();
			// photonView.RPC("MovementUpdate", RpcTarget.All);
			MovementUpdate();
		}
	}

	[PunRPC]
	public void FaceCorrectDirection() {
		if (photonView.IsMine) {
			if (CheckIsAnimActive("Attack") || CheckIsAnimActive("Roll")) {
				return;
			}
			dirFacing = Input.mousePosition.x/Screen.width - offsetHalfGameScreen;
			if (dirFacing > 0) {
				transform.localScale = new Vector3(-1, 1, 1);
			} else if (dirFacing <= 0) {
				transform.localScale = new Vector3(1, 1, 1);
			}
		}
	}

	void AnimUpdate() {
		if (photonView.IsMine) {
			if (CheckIsAnimActive("Attack") || CheckIsAnimActive("Roll")) {
				return;
			}
			if (Input.GetKey(KeyCode.Mouse0)) {
				photonView.RPC("playAnim", RpcTarget.All, "Attack");

			}
			if ((Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)) || (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))) {
				photonView.RPC("playAnim", RpcTarget.All, "Roll");
			}

			if (CheckIsAnimActive("Jump")) {
				return;
			}
			
			//Idle & run animation
			playerMoveX = Input.GetAxis("Horizontal");
			if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)) {
				photonView.RPC("playAnim", RpcTarget.All, "Run");
			}
			if (playerMoveX == 0) {
				photonView.RPC("playAnim", RpcTarget.All, "Idle");
			}
		}
	}

	[PunRPC]
	public void playAnim(string action) {
		playerAnim.Play(action);
	}

	public bool CheckIsAnimActive(string action) {
		if (playerAnim.GetCurrentAnimatorStateInfo(0).IsName(action)) {
			return true;
		} else {
			return false;
		}
	}

	void MovementUpdate() 
	{
		if (photonView.IsMine) {
			if (CheckIsAnimActive("Roll")) {
				photonView.RPC("HandleRoll", RpcTarget.All);
				return;
			}

			playerMoveX = Input.GetAxis("Horizontal");
			//Run right
			if (Input.GetKey(KeyCode.D)) {
				photonView.RPC("HandleMoveRight", RpcTarget.All);
			}
			//Run left
			else if (Input.GetKey(KeyCode.A)) {
				photonView.RPC("HandleMoveLeft", RpcTarget.All);
			}

			if (CheckIsAnimActive("Attack")) {
				return; 
			}

			if (Input.GetKey(KeyCode.Mouse0)) {
				StartCoroutine(ActivateHitBox());
			}
			if ((Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)) || (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))) {
				StartCoroutine(NegateCollider()); //make the collider temporarily transparent to other players' colliders
			}
			//jump
			if (isGrounded && Input.GetKeyDown(KeyCode.Space)) {
				photonView.RPC("AddJumpForce", RpcTarget.All, initialJumpForce);	
				StartCoroutine(ToggleIsJumping());
			}
			if (isJumping && Input.GetKey(KeyCode.Space)) {
				photonView.RPC("AddJumpForce", RpcTarget.All, additionalJumpForce);	
			}
		}
	}

	IEnumerator ActivateHitBox() {
		yield return new WaitForSeconds(attackDelay);
		photonView.RPC("WeaponColliderSetActive", RpcTarget.All, true);
		yield return new WaitForSeconds(attackDuration);
		photonView.RPC("WeaponColliderSetActive", RpcTarget.All, false);
	}

	[PunRPC]
	void WeaponColliderSetActive(Boolean val) {
		weaponCollider.SetActive(val);
	}

	IEnumerator NegateCollider() {
		yield return new WaitForSeconds(rollDelay);
		photonView.RPC("SwitchLayerTo", RpcTarget.All, 13);
		yield return new WaitForSeconds(rollDuration);
		photonView.RPC("SwitchLayerTo", RpcTarget.All, 11);
	}

	[PunRPC]
	void SwitchLayerTo(int i) {
		Debug.Log("switching to layer " + i);
		gameObject.layer = i;
	}

	[PunRPC]
	void HandleRoll() {
		if (playerMoveX == 0) {
			savedPlayerVelocity = 0;
		}
		transform.transform.Translate(new Vector3(savedPlayerVelocity * rollSpeedMultiplier * Time.deltaTime, 0, 0));
	}

	[PunRPC]
	void HandleMoveRight() {
		//if walking forward, walk in normal speed
		if (dirFacing >= 0) {
				savedPlayerVelocity = playerMoveX * moveSpeed;
				transform.transform.Translate(new Vector3(savedPlayerVelocity * Time.deltaTime, 0, 0));
			} 
		//if walking backwards, walk in slower speed
		else {
			savedPlayerVelocity = playerMoveX * walkBackSlowDownFactor * moveSpeed;
			transform.transform.Translate(new Vector3(savedPlayerVelocity * Time.deltaTime, 0, 0));
		}
	}

	[PunRPC]
	void HandleMoveLeft() {
		//if walking forward, walk in normal speed
		if (dirFacing < 0) {
			savedPlayerVelocity = playerMoveX * moveSpeed;
			transform.transform.Translate(new Vector3(playerMoveX * moveSpeed * Time.deltaTime, 0, 0));
		} 
		//if walking backwards, walk in slower speed
		else {
			savedPlayerVelocity = playerMoveX * walkBackSlowDownFactor * moveSpeed;
			transform.transform.Translate(new Vector3(savedPlayerVelocity * Time.deltaTime, 0, 0));
		}
	}

	IEnumerator ToggleIsJumping() {
		photonView.RPC("IsJumpingSetBoolean", RpcTarget.All, true);
		yield return new WaitForSeconds(jumpAdditionWindow);
		photonView.RPC("IsJumpingSetBoolean", RpcTarget.All, false);
	}

	[PunRPC]
	void IsJumpingSetBoolean(Boolean val) {
		isJumping = val;
	}


	[PunRPC]
	void AddJumpForce(float force) {
		playerRigidbody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
	}

	[PunRPC]
	public void ToggleIsGrounded() {
		BloodParticleEffect.GetComponent<ParticleSystem>().Play();
	}


	[PunRPC]
	public void Bleed() {
		BloodParticleEffect.GetComponent<ParticleSystem>().Play();
	}

}
