using UnityEngine;
using System.Collections;
using FishNet;
using System;
using FishNet.Object;
using Cinemachine;
using FishNet.Object.Synchronizing;
using TMPro;

public class PlayerController : NetworkBehaviour {
    private PlayerMoveset moveset;
    private Rigidbody2D rb;
    private Animator animator;
    private ParticleManager particleManager;
    private SoundsManager soundsManager;
    private UGS_Analytics analytics;

    [SyncVar] public bool faceDir;//false is left true is right
    [HideInInspector] public bool stun = false;
    [HideInInspector] public bool moveWhileStun = false;
    [HideInInspector] public bool knockback = false;
    private float knockbackReleaseTime;//minimun time before knockback breakout
    private float nextAttackTime = 0f;

    [Header("Movement")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float horizDrag = .2f;
    [SerializeField] private float slowDownSpeed = .5f;
    private bool hasJumpedAfterTouchingPlatform = true;
    private Vector2 directionalInput;

    [Header("Jumps and Recovery")]
    [SerializeField] private GameObject jumpAudio;
    [SerializeField] private GameObject doubleJumpAudio;
    [SerializeField] private float vertDrag = .0f;
    [SerializeField] private float jumpHeight = 30f;
    [SerializeField] private int maxAirJumps = 2;
    private int remainingAirJumps = 2;
    [SerializeField] private float airJumpHeight = 30f;
    [SerializeField] private float airJumpCooldown = .1f;
    private float nextAirJumpTime = 0f;
    [SerializeField] private float wallSlideSpeed = 5f;
    [SerializeField] private Vector2 wallJumpDir = new Vector2(1, 1);
    [SerializeField] private float wallJumpHeight = 40f;
    [SerializeField] private int maxRecovery = 1;
    private int remainingRecovery = 1;
    [SerializeField] private int maxWallSlides = 3;
    [SerializeField] private float secondsAfterResetingWallSlide = 1f;
    private int remainingWallSlides = 3;

    [Header("Knockback")]
    [SerializeField] private float horizDragKnockback = .2f;
    [SerializeField] private float vertDragKnockback = .0f;
    [SerializeField] private float verticalKnockbackBreak = 20f; // with the current physics system they probably dont slow down when knocked straight down
    [SerializeField] private float horizontalKnockbackBreak = 15f;

    [Header("Camera Shake")]
    [SerializeField] private float shakeMagnitude = 3f;
    [SerializeField] private float shakeDuration = 2f;
    [SerializeField] public AnimationCurve shakeCurve;

    [Header("Other")]
    [SerializeField] private LayerMask standingLayers;
    [SerializeField] private LayerMask climbingLayers;
    [HideInInspector] public bool isGrounded = false;
    private bool isWallRight = false;
    private bool isWallLeft = false;
    private bool wallslide = false;
    private bool jumpHeld = false;
    [SerializeField] private GameObject visuals;
    private Vector2 scale;
    [SyncVar][HideInInspector] public attacks sideSig = attacks.Scorpio;
    [SyncVar][HideInInspector] public attacks neutralSig = attacks.Taurus;
    [SyncVar][HideInInspector] public attacks downSig = attacks.Aquarius;
    private string lastHitBy = "null";
    private float lastRespawnTime = 0f;
    [SyncVar][HideInInspector] private int percentage;
    [SyncVar][HideInInspector] public int stocks;
    [SerializeField] private TMP_Text UIPercentText;
    private bool hitByCancerSmash = false;

    private void Start() {
        moveset = GetComponent<PlayerMoveset>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        analytics = GameObject.FindGameObjectWithTag("AnalyticsManager").GetComponent<UGS_Analytics>();
        scale = visuals.transform.localScale;
        particleManager = GetComponent<ParticleManager>();
        soundsManager = GetComponent<SoundsManager>();
    }

    private void FixedUpdate()
    {
        if (!base.IsOwner)
        {
            return;
        }
        isGrounded = IsGrounded();
        isWallRight = IsWallRight();
        isWallLeft = IsWallLeft();

        Vector2 vel = rb.velocity;
        if ((!knockback && !stun) || moveWhileStun)
        {
            vel.x *= 1f - horizDrag;
            vel.y *= 1f - vertDrag;
        }
        if (knockback)
        {
            if (vel.x > 0)
            {
                vel.x -= horizDragKnockback;
            }
            else if (vel.x < 0)
            {
                vel.x += horizDragKnockback;
            }

            if (vel.y > 0)
            {
                vel.y -= vertDragKnockback;
            }
            else if (vel.y < 0)
            {
                vel.y += vertDragKnockback;
            }
        }
        if (wallslide)
        {
            vel.y = -wallSlideSpeed;
        }
        rb.velocity = vel;
        rb.AddForce(directionalInput * speed, ForceMode2D.Impulse);
    }

    private void Update() {
        UIPercentText.text = percentage + "%\n";
        //-------------------------------------------------------------------------------------------------------------visual stuff
        if (faceDir)
        {
            visuals.transform.localScale = new Vector3(scale.x, scale.y);
        }
        else
        {
            visuals.transform.localScale = new Vector3(-scale.x, scale.y);
        }
        //-------------------------------------------------------------------------------------------------------------network check
        if (!base.IsOwner)
        {
            return;
        }
        //-------------------------------------------------------------------------------------------------------------stun check stuff
        Vector2 _directionalInput;
        if (moveWhileStun)
        {
            _directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            SetDirectionalInput(_directionalInput);
            return;
        }
        if (stun)
        {
            SetDirectionalInput(new Vector2(0, 0));
            return;
        }
        if (knockback)
        {
            if (Time.time > knockbackReleaseTime)//knockback breakout system
            {
                if (rb.velocity.y < verticalKnockbackBreak)
                {
                    if (rb.velocity.x > 0 && rb.velocity.x < horizontalKnockbackBreak)
                    {
                        knockback = false;
                        Debug.Log("broke out of knockback|x:" + rb.velocity.x + "|y:" + rb.velocity.y);
                    }
                    else if (rb.velocity.x < 0 && rb.velocity.x > -horizontalKnockbackBreak)
                    {
                        knockback = false;
                        Debug.Log("broke out of knockback|x:" + rb.velocity.x + "|y:" + rb.velocity.y);
                    }
                    else if (rb.velocity.x == 0)
                    {
                        knockback = false;
                        Debug.Log("broke out of knockback|x:" + rb.velocity.x + "|y:" + rb.velocity.y);
                    }
                }
            }
            if (knockback)
            {
                animator.SetInteger("State", 4);//knockback
                SetDirectionalInput(new Vector2(0, 0));
                return;
            }
        }

        //-------------------------------------------------------------------------------------------------------------Base Movement
        _directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        SetDirectionalInput(_directionalInput);
        if (_directionalInput.x > 0)//set last facing direction
        {
            faceDir = true;
            RpcSetFaceDir(true);
        }
        else if (_directionalInput.x < 0)
        {
            faceDir = false;
            RpcSetFaceDir(false);
        }

        //-------------------------------------------------------------------------------------------------------------animations
        if (isGrounded && _directionalInput.x != 0)//run/idle animation
        {
            animator.SetInteger("State", 3);//run
        }
        else if (isGrounded && _directionalInput.y < 0)
        {
            animator.SetInteger("State", 16);//crouch
        }
        else if (isGrounded && _directionalInput.x == 0)
        {
            animator.SetInteger("State", 0);//idle
        }
        else if (!isGrounded && rb.velocity.y > 0)
        {
            animator.SetInteger("State", 1);//float
        }
        else if (!isGrounded && rb.velocity.y < 0)
        {
            animator.SetInteger("State", 2);//fall
        }
        //-------------------------------------------------------------------------------------------------------------Jumping
        if (Input.GetAxisRaw("Jump") > 0)
        {
            if (!jumpHeld)
            {
                jumpHeld = true;
                //animator.SetTrigger("Jump");
                OnJumpInput();
            }
        }
        else
        {
            jumpHeld = false;
        }
        //-------------------------------------------------------------------------------------------------------------WallClimb


        if (isWallRight && directionalInput.x > 0 && Time.time >= nextAirJumpTime)
        {
            wallslide = true;
        }
        else if (isWallLeft && directionalInput.x < 0 && Time.time >= nextAirJumpTime)
        {
            wallslide = true;
        }
        else
        {
            wallslide = false;
        }
        if (wallslide)
        {
            animator.SetInteger("State", 12);//wallslide
        }


        //-------------------------------------------------------------------------------------------------------------Attacks
        if (nextAttackTime > Time.time)
        {
            return;
        }
        //old key input "Input.GetAxisRaw("Fire1") > 0"

        if (GetComponent<PlayerIdentity>().isPlaying)
        {
            if (_directionalInput.y < 0 && (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.J)))//down light 
            {
                moveset.StartAttack(faceDir, attacks.downLight);
                stun = true;
                playAnim(attacks.downLight);
                RpcStartMove(attacks.downLight);
            }
            else if ((_directionalInput.y >= 0 && _directionalInput.x == 0 && (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.J))) || (_directionalInput.y > 0 && (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.J))))//neutral light
            {
                moveset.StartAttack(faceDir, attacks.neutralLight);
                stun = true;
                playAnim(attacks.neutralLight);
                RpcStartMove(attacks.neutralLight);
            }
            else if (_directionalInput.x != 0 && (Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.J)))//side light
            {
                moveset.StartAttack(faceDir, attacks.sideLight);
                stun = true;
                playAnim(attacks.sideLight);
                RpcStartMove(attacks.sideLight);
            }


            else if (_directionalInput.y < 0 && (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.K)))//down sig
            {
                moveset.StartAttack(faceDir, downSig);
                stun = true;
                playAnim(downSig);
                RpcStartMove(downSig);
            }
            else if (_directionalInput.y == 0 && _directionalInput.x == 0 && (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.K)) && (remainingRecovery > 0 || moveset.sagArrow != null))//neutral sig
            {
                if (moveset.sagArrow == null)
                {
                    moveset.StartAttack(faceDir, neutralSig);
                    stun = true;
                    playAnim(neutralSig);
                    RpcStartMove(neutralSig);
                    remainingRecovery--;
                    
                    if (neutralSig == attacks.Cancer)
                    {
                        Invoke("SetCancerSmashHit", 1f);
                    }
                }
                else
                {
                    moveset.StartAttack(faceDir, neutralSig);
                    if (neutralSig == attacks.Cancer)
                    {
                        Invoke("SetCancerSmashHit", 1f);
                    }
                }
            }
            else if (_directionalInput.y > 0 && (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.K)) && (remainingRecovery > 0 || moveset.sagArrow != null))//neutral sig alt input
            {
                if (moveset.sagArrow == null)
                {
                    moveset.StartAttack(faceDir, neutralSig);
                    stun = true;
                    playAnim(neutralSig);
                    RpcStartMove(neutralSig);
                    remainingRecovery--;
                    if (neutralSig == attacks.Cancer)
                    {
                        Invoke("SetCancerSmashHit", 1f);
                    }
                }
                else
                {
                    moveset.StartAttack(faceDir, neutralSig);
                    if (neutralSig == attacks.Cancer)
                    {
                        Invoke("SetCancerSmashHit", 1f);
                    }
                }
            }
            else if (_directionalInput.x != 0 && (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.K)))//side sig
            {
                moveset.StartAttack(faceDir, sideSig);
                stun = true;
                playAnim(sideSig);
                RpcStartMove(sideSig);
            }
        }
    }



    #region Input
    private void SetDirectionalInput(Vector2 newDirectionalInput)
    {
        newDirectionalInput.y = 0;
        if (directionalInput != newDirectionalInput)
        {
            if (directionalInput.x > 0 && newDirectionalInput.x <= 0)
            {
                rb.velocity = new Vector2(slowDownSpeed, rb.velocity.y);
            }
            if (directionalInput.x < 0 && newDirectionalInput.x >= 0)
            {
                rb.velocity = new Vector2(-slowDownSpeed, rb.velocity.y);
            }
            directionalInput = newDirectionalInput;
        }
    }
    private void OnJumpInput()
    {
        hasJumpedAfterTouchingPlatform = true;
        if (isGrounded)
        {
            nextAirJumpTime = Time.time + airJumpCooldown;
            rb.AddForce(new Vector2(0, jumpHeight), ForceMode2D.Impulse);
            GameObject go = Instantiate(jumpAudio);
        }
        else if (isWallLeft)
        {
            nextAirJumpTime = Time.time + airJumpCooldown;
            rb.velocity = new Vector2(0, 0);
            rb.AddForce(wallJumpDir.normalized * wallJumpHeight, ForceMode2D.Impulse);
            GameObject go = Instantiate(jumpAudio);
        }
        else if (isWallRight)
        {
            nextAirJumpTime = Time.time + airJumpCooldown;
            rb.velocity = new Vector2(0, 0);
            rb.AddForce(new Vector2(-wallJumpDir.x, wallJumpDir.y).normalized * wallJumpHeight, ForceMode2D.Impulse);
            GameObject go = Instantiate(jumpAudio);
        }
        else if (remainingAirJumps > 0 && Time.time >= nextAirJumpTime)
        {
            nextAirJumpTime = Time.time + airJumpCooldown;
            remainingAirJumps--;
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(new Vector2(0, airJumpHeight), ForceMode2D.Impulse);
            GameObject go = Instantiate(doubleJumpAudio);
        }
    }
    #endregion

    #region Physics
    private bool IsGrounded()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.down;
        float distance = transform.localScale.y / 2 + 0.2f;

        RaycastHit2D hit = Physics2D.Raycast(position + (Vector2.right * (transform.localScale.x / 2 - .01f)), direction, distance, standingLayers);
        RaycastHit2D hit2 = Physics2D.Raycast(position - (Vector2.right * (transform.localScale.x / 2 - .01f)), direction, distance, standingLayers);
        if (hit.collider != null || hit2.collider != null)
        {
            remainingAirJumps = maxAirJumps;
            remainingRecovery = maxRecovery;
            //if (!stun) {GetComponent<PlayerIdentity>().ServerRpcUpdateLastHitByPlayer(GetComponent<PlayerIdentity>().playerNumber);}
            return true;
        }
        Debug.DrawRay(position + (Vector2.right * (transform.localScale.x / 2 - .01f)), direction * distance);
        Debug.DrawRay(position - (Vector2.right * (transform.localScale.x / 2 - .01f)), direction * distance);

        return false;
    }
    private bool IsWallRight()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.right;
        float distance = transform.localScale.x / 2 + 0.2f;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, climbingLayers);
        RaycastHit2D hit2 = Physics2D.Raycast(position + (Vector2.up * (transform.localScale.y / 2 - .01f)), direction, distance, climbingLayers);
        RaycastHit2D hit3 = Physics2D.Raycast(position - (Vector2.up * (transform.localScale.y / 2 - .01f)), direction, distance, climbingLayers);
        RaycastHit2D hit4 = Physics2D.Raycast(position + (Vector2.up * (transform.localScale.y / 4)), direction, distance, climbingLayers);
        RaycastHit2D hit5 = Physics2D.Raycast(position - (Vector2.up * (transform.localScale.y / 4)), direction, distance, climbingLayers);
        if ((remainingWallSlides > 0) && (hit.collider != null || hit2.collider != null || hit3.collider != null || hit4.collider != null || hit5.collider != null))
        {
            remainingAirJumps = maxAirJumps;
            remainingRecovery = maxRecovery;
            //if (!stun) {GetComponent<PlayerIdentity>().ServerRpcUpdateLastHitByPlayer(GetComponent<PlayerIdentity>().playerNumber);}
            return true;
        }
        Debug.DrawRay(position, direction * distance);
        Debug.DrawRay(position + (Vector2.up * (transform.localScale.y / 2 - .01f)), direction * distance);
        Debug.DrawRay(position - (Vector2.up * (transform.localScale.y / 2 - .01f)), direction * distance);
        Debug.DrawRay(position + (Vector2.up * (transform.localScale.y / 4)), direction * distance);
        Debug.DrawRay(position - (Vector2.up * (transform.localScale.y / 4)), direction * distance);

        return false;
    }
    private bool IsWallLeft()
    {
        Vector2 position = transform.position;
        Vector2 direction = Vector2.left;
        float distance = transform.localScale.x / 2 + 0.2f;

        RaycastHit2D hit = Physics2D.Raycast(position, direction, distance, climbingLayers);
        RaycastHit2D hit2 = Physics2D.Raycast(position + (Vector2.up * (transform.localScale.y / 2 - .01f)), direction, distance, climbingLayers);
        RaycastHit2D hit3 = Physics2D.Raycast(position - (Vector2.up * (transform.localScale.y / 2 - .01f)), direction, distance, climbingLayers);
        RaycastHit2D hit4 = Physics2D.Raycast(position + (Vector2.up * (transform.localScale.y / 4)), direction, distance, climbingLayers);
        RaycastHit2D hit5 = Physics2D.Raycast(position - (Vector2.up * (transform.localScale.y / 4)), direction, distance, climbingLayers);
        if ((remainingWallSlides > 0) && (hit.collider != null || hit2.collider != null || hit3.collider != null || hit4.collider != null || hit5.collider != null))
        {
            remainingAirJumps = maxAirJumps;
            remainingRecovery = maxRecovery;
            //if (!stun) {GetComponent<PlayerIdentity>().ServerRpcUpdateLastHitByPlayer(GetComponent<PlayerIdentity>().playerNumber);}
            return true;
        }
        Debug.DrawRay(position, direction * distance);
        Debug.DrawRay(position + (Vector2.up * (transform.localScale.y / 2 - .01f)), direction * distance);
        Debug.DrawRay(position - (Vector2.up * (transform.localScale.y / 2 - .01f)), direction * distance);
        Debug.DrawRay(position + (Vector2.up * (transform.localScale.y / 4)), direction * distance);
        Debug.DrawRay(position - (Vector2.up * (transform.localScale.y / 4)), direction * distance);

        return false;
    }

    #endregion

    #region Knockback

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!base.IsOwner)
        {
            return;
        }
        else if (!GetComponent<PlayerIdentity>().isPlaying)
        {
            return;
        }
        else if (col.tag == "BlastZone")
        {
            analytics.PlayerDeathCustomEvent(lastHitBy, percentage, lastRespawnTime - Time.time);

            GetComponent<PlayerIdentity>().updateKillersList(lastHitBy);
            GetComponent<PlayerIdentity>().ServerRpcAddToDeathCount(1);

            remainingWallSlides = maxWallSlides;

            PlayDeathAnimation(GetComponent<PlayerIdentity>().playerNumber, gameObject.transform.position);

            SupCamera Cam = GameObject.FindWithTag("MainCamera").GetComponent<SupCamera>();
            if (Cam != null)
            {
                StartCoroutine(Cam.ShakeCamera(shakeMagnitude, shakeDuration, shakeCurve));
            }

            StartCoroutine(Respawn(1.3f));

            hitByCancerSmash = false;

            knockback = true;
        }

    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (!base.IsOwner)
        {
            return;
        }
        if (hasJumpedAfterTouchingPlatform && other.gameObject.tag == "Wall")
        {
            hasJumpedAfterTouchingPlatform = false;
            CancelInvoke("ResetWallSlides");
            remainingWallSlides--;
        }
        if (other.gameObject.layer == 6 && hitByCancerSmash && other.gameObject.tag != "Wall")
        {
            hitByCancerSmash = false;
            RpcSpawnCancerVFX(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D other) {
        if (!base.IsOwner)
        {
            return;
        }
        if (hasJumpedAfterTouchingPlatform && other.gameObject.tag == "Wall")
        {
            InvokeRepeating("ResetWallSlides", secondsAfterResetingWallSlide, 0);
        }
    }

    public void ResetWallSlides()
    {
        remainingWallSlides = maxWallSlides;
    }

    [ObserversRpc] public void RpcShowVictoryScreen(string name)
    {
        if (IsOwner)
        {
            StartCoroutine(VictoryScreen(name));
        }
    }
    IEnumerator VictoryScreen(string name)
    {
        TMP_Text victoryScreenText = GameObject.FindGameObjectWithTag("VictoryScreen").GetComponent<TMP_Text>();
        victoryScreenText.text = name + " Wins!";
        victoryScreenText.enabled = true;
        yield return new WaitForSeconds(1f);
        VictoryScreenCanvas();
        yield return new WaitForSeconds(6f);
        victoryScreenText.enabled = false;  
    }

    private void VictoryScreenCanvas()
    {
        GameObject victoryScreenCanvas = Resources.FindObjectsOfTypeAll<victoryScreenCanvas>()[0].gameObject;
        Debug.Log(victoryScreenCanvas);
        gameObject.GetComponent<PlayerIdentity>().FindPlayers();
        victoryScreenCanvas.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)] public void PlayDeathAnimation(int playerNumber, Vector3 playerTransform)
    {
        RpcPlayDeathAnimation(playerNumber, playerTransform);
    }
    [ObserversRpc]  public void RpcPlayDeathAnimation(int playerNumber, Vector3 playerTransform)
    {
        particleManager.SpawnDeathParticle(playerNumber, playerTransform);
    }
    [ServerRpc(RequireOwnership = false)] public void RpcSpawnCancerVFX(Transform playerTransform)
    {
        RpcCancerSpawn(playerTransform);
    }
    [ObserversRpc] public void RpcCancerSpawn(Transform playerTransform)
    {
        particleManager.SpawnCancerSmashSlamVFX(playerTransform);
    }
    [ServerRpc(RequireOwnership = false)] public void RpcHitPlayer(attackBoxDetails hitDetails, int hitByPlayerNumber, Vector3 hitPlayerTransform)
    {
        RpcKnockback(hitDetails, hitByPlayerNumber, hitPlayerTransform);
    }
    [ObserversRpc] public void RpcKnockback(attackBoxDetails hitDetails, int hitByPlayerNumber, Vector3 hitPlayerTransform)
    {
        particleManager.PlayParticle(hitDetails.attack);
        soundsManager.PlaySound(hitDetails.attack);
        particleManager.SpawnHitVFX(transform.position, hitPlayerTransform);

        if (!IsOwner) 
        { 
            return; 
        }

        lastHitBy = hitDetails.attackName;
        GetComponent<PlayerIdentity>().ServerRpcUpdateLastHitByPlayer(hitByPlayerNumber);

        moveset.Interupt();

        float force = hitDetails.attackForceBase + (hitDetails.attackForceScale * percentage);
        Vector2 direction = hitDetails.attackDirection;

        rb.velocity = Vector2.zero;
        rb.AddForce(direction.normalized * force);

        knockbackReleaseTime = (Time.time + hitDetails.knockbackStunTime);
        knockback = true;

        // Add a jump every time you are hit in the air
        if (!isGrounded && remainingAirJumps < maxAirJumps)
        {
            remainingAirJumps++;
        }

        RpcIncreasePercentage(hitDetails.attackDMG);
    }

    IEnumerator Respawn(float hoverTime)
    {
        hitByCancerSmash = false;
        if (stocks > 0)
        {
            RpcDecreaseStocks();
            stun = true;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            transform.position = GetComponent<PlayerIdentity>().GetSpawnTransform().position;
            animator.SetInteger("State", 24);
            yield return new WaitForSeconds(hoverTime);
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            RpcResetPercentage();
            stun = false;
            knockback = false;
            lastRespawnTime = Time.time;
            GetComponent<PlayerIdentity>().resetNameTime();
            GetComponent<PlayerIdentity>().isReady = false;
        }
        else
        {
            GetComponent<PlayerIdentity>().RpcOutOfStocks();
            GetComponent<PlayerIdentity>().calculatePlacement();
            stun = true;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            transform.position = new Vector2(0, 100);
            GetComponent<PlayerIdentity>().RpcUpdateUIStocksCommand(0);
        }
    }
    IEnumerator Spawn(float hoverTime)
    {
        hitByCancerSmash = false;
        stun = true;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        transform.position = GetComponent<PlayerIdentity>().GetSpawnTransform().position;
        animator.SetInteger("State", 24);
        yield return new WaitForSeconds(hoverTime);
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        RpcResetPercentage();
        stun = false;
        knockback = false;
        lastRespawnTime = Time.time;
        GetComponent<PlayerIdentity>().resetNameTime();
        GetComponent<PlayerIdentity>().RpcUpdateUIStocksCommand(stocks+1);
    }

    [ServerRpc] public void RpcIncreasePercentage(int dmg)
    {
        percentage += dmg;
    }
    [ServerRpc] public void RpcDecreaseStocks()
    {
        GetComponent<PlayerIdentity>().RpcUpdateUIStocksCommand(stocks);
        stocks--;
    }
    [ServerRpc] public void RpcResetPercentage(int _percentage = 0)
    {
        percentage = _percentage;
    }
    [ServerRpc(RequireOwnership = false)] public void RpcResetStocks(int _stocks = 2)
    {
        stocks = _stocks;
        GetComponent<PlayerIdentity>().RpcUpdateUIStocksCommand(stocks+1);
    }
    [ObserversRpc] public void RpcSpawn()
    {
        if (!IsOwner)
        {
            return;
        }
        StartCoroutine(Spawn(1.3f));
    }
    [ObserversRpc] public void RpcRespawn()
    {
        if (!IsOwner)
        {
            return;
        }
        StartCoroutine(Respawn(1.3f));
    }
    public void OnDestroy()
    {
        if (UIPercentText != null)
        {
            Destroy(UIPercentText.gameObject.transform.parent.gameObject);
        }
    }

    #endregion

    #region Move Customization

    public void NewChangeSideAttack(int attack)
    {
        Debug.Log(attack);
        RpcSetSideSigNew(attack);
    }
    public void NewChangeNeutralAttack(int attack)
    {
        Debug.Log(attack);
        RpcSetNeutralSigNew(attack);
    }
    public void NewChangeDownAttack(int attack)
    {
        Debug.Log(attack);
        RpcSetDownSigNew(attack);
    }

    public void SetCancerSmashHit()
    {
        hitByCancerSmash = true;
    }


    [ServerRpc]
    private void RpcSetSideSigNew(int newValue)
    {
        sideSig = (attacks)newValue;
    }
    [ServerRpc]
    private void RpcSetNeutralSigNew(int newValue)
    {
        neutralSig = (attacks)newValue;
    }
    [ServerRpc]
    private void RpcSetDownSigNew(int newValue)
    {
        downSig = (attacks)newValue;
    }
    #endregion

    #region Other
    [ServerRpc] private void RpcSetFaceDir(bool dir)
    {
        faceDir = dir;
    }
    public void SetNextAttackTime(float value)
    {
        nextAttackTime = value;
    }
    private void playAnim(attacks atk)//this is because i failed to plan ahead and number the attacks in a way that made sense
    {
        switch (atk)
        {
            case attacks.downLight:
                animator.SetInteger("State", 14);
                break;
            case attacks.neutralLight:
                animator.SetInteger("State", 6);
                break;
            case attacks.sideLight:
                animator.SetInteger("State", 7);
                break;
            case attacks.Aries:
                animator.SetInteger("State", 11);
                break;
            case attacks.Taurus:
                animator.SetInteger("State", 9);
                break;
            case attacks.Gemini:
                animator.SetInteger("State", 21);
                break;
            case attacks.Cancer:
                animator.SetInteger("State", 13);
                break;
            case attacks.Leo:
                animator.SetInteger("State", 19);
                break;
            case attacks.Virgo:
                animator.SetInteger("State", 15);
                break;
            case attacks.Libra:
                animator.SetInteger("State", 23);
                break;
            case attacks.Scorpio:
                animator.SetInteger("State", 10);
                break;
            case attacks.Sagittarius:
                animator.SetInteger("State", 18);
                break;
            case attacks.Capricorn:
                if (isGrounded)
                {
                    animator.SetInteger("State", 20);
                }
                else
                {
                    animator.SetInteger("State", 22);
                }
                break;
            case attacks.Aquarius:
                animator.SetInteger("State", 8);
                break;
            case attacks.Pisces:
                animator.SetInteger("State", 17);
                break;
            default:
                break;
        }
    }

    [ObserversRpc] public void RpcStartMove(attacks atk)
    {
        particleManager.PlayStartParticle(atk);
        soundsManager.PlayStartSound(atk);
    }
        #endregion
    }