using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UI_SkillSelectIcon : MonoBehaviour {

	Image Icon;
	Image ElementBorder;
	Image Element;
	Image Border;
	Text Level;
	Image Eff;
	Animator EffAnim;

	public bool bLock = true;

	Byte u1SlotNum;
	UInt16 u2ClassID;
	UInt16 u2SkillID;

	Vector3 iconSize = new Vector3(0.78f,0.78f,1f);

	RectTransform myRectTr;

	public void Init(bool locked, Byte slotNum, UInt16 classID, UInt16 skillID, UInt16 lv, Byte element){
		u1SlotNum = slotNum;
		u2ClassID = classID;
		u2SkillID = skillID;
		bLock = locked;

		myRectTr = GetComponent<RectTransform> ();
		
		Icon = transform.FindChild("Icon").GetComponent<Image>();
		ElementBorder = transform.FindChild("ElementBorder").GetComponent<Image>();
		Element = transform.FindChild("SkillElement").GetComponent<Image>();
		Eff = transform.FindChild("Effect").GetComponent<Image>();
		Border = transform.FindChild("Border").GetComponent<Image>();
		Level = transform.FindChild("Level").GetComponent<Text>();
		EffAnim = transform.FindChild("Effect").GetComponent<Animator>();
		EffAnim.enabled = false;

		if(bLock){
			Icon.enabled = true;
			Icon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.lock_icon_2");
			if (Icon.color.a < 1f) {
				LeanTween.cancel(Icon.gameObject);
				Icon.color = Color.white;
			}
			Icon.SetNativeSize();
			ElementBorder.enabled = false;
			Element.enabled = false;
			Level.enabled = false;
			Border.enabled = false;
			Eff.enabled = false;
			GetComponent<ButtonSound>().buttonClip = Resources.Load("Sound/UI/01. Common/UI_Button_Slot_1", typeof(AudioClip)) as AudioClip;
		}else{
			if (skillID > 0) {
				Border.enabled = true;
				Icon.enabled = true;
				if (Icon.color.a < 1f) {
					LeanTween.cancel(Icon.gameObject);
					Icon.color = Color.white;
				}
				Icon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Skill/Atlas_SkillIcon_" + classID + "." + skillID);
				Icon.SetNativeSize();
				Icon.rectTransform.localScale = iconSize;
				ElementBorder.enabled = true;
				ElementBorder.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.common_02_skill_element_" + element);
				Element.enabled = true;
				Element.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.element_" + element);
				Level.enabled = true;
				Level.text = lv.ToString();
			} else {
				Icon.enabled = false;
				ElementBorder.enabled = false;
				Element.enabled = false;
				Level.enabled = false;
				Border.enabled = false;
			}
		}
	}

	public Button GetButton(){
		return GetComponent<Button>();
	}

	public void UnLock(){
		LeanTween.alpha (Icon.rectTransform, 0f, 0.08f).setLoopPingPong(4);
		LeanTween.alpha (Icon.rectTransform, 0f, 0.4f).setDelay(0.8f);
		bLock = false;
		GetComponent<ButtonSound>().buttonClip = Resources.Load("Sound/UI/01. Common/UI_Button_2", typeof(AudioClip)) as AudioClip;
	}

	public void UnUse(){
		if(u2SkillID == 0) return;
		u2SkillID = 0;
		Icon.enabled = false;
		ElementBorder.enabled = false;
		Element.enabled = false;
		Border.enabled = false;
		Level.enabled = false;
	}

	public void Change(UInt16 skillID, Byte element, UInt16 lv){
		u2SkillID = skillID;
		Border.enabled = true;
		Icon.enabled = true;
		if (Icon.color.a < 1f) {
			LeanTween.cancel(Icon.gameObject);
			Icon.color = Color.white;
		}
		Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Skill/Atlas_SkillIcon_"+u2ClassID+"."+skillID);
		Icon.SetNativeSize();
		Icon.rectTransform.localScale = iconSize;
		ElementBorder.enabled = true;
		ElementBorder.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.common_02_skill_element_" + element);
		Element.enabled = true;
		Element.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + element);
		Level.enabled = true;
		Level.text = lv.ToString();

		myRectTr.localScale = Vector3.one;
		LeanTween.cancel(gameObject);
		LeanTween.scale (myRectTr, Vector3.one*1.1f, 0.1f).setLoopPingPong(1);
	}

	public void SetSelectEff(bool bEnable){
		if(bLock) return;

		if (bEnable) {
			Eff.enabled = true;
			EffAnim.enabled = true;
			EffAnim.Play("Ani_UI_Main_SkillSlot",0,0.0f);
		} else {
			Eff.enabled = false;
			EffAnim.enabled = false;
		}
	}
}
