using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Cheat_Main_AddEtcGoods : MonoBehaviour {
	[SerializeField] InputField _inputCount;
	[SerializeField] Text _txtCount;

	Byte _u1GoodsType=1;
	int _nCount=1;
	public void OnClickItemType(int type)
	{
		_u1GoodsType = (Byte)type;
	}

	public void OnClickConfirm()
	{

		if(!int.TryParse(_txtCount.text, out _nCount) || _txtCount.text == "")
		{
			_inputCount.text = "1";
			_nCount = 1;
		}
		if(_nCount > 150000)
		{
			_inputCount.text = "150000";
			_nCount = 150000;
		}
		string COMMAND = string.Format("Item Add {0} 0 {1}", _u1GoodsType, _nCount);
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
			DebugMgr.Log("Add ITEM : " + _u1GoodsType);
			Legion.Instance.AddGoods(new Goods(_u1GoodsType, 0, (UInt16)_nCount));
		}
	}
}
