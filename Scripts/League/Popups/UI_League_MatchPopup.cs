using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;

public class UI_League_MatchPopup : MonoBehaviour
{
    [SerializeField] GameObject objCharInfoPopup;
    [SerializeField] GameObject objEnemyCrewInfopopup;
    [SerializeField] Text txtCrewName;
    [SerializeField] Text txtEnemyCrewName;
    [SerializeField] Text txtCrewPower;
    [SerializeField] Text txtEnemyCrewPower;
    [SerializeField] UI_CharCursorElement[] MyCrewSlots;
    [SerializeField] UI_CharCursorElement[] EnemySlots;
    StringBuilder tempStringBuilder;

    void Awake()
    {
        tempStringBuilder = new StringBuilder();
    }

    void OnEnable()
    {
        Init();
    }

    void Init()
    {
        for(int i=0; i<LeagueCrew.MAX_CHAR_IN_CREW; i++)
        {
            if(Legion.Instance.cLeagueCrew.acLocation[i] == null)
                MyCrewSlots[i].gameObject.SetActive(false);
            else
            {
                MyCrewSlots[i].gameObject.SetActive(true);
                MyCrewSlots[i].SetData((Hero)Legion.Instance.cLeagueCrew.acLocation[i]);
            }
            if(UI_League.Instance.EnemyCrew.acLocation[i] == null)
                EnemySlots[i].gameObject.SetActive(false);
            else
            {
                EnemySlots[i].gameObject.SetActive(true);
                EnemySlots[i].SetData((Hero)UI_League.Instance.EnemyCrew.acLocation[i]);
            }
        }
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
		tempStringBuilder.Append(TextManager.Instance.GetText("mark_league_power")).Append(" ").Append(Legion.Instance.cLeagueCrew.u4Power);
        txtCrewPower.text = tempStringBuilder.ToString();

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
		tempStringBuilder.Append(TextManager.Instance.GetText("mark_league_power")).Append(" ").Append(UI_League.Instance.EnemyCrew.u4Power);
        txtEnemyCrewPower.text = tempStringBuilder.ToString();

        txtCrewName.text = Legion.Instance.sName;
        txtEnemyCrewName.text = UI_League.Instance.sEnemyName;
    }

    public void OnClickEnemyCrewInfo()
    {
        objEnemyCrewInfopopup.SetActive(true);
    }

    public void OnClickBattleStart()
    {
        if(!Legion.Instance.CheckEnoughGoods((int)GoodsType.LEAGUE_KEY, LeagueInfoMgr.Instance.dicLeagueData[Legion.Instance.GetDivisionID].cPlayGoods.u4Count))
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_league_key_not"), TextManager.Instance.GetText("popup_desc_league_key_not"), null);
            return;
        }
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestLeagueMatchStart(StartLeague);
    }

    public void StartLeague(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(((int)err).ToString()), Server.ServerMgr.Instance.CallClear);
			return;
        }
        else if(err == Server.ERROR_ID.NONE)
        {
            UI_League.Instance.u8SelectEnemyCrewSN = 0;
            //UI_League.Instance.u1SelectEnemyCrewRevenge = 0;
            StartCoroutine(ChangeScene());
        }
	}


	private IEnumerator ChangeScene()
	{
		FadeEffectMgr.Instance.FadeOut(1f);
		yield return new WaitForSeconds(1f);
		AssetMgr.Instance.SceneLoad("ALeagueLoading");
	}
}
