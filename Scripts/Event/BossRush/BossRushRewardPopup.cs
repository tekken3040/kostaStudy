using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class BossRushRewardPopup : MonoBehaviour
{
    [SerializeField] Text txtTitle;
    [SerializeField] GameObject objRewardGroup;
    [SerializeField] Transform _parent;
    StringBuilder tempStringBuilder;

    public void SetData(BossRushInfo info, int idx)
    {
        tempStringBuilder = new StringBuilder();
        tempStringBuilder.Append(20*(idx+1)).Append("%").Append(TextManager.Instance.GetText("event_bossrush_popup_reward"));
        txtTitle.text = tempStringBuilder.ToString();
        for(int i=0; i<info.REWARD_STEP_IN_CNT; i++)
        {
            GameObject rewardSlot = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/BossRush/BossRushRewardSlot.prefab", typeof(GameObject)) as GameObject);
            rewardSlot.transform.SetParent(_parent);
            rewardSlot.transform.localPosition = Vector3.zero;
            rewardSlot.transform.localScale = Vector3.one;
            rewardSlot.GetComponent<BossRushRewardSlot>().SetData(info.rewardGoods[idx*3 + i]);
        }
    }

    public void OnClickClose()
    {
        for(int i=0; i<_parent.GetChildCount(); i++)
            Destroy(_parent.GetChild(i).gameObject);
        PopupManager.Instance.RemovePopup(this.gameObject);
        this.gameObject.SetActive(false);
    }
}
