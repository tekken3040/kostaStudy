using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomPanelManager : MonoBehaviour
{
    [SerializeField] private GameObject    objPanelList;   // 액션 버튼 리스트 오브젝트
    [SerializeField] private Button        btnPanelList;   // 액션 버튼 리스트의 버튼 컴포넌트
    [SerializeField] private Image[]       imgActions;     // 액션 버튼들
    [SerializeField] private Animator      panelAnimator;  // 패널 애니메이터

    private Button[]                       btnAction;      // 버튼 컴포넌트
    private LeftPanelManager               leftPanel;      // 왼쪽 패널 매니저 객체
    private Byte                           u1TempPower;    // 임시 파워 스택

    private void Start()
    {
        btnAction = new Button[imgActions.Length];
        for(int i=0; i<imgActions.Length; i++)
            btnAction[i] = objPanelList.transform.GetChild(i).GetComponent<Button>();
        leftPanel = GameObject.Find("LeftPanel").GetComponent<LeftPanelManager>();
    }

    // 액션 리스트 활성화
    public void EneableActionList()
    {
        if(!panelAnimator.enabled)
        {
            panelAnimator.enabled = true;
            SetBtnActive(true);
            return;
        }

        panelAnimator.SetBool("Open", true);
        SetBtnActive(true);
    }

    // 버튼 활성/비활성
    public void SetBtnActive(bool isActive)
    {
        //objPanelList.SetActive(isActive);
        btnPanelList.interactable = isActive;
    }

    // 확인 버튼 클릭 이벤트
    public void OnClickOk()
    {
        for(int i=0; i<Defines.Action_Cnt; i++)
        {
            if(Manager.Instance.PlayerActions[i].Equals(null) || Manager.Instance.PlayerActions[i].Equals((Byte)Defines.ACTION_TYPE.UNKNOWN))
            {
                // 아직 선택하지 않은 액션 슬롯이 있다고 팝업 출력
                PopupManager.Instance.ShowPopup(Defines.NotSelectedAction);
                return;
            }
        }
        Manager.Instance.SetReady();
        SetBtnActive(false);
        panelAnimator.SetBool("Open", false);
    }

    // 버튼 클릭 이벤트
    public void OnClickAction(String strType)
    {
        // 임시 파워스택에 현재 가드 스킬에 따라 스택을 부여
        u1TempPower = 0;
        for(int i=0; i<Manager.Instance.PlayerActions.Length; i++)
        {
            if(Manager.Instance.PlayerActions[i].Equals(Convert.ToByte(Defines.ACTION_TYPE.GUARD)))
                u1TempPower++;
        }

        switch(strType)
        {
            case "ATTACK":
                leftPanel.SetAction(Manager.Instance.GetSelectActionSlot(), Defines.ACTION_TYPE.ATTACK);
                Manager.Instance.PlayerActions[Manager.Instance.GetSelectActionSlot()] = Convert.ToByte(Defines.ACTION_TYPE.ATTACK);
                break;

            case "GUARD":
                leftPanel.SetAction(Manager.Instance.GetSelectActionSlot(), Defines.ACTION_TYPE.GUARD);
                Manager.Instance.PlayerActions[Manager.Instance.GetSelectActionSlot()] = Convert.ToByte(Defines.ACTION_TYPE.GUARD);
                break;

            case "GUARD_BREAK":
                leftPanel.SetAction(Manager.Instance.GetSelectActionSlot(), Defines.ACTION_TYPE.GUARD_BREAK);
                Manager.Instance.PlayerActions[Manager.Instance.GetSelectActionSlot()] = Convert.ToByte(Defines.ACTION_TYPE.GUARD_BREAK);
                break;

            case "HEAVY_ATTACK":
                if(u1TempPower.Equals(0))
                {
                    // 파워 스택이 없으면 필살기를 사용할 수 없음
                    return;
                }
                else if(u1TempPower.Equals(1) && Manager.Instance.PlayerActions[Manager.Instance.GetSelectActionSlot()].Equals(Convert.ToByte(Defines.ACTION_TYPE.GUARD)))
                {
                    // 현재 슬롯에서만 파워 스택을 얻을 수 있을 경우 필살기를 사용할 수 없음
                    return;
                }
                leftPanel.SetAction(Manager.Instance.GetSelectActionSlot(), Defines.ACTION_TYPE.HEAVY_ATTACK);
                Manager.Instance.PlayerActions[Manager.Instance.GetSelectActionSlot()] = Convert.ToByte(Defines.ACTION_TYPE.HEAVY_ATTACK);
                break;
        }
    }
}
