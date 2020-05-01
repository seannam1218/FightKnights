using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Swordman : PlayerController
{
	float dirFacing;
	bool lockAnim = false;
	float walkBackSlowDown = 0.4f;
	float attackDelay = 0.2f;
	float attackDuration = 0.1f;
	float offsetHalfGameScreen = 0.5f;

    private void Start()
    {
    	if (photonView.IsMine) {
            camera.SetActive(true);
            crosshair.SetActive(true);
        }

        m_CapsuleCollider  = this.GetComponent<CapsuleCollider2D>();
        m_Anim = this.transform.Find("model").GetComponent<Animator>();
        m_rigidbody = this.transform.GetComponent<Rigidbody2D>();
        AttackHitBox.SetActive(false);
    }


    private void FixedUpdate()
    {
       	if (!photonView.IsMine) {
            return;
        }
    	//turn the player to face the right direction while rolling or attacking
    	faceCorrectDirection();
    	checkInput();
    }

    public void faceCorrectDirection() {
    	if (!lockAnim) {
    		dirFacing = Input.mousePosition.x/Screen.width - offsetHalfGameScreen;
    	}
    	if (dirFacing > 0 && !lockAnim) {
    		transform.localScale = new Vector3(-1, 1, 1);
    	} else if (dirFacing <= 0 && !lockAnim) {
    		transform.localScale = new Vector3(1, 1, 1);
    	}
    }

    public void checkInput()
    {	
		if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack") ||
       		m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Roll"))
       		lockAnim = true;
       	else
       		lockAnim = false;

       	if (!lockAnim) {
			// sitting and rolling logic
	        if (Input.GetKey(KeyCode.S))  
	        {
	            IsSit = true;
	            m_Anim.Play("Sit");

	            if (Input.GetKey(KeyCode.D)){
	            	m_Anim.Play("Roll");
	            	IsSit = false;
	            	dirFacing = 1;
	            } else if (Input.GetKey(KeyCode.A)) {
	            	m_Anim.Play("Roll");
	            	IsSit = false;
	            	dirFacing = -1;
	            }
	        }
	        else if (Input.GetKeyUp(KeyCode.S))
	        {
	            m_Anim.Play("Idle");
	            IsSit = false;
	        }

	         // sit나 die일때 애니메이션이 돌때는 다른 애니메이션이 되지 않게 한다. 
	        if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Sit") || m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Die"))
	        {
	            if (Input.GetKeyDown(KeyCode.Space))
	            {
	                if (currentJumpCount < JumpCount)  // 0 , 1
	                {
	                    DownJump();
	                }
	            }
	            return;
	        }

	        m_MoveX = Input.GetAxis("Horizontal");

	        GroundCheckUpdate();

	       
	        if (Input.GetKey(KeyCode.Mouse0))
	        {
	            m_Anim.Play("Attack");
	            StartCoroutine(DoAttack());
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
	        
	        // Die animation
	        if (Input.GetKey(KeyCode.Alpha1))
	        {
	            m_Anim.Play("Die");
	        }
			
	        //Jump trigger logic
	        if (Input.GetKeyDown(KeyCode.Space))
	        {
	            if (currentJumpCount < JumpCount)  // 0 , 1
	            {
	                if (!IsSit)
	                {
	                    performJump();
	                }
	                else
	                {
	                    DownJump();
	                }
	            }
	        }
	    }   

	    // Running logic
        if (Input.GetKey(KeyCode.D))
        {
        	//if walking forward, walk in normal speed
			if (dirFacing >= 0) {
        		transform.transform.Translate(new Vector3(m_MoveX * MoveSpeed * Time.deltaTime, 0, 0));
        	} 
        	//if walking backwards, walk in slower speed
        	else {
        		transform.transform.Translate(new Vector3(m_MoveX * walkBackSlowDown*MoveSpeed * Time.deltaTime, 0, 0));
        	}
        }
        else if (Input.GetKey(KeyCode.A))
        {
			//if walking forward, walk in normal speed
			if (dirFacing < 0) {
        		transform.transform.Translate(new Vector3(m_MoveX * MoveSpeed * Time.deltaTime, 0, 0));
        	} 
        	//if walking backwards, walk in slower speed
        	else {
        		transform.transform.Translate(new Vector3(m_MoveX * walkBackSlowDown*MoveSpeed * Time.deltaTime, 0, 0));
        	}
        }
    }

    IEnumerator DoAttack() {
    	yield return new WaitForSeconds(attackDelay);
    	AttackHitBox.SetActive(true);
    	yield return new WaitForSeconds(attackDuration);
    	AttackHitBox.SetActive(false);
    }


    protected override void LandingEvent()
    {
        if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Run") && !m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
            m_Anim.Play("Idle");
    }
}
