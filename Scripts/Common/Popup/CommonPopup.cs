using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class CommonPopup : Popup {
	public Text lbl_Title;
	public Text lbl_Content;
	
	public override void Show(string title, string content)
	{
		lbl_Title.text = title;
		lbl_Content.text = content;
		gameObject.SetActive(true);
	}

	protected virtual void OnClickYes()
	{
		
	}
	
	protected virtual void OnClickNo()
	{
		
	}

	// Use this for initialization
	new void Start () {
		DebugMgr.Log("Call CommonPopup.Start");
		base.Start();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
