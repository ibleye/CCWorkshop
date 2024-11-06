using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AttackDetails
{
    [Header("Movement")]
    [HideInInspector] public bool resetVelocity;
    [HideInInspector] public bool moveDuringLifetime;
    [HideInInspector] public float speedModifier;//not implemented yet
    [Space(7)]


    [Header("Hitbox/Projectile")]
    [HideInInspector] public bool hasHitbox;
    [HideInInspector] public GameObject projectileR;
    [HideInInspector] public bool spawnProjectileMiddle;
    [Space(5)]
    [HideInInspector] public string hitboxName;
    public float attackForceBase;
    public float attackForceScale;
    public Vector2 attackDirection;
    [HideInInspector] public float knockbackStunTime;
    public int attackDMG;
    [Space(5)]
    [HideInInspector] public Vector3 pos;
    [HideInInspector] public Vector3 rot;
    [HideInInspector] public Vector3 sca;
    [Space(7)]


    [Header("Timings (should add up to total frames)")]
    [HideInInspector] public int windupFrames;
    [HideInInspector] public int lifetimeFrames;
    [HideInInspector] public int endLagFrames;
    [HideInInspector]public float nextAttackDelay = 0.2f;
    [Space(7)]


    [Header("Self Movement")]
    [HideInInspector] public Vector2 pushDirection;
    [HideInInspector] public float pushForce;
    [Space(7)]


    [Header("Particles")]
    [HideInInspector] public int ParticleSystemID;
    [Space(7)]


    [Header("Debug Settings")]
    [HideInInspector] public bool showHitboxGizmo;
}