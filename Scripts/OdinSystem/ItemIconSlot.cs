using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemIconSlot : MonoBehaviour
{
    public Image imgRewardSlot;
    public Image imgRewardGrade;
    public Image imgRewardIcon;

    public Text txtRewardItemName;

    public void SetReward(Goods reward)
    {
        if(reward.u1Type == 0)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(true);
        }

        if((GoodsType)reward.u1Type == GoodsType.ODIN_POINT)
        {
            imgRewardIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_04_renew.Odin_Point_Reward_Icon");
        }
        else
        {
            imgRewardGrade.sprite = AtlasMgr.Instance.GetGoodsGrade(reward);
            imgRewardIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(reward);
        }
        imgRewardIcon.SetNativeSize();

        txtRewardItemName.text = string.Format("{0} {1}",Legion.Instance.GetGoodsName(reward), reward.u4Count);
    }
}
