using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using TMPro;
using UnityEngine.UI;
using FishNet.Connection;
using FishNet.Transporting.FishyUTPPlugin;
using UnityEditor;

public class PlayerIdentity : NetworkBehaviour
{

    [SyncVar] public int playerNumber = 0;
    [SyncVar] public bool isPlaying = false;
    [SyncVar] public bool isSelecting = true;
    [SyncVar] public bool isReady = false;

    [SerializeField] private GameObject victoryScreen;

    [SyncVar] public string playerName = "default";

    private Animator animator;

    [SyncVar] private Transform spawnTransform;

    [SyncVar] private int lastHitPlayerNumber = 0;
    [SyncVar] public int playersKilled = 0;

    [SyncVar] private int deathCount = 0;
    [SyncVar] public int placement = 1;

    [SerializeField] public SpriteRenderer damageBorderSpriteRenderer;

    [SerializeField] private Sprite localPlayerDamageBorder;
    [SerializeField] private Sprite serverPlayerDamageBorder;

    [SerializeField] private TMP_FontAsset[] playerFonts;
    [SerializeField] private TextMeshProUGUI playerColorText;

    [SerializeField] private List<Sprite> abilityIcons;
    [SerializeField] private List<Sprite> StockUISpritesBlue;
    [SerializeField] private List<Sprite> StockUISpritesGreen;
    [SerializeField] private List<Sprite> StockUISpritesCyan;
    [SerializeField] private List<Sprite> StockUISpritesRed;

    private GameObject spawnPositions;
    private GameObject stocksUI;
    private GameObject personalStockUI;

    private PlayerIdentity[] players = new PlayerIdentity[4];

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject PlayerArrow;
    [SerializeField] private GameObject[] ScreenArrows;
    [SerializeField] private TMP_Text nameDisplay;

    private float forceNameTime = 0f;
    private int nameDisplayMode = 0;

    private GameManager gameManager;

    //[SerializeField] private GameObject waitingText;
    private CustomizationDetails customizationMenu;
    private GameObject MapSelectionScreen;
    //[SerializeField] private Button StartGameButton;
    //[SerializeField] private GameObject[] customizationMenuPlayerDesc;
    //[SerializeField] private TMP_Text joinCode;

    //[SerializeField] private TMP_Dropdown[] sideSigSelection;
    //[SerializeField] private TMP_Dropdown[] neutralSigSelection;
    //[SerializeField] private TMP_Dropdown[] downSigSelection;


    
    public override void OnStartClient()
    {
        base.OnStartClient();
        if (base.IsOwner)
        {
            RpcSetName(PlayerPrefs.GetString("Name", "PlayerName"));
            SetCustomizationMenu();
            customizationMenu.Setup(this.gameObject);
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
            //waitingText.transform.parent = GameObject.FindGameObjectWithTag("PlayerPanel").transform.parent;
            customizationMenu.joinCode.text = "ID: " + InstanceFinder.NetworkManager.gameObject.GetComponent<FishyRelayManager>().joinCode;
            PlayerArrow.SetActive(true);
            ScreenArrows[0].transform.parent.transform.parent = null;
            ScreenArrows[0].transform.parent.transform.position = Vector3.zero;
            ScreenArrows[0].transform.parent.gameObject.SetActive(true);
            if (MapSelectionScreen)
            {
                MapSelectionScreen.GetComponent<MapSelectionScreen>().mapSelectTextClient.SetActive(true);
                MapSelectionScreen.GetComponent<MapSelectionScreen>().mapSelectTextClientbackground.SetActive(true);
                MapSelectionScreen.GetComponent<MapSelectionScreen>().mapSelectTextServer.SetActive(false);
                MapSelectionScreen.GetComponent<MapSelectionScreen>().mapSelectTextServerbackground.SetActive(false);
            }
        }

    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        SetCustomizationMenu();
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        spawnPositions = GameObject.FindGameObjectWithTag("SpawnPositions");
        stocksUI = GameObject.FindGameObjectWithTag("StockUI");
        victoryScreen.SetActive(false);
        Debug.Log(stocksUI);
        foreach( Transform child in stocksUI.transform )
        {
            child.gameObject.SetActive(false);
        }
        playerNumber = gameManager.AddPlayer(this);
        lastHitPlayerNumber = playerNumber;
        Debug.Log("Updating last player hit! " + lastHitPlayerNumber);
        RpcUpdateColor(playerNumber);
        RpcUpdateSpawnPosition();
        RpcUpdateTextColor();
        animator = GetComponentInChildren<Animator>();
        animator.SetInteger("Color", playerNumber);
        customizationMenu.startButton.interactable = true;
        if (MapSelectionScreen)
        {
            MapSelectionScreen.GetComponent<MapSelectionScreen>().randomButton.interactable = true;
            MapSelectionScreen.GetComponent<MapSelectionScreen>().starySkyButton.interactable = true;
            MapSelectionScreen.GetComponent<MapSelectionScreen>().aruroaButton.interactable = true;
            MapSelectionScreen.GetComponent<MapSelectionScreen>().colosseumButton.interactable = true;
            MapSelectionScreen.GetComponent<MapSelectionScreen>().backButton.interactable = true;
            MapSelectionScreen.GetComponent<MapSelectionScreen>().mapSelectTextServer.SetActive(true);
            MapSelectionScreen.GetComponent<MapSelectionScreen>().mapSelectTextServerbackground.SetActive(true);
            MapSelectionScreen.GetComponent<MapSelectionScreen>().mapSelectTextClient.SetActive(false);
            MapSelectionScreen.GetComponent<MapSelectionScreen>().mapSelectTextClientbackground.SetActive(false);
        }
    }
    public override void OnStopServer()
    {
        base.OnStopServer();
        gameManager.RemovePlayer(this);
    }


    private void Start()
    {
        gameObject.name = "Player" + ObjectId;
        animator = GetComponentInChildren<Animator>();
        
        spawnPositions = GameObject.FindGameObjectWithTag("SpawnPositions");
        stocksUI = GameObject.FindGameObjectWithTag("StockUI");
        Debug.Log(victoryScreen);
        victoryScreen.SetActive(false);
        foreach( Transform child in stocksUI.transform )
        {
            child.gameObject.SetActive(false);
        }

        SupCamera movingCamera = GameObject.FindWithTag("MainCamera").GetComponent<SupCamera>();
        movingCamera.Players.Add(gameObject);

        //damageBorderSpriteRenderer.sprite = serverPlayerDamageBorder;
        
        //spriteRenderer.color = new Color(colorValues.x, colorValues.y, colorValues.z, 1f);
        //UIPercentText.transform.parent.GetComponent<Image>().color = new Color(colorValues.x, colorValues.y, colorValues.z, .5f);
        RpcRefreshColorCommand();
        RpcRefreshSpawnCommand();
        RpcRefreshTextColorCommand();

        nameDisplayMode = PlayerPrefs.GetInt("NameDisplayMode");
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Tab) || Input.GetKey(KeyCode.LeftShift))
        {
            nameDisplay.text = playerName;
            nameDisplay.gameObject.SetActive(true);
        }
        else
        {
            nameDisplay.gameObject.SetActive(false);
        }
        if (base.IsOwner && (forceNameTime > Time.time || nameDisplayMode == 1))
        {
            nameDisplay.text = playerName;
            nameDisplay.gameObject.SetActive(true);
        }
        if (!isPlaying)
        {
            resetNameTime();
        }


        if (!IsOwner)
        {
            return;
        }
        customizationMenu.waitingText.SetActive(!isPlaying);
        customizationMenu.gameObject.SetActive(isSelecting);
        if (isSelecting)
        {
            FindPlayers();
            customizationMenu.readyButton.gameObject.SetActive(true);
            if (isReady)
            {
                customizationMenu.readyButton.interactable = false;
                customizationMenu.readyButtonHighlight.gameObject.SetActive(false);
            }
            else
            {
                customizationMenu.readyButton.interactable = true;
                customizationMenu.readyButtonHighlight.gameObject.SetActive(true);
            }
            if (players[0] != null && !players[0].isReady) return;
            if (players[1] != null && !players[1].isReady) return;
            if (players[2] != null && !players[2].isReady) return;
            if (players[3] != null && !players[3].isReady) return;
            Debug.Log("all ready");
            customizationMenu.startButton.gameObject.SetActive(true);
            //customizationMenu.startButtonHighlight.gameObject.SetActive(true);
            customizationMenu.readyButton.gameObject.SetActive(false);
        }

        ScreenArrows[0].SetActive(false);
        ScreenArrows[1].SetActive(false);
        ScreenArrows[2].SetActive(false);
        ScreenArrows[3].SetActive(false);
        if (transform.position.x > 30)
        {
            ScreenArrows[3].transform.position = new Vector2(ScreenArrows[3].transform.position.x, transform.position.y);
            ScreenArrows[3].SetActive(true);
        }
        if (transform.position.x < -30)
        {
            ScreenArrows[2].transform.position = new Vector2(ScreenArrows[2].transform.position.x, transform.position.y);
            ScreenArrows[2].SetActive(true);
        }
        if (transform.position.y > 18)
        {
            ScreenArrows[0].transform.position = new Vector2(transform.position.x, ScreenArrows[0].transform.position.y);
            ScreenArrows[0].SetActive(true);
        }
        if (transform.position.y < -20)
        {
            ScreenArrows[1].transform.position = new Vector2(transform.position.x, ScreenArrows[1].transform.position.y);
            ScreenArrows[1].SetActive(true);
        }
    }

    public void resetNameTime()
    {
        forceNameTime = Time.time + 3f;
    }

    public void updateKillersList(string attackName)
    {
        players[lastHitPlayerNumber - 1].ServerRpcAddToKillList(playerNumber, playerName, attackName);
    }

    private int numPlayersStillAlive()
    {
        int numPlayersAlive = 0;
        foreach (PlayerIdentity player in players)
        {
            if (player != null)
            {
                Debug.Log(player.isPlaying);
                if (player.isPlaying)
                {
                    numPlayersAlive++;
                }
            }
            
        }
        return numPlayersAlive;
    }

    public void calculatePlacement()
    {
        ServerRpcUpdatePlacement(numPlayersStillAlive());
    }

    public void FindPlayers()
    {
        players = new PlayerIdentity[4];
        GameObject[]  _players = GameObject.FindGameObjectsWithTag("Player");
        foreach (Transform child in stocksUI.transform)
        {
            child.gameObject.SetActive(false);
        }
        foreach (GameObject player in _players)
        {
            stocksUI.transform.GetChild(player.GetComponent<PlayerIdentity>().playerNumber - 1).gameObject.SetActive(true);
            PlayerIdentity id = player.GetComponent<PlayerIdentity>();
            PlayerController controller = player.GetComponent<PlayerController>();
            if (id.playerNumber <= 4 && id.playerNumber >= 1)
            {
                players[id.playerNumber-1] = id;

                personalStockUI = stocksUI.transform.GetChild(id.playerNumber - 1).gameObject;
                RpcRefreshAbilityIconCommand();

                if (id.playerNumber == playerNumber)
                {
                    id.damageBorderSpriteRenderer.sprite = localPlayerDamageBorder;
                }
                else {id.damageBorderSpriteRenderer.sprite = serverPlayerDamageBorder;}

                customizationMenu.SetPlayerValues(id.playerNumber, controller);
            }
        }
        for (int i = 0; i < 4; i++)
        {
            customizationMenu.playerPictures[i].transform.parent.transform.parent.gameObject.SetActive(players[i]!=null);//enable matching elements obj if player != null
            customizationMenu.vicScreen.playerPictures[i].transform.parent.transform.parent.gameObject.SetActive(players[i] != null);//enable matching elements obj if player != null
        }
    }

    [ServerRpc(RequireOwnership = false)] public void RpcStartGame()
    {
        gameManager.RestartGame();
    }

    [ServerRpc(RequireOwnership = false)] private void RpcRefreshColorCommand()
    {
        RpcUpdateColor(playerNumber);
    }
    [ObserversRpc] private void RpcUpdateColor(int _playerColor)
    {
        Debug.Log("set " + transform.name + "'s color to " + _playerColor);
        //UIPercentText.transform.parent.GetComponent<Image>().color = new Color(_colorValues.x, _colorValues.y, _colorValues.z, .5f);
        animator.SetInteger("Color", _playerColor);
        GameObject.FindGameObjectWithTag("CustomizationMenu").GetComponent<CustomizationDetails>().SetPlayerName(_playerColor, playerName);
    }
    [ObserversRpc]public void RpcBridges()
    {
        GameObject.FindGameObjectWithTag("Colosseum").GetComponent<BridgeManager>().ResetBridge();
        GameObject.FindGameObjectWithTag("Colosseum").GetComponent<BridgeManager>().DelayStart();
    }
    [ServerRpc(RequireOwnership = false)] private void RpcRefreshSpawnCommand()
    {
        RpcUpdateSpawnPosition();
    }
    [ObserversRpc] private void RpcUpdateSpawnPosition()
    {
        spawnTransform = spawnPositions.transform.GetChild(playerNumber - 1).transform;
        //Debug.Log("set " + transform.name + "'s spawn position to " + spawnTransform.name);
    }
    [ServerRpc(RequireOwnership = false)] private void RpcRefreshTextColorCommand()
    {
        RpcUpdateTextColor();
    }
    [ObserversRpc] private void RpcUpdateTextColor()
    {
        playerColorText.font = playerFonts[playerNumber - 1];
    }
    [ServerRpc(RequireOwnership = false)] public void ServerRpcUpdateLastHitByPlayer(int playerNumber)
    {
        ObserverRpcUpdateLastHitByPlayer(playerNumber);
    }
    [ObserversRpc] private void ObserverRpcUpdateLastHitByPlayer(int playerNumber)
    {
        lastHitPlayerNumber = playerNumber;
        Debug.Log("Updating last player hit! " + lastHitPlayerNumber);
    }
    [ServerRpc(RequireOwnership = false)] public void ServerRpcAddToKillList(int playerNumberKilled, string playerNameKilled, string attackName)
    {
        ObserverRpcAddToKillList(playerNumberKilled, playerNameKilled, attackName);
    }
    [ObserversRpc] private void ObserverRpcAddToKillList(int playerNumberKilled, string playerNameKilled, string attackName)
    {
        playersKilled += 1;
        KillFeed.instance.AddKillListingWithImages(playerName, playerNameKilled, playerNumber - 1, playerNumberKilled - 1, attackName);
        Debug.Log(playerNumber + " just killed " + playerNumberKilled);
        Debug.Log(playersKilled);
    }
    [ServerRpc(RequireOwnership = false)] public void ServerRpcAddToDeathCount(int deathAmount)
    {
        ObserverRpcAddToDeathCount(deathAmount);
    }
    [ObserversRpc] private void ObserverRpcAddToDeathCount(int deathAmount)
    {
        deathCount += deathAmount;
        Debug.Log("Death count: " + deathCount);
    }
    [ServerRpc(RequireOwnership = false)] public void ServerRpcUpdatePlacement(int position)
    {
        ObserverRpcUpdatePlacement(position);
    }
    [ObserversRpc] private void ObserverRpcUpdatePlacement(int position)
    {
        placement = position;
        Debug.Log("player placed: " + placement);
    }
    [ServerRpc(RequireOwnership = false)] public void ServerRpcResetVictoryScreenStats()
    {
        ObserverRpcResetVictoryScreenStats();
    }
    [ObserversRpc] private void ObserverRpcResetVictoryScreenStats()
    {
        lastHitPlayerNumber = playerNumber;
        Debug.Log("Updating last player hit! " + lastHitPlayerNumber);
        playersKilled = 0;
        deathCount = 0;
        placement = 0;
        Debug.Log("reset victory screen stats");
    }
    [ServerRpc] public void RpcOutOfStocks()
    {
        isPlaying = false;
    }
    [ServerRpc(RequireOwnership = false)] private void RpcRefreshAbilityIconCommand()
    {
        RpcUpdateStockUIAbilityIcons();
    }
    [ObserversRpc] private void RpcUpdateStockUIAbilityIcons()
    {
        PlayerController controller = GetComponent<PlayerController>();
        int i = 0;
        personalStockUI = stocksUI.transform.GetChild(playerNumber - 1).gameObject;
        Debug.Log(personalStockUI);
        foreach( Transform child in personalStockUI.transform )
        {
            if (child.gameObject.tag != "AbilityIcon") {continue;}
            if (i == 0)
            {
                switch(controller.sideSig)
                {
                    case attacks.Leo:
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = abilityIcons[5];
                        break;
                    case attacks.Scorpio:
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = abilityIcons[9];
                        break;
                    case attacks.Pisces:
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = abilityIcons[7];
                        break;
                    case attacks.Virgo:
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = abilityIcons[11];
                        break;
                }
            }
            else if (i == 1)
            {
                switch(controller.neutralSig)
                {
                    case attacks.Taurus:
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = abilityIcons[10];
                        break;
                    case attacks.Sagittarius:
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = abilityIcons[8];
                        break;
                    case attacks.Cancer:
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = abilityIcons[2];
                        break;
                    case attacks.Libra:
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = abilityIcons[6];
                        break;
                }
            }
            else
            {
                switch(controller.downSig)
                {
                    case attacks.Aries:
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = abilityIcons[1];
                        break;
                    case attacks.Capricorn:
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = abilityIcons[3];
                        break;
                    case attacks.Gemini:
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = abilityIcons[4];
                        break;
                    case attacks.Aquarius:
                        child.gameObject.GetComponent<SpriteRenderer>().sprite = abilityIcons[0];
                        break;
                }
            }
            i++;
        }
    }
    [ServerRpc(RequireOwnership = false)] public void RpcUpdateUIStocksCommand(int numOfStocks)
    {
        RpcUpdateUIStockAmount(numOfStocks);
    }
    [ObserversRpc] private void RpcUpdateUIStockAmount(int numOfStocks)
    {
        Debug.Log("set " + transform.name + "'s stock amount to " + numOfStocks);
        personalStockUI = stocksUI.transform.GetChild(playerNumber - 1).gameObject;
        if (numOfStocks == 0)
        {
            personalStockUI.SetActive(false);
            return;
        }
        switch(playerNumber)
        {
            case 1:
                personalStockUI.GetComponent<SpriteRenderer>().sprite = StockUISpritesBlue[numOfStocks];
                break;
            case 2:
                personalStockUI.GetComponent<SpriteRenderer>().sprite = StockUISpritesGreen[numOfStocks];
                break;
            case 3:
                personalStockUI.GetComponent<SpriteRenderer>().sprite = StockUISpritesCyan[numOfStocks];
                break;
            case 4:
                personalStockUI.GetComponent<SpriteRenderer>().sprite = StockUISpritesRed[numOfStocks];
                break;
        }
        foreach( Transform child in personalStockUI.transform )
        {
            if (child.gameObject.tag == "LivesText")
            {
                child.gameObject.GetComponent<TextMeshProUGUI>().text = numOfStocks.ToString();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)] public void RpcStarrySky()
    {
        RpcSetStarrySky();
    }
    [ObserversRpc] private void RpcSetStarrySky()
    {
        GameObject StarrySky = GameObject.FindWithTag("StarrySky");
        foreach(Transform child in StarrySky.transform)
        {
            child.gameObject.SetActive(true);
        }
        GameObject Aurora = GameObject.FindWithTag("Aurora");
        foreach (Transform child in Aurora.transform)
        {
            child.gameObject.SetActive(false);
        }
        GameObject Colosseum = GameObject.FindWithTag("Colosseum");
        Colosseum.GetComponent<BridgeManager>().enabled = false;
        Colosseum.GetComponent<BridgeManager>().isActiveMap = false;
        foreach (Transform child in Colosseum.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    [ServerRpc(RequireOwnership = false)] public void RpcAurora()
    {
        RpcSetAurora();
    }
    [ObserversRpc] private void RpcSetAurora()
    {
        GameObject StarrySky = GameObject.FindWithTag("StarrySky");
        foreach(Transform child in StarrySky.transform)
        {
            child.gameObject.SetActive(false);
        }
        GameObject Aurora = GameObject.FindWithTag("Aurora");
        foreach (Transform child in Aurora.transform)
        {
            child.gameObject.SetActive(true);
        }
        GameObject Colosseum = GameObject.FindWithTag("Colosseum");
        Colosseum.GetComponent<BridgeManager>().enabled = false;
        Colosseum.GetComponent<BridgeManager>().isActiveMap = false;
        foreach (Transform child in Colosseum.transform)
        {
            child.gameObject.SetActive(false);
        }
    }
    [ServerRpc(RequireOwnership = false)] public void RpcColosseum()
    {
        RpcSetColosseum();
    }
    [ObserversRpc] private void RpcSetColosseum()
    {
        GameObject StarrySky = GameObject.FindWithTag("StarrySky");
        foreach(Transform child in StarrySky.transform)
        {
            child.gameObject.SetActive(false);
        }
        GameObject Aurora = GameObject.FindWithTag("Aurora");
        foreach (Transform child in Aurora.transform)
        {
            child.gameObject.SetActive(false);
        }
        GameObject Colosseum = GameObject.FindWithTag("Colosseum");
        Colosseum.GetComponent<BridgeManager>().enabled = true;
        Colosseum.GetComponent<BridgeManager>().ResetBridge();
        Colosseum.GetComponent<BridgeManager>().isActiveMap = true;
        foreach (Transform child in Colosseum.transform)
        {
            child.gameObject.SetActive(true);
        }
    }
    [ServerRpc(RequireOwnership = false)] public void RpcUpdateMapSelectServer(bool enableMapSelect)
    {
        RpcUpdateMapSelectObserver(enableMapSelect);
    }
    [ObserversRpc] private void RpcUpdateMapSelectObserver(bool enableMapSelect)
    {
        MapSelectionScreen = MapSelectionScreen ? MapSelectionScreen : Resources.FindObjectsOfTypeAll<MapSelectionScreen>()[0].gameObject;
        if (MapSelectionScreen != null)
        {
            MapSelectionScreen.SetActive(enableMapSelect);
        }
        else
        {
            Debug.Log("mapSelectScreen is null");
        }
    }



    public void LeaveGame()
    {
        if (base.IsServer)
        {
            InstanceFinder.NetworkManager.ServerManager.StopConnection(true);
        }
        InstanceFinder.NetworkManager.ClientManager.StopConnection();
    }
    public void CopyCode()
    {
        TextEditor te = new TextEditor();
        te.text = InstanceFinder.NetworkManager.gameObject.GetComponent<FishyRelayManager>().joinCode;
        te.SelectAll();
        te.Copy();
        //EditorGUIUtility.systemCopyBuffer = InstanceFinder.NetworkManager.gameObject.GetComponent<FishyRelayManager>().joinCode;
    }

    public void SetCustomizationMenu()
    {
        if (customizationMenu == null)
        {
            customizationMenu = GameObject.FindGameObjectWithTag("CustomizationMenu").GetComponent<CustomizationDetails>();
        }
        MapSelectionScreen = MapSelectionScreen ? MapSelectionScreen : Resources.FindObjectsOfTypeAll<MapSelectionScreen>()[0].gameObject;
    }

    [ServerRpc] public void RpcSetName(string _playerName)
    {
        playerName = _playerName;
    }

    public Transform GetSpawnTransform()
    {
        if (spawnTransform == null)
        {
            spawnTransform = spawnPositions.transform.GetChild(playerNumber - 1).transform;
            Debug.LogWarning("something went wrong with the spawnpoints DANIEL");
        }
        return spawnTransform;
    }

    [ServerRpc(RequireOwnership = true)]
    public void ServerRpcReadyUp()
    {
        isReady = true;
    }
}
