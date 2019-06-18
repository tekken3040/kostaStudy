using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class UI_Button_CharacterInfo_Equipment_StatInfo : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {
	[SerializeField] RectTransform _infoPopup;
	public Vector3 subPosition;
	public Byte _u1StatType;
	public void SetData(Byte statType)
	{
		_u1StatType = statType;
	}

	#region IPointerDownHandler implementation
	public void OnPointerDown (PointerEventData eventData)
	{
		_infoPopup.GetComponent<UI_SubPanel_CharacterInfo_Equipment_StatInfo>().SetData(_u1StatType);
		_infoPopup.gameObject.SetActive(true);
		_infoPopup.transform.position = transform.position;
		//_infoPopup.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
		_infoPopup.localPosition += subPosition;
		// 2016. 07. 11 jy
		// 팝업 이미지의 높이를 더하여 팝업을 위치를 변경함
		Vector3 popPos = _infoPopup.anchoredPosition3D;
		popPos.y += _infoPopup.sizeDelta.y;
		_infoPopup.anchoredPosition3D = popPos;
	}
	#endregion

	#region IPointerUpHandler implementation

	public void OnPointerUp (PointerEventData eventData)
	{
		_infoPopup.gameObject.SetActive(false);
	}

	#endregion
}