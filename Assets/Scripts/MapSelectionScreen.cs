using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectionScreen : MonoBehaviour
{

    private CustomizationDetails DetailsScript;

    public GameObject mapSelectTextServer;
    public GameObject mapSelectTextServerbackground;
    public GameObject mapSelectTextClient;
    public GameObject mapSelectTextClientbackground;

    public Button randomButton;
    public Button starySkyButton;
    public Button aruroaButton;
    public Button colosseumButton;
    public Button backButton;

    void Start()
    {
        gameObject.SetActive(false);
        DetailsScript = GameObject.FindObjectOfType<CustomizationDetails>();
    }

    public void LoadStarrySky()
    {
        PlayerIdentity identity = GameObject.FindObjectOfType<PlayerIdentity>();
        identity.RpcStarrySky();
        identity.RpcStartGame();
        enableMap(false);
    }

    public void LoadAurora()
    {
        PlayerIdentity identity = GameObject.FindObjectOfType<PlayerIdentity>();
        identity.RpcAurora();
        identity.RpcStartGame();
        enableMap(false);
    }

    public void LoadColosseum()
    {
        Debug.Log("LoadColosseum");
        PlayerIdentity identity = GameObject.FindObjectOfType<PlayerIdentity>();
        identity.RpcColosseum();
        identity.RpcStartGame();
        enableMap(false);
    }

    public void LoadRandom()
    {
        int rand = Random.Range(0, 3);
        switch (rand)
        {
            case 0:
                LoadStarrySky();
                break;
            case 1:
                LoadAurora();
                break;
            case 2:
                LoadColosseum();
                break;
            default:
                LoadStarrySky();
                break;
        }
    }

    public void enableMap(bool enable)
    {
        PlayerIdentity identity = GameObject.FindObjectOfType<PlayerIdentity>();
        identity.RpcUpdateMapSelectServer(enable);
    }
}
