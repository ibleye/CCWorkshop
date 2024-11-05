using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleCleanup : MonoBehaviour
{
    private float removeTime;
    public static float lifetime = 5f;

    // Start is called before the first frame update
    void Start()
    {
        removeTime = Time.time + lifetime;
    }

    // Update is called once per frame
    void Update()
    {
        if (removeTime < Time.time)
        {
            Destroy(this.gameObject);
        }
    }
}
