using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class TrainingHeroEquipWindow : MonoBehaviour 
{
	[SerializeField]
	private TrainingSlot[] _acTrainingSlots;
	[SerializeField]
	private GameObject[] _accObjLevelup;

	public void SetInfo(Hero cHero, UInt16 roomID, int selectTimeIndex)
	{
		if(cHero == null)
		{
			OnClickClose();
			return; 
		}

		UInt64 addExp = QuestInfoMgr.Instance.GetCharTrainingInfo()[roomID].arrExp[selectTimeIndex];
		int equipCount = cHero.acEquips.Length;
		for(int i = 0; i < equipCount; ++i)
		{
			_acTrainingSlots[i].SetEquipSlot(cHero.acEquips[i]);
			UInt16 level = CheckEquipLevelup(cHero.acEquips[i], addExp);
			if(cHero.acEquips[i].cLevel.u2Level != level)
			{
				_accObjLevelup[i].SetActive(true);
				_acTrainingSlots[i].level.text = level.ToString();
			}
			else
				_accObjLevelup[i].SetActive(false);
		}
	}

	protected UInt16 CheckEquipLevelup(EquipmentItem info, UInt64 addExp)
	{
		UInt16 level = info.cLevel.u2Level;
		if( level < Server.ConstDef.MaxHeroLevel )
		{
			UInt64 nowExp = info.cLevel.u8Exp + addExp;
			UInt64 nextExp = ClassInfoMgr.Instance.GetNextExp(level);
			while(nowExp >= nextExp)
			{                    
				nowExp -= nextExp;
				level++;
				nextExp = ClassInfoMgr.Instance.GetNextExp(level);
			}
		}
		return level;
	}

	public void OnClickClose()
	{
		this.gameObject.SetActive(false);
	}
}
