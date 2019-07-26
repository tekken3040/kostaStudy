using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Manager : Singleton<Manager>
{
    [SerializeField] private TopPanelManager        topPanel;       // 상단 매니저
    [SerializeField] private LeftPanelManager       leftPanel;      // 왼쪽 매니저
    [SerializeField] private RightPanelManager      rightPanel;     // 오른쪽 매니저
    [SerializeField] private BottomPanelManager     bottomPanel;    // 바텀 매니저

    private Byte u1SelectedSlot = 0;                                // 현재 선택된 플레이어 액션 슬롯
    private Byte u1PlayerPower = 0;                                 // 플레이어 파워 스택
    public Byte PlayerPower                                         // 플레이어 파워 스택 get set
    {
        get{ return u1PlayerPower; }
        set{ u1PlayerPower = value; }
    }
    private Byte u1EnemyPower = 0;                                  // 적 파워 스택
    public Byte EnemyPower                                          // 적 파워 스택 get set
    {
        get{ return u1EnemyPower; }
        set{ u1EnemyPower = value; }
    }

    private Byte[] u1PlayerActions;                                 // 플레이어가 선택한 액션들
    public Byte[] PlayerActions
    {
        get{ return u1PlayerActions; }
    }
    private Byte[] u1EnemyActions;                                  // 적이 선택한 액션들
    public Byte[] EnemyActions
    {
        get{ return u1EnemyActions; }
    }

    private GameObject objPlayer;                                   // 플레이어 오브젝트
    private Player cPlayer;                                         // 플레이어 컴포넌트
    private GameObject objEnemy;                                    // 적 오브젝트
    private Enemy cEnemy;                                           // 적 컴포넌트

    private void Awake()
    {
        u1PlayerActions = new Byte[Defines.Action_Cnt];
        u1EnemyActions = new Byte[Defines.Action_Cnt];
        for(int i=0; i<Defines.Action_Cnt; i++)
        {
            u1PlayerActions[i] = Convert.ToByte(Defines.ACTION_TYPE.UNKNOWN);
            u1EnemyActions[i] = Convert.ToByte(Defines.ACTION_TYPE.UNKNOWN);
        }
        objPlayer = GameObject.FindWithTag("Player");
        cPlayer = objPlayer.GetComponent<Player>();
        objEnemy = GameObject.FindWithTag("Enemy");
        cEnemy = objEnemy.GetComponent<Enemy>();
    }
    public void InitCall()
    {
        StartCoroutine(DelayedInit());
    }

    IEnumerator DelayedInit()
    {
        yield return null;
        if(SceneManager.GetActiveScene().isLoaded)
        {
            topPanel = GameObject.Find("TopPanel").GetComponent<TopPanelManager>();
            leftPanel = GameObject.Find("LeftPanel").GetComponent<LeftPanelManager>();
            rightPanel = GameObject.Find("RightPanel").GetComponent<RightPanelManager>();
            bottomPanel = GameObject.Find("BottomPanel").GetComponent<BottomPanelManager>();

            // 추후 연출이 들어가면 옴길것
            Init();
        }
        else
            StartCoroutine(DelayedInit());
    }

    // 초기화 Init함수
    private void Init()
    {
        topPanel.Init();
        rightPanel.Init();
        leftPanel.Init();
        leftPanel.OnClickActionButton(u1SelectedSlot);
        bottomPanel.EneableActionList();
    }

    // 플레이어 준비 완료
    public void SetReady( )
    {
        cPlayer.ActionList.Clear();
        for(int i=0; i<Defines.Action_Cnt; i++)
            cPlayer.ActionList.Add(u1PlayerActions[i]);
        cEnemy.SetActions();
        PlayController.Instance.PlayStart();
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
