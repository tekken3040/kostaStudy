using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayController : Singleton<PlayController>
{
    private bool isPlaying = false;                 // 라운드 진행중
    public bool PlayState
    {
        get{ return isPlaying; }
        set{ isPlaying = value; }
    }
    private byte u1CurrectActionStage = 5;          // 현재 진행중인 액션 슬롯

    // 라운드 시작 함수
    public void PlayStart()
    {
        isPlaying = true;
        StartCoroutine(PlayRound());
    }

    // 라운드 시작 코루틴
    IEnumerator PlayRound()
    {
        // 라운드 및 스타트 연출 추가

        // 연출 대기 시간 설정
        yield return new WaitForSeconds(2f);
        byte u1Temp = 0;
        u1Temp = u1CurrectActionStage;
        while(true)
        {
            if(u1CurrectActionStage.Equals(0))
            {
                // 모든 액션 종료. 다음 루틴 호출
                isPlaying = false;
                yield break;
            }
            // 각 액션당 대기 시간
            yield return new WaitForSeconds(2f);
            
        }
    }
}
