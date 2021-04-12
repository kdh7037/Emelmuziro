using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class throwHook : MonoBehaviour
{
    public GameObject hook;

    GameObject curHook;
    public bool ropeActive;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            if (ropeActive == false)
            {
                Vector2 destiny = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                curHook = Instantiate(hook, transform.position, Quaternion.identity);
                curHook.GetComponent<RopeScript>().destiny = destiny;

                ropeActive = true;
            }
            else
            {
                Destroy(curHook);
                ropeActive = false;
            }
        }
    }
}