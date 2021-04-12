using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.UI;

public class grapplinghook : MonoBehaviour
{
    public LineRenderer line;
    DistanceJoint2D joint;
    Vector3 targetPos;
    RaycastHit2D hit;
    public float distance = 10f;
    public LayerMask mask;
    public float step = 0.2f;
    [System.NonSerialized] public bool IsShoot;
    private PlayerController playerCtrl;
    private Animator animator;
    void Start()
    {
        joint = GetComponent<DistanceJoint2D>();
        joint.enabled = false;
        line.enabled = false;
        IsShoot = false;
        playerCtrl = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetButtonDown("Fire1"))
        {
            if (!IsShoot)
            {
                targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                targetPos.z = 0;
                hit = Physics2D.Raycast(transform.position, targetPos - transform.position, distance, mask);
                
                if (hit.collider != null && hit.collider.gameObject.GetComponent<Rigidbody2D>() != null)
                { // 레이캐스티을 해서 무언가에 닿았을 때 로프를 생성
                    joint.enabled = true;
                    joint.connectedBody = hit.collider.gameObject.GetComponent<Rigidbody2D>();
                    if (hit.transform.tag == "Ring")
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
                    if (hit.transform.tag == "Ring")
                    {
                        line.SetPosition(1, hit.transform.position);
                    }
                    else
                    {
                        line.SetPosition(1, hit.point);
                    }
                    
                    line.GetComponent<ropeRatio>().grabPos = hit.point;

                    switch (playerCtrl.jumpCount)
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
                    playerCtrl.jumped = true;
                    playerCtrl.jumpCount = 1;
                  

                    animator.SetBool("Rope", true);
                    IsShoot = true;
                }
            }
            else
            {
                joint.enabled = false;
                line.enabled = false;
                animator.SetBool("Rope", false);
                IsShoot = false;
            }
        }

        if(IsShoot)
        {
            if (Input.GetAxis("Vertical") > 0)
            {
                if (joint.distance > 1f)
                {
                    joint.distance -= step;

                }
                else
                {
                    joint.enabled = false;
                    line.enabled = false;
                    IsShoot = false;
                    animator.SetBool("Rope", false);
                }
                
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                
                if (joint.distance > 1f)
                {
                    joint.distance += step;

                }
                else
                {
                    joint.enabled = false;
                    line.enabled = false;
                }
                
            }
        }

        if(IsShoot)
        {
            line.SetPosition(0, transform.position);
        }

    }
}