using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UI_SkillLearnPopup : YesNoPopup {

	public GameObject LearnGroup;
	public GameObject BtnClose;
	public Image Icon;
	public Image Element;
	public Image EleMini;
	public Text TextComment;
	public Text TextLv;

	public Image Icon2;
	public Image Element2;
	public Image EleMini2;
	public Text TextLv2;

	public Text TextInfo;
	public Text TextInfo2;
	
	public void SetLearnPopup(UInt16 u2ClassID, UInt16 u2ID, Byte u1Ele, UInt16 u2Lv, string txt1, string txt2){
		if (u2Lv > 0) {
			TextComment.text = String.Format(TextManager.Instance.GetText("popup_desc_upgrade_skill"), (u2Lv+1));
			TextLv.text = u2Lv.ToString();
			TextLv2.text = (u2Lv+1).ToString();

			Element2.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.common_02_skill_element_"+u1Ele);
			EleMini2.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_"+u1Ele);
			Icon2.sprite = AtlasMgr.Instance.GetSprite("Sprites/Skill/Atlas_SkillIcon_"+u2ClassID+"."+u2ID);

			TextInfo.text = txt1;
			TextInfo2.text = txt2;
		} else {
			TextComment.text = TextManager.Instance.GetText("popup_desc_active_skill");
			LearnGroup.SetActive(false);
		}

		Element.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.common_02_skill_element_"+u1Ele);
		EleMini.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_"+u1Ele);
		Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Skill/Atlas_SkillIcon_"+u2ClassID+"."+u2ID);

		lbl_content.color = EquipmentItem.equipElementColors[u1Ele-1];

		if(Legion.Instance.cTutorial.bIng) BtnClose.SetActive(false);
	}
}
