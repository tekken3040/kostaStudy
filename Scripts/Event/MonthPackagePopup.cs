using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class MonthPackagePopup : BaseEventPopup
{
    public Button _btnBuy;
	public Image _imgPopupBG;
    public Text _txtPackagePrice;
    public Text _txtPackageNotice;
    public override bool SetPopup(bool isStrongPopup = false)
    {
        base.SetPopup(isStrongPopup);

		StringBuilder tempString = new StringBuilder();
		tempString.Append("Sprites/").Append(TextManager.Instance.GetImagePath());
		tempString.Append("_30DayPopup").Append(".png");
		_imgPopupBG.sprite = AssetMgr.Instance.AssetLoad(tempString.ToString(), typeof(Sprite)) as Sprite;

        List<EventPackageInfo> monthPackList = EventInfoMgr.Instance.GetEventListByEventType(5);
        if (monthPackList.Count <= 0)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_buy_disable"), TextManager.Instance.GetText("event_goods_no_sales"), null);
            OnClickClosePopup();
            return false;
        }

        bool isBuyPackage = false;
        int buyIndex = -1;
        for (int i = 0; i < monthPackList.Count; ++i)
        {
            if (EventInfoMgr.Instance.CheckBuyPossible(monthPackList[i].u2ID) != 1 &&
                EventInfoMgr.Instance.dicEventReward.ContainsKey(monthPackList[i].u2ID))
            {
                isBuyPackage = true;
                buyIndex = i;
                break;
            }

            if (monthPackList[i].u1EventGroupIdx == 0)
            {
                buyIndex = i;
            }
        }
        string packagePrice = GetEventPackagePriceString(monthPackList[buyIndex]);
        _txtPackageNotice.text = string.Format(TextManager.Instance.GetText("30day_pack_text_normal"), packagePrice);

        // 상품 구매가 되었는지 확인한다
        if (isBuyPackage == false && buyIndex >= 0)
        {
            _txtPackagePrice.text = string.Format("{0} {1}", TextManager.Instance.GetText("btn_event_grow_buy"), packagePrice);
            _btnBuy.interactable = true;
            _btnBuy.onClick.AddListener(() => OnClickBuyBtn(monthPackList[buyIndex]));
        }
        else
        {
            _btnBuy.interactable = false;
            string msg = EventInfoMgr.Instance.dicEventReward[monthPackList[buyIndex].u2ID].u4RecordValue + TextManager.Instance.GetText("mark_event_day_remain");//"일 남음";
            
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), msg, null);
            _txtPackagePrice.text = msg;
        }

        return true;
    }

    public void OnClickPackageBuy()
    {
        if (_cBuyPackageInfo == null)
            return;

        BuyPackage(_cBuyPackageInfo);
    }

    protected override void BuyRefiesh()
    {
        _btnBuy.interactable = false;
    }
}
