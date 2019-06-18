using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using IgaworksUnityAOS;

public class ShopTab : MonoBehaviour {

    public enum  ShopType
    {   
        GOLD = 1,    
        CASH = 2,
        KEY = 3,                
        EQUIP = 10
    }

    public ShopType shopType;
    public ScrollRect scrollRect;
    public RectTransform content;
    public ShopInfoWindow shopInfoWindow;
    
    private bool init = false;
    private GameObject shopSlotItem;    
    private GachaEquipResult gachaResult;

    private List<ShopSlot> m_SlotList = new List<ShopSlot>();
    void OnEnable()
    {
        scrollRect.StopMovement();
        scrollRect.horizontalNormalizedPosition = 0f;
    }
    
    public void Init()
    {
        SetTab();     
        ReflashItemList();
        if(shopType == ShopType.CASH)
        {
#if UNITY_ONESTORE
            ShopInfoMgr.Instance.SettingInAppOneStore();
            // 인앱 초기화
#else
            ShopInfoMgr.Instance.SettingInApp();
            
#endif
        }
    }
    
    public void SetTab()
    {
        switch(shopType)
        {
            case ShopType.EQUIP:
                break;

            case ShopType.GOLD:
                break;

            case ShopType.KEY:
                break;

            case ShopType.CASH:
                break;
        }
        if(init)
            return;
            
        if(shopSlotItem == null)
            shopSlotItem = AssetMgr.Instance.AssetLoad("Prefabs/UI/Shop/ShopSlot.prefab", typeof(GameObject)) as GameObject;

        // 판매 아이템 목록 생성

        int i = 0, gachaIdx = 0;
        foreach (ShopGoodInfo info in ShopInfoMgr.Instance.dicShopGoodData.Values)
        {
			if(info.cShopItem.u1Type == (Byte)(shopType) && info.bSell)
            {
                GameObject slot = Instantiate(shopSlotItem) as GameObject;
                slot.GetComponent<RectTransform>().SetParent(content);
                slot.transform.localScale = Vector3.one;
                slot.transform.localPosition = Vector3.zero;
                
                ShopSlot shopSlot = slot.GetComponent<ShopSlot>();
                shopSlot.InitSlot(info.u2ID);
                shopSlot.onClickSlot = OnClickSlot;

                m_SlotList.Add(shopSlot);
				if(info.u2ID == 9010)
                {                    
					slot.AddComponent<TutorialButton>().id = "Gacha_primium_equip";
                    gachaIdx = i;
                }
                ++i;
            }
        }

        if (Legion.Instance.cTutorial.au1Step[8] == 1)
        {
            float normalized = (float)gachaIdx / (float)(content.childCount - 1);
            scrollRect.horizontalNormalizedPosition = normalized;
        }
         
        init = true;
    }
    
    private UInt16 selectedID;
    public void OnClickSlot(UInt16 id)
    {
        selectedID = id;
        
        ShopGoodInfo shopGoodInfo = ShopInfoMgr.Instance.dicShopGoodData[id];
        // 장비가 아니면 정보창 띄워줌
        if (shopType != ShopType.EQUIP)
        {
            string title = "";
            switch (shopType)
            {
                case ShopType.GOLD: title = TextManager.Instance.GetText("popup_title_shop_gold"); break;
                case ShopType.CASH: title = TextManager.Instance.GetText("popup_title_shop_cash"); break;
                case ShopType.KEY: title = TextManager.Instance.GetText("popup_title_shop_key"); break;
            }
            shopInfoWindow.gameObject.SetActive(true);
            shopInfoWindow.onClickBuy = OnClickBuy;
            shopInfoWindow.SetInfo(shopGoodInfo, title);
			if(shopGoodInfo.cBuyGoods.u1Type == (Byte)GoodsType.PURCHASE)
				AccountManager.Instance.FBEventLogWithParam (Facebook.Unity.AppEventName.AddedToCart, shopGoodInfo.iOSPrice);
        }
        else
        {
            //슬롯이 없으면 뽑기 불가능
			if(!Legion.Instance.CheckEmptyInven())
            {
                return;
            }
            
            // 튜토리얼 시 재화 체크 안함
			if(!Legion.Instance.cTutorial.bIng)
            {            
                // 재화 부족시 불가능
                if(!Legion.Instance.CheckEnoughGoods(shopGoodInfo.cBuyGoods))
                {
                    PopupManager.Instance.ShowChargePopup(shopGoodInfo.cBuyGoods.u1Type);
                    return;
                }
            }

            // 구입 요청
			if (Legion.Instance.cTutorial.au1Step [8] == 1) {
                PopupManager.Instance.ShowLoadingPopup(1);
				Server.ServerMgr.Instance.ShopFixShop (id, 0, 8, Server.ConstDef.LastTutorialStep, "", "", AckFixShop);
			}
            else
            {
                StringBuilder tempString = new StringBuilder();
                tempString.Append(shopGoodInfo.cBuyGoods.GetGoodsString()).Append(TextManager.Instance.GetText("popup_buy_gacha_ask"));
                tempString.Replace("{0}", TextManager.Instance.GetText(shopGoodInfo.title));

                PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("title_notice"), tempString.ToString(), RrequestGacha, null);
			}
        }
    }

    private void RrequestGacha(object[] param)
    {
        // 튜토리얼중이 아니라면 팝업을 띄워 확인 받는다
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.ShopFixShop(selectedID, 0, 255, 0, "", "", AckFixShop);
    }

    public void OnClickBuy()
    {
		// 한계수량이 넘는지 체크
		if(Legion.Instance.CheckGoodsLimitExcessx(ShopInfoMgr.Instance.dicShopGoodData[selectedID]))
		{
			Legion.Instance.ShowGoodsOverMessage(ShopInfoMgr.Instance.dicShopGoodData[selectedID].cShopItem.u1Type);
			shopInfoWindow.gameObject.SetActive(false);
			return;
		}

        //캐쉬 상점일 경우
        if(shopType == ShopType.CASH)
        {
#if UNITY_EDITOR
            RequestBuy();
            
#elif UNITY_ONESTORE
            if(IapManager.Instance.IsInited)
            {
                PopupManager.Instance.ShowLoadingPopup(1);
                ShopResult.OnResponse CallBack = new ShopResult.OnResponse(RequestBuy);
                DebugMgr.LogError("원스토어 결제 호출");
                IapManager.Instance.RequestPaymenet(ShopInfoMgr.Instance.dicShopGoodData[selectedID].OnestoreCord, 
                    //ShopInfoMgr.Instance.dicShopGoodData[selectedID].title, 
                    TextManager.Instance.GetText(ShopInfoMgr.Instance.dicShopGoodData[selectedID].title),
                    ShopInfoMgr.Instance.dicShopGoodData[selectedID].u2ID.ToString(), 
                    ShopInfoMgr.Instance.dicShopGoodData[selectedID].title, CallBack);
            }
#else
            if(UM_InAppPurchaseManager.Instance.IsInited)
            {
                PopupManager.Instance.ShowLoadingPopup(1);
                UM_InAppPurchaseManager.OnPurchaseFlowFinishedAction += OnPurchaseFlowFinishedAction;
                UM_InAppPurchaseManager.Instance.Purchase(selectedID.ToString());
				AccountManager.Instance.FBEventLogWithParam (Facebook.Unity.AppEventName.InitiatedCheckout, ShopInfoMgr.Instance.dicShopGoodData[selectedID].iOSPrice);
            }
            else
            {
                DebugMgr.LogError("InAppPurchase Init Failed");
            }
#endif
        }
        else
        {
			if(!Legion.Instance.CheckEnoughGoods(ShopInfoMgr.Instance.dicShopGoodData[selectedID].cBuyGoods))
            {
				PopupManager.Instance.ShowChargePopup(ShopInfoMgr.Instance.dicShopGoodData[selectedID].cBuyGoods.u1Type);
                return;
            }

            RequestBuy();               
        }
    }
    
    // 서버로 구입 요청 처리
    private void RequestBuy(string receipt = "", string Txid = "")
    {
//		if (Legion.Instance.cTutorial.au1Step[8] == 1) {
//            PopupManager.Instance.ShowLoadingPopup(1);
//			Server.ServerMgr.Instance.ShopFixShop (selectedID, 0, 8, Server.ConstDef.LastTutorialStep, receipt, AckFixShop);
//        } else {
            DebugMgr.LogError("레시피 스트링 : " + receipt);
            DebugMgr.LogError("텍스 아이디 스트링 : " + Txid);
            DebugMgr.LogError("서버에 결제 요청");
            PopupManager.Instance.ShowLoadingPopup(1);
            Server.ServerMgr.Instance.ShopFixShop (selectedID, 0, 255, 0, receipt, Txid, AckFixShop);
//        }
    }
    
    // OS 결제 결과 처리
	private void OnPurchaseFlowFinishedAction (UM_PurchaseResult result) {
        
        PopupManager.Instance.CloseLoadingPopup();

		AccountManager.Instance.FBLogPurchase (ShopInfoMgr.Instance.dicShopGoodData[selectedID].iOSPrice);
        
		UM_InAppPurchaseManager.OnPurchaseFlowFinishedAction -= OnPurchaseFlowFinishedAction;
		if(result.isSuccess) {
#if UNITY_IOS          
            DebugMgr.Log(result.IOS_PurchaseInfo.Receipt);              
			RequestBuy(result.IOS_PurchaseInfo.Receipt);

			string currency = result.product.CurrencyCode;

			IgaworksCommercePluginIOS.IgaworksCommercePurchase(
			result.IOS_PurchaseInfo.Receipt,
			result.product.IOSId,
			result.product.IOSTemplate.DisplayName,
			result.product.ActualPriceValue,
			1,
			currency,
			"");

			AdBrixPluginIOS.Retention ("Purchase");
#elif UNITY_ANDROID
            DebugMgr.Log(result.Google_PurchaseInfo.token);
            RequestBuy(result.Google_PurchaseInfo.token);

			List<IgaworksUnityPluginAOS.IgawCommerceItemModel> items = new List<IgaworksUnityPluginAOS.IgawCommerceItemModel>();
			IgaworksUnityPluginAOS.IgawCommerceItemModel item = new IgaworksUnityPluginAOS.IgawCommerceItemModel (
			result.Google_PurchaseInfo.orderId,
			result.product.AndroidId,
			result.Google_PurchaseInfo.packageName,
			result.product.ActualPriceValue,
			1,
			result.product.CurrencyCode,
			"");

			items.Add (item);

			IgaworksUnityPluginAOS.Adbrix.purchase (items);
			IgaworksUnityPluginAOS.Adbrix.retention ("Purchase");
#endif            
		} else  {            
            DebugMgr.Log(result.ResponceCode);            
			//PopupManager.Instance.ShowOKPopup("구매 실패", "구매를 실패하였습니다", null);
		}
	}    
    
    // 구입 결과 처리
    private void AckFixShop(Server.ERROR_ID err)
    {
        DebugMgr.Log("AckFixShop " + err);

		PopupManager.Instance.CloseLoadingPopup();
		
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup("", TextManager.Instance.GetError(Server.MSGs.SHOP_FIXSHOP, err), Server.ServerMgr.Instance.CallClear);
		}
		else
        {
            shopInfoWindow.gameObject.SetActive(false);

            // 뽑기가 아닐 경우
            if (shopType != ShopType.EQUIP)
            {
                ShopGoodInfo shopGoodInfo = ShopInfoMgr.Instance.getDeepCopyShopGoodInfo(selectedID);
                //LegionInfoMgr.Instance.SetAddVipPoint(shopGoodInfo);
                int addValue = LegionInfoMgr.Instance.GetAddVipValue(shopGoodInfo) + shopGoodInfo.GetBuyBonus();
                Legion.Instance.AddGoods(shopGoodInfo.cShopItem);

				if ((GoodsType)shopGoodInfo.cBuyGoods.u1Type == GoodsType.PURCHASE)
					Legion.Instance.bCheckCash = true;

                Legion.Instance.AddGoods(new Goods(shopGoodInfo.cShopItem.u1Type, shopGoodInfo.cShopItem.u2ID, (uint)addValue), false);

                uint price = shopGoodInfo.cBuyGoods.u4Count;

				if (shopGoodInfo.u1Type < 5) {
					EventDisCountinfo disInfo = EventInfoMgr.Instance.GetDiscountEventinfo (DISCOUNT_ITEM.LOTTERY);
					if (disInfo != null) price = (uint)(shopGoodInfo.cBuyGoods.u4Count * disInfo.discountRate);
				} else if (shopGoodInfo.u1Type == 5) {
					EventDisCountinfo disInfo = EventInfoMgr.Instance.GetDiscountEventinfo (DISCOUNT_ITEM.BUYSHOP);
					if (disInfo != null) price = (uint)(shopGoodInfo.cBuyGoods.u4Count * disInfo.discountRate);
				} else if (shopGoodInfo.u1Type == 6) {
					EventDisCountinfo disInfo = EventInfoMgr.Instance.GetDiscountEventinfo (DISCOUNT_ITEM.BUYGOLD);
					if (disInfo != null) price = (uint)(shopGoodInfo.cBuyGoods.u4Count * disInfo.discountRate);
				} else if (shopGoodInfo.u1Type == 7) {
					EventDisCountinfo disInfo = EventInfoMgr.Instance.GetDiscountEventinfo (DISCOUNT_ITEM.BUYKEY);
					if (disInfo != null) price = (uint)(shopGoodInfo.cBuyGoods.u4Count * disInfo.discountRate);
				}

				Legion.Instance.SubGoods(new Goods(shopGoodInfo.cBuyGoods.u1Type, shopGoodInfo.cBuyGoods.u2ID, price));
                LegionInfoMgr.Instance.SetAddVipPoint(shopGoodInfo);
                ReflashItemList();

                string title = TextManager.Instance.GetText("popup_title_shop_buy_common_result");
                string itemName;
                if (addValue > 0)
                {
                    itemName = TextManager.Instance.GetText(ShopInfoMgr.Instance.dicShopGoodData[selectedID].title) +
                                      "(+" + addValue + ")" +
                                      TextManager.Instance.GetText("popup_desc_shop_buy_common_result");
                }
                else
                {
                    itemName = TextManager.Instance.GetText(ShopInfoMgr.Instance.dicShopGoodData[selectedID].title) + 
                        TextManager.Instance.GetText("popup_desc_shop_buy_common_result");
                }
                
                DebugMgr.LogError("가격 : " + shopGoodInfo.cBuyGoods.u4Count);
                DebugMgr.LogError("수량 : " + 1);
                DebugMgr.LogError("Item Code : " + shopGoodInfo.u2ID.ToString());

                PopupManager.Instance.ShowOKPopup(title, itemName, null);
            }
            else
            {
                // 뽑기 결과창 표시

                if(gachaResult == null)
                {
                    GameObject resultObject = AssetMgr.Instance.AssetLoad("Prefabs/UI/Shop/GachaEquipResult.prefab", typeof(GameObject)) as GameObject;
                    gachaResult = Instantiate(resultObject).GetComponent<GachaEquipResult>();
                    gachaResult.GetComponent<RectTransform>().SetParent(transform.parent.parent);
                    gachaResult.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, 0f, -100f);
                    gachaResult.GetComponent<RectTransform>().sizeDelta = Vector3.zero;
                    gachaResult.transform.localScale = Vector3.one;
                    gachaResult.transform.name = "GachaEquipResult";
                }
                
                gachaResult.gameObject.SetActive(true);
                gachaResult.SetInfo(selectedID);
#if UNITY_EDITOR
				DebugMgr.LogError(Legion.Instance.cTutorial.au1Step[8]);
#endif
                if(Legion.Instance.cTutorial.au1Step[8] != 2)
                {
                    ShopGoodInfo shopGoodInfo = ShopInfoMgr.Instance.dicShopGoodData[selectedID];
                    Legion.Instance.SubGoods(shopGoodInfo.cBuyGoods);
                }
                else
                {
                    Legion.Instance.bCheckCash = false;
                }
            }
        }
    }

    public GachaEquipResult GetGachaResult()
    {
        return gachaResult;
    }

    void ReflashItemList()
    {
        foreach (ShopSlot slot in m_SlotList)
        {
            slot.refleshSlot();
        }
    }
}
