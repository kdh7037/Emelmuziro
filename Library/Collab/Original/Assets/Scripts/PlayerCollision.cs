using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    public PlayerController playerCtrl;
    public SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        playerCtrl = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        //playerCtrl.spriteRenderer.color = new Color(1, 1, 1, 0.4f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Road")
        {
            Vector2 contact = collision.contacts[0].point;
            OnDamaged(contact);
        }
    }

    public void OnDamaged(Vector2 targetPos)
    {
        //playerCtrl.OnDamagedSprite();
        playerCtrl.spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        playerCtrl.activeSts = false;
        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        playerCtrl.rigid.AddForce(new Vector2(dirc, 1) * 20, ForceMode2D.Impulse);
        Invoke("OffDamaged", 1.5f);
    }

    void OffDamaged()
    {
        // playerCtrl.OffDamagedSprite();
        playerCtrl.activeSts = true;
        playerCtrl.spriteRenderer.color = new Color(1, 1, 1, 1);
    }
}
