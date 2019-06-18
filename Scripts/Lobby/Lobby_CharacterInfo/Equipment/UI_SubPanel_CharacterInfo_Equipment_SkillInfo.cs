using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UI_SubPanel_CharacterInfo_Equipment_SkillInfo : MonoBehaviour {
	[SerializeField] Image _imgSkillIcon;
	[SerializeField] Image _imgSkillElement;
//	[SerializeField] Image _imgSkillLevelElement;
//	[SerializeField] Text _txtSkillLevel;
	[SerializeField] Text _txtSkillName;
	[SerializeField] Text _txtSkillDescription;

	SkillInfo _cSkillInfo;
	public void SetData(SkillInfo skillInfo)
	{
		_cSkillInfo = skillInfo;
		_imgSkillIcon.sprite = AtlasMgr.Instance.GetSprite (String.Format ("Sprites/Skill/Atlas_SkillIcon_{0}.{1}", skillInfo.u2ClassID, skillInfo.u2ID));
//		_imgSkillIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Skill/Atlas_SkillIcon_" + skillInfo.u2ClassID + "." + skillInfo.u2ID);
		_imgSkillElement.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.common_02_skill_element_" + skillInfo.u1Element);
		_txtSkillName.text = TextManager.Instance.GetText(skillInfo.sName);
		UIManager.Instance.SetGradientFromElement(_txtSkillName.GetComponent<Gradient>(), skillInfo.u1Element);
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
		_txtSkillDescription.text = TextManager.Instance.GetText(skillInfo.sDescription + "_2");
	}
}
