using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// 아이템 정보 창
public class ShopInfoWindow : MonoBehaviour {
    
    public delegate void OnClickBuy();
    public OnClickBuy onClickBuy;
    
    public enum ShopType
    {
        Equip,
        Item,
        Goods
    }
    
    public Text title;
    public Text itemName;
    
    //Equip
    public GameObject equipObject;
    public Image equipIcon;
    public Image equipGrade;
    public Text equipDesc;
    
    //3D ItemModel
    public GameObject itemModel;
    
    //Item
    public GameObject itemObject;
    public Image itemIcon;
    public Image itemGrade;
    public Text itemCount;
    public Text itemDesc;
    
    public GameObject goodObject;
    public Image goodIcon;
    public Text goodValue;
    public Image priceIcon;
    public Text priceTypeText;
    public Text priceValue;
    
    public void SetInfo(ShopType type, ShopItem shopItem, string popupTitle)
    {
        switch(type)
        {
            case ShopType.Equip:
            equipObject.SetActive(true);
            itemObject.SetActive(false);
            goodObject.SetActive(false);
            itemName.gameObject.SetActive(true);
            title.text = popupTitle;
            break;
            
            case ShopType.Item:
            equipObject.SetActive(false);
            itemObject.SetActive(true);
            goodObject.SetActive(false);
            itemName.gameObject.SetActive(true);
            title.text = popupTitle;
            break;
        }
        
        priceTypeText.gameObject.SetActive(false);        
        
		ItemInfo.ITEM_ORDER orderType = ItemInfoMgr.Instance.GetItemType(shopItem.u2ItemID);

		uint price = shopItem.u4Price;
		if (type == ShopType.Item) {
			EventDisCountinfo disInfo = EventInfoMgr.Instance.GetDiscountEventinfo (DISCOUNT_ITEM.BUYSHOP);
			if (disInfo != null) {
				price = (uint)(shopItem.u4Price * disInfo.discountRate);
			}
		}

        // 아이템 타입에 따라서 정보 세팅
		switch(orderType)
		{
		case ItemInfo.ITEM_ORDER.MATERIAL:
			itemIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + ItemInfoMgr.Instance.GetMaterialItemInfo(shopItem.u2ItemID).u2IconID);
            itemGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(shopItem.u2ItemID));
			MaterialItemInfo materialInfo = ItemInfoMgr.Instance.GetMaterialItemInfo(shopItem.u2ItemID);
			itemName.text = string.Format("{0} x{1}", TextManager.Instance.GetText(materialInfo.sName), shopItem.u4Count);
			itemDesc.text = TextManager.Instance.GetText(materialInfo.sDescription);
            itemCount.text = shopItem.u4Count.ToString();
            itemGrade.SetNativeSize();
			break;

		case ItemInfo.ITEM_ORDER.CONSUMABLE:
			itemIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + shopItem.u2ItemID);
            itemGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(shopItem.u2ItemID));
			ConsumableItemInfo consumableInfo = ItemInfoMgr.Instance.GetConsumableItemInfo(shopItem.u2ItemID);
			itemName.text = string.Format("{0} x{1}", TextManager.Instance.GetText(consumableInfo.sName), shopItem.u4Count);
			itemDesc.text = TextManager.Instance.GetText(consumableInfo.sDescription);
            itemCount.text = shopItem.u4Count.ToString();
            itemGrade.SetNativeSize();
			break;
		// case ItemInfo.ITEM_ORDER.RUNE:
		// 	RuneInfo runeInfo = ItemInfoMgr.Instance.GetRuneInfo(shopItem.u2ItemID);
		// 	itemIcon.sprite = AtlasMgr.Instance.GetSprite(string.Format("Sprites/Item/rune_01.rune_{0}_{1}", runeInfo.u1Type, runeInfo.u1IncreaseType));
		// 	itemName.text = string.Format("LV.{0} {1} x{2}",runeInfo.u2Level, TextManager.Instance.GetText(runeInfo.sName), shopItem.u1Count);
		// 	itemDesc.text = ItemInfoMgr.Instance.GetRuneDescription(runeInfo, false);
		// 	break;
		case ItemInfo.ITEM_ORDER.EQUIPMENT:
        
            EquipmentInfo equipInfo = EquipmentInfoMgr.Instance.GetInfo(shopItem.u2ItemID);
            
            ModelInfo modelInfo = ModelInfoMgr.Instance.GetInfo(equipInfo.u2ModelID);
            
            if(modelInfo != null)
            {
                ForgeInfo forgeInfo = ForgeInfoMgr.Instance.GetList()[shopItem.cEquipInfo.u1SmithingLevel-1];
                
                string imagePath = "Sprites/Item/" + modelInfo.sImagePath;         
                equipIcon.sprite = AtlasMgr.Instance.GetSprite(imagePath);
                equipGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + forgeInfo.u2ID);
                equipIcon.transform.localScale = Vector3.one;
                equipIcon.SetNativeSize();
                equipGrade.SetNativeSize();
                
            }
			
			itemName.text = string.Format("{0} {1}", shopItem.cEquipInfo.strItemName, TextManager.Instance.GetText(equipInfo.sName));
            equipDesc.text = TextManager.Instance.GetText(equipInfo.EquipTypeKey());
			break;                        
		}
		
        // 가격 정보 셋팅
		Goods good = new Goods(shopItem.u1PriceType, 0, price);
        priceIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(good);
		priceValue.text = string.Format("{0}", price);   
        
        PopupManager.Instance.AddPopup(gameObject, OnClickClose);     
    }
    
	uint GetDiscountPrice(ShopGoodInfo info){
		uint price = info.cBuyGoods.u4Count;
		byte disRate = 0;
		if (info.u1Type < 5) {
			EventDisCountinfo disInfo = EventInfoMgr.Instance.GetDiscountEventinfo (DISCOUNT_ITEM.LOTTERY);
			if (disInfo != null) {
				price = (uint)(info.cBuyGoods.u4Count * disInfo.discountRate);
				disRate = disInfo.u1DiscountRate;
			}
		} else if (info.u1Type == 6) {
			EventDisCountinfo disInfo = EventInfoMgr.Instance.GetDiscountEventinfo (DISCOUNT_ITEM.BUYGOLD);
			if (disInfo != null){
				price = (uint)(info.cBuyGoods.u4Count * disInfo.discountRate);
				disRate = disInfo.u1DiscountRate;
			}
		} else if (info.u1Type == 7) {
			EventDisCountinfo disInfo = EventInfoMgr.Instance.GetDiscountEventinfo (DISCOUNT_ITEM.BUYKEY);
			if (disInfo != null){
				price = (uint)(info.cBuyGoods.u4Count * disInfo.discountRate);
				disRate = disInfo.u1DiscountRate;
			}
		}

		return price;
	}

    public void SetInfo(ShopGoodInfo shopGoodInfo, string popupTitle)
    {
        title.text = popupTitle;
        
        equipObject.SetActive(false);
        itemObject.SetActive(false);
        goodObject.SetActive(true);
        itemName.gameObject.SetActive(false);
        
		if(shopGoodInfo.u1Type == 9) goodIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Shop/" + shopGoodInfo.imagePath);
		else goodIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02." + shopGoodInfo.imagePath);

        goodIcon.SetNativeSize();
		goodIcon.GetComponent<RectTransform>().sizeDelta *= 0.8f;
        int addValue = 0;
		addValue = LegionInfoMgr.Instance.GetAddVipValue(shopGoodInfo) + shopGoodInfo.GetBuyBonus();
        if (shopGoodInfo.u1Type == 9)
        {
            // 천원샵
            goodValue.text = TextManager.Instance.GetText(shopGoodInfo.title + "_desc"); ;
        }
        else if (addValue > 0)
        {
            goodValue.text = TextManager.Instance.GetText(shopGoodInfo.title) + "(+" + addValue + ")";
        }
        else
        {
            goodValue.text = TextManager.Instance.GetText(shopGoodInfo.title);
        }

		uint price = GetDiscountPrice (shopGoodInfo);

		Goods good = new Goods(shopGoodInfo.cBuyGoods.u1Type, 0, price);
        priceIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(good);
		priceValue.text = string.Format("{0}", price);
        
        if((GoodsType)shopGoodInfo.cBuyGoods.u1Type == GoodsType.PURCHASE)
        {
            priceIcon.gameObject.SetActive(false);
            priceTypeText.gameObject.SetActive(true);
#if UNITY_ANDROID || UNITY_EDITOR
            if (TextManager.Instance.eLanguage == TextManager.LANGUAGE_TYPE.KOREAN)
            {
                priceTypeText.text = "￦";
                priceValue.text = shopGoodInfo.cBuyGoods.u4Count.ToString();
            }
            else
            {
                priceTypeText.text = "$";
                priceValue.text = shopGoodInfo.iOSPrice;
            }
#elif UNITY_IOS
            priceTypeText.text = "$";
            priceValue.text = shopGoodInfo.iOSPrice;
#endif          
        }
        else
        {
            priceIcon.gameObject.SetActive(true);
            priceIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(shopGoodInfo.cBuyGoods);
            priceTypeText.gameObject.SetActive(false);
        }                 
        
        PopupManager.Instance.AddPopup(gameObject, OnClickClose);     
    }
    
    public void OnClickClose()
    {
        PopupManager.Instance.RemovePopup(gameObject);
        itemModel.SetActive(true);
        gameObject.SetActive(false);
    }    
    
    public void OnClickBuyButton()
    {
        if(onClickBuy != null)
            onClickBuy();
    }
}
