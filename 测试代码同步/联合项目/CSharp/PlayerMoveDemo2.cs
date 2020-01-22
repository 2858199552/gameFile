using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>

public class PlayerMoveDemo2 : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D Rigid;    //刚体组件(不用设置)
    [HideInInspector] public float WSMove;            //WS检测(不用设置)
    [HideInInspector] public Animator anim;          //Animator动画组件(不用设置)
    [HideInInspector] public bool IsPlace;            //是否在地面上(不用设置)
    [HideInInspector] public float InPlaceTime;  //跳跃记时(不用设置)
    [HideInInspector] public RaycastHit2D hit;        //射线返回(不用设置)
    [Range(0, 5)] public float SpeedDown;            //下落速度
    [Range(0.001f,0.050f)] public float interval = 0.02f;  //跳跃降落与再次触发间隔(慎改！)
    [Range(0.01f,0.5f)] public float Percentage=0.1f;  //每帧上升百分比
    public float Height;             //跳跃高度
    public LayerMask maskUp;    //撞击层检测
    public LayerMask groundMask; //地面层检测
    public LayerMask maskWall; //墙壁检测层
    public float GameObjectHeight;   //物体高度
    public float GameObjectInPlace; //物体距离地面高度
    public float length;  //墙壁碰撞射线长度
    public int Count=30;                    //跳跃帧(一般不用管)
    private int index;                  //跳跃次数(不用设置)
    private bool IsInWall;   //是否降落墙边
    private void Start()
    {
        Rigid = this.GetComponent<Rigidbody2D>();
        anim = this.GetComponent<Animator>();
    }
    private void Update()
    {
        WSMove = Input.GetAxisRaw("Vertical");
        NormalJump();
    }
    private void OnEnable()
    {
        IsPlace = true;
    }
    private void NormalJump()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            index += 1;
            if (IsPlace)
            {
                IsPlace = false;
                StartCoroutine("JumpUse");
                anim.SetFloat("MoveWS", 1);
                anim.SetBool("IsInPlace", false);
            }
        }
    }
    private IEnumerator JumpUse()
    {
        Again:
        float MoveUP = this.transform.position.y + Height;
        InPlaceTime = 0;
        Rigid.gravityScale = 0;
        for (int i = 0; i <Count; i++)
        {
            if (Physics2D.Raycast(this.transform.position, transform.transform.up, GameObjectHeight, maskUp.value)) break;
            if (index == 2)
            {
                index += 1;
                goto Again;
            }
            yield return new WaitForFixedUpdate();
            this.transform.position=Vector2.Lerp(this.transform.position, new Vector2(this.transform.position.x, MoveUP),Percentage);
            InPlaceTime +=0.1f;
        }
        Rigid.gravityScale = SpeedDown;
        for (;InPlaceTime>0;)
        {
            if (Physics2D.Raycast(this.transform.position, -transform.transform.up, GameObjectInPlace, groundMask.value)) break;
             else if (Physics2D.Raycast(this.transform.position, transform.transform.right, length, maskWall.value)) break;
            InPlaceTime -= interval;
            yield return new WaitForFixedUpdate();
            if (index == 2)
            {
                index += 1;
                goto Again;
            }
        }
        IsPlace = true;
        index = 0;
        anim.SetBool("IsInPlace",true);
        anim.SetFloat("MoveWS", 0);
    }
}
