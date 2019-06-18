using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class OdinMissionClearPopup : MonoBehaviour
{
    public RectTransform rcTrPopup;
    public Text txtCurrectOdinGrade;
    public Text txtClearMissionInfo;
    public Button BtnMoveOdinPopup;

    private Byte achievementType;
    private PopupManager.OnClickEvent closeCallFunction;

    private void OnEnable()
    {
        rcTrPopup.localScale = new Vector3(2f, 2f, 2f);
        LeanTween.scale(rcTrPopup, Vector3.one, 0.2f);
        PopupManager.Instance.AddPopup(this.gameObject, OnClickClose);
    }

    public void SetClearPopup(UInt16 clearMissionID, Byte achievementType)
    {
        if (this.gameObject.activeSelf || Legion.Instance.cTutorial.bIng)
            return;

        OdinMissionInfo missionInfo;
        if (clearMissionID == 0 || !QuestInfoMgr.Instance.TryGetOdinMissionInfo(clearMissionID, out missionInfo))
        {
            OnClickClose();
            return;
        }

        if(missionInfo.u1MinssionType != achievementType)
        {
            OnClickClose();
            return;
        }

        txtClearMissionInfo.text = TextManager.Instance.GetText(missionInfo.strName);
        txtCurrectOdinGrade.text = TextManager.Instance.GetText(string.Format("odin_name_{0}", Legion.Instance.u1VIPLevel));

        closeCallFunction = null;
        this.achievementType = achievementType;

        // 임무 타입이 크루 오픈이라면 이동 버튼을 비활성화 한다
        if (AchievementTypeData.CrewOpen == missionInfo.u1MinssionType)
        {
            BtnMoveOdinPopup.gameObject.SetActive(false);
        }
        else
        {
            BtnMoveOdinPopup.gameObject.SetActive(true);
        }

        Legion.Instance.cQuest.u2ClearOdinMissionID = 0;
        this.gameObject.SetActive(true);
    }

    public void SetClearPopup(UInt16 clearMissionID, PopupManager.OnClickEvent closeMethot)
    {
        if (this.gameObject.activeSelf || Legion.Instance.cTutorial.bIng)
            return;

        OdinMissionInfo missionInfo;
        if (clearMissionID == 0 || !QuestInfoMgr.Instance.TryGetOdinMissionInfo(clearMissionID, out missionInfo))
        {
            OnClickClose();
            return;
        }

        txtClearMissionInfo.text = TextManager.Instance.GetText(missionInfo.strName);
        txtCurrectOdinGrade.text = TextManager.Instance.GetText(string.Format("odin_name_{0}", Legion.Instance.u1VIPLevel));
        
        achievementType = 0;
        closeCallFunction = closeMethot;

        // 임무 타입이 크루 오픈이라면 이동 버튼을 비활성화 한다
        if (AchievementTypeData.CrewOpen == missionInfo.u1MinssionType)
        {
            BtnMoveOdinPopup.gameObject.SetActive(false);
        }
        else
        {
            BtnMoveOdinPopup.gameObject.SetActive(true);
        }

        Legion.Instance.cQuest.u2ClearOdinMissionID = 0;
        this.gameObject.SetActive(true);
    }

    public void OnClickMoveOdinPopup()
    {
        Legion.Instance.AwayBattle();
        FadeEffectMgr.Instance.QuickChangeScene(MENU.MAIN, (Byte)POPUP_MAIN.ODIN, null);

        achievementType = 0;
        closeCallFunction = null;

        BtnMoveOdinPopup.gameObject.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void OnClickClose()
    {
        if (closeCallFunction != null)
        {
            closeCallFunction(null);
        }
        else if (achievementType != 0)
        {
            Legion.Instance.cQuest.CheckEndDirection(achievementType);
        }

        PopupManager.Instance.RemovePopup(this.gameObject);
        this.gameObject.SetActive(false);
    }
}
