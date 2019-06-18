using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class UpbringingRewardPopup : BaseEventPopup 
{
	public Text _txtInstantBuyReward;	// 즉시 구매 보상 정보
	public Text _txtPackageNotice;		// 패키지 흥보 문구
	public Text _txtPackagePrice;		// 패키지 금액
	public Button _btnPackageBuy;		// 패키지 구매 버튼

	protected int _nSelectIdx;
	List<UpbringingRewardSlot> rewerdSlotList = new List<UpbringingRewardSlot>();

	public override bool SetPopup(bool isStrongPopup = false)
    {
        base.SetPopup(isStrongPopup);
        // 이미 셋팅이 되어 있다며
        if (_trSlotParent.childCount > 0)
			return true;

        Image titleImg = _titleObj.GetComponent<Image>();
        titleImg.sprite = AtlasMgr.Instance.GetSprite(string.Format("Sprites/{0}.Upbringing_title", TextManager.Instance.GetImagePath()));
        titleImg.SetNativeSize();

        List<EventPackageInfo> curEventList = EventInfoMgr.Instance.GetOpenEventListByEventType(3);
        if (curEventList.Count <= 0)
            return false;

        StringBuilder tempString = new StringBuilder();
        if (_objSlot == null)
        {
            _objSlot = AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_UpbringingRewardSlot.prefab", typeof(GameObject)) as GameObject;
        }

        // 패키지 보유 여부 확인
        bool isBuyPackage = EventInfoMgr.Instance.dicEventReward.ContainsKey (curEventList[0].u2ID);
		UInt32 totalRewardCount = 0;
		// 아이템 슬롯 셋팅
		Goods[] PackageReward = curEventList[0].acPackageRewards;
		for(int i = 0; i < PackageReward.Length; ++i)
		{
			// 아이템 타입이 0라면 슬롯을 생성하지 않는다
			if(PackageReward[i].u1Type == 0)
				continue;
			
			totalRewardCount += PackageReward[i].u4Count;
			GameObject objUpringSlot = Instantiate(_objSlot) as GameObject;
			RectTransform rtTr = objUpringSlot.GetComponent<RectTransform>();
			rtTr.SetParent(_trSlotParent);
			rtTr.anchoredPosition3D = Vector3.zero;
			rtTr.localScale = Vector3.one;

			UpbringingRewardSlot rewardSlot = rtTr.GetComponent<UpbringingRewardSlot>();
			rewardSlot.SetSlot(curEventList[0], i);

			if(isBuyPackage == true)
			{
				EventPackageInfo info = (curEventList [0]);
				rewardSlot._btnGetReward.onClick.AddListener (() => GetReward (info, rewardSlot.SlotIndex));
				// 슬롯 보상 여부 확인
				if (EventInfoMgr.Instance.CheckRewardPossibleIndex (curEventList [0].u2ID, (Byte)i)) 
				{
					if(Legion.Instance.TopLevel >= (i + 1) * curEventList[0].u4PensionNeedMin)
						rewardSlot.SetRewardDone(false);
					else
						rewardSlot.DisableButton();	
				}
				else
				{
					rewardSlot.SetRewardDone(true);
				}
			}
			else
			{
				rewardSlot.DisableButton();
			}

			rewerdSlotList.Add(rewardSlot);
		}

		string packagePrice = " ";
#if UNITY_ANDROID || UNITY_EDITOR
		if(TextManager.Instance.eLanguage == TextManager.LANGUAGE_TYPE.KOREAN)
			packagePrice = curEventList[0].cNeedGoods.u4Count.ToString();
		else
			packagePrice = curEventList[0].iOSPrice.ToString();
#elif UNITY_IOS
			packagePrice = curEventList[0].iOSPrice.ToString();
#endif

		// 패키지 구매 버튼 셋팅
		if(isBuyPackage == true)
		{
			_btnPackageBuy.interactable = false;
			_txtPackagePrice.text = TextManager.Instance.GetText ("mark_event_package");
		}
		else
		{
			_btnPackageBuy.interactable = true;

            tempString.Remove(0, tempString.Length);
            _txtPackagePrice.text = tempString.Append(TextManager.Instance.GetText("btn_event_grow_buy")).Append(" ").Append(GetEventPackagePriceString(curEventList[0])).ToString();

			EventPackageInfo info = (curEventList[0]);
			_btnPackageBuy.onClick.AddListener(() => BuyPackage(info));
		}

		// 패키지 광고글 넣기
		_txtPackageNotice.text = string.Format(TextManager.Instance.GetText("mark_event_grow_desc"), totalRewardCount, packagePrice);

		// 즉시 구매 보상 정보 셋팅
		tempString.Remove(0,tempString.Length);
		tempString.Append(Legion.Instance.GetConsumeString(curEventList[0].cRewardOnce.u1Type));
		tempString.Append(" ").Append(curEventList[0].cRewardOnce.u4Count).Append(TextManager.Instance.GetText("mark_goods_number_ea"));
		_txtInstantBuyReward.text = tempString.ToString();

        return true;
	}

	public void RefreshSlots()
	{
		for(int i = 0; i < rewerdSlotList.Count; ++i)
		{
			rewerdSlotList[i].SetRewardDone(false);
		}

	}
		
	public void GetReward(EventPackageInfo info, int slotIdx)
	{
		if(!Legion.Instance.CheckEmptyInven())
		{
			return;
		}

		if(Legion.Instance.CheckGoodsLimitExcessx(info.acPackageRewards[slotIdx].u1Type) == true)
		{
			Legion.Instance.ShowGoodsOverMessage(info.acPackageRewards[slotIdx].u1Type);
			return;
		}
		PopupManager.Instance.ShowLoadingPopup (1);
		_cBuyPackageInfo = info;
		_nSelectIdx = slotIdx;
		Server.ServerMgr.Instance.RequestEventGoodsReward (info.u2ID, RewardResult);
	}

	// 보상 결과 처리
	private void RewardResult(Server.ERROR_ID err)
	{
		//DebugMgr.Log("rewardResult " + err);

		PopupManager.Instance.CloseLoadingPopup();
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EVENT_REWARD, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
			StringBuilder tempString = new StringBuilder();
			rewerdSlotList[_nSelectIdx].SetRewardDone (true);

			if (EventInfoMgr.Instance.dicEventReward.ContainsKey (_cBuyPackageInfo.u2ID))
				EventInfoMgr.Instance.dicEventReward.Remove(_cBuyPackageInfo.u2ID);

			EventReward _eventReward = new EventReward();
			_eventReward.u2EventID = _cBuyPackageInfo.u2ID;
			_eventReward.u1RewardIndex = EventInfoMgr.Instance.sEventGoodsReward.u1LastRewardIndex;
			_eventReward.u4RecordValue = EventInfoMgr.Instance.sEventGoodsReward.u4RecordValue;
			EventInfoMgr.Instance.dicEventReward.Add (_cBuyPackageInfo.u2ID, _eventReward);

			tempString.Remove(0, tempString.Length);
			EventPackageInfo cPackage = EventInfoMgr.Instance.GetPackageInfo (_cBuyPackageInfo.u2ID);
			if (_nSelectIdx > -1) 
			{
				Legion.Instance.AddGoods (cPackage.acPackageRewards [_nSelectIdx]);
				if (cPackage.acPackageRewards [_nSelectIdx].u1Type == (byte)GoodsType.CONSUME)
				{
					tempString.Append(TextManager.Instance.GetText (ItemInfoMgr.Instance.GetConsumableItemInfo (cPackage.acPackageRewards [_nSelectIdx].u2ID).sName));
				}
				else
				{
					tempString.Append(Legion.Instance.GetConsumeString (cPackage.acPackageRewards [_nSelectIdx].u1Type));
				}
				tempString.Append(" ").Append(cPackage.acPackageRewards [_nSelectIdx].u4Count);
				tempString.Append("\n").Append(TextManager.Instance.GetText("mark_event_get_goods"));
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("btn_vip_reward"), tempString.ToString(), null);
			} 
			else 
			{
				Legion.Instance.AddGoods (cPackage.acPackageRewards);
				tempString.Append(GetRewardString()).Append (TextManager.Instance.GetText("mark_event_get_goods"));
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("btn_vip_reward") ,tempString.ToString() , null);
			}

            LobbyScene lobbyScene =  Scene.GetCurrent() as LobbyScene;
            if (lobbyScene != null)
                lobbyScene._eventPanel.CheckAlarm();
        }
	}

	protected string GetRewardString()
	{
		int cnt = 0;
		StringBuilder tempString = new StringBuilder();
		EventPackageInfo info = EventInfoMgr.Instance.GetPackageInfo ((Byte)_nSelectIdx);
		for (int i = 0; i < info.acPackageRewards.Length; i++) {
			if (info.acPackageRewards [i].u1Type != 0) {
				if (cnt > 0) 
					tempString.Append("\n");
				if (info.acPackageRewards [i].u1Type == (byte)GoodsType.CONSUME) {
					tempString.Append(TextManager.Instance.GetText (ItemInfoMgr.Instance.GetConsumableItemInfo (info.acPackageRewards [i].u2ID).sName)).Append(" ").Append(info.acPackageRewards [i].u4Count);
				} else {
					tempString.Append(Legion.Instance.GetConsumeString (info.acPackageRewards [i].u1Type)).Append(" ").Append(info.acPackageRewards [i].u4Count);
				}
				cnt++;
			}
		}

		return tempString.ToString();
	}

    protected override void BuyRefiesh()
    {
        StringBuilder tempString = new StringBuilder();
        // 패키지 보유 여부 확인
        bool isBuyPackage = EventInfoMgr.Instance.dicEventReward.ContainsKey(_cBuyPackageInfo.u2ID);
        for (int i = 0; i < rewerdSlotList.Count; ++i)
        {
            if (isBuyPackage == true)
            {
                EventPackageInfo info = (_cBuyPackageInfo);
                UpbringingRewardSlot rewardSlot = rewerdSlotList[i];
                rewerdSlotList[i]._btnGetReward.onClick.AddListener(() => GetReward(info, rewardSlot.SlotIndex));
                // 슬롯 보상 여부 확인
                if (EventInfoMgr.Instance.CheckRewardPossibleIndex(_cBuyPackageInfo.u2ID, (Byte)i))
                {
                    if (Legion.Instance.TopLevel >= (i + 1) * _cBuyPackageInfo.u4PensionNeedMin)
                        rewerdSlotList[i].SetRewardDone(false);
                    else
                        rewerdSlotList[i].DisableButton();
                }
                else
                {
                    rewerdSlotList[i].SetRewardDone(true);
                }
            }
            else
            {
                rewerdSlotList[i].DisableButton();
            }
        }

        // 패키지 구매 버튼 셋팅
        if (isBuyPackage == true)
        {
            _btnPackageBuy.interactable = false;
            _txtPackagePrice.text = TextManager.Instance.GetText("mark_event_package");
        }
    }
}
