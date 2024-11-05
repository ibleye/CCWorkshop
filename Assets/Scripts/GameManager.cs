using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private PlayerIdentity[] players = new PlayerIdentity[4];
    List<PlayerIdentity> totalList = new List<PlayerIdentity>();
    List<PlayerIdentity> aliveList = new List<PlayerIdentity>();
    [SerializeField] List<Transform> spawnPoints = new List<Transform>();

    [SerializeField] private BridgeManager bridges;

    [SerializeField] bool soloPlayer = false;
    [SerializeField] private GameObject sandBagPrefab;

    private bool gameEnding = true;

    private void Update()
    {
        VictoryCheck();
    }


    public int AddPlayer(PlayerIdentity identity)
    {
        for (int i = 0; i < 4; i++)
        {
            if (players[i] == null)
            {
                players[i] = identity;
                totalList.Add(identity);
                return i + 1;
            }
        }
        Debug.Log("extra player assigned random color");
        totalList.Add(identity);
        return Random.Range(5, 9);
    }
    public void RemovePlayer(PlayerIdentity identity)
    {
        totalList.Remove(identity);
        if (identity.playerNumber > 0 && identity.playerNumber < 5)
        {
            players[identity.playerNumber - 1] = null;
        }
    }

    public void VictoryCheck()
    {
        int aliveCount = 0;
        aliveList.Clear();
        for (int i = 0; i < totalList.Count; i++)
        {
            if (totalList[i].isPlaying)
            {
                aliveList.Add(totalList[i]);
                aliveCount++;
            }
        }

        if (aliveCount <= 1 && !gameEnding && !soloPlayer)
        {
            aliveList[0].calculatePlacement();
            StartCoroutine(EndGameProcess(false, aliveList[0].playerName));
        }
        else if (aliveCount <= 0 && !gameEnding && soloPlayer)
        {
            StartCoroutine(EndGameProcess(true));
        }
    }
    IEnumerator EndGameProcess(bool _soloPlayer = false, string winnerName = "No One")
    {
        Debug.Log("end game process, winner is " + winnerName );
        for (int i = 0; i < totalList.Count; i++)
        {
            totalList[i].GetComponent<PlayerController>().RpcShowVictoryScreen(winnerName);
        }
        gameEnding = true;
        yield return new WaitForSeconds(5f);
        EndGame();
    }

    public void RestartGame()
    {
        gameEnding = false;
        for (int i = 0; i < totalList.Count; i++)
        {
            totalList[i].isSelecting = false;
            totalList[i].GetComponent<PlayerController>().RpcResetStocks();
            totalList[i].GetComponent<PlayerController>().RpcSpawn();
            totalList[i].GetComponent<PlayerController>().ResetWallSlides();
            totalList[i].GetComponent<PlayerIdentity>().RpcBridges();
            totalList[i].ServerRpcResetVictoryScreenStats();
            totalList[i].isPlaying = true;
        }
        if (soloPlayer)
        {
            if (sandBagPrefab)
            {
                sandBagPrefab.SetActive(true);
            }
        }
        else
        {
            if (sandBagPrefab)
            {
                sandBagPrefab.SetActive(false);
            }
        }
    }

    public void EndGame()
    {
        Debug.Log("end game");
        for (int i = 0; i < totalList.Count; i++)
        {
            totalList[i].GetComponent<PlayerController>().RpcResetStocks();
            totalList[i].GetComponent<PlayerController>().RpcRespawn();
            totalList[i].isReady = false;
            totalList[i].isSelecting = true;
            totalList[i].isPlaying = false;
        }
    }

    public void setSoloPlayer(bool _soloPlayer)
    {
        soloPlayer = _soloPlayer;
    }
}
