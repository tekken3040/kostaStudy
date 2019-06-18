using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public enum BLACK_SHOP_STATE
{
    NONE = 0,   // 상태 없음
    CLOSE,      // 닫혀 있음
    OPEN,       // 오픈되어 있음
    VIP_OPEN,   // VIP 상시오픈
};

public class ShopPanel : MonoBehaviour {

    private const int BLACK_SHOP_TYPE = 3;
    private const int BLACK_INDEX = 6;
    public GameObject menuObject;
    public GameObject shopListObject;
	public GameObject[] tabs;
	public Toggle[] toggles;
    
    public Text timeLabel;
    public Text timeValue;
    public GameObject blackIcon;
    public GameObject blackButton;
    public Text blackPrice;
    public GameObject[] scrollBack;
    public GameObject scrollOver;
    public GameObject scrollOver2;
    
    public GameObject friendShip;
    public GameObject[] alramObject;
    public GameObject[] toggleAlramObject;
    public BlackShopTimePopup _cBlackShopTimePopup;

	private int tabIndex = 0;
    private DateTime blackTime;
    private bool useCash;
    private bool blackOpen;

	private bool bTimeChecking;

	private bool bSmallShop = false;
    
    //public enum BLACK_SHOP_STATE
    //{
	//	CLOSE = 1,
	//	OPEN = 2,
	//	VIP_OPEN = 3,
    //}

    public void Init()
    {
		#if UNITY_ANDROID
		IgaworksUnityAOS.IgaworksUnityPluginAOS.Adbrix.retention("Shop");
		#elif UNITY_IOS
		AdBrixPluginIOS.Retention("Shop");
		#endif
        // 상점 알람 처리
        if(!ShopInfoMgr.Instance.shopVisit)
        {
            ShopInfoMgr.Instance.shopVisit = true; 
            PopupManager.Instance.ShowLoadingPopup(1);
            Server.ServerMgr.Instance.LegionMark(1, AckLegionMark);
            
            for(int i=0; i<alramObject.Length; i++)
                alramObject[i].SetActive(false);
            
            if(ShopInfoMgr.Instance.normalCheck)
            {
                alramObject[0].SetActive(true);
                toggleAlramObject[0].SetActive(true);
            }
                
            if(ShopInfoMgr.Instance.equipCheck)
            {
                alramObject[1].SetActive(true);
                toggleAlramObject[1].SetActive(true);
            }
                
            if(ShopInfoMgr.Instance.blackCheck)
            {
                alramObject[2].SetActive(true);
                toggleAlramObject[2].SetActive(true);
            }
        }
        else
        {
            if(!ShopInfoMgr.Instance.normalCheck)
            {
                alramObject[0].SetActive(false);
                toggleAlramObject[0].SetActive(false);
            }
                
            if(!ShopInfoMgr.Instance.equipCheck)
            {
                alramObject[1].SetActive(false);
                toggleAlramObject[1].SetActive(false);
            }
                
            if(!ShopInfoMgr.Instance.blackCheck)
            {
                alramObject[2].SetActive(false);
                toggleAlramObject[2].SetActive(false);
            }
        }
        Legion.Instance.cTutorial.CheckTutorial(MENU.SHOP);
        
        // 암시장 상태값에 따라서 입장 가능 처리
		ChangeBlackShopUI();
       
        menuObject.SetActive(false);      
        shopListObject.SetActive(true);
        
        for(int i=0; i<tabs.Length; i++)
            tabs[i].SetActive(false); 
            
        PopupManager.Instance.AddPopup(gameObject, OnClickClose);

		if(Legion.Instance.bStageFailed)
			PopupManager.Instance.SetNoticePopup (MENU.SHOP);
    }

	public void Open(POPUP_SHOP openPopupType)
	{
        switch(openPopupType)
        {
            case POPUP_SHOP.GOLD:
            OnClickMenu(3);
            //OnClickMenu(5);
            break;
            
            case POPUP_SHOP.CASH:
            OnClickMenu(5);
            break;
            
            case POPUP_SHOP.ENERGY:
            OnClickMenu(4);
            break;
            
            case POPUP_SHOP.NORMAL:
            OnClickMenu(0);
            break;
            
            case POPUP_SHOP.EQUIPMENT:
            OnClickMenu(1);
            break;
            
            case POPUP_SHOP.RANDOM:
            OnClickMenu(2);
            break;
            
            case POPUP_SHOP.BLACK:
            OnClickMenu(6);
            break;

			case POPUP_SHOP.THOUSAND:
			OnClickMenu(7);
			break;
        }
	}
    
    public void OnClickMenu(int index)
    {
        if(index != BLACK_INDEX)
        {
            if(!menuObject.activeSelf)
                menuObject.SetActive(true);
            
            if(shopListObject.activeSelf)
                shopListObject.SetActive(false);                    
            
            tabIndex = index;
			if (bSmallShop) {
				tabs[7].SetActive(false);
				toggles [tabIndex].GetComponent<ToggleImage> ().image.sprite = toggles [tabIndex].GetComponent<ToggleImage> ().changeSprite;
			}
			bSmallShop = false;

            toggles[index].isOn = true;
            tabs[index].SetActive(true);
            //tabs[index].SendMessage("Init");
            
            //토글 알람//
            if(index < 2)            
                toggleAlramObject[index].SetActive(false);    
                
            // 뽑기상점 우정 포인트 활성화
			//기능막음 2017.08.08 jc
//			if(index == 2 || index == 4)
//                friendShip.gameObject.SetActive(true);
//            else
				friendShip.gameObject.SetActive(false);
            GoodsInfo[] goodInfo = GameObject.FindObjectsOfType<GoodsInfo>() as GoodsInfo[];
        
            for(int i=0; i<goodInfo.Length; i++)
                goodInfo[i].Refresh();

            switch(index)
            {
                case 0:
                    scrollBack[0].SetActive(true);
                    scrollBack[1].SetActive(false);
                    scrollBack[2].SetActive(false);
                    break;
                case 1:
                    scrollBack[0].SetActive(false);
                    scrollBack[1].SetActive(true);
                    scrollBack[2].SetActive(false);
                    break;
                case 2:
                    scrollBack[0].SetActive(false);
                    scrollBack[1].SetActive(false);
                    scrollBack[2].SetActive(true);
                    break;
                case 3:
                    scrollBack[0].SetActive(false);
                    scrollBack[1].SetActive(false);
                    scrollBack[2].SetActive(true);
                    break;
                case 4:
                    scrollBack[0].SetActive(false);
                    scrollBack[1].SetActive(false);
                    scrollBack[2].SetActive(true);
                    break;
                case 5:
                    scrollBack[0].SetActive(false);
                    scrollBack[1].SetActive(false);
                    scrollBack[2].SetActive(true);
                    break;
                case 6:
                    scrollBack[0].SetActive(true);
                    scrollBack[1].SetActive(false);
                    scrollBack[2].SetActive(false);
                    break;
            }
            if(index > 1 && index < BLACK_INDEX)
            {                
                scrollOver.gameObject.SetActive(true);
                scrollOver2.gameObject.SetActive(true);
            }
            else
            {
                scrollOver.gameObject.SetActive(false);
                scrollOver2.gameObject.SetActive(false);
            } 
        }
        else
        {
            useCash = false;
            RequestBlackMarket();
        }
    }
	
	public void OnClickShop(int index)
	{
        if(toggles[index].isOn)
        {
            // if(tabIndex == index)
            //   return;
            // 천원샵이 오픈되어 있다면 천원샵 비활성화
            if (bSmallShop)
            {
                tabs[7].SetActive(false);
                toggles[index].GetComponent<ToggleImage>().image.sprite = toggles[index].GetComponent<ToggleImage>().changeSprite;
            }
            bSmallShop = false;
            
            if (index == BLACK_INDEX)
            {
				RequestBlackMarket(true);
                return;
            }
            tabIndex = index;

            if(!menuObject.activeSelf)
            menuObject.SetActive(true);
            
            if(shopListObject.activeSelf)
            shopListObject.SetActive(false);
            
            tabs[index].SetActive(true);
            tabs[index].SendMessage("Init");
            
            //토글 알람//
            if(index < 2)            
                toggleAlramObject[index].SetActive(false);              
            
            // 뽑기상점 우정 포인트 활성화
			//기능막음 2017.08.08 jc
//			if(index == 2 || index == 4)
//                friendShip.gameObject.SetActive(true);
//            else
                friendShip.gameObject.SetActive(false);            
                
            switch(index)
            {
                case 0:
                    scrollBack[0].SetActive(true);
                    scrollBack[1].SetActive(false);
                    scrollBack[2].SetActive(false);
                    break;
                case 1:
                    scrollBack[0].SetActive(false);
                    scrollBack[1].SetActive(true);
                    scrollBack[2].SetActive(false);
                    break;
                case 2:
                    scrollBack[0].SetActive(false);
                    scrollBack[1].SetActive(false);
                    scrollBack[2].SetActive(true);
                    break;
                case 3:
                    scrollBack[0].SetActive(false);
                    scrollBack[1].SetActive(false);
                    scrollBack[2].SetActive(true);
                    break;
                case 4:
                    scrollBack[0].SetActive(false);
                    scrollBack[1].SetActive(false);
                    scrollBack[2].SetActive(true);
                    break;
                case 5:
                    scrollBack[0].SetActive(false);
                    scrollBack[1].SetActive(false);
                    scrollBack[2].SetActive(true);
                    break;
                case 6:
                    scrollBack[0].SetActive(true);
                    scrollBack[1].SetActive(false);
                    scrollBack[2].SetActive(false);
                    break;
            }
                
            if(index > 1 && index < BLACK_INDEX)
            {                
                scrollOver.gameObject.SetActive(true);
                scrollOver2.gameObject.SetActive(true);
            }
            else
            {
                scrollOver.gameObject.SetActive(false);
                scrollOver2.gameObject.SetActive(false);
            }                
        }
        else
        {
            tabs[index].SetActive(false);
        }
	}

	public void OnClickSmallPayShop()
	{
		if (bSmallShop)
			return;

		toggles [tabIndex].GetComponent<ToggleImage> ().image.sprite = toggles [tabIndex].GetComponent<ToggleImage> ().defaultSprite;
		tabs[tabIndex].SetActive(false);
		bSmallShop = true;

		tabs[7].SetActive(true);
		tabs[7].SendMessage("Init");

		if(!menuObject.activeSelf)
			menuObject.SetActive(true);

		if(shopListObject.activeSelf)
			shopListObject.SetActive(false);
		
		friendShip.gameObject.SetActive(false); 

		scrollBack[0].SetActive(false);
		scrollBack[1].SetActive(false);
		scrollBack[2].SetActive(true);

		scrollOver.gameObject.SetActive(true);
		scrollOver2.gameObject.SetActive(true);
	}
    
    // 암시장 즉시 입장
	public void OnClickEnter(object[] param)
    {       
        Goods goods = ShopInfoMgr.Instance.dicShopData[BLACK_SHOP_TYPE].cPassGoods;
        
        //재화 부족시 불가능
		if(!Legion.Instance.CheckEnoughGoods(goods.u1Type, goods.u4Count))
		{
			//PopupManager.Instance.ShowChargePopup(goods.u1Type);
            StartCoroutine(NotEnoughGoods(goods));
			return;
		}        
        
        useCash = true;
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequsetBlackShopList((Byte)BLACK_SHOP_TYPE, 0, 1, AckShopAutoRefresh);
    }
    
    IEnumerator NotEnoughGoods(Goods goods)
    {
        if(PopupManager.Instance.objYesNoPopup.activeSelf)
            PopupManager.Instance.objYesNoPopup.GetComponent<YesNoPopup>().Close();
        yield return new WaitForSeconds(0.0f);
        PopupManager.Instance.ShowChargePopup(goods.u1Type);
    }

    // 일반 입장
	private void RequestBlackMarket(bool isTopMenuClick = false) // <= 상점 메뉴와 탑 메뉴와의 클릭여부를 판별
    {   
        // 열리는 시간이 아닐 경우 불가능 
		if((BLACK_SHOP_STATE)Legion.Instance.u1BlackMarketOpen == BLACK_SHOP_STATE.CLOSE)   
        {    
			if(useCash == false)
			{
				// 2016. 07. 27 jy 
				// 기존의 열리지 않으면 팝업창을 암시장 오픈하지 않았다는 팝업을 띄우기만 했지만
				// 팝업창을 띄우고 바로 입장 할 수 있도록 변경
				string msg = string.Format(TextManager.Instance.GetText("popup_blackmarket_desc"), ShopInfoMgr.Instance.dicShopData[BLACK_SHOP_TYPE].cPassGoods.u4Count.ToString());
                //PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("popup_title_shop_buy_black"), msg, TextManager.Instance.GetText("btn_shop_main_sub_now"), OnClickEnter, null);
                _cBlackShopTimePopup.Show(TextManager.Instance.GetText("popup_title_shop_buy_black"), msg, TextManager.Instance.GetText("btn_shop_main_sub_now"), OnClickEnter, null);
                _cBlackShopTimePopup.SetTimeTitle(TextManager.Instance.GetText("mark_shop_main_black_open"));
                PopupManager.Instance.AddPopup(_cBlackShopTimePopup.gameObject, _cBlackShopTimePopup.OnClickNo);
                
                if (_cBlackShopTimePopup.gameObject.activeSelf == true)
                {
                    TimeSpan timeSpan = Legion.Instance.BlackMarketLeftTime - Legion.Instance.ServerTime;
                    if (timeSpan.Ticks > 0)
                        _cBlackShopTimePopup.SetTime(string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds));
                }
                // 탑 메뉴로 클릭 했을시에만 상점이 닫혔있을때 UI 변경을 하지않는다
                if (isTopMenuClick) 
					toggles[tabIndex].isOn = true;
			}
            return;
        }
        else if((BLACK_SHOP_STATE)Legion.Instance.u1BlackMarketOpen == BLACK_SHOP_STATE.OPEN ||
                (BLACK_SHOP_STATE)Legion.Instance.u1BlackMarketOpen == BLACK_SHOP_STATE.VIP_OPEN) 
        {
            // 암시장 정보가 있으면 암시장 오픈 아니면 정보 요청
            if(ShopInfoMgr.Instance.ShopInfoBlack == null)
            {
                useCash = false;
                PopupManager.Instance.ShowLoadingPopup(1);
                Server.ServerMgr.Instance.RequsetShopList((Byte)BLACK_SHOP_TYPE, 0, AckShopAutoRefresh);
            }
            else
            {
                if(!menuObject.activeSelf)
                    menuObject.SetActive(true);
                
                if(shopListObject.activeSelf)
                    shopListObject.SetActive(false);            
                            
                tabIndex = BLACK_INDEX;
                toggles[BLACK_INDEX].isOn = true;
                tabs[BLACK_INDEX].SetActive(true);
                tabs[BLACK_INDEX].SendMessage("Init");                   
                toggleAlramObject[2].SetActive(false);
                scrollBack[0].SetActive(true);
                scrollBack[1].SetActive(false);
                scrollBack[2].SetActive(false);

				scrollOver.gameObject.SetActive(false);
				scrollOver2.gameObject.SetActive(false);
            }
        }
        else
        {
            PopupManager.Instance.ShowLoadingPopup(1);
            Server.ServerMgr.Instance.RequsetShopList((Byte)BLACK_SHOP_TYPE, 0, AckShopAutoRefresh);
        }
    }
    
    // 암시장 요청 결과 처리
	private void AckShopAutoRefresh(Server.ERROR_ID err)
	{
		//DebugMgr.Log("AckShopAuto " + err);

		PopupManager.Instance.CloseLoadingPopup();
		
		if(err != Server.ERROR_ID.NONE)
		{
            if(err == Server.ERROR_ID.SHOP_NO_DATA)
            {
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_shop_buy_black"), TextManager.Instance.GetText("popup_desc_shop_open_black_fail"), Server.ServerMgr.Instance.CallClear);
            }
            else
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.SHOP_LIST, err), Server.ServerMgr.Instance.CallClear);
            
            // 상점이 열리지 않을 경우
            if(err == Server.ERROR_ID.SHOP_NO_DATA)
            {
				Legion.Instance.u1BlackMarketOpen = (Byte)BLACK_SHOP_STATE.CLOSE; // 상점 닫혀있는 상태로 변경 
				ChangeBlackShopUI();
				OnClickShop(tabIndex);
            }
		}
		else
		{
            if(!menuObject.activeSelf)
                menuObject.SetActive(true);
            
            if(shopListObject.activeSelf)
                shopListObject.SetActive(false);            
                        
            tabIndex = BLACK_INDEX;
            toggles[BLACK_INDEX].isOn = true;
            tabs[BLACK_INDEX].SetActive(true);
            tabs[BLACK_INDEX].SendMessage("Init");
            toggleAlramObject[2].SetActive(false);
            
			/// VIP_OPEN 상태가 아닌 상태를 제외하고 일반 오픈상태로 변경
            if((BLACK_SHOP_STATE)Legion.Instance.u1BlackMarketOpen != BLACK_SHOP_STATE.VIP_OPEN)      
                Legion.Instance.u1BlackMarketOpen = (Byte)BLACK_SHOP_STATE.OPEN;
            
            // 즉시 입장일 경우 캐쉬 소모
            if(useCash)
            {
                Legion.Instance.SubGoods(ShopInfoMgr.Instance.dicShopData[BLACK_SHOP_TYPE].cPassGoods);
            }      
            scrollBack[0].SetActive(true);
            scrollBack[1].SetActive(false);
            scrollBack[2].SetActive(false);

			scrollOver.gameObject.SetActive(false);
			scrollOver2.gameObject.SetActive(false);
		}
	}
	
	// public void RequestFixShop(int fixID)
	// {
	// 	Server.ServerMgr.Instance.ShopFixShop((UInt16)fixID, AckFixShop);
	// }
	
	// private void AckFixShop(Server.ERROR_ID err)
	// {		
	// 	if(err != Server.ERROR_ID.NONE)
	// 	{
	// 		PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(((int)err).ToString()), null);
	// 		Server.ServerMgr.Instance.ClearFirstJobError ();
	// 	}
	// 	else if(err == Server.ERROR_ID.NONE)
	// 	{
	// 		//ShopItem shopItem = ShopInfoMgr.Instance.shopFixItem;			
	// 		//DebugMgr.Log(shopItem.u2ItemID);
	// 	}
	// }

    // 상점 확인 처리
    public void AckLegionMark(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        ShopInfoMgr.Instance.lastShopTime = Legion.Instance.ServerTime;

		if( bTimeChecking == false )
			ChangeBlackShopUI();

        DebugMgr.Log(err);
    }

	public void OnClickClose()
	{
        scrollBack[0].SetActive(false);
        scrollBack[1].SetActive(false);
        scrollBack[2].SetActive(false);
        bSmallShop = false;

        PopupManager.Instance.RemovePopup(gameObject);
		StartCoroutine(Fade());		
	}
    
    IEnumerator Fade()
    {
        FadeEffectMgr.Instance.FadeOut();
        yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
        scrollOver.gameObject.SetActive(false);
        scrollOver2.gameObject.SetActive(false);
        GameObject.FindObjectOfType<BaseScene>().CloseShop();
        gameObject.SetActive(false);
		if (Legion.Instance.cTutorial.au1Step [8] == 1) {
			Legion.Instance.cTutorial.CheckTutorial(MENU.MAIN);
		}
	}
	/*
    // 암시장이 열리는 시간을 체크해서 보여준다
	private void TimeCount(Byte BlackMarketOpen)
	{
		int length = ShopInfoMgr.Instance.dicShopData[BLACK_SHOP_TYPE].u1ArrResetTime.Length;

        switch(BlackMarketOpen)
        {
			/*
            case 2: // 오픈
                int j;
                for(int i=0; i<length; i++)
		        {
                    if((i+1) >= length)
                        j = 0;
                    else
                        j = i+1;
                    if(Legion.Instance.ServerTime < Legion.Instance.ServerTime.Date.AddHours(ShopInfoMgr.Instance.dicShopData[BLACK_SHOP_TYPE].u1ArrResetTime[j]).AddSeconds(ShopInfoMgr.Instance.dicShopData[BLACK_SHOP_TYPE].u4KeepTime))
		        	{
                        if((j+1) >= length)
                            j=0;

                        blackTime = Legion.Instance.ServerTime.Date.AddHours(ShopInfoMgr.Instance.dicShopData[BLACK_SHOP_TYPE].u1ArrResetTime[j+1]);
                        DateTime blackTimeKepp = blackTime.AddSeconds(ShopInfoMgr.Instance.dicShopData[BLACK_SHOP_TYPE].u4KeepTime);
                        timeValue.text = string.Format("{0:D2}:{1:D2} ~ {2:D2}:{3:D2}", blackTime.Hour, blackTime.Minute, blackTimeKepp.Hour, blackTimeKepp.Minute);                                
                       						
		        		break;
		        	}        
		        }
                //blackOpen = false;
                break;
            case 2:
                for(int i=0; i<length; i++)
		        {
                    if(Legion.Instance.ServerTime < Legion.Instance.ServerTime.Date.AddHours(ShopInfoMgr.Instance.dicShopData[BLACK_SHOP_TYPE].u1ArrResetTime[i]).AddSeconds(ShopInfoMgr.Instance.dicShopData[BLACK_SHOP_TYPE].u4KeepTime))
		        	{
		        		blackTime = Legion.Instance.ServerTime.Date.AddHours(ShopInfoMgr.Instance.dicShopData[BLACK_SHOP_TYPE].u1ArrResetTime[i]);
                        DateTime blackTimeKepp = blackTime.AddSeconds(ShopInfoMgr.Instance.dicShopData[BLACK_SHOP_TYPE].u4KeepTime);
                        timeValue.text = string.Format("{0:D2}:{1:D2} ~ {2:D2}:{3:D2}", blackTime.Hour, blackTime.Minute, blackTimeKepp.Hour, blackTimeKepp.Minute);                                
						
		        		break;
		        	}        
		        }
                //blackOpen = false;
                break;

            case 3:
                for(int i=0; i<length; i++)
                {
                    // 현재 시간이 암시장 오픈 시간일 경우
                    if(Legion.Instance.ServerTime >= Legion.Instance.ServerTime.Date.AddHours(ShopInfoMgr.Instance.dicShopData[BLACK_SHOP_TYPE].u1ArrResetTime[i])
                    && Legion.Instance.ServerTime <= Legion.Instance.ServerTime.Date.AddHours(ShopInfoMgr.Instance.dicShopData[BLACK_SHOP_TYPE].u1ArrResetTime[i]).AddSeconds(ShopInfoMgr.Instance.dicShopData[BLACK_SHOP_TYPE].u4KeepTime))
                    {                
                        blackOpen = true;
                        break;
                    }
                }
                break;
            case 4:
                for(int i=0; i<length; i++)
                {
                    // 현재 시간이 암시장 오픈 시간일 경우
                    if(Legion.Instance.ServerTime >= Legion.Instance.ServerTime.Date.AddHours(ShopInfoMgr.Instance.dicShopData[BLACK_SHOP_TYPE].u1ArrResetTime[i])
                    && Legion.Instance.ServerTime <= Legion.Instance.ServerTime.Date.AddHours(ShopInfoMgr.Instance.dicShopData[BLACK_SHOP_TYPE].u1ArrResetTime[i]).AddSeconds(ShopInfoMgr.Instance.dicShopData[BLACK_SHOP_TYPE].u4KeepTime))
                    {                
                        blackOpen = true;
                        break;
                    }
                }
                break;
            
        }
	}
	*/
	private void ChangeBlackShopUI()
	{
		// 16.6.22 jy
		// 마켓 오픈 상태 Index값이 변경
		switch((BLACK_SHOP_STATE)Legion.Instance.u1BlackMarketOpen)
		{
		case BLACK_SHOP_STATE.CLOSE:       // 닫혀있음     
			timeLabel.text = TextManager.Instance.GetText("mark_shop_main_black_open");
			if( bTimeChecking == false)
				StartCoroutine("CheckCloseTime");
			break;
		case BLACK_SHOP_STATE.OPEN:		// 오픈
			timeLabel.text = TextManager.Instance.GetText("mark_time_remaining");
			if( bTimeChecking == false)
				StartCoroutine("CheckCloseTime");
			break;
		case BLACK_SHOP_STATE.VIP_OPEN:		// vip 오픈  
			StopCoroutine("CheckCloseTime");
			bTimeChecking = false;
			timeLabel.text = "";
			timeValue.text = "";
			break;
		}             
	}

    public ShopTab GetShopTab(int idx)
    {
        if(tabs.Length <= idx)
        {
            return null;
        }

        return tabs[idx].GetComponent<ShopTab>();
    }

	public void OnDisable()
	{
		bTimeChecking = false; // 시간 갱신 멈춤
	}

    //폐쇠시간 체크
    private IEnumerator CheckCloseTime()
    {
        bTimeChecking = true;
        while (true)
        {
            TimeSpan timeSpan = Legion.Instance.BlackMarketLeftTime - Legion.Instance.ServerTime;
            if (timeSpan.Ticks > 0)
            {
                timeValue.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                // 암시장이 팝업이 오픈되어 있다면 시간을 셋팅한다
                if(_cBlackShopTimePopup.gameObject.activeSelf == true)
                    _cBlackShopTimePopup.SetTime(timeValue.text);
            }
            else
            {
                // 2016.06.27 jy
                // 상점 오픈 타임 및 닫는 타임을 서버로 부터 받아 처리 한다
                bTimeChecking = false;
                // 암시장 팝업이 오픈되어 있다면 비활성화 처리한다
                if (_cBlackShopTimePopup.gameObject.activeSelf == true)
                {
                    _cBlackShopTimePopup.gameObject.SetActive(false);
                    PopupManager.Instance.RemovePopup(_cBlackShopTimePopup.gameObject);
                }

                Server.ServerMgr.Instance.LegionMark(1, AckLegionMark);
                yield break;
            }

            yield return new WaitForSeconds(1f);
        }
    }
}
