using UnityEngine;
using UnityEngine.UI;
using System;

public class UI_SubPanel_CharacterInfo_Equipment_StateInfo : MonoBehaviour
{
    [SerializeField] Text _txtStatName;
	[SerializeField] Text _txtStatDescription;

	Byte _u1StateType;
	public void SetData(Byte stateType)
	{
		_u1StateType = stateType;
	
        switch(_u1StateType)
        {
            case (Byte)ClassInfo.ATTACK_ELEMENT.PHYSICAL:
                _txtStatName.text = TextManager.Instance.GetText("phy");
                _txtStatDescription.text = TextManager.Instance.GetText("phy_desc");
                break;

            case (Byte)ClassInfo.ATTACK_ELEMENT.MAGICAL:
                _txtStatName.text = TextManager.Instance.GetText("mag");
                _txtStatDescription.text = TextManager.Instance.GetText("mag_desc");
                break;

            case (Byte)ClassInfo.ATTACK_ELEMENT.OFFENSIVE:
                _txtStatName.text = TextManager.Instance.GetText("equip_atk");
                _txtStatDescription.text = TextManager.Instance.GetText("equip_atk_desc");
                break;

            case (Byte)ClassInfo.ATTACK_ELEMENT.DEFENSIVE:
                _txtStatName.text = TextManager.Instance.GetText("equip_def");
                _txtStatDescription.text = TextManager.Instance.GetText("equip_def_desc");
                break;

            case (Byte)ClassInfo.ATTACK_ELEMENT.BALANCE:
                _txtStatName.text = TextManager.Instance.GetText("equip_bal");
                _txtStatDescription.text = TextManager.Instance.GetText("equip_bal_desc");
                break;

            case (Byte)ClassInfo.ATTACK_ELEMENT.SPECIALIZE:
                _txtStatName.text = TextManager.Instance.GetText("equip_tal");
                _txtStatDescription.text = TextManager.Instance.GetText("equip_tal_desc");
                break;
        }
	}
}
