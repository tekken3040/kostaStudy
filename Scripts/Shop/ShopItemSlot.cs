using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public struct ShopSlotData
{
	public int index;
	public ShopItem shopItem;
}

//장비를 제외한 아이템 슬롯
public class ShopItemSlot : MonoBehaviour, ISlot<ShopSlotData> {

	public delegate void OnClickSlot(int index);
	public OnClickSlot onClickSlot;
    
    public Button button;
	public Image itemIcon;
    public Image itemGrade;
    public Text itemCount;
    
    public Image priceIcon;
    public Text priceValue;
    public GameObject soldOut;
    
	public ShopSlotData slotData;

	public GameObject disObj;
	public DiscountUI disScript;

	bool bBlackShop = false;

    //정보 셋팅
	public void InitSlot(ShopSlotData slotData)
	{
		this.slotData = slotData;

		UInt16 divID = (UInt16)(slotData.shopItem.u2ItemID / 1000);
		ItemInfo.ITEM_ORDER type = ItemInfoMgr.Instance.GetItemType(slotData.shopItem.u2ItemID);

		switch(type)
		{
            case ItemInfo.ITEM_ORDER.MATERIAL:
			itemIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + ItemInfoMgr.Instance.GetMaterialItemInfo(slotData.shopItem.u2ItemID).u2IconID);
            itemGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(slotData.shopItem.u2ItemID));
			MaterialItemInfo materialInfo = ItemInfoMgr.Instance.GetMaterialItemInfo(slotData.shopItem.u2ItemID);
            itemCount.text = slotData.shopItem.u4Count.ToString();
			break;

		case ItemInfo.ITEM_ORDER.CONSUMABLE:
			itemIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + slotData.shopItem.u2ItemID);
            itemGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(slotData.shopItem.u2ItemID));
			ConsumableItemInfo consumableInfo = ItemInfoMgr.Instance.GetConsumableItemInfo(slotData.shopItem.u2ItemID);
            itemCount.text = slotData.shopItem.u4Count.ToString();
			break;
        }
        
        Goods good = new Goods(slotData.shopItem.u1PriceType, 0, slotData.shopItem.u4Price);
        priceIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(good);
		priceValue.text = string.Format("{0}", SetDisCountInfo());   
        priceIcon.SetNativeSize();
        itemGrade.SetNativeSize();
        itemIcon.SetNativeSize();

		CheckSoldOut();
	}

	public void SetIsBlackShop(bool bBlack){
		bBlackShop = bBlack;
	}
    
    // 품절 체크 및 처리
	public void CheckSoldOut()
	{
		if(slotData.shopItem.u1SoldOut == 1)
		{
           AtlasMgr.Instance.SetGrayScale(itemIcon);
           AtlasMgr.Instance.SetGrayScale(itemGrade);
           itemCount.color = Color.gray;
           
           priceIcon.gameObject.SetActive(false);
           priceValue.gameObject.SetActive(false);
           soldOut.SetActive(true);
           button.interactable = false;
		}
		else
		{
           AtlasMgr.Instance.SetDefaultShader(itemIcon);
           AtlasMgr.Instance.SetDefaultShader(itemGrade);
           itemCount.color = Color.white; 
            
           priceIcon.gameObject.SetActive(true);
           priceValue.gameObject.SetActive(true);
           soldOut.SetActive(false);
           button.interactable = true;
		}
	}

	uint SetDisCountInfo(){
		uint price = slotData.shopItem.u4Price;
		Byte disRate = 0;

		if (bBlackShop)
			return price;

		EventDisCountinfo disInfo = EventInfoMgr.Instance.GetDiscountEventinfo (DISCOUNT_ITEM.BUYSHOP);
		if (disInfo != null){
			price = (uint)(slotData.shopItem.u4Price * disInfo.discountRate);
			disRate = disInfo.u1DiscountRate;
		}

		if (price != slotData.shopItem.u4Price) {
			disObj.SetActive (true);
			disScript.SetData (slotData.shopItem.u4Price, disRate);
		} else {
			disObj.SetActive (false);
		}

		return price;
	}
	
	public void OnClickEvent()
	{
		if(onClickSlot != null)
		{
			if(slotData.shopItem.u1SoldOut == 1)
				return;

			onClickSlot(slotData.index);
		}
	}
}
