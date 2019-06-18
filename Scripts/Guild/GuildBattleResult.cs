using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;

public class GuildBattleResult : MonoBehaviour 
{
	[SerializeField] Image ResultTextImage;
	[SerializeField] Text txtRank;
	[SerializeField] Text VictoryPoint;
	[SerializeField] Text RewardOdin;
	[SerializeField] GameObject VictoryObj;
	[SerializeField] UI_CharCursorElement[] _Slots;

	[SerializeField] Text GuildDeckPower;
	[SerializeField] Text GuildName;

	void Start(){
		FadeEffectMgr.Instance.FadeIn();

		txtRank.text = GuildInfoMgr.Instance.GetGuildTier(GuildInfoMgr.Instance.cGuildMemberInfo.u2Score).ToString();
		GuildDeckPower.text = TextManager.Instance.GetText("mark_power")+" "+GuildInfoMgr.Instance.GetUserDeckPower().ToString ();
		GuildName.text = GuildInfoMgr.Instance.cGuildMemberInfo.strGuildName;

		string resStr = "";
		//#ODIN [길드전 결과에 따른 오딘 포인트를 담을 지역 변수 선언]
		Goods resultOdinPoint = null;
		if (Legion.Instance.u1LastBattleResult == 0 || Legion.Instance.u1LastBattleResult == 2) {
			resStr = "defeat";
			StartCoroutine(PlayResultSound("Dea"));
			resultOdinPoint = GuildInfoMgr.Instance.cGuildInfo.gDefeatOdinGoods;
		} else if (Legion.Instance.u1LastBattleResult == 1) {
			resStr = "victory";
			StartCoroutine(PlayResultSound("Vic"));
			resultOdinPoint = GuildInfoMgr.Instance.cGuildInfo.gWinOdinGoods;
		}

		//#ODIN [길드전 결과 오딘 포인트 추가 및 UI 셋팅]
		if (resultOdinPoint != null && resultOdinPoint.u1Type != 0)
		{
            VictoryObj.SetActive(true);
            Legion.Instance.AddGoods(resultOdinPoint);
            // UI 셋팅
            RewardOdin.text = resultOdinPoint.u4Count.ToString();
        }
		else
		{
            // 오딘 포인트 UI 비활성화
            VictoryObj.SetActive(false);
        }

        if (Legion.Instance.u2LastBattleResultPoint > 0) VictoryPoint.text = "+"+Legion.Instance.u2LastBattleResultPoint.ToString();
		VictoryPoint.text = Legion.Instance.u2LastBattleResultPoint.ToString();

		LeanTween.scale (VictoryPoint.rectTransform, Vector3.one*1.3f, 0.2f).setDelay (0.5f).setLoopPingPong(1);

		ResultTextImage.sprite = AtlasMgr.Instance.GetSprite ("Sprites/BattleField/league_03.league_result_" + resStr);
		ResultTextImage.SetNativeSize ();

		LeanTween.alpha (ResultTextImage.rectTransform, 1f, 0.1f).setDelay (0.3f);
		LeanTween.scale (ResultTextImage.rectTransform, Vector3.one, 0.15f).setDelay (0.3f).setEase(LeanTweenType.easeOutBack);

		for(int i=0; i<GuildInfoMgr.Instance.cUserCrews.Length; i++)
		{
			for (int j = 0; j < GuildInfoMgr.Instance.cUserCrews [i].acLocation.Length; j++) {
				if (GuildInfoMgr.Instance.cUserCrews [i].acLocation [j] != null)
					_Slots [(i*3)+j].SetData ((Hero)GuildInfoMgr.Instance.cUserCrews [i].acLocation [j]);
				else
					_Slots [(i*3)+j].gameObject.SetActive (false);
			}
		}
	}

	IEnumerator PlayResultSound(string rStr)
	{
		yield return new WaitForSeconds (0.3f);
		SoundManager.Instance.PlayEff ("Sound/UI/12. Battle/UI_Leag_"+rStr);
	}

	public void OnClickGuild()
	{
		StartCoroutine(ChangeSceneFade());

		Legion.Instance.u2LastBattleResultPoint = 0;
		Legion.Instance.u1LastBattleResult = 100;
	}

	IEnumerator ChangeSceneFade()
	{
		PopupManager.Instance.CloseLoadingPopup();
		FadeEffectMgr.Instance.FadeOut();
		yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
		AssetMgr.Instance.SceneLoad("GuildScene", false);
	}
}
