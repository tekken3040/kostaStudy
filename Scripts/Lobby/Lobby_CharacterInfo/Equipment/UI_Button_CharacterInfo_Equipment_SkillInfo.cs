using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class UI_Button_CharacterInfo_Equipment_SkillInfo : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
	[SerializeField] RectTransform _infoPopup;
	public Vector3 subPosition;
	SkillInfo _cSkillinfo;
	public void SetData(SkillInfo skillInfo)
	{
		_cSkillinfo = skillInfo;
	}

	#region IPointerDownHandler implementation
	public void OnPointerDown (PointerEventData eventData)
	{

		_infoPopup.GetComponent<UI_SubPanel_CharacterInfo_Equipment_SkillInfo>().SetData(_cSkillinfo);
		_infoPopup.gameObject.SetActive(true);
		_infoPopup.position = transform.position;
		_infoPopup.localPosition += subPosition;
//		if(transform.position.x + _infoPopup.GetComponent<RectTransform>().sizeDelta.x > (Screen.resolutions[0].width))
//		{
//			_infoPopup.transform.position = transform.position;
//			_infoPopup.transform.localPosition = new Vector3(_infoPopup.transform.localPosition.x - 300f, _infoPopup.transform.localPosition.y, _infoPopup.transform.localPosition.z);
//		}
//		else
//		{
//			_infoPopup.transform.position = transform.position;
//		}
	}
	#endregion

	#region IPointerUpHandler implementation

	public void OnPointerUp (PointerEventData eventData)
	{
		_infoPopup.gameObject.SetActive(false);
	}

	#endregion

}