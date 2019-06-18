using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class RepeatTargetInfo : MonoBehaviour
{
    private const UInt32 MAX_TARGET_COUNT = 99;
    public Image ItemIcon;
    public Image ItmeGrade;
    public RewardButton cRewardButton;
    public Text ItemTargetCount;

    private UInt32 m_u4TargetCount;

    public void SetTargetInfo(Goods targetInfo)
    {
        ItemIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/Item_01." + ItemInfoMgr.Instance.GetMaterialItemInfo(targetInfo.u2ID).u2IconID);
        ItemIcon.SetNativeSize();
        ItmeGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(targetInfo.u2ID));
        cRewardButton.SetButton(targetInfo.u1Type, targetInfo.u2ID);

        m_u4TargetCount = targetInfo.u4Count;
        ItemTargetCount.text = m_u4TargetCount.ToString();
    }

    public void TargetCountUp()
    {
        ++m_u4TargetCount;
        if (m_u4TargetCount > MAX_TARGET_COUNT)
            m_u4TargetCount = 1;

        ItemTargetCount.text = m_u4TargetCount.ToString();
        StageInfoMgr.Instance.RepeatTargetItem.u4Count = m_u4TargetCount;
    }

    public void TargetCountDown()
    {
        --m_u4TargetCount;
        if (m_u4TargetCount <= 0)
            m_u4TargetCount = MAX_TARGET_COUNT;

        ItemTargetCount.text = m_u4TargetCount.ToString();
        StageInfoMgr.Instance.RepeatTargetItem.u4Count = m_u4TargetCount;
    }
}
