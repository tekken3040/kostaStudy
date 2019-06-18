using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
public class BreakText : MonoBehaviour {
	public GameObject leftText;
	public GameObject rightText;

	public RectTransform trLight2;
	public Image imgLight1;
	public Image imgLight2;

	public CanvasGroup cGroup;

	float moveDist = 30.0f;

	void OnEnable () {
		gameObject.transform.localScale = Vector3.one * 2.0f;

		LeanTween.scale (gameObject, Vector3.one, 0.3f).setEase (LeanTweenType.easeOutQuart);

		LeanTween.value (imgLight1.gameObject, 0f, 1f, 0.2f).setOnUpdate((float fill)=>{imgLight1.fillAmount = fill;}).setDelay(0.3f);
		LeanTween.value (imgLight2.gameObject, 0f, 1f, 0.2f).setOnUpdate((float fill)=>{imgLight2.fillAmount = fill;}).setDelay(0.3f);

		LeanTween.moveLocal (leftText, new Vector3(-moveDist,45f,0f), 0.8f).setDelay (0.5f);
		LeanTween.moveLocal (rightText, new Vector3(moveDist,-46f,0f), 0.8f).setDelay (0.5f);

		LeanTween.alpha (trLight2, 0f, 0.5f).setDelay (0.4f);

		LeanTween.value (cGroup.gameObject, 1f, 0f, 0.3f).setOnUpdate((float alpha)=>{cGroup.alpha = alpha;}).setDelay(0.9f);
	}
}
