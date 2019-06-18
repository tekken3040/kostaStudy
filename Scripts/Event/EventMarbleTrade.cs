using UnityEngine;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using Server;
using CodeStage.AntiCheat.ObscuredTypes;

public class EventMarbleTrade : MonoBehaviour
{
    public Button _CloseBtn;
    public GameObject[] completeImg;
    public Button[] TradeItemBtn;
    public Text[] _txtTradeItemNeedInfo;    // 교환에 필요한 Item 정보
    public Text[] _txtTradeItemInfo;        // 교환 Item 정보
    public Text _txtGuardianMarkCount;

    private EventMarbleGame marbleInfo;
    private int rewardIndex;
    private EventItemBuyCountInfo[] eventItemBuyInfo;

    public void SetTradeItemInfo(EventMarbleGame marbleEventInfo)
    {
        marbleInfo = marbleEventInfo;
        int tradeItemCount = marbleInfo.au2ShopID.Length;
        eventItemBuyInfo = new EventItemBuyCountInfo[tradeItemCount];

        SetGuardianMark();
        for (int i = 0; i < tradeItemCount; ++i)
        {
            if (ShopInfoMgr.Instance.dicShopGoodData.ContainsKey(marbleInfo.au2ShopID[i]) == false)
                continue;

            if (TradeItemBtn.Length <= i)
                continue;

            SetTradeItem(ShopInfoMgr.Instance.dicShopGoodData[marbleInfo.au2ShopID[i]], i);
        }
    }

    // 증표 셋팅
    private void SetGuardianMark()
    {
        UInt32 markCount = 0;
        if (ShopInfoMgr.Instance.dicShopGoodData.ContainsKey(marbleInfo.au2ShopID[0]))
        {
            UInt16 u2SymbolMarkID = ShopInfoMgr.Instance.dicShopGoodData[marbleInfo.au2ShopID[0]].cBuyGoods.u2ID;
            // 가디언 징표 갯수 텍스트 셋팅
            if (EventInfoMgr.Instance.dicMarbleBag.ContainsKey(u2SymbolMarkID))
                markCount = EventInfoMgr.Instance.dicMarbleBag[u2SymbolMarkID].u4Count;
            else
                markCount = 0;
        }
        else
            markCount = 0;
        
        _txtGuardianMarkCount.text = string.Format(TextManager.Instance.GetText("event_marble_guardiansmark_count"), markCount);
    }

    private void SetTradeItem(ShopGoodInfo shopGoodsInfo, int slotIndex)
    {
        StringBuilder tempString = new StringBuilder();
        // 필요 아이템 셋팅
        tempString.Append(Legion.Instance.GetGoodsName(shopGoodsInfo.cBuyGoods));
        tempString.Append(" ").Append(shopGoodsInfo.cBuyGoods.u4Count).Append(TextManager.Instance.GetText("mark_goods_number_ea"));
        _txtTradeItemNeedInfo[slotIndex].text = tempString.ToString();

        tempString.Remove(0, tempString.Length);
        _txtTradeItemInfo[slotIndex].text = TextManager.Instance.GetText(shopGoodsInfo.title);
        // 구매한 내역이 있는지 확인
        bool isBuy = IsEventItemBuy(shopGoodsInfo, slotIndex);
        ItemSlotEnable(slotIndex, isBuy);
    }

    private void ItemSlotEnable(int index, bool isEnable)
    {
        completeImg[index].SetActive(isEnable);
        TradeItemBtn[index].interactable = !isEnable;
    }

    private bool IsEventItemBuy(ShopGoodInfo shopGoodsInfo, int index)
    {
        if (Legion.Instance.cEvent.lstItemBuyHistory.Count != 0)
        {
            if (EventInfoMgr.Instance.dicMarbleBag.ContainsKey(shopGoodsInfo.cBuyGoods.u2ID) == true)
            {
                List<EventItemBuyCountInfo> itemBuyHistory = Legion.Instance.cEvent.lstItemBuyHistory;
                int historyCount = itemBuyHistory.Count;

                for (int i = 0; i < historyCount; ++i)
                {
                    if (itemBuyHistory[i].u2ShopGroupID != shopGoodsInfo.u2GroupIndex)
                        continue;

                    if (itemBuyHistory[i].u2ShopID != shopGoodsInfo.u2ID)
                        continue;

                    eventItemBuyInfo[index] = itemBuyHistory[i];
                    if (itemBuyHistory[i].u4BuyCount >= shopGoodsInfo.u2BuyRestriction)
                        return true;
                }
            }
        }
        return false;
    }
    
    public void OnClickGuadianReward(int index)
    {
        ShopGoodInfo shopGoodsInfo = ShopInfoMgr.Instance.dicShopGoodData[marbleInfo.au2ShopID[index]];
        if (shopGoodsInfo == null)
            return;

        if (EventInfoMgr.Instance.dicMarbleBag.ContainsKey(shopGoodsInfo.cBuyGoods.u2ID))
        {
            rewardIndex = index;
            Goods tradeGoods = EventInfoMgr.Instance.dicMarbleBag[shopGoodsInfo.cBuyGoods.u2ID];
            // 구매 가능 횟수보다 크거나 동일하면 이미 구매했기 때문에 리턴
            if (eventItemBuyInfo[index] != null)
            {
                if (eventItemBuyInfo[rewardIndex].u4BuyCount >= shopGoodsInfo.u2BuyRestriction)
                    return;
            }

            // 가디언 징표 갯수가 보상받을 수 있는 갯수보다 적으면 보상불가
            if (tradeGoods.u4Count < shopGoodsInfo.cBuyGoods.u4Count)
            {
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetText("event_marble_missing_guardianmark"), null);
                return;
            }
        }
        else
        {
            // 팝업
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetText("event_marble_not_get_guardianmark"), null);
            return;
        }

        ServerMgr.Instance.ShopFixShop(shopGoodsInfo.u2ID, 0, 255, 0, "", "", RecieveGuardianMarkReward);
    }

    void RecieveGuardianMarkReward(Server.ERROR_ID err)
    {
        if (err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"),
                TextManager.Instance.GetError(Server.MSGs.SHOP_FIXSHOP, err), null);
            return;
        }
        else if (err == Server.ERROR_ID.NONE)
        {
            ItemSlotEnable(rewardIndex, true);

            Legion.Instance.SubGoods(ShopInfoMgr.Instance.dicShopGoodData[marbleInfo.au2ShopID[rewardIndex]].cBuyGoods);
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("event_marble_get_reward"), TextManager.Instance.GetText("event_marble_check_post"), null);
            SetGuardianMark();
        }
    }
}
