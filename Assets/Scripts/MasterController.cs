﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MasterController : MonoBehaviourPunCallbacks
{
	public Animator playerAnim;
	private Rigidbody2D playerRigidbody;
	private CapsuleCollider2D playerCapsuleCollider;
	public GameObject camera;
	public GameObject crosshair;
	public GameObject AttackHitBox;


	private float playerMoveX;
	private float savedPlayerVelocity;
	public bool isGrounded;
	private float dirFacing; 
	private static float offsetHalfGameScreen = 0.5f;
	private static float attackDelay = 0.2f;
	private static float attackDuration = 0.1f;

	[Header("[Setting]")]
	public float moveSpeed;
	public float walkBackSlowDownFactor;
	public float jumpForce;


	void Start()
	{
		playerCapsuleCollider  = this.GetComponent<CapsuleCollider2D>();
		playerAnim = this.transform.GetComponent<Animator>();
		playerRigidbody = this.transform.GetComponent<Rigidbody2D>();

		if (photonView.IsMine) {
			camera.SetActive(true);
			crosshair.SetActive(true);
		}
	}


	// Update is called once per frame
	void Update()
	{
		FaceCorrectDirection();
		AnimUpdate();
		MovementUpdate();
	}

	public void FaceCorrectDirection() {
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

	void AnimUpdate() {
		if (CheckIsAnimActive("Attack") || CheckIsAnimActive("Roll")) {
			return;
		}
		if (Input.GetKeyDown(KeyCode.Mouse0)) {
			playerAnim.Play("Attack");
		}
		if ((Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)) || (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))) {
			playerAnim.Play("Roll");
		}

		if (CheckIsAnimActive("Jump")) {
			return;
		}
		
		//Idle & run animation
		playerMoveX = Input.GetAxis("Horizontal");
		if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A)) {
			playerAnim.Play("Run");
		}
		if (playerMoveX == 0) {
			playerAnim.Play("Idle");
		}
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
		if (CheckIsAnimActive("Attack") || CheckIsAnimActive("Roll")) {
			if (playerMoveX == 0) {
				savedPlayerVelocity = 0;
			}
			transform.transform.Translate(new Vector3(savedPlayerVelocity * Time.deltaTime, 0, 0));
			return;
		}
		if (Input.GetKeyDown(KeyCode.Mouse0)) {
			ActivateAttackBox();
		}
		if ((Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.A)) || (Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.D))) {
			DeactivateCollider(); //make the collider temporarily transparent to other players' colliders
		}

		//jump
		if (isGrounded && Input.GetKeyDown(KeyCode.Space)) {
			playerRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
		}

		//Run right
		playerMoveX = Input.GetAxis("Horizontal");
		if (Input.GetKey(KeyCode.D)) {
			HandleMoveRight();
		}
		//Run left
		else if (Input.GetKey(KeyCode.A)) {
			HandleMoveLeft();
		}
	}

	IEnumerator ActivateAttackBox() {
		yield return new WaitForSeconds(attackDelay);
		AttackHitBox.SetActive(true);
		yield return new WaitForSeconds(attackDuration);
		AttackHitBox.SetActive(false);
	}

	IEnumerator DeactivateCollider() {
		yield return new WaitForSeconds(attackDelay);

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
