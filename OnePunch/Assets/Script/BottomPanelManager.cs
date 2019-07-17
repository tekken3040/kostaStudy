using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomPanelManager : MonoBehaviour
{
    [SerializeField] GameObject     objPanelList;   // 액션 버튼 리스트 오브젝트
    [SerializeField] Image[]        imgActions;     // 액션 버튼들

    private Button[]                btnAction;      // 버튼 컴포넌트
    private LeftPanelManager        leftPanel;      // 왼쪽 패널 매니저 객체

    private void Start()
    {
        btnAction = new Button[imgActions.Length];
        for(int i=0; i<imgActions.Length; i++)
            btnAction[i] = objPanelList.transform.GetChild(i).GetComponent<Button>();
        leftPanel = new LeftPanelManager();
    }

    // 버튼 활성/비활성
    public void SetBtnActive(bool isActive)
    {
        objPanelList.SetActive(isActive);
    }

    // 버튼 클릭 이벤트
    public void OnClickAction(String strType)
    {
        switch(strType)
        {
            case "ATTACK":
                leftPanel.SetAction(0, Manager.ACTION_TYPE.ATTACK);
                break;

            case "GUARD":
                leftPanel.SetAction(1, Manager.ACTION_TYPE.GUARD);
                break;

            case "GUARD_BREAK":
                leftPanel.SetAction(2, Manager.ACTION_TYPE.GUARD_BREAK);
                break;

            case "HEAVY_ATTACK":
                leftPanel.SetAction(3, Manager.ACTION_TYPE.HEAVY_ATTACK);
                break;
        }
    }
}
