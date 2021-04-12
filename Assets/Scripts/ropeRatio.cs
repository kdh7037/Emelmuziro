using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ropeRatio : MonoBehaviour
{
    public GameObject player;
    public float ratio;

    [System.NonSerialized] public Vector3 grabPos;
     
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float scaleX = Vector3.Distance(player.transform.position, grabPos)/ratio;
        GetComponent<LineRenderer>().material.mainTextureScale = new Vector2(scaleX, 1f);
    }
}
