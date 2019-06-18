using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using IgaworksUnityAOS;

public class PackagePopup : MonoBehaviour {

	public enum PackageMenu
	{
		Recommend = 6,
		Limit = 7,
		Promot = 3,
		Month = 5,
		Character = 8,
        EquipPack = 9
	}

	public GameObject PrefRecomSlot;
	public GameObject PrefLimitSlot;
	public GameObject PrefPromotSlot;
	public GameObject PrefMonthSlot;

	public GameObject RecomObj;
	public RectTransform RecomSlotParent;
	public GameObject LimitObj;
	public RectTransform LimitSlotParent;
	public GameObject PromotObj;
	public RectTransform PromotSlotParent;
	public GameObject MonthObj;
	public RectTransform MonthSlotParent;

	public Text textTitle;
	public Text textPeriod;
	public GameObject RecomLimitBtn;
	public Toggle[] toggleBtns;
	public GameObject BuyBtn;
	public Text BuyText;

	public Text _textNotice;	// 안내문

	PackageMenu ePopupType = PackageMenu.Recommend;

	List<EventPackageInfo> lstEventList;

	void Start() 
	{
		#if UNITY_ONESTORE
		ShopInfoMgr.Instance.SettingInAppOneStore();
		#else
		ShopInfoMgr.Instance.SettingInApp();
		#endif
	}

	public void SetPopup(PackageMenu etype){
		//BuyPackage (EventInfoMgr.Instance.GetPackageInfo (9365));
		ePopupType = etype;

		//lstEventList = new List<EventPackageInfo> ();
		lstEventList = EventInfoMgr.Instance.GetEventListByEventType ((byte)etype);

		if (lstEventList.Count == 0)
			return;

		switch (etype) {
		case PackageMenu.Recommend:
			
			textTitle.text = TextManager.Instance.GetText("title_event_recomm_goods");//  "추천 상품";
			if (lstEventList.Count > 0) {
				if (EventInfoMgr.Instance.dicEventBuy.ContainsKey (lstEventList [0].u2ID)) {
					if (EventInfoMgr.Instance.dicEventBuy [lstEventList [0].u2ID].u8BuyEnd == 0)
						textPeriod.text = TextManager.Instance.GetText("mark_event_package_desc");
					else
						textPeriod.text = EventInfoMgr.Instance.dicEventBuy[lstEventList[0].u2ID].dtBuyBegin.ToString("yyyy.MM.dd")+" ~ "
							+EventInfoMgr.Instance.dicEventBuy[lstEventList[0].u2ID].dtBuyEnd.ToString("yyyy.MM.dd");
				}
			}

			RecomObj.SetActive (true);
			LimitObj.SetActive (false);
			PromotObj.SetActive (false);
			MonthObj.SetActive (false);

			RecomLimitBtn.SetActive (true);
			BuyBtn.SetActive (false);

			toggleBtns [0].GetComponent<ToggleImage> ().image.sprite = toggleBtns [0].GetComponent<ToggleImage> ().changeSprite;
			toggleBtns [1].GetComponent<ToggleImage> ().image.sprite = toggleBtns [1].GetComponent<ToggleImage> ().defaultSprite;

			if (RecomSlotParent.childCount > 0)
				return;
			
			for (int i = 0; i < lstEventList.Count; i++) {
				if (!EventInfoMgr.Instance.dicEventBuy.ContainsKey (lstEventList [i].u2ID))
					continue;

				if (!CheckSellPeriod (lstEventList [i].u2ID))
					continue;

				if (EventInfoMgr.Instance.CheckBuyPossible (lstEventList [i].u2ID) == 0)
					continue;

				GameObject slot = Instantiate (PrefRecomSlot) as GameObject;
				slot.transform.SetParent (RecomSlotParent);
				slot.transform.localScale = Vector3.one;
				slot.transform.localPosition = Vector3.zero;
				PackageSlot script = slot.GetComponent<PackageSlot> ();
				script.SetSlot(lstEventList[i]);

				// 2016. 09. 09 jy
				// 상품 구매 했어도 상품 출력하고 구매만 못하게 요청와서 조건 변경함
				//if(EventInfoMgr.Instance.CheckBuyPossible(lstEventList[i].u2ID) == 0){
				if(EventInfoMgr.Instance.CheckBuyPossible(lstEventList[i].u2ID) == 2){
					script.DisableButton ();
				}else{
					EventPackageInfo info = (lstEventList [i]);
					script.btnBuy.onClick.AddListener (() => BuyPackage (info, script));
				}
			}

			break;
		
		case PackageMenu.Limit:
			textTitle.text = TextManager.Instance.GetText("mark_event_limited");//"한정 상품";
			textPeriod.text = TextManager.Instance.GetText("mark_event_package_desc");

			RecomObj.SetActive (false);
			LimitObj.SetActive (true);
			PromotObj.SetActive (false);
			MonthObj.SetActive (false);

			RecomLimitBtn.SetActive (true);
			BuyBtn.SetActive (false);

			toggleBtns [1].GetComponent<ToggleImage> ().image.sprite = toggleBtns [1].GetComponent<ToggleImage> ().changeSprite;
			toggleBtns [0].GetComponent<ToggleImage> ().image.sprite = toggleBtns [0].GetComponent<ToggleImage> ().defaultSprite;

			if (LimitSlotParent.childCount > 0)
				return;

			for (int i = 0; i < lstEventList.Count; i++) {
				if (!EventInfoMgr.Instance.dicEventBuy.ContainsKey (lstEventList [i].u2ID))
					continue;

				if (!CheckSellPeriod (lstEventList [i].u2ID))
					continue;

				if (EventInfoMgr.Instance.CheckBuyPossible (lstEventList [i].u2ID) == 0)
					continue;
				
				GameObject slot = Instantiate (PrefLimitSlot) as GameObject;
				slot.transform.SetParent (LimitSlotParent);
				slot.transform.localScale = Vector3.one;
				slot.transform.localPosition = Vector3.zero;
				PackageSlot script = slot.GetComponent<PackageSlot> ();
				script.SetSlot(lstEventList[i]);
				// 2016. 09. 09 jy
				// 상품 구매 했어도 상품 출력하고 구매만 목하게 요청와서 조건 변경함
				//if(EventInfoMgr.Instance.CheckBuyPossible(lstEventList[i].u2ID) == 0){
				if(EventInfoMgr.Instance.CheckBuyPossible(lstEventList[i].u2ID) == 2){
					script.DisableButton ();
				}else{
					EventPackageInfo info = (lstEventList [i]);
					script.btnBuy.onClick.AddListener (() => BuyPackage (info, script));
				}
			}
			break;

		case PackageMenu.Promot:
			textTitle.text = TextManager.Instance.GetText ("mark_event_grow");//"육성 패키지";
			textPeriod.text = "";

			RecomObj.SetActive (false);
			LimitObj.SetActive (false);
			PromotObj.SetActive (true);
			MonthObj.SetActive (false);

			RecomLimitBtn.SetActive (false);
			if (EventInfoMgr.Instance.dicEventReward.ContainsKey (lstEventList [0].u2ID)) {
				textPeriod.text = TextManager.Instance.GetText ("mark_event_package");//"패키지 보유중";
				BuyBtn.SetActive (false);
			} else {
				if (EventInfoMgr.Instance.dicEventBuy.ContainsKey (lstEventList [0].u2ID)) {
					BuyBtn.SetActive (true);
					if (lstEventList [0].cNeedGoods.u1Type == (byte)GoodsType.PURCHASE) {
						//#if UNITY_ANDROID
						//BuyText.text = TextManager.Instance.GetText("btn_event_grow_buy")+" ￦"+lstEventList[0].cNeedGoods.u4Count;
						//#else
						BuyText.text = /*TextManager.Instance.GetText("btn_event_grow_buy") +*/"$" + lstEventList [0].iOSPrice;
						//#endif
					} else {
						BuyText.text = /*TextManager.Instance.GetText("btn_event_grow_buy")+" "++*/lstEventList [0].cNeedGoods.u4Count + " " + Legion.Instance.GetConsumeString (lstEventList [0].cNeedGoods.u1Type);
					}
				} else {
					BuyBtn.SetActive (false);
					return;
				}
			}

			if (PromotSlotParent.childCount > 0)
				return;

			UInt32 totalRewardCount = 0;
			for (int i = 0; i < lstEventList [0].acPackageRewards.Length; i++) {
				totalRewardCount += lstEventList [0].acPackageRewards [i].u4Count;
			}
			_textNotice.text = string.Format(TextManager.Instance.GetText("mark_event_grow_desc"), totalRewardCount , lstEventList[0].cRewardOnce.u4Count);

			Byte idx = 0;
			for (uint i = lstEventList [0].u4PensionNeedMin; i <= lstEventList [0].u4PensionNeedMax; i += lstEventList [0].u4PensionNeedTurm) {
				GameObject slot = Instantiate (PrefPromotSlot) as GameObject;
				slot.transform.SetParent (PromotSlotParent);
				slot.transform.localScale = Vector3.one;
				slot.transform.localPosition = Vector3.zero;
				PackageSlot script = slot.GetComponent<PackageSlot> ();
				script.SetSlot (lstEventList [0], idx, i);

				if (EventInfoMgr.Instance.dicEventReward.ContainsKey (lstEventList [0].u2ID)) 
				{
					if (Legion.Instance.CheckEnoughGoods (new Goods (lstEventList [0].u1PensionNeedType, lstEventList [0].u1PensionNeedID, i))) 
					{
						if (EventInfoMgr.Instance.CheckRewardPossibleIndex (lstEventList [0].u2ID, idx)) {
							script.rewardIndicator.SetActive (true);
							script.rewardedMark.SetActive (false);
							EventPackageInfo info = (lstEventList [0]);
							script.btnBuy.onClick.AddListener (() => GetReward (info, script));
						} else {
							script.rewardIndicator.SetActive (false);
							script.rewardedMark.SetActive (true);
						}
					} else {
						script.rewardIndicator.SetActive (false);
						script.rewardedMark.SetActive (false);
						script.DisableButton ();
					}
				} else {
					script.rewardIndicator.SetActive (false);
					script.rewardedMark.SetActive (false);
					script.DisableButton ();
				}
				idx++;
			}
			break;

		case PackageMenu.Month:
			textTitle.text = TextManager.Instance.GetText("mark_event_30day");//"30일 패키지";
			textPeriod.text = "";

			RecomObj.SetActive (false);
			LimitObj.SetActive (false);
			PromotObj.SetActive (false);
			MonthObj.SetActive (true);

			RecomLimitBtn.SetActive (false);

			if (EventInfoMgr.Instance.dicEventReward.ContainsKey (lstEventList [0].u2ID)) {
				textPeriod.text = (lstEventList [0].u2KeepDay + 1 - EventInfoMgr.Instance.dicEventReward[lstEventList [0].u2ID].u1RewardIndex)+TextManager.Instance.GetText("mark_event_day_remain");//"일 남음";
			}

			// 2016. 09. 09 jy
			// 상품 구매 했어도 상품 출력하고 구매만 목하게 요청와서 조건 변경함
			//if(EventInfoMgr.Instance.CheckBuyPossible(lstEventList[0].u2ID) > 0){
			if(EventInfoMgr.Instance.CheckBuyPossible(lstEventList[0].u2ID) == 1){
				BuyBtn.SetActive (true);
				if (lstEventList[0].cNeedGoods.u1Type == (byte)GoodsType.PURCHASE) {
					//#if UNITY_ANDROID
					//BuyText.text = TextManager.Instance.GetText("btn_event_grow_buy")+" ￦"+lstEventList[0].cNeedGoods.u4Count;
					//#else
					BuyText.text = /*TextManager.Instance.GetText("btn_event_grow_buy")+*/"$" + lstEventList[0].iOSPrice;
					//#endif
				} else {
					BuyText.text = /*TextManager.Instance.GetText("btn_event_grow_buy")+" "+*/lstEventList[0].cNeedGoods.u4Count+" "+TextManager.Instance.GetText (Legion.Instance.GetConsumeString (lstEventList[0].cNeedGoods.u1Type));
				}
			}else{
				BuyBtn.SetActive (false);
			}

			if (MonthSlotParent.childCount > 0)
				return;

			if (!EventInfoMgr.Instance.dicEventBuy.ContainsKey (lstEventList [0].u2ID))
				return;

			for (int i = 0; i < lstEventList.Count; i++) {
				GameObject slot = Instantiate (PrefMonthSlot) as GameObject;
				slot.transform.SetParent (MonthSlotParent);
				slot.transform.localScale = Vector3.one;
				slot.transform.localPosition = Vector3.zero;
				slot.GetComponent<PackageSlot>().SetSlot(lstEventList[i]);
			}
			break;
		}
	}

	bool CheckSellPeriod(UInt16 id){
		if (EventInfoMgr.Instance.dicEventBuy [id].u8BuyBegin == 0) {
			if (EventInfoMgr.Instance.dicEventBuy [id].u8BuyEnd != 0) {
				if (EventInfoMgr.Instance.dicEventBuy [id].dtBuyEnd < Legion.Instance.ServerTime)//DateTime.Now)
					return false;
			}
		}
		
		if (EventInfoMgr.Instance.dicEventBuy [id].u8BuyEnd == 0) {
			if (EventInfoMgr.Instance.dicEventBuy [id].u8BuyBegin != 0) {
				if (EventInfoMgr.Instance.dicEventBuy [id].dtBuyBegin > Legion.Instance.ServerTime)//DateTime.Now)
					return false;
			}
		}

		return true;
	}

	public void SetRecom(){
		if (ePopupType == PackageMenu.Recommend)
			return;
		
		SetPopup (PackageMenu.Recommend);
	}

	public void SetLimit(){
		if (ePopupType == PackageMenu.Limit)
			return;
		
		SetPopup (PackageMenu.Limit);
	}

	public void OnClickBuy(){
		switch (ePopupType) {
		case PackageMenu.Promot:
			if (lstEventList.Count <= 0)
				return;

			BuyPackage (lstEventList [0], null);
			break;

		case PackageMenu.Month:
			if (lstEventList.Count <= 0)
				return;

			BuyPackage (lstEventList [0], null);
			break;
		}
	}

	private Byte selectedType;
	private UInt16 selectedID;
	private PackageSlot selectedSlot;
	public void BuyPackage(EventPackageInfo info, PackageSlot slot){
		if (info.u1EventType == (Byte)PackageMenu.Recommend || info.u1EventType == (Byte)PackageMenu.Limit) {
			// 
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

			if (Legion.Instance.CheckEnoughGoods (info.cNeedMinPeriod)) {
				if (info.cNeedMaxPeriod.u1Type == (Byte)GoodsType.LEVEL) {
					if (Legion.Instance.TopLevel > info.cNeedMaxPeriod.u4Count) {
						PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_event_buy_wrong") , TextManager.Instance.GetText("mark_event_over_level"), null);
						return;
					}
				}
			} else {
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
		selectedType = info.cNeedGoods.u1Type;
		selectedID = info.u2ID;
		selectedSlot = slot;

		//결제일 경우
		if(info.cNeedGoods.u1Type == (byte)GoodsType.PURCHASE)
		{
			#if UNITY_EDITOR
			RequestBuy();

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
	private void OnPurchaseFlowFinishedAction (UM_PurchaseResult result) {

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
	private void RequestBuy(string receipt = "", string txid = "")
	{
		PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.RequestEventGoodsBuy(selectedID, receipt, txid, AckFixShop);
	}

	// 구입 결과 처리
	private void AckFixShop(Server.ERROR_ID err)
	{
		DebugMgr.Log("AckFixShop " + err);

		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EVENT_BUY, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
			if (selectedSlot == null)
				BuyBtn.SetActive (false);
			else
				selectedSlot.DisableButton ();

			EventPackageInfo info = EventInfoMgr.Instance.GetPackageInfo(selectedID);
            LegionInfoMgr.Instance.SetAddVipPoint(info);
			// 결제일 경우
			if(selectedType == (byte)GoodsType.PURCHASE)
			{
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText(info.sName) , TextManager.Instance.GetText(info.sName)+ TextManager.Instance.GetText("mark_event_buygoods"), null);
				if (!EventInfoMgr.Instance.dicEventReward.ContainsKey (selectedID)) {
					EventReward _eventReward = new EventReward ();
					_eventReward.u2EventID = selectedID;
					_eventReward.u1RewardIndex = 0;
					_eventReward.u4RecordValue = info.u2KeepDay;

					EventInfoMgr.Instance.dicEventReward.Add (selectedID, _eventReward);
				} else {
					EventReward _eventReward = EventInfoMgr.Instance.dicEventReward [selectedID];
					_eventReward.u1RewardIndex = 0;
					_eventReward.u4RecordValue = info.u2KeepDay;
				}

				if (info.u1EventType == (Byte)PackageMenu.Promot) {
					for (int i = 0; i < PromotSlotParent.childCount; i++) {
						if (i > PromotSlotParent.childCount - 1)
							i--;
						Destroy (PromotSlotParent.GetChild (i).gameObject);

						if (PromotSlotParent.childCount == 0)
							break;
					}
				}

				OnClickClose ();
			}
			else
			{
				//결과창 표시
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText(info.sName) , TextManager.Instance.GetText(info.sName)+TextManager.Instance.GetText("mark_event_buygoods"), null);
				Legion.Instance.SubGoods (info.cNeedGoods);
			}

			if (EventInfoMgr.Instance.dicEventBuy.ContainsKey (selectedID)) {
				EventBuy temp = EventInfoMgr.Instance.dicEventBuy [selectedID];
				temp.u1EventBuyCnt++;
			}

			if (info.u1EventType == (Byte)PackageMenu.Month) {

			} else if (info.u1EventType == (Byte)PackageMenu.Promot) {
				if (!info.cRewardOnce.IsCoupon ()) {
					Legion.Instance.AddGoods (info.cRewardOnce);
				}
			} else {
				for(int i=0; i<info.acPackageRewards.Length; i++){
					if (!info.acPackageRewards[i].IsCoupon()) {
						Legion.Instance.AddGoods (info.acPackageRewards[i]);
					}
				}
			}

			for(int i=0; i<EventInfoMgr.Instance.lstEventGoodsBuy.Count; i++){
				EventGoodsBuy temp = EventInfoMgr.Instance.lstEventGoodsBuy [i];
				UInt32[] tempStat = new UInt32[Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType*2];

				if(temp.u4Stat != null){
					for(int j=0; j< Server.ConstDef.EquipStatPointType; j++)
						tempStat[j + Server.ConstDef.EquipStatPointType] = temp.u4Stat[j];
				}

				if (temp.u1ItemType == (Byte)GoodsType.EQUIP) {
					Legion.Instance.cInventory.AddEquipment (0, 0, temp.u2ItemID, temp.u2Level, 0, temp.u1SkillSlot, tempStat, 0, "", "", temp.u2ModelID, true, temp.u1SmithingLevel, 0, 0, temp.u1Completeness);
				} else {
					Legion.Instance.AddGoods (new Goods (temp.u1ItemType, temp.u2ItemID, temp.u4Count));
				}
			}

			LobbyScene lScene = Scene.GetCurrent() as LobbyScene;

			if(lScene != null){
				lScene._eventPanel.CheckAlarm ();
			}
		}
	}

	public void GetReward(EventPackageInfo info, PackageSlot slot)
	{
		if(Legion.Instance.CheckGoodsLimitExcessx(info.acPackageRewards[slot.promotIndex].u1Type) == true)
		{
			Legion.Instance.ShowGoodsOverMessage(info.acPackageRewards[slot.promotIndex].u1Type);
			return;
		}
		PopupManager.Instance.ShowLoadingPopup (1);
		selectedID = info.u2ID;
		selectedSlot = slot;
		Server.ServerMgr.Instance.RequestEventGoodsReward (info.u2ID, rewardResult);
	}

	// 보상 결과 처리
	private void rewardResult(Server.ERROR_ID err)
	{
		DebugMgr.Log("rewardResult " + err);

		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EVENT_REWARD, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
			selectedSlot.SetRewarded ();

			if (EventInfoMgr.Instance.dicEventReward.ContainsKey (selectedID)) {
				EventInfoMgr.Instance.dicEventReward.Remove(selectedID);

				EventReward _eventReward = new EventReward();
				_eventReward.u2EventID = selectedID;
				_eventReward.u1RewardIndex = EventInfoMgr.Instance.sEventGoodsReward.u1LastRewardIndex;
				_eventReward.u4RecordValue = EventInfoMgr.Instance.sEventGoodsReward.u4RecordValue;
				EventInfoMgr.Instance.dicEventReward.Add (selectedID, _eventReward);
			} else {
				EventReward _eventReward = new EventReward();
				_eventReward.u2EventID = selectedID;
				_eventReward.u1RewardIndex = EventInfoMgr.Instance.sEventGoodsReward.u1LastRewardIndex;
				_eventReward.u4RecordValue = EventInfoMgr.Instance.sEventGoodsReward.u4RecordValue;
				EventInfoMgr.Instance.dicEventReward.Add (selectedID, _eventReward);
			}

			EventPackageInfo cPackage = EventInfoMgr.Instance.GetPackageInfo (selectedID);
			if (selectedSlot.promotIndex > -1) {
				Legion.Instance.AddGoods (cPackage.acPackageRewards [selectedSlot.promotIndex]);
				string reward = "";
				if (cPackage.acPackageRewards [selectedSlot.promotIndex].u1Type == (byte)GoodsType.CONSUME) {
					reward = TextManager.Instance.GetText (ItemInfoMgr.Instance.GetConsumableItemInfo (cPackage.acPackageRewards [selectedSlot.promotIndex].u2ID).sName) + " " + cPackage.acPackageRewards [selectedSlot.promotIndex].u4Count;
				} else {
					reward = Legion.Instance.GetConsumeString (cPackage.acPackageRewards [selectedSlot.promotIndex].u1Type) + " " + cPackage.acPackageRewards [selectedSlot.promotIndex].u4Count;
				}
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("btn_vip_reward") , reward +"\n"+ TextManager.Instance.GetText("mark_event_get_goods"), null);
			} else {
				Legion.Instance.AddGoods (cPackage.acPackageRewards);
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("btn_vip_reward") , GetRewardString()+ TextManager.Instance.GetText("mark_event_get_goods"), null);
			}
		}
	}

	public void OnClickClose(){
		gameObject.SetActive (false);
	}

	string GetRewardString(){
		int cnt = 0;
		string reward = "";
		EventPackageInfo info = EventInfoMgr.Instance.GetPackageInfo (selectedID);
		for (int i = 0; i < info.acPackageRewards.Length; i++) {
			if (info.acPackageRewards [i].u1Type != 0) {
				if (cnt > 0) reward += "\n";
				if (info.acPackageRewards [i].u1Type == (byte)GoodsType.CONSUME) {
					reward += TextManager.Instance.GetText (ItemInfoMgr.Instance.GetConsumableItemInfo (info.acPackageRewards [i].u2ID).sName) + " " + info.acPackageRewards [i].u4Count;
				} else {
					reward += Legion.Instance.GetConsumeString (info.acPackageRewards [i].u1Type) + " " + info.acPackageRewards [i].u4Count;
				}
				cnt++;
			}
		}

		return reward;
	}
}
