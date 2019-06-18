using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class EventMarbleBoardSlot : MonoBehaviour
{
    //public Image imgRewardIcon; // 추후 보상아이콘 변경을 사용할 시를 위하여
    public Text txtRewardInfo;
    public Image imgBoardEff;   // 추후 데이터 테이블에서 읽어서 셋팅하게 될 수 있기 때문에 GameObject가 아닌 Image로 셋팅한다

    public void SetBoardSlot(EventMarbleBoard boardInfo)
    {
        BoardEffectActive(false);
        if (boardInfo.cReward.u1Type != 0)
        {
            // 마블이 시작점이라면 별도의 텍스트를 셋팅한다
            if (boardInfo.u4BoardPos == 1)
                txtRewardInfo.text = TextManager.Instance.GetText("event_marble_start_desc");
            else
                SetRewardInfo(boardInfo.cReward);
        }            
        else
            SetMoveInfo(boardInfo.u4Move);
    }

    private void SetMoveInfo(Int32 move)
    {
        StringBuilder tempString = new StringBuilder();
        if (move > 0)
        {
            tempString.Append(TextManager.Instance.GetText("event_marble_forward"));
            tempString.Replace("{0}", move.ToString());
        }
        else
        {
            tempString.Append(TextManager.Instance.GetText("event_marble_backward"));
            // 음수값을 양수 값으로 변경하여 셋팅
            tempString.Replace("{0}", Math.Abs(move).ToString());
        }

        txtRewardInfo.text = tempString.ToString();
    }

    private void SetRewardInfo(Goods reward)
    {
        // 보상 아이콘 셋팅
        //imgRewardIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(reward);
        StringBuilder tempString = new StringBuilder();
        tempString.Append(Legion.Instance.GetGoodsName(reward));
        tempString.Append(" ").Append(reward.u4Count);
        // 골드 및 우정 포인트를 제외하고 ~개를 붙인다
        if ((GoodsType)reward.u1Type != GoodsType.GOLD &&
            (GoodsType)reward.u1Type != GoodsType.FRIENDSHIP_POINT)
        {
            tempString.Append(TextManager.Instance.GetText("mark_goods_number_ea"));
        }

        txtRewardInfo.text = tempString.ToString();
    }

    // 보드 이펙트 활성화 여부
    public void BoardEffectActive(bool isActive)
    {
        if (imgBoardEff != null)
            imgBoardEff.gameObject.SetActive(isActive);
    }
}