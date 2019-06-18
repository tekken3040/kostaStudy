using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class OdinMissionSlot : MonoBehaviour
{
    public Image imgMissionSlot;            // 미션 슬롯
    public Text txtMissionTitle;            // 미션 이름
    public Text txtMissionDesc;             // 클리어 정보
    public Text txtRewardPoint;             // 보상 포인트

    public GameObject _objMissionInfo;      // 미션 정보
    public GameObject _objMissionComplete;  // 미션 완료

    private UserOdinMission userMissionInfo;
    public UserOdinMission SlotMissionInfo { get { return userMissionInfo; } }

    public void SetMissionSlot(UserOdinMission userOidnMission)
    {
        userMissionInfo = userOidnMission;
        OdinMissionInfo missionInfo = userMissionInfo.GetInfo();

        // 임무 진행 타입에 따른 기본 슬롯 셋팅
        if (missionInfo.u1RollingType == 1)
        {
            // 미션 슬롯 셋팅
            imgMissionSlot.sprite = AtlasMgr.Instance.GetSprite(string.Format("Sprites/Common/common_05_renew.odinMissionSlot_{0}", missionInfo.u1MissionKind));
            imgMissionSlot.SetNativeSize();

            txtRewardPoint.gameObject.SetActive(true);
            txtRewardPoint.text = string.Format("X  {0}", missionInfo.cFastReward.u4Count);
        }
        else
        {
            txtRewardPoint.gameObject.SetActive(false);
        }

        // 미션 클리어 여부
        if (userOidnMission.IsClear())
        {
            _objMissionComplete.SetActive(true);
            _objMissionInfo.SetActive(false);
        }
        else
        {
            _objMissionComplete.SetActive(false);
            _objMissionInfo.SetActive(true);

            txtMissionTitle.text = TextManager.Instance.GetText(missionInfo.strName);
            // 임무 진행 타입에 따른 정보 셋팅
            if (missionInfo.u1RollingType == 1)
            {
                txtMissionDesc.text = string.Format("{0} ({1}/{2})", TextManager.Instance.GetText(missionInfo.strDescription),
                userMissionInfo.MissionProgressCount, missionInfo.u4TargetValue);
            }
            else
            {
                txtMissionDesc.text = string.Format(TextManager.Instance.GetText("odin_mid_mission_desc"),
                    string.Format("{0} ({1}/{2})", 
                        TextManager.Instance.GetText(missionInfo.strDescription), // 미션 클리어 조건
                        userMissionInfo.MissionProgressCount,                       // 현재 진행 갯수 
                        missionInfo.u4TargetValue));                        // 완료 조건 갯수
            }
        }
    }
}
