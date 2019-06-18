using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class UI_Slot_CharacterInfo_Equipment : UI_ListElement_Forge_Fusion_Material {
	[SerializeField] GameObject _objAttachEffect;
	[SerializeField] public GameObject _objStatPointEffect;
	EquipmentItem _cEquipItem;
	public void SetData (EquipmentItem equipItem)
	{
		base.SetData(equipItem, null);
		equipItem.GetComponent<StatusComponent>().CountingStatPointEquip(equipItem.cLevel.u2Level);
		int statPoint = (int)equipItem.GetComponent<StatusComponent>().STAT_POINT;
//		DebugMgr.Log(String.Format("{0} StatPoint : {1}", equipItem.GetEquipmentInfo().u2ID, statPoint));
        List<EquipmentItem> ret = new List<EquipmentItem>();
        ret = Legion.Instance.cInventory.GetNewEquip();
        bool equipNew = false;
        for(int i=0; i<ret.Count; i++)
        {
            if(ret[i].GetEquipmentInfo().u2ClassID != equipItem.GetEquipmentInfo().u2ClassID)
                equipNew = false;
            else if(ret[i].GetEquipmentInfo().u1PosID == equipItem.GetEquipmentInfo().u1PosID && ret[i].isNew)
            {
                equipNew = true;
                break;
            }
            else
                equipNew = false;
        }
		//if(statPoint != 0)
		//{
		//	_objStatPointEffect.SetActive(true);
		//}
        /*else*/ if(equipNew)
            _objStatPointEffect.SetActive(true);
		else
		{
			_objStatPointEffect.SetActive(false);
		}

		if(_cEquipItem == null)
		{
			_cEquipItem = equipItem;
			return;
		}
		if(_cEquipItem != equipItem)
		{
			_cEquipItem = equipItem;
			enableAttachEffect();
		}
	}

	public void enableAttachEffect()
	{
		StartCoroutine(startAttacEffect());
	}
	IEnumerator startAttacEffect()
	{
		yield return new WaitForSeconds(0.1f);
		_objAttachEffect.SetActive(true);
	}

	public void disableAttachEffect()
	{
		_objAttachEffect.SetActive(false);
	}

	void OnEnable()
	{
		_objAttachEffect.SetActive(false);
	}
}
