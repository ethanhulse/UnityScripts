using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    #region classMembers
    Animator anim;
    Rigidbody2D rb;
    [SerializeField] Collider2D standingCollider;
    [SerializeField] Transform groundCheckCollider;
    [SerializeField] Transform overheadCheckCollider;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float jumpPower;
    public float movementModifier;
    public float runModifier = 2f;
    public float crouchSpeedModifier = 0.5f;
    float hVal;
    const float groundCheckRadius = 0.2f;
    const float overheadCheckRadius = 0.2f;
    bool facingRight = true;
    bool isRunning = false;
    bool isGrounded = false;
    bool crouchPressed;
    #endregion
    void Awake()
    {
        //Init player RB2D at game start
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }
    void Update()
    {
        //Poll for horizontal input at each frame
        hVal = Input.GetAxisRaw("Horizontal");

        //Poll for Lshift key for running
        if (Input.GetKeyDown(KeyCode.LeftShift))
            isRunning = true;
        if (Input.GetKeyUp(KeyCode.LeftShift))
            isRunning = false;
        #region Jump and Crouch Key
        //Jump
        if (Input.GetButtonDown("Jump"))
            Jump();
        //Crouch
        if (Input.GetButtonDown("Crouch"))
            crouchPressed = true;
        else if (Input.GetButtonUp("Crouch"))
            crouchPressed = false;

        //Set yvelocity in animator
        anim.SetFloat("yVelocity", rb.velocity.y);
        #endregion
    }
    void FixedUpdate()
    {
        //Check if player is standing on ground layer
        GroundCheck();
        //Call Jump function

        //Call move function with axis input each update
        Move(hVal, crouchPressed);
    }
    void GroundCheck()
    {
        isGrounded = false;
        //Check if player groundcheck object is colliding with ground colliders
        //If yes: isGrounded = true else:isGrounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckCollider.position, groundCheckRadius, groundLayer);
        if (colliders.Length > 0)
        {
            isGrounded = true;
        }
        //As long as isGrounded=true. jump animation bool is disabled
        anim.SetBool("Jump", !isGrounded);
    }
    void Jump()
    {
        if (isGrounded)
        {
            //OLD-> rb.AddForce(new Vector2(0f, jumpPower));
            rb.velocity = Vector2.up * jumpPower;
            anim.SetBool("Jump", true);
        }
    }
    void Move(float dir, bool crouchFlag)
    {
        #region crouch
        //If we are crouching and disabled crouching
        //Check for overhead collision with ground
        //If check returns true, remain crouched
        //Otherwise cancel crouch 
        if (!crouchFlag)
        {
            if (Physics2D.OverlapCircle(overheadCheckCollider.position, overheadCheckRadius, groundLayer))
                crouchFlag = true;
        }

        //If we hit crouch key, disable standingCollider while crouched + animate crouch
        //Reduce the speed while crouched to half walking speed (3)
        //if released, resume original speed and enable standing collider, disable crouch animation

        //Set Animation
        anim.SetBool("isCrouching", crouchFlag);
        standingCollider.enabled = !crouchFlag;
        //If player is grounded and presses space -> jump
        #endregion
        #region move&run

        //Current scale data.
        Vector3 currentScale = transform.localScale;

        //Base value of x axis velocity. Dictated by direction of axis and movement modifier float
        float xvalue = dir * movementModifier * Time.fixedDeltaTime;

        //If running, increase speed by runModifier
        if (isRunning)
            xvalue = dir * movementModifier * runModifier * Time.fixedDeltaTime;
        //If crouched, reduce speed by crouchSpeedModifier
        if (crouchFlag)
            xvalue = dir * movementModifier * crouchSpeedModifier * Time.fixedDeltaTime;

        //Final Run speed
        rb.velocity = new Vector2(xvalue, rb.velocity.y);

        //If player is facing right and axis is negative, flip left
        if (facingRight && dir < 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            facingRight = false;
        }
        //If player is facing left and axis is positive, flip right
        else if (!facingRight && dir > 0)
        {
            transform.localScale = new Vector3((transform.localScale.x * -1), transform.localScale.y, transform.localScale.z);
            facingRight = true;
        }
        currentScale = transform.localScale;
        //0 idle,3 crouched, 6 walking, 12 running
        //Set xvelocity of anim to abs value of x velocity
        anim.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
        #endregion

    }
}
