using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Popup : MonoBehaviour {
	public int zOrder;

	public Text lbl_title;
	public Text lbl_content;
	public Text btnOkText;
	public Text btnNoText;

	public virtual void Show()
	{
		gameObject.SetActive(true);
	}

	public virtual void Show(string title, string content)
	{
		lbl_title.text = title;
		lbl_content.text = content;

		GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
		GetComponent<RectTransform>().sizeDelta = Vector2.zero;
	}

	public virtual void Show(string content)
	{
		lbl_content.text = content;
	}

	public void Close()
	{
		if (gameObject.GetComponent<DownloadPopup> () != null)
			return;
		
        PopupManager.Instance.RemovePopup(gameObject);
		gameObject.SetActive(false);
	}

	// Use this for initialization
	protected void Start () {

	}

	protected void OnEnable()
	{
		//DebugMgr.Log("PopupEnable : " + transform.name);
		/** 팝업이 보여지는 순서 */
		transform.SetSiblingIndex(zOrder);
		PopupManager.Instance.registerPopup(gameObject, zOrder);
	}
}
