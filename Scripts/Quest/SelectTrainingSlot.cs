using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class SelectTrainingSlot : MonoBehaviour 
{
	public GameObject objSlot;
	public Image imgPortrait;
	public Image imgElement;
	public Image imgGrade;
	public Text txtName;
	public Text txtLevel;
	public Text txtExp;
	public Image imgExpGague;
	public GameObject objLockMark;
	public GameObject objEmptyMark;
	public GameObject objLevelupMark;
	public Button btnEquipOpen;

	private int _nSeatType;
	public int SeatType 
	{
		get { return _nSeatType; } 
		set { _nSeatType = value; }
	}
	private int _nSlotIndex;
	public int SlotIndex
	{ 
		get { return _nSlotIndex;}
		set { _nSlotIndex = value;  }
	}
	public Hero cHero;
	public TrainingSelectedSlotDrag slotDrag;

	public void SetSlot(TrainingSlot slot)
	{
		objSlot.SetActive(true);
		objLockMark.SetActive(false);
		objEmptyMark.SetActive(false);
		imgPortrait.sprite = slot.portrait.sprite;
		imgElement.sprite = slot.element.sprite;
		txtLevel.text = slot.level.text;
		imgPortrait.SetNativeSize();
		imgPortrait.transform.localScale = Vector3.one;
	}
	// 영웅 정보
	public void SetHero(Hero hero)
	{
		txtName.text = hero.sName;
		cHero = hero;

		objSlot.SetActive(true);
	}
	// 영웅 경험치 셋팅
	public void SetHeroExp(TrainingInfo info, int selectTimeIndex)
	{
		if( _nSeatType != 1 )
			return;
		
		Hero hero = Legion.Instance.acHeros[slotDrag.GetItemIndex()];
		UInt16 level = hero.cLevel.u2Level;
		UInt64 nowExp = hero.cLevel.u8Exp + info.arrExp[selectTimeIndex]; 
		UInt64 nextExp = ClassInfoMgr.Instance.GetNextExp(level);

		// 2016. 12. 07 jy
		// 만렙 아닌 경우만 체크 다음 레벨을 체크한다
		if( level < Server.ConstDef.MaxHeroLevel )
		{
			while(nowExp >= nextExp)
			{                    
				nowExp -= nextExp;
				level++;
				nextExp = ClassInfoMgr.Instance.GetNextExp(level);
			}
		}

		txtLevel.text = level.ToString();
		txtExp.text = ConvertExpValue(nowExp) +  " / " + ConvertExpValue(nextExp);//nowExp + "/" + nextExp;
		imgExpGague.fillAmount = (float)((float)nowExp / (float)nextExp);

		if(level > hero.cLevel.u2Level)
			objLevelupMark.SetActive(true);
		else                                        
			objLevelupMark.SetActive(false);

	}
	// 장비 경험치 셋팅
	public bool SetEquipItemExp(TrainingInfo info, int selectTimeIndex)
	{
		if( _nSeatType != 1 )
			return false;

		// 획득 예상 경험치 처리
		EquipmentItem equipItem = Legion.Instance.cInventory.lstSortedEquipment[slotDrag.GetItemIndex()];                    

		UInt16 level = equipItem.cLevel.u2Level;
		UInt64 nowExp = equipItem.cLevel.u8Exp + info.arrExp[selectTimeIndex]; 
		UInt64 nextExp = ClassInfoMgr.Instance.GetNextExp(level);

		while(nowExp >= nextExp)
		{                    
			nowExp -= nextExp;
			level++;
			nextExp = ClassInfoMgr.Instance.GetNextExp(level);
		}

		txtLevel.text = level.ToString();
		//txtExp.text = nowExp + "/" + nextExp;
		txtExp.text = ConvertExpValue(nowExp) +  " / " + ConvertExpValue(nextExp);
		imgExpGague.fillAmount = (float)nowExp / (float)nextExp;

		if(level > equipItem.cLevel.u2Level)
			objLevelupMark.SetActive(true);
		else
			objLevelupMark.SetActive(false);         

		objSlot.SetActive(true);
		if(level > Legion.Instance.TopLevel)
			return true;
		else
			return false;
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

	// 빈 슬롯 셋팅
	public void SetEmpty()
	{
		_nSeatType = 0;
		objSlot.SetActive(false);
		objLockMark.SetActive(false);
		objEmptyMark.SetActive(true);
		cHero = null;
	}
	// 슬롯 잠김 셋팅
	public void SetSlotLock(bool isLock)
	{
		objSlot.SetActive(false);
		if( isLock == true )
		{
			_nSeatType = -1;
			objLockMark.SetActive(true);
			objEmptyMark.SetActive(false);
		}
		else
		{
			_nSeatType = 0;
			objLockMark.SetActive(false);
			objEmptyMark.SetActive(true);
		}
	}
}
