using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCharacterController : MonoBehaviour
{

	// === 외부 파라미터(Inspector 표시) =====================
	public Vector2 velocityMin = new Vector2(-30.0f, -50.0f);
	public Vector2 velocityMax = new Vector2(+30.0f, +50.0f);
	public LayerMask collisionMask;

	// === 외부 표시 파라미터 =================================
	[SerializeField] protected float slopeCheckDistance;
	[SerializeField] protected LayerMask whatIsGround;
	[SerializeField] protected float maxSlopeAngle;
	[SerializeField] protected float maxDescendAngle;
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
	[System.NonSerialized] public bool isRoll = false;


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
	protected BoxCollider2D boxCollider;
	protected Vector2 CheckPos_up;
	protected Vector2 CheckPos_down;
	protected float circleColliderSize;
	protected bool isOnSlope = false;
	protected bool canWalkOnSlope;
	protected float slopeDownAngle;
	protected float slopeSideAngle;
	protected float lastSlopeAngle;
	protected Vector2 slopeNormalPerp;
	protected int facingDirection = 1;
	protected float skinWidth = 0.015f;
	protected int horizontalRayCount = 4;
	protected int verticalRayCount = 4;
	protected float horizontalRaySpacing;
	protected float verticalRaySpacing;
	protected float maxClimbAngle = 80;
	protected CollisionInfo collisions;
	protected RaycastOrigins raycastOrigins;

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
		boxCollider = GetComponentInChildren<BoxCollider2D>();
		CalculateRaySpacing();
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

		/*groundCheck_OnRoadObject = null;
		groundCheck_OnMoveObject = null;
		groundCheck_OnEnemyObject = null;

		Collider2D[][] groundCheckCollider = new Collider2D[3][];
		groundCheckCollider[0] = Physics2D.OverlapPointAll(groundCheck_L.position);
		groundCheckCollider[1] = Physics2D.OverlapPointAll(groundCheck_C.position);
		groundCheckCollider[2] = Physics2D.OverlapPointAll(groundCheck_R.position);

		foreach (Collider2D[] groundCheckList in groundCheckCollider)
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

		// 경사로 체크
		SlopeCheck();
		UpdateRaycastOrigins();

		// 캐릭터 개별 처리
		FixedUpdateCharacter();


		if (isOnSlope)
		{
			//Debug.Log("is on slope");
		}


		// 이동 계산

		if ((!IsShoot || (IsShoot && grounded)) && !momentum && !isRoll)
		{
			if (grounded && !isOnSlope && !jumped && !isRoll) //if not on slope (&& !jumped)
			{
				rb.velocity = new Vector2(speedVx, 0.0f);
				Debug.Log("Velocity Cal_1");
			}
			else if (grounded && isOnSlope && canWalkOnSlope && !jumped && !isRoll) //If on slope
			{
				//if(!cc.OverlapPoint(CheckPos))
				//   rb.velocity = new Vector2(-speedVx * slopeNormalPerp.x, rb.velocity.y);
				//else
				rb.velocity = new Vector2(-speedVx * slopeNormalPerp.x, -speedVx * slopeNormalPerp.y);
				Debug.Log("Velocity Cal_2");
			}
			else if (!grounded && (jumped || falling))
			{
				rb.velocity = new Vector2(speedVx, rb.velocity.y);
				Debug.Log("Velocity Cal_3");
			}

		}
	
		


        // Veclocity값 검사
		
        float vx = Mathf.Clamp(rb.velocity.x, velocityMin.x, velocityMax.x);
		float vmx = Mathf.Clamp(rb.velocity.x, -20.0f, 20.0f);
		float vy = Mathf.Clamp(rb.velocity.y, velocityMin.y, velocityMax.y);

		if ((!IsShoot || (IsShoot && grounded)) && !momentum && !isRoll)
		{
			rb.velocity = new Vector2(vx, vy);
		}
		else if (!IsShoot || (IsShoot && grounded) && momentum && !isRoll)
		{
			rb.velocity = new Vector2(vmx, vy);
		}
    }

	protected virtual void FixedUpdateCharacter()
	{
	}

	protected virtual void SlopeCheck()
	{
		CheckPos_up = transform.position + (Vector3)(0.7f * Vector2.up);
		CheckPos_down = transform.position - ((Vector3)(new Vector2(0.0f, cc.radius) - cc.offset) * 3.0f);

		Vector2 rayEdge_right = CheckPos_down + new Vector2(slopeCheckDistance, 0.0f);
		Vector2 rayEdge_left = CheckPos_down - new Vector2(slopeCheckDistance, 0.0f);
		Vector2 rayEdge_down = CheckPos_down - new Vector2(0.0f, slopeCheckDistance);

		// 콜라이더의 가장 낮은 곳에서 체크 거리까지를 그림
		Debug.DrawLine(CheckPos_down, rayEdge_right, Color.yellow);
		Debug.DrawLine(CheckPos_down, rayEdge_left, Color.cyan);
		Debug.DrawLine(CheckPos_down, rayEdge_down, Color.red);
		
		SlopeCheckHorizontal(CheckPos_down);
		SlopeCheckVertical(CheckPos_down);
	}

	protected void HorizontalCollisions(ref Vector3 velocity)
    {
		float directionX = Mathf.Sign(velocity.x);
		float rayLength = Mathf.Abs(velocity.x) + skinWidth;

		for (int i = 0; i < horizontalRayCount; i++)
		{
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

			if (hit)
			{
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

				if (i == 0 && slopeAngle <= maxClimbAngle)
				{
					if (collisions.descendingSlope)
					{
						collisions.descendingSlope = false;
						velocity = collisions.velocityOld;
					}
					float distanceToSlopeStart = 0;
					if (slopeAngle != collisions.slopeAngleOld)
					{
						distanceToSlopeStart = hit.distance - skinWidth;
						velocity.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope(ref velocity, slopeAngle);
					velocity.x += distanceToSlopeStart * directionX;
				}

				if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                {
					velocity.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					if(collisions.climbingSlope)
                    {
						velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
                }
            }
        }
    }

	protected void VerticalCollisions(ref Vector3 velocity)
    {
		float directionY = Mathf.Sign(velocity.y);
		float rayLength = Mathf.Abs(velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i++)
        {
			Vector2 rayOrgin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrgin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrgin, Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay(rayOrgin, Vector2.up * directionY * rayLength, Color.red);

			if(hit)
            {
				velocity.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				if(collisions.climbingSlope)
                {
					velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;
            }
        }

		if(collisions.climbingSlope)
        {
			float directionX = Mathf.Sign(velocity.x);
			rayLength = Mathf.Abs(velocity.x) + skinWidth;
			Vector2 rayOrgin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
			RaycastHit2D hit = Physics2D.Raycast(rayOrgin, Vector2.right * directionX, rayLength, collisionMask);

			if(hit)
            {
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				if (slopeAngle != collisions.slopeAngle)
                {
					velocity.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }

	protected void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
		float moveDistance = Mathf.Abs(velocity.x);
		float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (velocity.y <= climbVelocityY)
        { // not jumbping
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
        }
    }

	protected void DescendSlope(ref Vector3 velocity)
    {
		float directionX = Mathf.Sign(velocity.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if(hit)
        {
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if(slopeAngle != 0 && slopeAngle <= maxDescendAngle)
            {
				if(Mathf.Sign(hit.normal.x) == directionX)
                {
					if(hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
					{ // How far from we have to move on the y-axis based on this slope and our velocity on the x axis
						float moveDistance = Mathf.Abs(velocity.x);
						float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
						velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
						velocity.y -= descendVelocityY;

						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
                }
            }
        }
    }

	protected void UpdateRaycastOrigins()
    {
		Bounds bounds = boxCollider.bounds;
		bounds.Expand(skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
	}

	protected void CalculateRaySpacing ()
    {
		Bounds bounds = boxCollider.bounds;
		bounds.Expand(skinWidth * -2);

		horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		Debug.Log("bounds.size.y : " + bounds.size.y + ", horizontalRaySpacing : " + horizontalRaySpacing);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

	protected void SlopeCheckHorizontal(Vector2 checkPos)
	{
		RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, whatIsGround);
		RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, whatIsGround);

		Debug.DrawRay(slopeHitFront.point, Vector2.Perpendicular(slopeHitFront.normal).normalized, Color.blue);
		Debug.DrawRay(slopeHitFront.point, slopeHitFront.normal, Color.green);

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

			//Debug.DrawRay(hit.point, slopeNormalPerp, Color.blue);
			//Debug.DrawRay(hit.point, hit.normal, Color.green);
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
		if (n != 0.0f || !isRoll)
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

	protected struct RaycastOrigins
	{
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}

	protected struct CollisionInfo
	{
		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public bool descendingSlope;
		public float slopeAngle, slopeAngleOld;
		public Vector3 velocityOld;

		public void Reset()
		{
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}
}
