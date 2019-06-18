using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class UI_Panel_Forge_Fusion_Module_Result : MonoBehaviour
{
    private const int UNLOCK_LV = 10;
	// [SerializeField] GameObject _panel;
	[SerializeField] Image _imgElementIcon;
	[SerializeField] RectTransform _trNameGroup;
    [SerializeField] RectTransform _trStarGroup;
	[SerializeField] Text _txtTier;
	[SerializeField] Text _txtEquipName;
	[SerializeField] Image _imgElement;
	[SerializeField] Text _txtLevel;
	[SerializeField] Text _txtExp;
    [SerializeField] Text _txtStatPoint;
    [SerializeField] Text _txtStatPointProgress;
    [SerializeField] GameObject _effStatPoint;
	[SerializeField] Image _imgExpGauge;
	[SerializeField] Text _txtCreator;
	[SerializeField] Text[] _txtStatType;
	[SerializeField] Text[] _txtStatValue;
	[SerializeField] UI_Button_CharacterInfo_Equipment_StatInfo[] _btnStatInfo;
	[SerializeField] Image[] _imgEffects;
    [SerializeField] GameObject[] Btn_StatUp;
    [SerializeField] GameObject[] Btn_StatUp10;
    [SerializeField] GameObject[] Btn_SkillUp;
	[SerializeField] GameObject[] _objSkill;
	[SerializeField] GameObject[] _objNoneSkill;
	[SerializeField] Image[] _imgSkillIcon;
	[SerializeField] Image[] _imgSkillElement;
	[SerializeField] Text[] _txtSkillValue;
	[SerializeField] UI_Button_CharacterInfo_Equipment_SkillInfo[] _btnSkillInfo;
	[SerializeField] GameObject remainEffect;
	[SerializeField] Button _btnAuto;
	[SerializeField] Text _txtAuto;
	[SerializeField] Button _btnReset;
	[SerializeField] Text _txtReset;
	private UI_PointBuyPopup pointBuyPopup;
	private GameObject pointingEffect;

	[SerializeField] Transform _trEquipModelParent;
	[SerializeField] SpriteRenderer _imgAccessory;
	[SerializeField] Image _imgAccessoryUI;
	[SerializeField] Transform _trEquipEffectParent;
    [SerializeField] UI_Button_CharacterInfo_Equipment_StateInfo _specializeBtn;
    [SerializeField] Text _equipmentClassName;
    [SerializeField] Image _imgClassIcon;
    //[SerializeField] GameObject Pref_Star;
    [SerializeField] GameObject starPos;

	GameObject _objEquipModel;
	GameObject _objEquipEffect;
    UI_Panel_Forge_Fusion_Module _cParent;
	EquipmentItem _cEquipItem;
	private Byte[] statType;
	private UInt32[] tempStatus;
	int statPoint;
	int statPointOrigin; 
    private GameObject resetPopup;
    private List<GameObject> listEffects = new List<GameObject>();
    StringBuilder tempStringBuilder;

    void Start()
    {
        for(int i=0; i<_imgEffects.Length; i++)
            LeanTween.rotate(_imgEffects[i].GetComponent<RectTransform>(), -360f, 1f).setLoopType(LeanTweenType.easeInElastic).setLoopCount(0);
    }
    
	public void SetData(EquipmentItem equipItem, UI_Panel_Forge_Fusion_Module _parent)
	{
        tempStringBuilder = new StringBuilder();
		_imgElementIcon.sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.Append("Sprites/Common/common_02_renew.element_icon_").Append(equipItem.GetEquipmentInfo().u1Element).ToString());
		_imgElementIcon.SetNativeSize();
		if(pointingEffect == null)
			pointingEffect = AssetMgr.Instance.AssetLoad("Prefabs/UI_Effects/StatusPoint02.prefab", typeof(GameObject)) as GameObject;

		_cEquipItem = equipItem;
        _cParent = _parent;

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        _imgElement.sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.Append("Sprites/Common/common_02_renew.element_").Append(_cEquipItem.GetEquipmentInfo().u1Element).ToString());
		tempStatus = _cEquipItem.GetPoints();
		statType = new Byte[Server.ConstDef.EquipStatPointType];
		_cEquipItem.GetComponent<StatusComponent>().CountingStatPointEquip(_cEquipItem.cLevel.u2Level);
        //statPoint = (int)_cEquipItem.GetComponent<StatusComponent>().CountingStatPointEquip();
        statPoint = (int)_cEquipItem.GetComponent<StatusComponent>().UNSET_STATPOINT;
		statPointOrigin = statPoint;
        _specializeBtn.SetData((Byte)(_cEquipItem.GetEquipmentInfo().u1Specialize+2));
        Color tempColor;
        ColorUtility.TryParseHtmlString(_cEquipItem.GetEquipmentInfo().GetHexColor((Byte)(_cEquipItem.GetEquipmentInfo().u1Specialize+2)), out tempColor);
        _specializeBtn.transform.parent.GetComponent<Image>().color = tempColor;
        if(_cEquipItem.GetEquipmentInfo().u2ClassID <= ClassInfo.LAST_CLASS_ID)
        {
            _equipmentClassName.text = TextManager.Instance.GetText(ClassInfoMgr.Instance.GetInfo(_cEquipItem.GetEquipmentInfo().u2ClassID).sName);

            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            _imgClassIcon.sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.Append("Sprites/Common/common_class.common_class_").Append(_cEquipItem.GetEquipmentInfo().u2ClassID).ToString());
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

        //_txtTier.text = "<" + TextManager.Instance.GetText("forge_level_" + _cEquipItem.u1SmithingLevel) + ">";
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        _txtTier.text = TextManager.Instance.GetText(tempStringBuilder.Append("forge_level_").Append(_cEquipItem.u1SmithingLevel).ToString());
		UIManager.Instance.SetGradientFromTier(_txtTier.GetComponent<Gradient>(), _cEquipItem.u1SmithingLevel);
        //string equipName = "";
        //equipName = _cEquipItem.itemName;
        //if(equipName != "")
        //	equipName = equipName + " " + TextManager.Instance.GetText( _cEquipItem.GetEquipmentInfo().sName );
        //else
        //	equipName = TextManager.Instance.GetText( _cEquipItem.GetEquipmentInfo().sName );
        //_txtEquipName.text = equipName;
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        if (_cEquipItem.itemName != "")
            tempStringBuilder.Append(_cEquipItem.itemName).Append(" ").Append(TextManager.Instance.GetText(_cEquipItem.GetEquipmentInfo().sName));
        else
            tempStringBuilder.Append(TextManager.Instance.GetText(_cEquipItem.GetEquipmentInfo().sName));

        _txtEquipName.text = tempStringBuilder.ToString();

        _txtEquipName.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
		UIManager.Instance.SetGradientFromElement(_txtEquipName.GetComponent<Gradient>(), _cEquipItem.GetEquipmentInfo().u1Element);
		//UIManager.Instance.SetSizeTextGroup(_trNameGroup, 18);
        UIManager.Instance.SetSizeTextGroup(_trStarGroup, 18);

		_txtLevel.text = _cEquipItem.cLevel.u2Level.ToString();
		_txtExp.text = string.Format("{0}/{1}", ConvertExpValue(_cEquipItem.cLevel.u8Exp), ConvertExpValue(_cEquipItem.cLevel.u8NextExp));
		_imgExpGauge.fillAmount = (float)((float)_cEquipItem.cLevel.u8Exp / (float)_cEquipItem.cLevel.u8NextExp);
		if(_cEquipItem.createrName != "")
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            _txtCreator.text = tempStringBuilder.Append("By ").Append( _cEquipItem.createrName).ToString();
        }
		else
			_txtCreator.text = "";
		for(int i=0; i<Server.ConstDef.EquipStatPointType; i++)
		{
			_txtStatType[i].text = Status.GetStatText( _cEquipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType );
			_txtStatValue[i].text = _cEquipItem.cStatus.GetStat(_cEquipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType, _cEquipItem.GetPoints()[i + Server.ConstDef.SkillOfEquip]).ToString();
			_btnStatInfo[i].SetData(_cEquipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType);
		}

		if (_cEquipItem.u1SmithingLevel != 0) {
			ForgeInfo smithingForgeInfo = ForgeInfoMgr.Instance.GetList () [(_cEquipItem.u1SmithingLevel - 1)];
			Byte skillCount = (Byte)(smithingForgeInfo.cSmithingInfo.u1RandomSkillCount + smithingForgeInfo.cSmithingInfo.u1SelectSkillCount);
			for (Byte i=0; i<skillCount; i++) {
				if(_cEquipItem.skillSlots [i] == 0)
				{
					_objSkill [i].SetActive (false);
					_objNoneSkill[i].SetActive(true);
					continue;
				}
				SkillInfo skillInfo = SkillInfoMgr.Instance.GetInfoBySlot (_cEquipItem.GetEquipmentInfo ().u2ClassID, _cEquipItem.skillSlots [i]);
				_imgSkillIcon [i].sprite = AtlasMgr.Instance.GetSprite (String.Format ("Sprites/Skill/Atlas_SkillIcon_{0}.{1}", _cEquipItem.GetEquipmentInfo ().u2ClassID, skillInfo.u2ID));
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                _imgSkillElement[i].sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.Append("Sprites/Common/common_02_renew.common_02_skill_element_").Append(skillInfo.u1Element).ToString());
				_objSkill [i].SetActive (true);
				_objNoneSkill[i].SetActive(false);
				_btnSkillInfo[i].SetData(skillInfo);
			}
			for (Byte i=skillCount; i<EquipmentInfo.ADD_SKILL_MAX; i++) {
				_objSkill [i].SetActive (false);
				_objNoneSkill[i].SetActive(true);
			}
		}
		for(int i=0; i<_trEquipModelParent.childCount; i++)
		{
			if(_trEquipModelParent.GetChild(i) != null)
				DestroyObject(_trEquipModelParent.GetChild(i).gameObject);
		}

		if(_cEquipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 && _cEquipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
		{
            //if(_cEquipItem.cObject == null)
			    _cEquipItem.InitViewModelObject();
			_cEquipItem.cObject.GetComponent<EquipmentObject>().SetLayer( LayerMask.NameToLayer("UI") );
			_cEquipItem.cObject.transform.SetParent( _trEquipModelParent );
			_trEquipModelParent.GetComponent<RotateCharacter>().characterTransform = _cEquipItem.cObject.transform;
			_cEquipItem.cObject.transform.localScale = Vector3.one;
			_cEquipItem.cObject.transform.localPosition = new Vector3(0f, -150f, 0f);
			_cEquipItem.cObject.transform.name = "EquipmentObject";
			_objEquipModel = _cEquipItem.cObject;
			_imgAccessory.gameObject.SetActive(false);
			_imgAccessoryUI.gameObject.SetActive(false);
		}
		else
		{
            tempStringBuilder.Remove(0, tempStringBuilder.Length);

            _imgAccessory.sprite = AssetMgr.Instance.AssetLoad(tempStringBuilder.Append("Sprites/Item/Accessory/acc_").Append(_cEquipItem.u2ModelID).Append(".png").ToString(), typeof(Sprite)) as Sprite;
			_imgAccessory.gameObject.SetActive(true);
			_imgAccessoryUI.gameObject.SetActive(false);
		}
		if(_objEquipEffect != null) DestroyObject(_objEquipEffect);
		if(_cEquipItem.u1SmithingLevel >= 1)
		{
			_objEquipEffect = Instantiate(AssetMgr.Instance.AssetLoad(string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}.prefab", _cEquipItem.u1SmithingLevel), typeof(GameObject))) as GameObject;
			_objEquipEffect.transform.SetParent(_trEquipEffectParent);
			_objEquipEffect.transform.name = "WeaponEffect";
			_objEquipEffect.transform.localScale = new Vector3(1f, 1f, 1f);
			_objEquipEffect.transform.localPosition = Vector3.zero;
			listEffects.Add(_objEquipEffect);
		}   
        _txtStatPoint.text = _cEquipItem.GetComponent<StatusComponent>().UNSET_STATPOINT.ToString();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append("(");
        tempStringBuilder.Append((_cEquipItem.statusComponent.STATPOINT_EXP/StatusComponent.MAX_STATEXP_PROGRESS).ToString());
        tempStringBuilder.Append(".");
        if((_cEquipItem.statusComponent.STATPOINT_EXP%StatusComponent.MAX_STATEXP_PROGRESS) >= 10)
            tempStringBuilder.Append((_cEquipItem.statusComponent.STATPOINT_EXP%StatusComponent.MAX_STATEXP_PROGRESS).ToString().Remove(1));
        else
            tempStringBuilder.Append((_cEquipItem.statusComponent.STATPOINT_EXP%StatusComponent.MAX_STATEXP_PROGRESS).ToString());
        tempStringBuilder.Append("%)");
        _txtStatPointProgress.text = tempStringBuilder.ToString();
        if(equipItem.statusComponent.STAT_POINT != 0)
            _effStatPoint.SetActive(true);
        else
            _effStatPoint.SetActive(false);
		SetStatButtons();
	}

	//index : 3,4,5
	public void OnClickStatUp(int index)
	{
		int needPoint = EquipmentInfoMgr.Instance.statPointPerLevel;

		if(statPoint < needPoint)
			return;
        
        statPoint -= needPoint;
		_txtStatPoint.text = statPoint.ToString();
        tempStatus[index] += (UInt16)EquipmentInfoMgr.Instance.statPointPerLevel;
		_txtStatValue[index-Server.ConstDef.SkillOfEquip].text = _cEquipItem.cStatus.GetStat(statType[index-Server.ConstDef.SkillOfEquip], tempStatus[index]).ToString();
        
		ShowEffect(_txtStatValue[index-Server.ConstDef.SkillOfEquip].transform.parent.GetComponent<RectTransform>());
        
        SetStatButtons();
	}
    //index : 3,4,5
	public void OnClickStatUp10(int index)
	{
		int needPoint = EquipmentInfoMgr.Instance.statPointPerLevel;

		if(statPoint < needPoint)
			return;
        
        statPoint -= needPoint*10;
		_txtStatPoint.text = statPoint.ToString();
        tempStatus[index] += (UInt16)(EquipmentInfoMgr.Instance.statPointPerLevel*10);
		_txtStatValue[index-Server.ConstDef.SkillOfEquip].text = _cEquipItem.cStatus.GetStat(statType[index-Server.ConstDef.SkillOfEquip], tempStatus[index]).ToString();
        
		ShowEffect(_txtStatValue[index-Server.ConstDef.SkillOfEquip].transform.parent.GetComponent<RectTransform>());
        
        SetStatButtons();
	}
	//index 0, 1, 2
	public void OnClickSkillUp(int index)
	{
		int needPoint = EquipmentInfoMgr.Instance.skillPointPerLevel;

		if(statPoint < needPoint)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_desc_no_pt"), string.Format(TextManager.Instance.GetText("popup_desc_stat_pt_lack"), needPoint), null);
			return;
		}        

        statPoint -= needPoint;            
		_txtStatPoint.text = statPoint.ToString();
        tempStatus[index] += 1;
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        _txtSkillValue[index].text = tempStringBuilder.Append("+ ").Append(tempStatus[index]).ToString();
                        
        ShowEffect(_objSkill[index].GetComponent<RectTransform>()); 
        
        SetStatButtons();
	}
	private void AckStatUp(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();
		
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EQUIP_STAT_POINT, err), Server.ServerMgr.Instance.CallClear);
			statPoint = statPointOrigin;
            tempStatus = _cEquipItem.GetPoints();
			_txtStatPoint.text = statPoint.ToString();
			SetStatButtons();
		}
		else
		{
			_cEquipItem.GetComponent<StatusComponent>().points = tempStatus;
			_cEquipItem.GetComponent<StatusComponent>().CountingStatPointEquip(_cEquipItem.cLevel.u2Level);
			_cEquipItem.GetComponent<StatusComponent>().SetByLevelEquip(_cEquipItem.cLevel);
			            
			//statPoint = _cEquipItem.u1StatPoint;
            //statPoint = _cEquipItem.statusComponent.UNSET_STATPOINT;
            
			Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.EquipStatPoint, 0, 0, 0, 0, (uint)(statPointOrigin - statPoint));
            
			statPointOrigin = statPoint;
			_txtStatPoint.text = statPoint.ToString();
			_cEquipItem.statusComponent.UNSET_STATPOINT = (UInt16)statPoint;
			SetStatButtons();    

            gameObject.SetActive(false);
			PopupManager.Instance.RemovePopup(gameObject);
            RemoveEffects();
            PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.EquipStatPoint);
        }
	}

	public void SetStatButtons()
	{
		_txtStatPoint.text = statPoint.ToString();
		
		//스탯 포인트 없으면 찍기 비활성화
		if(statPoint == 0)
		{
			remainEffect.SetActive(false);
			for(int i=0; i<Server.ConstDef.SkillOfEquip; i++)
			{
                Btn_StatUp[i].SetActive(false);
                Btn_SkillUp[i].SetActive(false);
                Btn_StatUp10[i].SetActive(false);

				//스킬이 없을경우
				if(_cEquipItem.skillSlots[i] == 0)
				{
					_objSkill[i].gameObject.SetActive(false);
					_objNoneSkill[i].SetActive(true);
				}
				//있을경우
				else
				{			
					_objSkill[i].gameObject.SetActive(true);
					_objNoneSkill[i].SetActive(false);
				}
			}
			
			_btnAuto.interactable = false;
			_txtAuto.color = Color.gray;
		}
		else
		{
			remainEffect.SetActive(true);
			for(int i=0; i<Server.ConstDef.SkillOfEquip; i++)
			{
                Btn_StatUp[i].SetActive(true);
                Btn_SkillUp[i].SetActive(true);
                if(statPoint > 19)
                    Btn_StatUp10[i].SetActive(true);
                else
                    Btn_StatUp10[i].SetActive(false);

				if(_cEquipItem.skillSlots[i] == 0)
				{
					_objSkill[i].gameObject.SetActive(false);
					_objNoneSkill[i].SetActive(true);
				}
				else
				{
					_objSkill[i].gameObject.SetActive(true);
					_objNoneSkill[i].SetActive(false);
				}
			}
		}
		
		statType = new Byte[Server.ConstDef.EquipStatPointType];
		for(int i=0; i<statType.Length; i++)
		{
			statType[i] = _cEquipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType;
			_txtStatType[i].text = Status.GetStatText(statType[i]);
			_txtStatValue[i].text = _cEquipItem.cStatus.GetStat(statType[i], tempStatus[i + Server.ConstDef.SkillOfEquip]).ToString();
		}
		
		for(int i=0; i<Server.ConstDef.SkillOfEquip; i++)
		{
			if(_cEquipItem.skillSlots[i] == 0)
			{
				//	skillNames[i].text = "NO SKILLS";
				//	skillTexts[i].text = "";
			}
			else
			{
				SkillInfo skillInfo = SkillInfoMgr.Instance.GetInfoBySlot(_cEquipItem.GetEquipmentInfo().u2ClassID, _cEquipItem.skillSlots[i]);
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                _imgSkillIcon[i].sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.Append("Sprites/Skill/Atlas_SkillIcon_").Append(_cEquipItem.GetEquipmentInfo().u2ClassID).Append(".").Append(skillInfo.u2ID).ToString());
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                _imgSkillElement[i].sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.Append("Sprites/Common/common_02_renew.common_02_skill_element_").Append(skillInfo.u1Element).ToString());
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                _txtSkillValue[i].text = tempStringBuilder.Append("+ ").Append(tempStatus[i]).ToString();
			}
		}
	}

	void NoEvent_In_AddPointPopup(object[] param)
	{
		_objEquipModel.SetActive(true);
		_objEquipEffect.SetActive(true);
		if(pointBuyPopup != null)
			DestroyObject(pointBuyPopup.gameObject);
	}

	private void ShowEffect(RectTransform rect)
	{
		GameObject instEffect = Instantiate(pointingEffect);
		instEffect.transform.SetParent(transform);
		instEffect.transform.localPosition = rect.anchoredPosition3D + Vector3.down * 20f;
		instEffect.transform.localScale = Vector3.one;
        
        listEffects.Add(instEffect);
	}
	public void OnClickAuto()
	{
		int index = Server.ConstDef.SkillOfEquip;
		
		Dictionary<int, int> upIndex = new Dictionary<int, int>();
		
		while(statPoint > 0)
		{
			if(index < Server.ConstDef.SkillOfEquip)
			{
				if(_cEquipItem.skillSlots[index] != 0 && statPoint > EquipmentInfoMgr.Instance.skillPointPerLevel)
				{
					//int unlockSlot = 1 + (_cEquipItem.cLevel.u2Level / UNLOCK_LV);
					
					// if(unlockSlot > index)
					// {
						tempStatus[index] += 1;
						statPoint -= EquipmentInfoMgr.Instance.skillPointPerLevel;
                        
                        if(upIndex.ContainsKey(index) == false)
                        {
                            upIndex.Add(index, index);
                            ShowEffect(_objSkill[index].GetComponent<RectTransform>());
                        }                             
					// }
				}
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                _txtSkillValue[index].text = tempStringBuilder.Append("+ ").Append(tempStatus[index]).ToString();
			}
			else
			{
				tempStatus[index] += (UInt16)EquipmentInfoMgr.Instance.statPointPerLevel;
				statPoint -= EquipmentInfoMgr.Instance.statPointPerLevel;
				_txtStatValue[index-Server.ConstDef.SkillOfEquip].text = _cEquipItem.cStatus.GetStat(statType[index-Server.ConstDef.SkillOfEquip], tempStatus[index]).ToString();
				
				if(upIndex.ContainsKey(index) == false)
				{
					upIndex.Add(index, index);
					ShowEffect(_txtStatValue[index-Server.ConstDef.SkillOfEquip].transform.parent.GetComponent<RectTransform>());
				}    
			}
			
			index++;
			
			if(index >= tempStatus.Length)
				index = 0;
			
			_txtStatPoint.text = statPoint.ToString();
		}

        SetStatButtons();      
	}
    
    public void OnClickClose()
    {
		if(statPoint < statPointOrigin)
		{
			PopupManager.Instance.ShowLoadingPopup(1);

			UInt16[] tmp = new UInt16[tempStatus.Length];

			for(int i=0; i<tempStatus.Length; i++){
				tmp[i] = (UInt16)tempStatus[i];
			}

			Server.ServerMgr.Instance.PointEquipmentStatus(_cEquipItem, tmp, AckStatUp);
		}
		else
        {
		    gameObject.SetActive(false);
			PopupManager.Instance.RemovePopup(gameObject);
            RemoveEffects();
        }        
    }
    
    private void RemoveEffects()
    {
        for(int i=0; i<listEffects.Count; i++)
        {
            if(listEffects[i] != null)
                Destroy(listEffects[i]);
        }
        
        listEffects.Clear();
        _cParent.OnClickClose();
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
