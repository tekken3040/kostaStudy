using UnityEngine;
using System.Collections;

public class UI_GoodsPopup : Popup {
event PopupManager.OnClickEvent _yesEvent;
	public object[] _yesEventParam;
	event PopupManager.OnClickEvent _noEvent;
	object[] _noEventParam;

	public void Show(string title, string content, PopupManager.OnClickEvent yesEvent, object[] yesEventParam)
	{
		base.Show(title, content);
		_yesEvent = yesEvent;
		_yesEventParam = yesEventParam;
		_noEvent = null;
		_noEventParam = null;
		gameObject.SetActive(true);
	}

	public void Show(string title, string content, string btnText, PopupManager.OnClickEvent yesEvent, object[] yesEventParam)
	{
		base.Show(title, content);
		_yesEvent = yesEvent;
		_yesEventParam = yesEventParam;
		_noEvent = null;
		_noEventParam = null;
		btnOkText.text = btnText;
		gameObject.SetActive(true);
	}

	public void Show(string title, string content, PopupManager.OnClickEvent yesEvent, object[] yesEventParam, PopupManager.OnClickEvent noEvent, object[] noEventParam)
	{
		base.Show(title, content);
		_yesEvent = yesEvent;
		_yesEventParam = yesEventParam;
		_noEvent = noEvent;
		_noEventParam = noEventParam;
		gameObject.SetActive(true);
	}

	public void OnClickYes()
	{
		_yesEvent(_yesEventParam);
		Close();
	}

	public void OnClickYesWithDest(){
		OnClickYes();
		Destroy(gameObject);
	}
	
	public void OnClickNoWithDest(){
		Destroy(gameObject);
	}

	public void OnClickNo()
	{
		if(_noEvent != null) {
			_noEvent(_noEventParam);
		} else {
			Close();
		}
	}
}

