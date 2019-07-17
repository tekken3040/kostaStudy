using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftPanelManager : MonoBehaviour
{
    [SerializeField] Image[]        imgActions;         // 액션 버튼
    [SerializeField] Sprite[]       actionSprites;      // 액션에 따라 교체될 스프라이트
    [SerializeField] Sprite         unknownSprite;      // 감춰둘 액션 스프라이트

    private Button[]                btnActions;         // 액션 버튼 컴포넌트
    private ActionListBtn[]         actionListBtns;     // 액션 하이라이트 컨트롤러

    private void Start()
    {
        btnActions = new Button[imgActions.Length];
        for(int i=0; i<imgActions.Length; i++)
            btnActions[i] = this.transform.GetChild(0).GetChild(i).GetComponent<Button>();
        actionListBtns = new ActionListBtn[imgActions.Length];
        for(int i=0; i<imgActions.Length; i++)
            actionListBtns[i] = this.transform.GetChild(0).GetChild(i).GetComponent<ActionListBtn>();
    }

    // 초기화
    public void Init()
    {
        for(int i=0; i<imgActions.Length; i++)
            imgActions[i].sprite = unknownSprite;
    }

    // 액션버튼에 이미지 셋팅
    public void SetAction(Byte u1Slot, Manager.ACTION_TYPE _type)
    {
        imgActions[u1Slot].sprite = actionSprites[(Byte)_type];
    }

    // 액션 버튼 클릭
    public void OnClickActionButton(int slot)
    {
        for(int i=0; i<actionListBtns.Length; i++)
        {
            if(i.Equals(slot))
                actionListBtns[i].OnEdgeActive(true);
            else
                actionListBtns[i].OnEdgeActive(false);
        }
    }
}
