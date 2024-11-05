using FishNet.Object;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackBox : MonoBehaviour
{
    public float attackForceBase;
    public float attackForceScale;
    public Vector2 attackDirection;
    public float knockbackStunTime;
    public int attackDMG;

    public attacks attack;
    public bool disjointedHitbox;

    private NetworkBehaviour playerController;

    private int libraCount = 0;

    private int playerNumber;

    private List<string> hitPlayers = new List<string>();

    private void Awake()
    {
        playerController = transform.root.GetComponent<PlayerController>();
        if (playerController == null)
        {
            playerController = transform.root.GetComponent<Projectile>();
        }
    }

    private void Start()
    {
        setPlayerNumber();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (disjointedHitbox && col.tag == "Player" && col.GetComponent<NetworkBehaviour>().OwnerId != transform.root.GetComponent<Hazard>().owner ||
            !disjointedHitbox && col.tag == "Player" && col.GetComponent<NetworkBehaviour>().OwnerId != playerController.OwnerId)
        {
            if (hitPlayers.Contains(col.name))
            {
                return;
            }
            hitPlayers.Add(col.name);
            if (playerNumber == 0)
            {
                CheckOwner();
            }
            col.GetComponent<PlayerController>().RpcHitPlayer(new attackBoxDetails(attackForceBase, attackForceScale, attackDirection, knockbackStunTime, attackDMG, transform.name, attack), playerNumber, transform.position);
            if (attack == attacks.Libra)
            {
                if (libraCount == 0)
                {
                    col.GetComponent<PlayerController>().stun = true;
                    libraCount++;
                }
                else
                {
                    col.GetComponent<PlayerController>().stun = false;
                    libraCount = 0;
                }
            }
        }
    }

    private void setPlayerNumber()
    {
        if (transform.root.GetComponent<PlayerIdentity>() != null)
        {
            playerNumber = transform.root.GetComponent<PlayerIdentity>().playerNumber;
        }
        else if (transform.root.GetComponent<Projectile>() != null)
        {
            Invoke("CheckOwner", 0.1f);
        }
        else if (transform.root.GetComponent<Hazard>() != null)
        {
            playerNumber = transform.root.GetComponent<Hazard>().owner;
            Debug.Log("The player number is" + transform.root.GetComponent<Hazard>().owner);
        }
    }

    void CheckOwner()
    {
        playerNumber = transform.root.GetComponent<Projectile>().owner.playerNumber;
    }
}

public struct attackBoxDetails
{
    public float attackForceBase;
    public float attackForceScale;
    public Vector2 attackDirection;
    public float knockbackStunTime;
    public int attackDMG;
    public string attackName;
    public attacks attack;

    public attackBoxDetails(float _attackForceBase, float _attackForceScale, Vector2 _attackDirection, float _knockbackStunTime, int _attackDMG, string _attackName, attacks _attack)
    {
        attackForceBase = _attackForceBase;
        attackForceScale = _attackForceScale;
        attackDirection = _attackDirection;
        knockbackStunTime = _knockbackStunTime;
        attackDMG = _attackDMG;
        attackName = _attackName;
        attack = _attack;
    }
}
