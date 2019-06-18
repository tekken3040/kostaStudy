using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class TutorialEventPopup : BaseEventPopup
{
    public delegate void PoppupCloseEvent();
    public PoppupCloseEvent closeEvent;

    public Button _btnBuy;
    public Image _imgPopupBG;
    public Text _txtPackageNotice;
    public Text _txtBeforeDiscountPrice;        // 할인 전 금액
    public Text _txtPackagePrice;

    public override bool SetPopup(bool isStrongPopup = false)
    {
        base.SetPopup(isStrongPopup);

        StringBuilder tempString = new StringBuilder();
        tempString.Append("Sprites/").Append(TextManager.Instance.GetImagePath());
        tempString.Append("_Tutorial_30day_popup").Append(".png");
        _imgPopupBG.sprite = AssetMgr.Instance.AssetLoad(tempString.ToString(), typeof(Sprite)) as Sprite;

        List<EventPackageInfo> monthPackList = EventInfoMgr.Instance.GetEventListByEventType(5);
        if (monthPackList.Count <= 0)
        {
            OnClickClosePopup();
            return false;
        }
        
        bool isBuyPackage = false;
        int buyIndex = -1;
        for(int i = 0; i < monthPackList.Count; ++i)
        {
            // Tutorial Package은 그룹 인덱스가 1
            if (monthPackList[i].u1EventGroupIdx == 1)
            {
                buyIndex = i;
                if (EventInfoMgr.Instance.CheckBuyPossible(monthPackList[i].u2ID) != 1)
                {
                    isBuyPackage = true;
                }
            }
            else
            {
                // 패키지 할인전 금액 셋팅
                _txtBeforeDiscountPrice.text = GetEventPackagePriceString(monthPackList[i]);
            }
        }

        string packagePrice = GetEventPackagePriceString(monthPackList[buyIndex]);
        _txtPackageNotice.text = string.Format(TextManager.Instance.GetText("30day_pack_text_sale"), packagePrice);

        tempString.Remove(0, tempString.Length);
        // 상품 구매가 되었는지 확인한다
        if (!isBuyPackage && buyIndex >= 0)
        {
            _btnBuy.interactable = true;
            _txtPackagePrice.text = tempString.Append(TextManager.Instance.GetText("btn_event_grow_buy")).Append(" ").Append(packagePrice).ToString();
            _btnBuy.onClick.AddListener(() => OnClickBuyBtn(monthPackList[buyIndex]));
        }
        else
        {
            OnClickClosePopup();
            return false;
        }

        return true;
    }

    protected override void BuyRefiesh()
    {
        _btnBuy.interactable = false;
    }

    public override void OnClickClosePopup()
    {
        if (closeEvent != null)
        {
            closeEvent();
        }
        Legion.Instance.SubLoginPopupStep(Legion.LoginPopupStep.EVENT_30DAY_PACK);
        Destroy(gameObject);
    }
}
