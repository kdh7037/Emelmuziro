using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public GameObject startUI;
    private grapplinghook grapplingHook;
    private PlayerController playerController;
    private PlayerMain playerMain;
    public GameObject menuSet;
    public GameObject player;
    public GameObject original;
    public GameObject saveMenu;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        grapplingHook = GameObject.Find("Player").GetComponent<grapplinghook>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        playerMain = GameObject.Find("Player").GetComponent<PlayerMain>();
        startUI = transform.Find("GameStart").gameObject;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //Sub Menu
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuSet.activeSelf)
                menuSet.SetActive(false);
            else
                menuSet.SetActive(true);
        }
    }

    public void GameStart()
    {
        anim.SetBool("StartMenu", false);
        anim.SetBool("LoadMenu", true);
    }

    public void StartToLoad()
    {
        anim.SetBool("StartMenu", false);
        anim.SetBool("LoadMenu", true);
    }

    public void LoadToStart()
    {
        anim.SetBool("LoadMenu", false);
        anim.SetBool("StartMenu", true);
    }

    public void LoadToGame1()
    {
        anim.SetBool("StartMenu", false);
        anim.SetBool("LoadMenu", false);

        if (PlayerPrefs.HasKey("PlayerX1"))
            player.transform.position = new Vector3(PlayerPrefs.GetFloat("PlayerX1"), PlayerPrefs.GetFloat("PlayerY1"), 0);
        else
            player.transform.position = new Vector3(-32, 2, 0);

        grapplingHook.enabled = true;
        playerController.enabled = true;
        playerMain.enabled = true;
    }

    public void LoadToGame2()
    {
        anim.SetBool("StartMenu", false);
        anim.SetBool("LoadMenu", false);

        if (PlayerPrefs.HasKey("PlayerX2"))
            player.transform.position = new Vector3(PlayerPrefs.GetFloat("PlayerX2"), PlayerPrefs.GetFloat("PlayerY2"), 0);
        else
            player.transform.position = new Vector3(-32, 2, 0);

        grapplingHook.enabled = true;
        playerController.enabled = true;
        playerMain.enabled = true;
    }

    public void LoadToGame3()
    {
        anim.SetBool("StartMenu", false);
        anim.SetBool("LoadMenu", false);

        if (PlayerPrefs.HasKey("PlayerX3"))
            player.transform.position = new Vector3(PlayerPrefs.GetFloat("PlayerX3"), PlayerPrefs.GetFloat("PlayerY3"), 0);
        else
            player.transform.position = new Vector3(-32, 2, 0);


        grapplingHook.enabled = true;
        playerController.enabled = true;
        playerMain.enabled = true;
    }

    public void SaveGameOpen()
    {
        original.SetActive(false);
        saveMenu.SetActive(true);
    }

    public void SaveGameClose()
    {
        original.SetActive(true);
        saveMenu.SetActive(false);
    }
}
