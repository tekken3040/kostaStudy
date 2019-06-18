using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UI_Panel_CharacterInfo : MonoBehaviour {
	
	public enum SUB_PANEL
	{
		NONE = 0,
		STATUS = 1,
		EQUIPMENT = 2,
		SKILL = 3,
        SMITH_RATING = 4,
	}
	SUB_PANEL openedPanel;
	
	[SerializeField] GameObject _resPanelStatus;
	[SerializeField] GameObject _resPanelEquipment;
	[SerializeField] GameObject _resPanelSkill;
	[SerializeField] RectTransform _trCharacterSlot;

    [SerializeField] Button ElementInfo;
    [SerializeField] Button ComboInfo;
    [SerializeField] GameObject[] ElementDialog;
    [SerializeField] GameObject[] ComboDialog;
    [SerializeField] GameObject AlramSmithing;
    [SerializeField] GameObject[] CharacterSlot;
	//	[SerializeField] UI_Panel_CharacterInfo_Equipment _panelStatus;
	UI_Panel_CharacterInfo_Equipment _panelEquipment;
	//	[SerializeField] UI_Panel_CharacterInfo_Equipment _panelSkill;
	UI_Panel_CharacterInfo_Status _panelStatus;
	
	UI_SkillInfo _panelSkill;
	
	GameObject _objCurrentPanel;
    GameObject _objUpgradeForgePanel;
    public GameObject _objPower;

    Hero _cHero;
    public Hero GetHero
    {
        get
        {
            return _cHero;
        }
    }
	LobbyScene _parentScript;

    int _inCrewList;
	
	public Toggle[] acToggleBtn;
	bool bClose = false;

	public Animator _MenuAniContorller;
	private bool m_isMenuHide;
	
    public UI_Panel_CharacterInfo_Equipment GetPanelEquipment()
    {
        return _panelEquipment;
    }
    public UI_Panel_CharacterInfo_Status GetPanelStatus()
    {
        return _panelStatus;
    }

    public int GetInCrewList()
    {
        return _inCrewList;
    }

	void InitCharacterInfo()
	{
		openedPanel = SUB_PANEL.NONE;
		
		bClose = false;
		OnClickMenu(1);
	}
	
	void DisableToggle(){
		for(int i=0; i<acToggleBtn.Length; i++){
			acToggleBtn [i].GetComponent<ToggleImage> ().image.gameObject.SetActive (false);
		}
	}
	
	public void SetData(Hero hero, LobbyScene parent, int _inList)
	{
		_parentScript = parent;
		_cHero = hero;
        _inCrewList = _inList;
		InitCharacterInfo();
        if(_inCrewList == 1)
            DebugMgr.LogError("InList == 1");
		_panelStatus = _resPanelStatus.GetComponent<UI_Panel_CharacterInfo_Status>();
		_panelEquipment = _resPanelEquipment.GetComponent<UI_Panel_CharacterInfo_Equipment>();
		_panelSkill = _resPanelSkill.GetComponent<UI_SkillInfo> ();

        if(_inCrewList == 0 || _inCrewList == 2)
        {
            for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
            {
                CharacterSlot[i].transform.localScale = new Vector3(0.8f, 0.8f, 1f);
                if(Legion.Instance.cBestCrew.acLocation[i] == null)
                    CharacterSlot[i].SetActive(false);
                else
                {
                    CharacterSlot[i].SetActive(true);
                    CharacterSlot[i].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Hero/hero_icon." + Legion.Instance.cBestCrew.acLocation[i].cClass.u2ID);
                    CharacterSlot[i].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().SetNativeSize();
                }
                if(_cHero == (Hero)Legion.Instance.cBestCrew.acLocation[i])
                    CharacterSlot[i].transform.localScale = Vector3.one;
            }
            //for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
            //{
            //    if(Legion.Instance.cBestCrew.acLocation[i] != null)
            //    {
            //        CharacterSlot[i].transform.localScale = Vector3.one;
            //        break;
            //    }
            //}
        }
        else
        {
            for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
            {
                CharacterSlot[i].SetActive(false);
            }
        }
        
        //if(_cHero.cObject != null)
		    //_cHero.cObject.transform.parent = transform;
        /*else
        {
            _cHero.InitModelObject();
            _cHero.cObject.transform.parent = transform;
        }*/
		//_cHero.cObject.transform.localPosition = new Vector3(0f, -310f, -720f);
        //_cHero.cObject.transform.localPosition = new Vector3(0f, -280f, -720f);
		//_cHero.cObject.transform.rotation = Quaternion.Euler(new Vector3(3.5f, 171.7f, 359.5f));
        //_cHero.cObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 171.7f, 0f));
		//_cHero.cObject.transform.localScale = new Vector3(600f, 600f, 600f);

        //_cHero.cObject.transform.parent.localScale = new Vector3(36f, 36f, 36f);

        switch(hero.cClass.u2ID)
		{
		case 1:
			_cHero.cObject.transform.localScale = Vector3.one;
			break;
		case 2: case 10: case 7: case 8: case 9:
			_cHero.cObject.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f); 
			break;
		case 3: case 5: case 6:
			_cHero.cObject.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
			break;
		case 4:
			_cHero.cObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f); 
			break;
		}
        
		_cHero.InitModelObject();
		_cHero.cObject.transform.parent = ObjMgr.Instance.transform;
		_cHero.cObject.transform.localPosition = new Vector3(27f, 0f, 10f);
		_cHero.cObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
		//_cHero.cObject.transform.localScale = new Vector3(1f, 1f, 1f);
		_trCharacterSlot.GetComponent<RotateCharacter> ().characterTransform = _cHero.cObject.transform;
        _cHero.cObject.GetComponent<HeroObject>().SetLayer(LayerMask.NameToLayer("BGMainMap"));
		
        //메뉴버튼 알림 설정
        SetToggleBtn();
        PopupManager.Instance.AddPopup(gameObject, OnClickBack);
	}
	
    public void OnClickCharacterSlot(int idx)
    {
        StartCoroutine(SwitchCharacter(idx)); 
    }

    IEnumerator SwitchCharacter(int idx)
    {
        Legion.Instance.eCharState = Legion.ChangeCharInfo.NONE;

        CheckClose();

        while(true)
        {
            if(Legion.Instance.eCharState == Legion.ChangeCharInfo.CHANGED)
            {
                Legion.Instance.eCharState = Legion.ChangeCharInfo.NONE;
                break;
            }
            else if(Legion.Instance.eCharState == Legion.ChangeCharInfo.NONE)
                yield return new WaitForEndOfFrame();
            else if(Legion.Instance.eCharState == Legion.ChangeCharInfo.ERROR)
            {
                yield break;
            }
        }

        for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
            CharacterSlot[i].transform.localScale = new Vector3(0.8f, 0.8f, 1f);

        if(_cHero.cObject !=null)
        {
            if(_inCrewList == 0 || _inCrewList == 2)
            {
                _cHero.cObject.transform.localScale = Vector3.one;
                _cHero.cObject.GetComponent<HeroObject>().LoadTransform();
                _cHero.cObject.SetActive(true);
            }
            else
                _cHero.DestroyModelObject();
        }
        
        _cHero = (Hero)Legion.Instance.cBestCrew.acLocation[idx];
        CharacterSlot[idx].transform.localScale = Vector3.one;
        CharacterSlot[idx].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Hero/hero_icon." + _cHero.cClass.u2ID);
        CharacterSlot[idx].transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>().SetNativeSize();
        
        InitCharacterInfo();
        _panelStatus.ReFreshData(_cHero);
        switch(_cHero.cClass.u2ID)
		{
		case 1: case 6: case 7: case 8: case 9:
			_cHero.cObject.transform.localScale = Vector3.one;
			break;
		case 2: case 10:
			_cHero.cObject.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f); 
			break;
		case 3: case 5:
			_cHero.cObject.transform.localScale = new Vector3(1.05f, 1.05f, 1.05f);
			break;
		case 4:
			_cHero.cObject.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f); 
			break;
		}
        if(_cHero.cObject != null)
            _cHero.cObject.GetComponent<HeroObject>().SaveTransform();
		_cHero.InitModelObject();
		_cHero.cObject.transform.parent = ObjMgr.Instance.transform;
		_cHero.cObject.transform.localPosition = new Vector3(27f, 0f, 10f);
		_cHero.cObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));
		//_cHero.cObject.transform.localScale = new Vector3(1f, 1f, 1f);
		_trCharacterSlot.GetComponent<RotateCharacter> ().characterTransform = _cHero.cObject.transform;
        _cHero.cObject.GetComponent<HeroObject>().SetLayer(LayerMask.NameToLayer("BGMainMap"));
		
        //메뉴버튼 알림 설정
        SetToggleBtn();
    }

    public void OpenInven()
    {
        _parentScript.OnClickInventory();
    }

    public void OpenUpgradeForge()
    {
        if(_objUpgradeForgePanel == null)
        {
            _objUpgradeForgePanel = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Equipment/Pref_UI_SubPanel_Forge_UpgradeForge.prefab", typeof(GameObject)) as GameObject);
			RectTransform rcTr = _objUpgradeForgePanel.GetComponent<RectTransform>();
            rcTr.SetParent(_parentScript.transform);
            //rcTr.SetParent(PopupManager.Instance._objPopupManager.transform);//_parentScript.transform);
            //rcTr.SetAsFirstSibling();
            rcTr.localPosition = Vector3.zero;
			rcTr.localScale = Vector3.one;
			rcTr.sizeDelta = Vector3.zero;
			rcTr.Find("Panel/Btn_Close").GetComponent<Button>().onClick.AddListener(()=>CloseUpgradeForge());
        }
        else
            _objUpgradeForgePanel.SetActive(true);

        _panelStatus.StatPointEffEnable(false);
        PopupManager.Instance.AddPopup(_objUpgradeForgePanel, CloseUpgradeForge);
    }

    public void CloseUpgradeForge()
    {
        if (openedPanel == SUB_PANEL.STATUS)
            _panelStatus.StatPointEffAutoEnable();
        else if (openedPanel == SUB_PANEL.SKILL)
            _panelSkill.SkillPointEffAutoEnable();

        _objUpgradeForgePanel.SetActive(false);
        PopupManager.Instance.RemovePopup(_objUpgradeForgePanel);
    }

	public void OnClickMenu(int menuNum)
	{
		if(bClose) 
			return;
		
		if(menuNum == 4)
		{
			if(openedPanel == SUB_PANEL.STATUS)
            {
				_panelStatus.SwitchInfoMenu();
                OpenInven();
            }
			else if(openedPanel == SUB_PANEL.EQUIPMENT)
                _panelEquipment.CheckChangeEquip();
            else if(openedPanel == SUB_PANEL.SKILL)
            {
                _panelSkill.SubmitSkillSetting();
                OpenInven();
            }
			//OpenInven();
			return;
		}

        if(menuNum == 5)
		{
            if (openedPanel == SUB_PANEL.STATUS)
            {
                //_panelStatus.SwitchInfoMenu();
                OpenUpgradeForge();
            }
            else if (openedPanel == SUB_PANEL.EQUIPMENT)
            {
                //_panelEquipment.CheckChangeEquip(); 
                OpenUpgradeForge();
            }
            else if (openedPanel == SUB_PANEL.SKILL)
            {
                //_panelSkill.SubmitSkillSetting();
                _panelSkill.SkillPointEffEnable(false);
                OpenUpgradeForge();
            }
			//OpenInven();
			return;
		}

		for(int i=0; i<acToggleBtn.Length; i++)
		{
			if (i == menuNum - 1) {
				acToggleBtn [i].GetComponent<ToggleImage> ().image.gameObject.SetActive (true);
			} else {
				acToggleBtn [i].GetComponent<ToggleImage> ().image.gameObject.SetActive (false);
			}
		}

		DebugMgr.Log("OnClickMenu : " + menuNum);

		if(openedPanel != (SUB_PANEL)menuNum)
		{
			switch((SUB_PANEL)menuNum)
			{
			case SUB_PANEL.STATUS:
                    Legion.Instance.googleAnalytics.LogEvent(new EventHitBuilder().SetEventCategory("Status").SetEventAction("Open").SetEventLabel("StatusOpen"));
				if(openedPanel == SUB_PANEL.SKILL) 
					_panelSkill.SubmitSkillSetting();
				else if(openedPanel == SUB_PANEL.EQUIPMENT) 
					_panelEquipment.OnClickBack();
                else if (openedPanel == SUB_PANEL.SMITH_RATING)
                        OpenUpgradeForge();

                    if (_objCurrentPanel != null) 
					_objCurrentPanel.SetActive(false);
				
                
				_resPanelStatus.SetActive(true);
				_objCurrentPanel = _resPanelStatus;
				//_parentScript._goods.transform.FindChild ("Btn_Gold").gameObject.SetActive (true);
				//_parentScript._goods.transform.FindChild ("Btn_Key").gameObject.SetActive (true);
				_objCurrentPanel.transform.parent.SetSiblingIndex(1);
                    _cHero.cObject.SetActive(true);
				_objPower.SetActive(true);
				break;
			case SUB_PANEL.EQUIPMENT:
                    Legion.Instance.googleAnalytics.LogEvent(new EventHitBuilder().SetEventCategory("Equipment").SetEventAction("Open").SetEventLabel("EquipmentOpen"));
				if(openedPanel == SUB_PANEL.SKILL) 
					_panelSkill.SubmitSkillSetting();
				else if(openedPanel == SUB_PANEL.STATUS)
					_panelStatus.SwitchInfoMenu();
                else if(openedPanel == SUB_PANEL.SMITH_RATING)
                    OpenUpgradeForge();

                if (_objCurrentPanel != null) 
					_objCurrentPanel.SetActive(false);
				
				_resPanelEquipment.SetActive(true);
				_objCurrentPanel = _resPanelEquipment;
				_panelEquipment.SetData(_cHero/*, this*/);
				//_parentScript._goods.transform.FindChild ("Btn_Gold").gameObject.SetActive (true);
				//_parentScript._goods.transform.FindChild ("Btn_Key").gameObject.SetActive (true);
                _objCurrentPanel.transform.parent.SetSiblingIndex(4);
				_objPower.SetActive(true);
				break;
			case SUB_PANEL.SKILL:
                    Legion.Instance.googleAnalytics.LogEvent(new EventHitBuilder().SetEventCategory("Skill").SetEventAction("Open").SetEventLabel("SkillOpen"));
				if(openedPanel == SUB_PANEL.EQUIPMENT) 
					_panelEquipment.OnClickBack();
				else if(openedPanel == SUB_PANEL.STATUS) 
					_panelStatus.SwitchInfoMenu();
                else if (openedPanel == SUB_PANEL.SMITH_RATING)
                    OpenUpgradeForge();

                if (_objCurrentPanel != null) 
					_objCurrentPanel.SetActive(false);
				
				_resPanelSkill.SetActive(true);
				_objCurrentPanel = _resPanelSkill;
				_panelSkill.SetHero(_cHero);
				//_parentScript._goods.transform.FindChild ("Btn_Gold").gameObject.SetActive (false);
				//_parentScript._goods.transform.FindChild ("Btn_Key").gameObject.SetActive (false);
                _objCurrentPanel.transform.parent.SetSiblingIndex(1);
				_objPower.SetActive(false);
				break;
			}
			
			//_objCurrentPanel.transform.SetParent( transform );
			//_objCurrentPanel.transform.localPosition = Vector3.zero;
			//_objCurrentPanel.transform.localScale = Vector3.one;
			//_objCurrentPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
			//if((SUB_PANEL)menuNum == SUB_PANEL.EQUIPMENT)_objCurrentPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, -35f);
			//_objCurrentPanel.GetComponent<RectTransform>().sizeDelta = new Vector2(0f, 0f);
			//_objCurrentPanel.transform.SetSiblingIndex(sibling);
			openedPanel = (SUB_PANEL)menuNum;	
		}
	}
    
    public void Open(POPUP_CHARACTER_INFO popup)
    {
        switch(popup)
        {
            case POPUP_CHARACTER_INFO.STATUS:
            case POPUP_CHARACTER_INFO.STATUS_USE_POTION:
            case POPUP_CHARACTER_INFO.STATUS_BUY_POINT:
            case POPUP_CHARACTER_INFO.STATUS_AUTO:
            case POPUP_CHARACTER_INFO.STATUS_RETIRE:
            OnClickMenu(1);
            break;
            
            case POPUP_CHARACTER_INFO.EQUIPMENT:
            case POPUP_CHARACTER_INFO.EQUIPMENT_AUTO:
            case POPUP_CHARACTER_INFO.EQUIPMENT_AUTO_STATUS:
            OnClickMenu(2);
            break;
            
            case POPUP_CHARACTER_INFO.SKILL:
            case POPUP_CHARACTER_INFO.SKILL_AUTO:
            case POPUP_CHARACTER_INFO.SKILL_SELECT_SLOT:
            case POPUP_CHARACTER_INFO.SKILL_BUY_POINT:
            case POPUP_CHARACTER_INFO.SKILL_ACTIVE:
            case POPUP_CHARACTER_INFO.SKILL_RESET_POINT:
            case POPUP_CHARACTER_INFO.SKILL_UPGRADE:
            OnClickMenu(3);
            break;
		case POPUP_CHARACTER_INFO.EQUIP_CREATE:
				OnClickMenu (2);
				if (Legion.Instance.equipShortCut > 0) {
					_panelEquipment.OpenSmithingPanel (Legion.Instance.equipShortCut);
					Legion.Instance.equipShortCut = 0;
				} else {
					_panelEquipment.OpenSmithingPanel (7);
				}
                break;
            case POPUP_CHARACTER_INFO.EQUIP_CREATE_UPGRADE:
                openedPanel = SUB_PANEL.SMITH_RATING;
                OnClickMenu(2);
            break;
        }
    }
	
	public void OnClickBack()
	{
        Legion.Instance.eCharState = Legion.ChangeCharInfo.NONE;
		StartCoroutine(disable());
		//Destroy(gameObject);
	}
	
	IEnumerator disable()
	{
        CheckClose();

        while(true)
        {
            if(Legion.Instance.eCharState == Legion.ChangeCharInfo.CHANGED)
            {
                Legion.Instance.eCharState = Legion.ChangeCharInfo.NONE;
                break;
            }
            else if(Legion.Instance.eCharState == Legion.ChangeCharInfo.NONE)
                yield return new WaitForEndOfFrame();
            else if(Legion.Instance.eCharState == Legion.ChangeCharInfo.ERROR)
            {
                yield break;
            }
        }

		FadeEffectMgr.Instance.FadeOut();
		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
		//        Legion.Instance.cTutorial.CheckTutorial(MENU.MAIN);
		//if(_parentScript._from == 2 || _parentScript._from == 1)
		//    _parentScript._CrewMenu.gameObject.SetActive(true);

		bool bBeforeMenuIsCrew = false;

		if (_inCrewList == 2)
			bBeforeMenuIsCrew = true;

		_parentScript.CloseCharacterInfo(bBeforeMenuIsCrew);

		CloseScene(); 

		gameObject.SetActive(false);
		PopupManager.Instance.RemovePopup(gameObject);
		FadeEffectMgr.Instance.FadeIn();
	}

    public void CheckClose()
    {
        switch(openedPanel)
		{
		case SUB_PANEL.STATUS:
            _panelStatus.OnClickBack();
			break;
		case SUB_PANEL.EQUIPMENT:
			_panelEquipment.OnClickBack();
			break;
		case SUB_PANEL.SKILL:
			_panelSkill.SubmitSkillSetting();
			break;
		}
    }

    public void CloseScene()
    {
        bClose = true;
		//		for(int i=0; i<3; i++)
		//			this.transform.parent.GetComponent<LobbyScene>()._characterPos.transform.GetChild(i)
        //_objPower.SetActive(false);
        if(_cHero.cObject != null)
            _cHero.cObject.transform.localScale = Vector3.one;
		switch(openedPanel)
		{
		case SUB_PANEL.STATUS:
			
            if(_inCrewList == 0)
            {
                if(_cHero.cObject !=null)
			        _cHero.cObject.GetComponent<HeroObject>().LoadTransform();
                //this.transform.parent.GetComponent<Toggle>().interactable = true;
				_parentScript.SetMenuHideButtonEnable(true);
            }
            else if(_inCrewList == 2)
            {
                if(_cHero.cObject != null)
                    _cHero.cObject.GetComponent<HeroObject>().LoadTransform();
                else
                    _cHero.DestroyModelObject();
                //_parentScript._CrewMenu.GetComponent<UI_CrewMenu>().InitCharacterList();
            }
            else if(_inCrewList == 1)
            {
                _parentScript._CrewMenu.SetActive(true);
                if(_cHero.cObject != null)
                    _cHero.DestroyModelObject();
            }
            else
            {
                //_parentScript._CrewMenu.GetComponent<UI_CrewMenu>().InitCharacterList();
                _cHero.DestroyModelObject();
            }
            
            //_panelStatus.OnClickBack();
			break;
		case SUB_PANEL.EQUIPMENT:
			//if(!_panelEquipment.OnClickBack())
			//{
                if(_inCrewList == 0)
                {
                    if(_cHero.cObject !=null)
				        _cHero.cObject.GetComponent<HeroObject>().LoadTransform();
                    //this.transform.parent.GetComponent<Toggle>().interactable = true;
					_parentScript.SetMenuHideButtonEnable(true);
                }
                else if(_inCrewList == 2)
                {
                    _cHero.cObject.GetComponent<HeroObject>().LoadTransform();
                }
                else if(_inCrewList == 1)
                {
                    if(_cHero.cObject != null)
                        _cHero.DestroyModelObject();
                    _parentScript._CrewMenu.SetActive(true);
                }
                else
                    _cHero.DestroyModelObject();
				
			//}
			break;
		case SUB_PANEL.SKILL:
			//_panelSkill.SubmitSkillSetting();
            
            if((_inCrewList != 1) && (_cHero.cObject !=null))
            {
                _cHero.cObject.SetActive(true);
                _cHero.cObject.GetComponent<HeroObject>().LoadTransform();
            }
            if(_inCrewList == 0)
            {
                if(_cHero.cObject !=null)
                {
			        _cHero.cObject.GetComponent<HeroObject>().LoadTransform();
                }
                //this.transform.parent.GetComponent<Toggle>().interactable = true;
				_parentScript.SetMenuHideButtonEnable(true);
            }
            /*else if(_inCrewList == 2)
            {
                _cHero.cObject.GetComponent<HeroObject>().LoadTransform();
            }*/
            else if(_inCrewList == 1)
            {
                _parentScript._CrewMenu.SetActive(true);
                //_cHero.DestroyModelObject();
                if(_cHero.cObject != null)
                    _cHero.DestroyModelObject();
                _parentScript.gameObject.transform.GetChild(0).FindChild("Pref_UI_Main_CrewMenu").GetComponent<UI_CrewMenu>().SetCrewCharacters();
            }
			//_cHero.cObject.SetActive(true);
			
			break;
		}
        if(_cHero.u1AssignedCrew != 0)
			_parentScript.OnCloseCharacterInfo(Legion.Instance.acCrews[_cHero.u1AssignedCrew-1], _cHero.u1AssignedCrew);
        else
        {
			_parentScript.OnCloseCharacterInfo(Legion.Instance.cBestCrew, _cHero.u1AssignedCrew);
			//_parentScript.transform.GetChild(0).FindChild("Pref_UI_Main_CrewMenu").GetComponent<UI_CrewMenu>().SetCrewCharacters();
        }
		//openedPanel = SUB_PANEL.STATUS;
		DisableToggle();
    }
	
	void OnEnable()
	{
		FadeEffectMgr.Instance.FadeIn();
        _parentScript._goods.SetActive(false);
		/*
		if (Legion.Instance.cTutorial.CurrentTutorial != null && Legion.Instance.cTutorial.CurrentTutorial.u1TutorialType == 1) {
			if(Legion.Instance.cTutorial.CheckTutorial(MENU.CHARACTER_INFO, (UInt16)POPUP_CHARACTER_INFO.EQUIPMENT))
			{
				DebugMgr.Log("Tutorial Equip");
			}
		}
		else if(_cHero.GetComponent<StatusComponent>().StatPoint > 0 && _cHero.GetComponent<SkillComponent>().SkillPoint > 0 && Legion.Instance.cTutorial.au1Step[1] != 200)
		{
			if(Legion.Instance.cTutorial.CheckTutorial(MENU.CHARACTER_INFO, (UInt16)POPUP_CHARACTER_INFO.STATUS)){

			}
		}
		*/
	}
	
    public void SetToggleBtn()
    {
        //_cHero.GetComponent<StatusComponent>().CountingStatPoint(_cHero.cLevel.u2Level);
        //if(_cHero.GetComponent<StatusComponent>().StatPoint > 0)
        if(_resPanelStatus.GetComponent<UI_Panel_CharacterInfo_Status>().GetStatPoint() > 0)
        {
            acToggleBtn[0].transform.GetChild(3).gameObject.SetActive(true);
        }
        else
            acToggleBtn[0].transform.GetChild(3).gameObject.SetActive(false);

        if(_cHero.GetComponent<SkillComponent>().GetTotalPoint() > 0)
        {
            acToggleBtn[2].transform.GetChild(3).gameObject.SetActive(true);
        }
        else
            acToggleBtn[2].transform.GetChild(3).gameObject.SetActive(false);

        Legion.Instance.cInventory.AllInvenSort();
        List<EquipmentItem> ret = new List<EquipmentItem>();
        ret = Legion.Instance.cInventory.GetNewEquip();
        bool equipNew = false;
        for(int i=0; i<_cHero.acEquips.Length; i++)
        {
            for(int j=0; j<ret.Count; j++)
            {
                if(ret[j].GetEquipmentInfo().u2ClassID != _cHero.acEquips[i].GetEquipmentInfo().u2ClassID)
                    continue;
                if(ret[j].attached.hero != null)
                    continue;
                if(ret[j].GetEquipmentInfo().u1PosID == _cHero.acEquips[i].GetEquipmentInfo().u1PosID && ret[j].isNew)
                {
                    equipNew = true;
                    break;
                }
            }
            //if(_cHero.acEquips[i].GetComponent<StatusComponent>().CheckHaveEquipStatPoint(_cHero.acEquips[i].cLevel.u2Level))
            //{
            //    acToggleBtn[1].transform.GetChild(3).gameObject.SetActive(true);
            //    break;
            //}
            /*else*/ if(equipNew)
            {
                acToggleBtn[1].transform.GetChild(3).gameObject.SetActive(true);
                break;
            }
            else
                acToggleBtn[1].transform.GetChild(3).gameObject.SetActive(false);
        }
        AlramSmithing.SetActive(CheckMaterial());
    }
    public void SetSmithingAlram(bool bCheck)
    {
        AlramSmithing.SetActive(bCheck);
    }
    bool CheckMaterial()
	{
		bool materialCountCheck=false;
        ForgeInfo _cNextForgeInfo;
        if(Legion.Instance.u1ForgeLevel < ForgeInfo.FORGE_LEVEL_MAX-5)
            _cNextForgeInfo = ForgeInfoMgr.Instance.GetList()[Legion.Instance.u1ForgeLevel];
        else
            _cNextForgeInfo = ForgeInfoMgr.Instance.GetList()[Legion.Instance.u1ForgeLevel-1];
		for(int i=0; i<_cNextForgeInfo.cUpgradeInfo.acUpgradeMaterials.Length; i++)
		{
			UInt16 ownCount = 0;
			Item item = null;
			UInt16 invenSlotNum = 0;
			if(Legion.Instance.cInventory.dicItemKey.TryGetValue(_cNextForgeInfo.cUpgradeInfo.acUpgradeMaterials[i].u2ID, out invenSlotNum))
			{
				if(Legion.Instance.cInventory.dicInventory.TryGetValue(invenSlotNum, out item))
				{
					ownCount = ((MaterialItem)item).u2Count;
				}
			}
			else
			{
				materialCountCheck = false;
				break;
			}
			
			if(ownCount >= _cNextForgeInfo.cUpgradeInfo.acUpgradeMaterials[i].u4Count)
			{
				materialCountCheck = true;
			}
			else
			{
				materialCountCheck = false;
				break;
			}
		}
		
		return materialCountCheck;
	}
	public void OnClickMenuHideBtn()
	{
		if(m_isMenuHide == false)
        {
			_MenuAniContorller.Play("Ani_UI_CharacterInfo_HideMenu");
            LeanTween.cancel(_parentScript._characterViewCam.gameObject);
            LeanTween.value(_parentScript._characterViewCam.gameObject, 46f, 31f, 0.15f).setOnUpdate((float fov)=> {_parentScript._characterViewCam.fieldOfView = fov; });
        }
		else
        {
			_MenuAniContorller.Play("Ani_UI_CharacterInfo_ShowMenu");
            LeanTween.cancel(_parentScript._characterViewCam.gameObject);
            LeanTween.value(_parentScript._characterViewCam.gameObject, 31f, 46f, 0.15f).setOnUpdate((float fov)=> {_parentScript._characterViewCam.fieldOfView = fov; });
        }

		m_isMenuHide = !m_isMenuHide;
	}
    int elementPage = 0;
    public void OnClickBtnElement()
    {
        ElementInfo.interactable = true;
        ElementDialog[0].SetActive(true);
        ElementDialog[1].SetActive(false);
    }
    public void OnClickElementInfo()
    {
        switch(elementPage)
        {
            case 0:
                ElementDialog[0].SetActive(false);
                ElementDialog[1].SetActive(true);
                elementPage = 1;
                break;

            case 1:
                ElementDialog[0].SetActive(false);
                ElementDialog[1].SetActive(false);
                elementPage = 0;
                ElementInfo.interactable = false;
                break;
        }
    }
    int comboPage = 0;
    public void OnClickBtnCombo()
    {
        ComboInfo.interactable = true;
        ComboDialog[0].SetActive(true);
        ComboDialog[1].SetActive(false);
        ComboDialog[2].SetActive(false);
        ComboDialog[3].SetActive(false);
        ComboDialog[4].SetActive(false);
    }
    public void OnClickComboInfo()
    {
        switch(comboPage)
        {
            case 0:
                ComboDialog[0].SetActive(false);
                ComboDialog[1].SetActive(true);
                ComboDialog[2].SetActive(false);
                ComboDialog[3].SetActive(false);
                ComboDialog[4].SetActive(false);
                comboPage++;
                break;

            case 1:
                ComboDialog[0].SetActive(false);
                ComboDialog[1].SetActive(false);
                ComboDialog[2].SetActive(true);
                ComboDialog[3].SetActive(false);
                ComboDialog[4].SetActive(false);
                comboPage++;
                break;

            case 2:
                ComboDialog[0].SetActive(false);
                ComboDialog[1].SetActive(false);
                ComboDialog[2].SetActive(false);
                ComboDialog[3].SetActive(true);
                ComboDialog[4].SetActive(false);
                comboPage++;
                break;

            case 3:
                ComboDialog[0].SetActive(false);
                ComboDialog[1].SetActive(false);
                ComboDialog[2].SetActive(false);
                ComboDialog[3].SetActive(false);
                ComboDialog[4].SetActive(true);
                comboPage++;
                break;

            case 4:
                ComboDialog[0].SetActive(false);
                ComboDialog[1].SetActive(false);
                ComboDialog[2].SetActive(false);
                ComboDialog[3].SetActive(false);
                ComboDialog[4].SetActive(false);
                comboPage = 0;
                ComboInfo.interactable = false;
                break;
        }
    }
}
