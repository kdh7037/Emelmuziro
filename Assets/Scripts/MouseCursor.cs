using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    new Camera camera;
    Vector3 MousePosition;
    Texture2D cursorTexture;
    public Texture2D originalTexture;
    public Texture2D aimingCursorTexture;
    public bool hotSpotIsCenter = false;
    public Vector2 adjustHotSpot = Vector2.zero;
    public Vector2 hotSpot;
    // Start is called before the first frame update

    private void Awake()
    {

    }
    void Start()
    {
        camera = GetComponent<Camera>();
        Cursor.SetCursor(originalTexture, Vector2.zero, CursorMode.Auto);
        //StartCoroutine("MyCursor");
    }
    /*
    IEnumerator MyCursor()
    {
        yield return new WaitForEndOfFrame();

        if (hotSpotIsCenter)
        {
            hotSpot.x = cursorTexture.width / 2;
            hotSpot.y = cursorTexture.height / 2;
        }
        else
        {
            hotSpot = adjustHotSpot;
        }

        Debug.Log(cursorTexture);
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }
    */

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetMouseButtonDown(0))
        {
            MousePosition = Input.mousePosition;
            MousePosition = camera.ScreenToWorldPoint(MousePosition);

            RaycastHit2D hit = Physics2D.Raycast(MousePosition, transform.forward, 20f);
            Debug.DrawRay(MousePosition, transform.forward * 10, Color.red, 0.3f);
            if (hit.collider.transform.tag == "Tree")
            {
                cursorTexture = aimingCursorTexture;
            }
            if (!hit)
            {
                cursorTexture = originalTexture;
            }
        }
        */
    }

    public void OnMouseOver()
    {
        Debug.Log("작동함");
        Cursor.SetCursor(aimingCursorTexture, Vector2.zero, CursorMode.Auto);
    }

    public void OnMouseExit()
    {
        Debug.Log("왜 작동 안함?");
        Cursor.SetCursor(originalTexture, Vector2.zero, CursorMode.Auto);
    }
}
