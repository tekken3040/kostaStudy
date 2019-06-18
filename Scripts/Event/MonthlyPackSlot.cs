using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class MonthlyPackSlot : MonoBehaviour
{
    public Text _txtTitle;
    public Text _txtDiscountRate;
    public Image[] _aimgRewardIcons;
    public Text[] _atxtRewardName;
    public Text[] _atxtRewardCount;
    public Text _txtPackPrice;
    public Button _btnBuyBtn;

    public void SetSlot(EventPackageInfo eventInfo)
    {
        _txtTitle.text = TextManager.Instance.GetText(eventInfo.sName);
        _txtDiscountRate.text = eventInfo.sDiscountRate + " %";
        _txtPackPrice.text = GetEventPackagePriceString(eventInfo);

        for(int i = 0; i < _aimgRewardIcons.Length; ++i)
        {
            _aimgRewardIcons[i].gameObject.SetActive(false);
            _atxtRewardName[i].gameObject.SetActive(false);
        }

        for(int i = 0, j = 0; i < eventInfo.acPackageRewards.Length; ++i)
        {
            if ( eventInfo.acPackageRewards[i].u1Type == 0 )
                continue;

            Goods goods = eventInfo.acPackageRewards[i];
            _aimgRewardIcons[i].gameObject.SetActive(true);
            _aimgRewardIcons[j].sprite = GetItemICon(eventInfo.acPackageRewards[i]);
            _aimgRewardIcons[j].SetNativeSize();
            _aimgRewardIcons[j].GetComponent<RectTransform>().sizeDelta *= 0.5f;

            // 아이템 정보 셋팅
            string itmeName;
            if (goods.u1Type == (byte)GoodsType.CONSUME)
                itmeName = TextManager.Instance.GetText(ItemInfoMgr.Instance.GetConsumableItemInfo(goods.u2ID).sName);
            else if (goods.u1Type == (byte)GoodsType.MATERIAL)
                itmeName = TextManager.Instance.GetText(ItemInfoMgr.Instance.GetMaterialItemInfo(goods.u2ID).sName);
            else
                itmeName = Legion.Instance.GetConsumeString(goods.u1Type);

            _atxtRewardName[i].gameObject.SetActive(true);
            _atxtRewardName[j].text = itmeName;
            _atxtRewardName[j].GetComponent<Gradient>().EndColor = GetTextColor(goods.u1Type);
            _atxtRewardCount[j].text = goods.u4Count.ToString();

            j++; // 슬롯 인덱스값 증가
        }
    }
    
    protected virtual Color GetTextColor(Byte itmeType)
    {
        Color color;
        switch ((GoodsType)itmeType)
        {
            case GoodsType.GOLD:
            case GoodsType.KEY:
                color = new Color32(255, 180, 100, 255);
                break;
            case GoodsType.CASH:
                color = new Color32(255, 100, 100, 255);
                break;
            default:
                color = new Color32(101, 218, 209, 255);
                break;
        }
        return color;
    }
    /*
    protected Color SetColor(int r, int g, int b, int a = 255)
    {
        Color resultColor = new Color(
            (r == 0) ? r : (float)Math.Round((double)r / 255, 3),
            (g == 0) ? g : (float)Math.Round((double)g / 255, 3),
            (b == 0) ? b : (float)Math.Round((double)b / 255, 3),
            (a == 0) ? a : (float)Math.Round((double)a / 255, 3)
        );

        return resultColor;
    }
    */
    protected Sprite GetItemICon(Goods good)
    {
        switch ((GoodsType)good.u1Type)
        {
            case GoodsType.GOLD:
                return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.gold_3");
            case GoodsType.CASH:
                return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.cash_3");
            case GoodsType.KEY:
                return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.key_3");
            case GoodsType.LEAGUE_KEY:
                return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.key_3");
            case GoodsType.FRIENDSHIP_POINT:
                return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.friend_3");
            case GoodsType.MATERIAL:
                return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.forge_stuff_1");//AtlasMgr.Instance.GetSprite("Sprites/Item/Item_01." + ItemInfoMgr.Instance.GetMaterialItemInfo(good.u2ID).u2IconID);
            case GoodsType.CONSUME:
                if (good.u2ID >= 58006 && good.u2ID <= 58009 || good.u2ID == 58004)
                    return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02." + good.u2ID);
                else
                    return AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + good.u2ID);
            case GoodsType.EQUIP_COUPON:
                return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.equip_drawing");
            case GoodsType.MATERIAL_COUPON:
                return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.stuff_drawing");
        }

        return null;
    }

    protected string GetEventPackagePriceString(EventPackageInfo packageInfo)
    {
        StringBuilder tempString = new StringBuilder();
#if UNITY_ANDROID || UNITY_EDITOR
        if (TextManager.Instance.eLanguage == TextManager.LANGUAGE_TYPE.KOREAN)
            tempString.Append("￦").Append(packageInfo.cNeedGoods.u4Count.ToString());
        else
            tempString.Append("$").Append(packageInfo.iOSPrice.ToString());
#elif UNITY_IOS
		tempString.Append("$ ").Append(packageInfo.iOSPrice.ToString());
#endif
        return tempString.ToString();
    }

}
