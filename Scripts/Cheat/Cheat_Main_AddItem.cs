using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Cheat_Main_AddItem : MonoBehaviour {
	[SerializeField] RectTransform _trListParent;
	[SerializeField] GameObject _resListElement_EquipList;
	[SerializeField] InputField _inputSearch;
	[SerializeField] Text _txtSearch;
	[SerializeField] Text _txtInputID;
	[SerializeField] InputField _inputCount;
	[SerializeField] Text _txtCount;

	int _type=0;
	Byte _u1GoodsType=11;
	UInt16 _u2SelectedItemId = 2001;
	int _nCount=1;
	public void OnClickItemType(int type)
	{
		switch(type)
		{
		case 1:		InitMaterialList(); break;
		case 2:		InitConsumableList(); break;
		default: break;
		}
		_type = type;
	}

	public void OnClickMaterial(int equipID)
	{
		_u2SelectedItemId = (UInt16)equipID;
		DebugMgr.Log("SELECT ITEM : " + _u2SelectedItemId);
	}

	List<MaterialItemInfo> _lstMaterial;
	void InitMaterialList()
	{
		for(int i=0; i<_trListParent.childCount; i++)
		{
			DestroyObject(_trListParent.GetChild(i).gameObject);
		}


		_lstMaterial = new List<MaterialItemInfo>();
		_lstMaterial = ItemInfoMgr.Instance.GetMaterialList();
		int listCount=0;
		foreach(MaterialItemInfo matInfo in _lstMaterial)
		{
			GameObject listElement = Instantiate(_resListElement_EquipList) as GameObject;
			listElement.transform.SetParent(_trListParent);
			listElement.transform.localPosition = Vector3.zero;
			listElement.transform.localScale = Vector3.one;
			listElement.transform.name = matInfo.u2ID.ToString();
			listElement.transform.GetChild(0).GetComponent<Text>().text = TextManager.Instance.GetText( matInfo.sName );
			int matID = 0;
			matID = (int)matInfo.u2ID;
			listElement.GetComponent<Button>().onClick.AddListener( () => OnClickMaterial(matID) );
			listElement.SetActive(true);
			listCount++;
		}
		_trListParent.sizeDelta = new Vector2(0, listCount*40f);
	}


	List<ConsumableItemInfo> _lstConsumable;
	void InitConsumableList()
	{
		for(int i=0; i<_trListParent.childCount; i++)
		{
			DestroyObject(_trListParent.GetChild(i).gameObject);
		}


		_lstConsumable = new List<ConsumableItemInfo>();
		_lstConsumable = ItemInfoMgr.Instance.GetConsumableList();
		int listCount=0;
		foreach(ConsumableItemInfo consumeInfo in _lstConsumable)
		{
			GameObject listElement = Instantiate(_resListElement_EquipList) as GameObject;
			listElement.transform.SetParent(_trListParent);
			listElement.transform.localPosition = Vector3.zero;
			listElement.transform.localScale = Vector3.one;
			listElement.transform.name = consumeInfo.u2ID.ToString();
			listElement.transform.GetChild(0).GetComponent<Text>().text = TextManager.Instance.GetText( consumeInfo.sName );
			int consumeID = 0;
			consumeID = (int)consumeInfo.u2ID;
			listElement.GetComponent<Button>().onClick.AddListener( () => OnClickMaterial(consumeID) );
			listElement.SetActive(true);
			listCount++;
		}
		_trListParent.sizeDelta = new Vector2(0, listCount*40f);
	}

	int searchListElementIdx=0;
	public void OnClickSearch()
	{
		string word = _txtSearch.text;
		if(_type == 1) // Material
		{
			for(int i=0; i<_lstMaterial.Count; i++)
			{
				if(word == TextManager.Instance.GetText( _lstMaterial[i].sName ))
				{
					_u2SelectedItemId = _lstMaterial[i].u2ID;
					searchListElementIdx = i;
					_trListParent.GetChild(i).GetComponent<Button>().Select();
					_trListParent.anchoredPosition = new Vector2(0f, i*40f);
					break;
				}
			}
		}
		else if(_type == 2) // Consume
		{
			for(int i=0; i<_lstConsumable.Count; i++)
			{
				if(word == TextManager.Instance.GetText( _lstConsumable[i].sName ))
				{
					_u2SelectedItemId = _lstConsumable[i].u2ID;
					searchListElementIdx = i;
					_trListParent.GetChild(i).GetComponent<Button>().Select();
					_trListParent.anchoredPosition = new Vector2(0f, i*40f);
					break;
				}
			}
		}

	}

	public void OnClickInputID()
	{
		_u2SelectedItemId = Convert.ToUInt16( _txtInputID.text);
	}

	public void OnClickConfirm()
	{

		if(!int.TryParse(_txtCount.text, out _nCount) || _txtCount.text == "")
		{
			_inputCount.text = "1";
			_nCount = 1;
		}
		if(_nCount > 500)
		{
			_inputCount.text = "500";
			_nCount = 500;
		}
		string COMMAND = string.Format("Item Add 11 {0} {1}", _u2SelectedItemId, _nCount);
		DebugMgr.Log(COMMAND);
		Server.ServerMgr.Instance.CheatMsg(COMMAND, AckCheat);
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
			DebugMgr.Log("Add ITEM : " + _u2SelectedItemId);
			Legion.Instance.cInventory.AddItem(0, _u2SelectedItemId, (UInt16)_nCount);
		}
	}
}
