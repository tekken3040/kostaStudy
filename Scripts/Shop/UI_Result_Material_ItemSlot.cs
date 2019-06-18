using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_Result_Material_ItemSlot : MonoBehaviour
{
    [SerializeField] Image _grade;
    [SerializeField] Image _icon;
    [SerializeField] Text _txtCount;
    [SerializeField] Text _txtName;
    [SerializeField] Button Btn_itemWindow;

    ItemInfoWindow _cItemInfoWindow;
    MaterialItem _cMaterialItem;
	ConsumableItem _cConsumableItem;

    public void SetData(MaterialItem _material, ItemInfoWindow _itemWindow)
    {
        Btn_itemWindow.interactable = false;
        _cItemInfoWindow = _itemWindow;
        _cMaterialItem = _material;
        _icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + _material.GetMaterialItemInfo().u2IconID);
        _icon.SetNativeSize();
        _grade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + _material.GetMaterialItemInfo().u2Grade);
        _grade.SetNativeSize();

        _txtCount.text = _material.u2Count.ToString();
        _txtName.text = TextManager.Instance.GetText(_material.GetMaterialItemInfo().sName);

        //StartCoroutine(StartAnimations());
    }

	public void SetData(ConsumableItem _consumable, ItemInfoWindow _itemWindow)
	{
		Btn_itemWindow.interactable = false;
		_cItemInfoWindow = _itemWindow;
		_cConsumableItem = _consumable;
		_icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + _consumable.GetItemInfo().u2ID);
		_icon.SetNativeSize();
		_grade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + _consumable.GetItemInfo().u2Grade);
		_grade.SetNativeSize();

		_txtCount.text = _consumable.u2Count.ToString();
		_txtName.text = TextManager.Instance.GetText(_consumable.GetItemInfo().sName);

		//StartCoroutine(StartAnimations());
	}

    public void OnClickItem()
    {
        PopupManager.Instance.AddPopup(_cItemInfoWindow.gameObject, _cItemInfoWindow.OnClickClose);
        _cItemInfoWindow.gameObject.SetActive(true);
		if(_cMaterialItem != null)
        	_cItemInfoWindow.SetInfo(Legion.Instance.cInventory.dicInventory[_cMaterialItem.u2SlotNum]);
		else if(_cConsumableItem != null)
			_cItemInfoWindow.SetInfo(Legion.Instance.cInventory.dicInventory[_cConsumableItem.u2SlotNum]);
    }

    IEnumerator StartAnimations()
    {
        yield return new WaitForSeconds(1f);
        GetComponent<Animator>().enabled = true;
        yield return new WaitForSeconds(1f);
        Btn_itemWindow.interactable = true;
    }
}
