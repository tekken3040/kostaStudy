using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class UI_League_ListSlot : MonoBehaviour
{
    [SerializeField] Text _txtRank;
    [SerializeField] Text _txtCrewName;
    [SerializeField] Text _txtRankPoint;
    [SerializeField] Text _txtTountMsg;
    [SerializeField] Text _txtLeftTime;
    [SerializeField] Text _txtBtnMatch;

    [SerializeField] Image _imgBgGradient;
    [SerializeField] Image _imgBgClassIcon;
    [SerializeField] Image _imgClassIcon;
    [SerializeField] Image _imgEdgeClassIcon;
    [SerializeField] Image _imgBtnMatch;

    [SerializeField] GameObject _objTalkBlank;
    [SerializeField] GameObject _objLeftTime;
    [SerializeField] GameObject _objBgGradient;
    [SerializeField] GameObject _objBtnMatch;

    Sprite[] _matchBtnImg;
    Sprite[] _gradientBGImg;

    StringBuilder tempStringBuilder;
    LeagueMatchList.ListSlotData _slotData;
    LeagueScene _leagueScene;

    void Awake()
    {
        tempStringBuilder = new StringBuilder();
        _matchBtnImg = new Sprite[2];
        _matchBtnImg[0] = AtlasMgr.Instance.GetSprite("Sprites/Common/common_01_renew.btn_on_w116");
        _matchBtnImg[1] = AtlasMgr.Instance.GetSprite("Sprites/Common/common_01_renew.btn_on_w106");
        _gradientBGImg = new Sprite[2];
        _gradientBGImg[0] = AtlasMgr.Instance.GetSprite("Sprites/BattleField/league_03.gradient_yellow");
        _gradientBGImg[1] = AtlasMgr.Instance.GetSprite("Sprites/BattleField/league_03.gradient_red");
    }

    void OnDisable()
    {
        StopCoroutine("CheckLeftTime");
    }

    public bool SetData(LeagueMatchList.ListSlotData _listSlotData, LeagueScene _scene)
    {
		bool bMy = false;
        _slotData = _listSlotData;
        _leagueScene = _scene;
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
		tempStringBuilder.Append(_slotData.u4Rank).Append(" ").Append(TextManager.Instance.GetText("mark_league_rank"));
        _txtRank.text = tempStringBuilder.ToString();
        _txtCrewName.text = _slotData.strLegionName;
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
		tempStringBuilder.Append(_slotData.u4Point).Append(" ").Append(TextManager.Instance.GetText("mark_league_point"));
        _txtRankPoint.text = tempStringBuilder.ToString();
        _txtTountMsg.text = _slotData.strRevengeMessage;
        if(_slotData.u4Rank<=3)
            _imgEdgeClassIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/BattleField/league_03.btn_gold");
        else
            _imgEdgeClassIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/BattleField/league_03.btn_silver");

        if(_slotData.u4Rank<=3)
            _imgBgClassIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/BattleField/league_03.rect_red");
        else if(_slotData.strLegionName == Legion.Instance.sName)
            _imgBgClassIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/BattleField/league_03.rect_green");
        else
            _imgBgClassIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/BattleField/league_03.rect_gray");

        _imgClassIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/BattleField/league_04.char_"+_slotData.u2ClassID);
        _imgClassIcon.SetNativeSize();

        if(_txtTountMsg.text == "")
            _objTalkBlank.SetActive(false);

        tsRevengeLeftTime = new TimeSpan(_slotData.dtTime.Ticks - Legion.Instance.ServerTime.Ticks);
        if(_slotData.strLegionName == Legion.Instance.sName)
        {
            _objBtnMatch.SetActive(false);
            _objLeftTime.SetActive(false);
            _objBgGradient.SetActive(true);
            _imgBgGradient.sprite = _gradientBGImg[0];
            UI_League.Instance.MyLeagueCrewSlot = gameObject;
			bMy = true;
        }
        else if(_slotData.u8Time != 0)
        {
            _objBtnMatch.SetActive(false);
            _objLeftTime.SetActive(true);
            _objBgGradient.SetActive(false);
            StartCoroutine("CheckLeftTime");
        }
        else if(_slotData.u1Revenge == 0)
        {
            _objBtnMatch.SetActive(true);
            _objLeftTime.SetActive(false);
			_txtBtnMatch.text = TextManager.Instance.GetText("btn_league_battle");//"도전";
            _imgBtnMatch.sprite = _matchBtnImg[0];
            _objBgGradient.SetActive(false);
        }
        else
        {
            _objBtnMatch.SetActive(true);
            _objLeftTime.SetActive(false);
			_txtBtnMatch.text = TextManager.Instance.GetText("btn_league_revange");//"복수";
            _imgBtnMatch.sprite = _matchBtnImg[1];
            _objBgGradient.SetActive(true);
            _imgBgGradient.sprite = _gradientBGImg[1];
        }

		return bMy;
    }

    public void OnClickTalkBlank()
    {
        _objTalkBlank.SetActive(false);
    }
    public void OnClickMatch()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestLeagueMatch(_slotData.u8UserSN, _slotData.u1Revenge, ReceiveMatchPlayer);
    }

    public void ReceiveMatchPlayer(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(((int)err).ToString()), Server.ServerMgr.Instance.CallClear);
			return;
        }
        else if(err == Server.ERROR_ID.NONE)
        {
            UI_League.Instance.CreateEnemyCrew();
            //StartLeague();
            UI_League.Instance.u1SelectEnemyCrewRevenge = _slotData.u1Revenge;
            UI_League.Instance.u8SelectEnemyCrewSN = _slotData.u8UserSN;
            _leagueScene.ShowBattleScreen();
        }
    }
    public void StartLeague()
    {
        if(!Legion.Instance.CheckEnoughGoods((int)GoodsType.LEAGUE_KEY, LeagueInfoMgr.Instance.dicLeagueData[Legion.Instance.GetDivisionID].cPlayGoods.u4Count))
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_league_key_not"), TextManager.Instance.GetText("popup_desc_league_key_not"), null);
            return;
        }
        UI_League.Instance.u8SelectEnemyCrewSN = 0;
        UI_League.Instance.u1SelectEnemyCrewRevenge = 0;
		StartCoroutine(ChangeScene());
	}

	private IEnumerator ChangeScene()
	{
		FadeEffectMgr.Instance.FadeOut(1f);
		yield return new WaitForSeconds(1f);
		AssetMgr.Instance.SceneLoad("ALeagueLoading");
	}
    TimeSpan tsRevengeLeftTime;
    private IEnumerator CheckLeftTime()
    {
        while (true)
        {
            if(tsRevengeLeftTime.TotalSeconds <= 0)
            {
                if(_slotData.u1Revenge == 0)
                {
                    _objBtnMatch.SetActive(true);
                    _objLeftTime.SetActive(false);
					_txtBtnMatch.text = TextManager.Instance.GetText("btn_league_battle");
                    _imgBtnMatch.sprite = _matchBtnImg[0];
                }
                else
                {
                    _objBtnMatch.SetActive(true);
                    _objLeftTime.SetActive(false);
					_txtBtnMatch.text = TextManager.Instance.GetText("btn_league_revange");
                    _imgBtnMatch.sprite = _matchBtnImg[1];
                }
                yield break;
            }
            tsRevengeLeftTime = tsRevengeLeftTime.Subtract(TimeSpan.FromSeconds(1f));
            //_txtLeftTime.text = tsRevengeLeftTime.Duration().ToString();
            _txtLeftTime.text = String.Format("{0:00}:{1:00}", tsRevengeLeftTime.Minutes, tsRevengeLeftTime.Seconds);
            yield return StartCoroutine(Utillity.WaitForRealSeconds(1f));
        }
    }
}
