using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using IgaworksUnityAOS;

public class SmallPayShop : MonoBehaviour {

    public ScrollRect scrollRect;
    public RectTransform content;
    public ShopInfoWindow shopInfoWindow;
    
    private bool init = false;
    private GameObject shopSlotItem;    
	private EventDungeonShopInfo shopInfo;
	private ShopGoodInfo buyInfo;

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
#if UNITY_ONESTORE
            ShopInfoMgr.Instance.SettingInAppOneStore();
            // 인앱 초기화
#elif UNITY_ANDROID && !UNITY_ONESTORE
            ShopInfoMgr.Instance.SettingInApp();
            
#endif
    }
    
    public void SetTab()
    {
        if(init)
            return;
            
        if(shopSlotItem == null)
            shopSlotItem = AssetMgr.Instance.AssetLoad("Prefabs/UI/Shop/ShopSlot.prefab", typeof(GameObject)) as GameObject;     

		shopInfo = EventInfoMgr.Instance.CheckOpenSmallPayShop();

		for (int i = 0; i < shopInfo.au2ShopID.Count; i++) {
			UInt16 shopId = shopInfo.au2ShopID [i];
			ShopGoodInfo info = ShopInfoMgr.Instance.dicShopGoodData [shopId];
			GameObject slot = Instantiate(shopSlotItem) as GameObject;
			slot.GetComponent<RectTransform>().SetParent(content);
			slot.transform.localScale = Vector3.one;
			slot.transform.localPosition = Vector3.zero;

			ShopSlot shopSlot = slot.GetComponent<ShopSlot>();
			shopSlot.InitSlot(info.u2ID);
			shopSlot.onClickSlot = OnClickSlot;

			m_SlotList.Add(shopSlot);
		}
         
        init = true;
    }
    
    private UInt16 selectedID;
    public void OnClickSlot(UInt16 id)
    {
        selectedID = id;

		buyInfo = ShopInfoMgr.Instance.dicShopGoodData[id];

        shopInfoWindow.gameObject.SetActive(true);
        shopInfoWindow.onClickBuy = OnClickBuy;
		shopInfoWindow.SetInfo(buyInfo, TextManager.Instance.GetText(buyInfo.title));
		if(buyInfo.cBuyGoods.u1Type == (Byte)GoodsType.PURCHASE)
			AccountManager.Instance.FBEventLogWithParam (Facebook.Unity.AppEventName.AddedToCart, buyInfo.iOSPrice);
    }
    
    public void OnClickBuy()
    {
		// 한계수량이 넘는지 체크
		if(Legion.Instance.CheckGoodsLimitExcessx(ShopInfoMgr.Instance.dicShopGoodData[selectedID].cShopItem.u1Type) == true)
		{
			Legion.Instance.ShowGoodsOverMessage(ShopInfoMgr.Instance.dicShopGoodData[selectedID].cShopItem.u1Type);
			shopInfoWindow.gameObject.SetActive(false);
			return;
		}

		//if (ShopInfoMgr.Instance.dicShopGoodData [selectedID].cShopItem.u1Type > 9) {
			if (!Legion.Instance.CheckEmptyInven ())
				return;
		//}
			
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
			ShopGoodInfo shopGoodInfo = ShopInfoMgr.Instance.getDeepCopyShopGoodInfo(selectedID);

			Legion.Instance.SubGoods(shopGoodInfo.cBuyGoods);
			LegionInfoMgr.Instance.SetAddVipPoint(shopGoodInfo);

            // 2017. 5. 19 jy
            // 재화 조건 식에만 되어 있어서 밖으로 빼냄 구매 상품 추가
            Goods reward = new Goods(ShopInfoMgr.Instance.lstFixItem[0].u1Type, ShopInfoMgr.Instance.lstFixItem[0].u2ItemID, ShopInfoMgr.Instance.lstFixItem[0].u4Count);
            Legion.Instance.AddGoods(reward);
            
            if (buyInfo.cShopItem.u1Type == 10)//장비
			{
				GameObject resultObject = AssetMgr.Instance.AssetLoad("Prefabs/UI/Shop/GachaResult.prefab", typeof(GameObject)) as GameObject;
				GachaResult gachaResult = Instantiate(resultObject).GetComponent<GachaResult>();
				gachaResult.GetComponent<RectTransform>().SetParent(transform.parent.parent);
				gachaResult.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, 0f, -100f);
				gachaResult.GetComponent<RectTransform>().sizeDelta = Vector3.zero;
				gachaResult.transform.localScale = Vector3.one;
				gachaResult.transform.name = "GachaResult";
				gachaResult.SetInfo(ShopInfoMgr.Instance.lstFixItem [0]);
			}
			else if(buyInfo.cShopItem.u1Type == 11)//재료
			{
				GameObject resultObject = AssetMgr.Instance.AssetLoad("Prefabs/UI/Shop/GachaResult.prefab", typeof(GameObject)) as GameObject;
				GachaResult gachaResult = Instantiate(resultObject).GetComponent<GachaResult>();
				gachaResult.GetComponent<RectTransform>().SetParent(transform.parent.parent);
				gachaResult.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, 0f, -100f);
				gachaResult.GetComponent<RectTransform>().sizeDelta = Vector3.zero;
				gachaResult.transform.localScale = Vector3.one;
				gachaResult.transform.name = "GachaResult";
				gachaResult.SetInfo(ShopInfoMgr.Instance.lstFixItem [0]);
			}
			else if(buyInfo.cShopItem.u1Type == 12)//소모품
			{
				GameObject resultObject = AssetMgr.Instance.AssetLoad("Prefabs/UI/Shop/GachaResult.prefab", typeof(GameObject)) as GameObject;
				GachaResult gachaResult = Instantiate(resultObject).GetComponent<GachaResult>();
				gachaResult.GetComponent<RectTransform>().SetParent(transform.parent.parent);
				gachaResult.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, 0f, -100f);
				gachaResult.GetComponent<RectTransform>().sizeDelta = Vector3.zero;
				gachaResult.transform.localScale = Vector3.one;
				gachaResult.transform.name = "GachaResult";
				gachaResult.SetInfo(ShopInfoMgr.Instance.lstFixItem [0]);
			}
			else //기타 재화
            {
                //Goods reward = new Goods (ShopInfoMgr.Instance.lstFixItem [0].u1Type, ShopInfoMgr.Instance.lstFixItem [0].u2ItemID, ShopInfoMgr.Instance.lstFixItem [0].u4Count);

                string title = TextManager.Instance.GetText("popup_title_shop_buy_common_result");

				PopupManager.Instance.ShowOKPopup(title, reward.u4Count+" "+Legion.Instance.GetGoodsName(reward)+TextManager.Instance.GetText("popup_desc_shop_buy_common_result"), null);
            }

            ReflashItemList();
        }
    }

    void ReflashItemList()
    {
        foreach (ShopSlot slot in m_SlotList)
        {
            slot.refleshSlot();
        }
    }
}
