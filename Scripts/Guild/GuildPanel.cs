using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
public class GuildPanel : MonoBehaviour
{
    [SerializeField] Text txtGuildName;
    [SerializeField] Text txtGuildRank;
    [SerializeField] Text txtGuildTier;
    [SerializeField] Text txtGuildScore;
    [SerializeField] Text txtSelectCrew;
    [SerializeField] Text txtCrewPower;
    [SerializeField] Text txtKeyTime;

    [SerializeField] Image imgGuildTier;
    [SerializeField] Image imgEntryBg;

    [SerializeField] Toggle tglUserEntry;
    [SerializeField] Toggle tglGuildEntry;

    public GameObject objGuildBattleList;
    [SerializeField] GameObject objPopupGroup;
    [SerializeField] GameObject objMatchPopup;
    [SerializeField] GameObject objSelectCrewPopup;
    [SerializeField] GameObject objGuildInfoPopup;
    [SerializeField] GameObject objSelectEntryCrewPopup;
    [SerializeField] GameObject objRankInfoPopup;
    [SerializeField] GameObject objBattleInfoPopup;
	public GameObject objCalculate;
    [SerializeField] GameObject popupIntroduce;

    [SerializeField] Button[] btnEntrySlot;
    public GameObject[] objEntrySlot;
    [SerializeField] GameObject[] objKeys;

    public SubChatting cSubChatWindow;

    StringBuilder tempStringBuilder;
    Byte u1UserCrewIdxinList = 0;
    bool bMasterEntry = false;
    public bool MASTER_ENTRY
    {
        set{bMasterEntry = value;}
        get{return bMasterEntry;}
    }
    private UInt16 u2Keys = 0;

    private void OnEnable()
    {
        for(int i=0; i<objGuildBattleList.transform.GetChildCount(); i++)
            objGuildBattleList.transform.GetChild(i).gameObject.SetActive(false);
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestGuildInfo(AckGuildInfo);
    }

    private void AckGuildInfo(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
            if(err == Server.ERROR_ID.FRIEND_REQUEST_DELETED || err == Server.ERROR_ID.GUILD_REQUEST_NOT_YET)
            {
                GuildInfoMgr.Instance.CheckGuildError(err);
                return;
            }
            else
            {
			    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.GUILD_INFO, err), Server.ServerMgr.Instance.CallClear);
                return;
            }
		}
		else
		{
            InitGuildData();
            SetEntryCrew();
			StartCoroutine (DelayedEntryCall ());
			StartCoroutine (CheckKeyTime ());

			OnClickRefreashList ();
        }
    }

    private IEnumerator CheckKeyTime()
    {
        while (true)
        {
            if(GuildInfoMgr.Instance.GuildKey < GuildInfoMgr.Instance.cGuildInfo.u1MaxKey)
            {                
		       if(txtKeyTime != null)
                {
                    txtKeyTime.text = string.Format("({0:00}:{1:00}:{2:00})", GuildInfoMgr.Instance.tsGuildKeyTime.Hours, GuildInfoMgr.Instance.tsGuildKeyTime.Minutes, GuildInfoMgr.Instance.tsGuildKeyTime.Seconds);
                    //if(GuildInfoMgr.Instance.tsGuildKeyTime.Hours > 0)
					//    txtKeyTime.text = string.Format("({0:00}:{1:00})", GuildInfoMgr.Instance.tsGuildKeyTime.Hours, GuildInfoMgr.Instance.tsGuildKeyTime.Minutes);
                    //else
                    //    txtKeyTime.text = string.Format("({0:00}:{1:00})", GuildInfoMgr.Instance.tsGuildKeyTime.Minutes, GuildInfoMgr.Instance.tsGuildKeyTime.Seconds);
                }
            }
            else
            {
                if(txtKeyTime != null)
                    txtKeyTime.text = "";
            }
            if(u2Keys != GuildInfoMgr.Instance.GuildKey)
            {
                u2Keys = GuildInfoMgr.Instance.GuildKey;
                for(int i=0; i<objKeys.Length; i++)
                {
                    if(i<GuildInfoMgr.Instance.GuildKey)
                        objKeys[i].SetActive(true);
                    else
                        objKeys[i].SetActive(false);
                }
            }
            yield return StartCoroutine(Utillity.WaitForRealSeconds(1f));
        }
    }

    private void InitGuildData()
    {
        tempStringBuilder = new StringBuilder();
        txtGuildName.text = GuildInfoMgr.Instance.cGuildMemberInfo.strGuildName;
        txtGuildRank.text = GuildInfoMgr.Instance.cGuildMemberInfo.u8Rank.ToString();
        txtGuildTier.text = GuildInfoMgr.Instance.GetGuildTier(GuildInfoMgr.Instance.cGuildMemberInfo.u2Score).ToString();
        txtGuildScore.text = GuildInfoMgr.Instance.cGuildMemberInfo.u2Score.ToString();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(TextManager.Instance.GetText("btn_crew")).Append(" ").Append(GuildInfoMgr.Instance.u1GuildCrewIndex);
        txtSelectCrew.text = tempStringBuilder.ToString();
        txtCrewPower.text = Legion.Instance.acCrews[GuildInfoMgr.Instance.u1GuildCrewIndex-1].u4Power.ToString();
        u2Keys = GuildInfoMgr.Instance.GuildKey;
        for(int i=0; i<objKeys.Length; i++)
        {
            if(i<GuildInfoMgr.Instance.GuildKey)
                objKeys[i].SetActive(true);
            else
                objKeys[i].SetActive(false);
        }

        //#CHATTING
        if (cSubChatWindow != null)
        {
            if (PopupManager.Instance.IsChattingActive())
            {
                PopupManager.Instance.SetSubChtting(cSubChatWindow, ChattingManager.ChattingTabType.GuidTab);
                PopupManager.Instance.GuildChatConnect();
                cSubChatWindow.gameObject.SetActive(true);
            }
        }
    }

    private void SetEntryCrew()
    {
        GuildInfoMgr.Instance.dicUserEntry.Clear();
        GuildInfoMgr.Instance.dicMasterEntry.Clear();

        for(int i=0; i<GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember.Count; i++)
        {
            if((GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i].u1Option & 0x01) != 0)
                GuildInfoMgr.Instance.dicMasterEntry.Add(1, GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i]);
            else if((GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i].u1Option & 0x02) != 0)
                GuildInfoMgr.Instance.dicMasterEntry.Add(2, GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i]);
            else if((GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i].u1Option & 0x04) != 0)
                GuildInfoMgr.Instance.dicMasterEntry.Add(3, GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i]);

            if((GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i].u1Option & 0x20) != 0)
                GuildInfoMgr.Instance.dicUserEntry.Add(1, GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i]);
            else if((GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i].u1Option & 0x40) != 0)
                GuildInfoMgr.Instance.dicUserEntry.Add(2, GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i]);
            else if((GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i].u1Option & 0x80) != 0)
                GuildInfoMgr.Instance.dicUserEntry.Add(3, GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i]);
        }
    }

    IEnumerator DelayedEntryCall()
    {
        yield return new WaitForEndOfFrame();
        tglUserEntry.isOn = true;
    }

    public void OnClickRefreashList()
    {
		if (GuildInfoMgr.Instance.cGuildMemberInfo.dtRewardEndTime <= Legion.Instance.ServerTime) {
			if (!objCalculate.activeSelf) {
				objCalculate.SetActive (true);
				objGuildBattleList.SetActive (false);
			}else{
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_guild_calculate"), TextManager.Instance.GetText("popup_desc_guild_calculate"), null);
			}
		} else {
			PopupManager.Instance.ShowLoadingPopup (1);
			Server.ServerMgr.Instance.RequestGuildMatchInfo (AckGuildMatchInfo);
		}
    }

    private void AckGuildMatchInfo(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
            if(err == Server.ERROR_ID.FRIEND_REQUEST_DELETED || err == Server.ERROR_ID.GUILD_REQUEST_NOT_YET)
            {
                GuildInfoMgr.Instance.CheckGuildError(err);
                return;
            }
            else
            {
			    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.GUILD_MATCH_INFO, err), Server.ServerMgr.Instance.CallClear);
                return;
            }
		}
		else
		{
            for(int i=0; i<objGuildBattleList.transform.GetChildCount(); i++)
                objGuildBattleList.transform.GetChild(i).gameObject.SetActive(false);
            StartCoroutine(SetMatchList());
        }
    }

    private IEnumerator SetMatchList()
    {
        yield return new WaitForEndOfFrame();
        for(int i=0; i<GuildInfoMgr.Instance.lstGuildMatchList.Count; i++)
        {
            objGuildBattleList.transform.GetChild(i).gameObject.SetActive(true);
            objGuildBattleList.transform.GetChild(i).GetComponent<GuildMatchSlot>().SetData(GuildInfoMgr.Instance.lstGuildMatchList[i], this);
        }
    }

    public void OnClickGuildInfo()
    {
		objGuildInfoPopup.SetActive (true);
    }

    public void OnClickIntroducePopup()
    {
        popupIntroduce.SetActive(true);
        PopupManager.Instance.AddPopup(popupIntroduce, OnClickCloseIntroPopup);
    }

    public void OnClickCloseIntroPopup()
    {
        PopupManager.Instance.RemovePopup(popupIntroduce);
        popupIntroduce.SetActive(false);
    }

    public void OnClickRank()
    {
		if (GuildInfoMgr.Instance.cGuildMemberInfo.dtRewardEndTime <= Legion.Instance.ServerTime) {
			objCalculate.SetActive (true);
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText ("popup_title_guild_calculate"), TextManager.Instance.GetText ("popup_desc_guild_calculate"), null);
			objGuildBattleList.SetActive (false);
		} else {
			PopupManager.Instance.ShowLoadingPopup (1);
			Server.ServerMgr.Instance.RequestGuildRankInfo ((Byte)GuildInfoMgr.GUILD_RANK_TYPE.RankList, 0, AckRankList);
		}
    }

    private void AckRankList(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			if(err == Server.ERROR_ID.GUILD_DATA_DENIED)
			{
				GuildInfoMgr.Instance.CheckGuildError(err);
				return;
			}

            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.GUILD_SET_CREW, err), Server.ServerMgr.Instance.CallClear);
            return;
        }
        else
        {
            objRankInfoPopup.SetActive(true);
        }
    }

    public void OnClickBattleInfo()
    {
        objBattleInfoPopup.SetActive(true);
    }

    public void OnClickEntryToggle()
    {
        if(GuildInfoMgr.Instance.bDirty)
            ChangeEntryCrew();
        bMasterEntry = false;
        imgEntryBg.color = Color.white;
        for(int i=0; i<3; i++)
        {
            if(GuildInfoMgr.Instance.dicUserEntry.ContainsKey((Byte)(i+1)))
                objEntrySlot[i].GetComponent<GuildEntrySlot>().SetData(GuildInfoMgr.Instance.dicUserEntry[(Byte)(i+1)], false);
            else
				objEntrySlot[i].GetComponent<GuildEntrySlot>().SetEmpty(false);
            btnEntrySlot[i].interactable = true;
        }
    }

    public void OnClickGuildEntryToggle()
    {
        if(GuildInfoMgr.Instance.bDirty)
            ChangeEntryCrew();
        bMasterEntry = true;
        imgEntryBg.color = Color.red;
        for(int i=0; i<3; i++)
        {
            if(GuildInfoMgr.Instance.dicMasterEntry.ContainsKey((Byte)(i+1)))
                objEntrySlot[i].GetComponent<GuildEntrySlot>().SetData(GuildInfoMgr.Instance.dicMasterEntry[(Byte)(i+1)], true);
            else
                objEntrySlot[i].GetComponent<GuildEntrySlot>().SetEmpty(true);
            if(GuildInfoMgr.Instance.bGuildMaster)
                btnEntrySlot[i].interactable = true;
            else
                btnEntrySlot[i].interactable = false;
        }
    }

    public void ChangeEntryCrew()
    {
        UInt64[] tempSN = new UInt64[3];

        for(int i=0; i<3; i++)
        {
            if(bMasterEntry)
            {
                if(GuildInfoMgr.Instance.dicMasterEntry.ContainsKey((Byte)(i+1)))
                    tempSN[i] = GuildInfoMgr.Instance.dicMasterEntry[(Byte)(i+1)].u8UserSN;
                else
                    tempSN[i] =  0;
                }
            else
            {
                if(GuildInfoMgr.Instance.dicUserEntry.ContainsKey((Byte)(i+1)))
                    tempSN[i] = GuildInfoMgr.Instance.dicUserEntry[(Byte)(i+1)].u8UserSN;
                else
                    tempSN[i] =  0;
            }
            
        }
        PopupManager.Instance.ShowLoadingPopup(1);
        if(bMasterEntry)
            Server.ServerMgr.Instance.RequestGuildSetCrew((Byte)GuildInfoMgr.SET_CREW_TYPE.GuildMaster, tempSN[0], tempSN[1], tempSN[2], 0, AckSetCrew);
        else
            Server.ServerMgr.Instance.RequestGuildSetCrew((Byte)GuildInfoMgr.SET_CREW_TYPE.UserCustom, tempSN[0], tempSN[1], tempSN[2], 0, AckSetCrew);
    }

    private void AckSetCrew(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
            if(err == Server.ERROR_ID.FRIEND_REQUEST_DELETED || err == Server.ERROR_ID.GUILD_REQUEST_NOT_YET)
            {
                GuildInfoMgr.Instance.CheckGuildError(err);
                return;
            }
            else
            {
			    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.GUILD_SET_CREW, err), Server.ServerMgr.Instance.CallClear);
                return;
            }
		}
		else
		{
            GuildInfoMgr.Instance.bDirty = false;
        }
    }

    public void OnClickSetCrew()
    {
        objSelectCrewPopup.SetActive(true);
		objSelectCrewPopup.GetComponent<GuildSelectCrewPopup> ().SetCrewData (this);
    }

    public void SetGuildCrew()
    {
		if (GuildInfoMgr.Instance.u1SelectedCrewIndex <= 0)
			return;
		
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(TextManager.Instance.GetText("btn_crew")).Append(" ").Append(GuildInfoMgr.Instance.u1SelectedCrewIndex);
        txtSelectCrew.text = tempStringBuilder.ToString();

        txtCrewPower.text = Legion.Instance.acCrews[GuildInfoMgr.Instance.u1SelectedCrewIndex-1].u4Power.ToString();

        for(int i=0; i<GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember.Count; i++)
        {
            if(GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i].strLegionName == Legion.Instance.sName)
            {
                for(int j=0; j<Crew.MAX_CHAR_IN_CREW; j++)
                {
                    GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i].u2ClassID[j] = 0;
                    GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i].u2Level[j] = 0;
                    GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i].u1Element[j] = 0;
                    if(Legion.Instance.acCrews[GuildInfoMgr.Instance.u1SelectedCrewIndex-1].acLocation[j] != null)
                    {
                        GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i].u2ClassID[j] = Legion.Instance.acCrews[GuildInfoMgr.Instance.u1SelectedCrewIndex-1].acLocation[j].cClass.u2ID;
                        GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i].u2Level[j] = Legion.Instance.acCrews[GuildInfoMgr.Instance.u1SelectedCrewIndex-1].acLocation[j].cLevel.u2Level;
                        GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i].u1Element[j] = ((Hero)Legion.Instance.acCrews[GuildInfoMgr.Instance.u1SelectedCrewIndex-1].acLocation[j]).acEquips[6].GetEquipmentInfo().u1Element;
                    }
                }
                GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i].u8Power = Legion.Instance.acCrews[GuildInfoMgr.Instance.u1SelectedCrewIndex-1].u4Power;
                u1UserCrewIdxinList = (Byte)i;
                break;
            }
        }

        for(int i=0; i<3; i++)
        {
            if(GuildInfoMgr.Instance.dicUserEntry.ContainsKey((Byte)(i+1)))
            {
                if(GuildInfoMgr.Instance.dicUserEntry[(Byte)(i+1)].strLegionName == Legion.Instance.sName)
                {
                    GuildInfoMgr.Instance.dicUserEntry.Remove((Byte)(i+1));
                    GuildInfoMgr.Instance.dicUserEntry.Add((Byte)(i+1), GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[u1UserCrewIdxinList]);
                }
            }

            if(GuildInfoMgr.Instance.dicMasterEntry.ContainsKey((Byte)(i+1)))
            {
                if(GuildInfoMgr.Instance.dicMasterEntry[(Byte)(i+1)].strLegionName == Legion.Instance.sName)
                {
                    GuildInfoMgr.Instance.dicMasterEntry.Remove((Byte)(i+1));
                    GuildInfoMgr.Instance.dicMasterEntry.Add((Byte)(i+1), GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[u1UserCrewIdxinList]);
                }
            }
        }

        if(!bMasterEntry)
            OnClickEntryToggle();
        else
            OnClickGuildEntryToggle();
    }

    public void OnClickEntrySlot(int i)
    {
        objSelectEntryCrewPopup.SetActive(true);
        objSelectEntryCrewPopup.GetComponent<GuildSelectEntryCrewPopup>().SlotIndex = i;
        objSelectEntryCrewPopup.GetComponent<GuildSelectEntryCrewPopup>()._parent = this;
        GuildInfoMgr.Instance.dicUserEntryBackUp = GuildInfoMgr.Instance.dicUserEntry;
    }

    public void ShowMatchPopup()
    {
        objMatchPopup.SetActive(true);
    }
}
