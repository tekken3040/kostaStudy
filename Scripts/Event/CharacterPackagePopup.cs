using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using IgaworksUnityAOS;

public class CharacterPackagePopup : MonoBehaviour {

	bool bAdv;
	UInt16 u2PackageID;

	int idx = 0;

	public GameObject checkMass;
	public Image bg;
	public Text Price;

	List<EventPackageInfo> lstPackage;

	public void InitPopup(List<EventPackageInfo> lst, bool bAd){
		lstPackage = new List<EventPackageInfo> ();
		lstPackage = lst;
		bAdv = bAd;
        PopupManager.Instance.AddPopup(this.gameObject, Close);
		StartCoroutine ("StartLoadBG");	}

	IEnumerator StartLoadBG(){
		SetPopup();
		WWW www = new WWW (PopupManager.Instance.GetImageURL() + "AD/" + TextManager.Instance.eLanguage.ToString() + "/Advertising_" + lstPackage [idx].u2ID+".png");

		yield return www;

		bg.sprite = Sprite.Create(www.texture, new Rect(0,0,www.texture.width,www.texture.height), new Vector2(0,0));
	}

	void SetPopup(){
		//bg.sprite = AtlasMgr.Instance.GetSprite("Sprites/Event/Advertising_"+lstPackage[idx].u2ID+".Advertising_"+lstPackage[idx].u2ID);

		if (lstPackage[idx].cNeedGoods.u1Type == (byte)GoodsType.PURCHASE) {
			//#if UNITY_ANDROID
			//Price.text ="￦"+lstPackage[idx].cNeedGoods.u4Count;
			//#else
			Price.text = "$" + lstPackage[idx].iOSPrice;
			//#endif
		} else {
			Price.text = lstPackage[idx].cNeedGoods.u4Count + " " + Legion.Instance.GetConsumeString (lstPackage[idx].cNeedGoods.u1Type);
		}

		if (!bAdv) {
			checkMass.SetActive (false);
		}
	}

	public void OnClickCheckBox(){
		PlayerPrefs.SetString ("AD"+lstPackage[idx].u2ID, Legion.Instance.ServerTime.ToShortDateString());//DateTime.Now.Date.ToShortDateString ());

		Close ();
	}

	public void OnClickClose(){
		Close ();
	}

	void Close(){
		if (idx < lstPackage.Count-1) {
			idx++;
			StopCoroutine ("StartLoadBG");
			StartCoroutine ("StartLoadBG");
			return;
		}

		lstPackage.Clear ();
		lstPackage = null;
        PopupManager.Instance.RemovePopup(this.gameObject);

		//Legion.Instance.ShowCafeOnce ();
		Destroy (gameObject);
	}

	public void OnClickBuy(){
		buyIdx = idx;
		BuyPopup ();
	}

	void BuyPopup(){
		// 2016. 10. 24 jy 
		// 구매 보상으로 재화가 오버시 예외처리
		if(Legion.Instance.CheckGoodsLimitExcessx(lstPackage[buyIdx].cRewardOnce.u1Type) == true)
		{
			Legion.Instance.ShowGoodsOverMessage(lstPackage[buyIdx].cRewardOnce.u1Type);
			return;
		}
		PopupManager.Instance.ShowYesNoPopup (TextManager.Instance.GetText("popup_title_buy_class"), TextManager.Instance.GetText("popup_desc_buy_class"), SettingInApp, null);
	}

	int buyIdx = 0;

	//인앱 결제 정보 초기화
	void SettingInApp(object[] obj)
	{
		if (!UM_InAppPurchaseManager.Instance.IsInited) {
			UM_InAppPurchaseManager.OnBillingConnectFinishedAction += OnBillingConnectFinishedAction;            
			UM_InAppPurchaseManager.Instance.Init ();
			PopupManager.Instance.ShowLoadingPopup (1);
		} else {
			BuyPackage ();
		}
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

		BuyPackage ();
		PopupManager.Instance.CloseLoadingPopup();
	}  

	public void BuyPackage(){

		//결제일 경우
		if(lstPackage[buyIdx].cNeedGoods.u1Type == (byte)GoodsType.PURCHASE)
		{
			#if UNITY_EDITOR
			RequestBuy();

#else
			if(UM_InAppPurchaseManager.Instance.IsInited)
			{
			PopupManager.Instance.ShowLoadingPopup(1);
			UM_InAppPurchaseManager.OnPurchaseFlowFinishedAction += OnPurchaseFlowFinishedAction;
			UM_InAppPurchaseManager.Instance.Purchase(lstPackage[buyIdx].u2ID.ToString());
			}
			else
			{
			DebugMgr.LogError("InAppPurchase Init Failed");
			}
#endif
        }
		else
		{
			if(!Legion.Instance.CheckEnoughGoods(lstPackage[buyIdx].cNeedGoods))
			{
				PopupManager.Instance.ShowChargePopup(lstPackage[buyIdx].cNeedGoods.u1Type);
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
		Server.ServerMgr.Instance.RequestEventGoodsBuy(lstPackage[buyIdx].u2ID, receipt, txid, AckFixShop);
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
			EventPackageInfo info = lstPackage [buyIdx];
			LegionInfoMgr.Instance.SetAddVipPoint(info);
			// 결제일 경우
			if(info.cNeedGoods.u1Type == (byte)GoodsType.PURCHASE)
			{
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText(info.sName) , TextManager.Instance.GetText(info.sName)+ TextManager.Instance.GetText("mark_event_buygoods"), null);
				if (!EventInfoMgr.Instance.dicEventReward.ContainsKey (lstPackage[buyIdx].u2ID)) {
					EventReward _eventReward = new EventReward ();
					_eventReward.u2EventID = lstPackage[buyIdx].u2ID;
                    _eventReward.u1EventType = lstPackage[buyIdx].u1EventType;
                    _eventReward.u1RewardIndex = 0;
					_eventReward.u4RecordValue = info.u2KeepDay;

					EventInfoMgr.Instance.dicEventReward.Add (lstPackage[buyIdx].u2ID, _eventReward);
				} else {
					EventReward _eventReward = EventInfoMgr.Instance.dicEventReward [lstPackage[buyIdx].u2ID];
					_eventReward.u1RewardIndex = 0;
					_eventReward.u4RecordValue = info.u2KeepDay;
				}
			}
			else
			{
				//결과창 표시
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText(info.sName) , TextManager.Instance.GetText(info.sName)+TextManager.Instance.GetText("mark_event_buygoods"), DownloadFile);
				Legion.Instance.SubGoods (info.cNeedGoods);
			}

			if (EventInfoMgr.Instance.dicEventBuy.ContainsKey (lstPackage[buyIdx].u2ID)) {
				EventBuy temp = EventInfoMgr.Instance.dicEventBuy [lstPackage[buyIdx].u2ID];
				temp.u1EventBuyCnt++;
			}

			AssetMgr.Instance.InitDownloadList ();

			for(int i=0; i<info.acPackageRewards.Length; i++){
				if (!info.acPackageRewards[i].IsCoupon()) {
					Legion.Instance.AddGoods (info.acPackageRewards[i]);
					if (info.acPackageRewards [i].u1Type == (Byte)GoodsType.CHARACTER_PACKAGE) {
						UInt16 classId = EventInfoMgr.Instance.dicClassGoods [info.acPackageRewards [i].u2ID].u2ClassID;
						AssetMgr.Instance.AddDivisionDownload (1, classId);
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
		}

		Destroy (gameObject);
		Legion.Instance.ShowCafeOnce ();
	}

	void DownloadFile(object[] obj)
	{
		AssetMgr.Instance.ShowDownLoadPopup ();
	}
}
