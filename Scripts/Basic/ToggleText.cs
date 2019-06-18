using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Toggle))]
public class ToggleText : MonoBehaviour {

	public bool useOutLine;
	public Color textColor;
	public Color outLineColor;
	public Vector2 outLineDist;
	public Text text;

	private Color defaultColor;
	private bool toggleOn = false;

	void Awake()
	{
		Toggle toggle = GetComponent<Toggle>();
		toggle.onValueChanged.AddListener( (x) => onValueChange(x) );

		defaultColor = text.color;

		onValueChange(toggle.isOn);

		toggleOn = toggle.isOn;
	}

	private void onValueChange(bool isOn)
	{
		if(toggleOn == isOn)
			return;

		toggleOn = isOn;

		if(isOn)
		{
			text.color = textColor;
			if(useOutLine)
			{
				Outline textOutline = text.gameObject.AddComponent<Outline>();
				textOutline.effectDistance = outLineDist;
				textOutline.effectColor = outLineColor;
			}
		}
		else
		{
			text.color = defaultColor;
			if(useOutLine)
			{
				Destroy(text.GetComponent<Outline>());
			}
		}
	}
}
