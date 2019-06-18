using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class UI_League_IntroducePopup : MonoBehaviour
{
    [SerializeField] GameObject objSeason;
    [SerializeField] GameObject objDivisionGroup;
    [SerializeField] GameObject objSeasonGroup;
    [SerializeField] Text txtSeasonInfoTitle;
    [SerializeField] Text txtSeasonInfoContent;

    [SerializeField] GameObject objReward;
    [SerializeField] Text txtRewardInfoTitle;
    [SerializeField] Text txtRewardInfoContent;

    [SerializeField] Button btnPrev;
    [SerializeField] Button btnNext;

    private Byte u1Page = 0;
    private StringBuilder tempStringBuilder;

    private void OnEnable()
    {
        tempStringBuilder = new StringBuilder();
        objSeason.SetActive(true);
        objDivisionGroup.SetActive(true);
        objSeasonGroup.SetActive(false);

        //스트링 코드 추가 후 작업
        //txtSeasonInfoTitle.text = TextManager.Instance.GetText("");
        //txtSeasonInfoContent.text = TextManager.Instance.GetText("");

        objReward.SetActive(false);
        u1Page = 0;
        RefreshPageButton();
    }

    public void RefreshPageButton()
    {
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        switch(u1Page)
        {
            case 0:
                btnPrev.interactable = false;
                btnNext.interactable = true;
                objSeason.SetActive(true);
                objDivisionGroup.SetActive(true);
                objSeasonGroup.SetActive(false);

                txtSeasonInfoTitle.text = TextManager.Instance.GetText("popup_league_desc_league_title");
                txtSeasonInfoContent.text = TextManager.Instance.GetText("popup_league_desc_league");

                objReward.SetActive(false);
                break;

            case 1:
                btnPrev.interactable = true;
                btnNext.interactable = true;
                objSeason.SetActive(true);
                objDivisionGroup.SetActive(false);
                objSeasonGroup.SetActive(true);
                
                txtSeasonInfoTitle.text = TextManager.Instance.GetText("popup_league_desc_league_title");
                txtSeasonInfoContent.text = TextManager.Instance.GetText("popup_league_desc_season");

                objReward.SetActive(false);
                break;

            case 2:
                btnPrev.interactable = true;
                btnNext.interactable = true;
                objSeason.SetActive(false);
                objDivisionGroup.SetActive(false);
                objSeasonGroup.SetActive(false);

                objReward.SetActive(true);
                
                txtRewardInfoTitle.text = TextManager.Instance.GetText("popup_league_desc_reward_frontline");
                tempStringBuilder.Append(
                String.Format(TextManager.Instance.GetText("popup_league_desc_reward_1"), 
                    LeagueInfoMgr.Instance.dicLeagueData[4506].u2RewardCount[0][0], LeagueInfoMgr.Instance.dicLeagueData[4506].u2RewardCount[0][1], LeagueInfoMgr.Instance.dicLeagueData[4506].u2RewardCount[0][2],
					LeagueInfoMgr.Instance.dicLeagueData[4506].u2RewardCount[1][0], LeagueInfoMgr.Instance.dicLeagueData[4506].u2RewardCount[1][1], LeagueInfoMgr.Instance.dicLeagueData[4506].u2RewardCount[1][2],
					LeagueInfoMgr.Instance.dicLeagueData[4506].u2RewardCount[2][0], LeagueInfoMgr.Instance.dicLeagueData[4506].u2RewardCount[2][1], 
					LeagueInfoMgr.Instance.dicLeagueData[4506].u2RewardCount[3][0], LeagueInfoMgr.Instance.dicLeagueData[4506].u2RewardCount[3][1], 
					LeagueInfoMgr.Instance.dicLeagueData[4506].u2RewardCount[4][0], LeagueInfoMgr.Instance.dicLeagueData[4506].u2RewardCount[4][1], 
					LeagueInfoMgr.Instance.dicLeagueData[4506].u2RewardCount[5][0], LeagueInfoMgr.Instance.dicLeagueData[4506].u2RewardCount[6][0]).ToString());

                txtRewardInfoContent.text = tempStringBuilder.ToString();
                break;

            case 3:
                btnPrev.interactable = true;
                btnNext.interactable = false;
                objSeason.SetActive(false);
                objDivisionGroup.SetActive(false);
                objSeasonGroup.SetActive(false);

                objReward.SetActive(true);
                
                txtRewardInfoTitle.text = TextManager.Instance.GetText("popup_league_desc_reward_frontline");
                tempStringBuilder.Append(
				String.Format(TextManager.Instance.GetText("popup_league_desc_reward_2").ToString(), LeagueInfoMgr.Instance.dicLeagueData[4505].u2RewardCount[6][0], LeagueInfoMgr.Instance.dicLeagueData[4504].u2RewardCount[6][0],
					LeagueInfoMgr.Instance.dicLeagueData[4503].u2RewardCount[6][0], LeagueInfoMgr.Instance.dicLeagueData[4502].u2RewardCount[6][0], LeagueInfoMgr.Instance.dicLeagueData[4501].u2RewardCount[6][0]).ToString());
                txtRewardInfoContent.text = tempStringBuilder.ToString();
                break;
        }
    }

    public void OnClickPrev()
    {
        if(u1Page > 0)
            u1Page--;

        RefreshPageButton();
    }

    public void OnClickNext()
    {
        if(u1Page <3)
            u1Page++;

        RefreshPageButton();
    }
}
