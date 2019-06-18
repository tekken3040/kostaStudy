using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

// 2016. 08. 12 jy 
// 클릭시에만 오픈 되는 팝업의 클래스 추후 비슷한 류의 팝업은 상속 받도록 한다
// 클래스 이름은 생각이 나지 않아 PressPopup으로 했다
public class PressPopupController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField] 
	private RectTransform _InfoPopup;
	public string MessageKey;
	public Vector3 SubPosition;

	// 2016. 08. 12 jy 
	public void OnPointerDown (PointerEventData eventData)
	{
		Vector2 popupSize = _InfoPopup.sizeDelta;
		Vector3 popupPos = this.GetComponent<RectTransform>().anchoredPosition3D;

		popupPos += SubPosition;

		_InfoPopup.anchoredPosition3D = popupPos;
		_InfoPopup.gameObject.SetActive(true);
		ClickDownFunction(eventData);
	}

	public void OnPointerUp (PointerEventData eventData)
	{
		ClickUpFunction(eventData);
		_InfoPopup.gameObject.SetActive(false);
	}

	// 2016. 08. 12 jy 
	// 상속 받아 사용 할수 있도록 기능을 재정의 할 수 있게 함수로 뺀다
	protected virtual void ClickDownFunction(PointerEventData eventData)
	{
		_InfoPopup.GetComponent<PressPopup>().SetData(TextManager.Instance.GetText(MessageKey));
	}

	protected virtual void ClickUpFunction(PointerEventData eventData)
	{
		
	}
}
