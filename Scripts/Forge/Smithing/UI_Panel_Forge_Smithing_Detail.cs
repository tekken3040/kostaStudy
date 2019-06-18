using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;

public class UI_Panel_Forge_Smithing_Detail : MonoBehaviour {
	[SerializeField] GameObject _panel;
	[SerializeField] Image _imgSmithingLevel;
	[SerializeField] Text _txtElement;
    [SerializeField] Image _imgClassIcon;
	[SerializeField] Text[] _txtStatType;
	//[SerializeField] Text[] _txtStatValueMin;
	[SerializeField] Text[] _txtStatValueMax;
	[SerializeField] UI_Button_CharacterInfo_Equipment_StatInfo[] _btnStatInfo;
	[SerializeField] Text[] _txtSkillName;
	[SerializeField] RectTransform _trMaterialListParent;
	[SerializeField] RectTransform _trNameGroup;
	[SerializeField] Text _txtTier;
	[SerializeField] Text _txtEquipName;
	[SerializeField] Text _txtEquipNameInConfirmPopup;
	[SerializeField] Image _imgGoodsIcon;
	[SerializeField] Text _txtGoodsCount;
	public GameObject _objAlramIcon;

	[SerializeField] Transform _trEquipModelParent;
	[SerializeField] SpriteRenderer _imgAccessory;
	[SerializeField] Image _imgAccessoryUI;
	[SerializeField] Transform _trEquipEffectParent;
	public GameObject _objEquipModel;
	GameObject _objEquipEffect;

	public GameObject _objSmithingEffect;
	public GameObject _objResultPopupEffect;

	[SerializeField] GameObject _subPanelConfirm;

	[SerializeField] GameObject _subPanelSelectLevel;
	[SerializeField] UI_SubPanel_Forge_Smithing_SelectLevel _scriptSelectLevel;

	[SerializeField] GameObject _subPanelSelectSkills;
	[SerializeField] UI_SubPanel_Forge_Smithing_SelectSkill _scriptSelectSkill;

	[SerializeField] GameObject _subPanelMaterialShortCut;
	[SerializeField] UI_SubPanel_Forge_Smithing_MaterialShortcut _scriptMaterialShortcut;

    [SerializeField] Text txt_Btn_Craft;

    public UI_Forge_Smithing_module _cParent2;

	public EquipmentInfo _cEquipInfo;
	public ForgeInfo _cForgeInfo;
	public EquipmentInfo.SmithingMaterial _cSmithingMaterialInfo;
	public Byte _u1SkillCount = 5;
	public Byte[] _au1SelectedSkill;
	public Byte _u1SelectedSkillCount;

    private bool bReforged = false;

    public bool CheckReforged
    {
        get{ return bReforged; }
        set{ bReforged = value; }
    }

    [SerializeField] UI_Button_CharacterInfo_Equipment_StateInfo _specializeBtn;
    [SerializeField] Text _equipmentClassName;
    [SerializeField] Text txtHaveCount;

	public GameObject disObj;
	public DiscountUI disScript;

    public void SetData(EquipmentInfo equipInfo, ForgeInfo forgeInfo, UI_Forge_Smithing_module parent)
	{
		if(_objEquipModel != null)
			DestroyObject(_objEquipModel);

		_cParent2 = parent;
		_cEquipInfo = equipInfo;
        if(Legion.Instance.acEquipDesignMake.Get(_cEquipInfo.u2ID - Server.ConstDef.BaseEquipDesignID))
            _cParent2._imgNewProduct.SetActive(true);
        else
            _cParent2._imgNewProduct.SetActive(false);
		
		SetSmithingDetail(forgeInfo);
	}

	private void SetSmithingDetail(ForgeInfo forgeInfo)
	{
        StringBuilder tempString = new StringBuilder();
        // 제작등급 정보가 null로 유지되어있을경우 에러를 방지하기위해 초기화 시켜줌
        if (forgeInfo != null)
		{
			_cForgeInfo = forgeInfo;
			ObscuredPrefs.SetInt("SelectedForgeLevel", _cForgeInfo.u1Level-1);
		}
		else
		{
			if(_cForgeInfo == null)
			{
				// 2016. 11. 23 jy
				// 선택되어 있는 티어 레벨 불러오기
				int SelectedForgeLevel = ObscuredPrefs.GetInt("SelectedForgeLevel", 0);
				// pc 테스트 중 ID 변경하여 로그인시 이전 LeveL 셋팅된 값이 로드 되어 예외 처리
				if(SelectedForgeLevel >= Legion.Instance.u1ForgeLevel)
				{
					SelectedForgeLevel = 0;
					ObscuredPrefs.SetInt("SelectedForgeLevel", SelectedForgeLevel);
				}
				_cForgeInfo = ForgeInfoMgr.Instance.GetList()[SelectedForgeLevel];
			}
			forgeInfo = _cForgeInfo;
		}

		_cSmithingMaterialInfo = _cEquipInfo.dicSmithingMaterial[forgeInfo.u1Level];
		_au1SelectedSkill = new byte[_u1SkillCount];

        // 악세사리에서는 스킬이 없다
        if (_cEquipInfo.u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 && _cEquipInfo.u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
            _u1SkillCount = (Byte)(forgeInfo.cSmithingInfo.u1RandomSkillCount + forgeInfo.cSmithingInfo.u1SelectSkillCount);
        else
            _u1SkillCount = 0;

        tempString.Remove(0, tempString.Length);
        _imgSmithingLevel.sprite = AtlasMgr.Instance.GetSprite(tempString.Append("Sprites/Forge/Forge_grade_01.tier_icon_").Append(forgeInfo.u1Level).ToString());
		_imgSmithingLevel.SetNativeSize();

        //_txtTier.text = "<" + TextManager.Instance.GetText("forge_level_" + forgeInfo.u1Level) + ">";
        tempString.Remove(0, tempString.Length);
        _txtTier.text = TextManager.Instance.GetText(tempString.Append("forge_level_").Append(forgeInfo.u1Level).ToString());
		UIManager.Instance.SetGradientFromTier( _txtTier.GetComponent<Gradient>(), forgeInfo.u1Level);

		_txtEquipName.text =  TextManager.Instance.GetText( _cEquipInfo.sName );
		_txtEquipName.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
		UIManager.Instance.SetGradientFromElement( _txtEquipName.GetComponent<Gradient>(), _cEquipInfo.u1Element );
		UIManager.Instance.SetSizeTextGroup(_trNameGroup, 18);

        tempString.Remove(0, tempString.Length);
        _txtElement.text = TextManager.Instance.GetText(tempString.Append("element_").Append(_cEquipInfo.u1Element).ToString());
		UIManager.Instance.SetGradientFromElement(	_txtElement.GetComponent<Gradient>(), _cEquipInfo.u1Element );

		_specializeBtn.SetData((Byte)(_cEquipInfo.u1Specialize+2));
		Color tempColor;
		ColorUtility.TryParseHtmlString(_cEquipInfo.GetHexColor((Byte)(_cEquipInfo.u1Specialize+2)), out tempColor);
		_specializeBtn.transform.parent.GetComponent<Image>().color = tempColor;
		if(_cEquipInfo.u2ClassID <= ClassInfo.LAST_CLASS_ID)
		{
            tempString.Remove(0, tempString.Length);
            _equipmentClassName.text = TextManager.Instance.GetText(ClassInfoMgr.Instance.GetInfo(_cEquipInfo.u2ClassID).sName);
			_imgClassIcon.sprite = AtlasMgr.Instance.GetSprite(tempString.Append("Sprites/Common/common_class.common_class_").Append(_cEquipInfo.u2ClassID).ToString());
			_imgClassIcon.enabled = true;
			_imgClassIcon.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
			_imgClassIcon.SetNativeSize();
		}
		else
		{
			_equipmentClassName.text = TextManager.Instance.GetText("equip_common");
			_imgClassIcon.enabled = false;
		}

		for(Byte i=0; i<EquipmentInfo.ADD_STAT_TYPE_MAX; i++)
		{
            _txtStatType[i].text = Status.GetStatText(_cEquipInfo.acStatAddInfo[i].u1StatType);
            tempString.Remove(0, tempString.Length);
            tempString.Append((_cEquipInfo.acStatAddInfo[i].u2BaseStatMin + (_cEquipInfo.acStatAddInfo[i].u2AddStatMinForgeLevel * (forgeInfo.u4StatMultiplePerSmithingLevel))).ToString());
            tempString.Append(" ~ ");
            tempString.Append((_cEquipInfo.acStatAddInfo[i].u2BaseStatMax + (_cEquipInfo.acStatAddInfo[i].u2AddStatMaxForgeLevel * (forgeInfo.u4StatMultiplePerSmithingLevel))).ToString());
            //_txtStatValueMin[i].text = (_cEquipInfo.acStatAddInfo[i].u2BaseStatMin + (_cEquipInfo.acStatAddInfo[i].u2AddStatMinForgeLevel * (forgeInfo.u4StatMultiplePerSmithingLevel))).ToString(); //* forgeInfo.cSmithingInfo
            //_txtStatValueMax[i].text = (_cEquipInfo.acStatAddInfo[i].u2BaseStatMax + ( _cEquipInfo.acStatAddInfo[i].u2AddStatMaxForgeLevel * (forgeInfo.u4StatMultiplePerSmithingLevel) )).ToString();
            _txtStatValueMax[i].text = tempString.ToString();
            _btnStatInfo[i].SetData(_cEquipInfo.acStatAddInfo[i].u1StatType);
		}

		for(Byte i=0; i<forgeInfo.cSmithingInfo.u1RandomSkillCount; i++)
		{
			_txtSkillName[i].gameObject.SetActive(true);
			_txtSkillName[i].text = TextManager.Instance.GetText("random_skill");
			_txtSkillName[i].color = Color.white;
		}

		for(Byte i=forgeInfo.cSmithingInfo.u1RandomSkillCount; i<_u1SkillCount; i++)
		{
			_txtSkillName[i].gameObject.SetActive(true);
			_txtSkillName[i].text = TextManager.Instance.GetText("select_skill");
			_txtSkillName[i].color = Color.white;
		}

		for(Byte i=_u1SkillCount; i<EquipmentInfo.ADD_SKILL_MAX; i++)
		{
			_txtSkillName[i].gameObject.SetActive(true);
			_txtSkillName[i].text = TextManager.Instance.GetText("none_skill");
			_txtSkillName[i].color = UI_ListElement_Forge_Smithing_Material.lowerCount;
		}

		// 악세서리가 아니면 3D모델을 보여주고, 악세서리면 2D Sprite로 보여줌
		if(_cEquipInfo.u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 && _cEquipInfo.u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
		{
			EquipmentItem tmpEquipItem = new EquipmentItem(_cEquipInfo.u2ID);
			tmpEquipItem.InitViewModelObject();
			tmpEquipItem.cObject.GetComponent<EquipmentObject>().SetLayer( LayerMask.NameToLayer("UI") );
			_objEquipModel = tmpEquipItem.cObject;
			tmpEquipItem.cObject.transform.SetParent( _trEquipModelParent );
			tmpEquipItem.cObject.transform.localScale = Vector3.one;
			tmpEquipItem.cObject.transform.localPosition = new Vector3(0f, -150f, 0f);
			_imgAccessory.gameObject.SetActive(false);
			_trEquipModelParent.gameObject.SetActive(true);
			_trEquipModelParent.GetComponent<RotateCharacter>().characterTransform = _objEquipModel.transform;
		}
		else
		{
            tempString.Remove(0, tempString.Length);
            _imgAccessory.sprite = AssetMgr.Instance.AssetLoad(tempString.Append("Sprites/Item/Accessory/acc_").Append(_cEquipInfo.u2ModelID).Append(".png").ToString(), typeof(Sprite)) as Sprite;
			_imgAccessory.gameObject.SetActive(true);
			_imgAccessoryUI.gameObject.SetActive(false);
		}

		if(_objEquipEffect != null) DestroyObject(_objEquipEffect);
		if(forgeInfo.u1Level >= 1)
		{
			DebugMgr.Log("Asset : " +string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}", forgeInfo.u1Level));
			_objEquipEffect = Instantiate(AssetMgr.Instance.AssetLoad(string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}.prefab", forgeInfo.u1Level), typeof(GameObject))) as GameObject;
			_objEquipEffect.transform.SetParent(_trEquipEffectParent);
			_objEquipEffect.transform.name = "WeaponEffect";
			_objEquipEffect.transform.localScale = Vector3.one;
			_objEquipEffect.transform.localPosition = Vector3.zero;
		}

		// 제작 등급 선택창 초기화
		_scriptSelectLevel.SetData(_cForgeInfo, this);

        // 확인창(재료) 초기화
        tempString.Remove(0, tempString.Length);
        tempString.Append(TextManager.Instance.GetText("popup_title_product_equip")).Append("    <").Append(TextManager.Instance.GetText(_cEquipInfo.sName)).Append(">");
        _txtEquipNameInConfirmPopup.text = tempString.ToString();
		InitMaterialList();

        EventDisCountinfo info = EventInfoMgr.Instance.GetDiscountEventinfo(DISCOUNT_ITEM.SMITH);

        if (info != null)
        {
			disObj.SetActive (true);
			disScript.SetData (_cSmithingMaterialInfo.cCreateGoods.u4Count, info.u1DiscountRate);
            uint discountGold = (uint)(_cSmithingMaterialInfo.cCreateGoods.u4Count * info.discountRate);
            _txtGoodsCount.text = discountGold.ToString();
        }

        else
        {
			disObj.SetActive (false);
            _txtGoodsCount.text = (_cSmithingMaterialInfo.cCreateGoods.u4Count).ToString();
        }

		CheckHaveCount();
		CheckedMaterials();
		CheckNew();
	}

    //자신이 보유중인 같은 종류 장비의 갯수
    public void CheckHaveCount()
    {
        int tempHaveCnt = 0;
        Legion.Instance.cInventory.EquipSort();
        for(int i=0; i<Legion.Instance.cInventory.lstSortedEquipment.Count; i++)
        {
            if((_cEquipInfo.u2ID == Legion.Instance.cInventory.lstSortedEquipment[i].cItemInfo.u2ID) && (_cForgeInfo.u1Level == Legion.Instance.cInventory.lstSortedEquipment[i].u1SmithingLevel))
                tempHaveCnt++;
        }
        txtHaveCount.text = tempHaveCnt.ToString();
    }

	// 도안을 얻고 처음 보는 장비면 제작버튼에 New알림
	void CheckNew()
	{
		if(Legion.Instance.acEquipDesignNew.Get(_cEquipInfo.u2ID - Server.ConstDef.BaseEquipDesignID))
		{
            _cParent2.checkIDs.Add(_cEquipInfo.u2ID);
			Legion.Instance.acEquipDesignNew.Set((int)(_cEquipInfo.u2ID - Server.ConstDef.BaseEquipDesignID), false);
			//_objAlramIcon.SetActive(true);
		}
		else
		{
			//_objAlramIcon.SetActive(false);
		}
	}

	// 제작 재료 정보 및 보유 개수 표시(제작버튼 눌렀을시 나오는 팝업
	public void InitMaterialList()
	{
		for(int i=0; i<_trMaterialListParent.childCount; i++)
		{
			DestroyObject(_trMaterialListParent.GetChild(i).gameObject);
		}

		EquipmentInfo.SmithingMaterial smithingMaterialInfo = _cEquipInfo.dicSmithingMaterial[_cForgeInfo.u1Level];

		int makeIdx=0;
		for(makeIdx=0; makeIdx<EquipmentInfo.SmithingMaterial.MAX_MATERIAL && smithingMaterialInfo.acMaterial[makeIdx].u1Type != 0; makeIdx++)
		{
			GameObject listElement = Instantiate( AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Equipment/Pref_UI_ListElement_Forge_Smithing_Material.prefab", typeof(GameObject)) ) as GameObject;
			RectTransform trListElement = listElement.GetComponent<RectTransform>();
			trListElement.SetParent(_trMaterialListParent);
			trListElement.localScale = Vector3.one;
			trListElement.localPosition = Vector3.zero;
			
			UInt16 ownCount = 0;
			Item item = null;
			UInt16 invenSlotNum = 0;
			if(Legion.Instance.cInventory.dicItemKey.TryGetValue(smithingMaterialInfo.acMaterial[makeIdx].u2ID, out invenSlotNum))
			{
				if(Legion.Instance.cInventory.dicInventory.TryGetValue(invenSlotNum, out item))
				{
					ownCount = ((MaterialItem)item).u2Count;
				}
			}
            UI_ListElement_Forge_Smithing_Material smithingMaterial = listElement.GetComponent<UI_ListElement_Forge_Smithing_Material>();
            smithingMaterial.SetDataSmithing(smithingMaterialInfo.acMaterial[makeIdx], ownCount);
            Goods materialInfo = new Goods(smithingMaterialInfo.acMaterial[makeIdx].u1Type, smithingMaterialInfo.acMaterial[makeIdx].u2ID, smithingMaterialInfo.acMaterial[makeIdx].u4Count);
            // 필요한 재료 갯수보다 많이 가지고 있다면 필요갯수를 0으로 만든다
            if (materialInfo.u4Count <= ownCount)
                materialInfo.u4Count = 1;

            smithingMaterial.BtnDrop.onClick.AddListener( () => OnClick_MaterialListElement(materialInfo) );
		}
	}

	// 제작등급 변경시 장비정보 갱신
	public void ChangeLevel(Byte level)
	{
        ShowModelObject(true);

        SetData(_cEquipInfo, ForgeInfoMgr.Instance.GetList()[(level-1)], _cParent2);
	}

	// 우측상단 제작등급 버튼 눌렀을 시 제작등급 변경 팝업
	public void Show_SelectSmithingLevel()
	{
		ShowModelObject(false);
		_subPanelSelectLevel.SetActive(true);
		PopupManager.Instance.AddPopup(_subPanelSelectLevel, Close_SelectSmithingLevel);
	}

	// 장비제작 버튼 눌렀을 시 뜨는 팝업(재료정보)
	public void Show_ConfirmPopup()
	{
		ShowModelObject(false);
		_subPanelConfirm.SetActive(true);
		PopupManager.Instance.AddPopup(_subPanelConfirm, Close_Confirm);
	}

    public void CheckedMaterials()
    {
        bool materialCountCheck=false;
		if (!Legion.Instance.cTutorial.bIng)
        {
			Legion.Instance.CheckEmptyInven ();

			for (int i = 0; i < EquipmentInfo.SmithingMaterial.MAX_MATERIAL && _cSmithingMaterialInfo.acMaterial [i].u1Type != 0; i++)
            {
				UInt16 ownCount = 0;
				Item item = null;
				UInt16 invenSlotNum = 0;
				if (Legion.Instance.cInventory.dicItemKey.TryGetValue (_cSmithingMaterialInfo.acMaterial [i].u2ID, out invenSlotNum))
                {
					if (Legion.Instance.cInventory.dicInventory.TryGetValue (invenSlotNum, out item))
                    {
						ownCount = ((MaterialItem)item).u2Count;
					}
				}
                else
                {
					materialCountCheck = false;
					break;
				}

				if (ownCount >= _cSmithingMaterialInfo.acMaterial [i].u4Count)
                {
					materialCountCheck = true;
				}

                else
                {
					materialCountCheck = false;
					break;
				}
			}

			if (materialCountCheck)
            {
                //txt_Btn_Craft.text = TextManager.Instance.GetText ("popup_title_equip_product_material_lack");
                txt_Btn_Craft.text = TextManager.Instance.GetText("popup_btn_product_equip");
			}
            else
            {
                txt_Btn_Craft.text = TextManager.Instance.GetText("popup_title_get_material");
            }

			//if (_cSmithingMaterialInfo.cCreateGoods.u4Count > Legion.Instance.u4Gold)
            //{
			//	PopupManager.Instance.ShowChargePopup (_cSmithingMaterialInfo.cCreateGoods.u1Type);
			//	return;
			//}
		}
        else
        {
            txt_Btn_Craft.text = TextManager.Instance.GetText("popup_btn_product_equip");
        }
    }

	// 장비제작 버튼 -> 재료팝업 -> 제작 버튼(최종 승인) 눌렀을 시,
	// 재료 재화 체크 -> 재화 부족하면 경고팝업, 
	// 제작등급 체크 -> 스킬을 직접 부여하는 등급이면 스킬선택 팝업
	UInt16 seqNo = 0;
	public void OnClickConfirm()
	{
		bool materialCountCheck=false;
		if (!Legion.Instance.cTutorial.bIng) {
			if (!Legion.Instance.CheckEmptyInven ())
				return;

			for (int i = 0; i < EquipmentInfo.SmithingMaterial.MAX_MATERIAL && _cSmithingMaterialInfo.acMaterial [i].u1Type != 0; i++) {
				UInt16 ownCount = 0;
				Item item = null;
				UInt16 invenSlotNum = 0;
				if (Legion.Instance.cInventory.dicItemKey.TryGetValue (_cSmithingMaterialInfo.acMaterial [i].u2ID, out invenSlotNum)) {
					if (Legion.Instance.cInventory.dicInventory.TryGetValue (invenSlotNum, out item)) {
						ownCount = ((MaterialItem)item).u2Count;
					}
				} else {
					materialCountCheck = false;
					break;
				}

				if (ownCount >= _cSmithingMaterialInfo.acMaterial [i].u4Count) {
					materialCountCheck = true;
				} else {
					materialCountCheck = false;
					break;
				}
			}

			if (!materialCountCheck) {
				PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_equip_product_material_lack"), TextManager.Instance.GetText ("popup_desc_equip_product_material_lack"), emptyMethod);
				return;
			}

			if (_cSmithingMaterialInfo.cCreateGoods.u4Count > Legion.Instance.Gold) {
				PopupManager.Instance.ShowChargePopup (_cSmithingMaterialInfo.cCreateGoods.u1Type);
				return;
			}
		}
			
		_subPanelConfirm.SetActive(false);
		PopupManager.Instance.RemovePopup(_subPanelConfirm);
		if(_cForgeInfo.cSmithingInfo.u1SelectSkillCount != 0)
		{
			if(_cEquipInfo.u2ClassID != ClassInfo.COMMON_CLASS_ID) 
			{
				Show_SelectSkillPopup();
			}
			else
			{
				RequestSmithing();
			}
		}
		else
		{
			RequestSmithing();
		}
	}

	public void emptyMethod(object[] param)
	{

	}

	public void RequestSmithing()
	{
		PopupManager.Instance.ShowLoadingPopup(1);
		_subPanelSelectSkills.SetActive(false);
		_subPanelConfirm.SetActive(false);
		if (Legion.Instance.cTutorial.au1Step [7] == 1) {
			seqNo = Server.ServerMgr.Instance.SmithingEquipmentItem (this, 7, Server.ConstDef.LastTutorialStep, SmithingSuccess);
		}else{
			seqNo = Server.ServerMgr.Instance.SmithingEquipmentItem (this, 255, 0, SmithingSuccess);
		}
		PopupManager.Instance.ShowLoadingPopup(1);
	}
	public UInt16 _u2LastSlotNum=0;
	public void SmithingSuccess(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();
		if(err != Server.ERROR_ID.NONE)
		{
			DebugMgr.Log("SmithingFailed");
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.FORGE_SMITH, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
			DebugMgr.Log("SmithingSuccess");
			PopupManager.Instance.CloseLoadingPopup();

			if (Legion.Instance.cTutorial.au1Step [7] != 1) {
				for (int i = 0; i < EquipmentInfo.SmithingMaterial.MAX_MATERIAL && _cSmithingMaterialInfo.acMaterial [i].u1Type != 0; i++) {
					UInt16 ownCount = 0;
					Item item = null;
					if (Legion.Instance.cInventory.dicInventory.TryGetValue (Legion.Instance.cInventory.dicItemKey [_cSmithingMaterialInfo.acMaterial [i].u2ID], out item)) {
						ownCount = ((MaterialItem)item).u2Count;
					}
				
					if (ownCount > _cSmithingMaterialInfo.acMaterial [i].u4Count) {
						((MaterialItem)item).u2Count -= (UInt16)(_cSmithingMaterialInfo.acMaterial [i].u4Count);
					} else if (ownCount == _cSmithingMaterialInfo.acMaterial [i].u4Count) {
						Legion.Instance.cInventory.dicInventory.Remove (Legion.Instance.cInventory.dicItemKey [_cSmithingMaterialInfo.acMaterial [i].u2ID]);
						Legion.Instance.cInventory.dicItemKey.Remove (_cSmithingMaterialInfo.acMaterial [i].u2ID);
					}
				}
			}
            Legion.Instance.acEquipDesignMake.Set(_cEquipInfo.u2ID - Server.ConstDef.BaseEquipDesignID, false);

            _cParent2._imgNewProduct.SetActive(false);
            if(!bReforged)
            {
                //bPlayingResultEffect = true;
			    StartCoroutine(showResultBeforEff());
                StopCoroutine("ResultEffectSpeedControll");
                StartCoroutine("ResultEffectSpeedControll");
            }
			else
                StartCoroutine(showResultReforged());
		}
	}
	//bool bPlayingResultEffect;
	//void Update()
	//{
	//	if(bPlayingResultEffect)
	//	{
    //
    //    }
	//}

    IEnumerator ResultEffectSpeedControll()
    {
        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Time.timeScale = 2f;
            }
            if (Input.GetMouseButtonUp(0))
            {
                Time.timeScale = 1f;
            }
            yield return null;
        }
    }

    // 제작완료 후 결과팝업 연출
    IEnumerator showResultBeforEff()
	{
		_objSmithingEffect.SetActive(true);
		yield return new WaitForSeconds(2.6125f);
        // 연출이 끝나면 속도먼저 줄임
        StopCoroutine("ResultEffectSpeedControll");
        Time.timeScale = 1.0f;
        //bPlayingResultEffect = false;

        _cParent2.ShowSmithingResult(_cForgeInfo.u1Level, _u2LastSlotNum);

		yield return new WaitForSeconds(0.3f);

        PopupManager.Instance.AddPopup(_cParent2._scriptSmithingResult.gameObject, _cParent2.CloseResult);
		yield return new WaitForSeconds(0.7f);
		_objSmithingEffect.SetActive(false);
		
		_objResultPopupEffect.SetActive(false);
		InitMaterialList();
	}
    //다시제작후 결과창
    IEnumerator showResultReforged()
	{
        _cParent2.ShowSmithingResult(_cForgeInfo.u1Level, _u2LastSlotNum);
		yield return new WaitForSeconds(0.3f);

        PopupManager.Instance.AddPopup(_cParent2._scriptSmithingResult.gameObject, _cParent2.CloseResult);
		yield return new WaitForSeconds(0.7f);
        bReforged = false;
		InitMaterialList();
	}
	// 스킬선택값 저장
	public void SelectedSkills(Byte count, Byte[] skillSlot)
	{
		_u1SelectedSkillCount = count;
		_au1SelectedSkill = skillSlot;
		
		int skillSlotIdx=0;
		for(Byte i=_cForgeInfo.cSmithingInfo.u1RandomSkillCount; i<_u1SkillCount-count; i++)
		{
			_txtSkillName[i].text = TextManager.Instance.GetText("select_skill");
		}
		for(Byte i=(Byte)(_u1SkillCount-count); i<_u1SkillCount; i++)
		{
			_txtSkillName[i].text = TextManager.Instance.GetText( SkillInfoMgr.Instance.GetInfoBySlot(_cEquipInfo.u2ClassID,skillSlot[skillSlotIdx++]).sName );
		}
	}

	public void Show_SelectSkillPopup()
	{
		_scriptSelectSkill.SetData(_cEquipInfo.u2ClassID, _cForgeInfo.cSmithingInfo.u1SelectSkillCount, this);
		_subPanelSelectSkills.SetActive(true);
		PopupManager.Instance.AddPopup(_subPanelSelectSkills, Close_SelectSkill);
	}

	// 제작 재료목록 누르면 바로가기 팝업
	public void OnClick_MaterialListElement(Goods materialInfo)
	{
		MaterialItemInfo itemInfo = ItemInfoMgr.Instance.GetMaterialItemInfo(materialInfo.u2ID);
		_scriptMaterialShortcut.SetData(itemInfo);

        ShowModelObject(false);

		_subPanelConfirm.SetActive(false);
		_subPanelMaterialShortCut.SetActive(true);
        PopupManager.Instance.AddPopup(_subPanelMaterialShortCut, Close_MaterialShortCut);
	}

	public void Close_SelectSmithingLevel()
	{
		ShowModelObject(true);

		_subPanelSelectLevel.SetActive(false);
		PopupManager.Instance.RemovePopup(_subPanelSelectLevel);
	}

	public void Close_SelectSkill()
	{
		ShowModelObject(true);

		_subPanelSelectSkills.SetActive(false);
		PopupManager.Instance.RemovePopup(_subPanelSelectSkills);
	}

	public void Close_Confirm()
	{
		ShowModelObject(true);

		_subPanelConfirm.SetActive(false);
		PopupManager.Instance.RemovePopup(_subPanelConfirm);
	}

	public void Close_MaterialShortCut()
	{
		_subPanelConfirm.SetActive(true);
		_subPanelMaterialShortCut.SetActive(false);
        PopupManager.Instance.RemovePopup(_subPanelMaterialShortCut);
	}

	public void ShowModelObject(bool active)
	{
		if(active)
		{
            _trEquipEffectParent.gameObject.SetActive(true);
			if(_cEquipInfo.u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 && _cEquipInfo.u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
			{
				_trEquipModelParent.gameObject.SetActive(true);
				_imgAccessory.gameObject.SetActive(false);
			}
			else
			{
				_trEquipModelParent.gameObject.SetActive(false);
				_imgAccessory.gameObject.SetActive(true);
			}
		}
		else
		{
            _trEquipEffectParent.gameObject.SetActive(false);
			if(_cEquipInfo.u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 && _cEquipInfo.u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
			{
				_trEquipModelParent.gameObject.SetActive(false);
				_imgAccessory.gameObject.SetActive(false);
			}
			else
			{
				_trEquipModelParent.gameObject.SetActive(false);
				_imgAccessory.gameObject.SetActive(false);
			}
		}
	}
}
