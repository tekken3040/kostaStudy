using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class CharInfoPopup : MonoBehaviour
{
    [SerializeField] Text txtClassName;
    [SerializeField] Text txtCharName;
    [SerializeField] Text txtCharLevel;
    [SerializeField] Text txtCharPower;
    [SerializeField] Text[] StatValues;

    [SerializeField] Image imgCharIcon;
    [SerializeField] Image imgCharElement;
    [SerializeField] Image imgCharElementClassName;

    [SerializeField] UI_ListElement_Forge_Fusion_Material[] _slots;

    StringBuilder tempStringBuilder;

    String[] strColor;

    void Awake()
    {
        strColor = new String[3];
        strColor[0] = "a21319";
        strColor[1] = "3358ae";
        strColor[2] = "80a685";
    }

    public void SetData(Hero _hero)
    {
        tempStringBuilder = new StringBuilder();
        txtClassName.text = TextManager.Instance.GetText(_hero.cClass.sName);
        txtCharName.text = _hero.sName;
        tempStringBuilder.Append("Lv ").Append(_hero.cLevel.u2Level);
        txtCharLevel.text = tempStringBuilder.ToString();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
		tempStringBuilder.Append(TextManager.Instance.GetText("mark_league_power")).Append(" ").Append(_hero.u4Power);
        txtCharPower.text = tempStringBuilder.ToString();
        imgCharIcon.sprite = AssetMgr.Instance.AssetLoad("Sprites/Tutorial/class_"+_hero.cClass.u2ID.ToString("000")+".png", typeof(Sprite)) as Sprite;
        //if(_hero.acEquips[(Byte)(EquipmentInfo.EQUIPMENT_POS.WEAPON_R-1)].GetEquipmentInfo().u1Element
        imgCharElement.color = EquipmentItem.equipElementColors[_hero.acEquips[(Byte)(EquipmentInfo.EQUIPMENT_POS.WEAPON_R-1)].GetEquipmentInfo().u1Element];
        imgCharElementClassName.color = EquipmentItem.equipElementColors[_hero.acEquips[(Byte)(EquipmentInfo.EQUIPMENT_POS.WEAPON_R-1)].GetEquipmentInfo().u1Element];
        for(int i=0; i<Server.ConstDef.MaxItemSlot; i++)
        {
            _slots[i].SetData(_hero.acEquips[i], _hero.acEquips[i], true);
        }
		Status skillResult = _hero.GetComponent<SkillComponent>().GetPassiveStatus();
        for(int i=0; i<Server.ConstDef.CharStatPointType; i++)
        {
			StatValues[i].text = (_hero.cFinalStatus.GetStat((Byte)(i+1)) + skillResult.GetStat((Byte)(i+1))).ToString();
        }
    }
}
