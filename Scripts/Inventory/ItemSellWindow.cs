using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

// 장비 아닌 아이템 판매 팝업
public class ItemSellWindow : MonoBehaviour {

    public Text itemName;
    public Text itemDesc;
    public Image itemGrade;
    public Image itemIcon;
    public Text sellCountText;
    public Text sellPriceText;
    
    public InventoryPanel inventoryPanel;
    public ItemInfoWindow itemInfoWindow;
    public ItemSellResult itemSellResult;
    
    public GameObject SellConfirmPopup;
    public Text txtSellConfirmDesc;
    public Text txtSellPrice;

    private Item item;
    private UInt16 itemCount;
    public UInt16 GetItemCount
    {
        get
        {
            return itemCount;
        }
    }
    private UInt16 sellCount = 1;
    public UInt16 GetSellCount
    {
        get
        {
            return sellCount;
        }
    }
    private UInt32 sellPrice;
    private Goods sellGoods;
    private UInt16 tempSellCnt = 1;
    
    public void SetInfo(Item item)
    {
        this.item = item;        
		ushort iconID = item.cItemInfo.u2ID;

        switch(item.cItemInfo.ItemType)
        {
            case ItemInfo.ITEM_TYPE.CONSUMABLE:
            {
                ConsumableItem consumeItem = (ConsumableItem)item;
                itemName.text = string.Format("{0} x{1}", TextManager.Instance.GetText(consumeItem.cItemInfo.sName), consumeItem.u2Count);
                itemDesc.text = TextManager.Instance.GetText(consumeItem.cItemInfo.sDescription);
                itemCount = consumeItem.u2Count;
                sellPrice = consumeItem.GetItemInfo().cSellGoods.u4Count;
                sellGoods = consumeItem.GetItemInfo().cSellGoods;
            }
            break;
            
            case ItemInfo.ITEM_TYPE.MATERIAL:
            {
                MaterialItem materialItem = (MaterialItem)item;
				iconID = ((MaterialItemInfo)item.cItemInfo).u2IconID;
                itemName.text = string.Format("{0} x{1}", TextManager.Instance.GetText(materialItem.cItemInfo.sName), materialItem.u2Count);
                itemDesc.text = TextManager.Instance.GetText(materialItem.cItemInfo.sDescription);
                itemCount = materialItem.u2Count;
                sellPrice = materialItem.GetMaterialItemInfo().cSellGoods.u4Count;
                sellGoods = materialItem.GetMaterialItemInfo().cSellGoods;
            }
            break;
        }
        
		itemIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/Item_01." + iconID);
		itemGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(item.cItemInfo.u2ID));
        itemIcon.SetNativeSize();
        itemGrade.SetNativeSize();
        
        sellCount = 1;
        sellCountText.text = sellCount.ToString();
        sellPriceText.text = (sellCount * sellPrice).ToString();      
        
        PopupManager.Instance.AddPopup(gameObject, OnClickClose);  
    }
    
    public void Refresh()
    {
        SetInfo(item);
    }   
    
    public void OnClickPlus()
    {
        if(sellCount >= itemCount)
            return;
            
        sellCount++;
        sellCountText.text = sellCount.ToString();
        sellPriceText.text = (sellCount * sellPrice).ToString();
    }
    public void OnClickPlus10()
    {
        if(sellCount >= itemCount)
            return;
            
        sellCount += 10;
        if(sellCount >= itemCount)
            sellCount = itemCount;
        sellCountText.text = sellCount.ToString();
        sellPriceText.text = (sellCount * sellPrice).ToString();
    }
    public void OnClickMinus()
    {
        if(sellCount < 2)
            return;
            
        sellCount--;
        sellCountText.text = sellCount.ToString();
        sellPriceText.text = (sellCount * sellPrice).ToString();            
    }
    public void OnClickMinus10()
    {
        if(sellCount < 2)
            return;

        if(sellCount < 11)
            sellCount = 1;
        else
            sellCount -= 10;
        sellCountText.text = sellCount.ToString();
        sellPriceText.text = (sellCount * sellPrice).ToString();            
    }
    
    public void OnClickSell()
    {
        tempSellCnt = sellCount;
		bool isOverGoods = false;
		if( sellCount > 1)
		{
			Goods goods = new Goods(sellGoods.u1Type, sellGoods.u2ID, sellGoods.u4Count * sellCount);
			isOverGoods = Legion.Instance.CheckGoodsLimitExcessx(goods);
		}
		else
			isOverGoods = Legion.Instance.CheckGoodsLimitExcessx(sellGoods.u1Type);

		if(isOverGoods == true)
		{
			Legion.Instance.ShowGoodsOverMessage(sellGoods.u1Type);
			return;
		}
        OpenSellConfirmPopup();
    }

    public void OnClickSellAll()
    {
        bool isOverGoods = false;

        //sellCount = itemCount;
        tempSellCnt = itemCount;
		Goods goods = new Goods(sellGoods.u1Type, sellGoods.u2ID, sellGoods.u4Count * itemCount);
        
		isOverGoods = Legion.Instance.CheckGoodsLimitExcessx(goods);

		if(isOverGoods == true)
		{
			Legion.Instance.ShowGoodsOverMessage(sellGoods.u1Type);
			return;
		}
        OpenSellConfirmPopup();
    }
    
    public void OpenSellConfirmPopup()
    {
        SellConfirmPopup.SetActive(true);

        switch(item.cItemInfo.ItemType)
        {
            case ItemInfo.ITEM_TYPE.CONSUMABLE:
            {
                ConsumableItem consumeItem = (ConsumableItem)item;
                txtSellConfirmDesc.text = string.Format("{0} x{1}", TextManager.Instance.GetText(consumeItem.cItemInfo.sName), tempSellCnt);
                txtSellPrice.text = (sellGoods.u4Count * tempSellCnt).ToString();
            }
            break;
            
            case ItemInfo.ITEM_TYPE.MATERIAL:
            {
                MaterialItem materialItem = (MaterialItem)item;
                txtSellConfirmDesc.text = string.Format("{0} x{1}", TextManager.Instance.GetText(materialItem.cItemInfo.sName), tempSellCnt);
                txtSellPrice.text = (sellGoods.u4Count * tempSellCnt).ToString();
            }
            break;
        }
    }

    public void SellItem()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.InvenSellItem(item.u2SlotNum, tempSellCnt, AckItemSell); 
    }

    public void OnClickCloseSellConfirmPopup()
    {
        SellConfirmPopup.SetActive(false);
    }

    private void AckItemSell(Server.ERROR_ID err)
    {
       	PopupManager.Instance.CloseLoadingPopup();
		
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.ITEM_SELL, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{            
            Legion.Instance.AddGoods(sellGoods.u1Type, sellGoods.u4Count * tempSellCnt);
            
            string name = null;
            OnClickCloseSellConfirmPopup();
            switch(item.cItemInfo.ItemType)
            {
                case ItemInfo.ITEM_TYPE.CONSUMABLE:
                {
                    ConsumableItem consumeItem = (ConsumableItem)item;
                    consumeItem.u2Count -= tempSellCnt;
                    name = TextManager.Instance.GetText(consumeItem.cItemInfo.sName);
                    
                    if(consumeItem.u2Count == 0)
                    {
                        OnClickClose();
                        Legion.Instance.cInventory.RemoveItem(item.cItemInfo.u2ID);
                        itemInfoWindow.OnClickClose();
                    }
                    else
                    {
                        itemInfoWindow.Refresh();
                    }
                }
                break;
                
                case ItemInfo.ITEM_TYPE.MATERIAL:
                {
                    MaterialItem materialItem = (MaterialItem)item;
                    materialItem.u2Count -= tempSellCnt;
                    name = TextManager.Instance.GetText(materialItem.cItemInfo.sName);
                    
                    if(materialItem.u2Count == 0)
                    {
                        OnClickClose();
                        Legion.Instance.cInventory.RemoveItem(item.cItemInfo.u2ID);
                        itemInfoWindow.OnClickClose();
                    }
                    else
                    {
                        itemInfoWindow.Refresh();
                    }
                }
                break;
            }

            string result = String.Format("{0}({1})"+TextManager.Instance.GetText("popup_desc_sell_result"), name, tempSellCnt);            
            itemSellResult.gameObject.SetActive(true);
            itemSellResult.SetText(result);            

            Refresh();
            inventoryPanel.RefreshSlot();
        }
    }
    
    public void OnClickClose()
    {
        PopupManager.Instance.RemovePopup(gameObject);
        gameObject.SetActive(false);
    }
}
