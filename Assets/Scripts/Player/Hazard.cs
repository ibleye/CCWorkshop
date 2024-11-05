using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour
{
    [SerializeField] private GameObject attackBoxPrefab;
    [SerializeField] private bool debugMode = false;
    [SerializeField] private Color hitboxColor;

    public AttackDetails[] attackDetails;
    private bool setForDestroy = false;
    public int owner;
    public bool server;
    private int animFramerate = 12;




    void OnEnable()
    {
        foreach (var stage in attackDetails)
        {
            StartCoroutine(EnableAttack(stage));
        }
    }

    private IEnumerator EnableAttack(AttackDetails details)
    {
        GameObject attackBox = null;

        yield return new WaitForSeconds((float)details.windupFrames / animFramerate);//--------------------------------------time before anything activates

        if (details.hasHitbox && server)//spawn in hitbox and set variables
        {
            attackBox = Instantiate(attackBoxPrefab, transform);
            AttackBox hitboxScript = attackBox.GetComponent<AttackBox>();

            hitboxScript.name = details.hitboxName;
            hitboxScript.disjointedHitbox = true;
            hitboxScript.attackForceBase = details.attackForceBase;
            hitboxScript.attackForceScale = details.attackForceScale;
            hitboxScript.attackDirection = details.attackDirection;
            hitboxScript.knockbackStunTime = details.knockbackStunTime;
            hitboxScript.attackDMG = details.attackDMG;
            if (details.projectileR != null)
            {
                hitboxScript.disjointedHitbox = true;
            }

            hitboxScript.transform.localPosition = details.pos;
            hitboxScript.transform.rotation = Quaternion.Euler(0, 0, details.rot.z);
            hitboxScript.transform.localScale = details.sca;
        }

        yield return new WaitForSeconds((float)details.lifetimeFrames / animFramerate);//--------------------------------------time hitbox is active

        if (attackBox != null)
        {
            Destroy(attackBox);
        }

        yield return new WaitForSeconds((float)details.endLagFrames / animFramerate);//--------------------------------------time before end of attack

        setForDestroy = true;
    }

    private void Update()
    {
        if (setForDestroy)
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    public void OnDrawGizmos()
    {
        if (!debugMode) { return; }
        Gizmos.color = hitboxColor;

        foreach (var stage in attackDetails)
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
