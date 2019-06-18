using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using CodeStage.AntiCheat.ObscuredTypes;

public class BattleFieldPanel : MonoBehaviour
{
    //public GameObject _mainPanel;
    public GameObject TopMenu;
    public GameObject ListMenu;
    public GameObject RankWindows;
    public GameObject _rankBoard;
    public GameObject _rewardWindow;
    public GameObject Btn_RecvRankReward;
    public GameObject objBossRushLock;
    public GameObject[] Tabs;               //0:리그, 1:랭크, 2:레이드
    public GameObject[] Alrams;             //0:리그, 1:랭크, 2:레이드, 3:랭크보상, 4:리그메뉴, 5:랭크메뉴, 6:레이드메뉴
    public Toggle[] Tgl_Menu;

    public Button[] _MenuBtns;  //0:리그, 1:랭크, //2:레이드
    public Text[] _txtMenuOpenCondition;

    StringBuilder tempStringBuilder;

    public void OnEnable()
    {
        tempStringBuilder = new StringBuilder();
        FadeEffectMgr.Instance.FadeIn(FadeEffectMgr.GLOBAL_FADE_TIME);
        PopupManager.Instance.AddPopup(this.gameObject, OnClickBack);

        if (Legion.Instance.sName == "")
            ListMenu.transform.GetChild(0).GetComponent<Button>().interactable = false;
        else
            ListMenu.transform.GetChild(0).GetComponent<Button>().interactable = true;

        for (int i = 0; i < _MenuBtns.Length; ++i)
        {
            if (CheckContentOpen(i) == true)
            {
                _MenuBtns[i].interactable = true;
                _txtMenuOpenCondition[i].gameObject.SetActive(false);

                if (i == 0)
                {
                    if (!ObscuredPrefs.GetBool("RegistLeague", false))
                    {
                        Alrams[0].SetActive(true);
                        Alrams[4].SetActive(true);
                    }
                }
            }
            else
            {
                _MenuBtns[i].interactable = false;
                Goods conditionsInfo = GetContentInfo(i);
                if (conditionsInfo != null)
                {
                    tempStringBuilder.Remove(0, tempStringBuilder.Length);
                    _txtMenuOpenCondition[i].gameObject.SetActive(true);
                    tempStringBuilder.Append(TextManager.Instance.GetText("open_contents_league_crew")).Append(" ").Append(
                        string.Format(TextManager.Instance.GetText("open_contetns"),
                                                Legion.Instance.GetConsumeString(conditionsInfo.u1Type),
                                                conditionsInfo.u4Count.ToString()));
                    _txtMenuOpenCondition[i].text = tempStringBuilder.ToString();
                }
                else
                    _txtMenuOpenCondition[i].gameObject.SetActive(false);
            }
        }
        
        if(Legion.Instance.u1RankRewad == 1)
        {
            Alrams[1].SetActive(true);
            Alrams[3].SetActive(true);
            Alrams[5].SetActive(true);
            Btn_RecvRankReward.GetComponent<Button>().interactable = true;
        }
        else
        {
            Alrams[1].SetActive(false);
            Alrams[3].SetActive(false);
            Alrams[5].SetActive(false);
            Btn_RecvRankReward.GetComponent<Button>().interactable = false;
        }
        //Tgl_Menu[0].interactable = false;
        //Tgl_Menu[2].interactable = false;
        if(!EventInfoMgr.Instance.dicEventReward.ContainsKey(EventInfoMgr.Instance.lstBossRush[0].u2EventID))
        {
            _MenuBtns[2].GetComponent<Button>().interactable = false;
            objBossRushLock.SetActive(true);
        }
        else if(StageInfoMgr.Instance.OpenBossRush == 0)
        {
            _MenuBtns[2].GetComponent<Button>().interactable = false;
            objBossRushLock.SetActive(true);
        }
        else
        {
            _MenuBtns[2].GetComponent<Button>().interactable = true;
            objBossRushLock.SetActive(false);
        }
    }

    public void OnClickBack()
    {
        StartCoroutine(OpenMainPanel());
        PopupManager.Instance.RemovePopup(this.gameObject);
    }

    IEnumerator OpenMainPanel()
    {
        RankInfoMgr.Instance.ClearData();
        FadeEffectMgr.Instance.FadeOut(FadeEffectMgr.GLOBAL_FADE_TIME);
        yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
        //this.transform.parent.GetComponent<LobbyScene>().RefreshAlram();
        Scene.GetCurrent().RefreshAlram();
        for(int i=0; i<Tabs.Length; i++)
            Tabs[i].gameObject.SetActive(false);
        TopMenu.SetActive(false);
        ListMenu.SetActive(true);
        _rankBoard.SetActive(false);
        RankWindows.SetActive(false);
        //_mainPanel.SetActive(true);
        //Scene.GetCurrent().mainPanel.SetActive(true);
		((LobbyScene)Scene.GetCurrent()).SetMenuHideButtonEnable(true); //.mainPanel.GetComponent<Toggle>().interactable = true;
        //this.gameObject.SetActive(false);
        FadeEffectMgr.Instance.FadeIn(FadeEffectMgr.GLOBAL_FADE_TIME);
        Destroy(this.gameObject);
    }

    public void OnClickRecvRankReward()
    {
		if(RankInfoMgr.Instance.u1RankRewardCount <= 0 )
			return;
			
		for(UInt16 i = 0; i < RankInfoMgr.Instance.u1RankRewardCount; ++i)
		{
			RankReward reward = RankInfoMgr.Instance.dicRankRewardData[i];
			RankInfo rankInfo = RankInfoMgr.Instance.dicRankData[reward.u1RankType];
			if(Legion.Instance.CheckGoodsLimitExcessx(rankInfo.u1RewardType1[reward.u1RewardIndex-1]) == true)
			{
				Legion.Instance.ShowGoodsOverMessage(rankInfo.u1RewardType1[reward.u1RewardIndex-1]);
				return;
			}
			if(Legion.Instance.CheckGoodsLimitExcessx(rankInfo.u1RewardType2[reward.u1RewardIndex-1]) == true)
			{
				Legion.Instance.ShowGoodsOverMessage(rankInfo.u1RewardType2[reward.u1RewardIndex-1]);
				return;
			}
		}

        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestRankReward(RecvRankReward);
    }

    public void RecvRankReward(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.RANK_REWARD,err), Server.ServerMgr.Instance.CallClear);
		}
        else
        {
            if(RankInfoMgr.Instance.dicRankRewardData.Count != 0)
            {
                if(RankInfoMgr.Instance.dicRankRewardData[0].u1RewardIndex == 0)
                    _rewardCount++;
                OpenRewardWindow((UInt16)_rewardCount);
                _rewardCount++;
            }
            Btn_RecvRankReward.GetComponent<Button>().interactable = false;
        }
    }

    int _rewardCount = 0;
    public void OpenRewardWindow(UInt16 _cnt)
    {
        _rewardWindow.SetActive(true);
        _rewardWindow.GetComponent<RankRewardRecvWindow>().SetData(RankInfoMgr.Instance.dicRankRewardData[_cnt], _cnt);
    }

    public void OnClickCloseRewardWindow()
    {
        _rewardWindow.SetActive(false);
        if(!RankInfoMgr.Instance.dicRankRewardData.ContainsKey((UInt16)_rewardCount))
            return;
        if(RankInfoMgr.Instance.dicRankRewardData[(UInt16)_rewardCount].u1RewardIndex == 0)
        {
            _rewardCount++;
            OnClickCloseRewardWindow();
            return;
        }
        if(_rewardCount < RankInfoMgr.Instance.u1RankRewardCount)
        {
            OpenRewardWindow((UInt16)_rewardCount);
            _rewardCount++;
        }
        else
        {
            RankInfoMgr.Instance.dicRankRewardData.Clear();
            RankInfoMgr.Instance.u1RankRewardCount = 0;
            _rewardCount = 0;
            Legion.Instance.u1RankRewad = 0;
            Alrams[1].SetActive(false);
            Alrams[3].SetActive(false);
        }
    }

    public void OnClickTopMenu(int _index)
    {
        switch(_index)
        {
            case 0:
				if (AssetMgr.Instance.CheckDivisionDownload(6,0)) {
					return;
				}
                //Tabs[0].SetActive(true);
                //Tabs[1].SetActive(false);
                //Tabs[2].SetActive(false);
                //RankWindows.SetActive(false);
                AssetMgr.Instance.SceneLoad("ALeagueScene", false);
                break;

            case 1:
                Tabs[0].SetActive(false);
                Tabs[1].SetActive(true);
                Tabs[2].SetActive(false);
                RankWindows.SetActive(true);
                if(Legion.Instance.u1RankRewad == 1)
                {
                    Alrams[1].SetActive(true);
                    Alrams[3].SetActive(true);
                }
                else
                {
                    Alrams[1].SetActive(false);
                    Alrams[3].SetActive(false);
                }
                break;

            case 2:
                //Tabs[0].SetActive(false);
                //Tabs[1].SetActive(false);
                //Tabs[2].SetActive(true);
                //RankWindows.SetActive(false);
                if(AssetMgr.Instance.CheckDivisionDownload(7, EventInfoMgr.Instance.lstBossRush[0].u2EventID))
                {
					return;
				}
                if(StageInfoMgr.Instance.OpenBossRush == 0)
                {
                    //PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("event_bossrush_title"), TextManager.Instance.GetText("event_end_time"), null);
                    return;
                }
                else
                {
                    AssetMgr.Instance.SceneLoad("BossRushScene");
                }
                break;

			case 3:
				if (Legion.Instance.sName == "") {
					PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("menu_guild"), TextManager.Instance.GetText("popup_desc_guild_lock"), null);
					return;
				}

				AssetMgr.Instance.SceneLoad("GuildScene");
				break;
        }
    }

    public void OnClickListMenu(int _index)
    {
        // 2017. 01. 25 jy
        // 컨텐츠 오픈 여부 확인
        if (CheckContentOpen(_index) == false)
            return;
        
		if (_index == 0)
        {
			if (AssetMgr.Instance.CheckDivisionDownload(6,0))
            {
				return;
			}
		}
        else if(_index == 2)
        {
            if(AssetMgr.Instance.CheckDivisionDownload(7, EventInfoMgr.Instance.lstBossRush[0].u2EventID))
                return;
        }
        TopMenu.SetActive(true);
        ListMenu.SetActive(false);
        switch(_index)
        {
		case 0:
				Tabs [0].SetActive (true);
				Tabs [1].SetActive (false);
				Tabs [2].SetActive (false);
				Tgl_Menu [0].isOn = true;
				Tgl_Menu [1].isOn = false;
				Tgl_Menu [2].isOn = false;
				RankWindows.SetActive (false);
                break;

            case 1:
                Tabs[0].SetActive(false);
                Tabs[1].SetActive(true);
                Tabs[2].SetActive(false);
                Tgl_Menu[0].isOn = false;
                Tgl_Menu[1].isOn = true;
                Tgl_Menu[2].isOn = false;
                RankWindows.SetActive(true);
                break;

            case 2:
                //if(AssetMgr.Instance.CheckDivisionDownload(7, EventInfoMgr.Instance.lstBossRush[0].u2EventID))
                //{
				//	return;
				//}
                if(StageInfoMgr.Instance.OpenBossRush == 0)
                {
                    return;
                    //PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("event_bossrush_title"), TextManager.Instance.GetText("event_end_time"), null);
                }
                else
                {
                    AssetMgr.Instance.SceneLoad("BossRushScene");
                }
                break;
			case 3:
				if (Legion.Instance.sName == "") {
					PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("menu_guild"), TextManager.Instance.GetText("popup_desc_guild_lock"), null);
					return;
				}

				AssetMgr.Instance.SceneLoad("GuildScene");
				break;
        }
    }

    public bool CheckContentOpen(int idx)
    {
        bool isOpen = false;
        switch (idx)
        {
            case 0:
                isOpen = LegionInfoMgr.Instance.IsContentOpen(LegionInfoMgr.LEAGUE_CONTENT_ID);
                break;
            case 1:
                isOpen = LegionInfoMgr.Instance.IsContentOpen(LegionInfoMgr.RANK_CONTENT_ID);
                break;
            case 2:
                isOpen = true;
                break;
			case 3:
				isOpen = true;
				break;
        }

        return isOpen;
    }

    public Goods GetContentInfo(int idx)
    {
        Goods contentInfo = null;
        switch (idx)
        {
            case 0:
                contentInfo = LegionInfoMgr.Instance.dicContentOpenData[LegionInfoMgr.LEAGUE_CONTENT_ID].cConditions;
                break;
            case 1:
                contentInfo = LegionInfoMgr.Instance.dicContentOpenData[LegionInfoMgr.RANK_CONTENT_ID].cConditions;
                break;
        }

        return contentInfo;
    }
}
