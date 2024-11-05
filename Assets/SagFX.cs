using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SagFX : MonoBehaviour
{

    public int Uses;
    public float cooldown, distance, speed, destinationMultiplyer, cameraHeight;
    public Text UIText;
    public Transform cam;
    public LayerMask LayerMask;

    int MaxUses;
    float CoolDownTimer;
    bool blinking = false;
    Vector3 destination;
    ParticleSystem Trail;

    // Start is called before the first frame update
    void Start()
    {
        
        MaxUses = Uses;
        CoolDownTimer = cooldown;
        UIText.text = Uses.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            blink();
        }

        if (CoolDownTimer > 0)
        {
            CoolDownTimer -= Time.deltaTime;
        }
        else
        {
            Uses += 1;
            CoolDownTimer = cooldown;
            UIText.text = Uses.ToString();
        }

        if (blinking)
        {
            
           
        }
    }

    void blink()
    {
        if (Uses > 0)
        {
            Uses -= 1;
            UIText.text = Uses.ToString();
            

            
         

            destination.y += cameraHeight;
            blinking = true;
        }
    }
}
