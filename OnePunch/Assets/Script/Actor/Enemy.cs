using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Actor
{
    private System.Random random;                   // 시간을 시드로 받는 랜덤 변수

    private void Start()
    {
        // 상속받은 액션 리스트 초기화
        u1ActionsList = new List<byte>();
        // 초기값인 알수없음으로 설정
        for(int i=0; i<Defines.Action_Cnt; i++)
            u1ActionsList.Add(Convert.ToByte(Defines.ACTION_TYPE.UNKNOWN));
    }
    
    // 랜덤변수로 사용할 액션변수 설정
    public void SetActions()
    {
        random = new System.Random();

        for(int i=0; i<Defines.Action_Cnt; i++)
        {
            byte u1Action = Convert.ToByte(random.Next(0, 4));
            u1ActionsList[i] = u1Action;
        }
    }
}
