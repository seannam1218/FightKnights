using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public abstract class PlayerController : MonoBehaviourPunCallbacks
{
    public bool IsSit = false;
    public int currentJumpCount = 0; 
    public bool isGrounded = false;
    public bool OnceJumpRayCheck = false;

    public bool Is_DownJump_GroundCheck = false;   // 다운 점프를 하는데 아래 블록인지 그라운드인지 알려주는 불값
    protected float m_MoveX;
    public Rigidbody2D m_rigidbody;
    protected CapsuleCollider2D m_CapsuleCollider;
    protected Animator m_Anim;

    [SerializeField]
    public GameObject AttackHitBox;

    public GameObject camera;
    public GameObject crosshair;

    [Header("[Setting]")]
    public float MoveSpeed = 6;
    public int JumpCount = 2;
    public float jumpForce = 15f;

    protected void AnimUpdate()
    {
        if (!photonView.IsMine) {
            return;
        }

        if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") && !m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Roll"))
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                m_Anim.Play("Attack");
            }
            else
            {
                if (m_MoveX == 0)
                {
                    if (!OnceJumpRayCheck)
                        m_Anim.Play("Idle");
                }
                else
                {
                    m_Anim.Play("Run");
                }
            }
        }
    }


    protected void performJump()
    {
        m_Anim.Play("Jump");


    }

    protected void DownJump()
    {
        if (!isGrounded)
            return;

        if (!Is_DownJump_GroundCheck)
        {
            m_Anim.Play("Jump");

            m_rigidbody.AddForce(-Vector2.up * 10);
            isGrounded = false;

            m_CapsuleCollider.enabled = false;

            StartCoroutine(GroundCapsuleColliderTimerFuc());
        }


    }

    IEnumerator GroundCapsuleColliderTimerFuc()
    {
        yield return new WaitForSeconds(0.3f);
        m_CapsuleCollider.enabled = true;
    }


    //////바닥 체크 레이케스트 
    Vector2 RayDir = Vector2.down;


    float PretmpY;
    float GroundCheckUpdateTic = 0;
    float GroundCheckUpdateTime = 0.01f;
    protected void GroundCheckUpdate()
    {
        if (!OnceJumpRayCheck)
            return;

        GroundCheckUpdateTic += Time.deltaTime;



        if (GroundCheckUpdateTic > GroundCheckUpdateTime)
        {
            GroundCheckUpdateTic = 0;



            if (PretmpY == 0)
            {
                PretmpY = transform.position.y;
                return;
            }



            float reY = transform.position.y - PretmpY;  //    -1  - 0 = -1 ,  -2 -   -1 = -3

            if (reY <= 0)
            {

                if (isGrounded)
                {

                    LandingEvent();
                    OnceJumpRayCheck = false;

                }
            }


            PretmpY = transform.position.y;

        }




    }



    protected abstract void LandingEvent();


}