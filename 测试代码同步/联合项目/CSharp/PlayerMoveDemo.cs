using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>

public class PlayerMoveDemo : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D Rigid;  //刚体组件(不用设置)
    [HideInInspector] public float LRMove;//左右键检测(不用设置
    [HideInInspector] private Animator anim;
    public float MoveADDSpeed =0.5f;  //冲刺速度
    public float PlayerLeft;    //玩家左侧碰撞
    public float PlayerRight;  //玩家右侧碰撞
    public float Speed = 10;  //移动速度
    public LayerMask mask; //冲刺检测层
    public float length; //冲刺距离
    public bool HowMove; //移动方式(勾选普通，否则物理)
    private int ADKeyDown;  //左右键计数(不用设置)
    private int index;             //次数判断(不用设置)
    private float time;  //内置计时器(不用设置)
    private bool IsMoveADD; //是否正在冲刺;
    private Vector2 vector; //冲刺目标位置记录;

    private void Start()
    {
        Rigid = this.GetComponent<Rigidbody2D>();
        anim = this.GetComponent<Animator>();
    }
    private void OnEnable()
    {
        IsMoveADD = false;
    }
    private void Update()
    {
        if (!IsMoveADD)
        {
            if (time > 0.4f) index = 0;
            if (index != 0)
            {
                time += Time.deltaTime;
            }
            else time = 0;
            LRMove = Input.GetAxisRaw("Horizontal");
            AnimationMove();
            if (LRMove != 0)
            {
                if (HowMove)
                    NormalMove();
                else
                    RigidMove();
            }
            KeyDowns();
        }
    }
    private void RigidMove() //惯性移动
    {
        DirectionMove();
        Rigid.AddForce(transform.right * Speed * Mathf.Abs(LRMove) * Time.deltaTime);
    }
    private void NormalMove() //正常移动
    {
        DirectionMove();
        transform.position += transform.right * Speed * Time.deltaTime * Mathf.Abs(LRMove);
    }
    private void DirectionMove() //转向
    {
        if (LRMove < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }
    private void AnimationMove()
    {
        anim.SetBool("IsMove", LRMove != 0);
    }
    private void KeyDowns()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (ADKeyDown != 1) index = 0;
            ADKeyDown = 1;
            index += 1;
            if (index == 2 && ADKeyDown == 1)
            {
                anim.SetBool("MoveAdd", true);
                if (HowMove)
                    Delivery(PlayerLeft);
                else
                    Delivery();
            }
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (ADKeyDown != 2) index = 0;
            ADKeyDown = 2;
            index += 1;
            if (index == 2 && ADKeyDown == 2)
            {
                anim.SetBool("MoveAdd", true);
                if (HowMove)
                    Delivery(PlayerRight);
                else
                    Delivery();
            }
        }
    }
    private void Delivery(float direction)
    {
        RaycastHit2D hit;
        {
            if (hit = Physics2D.Raycast(this.transform.position, this.transform.right, length, mask.value)) 
                vector = new Vector2(hit.point.x - direction, this.transform.position.y);
            else vector=this.transform.position + this.transform.right * length;
        }
        StartCoroutine("MoveADD");
    }
    private void Delivery()
    {
        Rigid.AddForce(transform.right *length);
    }
    private IEnumerator MoveADD()
    {
        IsMoveADD = true;
        bool BigOrSmall;
        if (vector.x < this.transform.position.x) BigOrSmall = true;
        else BigOrSmall = false;
        for (; (this.transform.position.x - vector.x > 0.5f && BigOrSmall) || (vector.x - this.transform.position.x > 0.5f && !BigOrSmall);)
        {
            yield return new WaitForFixedUpdate();
            this.transform.position = Vector2.MoveTowards(this.transform.position, new Vector2(vector.x, this.transform.position.y), MoveADDSpeed);
        }
        IsMoveADD = false;
        anim.SetBool("MoveAdd", false);
    }
}
