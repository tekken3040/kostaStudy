using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_ConditionIcon : MonoBehaviour {

	Image Cool;
	Text txtName;
//	Image Icon;
	UInt16 u2ID;

	bool bDestroy;

	void Awake () {
		Cool = transform.FindChild("Cool").GetComponent<Image>();
		txtName = transform.FindChild("Text").GetComponent<Text>();
//		Icon = transform.FindChild("Icon").GetComponent<Image>();
	}

	public void SetInfo(ConditionInfo cInfo, bool bUser){
		u2ID = cInfo.u2ID;
		Cool.color = GetCoolColor (bUser, cInfo.sCircle);
//		Cool.sprite = AtlasMgr.Instance.GetSprite("Sprites/Battle/Condition.cond_circle_"+cInfo.sCircle);
		txtName.text = TextManager.Instance.GetText (cInfo.sName);
//		Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Battle/Condition.cond_"+cInfo.sIcon);
	}

	Color GetCoolColor(bool bUser, string sBuff){
		switch (bUser) {
		case true:
			if(sBuff == "buff")
				return new Color32(146, 208, 80, 255);
			else
				return new Color32(247, 150, 70, 255);
			break;

		case false:
			if (sBuff == "buff")
				return new Color32 (75, 172, 198, 255);
			else 
				return new Color32(255, 255, 0, 255);
			break;
		}

		return Color.white;
	}

	public void UpdateCoolTime (float amount) {
		if(Cool != null) Cool.fillAmount = amount;
	}

	public UInt16 GetID(){
		return u2ID;
	}

	public void DestroyMe(){
		if(bDestroy) return;

		EventTrigger trigger = gameObject.GetComponent<EventTrigger>();
		if(trigger != null) trigger.OnPointerUp (null);

//		Cool = null;
//		txtName = null;
//		Icon = null;
		Destroy (gameObject);
		bDestroy = true;
	}
}
