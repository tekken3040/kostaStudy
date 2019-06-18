using UnityEngine;
using System;
using System.Collections;

public class GuildScene : BaseScene
{
    [SerializeField] GameObject panelNoGuild;
    [SerializeField] GameObject panelGuild;

    private void Awake()
    {

    }

    private void Start()
    {
		FadeEffectMgr.Instance.FadeIn ();

        if (GuildInfoMgr.Instance.u8GuildSN == 0) {
			if(GuildInfoMgr.Instance.bRefreshGuild){
				panelGuild.SetActive(true);
				GuildInfoMgr.Instance.bRefreshGuild = false;
			}else{
				panelNoGuild.SetActive (true);
			}
		} else
            panelGuild.SetActive(true);
    }

    public void OnClickBack()
    {
        if(GuildInfoMgr.Instance.bDirty)
        {
            UInt64[] tempSN = new UInt64[3];

            for(int i=0; i<3; i++)
            {
                if(panelGuild.GetComponent<GuildPanel>().MASTER_ENTRY)
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
            if(panelGuild.GetComponent<GuildPanel>().MASTER_ENTRY)
                Server.ServerMgr.Instance.RequestGuildSetCrew((Byte)GuildInfoMgr.SET_CREW_TYPE.GuildMaster, tempSN[0], tempSN[1], tempSN[2], 0, AckSelectCrew);
            else
                Server.ServerMgr.Instance.RequestGuildSetCrew((Byte)GuildInfoMgr.SET_CREW_TYPE.UserCustom, tempSN[0], tempSN[1], tempSN[2], 0, AckSelectCrew);
        }
        else
            AssetMgr.Instance.SceneLoad("LobbyScene");
    }

    private void AckSelectCrew(Server.ERROR_ID err)
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
            AssetMgr.Instance.SceneLoad("LobbyScene");
        }
    }

    public override IEnumerator CheckReservedPopup()
	{
        yield return new WaitForEndOfFrame();
    }
    
    public override void RefreshAlram()
    {
    }

    // 메인채팅창 오픈 버튼 클릭 이벤트 함수
    public void OnClickOpenChatWindow()
    {
        PopupManager.Instance.OpenChattingType(ChattingManager.ChattingTabType.GuidTab);
    }
}
