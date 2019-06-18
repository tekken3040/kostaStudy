using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
public class UI_SubPanel_CharacterInfo_Equipment_ItemList : MonoBehaviour {
	public Color32 disableColorStart = new Color32(190,190,190,255);
	public Color32 disableColorEnd = new Color32(102,102,102,255);
	public Color32 enableColorStart = Color.white;
	public Color32 enableColorEnd = new Color32(137, 163, 180, 255);
	private const int UNLOCK_LV = 10;
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
    [SerializeField] GameObject[] _objStatBtn;
    [SerializeField] GameObject[] _objStat10Btn;
	[SerializeField] GameObject[] _objSkill;
    [SerializeField] GameObject[] _objSkillBtn;
	[SerializeField] GameObject[] _objNoneSkill;
	[SerializeField] Image[] _imgSkillIcon;
	[SerializeField] Image[] _imgSkillElement;
	[SerializeField] Text[] _txtSkillValue;
	[SerializeField] UI_Button_CharacterInfo_Equipment_SkillInfo[] _btnSkillInfo;
	[SerializeField] Image[] _imgEffects;
	[SerializeField] Text _txtStatPoint;
    [SerializeField] Text _txtStatPointExp;
	[SerializeField] GameObject remainEffect;
	private GameObject pointingEffect;
	[SerializeField] Sprite _imgEnableButton;
	[SerializeField] Sprite _imgDisableButton;
	[SerializeField] Button _btnClose;
    [SerializeField] Button _btnSmithing;
    [SerializeField] Button _btnFusion;
    [SerializeField] Button _btnChangeLook;

	[SerializeField] GameObject _objListPanel;
	//[SerializeField] GameObject _resListElement;
	[SerializeField] RectTransform _trScrollParent;
	[SerializeField] RectTransform _trListParent;

	[SerializeField] RectTransform _trModelObjParent;
	public SpriteRenderer _imgAccessory;
	public Image _imgAccessoryUI;
	[SerializeField] RectTransform _trEquipEffParent;

    [SerializeField] UI_Panel_CharacterInfo_Equipment characterInfo_equipment;
	GameObject _objEquipModel;
	public GameObject _objEquipEffect;
	private UI_PointBuyPopup pointBuyPopup;
    private GameObject resetPopup;
	[SerializeField] UI_SubPanel_CharacterInfo_Equipment_ItemInfo _scriptDiffPanel;
    //[SerializeField] GameObject _prefEquipmentFusion;
    //[SerializeField] GameObject _prefEquipmentChangeLook;
    //[SerializeField] GameObject _prefEquipmentSmithing;
    [SerializeField] UI_Button_CharacterInfo_Equipment_StateInfo _specializeBtn;
    [SerializeField] Text _equipmentClassName;
    [SerializeField] Image _imgClassIcon;
    //[SerializeField] GameObject Pref_Star;
    [SerializeField] GameObject starPos;

	Hero _cHero;
	Byte _posID;
	UI_Panel_CharacterInfo_Equipment _parentScript;
	int _oldSelectedInvenSlotNum;
	EquipmentItem _cBaseEquipItem;
    EquipmentItem _cDummyBaseEquipItem;
	EquipmentItem _cDiffEquipItem;

	private Byte[] statType;
	private UInt32[] tempStatus;
	int statPoint;
	int statPointOrigin;
    private List<UInt16> _isNewSlots;
    
    private List<GameObject> listEffects = new List<GameObject>();
    StringBuilder tempStringBuilder;
    String CompareStatColor16 = "#1CFF5FFF";
    String CurrentStatColor16 = "#EBC378FF";

	public void InitItemList_All()
	{
        this.gameObject.SetActive(true);
        _objListPanel.SetActive(true);
        InitMaterialItemList();
	}

	public void SetData(EquipmentItem baseEquipItem, UI_Panel_CharacterInfo_Equipment parent)
	{
        _isNewSlots = new List<UInt16>();
		if(pointingEffect == null)
			pointingEffect = AssetMgr.Instance.AssetLoad("Prefabs/UI_Effects/StatusPoint02.prefab", typeof(GameObject)) as GameObject;
        tempStringBuilder = new StringBuilder();
		_parentScript = parent;
		_cBaseEquipItem = baseEquipItem;
		_cHero = _cBaseEquipItem.attached.hero;
		_posID = (Byte)_cBaseEquipItem.GetEquipmentInfo().u1PosID;

		tempStatus = _cBaseEquipItem.GetPoints();
		statType = new Byte[Server.ConstDef.EquipStatPointType];
		_cBaseEquipItem.GetComponent<StatusComponent>().CountingStatPointEquip(_cBaseEquipItem.cLevel.u2Level);
        //statPoint = (int)_cBaseEquipItem.GetComponent<StatusComponent>().CountingStatPointEquip();
        statPoint = (int)_cBaseEquipItem.GetComponent<StatusComponent>().UNSET_STATPOINT;
		statPointOrigin = statPoint;
        _specializeBtn.SetData((Byte)(_cBaseEquipItem.GetEquipmentInfo().u1Specialize+2));
        Color tempColor;
        ColorUtility.TryParseHtmlString(_cBaseEquipItem.GetEquipmentInfo().GetHexColor((Byte)(_cBaseEquipItem.GetEquipmentInfo().u1Specialize+2)), out tempColor);
        _specializeBtn.transform.parent.GetComponent<Image>().color = tempColor;
        if(_cBaseEquipItem.GetEquipmentInfo().u2ClassID <= ClassInfo.LAST_CLASS_ID)
        {
            _equipmentClassName.text = TextManager.Instance.GetText(ClassInfoMgr.Instance.GetInfo(_cBaseEquipItem.GetEquipmentInfo().u2ClassID).sName);
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            _imgClassIcon.sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.Append("Sprites/Common/common_class.common_class_").Append(_cBaseEquipItem.GetEquipmentInfo().u2ClassID).ToString());
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
        for(int i=0; i<_cBaseEquipItem.u1Completeness; i++)
        {
            starPos.transform.GetChild(i).gameObject.SetActive(true);
            UIManager.Instance.SetGradientFromTier(starPos.transform.GetChild(i).GetComponent<Gradient>(), _cBaseEquipItem.u1SmithingLevel);
        }
        starPos.GetComponent<GridLayoutGroup>().SetLayoutHorizontal();

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        _imgElementIcon.sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.Append("Sprites/Common/common_02_renew.element_icon_").Append(_cBaseEquipItem.GetEquipmentInfo().u1Element).ToString());
		_imgElementIcon.SetNativeSize();

        //_txtTier.text = "<" + TextManager.Instance.GetText( "forge_level_" + _cBaseEquipItem.u1SmithingLevel) + ">";
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        _txtTier.text = TextManager.Instance.GetText(tempStringBuilder.Append("forge_level_").Append(_cBaseEquipItem.u1SmithingLevel).ToString());
		UIManager.Instance.SetGradientFromTier(_txtTier.GetComponent<Gradient>(), _cBaseEquipItem.u1SmithingLevel);
		string equipName = "";
		equipName = _cBaseEquipItem.itemName;
		if(equipName != "")
			equipName = equipName + " " + TextManager.Instance.GetText( _cBaseEquipItem.GetEquipmentInfo().sName );
		else
			equipName = TextManager.Instance.GetText( _cBaseEquipItem.GetEquipmentInfo().sName );
		_txtEquipName.text = equipName;
        _txtEquipName.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
		UIManager.Instance.SetGradientFromElement(_txtEquipName.GetComponent<Gradient>(), _cBaseEquipItem.GetEquipmentInfo().u1Element);
		//UIManager.Instance.SetSizeTextGroup(_trNameGroup, 18);
        UIManager.Instance.SetSizeTextGroup(_trStarGroup, 18);

        if (_cBaseEquipItem.createrName != "")
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            _txtCreator.text = tempStringBuilder.Append("By ").Append(_cBaseEquipItem.createrName).ToString();
        }
        else
            _txtCreator.text = "";

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        _imgElement.sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.Append("Sprites/Common/common_02_renew.element_").Append(_cBaseEquipItem.GetEquipmentInfo().u1Element).ToString());
		_txtLevel.text = _cBaseEquipItem.cLevel.u2Level.ToString();
		_txtExp.text = string.Format("{0}/{1}",ConvertExpValue(_cBaseEquipItem.cLevel.u8Exp), ConvertExpValue(_cBaseEquipItem.cLevel.u8NextExp));
		_imgExpGauge.fillAmount = (float)((float)_cBaseEquipItem.cLevel.u8Exp / (float)_cBaseEquipItem.cLevel.u8NextExp);
		if(_cBaseEquipItem.u1SmithingLevel != 0 && _cBaseEquipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 && _cBaseEquipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2 )
		{
			ForgeInfo smithingForgeInfo = ForgeInfoMgr.Instance.GetList()[(_cBaseEquipItem.u1SmithingLevel-1)];
			Byte skillCount = (Byte)(smithingForgeInfo.cSmithingInfo.u1RandomSkillCount + smithingForgeInfo.cSmithingInfo.u1SelectSkillCount);
			for(Byte i=0; i<skillCount; i++)
			{
				SkillInfo skillInfo = SkillInfoMgr.Instance.GetInfoBySlot(_cBaseEquipItem.GetEquipmentInfo().u2ClassID, _cBaseEquipItem.skillSlots[i]);
				if(skillInfo != null)
				{
                    _imgSkillIcon[i].sprite = AtlasMgr.Instance.GetSprite(String.Format("Sprites/Skill/Atlas_SkillIcon_{0}.{1}", _cBaseEquipItem.GetEquipmentInfo().u2ClassID, skillInfo.u2ID));
					_objSkill[i].SetActive(true);
					_objNoneSkill[i].SetActive(false);
				}
			}
			for(Byte i=skillCount; i<EquipmentInfo.ADD_SKILL_MAX; i++)
			{
				_objSkill[i].SetActive(false);
				_objNoneSkill[i].SetActive(true);
			}
		}
		else
		{
			for(Byte i=0; i<EquipmentInfo.ADD_SKILL_MAX; i++)
			{
				_objSkill[i].SetActive(false);
				_objNoneSkill[i].SetActive(true);
			}
		}

		for(int i=0; i<_trModelObjParent.childCount; i++)
		{
			if(_trModelObjParent.FindChild("EquipmentObject") != null)
				DestroyObject(_trModelObjParent.FindChild("EquipmentObject").gameObject);
		}

		if(_cBaseEquipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 && _cBaseEquipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
		{
			_cBaseEquipItem.InitViewModelObject();
			_cBaseEquipItem.cObject.GetComponent<EquipmentObject>().SetLayer( LayerMask.NameToLayer("UI") );
			_cBaseEquipItem.cObject.transform.SetParent( _trModelObjParent );
			_cBaseEquipItem.cObject.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
			_cBaseEquipItem.cObject.transform.localPosition = new Vector3(3f, -8f, 0);//Vector3.zero;
			_cBaseEquipItem.cObject.transform.name = "EquipmentObject";
			_objEquipModel = _cBaseEquipItem.cObject;
			_imgAccessory.gameObject.SetActive(false);
			_imgAccessoryUI.gameObject.SetActive(false);
		}
		else
		{
			DebugMgr.Log("Acc Name : " + _cBaseEquipItem.GetEquipmentInfo().u2ID);
			UInt16 modelID = _cBaseEquipItem.u2ModelID;
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            if (modelID == 0)
			{
                tempStringBuilder.Append("Sprites/Item/Accessory/acc_").Append(_cBaseEquipItem.GetEquipmentInfo().u2ModelID).Append(".png");
                _imgAccessory.sprite = AssetMgr.Instance.AssetLoad(tempStringBuilder.ToString(), typeof(Sprite)) as Sprite;
				_imgAccessoryUI.sprite = AssetMgr.Instance.AssetLoad(tempStringBuilder.ToString(), typeof(Sprite)) as Sprite;
			}
			else
			{
                tempStringBuilder.Append("Sprites/Item/Accessory/acc_").Append(_cBaseEquipItem.u2ModelID).Append(".png");
                _imgAccessory.sprite = AssetMgr.Instance.AssetLoad(tempStringBuilder.ToString(), typeof(Sprite)) as Sprite;
                _imgAccessoryUI.sprite = AssetMgr.Instance.AssetLoad(tempStringBuilder.ToString(), typeof(Sprite)) as Sprite;
			}
			_imgAccessory.gameObject.SetActive(true);
			_imgAccessoryUI.gameObject.SetActive(false);
		}

		if(_objEquipEffect != null) DestroyObject(_objEquipEffect);
		if(_cBaseEquipItem.u1SmithingLevel >= 1)
		{
			_objEquipEffect = Instantiate(AssetMgr.Instance.AssetLoad(string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}.prefab", _cBaseEquipItem.u1SmithingLevel), typeof(GameObject))) as GameObject;
			_objEquipEffect.transform.SetParent(_trModelObjParent.parent);
			_objEquipEffect.transform.name = "WeaponEffect";
			_objEquipEffect.transform.localScale = new Vector3(1f, 1f, 1f);
			_objEquipEffect.transform.localPosition = new Vector3(0f, -10f, -50f);
		}

		ClearList();
		Legion.Instance.cInventory.EquipSort();
		List<EquipmentItem> inven = Legion.Instance.cInventory.lstSortedEquipment;
		
		_objListPanel.SetActive (true);
		InitMaterialItemList();

		_scriptDiffPanel.gameObject.SetActive (false);
        
        _txtStatPoint.text = _cBaseEquipItem.GetComponent<StatusComponent>().UNSET_STATPOINT.ToString();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append("(");
        tempStringBuilder.Append((_cBaseEquipItem.GetComponent<StatusComponent>().STATPOINT_EXP/StatusComponent.MAX_STATEXP_PROGRESS).ToString());
        tempStringBuilder.Append(".");
        if((_cBaseEquipItem.statusComponent.STATPOINT_EXP%StatusComponent.MAX_STATEXP_PROGRESS) >= 10)
            tempStringBuilder.Append((_cBaseEquipItem.statusComponent.STATPOINT_EXP%StatusComponent.MAX_STATEXP_PROGRESS).ToString().Remove(1));
        else
            tempStringBuilder.Append((_cBaseEquipItem.statusComponent.STATPOINT_EXP%StatusComponent.MAX_STATEXP_PROGRESS).ToString());
        tempStringBuilder.Append("%)");
        _txtStatPointExp.text = tempStringBuilder.ToString();
		SetStatButtons();
        Byte tempPosID = 0;
        if(_posID > 9)
            tempPosID = 9;
        else
            tempPosID = _posID;
		if(StageInfoMgr.Instance.GetStageClearStar(ForgeInfoMgr.Instance.dicUnlockInfo[tempPosID].u2EquipSlotID) > 0)
            _btnSmithing.gameObject.SetActive(true);
        else
            _btnSmithing.gameObject.SetActive(false);
        if(tempPosID == 9)
            _btnChangeLook.gameObject.SetActive(false);
        else
            _btnChangeLook.gameObject.SetActive(true);
	}
	void InitMaterialItemList()
	{
		int listHeight = 0;
        _isNewSlots.Clear();
		for(int i=0; i<_trListParent.childCount; i++)
		{
			DestroyObject(_trListParent.GetChild(i).gameObject);
		}
		int itemCount = 0;
		Legion.Instance.cInventory.EquipSort();
		for(int i=0; i<Legion.Instance.cInventory.lstSortedEquipment.Count; i++)
		{
			EquipmentItem equipItem = Legion.Instance.cInventory.lstSortedEquipment[i];
            if(_cBaseEquipItem.GetEquipmentInfo().u2ClassID != equipItem.GetEquipmentInfo().u2ClassID)
                continue;
            if(equipItem.attached.hero != null)
            {
                if(Legion.Instance.cInventory.dicInventory[equipItem.u2SlotNum].isNew)
                    _isNewSlots.Add(equipItem.u2SlotNum);
                continue;
            }
            if(equipItem.GetEquipmentInfo().u1PosID != _cBaseEquipItem.GetEquipmentInfo().u1PosID)
                continue;
            if(equipItem == _cBaseEquipItem)
                continue;
			//if(equipItem != _cBaseEquipItem && equipItem.attached.hero == null && _cBaseEquipItem.GetEquipmentInfo().u2ClassID == equipItem.GetEquipmentInfo().u2ClassID &&
			//   equipItem.GetEquipmentInfo().u1PosID == _cBaseEquipItem.GetEquipmentInfo().u1PosID )
			//{
				GameObject listElement = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Equipment/Pref_UI_ListElement_Forge_Fusion_Material.prefab", typeof(GameObject))) as GameObject;
				Transform listElementTr = listElement.transform;
				listElementTr.SetParent( _trListParent );
				listElementTr.name = equipItem.u2SlotNum.ToString();
				listElementTr.localScale = Vector3.one;
				listElementTr.localPosition = Vector3.zero;
                if(Legion.Instance.cInventory.dicInventory[equipItem.u2SlotNum].isNew)
                {
                    _isNewSlots.Add(equipItem.u2SlotNum);
                }
				listElement.GetComponent<UI_ListElement_Forge_Fusion_Material>().SetData(equipItem, _cBaseEquipItem);
				int slotNum = 0;
				slotNum = equipItem.u2SlotNum;
				listElement.GetComponent<Button>().onClick.AddListener( () => OnClickListElement(slotNum) );
				if(Legion.Instance.cTutorial.bIng && itemCount==0) 
					listElement.AddComponent<TutorialButton>().id = "Selelct_Equip_1";
				itemCount++;
                
				//if(i%4 == 1) listHeight++;
			//}
		}
        if((_trListParent.transform.childCount / 4) == 0)
            listHeight = _trListParent.transform.childCount / 4;
        else
            listHeight = (_trListParent.transform.childCount / 4) + 1;
		DebugMgr.Log("Item Count : " + itemCount);
		_trScrollParent.sizeDelta = new Vector2(540, 148f*listHeight);
		
	}
	void ClearList()
	{
		for(int i=0; i<_trListParent.childCount; i++)
		{
			DestroyObject(_trListParent.GetChild(i).gameObject);
		}
	}
	
	void OnClickListElement(int invenSlotNum)
	{
		DebugMgr.Log("Select Equip : " + invenSlotNum);
		_cDiffEquipItem = ((EquipmentItem)Legion.Instance.cInventory.dicInventory [Convert.ToUInt16 (invenSlotNum)]);
        _scriptDiffPanel.gameObject.SetActive(true);
        _scriptDiffPanel.SetData (_cHero, _cDiffEquipItem, this);
		_btnClose.image.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.btn_back");
		_objListPanel.SetActive (false);
		_oldSelectedInvenSlotNum = invenSlotNum;
		_btnClose.GetComponent<ButtonSound>().buttonClip = Resources.Load("Sound/UI/01. Common/UI_Button_Close", typeof(AudioClip)) as AudioClip;
	}
	
	public void OnClickAttach()
	{
		if(_cHero.cLevel.u2Level < _cDiffEquipItem.cLevel.u2Level)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_desc_equip_error_exchange"), TextManager.Instance.GetText("popup_desc_equip_error_level"), null);
			return;
		}
		_cHero.cObject.SetActive(true);

		_cHero.ChangeWear (_posID, _cDiffEquipItem.u2SlotNum);
		_cHero.cObject.GetComponent<HeroObject> ().ChangeEquip (_cBaseEquipItem, _cDiffEquipItem);
		_cHero.cObject.GetComponent<HeroObject> ().SetAnimations_UI();
        Legion.Instance.cInventory.dicInventory[_cDiffEquipItem.u2SlotNum].isNew = false;
        if(_scriptDiffPanel.GetToggleCompare.isOn)
                _scriptDiffPanel.GetToggleCompare.isOn = false;
		_parentScript.OnClickAttach();
		_parentScript.OnClickBack();
		_btnClose.image.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.btn_close");
		_btnClose.GetComponent<ButtonSound>().buttonClip = Resources.Load("Sound/UI/01. Common/Close_Button_Touch_nonloop", typeof(AudioClip)) as AudioClip;
	}

	void OnEnable()
	{
		_oldSelectedInvenSlotNum = 0;
	}
	
	void OnDisable()
	{
		_scriptDiffPanel.gameObject.SetActive (false);
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
		_txtStatValue[index-Server.ConstDef.SkillOfEquip].text = _cBaseEquipItem.cStatus.GetStat(statType[index-Server.ConstDef.SkillOfEquip], tempStatus[index]).ToString();
		
		ShowEffect(_txtStatValue[index-Server.ConstDef.SkillOfEquip].transform.parent.GetComponent<RectTransform>());
        
        SetStatButtons();
		if(statPoint == 0)
		{
			_parentScript.InitEquipSlots();
		}
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
		_txtSkillValue[index].text = "+ " + tempStatus[index].ToString();        
		
		ShowEffect(_objSkill[index].GetComponent<RectTransform>()); 
        
        SetStatButtons();
	}
	private void AckStatUp(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();
		
		if(err != Server.ERROR_ID.NONE)
		{
			//DebugMgr.Log(err);
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EQUIP_STAT_POINT, err), Server.ServerMgr.Instance.CallClear);
			statPoint = statPointOrigin;
            tempStatus = _cBaseEquipItem.GetPoints();
			_txtStatPoint.text = statPoint.ToString();
			SetStatButtons();
		}
		else
		{
			_cBaseEquipItem.GetComponent<StatusComponent>().points = tempStatus;
			_cBaseEquipItem.GetComponent<StatusComponent>().CountingStatPointEquip(_cBaseEquipItem.cLevel.u2Level);
			_cBaseEquipItem.GetComponent<StatusComponent>().SetByLevelEquip(_cBaseEquipItem.cLevel);

			Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.EquipStatPoint, 0, 0, 0, 0, (uint)(statPointOrigin - statPoint));
            
			statPointOrigin = statPoint;
			_txtStatPoint.text = statPoint.ToString();
			_cBaseEquipItem.statusComponent.UNSET_STATPOINT = (UInt16)statPoint;
			SetStatButtons();    
			
            characterInfo_equipment.OnClose_ItemList();
			gameObject.SetActive(false);
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
			
			for(int i=0; i<Server.ConstDef.SkillOfEquip; i++)
			{
                _objStatBtn[i].SetActive(false);
                _objSkillBtn[i].SetActive(false);
                _objStat10Btn[i].SetActive(false);
                
				//스킬이 없을경우
				if(_cBaseEquipItem.skillSlots[i] == 0)
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
		}
		else
		{
			
			for(int i=0; i<Server.ConstDef.SkillOfEquip; i++)
			{
                _objStatBtn[i].SetActive(true);
                _objSkillBtn[i].SetActive(true);
                if(statPoint > 19)
                    _objStat10Btn[i].SetActive(true);
                else
                    _objStat10Btn[i].SetActive(false);

				if(_cBaseEquipItem.skillSlots[i] == 0)
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
			statType[i] = _cBaseEquipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType;
			_txtStatType[i].text = Status.GetStatText(statType[i]);
            _txtStatValue[i].text = _cBaseEquipItem.cStatus.GetStat(statType[i], tempStatus[i + Server.ConstDef.SkillOfEquip]).ToString();
			_btnStatInfo[i].SetData(statType[i]);
		}
		
		for(int i=0; i<Server.ConstDef.SkillOfEquip; i++)
		{
			if(_cBaseEquipItem.skillSlots[i] == 0)
			{
				//	skillNames[i].text = "NO SKILLS";
				//	skillTexts[i].text = "";
			}
			else
			{				
				SkillInfo skillInfo = SkillInfoMgr.Instance.GetInfoBySlot(_cBaseEquipItem.GetEquipmentInfo().u2ClassID, _cBaseEquipItem.skillSlots[i]);
				_imgSkillIcon[i].sprite = AtlasMgr.Instance.GetSprite("Sprites/Skill/Atlas_SkillIcon_" + _cBaseEquipItem.GetEquipmentInfo().u2ClassID + "." + skillInfo.u2ID);
				_imgSkillElement[i].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.common_02_skill_element_" + skillInfo.u1Element);
				_txtSkillValue[i].text = "+ " + tempStatus[i].ToString();
				_btnSkillInfo[i].SetData(skillInfo); 
			}
		}
		
		tempEquipResetPrice = ((int)LegionInfoMgr.Instance.cEquipStatResetGoods.u4Count + ((int)LegionInfoMgr.Instance.equipResetUpgrade * _cBaseEquipItem.GetComponent<StatusComponent>().ResetCount));
		//		//		resetBtnText.text = string.Format("초기화 - {0} {1}", Legion.Instance.GetConsumeString((int)LegionInfoMgr.Instance.equipResetType), tempEquipResetPrice);
	}
	//정환 코드 수정
	int tempEquipResetPrice;
	public void OnClickReset()
	{
        if(resetPopup == null)
        {
            resetPopup = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Skill/Pref_UI_Reset.prefab", typeof(GameObject))) as GameObject;
            resetPopup.transform.SetParent(PopupManager.Instance._objPopupManager.transform);
            resetPopup.transform.localScale = Vector3.one;
            resetPopup.transform.localPosition = Vector3.zero;
            resetPopup.GetComponent<YesNoPopup>().Show(TextManager.Instance.GetText("popup_title_stat_reset_char"), tempEquipResetPrice.ToString(), RequestReset, null);
            resetPopup.GetComponent<StatResetPopup>().Set(TextManager.Instance.GetText("popup_title_stat_reset_char"), "스탯 포인트를 초기화 하시겠습니까?");
        }
        
        PopupManager.Instance.AddPopup(resetPopup, resetPopup.GetComponent<YesNoPopup>().OnClickNoWithDest); 
	}
    
    public void RequestReset(object[] param)
    {
        if(!Legion.Instance.CheckEnoughGoods(2, tempEquipResetPrice))
        {
			PopupManager.Instance.ShowChargePopup(2);            
            return;
        }            			                    
        
	    PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.ResetEquipmentStatus(_cBaseEquipItem, AckResetStat);        
    }    
    
	private void AckResetStat(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();
		
		if(err != Server.ERROR_ID.NONE)
		{
			//DebugMgr.Log(err);
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EQUIP_STAT_RESET, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
			Legion.Instance.SubGoods((int)LegionInfoMgr.Instance.cEquipStatResetGoods.u1Type, tempEquipResetPrice);
			for(int i=0; i<tempStatus.Length; i++)
				tempStatus[i] = 0;
			
			_cBaseEquipItem.GetComponent<StatusComponent>().points = tempStatus;
			_cBaseEquipItem.GetComponent<StatusComponent>().CountingStatPointEquip(_cBaseEquipItem.cLevel.u2Level);
			_cBaseEquipItem.GetComponent<StatusComponent>().SetByLevelEquip(_cBaseEquipItem.cLevel);
			
			statPointOrigin = statPoint;

            _txtStatPoint.text = statPoint.ToString();
			_cBaseEquipItem.statusComponent.UNSET_STATPOINT = (UInt16)statPoint;
			SetStatButtons();

		}
	}
	public void OnClickAddButton()
	{
        if(_cBaseEquipItem.GetComponent<StatusComponent>().BuyPoint >= EquipmentInfoMgr.Instance.LIMIT_EQUIP_STATPOINT)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_desc_over_limit"), TextManager.Instance.GetText("popup_desc_pt_nobuy"), null);
            return;
        }  

		if(_objEquipEffect != null)
			_objEquipEffect.SetActive(false);
		if(_scriptDiffPanel._objEquipEffect != null)
			_scriptDiffPanel._objEquipEffect.SetActive(false);

		if(_cBaseEquipItem.GetEquipmentInfo().u1PosID == EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 ||
		   _cBaseEquipItem.GetEquipmentInfo().u1PosID == EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
		{
			_imgAccessory.gameObject.SetActive(false);
			_imgAccessoryUI.gameObject.SetActive(true);
		}
		if(_scriptDiffPanel.gameObject.activeSelf &&
		   (_cDiffEquipItem.GetEquipmentInfo().u1PosID == EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 ||
		 _cDiffEquipItem.GetEquipmentInfo().u1PosID == EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2) )
		{
			_scriptDiffPanel._imgAccessory.gameObject.SetActive(false);
			_scriptDiffPanel._imgAccessoryUI.gameObject.SetActive(true);
		}

		if(pointBuyPopup == null)
		{
			GameObject popupObject = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Skill/Pref_UI_PointBuy.prefab", typeof(GameObject))) as GameObject;
			RectTransform rect = popupObject.GetComponent<RectTransform>();
			rect.SetParent(transform);
			rect.anchoredPosition3D = Vector3.zero;
			rect.sizeDelta = Vector2.zero;
			rect.localScale = Vector3.one;
			popupObject.transform.localPosition = new Vector3(0f, 0f, -1000f);            
			pointBuyPopup = popupObject.GetComponent<UI_PointBuyPopup>();
		}
		
		pointBuyPopup.gameObject.SetActive(true);
		pointBuyPopup.Show(TextManager.Instance.GetText("popup_title_stat_buy_equip"), TextManager.Instance.GetText("popup_desc_stat_buy_equip"), RequestBuyPoint, null, CloseAddPopup, null);
		pointBuyPopup.SetBuyPointPopup(UI_PointBuyPopup.BuyType.Equip, _cBaseEquipItem.GetComponent<StatusComponent>().BuyPoint);
	}
	public void CloseAddPopup(object[] param)
	{
		if(_objEquipEffect != null)
			_objEquipEffect.SetActive(true);
		if(_scriptDiffPanel._objEquipEffect != null)
			_scriptDiffPanel._objEquipEffect.SetActive(true);

		if(_cBaseEquipItem.GetEquipmentInfo().u1PosID == EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 ||
		   _cBaseEquipItem.GetEquipmentInfo().u1PosID == EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
		{
			_imgAccessory.gameObject.SetActive(true);
			_imgAccessoryUI.gameObject.SetActive(false);
		}
		if(_scriptDiffPanel.gameObject.activeSelf &&
		   (_cDiffEquipItem.GetEquipmentInfo().u1PosID == EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 ||
		 _cDiffEquipItem.GetEquipmentInfo().u1PosID == EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2) )
		{
			_scriptDiffPanel._imgAccessory.gameObject.SetActive(true);
			_scriptDiffPanel._imgAccessoryUI.gameObject.SetActive(false);
		}

        if(pointBuyPopup != null)
		DestroyObject(pointBuyPopup.gameObject);
	}

	Byte buyPoint = 0;
    int pointPrice = 0;
    
    public void RequestBuyPoint(object[] param)
    {
        buyPoint = (Byte)param[0];
        pointPrice = (int)param[1];
        
        if(!Legion.Instance.CheckEnoughGoods(2, pointPrice))
        {
			PopupManager.Instance.ShowChargePopup(2);
            return;
        }
  
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.BuyEquipmentStatPoint(_cBaseEquipItem, buyPoint, AckBuyPoint);
    }
	
	public void AckBuyPoint(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();
		
		if(err != Server.ERROR_ID.NONE)
		{
			//DebugMgr.Log(err);
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EQUIP_BUY_STAT_POINT, err), Server.ServerMgr.Instance.CallClear);
			return;
		}
		else
		{
			_cBaseEquipItem.GetComponent<StatusComponent>().BuyPoint += buyPoint;
			statPoint += buyPoint;
			statPointOrigin = statPoint;
			_txtStatPoint.text = statPoint.ToString();            
			Legion.Instance.SubGoods(EquipmentInfoMgr.Instance.cEquipStatGoods.u1Type, pointPrice);
			buyPoint = 0;

			SetStatButtons();

			CloseAddPopup(null);
		}
	}

	private void ShowEffect(RectTransform rect)
	{
		GameObject instEffect = Instantiate(pointingEffect);
		instEffect.transform.SetParent(transform);
		instEffect.transform.localPosition = rect.anchoredPosition3D + Vector3.down * 20f;
		instEffect.transform.localScale = Vector3.one;
        
        listEffects.Add(instEffect);
	}

	public void OnClickAutoStatusConfirm()
	{
		int index = Server.ConstDef.SkillOfEquip;

		Dictionary<int, int> upIndex = new Dictionary<int, int>();

		while(statPoint > 0)
		{
			if(index < Server.ConstDef.SkillOfEquip)
			{
				if(_cBaseEquipItem.skillSlots[index] != 0 && statPoint > EquipmentInfoMgr.Instance.skillPointPerLevel)
				{
					tempStatus[index] += 1;
					statPoint -= EquipmentInfoMgr.Instance.skillPointPerLevel;

					if(upIndex.ContainsKey(index) == false)
					{
						upIndex.Add(index, index);
						ShowEffect(_objSkill[index].GetComponent<RectTransform>());
					}                                                           
				}

				_txtSkillValue[index].text = "+ " + tempStatus[index].ToString();				
			}
			else
			{
				tempStatus[index] += (UInt16)EquipmentInfoMgr.Instance.statPointPerLevel;
				statPoint -= EquipmentInfoMgr.Instance.statPointPerLevel;
				_txtStatValue[index-Server.ConstDef.SkillOfEquip].text = _cBaseEquipItem.cStatus.GetStat(statType[index-Server.ConstDef.SkillOfEquip], tempStatus[index]).ToString();

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
    public void MaxLevelDiffEquipment()
    {
        Color tempColor;
        _cDummyBaseEquipItem = new EquipmentItem(_cBaseEquipItem);
        _cDummyBaseEquipItem.u1SmithingLevel = _cBaseEquipItem.u1SmithingLevel;
	    _cBaseEquipItem.CopyStatus(_cDummyBaseEquipItem);
        _cDummyBaseEquipItem.GetComponent<LevelComponent>().Set((UInt16)Server.ConstDef.MaxHeroLevel, ClassInfoMgr.Instance.GetNextExp((UInt16)(Server.ConstDef.MaxHeroLevel-1))-1);

        _txtLevel.text = Server.ConstDef.MaxHeroLevel.ToString();
        _txtLevelMax.gameObject.SetActive(true);
        _txtExp.gameObject.SetActive(false);
        _imgExpGauge.gameObject.SetActive(false);
        for(Byte i=0; i<EquipmentInfo.ADD_STAT_TYPE_MAX; i++)
		{
			statType[i] = _cDummyBaseEquipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType;
			_txtStatType[i].text = Status.GetStatText(statType[i]);
			_txtStatValue[i].text = _cDummyBaseEquipItem.cStatus.GetStat(statType[i], tempStatus[i + Server.ConstDef.SkillOfEquip]).ToString();

            ColorUtility.TryParseHtmlString(CompareStatColor16, out tempColor);
            _txtStatValue[i].color = tempColor;
		}
    }

    public void CurrentLevelDiffEquipment()
    {
        Color tempColor;
        _txtLevel.text = _cBaseEquipItem.cLevel.u2Level.ToString();
        _txtLevelMax.gameObject.SetActive(false);
        _txtExp.gameObject.SetActive(true);
        _imgExpGauge.gameObject.SetActive(true);
        for(Byte i=0; i<EquipmentInfo.ADD_STAT_TYPE_MAX; i++)
		{
			statType[i] = _cBaseEquipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType;
			_txtStatType[i].text = Status.GetStatText(statType[i]);
			_txtStatValue[i].text = _cBaseEquipItem.cStatus.GetStat(statType[i], tempStatus[i + Server.ConstDef.SkillOfEquip]).ToString();

            ColorUtility.TryParseHtmlString(CurrentStatColor16, out tempColor);
            _txtStatValue[i].color = tempColor;
		}
    }
    UInt16 SeqNo = 0;
    GameObject FusionPanel;
    public void OnClickFusion()
    { 
        UInt16[] changedSlots;
		if(_cHero == null)
			changedSlots = null;
		
		else
			changedSlots = _cHero.GetChangingEquip();
		
		if (changedSlots != null)
		{
			PopupManager.Instance.ShowLoadingPopup(1);
			SeqNo = Server.ServerMgr.Instance.WearHero(_cHero, changedSlots, OnResponseFromServer);
		}
        else
        {
            OnResponseFromServer(Server.ERROR_ID.NONE);
        }
        //PopupManager.Instance.AddPopup(FusionPanel, OnClickClose);
    }
    public void OnResponseFromServer(Server.ERROR_ID err)
	{
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), err.ToString(), Server.ServerMgr.Instance.CallClear);
			return;
		}
		
		else if(err == Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			SeqNo = 0;
            if(_cBaseEquipItem.cObject != null)
            {
                GameObject.Destroy(_cBaseEquipItem.cObject);
                GameObject.Destroy(_objEquipModel);
                _cBaseEquipItem.cObject = null;
            }
            for(int i=0; i<_trModelObjParent.childCount; i++)
		    {
		    	if(_trModelObjParent.FindChild("EquipmentObject") != null)
		    		DestroyObject(_trModelObjParent.FindChild("EquipmentObject").gameObject);
		    }
            
            if(FusionPanel != null)
                FusionPanel.SetActive(true);
            else
            {
                FusionPanel = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Equipment/Pref_UI_Panel_Fusion_Detail.prefab", typeof(GameObject)) as GameObject);
				RectTransform rectTr = FusionPanel.GetComponent<RectTransform>();
				rectTr.SetParent(Scene.GetCurrent().mainPanel.transform);
				rectTr.localPosition = Vector3.zero;//new Vector3(0f, 0f, -1500f);
				rectTr.localScale = Vector3.one;
				rectTr.sizeDelta = Vector2.zero;
                FusionPanel.GetComponent<UI_Panel_Forge_Fusion_Module>().SetData(_cBaseEquipItem, 2);
            }
            if(_scriptDiffPanel.gameObject.activeSelf)
            {
                if(ChangeLookPanel != null)
                {
                    _objListPanel.SetActive (false);
			        _scriptDiffPanel.gameObject.SetActive(false);
                }
                else if(FusionPanel != null)
                {
                    _objListPanel.SetActive (false);
			        _scriptDiffPanel.gameObject.SetActive(false);
                }
                else if(SmithingPanel != null)
                {
                    _objListPanel.SetActive (false);
		            _scriptDiffPanel.gameObject.SetActive(false);
                }
			    else
                {
                    _objListPanel.SetActive (true);
			        _scriptDiffPanel.gameObject.SetActive(false);
                }
			    _btnClose.image.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.btn_close");
            }
            _cHero.StartChangingEquip();
            OnClickClose();
		}
	}
    GameObject ChangeLookPanel;
    public void OnClickChangeLook()
	{
        ChangeLookPanel = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Equipment/Pref_UI_Panel_Change_Look_Detail_module.prefab", typeof(GameObject)) as GameObject);
		RectTransform rectTr = ChangeLookPanel.GetComponent<RectTransform>();
		rectTr.SetParent(Scene.GetCurrent().mainPanel.transform);
		rectTr.localPosition = Vector3.zero;//new Vector3(0f, 0f, -1500f);
		rectTr.localScale = Vector3.one;
		rectTr.sizeDelta = Vector2.zero;
        ChangeLookPanel.GetComponent<UI_Panel_Forge_ChangeLook_Detail_module>().SetData(_cBaseEquipItem, 2);
        PopupManager.Instance.AddPopup(ChangeLookPanel, ChangeLookPanel.GetComponent<UI_Panel_Forge_ChangeLook_Detail_module>().OnClickClose);
        if(_scriptDiffPanel.gameObject.active)
        {
            if(ChangeLookPanel != null)
            {
                _objListPanel.SetActive (false);
		        _scriptDiffPanel.gameObject.SetActive(false);
            }
            else if(FusionPanel != null)
            {
                _objListPanel.SetActive (false);
		        _scriptDiffPanel.gameObject.SetActive(false);
            }
            else if(SmithingPanel != null)
            {
                _objListPanel.SetActive (false);
		        _scriptDiffPanel.gameObject.SetActive(false);
            }
		    else
            {
                _objListPanel.SetActive (true);
		        _scriptDiffPanel.gameObject.SetActive(false);
            }
		    _btnClose.image.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.btn_close");
        }
        OnClickClose();
	}

    GameObject SmithingPanel;
    public void OnClickSmithing()
    {
        SmithingPanel = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Equipment/Pref_UI_Forge_Smithing_Module.prefab", typeof(GameObject)) as GameObject);
		RectTransform rectTr = SmithingPanel.GetComponent<RectTransform>();
		rectTr.SetParent(Scene.GetCurrent().mainPanel.transform);
		rectTr.localPosition = Vector3.zero;//new Vector3(0f, 0f, -1500f);
		rectTr.localScale = Vector3.one;
		rectTr.sizeDelta = Vector2.zero;
        SmithingPanel.GetComponent<UI_Forge_Smithing_module>().SetData(_cBaseEquipItem.attached.attachSlotNum, (Byte)_cBaseEquipItem.attached.hero.cClass.u2ID, this);
        PopupManager.Instance.AddPopup(SmithingPanel, SmithingPanel.GetComponent<UI_Forge_Smithing_module>().OnClickClose);
        if(_scriptDiffPanel.gameObject.active)
        {
            if(SmithingPanel != null)
            {
                _objListPanel.SetActive (false);
		        _scriptDiffPanel.gameObject.SetActive(false);
            }
            else
            {
                _objListPanel.SetActive (true);
		        _scriptDiffPanel.gameObject.SetActive(false);
            }
		    _btnClose.image.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.btn_close");
        }
        _scriptDiffPanel.GetToggleCompare.isOn = false;
        this.gameObject.SetActive(false);
        //OnClickClose();
    }

	public void OnClickAuto()
	{
		int index = Server.ConstDef.SkillOfEquip;
		
		Dictionary<int, int> upIndex = new Dictionary<int, int>();
		
		while(statPoint > 0)
		{
			if(index < Server.ConstDef.SkillOfEquip)
			{
				if(_cBaseEquipItem.skillSlots[index] != 0 && statPoint > EquipmentInfoMgr.Instance.skillPointPerLevel)
				{
					// int unlockSlot = 1 + (_cBaseEquipItem.cLevel.u2Level / UNLOCK_LV);
					
					// if(unlockSlot > index)
					// {
						tempStatus[index] += 1;
						statPoint -= EquipmentInfoMgr.Instance.skillPointPerLevel;
					                    
                        if(upIndex.ContainsKey(index) == false)
                        {
                            upIndex.Add(index, index);
                            ShowEffect(_objSkill[index].GetComponent<RectTransform>());
                        }                        
                    //}                                     
				}
				
				_txtSkillValue[index].text = "+ " + tempStatus[index].ToString();				
			}
			else
			{
				tempStatus[index] += (UInt16)EquipmentInfoMgr.Instance.statPointPerLevel;
				statPoint -= EquipmentInfoMgr.Instance.statPointPerLevel;
				_txtStatValue[index-Server.ConstDef.SkillOfEquip].text = _cBaseEquipItem.cStatus.GetStat(statType[index-Server.ConstDef.SkillOfEquip], tempStatus[index]).ToString();
				
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

		if(statPoint == 0)
		{
			_parentScript.InitEquipSlots();
		}
	}
    
    public void OnClickClose()
    {
        //if(FusionPanel != null)
        //{
        //    PopupManager.Instance.RemovePopup(FusionPanel);
        //    Destroy(FusionPanel);
        //}
        _parentScript.InitEquipSlots();
		if(_scriptDiffPanel.gameObject.activeSelf)
		{
            if(_scriptDiffPanel.GetToggleCompare.isOn)
                _scriptDiffPanel.GetToggleCompare.isOn = false;
            if(ChangeLookPanel != null)
            {
                _objListPanel.SetActive (false);
			    _scriptDiffPanel.gameObject.SetActive(false);
            }
            else if(FusionPanel != null)
            {
                _objListPanel.SetActive (false);
			    _scriptDiffPanel.gameObject.SetActive(false);
            }
            else if(SmithingPanel != null)
            {
                _objListPanel.SetActive (false);
		        _scriptDiffPanel.gameObject.SetActive(false);
            }
			else
            {
                _objListPanel.SetActive (true);
			    _scriptDiffPanel.gameObject.SetActive(false);
            }
			_btnClose.image.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.btn_close");
            _btnClose.GetComponent<ButtonSound>().buttonClip = Resources.Load("Sound/UI/01. Common/UI_Button_Close", typeof(AudioClip)) as AudioClip;
		}
		else
		{
            if(_isNewSlots.Count == 0)
                CloseStatCheck(Server.ERROR_ID.NONE);
            else
            {
                PopupManager.Instance.ShowLoadingPopup(1);
                Server.ServerMgr.Instance.EquipCheckSlot(_isNewSlots.ToArray(), CloseStatCheck);
            }
			_btnClose.GetComponent<ButtonSound>().buttonClip = Resources.Load("Sound/UI/01. Common/Close_Button_Touch_nonloop", typeof(AudioClip)) as AudioClip;
		}
    }

    public void CloseStatCheck(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EQUIP_CHECK_SLOT, err), Server.ServerMgr.Instance.CallClear);
            return;
        }
        else
        {
            for(int i=0; i<_isNewSlots.Count; i++)
            {
                Legion.Instance.cInventory.dicInventory[_isNewSlots[i]].isNew = false;
            }
            _cHero.cObject.SetActive(true);
		    if(statPoint < statPointOrigin)
		    {
		    	PopupManager.Instance.ShowLoadingPopup(1);

		    	UInt16[] tmp = new UInt16[tempStatus.Length];

		    	for(int i=0; i<tempStatus.Length; i++){
		    		tmp[i] = (UInt16)tempStatus[i];
		    	}

		    	Server.ServerMgr.Instance.PointEquipmentStatus(_cBaseEquipItem, tmp, AckStatUp);
		    }
		    else
		    {
		    	characterInfo_equipment.OnClose_ItemList();
		    	RemoveEffects();
		    }        
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
