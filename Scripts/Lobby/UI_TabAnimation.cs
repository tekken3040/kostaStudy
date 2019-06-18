using UnityEngine;
using System.Collections;

public class UI_TabAnimation : MonoBehaviour {
	[SerializeField] RectTransform _objAnimBar;
	[SerializeField] Transform _trMenuGroup;
	[SerializeField] float width = 280f;
	[SerializeField] float height = 48f;


	RectTransform rectTransform;

	Vector2 _trMenuPos;
	Vector2 _movePos;
	public void OnClickTabMenu(int tabIdx)
	{
		rectTransform = _trMenuGroup.GetChild(tabIdx).GetComponent<RectTransform>();
		_trMenuPos.x = rectTransform.anchoredPosition.x;
		_trMenuPos.y = rectTransform.anchoredPosition.y - height;

		float targetX = 0;
		if (1280f < ((tabIdx + 1) * width))
		{
			targetX = 1280 - ((tabIdx + 1) * width);
		}
		LeanTween.move (_trMenuGroup.GetComponent<RectTransform> (), new Vector2(targetX, 0), 0.1f);
		LeanTween.move (_objAnimBar.GetComponent<RectTransform>(), _trMenuPos, 0.15f);
	}

	// Use this for initialization
	void Awake () {
		// _trMenuPos = _objAnimBar.anchoredPosition;
	}

	void OnEnable()
	{
		_objAnimBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, _objAnimBar.GetComponent<RectTransform>().anchoredPosition.y);
	}
}
