using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    private PlayerController controller;
    private Animator animator;


    [SerializeField] private ParticleSystem sprintParticleSystem;
    [SerializeField] private ParticleSystem knockbackParticleSystem;
    [SerializeField] private ParticleSystem[] deathParticleSystems;
    [SerializeField] private ParticleSystem hitVFX;
    [SerializeField] private GameObject cancerSmashSlamVFX;

    [Header("When Hits a Player")]
    [SerializeField] private GameObject downLightParticles;
    [SerializeField] private GameObject neutralLightParticles;
    [SerializeField] private GameObject sideLightParticles;
    [SerializeField] private GameObject AriesParticles;
    [SerializeField] private GameObject TaurusParticles;
    [SerializeField] private GameObject GeminiParticles;
    [SerializeField] private GameObject CancerParticles;
    [SerializeField] private GameObject LeoParticles;
    [SerializeField] private GameObject VirgoParticles;
    [SerializeField] private GameObject LibraParticles;
    [SerializeField] private GameObject ScorpioParticles;
    [SerializeField] private GameObject SagittariusParticles;
    [SerializeField] private GameObject CapricornParticles;
    [SerializeField] private GameObject AquariusParticles;
    [SerializeField] private GameObject PiscesParticles;

    [Header("When Starts Attack")]
    [SerializeField] private GameObject downLightParticlesI;
    [SerializeField] private GameObject neutralLightParticlesI;
    [SerializeField] private GameObject sideLightParticlesI;
    [SerializeField] private GameObject AriesParticlesI;
    [SerializeField] private GameObject TaurusParticlesI;
    [SerializeField] private GameObject GeminiParticlesI;
    [SerializeField] private GameObject CancerParticlesI;
    [SerializeField] private GameObject LeoParticlesI;
    [SerializeField] private GameObject VirgoParticlesI;
    [SerializeField] private GameObject LibraParticlesI;
    [SerializeField] private GameObject ScorpioParticlesI;
    [SerializeField] private GameObject SagittariusParticlesI;
    [SerializeField] private GameObject CapricornParticlesI;
    [SerializeField] private GameObject AquariusParticlesI;
    [SerializeField] private GameObject PiscesParticlesI;

    [Header("Timing Based Particles")]
    [SerializeField] private GameObject[] TimingAttacks;

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
        if (animator.GetInteger("State") == 3)
        {
            if (controller.faceDir)
            {
                sprintParticleSystem.gameObject.transform.rotation = Quaternion.Euler(-40,-90,0);
            }
            else
            {
                sprintParticleSystem.gameObject.transform.rotation = Quaternion.Euler(-40, 90, 0);

            }
            var main = sprintParticleSystem.main;
            main.loop = true;
            if (!sprintParticleSystem.isPlaying)
            {
                sprintParticleSystem.Play();
            }
        }
        else
        {
            var main = sprintParticleSystem.main;
            main.loop = false;
        }
        //----------------------------------------------------------------------------------------------knockback particles
        if (animator.GetInteger("State") == 4)
        {
            var main = knockbackParticleSystem.main;
            main.loop = true;
            if (!knockbackParticleSystem.isPlaying)
            {
                knockbackParticleSystem.Play();
            }
        }
        else
        {
            var main = knockbackParticleSystem.main;
            main.loop = false;
        }
    }

    public void PlayParticle(attacks attack)
    {
        switch (attack)
        {
            case attacks.downLight:
                 GameObject go = Instantiate(downLightParticles, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.neutralLight:
                 go = Instantiate(neutralLightParticles, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.sideLight:
                 go = Instantiate(sideLightParticles, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Aries:
                 go = Instantiate(AriesParticles, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Taurus:
                 go = Instantiate(TaurusParticles, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Gemini:
                 go = Instantiate(GeminiParticles, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Cancer:
                 go = Instantiate(CancerParticles, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Leo:
                 go = Instantiate(LeoParticles, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Virgo:
                 go = Instantiate(VirgoParticles, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Libra:
                 go = Instantiate(LibraParticles, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Scorpio:
                 go = Instantiate(ScorpioParticles, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Sagittarius:
                 go = Instantiate(SagittariusParticles, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Capricorn:
                 go = Instantiate(CapricornParticles, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Aquarius:
                 go = Instantiate(AquariusParticles, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Pisces:
                 go = Instantiate(PiscesParticles, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            default:
                break;
        }
    }

    public void PlayStartParticle(attacks attack)
    {
        switch (attack)
        {
            case attacks.downLight:
                GameObject go = Instantiate(downLightParticlesI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.neutralLight:
                go = Instantiate(neutralLightParticlesI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.sideLight:
                go = Instantiate(sideLightParticlesI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Aries:
                go = Instantiate(AriesParticlesI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Taurus:
                go = Instantiate(TaurusParticlesI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Gemini:
                go = Instantiate(GeminiParticlesI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Cancer:
                go = Instantiate(CancerParticlesI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Leo:
                go = Instantiate(LeoParticlesI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Virgo:
                go = Instantiate(VirgoParticlesI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Libra:
                go = Instantiate(LibraParticlesI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Scorpio:
                go = Instantiate(ScorpioParticlesI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Sagittarius:
                go = Instantiate(SagittariusParticlesI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Capricorn:
                go = Instantiate(CapricornParticlesI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Aquarius:
                go = Instantiate(AquariusParticlesI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            case attacks.Pisces:
                go = Instantiate(PiscesParticlesI, transform);
                go.AddComponent<ParticleCleanup>();
                break;
            default:
                break;
        }
    }

    public void PlayTimingParticle(int id)
    {
        GameObject go = Instantiate(TimingAttacks[id-1], transform);
        go.AddComponent<ParticleCleanup>();
    }

    public void SpawnDeathParticle(int playerNumber, Vector3 playerTransform)
    {
        GameObject go = Instantiate(deathParticleSystems[playerNumber-1].gameObject, new Vector3(playerTransform.x, playerTransform.y, playerTransform.z), Quaternion.Euler(new Vector3(-89.912f, 0, 0)));
        go.transform.LookAt(Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 100f))); 
        go.AddComponent<ParticleCleanup>();
    }

    public void SpawnHitVFX(Vector3 playerTransform, Vector3 hitterTransform)
    {
        GameObject go = Instantiate(hitVFX.gameObject, new Vector3(playerTransform.x, playerTransform.y, playerTransform.z), Quaternion.Euler(new Vector3(0, 0, 180f)));
        go.transform.LookAt(hitterTransform);
        go.AddComponent<ParticleCleanup>();
    }

    public void SpawnCancerSmashSlamVFX(Transform playerTransform)
    {
        Vector3 spawnTransform = new Vector3(playerTransform.position.x, playerTransform.position.y - 0.2f, playerTransform.position.z);
        GameObject go = Instantiate(cancerSmashSlamVFX, spawnTransform, Quaternion.identity);
        go.AddComponent<ParticleCleanup>();
    }
}
