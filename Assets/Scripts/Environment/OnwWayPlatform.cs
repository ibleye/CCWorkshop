using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnwWayPlatform : MonoBehaviour
{
    private PlatformEffector2D _platformEffecter2D;

    [SerializeField] float fallThroughBufferTime = 0.1f;
    private float timeBefore;
    private PlayerMoveset moveset;

    void Start()
    {
        moveset = FindObjectOfType<PlayerMoveset>();
        _platformEffecter2D = GetComponent<PlatformEffector2D>();
    }


    void Update()
    {
        Vector2 _directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        timeBefore += Time.deltaTime;
        if ((Input.GetAxisRaw("Fire2") > 0) || (Input.GetAxisRaw("Fire1") > 0)) { _platformEffecter2D.rotationalOffset = 0f;}
        else if ((_directionalInput.y < 0) && (Input.GetAxisRaw("Fire2") <= 0) && (Input.GetAxisRaw("Fire1") <= 0))
        {
            timeBefore = 0f;
            _platformEffecter2D.rotationalOffset = 180f;
        }
        else if (timeBefore > fallThroughBufferTime)
        {
            _platformEffecter2D.rotationalOffset = 0f;
        }
    }
}
