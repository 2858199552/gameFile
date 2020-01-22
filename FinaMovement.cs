using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinaMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D coll;
    private Animator anim;

    public float speed, jumpForce/*跳跃力*/;
    public Transform groundCheck;
    public LayerMask ground;

    public bool isGround, isJump;

    bool jumpPressed;
    int jumpCount;//跳跃次数
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Jump") && jumpCount > 0)
        {
            jumpPressed = true;
        }
    }

    private void FixedUpdate()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, 0.1f, ground);//检测是否在地面

        GroundMovement();

        Jump();

        SwitchAnim();
    }

    void GroundMovement()//左右移动
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(horizontalMove * speed, rb.velocity.y);

        if(horizontalMove != 0)
        {
            transform.localScale = new Vector3(horizontalMove, 1, 1);//翻转，尺寸不变
        }
    }

    void Jump()
    {
        if(isGround)
        {
            jumpCount = 2;
            isJump = false;//跳跃动画
        }
        if(jumpPressed && isGround)
        {
            isJump = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount--;
            jumpPressed = false;//判断update已经检测完jumpPressed，保证手感
        }
        else if(jumpPressed && jumpCount > 0 && isJump)//二段跳
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount--;
            jumpPressed = false;
        }
    }

    void SwitchAnim()
    {
        anim.SetFloat("running", Mathf.Abs(rb.velocity.x));

        if(isGround)
        {
            anim.SetBool("falling", false);
        }

        else if(!isGround && rb.velocity.y > 0)
        {
            anim.SetBool("jumping", true);
        }
        else if(rb.velocity.y < 0)
        {
            anim.SetBool("jumping", false);
            anim.SetBool("falling", true);
        }
    }


}
