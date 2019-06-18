using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UI_ListElement_Forge_ChangeLook_Model : MonoBehaviour {
	[SerializeField] Text _txtLookName;
	[SerializeField] Image _imgButton;
	[SerializeField] Sprite _imgSelect;
	[SerializeField] Sprite _imgDeSelect;
	public GameObject _objNewIcon;
	public void SetData (ModelInfo modelInfo)
	{
		_txtLookName.text = TextManager.Instance.GetText( modelInfo.sModelName );
	}

	public void SetActive(bool active)
	{
		if(active)
		{
			_imgButton.GetComponent<Button>().interactable = false;
			AtlasMgr.Instance.SetGrayScale(_imgButton);
		}
		else
		{
			_imgButton.GetComponent<Button>().interactable = false;
			AtlasMgr.Instance.SetDefaultShader(_imgButton);
		}
	}

	bool bCheck;
	public void Select()
	{
		bCheck = true;
		_imgButton.sprite = _imgSelect;
	}

	public void DeSelect()
	{
		bCheck = false;
		_imgButton.sprite = _imgDeSelect;
	}

	public void Toggle()
	{
		if(bCheck)
		{
			_imgButton.sprite = _imgDeSelect;
		}
		else
		{
			_imgButton.sprite = _imgSelect;
		}
		bCheck = !bCheck;
	}

}
