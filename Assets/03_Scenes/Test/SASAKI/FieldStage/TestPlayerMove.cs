using System.Collections;
using System.Collections.Generic;

// using UnityEditor.Tilemaps;
using UnityEngine;

public class TestPlayerMove : MonoBehaviour
{
    private float keyDirCheck;
    private bool isWallSliding = false;
    private bool isLeft = false;
    private bool isRightWall = false;
    private bool isLeftWall = false;

    private bool isWallJumping;
    private float wallJumpingDirection;
    private float wallJumingTime = 0.2f;
    private float inputBlockTimer;
    private float wallJumpingCounter;
    private float vx;
    [SerializeField] private float wallJumpingDuration = 0.3f;
    [SerializeField] private Vector2 wallJumpingPower = new Vector2(8f, 12f);
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float wallSlidingSpeed = 2f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform WallCheck;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool jumpPressed = false;
    private bool jumpReleased = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 左右の移動
        keyDirCheck = Input.GetAxis("Horizontal");

        if (Input.GetKey(KeyCode.D))
        {
            isLeft = false;
            vx = moveSpeed;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            isLeft = true;
            vx = -moveSpeed;
        }
        else
        {
            vx = 0;
        }

        // ジャンプボタンが押されたことをフラグで管理
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            jumpPressed = true;
        }

        // ジャンプボタンが離されたときの処理もフラグで管理
        if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0f)
        {
            jumpReleased = true;
        }

        WallSlide();
        WallJump();
    }

    private void FixedUpdate()
    {
        Debug.Log(isWallJumping);
        if (!isWallJumping && inputBlockTimer <= 0)
        {
            rb.velocity = new Vector2(vx, rb.velocity.y);
        }
        else if (isWallJumping && inputBlockTimer <= 0)
        {
            vx = Mathf.Clamp(vx, -vx / 2, vx / 2);
            rb.velocity = new Vector2(vx, rb.velocity.y);
        }

        // ジャンプ処理
        if (jumpPressed)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpPressed = false;
        }

        // ジャンプボタンが離された時の処理
        if (jumpReleased)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            jumpReleased = false;
        }
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
    }

    private bool IsWalled()
    {
        Vector2 origin = transform.position;

        if (Physics2D.OverlapCircle(origin - new Vector2(0.25f, 0.0f), 0.1f, groundLayer))
        {
            if (isLeft)
            {
                isLeftWall = true;
                if (isRightWall == true)
                {
                    inputBlockTimer = 0f;
                }
                return true;
            }
        }
        else if (Physics2D.OverlapCircle(origin + new Vector2(0.25f, 0.0f), 0.1f, groundLayer))
        {
            if (!isLeft)
            {
                isRightWall = true;
                if (isLeftWall == true)
                {
                    inputBlockTimer = 0f;
                }
                return true;
            }
        }

        isLeftWall = false;
        isRightWall = false;

        return false;
    }

    private void WallSlide()
    {
        if (IsWalled() && !isGrounded() && keyDirCheck != 0f)
        {
            isWallSliding = true;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -wallSlidingSpeed, float.MaxValue));
        }
        else
        {
            isWallSliding = false;
        }
    }

    private void WallJump()
    {
        if (isWallSliding)
        {
            if (inputBlockTimer <= 0)
            {
                isWallJumping = false;
            }

            if (isLeft)
            {
                wallJumpingDirection = Vector2.right.x;
            }
            else
            {
                wallJumpingDirection = Vector2.left.x;
            }

            wallJumpingCounter = wallJumingTime;
        }
        else
        {
            wallJumpingCounter -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isWallSliding)
        {
            isWallJumping = true;

            rb.velocity = new Vector2(wallJumpingDirection * wallJumpingPower.x, wallJumpingPower.y);

            wallJumpingCounter = 0f;
            inputBlockTimer = wallJumpingDuration;
        }

        inputBlockTimer -= Time.deltaTime;
        if (inputBlockTimer <= 0)
        {
            isWallJumping = false;
        }
    }
}