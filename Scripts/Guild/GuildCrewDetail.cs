using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class GuildCrewDetail : MonoBehaviour {
	[SerializeField] Text txtCrewName;
	[SerializeField] Text txtCrewPower;

	[SerializeField] Text[] txtCharPower;
	[SerializeField] GuildCrewDetailSlot[] _slotChar;

	[SerializeField] GameObject objCharDetail;

	StringBuilder tempStringBuilder;

	void Awake()
	{
		tempStringBuilder = new StringBuilder();
	}

	void OnEnable()
	{
		PopupManager.Instance.AddPopup(gameObject, OnClickClose);
		Init();
	}

	void Init()
	{
		GuildMember memberInfo = GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember.Find (cs => cs.u8UserSN == GuildInfoMgr.Instance.cGuildDetailData.u8UserSN);
		txtCrewName.text = memberInfo.strLegionName;
		tempStringBuilder.Remove(0, tempStringBuilder.Length);
		tempStringBuilder.Append(TextManager.Instance.GetText("mark_league_total_power")).Append(" ").Append(memberInfo.u8Power);
		txtCrewPower.text = tempStringBuilder.ToString();

		for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
		{
			if(GuildInfoMgr.Instance.cGuildDetailCrew.acLocation[i] != null)
			{
				txtCharPower [i].text = GuildInfoMgr.Instance.cGuildDetailCrew.acLocation [i].cFinalStatus.u4Power.ToString ();
				Byte tempIndex = (Byte)GuildInfoMgr.Instance.cGuildDetailCrew.acLocation [i].iIndexInCrew;
				_slotChar[i].SetData((Byte)i, false, true);
				_slotChar[i].GetComponent<Button>().interactable = true;
			}
			else
			{
				txtCharPower[i].text = "";
				_slotChar[i].SetData(0, true, true);
				_slotChar[i].GetComponent<Button>().interactable = false;
			}
		}
	}

	public void OnClickClose()
	{
		PopupManager.Instance.RemovePopup(gameObject);
		gameObject.SetActive(false);
	}

	public void OnClickChar(int idx){
		PopupManager.Instance.AddPopup(objCharDetail, RemovePopupInCharInfo);
		objCharDetail.SetActive(true);
		objCharDetail.GetComponent<CharInfoPopup>().SetData((Hero)GuildInfoMgr.Instance.cGuildDetailCrew.acLocation[idx]);
	}

	public void RemovePopupInCharInfo()
	{
		PopupManager.Instance.RemovePopup(objCharDetail);
		objCharDetail.SetActive(false);
	}
}
