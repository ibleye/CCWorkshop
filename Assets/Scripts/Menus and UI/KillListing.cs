using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KillListing : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI KillerText;
    [SerializeField] private TextMeshProUGUI KilledText;
    [SerializeField] private Image howImage;
    [SerializeField] private Image KillerImage;
    [SerializeField] private Image KilledImage;
    [SerializeField] private float ExperationTime = 10f;

    void Start()
    {
        Destroy(gameObject, ExperationTime);
    }

    public void SetNames(string killer, string killed)
    {
        KillerText.text = killer;
        KilledText.text = killed;
    }

    public void SetHowImage(Sprite how)
    {
        Debug.Log("SetHowImage" + how);
        howImage.sprite = how;
    }

    public void SetKillerImage(Sprite killer)
    {
        KillerImage.sprite = killer;
    }

    public void SetKilledImage(Sprite killed)
    {
        KilledImage.sprite = killed;
    }
}
