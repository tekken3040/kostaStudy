using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class ShopNomalTab : MonoBehaviour {

	private const int SHOP_TYPE = 1;

	public Text refreshtimeText;
    public RectTransform content;
    public ShopInfoWindow shopInfoWindow;
    public RefreshWindow refreshWindow;

	private int selectIndex = -1;
	private DateTime resetTime;
	private GameObject shopItemSlot;
	private List<ShopItemSlot> lstShopSlot = new List<ShopItemSlot>();
	private bool init = false;
	private bool timeChecking = false;

	public void Init()
	{
		if(init)
			return;

		PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.RequsetShopList((Byte)SHOP_TYPE, 0, AckShopAutoRefresh);
	}

	public void OnEnable()
	{
		if(!timeChecking && init)
			StartCoroutine(CheckRefreshTime());
            
        ShopInfoMgr.Instance.normalCheck = false;            
	}

	public void OnDisable()
	{
		timeChecking = false;
    }

	//아이템 목록 생성
	public void SetShopList()
	{
		if(shopItemSlot == null)
			shopItemSlot = AssetMgr.Instance.AssetLoad("Prefabs/UI/Shop/Shop_ItemSlot.prefab", typeof(GameObject)) as GameObject;

		Shop shopData = ShopInfoMgr.Instance.ShopInfoNormal;

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

		init = true;
	}

	//갱신 시간 체크
	private IEnumerator CheckRefreshTime()
	{
		timeChecking = true;

		while(true)
		{
			TimeSpan timeSpan = resetTime - Legion.Instance.ServerTime;

			if(timeSpan.Ticks > 0)
			{
				refreshtimeText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			}
			else 
			{
				PopupManager.Instance.ShowLoadingPopup(1);
				Server.ServerMgr.Instance.RequsetShopList(SHOP_TYPE, 0, AckShopAutoRefresh);
				timeChecking = false;
				yield break;
			}
			
			yield return new WaitForSeconds(1f);
		}
	}
    
    //아이템 클릭시 정보창 띄워줌
	public void OnClickItemSlot(int index)
	{
        selectIndex = index;
        shopInfoWindow.gameObject.SetActive(true);
        shopInfoWindow.onClickBuy = OnClickBuy;
        shopInfoWindow.SetInfo(ShopInfoWindow.ShopType.Item, lstShopSlot[index].slotData.shopItem, TextManager.Instance.GetText("popup_title_shop_buy_common"));
	}
    
    // 구입 요청
	public void OnClickBuy()
	{
		if(Legion.Instance.cInventory.IsInvenFull())
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_buy_disable"), TextManager.Instance.GetText("popup_desc_inven_full"), null);
			return;
		}

		ShopItem shopItem = lstShopSlot[selectIndex].slotData.shopItem;
		if(!Legion.Instance.CheckEnoughGoods(shopItem.u1PriceType, shopItem.u4Price))
		{
			PopupManager.Instance.ShowChargePopup(shopItem.u1PriceType);
			return;
		}

		//PopupManager.Instance.ShowYesNoPopup("상품 구입", "", RequestBuy, null);
        
        RequestBuy();
	}

	private void RequestBuy()
	{
		PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.ShopBuyItem(SHOP_TYPE, (Byte)(selectIndex+1), AckShopBuy);
	}
    
    //갱신 클릭 처리	
	public void OnClickRefresh()
	{
        byte priceType = ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u1Type;
		UInt32 price = (UInt32)(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u4Count
		                        + (ShopInfoMgr.Instance.ShopInfoNormal.u1RenewCount * ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2AddValue));
		
		if(price > ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2MaxValue)
			price = ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2MaxValue;
        
        refreshWindow.gameObject.SetActive(true);
        refreshWindow.SetInfo(priceType, (int)price);
        refreshWindow.onClickRefresh = Refresh;
	}
    
    // 갱신 요청
    private void Refresh()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.RequsetShopList(SHOP_TYPE, 1, AckShopInstantRefresh);
    }

	//즉시 갱신
	private void AckShopInstantRefresh(Server.ERROR_ID err)
	{
		DebugMgr.Log("AckShopInstant " + err);

		PopupManager.Instance.CloseLoadingPopup();
		
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.SHOP_LIST, err), Server.ServerMgr.Instance.CallClear);
		}
		else if(err == Server.ERROR_ID.NONE)
		{
			SetShopList();

			string priceType = Legion.Instance.GetConsumeString(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u1Type);
			UInt32 price = (UInt32)(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u4Count
			                        + (ShopInfoMgr.Instance.ShopInfoNormal.u1RenewCount * ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2AddValue));
			
			if(price > ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2MaxValue)
				price = ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2MaxValue;

			if(ShopInfoMgr.Instance.ShopInfoNormal.u1RenewCount != 0)
			{
				Legion.Instance.SubGoods(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u1Type, ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u4Count +
			 	                        (ShopInfoMgr.Instance.ShopInfoNormal.u1RenewCount-1) * ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2AddValue);
			}

			TimeCount();

			resetTime = ShopInfoMgr.Instance.ShopInfoNormal.leftTime;

			if(!timeChecking)
				StartCoroutine(CheckRefreshTime());
            
            refreshWindow.OnClickClose();
            PopupManager.Instance.RemovePopup(refreshWindow.gameObject);
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
			//shopInfoWindow.gameObject.SetActive(false);

			SetShopList();
			
			string refresh_priceType = Legion.Instance.GetConsumeString(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u1Type);
			UInt32 refresh_price = (UInt32)(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u4Count
			                        + (ShopInfoMgr.Instance.ShopInfoNormal.u1RenewCount * ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2AddValue));
			
			if(refresh_price > ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2MaxValue)
				refresh_price = ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2MaxValue;
			
			TimeCount();

			resetTime = ShopInfoMgr.Instance.ShopInfoNormal.leftTime;

			if(!timeChecking)
				StartCoroutine(CheckRefreshTime());
                
            refreshWindow.OnClickClose();
		}
	}
    
    // 구입 결과 처리
	private void AckShopBuy(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();
		
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.SHOP_BUY, err), Server.ServerMgr.Instance.CallClear);
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

			EventDisCountinfo disInfo = EventInfoMgr.Instance.GetDiscountEventinfo (DISCOUNT_ITEM.BUYSHOP);
			if (disInfo != null) price = (uint)(price * disInfo.discountRate);

			shopInfoWindow.OnClickClose();
                        
            string title = TextManager.Instance.GetText("popup_title_shop_buy_common_result");
            string itemName = shopInfoWindow.itemName.text +TextManager.Instance.GetText("popup_desc_shop_buy_common_result");
            PopupManager.Instance.ShowOKPopup(title, itemName, null);
		}
	}

	public void TimeCount()
	{
		int length = ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u1ArrResetTime.Length;

		for(int i=0; i<length; i++)
		{
			if(Legion.Instance.ServerTime < Legion.Instance.ServerTime.Date.AddHours(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u1ArrResetTime[i]))
			{
				ShopInfoMgr.Instance.ShopInfoNormal.leftTime = Legion.Instance.ServerTime.Date.AddHours(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u1ArrResetTime[i]);
				break;
			}
		}
	}
}
