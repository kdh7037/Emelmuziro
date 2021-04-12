using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeScript2 : MonoBehaviour
{
    public Vector2 destiny;
    public float speed = 1;
    public float distance = 2;

    public GameObject nodePrefab;

    public GameObject lastNode;
    public List<GameObject> Nodes = new List<GameObject>();

    private GameObject player;
    private LineRenderer lr;
    private bool done;
    private int vertexCount = 2;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        done = false;
        lastNode = transform.gameObject;
        Nodes.Add(transform.gameObject);
        lr = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, destiny, speed);


        if ((Vector2)transform.position != destiny)
        {
            if (Vector3.Distance(player.transform.position, lastNode.transform.position) > distance)
            {
                CreateNode();
            }
        }
        else if (done == false)
        {
            done = true;
            while (Vector3.Distance(player.transform.position, lastNode.transform.position) > distance)
            {
                CreateNode();
            }

            lastNode.GetComponent<HingeJoint2D>().connectedBody = player.GetComponent<Rigidbody2D>();
        }

        RenderLine();
    }

    void RenderLine()
    {
        lr.positionCount = vertexCount;

        int i;
        for (i = 0; i < Nodes.Count; i++)
        {
            lr.SetPosition(i, Nodes[i].transform.position);
        }

        lr.SetPosition(i, player.transform.position);
    }
    void CreateNode()
    {
        Vector2 pos2Create = player.transform.position - lastNode.transform.position;
        pos2Create.Normalize();
        pos2Create *= distance;
        pos2Create += (Vector2)lastNode.transform.position;

        GameObject go = Instantiate(nodePrefab, pos2Create, Quaternion.identity);

        go.transform.SetParent(transform);

        lastNode.GetComponent<HingeJoint2D>().connectedBody = go.GetComponent<Rigidbody2D>();

        lastNode = go;
        Nodes.Add(lastNode);
        vertexCount++;
    }
}
