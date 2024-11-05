using FishNet.Managing;
using FishNet.Transporting.FishyUTPPlugin;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Contribution - Chris
public class MenuUI : MonoBehaviour
{
    private NetworkManager _networkManager;
    [SerializeField] private TMP_InputField codeInput;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Toggle camToggle;
    [SerializeField] private Toggle nameToggle;
    [SerializeField] private GameObject loadingMenu;


    // Start is called before the first frame update
    void Start()
    {
        _networkManager = FindObjectOfType<NetworkManager>();
        if (_networkManager == null)
        {
            Debug.LogWarning("NetworkManager not found, HUD will not function.");
            return;
        }
        nameInput.text = PlayerPrefs.GetString("Name", "PlayerName");

        if (!PlayerPrefs.HasKey("CameraMove"))
        {
            PlayerPrefs.SetInt("CameraMove", 1);
        }
        if (!PlayerPrefs.HasKey("NameDisplayMode"))
        {
            PlayerPrefs.SetInt("NameDisplayMode", 0);
        }


        if (PlayerPrefs.GetInt("NameDisplayMode") == 1)
        {
            nameToggle.isOn = true;
        }
        else
        {
            nameToggle.isOn = false;
        }
    }


    public void ChangeName()
    {
        PlayerPrefs.SetString("Name", nameInput.text);
    }

    public void ChangeCameraMove()
    {
        if (camToggle.isOn)
        {
            PlayerPrefs.SetInt("CameraMove", 1);
        }
        else
        {
            PlayerPrefs.SetInt("CameraMove", 0);
        }
        Debug.Log("CameraMove: " + PlayerPrefs.GetInt("CameraMove", 1));
    }
    public void ChangeNameDisplay()
    {
        if (camToggle.isOn)
        {
            PlayerPrefs.SetInt("NameDisplayMode", 1);
        }
        else
        {
            PlayerPrefs.SetInt("NameDisplayMode", 0);
        }
    }

    public void StartServer()
    {
        _networkManager.ServerManager.StartConnection();
        loadingMenu.SetActive(true);
    }

    public void StartClient()
    {
        _networkManager.gameObject.GetComponent<FishyRelayManager>().joinCode = codeInput.text;
        _networkManager.ClientManager.StartConnection();
        loadingMenu.SetActive(true);
    }

    public void LeaveGame(bool _IsServer)
    {
        if (_IsServer)
        {
            _networkManager.ServerManager.StopConnection(true);
        }
        _networkManager.ClientManager.StopConnection();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
