﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightPanelManager : MonoBehaviour
{
    [SerializeField] private Image[]    imgActions;         // 액션 이미지
    [SerializeField] private Sprite[]   actionSprites;      // 교체할 액션 스프라이트

    // 초기화
    public void Init()
    {
        for(int i=0; i<imgActions.Length; i++)
            SetActionImage((Byte)i, Defines.ACTION_TYPE.UNKNOWN);
    }

    // 액션 이미지 변경
    public void SetActionImage(Byte u1Slot, Defines.ACTION_TYPE _type)
    {
        imgActions[u1Slot].sprite = actionSprites[(Byte)_type];
    }
}
