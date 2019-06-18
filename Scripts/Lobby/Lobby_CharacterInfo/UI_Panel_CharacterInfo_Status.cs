using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class UI_Panel_CharacterInfo_Status : MonoBehaviour
{
    GameObject _lobbyScene;								//최상위 로비 패널
    public UI_Panel_CharacterInfo CharacterInfoPanel;	//캐릭터 세부정보 스크린
    public Text CharInfo_Name;							//캐릭터 이름
    public Text CharInfo_Level;							//캐릭터 레벨
    public Text CharInfo_ClassName;						//클래스 이름
    public Image CharInfo_ClassImg;						//클래스 초상화de
    public Text CharInfo_Exp;							//현재 경험치
    public Image CharInfo_ExpGague;						//현재 경험치 게이지
    public Image CharInfo_Element;						//캐릭터 속성
    public Text Status_Point;							//남은 스텟 포인트
    public Image Element_Icon;							//속성 아이콘
    public Button AutoStatus;							//자동 스텟 추가
    public Button Btn_Reset;							//스텟초기화
    public Button Btn_BuyPoint;							//스텟구매
    public Button Btn_UsePotion;						//경험치 물약 사용
    public GoodsInfo _goodsInfo;						//재화 그룹
    public UI_Status_UseExpPotion _potionPopup;			//물약사용 팝업
    //public GameObject objPointBuyPop;					//스텟 구매 팝업
    //public GameObject objResetPop;					//스텟 초기화 팝업
    //public GameObject objAutoStatPop;					//스텟 자동 투자 팝업
    //public GameObject objRetirePop;					//캐릭터 은퇴 팝업
    //public GameObject objRetirePopOk;					//캐릭터 은퇴 확인 팝업
    //public GameObject Pref_AddStatEffect;				//스텟 추가 이펙트 (StatusPoint02)
    public UI_CharPower objPowerValue;					//캐릭터 전투력
    private GameObject objStatusDetail;					//세부정보

    public GameObject[] Status_Point_Ability;			//스테이터스 종류
    public GameObject[] Status_BG;						//스테이터스 배경
    public GameObject[] Btn_Status_Point_Add;			//스테이터스 추가 버튼
    public GameObject[] Btn_Status_Point_Add_10;		//스테이터스 10 추가 버튼

    //public Vector3 V3Charactar_Rotation;				//캐릭터 회전값 (3.5, 171.7, 359.5);
    public Vector3[] V3CharInfo_ClassImgPos;			//클래스 초상화 좌표

    public Text CharInfo_ClassState;          //클래스 속성

    public Sprite[] _elementIcon;                   //속성 아이콘(0 == 불, 1 == 바람, 2 == 물)
    public Sprite[] _elementIcon2;                  //속성 아이콘(0 == 불, 1 == 바람, 2 == 물)
    //public Color[] RGB_Element;                   //속성 색(0 == 불 D6416BFF, 1 == 바람 39FFFFFF, 2 == 물 7DFF95FF)
    //public Color[] RGB_SelectedStat;              //선택된 스텟 하이라이트(0 == 비활성 colorCode = 727C88FF, 1 == 활성 colorCode = AEDE0AFF) 
    public Color[] RGB_Button;                      //버튼 색상(0 == 비활성, 1 == 활성)

    StringBuilder tempStringBuilder;                //스트링 출력용 스트링 빌더
    int _statPoint;                                 //현재 스텟포인트
    int _usedStatPoint;                             //사용한 스텟포인트
    UInt32[] _status;                               //스테이터스(0 == HP, 1 == STR, 2 == INT, 3 == DEF, 4 == RES, 5 == AGI, 6 == CRI)
	UInt32[] _statusAddPoint;                       //투자한 스텟 포인트(0 == HP, 1 == STR, 2 == INT, 3 == DEF, 4 == RES, 5 == AGI, 6 == CRI)
    Byte _buyStatPoint;                             //구입할 스텟 포인트 수량
	int _buyPrice;
    StatusComponent cHeroStat;
    private Status skillResult;

    public Hero cHero;                                     //현재 선택한 캐릭터 히어로 클래스
    public UI_Button_CharacterInfo_Equipment_StateInfo _stateInfoBtn;

    bool RetireCharacter = false;
    List<ExpPotion> lstExpPotion;

    void Awake()
    {
        tempStringBuilder = new StringBuilder();
        _lobbyScene = GameObject.Find("LobbyScene");
		_status = new UInt32[7];
		_statusAddPoint = new UInt32[7];
        for(int i=0; i<7; i++)
        {
            _status[i] = 0;
            _statusAddPoint[i] = 0;
        }
    }

    void OnEnable()
    {
		// 예외처리
		if(lstExpPotion != null)
        	lstExpPotion.Clear();
		else
            lstExpPotion = new List<ExpPotion>();

		Legion.Instance.cTutorial.CheckTutorial (MENU.CHARACTER_INFO);
        _statPoint = 0;
        RetireCharacter = false;
        for(int i=0; i<7; i++)
        {
            _statusAddPoint[i] = 0;
            _status[i] = 0;
        }
        
        //SetCharacterInfo(_lobbyScene.GetComponent<LobbyScene>().infoHero);
        SetCharacterInfo(CharacterInfoPanel.GetHero);
    }

    public void ReFreshData(Hero _Hero)
    {
        // 예외처리
		if(lstExpPotion != null)
        	lstExpPotion.Clear();
		else
            lstExpPotion = new List<ExpPotion>();

		Legion.Instance.cTutorial.CheckTutorial (MENU.CHARACTER_INFO);
        _statPoint = 0;
        RetireCharacter = false;
        for(int i=0; i<7; i++)
        {
            _statusAddPoint[i] = 0;
            _status[i] = 0;
        }
        
        SetCharacterInfo(_Hero);
    }

    public int GetStatPoint()
    {
        return _statPoint;
    }

    public void SwitchInfoMenu()
    {
        _potionPopup.RefreashLevelUpEff();
        if(_potionPopup.gameObject.activeSelf)
        {
            _potionPopup.CheckUsePotion(false);
        }
        else
        {
            CallBackSwitchMenu();
        }
    }

    public void CallBackSwitchMenu()
    {
        if(!RetireCharacter)
        {
            if(cHero == null)
                return;
            
            if(_statPoint != cHero.GetComponent<StatusComponent>().STAT_POINT)
            {
                UInt16[] point = new UInt16[7];
                for(int i=0; i<7; i++)
                {
                    point[i] = (UInt16)(_statusAddPoint[i] / EquipmentInfoMgr.Instance.GetAddStatusPerPoint((Byte)(i + 1)));
                }
                
                PopupManager.Instance.ShowLoadingPopup(1);
                UInt16[][] tempPoint = new UInt16[1][];
                tempPoint[0] = new UInt16[Server.ConstDef.CharStatPointType];
                tempPoint[0] = point;
                Server.ServerMgr.Instance.PointCharacterStatus(new Hero[1]{cHero}, tempPoint, RequestStatPointChange);
            }
            else
            {
                cHero.GetComponent<StatusComponent>().STAT_POINT = (Byte)_statPoint;            
                SetCharacterInfo(cHero);
            }
        }
    }

    public void OnClickBack()
    {
        _potionPopup.RefreashLevelUpEff();
        if(_potionPopup.gameObject.activeSelf)
        {
            //DebugMgr.LogError("물약 팝업 열려있음");
            _potionPopup.CheckUsePotion(true);
        }
        else
        {
            if(!RetireCharacter)
            {
                //DebugMgr.LogError("은퇴가 아님");
                if(cHero == null)
                    return;
                
                if(_statPoint != cHero.GetComponent<StatusComponent>().STAT_POINT)
                {
                    UInt16[] point = new UInt16[7];
                    for(int i=0; i<7; i++)
                    {
                        point[i] = (UInt16)(_statusAddPoint[i] / EquipmentInfoMgr.Instance.GetAddStatusPerPoint((Byte)(i + 1)));
                    }
                    
                    PopupManager.Instance.ShowLoadingPopup(1);
                    UInt16[][] tempPoint = new UInt16[1][];
                    tempPoint[0] = new UInt16[Server.ConstDef.CharStatPointType];
                    tempPoint[0] = point;
                    Server.ServerMgr.Instance.PointCharacterStatus(new Hero[1]{cHero}, tempPoint, RequestStatPointChange);
                }
                else
                {
                    Legion.Instance.eCharState = Legion.ChangeCharInfo.CHANGED;
                    cHero.GetComponent<StatusComponent>().STAT_POINT = (Byte)_statPoint;            
                    SetCharacterInfo(cHero);
                }
            }
            else
            {
                //DebugMgr.LogError("은퇴 임");
                if(_lobbyScene.GetComponent<LobbyScene>().prevScreen == 2)
                {
                	UI_CrewMenu crewMenu = _lobbyScene.transform.GetChild(0).FindChild("Pref_UI_Main_CrewMenu").GetComponent<UI_CrewMenu>();
                	crewMenu.InitCharacterList();
                	crewMenu.SetCrewCharacters();
                }
                Legion.Instance.eCharState = Legion.ChangeCharInfo.CHANGED;
            }
        }
    }
    public void CallBackBtnBack()
    {
        if(!RetireCharacter)
        {
            if(cHero == null)
                return;
            
            if(_statPoint != cHero.GetComponent<StatusComponent>().STAT_POINT)
            {
                UInt16[] point = new UInt16[7];
                for(int i=0; i<7; i++)
                {
                    point[i] = (UInt16)(_statusAddPoint[i] / EquipmentInfoMgr.Instance.GetAddStatusPerPoint((Byte)(i + 1)));
                }
                
                PopupManager.Instance.ShowLoadingPopup(1);
                UInt16[][] tempPoint = new UInt16[1][];
                tempPoint[0] = new UInt16[Server.ConstDef.CharStatPointType];
                tempPoint[0] = point;
                Server.ServerMgr.Instance.PointCharacterStatus(new Hero[1]{cHero}, tempPoint, RequestStatPointChange);
            }
            else
            {
                cHero.GetComponent<StatusComponent>().STAT_POINT = (Byte)_statPoint;            
                SetCharacterInfo(cHero);
            }
        }
        else
        {
            if(_lobbyScene.GetComponent<LobbyScene>().prevScreen == 2)
            {
				UI_CrewMenu crewMenu = _lobbyScene.transform.GetChild(0).FindChild("Pref_UI_Main_CrewMenu").GetComponent<UI_CrewMenu>();
				crewMenu.InitCharacterList();
				crewMenu.SetCrewCharacters();
            }
        }
    }
    //추가한 스텟 서버 전송
    public void RequestStatPointChange(Server.ERROR_ID err)
    {
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
            Legion.Instance.eCharState = Legion.ChangeCharInfo.NONE;
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CHAR_STAT_POINT, err), Server.ServerMgr.Instance.CallClear);
			return;
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
            _potionPopup.RefreashLevelUpEff();
            if(_potionPopup.gameObject.activeSelf)
            {
                _potionPopup.OnClickClose();
            }

            RetireCharacter = false;
            cHero.GetComponent<StatusComponent>().STAT_POINT = (Byte)_statPoint;            
            SetCharacterInfo(cHero);
            if(_lobbyScene.GetComponent<LobbyScene>().prevScreen == 1)
			{
                // 2017. 01. 30 jy
                // 스텟 추가후 캐릭터가 메인 캐릭터인지 확인하여 메인 캐릭일때만 SetCrewCharacters() 작동시킨다
                // 메인 캐릭이 아닌 상태에서 스텟을 증가시키면 경험치바가 활성화 되는 버그 예외 처리
                bool isMainCrew = false;
                for(int i = 0; i < Crew.MAX_CHAR_IN_CREW; ++i)
                {
                    if (Legion.Instance.SelectedCrew.acLocation[i] == null)
                        continue;

                    if( Legion.Instance.SelectedCrew.acLocation[i] == cHero as Character)
                        isMainCrew = true;
                }

                if (isMainCrew == true)
                    _lobbyScene.transform.GetChild(0).FindChild("Pref_UI_Main_CrewMenu").GetComponent<UI_CrewMenu>().SetCrewCharacters();			
			}
			else
			{
				_lobbyScene.GetComponent<LobbyScene>().SetCrewPower();
			}
			Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.CharStatPoint, 0, 0, 0, 0, (UInt16)_usedStatPoint);
			_usedStatPoint = 0;
            CharacterInfoPanel.SetToggleBtn();

            Legion.Instance.eCharState = Legion.ChangeCharInfo.CHANGED;
            PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.CharStatPoint);
        }
    }
    IEnumerator ClosePanel()
    {
        yield return new WaitForSeconds(0.1f);
        GameObject.Destroy(this.transform.parent.gameObject);
    }

    //팝업 출력용
    void emptyMethod2(object[] param)
    {

    }
    //스텟 투자 이펙트
    public void AddStatPointEffect(GameObject obj)
    {
        int _statnum = 0;
        
        switch(obj.name)
        {
            case "HP":
                _statnum = 0;
                break;

            case "STR":
                _statnum = 1;
                break;

            case "INT":
                _statnum = 2;
                break;

            case "DEF":
                _statnum = 3;
                break;

            case "RES":
                _statnum = 4;
                break;

            case "AGI":
                _statnum = 5;
                break;

            case "CRI":
                _statnum = 6;
                break;
        }
        if(Status_Point_Ability[_statnum].transform.FindChild("statEffect"))
            return;
        GameObject statEffect = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI_Effects/StatusPoint02.prefab", typeof(GameObject)) as GameObject);
        statEffect.name = "statEffect";
        statEffect.transform.SetParent(Status_Point_Ability[_statnum].transform);
        statEffect.transform.localPosition = Vector3.zero;
        statEffect.transform.localScale = Vector3.one;
    }
    //스텟투자
    public void OnClickAddStat(GameObject obj)
    {
        if(_statPoint == 0)
        {
            return;
        }

        _statPoint--;
        _usedStatPoint++;
        Status_Point.text = _statPoint.ToString();
        AddStatPointEffect(obj);

        switch(obj.name)
        {
            case "HP":
				_statusAddPoint[0] += (UInt16)(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(1));
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append((_status[0] + _statusAddPoint[0]));
                tempStringBuilder.Append(" <color=#35FFBA>(+");
                tempStringBuilder.Append(cHero.GetComponent<StatusComponent>().EquipBase.GetStat(1) + skillResult.GetStat(1));
                tempStringBuilder.Append(")</color>");
                Status_Point_Ability[0].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
                break;

            case "STR":
				_statusAddPoint[1] += (UInt16)(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(2));
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append((_status[1] + _statusAddPoint[1]));
                tempStringBuilder.Append(" <color=#35FFBA>(+");
                tempStringBuilder.Append(cHero.GetComponent<StatusComponent>().EquipBase.GetStat(2) + skillResult.GetStat(2));
                tempStringBuilder.Append(")</color>");
                Status_Point_Ability[1].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
                break;

            case "INT":
				_statusAddPoint[2] += (UInt16)(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(3));
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append((_status[2] + _statusAddPoint[2]));
                tempStringBuilder.Append(" <color=#35FFBA>(+");
                tempStringBuilder.Append(cHero.GetComponent<StatusComponent>().EquipBase.GetStat(3) + skillResult.GetStat(3));
                tempStringBuilder.Append(")</color>");
                Status_Point_Ability[2].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
                break;

            case "DEF":
				_statusAddPoint[3] += (UInt16)(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(4));
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append((_status[3] + _statusAddPoint[3]));
                tempStringBuilder.Append(" <color=#35FFBA>(+");
                tempStringBuilder.Append(cHero.GetComponent<StatusComponent>().EquipBase.GetStat(4) + skillResult.GetStat(4));
                tempStringBuilder.Append(")</color>");
                Status_Point_Ability[3].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
                break;

            case "RES":
				_statusAddPoint[4] += (UInt16)(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(5));
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append((_status[4] + _statusAddPoint[4]));
                tempStringBuilder.Append(" <color=#35FFBA>(+");
                tempStringBuilder.Append(cHero.GetComponent<StatusComponent>().EquipBase.GetStat(5) + skillResult.GetStat(5));
                tempStringBuilder.Append(")</color>");
                Status_Point_Ability[4].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
                break;

            case "AGI":
				_statusAddPoint[5] += (UInt16)(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(6));
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append((_status[5] + _statusAddPoint[5]));
                tempStringBuilder.Append(" <color=#35FFBA>(+");
                tempStringBuilder.Append(cHero.GetComponent<StatusComponent>().EquipBase.GetStat(6) + skillResult.GetStat(6));
                tempStringBuilder.Append(")</color>");
                Status_Point_Ability[5].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
                break;

            case "CRI":
				_statusAddPoint[6] += (UInt16)(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(7));
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append((_status[6] + _statusAddPoint[6]));
                tempStringBuilder.Append(" <color=#35FFBA>(+");
                tempStringBuilder.Append(cHero.GetComponent<StatusComponent>().EquipBase.GetStat(7) + skillResult.GetStat(7));
                tempStringBuilder.Append(")</color>");
                Status_Point_Ability[6].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
                break;
        }
        if(_statPoint == 0)
        {
            StatPointEffEnable(false);
            if (EquipmentInfoMgr.Instance.LIMIT_CHAR_STATPOINT == cHero.GetComponent<StatusComponent>().BuyPoint)
            {
                Btn_BuyPoint.interactable = false;
            }
            else
            {
                Btn_BuyPoint.interactable = true;
            }

            for(int k=0; k<7; k++)
            {
                Btn_Status_Point_Add[k].SetActive(false);
                Btn_Status_Point_Add_10[k].SetActive(false);
            }
            
			SkillAutoBtnEnable(false);
            CharacterInfoPanel.SetToggleBtn();
        }

        else
        {
            for(int i=0; i<7; i++)
            {
                if(_statPoint >= 20)
                    Btn_Status_Point_Add_10[i].SetActive(true);
                else
                    Btn_Status_Point_Add_10[i].SetActive(false);
            }
        }

        if((_statPoint >= 0) && (cHero.GetComponent<StatusComponent>().STAT_POINT > _statPoint))
        {
            SkillRestBtnEnable(true);
        }
        else
        {
            SkillRestBtnEnable(false);
        }
        objPowerValue.SetData(cHero, _statusAddPoint);
    }
    //스텟 10개 투자
    public void OnClickAddStat10(GameObject obj)
    {
        if(_statPoint == 0)
        {
            return;
        }

        _statPoint -= 10;
        _usedStatPoint += 10;
        Status_Point.text = _statPoint.ToString();
        AddStatPointEffect(obj);

        switch(obj.name)
        {
            case "HP":
				_statusAddPoint[0] += (UInt16)(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(1)*10);
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append((_status[0] + _statusAddPoint[0]));
                tempStringBuilder.Append(" <color=#35FFBA>(+");
                tempStringBuilder.Append(cHero.GetComponent<StatusComponent>().EquipBase.GetStat(1) + skillResult.GetStat(1));
                tempStringBuilder.Append(")</color>");
                Status_Point_Ability[0].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
                break;

            case "STR":
				_statusAddPoint[1] += (UInt16)(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(2)*10);
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append((_status[1] + _statusAddPoint[1]));
                tempStringBuilder.Append(" <color=#35FFBA>(+");
                tempStringBuilder.Append(cHero.GetComponent<StatusComponent>().EquipBase.GetStat(2) + skillResult.GetStat(2));
                tempStringBuilder.Append(")</color>");
                Status_Point_Ability[1].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
                break;

            case "INT":
				_statusAddPoint[2] += (UInt16)(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(3)*10);
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append((_status[2] + _statusAddPoint[2]));
                tempStringBuilder.Append(" <color=#35FFBA>(+");
                tempStringBuilder.Append(cHero.GetComponent<StatusComponent>().EquipBase.GetStat(3) + skillResult.GetStat(3));
                tempStringBuilder.Append(")</color>");
                Status_Point_Ability[2].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
                break;

            case "DEF":
				_statusAddPoint[3] += (UInt16)(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(4)*10);
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append((_status[3] + _statusAddPoint[3]));
                tempStringBuilder.Append(" <color=#35FFBA>(+");
                tempStringBuilder.Append(cHero.GetComponent<StatusComponent>().EquipBase.GetStat(4) + skillResult.GetStat(4));
                tempStringBuilder.Append(")</color>");
                Status_Point_Ability[3].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
                break;

            case "RES":
				_statusAddPoint[4] += (UInt16)(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(5)*10);
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append((_status[4] + _statusAddPoint[4]));
                tempStringBuilder.Append(" <color=#35FFBA>(+");
                tempStringBuilder.Append(cHero.GetComponent<StatusComponent>().EquipBase.GetStat(5) + skillResult.GetStat(5));
                tempStringBuilder.Append(")</color>");
                Status_Point_Ability[4].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
                break;

            case "AGI":
				_statusAddPoint[5] += (UInt16)(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(6)*10);
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append((_status[5] + _statusAddPoint[5]));
                tempStringBuilder.Append(" <color=#35FFBA>(+");
                tempStringBuilder.Append(cHero.GetComponent<StatusComponent>().EquipBase.GetStat(6) + skillResult.GetStat(6));
                tempStringBuilder.Append(")</color>");
                Status_Point_Ability[5].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
                break;

            case "CRI":
				_statusAddPoint[6] += (UInt16)(EquipmentInfoMgr.Instance.GetAddStatusPerPoint(7)*10);
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append((_status[6] + _statusAddPoint[6]));
                tempStringBuilder.Append(" <color=#35FFBA>(+");
                tempStringBuilder.Append(cHero.GetComponent<StatusComponent>().EquipBase.GetStat(7) + skillResult.GetStat(7));
                tempStringBuilder.Append(")</color>");
                Status_Point_Ability[6].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
                break;
        }

        if(_statPoint == 0)
        {
            StatPointEffEnable(false);
            if (EquipmentInfoMgr.Instance.LIMIT_CHAR_STATPOINT == cHero.GetComponent<StatusComponent>().BuyPoint)
                Btn_BuyPoint.interactable = false;
            else
                Btn_BuyPoint.interactable = true;

            for(int k=0; k<7; k++)
            {
                Btn_Status_Point_Add[k].SetActive(false);
                Btn_Status_Point_Add_10[k].SetActive(false);
            }
            
			SkillAutoBtnEnable(false);
            CharacterInfoPanel.SetToggleBtn();
        }

        else
        {
            for(int i=0; i<7; i++)
            {
                if(_statPoint >= 20)
                    Btn_Status_Point_Add_10[i].SetActive(true);
                else
                    Btn_Status_Point_Add_10[i].SetActive(false);
            }
        }

        if((_statPoint >= 0) && (cHero.GetComponent<StatusComponent>().STAT_POINT > _statPoint))
        {
            SkillRestBtnEnable(true);
        }
        else
        {
            SkillRestBtnEnable(false);
        }
        objPowerValue.SetData(cHero, _statusAddPoint);
    }
    //스텟 자동 투자
    public void OnClickAutoAddStat()
    {
        if(_statPoint == 0)
            return;

        object[] yesEventParam = new object[1];

        yesEventParam[0] = cHero;
        if(_potionPopup.gameObject.activeSelf)
            _potionPopup.Close();
        GameObject pop = Instantiate (AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Status/Pref_UI_AutoStatAdd.prefab", typeof(GameObject)) as GameObject);
		pop.transform.SetParent(PopupManager.Instance._objPopupManager.transform);
		pop.transform.localScale = Vector3.one;
		pop.transform.localPosition = Vector3.zero;
        pop.GetComponent<UI_Popup_AutoStatAdd>().SetRenderHeroStatus(yesEventParam);
        pop.GetComponent<YesNoPopup>().Show(TextManager.Instance.GetText("popup_title_stat_auto_char"), TextManager.Instance.GetText("popup_desc_stat_auto_char"), AddAutoStatus, yesEventParam);
        PopupManager.Instance.AddPopup(pop, pop.GetComponent<YesNoPopup>().OnClickNoWithDest);
        return;
    }

    public void CheckAddAutoStatus(object[] param)
    {
        if(_statPoint != cHero.GetComponent<StatusComponent>().STAT_POINT)
        {
            UInt16[] point = new UInt16[7];
            for(int i=0; i<7; i++)
            {
                point[i] = (UInt16)(_statusAddPoint[i] / EquipmentInfoMgr.Instance.GetAddStatusPerPoint((Byte)(i + 1)));
            }
            
            PopupManager.Instance.ShowLoadingPopup(1);
            UInt16[][] tempPoint = new UInt16[1][];
            tempPoint[0] = new UInt16[Server.ConstDef.CharStatPointType];
            tempPoint[0] = point;
            Server.ServerMgr.Instance.PointCharacterStatus(new Hero[1]{cHero}, tempPoint, RequestSendStatPoint3);
        }
        else
        {
            RequestSendStatPoint3(Server.ERROR_ID.NONE);
        }
    }
    public void RequestSendStatPoint3(Server.ERROR_ID err)
    {
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CHAR_STAT_POINT, err), Server.ServerMgr.Instance.CallClear);
			return;
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            object[] yesEventParam = new object[1];
            PopupManager.Instance.CloseLoadingPopup();
			Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.CharStatPoint, 0, 0, 0, 0, (UInt16)_usedStatPoint);
            SetCharacterInfo(cHero);
			_usedStatPoint = 0;
            AddAutoStatus(yesEventParam);
        }
    }
    public void AddAutoStatus(object[] param)
    {
        int SelectedAutoStat = 0;

        StatPointEffEnable(true);

        for (int i=_statPoint; i>0;)
        {
            for(int j=0; j<ClassInfo.MAX_CHARACTER_AUTO_STATUS; j++)
            {
                SelectedAutoStat = (int)(cHero.cClass.au1AutoStat[j]);
                _statusAddPoint[SelectedAutoStat-1] += (UInt16)EquipmentInfoMgr.Instance.GetAddStatusPerPoint((byte)SelectedAutoStat);
                Status_Point_Ability[SelectedAutoStat-1].transform.GetChild(2).GetComponent<Text>().text = (_status[SelectedAutoStat-1] + _statusAddPoint[SelectedAutoStat-1]).ToString();
                AddStatPointEffect(Status_Point_Ability[SelectedAutoStat-1]);
                i--;
                _statPoint--;
                _usedStatPoint++;
                if(i == 0)
                {
                    StatPointEffEnable(false);
                    if (EquipmentInfoMgr.Instance.LIMIT_CHAR_STATPOINT == cHero.GetComponent<StatusComponent>().BuyPoint)
                    {
                        Btn_BuyPoint.interactable = false;
                    }
                    else
                    {
                        Btn_BuyPoint.interactable = true;
                    }

					SkillAutoBtnEnable(false);
                    break;
                }
            }
        }
        for(int i=0; i<7; i++)
        {
            Btn_Status_Point_Add[i].SetActive(false);
            Btn_Status_Point_Add_10[i].SetActive(false);
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append((_status[i] + _statusAddPoint[i]));
            tempStringBuilder.Append(" <color=#35FFBA>(+");
            tempStringBuilder.Append(cHero.GetComponent<StatusComponent>().EquipBase.GetStat((Byte)(i+1)) + skillResult.GetStat((Byte)(i+1)));
            tempStringBuilder.Append(")</color>");
            Status_Point_Ability[i].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
        }
        Status_Point.text = _statPoint.ToString();
        if((_statPoint >= 0) && (cHero.GetComponent<StatusComponent>().STAT_POINT > _statPoint))
        {
            SkillRestBtnEnable(true);
        }
        else
        {
            SkillRestBtnEnable(false);
        }
        CharacterInfoPanel.SetToggleBtn();
        objPowerValue.SetData(cHero, _statusAddPoint);
    }
    
    public void OnClickStatusDetail()
    {
        if(objStatusDetail == null)
        {
            objStatusDetail = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Status/Popup_StatInfo.prefab", typeof(GameObject)) as GameObject);
            RectTransform rtTr = objStatusDetail.GetComponent<RectTransform>();
            rtTr.SetParent(PopupManager.Instance._objPopupManager.transform);
            rtTr.anchoredPosition3D = Vector3.zero;
            rtTr.localScale = Vector3.one;
            rtTr.sizeDelta = Vector3.zero;   
        }
        objStatusDetail.SetActive(true);
        objStatusDetail.GetComponent<UI_Popup_StatDetail>().SetData(cHero, _statusAddPoint);
    }

    //스텟 초기화
    int tempPrice;
    public void OnClickStatReset()
    {
        object[] yesEventParam = new object[1];
        tempPrice = ((int)LegionInfoMgr.Instance.cCharStatResetGoods.u4Count + ((int)LegionInfoMgr.Instance.charResetUpgrade * (int)cHero.GetComponent<StatusComponent>().ResetCount));
        if(tempPrice > LegionInfoMgr.Instance.charResetPriceMax)
                tempPrice = (int)LegionInfoMgr.Instance.charResetPriceMax;

		GameObject pop = Instantiate (AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Status/Pref_UI_StatusReset.prefab", typeof(GameObject)) as GameObject);

		EventDisCountinfo disInfo = EventInfoMgr.Instance.GetDiscountEventinfo (DISCOUNT_ITEM.RESETSTAT);
		if (disInfo != null) {
			pop.GetComponent<StatResetPopup>().SetDiscount((uint)tempPrice, disInfo.u1DiscountRate);
			tempPrice = (int)(tempPrice * disInfo.discountRate);
		}

        yesEventParam[0] = tempPrice;
        
		pop.transform.SetParent(PopupManager.Instance._objPopupManager.transform);
		pop.transform.localScale = Vector3.one;
		pop.transform.localPosition = Vector3.zero;
        pop.GetComponent<YesNoPopup>().Show(TextManager.Instance.GetText("popup_title_stat_reset_char"), tempPrice.ToString(), SendStatPoint, yesEventParam);
        PopupManager.Instance.AddPopup(pop, pop.GetComponent<YesNoPopup>().OnClickNo);
        return;
    }
    //투자한 스텟 서버 전송
    public void SendStatPoint(object[] param)
    {
        if(_statPoint != cHero.GetComponent<StatusComponent>().STAT_POINT)
        {
            UInt16[] point = new UInt16[7];
            for(int i=0; i<7; i++)
            {
                point[i] = (UInt16)(_statusAddPoint[i] / EquipmentInfoMgr.Instance.GetAddStatusPerPoint((Byte)(i + 1)));
            }
            
            PopupManager.Instance.ShowLoadingPopup(1);
            UInt16[][] tempPoint = new UInt16[1][];
            tempPoint[0] = new UInt16[Server.ConstDef.CharStatPointType];
            tempPoint[0] = point;
            Server.ServerMgr.Instance.PointCharacterStatus(new Hero[1]{cHero}, tempPoint, RequestSendStatPoint);
        }
        else
        {
            RequestStatReset(param);
        }
    }
    public void RequestSendStatPoint(Server.ERROR_ID err)
    {
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CHAR_STAT_POINT, err), Server.ServerMgr.Instance.CallClear);
			return;
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            object[] yesEventParam = new object[1];
            PopupManager.Instance.CloseLoadingPopup();
			Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.CharStatPoint, 0, 0, 0, 0, (UInt16)_usedStatPoint);
            SetCharacterInfo(cHero);
			_usedStatPoint = 0;
            RequestStatReset(yesEventParam);
        }
    }
    //스텟 초기화 요청
    public void RequestStatReset(object[] param)
    {
        if (!Legion.Instance.CheckEnoughGoods((int)LegionInfoMgr.Instance.cCharStatResetGoods.u1Type, tempPrice))
        {
            if(tempPrice > LegionInfoMgr.Instance.charResetPriceMax)
                tempPrice = (int)LegionInfoMgr.Instance.charResetPriceMax;

            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(Legion.Instance.GetConsumeString((int)LegionInfoMgr.Instance.cCharStatResetGoods.u1Type)).Append(TextManager.Instance.GetText("popup_desc_nocost"));
			PopupManager.Instance.ShowChargePopup(LegionInfoMgr.Instance.cCharStatResetGoods.u1Type);
            return;
        }
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.ResetCharacterStatus(cHero, DoneStatReset);
    }
    //스텟 초기화 처리 완료
    public void DoneStatReset(Server.ERROR_ID err)
    {
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CHAR_STAT_RESET, err), Server.ServerMgr.Instance.CallClear);
			return;
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
            LeanTween.textAlpha(Status_Point.GetComponent<RectTransform>(), 0.5f, 0.2f).setOnComplete(LeanTween.textAlpha(Status_Point.GetComponent<RectTransform>(), 1f, 0.2f).onCompleteObject).setEase(LeanTweenType.easeInBack).setLoopCount(2);
            for(int i=0; i<7; i++)
                _statusAddPoint[i] = 0;
            SetCharacterInfo(cHero);
            _goodsInfo.Refresh();
            CharacterInfoPanel.SetToggleBtn();
        }
    }

    //스텟 갱신
    public void ReloadStatus(GameObject obj)
    {
        
    }

    //캐릭터 삭제
    public void OnClickRetire()
    {
        bool equipmentTierLv = false;

        for(int i=0; i<cHero.acEquips.Length; i++)
        {
            if(cHero.acEquips[i].u1SmithingLevel > 2)
            {
                EquipmentTier3Up();
                equipmentTierLv = true;
                break;
            }
        }

        if(!equipmentTierLv)
        {
			ShowRequestRetirePopup(null);
            return;
        }
        //DebugMgr.LogError("은퇴 버튼 클릭");
    }

    void EquipmentTier3Up()
    {
        //DebugMgr.LogError("3티어 이상 장비 착용 중");
        PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_retire_char"), TextManager.Instance.GetText("popup_3tier_over_retire"), ShowRequestRetirePopup);
    }

    void ShowRequestRetirePopup(object[] param)
    {
        GameObject pop = Instantiate (AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Status/Pref_UI_Retire.prefab", typeof(GameObject)) as GameObject);
		pop.transform.SetParent(PopupManager.Instance._objPopupManager.transform);
		pop.transform.localScale = Vector3.one;
		pop.transform.localPosition = Vector3.zero;
		pop.GetComponent<InputPopup>().Show(TextManager.Instance.GetText("popup_title_retire_char"), 
			string.Format(TextManager.Instance.GetText("popup_desc_retire_char"), TextManager.Instance.GetText("input_retire")), 
			SendStatPoint2, param);
		PopupManager.Instance.AddPopup(pop, pop.GetComponent<InputPopup>().OnClickNo);
        //DebugMgr.LogError("은퇴 팝업");
    }

    //경험치물약을 위한 인벤 공간 검사
    public bool CheckInven()
    {
        //영웅 경험치 계산
        UInt64 u4Exp = 0;
        if(cHero.cLevel.u2Level>1)
            u4Exp = ClassInfoMgr.Instance.GetAccExp((UInt16)(cHero.cLevel.u2Level-1));
        Dictionary<UInt16, ConsumableItemInfo> tempConsumeItem = new Dictionary<ushort, ConsumableItemInfo>();
        tempConsumeItem = ItemInfoMgr.Instance.GetConsumableItemInfo();
        List<ConsumableItemInfo> lstConsumeItem = new List<ConsumableItemInfo>();
        Byte itemCnt = 0;
        for(int i=tempConsumeItem.Count; i>0;)
        {
            if(u4Exp < tempConsumeItem[(ushort)(58000+i)].u4Exp)
            {
                if(itemCnt != 0)
                {
                    lstConsumeItem.Add(tempConsumeItem[(ushort)(58000+i)]);
                }
                i--;
                itemCnt = 0;
                continue;
            }
            else
            {
                if(i != 4)
                {
                    u4Exp -= tempConsumeItem[(ushort)(58000+i)].u4Exp;
                    itemCnt++;
                }
                else
                {
                    i--;
                }
            }
        }

		if((lstConsumeItem.Count + Legion.Instance.cInventory.dicInventory.Count) < LegionInfoMgr.Instance.GetMaxInvenSize())
            return true;
        else
            return false;
    }
    
    //투자한 스텟 서버 전송
    public void SendStatPoint2(object[] param)
    {
		string inputMessage = param[1].ToString();
		// 입력 문구가 없거나 다르면
		if(inputMessage.Equals("") == true || inputMessage.Equals(TextManager.Instance.GetText("input_retire")) == false)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_input_retire_wrong"), TextManager.Instance.GetText("popup_desc_input_retire_wrong"), null);
			return;
		}

        if(!CheckInven())
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_warnning"), TextManager.Instance.GetText("popup_desc_retire_error_inven"), null);
			return;
        }
        if(_statPoint != cHero.GetComponent<StatusComponent>().STAT_POINT)
        {
            UInt16[] point = new UInt16[7];
            for(int i=0; i<7; i++)
            {
                point[i] = (UInt16)(_statusAddPoint[i] / EquipmentInfoMgr.Instance.GetAddStatusPerPoint((Byte)(i + 1)));
            }
            
            PopupManager.Instance.ShowLoadingPopup(1);
            UInt16[][] tempPoint = new UInt16[1][];
            tempPoint[0] = new UInt16[Server.ConstDef.CharStatPointType];
            tempPoint[0] = point;
            Server.ServerMgr.Instance.PointCharacterStatus(new Hero[1]{cHero}, tempPoint, RequestSendStatPoint2);
            //DebugMgr.LogError("은퇴전 스텟 포인트 전송");
        }
        else
        {
            RequestRetireCharacter(param);
            //DebugMgr.LogError("은퇴 요청");
        }
    }
    public void RequestSendStatPoint2(Server.ERROR_ID err)
    {
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CHAR_STAT_POINT, err), Server.ServerMgr.Instance.CallClear);
			return;
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
			Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.CharStatPoint, 0, 0, 0, 0, (UInt16)_usedStatPoint);
            SetCharacterInfo(cHero);
			_usedStatPoint = 0;
            RequestRetireCharacter(null);
            //DebugMgr.LogError("은퇴전 스텟 전송 완료");
        }
    }
    //캐릭터 삭제 요청
    public void RequestRetireCharacter(object[] param)
    {
        Crew tempCrew;
        if(cHero.u1RoomNum != 0)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_retire_char"), TextManager.Instance.GetText("mark_training_retire_wrong_desc"), null);
            return;
        }

        if(cHero.bAssignedLeagueCrew)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_retire_char"), TextManager.Instance.GetText("popup_desc_leave_league"), null);
            return;
        }
        else if(CharacterInfoPanel.GetInCrewList() == 1)
        {
            PopupManager.Instance.ShowLoadingPopup(1);
            Server.ServerMgr.Instance.RetireCharacter(cHero, DoneRetireCharacter);
            //DebugMgr.LogError("은퇴 요청");
        }
        else if(cHero.u1AssignedCrew != 0)
        {
            tempCrew = Legion.Instance.acCrews[cHero.u1AssignedCrew-1];

            if(tempCrew.DispatchStage != null)
            {
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_retire_char"), TextManager.Instance.GetText("popup_desc_retire_error_dispatch"), null);
                return;
            }
            else if(tempCrew.u1Count == 1)
            {
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_retire_char"), TextManager.Instance.GetText("popup_error_retire"), null);
                return;
            }
            
            PopupManager.Instance.ShowLoadingPopup(1);
            Server.ServerMgr.Instance.RetireCharacter(cHero, DoneRetireCharacter);
            //DebugMgr.LogError("은퇴 요청");
        }
        else
        {
            PopupManager.Instance.ShowLoadingPopup(1);
            Server.ServerMgr.Instance.RetireCharacter(cHero, DoneRetireCharacter);
            //DebugMgr.LogError("은퇴 요청");
        }
        PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.CharStatPoint);
    }
    //캐릭터 삭제 완료
    public void DoneRetireCharacter(Server.ERROR_ID err)
    {
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CHAR_RETIRE, err), Server.ServerMgr.Instance.CallClear);
			return;
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
            object[] yesEventParam = new object[1];
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(TextManager.Instance.GetText("popup_desc_retire_char_result"));
            if(_lobbyScene.GetComponent<LobbyScene>()._CrewMenu != null)
                _lobbyScene.GetComponent<LobbyScene>()._CrewMenu.GetComponent<UI_CrewMenu>().DeleteCharacterList();
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_btn_retire_char"), tempStringBuilder.ToString(), RetireSuccess);
            //DebugMgr.LogError("은퇴 처리 완료1");
            return;
        }
    }
    public void RetireSuccess(object[] param)
    {
        LobbyScene lobbyScene = _lobbyScene.GetComponent<LobbyScene>();
        for (int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
        {
            lobbyScene._characterPos.transform.GetChild(i).gameObject.SetActive(true);

            if(lobbyScene._CrewMenu != null)
            {
                if(Legion.Instance.acCrews[lobbyScene._CrewMenu.GetComponent<UI_CrewMenu>().GetSelectedCrew().u1Index-1].acLocation[i] != null)
                {
                    lobbyScene._characterPos.transform.GetChild(i).GetComponent<Button>().interactable = true;
                }
                else
                {
                    lobbyScene._characterPos.transform.GetChild(i).GetComponent<Button>().interactable = false;
                    lobbyScene._characterPos.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
            else
            {
                if(Legion.Instance.cBestCrew.acLocation[i] != null)
                {
                    lobbyScene._characterPos.transform.GetChild(i).GetComponent<Button>().interactable = true;
                }
                else
                {
                    lobbyScene._characterPos.transform.GetChild(i).GetComponent<Button>().interactable = false;
                    lobbyScene._characterPos.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }

        cHero.DestroyModelObject();
        RetireCharacter = true;
		Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.RetireChar, 0, 0, 0, 0, 1);
        Legion.Instance.eCharState = Legion.ChangeCharInfo.CHANGED;
        //캐릭터 정보창 닫기
        CharacterInfoPanel.OnClickBack();
    }
    //스탯 구매
    public void OnClickBuyStatPoint()
    {
        object[] yesEventParam = new object[1];

        cHeroStat = cHero.GetComponent<StatusComponent>();

        if(EquipmentInfoMgr.Instance.LIMIT_CHAR_STATPOINT == cHeroStat.BuyPoint)
            return;

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(TextManager.Instance.GetText("popup_desc_stat_buy_char"));
        GameObject pop = Instantiate (AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Skill/Pref_UI_PointBuy.prefab", typeof(GameObject)) as GameObject);
		pop.transform.SetParent(PopupManager.Instance._objPopupManager.transform);
		pop.transform.localScale = Vector3.one;
		pop.transform.localPosition = Vector3.zero;
        pop.GetComponent<UI_PointBuyPopup>().Show(TextManager.Instance.GetText("popup_title_stat_buy_char"), tempStringBuilder.ToString(), RequestBuyStatusPoint, null);
		pop.GetComponent<UI_PointBuyPopup>().SetBuyPointPopup (UI_PointBuyPopup.BuyType.Char, cHeroStat.BuyPoint);
        PopupManager.Instance.AddPopup(pop, pop.GetComponent<YesNoPopup>().OnClickNo);
        return;
    }
    
    public void RequestBuyStatusPoint(object[] param)
    {
		if(!Legion.Instance.CheckEnoughGoods(EquipmentInfoMgr.Instance.cCharStatGoods.u1Type, (int)param[1]))
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(Legion.Instance.GetConsumeString((int)EquipmentInfoMgr.Instance.cCharStatGoods.u1Type)).Append(TextManager.Instance.GetText("popup_desc_nocost"));
            PopupManager.Instance.ShowChargePopup(EquipmentInfoMgr.Instance.cCharStatGoods.u1Type);
            return;
        }
        _buyStatPoint = (Byte)param[0];
		_buyPrice = (int)param [1];
        PopupManager.Instance.ShowLoadingPopup (1);
        Server.ServerMgr.Instance.BuyCharacterStatusPoint(cHero, _buyStatPoint, BuyStatPoint);
    }

    public void BuyStatPoint(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CHAR_BUY_STAT_POINT, err), Server.ServerMgr.Instance.CallClear);
			return;
        }

        else if(err == Server.ERROR_ID.NONE)
        {
			Legion.Instance.SubGoods(EquipmentInfoMgr.Instance.cCharStatGoods.u1Type, _buyPrice);
            _statPoint += _buyStatPoint;
            Status_Point.text = _statPoint.ToString();
            StatPointEffEnable(true);
            //구매한 포인트 계산
            if (EquipmentInfoMgr.Instance.LIMIT_CHAR_STATPOINT == cHero.GetComponent<StatusComponent>().BuyPoint)
            {
                Btn_BuyPoint.interactable = false;
            }
            else
            {
                Btn_BuyPoint.interactable = true;
            }
            
            for(int i=0; i<7; i++)
            {
                Btn_Status_Point_Add[i].SetActive(true);
                if(_statPoint >= 20)
                    Btn_Status_Point_Add_10[i].SetActive(true);
                else
                    Btn_Status_Point_Add_10[i].SetActive(false);
            }
			SkillAutoBtnEnable(true);
            LeanTween.textAlpha(Status_Point.GetComponent<RectTransform>(), 0.5f, 0.2f).setOnComplete(LeanTween.textAlpha(Status_Point.GetComponent<RectTransform>(), 1f, 0.2f).onCompleteObject).setEase(LeanTweenType.easeInBack).setLoopCount(2);
            CharacterInfoPanel.SetToggleBtn();
        }
    }
    //물약 사용 팝업
    public void OnClickPotionPopup()
    {
        if(_statPoint != cHero.GetComponent<StatusComponent>().STAT_POINT)
        {
            UInt16[] point = new UInt16[7];
            for(int i=0; i<7; i++)
            {
                point[i] = (UInt16)(_statusAddPoint[i] / EquipmentInfoMgr.Instance.GetAddStatusPerPoint((Byte)(i + 1)));
            }
            
            PopupManager.Instance.ShowLoadingPopup(1);
            UInt16[][] tempPoint = new UInt16[1][];
            tempPoint[0] = new UInt16[Server.ConstDef.CharStatPointType];
            tempPoint[0] = point;
            Server.ServerMgr.Instance.PointCharacterStatus(new Hero[1]{cHero}, tempPoint, RequestStatPointChange2);
        }
        else
        {
            RequestStatPointChange2(Server.ERROR_ID.NONE);
        }
    }
    public void RequestStatPointChange2(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();

        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CHAR_STAT_POINT, err), Server.ServerMgr.Instance.CallClear);
			return;
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            cHero.GetComponent<StatusComponent>().STAT_POINT = (Byte)_statPoint;            
            SetCharacterInfo(cHero);
			Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.CharStatPoint, 0, 0, 0, 0, (UInt16)_usedStatPoint);
            SetCharacterInfo(cHero);
			_usedStatPoint = 0;
            StatPointEffEnable(false);
            _potionPopup.gameObject.SetActive(true);
            PopupManager.Instance.AddPopup(_potionPopup.gameObject, _potionPopup.OnClickClose);
            PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.CharStatPoint);
            
        }
    }
    //등급 얻기
    string _grade;
	public string GetPowerGrade(byte _powerGrade)
	{
        switch(_powerGrade)
        {
            case 1:
                _grade = "E";
                break;

            case 2:
                _grade = "D";
                break;

            case 3:
                _grade = "C";
                break;

            case 4:
                _grade = "B";
                break;

            case 5:
                _grade = "A";
                break;

            case 6:
                _grade = "S";
                break;

            case 7:
                _grade = "SS";
                break;
        }

		return _grade;
	}

    private Status tempHeroStatus;
    //캐릭터 정보 초기화
    public void SetCharacterInfo(Hero _hero)
    {
        cHero = _hero;
        _statPoint = 0;
        for(int i=0; i<7; i++)
            _statusAddPoint[i] = 0;

        StatusComponent statusCom = cHero.GetComponent<StatusComponent>();
        tempPrice = ((int)LegionInfoMgr.Instance.cCharStatResetGoods.u4Count + ((int)LegionInfoMgr.Instance.charResetUpgrade * (int)statusCom.ResetCount));

        if(tempPrice > LegionInfoMgr.Instance.charResetPriceMax)
            tempPrice = (int)LegionInfoMgr.Instance.charResetPriceMax;
        //속성
        int temp = EquipmentInfoMgr.Instance.GetInfo(cHero.acEquips[6].cItemInfo.u2ID).u1Element;

        //캐릭터 레벨과 이름
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(cHero.sName);
        CharInfo_Name.text = tempStringBuilder.ToString();

        //캐릭터 레벨
        CharInfo_Level.text = cHero.cLevel.u2Level.ToString();

        //캐릭터 클래스
        CharInfo_ClassName.text = TextManager.Instance.GetText(cHero.cClass.sName);
        CharInfo_ClassImg.sprite = AtlasMgr.Instance.GetSprite("Sprites/Hero/hero_icon." + cHero.cClass.u2ID);
        CharInfo_ClassImg.SetNativeSize();
        CharInfo_ClassImg.transform.localPosition = V3CharInfo_ClassImgPos[cHero.cClass.u2ID-1];
        if(cHero.cClass.u1BasicAttackElement == (Byte)ClassInfo.ATTACK_ELEMENT.PHYSICAL)
            CharInfo_ClassState.text = TextManager.Instance.GetText("phy");
        else if(cHero.cClass.u1BasicAttackElement == (Byte)ClassInfo.ATTACK_ELEMENT.MAGICAL)
            CharInfo_ClassState.text = TextManager.Instance.GetText("mag");
        _stateInfoBtn.SetData(cHero.cClass.u1BasicAttackElement);

        //현재 경험치
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
		tempStringBuilder.Append("EXP ");
        tempStringBuilder.Append(ConvertExpValue(cHero.cLevel.u8Exp)).Append(" / ").Append(ConvertExpValue(cHero.cLevel.u8NextExp));
        CharInfo_Exp.text = tempStringBuilder.ToString();
        CharInfo_ExpGague.fillAmount = (float)(((float)cHero.cLevel.u8Exp)/((float)cHero.cLevel.u8NextExp));

        //속성 아이콘
        Element_Icon.sprite = _elementIcon[temp-2];
        CharInfo_Element.sprite = _elementIcon2[temp-2];
        CharInfo_Element.SetNativeSize();

        statusCom.EquipBase.Clear();
        for(int i=0; i<Server.ConstDef.MaxItemSlot; i++)
        {
            statusCom.EquipBase.Add(cHero.acEquips[i].cFinalStatus);
        }
        statusCom.SetByLevel(cHero.cLevel);
        statusCom.CountingStatPoint(cHero.cLevel.u2Level);
        _statPoint = statusCom.STAT_POINT;

        //남은 스텟 포인트
        Status_Point.text = _statPoint.ToString();

        //구매한 포인트 계산
        if(_statPoint == 0)
        {
            StatPointEffEnable(false);
            if (EquipmentInfoMgr.Instance.LIMIT_CHAR_STATPOINT == statusCom.BuyPoint)
            {
                Btn_BuyPoint.interactable = false;
            }
            else
            {
                Btn_BuyPoint.interactable = true;
            }

            for(int i=0; i<7; i++)
            {
                Btn_Status_Point_Add[i].SetActive(false);
                Btn_Status_Point_Add_10[i].SetActive(false);
            }
            
			SkillAutoBtnEnable(false);
        }

        else
        {
            if (!_potionPopup.gameObject.activeSelf)
                StatPointEffEnable(true);
            if (EquipmentInfoMgr.Instance.LIMIT_CHAR_STATPOINT == statusCom.BuyPoint)
            {
                Btn_BuyPoint.interactable = false;
            }
            else
            {
                Btn_BuyPoint.interactable = true;
            }

            for(int i=0; i<7; i++)
            {
                Btn_Status_Point_Add[i].SetActive(true);
                if(_statPoint >= 20)
                    Btn_Status_Point_Add_10[i].SetActive(true);
                else
                    Btn_Status_Point_Add_10[i].SetActive(false);
            }

			SkillAutoBtnEnable(true);
        }
        if(statusCom.USE_POINT > 0)
        {
            SkillRestBtnEnable(true);
        }
        else
        {
            SkillRestBtnEnable(false);
        }
        CharacterInfoPanel.SetToggleBtn();
        skillResult = cHero.GetComponent<SkillComponent>().GetPassiveStatus();

        //현재 캐릭터의 스텟(HP, STR, INT, DEF, RES, AGI, CRI 순서)
        for(int i=0; i<Server.ConstDef.CharStatPointType; i++)
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(cHero.cFinalStatus.GetStat((Byte)(i+1)) + skillResult.GetStat((Byte)(i+1)));
            tempStringBuilder.Append(" <color=#35FFBA>(+");
            tempStringBuilder.Append(statusCom.EquipBase.GetStat((Byte)(i+1)) + skillResult.GetStat((Byte)(i+1)));
            tempStringBuilder.Append(")</color>");
            Status_Point_Ability[i].transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
            _status[i] = Convert.ToUInt32(cHero.cFinalStatus.GetStat((Byte)(i+1)) + skillResult.GetStat((Byte)(i+1)));
        }

        //전투력
        if(cHero == null)
            objPowerValue.SetData(_lobbyScene.GetComponent<LobbyScene>().infoHero, _statusAddPoint);
        else
            objPowerValue.SetData(cHero, _statusAddPoint);

        //경험치 물약 보유 계산
        int emptySlot = Legion.Instance.cInventory.dicInventory.Count;

        //New 체크
        List<UInt16> slots = new List<UInt16>();
        
        for(int i=1; i<=emptySlot; i++)
        {
            if(!Legion.Instance.cInventory.dicInventory.ContainsKey((ushort)i))
            {
                emptySlot++;
                continue;
            }

            if((ItemInfoMgr.Instance.GetItemType(Legion.Instance.cInventory.dicInventory[(UInt16)i].cItemInfo.u2ID) == ItemInfo.ITEM_ORDER.CONSUMABLE) && 
                ItemInfoMgr.Instance.GetConsumableItemInfo(Legion.Instance.cInventory.dicInventory[(UInt16)i].cItemInfo.u2ID).u4Exp != 0)
            {
                ExpPotion _expPotion = new ExpPotion();
                _expPotion.u2ID = Legion.Instance.cInventory.dicInventory[(UInt16)i].cItemInfo.u2ID;
                _expPotion._consumableItem = (ConsumableItem)Legion.Instance.cInventory.dicInventory[(UInt16)i];
                lstExpPotion.Add(_expPotion);
            }
        }

        if(lstExpPotion.Count > 0)
            PotionUseBtnEnable(true);
        else
            PotionUseBtnEnable(false);

        for(int i=0; i<lstExpPotion.Count; i++)
        {
            if(lstExpPotion[i]._consumableItem.isNew)
                slots.Add(lstExpPotion[i]._consumableItem.u2SlotNum);
        }
        if(slots.Count > 0)
        {
            Btn_UsePotion.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
            Btn_UsePotion.transform.GetChild(1).gameObject.SetActive(false);
    }

    public void CheckOpenPotionPopup(bool bOpen)
    {
        for(int i=0; i<7; i++)
        {
            if(bOpen)
            {
                Btn_Status_Point_Add[i].SetActive(false);
                Btn_Status_Point_Add_10[i].SetActive(false);
            }
            else
            {
                if(_statPoint == 0)
                {
                    Btn_Status_Point_Add[i].SetActive(false);
                    Btn_Status_Point_Add_10[i].SetActive(false);
                }
                else if(_statPoint >= 20)
                {
                    Btn_Status_Point_Add[i].SetActive(true);
                    Btn_Status_Point_Add_10[i].SetActive(true);
                }
                else
                {
                    Btn_Status_Point_Add[i].SetActive(true);
                    Btn_Status_Point_Add_10[i].SetActive(false);
                }
            }
        }
    }

    public string ConvertExpValue(UInt64 u8Exp)
    {
        string strConvertedExp = "0";

        if(u8Exp < 1000)
            return (strConvertedExp = u8Exp.ToString());

        int tempExp = (int)(Math.Log(u8Exp)/Math.Log(1000));
        strConvertedExp = String.Format("{0:F2}{1}", u8Exp/Math.Pow(1000, tempExp), "KMB".ToCharArray()[tempExp-1]);

        return strConvertedExp;
    }

    public void ConfirmNewPotion(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(((int)err).ToString()), Server.ServerMgr.Instance.CallClear);
			return;
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            Btn_UsePotion.transform.GetChild(2).gameObject.SetActive(true);
            for(int i=0; i<lstExpPotion.Count; i++)
            {
                lstExpPotion[i]._consumableItem.isNew = false;
            }
        }
    }

    public void StatPointEffEnable(bool isEnable)
    {
        Status_Point.transform.GetChild(0).gameObject.SetActive(isEnable);
    }

    public void StatPointEffAutoEnable()
    {
        // 스텟 포인트가 0보다 크면
        StatPointEffEnable(_statPoint > 0);
    }

	/// <summary>
	/// 2016. 12. 20 jy
	/// 스킬 자동 분배 버튼 비/활성화 버튼 함수
	/// </summary>
	public void SkillAutoBtnEnable(bool isEanble)
	{
		// 기존 코드
		//AutoStatus.GetComponent<ButtonClickAni>().enabled = isEanble //false;
		//AutoStatus.GetComponent<Button>().interactable = isEanble //false;
		//AutoStatus.transform.GetComponent<Image>().color = (isEanble == true) ?  RGB_Button[1] : RGB_Button[0];
		//AutoStatus.transform.GetChild(0).GetComponent<Text>().color = (isEanble == true) ?  RGB_Button[1] : RGB_Button[0];

		AutoStatus.interactable = isEanble;
		AutoStatus.transform.GetChild(0).GetComponent<Text>().color = (isEanble == true) ? Color.white : Color.gray;
	}

    public void SkillRestBtnEnable(bool isEanble)
    {
        if (isEanble == true)
        {
            Btn_Reset.interactable = true;
            Btn_Reset.GetComponent<ButtonClickAni>().enabled = true;
            Btn_Reset.targetGraphic.color = RGB_Button[1];//GetComponent<Image>().color = RGB_Button[1];
            Btn_Reset.transform.GetChild(0).GetComponent<Text>().color = RGB_Button[1];
        }
        else
        {
            Btn_Reset.interactable = false;
            Btn_Reset.GetComponent<ButtonClickAni>().enabled = false;
            Btn_Reset.targetGraphic.color = RGB_Button[0];//GetComponent<Image>().color = RGB_Button[0];
            Btn_Reset.transform.GetChild(0).GetComponent<Text>().color = RGB_Button[0];
        }
    }

    public void PotionUseBtnEnable(bool isEanble)
    {
        if (isEanble == true)
        {
            Btn_UsePotion.interactable = true;
            Btn_UsePotion.targetGraphic.color = RGB_Button[1];//GetComponent<Image>().color = RGB_Button[1];
            Btn_UsePotion.transform.GetChild(0).GetComponent<Text>().color = RGB_Button[1];
        }
        else
        {
            Btn_UsePotion.interactable = false;
            Btn_UsePotion.targetGraphic.color = RGB_Button[0];//GetComponent<Image>().color = RGB_Button[0];
            Btn_UsePotion.transform.GetChild(0).GetComponent<Text>().color = RGB_Button[0];
        }
    }
}