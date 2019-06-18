using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class UI_Panel_Forge_Fusion_Module : MonoBehaviour
{
    [SerializeField] Image _imgElementIcon;
	[SerializeField] Image _imgELement;
	[SerializeField] RectTransform _trNameGroup;
    [SerializeField] RectTransform _trStarGroup;
	[SerializeField] Text _txtTier;
	[SerializeField] Text _txtEquipName;
	[SerializeField] Text _txtCreator;
	[SerializeField] Text _txtLevel;
	[SerializeField] Image _imgExpGauge;
	[SerializeField] Text _txtExp;
    [SerializeField] Text _txtStatPoint;
    [SerializeField] Text _txtStatPointProgress;
    [SerializeField] GameObject _effStatPoint;
	[SerializeField] Text[] _txtStatType;
	[SerializeField] Text[] _txtStatValue;
	[SerializeField] UI_Button_CharacterInfo_Equipment_StatInfo[] _btnStatInfo;
    [SerializeField] GameObject[] Btn_StatUp;
    [SerializeField] GameObject[] Btn_StatUp10;
    [SerializeField] GameObject[] Btn_SkillUp;
	[SerializeField] GameObject[] _objSkill;
	[SerializeField] GameObject[] _objNoneSkill;
	[SerializeField] Image[] _imgSkillIcon;
	[SerializeField] Image[] _imgSkillElement;
	[SerializeField] Text[] _txtSkillValue;
	[SerializeField] UI_Button_CharacterInfo_Equipment_SkillInfo[] _btnSkillInfo;
	[SerializeField] GameObject[] _objStatUpIcon;
	[SerializeField] RectTransform _trMaterialScrollParent;
	[SerializeField] RectTransform _trMaterialListParent;
	[SerializeField] Text _txtSelectMaterialCount;
	[SerializeField] Text _txtGoods;
	public Color32 disableColor = new Color32(58,68,95,255);
	public Color32 enableColor = new Color32(255, 255, 255,255);

	ForgeInfo _cForgeInfo;
	EquipmentItem _cBaseEquipItem;
	EquipmentItem _cDummyEquipItem;
	List<UInt16> _lstSelectedItemSlotNum; // 합성재료로 쓰일 장비 목록
    List<UInt32> _lstSelectedItemCost;      //선택된 재료장비 비용 합
	[SerializeField] Transform _trEquipModelParent;
	GameObject _objEquipModel;
	GameObject _objEquipEffect;
	[SerializeField] SpriteRenderer _imgAccessory;
	[SerializeField] Image _imgAccessoryUI;
	[SerializeField] Transform _trEquipEffectParent;
    [SerializeField] GameObject _resultPanel;
    [SerializeField] GameObject _resultAni;
    [SerializeField] GameObject _panel;
    [SerializeField] UI_Button_CharacterInfo_Equipment_StateInfo _specializeBtn;
    [SerializeField] Text _equipmentClassName;
    [SerializeField] Image _imgClassIcon;
    //[SerializeField] GameObject Pref_Star;
    [SerializeField] GameObject starPos;

    Transform trsEquipmodelParent;
    StringBuilder tempStringBuilder;
    UInt64 u8CntGoods = 0;
	UInt64 u8OriGoods = 0;
    Byte currentPanel = 0;

    int statPoint;
	int statPointOrigin;
    private Byte[] statType;
	private UInt32[] tempStatus;
    private GameObject pointingEffect;
    private List<GameObject> listEffects = new List<GameObject>();

	public GameObject disObj;
	public DiscountUI disScript;

    public void ShowFusionResult()
	{
        Destroy(_cBaseEquipItem.cObject);
		_resultPanel.GetComponent<UI_Panel_Forge_Fusion_Module_Result>().SetData(_cBaseEquipItem, this);
		_resultPanel.SetActive(false);
		PopupManager.Instance.AddPopup(_resultPanel, _resultPanel.GetComponent<UI_Panel_Forge_Fusion_Module_Result>().OnClickClose);
        _panel.SetActive(false);
		StartCoroutine(showResult_BeforeAni());
	}

    IEnumerator showResult_BeforeAni()
	{
		_resultAni.SetActive(true);
		yield return new WaitForSeconds(0.75f);
		_resultPanel.SetActive(true);
		yield return new WaitForSeconds(1.1f);
		_resultAni.SetActive(false);
        PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.EquipLevel);
        PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.FuseEquip);
    }

	public void SetData(EquipmentItem baseEquipItem, Byte _scene)
	{
        PopupManager.Instance.AddPopup(this.gameObject, OnClickClose);
        if(pointingEffect == null)
			pointingEffect = AssetMgr.Instance.AssetLoad("Prefabs/UI_Effects/StatusPoint02.prefab", typeof(GameObject)) as GameObject;

        tempStringBuilder = new StringBuilder();
		_cForgeInfo = ForgeInfoMgr.Instance.GetList()[(Legion.Instance.u1ForgeLevel-1)];
		_cBaseEquipItem = baseEquipItem;
        currentPanel = _scene;
		_lstSelectedItemSlotNum = new List<UInt16>();
        _lstSelectedItemCost = new List<UInt32>();
		addExp = 0;
        addStatExp = 0;
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

        //trsEquipmodelParent = _cBaseEquipItem.cObject.transform.parent;
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        _imgElementIcon.sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.Append("Sprites/Common/common_02_renew.element_icon_").Append(_cBaseEquipItem.GetEquipmentInfo().u1Element).ToString());
		_imgElementIcon.SetNativeSize();

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        _imgELement.sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.Append("Sprites/Common/common_02_renew.element_").Append(_cBaseEquipItem.GetEquipmentInfo().u1Element).ToString());

        //_txtTier.text = "<" + TextManager.Instance.GetText("forge_level_" + _cBaseEquipItem.u1SmithingLevel) + ">";
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        _txtTier.text = TextManager.Instance.GetText(tempStringBuilder.Append("forge_level_").Append(_cBaseEquipItem.u1SmithingLevel).ToString());
		UIManager.Instance.SetGradientFromTier(_txtTier.GetComponent<Gradient>(), _cBaseEquipItem.u1SmithingLevel);
        //string equipName = "";
        //equipName = _cBaseEquipItem.itemName;
        //if (equipName != "")
        //	equipName = equipName + " " + TextManager.Instance.GetText( _cBaseEquipItem.GetEquipmentInfo().sName );
        //else
        //	equipName = TextManager.Instance.GetText( _cBaseEquipItem.GetEquipmentInfo().sName );
        //_txtEquipName.text = equipName;
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        if (_cBaseEquipItem.itemName != "")
            tempStringBuilder.Append(_cBaseEquipItem.itemName).Append(" ").Append(TextManager.Instance.GetText(_cBaseEquipItem.GetEquipmentInfo().sName));
        else
            tempStringBuilder.Append(TextManager.Instance.GetText(_cBaseEquipItem.GetEquipmentInfo().sName)).ToString();

        _txtEquipName.text = tempStringBuilder.ToString();
        _txtEquipName.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
		UIManager.Instance.SetGradientFromElement(_txtEquipName.GetComponent<Gradient>(), _cBaseEquipItem.GetEquipmentInfo().u1Element);

		//UIManager.Instance.SetSizeTextGroup(_trNameGroup, 18);
        UIManager.Instance.SetSizeTextGroup(_trStarGroup, 18);

		if(_cBaseEquipItem.createrName != "")
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            _txtCreator.text = tempStringBuilder.Append("By ").Append(_cBaseEquipItem.createrName).ToString();
        }
		else
			_txtCreator.text = "";

		_txtLevel.text = _cBaseEquipItem.cLevel.u2Level.ToString();
		_imgExpGauge.fillAmount = (float)((float)_cBaseEquipItem.cLevel.u8Exp / (float)_cBaseEquipItem.cLevel.u8NextExp);
		_txtExp.text = string.Format("{0}/{1}", ConvertExpValue(_cBaseEquipItem.cLevel.u8Exp), ConvertExpValue(_cBaseEquipItem.cLevel.u8NextExp));
        _txtStatPoint.text = baseEquipItem.statusComponent.UNSET_STATPOINT.ToString();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append("(");
        tempStringBuilder.Append((baseEquipItem.statusComponent.STATPOINT_EXP/StatusComponent.MAX_STATEXP_PROGRESS).ToString());
        tempStringBuilder.Append(".");
        if((baseEquipItem.statusComponent.STATPOINT_EXP%StatusComponent.MAX_STATEXP_PROGRESS) >= 10)
            tempStringBuilder.Append((baseEquipItem.statusComponent.STATPOINT_EXP%StatusComponent.MAX_STATEXP_PROGRESS).ToString().Remove(1));
        else
            tempStringBuilder.Append((baseEquipItem.statusComponent.STATPOINT_EXP%StatusComponent.MAX_STATEXP_PROGRESS).ToString());
        tempStringBuilder.Append("%)");
        _txtStatPointProgress.text = tempStringBuilder.ToString();

		for(Byte i=0; i<EquipmentInfo.ADD_STAT_TYPE_MAX; i++)
		{
			_txtStatType[i].text = Status.GetStatText( _cBaseEquipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType );
			_txtStatValue[i].text = (_cBaseEquipItem.cFinalStatus.GetStat( _cBaseEquipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType )).ToString(); //* forgeInfo.cSmithingInfo
			_btnStatInfo[i].SetData(_cBaseEquipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType);
		}

		if (_cBaseEquipItem.u1SmithingLevel != 0) {
			ForgeInfo smithingForgeInfo = ForgeInfoMgr.Instance.GetList () [(_cBaseEquipItem.u1SmithingLevel - 1)];
			Byte skillCount = (Byte)(smithingForgeInfo.cSmithingInfo.u1RandomSkillCount + smithingForgeInfo.cSmithingInfo.u1SelectSkillCount);
			for (Byte i=0; i<skillCount; i++) {
				if(_cBaseEquipItem.skillSlots [i] == 0)
				{
					_objSkill [i].SetActive (false);
					_objNoneSkill[i].SetActive(true);
					continue;
				}
				SkillInfo skillInfo = SkillInfoMgr.Instance.GetInfoBySlot (_cBaseEquipItem.GetEquipmentInfo ().u2ClassID, _cBaseEquipItem.skillSlots [i]);
				_imgSkillIcon [i].sprite = AtlasMgr.Instance.GetSprite (String.Format ("Sprites/Skill/Atlas_SkillIcon_{0}.{1}", skillInfo.u2ClassID, skillInfo.u2ID));
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                _imgSkillElement[i].sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.Append("Sprites/Common/common_02_renew.common_02_skill_element_").Append(skillInfo.u1Element).ToString());
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
				_txtSkillValue[i].text = tempStringBuilder.Append("+ ").Append(_cBaseEquipItem.GetPoints()[i]).ToString();
                _objSkill [i].SetActive (true);
				_objNoneSkill[i].SetActive(false);
				_btnSkillInfo[i].SetData(skillInfo);
			}
			for (Byte i=skillCount; i<EquipmentInfo.ADD_SKILL_MAX; i++) {
				_objSkill [i].SetActive (false);
				_objNoneSkill[i].SetActive(true);
			}
		}

		_txtGoods.text = "0";

		for(int i=0; i<transform.childCount; i++)
		{
			if(transform.FindChild("EquipmentObject") != null)
				DestroyObject(transform.FindChild("EquipmentObject").gameObject);
		}

		if(_cBaseEquipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 && _cBaseEquipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
		{
            //if(_cBaseEquipItem.cObject == null)
			    _cBaseEquipItem.InitViewModelObject();
            _cBaseEquipItem.cObject.GetComponent<EquipmentObject>().SetLayer( LayerMask.NameToLayer("UI") );
			_cBaseEquipItem.cObject.transform.SetParent( _trEquipModelParent );
			_trEquipModelParent.parent.GetComponent<RotateCharacter>().characterTransform = _cBaseEquipItem.cObject.transform.parent;
			_cBaseEquipItem.cObject.transform.localScale = new Vector3(0.85f, 0.85f, 0.85f);
			//_cBaseEquipItem.cObject.transform.localPosition = new Vector3(0, -120f, -500f);
            _cBaseEquipItem.cObject.transform.localPosition = Vector3.zero;
			_cBaseEquipItem.cObject.transform.name = "EquipmentObject";
			//_objEquipModel = _cBaseEquipItem.cObject;
			_imgAccessory.gameObject.SetActive(false);
		}
		else
		{
			UInt16 modelID = _cBaseEquipItem.u2ModelID;
			if(modelID == 0) modelID = _cBaseEquipItem.GetEquipmentInfo().u2ModelID;

            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            _imgAccessory.sprite = AssetMgr.Instance.AssetLoad(tempStringBuilder.Append("Sprites/Item/Accessory/acc_").Append(modelID).Append(".png").ToString(), typeof(Sprite)) as Sprite;
			_imgAccessory.gameObject.SetActive(true);
		}

		if(_objEquipEffect != null) DestroyObject(_objEquipEffect);
		if(_cBaseEquipItem.u1SmithingLevel >= 1)
		{
			DebugMgr.Log("Asset : " +string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}", _cBaseEquipItem.u1SmithingLevel));
			_objEquipEffect = Instantiate(AssetMgr.Instance.AssetLoad(string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}.prefab", _cBaseEquipItem.u1SmithingLevel), typeof(GameObject))) as GameObject;
			_objEquipEffect.transform.SetParent(_trEquipEffectParent);
			_objEquipEffect.transform.name = "WeaponEffect";
			_objEquipEffect.transform.localScale = new Vector3(1f, 1f, 1f);
			_objEquipEffect.transform.localPosition = Vector3.zero;
		}

        _cDummyEquipItem = new EquipmentItem(_cBaseEquipItem);
        _cDummyEquipItem.u1SmithingLevel = _cBaseEquipItem.u1SmithingLevel;
		_cBaseEquipItem.CopyStatus(_cDummyEquipItem);
        prevDummyLv = _cDummyEquipItem.cLevel.u2Level;

		InitMaterialItemList();
		_txtSelectMaterialCount.text = "0/" + listElementCount;

        SetStatButtons();
	}

	UInt64 GetPrice(UInt64 price){
		EventDisCountinfo disInfo = EventInfoMgr.Instance.GetDiscountEventinfo (DISCOUNT_ITEM.FUSION);
		if (disInfo != null) {
			disObj.SetActive (true);
			disScript.SetData ((uint)price, disInfo.u1DiscountRate);
			price = (uint)(price * disInfo.discountRate);
		} else {
			disObj.SetActive (false);
		}

		return price;
	}

	int listElementCount = 0;

	// 합성재료로 쓰일 장비목록 초기화
	void InitMaterialItemList()
	{
		int listHeight = 0;
		listElementCount = 0;
		for(int i=0; i<_trMaterialListParent.childCount; i++)
		{
			DestroyObject(_trMaterialListParent.GetChild(i).gameObject);
		}

		Legion.Instance.cInventory.EquipSort();
		for(int i=0; i<Legion.Instance.cInventory.lstSortedEquipment.Count; i++)
		{
			EquipmentItem equipItem = Legion.Instance.cInventory.lstSortedEquipment[i];
			// 베이스장비, 현재 장착중인 장비, 경험치가 없는장비(레벨1의 경험치0)는 제외
			//if(equipItem != _cBaseEquipItem && equipItem.attached.hero == null && 
			//   !(equipItem.cLevel.u2Level == 1 && equipItem.cLevel.u8Exp == 0) )
            if(equipItem.u1RoomNum != 0)
                continue;
            if(equipItem != _cBaseEquipItem && equipItem.attached.hero == null)
			{
				GameObject listElement = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Equipment/Pref_UI_ListElement_Forge_Fusion_Material.prefab", typeof(GameObject))) as GameObject;
				Transform listElementTr = listElement.transform;
				listElementTr.SetParent( _trMaterialListParent );
				listElementTr.name = equipItem.u2SlotNum.ToString();
				listElementTr.localScale = Vector3.one;
				listElementTr.localPosition = Vector3.zero;

				listElement.GetComponent<UI_ListElement_Forge_Fusion_Material>().SetData(equipItem, null);
				int slotNum = 0;
				slotNum = equipItem.u2SlotNum;
				listElement.GetComponent<Button>().onClick.AddListener( () => OnClickEquipmentItem(slotNum) );
				listElement.GetComponent<ButtonSound>().buttonClip = Resources.Load("Sound/UI/01. Common/UI_Button_2", typeof(AudioClip)) as AudioClip;
				listElementCount++;
			}
		}
		DebugMgr.Log("S : " + _trMaterialListParent.childCount + " C : " + (16-_trMaterialListParent.childCount));
		DebugMgr.Log("Row : " + listHeight);
        
        if((_trMaterialListParent.transform.childCount / 4) == 0)
            listHeight = _trMaterialListParent.transform.childCount / 4;
        else
            listHeight = (_trMaterialListParent.transform.childCount / 4) + 1;
		//_trMaterialScrollParent.sizeDelta = new Vector2(540, 148f*listHeight);
		//_trMaterialScrollParent.FindChild("ListParent").GetComponent<RectTransform>().sizeDelta = new Vector2(540, 148f*listHeight);
		_trMaterialScrollParent.anchoredPosition = new Vector2(_trMaterialScrollParent.localPosition.x, 0f);
        _trMaterialScrollParent.sizeDelta = new Vector2(540, 148f*listHeight);
	}
    UInt16 prevDummyLv = 0;
    UInt16 nextDummyLv = 0;
	void DiffStat()
	{
		if(_lstSelectedItemSlotNum.Count == 0)
		{
			for(Byte i=0; i<EquipmentInfo.ADD_STAT_TYPE_MAX; i++)
			{
				UInt32 baseValue = (UInt32)(_cBaseEquipItem.cFinalStatus.GetStat( _cBaseEquipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType ));
				_txtStatValue[i].text = String.Format("{0}", baseValue);
				_txtLevel.text = _cBaseEquipItem.cLevel.u2Level.ToString();
				_imgExpGauge.fillAmount = (float)((float)_cBaseEquipItem.cLevel.u8Exp / (float)_cBaseEquipItem.cLevel.u8NextExp);
				_txtExp.text = string.Format("{0}/{1}", ConvertExpValue(_cBaseEquipItem.cLevel.u8Exp), ConvertExpValue(_cBaseEquipItem.cLevel.u8NextExp));
                //_txtStatPoint.text = _cBaseEquipItem.statusComponent.STAT_POINT.ToString();
                _txtStatPoint.text = _cBaseEquipItem.statusComponent.UNSET_STATPOINT.ToString();
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append("(");
                tempStringBuilder.Append((_cBaseEquipItem.statusComponent.STATPOINT_EXP/StatusComponent.MAX_STATEXP_PROGRESS).ToString());
                tempStringBuilder.Append(".");
                if((_cBaseEquipItem.statusComponent.STATPOINT_EXP%StatusComponent.MAX_STATEXP_PROGRESS) >= 10)
                    tempStringBuilder.Append((_cBaseEquipItem.statusComponent.STATPOINT_EXP%StatusComponent.MAX_STATEXP_PROGRESS).ToString().Remove(1));
                else
                    tempStringBuilder.Append((_cBaseEquipItem.statusComponent.STATPOINT_EXP%StatusComponent.MAX_STATEXP_PROGRESS).ToString());
                tempStringBuilder.Append("%)");
                _txtStatPointProgress.text = tempStringBuilder.ToString();
				_objStatUpIcon[i].SetActive(false);
			}
            _txtGoods.text = "0";
			disObj.SetActive (false);
			return;
		}
		else 
		{
			// 레벨업시 능력치 상승 시뮬레이션을 위해, 현재 장비의 더미를 생성
			//_cDummyEquipItem = new EquipmentItem(_cBaseEquipItem.cItemInfo.u2ID);
            _cDummyEquipItem = new EquipmentItem(_cBaseEquipItem);
            _cDummyEquipItem.u1SmithingLevel = _cBaseEquipItem.u1SmithingLevel;
			_cBaseEquipItem.CopyStatus(_cDummyEquipItem);
            //prevDummyLv = _cDummyEquipItem.cLevel.u2Level;
			_cDummyEquipItem.GetComponent<LevelComponent>().AddExp(addExp); // addExp는 재료가 될 장비를 선택 or 해제할때 누적되는 경험치 값
            
            nextDummyLv = _cDummyEquipItem.cLevel.u2Level;

            if(_cBaseEquipItem.attached.hero != null)
            {
                if(nextDummyLv > _cBaseEquipItem.attached.hero.cLevel.u2Level)
                {
                    _cDummyEquipItem.GetComponent<LevelComponent>().Set(_cBaseEquipItem.attached.hero.cLevel.u2Level, _cBaseEquipItem.attached.hero.cLevel.u8NextExp-1);
                    nextDummyLv = _cDummyEquipItem.cLevel.u2Level;
                    _txtLevel.text = _cDummyEquipItem.cLevel.u2Level.ToString();
                    _imgExpGauge.fillAmount = (float)((float)_cDummyEquipItem.cLevel.u8Exp / (float)_cDummyEquipItem.cLevel.u8NextExp);
			        _txtExp.text = string.Format("{0}/{1}", ConvertExpValue(_cDummyEquipItem.cLevel.u8Exp), ConvertExpValue(_cDummyEquipItem.cLevel.u8NextExp));
                }
                else
                {
                    _txtLevel.text = nextDummyLv.ToString();
                    _imgExpGauge.fillAmount = (float)((float)_cDummyEquipItem.cLevel.u8Exp / (float)_cDummyEquipItem.cLevel.u8NextExp);
			        _txtExp.text = string.Format("{0}/{1}", ConvertExpValue(_cDummyEquipItem.cLevel.u8Exp), ConvertExpValue(_cDummyEquipItem.cLevel.u8NextExp));
                }
            }
            else
            {
			    _txtLevel.text = _cDummyEquipItem.cLevel.u2Level.ToString();
                _imgExpGauge.fillAmount = (float)((float)_cDummyEquipItem.cLevel.u8Exp / (float)_cDummyEquipItem.cLevel.u8NextExp);
			    _txtExp.text = string.Format("{0}/{1}", ConvertExpValue(_cDummyEquipItem.cLevel.u8Exp), ConvertExpValue(_cDummyEquipItem.cLevel.u8NextExp));
            }
            
            _cDummyEquipItem.statusComponent.StatPointExpUp(addStatExp, _cDummyEquipItem.cLevel.u2Level);
			//_txtStatPoint.text = (_cDummyEquipItem.statusComponent.STAT_POINT /*+ _cBaseEquipItem.statusComponent.STAT_POINT*/).ToString();
            _txtStatPoint.text = (_cDummyEquipItem.statusComponent.UNSET_STATPOINT).ToString();
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append("(");
            tempStringBuilder.Append((_cDummyEquipItem.statusComponent.STATPOINT_EXP/StatusComponent.MAX_STATEXP_PROGRESS).ToString());
            tempStringBuilder.Append(".");
            if((_cDummyEquipItem.statusComponent.STATPOINT_EXP%StatusComponent.MAX_STATEXP_PROGRESS) >= 10)
                tempStringBuilder.Append((_cDummyEquipItem.statusComponent.STATPOINT_EXP%StatusComponent.MAX_STATEXP_PROGRESS).ToString().Remove(1));
            else
                tempStringBuilder.Append((_cDummyEquipItem.statusComponent.STATPOINT_EXP%StatusComponent.MAX_STATEXP_PROGRESS).ToString());
            tempStringBuilder.Append("%)");
            _txtStatPointProgress.text = tempStringBuilder.ToString();

			for(Byte i=0; i<EquipmentInfo.ADD_STAT_TYPE_MAX; i++)
			{
				UInt32 baseValue = (UInt32)(_cBaseEquipItem.cStatus.GetStat( _cBaseEquipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType, _cBaseEquipItem.GetPoints()[i + Server.ConstDef.SkillOfEquip] ));
				UInt32 dummyValue = (UInt32)(_cDummyEquipItem.cStatus.GetStat( _cDummyEquipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType, _cDummyEquipItem.GetPoints()[i + Server.ConstDef.SkillOfEquip] ));
				if(dummyValue-baseValue > 0)
				{
					_txtStatValue[i].text = String.Format("{0}", dummyValue);
					_objStatUpIcon[i].SetActive(true);
				}
				else
				{
					_txtStatValue[i].text = String.Format("{0}", baseValue);
					_objStatUpIcon[i].SetActive(false);
				}
			}
		}
	}

	// 재료가 될 장비를 클릭했을때
    UInt16 invenSlotNum;
	public void OnClickEquipmentItem(int slotNum)
	{
		invenSlotNum = (UInt16)slotNum;
        RequestStatUp();
	}
    private void ClickEquipmentItem()
    {
        if(!_lstSelectedItemSlotNum.Contains(invenSlotNum))		// 아직 선택하지 않은 장비
		{
			if(_cBaseEquipItem.attached.hero != null) // 베이스 장비가 현재 장착중일 경우, 베이스+재료의 결과가 장착중인 영웅보다 높으면 경고창 띄움
			{
				if(CheckAddMaterial(invenSlotNum)) // 경험치를 누적해도 되는지 판단
				{
					AddMaterial(invenSlotNum);
					_trMaterialListParent.FindChild(invenSlotNum.ToString()).GetComponent<UI_ListElement_Forge_Fusion_Material>().Select();
                    tempStringBuilder.Remove(0, tempStringBuilder.Length);
                    _txtSelectMaterialCount.text = tempStringBuilder.Append(_lstSelectedItemSlotNum.Count).Append("/").Append(listElementCount).ToString(); 
				}
				else
				{
					PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_equip_mix_over_lv"), TextManager.Instance.GetText("popup_desc_equip_mix_over_lv"), null);
					return;
				}
			}
			else
			{
				AddMaterial(invenSlotNum);
				_trMaterialListParent.FindChild(invenSlotNum.ToString()).GetComponent<UI_ListElement_Forge_Fusion_Material>().Select();
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                _txtSelectMaterialCount.text = tempStringBuilder.Append(_lstSelectedItemSlotNum.Count).Append("/").Append(listElementCount).ToString(); 
			}
		}
		else
		{
			SubMaterial(invenSlotNum);
			_trMaterialListParent.FindChild(invenSlotNum.ToString()).GetComponent<UI_ListElement_Forge_Fusion_Material>().DeSelect();

            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            _txtSelectMaterialCount.text = tempStringBuilder.Append(_lstSelectedItemSlotNum.Count).Append("/").Append(listElementCount).ToString(); 
		}
    }
	// 베이스 장비 + 재료장비 경험치를 합산했을 때, 베이스 장비 레벨이 착용한 영웅보다 레벨이 높아지는지 체크
	// 높지않으면 true 리턴(합성재료로 쓸 수 있다는 표시)
	bool CheckAddMaterial(UInt16 invenSlotNum)
	{
		bool ret = false;
		// 선택장비 누적경험치
		EquipmentItem matEquipItem = (EquipmentItem)Legion.Instance.cInventory.dicInventory[invenSlotNum];
		UInt64 matEquipExp = 0;
		if(matEquipItem.cLevel.u2Level > 0)
            if(matEquipItem.cLevel.u2Level > 1)
			    //matEquipExp = ClassInfoMgr.Instance.GetAccExp((Byte)(matEquipItem.cLevel.u2Level-1));
                matEquipExp = ClassInfoMgr.Instance.GetAccExp((Byte)(matEquipItem.cLevel.u2Level));
		matEquipExp += matEquipItem.cLevel.u8Exp;
		DebugMgr.Log("MAT EXP : " + matEquipExp);

		// 베이스 장비 누적경험치
		UInt64 baseEquipExp=0;
		if(_cBaseEquipItem.cLevel.u2Level > 0)
            if(_cBaseEquipItem.cLevel.u2Level > 1)
			    //baseEquipExp = ClassInfoMgr.Instance.GetAccExp((Byte)(_cBaseEquipItem.cLevel.u2Level-1));
                baseEquipExp = ClassInfoMgr.Instance.GetAccExp((Byte)(_cBaseEquipItem.cLevel.u2Level));
		baseEquipExp += _cBaseEquipItem.cLevel.u8Exp;

		// 캐릭터 레벨업 조건
		UInt64 maxExp = ClassInfoMgr.Instance.GetAccExp( _cBaseEquipItem.attached.hero.cLevel.u2Level );
		DebugMgr.Log("MAX EXP : " + maxExp);
		DebugMgr.Log("ADD EXP : " + addExp);
		DebugMgr.Log("ACC EXP : " + (matEquipExp + baseEquipExp + addExp));
		if(maxExp > matEquipExp + baseEquipExp + addExp)
		{
			ret = true;
		}
		else
		{
            if((_cBaseEquipItem.statusComponent.STAT_POINT + _cBaseEquipItem.statusComponent.USE_POINT) 
                < EquipmentInfoMgr.Instance.u2EquipStatPointTotalMax)
                ret = true;
            else
			    ret = false;
		}
		return ret;
	}

	UInt64 addExp = 0; // 재료 장비에서 추출한 누적경험치 합
    UInt16 addStatExp = 0; //재료장비 티어에 따른 스텟 경험치 합
	public void AddMaterial(UInt16 invenSlotNum)
	{
		EquipmentItem equipItem = (EquipmentItem)Legion.Instance.cInventory.dicInventory[invenSlotNum];

        if(equipItem.cLevel.u2Level>1)
		    addExp += ClassInfoMgr.Instance.GetAccExp((Byte)(equipItem.cLevel.u2Level-1));
		addExp += equipItem.cLevel.u8Exp;
        addExp += ForgeInfoMgr.Instance.GetInfo(ForgeInfoMgr.Instance.GetIDs()[equipItem.u1SmithingLevel-1]).u4FusionExp;

        int tempSmithingLvl = _cBaseEquipItem.u1SmithingLevel - equipItem.u1SmithingLevel;
        if (tempSmithingLvl > 0)
            addStatExp += (UInt16)(EquipmentInfoMgr.Instance.u2BaseStatPointExp + tempSmithingLvl*(EquipmentInfoMgr.Instance.MinusStatPointExp));
        else if(tempSmithingLvl < 0)
            addStatExp += (UInt16)(EquipmentInfoMgr.Instance.u2BaseStatPointExp + tempSmithingLvl*(EquipmentInfoMgr.Instance.PlusStatPointExp)*-1);
        else
            addStatExp += EquipmentInfoMgr.Instance.u2BaseStatPointExp;

		if(_lstSelectedItemSlotNum == null)
			_lstSelectedItemSlotNum = new List<UInt16>();
		_lstSelectedItemSlotNum.Add(invenSlotNum);

		DiffStat();
        //if(nextDummyLv - prevDummyLv == 0)
        //{
            //u8CntGoods += (UInt64)_cForgeInfo.cFusionGoods.u4Count;
            //u8CntGoods += ForgeInfoMgr.Instance.GetInfo(ForgeInfoMgr.Instance.GetIDs()[equipItem.u1SmithingLevel-1]).cFusionGoods.u4Count;
        
        Double dExpCost = (equipItem.cLevel.u8Exp + ClassInfoMgr.Instance.GetAccExp((UInt16)(equipItem.cLevel.u2Level-1))) * 0.004f;
        Double dGoodsFactor = ClassInfoMgr.Instance.GetCostFacter(equipItem.cLevel.u2Level);
        Double dCntGoods = (ForgeInfoMgr.Instance.GetInfo(ForgeInfoMgr.Instance.GetIDs()[equipItem.u1SmithingLevel-1]).cFusionGoods.u4Count * equipItem.cLevel.u2Level);
		UInt64 orginPrice = Convert.ToUInt64((UInt64)(dGoodsFactor*dExpCost) + dCntGoods);
		u8OriGoods += orginPrice;
		u8CntGoods = GetPrice(u8OriGoods);
		/*
		double price = (double)ContentsData.Manager.smithLevels[equipSmithinglevel].FusionCost.number * (double)equipLevel;
		price += (double)equipExp * 0.004 * ContentsData.Manager.levelInfo[equipLevel].equipShopCostFacter;
		*/
        //}
        //else
        //{
        //    u8CntGoods += (UInt64)(_cForgeInfo.cFusionGoods.u4Count*((nextDummyLv - prevDummyLv) + 1));
        //}
        prevDummyLv = _cDummyEquipItem.cLevel.u2Level;
		_txtGoods.text = u8CntGoods.ToString();
		//_txtGoods.text = (_cForgeInfo.cFusionGoods.u4Count * _lstSelectedItemSlotNum.Count).ToString();
	}

	public void SubMaterial(UInt16 invenSlotNum)
	{
		EquipmentItem equipItem = (EquipmentItem)Legion.Instance.cInventory.dicInventory[invenSlotNum];
		
        if(equipItem.cLevel.u2Level>1)
		    addExp -= ClassInfoMgr.Instance.GetAccExp((Byte)(equipItem.cLevel.u2Level-1));
		addExp -= equipItem.cLevel.u8Exp;
        addExp -= ForgeInfoMgr.Instance.GetInfo(ForgeInfoMgr.Instance.GetIDs()[equipItem.u1SmithingLevel-1]).u4FusionExp;

        int tempSmithingLvl = _cBaseEquipItem.u1SmithingLevel - equipItem.u1SmithingLevel;
        if (tempSmithingLvl > 0)
            addStatExp -= (UInt16)(EquipmentInfoMgr.Instance.u2BaseStatPointExp + tempSmithingLvl*(EquipmentInfoMgr.Instance.MinusStatPointExp));
        else if(tempSmithingLvl < 0)
            addStatExp -= (UInt16)(EquipmentInfoMgr.Instance.u2BaseStatPointExp + tempSmithingLvl*(EquipmentInfoMgr.Instance.PlusStatPointExp)*-1);
        else
            addStatExp -= EquipmentInfoMgr.Instance.u2BaseStatPointExp;
        
		_lstSelectedItemSlotNum.Remove(invenSlotNum);
		DiffStat();
        //if(prevDummyLv - nextDummyLv == 0)
        //{
            //u8CntGoods -= (UInt64)_cForgeInfo.cFusionGoods.u4Count;
            //u8CntGoods -= ForgeInfoMgr.Instance.GetInfo(ForgeInfoMgr.Instance.GetIDs()[equipItem.u1SmithingLevel-1]).cFusionGoods.u4Count;
        Double dExpCost = (equipItem.cLevel.u8Exp + ClassInfoMgr.Instance.GetAccExp((UInt16)(equipItem.cLevel.u2Level-1))) * 0.004f;
        Double dGoodsFactor = ClassInfoMgr.Instance.GetCostFacter(equipItem.cLevel.u2Level);
        Double dCntGoods = (ForgeInfoMgr.Instance.GetInfo(ForgeInfoMgr.Instance.GetIDs()[equipItem.u1SmithingLevel-1]).cFusionGoods.u4Count * equipItem.cLevel.u2Level);
		UInt64 orginPrice = Convert.ToUInt64((UInt64)(dGoodsFactor*dExpCost) + dCntGoods);
		u8OriGoods -= orginPrice;
		u8CntGoods = GetPrice(u8OriGoods);
        //}
        //else
        //{
        //    u8CntGoods -= (UInt64)(_cForgeInfo.cFusionGoods.u4Count*((prevDummyLv - nextDummyLv) + 1));
        //}

        _txtGoods.text = u8CntGoods.ToString();
        prevDummyLv = _cDummyEquipItem.cLevel.u2Level;
		//_txtGoods.text = (_cForgeInfo.cFusionGoods.u4Count * _lstSelectedItemSlotNum.Count).ToString();
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
		_txtStatValue[index-Server.ConstDef.SkillOfEquip].text = _cBaseEquipItem.cStatus.GetStat(statType[index-Server.ConstDef.SkillOfEquip], tempStatus[index]).ToString();
		
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

    private void RequestStatUp()
    {
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
            ClickEquipmentItem();
        }
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
			
			//statPoint = _cBaseEquipItem.u1StatPoint;
            //statPoint = _cBaseEquipItem.statusComponent.UNSET_STATPOINT;
            
			Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.EquipStatPoint, 0, 0, 0, 0, (uint)(statPointOrigin - statPoint));
            
			statPointOrigin = statPoint;
			_txtStatPoint.text = statPoint.ToString();
            _cBaseEquipItem.statusComponent.UNSET_STATPOINT = (UInt16)statPoint;
			
			SetStatButtons();    

			RemoveEffects();

            ClickEquipmentItem();

            PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.EquipStatPoint);
        }
	}

    public void SetStatButtons()
	{
		_txtStatPoint.text = statPoint.ToString();

		//스탯 포인트 없으면 찍기 비활성화
		if(statPoint == 0)
		{
            _effStatPoint.SetActive(false);
			for(int i=0; i<Server.ConstDef.SkillOfEquip; i++)
			{
                Btn_StatUp[i].SetActive(false);
                Btn_SkillUp[i].SetActive(false);
                Btn_StatUp10[i].SetActive(false);
                
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
			_effStatPoint.SetActive(true);
			for(int i=0; i<Server.ConstDef.SkillOfEquip; i++)
			{
                Btn_StatUp[i].SetActive(true);
                Btn_SkillUp[i].SetActive(true);
                if(statPoint > 19)
                    Btn_StatUp10[i].SetActive(true);
                else
                    Btn_StatUp10[i].SetActive(false);

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
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                _imgSkillIcon[i].sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.Append("Sprites/Skill/Atlas_SkillIcon_").Append(_cBaseEquipItem.GetEquipmentInfo().u2ClassID).Append(".").Append(skillInfo.u2ID).ToString());
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                _imgSkillElement[i].sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.Append("Sprites/Common/common_02_renew.common_02_skill_element_").Append(skillInfo.u1Element).ToString());

                tempStringBuilder.Remove(0, tempStringBuilder.Length);
				_txtSkillValue[i].text = tempStringBuilder.Append("+ ").Append(tempStatus[i]).ToString();
				_btnSkillInfo[i].SetData(skillInfo); 
			}
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
    private void RemoveEffects()
    {
        for(int i=0; i<listEffects.Count; i++)
        {
            if(listEffects[i] != null)
                Destroy(listEffects[i]);
        }
        
        listEffects.Clear();
    }   

	UInt16 seqNo = 0;
	public void OnClickFusion()
	{
        bool equipmentTierLv = false;

		if(Legion.Instance.Gold < (UInt64)(u8CntGoods))
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_equip_mix"), TextManager.Instance.GetText("popup_desc_equip_mix_not_enough_gold"), emptyMethod);
			return;
		}
		else if(_lstSelectedItemSlotNum.Count == 0)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_equip_mix"), TextManager.Instance.GetText("popup_desc_equip_mix_not_select_material"), emptyMethod);
			return;
		}

        for(int i=0; i<_lstSelectedItemSlotNum.Count; i++)
        {
            if(((EquipmentItem)Legion.Instance.cInventory.dicInventory[_lstSelectedItemSlotNum[i]]).u1SmithingLevel > 2)
            {
                EquipmentTier3Up();
                equipmentTierLv = true;
                break;
            }
            else
                continue;
        }

		if(!equipmentTierLv)
		{
			PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("popup_title_equip_mix"), TextManager.Instance.GetText("popup_desc_equip_mix"), RequestFusion, null);
		}
	}

    void EquipmentTier3Up()
    {
        PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("popup_title_equip_mix"), TextManager.Instance.GetText("popup_3tier_over_mix"), RequestFusion, null);
        return;
    }
	public void emptyMethod(object[] param)
	{

	}
	
	public void RequestFusion(object[] param)
	{
        if(statPoint < statPointOrigin)
		{
			PopupManager.Instance.ShowLoadingPopup(1);

			UInt16[] tmp = new UInt16[tempStatus.Length];

			for(int i=0; i<tempStatus.Length; i++){
				tmp[i] = (UInt16)tempStatus[i];
			}

			Server.ServerMgr.Instance.PointEquipmentStatus(_cBaseEquipItem, tmp, AckStatUpOnFusion);
		}
        else
        {
            PopupManager.Instance.ShowLoadingPopup(1);
		    seqNo = Server.ServerMgr.Instance.EquipmentFusion(_cBaseEquipItem, _lstSelectedItemSlotNum.ToArray(), ResponseFusion);
        }
	}
    private void AckStatUpOnFusion(Server.ERROR_ID err)
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
            statPointOrigin = statPoint;
            _cBaseEquipItem.statusComponent.UNSET_STATPOINT = (UInt16)statPoint;
			PopupManager.Instance.ShowLoadingPopup(1);
		    seqNo = Server.ServerMgr.Instance.EquipmentFusion(_cBaseEquipItem, _lstSelectedItemSlotNum.ToArray(), ResponseFusion);
		}
	}
	public void ResponseFusion(Server.ERROR_ID err)
	{
		if(err != Server.ERROR_ID.NONE)
		{
			DebugMgr.Log("FusionFailed");
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.FORGE_FUSION, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
			DebugMgr.Log("FusionSuccess");
			PopupManager.Instance.CloseLoadingPopup();

			//결과 연출
            InitMaterialItemList();
            ShowFusionResult();
		}
	}

	public void OnClickClose()
	{
        //DestroyObject(_objEquipModel);
        //DestroyObject(_cBaseEquipItem.cObject);
        //_cBaseEquipItem.cObject.transform.SetParent(trsEquipmodelParent);
        if(statPoint < statPointOrigin)
		{
			PopupManager.Instance.ShowLoadingPopup(1);

			UInt16[] tmp = new UInt16[tempStatus.Length];

			for(int i=0; i<tempStatus.Length; i++){
				tmp[i] = (UInt16)tempStatus[i];
			}

			Server.ServerMgr.Instance.PointEquipmentStatus(_cBaseEquipItem, tmp, AckStatUpOnClose);
		}
        else
        {
            Close();
        }
	}

    private void Close()
    {
        if(currentPanel == 1)
        {
            InventoryPanel inventoryPanel = Scene.GetCurrent().inventoryPanel;
            if (inventoryPanel != null)
                if(inventoryPanel.gameObject.activeSelf)
                    inventoryPanel.RefreshSlot();
        }
        else if(currentPanel == 2)
        {
            LobbyScene lobbyScene = Scene.GetCurrent().GetComponent<LobbyScene>();
            if (lobbyScene != null)
            {
                lobbyScene._characterInfo.GetPanelEquipment().InitEquipSlots();
                lobbyScene._characterInfo.GetPanelEquipment().ReloadPowerValue();
            }
        }
        
        PopupManager.Instance.RemovePopup(this.gameObject);
        Destroy(this.gameObject);
        //gameObject.SetActive(false);
    }
    private void AckStatUpOnClose(Server.ERROR_ID err)
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
            statPointOrigin = statPoint;
            _cBaseEquipItem.statusComponent.UNSET_STATPOINT = (UInt16)statPoint;
            Close();
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
}
