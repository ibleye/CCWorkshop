using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BridgeManager : MonoBehaviour
{
    [SerializeField] private Animator bridge0;
    [SerializeField] private Animator bridge1;

    [SerializeField] private GameObject bridge0hb;
    [SerializeField] private GameObject bridge1hb;

    private PlayerIdentity[] players = new PlayerIdentity[4];
    private int stockCount;
    private int playerCount;
    private bool finished = false;
    private bool started = false;
    private bool animPlaying = false;
    public bool isActiveMap = false;

    public int bridgeState = 0;


    private void OnEnable()
    {
        ResetBridge();
    }

    private void MapCheck()
    {
        if (isActiveMap) return;
        bridge0.gameObject.SetActive(false);
        bridge1.gameObject.SetActive(false);
        bridge0hb.SetActive(false);
        bridge1hb.SetActive(false);
    }

    public void ResetBridge()
    {
        bridgeState = 0;
        finished = false;
        started = false;
        bridge0hb.SetActive(true);
        bridge1hb.SetActive(true);
        bridge0.gameObject.SetActive(true);
        bridge1.gameObject.SetActive(true);
        
        bridge0.ResetTrigger("50hp");
        bridge1.ResetTrigger("50hp");
        bridge0.ResetTrigger("25hp");
        bridge1.ResetTrigger("25hp");
        bridge0.ResetTrigger("0hp");
        bridge1.ResetTrigger("0hp");
        MapCheck();
        StartCoroutine(BridgeState());
    }
    public void DelayStart()
    {
        StartCoroutine(DelayedStart());
    }

    // Update is called once per frame
    void Update()
    {
        MapCheck();
        if (finished || !started) { return; }
        stockCount = 0;
        playerCount = 0;
        players = new PlayerIdentity[4];
        GameObject[] _players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in _players)
        {
            stockCount += player.GetComponent<PlayerController>().stocks+1;
            playerCount++;
        }
        if (stockCount == 0 || playerCount == 0) { return; }

        if (stockCount <= playerCount * 2f && bridgeState == 0)
        {
            bridge0.SetTrigger("50hp");
            bridge1.SetTrigger("50hp");
            StartCoroutine(BridgeState());
            bridgeState++;
        }
        else if (stockCount <= playerCount * 1.5f && !animPlaying && bridgeState == 1)
        {
            bridge0.SetTrigger("25hp");
            bridge1.SetTrigger("25hp");
            bridge0.ResetTrigger("50hp");
            bridge1.ResetTrigger("50hp");
            StartCoroutine(BridgeState());
            bridgeState++;
        }
        else if (stockCount <= playerCount * 1f && !animPlaying && bridgeState == 2)
        {
            bridge0.SetTrigger("0hp");
            bridge1.SetTrigger("0hp");
            bridge0.ResetTrigger("25hp");
            bridge1.ResetTrigger("25hp");
            finished = true;
            StartCoroutine(BreakBridge());
            bridgeState++;
        }
    }

    IEnumerator BridgeResetDelay()
    {
        animPlaying = true;
        yield return new WaitForSeconds(.33f);
        animPlaying = false;
        ResetBridge();
    }

    IEnumerator BridgeState()
    {
        animPlaying = true;
        yield return new WaitForSeconds(.33f);
        animPlaying = false;
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(.33f);
        started = true;
    }

    IEnumerator BreakBridge()
    {
        yield return new WaitForSeconds(.33f);
        bridge0hb.SetActive(false);
        bridge1hb.SetActive(false);
        bridge0.gameObject.SetActive(false);
        bridge1.gameObject.SetActive(false);
        bridge0.ResetTrigger("0hp");
        bridge1.ResetTrigger("0hp");
    }
}
