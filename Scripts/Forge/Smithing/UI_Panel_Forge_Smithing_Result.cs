using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

public class UI_Panel_Forge_Smithing_Result : MonoBehaviour {
	[SerializeField] UI_Panel_Forge_Smithing_Detail _cParent;
	[SerializeField] GameObject _panel;
//	[SerializeField] Text _txtForgeName;
	[SerializeField] RectTransform _trNameGroup;
    [SerializeField] RectTransform _trStarGroup;
	[SerializeField] Text _txtTier;
	[SerializeField] Text _txtEquipName;
	[SerializeField] Text _txtElement;
    [SerializeField] Image _imgClassIcon;

	[SerializeField] Text[] _txtStatType;
	[SerializeField] Text[] _txtStatValue;
	[SerializeField] UI_Button_CharacterInfo_Equipment_StatInfo[] _btnStatInfo;
	[SerializeField] GameObject[] _objSkill;
	[SerializeField] Image[] _imgSkillIcon;
	[SerializeField] Image[] _imgSkillElement;
	[SerializeField] Text[] _txtSkillName;
	[SerializeField] Text[] _txtSkillValue;
	[SerializeField] UI_Button_CharacterInfo_Equipment_SkillInfo[] _btnSkillInfo;
	[SerializeField] Text[] _txtSkillNone;
	[SerializeField] GameObject _panelChangeName;
	[SerializeField] InputField _inputFieldEquipName;
	[SerializeField] Text _txtInputName;
	[SerializeField] Text _txtEquipNameInChangeNamePanel;
	[SerializeField] Text _txtCreator;

	[SerializeField] Transform _trEquipModelParent;
	[SerializeField] SpriteRenderer _imgAccessory;
	[SerializeField] Image _imgAccessoryUI;
	[SerializeField] Transform _trEquipEffectParent;
    [SerializeField] UI_Button_CharacterInfo_Equipment_StateInfo _specializeBtn;
    [SerializeField] Text _equipmentClassName;
    //[SerializeField] GameObject Pref_Star;
    [SerializeField] GameObject starPos;

	GameObject _objEquipModel;
	GameObject _objEquipEffect;

    StringBuilder tempStringBuilder;
	EquipmentItem _cEquipItem;
    Hero _cHero;
    UI_Panel_CharacterInfo _cCharacterInfoPanel;
	public void SetData(EquipmentItem equipItem, Byte forgeLevel)
	{
        PopupManager.Instance.AddPopup(gameObject, OnClickConfirm);
        tempStringBuilder = new StringBuilder();
        _cCharacterInfoPanel = GameObject.Find("Pref_UI_CharacterInfo").GetComponent<UI_Panel_CharacterInfo>();
        _cHero = _cCharacterInfoPanel.GetHero;
		_cEquipItem = equipItem;
        //		_txtForgeName.text = "<" + TextManager.Instance.GetText( ForgeInfoMgr.Instance.GetList()[(forgeLevel-1)].sName ) + ">";
        //		_txtForgeName.color = ForgeInfo.forgeLevelColor[(int)(forgeLevel-1)];
        //_txtTier.text = "<" + TextManager.Instance.GetText("forge_level_" + forgeLevel) + ">";
        StringBuilder tempString = new StringBuilder();
        _txtTier.text = TextManager.Instance.GetText(tempString.Append("forge_level_").Append(forgeLevel).ToString());
		UIManager.Instance.SetGradientFromTier(_txtTier.GetComponent<Gradient>(), forgeLevel);
		_txtEquipName.text = TextManager.Instance.GetText( equipItem.GetEquipmentInfo().sName);
        _txtEquipName.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
		UIManager.Instance.SetGradientFromElement(	_txtEquipName.GetComponent<Gradient>(), equipItem.GetEquipmentInfo().u1Element );
        //UIManager.Instance.SetSizeTextGroup(_trNameGroup, 18);

        tempString.Remove(0, tempString.Length);
        _txtElement.text = TextManager.Instance.GetText(tempString.Append("element_").Append(equipItem.GetEquipmentInfo().u1Element).ToString());
		DebugMgr.Log(tempString.ToString());
		UIManager.Instance.SetGradientFromElement(	_txtElement.GetComponent<Gradient>(), equipItem.GetEquipmentInfo().u1Element );

        _specializeBtn.SetData((Byte)(_cEquipItem.GetEquipmentInfo().u1Specialize+2));
        Color tempColor;
        ColorUtility.TryParseHtmlString(_cEquipItem.GetEquipmentInfo().GetHexColor((Byte)(_cEquipItem.GetEquipmentInfo().u1Specialize+2)), out tempColor);
        _specializeBtn.transform.parent.GetComponent<Image>().color = tempColor;
        if(_cEquipItem.GetEquipmentInfo().u2ClassID <= ClassInfo.LAST_CLASS_ID)
        {
            _equipmentClassName.text = TextManager.Instance.GetText(ClassInfoMgr.Instance.GetInfo(_cEquipItem.GetEquipmentInfo().u2ClassID).sName);
            tempString.Remove(0, tempString.Length);
            _imgClassIcon.sprite = AtlasMgr.Instance.GetSprite(tempString.Append("Sprites/Common/common_class.common_class_").Append(_cEquipItem.GetEquipmentInfo().u2ClassID).ToString());
            _imgClassIcon.enabled = true;
            _imgClassIcon.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
            _imgClassIcon.SetNativeSize();
        }
        else
        {
            _equipmentClassName.text = TextManager.Instance.GetText("equip_common");
            _imgClassIcon.enabled = false;
        }

        for(int i=0; i<starPos.transform.childCount; i++)
            starPos.transform.GetChild(i).gameObject.SetActive(false);
        for(int i=0; i<_cEquipItem.u1Completeness; i++)
        {
            starPos.transform.GetChild(i).gameObject.SetActive(true);
            UIManager.Instance.SetGradientFromTier(starPos.transform.GetChild(i).GetComponent<Gradient>(), _cEquipItem.u1SmithingLevel);
        }
        starPos.GetComponent<GridLayoutGroup>().SetLayoutHorizontal();
        UIManager.Instance.SetSizeTextGroup(_trStarGroup, 18);
		for(int i=0; i<Server.ConstDef.EquipStatPointType; i++)
		{
			_txtStatType[i].text = Status.GetStatText( equipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType );
			_txtStatValue[i].text = equipItem.cFinalStatus.GetStat(equipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType).ToString();
			_btnStatInfo[i].SetData(equipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType);
		}

		if (equipItem.u1SmithingLevel != 0) {
			if(equipItem.GetEquipmentInfo().u1PosID == EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 ||
				equipItem.GetEquipmentInfo().u1PosID == EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
			{
				for (Byte i=0; i<EquipmentInfo.ADD_SKILL_MAX; i++) {
					_objSkill[i].SetActive(false);
					_txtSkillNone[i].gameObject.SetActive(true);
				}
			}
			else
			{
				ForgeInfo smithingForgeInfo = ForgeInfoMgr.Instance.GetList () [(equipItem.u1SmithingLevel - 1)];
				Byte skillCount = (Byte)(smithingForgeInfo.cSmithingInfo.u1RandomSkillCount + smithingForgeInfo.cSmithingInfo.u1SelectSkillCount);
				if(skillCount != 0)
				{
					for (Byte i=0; i<skillCount; i++) {
						SkillInfo skillInfo = SkillInfoMgr.Instance.GetInfoBySlot (equipItem.GetEquipmentInfo ().u2ClassID, equipItem.skillSlots [i]);
						_txtSkillName[i].text = TextManager.Instance.GetText( skillInfo.sName );
						_imgSkillIcon [i].sprite = AtlasMgr.Instance.GetSprite (String.Format ("Sprites/Skill/Atlas_SkillIcon_{0}.{1}", equipItem.GetEquipmentInfo ().u2ClassID, skillInfo.u2ID));
                        tempString.Remove(0, tempString.Length);
                        _imgSkillElement[i].sprite = AtlasMgr.Instance.GetSprite(tempString.Append("Sprites/Common/common_02_renew.common_02_skill_element_").Append(skillInfo.u1Element).ToString());
                        tempString.Remove(0, tempString.Length);
						_txtSkillValue[i].text = tempString.Append("+ ").Append(equipItem.statusComponent.points[i]).ToString();//equipItem.skillSlots[i].ToString();
						_btnSkillInfo[i].SetData(skillInfo);
						_objSkill [i].SetActive (true);
					}
					for (Byte i=skillCount; i<EquipmentInfo.ADD_SKILL_MAX; i++) {
						_objSkill [i].SetActive (false);
					}
					for (Byte i=0; i<EquipmentInfo.ADD_SKILL_MAX; i++) {
						_txtSkillNone[i].gameObject.SetActive(false);
					}
				}
				else
				{
					for (Byte i=0; i<EquipmentInfo.ADD_SKILL_MAX; i++) {
						_objSkill[i].SetActive(false);
						_txtSkillNone[i].gameObject.SetActive(true);
					}
				}
			}

		}

		_txtEquipNameInChangeNamePanel.color = EquipmentItem.equipElementColors[(equipItem.GetEquipmentInfo().u1Element)];
		_txtEquipNameInChangeNamePanel.text = TextManager.Instance.GetText( _cEquipItem.GetEquipmentInfo().sName );
		_txtInputName.text = "";
		if(equipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 && equipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
		{
			_trEquipModelParent.gameObject.SetActive(true);
			equipItem.InitViewModelObject();
			equipItem.cObject.GetComponent<EquipmentObject>().SetLayer( LayerMask.NameToLayer("UI") );
			equipItem.cObject.transform.SetParent( _trEquipModelParent );
			_trEquipModelParent.GetComponent<RotateCharacter>().characterTransform = equipItem.cObject.transform;
			equipItem.cObject.transform.localScale = Vector3.one;
			equipItem.cObject.transform.localPosition = new Vector3(0f, -150f, 0f);
			_objEquipModel = equipItem.cObject;
			_imgAccessory.gameObject.SetActive(false);
			_imgAccessoryUI.gameObject.SetActive(false);
		}
		else
		{
			_trEquipModelParent.gameObject.SetActive(false);
            tempString.Remove(0, tempString.Length);
            _imgAccessory.sprite = AssetMgr.Instance.AssetLoad(tempString.Append("Sprites/Item/Accessory/acc_").Append(equipItem.GetEquipmentInfo().u2ModelID).Append(".png").ToString(), typeof(Sprite)) as Sprite;
			//_imgAccessory.SetNativeSize();
			_imgAccessory.gameObject.SetActive(true);
			_imgAccessoryUI.gameObject.SetActive(false);
		}

		if(_objEquipEffect != null) DestroyObject(_objEquipEffect);
		if(forgeLevel >= 1)
		{
			DebugMgr.Log("Asset : " +string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}", forgeLevel));
			_objEquipEffect = Instantiate(AssetMgr.Instance.AssetLoad(string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}.prefab", forgeLevel), typeof(GameObject))) as GameObject;
			_objEquipEffect.transform.SetParent(_trEquipEffectParent);
			_objEquipEffect.transform.name = "WeaponEffect";
			_objEquipEffect.transform.localScale = Vector3.one;
			_objEquipEffect.transform.localPosition = Vector3.zero;
		}

        if (Legion.Instance.sName != "")
        {
            tempString.Remove(0, tempString.Length);
            _txtCreator.text = tempString.Append("By ").Append(Legion.Instance.sName).ToString();
        }
        else
            _txtCreator.text = "";
	}

	public void OnClickChangeName()
	{
		_panel.SetActive(false);
		_panelChangeName.SetActive(true);
		PopupManager.Instance.AddPopup(_panelChangeName, Close_ChangeName);
		_txtEquipNameInChangeNamePanel.text = TextManager.Instance.GetText( _cEquipItem.GetEquipmentInfo().sName );
		_txtInputName.text = _cEquipItem.itemName;
		_inputFieldEquipName.text = _cEquipItem.itemName;
		if(_cEquipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 && _cEquipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
			_cEquipItem.cObject.SetActive(false);
		if(_objEquipEffect != null)
			_objEquipEffect.SetActive(false);
	}

	string _sName="";
    public void SetEquipmentName()
    {
        _sName = _inputFieldEquipName.text;
    }
	public void OnClickChangeNameConfirm()
	{
		StringBuilder tempString = new StringBuilder();		//if(Regex.Matches(_inputFieldEquipName.text, @"[\s\Wㄱ-ㅎㅏ-ㅣ]").Count != 0)
        //bool bTempMatch = false;
        //bTempMatch = Regex.IsMatch(_sName, @"[\s\Wㄱ-ㅎㅏ-ㅣ]");
        int tempInt = 0;
        var temp = Regex.Matches(_sName, @"[\s\Wㄱ-ㅎㅏ-ㅣ]");
        tempInt = (temp.Count);
        if(tempInt != 0)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_equip_change_name"), TextManager.Instance.GetErrorText("CREATE_ACCOUNT_ID_WRONGCHAR", "", false), null);
			return;
		}
        else
        {
            for(int i=0; i<_inputFieldEquipName.text.Length; i++)
            {
                if(_inputFieldEquipName.text.Substring(i, 1).Equals(" "))
                {
                    tempStringBuilder.Remove(0, tempStringBuilder.Length);
					tempStringBuilder.Append(TextManager.Instance.GetErrorText("crew_space_name","", false));
                    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_warnning"), tempStringBuilder.ToString(), null);
                    return;
                }
            }
        }		_cEquipItem.itemName = _txtInputName.text;
        _txtEquipName.text = tempString.Append(_cEquipItem.itemName).Append(" ").Append(TextManager.Instance.GetText( _cEquipItem.GetEquipmentInfo().sName )).ToString();
		//UIManager.Instance.SetSizeTextGroup(_trNameGroup, 18);
		_panel.SetActive(true);
		_panelChangeName.SetActive(false);
		if(_cEquipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 && _cEquipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
			_cEquipItem.cObject.SetActive(true);
		if(_objEquipEffect != null)
			_objEquipEffect.SetActive(true);
        PopupManager.Instance.RemovePopup(_panelChangeName);
	}

	public void Close_ChangeName()
	{
		//_cEquipItem.itemName = _txtInputName.text;
        _txtInputName.text = "";
        _inputFieldEquipName.text = "";
        StringBuilder tempString = new StringBuilder();
		_txtEquipName.text = tempString.Append(_cEquipItem.itemName).Append(" ").Append(TextManager.Instance.GetText( _cEquipItem.GetEquipmentInfo().sName )).ToString();
		_panel.SetActive(true);
		_panelChangeName.SetActive(false);
		_cEquipItem.cObject.SetActive(true);
		if(_objEquipEffect != null)
			_objEquipEffect.SetActive(true);

		PopupManager.Instance.RemovePopup(_panelChangeName);
	}

	UInt16 seqNo = 0;
	public void OnClickConfirm()
	{
		if(_txtInputName.text != "")
		{
			PopupManager.Instance.ShowLoadingPopup(1);
			seqNo = Server.ServerMgr.Instance.ChangeEquipName(_cEquipItem, _cEquipItem.itemName, ResponseChangeEquipName);
		}
		else
		{
			PopupManager.Instance.RemovePopup(gameObject);
            _cParent._cParent2.bAttach = true;
            _cParent._cParent2.OnClickClose();
            //gameObject.SetActive(false);
		}
//		UInt16[] checkParam = new UInt16[1];
//		checkParam[0] = _cEquipItem.GetEquipmentInfo().u2ID;
//		seqNo = Server.ServerMgr.Instance.CheckDesign(1, checkParam, emptyResponse);
		//_cParent.ShowModelObject(true);
  	}
    public void OnClickReForged()
    {
        gameObject.SetActive(false);
        //_cParent.ShowModelObject(true);
        _cParent.CheckReforged = true;
        _cParent.Show_ConfirmPopup();
        _cParent.OnClickConfirm();
    }
	void emptyResponse(Server.ERROR_ID err )
	{
		PopupManager.Instance.CloseLoadingPopup();
	}
	public void ResponseChangeEquipName(Server.ERROR_ID err)
	{
		if(err != Server.ERROR_ID.NONE)
		{
			DebugMgr.Log("SmithingFailed");
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.FORGE_NAME, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
			DebugMgr.Log("SmithingSuccess");
			PopupManager.Instance.CloseLoadingPopup();
			_cEquipItem.itemName = _txtInputName.text;
            _cParent.ShowModelObject(true);
			PopupManager.Instance.RemovePopup(gameObject);
            _cParent._cParent2.bAttach = true;
            _cParent._cParent2.OnClickClose();
            //gameObject.SetActive(false);
		}
	}

    public void OnClickAttach()
    {
        EquipmentItem oldItem = _cHero.acEquips[(int)(_cEquipItem.GetEquipmentInfo().u1PosID)-1];
        _cHero.ChangeWear ((UInt16)_cEquipItem.GetEquipmentInfo().u1PosID, _cEquipItem.u2SlotNum);
		_cHero.cObject.GetComponent<HeroObject> ().ChangeEquip (oldItem, _cEquipItem);
		_cHero.cObject.GetComponent<HeroObject> ().SetAnimations_UI();
        Legion.Instance.cInventory.dicInventory[_cEquipItem.u2SlotNum].isNew = false;
        _cCharacterInfoPanel.GetPanelEquipment().SelectPosIdx = ((int)(_cEquipItem.GetEquipmentInfo().u1PosID)-1);
        _cCharacterInfoPanel.GetPanelEquipment().OnClickAttach();
        //_cCharacterInfoPanel.GetPanelEquipment().GetPanelItemList.OnClickClose();
        OnClickConfirm();
    }

	void OnDisable()
	{
		Destroy(_objEquipModel);
		if(_objEquipEffect != null) DestroyObject(_objEquipEffect);
	}
}
