using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeScript : MonoBehaviour
{
    PlayerController playerCtrl;

    public Vector2 destiny;
    public float speed = 1;
    public float distance = 2;

    public GameObject nodePrefab;
    
    public GameObject lastNode;
    public List<GameObject> Nodes = new List<GameObject>();

    private GameObject player;
    //private bool done;
    //private int vertexCount = 2;

    // Start is called before the first frame update
    void Start()
    {
        playerCtrl = GetComponent<PlayerController>();
        player = GameObject.FindGameObjectWithTag("Player");
        //done = false;
        lastNode = transform.gameObject;
        Nodes.Add(transform.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, destiny, speed);
    }
}
