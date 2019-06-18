using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class OdinMissionReward : MonoBehaviour
{
    public GameObject _objEffect;
    public GameObject _objRewardInfo;

    public Text txtRewardOdinName;
    public ItemIconSlot[] rewardSlot;

    public Button CloseBtn;

    private Transform RewardTtitlePr;

    private void Awake()
    {
        RewardTtitlePr = txtRewardOdinName.transform.parent;
    }

    public void SetMissionReward(UInt16 missionID)
    {
        OdinMissionInfo missionInfo;
        if (QuestInfoMgr.Instance.TryGetOdinMissionInfo(missionID, out missionInfo))
        {
            SetMissionReward(missionInfo);
        }
    }

    public void SetMissionReward(OdinMissionInfo missionInfo)
    {
        CloseBtn.interactable = false;
        this.gameObject.SetActive(true);
        _objEffect.SetActive(false);
        _objRewardInfo.SetActive(false);

        txtRewardOdinName.text = string.Format(TextManager.Instance.GetText("odin_mission_name"), TextManager.Instance.GetText(string.Format("odin_name_{0}", Legion.Instance.u1VIPLevel)));

        int slotIdx = 0;
        // VIP 포인트 셋팅
        if (missionInfo.cFastReward.u1Type != 0)
        {
            rewardSlot[slotIdx].SetReward(missionInfo.cFastReward);
            ++slotIdx;
        }
        // 보상 셋팅
        for (int i = 0; i < missionInfo.acReward.Length; ++i)
        {
            if (missionInfo.acReward[i].u1Type != 0)
            {
                rewardSlot[slotIdx].SetReward(missionInfo.acReward[i]);
                ++slotIdx;
            }
        }
        // 남은 슬롯 비활성화 및 연출 준비
        for(int i = 0; i < rewardSlot.Length; ++i)
        {
            if (slotIdx <= i)
            {
                rewardSlot[i].gameObject.SetActive(false);
            }
            else
            {
                rewardSlot[i].transform.localRotation = Quaternion.Euler(0, 90, 0);
            }
        }
        
        StartCoroutine("StartEffect");
    }

    private IEnumerator StartEffect()
    {
        RewardTtitlePr.localScale = Vector3.zero;

        _objEffect.SetActive(true);
        yield return new WaitForSeconds(6f);

        _objEffect.SetActive(false);
        _objRewardInfo.SetActive(true);

        LeanTween.scale(RewardTtitlePr.gameObject, Vector3.one, 0.2f);
        yield return new WaitForSeconds(0.2f);

        for(int i = rewardSlot.Length - 1; i >= 0; --i)
        {
            if (!rewardSlot[i].gameObject.activeSelf)
                continue;

            LeanTween.rotate(rewardSlot[i].gameObject, Vector3.zero, 0.3f);
            yield return new WaitForSeconds(0.3f);
        }

        CloseBtn.interactable = true;
    }

    public void OnClickCloseBtn()
    {
        this.gameObject.SetActive(false);
    }
}
