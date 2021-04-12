using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private grapplinghook grapplingHook;
    private PlayerController playerController;
    private PlayerMain playerMain;
 
    public GameObject player;
    public GameObject menuSet;
    public GameObject original;
    public GameObject saveMenu;

    Animator anim;

    void Start()
    {
        GameStartLoad();
        grapplingHook = GameObject.Find("Player").GetComponent<grapplinghook>();
        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        playerMain = GameObject.Find("Player").GetComponent<PlayerMain>();
        anim = GameObject.Find("UI").GetComponent<Animator>();

        grapplingHook.enabled = false;
        playerController.enabled = false;
        playerMain.enabled = false;
    }

    void Update()
    {
        
    }

    public void GameSave1()
    {
        //player.x, player.y
        PlayerPrefs.SetFloat("PlayerX1", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY1", player.transform.position.y);
        PlayerPrefs.Save();

        original.SetActive(true);
        saveMenu.SetActive(false);
        menuSet.SetActive(false);
        anim.SetBool("SaveText1", true);
        Invoke("SaveText1", 1f);
    }

    public void GameSave2()
    {
        //player.x, player.y
        PlayerPrefs.SetFloat("PlayerX2", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY2", player.transform.position.y);
        PlayerPrefs.Save();

        original.SetActive(true);
        saveMenu.SetActive(false);
        menuSet.SetActive(false);
        anim.SetBool("SaveText2", true);
        Invoke("SaveText2", 1f);
    }

    public void GameSave3()
    {
        //player.x, player.y
        PlayerPrefs.SetFloat("PlayerX3", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY3", player.transform.position.y);
        PlayerPrefs.Save();

        original.SetActive(true);
        saveMenu.SetActive(false);
        menuSet.SetActive(false);
        anim.SetBool("SaveText3", true);
        Invoke("SaveText3", 1f);
    }

    public void GameStartLoad()
    {
        if (!PlayerPrefs.HasKey("PlayerX1") && !PlayerPrefs.HasKey("PlayerX2") && !PlayerPrefs.HasKey("PlayerX3"))
            return;

        float x = 0;
        float y = 0;

        if (PlayerPrefs.HasKey("PlayerX1"))
        {
            x = PlayerPrefs.GetFloat("PlayerX1");
            y = PlayerPrefs.GetFloat("PlayerY1");
        }
        else if (PlayerPrefs.HasKey("PlayerX2"))
        {
            x = PlayerPrefs.GetFloat("PlayerX2");
            y = PlayerPrefs.GetFloat("PlayerY2");
        }
        else if (PlayerPrefs.HasKey("PlayerX3"))
        {
            x = PlayerPrefs.GetFloat("PlayerX3");
            y = PlayerPrefs.GetFloat("PlayerY3");
        }

        player.transform.position = new Vector3(x, y, 0);
    }

    public void GameQuit()
    {
        Application.Quit();
    }

    void SaveText1()
    {
        anim.SetBool("SaveText1", false);
    }
    void SaveText2()
    {
        anim.SetBool("SaveText2", false);
    }
    void SaveText3()
    {
        anim.SetBool("SaveText3", false);
    }
}
