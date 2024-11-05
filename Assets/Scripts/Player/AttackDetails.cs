using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackDetails
{
    [Header("Movement")]
    public bool resetVelocity;
    public bool moveDuringLifetime;
    public float speedModifier;//not implemented yet
    [Space(7)]


    [Header("Hitbox/Projectile")]
    public bool hasHitbox;
    public GameObject projectileR;
    public bool spawnProjectileMiddle;
    [Space(5)]
    public string hitboxName;
    public float attackForceBase;
    public float attackForceScale;
    public Vector2 attackDirection;
    public float knockbackStunTime;
    public int attackDMG;
    [Space(5)]
    public Vector3 pos;
    public Vector3 rot;
    public Vector3 sca;
    [Space(7)]


    [Header("Timings (should add up to total frames)")]
    public int windupFrames;
    public int lifetimeFrames;
    public int endLagFrames;
    [HideInInspector]public float nextAttackDelay = 0.2f;
    [Space(7)]


    [Header("Self Movement")]
    public Vector2 pushDirection;
    public float pushForce;
    [Space(7)]


    [Header("Particles")]
    public int ParticleSystemID;
    [Space(7)]


    [Header("Debug Settings")]
    public bool showHitboxGizmo;
}