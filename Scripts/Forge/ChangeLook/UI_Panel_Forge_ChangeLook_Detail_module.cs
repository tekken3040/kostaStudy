using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class UI_Panel_Forge_ChangeLook_Detail_module : MonoBehaviour
{
    [System.Serializable]
	class EquipModelObject
	{
		public RectTransform _trEquipModelParent;
		public SpriteRenderer _imgAccessory;
		public GameObject _objEquipModel;
		public GameObject _objEquipEffect;
		public RectTransform _trEquipEffectParent;
	}

	[SerializeField] Text _txtTier;
	[SerializeField] Image _imgElementIcon;
	[SerializeField] Text _txtEquipName;

	[SerializeField] RectTransform _trScrollParent;
	[SerializeField] RectTransform _trLookListParent;

	[SerializeField] Image _imgElement;
	[SerializeField] Image _imgExpGauge;
	[SerializeField] Text _txtExpValue;
	[SerializeField] Text _txtLevel;
	[SerializeField] Text _txtCreator;

	[SerializeField] Text _txtGoodsName;
	[SerializeField] Text _txtGoodsValue;

	[SerializeField] Button _btnConfirm;

	[SerializeField] EquipModelObject _cOriginModel;
	[SerializeField] EquipModelObject _cChangeModel;

    [SerializeField] GameObject _panel;
    [SerializeField] GameObject _result;
    [SerializeField] GameObject _effResultPopupMask;

    [SerializeField] UI_Button_CharacterInfo_Equipment_StateInfo _specializeBtn;
    [SerializeField] Text _equipmentClassName;
    [SerializeField] RectTransform _trNameGroup;
    [SerializeField] RectTransform _trStarGroup;
    //[SerializeField] GameObject Pref_Star;
    [SerializeField] GameObject starPos;

	EquipmentItem _cEquipItem;
	ForgeInfo _cForgeInfo;

	int _nSelectedModelIdx = -1;
	UInt16 _u2SelectedModelID = 0;
	List<UInt16> lstLookDesign;
    Byte currentPanel = 0;

	public GameObject disObj;
	public DiscountUI disScript;

	void OnEnable()
	{
		_nSelectedModelIdx = -1;
		_btnConfirm.interactable = false;
		AtlasMgr.Instance.SetGrayScale(_btnConfirm.image);
		_btnConfirm.image.SetNativeSize();
	}

	public void SetData(EquipmentItem equipItem, Byte _scene)
	{
		_cEquipItem = equipItem;
        currentPanel = _scene;
		InitLookDesignListData ();
		_cForgeInfo = ForgeInfoMgr.Instance.GetList () [Legion.Instance.u1ForgeLevel-1];

		//_imgElementIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_icon_" + equipItem.GetEquipmentInfo().u1Element);
		//_imgElementIcon.SetNativeSize();

		//_txtTier.text = "<" + TextManager.Instance.GetText( "forge_level_" + equipItem.u1SmithingLevel ) + ">";
		_txtTier.text = TextManager.Instance.GetText( "forge_level_" + equipItem.u1SmithingLevel );
		UIManager.Instance.SetGradientFromTier(_txtTier.GetComponent<Gradient>(), equipItem.u1SmithingLevel);

        _specializeBtn.SetData((Byte)(_cEquipItem.GetEquipmentInfo().u1Specialize+2));
        Color tempColor;
        ColorUtility.TryParseHtmlString(_cEquipItem.GetEquipmentInfo().GetHexColor((Byte)(_cEquipItem.GetEquipmentInfo().u1Specialize+2)), out tempColor);
        _specializeBtn.transform.parent.GetComponent<Image>().color = tempColor;
        if(_cEquipItem.GetEquipmentInfo().u2ClassID <= ClassInfo.LAST_CLASS_ID)
            _equipmentClassName.text = TextManager.Instance.GetText(ClassInfoMgr.Instance.GetInfo(_cEquipItem.GetEquipmentInfo().u2ClassID).sName);
        else
            _equipmentClassName.text = TextManager.Instance.GetText("equip_common");
        
        for(int i=0; i<starPos.transform.GetChildCount(); i++)
            starPos.transform.GetChild(i).gameObject.SetActive(false);
        for(int i=0; i<_cEquipItem.u1Completeness; i++)
        {
            starPos.transform.GetChild(i).gameObject.SetActive(true);
            UIManager.Instance.SetGradientFromTier(starPos.transform.GetChild(i).GetComponent<Gradient>(), _cEquipItem.u1SmithingLevel);
        }
        starPos.GetComponent<GridLayoutGroup>().SetLayoutHorizontal();

		string equipName = "";
		equipName = equipItem.itemName;
		if(equipName != "")
			equipName = equipName + " " + TextManager.Instance.GetText( equipItem.GetEquipmentInfo().sName );
		else
			equipName = TextManager.Instance.GetText( equipItem.GetEquipmentInfo().sName );
		_txtEquipName.text = equipName;
		_txtEquipName.color = EquipmentItem.equipElementColors[(equipItem.GetEquipmentInfo().u1Element)];
        _txtEquipName.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
        UIManager.Instance.SetGradientFromElement(_txtEquipName.GetComponent<Gradient>(), equipItem.GetEquipmentInfo().u1Element);
		_imgExpGauge.fillAmount = (float)((float)equipItem.cLevel.u8Exp / (float)equipItem.cLevel.u8NextExp);

		_txtExpValue.text = string.Format("{0}/{1}", ConvertExpValue(equipItem.cLevel.u8Exp), ConvertExpValue(equipItem.cLevel.u8NextExp));

		_txtLevel.text = equipItem.cLevel.u2Level.ToString();

		if(equipItem.createrName != "")
			_txtCreator.text = "By " + equipItem.createrName;
		else
			_txtCreator.text = "";
		
        //UIManager.Instance.SetSizeTextGroup(_trNameGroup, 18);
        UIManager.Instance.SetSizeTextGroup(_trStarGroup, 18);

		UInt16 modelID = equipItem.u2ModelID;
		if(modelID == 0) modelID = equipItem.GetEquipmentInfo().u2ModelID;

		Byte heightCount=0;
		int listElementCount = 0;
		for(int i=0;i<_trLookListParent.childCount; i++)
		{
			DestroyObject(_trLookListParent.GetChild(i).gameObject);
		}
		for (int i=0; i<lstLookDesign.Count; i++)
		{
			GameObject listElement = null;
			Transform trListElement = null;
			listElement = Instantiate( AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Equipment/Pref_UI_ListElement_Forge_ChangeLook_Model.prefab", typeof(GameObject)) ) as GameObject;
			trListElement = listElement.GetComponent<RectTransform>();
			trListElement.SetParent(_trLookListParent);
			trListElement.localScale = Vector3.one;
			trListElement.localPosition = Vector3.zero;
			trListElement.name = lstLookDesign[i].ToString();
			listElement.GetComponent<UI_ListElement_Forge_ChangeLook_Model>().SetData(ModelInfoMgr.Instance.GetInfo(lstLookDesign[i]));
			if(Legion.Instance.acLookDesignNew.Get((int)(lstLookDesign[i] - Server.ConstDef.BaseLookDesignID)))
			{
				listElement.GetComponent<UI_ListElement_Forge_ChangeLook_Model>()._objNewIcon.SetActive(true);
				Legion.Instance.acLookDesignNew.Set((int)(lstLookDesign[i] - Server.ConstDef.BaseLookDesignID), false);
			}

			if(lstLookDesign[i] == equipItem.u2ModelID) listElement.GetComponent<UI_ListElement_Forge_ChangeLook_Model>().SetActive(false);

			int itemID = 0;
			itemID = lstLookDesign[i];
			listElement.GetComponentInChildren<Button>().onClick.AddListener( () => OnClickModelList_ModelID(itemID) );
			int listIdx = 0;
			listIdx = i;
			listElement.GetComponentInChildren<Button>().onClick.AddListener( () => OnClickModelList_IDX(listIdx) );
			listElementCount++;
			heightCount++;
		}

		_trScrollParent.sizeDelta = new Vector2(314, 90f*heightCount);

		_txtGoodsValue.text = GetPrice().ToString();

		for(int i=0; i<_cOriginModel._trEquipModelParent.childCount; i++)
		{
			DestroyObject(_cOriginModel._trEquipModelParent.GetChild(i).gameObject);
		}
		for(int i=0; i<_cChangeModel._trEquipModelParent.childCount; i++)
		{
			DestroyObject(_cChangeModel._trEquipModelParent.GetChild(i).gameObject);
		}

		if(equipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 && equipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
		{
			equipItem.InitViewModelObject();
			equipItem.cObject.GetComponent<EquipmentObject>().SetLayer( LayerMask.NameToLayer("UI") );
			equipItem.cObject.transform.SetParent( _cOriginModel._trEquipModelParent );
			_cOriginModel._trEquipModelParent.GetComponent<RotateCharacter>().characterTransform = equipItem.cObject.transform;
			equipItem.cObject.transform.localScale =  new Vector3(0.8f, 0.8f, 0.8f);
			equipItem.cObject.transform.localPosition = new Vector3(0f, -100f, 0f);
			equipItem.cObject.transform.name = "EquipmentObject";
			_cOriginModel._objEquipModel = equipItem.cObject;
			_cOriginModel._imgAccessory.gameObject.SetActive(false);
		}
		else
		{
			_cOriginModel._imgAccessory.sprite = AssetMgr.Instance.AssetLoad("Sprites/Item/Accessory/acc_" + modelID + ".png", typeof(Sprite)) as Sprite;
			_cOriginModel._imgAccessory.gameObject.SetActive(true);
		}

		if(_cOriginModel._objEquipEffect != null) DestroyObject(_cOriginModel._objEquipEffect);
		if(equipItem.u1SmithingLevel >= 1)
		{
			DebugMgr.Log("Asset : " +string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}", equipItem.u1SmithingLevel));
			_cOriginModel._objEquipEffect = Instantiate(AssetMgr.Instance.AssetLoad(string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}.prefab", equipItem.u1SmithingLevel), typeof(GameObject))) as GameObject;
			_cOriginModel._objEquipEffect.transform.SetParent(_cOriginModel._trEquipEffectParent);
			_cOriginModel._objEquipEffect.transform.name = "WeaponEffect";
			_cOriginModel._objEquipEffect.transform.localScale = Vector3.one;
			_cOriginModel._objEquipEffect.transform.localPosition = Vector3.zero;
		}

	}

	UInt32 GetPrice(){
		EventDisCountinfo disInfo = EventInfoMgr.Instance.GetDiscountEventinfo (DISCOUNT_ITEM.CHANGELOOK);
		UInt32 price = _cForgeInfo.cLookChangeInfo.cChangeGoods.u4Count;
		if (disInfo != null) {
			disObj.SetActive (true);
			disScript.SetData (price, disInfo.u1DiscountRate);
			price = (uint)(_cForgeInfo.cLookChangeInfo.cChangeGoods.u4Count * disInfo.discountRate);
		} else {
			disObj.SetActive (false);
		}

		return price;
	}

	void InitLookDesignListData()
	{
		lstLookDesign = new List<UInt16>();
		if(_cEquipItem.GetEquipmentInfo().u1PosID == EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 || _cEquipItem.GetEquipmentInfo().u1PosID == EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
		{
			lstLookDesign = EquipmentInfoMgr.Instance.GetListOwnLookDesign(ClassInfo.COMMON_CLASS_ID, (Byte)EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1, _cEquipItem.u2ModelID);
			lstLookDesign.AddRange(EquipmentInfoMgr.Instance.GetListOwnLookDesign(ClassInfo.COMMON_CLASS_ID, (Byte)EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2, _cEquipItem.u2ModelID));
		}
		else
		{
			lstLookDesign = EquipmentInfoMgr.Instance.GetListOwnLookDesign(_cEquipItem.GetEquipmentInfo().u2ClassID, (Byte)_cEquipItem.GetEquipmentInfo().u1PosID, _cEquipItem.u2ModelID);
		}
	}

	public void OnClickModelList_ModelID(int modelID)
	{
		_u2SelectedModelID = (UInt16)modelID;
		_btnConfirm.interactable = true;
		AtlasMgr.Instance.SetDefaultShader(_btnConfirm.image);

		for(int i=0; i<_cChangeModel._trEquipModelParent.childCount; i++)
		{
			if(_cChangeModel._trEquipModelParent.GetChild(i) != null)
				DestroyObject(_cChangeModel._trEquipModelParent.GetChild(i).gameObject);
		}

		EquipmentItem equipItem = new EquipmentItem(_cEquipItem.GetEquipmentInfo().u2ID);
		equipItem.u2ModelID = (UInt16)modelID;
		if(equipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 && equipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
		{
			equipItem.InitViewModelObject();
			equipItem.cObject.GetComponent<EquipmentObject>().SetLayer( LayerMask.NameToLayer("UI") );
			equipItem.cObject.transform.SetParent( _cChangeModel._trEquipModelParent );
			_cChangeModel._trEquipModelParent.GetComponent<RotateCharacter>().characterTransform = equipItem.cObject.transform;
			equipItem.cObject.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
			equipItem.cObject.transform.localPosition = new Vector3(0f, -100f, 0f);
			equipItem.cObject.transform.name = "EquipmentObject_Change";
			_cChangeModel._objEquipModel = equipItem.cObject;
			_cChangeModel._imgAccessory.gameObject.SetActive(false);
		}
		else
		{
			_cOriginModel._imgAccessory.sprite = AssetMgr.Instance.AssetLoad("Sprites/Item/Accessory/acc_" + modelID + ".png", typeof(Sprite)) as Sprite;
			_cOriginModel._imgAccessory.gameObject.SetActive(true);
		}

		if(_cChangeModel._objEquipEffect != null) DestroyObject(_cChangeModel._objEquipEffect);
		if(_cEquipItem.u1SmithingLevel >= 1)
		{
			DebugMgr.Log("Asset : " +string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}", _cEquipItem.u1SmithingLevel));
			_cChangeModel._objEquipEffect = Instantiate(AssetMgr.Instance.AssetLoad(string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}.prefab", _cEquipItem.u1SmithingLevel), typeof(GameObject))) as GameObject;
			_cChangeModel._objEquipEffect.transform.SetParent(_cChangeModel._trEquipEffectParent);
			_cChangeModel._objEquipEffect.transform.name = "WeaponEffect_Change";
			_cChangeModel._objEquipEffect.transform.localScale = Vector3.one;
			_cChangeModel._objEquipEffect.transform.localPosition = Vector3.zero;
		}
	}

	public void OnClickModelList_IDX(int idx)
	{
		DebugMgr.Log("Look Selected : " + _nSelectedModelIdx);
		DebugMgr.Log("Look Select : " + idx);
		if(_nSelectedModelIdx != -1)
		{
			_trLookListParent.GetChild(_nSelectedModelIdx).GetComponent<UI_ListElement_Forge_ChangeLook_Model>().DeSelect();
		}
		_trLookListParent.GetChild(idx).GetComponent<UI_ListElement_Forge_ChangeLook_Model>().Select();

		_nSelectedModelIdx = idx;
	}

	UInt16 seqNo = 0;
	public void OnClickChangeLook()
	{
		if (!Legion.Instance.CheckEnoughGoods (_cForgeInfo.cLookChangeInfo.cChangeGoods.u1Type, GetPrice()))
		{
			PopupManager.Instance.ShowChargePopup(_cForgeInfo.cLookChangeInfo.cChangeGoods.u1Type);
            //OnClickClose();
			return;
		}
		PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("btn_smith_main_change"), string.Format("{0}"+TextManager.Instance.GetText("popup_desc_equip_changelook"), 
			TextManager.Instance.GetText(ModelInfoMgr.Instance.GetInfo(_u2SelectedModelID).sModelName))
			                                     , RequestChangeLook, null);
	}
	public void emptyMethod(object[] param)
	{
		
	}
	
	public void RequestChangeLook(object[] param)
	{
		DebugMgr.Log("Request Look Model : " + _cEquipItem.u2SlotNum + ", " + _u2SelectedModelID);
		PopupManager.Instance.ShowLoadingPopup(1);
		seqNo = Server.ServerMgr.Instance.ChangeLook(_cEquipItem.u2SlotNum, _u2SelectedModelID, ResponseChangeLook);
	}

	public void ResponseChangeLook(Server.ERROR_ID err)
	{
		if(err != Server.ERROR_ID.NONE)
		{
			DebugMgr.Log("ChangeLookFailed");
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.FORGE_CHANGE_LOOK, err), Server.ServerMgr.Instance.CallClear);
			PopupManager.Instance.CloseLoadingPopup();
		}
		else
		{
			DebugMgr.Log("ChangeLookSuccess");
			DebugMgr.Log("Request Look Model : " + _cEquipItem.u2ModelID);
			PopupManager.Instance.CloseLoadingPopup();
            _result.SetActive(true);
            _result.GetComponent<UI_Panel_Forge_ChangeLook_Result_Module>().SetData(_cEquipItem);
            StartCoroutine(showResultPopupWithAnim());
		}
	}

    IEnumerator showResultPopupWithAnim()
	{
		_effResultPopupMask.SetActive(true);
        _panel.SetActive(false);
		yield return new WaitForSeconds(0.75f);
		_result.SetActive(true);
		PopupManager.Instance.AddPopup(_result, OnClickClose);
		//_result.SetActive(false);
		yield return new WaitForSeconds(1.1f);
		_effResultPopupMask.SetActive(false);
	}

	public void OnClickClose()
	{
        PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.ChangeLook);
        //Legion.Instance.cQuest.CheckEndDirection (AchievementTypeData.ChangeLook);
        if(currentPanel == 1)
        {
            if(Scene.GetCurrent().inventoryPanel != null)
                if(Scene.GetCurrent().inventoryPanel.gameObject.activeSelf)
                    Scene.GetCurrent().inventoryPanel.RefreshSlot();
        }
        else if(currentPanel == 2)
        {
            if(Scene.GetCurrent().GetComponent<LobbyScene>() != null)
            {
                Scene.GetCurrent().GetComponent<LobbyScene>()._characterInfo.GetPanelEquipment().InitEquipSlots();
            }
        }
        PopupManager.Instance.RemovePopup(this.gameObject);
        PopupManager.Instance.RemovePopup(_result);
		DestroyObject(_cOriginModel._objEquipModel);
		DestroyObject(_cEquipItem.cObject);
		//gameObject.SetActive(false);
        DestroyObject(gameObject);
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
}
