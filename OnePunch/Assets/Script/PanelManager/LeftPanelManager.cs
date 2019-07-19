using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftPanelManager : MonoBehaviour
{
    [SerializeField] private Image[]    imgActions;         // 액션 버튼
    [SerializeField] private Sprite[]   actionSprites;      // 액션에 따라 교체될 스프라이트

    private Button[]                    btnActions;         // 액션 버튼 컴포넌트
    private ActionListBtn[]             actionListBtns;     // 액션 하이라이트 컨트롤러

    private void Awake()
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
            SetAction((Byte)i, Defines.ACTION_TYPE.UNKNOWN);
    }

    // 액션버튼에 이미지 셋팅
    public void SetAction(Byte u1Slot, Defines.ACTION_TYPE _type)
    {
        imgActions[u1Slot].sprite = actionSprites[(Byte)_type];
    }

    // 액션 버튼 클릭
    public void OnClickActionButton(int slot)
    {
        for(int i=0; i<actionListBtns.Length; i++)
        {
            if(i.Equals(slot))
            {
                actionListBtns[i].OnEdgeActive(true);
                Manager.Instance.SetSelectActionSlot((Byte)i);
            }
            else
                actionListBtns[i].OnEdgeActive(false);
        }
    }
}
