using FishNet;
using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerStarter : NetworkBehaviour
{
    [SerializeField] private GameObject LoadingScreen;
    public override void OnStartServer()
    {
        base.OnStartServer();
        LoadingScreen.SetActive(true);
        InstanceFinder.ClientManager.StartConnection();
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        LoadingScreen.SetActive(false);
    }
}
