using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UI_ItemListElement_Common : UI_ItemListElement {
	[SerializeField] Image _imgItemGrade;
	[SerializeField] Image _imgItemIcon;
	[SerializeField] Text _txtCount;
	[SerializeField] RewardButton _cReeardButton;
	[SerializeField] GameObject _CountBG;
	[SerializeField] GameObject _CountBG2;
	[SerializeField] UI_ItemEquipSlot _EquipSlot;

	public void SetData(Item item)
	{
		if(_EquipSlot != null)
			_EquipSlot.gameObject.SetActive(false);
		_imgItemIcon.transform.parent.gameObject.SetActive(true);

		base.slotNum = item.u2SlotNum;

		int count=0;
		if(item is MaterialItem)
		{
			_imgItemIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Item/item_01." + ((MaterialItemInfo)item.cItemInfo).u2IconID);
			count = ((MaterialItem)item).u2Count;
		}
		else if(item is ConsumableItem)
		{
			_imgItemIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Item/item_01." + item.cItemInfo.u2ID);
			count = ((ConsumableItem)item).u2Count;
		}
		_txtCount.text = count.ToString();

		_imgItemGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(item.cItemInfo.u2ID));
		transform.name = item.u2SlotNum.ToString();
	}

	public void SetData(Goods goods, bool isTextOut = false)
	{
		if(_EquipSlot != null)
			_EquipSlot.gameObject.SetActive(false);
		_imgItemIcon.transform.parent.gameObject.SetActive(true);

		_CountBG2.SetActive(false);
        if (goods.isEquip())
        {
            EquipmentInfo temp = EquipmentInfoMgr.Instance.GetInfo(goods.u2ID);
            _imgItemIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/" + temp.cModel.sImagePath);
        }
        else if(goods.u1Type == (Byte)GoodsType.EQUIP_GOODS)
        {
            _imgItemIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(goods);
        }
        else if (goods.u1Type == (Byte)GoodsType.CASH) {
            _imgItemIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_Cash");
            _CountBG2.SetActive(true);
        } else if (goods.u1Type == (Byte)GoodsType.GOLD) {
            _imgItemIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_Gold");
            _CountBG2.SetActive(true);
        } else if (goods.u1Type == (Byte)GoodsType.KEY) {
            _imgItemIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_Key");
            _CountBG2.SetActive(true);
        } else if (goods.u1Type == (Byte)GoodsType.CONSUME) {
            _imgItemIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/Item_01." + goods.u2ID);
            //_imgItemGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(goods.u2ID));
        } else if (goods.u1Type == (Byte)GoodsType.MATERIAL) {
            _imgItemIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/Item_01." + ItemInfoMgr.Instance.GetMaterialItemInfo(goods.u2ID).u2IconID);
            //_imgItemGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(goods.u2ID));
        } else if (goods.u1Type == (Byte)GoodsType.SCROLL || goods.u1Type == (Byte)GoodsType.SCROLL_SET) {
            _imgItemIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/Item_01.2039");
        } else if (goods.u1Type == (Byte)GoodsType.FRIENDSHIP_POINT) {
            _imgItemIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_friendship");
            _CountBG2.SetActive(true);
        } else if (goods.u1Type == (Byte)GoodsType.TRAINING_ROOM || goods.u1Type == (Byte)GoodsType.EQUIP_TRAINING) {
            _imgItemIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_lock_gold");
        } else if (goods.u1Type == (Byte)GoodsType.EVENT_ITEM) {
            _imgItemIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(goods);
            _imgItemGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_04_renew.event_item_grade_bg");
        }
        else if (goods.u1Type == (Byte)GoodsType.EQUIP_COUPON)
        {
            _imgItemIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/Item_01.2721");//AtlasMgr.Instance.GetSprite("Sprites/Common/");
            _imgItemGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_04_renew.event_item_grade_bg");
        }

        if (goods.u1Type != (Byte)GoodsType.EVENT_ITEM){
            _imgItemGrade.sprite = AtlasMgr.Instance.GetGoodsGrade(goods);
        }

		_imgItemIcon.SetNativeSize();
		transform.name = goods.u2ID.ToString();

		// 기본 보상에서는 아이템 갯수를 표시하지 않는다
		if(isTextOut == false)
			_txtCount.text = goods.u4Count.ToString();
		else
			_txtCount.text = "?";
		
		if(_CountBG2.activeSelf == true)
		{
			_txtCount.transform.SetParent(_CountBG2.transform);
			_CountBG.SetActive(false);
		}
		else
		{
			_CountBG.SetActive(true);
			_txtCount.transform.SetParent(_CountBG.transform);
		}
		_txtCount.transform.localPosition = Vector3.zero;
		_cReeardButton.SetButton(goods.u1Type, goods.u2ID);
	}

	public void SetData(AchieveItem info)
	{
		_imgItemIcon.transform.parent.gameObject.SetActive(false);
		_EquipSlot.gameObject.SetActive(true);

		_EquipSlot.SetData(info);

		_cReeardButton.SetButton(info.cAchieveReward.u1Type, info.cAchieveReward.u2ID);
	}

	public void SetRandemItemData()
	{
		_imgItemIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_random_reward");
		_imgItemIcon.SetNativeSize();
		_imgItemGrade.sprite =  AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_4570");
		_cReeardButton.SetButton ((Byte)GoodsType.RANDOM_REWARD, 0);
		transform.name = GoodsType.RANDOM_REWARD.ToString();
		_CountBG.SetActive(true);
		_CountBG2.SetActive(false);
		_txtCount.text = "?";
		_txtCount.transform.SetParent(_CountBG.transform);
		_txtCount.transform.localPosition = Vector3.zero;	}
}
