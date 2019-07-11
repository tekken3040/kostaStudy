using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    [SerializeField] Button btnKick_L, btnKick_R, btnAttack_L, btnAttack_R;
    [SerializeField] Animator animator;

    public enum ATTACK_TYPE
    {
        NONE = 0,
        KICK_LEFT,
        KICK_RIGHT,
        PUNCH_LEFT,
        PUNCH_RIGHT,
    }

    int punchCnt = 0;
    int punchCnt2 = 0;

    public void OnClickAniBtn(string str)
    {
        if (punchCnt == 3)
            punchCnt = 0;
        if (punchCnt2 == 3)
            punchCnt2 = 0;
        
        switch (str)
        {
            case "Kick_L":
                animator.SetTrigger("Kick_L");
                break;

            case "Kick_R":
                animator.SetTrigger("Kick_R");
                break;

            case "Attack_L":
                if (punchCnt == 0)
                    animator.SetTrigger("Attack_L_1");
                else if (punchCnt == 1)
                    animator.SetTrigger("Attack_L_2");
                else if (punchCnt == 2)
                    animator.SetTrigger("Attack_L_3");
                punchCnt++;
                break;

            case "Attack_R":
                if (punchCnt2 == 0)
                    animator.SetTrigger("Attack_R_1");
                else if (punchCnt2 == 1)
                    animator.SetTrigger("Attack_R_2");
                else if (punchCnt2 == 2)
                    animator.SetTrigger("Attack_R_3");
                punchCnt2++;
                break;
        }
    }
}
