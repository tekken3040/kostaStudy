using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UI_SubPanel_CharacterInfo_Equipment_StatInfo : MonoBehaviour {
	[SerializeField] Text _txtStatName;
	[SerializeField] Text _txtStatDescription;

    byte _u1StatType;
	public void SetData(byte statType)
	{
		_u1StatType = statType;
	
		_txtStatName.text = Status.GetStatText(statType);
		_txtStatDescription.text = Status.GetStatDescription(statType);
	}
}
