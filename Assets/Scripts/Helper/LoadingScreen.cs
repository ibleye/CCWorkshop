using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private GameObject[] LoadingStars;
    [SerializeField] private float yeildTime = 0.5f;

    private void OnEnable()
    {
        StartCoroutine(SpawnStars());
    }

    private IEnumerator SpawnStars()
    {
        foreach (var star in LoadingStars)
        {
            star.SetActive(true);
            yield return new WaitForSeconds(yeildTime);
        }
    }

    private void OnDisable()
    {
        foreach (var star in LoadingStars)
        {
            star.SetActive(false);
        }
    }
}
