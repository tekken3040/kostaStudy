using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{
    // 액션 타입 enum
    public enum ACTION_TYPE
    {
        ATTACK = 0,
        GUARD,
        GUARD_BREAK,
        HEAVY_ATTACK,
    }

    // 플레이어와 적 구분 enum
    public enum TARGET
    {
        NONE = 0,
        PLAYER,
        ENEMY,
    }

    [SerializeField] TopPanelManager        topPanel;       // 상단 매니저
    [SerializeField] LeftPanelManager       leftPanel;      // 왼쪽 매니저
    [SerializeField] RightPanelManager      rightPanel;     // 오른쪽 매니저
    [SerializeField] BottomPanelManager     bottomPanel;    // 바텀 매니저

    private Byte u1SelectedSlot = 0;                        // 현재 선택된 플레이어 액션 슬롯

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        topPanel.Init();
        rightPanel.Init();
        leftPanel.Init();
        leftPanel.OnClickActionButton(u1SelectedSlot);
    }

    // 액션 슬롯 설정
    public void SetSelectActionSlot(Byte u1slot)
    {
        u1SelectedSlot = u1slot;
    }

    // 액션 슬롯 가져오기
    public Byte GetSelectActionSlot()
    {
        return u1SelectedSlot;
    }
}
