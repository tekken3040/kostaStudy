using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CustomItem : MonoBehaviour {

	public Image itemIcon;
	public Text itemText;

	public void SetItem(string name, Sprite icon)
	{
		itemText.text = name;
		itemIcon.sprite = icon;
		itemIcon.SetNativeSize();
	}

	public void SetImageColor(Color32 color)
	{
		itemIcon.color = color;
	}
}
