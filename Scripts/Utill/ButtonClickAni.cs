using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

[RequireComponent (typeof(Button))]
public class ButtonClickAni : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{
    Button button;
    Color[] color;
    public float scaleValueDown = 0.9f, scaleValueUp = 1f;
    public GameObject[] ColorObj;

	bool bScaleInit = false;
	float x = 1;
	float y = 1;

    void Awake()
    {
        color = new Color[2];
        color[0] = Color.gray;
        color[1] = Color.white;
        button = GetComponent<Button>();
    }

	void Start()
	{
		if (!bScaleInit) {
			if (transform.localScale != Vector3.one) {
				if (transform.localScale.x == transform.localScale.y) {
					scaleValueDown = transform.localScale.x * scaleValueDown;
					scaleValueUp = transform.localScale.x * scaleValueUp;
				}
			}
			if (transform.localScale.x < 0) {
				x = -1;
			}
			if (transform.localScale.y < 0) {
				y = -1;
			}
		}
		bScaleInit = true;
//		if (ColorObj.Length == 0) {
//			
//			Image[] imgObj = transform.GetComponentsInChildren<Image> ();
//			Text[] textObj = transform.GetComponentsInChildren<Text> ();
//
//			if (imgObj.Length + textObj.Length > 0) {
//				
//				ColorObj = new GameObject[imgObj.Length + textObj.Length];
//
//				for (int i = 0; i < imgObj.Length; i++)
//					ColorObj [i] = imgObj [i].gameObject;
//				
//				if (textObj.Length > 0) {
//					for (int i = 0; i < textObj.Length; i++)
//						ColorObj [imgObj.Length+i] = textObj [i].gameObject;
//				}
//			}
//		}
	}

    public void OnPointerDown (PointerEventData eventData)
    { 
		if (button != null && button.interactable == false)
			return;
        
		LeanTween.scale(this.GetComponent<RectTransform>(), new Vector2(scaleValueDown*x, scaleValueDown*y), 0.1f);
		for (int i = 0; i < ColorObj.Length; i++) {
			if(ColorObj [i].GetComponent<Text> () != null) LeanTween.textColor (ColorObj [i].GetComponent<RectTransform> (), color [0], 0.1f);
			else LeanTween.color (ColorObj [i].GetComponent<RectTransform> (), color [0], 0.1f);
		}
    }

    public void OnPointerUp (PointerEventData eventData)
    {
        if(button != null && button.interactable == false)
            return;
        
		LeanTween.scale(this.GetComponent<RectTransform>(), new Vector2(scaleValueUp*x, scaleValueUp*y), 0.1f);
		for (int i = 0; i < ColorObj.Length; i++) {
			if(ColorObj [i].GetComponent<Text> () != null) LeanTween.textColor (ColorObj [i].GetComponent<RectTransform> (), color [1], 0.1f);
			else LeanTween.color (ColorObj [i].GetComponent<RectTransform> (), color [1], 0.1f);
		}
    }
}
