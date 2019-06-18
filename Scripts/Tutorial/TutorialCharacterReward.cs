using UnityEngine;
using System;
using System.Collections;

public class TutorialCharacterReward : MonoBehaviour {

	// Use this for initialization
	private UInt16 equipSetID;

	public void OnClickChar(int classID){
		equipSetID = ClassInfoMgr.Instance.GetInfo ((ushort)classID).u2EquipSetID;
		ClassGoodsInfo info = EventInfoMgr.Instance.dicClassGoods [equipSetID];

		PopupManager.Instance.ShowLoadingPopup (1);
		Server.ServerMgr.Instance.CreateCharacterForTutorialReward (info.sName, (UInt16)classID, (byte)info.u2Hair, (byte)info.u2HairColor, (byte)info.u2Face, 4, Server.ConstDef.LastTutorialStep, rewardResult);
	}

	public void rewardResult(Server.ERROR_ID err)
	{
		#if UNITY_EDITOR
		DebugMgr.Log("rewardResult : " + err);
		#endif
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			string errText = TextManager.Instance.GetError(Server.MSGs.CHAR_CREATE, err);
			if(err > Server.ERROR_ID.LOGICAL_ERROR)
			{
				int subStringIdx = errText.LastIndexOf("\n");
				errText = errText.Substring(0, subStringIdx);
			}
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_char_create"), errText, Server.ServerMgr.Instance.CallClear);
			return;
		}
		else
		{
			Legion.Instance.AddCharacterGoods (equipSetID);
			LobbyScene _lobbyScene = Scene.GetCurrent () as LobbyScene;
			UI_CrewMenu crewMenu = _lobbyScene.transform.GetChild(0).FindChild("Pref_UI_Main_CrewMenu").GetComponent<UI_CrewMenu>();
			crewMenu.InitCharacterList();
			PopupManager.Instance.TutorialNextPage ();
			Legion.Instance.cTutorial.CheckTutorial (MENU.CREW);
		}
	}
}
