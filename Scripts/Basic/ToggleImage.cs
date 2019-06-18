using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Toggle))]
public class ToggleImage : MonoBehaviour {

	public enum ChangeType
	{
		OnOff,
		Change,
		None
	}

	public ChangeType changeType = ChangeType.OnOff;
	public Image image;
	public Sprite changeSprite;
	public Sprite defaultSprite;

	private bool toggleOn = false;
	
	void Awake()
	{
		Toggle toggle = GetComponent<Toggle>();
		toggle.onValueChanged.AddListener( (x) => onValueChange(x) );

		defaultSprite = image.sprite;

		onValueChange(toggle.isOn);
		if( changeType == ChangeType.OnOff)
			image.gameObject.SetActive(false);

		toggleOn = toggle.isOn;
	}
    void OnEnable()
    {
        onValueChange(GetComponent<Toggle>().isOn);
    }
	public void SetDefaultImage()
	{
		image.sprite = defaultSprite;
	}
	
	private void onValueChange(bool isOn)
	{
		if(changeType == ChangeType.None)
		{
			image.gameObject.SetActive(true);
			return;
		}

		if(toggleOn == isOn)
        {
			if(changeType == ChangeType.OnOff)
			{
				if(image.gameObject.active != toggleOn)
					image.gameObject.SetActive(toggleOn);
			}
			return;
        }

		toggleOn = isOn;

		if(isOn)
		{
			switch(changeType)
			{
			case ChangeType.OnOff:
				image.gameObject.SetActive(isOn);
				break;

			case ChangeType.Change:
				image.sprite = changeSprite;
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
				image.sprite = defaultSprite;
				break;
			}
		}
	}
}
