using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using UnityEngine;

[System.Runtime.InteropServices.Guid("2AD61E23-F88A-429C-B06C-701BF7C13171")]
public class PlayerController : BaseCharacterController
{

	// === 외부 파라미터（Inspector 표시） =====================
	public float initHpMax = 20.0f;
	[Range(0.1f, 100.0f)] public float initSpeed = 1.0f;
	//public LineRenderer line;
	public float distance = 10f;
	public LayerMask mask;
	public float step = 0.2f;
	public Rigidbody2D rigid;


	// === 외부 파라미터 ======================================
	[System.NonSerialized] public new float hpMax = 10.0f;
	[System.NonSerialized] public int jumpCount = 0;
	[System.NonSerialized] public bool ropeJump = false;
	[System.NonSerialized] public bool stonePull = false;

	// === 캐쉬 ==========================================
	DistanceJoint2D joint;

	// === 내부 파라미터 ======================================

	private Animator moveAnimator; // 부모의 애니메이터
	private Vector2 perpendicularDirection;
	bool breakEnabled = true;
	float groundFriction = 0.0f;
	Vector3 targetPos;
	public SpriteRenderer spriteRenderer;
	public RaycastHit2D hit;
	public Vector2 ropeHook;
	public float swingForce = 30f;
	protected bool ropeTrigger = false;
	public float acceleration = 15f;


	// === 코드（Monobehaviour기본 기능 구현） ================
	protected override void Awake()
	{
		base.Awake();

		// 파라미터 초기화
		speed = initSpeed;
		SetHP(initHpMax, initHpMax);

		rigid = GetComponent<Rigidbody2D>();
		//collision = transform.Find("Collider_Collision").gameObject.GetComponent<BoxCollider2D>();
		spriteRenderer = transform.Find("PlayerSprite").gameObject.GetComponent<SpriteRenderer>();
		//moveAnimator = transform.parent.GetComponent<Animator>();
	}

	protected override void Update()
	{
	}



	protected override void FixedUpdateCharacter()
	{
		// 착지했는지 검사
		if (falling && !momentum)
		{
			if ((grounded && !groundedPrev) || (grounded && Time.fixedTime > jumpStartTime + 1.0f))
			{
				animator.SetTrigger("Idle");
				animator.SetBool("Fall", false);
				switch (jumpCount)
				{
					case 1:
						animator.SetBool("Jump1", false);
						break;
					case 2:
						animator.SetBool("Jump2", false);
						break;
				}

				falling = false;
				jumped = false;
				ropeJump = false;
				jumpCount = 0;
			}
		}


		// 추락 검사
		if (GetComponent<Rigidbody2D>().velocity.y < 0 && !IsShoot && !isOnSlope)
		{
			if (!grounded && !falling)
			{
				if (!jumped)
				{
					jumpCount = 1;
					jumped = true;
					grounded = false;
					ActionFall();
					//UnityEngine.Debug.Log("jumped = true");
				}
				else
				{
					switch (jumpCount)
					{
						case 1:
							if (!grounded)
							{
								ActionFall();
							}
							break;

						case 2:
							if (!grounded)
							{
								ActionFall();
							}
							break;
					}
				}
			}
		}

        // 캐릭터 방향
        //transform.localScale = new Vector3(basScaleX * dir, transform.localScale.y, transform.localScale.z);

        // 점프 도중에 가로 이동 감속
        //if (jumped && !grounded)
        //{
        //	if (breakEnabled)
        //	{
        //		breakEnabled = false;
        //		speedVx *= 0.9f;
        //	}
        //}

        // 이동 정지(감속) 처리
        if (breakEnabled)
        {
            speedVx *= groundFriction;
        }

        if (grounded)
        {
			//animator.SetBool("Jump1", false);
			animator.SetBool("Jump2", false);
		}

		if (IsShoot)
		{
			rigid.AddForce(perpendicularDirection * 0.3f, ForceMode2D.Impulse);
			//Debug.Log("Velocity Cal_4");
		}



        // 카메라
        Camera.main.transform.position = transform.position - Vector3.forward;
	}

	// === 코드（기본 액션） =============================
	public override void ActionMove(float n)
	{
		if (!activeSts)
		{
			return;
		}


		// 초기화
		//float dirOld = dir;
		breakEnabled = false;

		// 애니메이션 지정
		float moveSpeed = Mathf.Clamp(n, -1.0f, +1.0f);
		//Debug.Log("moveSpeed : " + moveSpeed);
		if (!jumped && !falling && grounded)
		{
			//UnityEngine.Debug.Log("너도 실행되냐");
			animator.SetFloat("MoveSpeed", moveSpeed);
		}
		animator.speed = 1.0f + moveSpeed;

		// 이동 체크
		if (n != 0.0f)
		{
			// 이동
			//dir = Mathf.Sign(n);
			if (n == 1 && facingDirection == -1)
			{
				Flip();
			}
			else if (n == -1 && facingDirection == 1)
			{
				Flip();
			}
			moveSpeed = (moveSpeed < 0.5f) ? (moveSpeed * (1.0f / 0.5f)) : 1.0f;
			//Debug.Log("moveSpeed : " + moveSpeed);
			speedVx = initSpeed * moveSpeed;// * dir;
			//Debug.Log("speedVx : " + speedVx);
		}
		else
		{
			// 이동 정지
			breakEnabled = true;
		}

		// 그 시점에서 돌아보기 검사
		//if (dirOld != dir)
		//{
		//	breakEnabled = true;
		//}
	}

	public void ActionMove_2(float n)
    {
		//Debug.Log(n);
		//Debug.Log("무브작동");
		if (!activeSts)
		{
			return;
		}


		// 초기화
		breakEnabled = false;
		momentum = false;
		// 애니메이션 지정
		float moveSpeed = Mathf.Clamp(Mathf.Abs(n), -1.0f, +1.0f);
		//Debug.Log("moveSpeed : " + moveSpeed);
		if (!jumped && !falling && grounded)
		{
			//UnityEngine.Debug.Log("너도 실행되냐");
			animator.SetFloat("MoveSpeed", moveSpeed);
		}
		animator.speed = 1.0f + moveSpeed;

		// 방향 설정

		if (n > 0 && facingDirection == -1)
		{
			Flip();
		}
		else if (n < 0 && facingDirection == 1)
		{
			Flip();
		}

		// 이동 체크
		if (n != 0.0f)
		{
			// 이동
			//dir = Mathf.Sign(n);

			//moveSpeed = (moveSpeed < 0.5f) ? (moveSpeed * 2.0f/*(1.0f / 0.5f)*/) : 1.0f;
			//Debug.Log("moveSpeed : " + moveSpeed);
			Mathf.Clamp(n, -1.0f, +1.0f);
			speedVx = initSpeed * n;// * dir;
			//Debug.Log("speedVx : " + speedVx);
		}
		else
		{
			// 이동 정지
			breakEnabled = true;
		}
	}

	public void ActionRopeSwing(float n)
	{
		//Debug.Log("로프작동");
		animator.SetBool("Rope", true);
		animator.SetBool("Momentum", true);
		momentum = true;
		var playerToHookDirection = (ropeHook - (Vector2)transform.position).normalized;

		if (n < 0)
		{
			perpendicularDirection = new Vector2(-playerToHookDirection.y, playerToHookDirection.x);
			var leftPerpPos = (Vector2)transform.position - perpendicularDirection * -2f;
			UnityEngine.Debug.DrawLine(transform.position, leftPerpPos, Color.green, 0f);
		}
		else if (n > 0)
		{
			perpendicularDirection = new Vector2(playerToHookDirection.y, -playerToHookDirection.x);
			var rightPerpPos = (Vector2)transform.position + perpendicularDirection * 2f;
			UnityEngine.Debug.DrawLine(transform.position, rightPerpPos, Color.green, 0f);
		}
		else { perpendicularDirection = new Vector2(0, 0); }
		/*
		switch (jumpCount)
		{
			case 1:
				animator.SetBool("Jump1", false);
				break;


			case 2:
				animator.SetBool("Jump2", false);
				break;
		}
		if (animator.GetBool("Fall"))
		{
			animator.SetBool("Fall", false);
		}
		*/
		jumped = true;
		jumpCount = 1;
		/*
		dir = Mathf.Sign(n);
		float dirOld = dir;
		breakEnabled = false;

		/* 애니메이션 지정
		float moveSpeed = Mathf.Clamp(Mathf.Abs(n), -1.0f, +1.0f);
		if (!jumped && !falling)
		{
			animator.SetFloat("MoveSpeed", moveSpeed);
		}
		*/
		/*
		rigid.AddForce(Vector2.right * n, ForceMode2D.Impulse);

		float maxSpeed = 15f;

		if (rigid.velocity.x > maxSpeed)
			rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
		else if (rigid.velocity.x < maxSpeed * (-1))
			rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);

		speedVx = rigid.velocity.x;
		if (hit.transform.CompareTag("Stone") && grounded)
		{
			if ((transform.position.x - hit.transform.position.x > 0 && dir == -1) || (transform.position.x - hit.transform.position.x < 0 && dir == 1))
			{
				speedVx = 0;
			}
			else if (speedVx >= 3) { speedVx = 3; }
			else if (speedVx <= -3) { speedVx = -3; }

			stonePull = true;
		}
		if (rigid.velocity.x >= 0)
		{
			transform.localScale = new Vector3(basScaleX * dir, transform.localScale.y, transform.localScale.z);
		}
		else if (rigid.velocity.x < 0)
		{
			transform.localScale = new Vector3(basScaleX * dir, transform.localScale.y, transform.localScale.z);
		}*/
		/*
		// 초기화
		float dirOld = dir;
		breakEnabled = false;

		// 애니메이션 지정
		float moveSpeed = Mathf.Clamp(n, -10.0f, +10.0f);
		
		

		// 이동 체크
		if (n != 0.0f)
		{
			// 이동
			dir = Mathf.Sign(n);
			speedVx = moveSpeed * dir;
			rigid.AddForce(Vector2.right * n, ForceMode2D.Impulse);

		}

		else
		{
			// 이동 정지
			breakEnabled = true;
			/*if (!falling && !jumped)
			{
				animator.SetTrigger("Idle");
			}
		}

		// 그 시점에서 돌아보기 검사
		if (dirOld != dir)
		{
			breakEnabled = true;
		} */
	}

	//준영이가 봐줘야할 곳
	public void ActionRoll(float n)
	{
		//n은 방향 input임
		//Debug.Log(momentum);
		if (!activeSts)
		{
			return;
		}

		// 초기화
		breakEnabled = false;

		// 방향 설정

		/*
		float vmx = Mathf.Clamp(rigid.velocity.x, -20f, 20f);
		rigid.AddForce(new Vector2(20 * n, 0), ForceMode2D.Impulse);
		if (Input.GetKey(KeyCode.LeftArrow))
			rigid.velocity = new Vector2(vmx - 0.1f, rigid.velocity.y);
		else if (Input.GetKey(KeyCode.RightArrow))
			rigid.velocity = new Vector2(vmx + 0.1f, rigid.velocity.y);
		animator.SetBool("Momentum", true);*/
		//Input.GetKey(KeyCode.LeftArrow)
		if (n > 0)
		{
			rigid.velocity = new Vector2(rigid.velocity.x + (acceleration * Time.deltaTime), rigid.velocity.y);
			if(rigid.velocity.x > 0)
				animator.SetBool("Stop", false);
			else
				animator.SetBool("Stop", true);
		}
		else if (n < 0)
		{
			rigid.velocity = new Vector2(rigid.velocity.x - (acceleration * Time.deltaTime), rigid.velocity.y);
			if (rigid.velocity.x < 0)
				animator.SetBool("Stop", false);
			else
				animator.SetBool("Stop", true);
		}
		else // n == 0
		{
			if(rigid.velocity.x > 0)
				rigid.velocity = new Vector2(rigid.velocity.x + (0.5f * acceleration * Time.deltaTime), rigid.velocity.y);
			else if(rigid.velocity.x < 0)
				rigid.velocity = new Vector2(rigid.velocity.x - (0.5f * acceleration * Time.deltaTime), rigid.velocity.y);
		}

		if (n > 0 && facingDirection == -1 && rigid.velocity.x > 0)
		{
			Flip();
		}
		else if (n < 0 && facingDirection == 1 && rigid.velocity.x < 0)
		{
			Flip();
		}

		if (rigid.velocity.x > 30f)
			rigid.velocity = new Vector2(30f, rigid.velocity.y);
		else if (rigid. velocity.x < -30f)
			rigid.velocity = new Vector2(-30f, rigid.velocity.y);
		
		if (rigid.velocity.x > -1f && rigid.velocity.x < 1f)
		{
			//Debug.Log(rigid.velocity.x);
			//Debug.Log("이거 작동하냐?");
			// 이동 정지
			//breakEnabled = true;
			momentum = false;
			animator.SetBool("Momentum", false);
			/*
			float moveSpeed = Mathf.Clamp(Mathf.Abs(n), -1.0f, +1.0f);

			if (!jumped && !falling && grounded)
			{
				animator.SetFloat("MoveSpeed", moveSpeed);
			}
			animator.speed = 1.0f + moveSpeed;*/
		}
	}

	public void ActionJump()
	{
		switch (jumpCount)
		{
			case 0:
				if (grounded || IsShoot)
				{
					animator.SetBool("Jump1", true);
					rigid.velocity = new Vector3(rigid.velocity.x, 20.0f, 0);
					jumpStartTime = Time.fixedTime;
					jumped = true;
					falling = false;

					jumpCount++;
				}
				break;
			case 1:
				if (!grounded)
				{
					if (jumpCount == 1)
						animator.SetBool("Jump1", false);

					if (animator.GetBool("Rope"))
					{ // 로프에 매달려 있을 시
                        animator.SetBool("Rope", false);
                        joint.enabled = false;
                        //line.enabled = false;
                        IsShoot = false;
                        ropeJump = true;
                        transform.up = new Vector3(0, 1, 0);
                    }
					if (animator.GetBool("Fall"))
					{
						animator.SetBool("Fall", false);
					}
					animator.SetBool("Jump2", true);
					//UnityEngine.Debug.Log("jump2 = true");
					rigid.velocity = new Vector3(rigid.velocity.x, 20.0f, 0);
					//GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 10.0f);
					jumped = true;
					falling = false;


					jumpCount++;
				}
				break;
		}
	}

	public void ActionFall()
	{
		if (!falling)
		{
			if (!momentum)
			{
				animator.SetBool("Fall", true);
			}
			animator.SetFloat("MoveSpeed", 0);
			falling = true;
		}
	}

    /*
	public void ActionFireRope()
    {
		if (!IsShoot)
		{
			targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			targetPos.z = 0;
			hit = Physics2D.Raycast(transform.position, targetPos - transform.position, distance, mask);

			if (hit.collider != null && hit.collider.gameObject.GetComponent<Rigidbody2D>() != null)
			{ // 레이캐스티을 해서 무언가에 닿았을 때 로프를 생성
				curHook = Instantiate(hook, transform.position, Quaternion.identity);
				curHook.SetActive(true);
				curHook.GetComponent<RopeScript>().destiny = hit.point;

				joint.enabled = true;
				joint.connectedBody = hit.collider.gameObject.GetComponent<Rigidbody2D>();
				if (hit.transform.tag == "Ring" || hit.transform.tag == "Tree" || hit.transform.tag == "Stone")
				{
					joint.connectedAnchor = new Vector2(0, 0);
				}
				else
				{
					joint.connectedAnchor = hit.point - new Vector2(hit.collider.transform.position.x, hit.collider.transform.position.y);
				}
				joint.distance = Vector2.Distance(transform.position, hit.point);

				line.enabled = true;
				line.SetPosition(0, transform.position);
				line.SetPosition(1, curHook.transform.position);


				if (hit.transform.tag == "Stone")
                {
					stonePull = true;
                }

				switch (jumpCount)
				{
					case 1:
						animator.SetBool("Jump1", false);
						break;


					case 2:
						animator.SetBool("Jump2", false);
						break;
				}
				if (animator.GetBool("Fall"))
				{
					animator.SetBool("Fall", false);
				}
				// 
				jumped = true;
				jumpCount = 1;


				animator.SetBool("Rope", true);
				IsShoot = true;
			}
		}
		else
		{
			Destroy(curHook);
			joint.enabled = false;
			line.enabled = false;
			animator.SetBool("Rope", false);
			IsShoot = false;
			transform.up = new Vector3(0, 1, 0);
		}
	}
	*/

    public void ActionTreeAnimaiton()
	{
		/*
		if (hit.collider.tag == "Tree")
		{
			line.SetPosition(0, transform.position);
			//line.SetPosition(1, hit.collider.transform.position);
			line.SetPosition(1, curHook.transform.position);
			moveAnimator.SetBool("Tree", true);
			Destroy(curHook);
			joint.enabled = false;
			rigid.bodyType = RigidbodyType2D.Static;
		}*/
	}
	public void ActionTreeJump()
	{
		/*
		if (hit.collider.tag == "Tree")
		{
			Destroy(curHook);
			ropeJump = true;
			rigid.bodyType = RigidbodyType2D.Dynamic;
			moveAnimator.SetBool("Tree", false);
			line.enabled = false;
			rigid.AddForce(new Vector3(1, 1, 0) * 1000f);
		}*/
	}

	public void ActionStone()
	{
		if (grounded)
		{
			rigid.bodyType = RigidbodyType2D.Static;
		}
	}

	
}