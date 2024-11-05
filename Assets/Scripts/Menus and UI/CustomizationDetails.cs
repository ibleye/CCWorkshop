using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationDetails : MonoBehaviour
{
    public GameObject waitingText;
    public Button leaveButton;
    public Button startButton;
    public Button readyButton;
    public Button startButtonHighlight;
    public Button readyButtonHighlight;
    public TMP_Text joinCode;
    public Button quitButton;
    public victoryScreenCanvas vicScreen;

    public Sprite[] playerFrameSprites;
    public Sprite[] playerPictureSprites;

    public GameObject[] playerFrames;
    public GameObject[] playerPictures;
    public TMP_Dropdown[] sideSigs;
    public TMP_Dropdown[] neutralSigs;
    public TMP_Dropdown[] downSigs;

    public TMP_Text[] playerNameDisplays;

    private PlayerController controller;

    private playerDetails[] playerDetailsList = new playerDetails[4];
    public int[] playerDisplays = new int[4];

    private MapSelectionScreen mapSelectionScreen;

    
    public void Setup(GameObject player)
    {
        controller = player.GetComponent<PlayerController>();
        PlayerIdentity identity = player.GetComponent<PlayerIdentity>();
        SetDisplayValues(identity.playerNumber);
        

        joinCode.transform.parent.GetComponent<Button>().onClick.AddListener(identity.CopyCode);
        leaveButton.onClick.AddListener(identity.LeaveGame);
        vicScreen.leaveButton.onClick.AddListener(identity.LeaveGame);
        //startButton.onClick.AddListener(identity.RpcStartGame);
        quitButton.onClick.AddListener(identity.LeaveGame);
    }

    public void SetPlayerValues(int num, PlayerController _controller)
    {
        playerDetailsList[num - 1].sSig = GetDropdownValue(_controller.sideSig);
        playerDetailsList[num - 1].nSig = GetDropdownValue(_controller.neutralSig);
        playerDetailsList[num - 1].dSig = GetDropdownValue(_controller.downSig);
        playerDetailsList[num - 1].color = num;
        playerDetailsList[num - 1].ko = _controller.gameObject.GetComponent<PlayerIdentity>().playersKilled;
        playerDetailsList[num - 1].rank = _controller.gameObject.GetComponent<PlayerIdentity>().placement;
    }
    public void SetPlayerName(int playerNumber, string name)
    {
        //playerNameDisplays[playerNumber - 1].gameObject.SetActive(true);
        if (name == "default")
        {
            name = PlayerPrefs.GetString("Name", "PlayerName");
        }
        playerDetailsList[playerNumber - 1].name = name;
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        for (int i = 0; i < 4; i++)
        {
            if (playerDisplays[i] > 0 && playerDetailsList[playerDisplays[i]-1].color > 0)
            {
                sideSigs[i].value = playerDetailsList[playerDisplays[i]-1].sSig;
                neutralSigs[i].value = playerDetailsList[playerDisplays[i]-1].nSig;
                downSigs[i].value = playerDetailsList[playerDisplays[i]-1].dSig;
                playerFrames[i].GetComponent<Image>().sprite = playerFrameSprites[playerDetailsList[playerDisplays[i]-1].color - 1];
                playerPictures[i].GetComponent<Image>().sprite = playerPictureSprites[playerDetailsList[playerDisplays[i]-1].color - 1];
                playerNameDisplays[i].text = playerDetailsList[playerDisplays[i]-1].name;
                vicScreen.playerNameDisplays[i].text = playerDetailsList[playerDisplays[i]-1].name;
                vicScreen.playerKOCount[i].text = "KOs:            " + playerDetailsList[playerDisplays[i] - 1].ko;
                vicScreen.playerFrames[i].GetComponent<Image>().sprite = vicScreen.playerFrameSprites[playerDetailsList[playerDisplays[i] - 1].color - 1];
                vicScreen.playerPictures[i].GetComponent<Image>().sprite = vicScreen.playerPictureSprites[playerDetailsList[playerDisplays[i] - 1].color - 1];
            }
        }
    }

    private void SetDisplayValues(int playerNumber)
    {
        playerDisplays[0] = playerNumber;
        switch (playerNumber)
        {
            case 1:
                playerDisplays[1] = 2;
                playerDisplays[2] = 3;
                playerDisplays[3] = 4;
                break;
            case 2:
                playerDisplays[1] = 1;
                playerDisplays[2] = 3;
                playerDisplays[3] = 4;
                break;
            case 3:
                playerDisplays[1] = 1;
                playerDisplays[2] = 2;
                playerDisplays[3] = 4;
                break;
            case 4:
                playerDisplays[1] = 1;
                playerDisplays[2] = 2;
                playerDisplays[3] = 3;
                break;
            default:
                playerDisplays[0] = 1;
                playerDisplays[1] = 2;
                playerDisplays[2] = 3;
                playerDisplays[3] = 4;
                break;
        }
    }


    public void NewSideSig(int attack)
    {
        controller.NewChangeSideAttack(attack);
    }
    public void NewNeutralSig(int attack)
    {
        controller.NewChangeNeutralAttack(attack);
    }
    public void NewDownSig(int attack)
    {
        controller.NewChangeDownAttack(attack);
    }


    private int GetDropdownValue(attacks atk)
    {
        switch (atk)
        {
            case attacks.Aries:
                return 1;
            case attacks.Taurus:
                return 0;
            case attacks.Gemini:
                return 2;
            case attacks.Cancer:
                return 1;
            case attacks.Leo:
                return 3;
            case attacks.Virgo:
                return 1;
            case attacks.Libra:
                return 3;
            case attacks.Scorpio:
                return 0;
            case attacks.Sagittarius:
                return 2;
            case attacks.Capricorn:
                return 3;
            case attacks.Aquarius:
                return 0;
            case attacks.Pisces:
                return 2;
            default:
                return 0;
        }
    }
    public struct playerDetails
    {
        public int nSig;
        public int dSig;
        public int sSig;
        public int color;
        public string name;
        public int ko;
        public int rank;
    }

    public void ReadyUp()
    {
        controller.gameObject.GetComponent<PlayerIdentity>().ServerRpcReadyUp();
    }
}
