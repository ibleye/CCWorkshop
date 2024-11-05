using FishNet;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SerializeField] private Vector2 initalVelocity;
    [SerializeField] private float gravity;
    private Rigidbody2D RB;
    private Animator animator;
    [SerializeField] private GameObject hazard;
    private bool spawnedGeyser = false;
    [SerializeField] private Vector3 geyserOffset;
    private AttackBox hitbox;

    [SerializeField] private float lifetime;
    [SerializeField] private bool spawnHazardImmidately = false;
    private float deletetime = Mathf.Infinity;
            

    [SyncVar] [HideInInspector] public PlayerMoveset owner;
    [SyncVar] [HideInInspector] public bool faceDir;
    [SyncVar] [HideInInspector] public int color;

    private bool despawning = false;

    public void SetOwner(PlayerMoveset _owner)
    {
        owner = _owner;
        Debug.Log("owner set!!!!!!!");
    }

    void Start()
    {
        if (spawnHazardImmidately)
        {
            transform.position = transform.position + geyserOffset;
            GameObject go = Instantiate(hazard, transform.position, Quaternion.identity);
            if (!faceDir)
            {
                go.transform.localScale = new Vector3(-go.transform.localScale.x, go.transform.localScale.y, go.transform.localScale.z);
            }
            go.GetComponent<Hazard>().owner = OwnerId;
            go.GetComponent<Hazard>().server = base.IsServer;
            return;
        }
        RB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        animator.SetInteger("Color", color);
        if (lifetime > 0)
        {
            deletetime = Time.time + lifetime;
        }
        if (!faceDir)
        {
            initalVelocity.x = -initalVelocity.x;
        }
        RB.velocity = initalVelocity;
        hitbox = GetComponentInChildren<AttackBox>();
        if (gravity != 0)
        {
            RB.gravityScale = gravity;
            if (owner != null)
            {
                owner.sagArrow = this.gameObject;
            }
        }
        if (IsServer)
        {
            hitbox.gameObject.GetComponent<BoxCollider2D>().enabled = true;
        }
    }


    private void Update()
    {
        if (spawnHazardImmidately)
        {
            return;
        }
        if (base.IsServer && (transform.position.x < -31 || transform.position.x > 31 || transform.position.y < -17))
        {
            InstanceFinder.ServerManager.Despawn(this.gameObject);
        }
        if (base.IsServer && Time.time > deletetime)
        {
            InstanceFinder.ServerManager.Despawn(this.gameObject);
        }
        if (gravity == 0 && RB.velocity != initalVelocity)
        {
            hitbox.gameObject.SetActive(false);
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;

            if (!spawnedGeyser && hazard != null)
            {
                GameObject go = Instantiate(hazard, transform.position + geyserOffset, Quaternion.identity);
                go.GetComponent<Hazard>().owner = OwnerId;
                go.GetComponent<Hazard>().server = base.IsServer;
                spawnedGeyser =true;
            }
            if (base.IsServer && !despawning)
            {
                despawning = true;
                StartCoroutine(DelayDestroy(.5f));
            }
        }
        if (gravity != 0)
        {
            transform.rotation = Quaternion.LookRotation(Vector3.forward, RB.velocity);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (spawnHazardImmidately)
        {
            return;
        }
        Debug.Log(collision.gameObject.name);
        if (gravity != 0 && IsOwner && (collision.gameObject.layer == 6 || collision.gameObject.layer == 10))
        {
            owner.gameObject.transform.position = transform.position;
            RB.velocity = Vector2.zero;
            GetComponent<SpriteRenderer>().enabled = false;
        }
        if (gravity != 0 && IsServer)
        {
            InstanceFinder.ServerManager.Despawn(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsServer && collision.gameObject.layer == 11)
        {
            InstanceFinder.ServerManager.Despawn(this.gameObject);
        }
    }

    private IEnumerator DelayDestroy (float time)
    {
        yield return new WaitForSeconds(time);
        InstanceFinder.ServerManager.Despawn(this.gameObject);
    }

    private void OnDisable()
    {
        Destroy(this);
    }
}
