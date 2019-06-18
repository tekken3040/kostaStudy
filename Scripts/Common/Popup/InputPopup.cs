using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InputPopup : YesNoPopup 
{
	public InputField _inputField;
	public Button _btnOk;

	protected void Start () 
	{
		base.Start();
		CheckBtnEnabled();
	}

	public void OnClickYes()
	{
		YesEvent(new object[] { _yesEventParam, _inputField.textComponent.text });
		Close();
	}

	public void CheckBtnEnabled()
	{
		if( _inputField.textComponent.text.Length <= 0 )
		{
			AtlasMgr.Instance.SetGrayScale (_btnOk.image);
			_btnOk.interactable = false;
		}
		else
		{
			AtlasMgr.Instance.SetDefaultShader(_btnOk.image);
			_btnOk.interactable = true;
		}
	}
}
