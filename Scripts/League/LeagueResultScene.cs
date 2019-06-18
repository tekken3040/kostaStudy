using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

public class LeagueResultScene : MonoBehaviour 
{
	[SerializeField] UI_DivisionMark cDivisionMark;
	[SerializeField] Image ResultTextImage;
	[SerializeField] Text VictoryPoint;
	[SerializeField] Text RewardGold;
	[SerializeField] GameObject InputObject;
	[SerializeField] InputField WinMessage;
	[SerializeField] UI_League_Slot[] _charSlot;

    public Text txtRewardPointCount;

    public GameObject objVictoryGoods;
	//Gradient PointGrad;

	void Start(){
		FadeEffectMgr.Instance.FadeIn();

		string resStr = "";
        //#ODIN [리그 결과에 따른 오딘 포인트를 담을 지역 변수 선언]
        Goods resultOdinPoint = null;
		if (Legion.Instance.u1LastBattleResult == 0)
        {
			resStr = "defeat";
			StartCoroutine(PlayResultSound("Dea"));

            InputObject.SetActive (false);
            objVictoryGoods.SetActive(false);

            resultOdinPoint = Legion.Instance.SelectedLeague.arrResultOidnPoint[(int)LeagueInfo.LeagueResultType.LOSE];
		}
        else if (Legion.Instance.u1LastBattleResult == 1)
        {
			resStr = "victory";
			StartCoroutine(PlayResultSound("Vic"));

            InputObject.SetActive (true);
            objVictoryGoods.SetActive(true);
            RewardGold.text = Legion.Instance.SelectedLeague.cWinReward.u4Count.ToString();

            resultOdinPoint = Legion.Instance.SelectedLeague.arrResultOidnPoint[(int)LeagueInfo.LeagueResultType.WIN];
		}
        else if (Legion.Instance.u1LastBattleResult == 2)
        {
			resStr = "draw";

            objVictoryGoods.SetActive(false);
            InputObject.SetActive (false);

            resultOdinPoint = Legion.Instance.SelectedLeague.arrResultOidnPoint[(int)LeagueInfo.LeagueResultType.DRAW];
        }

        //#ODIN [리그 결과 오딘 포인트 추가 및 UI 셋팅]
        if (resultOdinPoint != null)
        {
            Legion.Instance.AddGoods(resultOdinPoint);
            // UI 셋팅
            txtRewardPointCount.text = resultOdinPoint.u4Count.ToString();
        }

        cDivisionMark.SetDivisionMark(Legion.Instance.GetDivision);

		if(Legion.Instance.u2LastBattleResultPoint > 0) VictoryPoint.text = "+"+Legion.Instance.u2LastBattleResultPoint.ToString();
		VictoryPoint.text = Legion.Instance.u2LastBattleResultPoint.ToString();

		LeanTween.scale (VictoryPoint.rectTransform, Vector3.one*1.3f, 0.2f).setDelay (0.5f).setLoopPingPong(1);

		ResultTextImage.sprite = AtlasMgr.Instance.GetSprite ("Sprites/BattleField/league_03.league_result_" + resStr);
		ResultTextImage.SetNativeSize ();

		LeanTween.alpha (ResultTextImage.rectTransform, 1f, 0.1f).setDelay (0.3f);
		LeanTween.scale (ResultTextImage.rectTransform, Vector3.one, 0.15f).setDelay (0.3f).setEase(LeanTweenType.easeOutBack);

		for(int i=0; i<Legion.Instance.cLeagueCrew.acLocation.Length; i++)
		{
			if(Legion.Instance.cLeagueCrew.acLocation[i] != null)
				//_charSlot[i].SetData((Byte)(((Hero)Legion.Instance.cLeagueCrew.acLocation[i]).u1Index-1));
                _charSlot[i].SetData((Byte)i);
			else
				_charSlot[i].SetData(0, true);
		}

        PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.Gold);
        PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.LeagueMatch);
	}

	IEnumerator PlayResultSound(string rStr)
	{
		yield return new WaitForSeconds (0.3f);
		SoundManager.Instance.PlayEff ("Sound/UI/12. Battle/UI_Leag_"+rStr);
	}

	public void OnClickLeague()
	{
        //if (WinMessage.text == "")
        //{
        //    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_create_guild"), TextManager.Instance.GetText("err_desc_guild_empty_name"), null);
        //    return;
        //}
        //else if (Regex.Matches(WinMessage.text, @"[\s\Wㄱ-ㅎㅏ-ㅣ]").Count != 0)
        //{
        //    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_league_desc_league_title"), TextManager.Instance.GetErrorText("CREATE_ACCOUNT_ID_WRONGCHAR", "", /false),/ null);
        //    return;
        //}

		if (WinMessage.text.Trim() != "")
        {
			PopupManager.Instance.ShowLoadingPopup (1);
			Server.ServerMgr.Instance.RequestLeagueRevengeMessage (WinMessage.text, GetLeagueInfoResult);
		}

        else
        {
			StartCoroutine(ChangeSceneFade());
		}

		Legion.Instance.u2LastBattleResultPoint = 0;
		Legion.Instance.u1LastBattleResult = 100;
	}

	void GetLeagueInfoResult(Server.ERROR_ID err){
		if(err != Server.ERROR_ID.NONE) 
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.LEAGUE_REVENGEMESSAGE, err), Server.ServerMgr.Instance.CallClear);
			return;
		}
		else 
		{
			StartCoroutine(ChangeSceneFade());
		}
	}

	IEnumerator ChangeSceneFade()
	{
		PopupManager.Instance.CloseLoadingPopup();
		FadeEffectMgr.Instance.FadeOut();
		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
		AssetMgr.Instance.SceneLoad("ALeagueScene", false);
	}
}
