using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class LimitPackagePopup : BaseEventPopup
{
    public Image m_ImgPackageTitle;                 // 패키지 이름 이미지
    public Text m_txtPackageContent;                // 패키지 이름 
    
    public Text m_txtBuyCondition;                  // 구매 조건
    public Text m_txtPackagePrice;                  // 패키지 금액
    public Button m_btnBuy;                         // 구매 버튼
    public LimitPackItemSlot[] m_cLimitItemSlot;    // 아이템 슬롯

    private List<EventPackageInfo> lstEventList;
    private int m_nPackageInfoIdx;                  // 패키지 인덱스
    private int m_nOpenPackageCount;

    public override bool SetPopup(bool isStrongPopup = false)
    {
        base.SetPopup(isStrongPopup);
        lstEventList = EventInfoMgr.Instance.GetSaleEventListByEventType(7);

        m_nPackageInfoIdx = 0;
        if (lstEventList.Count <= 0)
        {
            OnClickClosePopup();
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_buy_disable"), TextManager.Instance.GetText("event_goods_no_sales"), null);
            return false;
        }
            
        SetPackageInfo();

        return true;
    }

    private string GetDiscountBeforePrice(EventPackageInfo info)
    {
        StringBuilder tempString = new StringBuilder();
        float price = 0f;
        float discountRate = float.Parse(info.sDiscountRate);
#if UNITY_ANDROID || UNITY_EDITOR
        if (TextManager.Instance.eLanguage == TextManager.LANGUAGE_TYPE.KOREAN)
        {
            tempString.Append("\\");
            price = (int)(info.cNeedGoods.u4Count * (100 / discountRate));
            price = (int)(price / 1000);
            price *= 1000;
            tempString.AppendFormat("{0:f0}",price.ToString("n0"));
        }
        else
        {
            tempString.Append("$");
            price = float.Parse(info.iOSPrice) * (100 / discountRate);
            tempString.AppendFormat("{0:f2}",price);
        }
#elif UNITY_IOS
        tempString.Append("$");
        price = float.Parse(info.iOSPrice) * (100 / discountRate);
        tempString.AppendFormat("{0:f2}",price);
#endif
        return tempString.ToString();
    }
    
    public void SetPackageInfo()
    {
        EventPackageInfo info = lstEventList[m_nPackageInfoIdx];
        StringBuilder tempString = new StringBuilder();
        // 패키지 이름 이미지 셋팅
        tempString.Append("Sprites/").Append(TextManager.Instance.GetImagePath()).Append(".Limit_pack_title_").Append(info.u1EventGroupIdx);
        m_ImgPackageTitle.sprite = AtlasMgr.Instance.GetSprite(tempString.ToString());
        m_ImgPackageTitle.SetNativeSize();

        tempString.Remove(0, tempString.Length);
        tempString.Append("Sprites/Event/event_popup.SpecialPackage_BG_").Append(info.u1EventGroupIdx);
        _trSlotParent.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite(tempString.ToString());
        // 문구 추가
        m_txtPackageContent.text = string.Format(TextManager.Instance.GetText("event_sepcial_pack_desc_2"), GetDiscountBeforePrice(info), info.sDiscountRate);
        // 가격 셋팅
        tempString.Remove(0, tempString.Length);
        m_txtPackagePrice.text = tempString.Append(TextManager.Instance.GetText("btn_event_grow_buy")).Append(" ").Append(GetEventPackagePriceString(info)).ToString();

        // 판매 조건 셋팅
        if (EventInfoMgr.Instance.dicEventBuy[info.u2ID].u8BuyEnd == 0)
        {
            string messgae = "";
            if (info.cNeedMinPeriod.u1Type > 0)
                messgae = Legion.Instance.GetConsumeString(info.cNeedMinPeriod.u1Type) + info.cNeedMinPeriod.u4Count;
            if (info.cNeedMaxPeriod.u1Type > 0)
                messgae += info.cNeedMaxPeriod.u4Count;
            if (messgae != "")
                messgae += TextManager.Instance.GetText("mark_event_buynow");//"\n구매가능";

            m_txtBuyCondition.text = messgae;
        }
        else
        {
            m_txtBuyCondition.text = " ~ "+ EventInfoMgr.Instance.dicEventBuy[info.u2ID].dtBuyEnd.ToString("yyyy.MM.dd");
        }

        Goods[] packageItme = info.acPackageRewards;
        int setCount = 0;
        for (int i = 0; i < packageItme.Length; ++i)
        {
            if(setCount >= m_cLimitItemSlot.Length )
                break;

            if (packageItme[i].u1Type == 0)
                continue;

            m_cLimitItemSlot[setCount].SetEventItem(setCount, packageItme[i]);
            ++setCount;
        }

        m_btnBuy.interactable = true;
        m_btnBuy.onClick.RemoveAllListeners();
        m_btnBuy.onClick.AddListener(() => OnClickBuyBtn(info));
    }

    public override void OnClickClosePopup()
    {
        ++m_nPackageInfoIdx;
        if (m_nPackageInfoIdx >= lstEventList.Count)
            OnClosePopup();
        else
            StartCoroutine("ScaleEffect");
    }

    public void OnClosePopup()
    {
        PopupManager.Instance.RemovePopup(this.gameObject);
        Destroy(this.gameObject);        
    }

    private IEnumerator ScaleEffect()
    {
        LeanTween.scale(_trSlotParent, Vector3.zero, 0.1f);
        yield return new WaitForSeconds(0.15f);
        SetPackageInfo();
        LeanTween.scale(_trSlotParent, Vector3.one , 0.1f);
        yield return new WaitForSeconds(0.15f);
    }

    protected override void BuyRefiesh()
    {
        m_btnBuy.interactable = false;
    }
}
