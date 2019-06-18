using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.EventSystems;
public class UI_ItemListElement_Equipment : UI_ItemListElement
{
	[SerializeField] Image _imgEquipIcon;
	[SerializeField] Text _txtLevel;
	[SerializeField] GameObject[] _objStars;

	public UInt16 invenSlotNum;
	public EquipmentItem equipItem;
	public AudioClip TouchClip;
	public AudioClip DragClip;

	public void SetData(UInt16 slotNum)
	{

		invenSlotNum = slotNum;
		EquipmentItem equipItem = (EquipmentItem)Legion.Instance.cInventory.dicInventory[slotNum];
		SetData(equipItem);
	}

	public void SetData(Item item)
	{
		base.slotNum = item.u2SlotNum;
		equipItem = (EquipmentItem)item;
		_imgEquipIcon.sprite = AssetMgr.Instance.AssetLoad("Sprites/item_icon/"+equipItem.cItemInfo.u2ID + "png", typeof(Sprite)) as Sprite;
//		AtlasMgr.Instance.SetGrayScale(_imgEquipIcon);

		_txtLevel.text = "LV." + equipItem.cLevel.u2Level.ToString();
//		_txtLevel.text = equipItem.u2SlotNum.ToString();
        // RKH TO DO
		//for(int i=0; i<equipItem.cLevel.u1Star; i++)
		//{
		//	_objStars[i].SetActive(true);
		//}

		//for(int i=equipItem.cLevel.u1Star; i<Level.STAR_MAX_EQUIP; i++) 
		//{
		//	_objStars[i].SetActive(false);
		//}


		transform.name = equipItem.u2SlotNum.ToString();

	}

	public void OnClickSound()
	{
		this.gameObject.GetComponent<SoundPlayer>().audioClip = TouchClip;
		this.gameObject.GetComponent<SoundPlayer>().enabled = false;
		this.gameObject.GetComponent<SoundPlayer>().enabled = true;
	}
}