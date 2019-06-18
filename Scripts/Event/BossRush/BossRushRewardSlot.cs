using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class BossRushRewardSlot : MonoBehaviour
{
    [SerializeField] Image imgIcon;
    [SerializeField] Image imgItemGrade;
    [SerializeField] Text txtCount;
    [SerializeField] Text txtName;

    StringBuilder tempStringBuilder;
    Goods _goods;
    public void SetData(Goods goods)
    {
        _goods = goods;
        tempStringBuilder = new StringBuilder();
        tempStringBuilder.Append(" x").Append(goods.u4Count);
        txtCount.text = tempStringBuilder.ToString();
        switch(_goods.u1Type)
        {
            case (Byte)GoodsType.CASH:
                imgIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_Cash");
                imgIcon.SetNativeSize();
                imgItemGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_4570");
                txtName.text = TextManager.Instance.GetText("mark_cash");
                break;

            case (Byte)GoodsType.GOLD:
                imgIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_Gold");
                imgIcon.SetNativeSize();
                imgItemGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_4570");
                txtName.text = TextManager.Instance.GetText("mark_gold");
                break;

            case (Byte)GoodsType.FRIENDSHIP_POINT:
                imgIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_friendship");
                imgIcon.SetNativeSize();
                txtName.text = TextManager.Instance.GetText("mark_friendshippoint");
                break;

            case (Byte)GoodsType.MATERIAL:
                //imgIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/Item_01." + _goods.u2ID);
                //imgItemGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(_goods.u2ID));
                MaterialItemInfo itemInfo = ItemInfoMgr.Instance.GetMaterialItemInfo(_goods.u2ID);
                imgIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/Item_01." + itemInfo.u2IconID);
                imgItemGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + itemInfo.u2Grade);
                txtName.text = TextManager.Instance.GetText(itemInfo.sName);
                break;

            case (Byte)GoodsType.CONSUME:
                imgIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/Item_01." + _goods.u2ID);
                imgIcon.SetNativeSize();
                imgItemGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(_goods.u2ID));
                txtName.text = TextManager.Instance.GetText(ItemInfoMgr.Instance.GetConsumableItemInfo(_goods.u2ID).sName);
                break;
        }
    }

    public void OnClickShowItemInfo()
    {
        PopupManager.Instance.ShowItemInfo(_goods.u1Type, _goods.u2ID, (transform.root.position - transform.position), transform.root.localScale.x);
    }
}
