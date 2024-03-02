using Microsoft.Win32.SafeHandles;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const int PlayerRightDirection = 1;

    private Rigidbody2D rb;
    private Animator anim;
    public Transform groundCheck;
    public LayerMask WhatisGround;

    private float jumpTimerSet = 1.95f;
    private float movementInputDirection;
    private float jumpTimer;
    private float dashTimeLeft;
    private float lastImageXpos;
    private float lastDash;
    private float knockbackStartTime;
    private int facingDirection = 1;
    public int jumpExtra;

    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private float JumpForce;
    [SerializeField]
    private float airDragMultiPlier;
    [SerializeField]
    private float variableJumpHeight;
    [SerializeField]
    private float groundCheckRadius;
    [SerializeField]
    private float dashTime;
    [SerializeField]
    private float dashSpeed;
    [SerializeField]
    private float distanceBetweenImages;
    [SerializeField]
    private float dashCoolDown;
    [SerializeField]
    private float dashFirstTime;
    [SerializeField]
    private float knockbackDuration;
    [SerializeField]
    private int jumpExtraDuration = 2;


    private bool IsRunning;
    private bool isGrounded;
    private bool canJump;
    private bool isFacingRight;
    private bool isDashing;
    private bool canMove;
    private bool canFlip;
    private bool isKnockBack;


    public PlayerCombatController combat;

    [SerializeField]
    private Vector2 knockbackSpeed;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        combat = FindObjectOfType<PlayerCombatController>();
    }

    private void Start()
    {
        jumpExtra = jumpExtraDuration;
    }


    private void Update()
    {
        CheckInput();
        UpdateMoveAnimation();
        CheckJump();
        CheckDash();
        CheckKnockBack();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckMovementDirection();
        CheckSurroundings();
    }

    // Dieu kien dau vao de cho nhan vat co the di chuyen
    private void CheckInput()
    {
        movementInputDirection = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Horizontal"))
        {
            if (isGrounded && !isKnockBack && combat.noOfClick == 0)
            {
                canMove = true;
                canFlip = true;
            }
        }
        if (isGrounded)
        {
            jumpExtra = jumpExtraDuration;
        }
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                Jump();
            }
            //else if (!isGrounded)
            //{
            //    jumpTimer = jumpTimerSet;
            //}
        }
        if (combat.noOfClick > 0 && combat.noOfClick < 4)
        {
            rb.velocity = Vector2.zero;
        }
        if (Input.GetButtonUp("Jump") && jumpExtra >0) // Tao kha nang nhay cao cho nhan vat
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * variableJumpHeight);
            jumpExtra--;
        }
        else if (Input.GetButtonDown("Jump") && jumpExtra == 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpForce * 1.5f);
            jumpExtra--;
            if (jumpExtra < 0)
            {
                canJump = false;
            }
        }

        if (Input.GetButtonDown("Dash"))
        {
            if (Time.time < dashFirstTime || Time.time > (lastDash + dashCoolDown))
            {
                AttempToDash();
            }
        }
    }

    private void AttempToDash()
    {
        dashTimeLeft = dashTime;
        lastDash = Time.time;
        PlayerAfterImagePool.Instance.GetFromPool();
        lastImageXpos = transform.position.x;
    }

    // Ham kiem tra dash
    private void CheckDash()
    {
        if (dashTimeLeft > 0)
        {
            canMove = false;
            canFlip = false;
            rb.velocity = new Vector2(dashSpeed * facingDirection, rb.velocity.y);
            dashTimeLeft -= Time.deltaTime;
            if (Mathf.Abs(transform.position.x - lastImageXpos) > distanceBetweenImages)
            {
                PlayerAfterImagePool.Instance.GetFromPool();
                lastImageXpos = transform.position.x;
            }
        }
        else if (dashTimeLeft < 0 && combat.noOfClick == 0)
        {
            isDashing = false;
            canMove = true;
            canFlip = true;
        }
    }

    public void KnockBack(int direction)
    {
        isKnockBack = true;
        knockbackStartTime = Time.time;
        rb.velocity = new Vector2(knockbackSpeed.x * direction, knockbackSpeed.y);
        anim.SetBool("isKnockBack", true);
    }

    private void CheckKnockBack()
    {
        bool knockback = Time.time > knockbackStartTime + knockbackDuration;
        if (knockback && isKnockBack)
        {
            isKnockBack = false;
            anim.SetBool("isKnockBack", false);
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    // Ham kiem tra huong va cac chuyen dong cua nhan vat
    private void CheckMovementDirection()
    {
        if (canFlip)
        {
            if (isFacingRight && movementInputDirection > 0)
            {
                Flip();
            }

            if (!isFacingRight && movementInputDirection < 0)
            {
                Flip();
            }
        }

        if (rb.velocity.x != 0)
        {
            IsRunning = true;
        }
        else
        {
            IsRunning = false;
        }
    }

    // Ham thay doi huong nhin cua nhan vat
    private void Flip()
    {
        if (canFlip && !isKnockBack)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0, 180f, 0);
            facingDirection *= -1;
        }
    }

    public int GetFacingDirection()
    {
        return facingDirection;
    }

    public bool GetDashingState()
    {
        return isDashing;
    }

    // Ham su dung de kiem tra va cham voi mat dat
    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, WhatisGround); // Thiet lap kha nang va cham voi nen dat cho nhan vat
    }


    // Ham cap nhat cac animation cho nhan vat
    private void UpdateMoveAnimation()
    {
        if (IsRunning && !isKnockBack)
        {
            anim.SetBool("IsRunning", IsRunning);
        }
        else if (!IsRunning && !isKnockBack)
        {
            anim.SetBool("IsRunning", IsRunning);
        }

        anim.SetBool("IsGrounded", isGrounded);
        anim.SetFloat("YVelocity", rb.velocity.y);

    }

    // Ham tao kha nang nhay cho nhan vat
    private void Jump()
    {
        if (canJump && !isKnockBack)
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpForce * 1.5f);
            //jumpTimer = 0;
        }
    }

    // Ham kiem tra xem nhan vat da nhay hay chua
    private void CheckJump()
    {
        if (isGrounded && rb.velocity.y <= 0)
        {
            canJump = true;
        }
        else
        {
            canJump = false;
        }

        //if (jumpTimer > 0)
        //{
        //    if (isGrounded)
        //    {
        //        Jump();
        //    }
        //}

        //else
        //{
        //    jumpTimer -= Time.deltaTime;
        //}
    }


    // Vector van toc lam cho nhan vat co the di chuyen
    private void ApplyMovement()
    {
        if (isGrounded && canMove && !isKnockBack)
        {
            rb.velocity = new Vector2(movementInputDirection * movementSpeed, rb.velocity.y);
        }
        if (!isGrounded && movementInputDirection == 0 && !isKnockBack) // Thiet lap do giam van toc cho nhan vat khi nhay len khong trung
        {
            rb.velocity = new Vector2(rb.velocity.x * airDragMultiPlier, rb.velocity.y);
        }
        if (combat.noOfClick > 0 && combat.noOfClick < 4 && !isKnockBack)
        {
            rb.velocity = Vector2.zero;
        }
        if (combat.isPlayerDead)
        {
            rb.velocity = Vector2.zero;
            canFlip = false;
        }
    }

    // Ham hien thi kiem tra va cham cua nhan vat
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

}
