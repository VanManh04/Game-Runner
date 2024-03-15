using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    private bool isDead;
    [HideInInspector]public bool playerUnlocked;
    [HideInInspector]public bool extraLife;
    public bool extra=false;

    [Header("VFX")]
    [SerializeField] private ParticleSystem dustFx;
    [SerializeField] private ParticleSystem bloodFx;

    [Header("Knockback info")]
    [SerializeField] private Vector2 knockbackDir;
    private bool isKnocked;
    private bool canBeKnocked = true;

    [Header("Move info")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float speedMultiplier;
    private float defaultSpeed;
    [Space]
    [SerializeField] private float milestoneIncreaser;
    private float defaunlMinestoneIncrease;
    private float speedMilestone;

    private bool readyToLand;

    [Header("Jump info")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float doubleJumpForce;
    private bool canDoubleJump;

    [Header("Slide info")]
    [SerializeField] private float slideSpeed;
    [SerializeField] private float slideTime;
    [SerializeField] private float slideCooldown;
    [HideInInspector] public float slideCooldownCounter;
    private float slideTimerCounter;
    private bool isSliding;

    [Header("Collision info")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float ceillingCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform wallCheck;
    [SerializeField] private Vector2 wallCheckSize;
    private bool isGrounded;
    private bool wallDetected;
    private bool ceillingDetected;
    [HideInInspector] public bool ledgeDetected;

    [Header("Ledge info")]
    [SerializeField] private Vector2 offset1; //befor
    [SerializeField] private Vector2 offset2; //after

    private Vector2 climbBegunPosition;
    private Vector2 climbOverPosition;

    private bool canGrabLefge = true;
    private bool canClimb;
    private float defaulGravityScale;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        speedMilestone = milestoneIncreaser;
        defaultSpeed = moveSpeed;
        defaunlMinestoneIncrease = milestoneIncreaser;
        defaulGravityScale = rb.gravityScale;
        extraLife = false;
    }

    float time111;
    void Update()
    {
        CheckCollision();
        AnimatorControllers();

        slideTimerCounter -= Time.deltaTime;
        slideCooldownCounter -= Time.deltaTime;

        time111 -= Time.deltaTime;
        extraLife = time111<0;

        
        //extraLife = moveSpeed >= maxSpeed;
        //if(Input.GetKeyDown(KeyCode.K))
        //    Knockback();

        //if (Input.GetKeyDown(KeyCode.O) && !isDead)
        //    StartCoroutine(Die());

        if (isDead)
            return;

        if (isKnocked)
            return;

        if (playerUnlocked)
            SetupMovement();

        if (isGrounded)
            canDoubleJump = true;

        SpeedController();

        CheckForLanding();
        CheckForLedge();
        CheckForSlideCancel();
        CheckInput();
    }

    private void CheckForLanding()
    {
        if (rb.velocity.y < -5 && !isGrounded)
            readyToLand = true;

        if (readyToLand && isGrounded)
        {
            dustFx.Play();
            readyToLand = false;
        }
    }

    public void Damage()
    {
        time111 = 7;
        bloodFx.Play();
        if (extraLife)
        {
            Knockback();
        }else
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        AudioManager.instance.PlaySFX(3);
        isDead = true;
        canBeKnocked = false;
        rb.velocity = knockbackDir;
        anim.SetBool("isDead", true);

        Time.timeScale = .6f;

        yield return new WaitForSeconds(1f);

        Time.timeScale = 1f;
        rb.velocity = new Vector2(0, 0);
        //GameManager.instance.RestartLever();
        GameManager.instance.GameEnded();
    }

    #region Knockback
    private IEnumerator Invincibility()
    {
        Color originalColor = sr.color;
        Color darkenColor = new Color(sr.color.r, sr.color.g, sr.color.b,.5f);

        canBeKnocked = false;

        sr.color = darkenColor;
        yield return new WaitForSeconds(.1f);
        sr.color = originalColor;
        yield return new WaitForSeconds(.1f);
        sr.color = darkenColor;
        yield return new WaitForSeconds(.15f);
        sr.color = originalColor;
        yield return new WaitForSeconds(.15f);
        sr.color = darkenColor;
        yield return new WaitForSeconds(.25f);
        sr.color = originalColor;
        yield return new WaitForSeconds(.25f);
        sr.color = darkenColor;
        yield return new WaitForSeconds(.3f);
        sr.color = originalColor;
        yield return new WaitForSeconds(.35f);
        sr.color = darkenColor;
        yield return new WaitForSeconds(.4f);
        sr.color = originalColor;

        canBeKnocked = true;
    }

    private void Knockback()
    {
        if (!canBeKnocked)
            return;

        StartCoroutine(Invincibility());
        isKnocked = true;
        rb.velocity = knockbackDir;
        moveSpeed = defaultSpeed;
    }

    private void CancelKnockback()=>isKnocked = false;
    #endregion

    #region SpeedControll
    private void SpeedReset()
    {
        if (isSliding)
            return;
        moveSpeed = defaultSpeed;
        milestoneIncreaser = defaunlMinestoneIncrease;
    }

    private void SpeedController()
    {
        if (moveSpeed == maxSpeed)
            return;

        if (transform.position.x > speedMilestone)
        {
            speedMilestone = speedMilestone + milestoneIncreaser;

            moveSpeed = moveSpeed * speedMultiplier;
            milestoneIncreaser = milestoneIncreaser * speedMultiplier;

            if (moveSpeed > maxSpeed)
                moveSpeed = maxSpeed;
        }
    }
    #endregion

    #region Ledge Climb Region
    private void CheckForLedge()
    {
        if (ledgeDetected && canGrabLefge)
        {
            canGrabLefge = false;
            rb.gravityScale = 0;
            Vector2 ledgePosition = GetComponentInChildren<LedgeDetection>().transform.position;

            climbBegunPosition = ledgePosition + offset1;
            climbOverPosition = ledgePosition + offset2;

            canClimb = true;
        }

        if (canClimb)
            transform.position = climbBegunPosition;
    }
    private void LedgeClimOver()
    {
        canClimb = false;
        rb.gravityScale = defaulGravityScale;
        transform.position = climbOverPosition;
        Invoke("AllowLedgeGrab", .2f);
    }
    private void AllowLedgeGrab()
    {
        canGrabLefge = true;
    }

    #endregion

    #region Input
    private void CheckForSlideCancel()
    {
        if (slideTimerCounter < 0&&!ceillingDetected)
            isSliding = false;
    }

    private void SetupMovement()
    {
        if (wallDetected)
        {
            SpeedReset();
            return;
        }

        if (isSliding)
            rb.velocity = new Vector2(slideSpeed, rb.velocity.y);
        else
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }


    public void SlideButton()
    {
        if (isDead)
            return;

        if (rb.velocity.x != 0&&slideCooldownCounter<0)
        {
            dustFx.Play();
            isSliding = true;
            slideTimerCounter = slideTime;
            slideCooldownCounter = slideCooldown;
        }
    }

    public void JumpButton()
    {
        if (isSliding||isDead)
            return;

        RollAnimFinished();

        if (isGrounded)
        {
            Jump(jumpForce);
        }
        else if (canDoubleJump)
        {
            canDoubleJump = false;
            Jump(doubleJumpForce);
        }
    }

    private void Jump(float force)
    {
        dustFx.Play();
        AudioManager.instance.PlaySFX(Random.Range(1, 2));
        rb.velocity = new Vector2(rb.velocity.x, force);
    }

    private void CheckInput()
    {
        //if (Input.GetButtonDown("Fire2"))
        //    playerUnlocked = !playerUnlocked;

        if (Input.GetButtonDown("Jump"))
        {
            JumpButton();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
            SlideButton();
    }
    #endregion

    #region Animations
    private void AnimatorControllers()
    {
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetFloat("xVelocity", rb.velocity.x);

        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("canDoubleJump", canDoubleJump);
        anim.SetBool("isSliding", isSliding);
        anim.SetBool("canClimb", canClimb);
        anim.SetBool("isKnocked", isKnocked);

        if(rb.velocity.y<-20)
            anim.SetBool("canRoll", true);
    }

    private void RollAnimFinished()=> anim.SetBool("canRoll", false);
    #endregion


    private void CheckCollision()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        wallDetected = Physics2D.BoxCast(wallCheck.position, wallCheckSize, 0, Vector2.zero, 0, whatIsGround);
        ceillingDetected = Physics2D.Raycast(transform.position,Vector2.up,ceillingCheckDistance, whatIsGround);

        Debug.Log("Trèo "+ledgeDetected);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position,new Vector2(transform.position.x,transform.position.y+ceillingCheckDistance));
        Gizmos.DrawWireCube(wallCheck.position, wallCheckSize);
    }
}
