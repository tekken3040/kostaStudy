using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using IgaworksUnityAOS;

public class BaseEventPopup : MonoBehaviour
{
	public enum PackageMenu
	{
        Promot = 3,     // ??
        Month = 5,      // 월
        Growth = 6,     // 육성팩
		Limit = 7,      // 한정팩
		Character = 8,  // 캐릭터
        Equip = 9,       // 장비팩
        MonthlyGoods = 10, // 월간 상품
	}

    public GameObject _titleObj;        // 강력추천에서는 타이틀 오브젝트를 비활성화 해야 한다
    public GameObject _closeBtnObj;     // 
	public RectTransform _trSlotParent;	// 패키지 상품을 널을 슬롯 부모
    public Text _txtBeforeBuyNotice;    // 구입 안내 사항

    protected GameObject _objSlot;		// 패키지의 상품
    protected EventPackageInfo _cBuyPackageInfo;    // 구매할 상품 정보
    
    // 상속 받는 자식에서 구현한다
    public virtual bool SetPopup(bool isStrongPopup = false)
	{
        // 인앱 초기화
#if UNITY_EDITOR
#elif UNITY_ONESTORE
        ShopInfoMgr.Instance.SettingInAppOneStore();
#else
        ShopInfoMgr.Instance.SettingInApp();
#endif
        //if (_titleObj != null)
        //    _titleObj.SetActive(!isStrongPopup);

        if (_closeBtnObj != null)
            _closeBtnObj.SetActive(!isStrongPopup);

        if (_txtBeforeBuyNotice != null)
            SetBeforeBuyNotice();

        return true;
    }

	// 팝업을 닫는다
	public virtual void OnClickClosePopup()
	{
        //SetPopup();	// 임시 슬롯 생성 버튼으로 활용중...
        //this.gameObject.SetActive(false);
        PopupManager.Instance.RemovePopup(this.gameObject);
        Destroy(this.gameObject);
	}

	// 기기별 언어별 판매금액을 셋팅하여 반환한다
	protected string GetEventPackagePriceString(EventPackageInfo packageInfo)
	{
		StringBuilder tempString = new StringBuilder();
		#if UNITY_ANDROID || UNITY_EDITOR
		if(TextManager.Instance.eLanguage == TextManager.LANGUAGE_TYPE.KOREAN)
			tempString.Append("￦").Append(packageInfo.cNeedGoods.u4Count.ToString("n0"));
		else
			tempString.Append("$").Append(packageInfo.iOSPrice.ToString());
		#elif UNITY_IOS
		tempString.Append("$").Append(packageInfo.iOSPrice.ToString());
		#endif
		return tempString.ToString();
	}

    protected void OnClickBuyBtn(EventPackageInfo info)
    {
        if (info.u1EventType == (byte)EVENT_TYPE.CHARACTERGOODS)
            CharacterBuyMessagePopup(info);
        else
            BuyPackage(info);
    }

    protected void CharacterBuyMessagePopup(EventPackageInfo info)
    {
        // 2016. 10. 24 jy 
        // 구매 보상으로 재화가 오버시 예외처리
        if (Legion.Instance.CheckGoodsLimitExcessx(info.cRewardOnce.u1Type))
        {
            Legion.Instance.ShowGoodsOverMessage(info.cRewardOnce.u1Type);
            return;
        }
        // 상품 구매 가능 오버 체크
        for (int i = 0; i < info.acPackageRewards.Length; ++i)
        {
            if(info.acPackageRewards[i].u1Type == 0)
                break;

            if (Legion.Instance.CheckGoodsLimitExcessx(info.acPackageRewards[i].u1Type))
            {
                Legion.Instance.ShowGoodsOverMessage(info.acPackageRewards[i].u1Type);
                return;
            }
        }
        _cBuyPackageInfo = info;
        if (AssetMgr.Instance.CheckDivisionDownload(1, info.acPackageRewards[0].u2ID))
            PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("popup_title_buy_class"), TextManager.Instance.GetText("popup_desc_buy_class"), OkPackageBuy, null);
        else
            OkPackageBuy(null);
    }

    protected void OkPackageBuy(object[] obj)
    {
        if (_cBuyPackageInfo == null)
            return;

        BuyPackage(_cBuyPackageInfo);
    }

    // ========================= 결제 관련 함수 ======================= //
    public virtual void BuyPackage(EventPackageInfo info)//EventPackageInfo info, PackageSlot slot)
	{
        if (info.u1EventType == (Byte)PackageMenu.Growth || info.u1EventType == (Byte)PackageMenu.Limit) 
		{
			for(int i = 0; i < info.acPackageRewards.Length; ++i)
			{
				if(info.acPackageRewards[i].u1Type == 0)
					continue;

				// 상품이 바로 들오는 패키지는 미리 확인한다
				if(Legion.Instance.CheckGoodsLimitExcessx(info.acPackageRewards[i].u1Type) == true)
				{
					Legion.Instance.ShowGoodsOverMessage(info.acPackageRewards[i].u1Type);
					return;
				}
			}

			if (Legion.Instance.CheckEnoughGoods (info.cNeedMinPeriod))
			{
				if (info.cNeedMaxPeriod.u1Type == (Byte)GoodsType.LEVEL) 
				{
					if (Legion.Instance.TopLevel > info.cNeedMaxPeriod.u4Count) 
					{
						PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_event_buy_wrong") , TextManager.Instance.GetText("mark_event_over_level"), null);
						return;
					}
				}
			} 
			else 
			{
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_event_buy_wrong") , TextManager.Instance.GetText("mark_event_not_active"), null);
				return;
			}
		}

		// 2016. 10. 24 jy
		// 구매시 재화 보상을 체크한다
		if(Legion.Instance.CheckGoodsLimitExcessx(info.cRewardOnce.u1Type) == true)
		{
			Legion.Instance.ShowGoodsOverMessage(info.cRewardOnce.u1Type);
			return;
		}

        _cBuyPackageInfo = info;
        //결제일 경우
        if (info.cNeedGoods.u1Type == (byte)GoodsType.PURCHASE)
		{
#if UNITY_EDITOR
			RequestBuy();
#elif UNITY_ONESTORE
            if(IapManager.Instance.IsInited)
            {
                PopupManager.Instance.ShowLoadingPopup(1);
                ShopResult.OnResponse CallBack = new ShopResult.OnResponse(RequestBuy);
                DebugMgr.LogError("원스토어 결제 호출");
                IapManager.Instance.RequestPaymenet(info.OnestoreCord,
                    //info.sName,
                    TextManager.Instance.GetText(info.sName),
                    info.u2ID.ToString(),
                    info.sName, CallBack);
            }
#else
			if(UM_InAppPurchaseManager.Instance.IsInited)
			{
				PopupManager.Instance.ShowLoadingPopup(1);
				UM_InAppPurchaseManager.OnPurchaseFlowFinishedAction += OnPurchaseFlowFinishedAction;
				UM_InAppPurchaseManager.Instance.Purchase(info.u2ID.ToString());
			}
			else
			{
				DebugMgr.LogError("InAppPurchase Init Failed");
			}
#endif
        }
        else
		{
			if(!Legion.Instance.CheckEnoughGoods(info.cNeedGoods))
			{
				PopupManager.Instance.ShowChargePopup(info.cNeedGoods.u1Type);
				return;
			}        
			RequestBuy();               
		}
	}

	// OS 결제 결과 처리
	protected void OnPurchaseFlowFinishedAction (UM_PurchaseResult result)
	{
		PopupManager.Instance.CloseLoadingPopup();
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

	// 서버로 구입 요청 처리
	protected void RequestBuy(string receipt = "", string txid = "")
	{
		PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.RequestEventGoodsBuy(_cBuyPackageInfo.u2ID, receipt, txid, AckFixShop);
	}

	// 구입 결과 처리
	protected virtual void AckFixShop(Server.ERROR_ID err)
	{
		DebugMgr.Log("AckFixShop " + err);
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EVENT_BUY, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
			LegionInfoMgr.Instance.SetAddVipPoint(_cBuyPackageInfo);
            // 결제일 경우
            if (_cBuyPackageInfo.cNeedGoods.u1Type == (byte)GoodsType.PURCHASE)
			{
                //결과창 표시
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText(_cBuyPackageInfo.sName), TextManager.Instance.GetText(_cBuyPackageInfo.sName) + TextManager.Instance.GetText("mark_event_buygoods"), null);
                if (!EventInfoMgr.Instance.dicEventReward.ContainsKey (_cBuyPackageInfo.u2ID))
                {
					EventReward _eventReward = new EventReward ();
					_eventReward.u2EventID = _cBuyPackageInfo.u2ID;
                    _eventReward.u1EventType = _cBuyPackageInfo.u1EventType;
                    _eventReward.u1RewardIndex = 0;
					_eventReward.u4RecordValue = _cBuyPackageInfo.u2KeepDay;
					EventInfoMgr.Instance.dicEventReward.Add(_cBuyPackageInfo.u2ID, _eventReward);
				} else {
					EventReward _eventReward = EventInfoMgr.Instance.dicEventReward [_cBuyPackageInfo.u2ID];
					_eventReward.u1RewardIndex = 0;
					_eventReward.u4RecordValue = _cBuyPackageInfo.u2KeepDay;
				}
			}
			else
			{
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText(_cBuyPackageInfo.sName), TextManager.Instance.GetText(_cBuyPackageInfo.sName) + TextManager.Instance.GetText("mark_event_buygoods"), DownloadFile);
                Legion.Instance.SubGoods (_cBuyPackageInfo.cNeedGoods);
			}

            // 구매이력을 증가시킨다
            if (EventInfoMgr.Instance.dicEventBuy.ContainsKey (_cBuyPackageInfo.u2ID)) 
			{
				EventBuy temp = EventInfoMgr.Instance.dicEventBuy [_cBuyPackageInfo.u2ID];
				temp.u1EventBuyCnt++;
			}

            AssetMgr.Instance.InitDownloadList();
            // 품목 별 보상 추가
            if (_cBuyPackageInfo.u1EventType == (Byte)PackageMenu.Promot) 
			{
				if (!_cBuyPackageInfo.cRewardOnce.IsCoupon ()) 
				{
					Legion.Instance.AddGoods (_cBuyPackageInfo.cRewardOnce);
				}
			} 
            else if(_cBuyPackageInfo.u1EventType == (Byte)PackageMenu.Character)
            {
                for (int i = 0; i < _cBuyPackageInfo.acPackageRewards.Length; i++)
                {
                    if (!_cBuyPackageInfo.acPackageRewards[i].IsCoupon())
                    {
                        Legion.Instance.AddGoods(_cBuyPackageInfo.acPackageRewards[i]);
                        if (_cBuyPackageInfo.acPackageRewards[i].u1Type == (Byte)GoodsType.CHARACTER_PACKAGE)
                        {
                            UInt16 classId = EventInfoMgr.Instance.dicClassGoods[_cBuyPackageInfo.acPackageRewards[i].u2ID].u2ClassID;
                            AssetMgr.Instance.AddDivisionDownload(1, classId);
                        }
                    }
                }
            }
            else if (_cBuyPackageInfo.u1EventType == (Byte)PackageMenu.Equip)
            {
                for (int i = 0; i < _cBuyPackageInfo.acPackageRewards.Length; i++)
                {
                    if (_cBuyPackageInfo.acPackageRewards[i].u1Type == 0)
                        continue;

                    // 장비 라면 장비 셋팅하고 아니라면 타입에 맞도록 셋팅
                    if (_cBuyPackageInfo.acPackageRewards[i].u1Type == (Byte)GoodsType.EQUIP_GOODS)
                    {
                        ClassGoodsEquipInfo ginfo = EventInfoMgr.Instance.dicClassGoodsEquip[_cBuyPackageInfo.acPackageRewards[i].u2ID];
                        EquipmentInfo equipInfo = EquipmentInfoMgr.Instance.GetInfo(ginfo.u2Equip);
                        UInt16 slotNum = Legion.Instance.cInventory.AddEquipment(0, 0, ginfo.u2Equip, ginfo.u2Level, 0, ginfo.au1Skills, ginfo.au4Stats, 0, "",
                            Legion.Instance.sName, equipInfo.u2ModelID, true, ginfo.u1SmithingLevel, 0, 0, ginfo.u1StarLevel);

                        Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.EquipLevel, ginfo.u2Equip, (Byte)equipInfo.u1PosID, 0, 0, ginfo.u2Level);
                        Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.GetEquip, ginfo.u2Equip, (Byte)equipInfo.u1PosID, ginfo.u1SmithingLevel, (Byte)equipInfo.u2ClassID, 1);
                    }
                    else
                    {
                        Legion.Instance.AddGoods(_cBuyPackageInfo.acPackageRewards[i]);
                    }

                }
            }
            else if (_cBuyPackageInfo.u1EventType != (Byte)PackageMenu.Month) 
			{
				for(int i=0; i< _cBuyPackageInfo.acPackageRewards.Length; i++)
				{
					if (!_cBuyPackageInfo.acPackageRewards[i].IsCoupon()) 
					{
						Legion.Instance.AddGoods (_cBuyPackageInfo.acPackageRewards[i]);
					}
				}
			}
            
			for(int i=0; i<EventInfoMgr.Instance.lstEventGoodsBuy.Count; i++)
			{
				EventGoodsBuy temp = EventInfoMgr.Instance.lstEventGoodsBuy [i];
				UInt32[] tempStat = new UInt32[Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType*2];

				if(temp.u4Stat != null)
				{
					for(int j=0; j< Server.ConstDef.EquipStatPointType; j++)
						tempStat[j + Server.ConstDef.EquipStatPointType] = temp.u4Stat[j];
				}

				if (temp.u1ItemType == (Byte)GoodsType.EQUIP) 
					Legion.Instance.cInventory.AddEquipment (0, 0, temp.u2ItemID, temp.u2Level, 0, temp.u1SkillSlot, tempStat, 0, "", "", temp.u2ModelID, true, temp.u1SmithingLevel, 0, 0, temp.u1Completeness);
				else 
					Legion.Instance.AddGoods (new Goods (temp.u1ItemType, temp.u2ItemID, temp.u4Count));
			}

			LobbyScene lScene = Scene.GetCurrent() as LobbyScene;
			if(lScene != null)
				lScene._eventPanel.CheckAlarm ();

			BuyRefiesh();
		}
	}

	protected virtual void BuyRefiesh()
	{
    }

    protected void DownloadFile(object[] obj)
    {
        AssetMgr.Instance.ShowDownLoadPopup();
    }

    protected virtual void SetBeforeBuyNotice()
    {
        if (_txtBeforeBuyNotice == null)
            return;

        StringBuilder tempString = new StringBuilder();
        tempString.Append(TextManager.Instance.GetText("mark_goods_warning_1")).Append("\n");
        tempString.Append(TextManager.Instance.GetText("mark_goods_warning_2")).Append("\n");
        tempString.Append(TextManager.Instance.GetText("mark_goods_warning_3"));

        _txtBeforeBuyNotice.text = tempString.ToString();
    }
}
