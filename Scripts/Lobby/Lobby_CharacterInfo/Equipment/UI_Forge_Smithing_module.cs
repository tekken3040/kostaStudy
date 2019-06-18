using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class UI_Forge_Smithing_module : MonoBehaviour
{
	[SerializeField] GameObject _objNotOwnText;
	[SerializeField] GameObject _objPrevButton;
	[SerializeField] GameObject _objNextButton;

	[SerializeField] Text _txtEquipCount;

    // 2017. 01. 18 jy
    // 이펙트를 위하여 2개를 컨트롤 하는것보다 부모 하나만 컨트롤 하도록 함
	//[SerializeField] GameObject _objNewMsg;
	//[SerializeField] GameObject _objEffCamera;
    public GameObject _objNewMsgEffect;
    public GameObject _imgNewProduct;

	[SerializeField] GameObject _panelSmithingDetail;
	[SerializeField] UI_Panel_Forge_Smithing_Detail _scriptSmithingDetail;

	[SerializeField] GameObject _panelSmithingResult;
	public UI_Panel_Forge_Smithing_Result _scriptSmithingResult;
    private GameObject _PrefPanelSmithingResultEffect;
    private GameObject _panelSmithingResultEffect;
	private UI_Forge_Smithing_Result_Effect _scriptSmithingResultEffect;
    UI_SubPanel_CharacterInfo_Equipment_ItemList _cParent;
	public List<UInt16> checkIDs; // 서버에 New를 확인했다고 보낼 장비ID값을 저장

	UInt16 _u2SelectedClassID = 1;
	Byte _u1SelectedPosID = 7;
	//int _nSelectedPosIdx = 0;
	int _u1SelectedEquipIdx = 0; // 장비목록 중 현재 선택된 장비의 인덱스(좌, 우 버튼 누르면 감소, 증가)

	bool init=false;

	public void SetData(EquipmentInfo.EQUIPMENT_POS _pos, Byte _classID, UI_SubPanel_CharacterInfo_Equipment_ItemList _parent)
	{
		checkIDs = new List<UInt16>();
        _PrefPanelSmithingResultEffect = AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Equipment/Pref_UI_Panel_Smithing_Result_Effect.prefab", typeof(GameObject)) as GameObject;
        _u2SelectedClassID = _classID;
		_u1SelectedPosID = (Byte)_pos;
		_u1SelectedEquipIdx = 0;
        _cParent = _parent;
		// EquipDesign(장비도안)의 0번 인덱스는 "신규 장비 도안 획득"메세지를 봤는지 안봤는지 여부
		if(Legion.Instance.acEquipDesignNew.Get(0))
		{
            Server.ServerMgr.Instance.CheckDesign(1, 0, new UInt16[1]{(UInt16)Server.ConstDef.BaseEquipDesignID}, AckCheckDesignEff);
			StartCoroutine(CheckNewAni());
		}

        InitList();
		_txtEquipCount.text = (_u1SelectedEquipIdx+1) + " / " + lstEquipInfo.Count;
		InitDetailScreen();
		init = true;
	}

	void OnDisable()
	{
		// "신규 장비 도안 획득" 메세지 연출이 진행중일때 다른탭으로 넘어가면 바로 메세지 Off
        /*
        if(_objNewMsg != null)
        {
            if(_objNewMsg.transform.GetChild(0) != null)
                _objNewMsg.transform.GetChild(0).gameObject.SetActive(false);
            _objNewMsg.SetActive(false);
        }
		if (_objEffCamera != null) {
			if (_objEffCamera.activeSelf)
				_objEffCamera.SetActive (false);
		}
        */
        if(_objNewMsgEffect != null)
        {
            _objNewMsgEffect.SetActive(false);
        }
	}
	
	IEnumerator CheckNewAni()
	{
		//_objEffCamera.SetActive(true);
		//_objNewMsg.SetActive(false);
        //
		//_objNewMsg.SetActive(true);
		//_objNewMsg.transform.GetChild(0).gameObject.SetActive(true);
        if (_objNewMsgEffect != null)
        {
            _objNewMsgEffect.SetActive(true);
        }
        yield return new WaitForSeconds(1.9f);
        if (_objNewMsgEffect != null)
        {
            _objNewMsgEffect.SetActive(false);
        }
        //_objNewMsg.SetActive(false);
        //_objNewMsg.transform.GetChild(0).gameObject.SetActive(false);
        //_objEffCamera.SetActive(false);
    }
    public bool bAttach = false;
	public void OnClickClose()
    {
        PopupManager.Instance.RemovePopup(this.gameObject);
        _cParent.InitItemList_All();
        if(bAttach)
            _cParent.OnClickClose();
        Destroy(this.gameObject);
    }

	public void OnClickPrevEquip()
	{
		if(_u1SelectedEquipIdx == 0)
			_u1SelectedEquipIdx = (lstEquipInfo.Count-1);
		else
			_u1SelectedEquipIdx--;
		_txtEquipCount.text = (_u1SelectedEquipIdx+1) + " / " + lstEquipInfo.Count;
		UInt16 equipID = lstEquipInfo[_u1SelectedEquipIdx].u2ID;
		_scriptSmithingDetail.SetData(EquipmentInfoMgr.Instance.GetInfo(Convert.ToUInt16(equipID)), null, this);
	}

	public void OnClickNextEquip()
	{
		if(_u1SelectedEquipIdx == (lstEquipInfo.Count-1))
			_u1SelectedEquipIdx = 0;
		else
			_u1SelectedEquipIdx++;
		_txtEquipCount.text = (_u1SelectedEquipIdx+1) + " / " + lstEquipInfo.Count;
		UInt16 equipID = lstEquipInfo[_u1SelectedEquipIdx].u2ID;
		_scriptSmithingDetail.SetData(EquipmentInfoMgr.Instance.GetInfo(Convert.ToUInt16(equipID)), null, this);
	}

	List<EquipmentInfo> lstEquipInfo;
	void InitDetailScreen()
	{
		if(lstEquipInfo.Count <= 1)
		{
			_objNextButton.SetActive(false);
			_objPrevButton.SetActive(false);
		}
		else
		{
			_objNextButton.SetActive(true);
			_objPrevButton.SetActive(true);
		}

		if(lstEquipInfo.Count < 1)
		{
			_objNotOwnText.SetActive(true);
			_panelSmithingDetail.SetActive(false);
		}
		else
		{
			_objNotOwnText.SetActive(false);
			UInt16 equipID = lstEquipInfo[_u1SelectedEquipIdx].u2ID;

			// _scriptSmithingDetail.SetData 함수의 2번째 인자(ForgeInfo)로 null이 들어가면 현 상태 유지, 지정해주면 지정된 값으로 제작 장비 정보 설정
			// 처음 불러올때는 업그레이드를 마친 최고등급으로 설정.
			// 처음이 아닌 클래스나(좌측 리스트), 장착부위(하단 리스트) 변경시에는 유저가 변경한 값으로 유지
			ForgeInfo initForgeInfo = null;
			if(_scriptSmithingDetail._cForgeInfo == null)
			{
				initForgeInfo = ForgeInfoMgr.Instance.GetList()[(Legion.Instance.u1ForgeLevel-1)];
			}
            //PopupManager.Instance.AddPopup(_scriptSmithingDetail.gameObject, OnClickClose);

			//_scriptSmithingDetail.SetData(EquipmentInfoMgr.Instance.GetInfo(Convert.ToUInt16(equipID)),  initForgeInfo, this);
			_scriptSmithingDetail.SetData(EquipmentInfoMgr.Instance.GetInfo(Convert.ToUInt16(equipID)),  null, this);
			_panelSmithingDetail.SetActive(true);
		}
	}

    //모든 장비의 List 생성
    List<EquipmentInfo> lstAllEquipInfo;
    public void InitAllList()
    {
        lstAllEquipInfo = new List<EquipmentInfo>();
        lstAllEquipInfo = EquipmentInfoMgr.Instance.GetListAllDesign();
        for(int i=0; i<lstAllEquipInfo.Count; i++)
        {
            if(Legion.Instance.acEquipDesignNew.Get(lstAllEquipInfo[i].u2ID - Server.ConstDef.BaseEquipDesignID))
            {
                Legion.Instance.acEquipDesignNew.Set(lstAllEquipInfo[i].u2ID - Server.ConstDef.BaseEquipDesignID, false);
                if(!checkIDs.Contains(lstAllEquipInfo[i].u2ID))
                    checkIDs.Add(lstAllEquipInfo[i].u2ID);
            }
        }
    }
	// 클래스와 장착위치, 장비도안이 있는 조건을 판단하여 장비 List 생성
	void InitList()
	{
		if(lstEquipInfo == null)
			lstEquipInfo = new List<EquipmentInfo>();
		/*
		// 선택한 장착위치가 악세서리1 또는 악세서리2일경우 리스트를 합침
		//if(_u1SelectedPosID == (Byte)EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 || _u1SelectedPosID == (Byte)EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
		{
			lstEquipInfo = EquipmentInfoMgr.Instance.GetListOwnDesign(ClassInfo.COMMON_CLASS_ID, (Byte)EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1);
			lstEquipInfo.AddRange(EquipmentInfoMgr.Instance.GetListOwnDesign(ClassInfo.COMMON_CLASS_ID, (Byte)EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2));
		}
		*/
		// 2016. 11. 28 jy
		// 퍼플리셔 요청사항 악세사리 구분 하게 해달라고함
		if(_u1SelectedPosID == (Byte)EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 || _u1SelectedPosID == (Byte)EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
		{
			lstEquipInfo = EquipmentInfoMgr.Instance.GetListOwnDesign(ClassInfo.COMMON_CLASS_ID, _u1SelectedPosID);
		}
		else
		{
			lstEquipInfo = EquipmentInfoMgr.Instance.GetListOwnDesign(_u2SelectedClassID, _u1SelectedPosID);
		}
	}

	public void ShowSmithingResult(Byte forgeLevel, UInt16 lastSlotNum)
	{
		EquipmentItem equipItem = (EquipmentItem)Legion.Instance.cInventory.dicInventory[lastSlotNum];
		_scriptSmithingResult.SetData(equipItem, forgeLevel);
		//_panelSmithingResult.SetActive(true);
        GameObject EffectPanel = Instantiate(_PrefPanelSmithingResultEffect);
		RectTransform rectTr = EffectPanel.GetComponent<RectTransform>();
		rectTr.SetParent(this.transform);
		rectTr.localPosition = Vector3.zero;//new Vector3(0, 0, -400);
		rectTr.localScale = Vector3.one;
		rectTr.sizeDelta = Vector2.zero;
        _scriptSmithingResultEffect = EffectPanel.GetComponent<UI_Forge_Smithing_Result_Effect>();
        _panelSmithingResultEffect = EffectPanel;
        _scriptSmithingResultEffect.SetData(equipItem, forgeLevel, this);
        _panelSmithingResultEffect.SetActive(true);
	}

	public void RequestCheckDesign()
	{
		if(checkIDs.Count != 0)
		{
			PopupManager.Instance.ShowLoadingPopup(1);
            Server.ServerMgr.Instance.CheckDesign(1, 0, checkIDs.ToArray(), AckCheckDesign);
		}
	}

	public void AckCheckDesignEff(Server.ERROR_ID err)
	{
		if(err != Server.ERROR_ID.NONE)
		{
			DebugMgr.Log("CheckDesignError");
			PopupManager.Instance.CloseLoadingPopup();
		}
		else
		{
			Legion.Instance.acEquipDesignNew.Set(0, false);
		}
	}

	public void AckCheckDesign(Server.ERROR_ID err)
	{
		if(err != Server.ERROR_ID.NONE)
		{
			DebugMgr.Log("CheckDesignError");
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.FORGE_CHECK_DESIGN, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
            PopupManager.Instance.CloseLoadingPopup();
			for(int i=0; i<checkIDs.Count; i++)
			{
				Legion.Instance.acEquipDesignNew.Set(checkIDs[i] - Server.ConstDef.BaseEquipDesignID, false);
			}
			checkIDs.Clear();
		}
	}

	public void CloseResult()
	{
		_scriptSmithingResult.OnClickConfirm();
		Legion.Instance.cQuest.CheckEndDirection (AchievementTypeData.MakeEquip);
	}
}
