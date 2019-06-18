using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;

public enum TutorialSubPart {
	FIRST_BATTLE = 1,
	USE_SKILL = 4,
	BOSS_SKILL = 6,
	FIRST_BATTLE_END = 7,
	CREATE_CHAR = 8,
	SECOND_BATTLE = 10,
	SECOND_BATTLE_END = 11,
//	CHAR_INFO = 14,
	LAST_TUTORIAL = 13
}

public class Tutorial 
{
	public const UInt16 TUTORIAL_STAGE_ID = 6000;
	public const UInt16 TUTORIAL_BOSS_FIELD_ID = 4000;

	public Byte[] au1Step;

	public bool bIng = false;

	public TutorialInfo CurrentTutorial;

	Controller cCont = null;

	bool bClear = false;

	public bool bStageReservedPopup = false;

	public Tutorial(){

	}

//	public bool CheckTutorial(MENU screen, UInt16 popup)
//	{
//		for (int i=0; i<au1Step.Length; i++) {
//			if(au1Step[i] == Server.ConstDef.LastTutorialStep) continue;
//
//			if(i > 0){
//				if(au1Step[0] != Server.ConstDef.LastTutorialStep) continue;
//			}
//			Byte TutorialNo = TutorialInfoMgr.Instance.GetTutorialInfoByScreen((Byte)(i+1), (Byte)(au1Step[i]+1), (UInt16)screen);
//			if(TutorialNo > 0){
//				bIng = true;
//				CurrentTutorial = TutorialInfoMgr.Instance.GetTutorialInfo(TutorialNo);
//				PopupManager.Instance.SetTutorialPopup(CurrentTutorial);
//				return true;
//			}
//		}
//		
//		return false;
//	}

	public bool CheckTutorial(MENU screen)
	{
		return CheckTutorial((UInt16)screen);
	}

	public bool CheckTutorial(UInt16 _u1Screen){
//		for (int i=0; i<au1Step.Length; i++) {
//			DebugMgr.LogWarning ((i + 1) + " -> " + au1Step [i]);
//		}

		for (int i=0; i<au1Step.Length; i++) {
			if(au1Step[i] == Server.ConstDef.LastTutorialStep) continue;

			if(i==0 && au1Step[i] == 0) au1Step[0] = 3;

			if(!CheckException((Byte)(i+1))) continue;

			Byte TutorialNo = TutorialInfoMgr.Instance.GetTutorialInfoByScreen((Byte)(i+1), (Byte)(au1Step[i]+1), _u1Screen);

			if(TutorialNo > 0){
				bIng = true;
				CurrentTutorial = TutorialInfoMgr.Instance.GetTutorialInfo(TutorialNo);

				if (CurrentTutorial.sAnalyticsEventCode != "0") {
					AccountManager.Instance.FBEventLog (CurrentTutorial.sAnalyticsEventCode);
				}
				if (CurrentTutorial.lstTalks.Count == 0) {
					EndTutorial (CurrentTutorial);
					return true;
				}
				PopupManager.Instance.SetTutorialPopup(CurrentTutorial);
				return true;
			}
		}

		return false;
	}

	bool CheckException(Byte type){
		if (type == 4) {
			int OtherCrewCharCnt = 0;
			for (int i = 1; i < Legion.Instance.acCrews.Length; i++) {
				OtherCrewCharCnt += Legion.Instance.acCrews [i].u1Count;
			}
			if (OtherCrewCharCnt <= 0 || bStageReservedPopup)
				return false;
		}else if (type == 5) {
			UInt16 tempStageNum = LegionInfoMgr.Instance.acCrewOpenGoods[0][1].u2ID;
			if (StageInfoMgr.Instance.dicStageData [tempStageNum].clearState == 0) {
				return false;
			}
		}else if (type == 6) {
			if(!StageInfoMgr.Instance.IsClear(6011)) return false;
		}else if (type == 7) {
			if(!StageInfoMgr.Instance.IsClear(6015)) return false;
		}else if (type == 8) {
			if(!StageInfoMgr.Instance.IsClear(6004)) return false;
		}else if (type == 9) {
			if(!StageInfoMgr.Instance.IsClear(6007)) return false;
		}

		return true;
	}

	public void AddController(Controller cont){
		cCont = cont;
	}

	public void EndTutorial(TutorialInfo info){
//		DebugMgr.LogError(info.u1TutorialType+" -> "+info.u1TutorialPart);
		bIng = false;

		if (au1Step [info.u1TutorialType - 1] == Server.ConstDef.LastTutorialStep) {
			DebugMgr.Log("End");
			return;
		}

		Byte TutorialNo = TutorialInfoMgr.Instance.GetNextTutorialPart (info);

		if (TutorialNo > 0) {
			au1Step[info.u1TutorialType-1] = info.u1TutorialPart;
			//DebugMgr.LogError(au1Step[info.u1TutorialType-1]);
		}else{
			au1Step[info.u1TutorialType-1] = Server.ConstDef.LastTutorialStep;
			//DebugMgr.LogError(au1Step[info.u1TutorialType-1]);
		}

		CheckEndExcept(info);
		cCont = null;
	}

	public void CheckEndExcept(TutorialInfo info){
		bClear = false;
		switch (info.u1TutorialType) {
		case 1: 
			switch(info.u1TutorialPart)
            {
            case (Byte)TutorialSubPart.CREATE_CHAR:
                 Legion.Instance.u1VIPLevel = 1;
                 break;
			case (Byte)TutorialSubPart.USE_SKILL:
				Time.timeScale = 1.0f;
				au1Step [0] = 5;
				break;
            case (Byte)TutorialSubPart.BOSS_SKILL:
				cCont.ResumeGame ();
				cCont.UseSkillCompulsion (1, 0, 1);
				cCont.cBattle.battleUIMgr.SetActive (false);
				cCont.cBattle.battleUIMgr.cUICam.enabled = false;
				cCont.cBattle.UserAllDeathByTime(5.0f);
				cCont.cBattle.cCameraMove2.SetCameraFix(true);
				cCont.cBattle.cCameraMove2.SetDirectionCam("ZombieDragonDirection 1", true);
				break;
			case (Byte)TutorialSubPart.FIRST_BATTLE_END:
				Legion.Instance.cBestCrew = null;
				//FadeEffectMgr.Instance.FadeOut();
				cCont.cBattle.DeleteObject ();
				ObjMgr.Instance.RemoveMonsterPool();
				SoundManager.Instance.OffBattleListner ();
				PopupManager.Instance.ShowLoadingPopup(1);
				Server.ServerMgr.Instance.RequestTutorial((Byte)(info.u1TutorialType-1), info.u1TutorialPart, GoToCreateCharacterScene);
				return;
				//PopupManager.Instance.ShowLoadingPopup(1);
				//Server.ServerMgr.Instance.StartStage(Legion.Instance.cBestCrew, Legion.Instance.SelectedStage, 1, GoToBattle);
				break;
			case (Byte)TutorialSubPart.SECOND_BATTLE_END:
				//FadeEffectMgr.Instance.FadeOut();
				PopupManager.Instance.ShowLoadingPopup(1);
				Server.ServerMgr.Instance.RequestTutorial((Byte)(info.u1TutorialType-1), info.u1TutorialPart, RemoveLoading);
				//PopupManager.Instance.ShowLoadingPopup(1);
				//Server.ServerMgr.Instance.StartStage(Legion.Instance.cBestCrew, Legion.Instance.SelectedStage, 1, GoToBattle);.
				break;
//			case (Byte)TutorialSubPart.STATUS:
//				//FadeEffectMgr.Instance.FadeOut();
//				PopupManager.Instance.ShowLoadingPopup(1);
//				Server.ServerMgr.Instance.RequestTutorial((Byte)(info.u1TutorialType-1), info.u1TutorialPart, RemoveLoading);
//				//PopupManager.Instance.ShowLoadingPopup(1);
//				//Server.ServerMgr.Instance.StartStage(Legion.Instance.cBestCrew, Legion.Instance.SelectedStage, 1, GoToBattle);.
//				break;
//			case (Byte)TutorialSubPart.EQUIP:
//				//FadeEffectMgr.Instance.FadeOut();
//				PopupManager.Instance.ShowLoadingPopup(1);
//				Server.ServerMgr.Instance.RequestTutorial((Byte)(info.u1TutorialType-1), info.u1TutorialPart, RemoveLoading);
//				//PopupManager.Instance.ShowLoadingPopup(1);
//				//Server.ServerMgr.Instance.StartStage(Legion.Instance.cBestCrew, Legion.Instance.SelectedStage, 1, GoToBattle);.
//				break;
			case (Byte)TutorialSubPart.LAST_TUTORIAL:
                 bClear = true;
                 au1Step[0] = Server.ConstDef.LastTutorialStep;

                 PopupManager.Instance.ShowLoadingPopup(1);
                 Server.ServerMgr.Instance.RequestTutorial((Byte)(info.u1TutorialType - 1), Server.ConstDef.LastTutorialStep, EndLastTutorial);
                 
                 //if (Legion.Instance.u1VIPLevel == 0)
                 //    Legion.Instance.AddVIPPoint(0);
                 //
                 //PopupManager.Instance.ShowLoadingPopup(1);
                 //Server.ServerMgr.Instance.RequestTutorial((Byte)(info.u1TutorialType - 1), Server.ConstDef.LastTutorialStep, RemoveLoading);
                 //
                 //LobbyScene lobbyScene = Scene.GetCurrent() as LobbyScene;
                 //if (EventInfoMgr.Instance.u1EventCount != 0)
                 //{
                 //    Legion.Instance.AddLoginPopupStep(Legion.LoginPopupStep.LOGIN_REWARED);
                 //    lobbyScene.CheckLoginPopupStep();
                 //    //Server.ServerMgr.Instance.LegionMark(2, lobbyScene.MonthlyLoginReward);
                 //    //lobbyScene._eventPanel.GetComponent<EventPanel> ().OnClickMenu (0);
                 //}
                 ////Legion.Instance.checkLoginAchievement = 0;
                 break;   
			}
			break;
		case 2: case 4: case 6: case 7:
			switch(info.u1TutorialPart){
			case 1:
				PopupManager.Instance.ShowLoadingPopup(1);
				au1Step[info.u1TutorialType-1] = Server.ConstDef.LastTutorialStep;
				Server.ServerMgr.Instance.RequestTutorial((Byte)(info.u1TutorialType-1), Server.ConstDef.LastTutorialStep, RemoveLoading);
				PopupManager.Instance.ShowTutorialEnd(info.u1TutorialType);

				break;
			}

			break;
		
		case 3:
			switch(info.u1TutorialPart){
			case 3:
				//PopupManager.Instance.ShowLoadingPopup(1);
				//PopupManager.Instance.ShowTutorialEnd(info.u1TutorialType);
				PopupManager.Instance.ShowTutorialEnd(1);
				au1Step[info.u1TutorialType-1] = Server.ConstDef.LastTutorialStep;
				Server.ServerMgr.Instance.RequestTutorial((Byte)(info.u1TutorialType - 1), Server.ConstDef.LastTutorialStep, RemoveLoading);
				//Server.ServerMgr.Instance.RequestTutorial((Byte)(info.u1TutorialType-1), Server.ConstDef.LastTutorialStep, RemoveLoading);

				break;
			}

			break;
            
        case 5: 
			switch(info.u1TutorialPart){
			case 5:
				//PopupManager.Instance.ShowLoadingPopup (1);
				au1Step [4] = Server.ConstDef.LastTutorialStep;
				//Server.ServerMgr.Instance.RequestTutorial ((Byte)(info.u1TutorialType - 1), Server.ConstDef.LastTutorialStep, RemoveLoading);
				PopupManager.Instance.ShowTutorialEnd (5);
				//if (Legion.Instance.checkLoginAchievement == 2) {
				//	LobbyScene lobbyScene = Scene.GetCurrent () as LobbyScene;
                //    if(EventInfoMgr.Instance.u1EventCount != 0)
				//	    lobbyScene._eventPanel.GetComponent<EventPanel> ().OnClickMenu (0);
				//	Legion.Instance.checkLoginAchievement = 0;
				//}
				break;
			}			
			break;

		case 8:
			switch(info.u1TutorialPart){
			case 2:
				//PopupManager.Instance.ShowLoadingPopup(1);
				au1Step[info.u1TutorialType-1] = Server.ConstDef.LastTutorialStep;
				//Server.ServerMgr.Instance.RequestTutorial((Byte)(info.u1TutorialType-1), Server.ConstDef.LastTutorialStep, RemoveLoading);
				PopupManager.Instance.ShowTutorialEnd(info.u1TutorialType);

				break;
			}

			break;

		case 9:
			switch(info.u1TutorialPart){
			case 3:
				//PopupManager.Instance.ShowLoadingPopup(1);
				au1Step[info.u1TutorialType-1] = Server.ConstDef.LastTutorialStep;
				//Server.ServerMgr.Instance.RequestTutorial((Byte)(info.u1TutorialType-1), Server.ConstDef.LastTutorialStep, RemoveLoading);
				PopupManager.Instance.ShowTutorialEnd(info.u1TutorialType);

				break;
			}

			break;
		}

		if(cCont != null) cCont.ResumeGame();
	}

	void RemoveLoading(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();
		
		DebugMgr.Log(err);
		
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.LEGION_TUTORIAL, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
			if(bClear){
				PopupManager.Instance.ShowTutorialEnd(1);
				bClear = false;
			}
		}
	}

	void GoToCreateCharacterScene(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();
		
		DebugMgr.Log(err);
		
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.LEGION_TUTORIAL, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
			FadeEffectMgr.Instance.QuickChangeScene (MENU.CREATE_CHARACTER);
		}
	}
    
    void EndLastTutorial(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();

        DebugMgr.Log(err);

        if (err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup();
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.LEGION_TUTORIAL, err), Server.ServerMgr.Instance.CallClear);
        }
        else
        {
            if (bClear)
            {
                //GameObject obj = new GameObject();
                //obj.AddComponent<MonoBehaviour>();
                PopupManager.Instance.StartCoroutine(PopupManager.Instance.EndLastTutorialPopup());
                bClear = false;
            }
        }
    }
    //public IEnumerator EndLastTutorialPopup()
    //{
    //    PopupManager.Instance.ShowTutorialEnd(1);
    //    yield return new WaitForSeconds(2.5f);
    //
    //    if (Legion.Instance.u1VIPLevel == 0)
    //        Legion.Instance.AddVIPPoint(0);
    //
    //    while (true)
    //    {
    //        if (PopupManager.Instance.objOKPopup.activeSelf == false)
    //            break;
    //
    //        yield return null;
    //    }
    //
    //    yield return new WaitForSeconds(0.5f);
    //
    //    LobbyScene lobbyScene = Scene.GetCurrent() as LobbyScene;
    //    if (EventInfoMgr.Instance.u1EventCount != 0)
    //    {
    //        Legion.Instance.AddLoginPopupStep(Legion.LoginPopupStep.LOGIN_REWARED);
    //        lobbyScene.CheckLoginPopupStep();
    //    }
    //}

}
