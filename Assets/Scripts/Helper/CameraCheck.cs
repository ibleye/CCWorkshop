using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraCheck : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("CameraMove") != 1)
        {
            gameObject.GetComponent<CinemachineBrain>().enabled = false;
            gameObject.GetComponent<Camera>().orthographicSize = 16.3f;
        }
    }
}
