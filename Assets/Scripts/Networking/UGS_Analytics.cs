using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Analytics;
using Unity.Services.Core;
using Unity.Services.Core.Analytics;
public class UGS_Analytics : MonoBehaviour
{
    async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (ConsentCheckException e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void PlayerDeathCustomEvent(string lastHitBy, int percentage, float timeAlive)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>
        {
            { "lastHitBy", lastHitBy },
            { "percentage", percentage },
            { "timeAlive", timeAlive }
        };
        AnalyticsService.Instance.CustomData("playerDeath", parameters);
        AnalyticsService.Instance.Flush();
    }
}