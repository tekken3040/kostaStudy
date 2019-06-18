using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UI_League_Reward_ItemSlot : MonoBehaviour
{
    [SerializeField] Image imgGrade;
    [SerializeField] Image imgIcon;
    [SerializeField] Text txtCount;

    public void SetData(Goods _goods)
    {
        switch(_goods.u1Type)
        {
            case (Byte)GoodsType.GOLD:
                imgGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_4571");
                imgIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_Gold");
                break;

            case (Byte)GoodsType.CASH:
                imgGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_4571");
                imgIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_Cash");
                break;
            case (Byte)GoodsType.FRIENDSHIP_POINT:
                imgGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_4571");
                imgIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_friendship");
                break;
                
            case (Byte)GoodsType.KEY:
                imgGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_4571");
                imgIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_Key");
                break;

            case (Byte)GoodsType.ODIN_POINT:
                imgGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_4571");
				imgIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_04_renew.Icon_Adv_VIP");
                break;

            case (Byte)GoodsType.MATERIAL:
                imgGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_"+ItemInfoMgr.Instance.GetItemGrade(_goods.u2ID));
                imgIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/Item_01."+ItemInfoMgr.Instance.GetMaterialItemInfo(_goods.u2ID).u2IconID);
                break;

            case (Byte)GoodsType.CONSUME:
                imgGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_"+ItemInfoMgr.Instance.GetItemGrade(_goods.u2ID));
                imgIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/Item_01."+ItemInfoMgr.Instance.GetConsumableItemInfo(_goods.u2ID).u2ID);
                break;
        }

        txtCount.text = _goods.u4Count.ToString();
    }
}
