using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class RepeatReward : MonoBehaviour
{
    public enum REWARD_TYPE
    {
        NONE = 0,
        REPEAT_REARD = 1,
        STAR_REWARD = 2,
    }

    public REWARD_TYPE rewardType;
    public Button rewardBtn;
    public Text rewardBtnTxt;
    public Image rewardItemIcon;
    public Image rewardItemGrade;
    public Text rewardItemCount;
    public Text repeatCount;
    public GameObject repeatRewardOn;

    public RepeatResult cRepeatResultWindow;  // 보상 창

    private UInt16 u2ChapterID;
    private byte u1Difficult;

    public void SetRewardInfo(UInt16 chapterID, byte difficult)
    {
        u2ChapterID = chapterID;
        u1Difficult = difficult;
        if (StageInfoMgr.Instance.dicChapterData.ContainsKey(chapterID) == false)
        {
            RepeatRewardAction(false);
            return;
        }
        else
            RepeatRewardAction(true);

        switch (rewardType)
        {
            case REWARD_TYPE.REPEAT_REARD:
                SetRepeatReward();
                break;
            case REWARD_TYPE.STAR_REWARD:
                SetStarReward();
                break;
            default:
                RepeatRewardAction(false);
                break;
        }
    }

    // 반복보상 셋팅
    protected void SetRepeatReward()
    {
        ChapterInfo info = StageInfoMgr.Instance.dicChapterData[u2ChapterID];
        Goods reward = null;
        //첫보상 일 경우
        if (info.repeatType[u1Difficult - 1] == 0)
            reward = info.acFirstRewards[u1Difficult - 1];
        else // 첫보상이 이후 보상 처리
            reward = info.acRepeatRewards[info.repeatType[u1Difficult - 1] - 1][u1Difficult - 1];

        if(reward.u1Type == 0)
        {
            RepeatRewardAction(false);
            return;
        }

        rewardItemIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(reward);
        rewardItemGrade.sprite = AtlasMgr.Instance.GetGoodsGrade(reward);
        rewardItemCount.text = reward.u4Count.ToString();
        repeatCount.text = info.repeatCount[u1Difficult - 1] + "/" + info.u1RepeatCount;

        //보상 수령 가능
        SetRewardBtnEnable((info.repeatCount[u1Difficult - 1] >= info.u1RepeatCount));
    }

    protected void SetStarReward()
    {
        ChapterInfo info = StageInfoMgr.Instance.dicChapterData[u2ChapterID];
        Goods reward = info.acStarRewards[u1Difficult - 1];

        if (reward.u1Type == 0)
        {
            RepeatRewardAction(false);
            return;
        }

        rewardItemIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(reward);
        rewardItemGrade.sprite = AtlasMgr.Instance.GetGoodsGrade(reward);
        rewardItemCount.text = reward.u4Count.ToString();

        bool isGet = false;
        //마지막 스테이지 별 카운트가 3인데 챕터의 별 카운트가 0일경우 보상을 수령을 한 경우
        if (info.starCount[u1Difficult - 1] == 0)
        {
            if (StageInfoMgr.Instance.GetStageClearStar(info.lstStageID[info.lstStageID.Count - 1]) > 2)
                isGet = true;

			if (isGet) {
				repeatCount.text = info.u1StarCount + "/" + info.u1StarCount;
				SetRewardBtnEnable (false);
				return;
			} else {
				repeatCount.text = info.starCount [u1Difficult - 1] + "/" + info.u1StarCount;
			}
        }
        else
        {
            repeatCount.text = info.starCount[u1Difficult - 1] + "/" + info.u1StarCount;
        }

        //보상 수령 가능 버튼 활성화 여부
        SetRewardBtnEnable((!isGet && info.starCount[u1Difficult - 1] >= info.u1StarCount));
    }

    protected void SetRewardBtnEnable(bool isGetReward)
    {
        if (isGetReward == true)
        {
            rewardBtn.interactable = true;
            rewardBtnTxt.color = Color.white;
            repeatRewardOn.SetActive(true);
        }
        //불가능 처리
        else
        {
            rewardBtn.interactable = false;
            rewardBtnTxt.color = Color.gray;
            repeatRewardOn.SetActive(false);
        }
    }

    public void OnClickGetReward()
    {
        switch (rewardType)
        {
            case REWARD_TYPE.REPEAT_REARD:
                OnClickRepeatReward();
                break;
            case REWARD_TYPE.STAR_REWARD:
                OnClickStarReward();
                break;
        }
    }

    // 반복 보상 수령 클릭
    protected void OnClickRepeatReward()
    {
        ChapterInfo info = StageInfoMgr.Instance.dicChapterData[u2ChapterID];
        Goods goods;
        // 초기 보상 수령이라면 
        if (info.repeatType[u1Difficult - 1] == 0)
            goods = info.acFirstRewards[u1Difficult - 1];
        else
            goods = info.acRepeatRewards[info.repeatType[u1Difficult - 1] - 1][u1Difficult - 1];

        //보상을 받을시 재화 초과 여부를 체크하여 초과 하면 재화를 받지 않는다
        if (Legion.Instance.CheckGoodsLimitExcessx(goods.u1Type) == true)
        {
            Legion.Instance.ShowGoodsOverMessage(goods.u1Type);
            return;
        }

        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.StageRepeatReward(info.u2ID, u1Difficult, AckRepeatReward);
    }

    // 별 보상 수령 클릭
    protected void OnClickStarReward()
    {
        ChapterInfo info = StageInfoMgr.Instance.dicChapterData[u2ChapterID];
        //보상을 받을시 재화 초과 여부를 체크하여 초과 하면 재화를 받지 않는다
        if (Legion.Instance.CheckGoodsLimitExcessx(info.acStarRewards[u1Difficult - 1].u1Type) == true)
        {
            Legion.Instance.ShowGoodsOverMessage(info.acStarRewards[u1Difficult - 1].u1Type);
            return;
        }

        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.StageStarReward(info.u2ID, u1Difficult, AckStarReward);
    }

    // 반복 보상 수령 통신 처리
    private void AckRepeatReward(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if (err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.STAGE_REPEATREWARD, err), Server.ServerMgr.Instance.CallClear);
            return;
        }
        else
        {
            if (StageInfoMgr.Instance.repeatReward == 100)
            {
                DebugMgr.Log("Error");
                return;
            }
            else
            {
                ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[u2ChapterID];
                Goods reward;
                // 초기 보상 수령
                if (StageInfoMgr.Instance.repeatReward == 0)
                {
                    reward = chapterInfo.acFirstRewards[u1Difficult - 1];
                    Legion.Instance.AddGoods(reward);
                    chapterInfo.repeatType[u1Difficult - 1] = 1;
                }
                // 그 이후 보상 수령
                else
                {
                    reward = chapterInfo.acRepeatRewards[StageInfoMgr.Instance.repeatReward - 1][u1Difficult - 1];
                    Legion.Instance.AddGoods(reward);
                    chapterInfo.repeatType[u1Difficult - 1] += 1;

                    if (chapterInfo.repeatType[u1Difficult - 1] > ChapterInfo.MAX_REWARD_ROTATION)
                        chapterInfo.repeatType[u1Difficult - 1] = 1;
                }

                chapterInfo.repeatCount[u1Difficult - 1] -= chapterInfo.u1RepeatCount;

                cRepeatResultWindow.gameObject.SetActive(true);
                cRepeatResultWindow.SetInfo(reward);

                // 보상 결과 처리 팝업
                SetRepeatReward();

                StageInfoMgr.Instance.repeatReward = 100;
                SelectStageScene selectStageScene = Scene.GetCurrent() as SelectStageScene;

                if (selectStageScene != null)
                {
                    selectStageScene.RefreshRewardInfo();
                }
            }
        }
    }

    // 별 보상 수령 통신 처리 
    private void AckStarReward(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();

        DebugMgr.Log(err);

        if (err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.STAGE_STARREWARD, err), Server.ServerMgr.Instance.CallClear);
            return;
        }
        else
        {
            ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[u2ChapterID];
            Byte difficult = Legion.Instance.SelectedDifficult;
            Goods reward = null;
            reward = chapterInfo.acStarRewards[Legion.Instance.SelectedDifficult - 1];

            Legion.Instance.AddGoods(reward);
            chapterInfo.starCount[Legion.Instance.SelectedDifficult - 1] = 0; // 보상 수령시 챕터의 별 카운트를 0으로 해준다

            cRepeatResultWindow.gameObject.SetActive(true);
            cRepeatResultWindow.SetInfo(reward);

            SetStarReward();

            SelectStageScene selectStageScene = Scene.GetCurrent() as SelectStageScene;
            if (selectStageScene != null)
            {
                selectStageScene.RefreshRewardInfo();
            }
        }
    }

    public void RepeatRewardAction(bool isActive)
    {
        this.gameObject.SetActive(isActive);
    }
}
