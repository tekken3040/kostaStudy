using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopPanelManager : MonoBehaviour
{
    [SerializeField] private Image           imgPlayerHP, imgEnemyHP;        // 플레이어와 적의 HP 이미지 fill amount사용
    [SerializeField] private Image           imgPlayerWins, imgEnemyWins;    // 플레이어와 적의 현재 승리 횟수 이미지
    [SerializeField] private GameObject[]    playerPowerStack;               // 플레이어의 파워 스택
    [SerializeField] private GameObject[]    enemyPowerStack;                // 적의 파워 스택
    [SerializeField] private Text            txtRoundCnt;                    // 진행중인 라운드
    [SerializeField] private Sprite[]        winSprites;                     // 승리 횟수 교체용 스프라이트

    private float fPlayerHP = 1f;                                            // 플레이어 HP fill amount 초기값
    private float fEnemyHP = 1f;                                             // 적 HP fill amount 초기값
    private float parameterHP = 0.01f;                                       // 백분률을 0~1로 정규화 하기 위한 변수

    // 승리 스프라이트 enum
    public enum WIN_SPRITE
    {
        WIN_ONE = 0,
        WIN_TWO,
        WIN_THREE,
    }

    private void Start()
    {
        Init();
    }

    // 초기화
    public void Init()
    {
        for(int i = 0; i < playerPowerStack.Length; i++)
        {
            playerPowerStack[i].SetActive(false);
            enemyPowerStack[i].SetActive(false);
        }
        imgPlayerHP.fillAmount = 1f;
        imgEnemyHP.fillAmount = 1f;
    }

    // HP이미지 fill amount 설정
    public void SetHPImage(Defines.TARGET target, float fHP)
    {
        switch(target)
        {
            case Defines.TARGET.PLAYER:
                imgPlayerHP.fillAmount -= fHP * parameterHP;
                break;

            case Defines.TARGET.ENEMY:
                imgEnemyHP.fillAmount -= fHP * parameterHP;
                break;
        }
    }

    // 라운드 횟수 설정
    public void SetRoundCount(Byte u1Count)
    {
        txtRoundCnt.text = u1Count.ToString();
    }

    // 승리 횟수 설정
    public void SetWinCount(Defines.TARGET target, WIN_SPRITE wins)
    {
        switch(target)
        {
            case Defines.TARGET.PLAYER:
                imgPlayerWins.sprite = winSprites[(Byte)wins];
                break;

            case Defines.TARGET.ENEMY:
                imgEnemyWins.sprite = winSprites[(Byte)wins];
                break;
        }
    }

    // 파워 스택
    public void SetPower(Byte u1Power, Defines.TARGET _target)
    {
        switch(_target)
        {
            case Defines.TARGET.PLAYER:
                playerPowerStack[u1Power].SetActive(true);
                break;

            case Defines.TARGET.ENEMY:
                enemyPowerStack[u1Power].SetActive(true);
                break;
        }
    }
}
