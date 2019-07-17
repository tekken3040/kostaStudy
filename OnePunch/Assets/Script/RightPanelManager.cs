using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RightPanelManager : MonoBehaviour
{
    [SerializeField] Image[]    imgActions;         // 액션 이미지
    [SerializeField] Sprite[]   actionSprites;      // 교체할 액션 스프라이트
    [SerializeField] Sprite     unknownSprite;      // 감춰둘 액션 스프라이트

    // 초기화
    public void Init()
    {
        for(int i=0; i<imgActions.Length; i++)
            imgActions[i].sprite = unknownSprite;
    }

    // 액션 이미지 변경
    public void SetActionImage(Byte u1Slot, Manager.ACTION_TYPE _type)
    {
        imgActions[u1Slot].sprite = actionSprites[(Byte)_type];
    }
}
