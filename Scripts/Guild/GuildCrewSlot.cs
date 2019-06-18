using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class GuildCrewSlot : MonoBehaviour
{
    [SerializeField] Text txtCrewName;
    [SerializeField] Text txtCrewPower;
    [SerializeField] Text txtCrewLocked;

    [SerializeField] Button btnSelect;

    [SerializeField] GameObject objCrewFlag;
    [SerializeField] GameObject objCrewLock;
    [SerializeField] GameObject objCrewPower;
    [SerializeField] GameObject objCharList;

    [SerializeField] GameObject[] objCrewChar;

    int _index;
    GuildSelectCrewPopup _parent;
    StringBuilder tempStringBuilder;

    public void SetData(int crewIdx, GuildSelectCrewPopup parent)
    {
        tempStringBuilder = new StringBuilder();
        _index = crewIdx;
        _parent = parent;
		tempStringBuilder.Append(TextManager.Instance.GetText("btn_crew")).Append(" ").Append(crewIdx);
        //txtCrewName.text = Legion.Instance.sName;
        txtCrewName.text = tempStringBuilder.ToString();
        if(Legion.Instance.acCrews[crewIdx-1].IsLock)
        {
            objCrewFlag.SetActive(false);
            objCrewLock.SetActive(true);
            objCharList.SetActive(false);
            objCrewPower.SetActive(false);
            btnSelect.gameObject.SetActive(false);
            txtCrewLocked.gameObject.SetActive(true);
            return;
        }
        else if(Legion.Instance.acCrews[crewIdx-1].u1Count == 0)
        {
            objCrewFlag.SetActive(false);
            objCrewLock.SetActive(true);
            objCharList.SetActive(false);
            objCrewPower.SetActive(false);
            btnSelect.gameObject.SetActive(false);
            txtCrewLocked.gameObject.SetActive(true);
            return;
        }
        objCrewFlag.SetActive(true);
        objCrewLock.SetActive(false);
        objCharList.SetActive(true);
        objCrewPower.SetActive(true);
        btnSelect.gameObject.SetActive(true);
        txtCrewLocked.gameObject.SetActive(false);

        txtCrewPower.text = Legion.Instance.acCrews[_index-1].u4Power.ToString();
        for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
        {
            if(Legion.Instance.acCrews[_index-1].acLocation[i] == null)
            {
                objCrewChar[i].SetActive(false);
            }
            else
            {
                objCrewChar[i].SetActive(true);
                objCrewChar[i].GetComponent<UI_CharCursorElement>().SetData(((Hero)Legion.Instance.acCrews[_index-1].acLocation[i]));
            }
        }
    }

    public void OnClickSelectCrew()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestGuildSetCrew((Byte)GuildInfoMgr.SET_CREW_TYPE.UserSelect, 0, 0, 0, (Byte)_index, AckSelectCrew);
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
            GuildInfoMgr.Instance.u1SelectedCrewIndex = (Byte)_index;
            GuildInfoMgr.Instance.u1GuildCrewIndex = GuildInfoMgr.Instance.u1SelectedCrewIndex;
            _parent.OnClickClose();
        }
    }
}
