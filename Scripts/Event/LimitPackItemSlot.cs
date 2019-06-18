using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class LimitPackItemSlot : EventItemSlot
{
    public override void SetEventItem(int index, Goods info)
    {
        StringBuilder tempString = new StringBuilder();

        if (_objPlusIcon != null)
        {
            if (index == 0)
                _objPlusIcon.SetActive(false);
            else
                _objPlusIcon.SetActive(true);
        }
        _imgItemIcon.sprite = GetItemICon(info);
        _imgItemIcon.SetNativeSize();
        _imgItemIcon.transform.localPosition = Vector3.zero;

        //_imgItemIcon.GetComponent<RectTransform>().sizeDelta
        string itmeName;
        if (info.u1Type == (byte)GoodsType.CONSUME)
            itmeName = TextManager.Instance.GetText(ItemInfoMgr.Instance.GetConsumableItemInfo(info.u2ID).sName);
        else if (info.u1Type == (byte)GoodsType.MATERIAL)
            itmeName = TextManager.Instance.GetText(ItemInfoMgr.Instance.GetMaterialItemInfo(info.u2ID).sName);
        else
            itmeName = Legion.Instance.GetConsumeString(info.u1Type);

        tempString.Append(itmeName).Append(" ").Append(info.u4Count.ToString("n0"));
        // 골드가 아닐때만 뒤 부분에 "개"를 붙인다
        if (info.u1Type != (byte)GoodsType.GOLD)
            tempString.Append(TextManager.Instance.GetText("mark_goods_number_ea"));

        _txtItemInfo.text = tempString.ToString();
        _txtItemInfo.color = GetTextColor(info.u1Type);
    }

    protected override Color GetTextColor(Byte itmeType)
    {
        Color color;
        switch ((GoodsType)itmeType)
        {
            case GoodsType.GOLD:
            case GoodsType.KEY:
                color = new Color32(255, 170, 50, 255);
                break;
            case GoodsType.CASH:
                color = new Color32(255, 75, 50, 255);
                break;
            default:
                color = new Color32(255, 130, 50, 255);
                break;
        }
        return color;
    }

    protected override Sprite GetItemICon(Goods good)
    {
        switch ((GoodsType)good.u1Type)
        {
            case GoodsType.GOLD:
                return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.Event_Gold_Icon");
            case GoodsType.CASH:
                return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.Event_Cash_Icon");
            case GoodsType.KEY:
                return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.Event_Key_Icon");
            case GoodsType.LEAGUE_KEY:
                return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.key_3");
            case GoodsType.FRIENDSHIP_POINT:
                return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.friend_3");
            case GoodsType.MATERIAL:
                return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.forge_stuff_1");
            case GoodsType.CONSUME:
                if (good.u2ID == 58004)
                    return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.Event_ClearItem_Icon");
                if (good.u2ID >= 58001 && good.u2ID <= 58017)
                    return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.exp_potion_5");
                else
                    return AtlasMgr.Instance.GetSprite("Sprites/Item/item_01.Event_MaterialGacha_ICon");
            case GoodsType.EQUIP_COUPON:
                return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.equip_drawing");
            case GoodsType.MATERIAL_COUPON:
                return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.Event_MaterialGacha_ICon");
        }

        return null;
    }
}
