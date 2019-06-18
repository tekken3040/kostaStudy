using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

// 아이템 클릭 했을때 아이템 정보를 보여준다
public class ItemInfoPopup : MonoBehaviour {

	public RectTransform itemInfoObj;
	public Image itemIcon;
	public Image itemGrade;
	public Text itemName;
	public Text itemDesc;
	
	public void SetInfo(Byte itemType, UInt16 itemID, Vector3 pos, float scale)
	{        
        itemGrade.gameObject.SetActive(true);
        itemName.gameObject.SetActive(true);
		itemInfoObj.position = transform.root.position - pos * (transform.root.localScale.x / scale);
        
        if(itemInfoObj.anchoredPosition3D.x + 160f >= (transform.root.GetComponent<RectTransform>().sizeDelta.x / 2f - 180f))
            itemInfoObj.anchoredPosition3D = new Vector3(transform.root.GetComponent<RectTransform>().sizeDelta.x / 2f - 180f, itemInfoObj.anchoredPosition3D.y + 75f, 0f);
        else
            itemInfoObj.anchoredPosition3D = new Vector3(itemInfoObj.anchoredPosition3D.x + 160f, itemInfoObj.anchoredPosition3D.y + 75f, 0f);

		if(itemType == (Byte)GoodsType.CASH){
			itemIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.icon_Cash");
			itemName.text = TextManager.Instance.GetText ("mark_cash");
			itemDesc.text = TextManager.Instance.GetText ("cash_desc");
		}else if(itemType == (Byte)GoodsType.GOLD){
			itemIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.icon_Gold");
			itemName.text = TextManager.Instance.GetText ("mark_gold");
			itemDesc.text = TextManager.Instance.GetText ("gold_desc");
		}else if(itemType == (Byte)GoodsType.KEY){
			itemIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.icon_Key");
			itemName.text = TextManager.Instance.GetText ("mark_key");
			itemDesc.text = TextManager.Instance.GetText ("key_desc");
		}else if(itemType == (Byte)GoodsType.LEAGUE_KEY){
			itemIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.icon_Key");
			itemName.text = TextManager.Instance.GetText ("mark_leaguekey");
			itemDesc.text = TextManager.Instance.GetText ("leaguekey_desc");
		}else if(itemType == (Byte)GoodsType.FRIENDSHIP_POINT){
			itemIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.icon_friendship");
			itemName.text = TextManager.Instance.GetText ("mark_friendshippoint");
			itemDesc.text = TextManager.Instance.GetText ("friendsshoippoint_desc");
		}else if(itemType == (Byte)GoodsType.EQUIP_COUPON){
			itemIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Shop/shop_02.equip_drawing");
			itemDesc.text = TextManager.Instance.GetText(ShopInfoMgr.Instance.dicShopGoodData[itemID].title);
			itemName.text = TextManager.Instance.GetText("mark_equipment_gacha"); 
		}else if(itemType == (Byte)GoodsType.MATERIAL_COUPON){
			itemIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Shop/shop_02.stuff_drawing");
			itemDesc.text = TextManager.Instance.GetText(ShopInfoMgr.Instance.dicShopGoodData[itemID].title);
			itemName.text = TextManager.Instance.GetText("mark_material_gacha"); 
		}else if(itemType == (Byte)GoodsType.TRAINING_ROOM){
			itemIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.icon_lock_gold");
			itemName.text = TextManager.Instance.GetText ("btn_guild_main_tra_char");
			itemDesc.text = TextManager.Instance.GetText ("chartraining_desc");
		}else if(itemType == (Byte)GoodsType.EQUIP_TRAINING){
			itemIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.icon_lock_gold");
			itemName.text = TextManager.Instance.GetText ("btn_guild_main_tra_equip");
			itemDesc.text = TextManager.Instance.GetText ("equiptraining_desc");
		}else if(itemType == (Byte)GoodsType.RANDOM_REWARD){
            itemIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.icon_random_reward");
            itemDesc.text = TextManager.Instance.GetText ("random_reward_desc");
            itemName.text = TextManager.Instance.GetText ("random_reward_title");
            itemGrade.gameObject.SetActive(false);
		}else if(itemType == (Byte)GoodsType.EVENT_ITEM){
			itemIcon.sprite = AtlasMgr.Instance.GetGoodsIcon (new Goods (itemType, itemID, 1));
			itemDesc.text = TextManager.Instance.GetText(EventInfoMgr.Instance.dicMarbleGoods[itemID].sDesc);
			itemName.text = Legion.Instance.GetGoodsName (new Goods (itemType, itemID, 1));
			itemGrade.gameObject.SetActive(false);
		}else if(itemType == (Byte)GoodsType.EQUIP_GOODS){
            EquipmentInfo info = EquipmentInfoMgr.Instance.GetInfo(EventInfoMgr.Instance.dicClassGoodsEquip[itemID].u2Equip);
            itemIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(new Goods(itemType, itemID, 1));
            itemDesc.text = TextManager.Instance.GetText(info.sDescription);
            itemName.text = TextManager.Instance.GetText(info.sName); 
            itemGrade.gameObject.SetActive(false);
        }else{
			switch (ItemInfoMgr.Instance.GetItemType (itemID)) {
			case ItemInfo.ITEM_ORDER.CONSUMABLE:
				itemIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Item/Item_01." + itemID);
				itemGrade.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade (itemID));
				itemName.text = TextManager.Instance.GetText (ItemInfoMgr.Instance.GetConsumableItemInfo (itemID).sName);
				itemDesc.text = TextManager.Instance.GetText (ItemInfoMgr.Instance.GetConsumableItemInfo (itemID).sDescription);
				break;
				
			case ItemInfo.ITEM_ORDER.MATERIAL:
				itemIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Item/Item_01." + ItemInfoMgr.Instance.GetMaterialItemInfo (itemID).u2IconID);
				itemGrade.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade (itemID));
				itemName.text = TextManager.Instance.GetText (ItemInfoMgr.Instance.GetMaterialItemInfo (itemID).sName);
				itemDesc.text = TextManager.Instance.GetText (ItemInfoMgr.Instance.GetMaterialItemInfo (itemID).sDescription);
				break;
	            
			case ItemInfo.ITEM_ORDER.EQUIPMENT:
				itemIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Item/" + EquipmentInfoMgr.Instance.GetInfo(itemID).cModel.sImagePath);
				itemGrade.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade (itemID));
				itemName.text = TextManager.Instance.GetText (EquipmentInfoMgr.Instance.GetInfo (itemID).sName);
				itemDesc.text = TextManager.Instance.GetText (EquipmentInfoMgr.Instance.GetInfo (itemID).sDescription);
				break;

			case ItemInfo.ITEM_ORDER.DESIGN:
				itemIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Item/Item_01.2039");
				itemGrade.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade (itemID));
				itemName.text = TextManager.Instance.GetText (EquipmentInfoMgr.Instance.GetEquipmentSetInfo (itemID).sName);
				itemDesc.text = TextManager.Instance.GetText (EquipmentInfoMgr.Instance.GetEquipmentSetInfo (itemID).sDescription);
				break;
			}
		}
        PopupManager.Instance.AddPopup(gameObject, Close);
	}
	
	public void Close()
	{
        PopupManager.Instance.RemovePopup(gameObject);
		gameObject.SetActive(false);
	}
}
