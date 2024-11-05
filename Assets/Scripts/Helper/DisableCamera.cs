using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableCamera : MonoBehaviour
{
    void Start()
    {
        Debug.Log("CameraMove: " + PlayerPrefs.GetInt("CameraMove", 1));
        if (PlayerPrefs.GetInt("CameraMove", 1) == 0)
        {
            GetComponent<SupCamera>().enabled = false;
        }
        else
        {
            GetComponent<SupCamera>().enabled = true;
        }
    }
}
