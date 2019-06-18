using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
public class UI_Panel_Forge_UpgradeForge : MonoBehaviour {
	[SerializeField] Image _imgLevel;
	[SerializeField] Text _txtLevelName;
	[SerializeField] Text _txtNextLevelName;
	[SerializeField] GameObject _objMaterialTitle;
//	[SerializeField] GameObject _objMaterialTitle_MaxLevel;

	[SerializeField] RectTransform _trMaterialListParent;
	[SerializeField] GameObject _objGoods;
	[SerializeField] Text _txtGold;
	[SerializeField] GameObject _objMaxLevelMsg;
	[SerializeField] GameObject _objAlramIcon;
	[SerializeField] Button _btnConfirm;

	[SerializeField] GameObject _objConfirmPopup;
	[SerializeField] Text _txtConfirmPopupTitle;
	[SerializeField] Text _txtConfirmPopupContent;

	[SerializeField] GameObject _subPanelMaterialShortCut;
	[SerializeField] UI_SubPanel_Forge_Smithing_MaterialShortcut _scriptMaterialShortcut;

//	[SerializeField] GameObject _panelUpgradeForgeResult;
//	[SerializeField] UI_Panel_Forge_UpgradeForge_Result _scriptUpgradeForgeResult;

	[SerializeField] GameObject _objEff_BG;
	[SerializeField] Image _imgEff_ForgeIcon;
	[SerializeField] GameObject _objEff;

	ForgeInfo _cForgeInfo = null;
	ForgeInfo _cNextForgeInfo = null;
    //UI_Panel_CharacterInfo _cCharacterInfo;
	public GameObject disObj;
	public DiscountUI disScript;

	void OnEnable()
	{
        //티어7까지 제한
		if(Legion.Instance.u1ForgeLevel < ForgeInfo.FORGE_LEVEL_MAX-3)
		{
			SetData(ForgeInfoMgr.Instance.GetList()[(Legion.Instance.u1ForgeLevel-1)],
		        ForgeInfoMgr.Instance.GetList()[(Legion.Instance.u1ForgeLevel)]);
		}
		else
		{
			SetDataMaxLevel();
		}
        //_cCharacterInfo = GameObject.Find("Pref_UI_CharacterInfo").GetComponent<UI_Panel_CharacterInfo>();
		_objConfirmPopup.SetActive(false);
	}

	public void SetData(ForgeInfo forgeInfo, ForgeInfo nextForgeInfo)
	{
		_cForgeInfo = forgeInfo;
		_cNextForgeInfo = nextForgeInfo;
		string forgeNameNextLevel = "";

		_txtLevelName.text = TextManager.Instance.GetText (_cForgeInfo.sName) + "  " + TextManager.Instance.GetText("mark_upgrade_tier");
		UIManager.Instance.SetGradientFromTier( _txtLevelName.GetComponent<Gradient>(), (forgeInfo.u1Level));
		_imgLevel.sprite = AtlasMgr.Instance.GetSprite("Sprites/Forge/Forge_grade_01.Forge_grade_01_"+(forgeInfo.u1Level-1));
		_imgLevel.SetNativeSize();

		forgeNameNextLevel = _cNextForgeInfo.sName;
		_txtNextLevelName.text = TextManager.Instance.GetText(forgeNameNextLevel) + "  " + TextManager.Instance.GetText("mark_upgrade_marterial");
		UIManager.Instance.SetGradientFromTier(_txtNextLevelName.GetComponent<Gradient>(), nextForgeInfo.u1Level);
		_txtNextLevelName.gameObject.SetActive(true);
//		_objMaterialTitle.SetActive(true);

		_objGoods.SetActive(true);

        // 비용할인 적용 시

        EventDisCountinfo info = EventInfoMgr.Instance.GetDiscountEventinfo(DISCOUNT_ITEM.UPGRADEFORGE);

		UInt32 price = ForgeInfoMgr.Instance.GetList()[(Legion.Instance.u1ForgeLevel)].cUpgradeInfo.cUpgradeGoods.u4Count;

        if (info != null)
        {
			disObj.SetActive (true);
			disScript.SetData (price, info.u1DiscountRate);
            uint discountGold = (uint)(ForgeInfoMgr.Instance.GetList()[(Legion.Instance.u1ForgeLevel)].cUpgradeInfo.cUpgradeGoods.u4Count * info.discountRate);
            _txtGold.text = discountGold.ToString();
        }
        else
        {
			disObj.SetActive (false);
            _txtGold.text = ForgeInfoMgr.Instance.GetList()[(Legion.Instance.u1ForgeLevel)].cUpgradeInfo.cUpgradeGoods.u4Count.ToString();
        }

		_objMaxLevelMsg.SetActive(false);
		clearMaterialList();
		initMaterialList();
		if(CheckMaterial())
		{
            GameObject.Find("Pref_UI_CharacterInfo").GetComponent<UI_Panel_CharacterInfo>().SetSmithingAlram(true);
            _objAlramIcon.SetActive(true);
			_btnConfirm.interactable = true;
		}
		else
		{
            GameObject.Find("Pref_UI_CharacterInfo").GetComponent<UI_Panel_CharacterInfo>().SetSmithingAlram(false);
            _objAlramIcon.SetActive(false);
			_btnConfirm.interactable = false;
		}
	}

	bool CheckMaterial()
	{
		bool materialCountCheck=false;
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

	public void SetDataMaxLevel()
	{
		_cForgeInfo = ForgeInfoMgr.Instance.GetList()[(Legion.Instance.u1ForgeLevel-1)];

		_txtLevelName.text = TextManager.Instance.GetText (_cForgeInfo.sName)+" "+TextManager.Instance.GetText("mark_upgrade_tier");
		UIManager.Instance.SetGradientFromTier( _txtLevelName.GetComponent<Gradient>(), _cForgeInfo.u1Level);
		_imgLevel.sprite = AtlasMgr.Instance.GetSprite("Sprites/Forge/Forge_grade_01.Forge_grade_01_"+(_cForgeInfo.u1Level-1));
		_imgLevel.SetNativeSize();
		_txtNextLevelName.gameObject.SetActive(false);
//		_objMaterialTitle.SetActive(false);
		_objMaxLevelMsg.SetActive(true);

		_btnConfirm.interactable = false;
		_btnConfirm.GetComponent<Image>().color = _btnConfirm.colors.disabledColor;
		_btnConfirm.transform.FindChild("Text").GetComponent<Text>().color = _btnConfirm.colors.disabledColor;
		_txtNextLevelName.text = TextManager.Instance.GetText(_cForgeInfo.sName);
		_objGoods.SetActive(false);
		_txtGold.text = "0";

		_objAlramIcon.SetActive(false);
        GameObject.Find("Pref_UI_CharacterInfo").GetComponent<UI_Panel_CharacterInfo>().SetSmithingAlram(false);
		clearMaterialList();
	}
	
	void clearMaterialList()
	{
		for(int i=0; i<_trMaterialListParent.childCount; i++)
		{
			DestroyObject(_trMaterialListParent.GetChild(i).gameObject);
		}
	}

	void initMaterialList()
	{
		clearMaterialList();
		for(int i=0; i<_cNextForgeInfo.cUpgradeInfo.acUpgradeMaterials.Length; i++)
		{
			GameObject listElement = Instantiate( AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Equipment/Pref_UI_ListElement_Forge_Upgrade_Material.prefab", typeof(GameObject)) ) as GameObject;
			RectTransform trListElement = listElement.GetComponent<RectTransform>();
			trListElement.SetParent(_trMaterialListParent);
			trListElement.localScale = Vector3.one;
			trListElement.localPosition = Vector3.zero;
			
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
			listElement.GetComponent<UI_ListElement_Forge_Smithing_Material>().SetDataUpgrade(_cNextForgeInfo.cUpgradeInfo.acUpgradeMaterials[i], ownCount);
			UInt16 itemID = 0;
			itemID = _cNextForgeInfo.cUpgradeInfo.acUpgradeMaterials[i].u2ID;
			listElement.GetComponent<Button>().onClick.AddListener( () => OnClick_MaterialListElement(itemID) );
		}
	}

	public void OnClick_MaterialListElement(UInt16 itemID)
	{
		MaterialItemInfo itemInfo = ItemInfoMgr.Instance.GetMaterialItemInfo(itemID);
		_scriptMaterialShortcut.SetData(itemInfo);
		_subPanelMaterialShortCut.SetActive(true);
	}

	public void Close_ConfirmPopup()
	{
		_objConfirmPopup.SetActive(false);
		_btnConfirm.interactable = true;
		PopupManager.Instance.RemovePopup(_objConfirmPopup);
	}
	UInt16 seqNo = 0;
	public void OnClickConfirm()
	{
		_btnConfirm.interactable = false;
		PopupManager.Instance.AddPopup(_objConfirmPopup, Close_ConfirmPopup);
		bool materialCountCheck=CheckMaterial();
		
		if (!materialCountCheck)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_equip_product_material_lack"), TextManager.Instance.GetText("popup_desc_equip_product_material_lack"), emptyMethod);
			return;
		}

		if (!Legion.Instance.CheckEnoughGoods (_cNextForgeInfo.cUpgradeInfo.cUpgradeGoods.u1Type, _cNextForgeInfo.cUpgradeInfo.cUpgradeGoods.u4Count))
		{
			PopupManager.Instance.ShowChargePopup(_cNextForgeInfo.cUpgradeInfo.cUpgradeGoods.u1Type);
			return;

		}

//		_txtConfirmPopupTitle.text =
		_txtConfirmPopupContent.text = string.Format(TextManager.Instance.GetText("popup_desc_upgrade")+"\n\n{0} > {1}",
		                                             TextManager.Instance.GetText(_cForgeInfo.sName),
		                                             TextManager.Instance.GetText(_cNextForgeInfo.sName));
		_objConfirmPopup.SetActive(true);

//		PopupManager.Instance.ShowOKPopup("대장간 업그레이드",
//		                                  string.Format("업그레이드 하시겠습니까?\n\n\t\t\t\t{0} > {1}",
//		              						TextManager.Instance.GetText(_cForgeInfo.sName),
//		              						TextManager.Instance.GetText(_cNextForgeInfo.sName)), RequestUpgrade);

	}

	public void emptyMethod(object[] param)
	{
		
	}

	public void RequestUpgrade()
	{
		PopupManager.Instance.ShowLoadingPopup(1);
		_objConfirmPopup.SetActive(false);
		seqNo = Server.ServerMgr.Instance.UpgradeForge(ResponseUpgrade);
	}

	public void ResponseUpgrade(Server.ERROR_ID err)
	{
        if (err != Server.ERROR_ID.NONE)
        {
            DebugMgr.Log("UpgradeForgeFailed");
            PopupManager.Instance.CloseLoadingPopup();
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.FORGE_UPGRADE, err), Server.ServerMgr.Instance.CallClear);
        }
        else
        {
            DebugMgr.Log("UpgradeForgeSuccess");
            PopupManager.Instance.CloseLoadingPopup();
            //_scriptUpgradeForgeResult.SetData(_cNextForgeInfo, this);
            EventDisCountinfo info = EventInfoMgr.Instance.GetDiscountEventinfo(DISCOUNT_ITEM.UPGRADEFORGE);

            for (int i = 0; i < _cNextForgeInfo.cUpgradeInfo.acUpgradeMaterials.Length; i++)
            {
                UInt16 ownCount = 0;
                Item item = null;
                if (Legion.Instance.cInventory.dicInventory.TryGetValue(Legion.Instance.cInventory.dicItemKey[_cNextForgeInfo.cUpgradeInfo.acUpgradeMaterials[i].u2ID], out item))
                {
                    ownCount = ((MaterialItem)item).u2Count;
                }

                if (ownCount > _cNextForgeInfo.cUpgradeInfo.acUpgradeMaterials[i].u4Count)
                {
                    ((MaterialItem)item).u2Count -= (UInt16)(_cNextForgeInfo.cUpgradeInfo.acUpgradeMaterials[i].u4Count);
                }
                else if (ownCount == _cNextForgeInfo.cUpgradeInfo.acUpgradeMaterials[i].u4Count)
                {
                    Legion.Instance.cInventory.dicInventory.Remove(Legion.Instance.cInventory.dicItemKey[_cNextForgeInfo.cUpgradeInfo.acUpgradeMaterials[i].u2ID]);
                    Legion.Instance.cInventory.dicItemKey.Remove(_cNextForgeInfo.cUpgradeInfo.acUpgradeMaterials[i].u2ID);
                }
            }
            showUpgradeEffect();

            //bool isForgeUpdate = CheckMaterial();
            //GameObject.Find("Pref_UI_CharacterInfo").GetComponent<UI_Panel_CharacterInfo>().SetSmithingAlram(isForgeUpdate);
            //_objAlramIcon.SetActive(isForgeUpdate);
		}
	}

	//bool bPlayingResultEffect;
	//void Update()
	//{
	//	if(bPlayingResultEffect)
	//	{
	//		if(Input.GetMouseButtonDown(0))
	//		{
	//			Time.timeScale = 2f;
	//		}
	//	}
	//	
	//	if(Input.GetMouseButtonUp(0))
	//	{
	//		Time.timeScale = 1f;
	//	}
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

    public void showUpgradeEffect()
	{
		//bPlayingResultEffect = true;
		StartCoroutine( upgradeAni() );
        StopCoroutine("ResultEffectSpeedControll");
        StartCoroutine("ResultEffectSpeedControll");
    }

	IEnumerator upgradeAni()
	{
		_imgEff_ForgeIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Forge/Forge_grade_01.Forge_grade_01_"+(Legion.Instance.u1ForgeLevel-2));
		yield return new WaitForSeconds(0.5f);
		_objEff.SetActive(true);

		yield return new WaitForSeconds(1.3f);
		_imgEff_ForgeIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Forge/Forge_grade_01.Forge_grade_01_"+(Legion.Instance.u1ForgeLevel-1));
        //티어5까지 제한
		if(Legion.Instance.u1ForgeLevel < ForgeInfo.FORGE_LEVEL_MAX-5)
			SetData(_cNextForgeInfo, ForgeInfoMgr.Instance.GetList()[(Legion.Instance.u1ForgeLevel)]);
		else
			SetDataMaxLevel();

		yield return new WaitForSeconds(1.7f);
        //bPlayingResultEffect = false;
        StopCoroutine("ResultEffectSpeedControll");
        Time.timeScale = 1f;

        _objEff.SetActive(false);
		PopupManager.Instance.ShowYesNoPopup(
			TextManager.Instance.GetText("popup_title_upgrade_result"),

			string.Format(TextManager.Instance.GetText("popup_desc_upgrade_result"), TextManager.Instance.GetText(_cForgeInfo.sName)), CheckQuestComplete, new object[0], CheckQuestComplete, new object[0]);

//		_panelUpgradeForgeResult.SetActive(true);

	}

	public void CheckQuestComplete(object[] param)
	{
        PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.ForgeLevel);
        //Legion.Instance.cQuest.CheckEndDirection (AchievementTypeData.ForgeLevel);
	}
}
