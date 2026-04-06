using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public PlayerState currentState;
    public PlayerIdleState idleState;
    public PlayerMoveState moveState;
    public PlayerJumpState jumpState;
    public PlayerCrouchState crouchState;

    [Header("Components")]
    public Rigidbody2D rb;
    public PlayerInput playerInput;
    public Animator anim;
    public CapsuleCollider2D playerCollider;

    [Header("Movement Variables")]
    public float walkSpeed;
    public float runSpeed;
    public float jumpForce;
    public float jumpCutMultiplier = .5f;
    public float normalGravity;
    public float fallGravity;
    public float jumpGravity;

    public int facingDirection = 1;
    //Inputs
    public Vector2 moveInput;
    public bool runPressed;
    public bool jumpPressed;
    public bool jumpReleased;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask groundLayer;
    public bool isGrounded;

    [Header("Crouch Check")]
    public Transform headCheck;
    public float headCheckRadius = .2f;

    [Header("Slide Settings")]
    public float slideDuration = 0.6f;
    public float slideSpeed = 8f;
    public float slideStopDuration = .15f;
    public float slideHeight;
    public Vector2 slideOffset;
    public float normalHeight;
    public Vector2 normalOffset;

    private bool isSliding;

    private void Awake()
    {
        idleState = new PlayerIdleState(this);
        moveState = new PlayerMoveState(this);
        jumpState = new PlayerJumpState(this);
        crouchState = new PlayerCrouchState(this);
    }


    private void Start()
    {
        rb.gravityScale = normalGravity;
        ChangeState(idleState);  
    }

    void Update()
    {
        CheckGrounded();
        currentState.Update();
        if(!isSliding)
            Flip();
        HandleAnimations();
    }

    void FixedUpdate()
    {
    currentState.FixedUpdate();

    }

    public void ChangeState(PlayerState newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
            currentState.Enter();
    }

    public void Move()
    {
        float speed = runPressed ? runSpeed : walkSpeed;
        rb.linearVelocity = new Vector2(moveInput.x * speed, rb.linearVelocity.y);
    }

    private void HandleSlide()
    {
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;   
            rb.linearVelocity = new Vector2(facingDirection * slideSpeed, rb.linearVelocity.y);

            if (slideTimer <= 0)
            {
                isSliding = false;
                slideStopTime = slideStopDuration; 
            }
        }
        if (slideStopTime > 0) 
        {
            slideStopTime -= Time.deltaTime;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);    
        }

        if(isGrounded && runPressed && moveInput.y < -.1f && !isSliding && !slideInputLocked)
        {
            isSliding = true;
            slideInputLocked = true;
            slideTimer = slideDuration;
            SetColliderSlide();
        }

        if(slideStopTime <0 && moveInput.y >= -.1f)
        {
            slideInputLocked = false;
        }   
    }

    public void SetColliderNormal()
    {
        playerCollider.size = new Vector2(playerCollider.size.x, normalHeight);
        playerCollider.offset = normalOffset;
    }

    public void SetColliderSlide()
    {
        playerCollider.size = new Vector2(playerCollider.size.x, slideHeight);
        playerCollider.offset = slideOffset;
    }

    public void ApplyVariableGravity()
    {
        if (rb.linearVelocity.y > 0.1f) //rising
        {
            rb.gravityScale = jumpGravity;
        }
        else if (rb.linearVelocity.y < -0.1f) //falling
        {
            rb.gravityScale = fallGravity;
        }
        else //not moving vertically
        {
            rb.gravityScale = normalGravity;
        }
    }

    void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public bool CheckForCeiling() 
    { 
        return Physics2D.OverlapCircle(headCheck.position, headCheckRadius, groundLayer);
    }

    void HandleAnimations()
    {
        bool isCrouching = anim.GetBool("isCrouching");
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isSliding", isSliding);
        anim.SetFloat("yVelocity", rb.linearVelocity.y);
        bool isMoving = Mathf.Abs(moveInput.x) > .1f && isGrounded;
    }

    void Flip()
    {
        if (moveInput.x > 0.1f)
        {
            facingDirection = 1;
        }
        else if (moveInput.x < -0.1f)
        {
            facingDirection = -1;
        }
        transform.localScale = new Vector3(facingDirection, 1, 1);
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnRun(InputValue value)
    {
        runPressed = value.isPressed;
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed && isGrounded)
        {
            jumpPressed = true;
            jumpReleased = false;
        }
        else //button released
        {
            jumpReleased = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(headCheck.position, headCheckRadius);
    }
}
