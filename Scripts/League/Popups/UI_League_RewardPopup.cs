using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_League_RewardPopup : MonoBehaviour
{
    [SerializeField] Text txtSeasonTitle;
	[SerializeField] UI_DivisionMark cDivisionIcon;
    [SerializeField] Image imgDivisionTextIcon;
    [SerializeField] Image imgDivisionTextIcon2;
    [SerializeField] GameObject objSeasonRwd;
    
    [SerializeField] Text txtPromoTitle;
    [SerializeField] Text txtPromoContent;
    [SerializeField] GameObject objPromoRwd;

    [SerializeField] UI_League_Reward_ItemSlot[] _seasonItemSlots;
    [SerializeField] UI_League_Reward_ItemSlot[] _promotedItemSlots;
    
    int proRwdCnt = 0;

    void OnEnable()
    {
        if(UI_League.Instance._leagueReward.u1DivRwdCount != 0)
        {
            objSeasonRwd.SetActive(true);
            objPromoRwd.SetActive(false);
            RefreashRwd();
        }
        else if(UI_League.Instance._leagueReward.u1PromotionCount != 0)
        {
            objSeasonRwd.SetActive(false);
            objPromoRwd.SetActive(true);
            FirstProRwd();
        }
    }


    void OnDisable()
    {
        proRwdCnt = 0;
        objSeasonRwd.SetActive(false);
        objPromoRwd.SetActive(false);
    }

    public void OnClickClose()
    {
        if(proRwdCnt < UI_League.Instance._leagueReward.u1PromotionCount)
        {
            StartCoroutine(RefreashRwdPopup(1));
        }
        else
            gameObject.SetActive(false);
    }

    IEnumerator RefreashRwdPopup(int _rwdType)
    {
        objSeasonRwd.SetActive(false);
        objPromoRwd.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        if(_rwdType == 0)
            RefreashRwd();
        else
            RefreashProRwd();
    }

    void RefreashRwd()
    {
        objSeasonRwd.SetActive(true);
        for(int i=0; i<3; i++)
        {
            _seasonItemSlots[i].gameObject.SetActive(false);
        }

        //시즌보상 타이틀
		txtSeasonTitle.text = string.Format(TextManager.Instance.GetText("popup_title_league_season_end"), UI_League.Instance.cLeagueMatchList.u4PrevMyRank);

        //시즌보상 내용
		byte prevDivision = UI_League.Instance.cLeagueMatchList.u1PrevDivisionIndex;
		// 디비전 전설에 랭크 1위라면 챔피언 디비전으로 승급
		if(prevDivision == 6 && UI_League.Instance.cLeagueMatchList.u4PrevMyRank == 1)
			++prevDivision;
		if(prevDivision == 0)
            prevDivision = 1;
        if(Legion.Instance.GetDivision > 5)
        {
            imgDivisionTextIcon.gameObject.SetActive(true);
            imgDivisionTextIcon2.gameObject.SetActive(false);
			imgDivisionTextIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/BattleField/league_06.division_t_"+prevDivision);
            imgDivisionTextIcon.SetNativeSize();
        }
        else
        {
            imgDivisionTextIcon.gameObject.SetActive(false);
            imgDivisionTextIcon2.gameObject.SetActive(true);
			imgDivisionTextIcon2.sprite = AtlasMgr.Instance.GetSprite("Sprites/BattleField/league_06.division_t_"+prevDivision);
            imgDivisionTextIcon2.SetNativeSize();
        }
		//디비전 아이콘
		cDivisionIcon.SetDivisionMark(prevDivision);
        
		//imgDivisionIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/BattleField/league_06.division_"+);
        //imgDivisionIcon.SetNativeSize();
		for (int i = 0; i < _seasonItemSlots.Length; i++)
			_seasonItemSlots [i].gameObject.SetActive (false);
        for(int i=0; i<UI_League.Instance._leagueReward.u1DivRwdCount; i++)
        {
			if(UI_League.Instance._leagueReward.lstDivRwdItem[i].u1DivRwdType == 0)
            {
                _seasonItemSlots[i].gameObject.SetActive(false);
				continue;
            }
			
            _seasonItemSlots[i].gameObject.SetActive(true);
            _seasonItemSlots[i].SetData(new Goods(UI_League.Instance._leagueReward.lstDivRwdItem[i].u1DivRwdType, 
                UI_League.Instance._leagueReward.lstDivRwdItem[i].u2DivRwdID, 
                UI_League.Instance._leagueReward.lstDivRwdItem[i].u4DivRwdNumber));
        }
    }


	void FirstProRwd()
	{
		for(int i=0; i<3; i++)
		{
			_promotedItemSlots[i].gameObject.SetActive(false);
		}
		//승격보상 타이틀
		txtPromoTitle.text = TextManager.Instance.GetText("popup_title_league_result_up");

		// 현재 리그 등급을 받아옴
		int rewardDivision = Legion.Instance.GetDivision;
		// 챔피언은 7이 넘어오며 챔피언은 비공식 리그 등급이라 전설등급으로 하락시키고
		if(rewardDivision >= 7)
			--rewardDivision;

		// 현재 리그 - (승급 보상 갯수 -1) + 받은 겟수; 
		rewardDivision = rewardDivision - (UI_League.Instance._leagueReward.u1PromotionCount-1) + proRwdCnt;

		//승격보상 내용
		txtPromoContent.text = string.Format(TextManager.Instance.GetText("popup_desc_league_resuit_up"), TextManager.Instance.GetText("mark_division_" + rewardDivision));
		for(int i=0; i<UI_League.Instance._leagueReward.lstProRwdItem[0].u1ProRwdCount; i++)
		{
			if(UI_League.Instance._leagueReward.lstProRwdItem[0].lstProRwdItem[i].u1DivRwdType == 0)
				continue;

			_promotedItemSlots[i].gameObject.SetActive(true);
			_promotedItemSlots[i].SetData(new Goods(UI_League.Instance._leagueReward.lstProRwdItem[0].lstProRwdItem[i].u1DivRwdType,
				UI_League.Instance._leagueReward.lstProRwdItem[0].lstProRwdItem[i].u2DivRwdID,
				UI_League.Instance._leagueReward.lstProRwdItem[0].lstProRwdItem[i].u4DivRwdNumber));
		}
		proRwdCnt++;
	}
     
    void RefreashProRwd()
    {
        objPromoRwd.SetActive(true);
        for(int i=0; i<3; i++)
        {
            _promotedItemSlots[i].gameObject.SetActive(false);
        }
        //승격보상 타이틀
        txtPromoTitle.text = TextManager.Instance.GetText("popup_title_league_result_up");

		// 현재 리그 등급을 받아옴
		int rewardDivision = Legion.Instance.GetDivision;
		// 챔피언은 7이 넘어오며 챔피언은 비공식 리그 등급이라 전설등급으로 하락시키고
		if(rewardDivision >= 7)
			--rewardDivision;

		// 현재 리그 - (승급 보상 갯수 -1) + 받은 겟수; 
		rewardDivision = rewardDivision - (UI_League.Instance._leagueReward.u1PromotionCount-1) + proRwdCnt;

        //승격보상 내용
		txtPromoContent.text = string.Format(TextManager.Instance.GetText("popup_desc_league_resuit_up"), TextManager.Instance.GetText("mark_division_" + rewardDivision));
        for(int i=0; i<UI_League.Instance._leagueReward.lstProRwdItem[proRwdCnt].u1ProRwdCount; i++)
        {
			if(UI_League.Instance._leagueReward.lstProRwdItem[proRwdCnt].lstProRwdItem[i].u1DivRwdType == 0)
				continue;
			
            _promotedItemSlots[i].gameObject.SetActive(true);
            _promotedItemSlots[i].SetData(new Goods(UI_League.Instance._leagueReward.lstProRwdItem[proRwdCnt].lstProRwdItem[i].u1DivRwdType,
                UI_League.Instance._leagueReward.lstProRwdItem[proRwdCnt].lstProRwdItem[i].u2DivRwdID,
                UI_League.Instance._leagueReward.lstProRwdItem[proRwdCnt].lstProRwdItem[i].u4DivRwdNumber));
        }
        proRwdCnt++;
    }
}
