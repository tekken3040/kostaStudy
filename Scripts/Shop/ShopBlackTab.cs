using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ShopBlackTab : MonoBehaviour {

	private const int SHOP_TYPE = 3;

	public Text refreshText;
	public Text refreshtimeText;

	public RectTransform content;
    public ShopInfoWindow shopInfoWindow;
    public RefreshWindow refreshWindow;
	
	private int selectIndex = -1;
	private DateTime closeTime;
	private GameObject shopItemSlot;
	private List<ShopItemSlot> lstShopSlot = new List<ShopItemSlot>();
	private bool closed = false;
	private bool init = false;
	private bool timeChecking = false;
	
	public void Init()
	{
		if(init)
			return;
            
        SetShopList();
	}

	public void OnEnable()
	{
		if((BLACK_SHOP_STATE)Legion.Instance.u1BlackMarketOpen == BLACK_SHOP_STATE.VIP_OPEN)
		{
			TimeCount();
			if(!timeChecking)
				StartCoroutine(CheckRefreshTime());
		}
		else
		{
			// 남은시간 갱신
			Legion.Instance.BlackMarketLeftTime = ShopInfoMgr.Instance.ShopInfoBlack.leftTime;
			closeTime = ShopInfoMgr.Instance.ShopInfoBlack.leftTime;
			if(!timeChecking)
				StartCoroutine(CheckCloseTime());

		}
		//if(Legion.Instance.u1BlackMarketOpen == 3)
		//{
        //    resetTime = ShopInfoMgr.Instance.ShopInfoBlack.leftTime;
		//	refreshText.text = TextManager.Instance.GetText("mark_shop_update_time");
        //    TimeCount();
        //    if(!timeChecking && init)
		//	    StartCoroutine(CheckRefreshTime());
		//}
		//else
		//{
	    //    closeTime = ShopInfoMgr.Instance.ShopInfoBlack.leftTime;
		//	refreshText.text = TextManager.Instance.GetText("mark_time_black");
		//	if(!timeChecking && init)
		//		StartCoroutine(CheckCloseTime()); // 시간 갱신 시작
		//}   
        //
        //ShopInfoMgr.Instance.blackCheck = false;      
		//StartCoroutine(CheckRefreshTime());
	}

	public void OnDisable()
	{
		timeChecking = false; // 시간 갱신 멈춤
	}

	//아이템 목록 생성
	public void SetShopList()
	{
		if(shopItemSlot == null)
			shopItemSlot = AssetMgr.Instance.AssetLoad("Prefabs/UI/Shop/Shop_ItemSlot.prefab", typeof(GameObject)) as GameObject;
		
		Shop shopData = ShopInfoMgr.Instance.ShopInfoBlack;

		if(shopData == null)
			return;
		
		int index = 0;
		
		for(int i=0; i<shopData.lstShopItem.Count; i++)
		{		
			ShopItemSlot shopSlot = null;
			
			ShopSlotData shopSlotData = new ShopSlotData();
			shopSlotData.index = index;
			shopSlotData.shopItem = shopData.lstShopItem[i];
			
			if(lstShopSlot.Count > index)
			{
				shopSlot = lstShopSlot[index];
			}
			else
			{
				GameObject item = Instantiate(shopItemSlot) as GameObject;
				RectTransform itemRect = item.GetComponent<RectTransform>();
				itemRect.SetParent(content);
				itemRect.localPosition = Vector3.zero;
				itemRect.localScale = Vector3.one;
				shopSlot = item.GetComponent<ShopItemSlot>();
				
				lstShopSlot.Add(shopSlot);
			}

			shopSlot.SetIsBlackShop (true);
			shopSlot.InitSlot(shopSlotData);
			shopSlot.onClickSlot = OnClickItemSlot;			
			
			index++;
		}
		
		int lstCount = lstShopSlot.Count;
		
		for(int i=0; i<lstCount; i++)
		{
			if(i >= index)
				lstShopSlot[i].gameObject.SetActive(false);
			else
				lstShopSlot[i].gameObject.SetActive(true);
		} 

		if((BLACK_SHOP_STATE)Legion.Instance.u1BlackMarketOpen == BLACK_SHOP_STATE.VIP_OPEN)
		{
            TimeCount();
            refreshText.text = TextManager.Instance.GetText("mark_shop_update_time");
			//resetTime = ShopInfoMgr.Instance.ShopInfoBlack.leftTime;
			if(!timeChecking)
				StartCoroutine(CheckRefreshTime());
		}
		else
		{
			refreshText.text = TextManager.Instance.GetText("mark_time_black");
			closeTime = ShopInfoMgr.Instance.ShopInfoBlack.leftTime;
        	StartCoroutine(CheckCloseTime());
		}
        //if(Legion.Instance.u1BlackMarketOpen == 3)
		//{
        //    resetTime = ShopInfoMgr.Instance.ShopInfoBlack.leftTime;
		//	refreshText.text = TextManager.Instance.GetText("mark_shop_update_time");
        //    TimeCount();
        //    if(!timeChecking && init)
		//	    StartCoroutine(CheckRefreshTime());
		//}
		//else
		//{
	    //    closeTime = ShopInfoMgr.Instance.ShopInfoBlack.leftTime;
		//	refreshText.text = TextManager.Instance.GetText("mark_time_black");
		//	if(!timeChecking && init)
		//		StartCoroutine(CheckCloseTime()); // 시간 갱신 시작
		//}   
        //
        ShopInfoMgr.Instance.blackCheck = false;    

		init = true;
	}

	//폐쇠시간 체크
	private IEnumerator CheckCloseTime()
	{
		timeChecking = true;

		while(true)
		{
			TimeSpan timeSpan = closeTime - Legion.Instance.ServerTime;
			if(timeSpan.Ticks > 0)
			{
				refreshtimeText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			}
			else 
			{
				closed = true;
				timeChecking = false;
				//Legion.Instance.u1BlackMarketOpen = 0;
				Legion.Instance.u1BlackMarketOpen = (Byte)BLACK_SHOP_STATE.CLOSE;
				yield break;
			}
			
			yield return new WaitForSeconds(1f);
		}
	}
	//갱신 시간 체크
    private TimeSpan resetTime;
	private IEnumerator CheckRefreshTime()
	{
        timeChecking = true;
        int length = ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u1ArrResetTime.Length;

        while (true)
		{
			if(resetTime.Ticks > 0)
			{
				refreshtimeText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", resetTime.Hours, resetTime.Minutes, resetTime.Seconds);
			}
			else 
			{
				PopupManager.Instance.ShowLoadingPopup(1);
				Server.ServerMgr.Instance.RequsetShopList(SHOP_TYPE, 1, AckShopAutoRefresh);
				timeChecking = false;
				yield break;
			}
			
			yield return new WaitForSeconds(1f);
            resetTime = resetTime.Subtract(TimeSpan.FromSeconds(1));
        }
	}
    //시간에 맞춰 갱신
	private void AckShopAutoRefresh(Server.ERROR_ID err)
	{
		DebugMgr.Log("AckShopAuto " + err);

		PopupManager.Instance.CloseLoadingPopup();
		
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.SHOP_LIST, err), Server.ServerMgr.Instance.CallClear);
		}
		else if(err == Server.ERROR_ID.NONE)
		{
			// 상점 리스트 갱신(품절 처리)
			SetShopList();  

			// 갱신 비용 처리
			string priceType = Legion.Instance.GetConsumeString(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u1Type);
			UInt32 price = (UInt32)(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u4Count
			                        + (ShopInfoMgr.Instance.ShopInfoBlack.u1RenewCount * ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2AddValue));
			
            // 갱신 비용 최대치 초과시 최대치로
			if(price > ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2MaxValue)
				price = ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2MaxValue;
			
			TimeCount();

			if(!timeChecking)
				StartCoroutine(CheckRefreshTime());
                
            refreshWindow.OnClickClose();
		}
	}

    public void TimeCount()
	{
        bool isFindTime = false;
		int length = ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u1ArrResetTime.Length;
        DateTime serverTime = Legion.Instance.ServerTime;
        for (int i = 0; i < length; i++)
        {
            // 현재 시간이 갱신시간보다 크다면 다음 갱신비교 한다
            if (serverTime.Hour >= ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u1ArrResetTime[i])
            {
                continue;
            }
            else
            {
                resetTime = new DateTime().AddHours(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u1ArrResetTime[i])
                    .Subtract(new DateTime().AddHours(serverTime.Hour).AddMinutes(serverTime.Minute).AddSeconds(serverTime.Second));

                isFindTime = true;
                break;
            }
        }

        if (!isFindTime)
        {
            resetTime = new DateTime().AddDays(1).AddHours(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u1ArrResetTime[0])
                .Subtract(new DateTime().AddHours(serverTime.Hour).AddMinutes(serverTime.Minute).AddSeconds(serverTime.Second));
        }
	}
    //아이템 선택시 아이템 정보를 보여준다
	public void OnClickItemSlot(int index)
	{
        selectIndex = index;
        shopInfoWindow.gameObject.SetActive(true);
        shopInfoWindow.onClickBuy = OnClickBuy;
        shopInfoWindow.SetInfo(ShopInfoWindow.ShopType.Item, lstShopSlot[index].slotData.shopItem, TextManager.Instance.GetText("popup_title_shop_buy_black"));
	}
	
    // 구매 요청
	public void OnClickBuy()
	{
        //닫힌 경우 구매 불가
		if(closed)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_buy_disable"), TextManager.Instance.GetText("popup_desc_shop_black_shutdown"), null);
			return;
		}

        //빈 슬롯이 없어도 구매 불가
		if(Legion.Instance.cInventory.IsInvenFull())
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_buy_disable"), TextManager.Instance.GetText("popup_desc_inven_full"), null);
			return;
		}

        // 비용이 부족해도 구매 불가
		ShopItem shopItem = lstShopSlot[selectIndex].slotData.shopItem;
		if(!Legion.Instance.CheckEnoughGoods(shopItem.u1PriceType, shopItem.u4Price))
		{
			PopupManager.Instance.ShowChargePopup(shopItem.u1PriceType);
			return;
		}
	    
        RequestBuy();
	}
	
	private void RequestBuy()
	{
		PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.ShopBuyItem(SHOP_TYPE, (Byte)(selectIndex+1), AckShopBuy);
	}
	
	public void OnClickRefresh()
	{
		if(closed)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_no_refreash"), TextManager.Instance.GetText("popup_desc_no_refreash_black"), null);
			return;
        }
        
        // 갱신 비용
		byte priceType = ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u1Type;
		UInt32 price = (UInt32)(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u4Count
		                        + (ShopInfoMgr.Instance.ShopInfoBlack.u1RenewCount * ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2AddValue));
		
		if(price > ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2MaxValue)
			price = ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2MaxValue;
        
		refreshWindow.gameObject.SetActive(true);
        refreshWindow.SetInfo(priceType, (int)price);
        refreshWindow.onClickRefresh = Refresh;
	}
	
    // 상점 갱신 요청
    private void Refresh()
    {
		PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.RequsetShopList(SHOP_TYPE, 1, AckShopInstantRefresh);        
    }
    
	private void AckShopInstantRefresh(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();
		
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.SHOP_LIST, err), Server.ServerMgr.Instance.CallClear);
		}
		else if(err == Server.ERROR_ID.NONE)
		{
            // 상점 리스트 갱신(품절 처리)
			SetShopList();
			
            // 갱신 비용 처리
			string priceType = Legion.Instance.GetConsumeString(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u1Type);
			UInt32 price = (UInt32)(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u4Count
			                        + (ShopInfoMgr.Instance.ShopInfoBlack.u1RenewCount * ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2AddValue));
			
            // 갱신 비용 최대치 초과시 최대치로
			if(price > ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2MaxValue)
				price = ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2MaxValue;
			
            // 비용 처리
			Legion.Instance.SubGoods(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u1Type, ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u4Count +
			                         (ShopInfoMgr.Instance.ShopInfoBlack.u1RenewCount-1) * ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2AddValue);
            if((BLACK_SHOP_STATE)Legion.Instance.u1BlackMarketOpen == BLACK_SHOP_STATE.VIP_OPEN)
            {
				TimeCount();

				if(!timeChecking)
					StartCoroutine(CheckRefreshTime());
            }
            else
            {
				// 남은시간 갱신
				Legion.Instance.BlackMarketLeftTime = ShopInfoMgr.Instance.ShopInfoBlack.leftTime;
				closeTime = ShopInfoMgr.Instance.ShopInfoBlack.leftTime;
				if(!timeChecking)
					StartCoroutine(CheckCloseTime());
               
            }
            
                
            refreshWindow.OnClickClose();    
		}
	}
	
	// private void AckShopAutoRefresh(Server.ERROR_ID err)
	// {
	// 	PopupManager.Instance.CloseLoadingPopup();
		
	// 	if(err != Server.ERROR_ID.NONE)
	// 	{
	// 		PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(((int)err).ToString()), Server.ServerMgr.Instance.CallClear);
	// 	}
	// 	else if(err == Server.ERROR_ID.NONE)
	// 	{			
	// 		SetShopList();
			
	// 		string priceType = Legion.Instance.GetConsumeString(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u1Type);
	// 		UInt32 price = (UInt32)(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u4Count
	// 		                        + (ShopInfoMgr.Instance.ShopInfoBlack.u1RenewCount * ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2AddValue));
			
	// 		if(price > ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2MaxValue)
	// 			price = ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2MaxValue;

	// 		Legion.Instance.BlackMarketLeftTime = ShopInfoMgr.Instance.ShopInfoBlack.leftTime;
	// 		closeTime = ShopInfoMgr.Instance.ShopInfoBlack.leftTime;

	// 		if(!timeChecking)
	// 			StartCoroutine(CheckCloseTime());
                
    //         refreshWindow.OnClickClose();    
	// 	}
	// }
	
	private void AckShopBuy(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();
		
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.SHOP_BUY, err), Server.ServerMgr.Instance.CallClear);
		}
		else if(err == Server.ERROR_ID.NONE)
		{
			//품절 처리
			lstShopSlot[selectIndex].slotData.shopItem.u1SoldOut = 1;
			lstShopSlot[selectIndex].CheckSoldOut();
			//아이템 추가
			ItemInfo.ITEM_ORDER type = ItemInfoMgr.Instance.GetItemType(lstShopSlot[selectIndex].slotData.shopItem.u2ItemID);
			if(type == ItemInfo.ITEM_ORDER.RUNE)
			{
				Legion.Instance.cRuneventory.AddItem(lstShopSlot[selectIndex].slotData.shopItem.u2ItemID, lstShopSlot[selectIndex].slotData.shopItem.u4Count);
			}
			else
				Legion.Instance.cInventory.AddItem(0, lstShopSlot[selectIndex].slotData.shopItem.u2ItemID, lstShopSlot[selectIndex].slotData.shopItem.u4Count);
			//재화 소모
			byte priceType = lstShopSlot[selectIndex].slotData.shopItem.u1PriceType;
			UInt32 price = lstShopSlot[selectIndex].slotData.shopItem.u4Price;
            shopInfoWindow.OnClickClose();
            
            string title = TextManager.Instance.GetText("popup_title_shop_buy_common_result");
            string itemName = shopInfoWindow.itemName.text +TextManager.Instance.GetText("popup_desc_shop_buy_common_result");
            PopupManager.Instance.ShowOKPopup(title, itemName, null);
        }
	}
}
