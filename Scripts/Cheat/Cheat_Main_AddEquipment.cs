using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
public class Cheat_Main_AddEquipment : MonoBehaviour {
	[SerializeField] RectTransform _trListParent;
	[SerializeField] GameObject _resListElement_EquipList;
	[SerializeField] InputField _inputSearch;
	[SerializeField] Text _txtSearch;
	[SerializeField] InputField _inputCount;
	[SerializeField] Text _txtCount;

	Byte _u1SelectedClassID=1;
	Byte _u1SelectedPosID=1;
	UInt16 _u2SelectedEquipID = 10001;
	public void OnClickClass(int classID)
	{
		_u1SelectedClassID = (Byte)classID;
	}

	public void OnClickPos(int posID)
	{
		_u1SelectedPosID = (Byte)posID;
		if(posID == (Byte)EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 || posID == (Byte)EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
			_u1SelectedClassID = ClassInfo.COMMON_CLASS_ID;
		InitList();
	}

	public void OnClickEquip(int equipID)
	{
		_u2SelectedEquipID = (UInt16)equipID;
		DebugMgr.Log("SELECT ITEM : " + _u2SelectedEquipID);
	}

	List<EquipmentInfo> _lstEquip;
	void InitList()
	{
		for(int i=0; i<_trListParent.childCount; i++)
		{
			DestroyObject(_trListParent.GetChild(i).gameObject);
		}

		_lstEquip = new List<EquipmentInfo>();
		_lstEquip = EquipmentInfoMgr.Instance.GetList(_u1SelectedClassID, _u1SelectedPosID);
		int listCount=0;
		foreach(EquipmentInfo equipInfo in _lstEquip)
		{
			GameObject listElement = Instantiate(_resListElement_EquipList) as GameObject;
			listElement.transform.SetParent(_trListParent);
			listElement.transform.localPosition = Vector3.zero;
			listElement.transform.localScale = Vector3.one;
			listElement.transform.name = equipInfo.u2ID.ToString();
			listElement.transform.GetChild(0).GetComponent<Text>().text = TextManager.Instance.GetText( equipInfo.sName );
			int equipID = 0;
			equipID = (int)equipInfo.u2ID;
			listElement.GetComponent<Button>().onClick.AddListener( () => OnClickEquip(equipID) );
			listElement.SetActive(true);
			listCount++;
		}
		_trListParent.sizeDelta = new Vector2(0, listCount*40f);
	}

	int searchListElementIdx=0;
	public void OnClickSearch()
	{
		string word = _txtSearch.text;
		for(int i=0; i<_lstEquip.Count; i++)
		{
			if(word == TextManager.Instance.GetText( _lstEquip[i].sName ))
			{
				_u2SelectedEquipID = _lstEquip[i].u2ID;
				searchListElementIdx = i;
				_trListParent.GetChild(i).GetComponent<Button>().Select();
				_trListParent.anchoredPosition = new Vector2(0f, i*40f);
				break;
			}
		}
	}

	public void OnClickConfirm()
	{
		int count;
		if(!int.TryParse(_txtCount.text, out count) || _txtCount.text == "")
		{
			_inputCount.text = "1";
			count = 1;
		}
		if(count > 10)
		{
			_inputCount.text = "10";
			count = 10;
		}
		string COMMAND = string.Format("Item Add 10 {0} {1}", _u2SelectedEquipID, count);
		DebugMgr.Log(COMMAND);
		Server.ServerMgr.Instance.CheatMsg(COMMAND, AckCheat);
	}

	public void OnClickDesignConfirm()
	{
		int count;
		if(!int.TryParse(_txtCount.text, out count) || _txtCount.text == "")
		{
			_inputCount.text = "1";
			count = 1;
		}
		if(count > 1)
		{
			_inputCount.text = "1";
			count = 1;
		}
		string COMMAND = string.Format("Item Add 13 {0} 1", _u2SelectedEquipID);
		DebugMgr.Log(COMMAND);
		Server.ServerMgr.Instance.CheatMsg(COMMAND, AckAddDesign);
	}

	public void EmptyMethod(object[] param)
	{}

	public void AckCheat(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowYesNoPopup("에러", "치트오류", EmptyMethod, null);
			return;
		}
		else
		{
			DebugMgr.Log("Add ITEM : " + _u2SelectedEquipID);
//			UInt16 invenSlotNum = Legion.Instance.cInventory.AddEquipment(0, _u2SelectedEquipID, skillSlots, stats, "", Legion.Instance.sName, 0, smithingDetail._cForgeInfo.u2Level);
//			smithingDetail._u2LastSlotNum = invenSlotNum;
		}
	}

	public void AckAddDesign(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowYesNoPopup("에러", "치트오류", EmptyMethod, null);
			return;
		}
		else
		{
			DebugMgr.Log("Add Equip Design : " + _u2SelectedEquipID);
			Legion.Instance.AddGoods(new Goods((Byte)GoodsType.SCROLL, _u2SelectedEquipID, 1));
//			Legion.Instance.acEquipDesign.Set((_u2SelectedEquipID-Server.ConstDef.BaseEquipDesignID), true);
		}
	}
}
