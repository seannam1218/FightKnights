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

	private float playerMoveX;
	private float savedPlayerVelocity;
	public bool isGrounded;
	public bool isRolling;
	private float dirFacing; 

	private static float offsetHalfGameScreen = 0.5f;
	private static float attackDelay = 0.32f;
	private static float attackDuration = 0.2f;
	private static float rollDelay = 0.1f;
	private static float rollDuration = 0.3f;

	[Header("[Setting]")]
	public float moveSpeed;
	public float walkBackSlowDownFactor;
	public float jumpForce;
	public float rollSpeedMultiplier;


	void Start()
	{
		playerCapsuleCollider  = this.GetComponent<CapsuleCollider2D>();
		playerAnim = this.GetComponent<Animator>();
		playerRigidbody = this.GetComponent<Rigidbody2D>();
		weaponCollider = GameObject.Find("WeaponCollider");
		weaponCollider.SetActive(false);

		if (photonView.IsMine) {
			camera.SetActive(true);
			crosshair.SetActive(true);
		}	
	}

	void Update() 
	{
		if (photonView.IsMine) {
			photonView.RPC("FaceCorrectDirection", RpcTarget.All);
			AnimUpdate();
			photonView.RPC("MovementUpdate", RpcTarget.All);
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

	[PunRPC]
	void MovementUpdate() 
	{
		if (photonView.IsMine) {
			if (CheckIsAnimActive("Roll")) {
				if (playerMoveX == 0) {
					savedPlayerVelocity = 0;
				}
				transform.transform.Translate(new Vector3(savedPlayerVelocity * rollSpeedMultiplier * Time.deltaTime, 0, 0));
				return;
			}

			playerMoveX = Input.GetAxis("Horizontal");
			//Run right
			if (Input.GetKey(KeyCode.D)) {
				HandleMoveRight();
			}
			//Run left
			else if (Input.GetKey(KeyCode.A)) {
				HandleMoveLeft();
			}

			if (CheckIsAnimActive("Attack")) {
				return; 
			}

			if (Input.GetKey(KeyCode.Mouse0)) {
				StartCoroutine(ActivateHitBox());
			}
			if ((Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)) || (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))) {
				StartCoroutine(DeactivateCollider()); //make the collider temporarily transparent to other players' colliders
			}
			//jump
			if (isGrounded && Input.GetKeyDown(KeyCode.Space)) {
				playerRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
			}
		}
	}

	IEnumerator ActivateHitBox() {
		yield return new WaitForSeconds(attackDelay);
		weaponCollider.SetActive(true);
		yield return new WaitForSeconds(attackDuration);
		weaponCollider.SetActive(false);
	}

	//TODO: implement this function!
	IEnumerator DeactivateCollider() {
		yield return new WaitForSeconds(rollDelay);
		gameObject.layer = 13;
		yield return new WaitForSeconds(rollDuration);
		gameObject.layer = 11;
	}

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


}
