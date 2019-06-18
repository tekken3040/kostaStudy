using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WaitOKPopupInfo
{
    public string title;
    public string content;
    public PopupManager.OnClickEvent okEvent;
    public object[] okEventParam;

    public void Set(string title, string content, PopupManager.OnClickEvent okEvent, object[] okEventParam)
    {
        this.title = title;
        this.content = content;
        this.okEvent = okEvent;
        this.okEventParam = okEventParam;
    }
}

public class OKPopup : Popup {

	int stack = 0;
    //public delegate void OnClickEvent(object[] param);
    //public static event OnClickEvent OK
    protected Queue<WaitOKPopupInfo> waitPopupInfo = new Queue<WaitOKPopupInfo>();
	// Use this for initialization
	new void Start () {
		base.Start();
    }

	event PopupManager.OnClickEvent _okEvent;
	object[] _okEventParam;
	public void Init(string title, string content, PopupManager.OnClickEvent okEvent, object[] okEventParam)
	{
        // 2017. 05. 19 jy
        // 팝업 동시 셋팅시 마지막으로 셋팅되고 버튼만 여러번 셋팅한 만큼 눌러야 팝업이 종료 되므로
        // 입력 된 순서대로 셋팅하여 Ok 버튼 눌리시 대기 정보가 있다면 대기정보를 셋팅한다
        if (stack > 0)
        {
            WaitOKPopupInfo info = new WaitOKPopupInfo();
            info.Set(title, content, okEvent, okEventParam);

            waitPopupInfo.Enqueue(info);
        }
        else
        {
            SetPopup(title, content, okEvent, okEventParam);
        }
        stack++;
    }

    protected void SetPopup(WaitOKPopupInfo info)
    {
        lbl_title.text = info.title;
        lbl_content.text = info.content;
        _okEvent = info.okEvent;
        _okEventParam = info.okEventParam;
    }

    protected void SetPopup(string title, string content, PopupManager.OnClickEvent okEvent, object[] okEventParam)
    {
        lbl_title.text = title;
        lbl_content.text = content;
        _okEvent = okEvent;
        _okEventParam = okEventParam;
    }

	public void OnClickOK()
	{
        if (_okEvent != null)
        {
            _okEvent(_okEventParam);
        }

		if (waitPopupInfo.Count > 0)
        {
            SetPopup(waitPopupInfo.Dequeue());
            stack--;
        }
        else
        {
            stack = 0;
            Close();
            PopupManager.Instance.RemovePopup(gameObject);
        }   
    }
}
