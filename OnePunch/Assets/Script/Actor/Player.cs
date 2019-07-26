using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Actor
{
    public enum ANIM_PARAMETER
    {
        Kick_L = 0,     //왼발킥
        Kick_R,         //오른발킥
        Attack_L_1,     //왼펀치1
        Attack_L_2,     //왼펀치2
        Attack_L_3,     //왼펀치3
        Attack_R_1,     //오른펀치1
        Attack_R_2,     //오른펀치2
        Attack_R_3,     //오른펀치3
        Blend,          //이동블랜드트리
        MoveX,          //이동X값
        MoveY,          //이동Z값
        Idle,           //아이들
        Jump,           //점프
        isGround,       //착지
        Jump_Double,    //더블점프
        Jump_Double_Fall,    //더블점프낙하중
    }

    [SerializeField] CharacterController characterController;
    [SerializeField] Animator anim;
    [SerializeField] float speed = 3f;
    [SerializeField] float jumpSpeed = 4f;

    Vector3 direction = Vector3.zero;
    float fJump = 0;
    bool isJump = false;
    private bool bAction = false;
    public bool isAction
    {
        get{ return bAction; }
        set{ bAction = value; }
    }

    public List<byte> ActionList
    {
        get{ return u1ActionsList; }
    }

    private void Start()
    {
        // 상속받은 액션 리스트 초기화
        u1ActionsList = new List<byte>();
        // 초기값인 알수없음으로 설정
        for(int i = 0; i < Defines.Action_Cnt; i++)
            u1ActionsList.Add(Convert.ToByte(Defines.ACTION_TYPE.UNKNOWN));
    }

    void Update()
    {
        //direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        //direction = this.transform.TransformDirection(direction);
    }

    void FixedUpdate()
    {
        //characterController.Move(direction*Time.deltaTime);
    }

    void SetMoveAnim(float _x, float _y)
    {
        if (!characterController.velocity.normalized.x.Equals(0) || !characterController.velocity.normalized.z.Equals(0))
        {
            anim.SetBool("Idle", false);
            if(!isJump)
                anim.SetFloat("Blend", 1);
        }
        else
        {
            if (!isJump)
                anim.SetFloat("Blend", 0);
            anim.SetBool("Idle", true);
        }

        anim.SetFloat("MoveX", _x);
        anim.SetFloat("MoveY", _y);
    }
}
