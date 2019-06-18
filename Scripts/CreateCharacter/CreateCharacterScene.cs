using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

using System.Text.RegularExpressions;

public class CreateCharacterScene : MonoBehaviour {

	private string characterName;
	private int classID;
	private int hairIndex;
	private int hairColorIndex;
	private int faceIndex;

	public int ClassID
	{
		get{ return classID; }
		set{ classID = value; }
	}

	public int HairIndex
	{
		get{ return hairIndex; }
		set{ hairIndex = value; }
    }

	public int HairColorIndex
	{
		get{ return hairColorIndex; }
		set{ hairColorIndex = value; }
    }

	public int FaceIndex
	{
		get{ return faceIndex; }
		set{ faceIndex = value; }
    }

	public CustomListWindow customListWindow;
	public InputField inputField;
	public Toggle[] topToggles;
	public GameObject[] tabMenus;
	public GameObject backButton;
	public GameObject doneButton;
    public Animator doneAnim;
	private int m_nTopToggleIndex = 0;

	// Use this for initialization
	void Awake () {	          
        FadeEffectMgr.Instance.FadeIn();
        
		if(Legion.Instance.acHeros.Count == 0 || Legion.Instance.cTutorial.bIng || Legion.Instance.cTutorial.au1Step[4] == 2)
			backButton.SetActive(false);
		else
			PopupManager.Instance.AddPopup(gameObject, OnClickBack);
            
		tabMenus[0].SetActive(true);

		customListWindow.SetClassList();
        for(int i=0; i<ClassInfo.LAST_CLASS_ID; i++)
        {
            if(Legion.Instance.charAvailable[i] == 1)
            {
		        customListWindow.RefreshScrollBtn(i);
                break;
            }
        }
        
        Legion.Instance.cTutorial.CheckTutorial(MENU.CREATE_CHARACTER);
	}

	public void OnClickToggle(int index)
	{
		// 2016. 7. 18 jy
		// 3D 카메라 무빙 꼬임으로 
		// 카메라 이동중일때는 토글 메뉴를 클릭할 수 없도록 함
		if( customListWindow.IsCamMoving == true)
		{
			topToggles[m_nTopToggleIndex].isOn = true;
			return;
		}

		// 2016. 09. 05 jy
		// 오픈 상태가 아니라면 다른 메뉴로 넘기지 않는다
		if(customListWindow.SelectClassLockType != 1)
		{
			topToggles[m_nTopToggleIndex].isOn = true;
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetText("lock_menu"), null);
			return;
		}

		if(!topToggles[index].isOn)
		{
			tabMenus[index].SetActive(false);
			return;
		}

		m_nTopToggleIndex = index;
		customListWindow.IsCamMoving = true;
		// 0 : Class
		// 1 : Hair
		// 2 : HairColor
		// 3 : Face
		tabMenus[index].SetActive(true);
 
		customListWindow.ZoomInOut(index);
		/*
		if(index == 0)
			customListWindow.ZoomInOut(false);
		else
			customListWindow.ZoomInOut(true);
		*/
		customListWindow.ShowAnim(index);
	}

	public void OnClickBack()
	{
		object[] obj = new object[1];
		obj[0] = "LobbyScene";
		PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("popup_title_char_create_cancel"), TextManager.Instance.GetText("popup_desc_char_create_cancel"), ChangeScene, obj);
	}

	public void ChangeScene(object[] param)
	{
        FadeEffectMgr.Instance.FadeOut();
		//AssetMgr.Instance.SceneLoad(param[0].ToString());
        StartCoroutine(SceneLoad(param));
	}

    IEnumerator SceneLoad(object[] param)
    {
        yield return new WaitForSeconds(0.3f);
        AssetMgr.Instance.SceneLoad("LobbyScene_background_01", false);
        AssetMgr.Instance.SceneLoad(param[0].ToString(), true);
    }

	public void OnClickDone()
	{
		if(string.IsNullOrEmpty(characterName))
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_char_create"),  TextManager.Instance.GetErrorText("character_enter_name", "", false), null);
			return;
		}
		else if(characterName.Contains(" "))
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_char_create"), TextManager.Instance.GetErrorText("crew_space_name", "", false), null);
			return;
		}
			
		if(Regex.Matches(characterName, @"[\s\Wㄱ-ㅎㅏ-ㅣ]").Count != 0)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_char_create"), TextManager.Instance.GetText("popup_char_name_special"), null);
			return;
		}
#if UNITY_EDITOR
		DebugMgr.Log("Class : " + classID);
		DebugMgr.Log("Hair : " + hairIndex);
		DebugMgr.Log("Color : " + hairColorIndex);
		DebugMgr.Log("Face : " + faceIndex);
#endif
		RequsetCreateCharacter();
	}

    public void SetCharacteDefaultName()
    {
        ClassInfo classInfo = ClassInfoMgr.Instance.GetInfo((UInt16)(classID));
        if(classInfo != null)
        {
            characterName = classInfo.ClassDefaultName;
            inputField.text = classInfo.ClassDefaultName;
        }
        else
        {
            inputField.text = "";
        }
    }

	public void SetCharacterName()
	{
		characterName = inputField.text;
		DebugMgr.Log(characterName);
	}

	public void RequsetCreateCharacter()
	{
#if UNITY_EDITOR
		string log = string.Format("[{0}] [characterName : {1}] [class : {2}] [hair : {3}] [hairColor : {4}] [face : {5}]", 
		                           Server.MSGs.CHAR_CREATE, characterName, classID, hairIndex, hairColorIndex, faceIndex);

		DebugMgr.Log(log);
#endif
		PopupManager.Instance.ShowLoadingPopup(1);

        //최초 생성 요청
        if (Legion.Instance.cTutorial.au1Step [0] < Server.ConstDef.LastTutorialStep) {
			Server.ServerMgr.Instance.CreateCharacter (characterName, (UInt16)classID, (byte)hairIndex, (byte)hairColorIndex, (byte)faceIndex, 0, (byte)TutorialSubPart.CREATE_CHAR, AckCreateSuccess);
		} else if (Legion.Instance.cTutorial.au1Step [4] == 2) {
			Server.ServerMgr.Instance.CreateCharacter (characterName, (UInt16)classID, (byte)hairIndex, (byte)hairColorIndex, (byte)faceIndex, 4, 3, AckCreateSuccess);
		} else {
			Server.ServerMgr.Instance.CreateCharacter (characterName, (UInt16)classID, (byte)hairIndex, (byte)hairColorIndex, (byte)faceIndex, 255, 0, AckCreateSuccess);
		}
	}

	public void AckCreateSuccess(Server.ERROR_ID err)
	{
#if UNITY_EDITOR
		DebugMgr.Log("ACK : " + err);
#endif
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			// 캐릭터 생성시 치명적 에러코드를 제외하고는 에러코드를 보여 주지 않는다
			string errText = TextManager.Instance.GetError(Server.MSGs.CHAR_CREATE, err);
			if(err > Server.ERROR_ID.LOGICAL_ERROR)
			{
				int subStringIdx = errText.LastIndexOf("\n");
				errText = errText.Substring(0, subStringIdx);
			}
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_char_create"), errText, Server.ServerMgr.Instance.CallClear);
            doneAnim.Play("Default");
			return;
		}
		else
		{
			//최초 생성시
			if (Legion.Instance.acHeros.Count == 1)
			{
				Legion.Instance.cBestCrew = Legion.Instance.acCrews[0];
				Legion.Instance.cBestCrew.UnLock();
				Legion.Instance.cBestCrew.Fill(Legion.Instance.cLastUpdatedHero, 0);
			}
          
            for(int i=0; i<Legion.Instance.cLastUpdatedHero.acEquips.Length; i++)
            {
                Legion.Instance.cInventory.dicInventory[Legion.Instance.cLastUpdatedHero.acEquips[i].u2SlotNum].isNew = false;
            }
            Legion.Instance.cLastUpdatedHero = null;
            
            //생성 성공시 애니메이션 실행
            customListWindow.ZoomInOut(/*false,*/ 0);
            doneAnim.Play("Done");
            customListWindow.PlayDoneAnim();
			doneButton.gameObject.SetActive(false);
		}
	}
    
    public void EndAnim()
    {            
        //애니메이션 종료 후 씬 전환
        FadeEffectMgr.Instance.FadeOut();
        StartCoroutine(LobbySceneLoad());    
    }

    IEnumerator LobbySceneLoad()
    {
		ObjMgr.Instance.RemoveHeroModelPool ();
        yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);

		if (Legion.Instance.cTutorial.au1Step [4] != 2)
			Legion.Instance.cQuest.CheckEndDirection (AchievementTypeData.CreateChar);

        AssetMgr.Instance.SceneLoad("LobbyScene_background_01", false);
        AssetMgr.Instance.SceneLoad("LobbyScene", true);
    }
}