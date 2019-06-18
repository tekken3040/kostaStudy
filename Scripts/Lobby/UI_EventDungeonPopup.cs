using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class UI_EventDungeonPopup : MonoBehaviour {
	public GameObject ExchangeList;
	public GameObject ScrollExchangeTabs;
	public Transform ScrollExchangeTabsGrid;
	public GameObject ScrollExchangeList;
	public Transform ScrollExchangeListGrid;
	public GameObject StageList;
	public Transform StageListGrid;
	public GameObject StageRewardList;//AdvRewardList
	public Transform StageRewardListGrid;

	public GameObject ShopGoSlot;
	public GameObject ShopScrollTab;
	public GameObject ShopScrollSlot;
	public GameObject EventStageSlot;
	public GameObject EventStageRewardSlot;

	public Image imgPopupBG;
	public Image imgPopupSubBG;

	public GameObject AdventoMass;
	public Image imgAdventoTitle;
	public Text txtAdventoDesc;
	public Text txtAdventoDate;

	public GameObject AdventoStartBtn;
	public GameObject AdventoExchangeBtn;

	public GameObject EtcEventMass;
	public Image imgEtcEventTitle;
	public Text txtEtcEventDesc;
	public Text txtEtcEventDate;

	public GameObject EtcEventStartBtn;
    public GameObject objEventUserGoodsIcon;
    public Text txtEventUseGoodsCount;

	public Image imgIllust;

	public Text txtHaveGoods;
	public Text txtSubscription;

	public Color[] colorEventText;
	public Color[] colorEventTitleStart;
	public Color[] colorEventTitleEnd;

    enum UIType{
		Stage = 1,
		ShopGo = 2,
		ShopScroll = 3
	}

	EventDungeonShopInfo shopInfo;
	List<EventDungeonStageInfo> eStageInfo = new List<EventDungeonStageInfo>();

	int curTab = 0;

	public void Set(EventDungeonShopInfo info){
		shopInfo = info;

		if (shopInfo.sIllustPath == "0")
			imgIllust.enabled = false;
		else imgIllust.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Tutorial/" + shopInfo.sIllustPath + "."+shopInfo.sIllustPath);

		string dateText = "";

		if (EventInfoMgr.Instance.dicEventReward [shopInfo.u2EventID].u8EventBegin == 0 && EventInfoMgr.Instance.dicEventReward [shopInfo.u2EventID].u8EventEnd == 0) {
			dateText = "";
		} else {
			string date = "";
			if(EventInfoMgr.Instance.dicEventReward [shopInfo.u2EventID].u8EventBegin != 0){
				date += EventInfoMgr.Instance.dicEventReward [shopInfo.u2EventID].dtEventBegin.ToString ("yyyy.MM.dd");
			}
			date += "~";
			if (EventInfoMgr.Instance.dicEventReward [shopInfo.u2EventID].u8EventEnd != 0) {
				date += EventInfoMgr.Instance.dicEventReward [shopInfo.u2EventID].dtEventEnd.ToString ("yyyy.MM.dd");
			} else {
				date += TextManager.Instance.GetText ("event_dungeon_open_notice");
			}
			dateText = TextManager.Instance.GetText ("event_dungeon_open_time") + ": " + date;
		}

		dateText += EventInfoMgr.Instance.GetEventDate (shopInfo.u2EventID);

		switch ((UIType)info.u1UIType) {
		case UIType.Stage:
			AdventoMass.SetActive (true);
			EtcEventMass.SetActive (false);
			imgAdventoTitle.sprite = AtlasMgr.Instance.GetSprite ("Sprites/" + TextManager.Instance.GetImagePath () + "_02." + shopInfo.sTitle);
			imgAdventoTitle.SetNativeSize ();
			txtAdventoDesc.text = TextManager.Instance.GetText (shopInfo.sDescription);
			txtAdventoDesc.GetComponent<Outline> ().effectColor = colorEventText [shopInfo.u1UIType - 1];
			txtAdventoDate.text = dateText;

			imgPopupSubBG.enabled = true;
			imgPopupSubBG.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Event/" + info.sBgImagePath);
			imgPopupBG.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Event/event_01.event_DungeonBg");
			imgPopupBG.GetComponent<RectTransform> ().sizeDelta = new Vector2 (1132, 689);
			SetShopGoList ();
			break;
		case UIType.ShopGo:
			EtcEventMass.SetActive (true);
			AdventoMass.SetActive (false);
			imgEtcEventTitle.sprite = AtlasMgr.Instance.GetSprite ("Sprites/" + TextManager.Instance.GetImagePath () + "_02." + shopInfo.sTitle);
			imgEtcEventTitle.SetNativeSize ();
			txtEtcEventDesc.text = TextManager.Instance.GetText (shopInfo.sDescription);
			txtEtcEventDesc.GetComponent<Outline> ().effectColor = colorEventText [shopInfo.u1UIType - 1];
			txtEtcEventDate.text = dateText;

			imgPopupSubBG.enabled = false;
			imgPopupBG.sprite = AtlasMgr.Instance.GetSprite("Sprites/Event/event_popup.EquipPackage_BG");
			imgPopupBG.GetComponent<RectTransform> ().sizeDelta = new Vector2 (1100, 660);
            SetShopGoList ();
			break;
		case UIType.ShopScroll:
			eStageInfo = EventInfoMgr.Instance.lstDungeonStage.FindAll (cs => cs.u2EventID == shopInfo.u2EventID);

			EtcEventMass.SetActive (true);
			AdventoMass.SetActive (false);
			imgEtcEventTitle.sprite = AtlasMgr.Instance.GetSprite ("Sprites/" + TextManager.Instance.GetImagePath () + "_02." + shopInfo.sTitle);
			imgEtcEventTitle.SetNativeSize ();
			txtEtcEventDesc.text = TextManager.Instance.GetText (shopInfo.sDescription);
			txtEtcEventDesc.GetComponent<Outline> ().effectColor = colorEventText [shopInfo.u1UIType - 1];
			txtEtcEventDate.text = dateText;

			imgPopupSubBG.enabled = false;
			imgPopupBG.sprite = AtlasMgr.Instance.GetSprite("Sprites/Event/event_popup.EquipPackage_BG");
			imgPopupBG.GetComponent<RectTransform> ().sizeDelta = new Vector2 (1100, 660);
            SetShopScrollList ();
			break;
		}
    }

	void SetStageList(){
		for (int i = 0; i < StageListGrid.transform.childCount; i++) {
			Destroy(StageListGrid.transform.GetChild(i).gameObject);
		}

		ExchangeList.SetActive (false);
		ScrollExchangeList.SetActive (false);
		ScrollExchangeTabs.SetActive (false);
		StageList.SetActive (true);

		AdventoStartBtn.SetActive (false);
		EtcEventStartBtn.SetActive (false);

		StageRewardList.SetActive(false);

		if ((UIType)shopInfo.u1UIType == UIType.Stage) {
			AdventoExchangeBtn.SetActive (true);
		}

		EventDungeonStageInfo eInfo = EventInfoMgr.Instance.lstDungeonStage.Find (cs => cs.u2EventID == shopInfo.u2EventID);

		if (eInfo == null) {
			DebugMgr.LogError ("Have Not Stage");
			return;
		}

		for (int i = 0; i < eInfo.au2StageID.Length; i++) {
			UInt16 stageID = eInfo.au2StageID [i];
			int idx = i;
			if (stageID > 0) {
				GameObject tSlot = Instantiate (EventStageSlot) as GameObject;
				tSlot.transform.SetParent (StageListGrid);
				tSlot.transform.localScale = Vector3.one;
				tSlot.transform.localPosition = Vector3.zero;
				tSlot.AddComponent<UI_EventDungeonStageSlot> ().SetSlot (eInfo, idx);
				tSlot.GetComponent<Button>().onClick.AddListener(() => { OnClickStageBtn(stageID, idx); }); 
			}
		}
	}

	void SetShopGoList(){
		txtSubscription.text = "";

		if (shopInfo.u1UIType == (Byte)UIType.ShopGo) {
			for (int i = 0; i < ExchangeList.transform.childCount; i++) {
				Destroy (ExchangeList.transform.GetChild (i).gameObject);
			}

			StageList.SetActive (false);
			ScrollExchangeList.SetActive (false);
			ScrollExchangeTabs.SetActive (false);
			ExchangeList.SetActive (true);

			StageRewardList.SetActive(false);

			AdventoStartBtn.SetActive (true);
			EtcEventStartBtn.SetActive (true);
            EventDungeonStageInfo stageInfo = EventInfoMgr.Instance.lstDungeonStage.Find(cs => cs.u2EventID == shopInfo.u2EventID);
            if (stageInfo != null)
                txtEventUseGoodsCount.text = stageInfo.acConsumeItem[0].u4Count.ToString();
            else
                objEventUserGoodsIcon.SetActive(false);

            AdventoExchangeBtn.SetActive (false);

			for (int i = 0; i < shopInfo.au2ShopID.Count; i++) {
				UInt16 shopId = shopInfo.au2ShopID [i];
				GameObject tSlot = Instantiate (ShopGoSlot) as GameObject;
				tSlot.transform.SetParent (ExchangeList.transform);
				tSlot.transform.localScale = Vector3.one;
				tSlot.transform.localPosition = Vector3.zero;
				tSlot.AddComponent<UI_EventDungeonShopSlot> ().SetSlot (shopInfo.u2EventID, shopId);
				tSlot.GetComponent<Button> ().onClick.AddListener (() => {
					OnClickExchangeBtn (shopId);
				}); 
			}

			EventMarbleItem needGoods = EventInfoMgr.Instance.dicMarbleGoods.First (cs => cs.Value.u2EventID == shopInfo.u2EventID).Value;

			uint count = EventInfoMgr.Instance.GetEventItemCount (needGoods.u2ItemID);

			txtHaveGoods.text = TextManager.Instance.GetText (needGoods.sName) + " " + TextManager.Instance.GetText ("popup_item_info_amount") + ": " + count;
		}else if(shopInfo.u1UIType == (Byte)UIType.Stage) {
			for (int i = 0; i < StageRewardListGrid.childCount; i++) {
				Destroy (StageRewardListGrid.GetChild (i).gameObject);
			}

			StageList.SetActive (false);
			ScrollExchangeList.SetActive (false);
			ScrollExchangeTabs.SetActive (false);
			ExchangeList.SetActive (false);

			StageRewardList.SetActive(true);

			AdventoStartBtn.SetActive (true);
			EtcEventStartBtn.SetActive (true);

            AdventoExchangeBtn.SetActive (false);

			for (int i = 0; i < shopInfo.au2ShopID.Count; i++) {
				UInt16 shopId = shopInfo.au2ShopID [i];
				GameObject tSlot = Instantiate (EventStageRewardSlot) as GameObject;
				tSlot.transform.SetParent (StageRewardListGrid);
				tSlot.transform.localScale = Vector3.one;
				tSlot.transform.localPosition = Vector3.zero;
				tSlot.AddComponent<UI_EventDungeonStageRewardSlot> ().SetSlot (shopId);
				tSlot.transform.FindChild("Btn").GetComponent<Button> ().onClick.AddListener (() => {
					OnClickExchangeBtn (shopId);
				}); 
			}

			EventMarbleItem needGoods = EventInfoMgr.Instance.dicMarbleGoods.First (cs => cs.Value.u2EventID == shopInfo.u2EventID).Value;

			txtHaveGoods.text = "";
		}
	}

	void SetShopScrollList(){
		if (curTab > 0) {
			OnClickTab (true, curTab, true);
			return;
		}

		txtSubscription.text = TextManager.Instance.GetText(shopInfo.sSubscription);

		for (int i = 0; i < ScrollExchangeTabsGrid.transform.childCount; i++) {
			Destroy(ScrollExchangeTabsGrid.GetChild(i).gameObject);
		}
		for (int i = 0; i < ScrollExchangeListGrid.transform.childCount; i++) {
			Destroy(ScrollExchangeListGrid.GetChild(i).gameObject);
		}

		StageList.SetActive (false);
		ExchangeList.SetActive (false);
		ScrollExchangeList.SetActive (true);
		ScrollExchangeTabs.SetActive (true);
		AdventoStartBtn.SetActive (false);
		EtcEventStartBtn.SetActive (false);
		AdventoExchangeBtn.SetActive (false);

		StageRewardList.SetActive(false);

		curTab = 0;
		for (int i = 0; i < shopInfo.au1TabCount.Length; i++) {
			if (shopInfo.au1TabCount [i] > 0) {
				int idx = i;
				GameObject tTab = Instantiate (ShopScrollTab) as GameObject;
				tTab.transform.SetParent (ScrollExchangeTabsGrid);
				tTab.transform.localScale = Vector3.one;
				tTab.transform.localPosition = Vector3.zero;
				tTab.AddComponent<UI_EventDungeonShopTab> ().SetTab (idx, shopInfo.asTabName [i]);
				tTab.GetComponent<Toggle> ().onValueChanged.AddListener ((x) => {
					OnClickTab (x, idx);
				}); 
				tTab.GetComponent<Toggle> ().group = ScrollExchangeTabsGrid.GetComponent<ToggleGroup> ();
				if (i == curTab)
					tTab.GetComponent<Toggle> ().isOn = true;
			} else {
				break;
			}
		}

		int dungeonIdx = 0;

		for (int i = 0; i < shopInfo.au2ShopID.Count; i++) {
			if (i < shopInfo.au1TabCount [0]) {
				UInt16 shopId = shopInfo.au2ShopID [i];

				GameObject tSlot = Instantiate (ShopScrollSlot) as GameObject;
				tSlot.transform.SetParent (ScrollExchangeListGrid);
				tSlot.transform.localScale = Vector3.one;
				tSlot.transform.localPosition = Vector3.zero;
				if (shopId < 50000) {
					tSlot.AddComponent<UI_EventDungeonShopSlotBig> ().SetSlot (shopId, shopInfo.asTabSlotBG [0]);
					tSlot.transform.FindChild ("Btn_Exchange").GetComponent<Button> ().onClick.AddListener (() => {
						OnClickExchangeBtn (shopId);
					});
				} else {
					if (dungeonIdx >= eStageInfo.Count)
						continue;

					bool bReward = false;

					if (shopInfo.au2ShopID.Count > (i + eStageInfo.Count)) {
						UInt16 rewardShopId = shopInfo.au2ShopID [i + eStageInfo.Count];
						ShopGoodInfo rewardShopInfo = ShopInfoMgr.Instance.dicShopGoodData[rewardShopId];

						if (rewardShopInfo.cShopItem.u1Type == (Byte)GoodsType.EQUIP_COUPON) {
							if (EventInfoMgr.Instance.GetEventItemCount (rewardShopInfo.cBuyGoods.u2ID) >= rewardShopInfo.cBuyGoods.u4Count) {
								bReward = true;

								tSlot.transform.SetParent (ScrollExchangeListGrid);
								tSlot.transform.localScale = Vector3.one;
								tSlot.transform.localPosition = Vector3.zero;
								tSlot.AddComponent<UI_EventDungeonShopSlotBig> ().SetSlot (rewardShopId, shopInfo.asTabSlotBG [1]);
								tSlot.transform.FindChild ("Btn_Exchange").GetComponent<Button> ().onClick.AddListener (() => {
									OnClickExchangeBtn (rewardShopId);
								});
							}
						}
					}

					if (!bReward) {
						int idx = dungeonIdx;
						EventDungeonStageInfo stageInfo = eStageInfo [idx];
						Goods openItem = eStageInfo [idx].cOpenItem;
						if (openItem != null) {
							tSlot.transform.SetParent (ScrollExchangeListGrid);
							tSlot.transform.localScale = Vector3.one;
							tSlot.transform.localPosition = Vector3.zero;
							tSlot.AddComponent<UI_EventDungeonShopSlotBig> ().SetSlot (openItem, stageInfo);
							tSlot.transform.FindChild ("Btn_Exchange").GetComponent<Button> ().onClick.AddListener (() => {
								OnClickUseBtn (stageInfo);
							}); 
						}
					}
				}
			} else {
				break;
			}
		}

//		EventMarbleItem needGoods = EventInfoMgr.Instance.dicMarbleGoods.First (cs => cs.Value.u2EventID == shopInfo.u2EventID).Value;

//		uint count = 0;
//		if (EventInfoMgr.Instance.dicMarbleBag.ContainsKey(needGoods.u2ItemID))
//			count = EventInfoMgr.Instance.dicMarbleBag[needGoods.u2ItemID].u4Count;
//
//		txtHaveGoods.text = TextManager.Instance.GetText(needGoods.sName)+" "+TextManager.Instance.GetText("popup_item_info_amount")+": "+count;
	}


	public void OnClickStart(){
		if (!EventInfoMgr.Instance.CheckOpen (Legion.Instance.cEvent.dicDungeonOpenInfo [shopInfo.u2EventID])) {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("event_dungeon_into"), 
				TextManager.Instance.GetText("event_dungeon_into_fail"), Server.ServerMgr.Instance.CallClear);
			return;
		}

		if ((UIType)shopInfo.u1UIType == UIType.Stage) {
			SetStageList ();
		} else {
			if (AssetMgr.Instance.CheckDivisionDownload(7,shopInfo.u2EventID)) {
				return;
			}

			EventDungeonStageInfo stageInfo = EventInfoMgr.Instance.lstDungeonStage.Find (cs => cs.u2EventID == shopInfo.u2EventID);
			if (stageInfo != null) {
				if (!Legion.Instance.CheckEnoughGoods (stageInfo.acConsumeItem [0])) {
					PopupManager.Instance.ShowChargePopup(stageInfo.acConsumeItem [0].u1Type);
					return;
				}
			}

			PopupManager.Instance.ShowLoadingPopup(1);

			Legion.Instance.u2SelectStageID = stageInfo.au2StageID[0];
			Legion.Instance.AUTOCONTINUE = false;
			Legion.Instance.SelectedDifficult = 1;
			Server.ServerMgr.Instance.StartStage (Legion.Instance.cBestCrew, StageInfoMgr.Instance.dicStageData [Legion.Instance.u2SelectStageID], Legion.Instance.SelectedDifficult, AckStartStage, false);
		}
	}

	public void OnClickExchange(){
		if ((UIType)shopInfo.u1UIType == UIType.Stage) {
			SetShopGoList ();
		}
	}

	public void OnClickStageBtn(UInt16 stageID, int diff){
		if (!EventInfoMgr.Instance.CheckOpen (Legion.Instance.cEvent.dicDungeonOpenInfo [shopInfo.u2EventID])) {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("event_dungeon_into"), 
				TextManager.Instance.GetText("event_dungeon_into_fail"), Server.ServerMgr.Instance.CallClear);
			return;
		}

		if (AssetMgr.Instance.CheckDivisionDownload(7,shopInfo.u2EventID)) {
			return;
		}

		EventDungeonStageInfo stageInfo = EventInfoMgr.Instance.lstDungeonStage.Find (cs => cs.u2EventID == shopInfo.u2EventID && cs.au2StageID [diff] == stageID);
		if (stageInfo != null) {
			if (!Legion.Instance.CheckEnoughGoods (stageInfo.acConsumeItem [diff])) {
				PopupManager.Instance.ShowChargePopup(stageInfo.acConsumeItem [diff].u1Type);
				return;
			}
		}

		PopupManager.Instance.ShowLoadingPopup(1);

		Legion.Instance.u2SelectStageID = stageID;
		Legion.Instance.AUTOCONTINUE = false;
		Legion.Instance.SelectedDifficult = 1;
		Server.ServerMgr.Instance.StartStage (Legion.Instance.cBestCrew, StageInfoMgr.Instance.dicStageData [Legion.Instance.u2SelectStageID], Legion.Instance.SelectedDifficult, AckStartStage, false);
	}

	private void AckStartStage(Server.ERROR_ID err)
	{
		DebugMgr.Log(err);

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.STAGE_START, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
			EventDungeonStageInfo stageInfo = EventInfoMgr.Instance.lstDungeonStage.Find (cs => cs.u2EventID == shopInfo.u2EventID && cs.au2StageID [0] == Legion.Instance.u2SelectStageID);
			if (stageInfo != null) {
				if(stageInfo.acConsumeItem[0].u1Type != (Byte)GoodsType.KEY) Legion.Instance.SubGoods (stageInfo.acConsumeItem [0]);
			}

			if (shopInfo.u1UIType == 1) {
				Legion.Instance.bAdventoStage = 0;
			} else {
				Legion.Instance.bAdventoStage = 1;
			}

			StartCoroutine(ChangeScene());
		}

		PopupManager.Instance.CloseLoadingPopup();
	}

	private IEnumerator ChangeScene()
	{
		FadeEffectMgr.Instance.FadeOut(1f);
		yield return new WaitForSeconds(1f);
		AssetMgr.Instance.SceneLoad("Battle");
	}

	public void OnClickTab(bool isOn, int index, bool bForce = false){
		if (!bForce) {
			if (!isOn)
				return;

			if (curTab == index)
				return;
		}
		
		curTab = index;

		for (int i = 0; i < ScrollExchangeListGrid.transform.childCount; i++) {
			Destroy(ScrollExchangeListGrid.GetChild(i).gameObject);
		}
			
		int startIdx = 0;

		for (int i = 0; i < shopInfo.au1TabCount.Length; i++) {
			if (shopInfo.au1TabCount [i] > 0) {
				if(i < index) startIdx = shopInfo.au1TabCount [i];
			}
		}
			
		int dungeonIdx = 0;

		for (int i = startIdx; i < shopInfo.au2ShopID.Count; i++) {
			if (i < shopInfo.au1TabCount [index]) {
				UInt16 shopId = shopInfo.au2ShopID [i];

				if (shopId < 50000) {
					GameObject tSlot = Instantiate (ShopScrollSlot) as GameObject;
					tSlot.transform.SetParent (ScrollExchangeListGrid);
					tSlot.transform.localScale = Vector3.one;
					tSlot.transform.localPosition = Vector3.zero;
					tSlot.AddComponent<UI_EventDungeonShopSlotBig> ().SetSlot (shopId, shopInfo.asTabSlotBG [index]);
					tSlot.transform.FindChild ("Btn_Exchange").GetComponent<Button> ().onClick.AddListener (() => {
						OnClickExchangeBtn (shopId);
					});
				} else {
					if (dungeonIdx >= eStageInfo.Count)
						continue;

					bool bReward = false;

					if (shopInfo.au2ShopID.Count > (i + eStageInfo.Count)) {
						UInt16 rewardShopId = shopInfo.au2ShopID [i + eStageInfo.Count];
						ShopGoodInfo rewardShopInfo = ShopInfoMgr.Instance.dicShopGoodData[rewardShopId];

						if (rewardShopInfo.cShopItem.u1Type == (Byte)GoodsType.EQUIP_COUPON) {
							if (EventInfoMgr.Instance.GetEventItemCount (rewardShopInfo.cBuyGoods.u2ID) >= rewardShopInfo.cBuyGoods.u4Count) {
								bReward = true;

								GameObject tSlot = Instantiate (ShopScrollSlot) as GameObject;
								tSlot.transform.SetParent (ScrollExchangeListGrid);
								tSlot.transform.localScale = Vector3.one;
								tSlot.transform.localPosition = Vector3.zero;
								tSlot.AddComponent<UI_EventDungeonShopSlotBig> ().SetSlot (rewardShopId, shopInfo.asTabSlotBG [index+1]);
								tSlot.transform.FindChild ("Btn_Exchange").GetComponent<Button> ().onClick.AddListener (() => {
									OnClickExchangeBtn (rewardShopId);
								});
							}
						}
					}

					if (!bReward) {
						int idx = dungeonIdx;
						EventDungeonStageInfo stageInfo = eStageInfo [idx];
						Goods openItem = eStageInfo [idx].cOpenItem;
						if (openItem != null) {
							GameObject tSlot = Instantiate (ShopScrollSlot) as GameObject;
							tSlot.transform.SetParent (ScrollExchangeListGrid);
							tSlot.transform.localScale = Vector3.one;
							tSlot.transform.localPosition = Vector3.zero;
							tSlot.AddComponent<UI_EventDungeonShopSlotBig> ().SetSlot (openItem, stageInfo);
							tSlot.transform.FindChild ("Btn_Exchange").GetComponent<Button> ().onClick.AddListener (() => {
								OnClickUseBtn (stageInfo);
							}); 
						}
					}
					dungeonIdx++;
				}
			} else {
				break;
			}
		}
	}

	UInt16 selectedShopID = 0;

	public void OnClickExchangeBtn(UInt16 shopID){
		ShopGoodInfo tInfo = ShopInfoMgr.Instance.getDeepCopyShopGoodInfo(shopID);
		if (!Legion.Instance.CheckEnoughGoods (tInfo.cBuyGoods)) {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_nocost"), Legion.Instance.GetGoodsName(tInfo.cBuyGoods)+"\n"+TextManager.Instance.GetText("popup_desc_nocost"), null);
			return;
		}

		PopupManager.Instance.ShowLoadingPopup(1);
		selectedShopID = shopID;
		Server.ServerMgr.Instance.ShopFixShop (shopID, 0, 255, 0, "", "", AckFixShop);
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
			ShopGoodInfo shopGoodInfo = ShopInfoMgr.Instance.getDeepCopyShopGoodInfo(selectedShopID);

			Legion.Instance.SubGoods(shopGoodInfo.cBuyGoods);
			if (shopGoodInfo.cShopItem.u1Type != (Byte)GoodsType.EVENT_ITEM) {
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_reward_chap"), TextManager.Instance.GetText ("event_get_reward_post"), null);
			} else {
				Legion.Instance.AddGoods (shopGoodInfo.cShopItem);
				LegionInfoMgr.Instance.SetAddVipPoint (shopGoodInfo);

				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_reward_chap"), 
					string.Format(TextManager.Instance.GetText("event_item_dungeon_get"), Legion.Instance.GetGoodsName(shopGoodInfo.cShopItem), shopGoodInfo.cShopItem.u4Count), null);
			}

			if(shopInfo.u1UIType == (Byte)UIType.Stage || shopInfo.u1UIType == (Byte)UIType.ShopGo) SetShopGoList ();
			else if(shopInfo.u1UIType == (Byte)UIType.ShopScroll) SetShopScrollList ();
		}
	}
		
	EventDungeonStageInfo selectedStage;
	public void OnClickUseBtn(EventDungeonStageInfo selectStg){
		if (Legion.Instance.cEvent.CheckOpenStage (selectStg.u2EventID, selectStg.au2StageID [0])) {
			PopupManager.Instance.ShowLoadingPopup (1);
			Legion.Instance.u2SelectStageID = selectStg.au2StageID [0];
			Legion.Instance.cEvent.selectedOpenEventID = selectStg.u2EventID;
			Legion.Instance.AUTOCONTINUE = false;
			Legion.Instance.SelectedDifficult = 1;
			Server.ServerMgr.Instance.StartStage (Legion.Instance.cBestCrew, StageInfoMgr.Instance.dicStageData [Legion.Instance.u2SelectStageID], Legion.Instance.SelectedDifficult, AckStartStage, false);
		} else {
			if (!Legion.Instance.CheckEnoughGoods (selectStg.cOpenItem)) {
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_nocost"), Legion.Instance.GetGoodsName(selectStg.cOpenItem)+"\n"+TextManager.Instance.GetText("popup_desc_nocost"), null);
				return;
			}
			selectedStage = selectStg;
			Server.ServerMgr.Instance.RequestEventItemUse (selectedStage.cOpenItem.u2ID, AckItemUse);
		}
	}

	public void ReOpen(object[] param)
	{
		selectedStage = (EventDungeonStageInfo)param[0];
		PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.RequestEventItemUse (selectedStage.cOpenItem.u2ID, AckItemUse);
	}

	// 사용 결과 처리
	private void AckItemUse(Server.ERROR_ID err)
	{
		DebugMgr.Log("AckItemUse " + err);

		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup("", TextManager.Instance.GetError(Server.MSGs.EVENT_ITEMUSE, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
			Legion.Instance.SubGoods(selectedStage.cOpenItem);
			Legion.Instance.cEvent.AddOpenStage (selectedStage.u2EventID, selectedStage.au2StageID [0], 0, 0);
			SetShopScrollList ();

			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("deselect"), 
				TextManager.Instance.GetText(StageInfoMgr.Instance.dicStageData[selectedStage.au2StageID[0]].sName)+"\n"+TextManager.Instance.GetText("stage")+" "+TextManager.Instance.GetText("deselect"), null);
		}
	}

	public void OnClickClose(){
		PopupManager.Instance.RemovePopup (gameObject);
		Destroy (gameObject);
	}
}


public class UI_EventDungeonShopSlot : MonoBehaviour {
	Image imgRewardIcon;
	Text txtRewardCount;
	Text txtNeedGoods;
	Text txtDesc;

	Image imgSoldOut;

	ShopGoodInfo shopInfo;

	public void SetSlot(UInt16 eventID, UInt16 shopID){
        imgRewardIcon = transform.FindChild ("Icon").GetComponent<Image> ();
		txtRewardCount = transform.FindChild ("Title").GetComponent<Text> ();
		txtNeedGoods = transform.FindChild ("Desc").GetComponent<Text> ();
		txtDesc = transform.FindChild ("Sub").GetComponent<Text> ();
		imgSoldOut = transform.FindChild ("SoldOut").GetComponent<Image> ();

		if (!ShopInfoMgr.Instance.dicShopGoodData.ContainsKey (shopID)) {
			DebugMgr.LogError ("Shop Id None");
			return;
		}

		shopInfo = ShopInfoMgr.Instance.dicShopGoodData[shopID];
		imgRewardIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Shop/" + shopInfo.imagePath);
		txtRewardCount.text = "x "+shopInfo.cShopItem.u4Count.ToString();
		txtNeedGoods.text = Legion.Instance.GetGoodsName (shopInfo.cBuyGoods) + " " + shopInfo.cBuyGoods.u4Count;

		if (shopInfo.cShopItem.u1Type == (Byte)GoodsType.EQUIP_COUPON) {
			txtRewardCount.rectTransform.anchoredPosition = new Vector2(35f, 0f);
			txtDesc.enabled = true;
			txtDesc.text = TextManager.Instance.GetText (shopInfo.title);
		}

		if (shopInfo.u1BuyOverlap == 3) {
			if (Legion.Instance.cEvent.GetBuyCount (shopID) >= shopInfo.u2BuyRestriction) {
				imgSoldOut.enabled = true;
				GetComponent<Button> ().interactable = false;
			}
		}
	}
}

public class UI_EventDungeonShopTab : MonoBehaviour {
	Text txtTabName;

	public void SetTab(int idx, string tabname){
		txtTabName = transform.FindChild ("Text").GetComponent<Text> ();
		txtTabName.text = TextManager.Instance.GetText(tabname);
	}
}

public class UI_EventDungeonShopSlotBig : MonoBehaviour {
	GameObject objExchangeMass;
	Image imgNeedGoodsIcon;
	Text txtNeedGoodsCount;

	GameObject objHaveGoods;
	Image imgHaveGoodsIcon;
	Text txtHaveGoods;

	Image imgRewardGoodsIcon;
	Text txtRewardGoodsCount;

	Image imgRewardLockIcon;

	Text txtRewardName;
	Text txtRewardName2;

	Image imgExchangeBtn;
	Text txtExchangeBtn;

	Text txtRewardDescForCoupon;
	Image imgNeedGoodsIconForCoupon;

	ShopGoodInfo shopInfo;

	public void SetSlot(UInt16 shopID, string bgPath){
		objExchangeMass = transform.FindChild ("ExchangeMass").gameObject;
		imgNeedGoodsIcon = objExchangeMass.transform.FindChild("ExchangeMat").GetComponent<Image> ();
		txtNeedGoodsCount = objExchangeMass.transform.FindChild("ExchangeMat").FindChild ("Count").GetComponent<Text> ();

		objHaveGoods = transform.FindChild("HaveGoods").gameObject;
		imgHaveGoodsIcon = objHaveGoods.transform.FindChild("Icon").GetComponent<Image> ();
		txtHaveGoods = objHaveGoods.transform.FindChild("Text").GetComponent<Text> ();

		imgRewardGoodsIcon = objExchangeMass.transform.FindChild("ExchangeReward").GetComponent<Image> ();
		txtRewardGoodsCount = objExchangeMass.transform.FindChild("ExchangeReward").FindChild ("Count").GetComponent<Text> ();

		imgRewardLockIcon = objExchangeMass.transform.FindChild("Lock").GetComponent<Image> ();

		txtRewardName = transform.FindChild ("Title").GetComponent<Text> ();
		txtRewardName2 = transform.FindChild ("Title2").GetComponent<Text> ();

		imgExchangeBtn = transform.FindChild("Btn_Exchange").GetComponent<Image> ();
		txtExchangeBtn = transform.FindChild("Btn_Exchange").FindChild ("Text").GetComponent<Text> ();

		txtRewardDescForCoupon = transform.FindChild ("Desc2").GetComponent<Text> ();
		imgNeedGoodsIconForCoupon = transform.FindChild ("Icon").GetComponent<Image> ();

		if (!ShopInfoMgr.Instance.dicShopGoodData.ContainsKey (shopID)) {
			DebugMgr.LogError ("Shop Id None");
			return;
		}

		shopInfo = ShopInfoMgr.Instance.dicShopGoodData[shopID];
		uint count = EventInfoMgr.Instance.GetEventItemCount (shopInfo.cBuyGoods.u2ID);

		if (count < shopInfo.cBuyGoods.u4Count) {
			AtlasMgr.Instance.SetGrayScale(imgExchangeBtn);
		}

		txtExchangeBtn.text = TextManager.Instance.GetText ("event_dungeon_trade");

		if (shopInfo.cShopItem.u1Type == (Byte)GoodsType.EQUIP_COUPON) {
			objExchangeMass.SetActive (false);
			objHaveGoods.SetActive (false);
			txtRewardDescForCoupon.enabled = true;
			imgNeedGoodsIconForCoupon.enabled = true;
			txtRewardName.enabled = false;

			txtRewardName2.text = Legion.Instance.GetGoodsName (shopInfo.cShopItem);

			txtRewardDescForCoupon.text = TextManager.Instance.GetText (shopInfo.title);

			imgNeedGoodsIconForCoupon.sprite = AtlasMgr.Instance.GetGoodsIcon (shopInfo.cShopItem);
		} else {
			txtRewardName.text = Legion.Instance.GetGoodsName (shopInfo.cShopItem);
			imgRewardGoodsIcon.sprite = AtlasMgr.Instance.GetGoodsIcon (shopInfo.cShopItem);
			txtRewardGoodsCount.text = "x" + shopInfo.cShopItem.u4Count.ToString ();

			imgHaveGoodsIcon.sprite = AtlasMgr.Instance.GetGoodsIcon (shopInfo.cBuyGoods);
			txtHaveGoods.text = count+"/"+shopInfo.cBuyGoods.u4Count.ToString ();

			imgNeedGoodsIcon.sprite = AtlasMgr.Instance.GetGoodsIcon (shopInfo.cBuyGoods);
			txtNeedGoodsCount.text = "x"+shopInfo.cBuyGoods.u4Count.ToString ();
		}
	}

	public void SetSlot(Goods good, EventDungeonStageInfo stage){
		objExchangeMass = transform.FindChild ("ExchangeMass").gameObject;
		imgNeedGoodsIcon = objExchangeMass.transform.FindChild("ExchangeMat").GetComponent<Image> ();
		txtNeedGoodsCount = objExchangeMass.transform.FindChild("ExchangeMat").FindChild ("Count").GetComponent<Text> ();

		objHaveGoods = transform.FindChild("HaveGoods").gameObject;
		imgHaveGoodsIcon = objHaveGoods.transform.FindChild("Icon").GetComponent<Image> ();
		txtHaveGoods = objHaveGoods.transform.FindChild("Text").GetComponent<Text> ();

		imgRewardGoodsIcon = objExchangeMass.transform.FindChild("ExchangeReward").GetComponent<Image> ();
		txtRewardGoodsCount = objExchangeMass.transform.FindChild("ExchangeReward").FindChild ("Count").GetComponent<Text> ();

		imgRewardLockIcon = objExchangeMass.transform.FindChild("Lock").GetComponent<Image> ();

		txtRewardName = transform.FindChild ("Title").GetComponent<Text> ();
		txtRewardName2 = transform.FindChild ("Title2").GetComponent<Text> ();

		imgExchangeBtn = transform.FindChild("Btn_Exchange").GetComponent<Image> ();
		txtExchangeBtn = transform.FindChild("Btn_Exchange").FindChild ("Text").GetComponent<Text> ();

		txtRewardDescForCoupon = transform.FindChild ("Desc2").GetComponent<Text> ();
		imgNeedGoodsIconForCoupon = transform.FindChild ("Icon").GetComponent<Image> ();

		txtRewardName.text = TextManager.Instance.GetText (StageInfoMgr.Instance.dicStageData[stage.au2StageID[0]].sName);

		uint count = EventInfoMgr.Instance.GetEventItemCount (stage.cOpenItem.u2ID);

		if (Legion.Instance.cEvent.CheckOpenStage (stage.u2EventID, stage.au2StageID [0])) {
			objExchangeMass.SetActive (false);
			imgNeedGoodsIconForCoupon.enabled = true;
			imgRewardLockIcon.enabled = false;

			imgHaveGoodsIcon.sprite = AtlasMgr.Instance.GetGoodsIcon (stage.cOpenItem);
			txtHaveGoods.text = count+"/"+stage.cOpenItem.u4Count.ToString ();

			StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[stage.au2StageID[0]];

			imgNeedGoodsIconForCoupon.sprite =  AtlasMgr.Instance.GetSprite("Sprites/Campaign/" + stageInfo.stageMiniIconPath + "_on");
			imgNeedGoodsIconForCoupon.SetNativeSize ();

			txtRewardName.text = TextManager.Instance.GetText (stageInfo.sName);

			txtExchangeBtn.text = TextManager.Instance.GetText ("event_find_exchange_key");

			AtlasMgr.Instance.SetDefaultShader(imgExchangeBtn);
		} else {
			imgRewardLockIcon.enabled = true;

			imgHaveGoodsIcon.sprite = AtlasMgr.Instance.GetGoodsIcon (stage.cOpenItem);
			txtHaveGoods.text = count+"/"+stage.cOpenItem.u4Count.ToString ();

			imgNeedGoodsIcon.sprite = AtlasMgr.Instance.GetGoodsIcon (stage.cOpenItem);
			txtNeedGoodsCount.text = "x"+stage.cOpenItem.u4Count.ToString ();

			txtExchangeBtn.text = TextManager.Instance.GetText ("event_dungeon_open");

			StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[stage.au2StageID[0]];

			txtRewardName.text = TextManager.Instance.GetText (stageInfo.sName);

			imgRewardGoodsIcon.sprite =  AtlasMgr.Instance.GetSprite("Sprites/Campaign/" + stageInfo.stageMiniIconPath + "_on");
			imgRewardGoodsIcon.SetNativeSize ();
			txtRewardGoodsCount.text = "";

			if (count < good.u4Count) {
				AtlasMgr.Instance.SetGrayScale(imgExchangeBtn);
			}
		}
	}
}

public class UI_EventDungeonStageSlot : MonoBehaviour
{
    Image imgBG;
    Image imgNeedGoods;
    Text txtNeedGoodsCnt;
    Text txtDifficult;
    Text txtRecomLevel;

    EventDungeonStageInfo dungeonInfo;

    public void SetSlot(EventDungeonStageInfo dgInfo, int idx)
    {
        dungeonInfo = dgInfo;

        UInt16 stageID = dgInfo.au2StageID[idx];

        imgBG = transform.GetComponent<Image>();
        imgNeedGoods = transform.FindChild("KeyIcon").GetComponent<Image>();
        txtNeedGoodsCnt = transform.FindChild("Key").GetComponent<Text>();
        txtDifficult = transform.FindChild("Title").GetComponent<Text>();
        txtRecomLevel = transform.FindChild("Desc").GetComponent<Text>();

        if (!StageInfoMgr.Instance.dicStageData.ContainsKey(stageID))
        {
            DebugMgr.LogError("Stage Id None");
            return;
        }
        StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[stageID];

        int bgNum = idx + 1;

        if (bgNum > 3)
            bgNum = 3;

        imgBG.sprite = AtlasMgr.Instance.GetSprite("Sprites/Event/event_01.event_Dungeon_Stage" + bgNum.ToString("00"));
        imgNeedGoods.sprite = AtlasMgr.Instance.GetGoodsIcon(dungeonInfo.acConsumeItem[idx]);

        txtNeedGoodsCnt.text = dungeonInfo.acConsumeItem[idx].u4Count.ToString();
        txtDifficult.text = TextManager.Instance.GetText(StageInfoMgr.Instance.dicStageData[stageID].sName);

        Color endGradientColor;
        if (bgNum == 1)
            endGradientColor = new Color32(50, 120, 200, 255);
        else if(bgNum == 2)
            endGradientColor = new Color32(200, 100, 200, 255);
        else
            endGradientColor = new Color32(255, 0, 0, 255);

        txtDifficult.GetComponent<Gradient>().EndColor = endGradientColor;
        txtRecomLevel.text = TextManager.Instance.GetText("popup_stage_mark_lv") + " " + StageInfoMgr.Instance.dicStageData[stageID].arrRecommandLevel[0].ToString();
    }
}
public class UI_EventDungeonStageRewardSlot : MonoBehaviour {
	Image imgSlotBG;
	Image imgEquipIcon;
	Image imgExchangeBtn;
	Text txtReward;
	Text txtNeedGoods;

	ShopGoodInfo shopInfo;

	public void SetSlot(UInt16 shopID){
		imgSlotBG = transform.GetComponent<Image> ();
		imgEquipIcon = transform.FindChild ("Icon").GetComponent<Image> ();
		imgExchangeBtn = transform.FindChild ("Btn").GetComponent<Image> ();
		txtReward = transform.FindChild ("Title").GetComponent<Text> ();
		txtNeedGoods = transform.FindChild ("Goods").GetComponent<Text> ();

		if (!ShopInfoMgr.Instance.dicShopGoodData.ContainsKey (shopID)) {
			DebugMgr.LogError ("Shop Id None");
			return;
		}
		shopInfo = ShopInfoMgr.Instance.dicShopGoodData[shopID];

		if (shopInfo.cShopItem.u1Type == 0) {
			txtReward.text = TextManager.Instance.GetText (shopInfo.title);
			txtNeedGoods.text = TextManager.Instance.GetText (shopInfo.gachaCount);

			imgEquipIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Event/event_01.event_Adv_RewardIcon3");
			imgEquipIcon.SetNativeSize ();

			imgExchangeBtn.enabled = false;
			imgExchangeBtn.gameObject.SetActive (false);
		} else {
			Byte tier = Convert.ToByte (shopInfo.itemLevel);
			Byte ele = Convert.ToByte (shopInfo.itemGrade);

			txtReward.text = TextManager.Instance.GetText (shopInfo.title);
			txtNeedGoods.text = Legion.Instance.GetGoodsName (shopInfo.cBuyGoods) + "\n" + EventInfoMgr.Instance.GetEventItemCount (shopInfo.cBuyGoods.u2ID) + "/" + shopInfo.cBuyGoods.u4Count;

			if (ele == 1)
				imgEquipIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Event/event_01.event_Adv_RewardIcon2");

			imgEquipIcon.SetNativeSize ();

			if (EventInfoMgr.Instance.GetEventItemCount (shopInfo.cBuyGoods.u2ID) >= shopInfo.cBuyGoods.u4Count) {
				imgExchangeBtn.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Event/event_01.event_Adv_Goods_On");
			}
		}
	}
}