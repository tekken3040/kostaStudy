using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_SubPanel_CharacterInfo_Equipment_ModelViewer : MonoBehaviour {
	[SerializeField] Image _imgElementIcon;
	[SerializeField] Text _txtName;
	[SerializeField] Text _txtPos;
	Transform equipTr;
	EquipmentItem _cEquipItem;
	public void SetData(EquipmentItem equipItem)
	{
		_cEquipItem = equipItem;
		SetName();
		_txtPos.text = equipItem.GetEquipmentInfo().u1PosID.ToString();
		SetElement();
		SetIcon();
		equipItem.InitViewModelObject();
		equipTr = equipItem.cObject.transform;
		equipTr.parent = transform;
		equipTr.localPosition = new Vector3(0f, 0f, -500f);
		equipTr.localScale = new Vector3(1000f, 1000f, 1000f);
		equipTr.GetComponent<EquipmentObject>().SetLayer( LayerMask.NameToLayer("UI") );
	}
	void SetIcon()
	{
		if( _cEquipItem.GetEquipmentInfo().u1Element > 0)
		{
			Sprite elementSprite = AtlasMgr.Instance.GetSprite("Sprites/resource_02.Element_" + _cEquipItem.GetEquipmentInfo().u1Element);
			_imgElementIcon.sprite = elementSprite;
		}
	}
	void SetName()
	{
		transform.name = _cEquipItem.u2SlotNum.ToString();
		_txtName.text = TextManager.Instance.GetText(_cEquipItem.GetEquipmentInfo().sName);
	}
	
	void SetElement()
	{
		if(_cEquipItem.GetEquipmentInfo().u1PosID == EquipmentInfo.EQUIPMENT_POS.WEAPON_L || 
		   _cEquipItem.GetEquipmentInfo().u1PosID == EquipmentInfo.EQUIPMENT_POS.WEAPON_R)
		{
			_txtName.color = EquipmentItem.equipElementColors[_cEquipItem.GetEquipmentInfo().u1Element-1];
		}
		else
		{
			_txtName.color = EquipmentItem.equipElementColors[0];
		}
	}
	void OnDisable()
	{
		Destroy(equipTr.gameObject);
	}
}
