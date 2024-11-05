using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VersionText : MonoBehaviour
{
    void Start()
    {
        TMP_Text versionText = GetComponent<TMP_Text>();
        versionText.text = "v" + Application.version;
    }
}
