using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

// 장비가 아닌 아이템 정보 팝업
public class ItemInfoWindow : MonoBehaviour {
    
    public Text itemName;
	public Text itemCount;
    public Text itemDesc;
    public Image itemIcon;
    public Image itemGrade;
    public ItemSellWindow itemSellWindow;
    
    private Item item;
    public void SetInfo(Item item)
    {
        this.item = item;
		ushort iconID = item.cItemInfo.u2ID;

        switch(item.cItemInfo.ItemType)
        {
            case ItemInfo.ITEM_TYPE.CONSUMABLE:
            {
                ConsumableItem consumeItem = (ConsumableItem)item;
                itemName.text = string.Format("{0}", TextManager.Instance.GetText(consumeItem.cItemInfo.sName));
				itemCount.text = string.Format("{0} : {1}",TextManager.Instance.GetText("popup_item_info_amount"), consumeItem.u2Count.ToString());
                itemDesc.text = TextManager.Instance.GetText(consumeItem.cItemInfo.sDescription);
            }
            break;
            
            case ItemInfo.ITEM_TYPE.MATERIAL:
            {
                MaterialItem materialItem = (MaterialItem)item;
				iconID = ((MaterialItemInfo)item.cItemInfo).u2IconID;
                itemName.text = string.Format("{0}", TextManager.Instance.GetText(materialItem.cItemInfo.sName));
				itemCount.text = string.Format("{0} : {1}",TextManager.Instance.GetText("popup_item_info_amount"),materialItem.u2Count.ToString());
                itemDesc.text = TextManager.Instance.GetText(materialItem.cItemInfo.sDescription);
            }
            break;
        }
        
		itemIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/Item_01." + iconID);
		itemGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(item.cItemInfo.u2ID));
        itemIcon.SetNativeSize();
        itemGrade.SetNativeSize();
        
        PopupManager.Instance.AddPopup(gameObject, OnClickClose);
    }
    
    public void Refresh()
    {
        SetInfo(item);
    }
    
    public void OnClickSell()
    {
        itemSellWindow.gameObject.SetActive(true);
        itemSellWindow.SetInfo(item);        
    }
    
    public void OnClickClose()
    {
        PopupManager.Instance.RemovePopup(gameObject);
        gameObject.SetActive(false);
    }
}
