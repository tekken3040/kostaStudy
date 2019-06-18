using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Panel_Forge_ChangeLook_Result_Module : MonoBehaviour
{
    [SerializeField] Text _txtEquipName;
	[SerializeField] SpriteRenderer _imgAccessory;
	[SerializeField] RectTransform _trEquipParent;
	EquipmentItem _cEquipItem;
	GameObject _objEquipModel;
	GameObject _objEquipEffect;
	public void SetData(EquipmentItem equipItem)
	{
		_cEquipItem = equipItem;
		string equipName = "";
		equipName = equipItem.itemName;
		if(equipName != "")
			equipName = equipName + " " + TextManager.Instance.GetText( equipItem.GetEquipmentInfo().sName );
		else
			equipName = TextManager.Instance.GetText( equipItem.GetEquipmentInfo().sName );
		_txtEquipName.text = TextManager.Instance.GetText("forge_level_" + equipItem.u1SmithingLevel) + " " + equipName;
		if(equipItem.GetEquipmentInfo().u1Element != 0)
			_txtEquipName.color = EquipmentItem.equipElementColors[(equipItem.GetEquipmentInfo().u1Element)];
		else
			_txtEquipName.color = Color.white;
		if(equipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 && equipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
		{
			DebugMgr.Log("Result : " + equipItem.u2SlotNum + "," + equipItem.u2ModelID);
			equipItem.InitViewModelObject();
			equipItem.cObject.GetComponent<EquipmentObject>().SetLayer( LayerMask.NameToLayer("UI") );
			equipItem.cObject.transform.SetParent( _trEquipParent );
			_trEquipParent.GetComponent<RotateCharacter>().characterTransform = equipItem.cObject.transform;
			equipItem.cObject.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
			equipItem.cObject.transform.localPosition = new Vector3(0, -110, -200f);
			_objEquipModel = equipItem.cObject;
			_imgAccessory.gameObject.SetActive(false);
		}
		else
		{
			_imgAccessory.sprite = AssetMgr.Instance.AssetLoad("Sprites/Item/Accessory/acc_" + equipItem.u2ModelID + ".png", typeof(Sprite)) as Sprite;
			_imgAccessory.gameObject.SetActive(true);
		}
		if(_objEquipEffect != null) DestroyObject(_objEquipEffect);
		if(equipItem.u1SmithingLevel >= 1)
		{
			DebugMgr.Log("Asset : " +string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}", equipItem.u1SmithingLevel));
			_objEquipEffect = Instantiate(AssetMgr.Instance.AssetLoad(string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}.prefab", equipItem.u1SmithingLevel), typeof(GameObject))) as GameObject;
			_objEquipEffect.transform.SetParent(_trEquipParent);
			_objEquipEffect.transform.name = "WeaponEffect";
			_objEquipEffect.transform.localScale = new Vector3(1f, 1f, 1f);
			_objEquipEffect.transform.localPosition = Vector3.zero;
		}
	}

	void OnDisable()
	{
		if(_cEquipItem != null)
		Destroy(_cEquipItem.cObject);
	}
}
