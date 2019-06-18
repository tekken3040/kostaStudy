using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Shop
{
	public Byte u1ShopType;
	public Byte u1RenewCount;
	public DateTime leftTime;
	public List<ShopItem> lstShopItem;
}

public class ShopItem
{
    public Byte u1Type;
	public UInt16 u2ItemID;
	public UInt32 u4Count;
	public Byte u1PriceType;
	public UInt32 u4Price;
	public Byte u1SoldOut;
	public ShopItemEquip cEquipInfo; //장비 아이템일 경우에만 사용
}

// 장비 아이템 정보
public class ShopItemEquip
{
	public string strItemName;
	public string strCreater;
	public Byte u1SmithingLevel;
	public UInt16 u2ModelID;
	public UInt16 u2Level;
	//public UInt32 u4Exp;
	public UInt64 u8Exp;
    public Byte u1Completeness;
	public UInt32[] u4ArrBaseStat;//UInt16[] u2ArrBaseStat;
	public Byte u1BuyPoint;
    public UInt16 u1VipPoint;
	public UInt16[] u2ArrStatsPoint;
	public Byte[] u1ArrSkillSlots;
	public UInt16[] u1ArrSkillPoint;
    public UInt16 u2TotalStatPoint;
    public UInt16 u2StatPointExp;
}
namespace ShopResult
{
    public delegate void OnResponse(string receipt = "", string Txid = "");
}
public class ShopInfoMgr : Singleton<ShopInfoMgr> {
    
	public Dictionary<UInt16, ShopInfo> dicShopData;
	public Dictionary<UInt16, ShopGoodInfo> dicShopGoodData;
	public Shop ShopInfoNormal;
	public Shop ShopInfoEquip;
	public Shop ShopInfoBlack;
	public int maxEquipRegist;
    public DateTime lastShopTime;
    public bool shopVisit = false;
    public bool normalCheck = false;
    public bool equipCheck = false;
    public bool blackCheck = false;
	public UInt16 buyedEventID = 0;
	
	public List<ShopItem> lstFixItem = new List<ShopItem>();

	private bool loadedInfo = false;
	public bool LoadedInfo
	{
		get { return loadedInfo; }
		set { loadedInfo = value; }
	}

	public void Init()
	{
		dicShopData = new Dictionary<UInt16, ShopInfo>();
		dicShopGoodData = new Dictionary<UInt16, ShopGoodInfo>();
		DataMgr.Instance.LoadTable(this.AddInfo, "Shop");
		DataMgr.Instance.LoadTable(this.AddInfoGoods, "ShopGoods");
	}

	public void AddInfo(string[] cols)
	{
		if (cols == null)
		{
			loadedInfo = true;
			return;
		}
		ShopInfo info = new ShopInfo();
		info.Set(cols);
		dicShopData.Add(info.u1Type, info);
	}

    public ShopGoodInfo getDeepCopyShopGoodInfo( UInt16 ID)
    {
        ShopGoodInfo info;
        
        if (dicShopGoodData.ContainsKey(ID))
        {
            info = new ShopGoodInfo();
            info.u2ID = dicShopGoodData[ID].u2ID;
            info.title = dicShopGoodData[ID].title;
            info.gachaCount = dicShopGoodData[ID].gachaCount;
            info.itemLevel = dicShopGoodData[ID].itemLevel;
            info.itemGrade = dicShopGoodData[ID].itemGrade;
            info.u1Type = dicShopGoodData[ID].u1Type;
            info.u2GroupIndex = dicShopGoodData[ID].u2GroupIndex;
            info.u1BuyOverlap = dicShopGoodData[ID].u1BuyOverlap;
            info.u2BuyRestriction = dicShopGoodData[ID].u2BuyRestriction;
            info.bBuyAll = dicShopGoodData[ID].bBuyAll;
            info.u1Discount = dicShopGoodData[ID].u1Discount;
            info.u1BonusRate = dicShopGoodData[ID].u1BonusRate;
            info.u1Repeat = dicShopGoodData[ID].u1Repeat;
            info.u2ProbabilityTableID = dicShopGoodData[ID].u2ProbabilityTableID;
            info.imagePath = dicShopGoodData[ID].imagePath;
            info.u1Grade = dicShopGoodData[ID].u1Grade;
            info.iOSPrice = dicShopGoodData[ID].iOSPrice;
            info.bSell = dicShopGoodData[ID].bSell;

            info.cBuyGoods = new Goods();
            info.cShopItem = new Goods();
            info.cVipGoods = new Goods();
            info.cBuyGoods.u1Type = dicShopGoodData[ID].cBuyGoods.u1Type;
            info.cBuyGoods.u2ID = dicShopGoodData[ID].cBuyGoods.u2ID;
            info.cBuyGoods.u4Count = dicShopGoodData[ID].cBuyGoods.u4Count;
            info.cShopItem.u1Type = dicShopGoodData[ID].cShopItem.u1Type;
            info.cShopItem.u2ID = dicShopGoodData[ID].cShopItem.u2ID;
            info.cShopItem.u4Count = dicShopGoodData[ID].cShopItem.u4Count;
            info.cVipGoods.u1Type = dicShopGoodData[ID].cVipGoods.u1Type;
            info.cVipGoods.u2ID = dicShopGoodData[ID].cVipGoods.u2ID;
            info.cVipGoods.u4Count = dicShopGoodData[ID].cVipGoods.u4Count;

            return info;

        }
        return null;

    }

	public void AddInfoGoods(string[] cols)
	{
		if (cols == null)
		{
			loadedInfo = true;
			return;
		}
		ShopGoodInfo info = new ShopGoodInfo();
		info.Set(cols);
		dicShopGoodData.Add(info.u2ID, info);
	}

	public ShopInfo GetInfo(UInt16 id)
	{
		ShopInfo ret = null;
		dicShopData.TryGetValue(id, out ret);
		return ret;
	}

	public void ClearShopList()
	{
		ShopInfoNormal = null;
		ShopInfoEquip = null;
		ShopInfoBlack = null;
	}

	public void SetShopItem(Shop shopData)
	{
		ShopInfoNormal = shopData;
	}

	public void SetShopEquip(Shop shopData)
	{
		ShopInfoEquip = shopData;
	}

	public void SetShopBlack(Shop shopData)
	{
		ShopInfoBlack = shopData;
        //if(Legion.Instance.u1BlackMarketOpen == 3)
        //    Legion.Instance.BlackMarketLeftTime = ShopInfoNormal.leftTime;
        //else
            Legion.Instance.BlackMarketLeftTime = ShopInfoBlack.leftTime;
	}

    public int getGachaDiscount(UInt16 shopID)
    {
		ShopGoodInfo info = ShopInfoMgr.Instance.dicShopGoodData[shopID];

		uint price = info.cBuyGoods.u4Count;
		Byte disRate = 0;
		if (info.u1Type < 5) {
			EventDisCountinfo disInfo = EventInfoMgr.Instance.GetDiscountEventinfo (DISCOUNT_ITEM.LOTTERY);
			if (disInfo != null) {
				price = (uint)(info.cBuyGoods.u4Count * disInfo.discountRate);
				disRate = disInfo.u1DiscountRate;
			}
		}

        int nDis = 0;
        if(dicShopGoodData[shopID].cBuyGoods.u1Type != (Byte)GoodsType.FRIENDSHIP_POINT)
			nDis = (int)(price * 
            ((100.0f - (float)(ShopInfoMgr.Instance.dicShopGoodData[shopID].u1Discount + 
                LegionInfoMgr.Instance.GetCurrentVIPInfo().u1VIPGachaPer)) / 100.0f));
        else
			nDis = (int)price;
        return nDis;
    }

    public Sprite GetGoodsSprites(Goods _goods)
    {
        Sprite _goodsSprite = new Sprite();

        switch(_goods.u1Type)
        {
            case (Byte)GoodsType.CASH:
                _goodsSprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_Cash");
                break;

            case (Byte)GoodsType.GOLD:
                _goodsSprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_Gold");
                break;

            case (Byte)GoodsType.FRIENDSHIP_POINT:
                _goodsSprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_friendship");
                break;
        }

        return _goodsSprite;
    }

    //인앱 결제 정보 초기화
    public void SettingInApp()
    {
        if(!UM_InAppPurchaseManager.Instance.IsInited)
        {
		    UM_InAppPurchaseManager.OnBillingConnectFinishedAction += OnBillingConnectFinishedAction;            
            UM_InAppPurchaseManager.Instance.Init();
            PopupManager.Instance.ShowLoadingPopup(1);
        }
    }
    //인앱 결제 정보 초기화 원스토어 16.12.27
    public void SettingInAppOneStore()
    {
        //if(!OneStoreIapManager.Instance.IsInited)
        //    OneStoreIapManager.Instance.init();
#if UNITY_ONESTORE
        if(!IapManager.Instance.IsInited)
            IapManager.Instance.Init();
#endif
    }

    // 결제 초기화 결과 처리
	private void OnBillingConnectFinishedAction (UM_BillingConnectionResult result) {
		UM_InAppPurchaseManager.OnBillingConnectFinishedAction -= OnBillingConnectFinishedAction;
		if(result.isSuccess) {
			DebugMgr.Log("Connected");
            
#if UNITY_ANDROID
        // 안드로이드의 경우 소모 처리되지 않은 품목이 남아 있을 경우 해당 품목은 재 구입이 불가능 하므로 소모처리를 해준다
        List<GooglePurchaseTemplate> inventory =  AndroidInAppPurchaseManager.Instance.Inventory.Purchases;
        foreach(GooglePurchaseTemplate purchase in inventory)
        {
            AndroidInAppPurchaseManager.Instance.Consume(purchase.SKU);          
        }
#endif
            
		} else {
			DebugMgr.Log("Failed to connect");
		}
        
        PopupManager.Instance.CloseLoadingPopup();
	}    
}
