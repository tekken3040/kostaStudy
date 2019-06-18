using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UI_PointBuyPopup : YesNoPopup {
	public enum BuyType{
		Char,
		Equip,
		Skill
	}

	public BuyType eType;
	Byte u1Point = 1;
	Byte u1Buyed;
	public Text PossibleText;
	public Text PointText;
	public Text PriceText;

	public void SetBuyPointPopup(BuyType tType, Byte Pt){
		eType = tType;
		u1Buyed = Pt;

		_yesEventParam = new object[2];

		UpdatePoint();
	}

	public void AddPoint(){
		switch (eType) {
		case BuyType.Char: 
            if(u1Buyed + u1Point < EquipmentInfoMgr.Instance.LIMIT_CHAR_STATPOINT){
				u1Point++;
			}
			break;
		case BuyType.Equip: 
            if(u1Buyed + u1Point < EquipmentInfoMgr.Instance.LIMIT_EQUIP_STATPOINT){
				u1Point++;
			}
			break;
		case BuyType.Skill: 
            if(u1Buyed + u1Point < EquipmentInfoMgr.Instance.LIMIT_SKILLPOINT){
				u1Point++;
			}
			break;
		}
		
		UpdatePoint ();
	}

	public void SubPoint(){
		if (u1Point > 1) {
			u1Point--;
			UpdatePoint ();
		}
	}

	void UpdatePoint(){
		int limit = 0;
		int add = 0;
		int total = 0;
		switch (eType) {
		case BuyType.Char: 
			for(int i=u1Buyed; i<u1Point+u1Buyed; i++){
				add = (int)EquipmentInfoMgr.Instance.cCharStatGoods.u4Count + (i*EquipmentInfoMgr.Instance.BuyCharStatAddGoods);
				if(add > EquipmentInfoMgr.Instance.BuySkillPointMaxGoods) add = EquipmentInfoMgr.Instance.BuyCharStatMaxGoods;
				total += add;
			}
			
			PriceText.text = total.ToString();
            limit = EquipmentInfoMgr.Instance.LIMIT_CHAR_STATPOINT;
			break;
		case BuyType.Equip: 
			for(int i=u1Buyed; i<u1Point+u1Buyed; i++){
				add = (int)EquipmentInfoMgr.Instance.cEquipStatGoods.u4Count + (i*EquipmentInfoMgr.Instance.BuyEquipStatAddGoods);
				if(add > EquipmentInfoMgr.Instance.BuySkillPointMaxGoods) add = EquipmentInfoMgr.Instance.BuyEquipStatMaxGoods;
				total += add;
			}
			
			PriceText.text = total.ToString();
            limit = EquipmentInfoMgr.Instance.LIMIT_EQUIP_STATPOINT;
			break;
		case BuyType.Skill: 
			for(int i=u1Buyed; i<u1Point+u1Buyed; i++){
				add = (int)EquipmentInfoMgr.Instance.cSkillPointGoods.u4Count + (i*EquipmentInfoMgr.Instance.BuySkillPointAddGoods);
				if(add > EquipmentInfoMgr.Instance.BuySkillPointMaxGoods) add = EquipmentInfoMgr.Instance.BuySkillPointMaxGoods;
				total += add;
			}

			PriceText.text = total.ToString();
            limit = EquipmentInfoMgr.Instance.LIMIT_SKILLPOINT;
			break;
		}

		PointText.text = u1Point.ToString();
		PossibleText.text = TextManager.Instance.GetText("popup_desc_info_stat_pt_equip")+"("+(u1Buyed+u1Point)+"/"+limit+")";


		_yesEventParam[0] = u1Point;
		_yesEventParam[1] = total;
	}

	public void AddNoEvent(PopupManager.OnClickEvent noEvent)
	{
		_noEvent += noEvent;
	}
}
