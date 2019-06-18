using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class UI_SubPanel_CharacterInfo_Equipment_ItemInfo : MonoBehaviour {
	//public Color32 disableColorStart = new Color32(190,190,190,255);
	//public Color32 disableColorEnd = new Color32(102,102,102,255);
	//public Color32 enableColorStart = Color.white;
	//public Color32 enableColorEnd = new Color32(137, 163, 180, 255);
	private const int UNLOCK_LV = 10;
	[SerializeField] Transform _trPanel;
	[SerializeField] Image _imgElementIcon;
	[SerializeField] RectTransform _trNameGroup;
    [SerializeField] RectTransform _trStarGroup;
	[SerializeField] Text _txtTier;
	[SerializeField] Text _txtEquipName;
	[SerializeField] Text _txtCreator;
	[SerializeField] Image _imgElement;
	[SerializeField] Text _txtLevel;
    [SerializeField] Text _txtLevelMax;
	[SerializeField] Image _imgExpGauge;
	[SerializeField] Text _txtExp;
	[SerializeField] Text[] _txtStatType;
	[SerializeField] Text[] _txtStatValue;
	[SerializeField] UI_Button_CharacterInfo_Equipment_StatInfo[] _btnStatInfo;
	[SerializeField] Image[] _imgStatUpIcon;
	[SerializeField] Image[] _imgEffects;
	[SerializeField] Text _txtStatPoint;
    [SerializeField] Text _txtStatPointProgress;
	[SerializeField] GameObject remainEffect;
	[SerializeField] GameObject[] _objSkill;
	[SerializeField] GameObject[] _objNoneSkill;
	[SerializeField] Image[] _imgSkillIcon;
	[SerializeField] Image[] _imgSkillElement;
	[SerializeField] Text[] _txtSkillValue;
	[SerializeField] UI_Button_CharacterInfo_Equipment_SkillInfo[] _btnSkillInfo;
	private GameObject pointingEffect;
	[SerializeField] Sprite _imgEnableButton;
	[SerializeField] Sprite _imgDisableButton;
	[SerializeField] Button _btnAuto;
//	[SerializeField] Text _txtAuto;
	[SerializeField] Button _btnReset;
//	[SerializeField] Text _txtReset;
	private UI_PointBuyPopup pointBuyPopup;
	Transform equipTr;
	[SerializeField] RectTransform _trModelObjParent;
	public SpriteRenderer _imgAccessory;
	public Image _imgAccessoryUI;
	[SerializeField] RectTransform _trModelEffParent;

	public GameObject _objEquipEffect;

    [SerializeField] UI_Button_CharacterInfo_Equipment_StateInfo _specializeBtn;
    [SerializeField] Text _equipmentClassName;
    [SerializeField] Image _imgClassIcon;
    //[SerializeField] GameObject Pref_Star;
    [SerializeField] GameObject starPos;
    [SerializeField] Toggle tglComapre;
	UI_SubPanel_CharacterInfo_Equipment_ItemList _cItemlist;
    String CompareStatColor16 = "#1CFF5FFF";
    String CurrentStatColor16 = "#EBC378FF";

    public Color StatUpColor = new Color32(0, 255, 255, 255);
    public Color StatDownColor = new Color32(255, 50, 0, 255);
    public Color StatDefaultColor = new Color32(235, 195, 120, 255);
    
    public Toggle GetToggleCompare
    {
        get
        {
            return tglComapre;
        }
    }

	Hero _cHero;
	EquipmentItem _cBaseEquipItem;
    EquipmentItem _cDummyBaseEquipItem;
	EquipmentItem _cEquipItem;
    EquipmentItem _cDummyEquipItem;
    StringBuilder tempStringBuilder;

	private Byte[] statType;
	private UInt32[] tempStatus;
    private GameObject resetPopup;
	int statPoint;
	int statPointOrigin;

	public void SetData(Hero selectedHero, EquipmentItem diffEquipItem, UI_SubPanel_CharacterInfo_Equipment_ItemList list)
	{
		if(pointingEffect == null)
			pointingEffect = AssetMgr.Instance.AssetLoad("Prefabs/UI_Effects/StatusPoint02.prefab", typeof(GameObject)) as GameObject;
        tempStringBuilder = new StringBuilder();
		_cHero = selectedHero;
		_cEquipItem = diffEquipItem;
		_cBaseEquipItem = selectedHero.acEquips[(int)(diffEquipItem.GetEquipmentInfo().u1PosID-1)];
		_cItemlist = list;

		tempStatus = diffEquipItem.GetPoints();
		diffEquipItem.GetComponent<StatusComponent>().CountingStatPointEquip(diffEquipItem.cLevel.u2Level);
        //statPoint = (int)diffEquipItem.GetComponent<StatusComponent>().CountingStatPointEquip();
        statPoint = (int)diffEquipItem.GetComponent<StatusComponent>().UNSET_STATPOINT;
		statPointOrigin = statPoint;
        _specializeBtn.SetData((Byte)(_cEquipItem.GetEquipmentInfo().u1Specialize+2));
        Color tempColor;
        ColorUtility.TryParseHtmlString(_cEquipItem.GetEquipmentInfo().GetHexColor((Byte)(_cEquipItem.GetEquipmentInfo().u1Specialize+2)), out tempColor);
        _specializeBtn.transform.parent.GetComponent<Image>().color = tempColor;
        if(_cEquipItem.GetEquipmentInfo().u2ClassID <= ClassInfo.LAST_CLASS_ID)
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            _equipmentClassName.text = TextManager.Instance.GetText(ClassInfoMgr.Instance.GetInfo(_cEquipItem.GetEquipmentInfo().u2ClassID).sName);
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

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        _imgElementIcon.sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.Append("Sprites/Common/common_02_renew.element_icon_").Append(diffEquipItem.GetEquipmentInfo().u1Element).ToString());
		_imgElementIcon.SetNativeSize();

        //_txtTier.text = "<" + TextManager.Instance.GetText("forge_level_" + diffEquipItem.u1SmithingLevel) + ">";
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        _txtTier.text = TextManager.Instance.GetText(tempStringBuilder.Append("forge_level_").Append(diffEquipItem.u1SmithingLevel).ToString());
		UIManager.Instance.SetGradientFromTier(_txtTier.GetComponent<Gradient>(), diffEquipItem.u1SmithingLevel);
		string equipName = "";
		equipName = diffEquipItem.itemName;

		if(equipName != "")
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            equipName = tempStringBuilder.Append(equipName).Append(" ").Append(TextManager.Instance.GetText(diffEquipItem.GetEquipmentInfo().sName)).ToString();
        }
		else
			equipName = TextManager.Instance.GetText( diffEquipItem.GetEquipmentInfo().sName );
		_txtEquipName.text = equipName;
        _txtEquipName.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
		UIManager.Instance.SetGradientFromElement(_txtEquipName.GetComponent<Gradient>(), diffEquipItem.GetEquipmentInfo().u1Element);
		//UIManager.Instance.SetSizeTextGroup(_trNameGroup, 18);
        UIManager.Instance.SetSizeTextGroup(_trStarGroup, 18);

		if(diffEquipItem.createrName != "")
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            _txtCreator.text = tempStringBuilder.Append("By ").Append(diffEquipItem.createrName).ToString();
        }
		else
			_txtCreator.text = "";

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        _imgElement.sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.Append("Sprites/Common/common_02_renew.element_").Append(diffEquipItem.GetEquipmentInfo().u1Element).ToString());
		_txtLevel.text = diffEquipItem.cLevel.u2Level.ToString();
		_txtExp.text = string.Format("{0}/{1}", ConvertExpValue(diffEquipItem.cLevel.u8Exp), ConvertExpValue(diffEquipItem.cLevel.u8NextExp));

		_imgExpGauge.fillAmount = (float)((float)diffEquipItem.cLevel.u8Exp / (float)diffEquipItem.cLevel.u8NextExp);
		statType = new Byte[Server.ConstDef.EquipStatPointType];

		for(Byte i=0; i<EquipmentInfo.ADD_STAT_TYPE_MAX; i++)
		{
			statType[i] = diffEquipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType;
			_txtStatType[i].text = Status.GetStatText(statType[i]);
			_txtStatValue[i].text = diffEquipItem.cStatus.GetStat(statType[i], tempStatus[i + Server.ConstDef.SkillOfEquip]).ToString();
			_btnStatInfo[i].SetData(statType[i]);
		}

		if (diffEquipItem.u1SmithingLevel != 0)
		{
			ForgeInfo smithingForgeInfo = ForgeInfoMgr.Instance.GetList()[(diffEquipItem.u1SmithingLevel-1)];
			Byte skillCount = (Byte)(smithingForgeInfo.cSmithingInfo.u1RandomSkillCount + smithingForgeInfo.cSmithingInfo.u1SelectSkillCount);
			for(Byte i=0; i<skillCount; i++)
			{
				SkillInfo skillInfo = SkillInfoMgr.Instance.GetInfoBySlot(diffEquipItem.GetEquipmentInfo().u2ClassID, diffEquipItem.skillSlots[i]);
				if(skillInfo != null)
				{
                    _imgSkillIcon[i].sprite = AtlasMgr.Instance.GetSprite(String.Format("Sprites/Skill/Atlas_SkillIcon_{0}.{1}", diffEquipItem.GetEquipmentInfo().u2ClassID, skillInfo.u2ID));
					_objSkill[i].SetActive(true);
					_objNoneSkill[i].SetActive(false);
					_btnSkillInfo[i].SetData(skillInfo);
				}
			}
			for(Byte i=skillCount; i<EquipmentInfo.ADD_SKILL_MAX; i++)
			{
				_objSkill[i].SetActive(false);
				_objNoneSkill[i].SetActive(true);
			}
		}

		for(int i=0; i<transform.childCount; i++)
		{
			if(transform.FindChild("EquipmentObject") != null)
				DestroyObject(transform.FindChild("EquipmentObject").gameObject);
		}

		SetEquipModelObject();

		if(_objEquipEffect != null) DestroyObject(_objEquipEffect);

		if(diffEquipItem.u1SmithingLevel >= 1)
		{
			_objEquipEffect = Instantiate(AssetMgr.Instance.AssetLoad(string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}.prefab", diffEquipItem.u1SmithingLevel), typeof(GameObject))) as GameObject;
			_objEquipEffect.transform.SetParent(_trModelObjParent.parent);
			_objEquipEffect.transform.name = "WeaponEffect";
			_objEquipEffect.transform.localScale = new Vector3(1f, 1f, 1f);
			_objEquipEffect.transform.localPosition = new Vector3(0f, -10f, -50f);
		}

        _txtStatPoint.text = diffEquipItem.GetComponent<StatusComponent>().CountingStatPointEquip().ToString();
        _txtStatPoint.text = diffEquipItem.GetComponent<StatusComponent>().UNSET_STATPOINT.ToString();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append("(");
        tempStringBuilder.Append((_cEquipItem.GetComponent<StatusComponent>().STATPOINT_EXP/StatusComponent.MAX_STATEXP_PROGRESS).ToString());
        tempStringBuilder.Append(".");

        if((_cEquipItem.statusComponent.STATPOINT_EXP%StatusComponent.MAX_STATEXP_PROGRESS) >= 10)
            tempStringBuilder.Append((_cEquipItem.statusComponent.STATPOINT_EXP%StatusComponent.MAX_STATEXP_PROGRESS).ToString().Remove(1));
        else
            tempStringBuilder.Append((_cEquipItem.statusComponent.STATPOINT_EXP%StatusComponent.MAX_STATEXP_PROGRESS).ToString());

        tempStringBuilder.Append("%)");
        _txtStatPointProgress.text = tempStringBuilder.ToString();
		SetStatButtons();
		SetStatusDiff(_cBaseEquipItem, _cEquipItem);
	}

	void SetStatusDiff(EquipmentItem _base, EquipmentItem _dummy)
	{
		int diffValue=0;
		for(int i=0; i<Server.ConstDef.EquipStatPointType; i++) // 현재 장비 스테이터스 3종류와, 교체될 장비 스테이터스 종류가 같은걸 판별해서 증/감 표시
		{
			for(int j=0; j<Server.ConstDef.EquipStatPointType; j++) // i:선택장비 j:현재장비 
			{
				if(_dummy.GetComponent<StatusComponent>().au1StatType[i] == _base.GetComponent<StatusComponent>().au1StatType[j])
				{
					switch(_dummy.GetComponent<StatusComponent>().au1StatType[i])
					{
	                    case 1: diffValue = (int)_dummy.cFinalStatus.GetStat(1) - (int)_base.cFinalStatus.GetStat(1); break;
						case 2: diffValue = _dummy.cFinalStatus.GetStat(2) - _base.cFinalStatus.GetStat(2); break;
						case 3: diffValue = _dummy.cFinalStatus.GetStat(3) - _base.cFinalStatus.GetStat(3); break;
						case 4: diffValue = _dummy.cFinalStatus.GetStat(4) - _base.cFinalStatus.GetStat(4); break;
						case 5: diffValue = _dummy.cFinalStatus.GetStat(5) - _base.cFinalStatus.GetStat(5); break;
						case 6: diffValue = _dummy.cFinalStatus.GetStat(6) - _base.cFinalStatus.GetStat(6); break;
						case 7: diffValue = _dummy.cFinalStatus.GetStat(7) - _base.cFinalStatus.GetStat(7); break;
					}

					if(diffValue < 0)
					{
						_imgStatUpIcon[i].sprite = AtlasMgr.Instance.GetSprite( "Sprites/Common/common_02_renew.arrow_down" );
						_imgStatUpIcon[i].gameObject.SetActive(true);
                        StartCoroutine(StatIconDownAni(i));
                        _txtStatValue[i].color = StatDownColor;
                    }
					else if(diffValue > 0)
					{
						_imgStatUpIcon[i].sprite = AtlasMgr.Instance.GetSprite( "Sprites/Common/common_02_renew.arrow_up" );
						_imgStatUpIcon[i].gameObject.SetActive(true);
                        StartCoroutine(StatIconUpAni(i));
                        _txtStatValue[i].color = StatUpColor;
                    }
                    else
                    {
                        _txtStatValue[i].color = StatDefaultColor;
                    }
					break;

				}
				else
				{
					_imgStatUpIcon[i].gameObject.SetActive(false);
                    _txtStatValue[i].color = StatDefaultColor;
                }
			}
		}
	}

	void SetEquipModelObject()
	{
		if(_cEquipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 &&
		   _cEquipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
		{
			_cEquipItem.InitViewModelObject();
			_imgAccessory.gameObject.SetActive(false);
			_imgAccessoryUI.gameObject.SetActive(false);
			_cEquipItem.cObject.transform.parent = _trModelObjParent;
			_cEquipItem.cObject.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
			_cEquipItem.cObject.transform.localPosition = Vector3.zero;
			_cEquipItem.cObject.GetComponent<EquipmentObject>().SetLayer( LayerMask.NameToLayer("UI") );
		}
		else
		{
			_imgAccessory.gameObject.SetActive(true);
			_imgAccessoryUI.gameObject.SetActive(false);
			UInt16 modelID = _cEquipItem.u2ModelID;
			if(modelID == 0) modelID = _cEquipItem.GetEquipmentInfo().u2ModelID;

            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append("Sprites/Item/Accessory/acc_").Append(modelID).Append(".png");
            _imgAccessory.sprite = AssetMgr.Instance.AssetLoad(tempStringBuilder.ToString(), typeof(Sprite)) as Sprite;
			_imgAccessoryUI.sprite = AssetMgr.Instance.AssetLoad(tempStringBuilder.ToString(), typeof(Sprite)) as Sprite;
		}
	}

	void OnDisable()
	{
		DebugMgr.Log("Destroy Equip Model Object");
		Destroy(_cEquipItem.cObject);
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
		tempStatus[index] += (UInt16)EquipmentInfoMgr.Instance.skillPointPerLevel;

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
		_txtSkillValue[index].text = tempStringBuilder.Append("+ ").Append(tempStatus[index]).ToString();        
		
		ShowEffect(_objSkill[index].GetComponent<RectTransform>()); 
	}
	private void AckStatUp(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();
		
		if(err != Server.ERROR_ID.NONE)
		{
			//DebugMgr.Log(err);
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(((int)err).ToString()), Server.ServerMgr.Instance.CallClear);
			statPoint = statPointOrigin;
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
            PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.EquipStatPoint);
        }
	}
	
	public void SetStatButtons()
	{
		_txtStatPoint.text = statPoint.ToString();
		
		_btnReset.interactable = false;
		_btnReset.GetComponent<Image>().sprite = _imgDisableButton;

		for(int i=0; i<tempStatus.Length; i++)
		{
			if(tempStatus[i] != 0)
			{
				_btnReset.interactable = true;
				_btnReset.GetComponent<Image>().sprite = _imgEnableButton;
				break;
			}
		}
		
		//스탯 포인트 없으면 찍기 비활성화
		if(statPoint == 0)
		{
			for(int i=0; i<Server.ConstDef.SkillOfEquip; i++)
			{
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
			
			_btnAuto.GetComponent<Image>().sprite = _imgDisableButton;
			_btnAuto.interactable = false;
		}
		else
		{
			for(int i=0; i<Server.ConstDef.SkillOfEquip; i++)
			{
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
			
			_btnAuto.interactable = true;
			_btnAuto.GetComponent<Image>().sprite = _imgEnableButton;
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

    public void OnClickCompare()
    {
        if(tglComapre.isOn)
        {
            MaxLevelDiffEquipment();
            _cItemlist.MaxLevelDiffEquipment();
        }
        else
        {
            CurrentLevelDiffEquipment();
            _cItemlist.CurrentLevelDiffEquipment();
        }
    }

    public void MaxLevelDiffEquipment()
    {
        Color tempColor;
        _cDummyEquipItem = new EquipmentItem(_cEquipItem);
        _cDummyEquipItem.u1SmithingLevel = _cEquipItem.u1SmithingLevel;
        _cDummyBaseEquipItem = new EquipmentItem(_cBaseEquipItem);
        _cDummyBaseEquipItem.u1SmithingLevel = _cBaseEquipItem.u1SmithingLevel;
        _cEquipItem.CopyStatus(_cDummyEquipItem);
	    _cBaseEquipItem.CopyStatus(_cDummyBaseEquipItem);
        _cDummyEquipItem.GetComponent<LevelComponent>().Set((UInt16)Server.ConstDef.MaxHeroLevel, ClassInfoMgr.Instance.GetNextExp((UInt16)(Server.ConstDef.MaxHeroLevel-1))-1);
        _cDummyBaseEquipItem.GetComponent<LevelComponent>().Set((UInt16)Server.ConstDef.MaxHeroLevel, ClassInfoMgr.Instance.GetNextExp((UInt16)(Server.ConstDef.MaxHeroLevel-1))-1);

        _txtLevel.text = Server.ConstDef.MaxHeroLevel.ToString();
        _txtLevelMax.gameObject.SetActive(true);
        _txtExp.gameObject.SetActive(false);
        _imgExpGauge.gameObject.SetActive(false);
        for(Byte i=0; i<EquipmentInfo.ADD_STAT_TYPE_MAX; i++)
		{
			statType[i] = _cDummyEquipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType;
			_txtStatType[i].text = Status.GetStatText(statType[i]);
			_txtStatValue[i].text = _cDummyEquipItem.cStatus.GetStat(statType[i], tempStatus[i + Server.ConstDef.SkillOfEquip]).ToString();
            
            ColorUtility.TryParseHtmlString(CompareStatColor16, out tempColor);
            _txtStatValue[i].color = tempColor;
		}

        SetStatusDiff(_cDummyBaseEquipItem, _cDummyEquipItem);
    }

    public void CurrentLevelDiffEquipment()
    {
        Color tempColor;
        _txtLevel.text = _cEquipItem.cLevel.u2Level.ToString();
        _txtLevelMax.gameObject.SetActive(false);
        _txtExp.gameObject.SetActive(true);
        _imgExpGauge.gameObject.SetActive(true);
        for(Byte i=0; i<EquipmentInfo.ADD_STAT_TYPE_MAX; i++)
		{
			statType[i] = _cEquipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType;
			_txtStatType[i].text = Status.GetStatText(statType[i]);
			_txtStatValue[i].text = _cEquipItem.cStatus.GetStat(statType[i], tempStatus[i + Server.ConstDef.SkillOfEquip]).ToString();
            
            ColorUtility.TryParseHtmlString(CurrentStatColor16, out tempColor);
            _txtStatValue[i].color = tempColor;
		}
        SetStatusDiff(_cBaseEquipItem, _cEquipItem);
    }

	private void ShowEffect(RectTransform rect)
	{
		GameObject instEffect = Instantiate(pointingEffect);
		instEffect.transform.SetParent(transform);
		instEffect.transform.localPosition = new Vector3(rect.anchoredPosition3D.x + 280f, rect.anchoredPosition3D.y - 20f);// + Vector3.down * 20f;
		instEffect.transform.localScale = Vector3.one;
	}
	public void OnClickAuto()
	{
		int index = Server.ConstDef.SkillOfEquip;
		
		Dictionary<int, int> upIndex = new Dictionary<int, int>();
		
		while(statPoint > 0)
		{
			if(index < Server.ConstDef.SkillOfEquip)
			{
				if(tempStatus[index] != 0)
				{
					int unlockSlot = 1 + (_cEquipItem.cLevel.u2Level / UNLOCK_LV);
					
					if(unlockSlot > index)
					{
						tempStatus[index] += (UInt16)EquipmentInfoMgr.Instance.skillPointPerLevel;
						statPoint -= EquipmentInfoMgr.Instance.skillPointPerLevel;
					}
				}
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                _txtSkillValue[index].text = tempStringBuilder.Append("+ ").Append(tempStatus[index]).ToString();
				
				if(upIndex.ContainsKey(index) == false)
				{
					upIndex.Add(index, index);
					ShowEffect(_objSkill[index].GetComponent<RectTransform>());
				}                 
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

    private IEnumerator StatIconUpAni(int idx)
    {
        _imgStatUpIcon[idx].rectTransform.anchoredPosition3D = Vector3.zero;
        while (true)
        {
            LeanTween.moveLocalY(_imgStatUpIcon[idx].gameObject, 7, 0.2f);
            yield return new WaitForSeconds(0.2f);
            LeanTween.moveLocalY(_imgStatUpIcon[idx].gameObject, 0, 0.2f);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator StatIconDownAni(int idx)
    {
        _imgStatUpIcon[idx].rectTransform.anchoredPosition3D = Vector3.zero;
        while (true)
        {
            LeanTween.moveLocalY(_imgStatUpIcon[idx].gameObject, -7, 0.2f);
            yield return new WaitForSeconds(0.2f);
            LeanTween.moveLocalY(_imgStatUpIcon[idx].gameObject, 0, 0.2f);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
