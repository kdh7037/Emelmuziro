using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMain : MonoBehaviour
{

	// === 캐쉬 ==========================================
	PlayerController playerCtrl;

	[SerializeField] [Range(0.01f, 0.99f)] private float timeScale;

	// === 코드（Monobehaviour 기본 기능 구현） ================
	void Awake()
	{
		playerCtrl = GetComponent<PlayerController>();
	}

	void Update()
	{
		// 조작 가능한지 확인
		if (!playerCtrl.activeSts)
		{
			return;
		}

		// 패드 처리
		float joyMv = Input.GetAxis("Horizontal");

		if (playerCtrl.IsShoot && !playerCtrl.grounded)
		{
			playerCtrl.ActionRopeSwing(joyMv);
		}
		else if (!playerCtrl.momentum)
			playerCtrl.ActionMove_2(joyMv);
		else
			playerCtrl.ActionMomentum(joyMv);
		


		// 점프
		if (Input.GetButtonDown("Jump"))
		{
			playerCtrl.ActionJump();
		}

		// 로프 발사
		if (Input.GetButtonDown("Fire2"))
		{
			/*
			//playerCtrl.ActionFireRope();
			if (Time.timeScale == 1.0f)
				Time.timeScale = timeScale;
			else
				Time.timeScale = 1.0f;
			*/

		}

		// 로프에서 위아래 이동 처리
		if (playerCtrl.IsShoot)
		{
			/*
			if (playerCtrl.hit.transform.tag == "Stone")
			{
				playerCtrl.line.SetPosition(0, playerCtrl.hit.transform.position);
				playerCtrl.line.SetPosition(1, playerCtrl.curHook.transform.position);
				playerCtrl.line.GetComponent<ropeRatio>().grabPos = playerCtrl.hit.transform.position;
			}
			joyMv = Input.GetAxis("Vertical");
			playerCtrl.ActionRopeUpDown(joyMv);
			*/
		}

		//나무 애니메이션 처리
		if (playerCtrl.IsShoot && Input.GetKey(KeyCode.E))
		{
			//playerCtrl.ActionTreeAnimaiton();
		}

		if (playerCtrl.IsShoot && Input.GetKeyUp(KeyCode.E))
		{
			//playerCtrl.ActionTreeJump();
		}

		//돌 끄는 애니메이션
		if (playerCtrl.stonePull && Input.GetKey(KeyCode.E))
		{
			//playerCtrl.ActionStone();
		}
		else if (playerCtrl.stonePull && Input.GetKeyUp(KeyCode.E) || !playerCtrl.IsShoot)
		{
			playerCtrl.rigid.bodyType = RigidbodyType2D.Dynamic;
			playerCtrl.stonePull = false;
		}

		// 카메라
		Camera.main.transform.position = transform.position - Vector3.forward;
	}
}