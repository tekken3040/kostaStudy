using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

public class GuildNoPanel : MonoBehaviour
{
    [SerializeField] Text txtGuildName;
    [SerializeField] Text txtCreateGuildName;
    [SerializeField] Text txtSearchGuildName;
    [SerializeField] Text txtCrewName;
    [SerializeField] Text txtCrewNumName;
    public InputField nameField;

    [SerializeField] Toggle tglPublic;

    [SerializeField] GameObject objCreateGuildPopup;
    [SerializeField] GameObject objSetCrewPopup;
    [SerializeField] GameObject objGuildList;
    [SerializeField] GameObject objPopupGroup;

	Text txtCreateTitle;
	Text txtCreateDesc;
	Text txtCreateValue;
	Text txtPublic;

    Byte u1Public = 0;

    StringBuilder tempStringBuilder;

	void Awake()
	{
		txtCreateTitle = objCreateGuildPopup.transform.FindChild ("Txt_CreateGuildTitle").GetComponent<Text>();
		txtCreateDesc = objCreateGuildPopup.transform.FindChild ("Txt_CreateGuildDesc").GetComponent<Text>();
		txtCreateValue = objCreateGuildPopup.transform.FindChild ("Txt_CreateGuildValue").GetComponent<Text>();
		txtPublic = tglPublic.transform.FindChild("Label").GetComponent<Text>();
	}

	void Start()
	{
		txtCreateTitle.text = TextManager.Instance.GetText ("popup_title_create_guild");
		txtCreateDesc.text = TextManager.Instance.GetText ("desc_create_guild");
		txtCreateValue.text = string.Format(TextManager.Instance.GetText("desc_create_guild_gold"), GuildInfoMgr.Instance.cGuildInfo.gCreateGuildGoods.u4Count);
	}

    private void OnEnable()
    {
		if (tglPublic.isOn) {
			u1Public = 1;
			txtPublic.text = TextManager.Instance.GetText ("btn_guild_open");
		} else {
			u1Public = 2;
			txtPublic.text = TextManager.Instance.GetText ("btn_guild_close");
		}

        tempStringBuilder = new StringBuilder();
        if(GuildInfoMgr.Instance.u1SelectedCrewIndex == 0)
        {
            //GuildInfoMgr.Instance.u1SelectedCrewIndex = Legion.Instance.cBestCrew.u1Index;
            GuildInfoMgr.Instance.u1SelectedCrewIndex = GuildInfoMgr.Instance.u1GuildCrewIndex;
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(TextManager.Instance.GetText("btn_crew")).Append(" ").Append(GuildInfoMgr.Instance.u1SelectedCrewIndex);
        }
        else
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(TextManager.Instance.GetText("btn_crew")).Append(" ").Append(GuildInfoMgr.Instance.u1SelectedCrewIndex);
        }
        txtGuildName.text = TextManager.Instance.GetText("mark_guild_title_name");
        txtCrewNumName.text = tempStringBuilder.ToString();
        txtCrewName.text = Legion.Instance.sName;
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestGuildSearch(String.Empty, AckGuildList);
    }

    public void OnClickRequestGuildList()
    {
        ClearGuildList();
        txtSearchGuildName.text = String.Empty;
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestGuildSearch(String.Empty, AckGuildList);
    }

    private void AckGuildList(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
            if(err == Server.ERROR_ID.GUILD_REQUEST_NOT_YET)
            {
                //Server.ServerMgr.Instance.ClearFirstJobError();
                //StartCoroutine(DelayedLoadScene());
                GuildInfoMgr.Instance.bRefreshGuild = true;
                //ReloadGuildScene(null);
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_guild_join_request"), TextManager.Instance.GetText("err_guild_already_success"), ReloadGuildScene);
                return;
            }
            else
            {
			    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.GUILD_SEARCH, err), Server.ServerMgr.Instance.CallClear);
                return;
            }
		}
		else
		{
            InitGuildList();
        }
    }

    IEnumerator DelayedLoadScene()
    {
        GuildInfoMgr.Instance.bRefreshGuild = true;
        //GuildInfoMgr.Instance.InitUserData();
        yield return new WaitForEndOfFrame();
        //GuildInfoMgr.Instance.u8GuildSN = 1;
        AssetMgr.Instance.SceneLoad("GuildScene");
        //PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("mark_search_guild"), TextManager.Instance.GetText("err_desc_guild_join"), ReloadGuildScene);
    }

    public void OnClickCreateGuild()
    {
        tglPublic.isOn = true;
        objCreateGuildPopup.SetActive(true);
    }

    public void OnClickCloseCreateGuildPopup()
    {
        objCreateGuildPopup.SetActive(false);
    }

    public void ToggleCreateGuildPublic()
    {
		if (tglPublic.isOn) {
			u1Public = 1;
			txtPublic.text = TextManager.Instance.GetText ("btn_guild_open");
		} else {
			u1Public = 2;
			txtPublic.text = TextManager.Instance.GetText ("btn_guild_close");
		}
    }

    public void OnClickRequestCreateGuild()
    {
        if (nameField.text == "")
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_create_guild"), TextManager.Instance.GetText("err_desc_guild_empty_name"), null);
            return;
        }
        else if (Regex.Matches(nameField.text, @"[\s\Wㄱ-ㅎㅏ-ㅣ]").Count != 0)
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(TextManager.Instance.GetErrorText("REVENGEMESSAGE_WRONGCHAR", "", false));
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_create_guild"), tempStringBuilder.ToString(), null);
            return;
        }
        else
        {
            for (int i = 0; i < nameField.text.Length; i++)
            {
                if (nameField.text.Substring(i, 1).Equals(" "))
                {
                    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_create_guild"), TextManager.Instance.GetText("err_desc_guild_space_name"), null);
                    return;
                }
            }
        }

		if (Legion.Instance.CheckEnoughGoods (GuildInfoMgr.Instance.cGuildInfo.gCreateGuildGoods))
        {
			PopupManager.Instance.ShowLoadingPopup (1);
			Server.ServerMgr.Instance.RequestCreateGuild (txtCreateGuildName.text, u1Public, AckCreateGuild);
		}
        else
        {
			PopupManager.Instance.ShowChargePopup(1);
		}
    }

    private void AckCreateGuild(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			if(err == Server.ERROR_ID.GUILD_REQUEST_NOT_YET)
			{
				GuildInfoMgr.Instance.bRefreshGuild = true;
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_create_guild"), TextManager.Instance.GetText("err_desc_guild_join"), GuildInfoMgr.Instance.ReloadScene);
				return;
			}
			else if(err == Server.ERROR_ID.LEGION_NAME_DUPLICATE)
			{
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_create_guild"), TextManager.Instance.GetText("err_desc_guild_name_duplicate"), null);
				return;
			}
			else
			{
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.GUILD_CREATE, err), Server.ServerMgr.Instance.CallClear);
	            return;
			}
		}
		else
		{
			objCreateGuildPopup.SetActive(false);
			Legion.Instance.SubGoods (GuildInfoMgr.Instance.cGuildInfo.gCreateGuildGoods);
			//GuildInfoMgr.Instance.u1GuildCrewIndex = Legion.Instance.cBestCrew.u1Index;
            //#CHATTING
            // 길드 채팅 연결
            PopupManager.Instance.GuildChatConnect();
			AssetMgr.Instance.SceneLoad("GuildScene");
        }
    }

    public void OnClickRefreashList()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        ClearGuildList();
        Server.ServerMgr.Instance.RequestGuildSearch(String.Empty, AckSearchGuild);
    }

    public void OnClickSearchGuild()
    {
        if(Regex.Matches(txtSearchGuildName.text, @"[\s\Wㄱ-ㅎㅏ-ㅣ]").Count != 0)
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(TextManager.Instance.GetErrorText("CREATE_ACCOUNT_ID_WRONGCHAR", "", false));
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("mark_search_guild"), tempStringBuilder.ToString(), null);
            return;
        }
        PopupManager.Instance.ShowLoadingPopup(1);
        ClearGuildList();
        Server.ServerMgr.Instance.RequestGuildSearch(txtSearchGuildName.text, AckSearchGuild);
    }

    private void AckSearchGuild(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
            if(err == Server.ERROR_ID.GUILD_REQUEST_NOT_YET)
            {
				GuildInfoMgr.Instance.bRefreshGuild = true;
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_guild_join_request"), TextManager.Instance.GetText("err_guild_already_success"), ReloadGuildScene);
                return;
            }
            else
            {
			    PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.GUILD_SEARCH, err), Server.ServerMgr.Instance.CallClear);
                return;
            }
		}
		else
		{
            InitGuildList();
        }
    }

    public void OnClickSetCrew()
    {
        objSetCrewPopup.SetActive(true);
		objSetCrewPopup.GetComponent<GuildSelectCrewPopup> ().SetCrewData (this);
    }

    public void OnClickCloseSetCrew()
    {
        objSetCrewPopup.SetActive(false);
    }

    public void SetGuildCrew()
    {
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(TextManager.Instance.GetText("btn_crew")).Append(" ").Append(GuildInfoMgr.Instance.u1SelectedCrewIndex);
        txtCrewNumName.text = tempStringBuilder.ToString();
    }

    int selectedIndex = 0;
    public void OnClickRequestSetCrew(int crewIndex)
    {
        selectedIndex = crewIndex;
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestGuildSetCrew((Byte)GuildInfoMgr.SET_CREW_TYPE.UserSelect, 0, 0, 0, (Byte)crewIndex, AckSetCrew);
    }

    private void AckSetCrew(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.GUILD_SET_CREW, err), Server.ServerMgr.Instance.CallClear);
            return;
		}
		else
		{
            GuildInfoMgr.Instance.u1SelectedCrewIndex = (Byte)selectedIndex;
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(TextManager.Instance.GetText("btn_crew")).Append(" ").Append(selectedIndex);
            txtCrewNumName.text = tempStringBuilder.ToString();
        }
    }

    private void ClearGuildList()
    {
        GuildInfoMgr.Instance.dicGuildList.Clear();
        for(int i=0; i<objGuildList.transform.GetChildCount(); i++)
        {
            Destroy(objGuildList.transform.GetChild(i).gameObject);
        }
    }

    private void InitGuildList()
    {
        for(int i=0; i<GuildInfoMgr.Instance.u1GuildListCount; i++)
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            GameObject objSlot = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Guild/GuildSlot_.prefab", typeof(GameObject)) as GameObject);
            objSlot.transform.SetParent(objGuildList.transform);
            objSlot.transform.localScale = Vector3.one;
            objSlot.transform.localPosition = Vector3.zero;
            tempStringBuilder.Append("GuildSlot_").Append(i);
            objSlot.name = tempStringBuilder.ToString();
            objSlot.GetComponent<GuildListSlot>().SetData(GuildInfoMgr.Instance.dicGuildList[(Byte)i]);
        }
    }

    private void ReloadGuildScene(object[] param)
    {
        //Server.ServerMgr.Instance.CallClear(null);
		Server.ServerMgr.Instance.ClearFirstJobError();
        AssetMgr.Instance.SceneLoad("GuildScene");
    }
}
