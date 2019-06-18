using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
public class UI_SubPanel_Forge_Smithing_SelectSkill : MonoBehaviour {
	[SerializeField] Sprite _DeactiveButtonBGPath;
	[SerializeField] Sprite _ActiveButtonBGPath;
	[SerializeField] Image _imgActiveSkillButton;
	[SerializeField] Image _imgPassiveSkillButton;

	[SerializeField] RectTransform _nameGroup;
	[SerializeField] Text _txtEquipName;
    [SerializeField] Text _txtClassName;
	[SerializeField] Text _txtSelectCountInfo;
	[SerializeField] RectTransform _trListParent;
	[SerializeField] Image _imgSkillIcon;
	[SerializeField] Image _imgElement;
	[SerializeField] Text _txtSkillName;
//	[SerializeField] Text _txtSkillType;
	[SerializeField] Text _txtSkillDescription;

	UInt16 _u2ClassID;
	UI_Panel_Forge_Smithing_Detail _cParent;
	Byte _u1CurrentSelectSkillSlot;
	Byte _u1MaxSkillCount;
	List<Byte> _lstSelectedSkillSlot = null;
	List<SkillInfo> lstSkillInfo;
	public void SetData(UInt16 classID, Byte maxSkillCount, UI_Panel_Forge_Smithing_Detail parent)
	{
		_lstSelectedSkillSlot = new List<byte> ();

		_u2ClassID = classID;
		_cParent = parent;
		_u1MaxSkillCount = maxSkillCount;

		_txtEquipName.text = "  <" + TextManager.Instance.GetText( parent._cEquipInfo.sName ) + ">";
		UIManager.Instance.SetSizeTextGroup(_nameGroup);

		_txtSelectCountInfo.text = String.Format (
			TextManager.Instance.GetText ("forge_smithing_skillcount"),
            "",
			//TextManager.Instance.GetText (ClassInfoMgr.Instance.GetInfo (_u2ClassID).sName),
			(_u1MaxSkillCount - _lstSelectedSkillSlot.Count) + "/" + _u1MaxSkillCount
		);
        _txtClassName.text = TextManager.Instance.GetText (ClassInfoMgr.Instance.GetInfo (_u2ClassID).sName);
		for(int i=0; i<_trListParent.childCount; i++)
		{
			DestroyObject(_trListParent.GetChild(i).gameObject);
		}
		lstSkillInfo = SkillInfoMgr.Instance.GetInfoListByClass(classID);
		for(int i=0; i<lstSkillInfo.Count; i++)
		{
			GameObject listElement = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Equipment/Pref_UI_ListElement_Forge_Smithing_Skill.prefab", typeof(GameObject))) as GameObject;
			RectTransform trListElement = listElement.GetComponent<RectTransform>();
			trListElement.SetParent(_trListParent);
			trListElement.localScale = Vector3.one;
			trListElement.localPosition = Vector3.zero;
			trListElement.name = lstSkillInfo[i].u1SlotNum.ToString();
			trListElement.FindChild("Img_SkillIcon").GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Skill/Atlas_SkillIcon_" + classID + "." + lstSkillInfo[i].u2ID);
			trListElement.FindChild("Img_Element").GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite(string.Format("Sprites/Common/common_02_renew.common_02_skill_element_{0}", lstSkillInfo[i].u1Element));
			trListElement.FindChild("Text_Name").GetComponent<Text>().text = TextManager.Instance.GetText( lstSkillInfo[i].sName );
			if(lstSkillInfo[i].u1Element != 5)
				trListElement.FindChild("Text_Name").GetComponent<Text>().color = EquipmentItem.equipElementColors[ (lstSkillInfo[i].u1Element) ];
			int skillSlot = 0;
			skillSlot = (int)lstSkillInfo[i].u1SlotNum;
			listElement.GetComponent<Button>().onClick.AddListener( () => OnClickSkillListElement(skillSlot) );

			trListElement.FindChild("Selected").gameObject.SetActive(false);
//			if (_lstSelectedSkillSlot != null)
//			{
//				if(_lstSelectedSkillSlot.Contains((Byte)skillSlot))
//				{
//					trListElement.FindChild("Selected").gameObject.SetActive(true);
//				}
//				else
//				{
//					trListElement.FindChild("Selected").gameObject.SetActive(false);
//				}
//			}

		}
		//SetSkillInfoData(lstSkillInfo[0].u1SlotNum);
		OnClick_ActiveSkill();
		SetSkillInfoData(lstSkillInfo[0].u1SlotNum);
	}

	public void OnClick_ActiveSkill()
	{
		_imgActiveSkillButton.sprite = _ActiveButtonBGPath;
		_imgPassiveSkillButton.sprite = _DeactiveButtonBGPath;
		FilterActiveSkill();
		
	}
	public void OnClick_PassiveSkill()
	{
		_imgActiveSkillButton.sprite = _DeactiveButtonBGPath;
		_imgPassiveSkillButton.sprite = _ActiveButtonBGPath;
		FilterPassiveSkill();
	}

	void FilterActiveSkill()
	{
		int activeCount=0;
		for(int i=0; i<_trListParent.childCount; i++)
		{
			SkillInfo skillInfo = SkillInfoMgr.Instance.GetInfoBySlot(_u2ClassID, Byte.Parse(_trListParent.GetChild(i).name));
			if(skillInfo.u1ActWay == 1)
			{
				_trListParent.FindChild(skillInfo.u1SlotNum.ToString()).gameObject.SetActive(true);
				activeCount++;
			}
			else
			{
				_trListParent.FindChild(skillInfo.u1SlotNum.ToString()).gameObject.SetActive(false);
			}
		}

		_trListParent.sizeDelta = new Vector2(660, 78f*(activeCount/2));

	}

	void FilterPassiveSkill()
	{
        int passiveCount =0;
		for(int i=0; i<_trListParent.childCount; i++)
		{
			SkillInfo skillInfo = SkillInfoMgr.Instance.GetInfoBySlot(_u2ClassID, Byte.Parse(_trListParent.GetChild(i).name));
			if(skillInfo.u1ActWay == 2)
			{
                _trListParent.FindChild(skillInfo.u1SlotNum.ToString()).gameObject.SetActive(true);
				passiveCount++;
			}
			else
			{
				_trListParent.FindChild(skillInfo.u1SlotNum.ToString()).gameObject.SetActive(false);
			}
		}
		
		_trListParent.sizeDelta = new Vector2(660, 78f*(passiveCount/2));
		SetSkillInfoData(0);
	}

	void OnClickSkillListElement(int nSkillSlot)
	{
		Byte skillSlot = (Byte)nSkillSlot;
		_u1CurrentSelectSkillSlot = skillSlot;
		SetSkillInfoData(skillSlot);
		OnClickSelect();
	}

	public void SetSkillInfoData(Byte paramSkillSlot)
	{
        //Byte skillSlot = (Byte)paramSkillSlot;
        Byte skillSlot = paramSkillSlot;
        SkillInfo skillInfo = SkillInfoMgr.Instance.GetInfoBySlot(_u2ClassID, skillSlot);
        if (skillInfo == null)
            return;
//		if(skillInfo.u1Element != 5)
//		{
//			_imgElement.gameObject.SetActive(true);
//			_imgElement.sprite = AtlasMgr.Instance.GetSprite(string.Format("Sprites/Common/common_02.common_02_skill_element_{0}", skillInfo.u1Element));
////			_imgElement.SetNativeSize();
//		}
//		else
//		{
//			_imgElement.gameObject.SetActive(false);
//		}
		_imgElement.sprite = AtlasMgr.Instance.GetSprite(string.Format("Sprites/Common/common_02_renew.common_02_skill_element_{0}", skillInfo.u1Element));
		_txtSkillName.text = TextManager.Instance.GetText(skillInfo.sName);
		if(skillInfo.u1Element != 5)
			_txtSkillName.color = EquipmentItem.equipElementColors[ (skillInfo.u1Element) ];
		
		_imgSkillIcon.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Skill/Atlas_SkillIcon_" + _u2ClassID + "." + skillInfo.u2ID);

//		_txtSkillType.text = TextManager.Instance.GetText(skillInfo.u1ActWay == 1 ? "skill_type_active" : "skill_type_passive");

		int Level = 1;

		string conditionTime = "";
		string conditionPer = "";
		string damage = "";
		string chainDamage = "";
		string passive = "";
		string heal = "";
		
		float LevelStat = 0;
		float LevelPer = 0;
		float LevelTime = 0;
		
		if (Level > 1) {
			for (int i=0; i<Level-1; i++) {
				LevelStat += skillInfo.fPercentLvUpBonus;
				LevelPer += skillInfo.fActPerLvUpBonus;
				LevelTime += skillInfo.fCondTimeLvUpBonus;
			}
		}
		
		if (skillInfo.cBuff != null){
			conditionTime = ((float)(skillInfo.cBuff.u4DurationTime/1000f + LevelTime)).ToString("#.00");
			
		}else if (skillInfo.cDebuff != null){
			conditionTime = ((float)(skillInfo.cDebuff.u4DurationTime/1000f + LevelTime)).ToString("#.00");
		}
		
		conditionPer = (skillInfo.u2ActSituationPercent + LevelPer).ToString("#.0");
		damage = (skillInfo.u2DamagePercent + LevelPer).ToString("0");
		chainDamage = skillInfo.u1LinkBonus.ToString();
		passive = (skillInfo.cPassive.GetFirstStat()+LevelStat).ToString("0");
		heal = (skillInfo.u2HealPercent + LevelPer).ToString("0");
		
		_txtSkillDescription.text = string.Format(TextManager.Instance.GetText(skillInfo.sDescription), conditionTime, conditionPer, damage, chainDamage, passive, heal);
	}

	public void OnClickSelect()
	{
		if(_u1MaxSkillCount == 1)
		{
			if(_lstSelectedSkillSlot.Count != 0)
			{
				Byte oldSlot =  _lstSelectedSkillSlot[0];
				_trListParent.FindChild(oldSlot.ToString()).FindChild("Selected").gameObject.SetActive(false);
			}
			_lstSelectedSkillSlot.Clear();
			_lstSelectedSkillSlot.Add(_u1CurrentSelectSkillSlot);
			_trListParent.FindChild(_u1CurrentSelectSkillSlot.ToString()).FindChild("Selected").gameObject.SetActive(true);
		}
		else
		{
			if(_lstSelectedSkillSlot.Contains(_u1CurrentSelectSkillSlot))
			{
				_lstSelectedSkillSlot.Remove(_u1CurrentSelectSkillSlot);
				_trListParent.FindChild(_u1CurrentSelectSkillSlot.ToString()).FindChild("Selected").gameObject.SetActive(false);
				
			}
			else
			{
				if(_lstSelectedSkillSlot.Count < _u1MaxSkillCount)
				{
					_lstSelectedSkillSlot.Add(_u1CurrentSelectSkillSlot);
					_trListParent.FindChild(_u1CurrentSelectSkillSlot.ToString()).FindChild("Selected").gameObject.SetActive(true);
					
				}
			}
		}
		StringBuilder strBuilder = new StringBuilder();
		_txtSelectCountInfo.text =  String.Format(
			TextManager.Instance.GetText("forge_smithing_skillcount"),
			TextManager.Instance.GetText(ClassInfoMgr.Instance.GetInfo(_u2ClassID).sName),
			(_u1MaxSkillCount - _lstSelectedSkillSlot.Count) + "/" + _u1MaxSkillCount
		);
	}

	public void OnClickConfirm()
	{

		if (_lstSelectedSkillSlot.Count < _u1MaxSkillCount)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_equip_skill_select"), TextManager.Instance.GetText("popup_desc_equip_skill_select"), _cParent.emptyMethod);
			return;
		}
		_cParent.SelectedSkills((Byte)_lstSelectedSkillSlot.Count, _lstSelectedSkillSlot.ToArray() );
		PopupManager.Instance.RemovePopup(gameObject);
		_cParent.RequestSmithing();
		gameObject.SetActive(false);
	}
//
//	public void OnClickCancel()
//	{
//		gameObject.SetActive (false);
//	}
}
