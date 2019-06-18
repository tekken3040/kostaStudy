using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleColor : MonoBehaviour {

	public enum ChangeType
	{
		OnOff,
		Change,
		None
	}
	
	public ChangeType changeType = ChangeType.OnOff;
	public Image image;
	public Color changeColor;
	public Color defaultColor;
	
	private bool toggleOn = false;
	
	void Awake()
	{
		Toggle toggle = GetComponent<Toggle>();
		toggle.onValueChanged.AddListener( (x) => onValueChange(x) );
		
		defaultColor = image.color;
		
		onValueChange(toggle.isOn);
		
		toggleOn = toggle.isOn;
	}
	
	public void SetDefaultColor()
	{
		image.color = defaultColor;
	}
	
	private void onValueChange(bool isOn)
	{
		if(changeType == ChangeType.None)
		{
			image.gameObject.SetActive(true);
			return;
		}
		
		if(toggleOn == isOn)
			return;
		
		toggleOn = isOn;
		
		if(isOn)
		{
			switch(changeType)
			{
			case ChangeType.OnOff:
				image.gameObject.SetActive(isOn);
				break;
				
			case ChangeType.Change:
				image.color = changeColor;
				break;
			}
		}
		else
		{
			switch(changeType)
			{
			case ChangeType.OnOff:
				image.gameObject.SetActive(isOn);
				break;
				
			case ChangeType.Change:
				image.color = defaultColor;
				break;
			}
		}
	}
}
