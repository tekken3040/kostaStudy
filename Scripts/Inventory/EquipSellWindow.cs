using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

//장비 판메 팝업
public class EquipSellWindow : MonoBehaviour {
	[System.Serializable]
	public class WindowProperty
	{
		public Vector2 namePosition;
		public Vector2 itemPosition;
		public Vector2 msgPosition;
	}
	public WindowProperty sellWindow;
	public WindowProperty registerWindow;

    public Text title;
    public Text itemName;
    
	public RectTransform itemTransform;
    public Image itemGrade;
    public Image itemIcon;
    public Text sellInfo;
    
    public Text registCount;
    public Image priceIcon;
    public Text price;
    
    // 판매할 장비 정보 세팅
    public void SetSell(EquipmentItem equipmentItem)
    {   
        title.text = TextManager.Instance.GetText("popup_title_sell");
        
        itemName.text = string.Format("{0} {1}", equipmentItem.itemName, TextManager.Instance.GetText(equipmentItem.GetEquipmentInfo().sName));
		itemName.color = EquipmentItem.equipElementColors[equipmentItem.GetEquipmentInfo().u1Element];        
        sellInfo.text = TextManager.Instance.GetText("popup_desc_sell");
        
        ModelInfo modelInfo = ModelInfoMgr.Instance.GetInfo(equipmentItem.GetEquipmentInfo().u2ModelID);
        
        //모델 정보가 없는경우는 악세사리, 악세사리는 스프라이트로 보여줌
        if(modelInfo != null)
        {
            string imagePath = "Sprites/Item/" + modelInfo.sImagePath;         
            itemIcon.sprite = AtlasMgr.Instance.GetSprite(imagePath);
        }
        
        itemIcon.SetNativeSize();     
        
        int smithingLevel = equipmentItem.u1SmithingLevel;
        
        if(smithingLevel < 1)
            smithingLevel = 1;        
               
        ForgeInfo forgeInfo = ForgeInfoMgr.Instance.GetList()[smithingLevel - 1];        
		itemGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + forgeInfo.u2ID);
        itemGrade.SetNativeSize();  
        
        registCount.gameObject.SetActive(false); 
        priceIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(equipmentItem.GetEquipmentInfo().cSellGoods);
		price.text = ((UInt16)(equipmentItem.GetEquipmentInfo().cSellGoods.u4Count * (equipmentItem.u1Completeness * 0.5f) * equipmentItem.u1SmithingLevel * equipmentItem.cLevel.u2Level)).ToString();
        
		itemName.GetComponent<RectTransform>().anchoredPosition = sellWindow.namePosition;
		itemTransform.anchoredPosition = sellWindow.itemPosition;
		sellInfo.GetComponent<RectTransform>().anchoredPosition = sellWindow.msgPosition;

        PopupManager.Instance.AddPopup(gameObject, OnClickClose);
    }
    
    //장비 상점에 등록할 장비 정보 세팅
    public void SetRegist(EquipmentItem equipmentItem)
    {
        title.text = TextManager.Instance.GetText("popup_title_referral_sell");
        
        itemName.text = string.Format("{0} {1}", equipmentItem.itemName, TextManager.Instance.GetText(equipmentItem.GetEquipmentInfo().sName));
		itemName.color = EquipmentItem.equipElementColors[equipmentItem.GetEquipmentInfo().u1Element];
        sellInfo.text = TextManager.Instance.GetText("popup_desc_referral_sell");
        		            
                            
        ModelInfo modelInfo = ModelInfoMgr.Instance.GetInfo(equipmentItem.GetEquipmentInfo().u2ModelID);        
        if(modelInfo != null)
        {                            
            string imagePath = "Sprites/Item/" + modelInfo.sImagePath;         
            itemIcon.sprite = AtlasMgr.Instance.GetSprite(imagePath);
        }
        
        itemIcon.SetNativeSize();
        int smithingLevel = equipmentItem.u1SmithingLevel;
        
        if(smithingLevel < 1)
            smithingLevel = 1; 
        ForgeInfo forgeInfo = ForgeInfoMgr.Instance.GetList()[smithingLevel - 1];    
        itemGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + forgeInfo.u2ID);
        itemGrade.SetNativeSize();  
        registCount.gameObject.SetActive(true);
        registCount.text = string.Format("{0}/{1}", Legion.Instance.cInventory.lstInShop.Count, ShopInfoMgr.Instance.maxEquipRegist);
        //priceIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(equipmentItem.GetEquipmentInfo().cSellGoods);
        //price.text = (equipmentItem.GetEquipmentInfo().cSellGoods.u4Count * equipmentItem.u1SmithingLevel * equipmentItem.cLevel.u2Level).ToString();
        priceIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(equipmentItem.GetEquipmentInfo().cBuyGoods);
        UInt64 tempAccExp = 0;
        if(equipmentItem.cLevel.u2Level > 1)
        {
            tempAccExp += ClassInfoMgr.Instance.GetAccExp((UInt16)(equipmentItem.cLevel.u2Level-1));
            tempAccExp += equipmentItem.cLevel.u8Exp;
        }
        else
        {
            tempAccExp += equipmentItem.cLevel.u8Exp;
        }
        UInt32 tempPrice = (UInt32)((equipmentItem.GetEquipmentInfo().cBuyGoods.u4Count * ((float)equipmentItem.u1Completeness / (float)(1 + equipmentItem.u1Completeness))
            * ForgeInfoMgr.Instance.GetInfo(ForgeInfoMgr.Instance.GetIDs()[equipmentItem.u1SmithingLevel-1]).u2EquipShopCostFacter)
            + (tempAccExp * 0.08f) * ClassInfoMgr.Instance.GetCostFacter(equipmentItem.cLevel.u2Level));
        price.text = tempPrice.ToString();
        
		itemName.GetComponent<RectTransform>().anchoredPosition = registerWindow.namePosition;
		itemTransform.anchoredPosition = registerWindow.itemPosition;
		sellInfo.GetComponent<RectTransform>().anchoredPosition = registerWindow.msgPosition;

        PopupManager.Instance.AddPopup(gameObject, OnClickClose);
    }
    
    public void OnClickClose()
    {
        PopupManager.Instance.RemovePopup(gameObject);
        gameObject.SetActive(false);
    }
}
