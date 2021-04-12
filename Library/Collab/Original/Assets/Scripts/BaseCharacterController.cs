using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacterController : MonoBehaviour
{

	// === 외부 파라미터(Inspector 표시) =====================
	public Vector2 velocityMin = new Vector2(-30.0f, -50.0f);
	public Vector2 velocityMax = new Vector2(+30.0f, +50.0f);

	// === 외부 표시 파라미터 =================================
	[SerializeField] protected float slopeCheckDistance;
	[SerializeField] protected LayerMask whatIsGround;
	[SerializeField] protected float maxSlopeAngle;
	[SerializeField] protected PhysicsMaterial2D noFriction;
	[SerializeField] protected PhysicsMaterial2D fullFriction;
	[SerializeField] protected float groundCheckRadius;
	[SerializeField] protected Transform groundCheck;

	// === 외부 파라미터 ======================================
	[System.NonSerialized] public float hpMax = 10.0f;
	[System.NonSerialized] public float hp = 10.0f;
	[System.NonSerialized] public float dir = 1.0f;
	[System.NonSerialized] public float speed = 6.0f;
	//[System.NonSerialized] public float basScaleX = 1.0f;
	[System.NonSerialized] public bool activeSts = false;
	[System.NonSerialized] public bool jumped = false;
	[System.NonSerialized] public bool falling = false;
	[System.NonSerialized] public bool grounded = false;
	[System.NonSerialized] public bool groundedPrev = false;
	[System.NonSerialized] public bool IsShoot = false;
	[System.NonSerialized] public bool momentum = false;
	[System.NonSerialized] public bool outOfControl = false;


	// === 캐쉬 ==========================================
	[System.NonSerialized] public Animator animator;
	protected Transform groundCheck_L;
	protected Transform groundCheck_C;
	protected Transform groundCheck_R;


	// === 내부 파라미터 ======================================
	protected float speedVx = 0.0f;
	protected float speedVxAddPower = 0.0f;
	protected float gravityScale = 10.0f;
	protected float jumpStartTime = 0.0f;
	protected CircleCollider2D cc;
	protected Vector2 CheckPos;
	protected float circleColliderSize;
	protected bool isOnSlope = false;
	protected bool canWalkOnSlope;
	protected float slopeDownAngle;
	protected float slopeSideAngle;
	protected float lastSlopeAngle;
	protected Vector2 slopeNormalPerp;
	protected int facingDirection = 1;

	protected Rigidbody2D rb;

	protected GameObject groundCheck_OnRoadObject;
	protected GameObject groundCheck_OnMoveObject;
	protected GameObject groundCheck_OnEnemyObject;



	// === 코드（Monobehaviour기본 기능 구현） ================
	protected virtual void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		cc = GetComponentInChildren<CircleCollider2D>();

		animator = GetComponent<Animator>();
		//groundCheck_L = transform.Find("GroundCheck_L");
		//groundCheck_C = transform.Find("GroundCheck_C");
		//groundCheck_R = transform.Find("GroundCheck_R");

		//dir = (transform.localScale.x > 0.0f) ? 1 : -1;
		//basScaleX = transform.localScale.x * dir;
		//transform.localScale = new Vector3(basScaleX, transform.localScale.y, transform.localScale.z);

		activeSts = true;
		gravityScale = GetComponent<Rigidbody2D>().gravityScale;
	}

	protected virtual void Start()
	{
		circleColliderSize = cc.radius;

	}

	protected virtual void Update()
	{
	}

	protected virtual void FixedUpdate()
	{
		// 구멍에 빠졌는지 검사
		if (transform.position.y < -30.0f)
		{
			Dead(false); // 사망
		}

		// 지면에 닿았는지 검사
		groundedPrev = grounded;
		grounded = false;

		grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);

		//groundCheck_OnRoadObject = null;
		//groundCheck_OnMoveObject = null;
		//groundCheck_OnEnemyObject = null;

		//Collider2D[][] groundCheckCollider = new Collider2D[3][];
		//groundCheckCollider[0] = Physics2D.OverlapPointAll(groundCheck_L.position);
		//groundCheckCollider[1] = Physics2D.OverlapPointAll(groundCheck_C.position);
		//groundCheckCollider[2] = Physics2D.OverlapPointAll(groundCheck_R.position);

		/*foreach (Collider2D[] groundCheckList in groundCheckCollider)
		{
			foreach (Collider2D groundCheck in groundCheckList)
			{
				if (groundCheck != null)
				{
					if (!groundCheck.isTrigger)
					{
						if(groundCheck.tag != "Player")
							grounded = true;
						
						if (grounded == true)
							//Debug.Log("velocity.y =" + GetComponent<Rigidbody2D>().velocity.y);
							//Debug.Log("grounded = true");
							if (groundCheck.tag == "Road")
							{
								groundCheck_OnRoadObject = groundCheck.gameObject;
							}
							else
							if (groundCheck.tag == "MoveObject")
							{
								groundCheck_OnMoveObject = groundCheck.gameObject;
							}
							else
							if (groundCheck.tag == "Enemy")
							{
								groundCheck_OnEnemyObject = groundCheck.gameObject;
							}
					}
				}
			}
		}*/


		// 캐릭터 개별 처리
		FixedUpdateCharacter();

		// 경사로 체크
		SlopeCheck();

		if (isOnSlope)
		{
			//Debug.Log("is on slope");
		}


		// 이동 계산
		
		if ((!IsShoot ||(IsShoot && grounded)) && !momentum) {
			if (grounded && !isOnSlope && !jumped) //if not on slope (&& !jumped)
			{
				rb.velocity = new Vector2(speedVx, 0.0f);
				//Debug.Log("Velocity Cal_1");
			}
			else if (grounded && isOnSlope && canWalkOnSlope && !jumped) //If on slope
			{
				//if(!cc.OverlapPoint(CheckPos))
				//   rb.velocity = new Vector2(-speedVx * slopeNormalPerp.x, rb.velocity.y);
				//else
				rb.velocity = new Vector2(-speedVx * slopeNormalPerp.x, -speedVx * slopeNormalPerp.y);
				//Debug.Log("Velocity Cal_2");
			}
			else if (!grounded && (jumped || falling))
			{
				rb.velocity = new Vector2(speedVx, rb.velocity.y);
				//Debug.Log("Velocity Cal_3");
			}

		}
        // Veclocity값 검사
		
        float vx = Mathf.Clamp(rb.velocity.x, velocityMin.x, velocityMax.x);
		float vy = Mathf.Clamp(rb.velocity.y, velocityMin.y, velocityMax.y);

		if ((!IsShoot || (IsShoot && grounded)) && !momentum)
		{
			rb.velocity = new Vector2(vx, vy);
		}
    }

	protected virtual void FixedUpdateCharacter() { }

	protected virtual void SlopeCheck()
	{
		CheckPos = transform.position - ((Vector3)(new Vector2(0.0f, cc.radius) - cc.offset) * 3.0f);
		//if (facingDirection == 1)
		//{
		//   checkPos = transform.position - ((Vector3)(new Vector2(0.0f, cc.radius) - cc.offset) * 3.0f);
		//}
		//else if (facingDirection == -1)
		//{
		//   checkPos = transform.position - ((Vector3)(new Vector2(0.0f, cc.radius) - new Vector2(-cc.offset.x, cc.offset.y)) * 3.0f);
		//}
		//Debug.Log("Doing SlopeCheck");

		Debug.DrawLine(CheckPos, transform.position, Color.red);
		SlopeCheckHorizontal(CheckPos);
		SlopeCheckVertical(CheckPos);
	}

	protected void SlopeCheckHorizontal(Vector2 checkPos)
	{
		RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, whatIsGround);
		RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, whatIsGround);

		if (slopeHitFront)
		{
			isOnSlope = true;

			slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);

		}
		else if (slopeHitBack)
		{
			isOnSlope = true;

			slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
		}
		else
		{
			slopeSideAngle = 0.0f;
			isOnSlope = false;
		}

	}

	protected void SlopeCheckVertical(Vector2 checkPos)
	{
		RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);

		if (hit)
		{

			slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;

			slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);

			if (slopeDownAngle != lastSlopeAngle)
			{
				isOnSlope = true;
			}

			lastSlopeAngle = slopeDownAngle;

			Debug.DrawRay(hit.point, slopeNormalPerp, Color.blue);
			Debug.DrawRay(hit.point, hit.normal, Color.green);

		}

		if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
		{
			canWalkOnSlope = false;
		}
		else
		{
			canWalkOnSlope = true;
		}

		if (isOnSlope && canWalkOnSlope && (Input.GetAxis("Horizontal") == 0.0f))
		{
			rb.sharedMaterial = fullFriction;
		}
		else
		{
			rb.sharedMaterial = noFriction;
		}
	}

	// === 코드（기본 액션） =============================
	public virtual void ActionMove(float n)
	{
		if (n != 0.0f)
		{
			dir = Mathf.Sign(n);
			speedVx = speed * n;
			animator.SetTrigger("Run");
		}
		else
		{
			speedVx = 0;
			animator.SetTrigger("Idle");
		}

		if (isOnSlope && canWalkOnSlope && n == 0.0f)
		{
			rb.sharedMaterial = fullFriction;
		}
		else
		{
			rb.sharedMaterial = noFriction;
		}
	}

	// === 코드（그 외） ====================================
	public virtual void Dead(bool gameOver)
	{
		if (!activeSts)
		{
			return;
		}
		activeSts = false;
	}

	public virtual bool SetHP(float _hp, float _hpMax)
	{
		hp = _hp;
		hpMax = _hpMax;
		return (hp <= 0);
	}

	public virtual void Flip()
	{
		facingDirection *= -1;
		transform.Rotate(0.0f, 180.0f, 0.0f);
	}
}
