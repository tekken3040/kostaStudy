using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class PressPopup : MonoBehaviour 
{
	public Text _PopupMessage;

	public virtual void SetData(string message)
	{
		_PopupMessage.text = message;
	}
}
