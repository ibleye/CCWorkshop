using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jumpDestroy : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(SelfDestruct());
    }
    // Update is called once per frame
    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(0.7f);
        Destroy(gameObject);
    }
}
