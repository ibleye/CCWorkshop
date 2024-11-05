using FishNet;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveset : NetworkBehaviour
{
    private PlayerController playerController;
    private Rigidbody2D rb;
    private ParticleManager particleManager;
    private int animFramerate = 12;
    private bool isAttacking = false;
    [SerializeField] private GameObject attackBoxPrefab;
    [SerializeField] private bool debugMode = false;
    [SerializeField] private Color hitboxColor;
    [HideInInspector] public GameObject sagArrow;
    public int playerNumber;

    private List<GameObject> hitboxes = new List<GameObject>();

    [SerializeField] private AttackDetails[] downLightDetails;
    [SerializeField] private AttackDetails[] neutralLightDetails;
    [SerializeField] private AttackDetails[] sideLightDetails;
    [SerializeField] private AttackDetails[] ariesDetails;
    [SerializeField] private AttackDetails[] taurusDetails;
    [SerializeField] private AttackDetails[] geminiDetails;
    [SerializeField] private AttackDetails[] cancerDetails;
    [SerializeField] private AttackDetails[] leoDetails;
    [SerializeField] private AttackDetails[] virgoDetails;
    [SerializeField] private AttackDetails[] libraDetails;
    [SerializeField] private AttackDetails[] scorpioDetails;
    [SerializeField] private AttackDetails[] sagittariusDetails;
    [SerializeField] private AttackDetails[] capricornDetails;
    [SerializeField] private AttackDetails[] capricornAirDetails;
    [SerializeField] private AttackDetails[] aquariusDetails;
    [SerializeField] private AttackDetails[] piscesDetails;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        particleManager = GetComponent<ParticleManager>();
        playerNumber = GetComponent<PlayerIdentity>().playerNumber;
    }

    [ServerRpc]
    public void RpcSpawnProjectile(GameObject projectile, Vector3 location, Quaternion rotation, NetworkConnection conn = null)
    {
        GameObject go = Instantiate(projectile, location, rotation);
        go.GetComponent<Projectile>().SetOwner(this);
        go.GetComponent<Projectile>().faceDir = playerController.faceDir;
        go.GetComponent<Projectile>().color = GetComponent<PlayerIdentity>().playerNumber;
        InstanceFinder.ServerManager.Spawn(go, conn);
    }

    [ServerRpc]
    public void RpcServerStartTimingParticle(int particleID)
    {
        RpcObserverStartTimingParticle(particleID);
    }
    [ObserversRpc]
    public void RpcObserverStartTimingParticle(int particleID)
    {
        particleManager.PlayTimingParticle(particleID);
    }

    public void Interupt()
    {
        StopAllCoroutines();
        playerController.moveWhileStun = false;
        playerController.stun = false;
        playerController.SetNextAttackTime(Time.time + 0.2f);
        isAttacking = false;
        foreach (GameObject box in hitboxes)
        {
            Destroy(box);
        }
        hitboxes = new List<GameObject>();
    }


    public void StartAttack(bool faceDir, attacks attack)//false is left, true is right
    {
        switch (attack)
        {
            case attacks.downLight:
                foreach (var stage in downLightDetails)
                {
                    StartCoroutine(EnableAttack(faceDir, stage, attack));
                }
                break;
            case attacks.neutralLight:
                foreach (var stage in neutralLightDetails)
                {
                    StartCoroutine(EnableAttack(faceDir, stage, attack));
                }
                break;
            case attacks.sideLight:
                foreach (var stage in sideLightDetails)
                {
                    StartCoroutine(EnableAttack(faceDir, stage, attack));
                }
                break;
            case attacks.Aries:
                foreach (var stage in ariesDetails)
                {
                    StartCoroutine(EnableAttack(faceDir, stage, attack));
                }
                break;
            case attacks.Taurus:
                foreach (var stage in taurusDetails)
                {
                    StartCoroutine(EnableAttack(faceDir, stage, attack));
                }
                break;
            case attacks.Gemini:
                foreach (var stage in geminiDetails)
                {
                    StartCoroutine(EnableAttack(faceDir, stage, attack));
                }
                break;
            case attacks.Cancer:
                foreach (var stage in cancerDetails)
                {
                    StartCoroutine(EnableAttack(faceDir, stage, attack));
                }
                break;
            case attacks.Leo:
                foreach (var stage in leoDetails)
                {
                    StartCoroutine(EnableAttack(faceDir, stage, attack));
                }
                break;
            case attacks.Virgo:
                foreach (var stage in virgoDetails)
                {
                    StartCoroutine(EnableAttack(faceDir, stage, attack));
                }
                break;
            case attacks.Libra:
                foreach (var stage in libraDetails)
                {
                    StartCoroutine(EnableAttack(faceDir, stage, attack));
                }
                break;
            case attacks.Scorpio:
                foreach (var stage in scorpioDetails)
                {
                    StartCoroutine(EnableAttack(faceDir, stage, attack));
                }
                break;
            case attacks.Sagittarius:
                if (sagArrow != null)
                {
                    transform.position = sagArrow.transform.position;
                    rb.velocity = Vector2.zero;
                    sagArrow.GetComponent<SpriteRenderer>().enabled = false;
                    InstanceFinder.ServerManager.Despawn(sagArrow);
                    sagArrow = null;
                    break;
                }
                foreach (var stage in sagittariusDetails)
                {
                    StartCoroutine(EnableAttack(faceDir, stage, attack));
                }
                break;
            case attacks.Capricorn:
                if (playerController.isGrounded)
                {
                    foreach (var stage in capricornDetails)
                    {
                        StartCoroutine(EnableAttack(faceDir, stage, attack));
                    }
                }
                else
                {
                    foreach (var stage in capricornAirDetails)
                    {
                        StartCoroutine(EnableAttack(faceDir, stage, attack));
                    }
                }
                break;
            case attacks.Aquarius:
                foreach (var stage in aquariusDetails)
                {
                    StartCoroutine(EnableAttack(faceDir, stage, attack));
                }
                break;
            case attacks.Pisces:
                foreach (var stage in piscesDetails)
                {
                    StartCoroutine(EnableAttack(faceDir, stage, attack));
                }
                break;
            default:
                break;
        }
    }

    IEnumerator EnableAttack(bool faceDir, AttackDetails details, attacks attack)
    {
        GameObject attackBox = null;
        playerController.stun = true;
        isAttacking = true;

        yield return new WaitForSeconds((float)details.windupFrames / animFramerate);//--------------------------------------time before anything activates

        if (details.resetVelocity)//reset velocity if checked
        {
            rb.velocity = Vector2.zero;
        }

        if (details.moveDuringLifetime)//let player move during lifetime of this attackdetail if checked
        {
            playerController.moveWhileStun = true;
        }

        if (details.projectileR != null)//spawn projectile if it is set
        {
            GameObject projectile = details.projectileR;

            if (details.spawnProjectileMiddle)
            {
                RpcSpawnProjectile(projectile, new Vector3(transform.position.x, transform.position.y), Quaternion.identity);
            }
            else
            {
                RpcSpawnProjectile(projectile, new Vector3(transform.position.x, transform.position.y - (transform.localScale.y / 2)), Quaternion.identity);
            }
            
        }

        if (details.ParticleSystemID != 0)//spawn particles if it is set
        {
            RpcServerStartTimingParticle(details.ParticleSystemID);
        }

        if (details.hasHitbox)//spawn in hitbox and set variables
        {
            attackBox = Instantiate(attackBoxPrefab, transform);
            hitboxes.Add(attackBox);
            AttackBox hitboxScript = attackBox.GetComponent<AttackBox>();

            hitboxScript.name = details.hitboxName;
            hitboxScript.attackForceBase = details.attackForceBase;
            hitboxScript.attackForceScale = details.attackForceScale;
            hitboxScript.attackDirection = details.attackDirection;
            hitboxScript.knockbackStunTime = details.knockbackStunTime;
            hitboxScript.attackDMG = details.attackDMG;
            hitboxScript.attack = attack;
            if (details.projectileR != null)
            {
                hitboxScript.disjointedHitbox = true;
            }

            hitboxScript.transform.localPosition = details.pos;
            hitboxScript.transform.rotation = Quaternion.Euler(0, 0, details.rot.z);
            hitboxScript.transform.localScale = details.sca;

            if (!faceDir)//if facing left invert x
            {
                hitboxScript.attackDirection = new Vector2(-hitboxScript.attackDirection.x, hitboxScript.attackDirection.y);
                hitboxScript.transform.rotation = Quaternion.Euler(0, 0, -details.rot.z);
                hitboxScript.transform.localPosition = new Vector3(-details.pos.x, details.pos.y, details.pos.z);
            }
        }

        if (faceDir)//apply self knockback in correct direction
        {
            rb.AddForce(details.pushDirection.normalized * details.pushForce);
        }
        else
        {
            rb.AddForce(new Vector2(details.pushDirection.normalized.x * -1, details.pushDirection.normalized.y) * details.pushForce);
        }

        yield return new WaitForSeconds((float)details.lifetimeFrames / animFramerate);//--------------------------------------time hitbox is active

        if (details.moveDuringLifetime)//stop letting the player move
        {
            playerController.moveWhileStun = false;
        }
        
        if (attackBox != null)
        {
            hitboxes.Remove(attackBox);
            Destroy(attackBox);
        }

        yield return new WaitForSeconds((float)details.endLagFrames / animFramerate);//--------------------------------------time before end of attack

        playerController.stun = false;
        playerController.SetNextAttackTime(Time.time + .2f);
        isAttacking = false;
    }

    public void OnDrawGizmos()
    {
        if (!debugMode) { return; }
        Gizmos.color = hitboxColor;

        foreach (var stage in downLightDetails)
        {
            if (stage.showHitboxGizmo)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(new Vector3(stage.pos.x * transform.lossyScale.x, stage.pos.y * transform.lossyScale.y, stage.pos.z * transform.lossyScale.z) + transform.position, Quaternion.Euler(stage.rot), new Vector3(stage.sca.x * transform.lossyScale.x, stage.sca.y * transform.lossyScale.y, stage.sca.z * transform.lossyScale.z));
                Gizmos.matrix = rotationMatrix;

                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }
        foreach (var stage in neutralLightDetails)
        {
            if (stage.showHitboxGizmo)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(new Vector3(stage.pos.x * transform.lossyScale.x, stage.pos.y * transform.lossyScale.y, stage.pos.z * transform.lossyScale.z) + transform.position, Quaternion.Euler(stage.rot), new Vector3(stage.sca.x * transform.lossyScale.x, stage.sca.y * transform.lossyScale.y, stage.sca.z * transform.lossyScale.z));
                Gizmos.matrix = rotationMatrix;

                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }

        foreach (var stage in sideLightDetails)
        {
            if (stage.showHitboxGizmo)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(new Vector3(stage.pos.x * transform.lossyScale.x, stage.pos.y * transform.lossyScale.y, stage.pos.z * transform.lossyScale.z) + transform.position, Quaternion.Euler(stage.rot), new Vector3(stage.sca.x * transform.lossyScale.x, stage.sca.y * transform.lossyScale.y, stage.sca.z * transform.lossyScale.z));
                Gizmos.matrix = rotationMatrix;

                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }

        foreach (var stage in ariesDetails)
        {
            if (stage.showHitboxGizmo)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(new Vector3(stage.pos.x * transform.lossyScale.x, stage.pos.y * transform.lossyScale.y, stage.pos.z * transform.lossyScale.z) + transform.position, Quaternion.Euler(stage.rot), new Vector3(stage.sca.x * transform.lossyScale.x, stage.sca.y * transform.lossyScale.y, stage.sca.z * transform.lossyScale.z));
                Gizmos.matrix = rotationMatrix;

                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }

        foreach (var stage in taurusDetails)
        {
            if (stage.showHitboxGizmo)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(new Vector3(stage.pos.x * transform.lossyScale.x, stage.pos.y * transform.lossyScale.y, stage.pos.z * transform.lossyScale.z) + transform.position, Quaternion.Euler(stage.rot), new Vector3(stage.sca.x * transform.lossyScale.x, stage.sca.y * transform.lossyScale.y, stage.sca.z * transform.lossyScale.z));
                Gizmos.matrix = rotationMatrix;

                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }

        foreach (var stage in geminiDetails)
        {
            if (stage.showHitboxGizmo)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(new Vector3(stage.pos.x * transform.lossyScale.x, stage.pos.y * transform.lossyScale.y, stage.pos.z * transform.lossyScale.z) + transform.position, Quaternion.Euler(stage.rot), new Vector3(stage.sca.x * transform.lossyScale.x, stage.sca.y * transform.lossyScale.y, stage.sca.z * transform.lossyScale.z));
                Gizmos.matrix = rotationMatrix;

                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }

        foreach (var stage in cancerDetails)
        {
            if (stage.showHitboxGizmo)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(new Vector3(stage.pos.x * transform.lossyScale.x, stage.pos.y * transform.lossyScale.y, stage.pos.z * transform.lossyScale.z) + transform.position, Quaternion.Euler(stage.rot), new Vector3(stage.sca.x * transform.lossyScale.x, stage.sca.y * transform.lossyScale.y, stage.sca.z * transform.lossyScale.z));
                Gizmos.matrix = rotationMatrix;

                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }

        foreach (var stage in leoDetails)
        {
            if (stage.showHitboxGizmo)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(new Vector3(stage.pos.x * transform.lossyScale.x, stage.pos.y * transform.lossyScale.y, stage.pos.z * transform.lossyScale.z) + transform.position, Quaternion.Euler(stage.rot), new Vector3(stage.sca.x * transform.lossyScale.x, stage.sca.y * transform.lossyScale.y, stage.sca.z * transform.lossyScale.z));
                Gizmos.matrix = rotationMatrix;

                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }

        foreach (var stage in virgoDetails)
        {
            if (stage.showHitboxGizmo)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(new Vector3(stage.pos.x * transform.lossyScale.x, stage.pos.y * transform.lossyScale.y, stage.pos.z * transform.lossyScale.z) + transform.position, Quaternion.Euler(stage.rot), new Vector3(stage.sca.x * transform.lossyScale.x, stage.sca.y * transform.lossyScale.y, stage.sca.z * transform.lossyScale.z));
                Gizmos.matrix = rotationMatrix;

                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }

        foreach (var stage in libraDetails)
        {
            if (stage.showHitboxGizmo)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(new Vector3(stage.pos.x * transform.lossyScale.x, stage.pos.y * transform.lossyScale.y, stage.pos.z * transform.lossyScale.z) + transform.position, Quaternion.Euler(stage.rot), new Vector3(stage.sca.x * transform.lossyScale.x, stage.sca.y * transform.lossyScale.y, stage.sca.z * transform.lossyScale.z));
                Gizmos.matrix = rotationMatrix;

                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }

        foreach (var stage in scorpioDetails)
        {
            if (stage.showHitboxGizmo)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(new Vector3(stage.pos.x * transform.lossyScale.x, stage.pos.y * transform.lossyScale.y, stage.pos.z * transform.lossyScale.z) + transform.position, Quaternion.Euler(stage.rot), new Vector3(stage.sca.x * transform.lossyScale.x, stage.sca.y * transform.lossyScale.y, stage.sca.z * transform.lossyScale.z));
                Gizmos.matrix = rotationMatrix;

                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }

        foreach (var stage in sagittariusDetails)
        {
            if (stage.showHitboxGizmo)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(new Vector3(stage.pos.x * transform.lossyScale.x, stage.pos.y * transform.lossyScale.y, stage.pos.z * transform.lossyScale.z) + transform.position, Quaternion.Euler(stage.rot), new Vector3(stage.sca.x * transform.lossyScale.x, stage.sca.y * transform.lossyScale.y, stage.sca.z * transform.lossyScale.z));
                Gizmos.matrix = rotationMatrix;

                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }

        foreach (var stage in capricornDetails)
        {
            if (stage.showHitboxGizmo)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(new Vector3(stage.pos.x * transform.lossyScale.x, stage.pos.y * transform.lossyScale.y, stage.pos.z * transform.lossyScale.z) + transform.position, Quaternion.Euler(stage.rot), new Vector3(stage.sca.x * transform.lossyScale.x, stage.sca.y * transform.lossyScale.y, stage.sca.z * transform.lossyScale.z));
                Gizmos.matrix = rotationMatrix;

                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }

        foreach (var stage in aquariusDetails)
        {
            if (stage.showHitboxGizmo)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(new Vector3(stage.pos.x * transform.lossyScale.x, stage.pos.y * transform.lossyScale.y, stage.pos.z * transform.lossyScale.z) + transform.position, Quaternion.Euler(stage.rot), new Vector3(stage.sca.x * transform.lossyScale.x, stage.sca.y * transform.lossyScale.y, stage.sca.z * transform.lossyScale.z));
                Gizmos.matrix = rotationMatrix;

                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }

        foreach (var stage in piscesDetails)
        {
            if (stage.showHitboxGizmo)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(new Vector3(stage.pos.x * transform.lossyScale.x, stage.pos.y * transform.lossyScale.y, stage.pos.z * transform.lossyScale.z) + transform.position, Quaternion.Euler(stage.rot), new Vector3(stage.sca.x * transform.lossyScale.x, stage.sca.y * transform.lossyScale.y, stage.sca.z * transform.lossyScale.z));
                Gizmos.matrix = rotationMatrix;

                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }

        foreach (var stage in capricornAirDetails)
        {
            if (stage.showHitboxGizmo)
            {
                Matrix4x4 rotationMatrix = Matrix4x4.TRS(new Vector3(stage.pos.x * transform.lossyScale.x, stage.pos.y * transform.lossyScale.y, stage.pos.z * transform.lossyScale.z) + transform.position, Quaternion.Euler(stage.rot), new Vector3(stage.sca.x * transform.lossyScale.x, stage.sca.y * transform.lossyScale.y, stage.sca.z * transform.lossyScale.z));
                Gizmos.matrix = rotationMatrix;

                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }

    }
}








public enum attacks
{
    downLight,
    neutralLight,
    sideLight,
    Aries,
    Taurus,
    Gemini,
    Cancer,
    Leo,
    Virgo,
    Libra,
    Scorpio,
    Sagittarius,
    Capricorn,
    Aquarius,
    Pisces
}
