using UnityEngine;
using System.Collections;

public class YesNoPopup : Popup {

	protected event PopupManager.OnClickEvent _yesEvent;
	public object[] _yesEventParam;
	protected event PopupManager.OnClickEvent _noEvent;
	protected object[] _noEventParam;

//	public void Show(string content, PopupManager.OnClickEvent yesEvent, object[] yesEventParam)
//	{
//		base.Show(content);
//		_yesEvent = yesEvent;
//		_yesEventParam = yesEventParam;
//		_noEvent = null;
//		_noEventParam = null;
//		gameObject.SetActive(true);
//	}

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
		gameObject.SetActive(true);
		btnOkText.text = btnText;
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

	public void Show(string title, string content, string btnText, PopupManager.OnClickEvent yesEvent, object[] yesEventParam, PopupManager.OnClickEvent noEvent, object[] noEventParam)
	{
		base.Show(title, content);
		_yesEvent = yesEvent;
		_yesEventParam = yesEventParam;
		_noEvent = noEvent;
		_noEventParam = noEventParam;
		gameObject.SetActive(true);
		btnOkText.text = btnText;
	}

	public void OnClickYes()
	{
		YesEvent(_yesEventParam);//_yesEvent(_yesEventParam);
		Close();        
	}

	public void OnClickYesWithDest(){
		OnClickYes();
		Destroy(gameObject);
	}
	
	public virtual void OnClickNoWithDest(){
		PopupManager.Instance.RemovePopup(gameObject);
        Destroy(gameObject);        
	}

	public void OnClickNo()
	{
		if(_noEvent != null) {
			_noEvent(_noEventParam);
			Close();
		} else {
			Close();
		}
	}

	protected void YesEvent(object[] param)
	{
		_yesEvent(param);
	}
}