using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    private PlayerController controller;
    private Animator animator;


    [SerializeField] private AudioSource sprintAudioSystem;
    [SerializeField] private AudioSource knockbackAudioSystem;


    [SerializeField] private GameObject deathSoundAudio;

    [Header("When Hits a Player")]
    [SerializeField] private GameObject downLightAudio;
    [SerializeField] private GameObject neutralLightAudio;
    [SerializeField] private GameObject sideLightAudio;
    [SerializeField] private GameObject AriesAudio;
    [SerializeField] private GameObject TaurusAudio;
    [SerializeField] private GameObject GeminiAudio;
    [SerializeField] private GameObject CancerAudio;
    [SerializeField] private GameObject LeoAudio;
    [SerializeField] private GameObject VirgoAudio;
    [SerializeField] private GameObject LibraAudio;
    [SerializeField] private GameObject ScorpioAudio;
    [SerializeField] private GameObject SagittariusAudio;
    [SerializeField] private GameObject CapricornAudio;
    [SerializeField] private GameObject AquariusAudio;
    [SerializeField] private GameObject PiscesAudio;

    [Header("When Starts Attack")]
    [SerializeField] private GameObject downLightAudioI;
    [SerializeField] private GameObject neutralLightAudioI;
    [SerializeField] private GameObject sideLightAudioI;
    [SerializeField] private GameObject AriesAudioI;
    [SerializeField] private GameObject TaurusAudioI;
    [SerializeField] private GameObject GeminiAudioI;
    [SerializeField] private GameObject CancerAudioI;
    [SerializeField] private GameObject LeoAudioI;
    [SerializeField] private GameObject VirgoAudioI;
    [SerializeField] private GameObject LibraAudioI;
    [SerializeField] private GameObject ScorpioAudioI;
    [SerializeField] private GameObject SagittariusAudioI;
    [SerializeField] private GameObject CapricornAudioI;
    [SerializeField] private GameObject AquariusAudioI;
    [SerializeField] private GameObject PiscesAudioI;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //----------------------------------------------------------------------------------------------run particles
        /**if (animator.GetInteger("State") == 3)
        {
            if (controller.faceDir)
            {
                sprintAudioSystem.gameObject.transform.rotation = Quaternion.Euler(-40,-90,0);
            }
            else
            {
                sprintAudioSystem.gameObject.transform.rotation = Quaternion.Euler(-40, 90, 0);

            }
            var main = sprintAudioSystem.main;
            main.loop = true;
            if (!sprintAudioSystem.isPlaying)
            {
                sprintAudioSystem.Play();
            }
        }
        else
        {
            var main = sprintAudioSystem.main;
            main.loop = false;
        }**/
        //----------------------------------------------------------------------------------------------knockback particles
        /**if (animator.GetInteger("State") == 4)
        {
            var main = knockbackAudioSystem.main;
            main.loop = true;
            if (!knockbackAudioSystem.isPlaying)
            {
                knockbackAudioSystem.Play();
            }
        }
        else
        {
            var main = knockbackAudioSystem.main;
            main.loop = false;
        }
        **/
    }

    public void PlaySound(attacks attack)
    {
        switch (attack)
        {
            case attacks.downLight:
                 GameObject go = Instantiate(downLightAudio, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.neutralLight:
                 go = Instantiate(neutralLightAudio, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.sideLight:
                 go = Instantiate(sideLightAudio, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Aries:
                 go = Instantiate(AriesAudio, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Taurus:
                 go = Instantiate(TaurusAudio, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Gemini:
                 go = Instantiate(GeminiAudio, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Cancer:
                 go = Instantiate(CancerAudio, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Leo:
                 go = Instantiate(LeoAudio, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Virgo:
                 go = Instantiate(VirgoAudio, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Libra:
                 go = Instantiate(LibraAudio, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Scorpio:
                 go = Instantiate(ScorpioAudio, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Sagittarius:
                 go = Instantiate(SagittariusAudio, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Capricorn:
                 go = Instantiate(CapricornAudio, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Aquarius:
                 go = Instantiate(AquariusAudio, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Pisces:
                 go = Instantiate(PiscesAudio, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            default:
                break;
        }
    }

    public void PlayStartSound(attacks attack)
    {
        switch (attack)
        {
            case attacks.downLight:
                GameObject go = Instantiate(downLightAudioI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.neutralLight:
                go = Instantiate(neutralLightAudioI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.sideLight:
                go = Instantiate(sideLightAudioI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Aries:
                go = Instantiate(AriesAudioI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Taurus:
                go = Instantiate(TaurusAudioI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Gemini:
                go = Instantiate(GeminiAudioI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Cancer:
                go = Instantiate(CancerAudioI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Leo:
                go = Instantiate(LeoAudioI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Virgo:
                go = Instantiate(VirgoAudioI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Libra:
                go = Instantiate(LibraAudioI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Scorpio:
                go = Instantiate(ScorpioAudioI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Sagittarius:
                go = Instantiate(SagittariusAudioI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Capricorn:
                go = Instantiate(CapricornAudioI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Aquarius:
                go = Instantiate(AquariusAudioI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Pisces:
                go = Instantiate(PiscesAudioI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            default:
                break;
        }
    }

    public void SpawnDeathParticle(int playerNumber, Vector3 playerTransform)
    {
        Debug.Log("Player " + playerNumber + " died");
        GameObject go = Instantiate(deathSoundAudio, new Vector3(playerTransform.x, playerTransform.y, playerTransform.z), Quaternion.Euler(new Vector3(-89.912f, 0, 0)));
        go.transform.LookAt(Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 100f))); 
        //go.transform.rotation = Quaternion.Euler(new Vector3(-89.912f, go.transform.rotation.y, go.transform.rotation.z));
        go.AddComponent<ParticleCleanup>();
    }
}
