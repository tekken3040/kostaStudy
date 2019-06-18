using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TalkPopup : MonoBehaviour {

	enum TalkPopupType{
		Tutorial,
		Quest,
		QuestComplete,
		Notice
	}

	enum PageType{
		None,
		Story,
		Talk,
		Indi,
	}

	public AudioClip SndChar;
	public AudioClip SndText;
	public AudioClip SndIndi;

	public GameObject objStory;
	public RectTransform imgStoryBG;
	public Text txtStoryText;
	public GameObject objTalk;
	public Image imgTalkFadeBox;
	public Image imgTalkBG;
	public GameObject objTextBox;
	public Text txtBoxText;
	public GameObject[] objTalkChar;
	public GameObject objIndicator;
	public GameObject objTouch;
	public GameObject skipButton;
	public Transform btnParent;

	Button BaseBtn;
	Toggle BaseToggle;

	TalkUI[] acTalkUI = new TalkUI[2];

	float storySpd = 0.8f;
	float touchSpd = 1.0f;

	bool bDirection = false;
	bool bBGDriection = false;

	PageType ePageType;

	float moveSize = 100f;

	float dirTime = 0.2f;

	bool bSkip = false;
	bool bTalkSkip = false;

	CanvasGroup cg;

	public class TalkUI
	{
		public Image _Illust;
		public RectTransform _Mass;
		public Text _Name;
		public Text _Speech;
		public Vector2 _firstPt;

		public TalkUI(){

		}
	}

	TutorialInfo cInfo;
	QuestDirection cDirectionInfo;

	TalkPopupType ePopupType;

	int page = 0;
	int talkIndex = 0;
	int textLength = 0;
	int textIndex = 0;

	TalkInfo cCurTalk = null;

	bool bCall = false;

	GameObject BtnIndic;

	private const int CrewCharStartPos = 201;

	float storyY;

	public GameObject SkipPopup;

	string savedSpeech = "";
	public string startCommand = null;
	public string endCommand = null;

	private const float fixedFadeTime = 0.5f;

	bool bChangeBG = false;

	public Text skipTitle;
	public Text skipMsg;

	QuestRewardPopup rewardPopup;

	MENU curMenu;

	string noticeName;

    private byte u1TypeVal;
    private object objVal;

    float SndDelay;

	void Awake(){
		cg = gameObject.GetComponent<CanvasGroup>();
		BtnIndic = AssetMgr.Instance.AssetLoad("Prefabs/UI/Tutorial/BtnIndic.prefab", typeof(GameObject)) as GameObject;

		for (int i=0; i<objTalkChar.Length; i++) {
			acTalkUI [i] = new TalkUI();
			acTalkUI [i]._Illust = objTalkChar [i].transform.FindChild ("Illust").GetComponent<Image> ();
			acTalkUI [i]._Mass = objTalkChar [i].transform.FindChild ("TextMass").GetComponent<RectTransform>();
			acTalkUI [i]._Name = acTalkUI [i]._Mass.FindChild ("Title").GetComponent<Text> ();
			acTalkUI [i]._Speech = acTalkUI [i]._Mass.FindChild ("Content").GetComponent<Text> ();
			acTalkUI [i]._firstPt = acTalkUI [i]._Illust.rectTransform.anchoredPosition;
		}
	}

	public void SetPopup(byte info){
		cg.alpha = 1.0f;
		imgTalkFadeBox.color = new Color(0,0,0,0);
		ePopupType = TalkPopupType.QuestComplete;

		page = 0;
		bSkip = false;
		skipButton.SetActive(false);

		cDirectionInfo = new QuestDirection ();
		cDirectionInfo.lstTalks = new List<string>();
		cDirectionInfo.lstTalks.Add(info.ToString());

		cDirectionInfo.u1PageCount = 1;
		cDirectionInfo.u2QuestID = (ushort)info;

		CheckPage();

		bCall = false;
	}

	public void SetPopup(MENU info, byte u1Type, ushort u2Star, object obj){
		if (Legion.Instance.bStageFailed) {
			Legion.Instance.bStageFailed = false;
			TalkInfo talk = TutorialInfoMgr.Instance.GetNoticeTalkInfo (((ushort)info).ToString (), 2);
			noticeName = talk.sTalkName;
		} else {
			TalkInfo talk = new TalkInfo ();
			switch(u1Type){
			case 1:
				talk = TutorialInfoMgr.Instance.GetNoticeTalkInfo (((ushort)info).ToString (), 1);
				break;
			case 3:
				talk = TutorialInfoMgr.Instance.GetGachaTalkInfo (u2Star);
				break;
			case 4:
				talk = TutorialInfoMgr.Instance.GetForgeTalkInfo (u2Star);
				break;
			}

			if (talk == null) {
				gameObject.SetActive (false);
                PopupManager.Instance.bUseNotice = false;
				return;
			}
			noticeName = talk.sTalkName;
            PopupManager.Instance.bUseNotice = true;
		}

        u1TypeVal = u1Type;
        objVal = obj;
		cg.alpha = 1.0f;
		imgTalkFadeBox.color = new Color(0,0,0,0);
		ePopupType = TalkPopupType.Notice;

		page = 0;
		bSkip = false;
		skipButton.SetActive(false);
		
		curMenu = info;

		cDirectionInfo = new QuestDirection ();
		cDirectionInfo.lstTalks = new List<string>();
		cDirectionInfo.lstTalks.Add(((ushort)info).ToString());

		cDirectionInfo.u1PageCount = 1;
		
		CheckPage();
		
		bCall = false;
	}

	public void SetPopup(QuestDirection info, GameObject rPop){
		cg.alpha = 1.0f;
		imgTalkFadeBox.color = new Color(0,0,0,0);
		ePopupType = TalkPopupType.Quest;

		page = 0;
		bSkip = false;
		skipButton.SetActive(false);
		
		cDirectionInfo = info;
		
		CheckPage();

		if(rPop != null) rewardPopup = rPop.GetComponent<QuestRewardPopup>();
		//PopupManager.Instance.bUseNotice = true;
		bCall = false;
	}

	public void SetPopup(TutorialInfo info){
		cg.alpha = 1.0f;
		imgTalkFadeBox.color = new Color(0,0,0,0);
		ePopupType = TalkPopupType.Tutorial;

		page = 0;
		bSkip = false;
		skipButton.SetActive(false);

		cInfo = info;

		CheckPage();

		bCall = false;
	}

	IEnumerator checkWaiting(){
		while (PopupManager.Instance.showLoading) {
			yield return new WaitForFixedUpdate ();
		}
		NextPage ();
	}

	void NextPage()
	{

		bool bLast = false;

		if (ePopupType == TalkPopupType.Notice || ePopupType == TalkPopupType.QuestComplete) {
			bLast = true;
		}else if (ePopupType == TalkPopupType.Quest) {
			if (page+1 >= cDirectionInfo.u1PageCount) {
				bLast = true;
			}
		} else if (ePopupType == TalkPopupType.Tutorial) {
			if (page+1 >= cInfo.u1PageCount) {
				bLast = true;
			}
		}

		if (!bLast) {
			if (PopupManager.Instance.showLoading) {
				StopCoroutine ("checkWaiting");
				StartCoroutine ("checkWaiting");
				return;
			}
		}

		if(!gameObject.activeSelf) return;

		DeleteObject();

		if (ePageType == PageType.Story) {
			ePageType = PageType.None;
			DelayNext(1.0f, true);
			return;
		}

		float delayTime = dirTime;
		bChangeBG = false;

		if (ePageType == PageType.Talk) {
			if (cCurTalk != null)
			{
				if (cCurTalk.sBGName == "End") {
					if (ePopupType == TalkPopupType.Quest) {
						if (page + 1 < cDirectionInfo.u1PageCount) {
							if(SetBGChange ()) delayTime = fixedFadeTime;
						}
					} else if (ePopupType == TalkPopupType.Tutorial) {
						if (page + 1 < cInfo.u1PageCount) {
							if(SetBGChange ()) delayTime = fixedFadeTime;
						}
					}
				} else {
					if(SetEndBG ()) delayTime = fixedFadeTime;
				}
			}

			switch(cCurTalk.eAnimType){
			case AnimationType.Out: case AnimationType.InOut:
				AnimTextBoxOut();
				AnimIllustOut(true);
				ePageType = PageType.None;
				DelayNext(delayTime, false);
				return;
				break;
			case AnimationType.FixOut: case AnimationType.FixInOut:
				AnimTextBoxOut();
				AnimIllustOut(false);
				ePageType = PageType.None;
				DelayNext(delayTime, false);
				return;
				break;
			}
		}

		page++;

		if (bLast) {
			EndTutorial();
			return;
		}

		cCurTalk = null;
		CheckPage();
	}

	bool SetBGChange(){
		TalkInfo nextInfo = GetNextTalkInfo ();

		if (nextInfo == null) {
			return false;
		}
			
		if (nextInfo.sButtonID == "0" && nextInfo.sBGName != "0") {
			if (cCurTalk.sBGName == "End") {
				imgTalkFadeBox.enabled = true;
				imgTalkFadeBox.color = Color.black;
			} else {
				if (nextInfo.bBGOnOff) {
					LeanTween.alpha (imgTalkFadeBox.GetComponent<RectTransform> (), 160f / 255f, fixedFadeTime);
				} else {
					LeanTween.alpha (imgTalkFadeBox.GetComponent<RectTransform> (), 0f, fixedFadeTime);
				}
			}
		} else if (nextInfo.sButtonID != "0"){
			if (nextInfo.bBGOnOff) {
				LeanTween.alpha (imgTalkFadeBox.GetComponent<RectTransform> (), 160f / 255f, fixedFadeTime);
			} else {
				LeanTween.alpha (imgTalkFadeBox.GetComponent<RectTransform> (), 0f, fixedFadeTime);
			}
		}
		bChangeBG = true;
		LeanTween.alpha (imgTalkBG.GetComponent<RectTransform> (), 0.0f, fixedFadeTime);
		if(!bBGDriection){
			bBGDriection = true;
			StartCoroutine (CheckBGDirection ());
		}

		return true;
	}

	bool SetEndBG(){
		TalkInfo nextInfo = GetNextTalkInfo ();

		if (nextInfo == null) {
			return false;
		}

		if (cCurTalk.bBGOnOff == nextInfo.bBGOnOff)
			return false;

		bChangeBG = true;

		if (nextInfo.bBGOnOff) {
			LeanTween.alpha (imgTalkFadeBox.GetComponent<RectTransform> (), 160f / 255f, fixedFadeTime);
		} else {
			LeanTween.alpha (imgTalkFadeBox.GetComponent<RectTransform> (), 0f, fixedFadeTime);
		}

		if(!bBGDriection){
			bBGDriection = true;
			StartCoroutine (CheckBGDirection ());
		}

		return true;
	}

	TalkInfo GetNextTalkInfo(){
		if (ePopupType == TalkPopupType.Tutorial) {
			if (page + 1 >= cInfo.u1PageCount) {
				return null;
			}
		} else {
			if (page + 1 >= cDirectionInfo.u1PageCount) {
				return null;
			}
		}

		if(ePopupType == TalkPopupType.Notice) return null;
		else if(ePopupType == TalkPopupType.QuestComplete) return QuestInfoMgr.Instance.GetCompTalkInfo(cDirectionInfo.lstTalks[page+1]);
		else if(ePopupType == TalkPopupType.Quest) return QuestInfoMgr.Instance.GetTalkInfo(cDirectionInfo.lstTalks[page+1]);
		else if(ePopupType == TalkPopupType.Tutorial) return TutorialInfoMgr.Instance.GetTalkInfo(cInfo.lstTalks[page+1].sName);

		return null;
	}

	void AnimTextBoxIn(){
		if (cCurTalk.u2ClassID == 0) {
			SoundManager.Instance.PlayEff (SndText);
			objTextBox.transform.localScale = new Vector3 (1f, 0f, 1f);
			LeanTween.cancel (objTextBox);
			LeanTween.scale (objTextBox.GetComponent<RectTransform>(), Vector3.one, dirTime).setEase (LeanTweenType.easeSpring);
		} else {
			int idx = 0;
			if (cCurTalk.u1Pos == 1) {
				
			} else if (cCurTalk.u1Pos == 2) {
				idx = 1;
			}
			
			acTalkUI [idx]._Mass.localScale = new Vector3 (1f, 0f, 1f);
			LeanTween.cancel (acTalkUI [idx]._Mass.gameObject);
			LeanTween.scale (acTalkUI [idx]._Mass, Vector3.one, dirTime).setEase (LeanTweenType.easeSpring);
		}
	}

	void AnimTextBoxOut(){
		if (cCurTalk.u2ClassID == 0) {
			SoundManager.Instance.PlayEff (SndText);
			objTextBox.transform.localScale = Vector3.one;
			LeanTween.cancel (objTextBox);
			LeanTween.scale (objTextBox.GetComponent<RectTransform> (), new Vector3 (1f, 0f, 1f), dirTime/2f);
		} else {
			int idx = 0;
			if (cCurTalk.u1Pos == 1) {
				
			} else if (cCurTalk.u1Pos == 2) {
				idx = 1;
			}
			
			acTalkUI [idx]._Mass.localScale = Vector3.one;
			LeanTween.cancel (acTalkUI [idx]._Mass.gameObject);
			LeanTween.scale (acTalkUI [idx]._Mass, new Vector3 (1f, 0f, 1f), dirTime / 2f);
		}
	}

	void AnimIllustIn(bool bMove){
		SoundManager.Instance.PlayEff (SndChar);
		float moveX = moveSize;
		int idx = 0;
		if (cCurTalk.u1Pos == 1) {

		}else if (cCurTalk.u1Pos == 2) {
			moveX = -moveSize;
			idx = 1;
		}

		acTalkUI[idx]._Illust.color = new Color(1,1,1,0f);

		if(bMove) acTalkUI[idx]._Illust.rectTransform.anchoredPosition = new Vector2(acTalkUI[idx]._firstPt.x-moveX, acTalkUI[idx]._Illust.rectTransform.anchoredPosition.y);
		else acTalkUI[idx]._Illust.rectTransform.anchoredPosition = acTalkUI[idx]._firstPt;

		LeanTween.cancel(acTalkUI [idx]._Illust.gameObject);
		if(bMove) LeanTween.move(acTalkUI[idx]._Illust.rectTransform, new Vector2(moveX, 0), dirTime);
		LeanTween.alpha(acTalkUI[idx]._Illust.rectTransform, 1.0f, dirTime);

		bDirection = true;
        if(this.gameObject.activeSelf)
		    StartCoroutine(CheckDirection(dirTime));
	}

	void AnimIllustOut(bool bMove){
		float moveX = moveSize;
		int idx = 0;
		if (cCurTalk.u1Pos == 1) {
			
		}else if (cCurTalk.u1Pos == 2) {
			moveX = -moveSize;
			idx = 1;
		}

		acTalkUI[idx]._Illust.color = new Color(1,1,1,1);
		
		acTalkUI [idx]._Illust.rectTransform.anchoredPosition = acTalkUI [idx]._firstPt;

		LeanTween.cancel(acTalkUI [idx]._Illust.gameObject);
		if(bMove) LeanTween.move(acTalkUI[idx]._Illust.rectTransform, new Vector2(-moveX, 0),dirTime);
		LeanTween.alpha(acTalkUI[idx]._Illust.rectTransform, 0.0f, dirTime);

		bDirection = true;
		StartCoroutine(CheckDirection(dirTime));
	}

	void CheckPage()
	{
		byte type = 2;
		float delay = 0f;
		float questMsgTime = 0f;
		bTalkSkip = false;

		if (ePopupType == TalkPopupType.Notice || ePopupType == TalkPopupType.QuestComplete) {
			bSkip = false;
			type = 2;
		}else if (ePopupType == TalkPopupType.Quest) {
			bSkip = cDirectionInfo.bSkip;
			type = 2;
			if(page == 0 && cDirectionInfo.u1DirectionPos == 1) questMsgTime = 2f;
		}else if (ePopupType == TalkPopupType.Tutorial) {
			type = cInfo.lstTalks [page].u1Type;
			//bSkip = cInfo.bSkip;
			bSkip = true;
		}

		talkIndex = 0;
		textIndex = 0;

		objTouch.SetActive(true);
		SkipPopup.SetActive(false);

		objStory.SetActive(false);
		objTalk.SetActive(false);
		objIndicator.SetActive(false);

		objTextBox.SetActive(false);
		for (int i=0; i<objTalkChar.Length; i++) {
			objTalkChar[i].SetActive(false);
		}
		
		switch (type)
		{
		case 1:
			ePageType = PageType.Story;
			LoadStory();
			break;
		case 2:
			ePageType = PageType.Talk;
			if(ePopupType == TalkPopupType.Notice) cCurTalk = TutorialInfoMgr.Instance.GetNoticeTalkInfo(noticeName);
			else if(ePopupType == TalkPopupType.QuestComplete) cCurTalk = QuestInfoMgr.Instance.GetCompTalkInfo(cDirectionInfo.lstTalks[page]);
			else if(ePopupType == TalkPopupType.Quest) cCurTalk = QuestInfoMgr.Instance.GetTalkInfo(cDirectionInfo.lstTalks[page]);
			else if(ePopupType == TalkPopupType.Tutorial) cCurTalk = TutorialInfoMgr.Instance.GetTalkInfo(cInfo.lstTalks[page].sName);
			objTalk.SetActive(true);

			if(CheckStartFadeBox()) delay = cCurTalk.fDelay+fixedFadeTime;
			else delay = cCurTalk.fDelay;
            
			if(ePopupType == TalkPopupType.Notice || ePopupType == TalkPopupType.QuestComplete) delay = fixedFadeTime;
			
			if(delay > 0){
				imgTalkFadeBox.enabled = true;
				bDirection = true;
				Invoke("LoadTalkAndBtn", delay+fixedFadeTime+questMsgTime);
				Invoke("CheckBG", delay);
			}else{
				LoadTalkAndBtn();
				CheckBG();
			}
			break;
		case 3:
			ePageType = PageType.Indi;
			LoadIndicator();
			break;
		case 4:
			ePageType = PageType.Talk;
			cCurTalk = TutorialInfoMgr.Instance.GetTalkInfo(cInfo.lstTalks[page].sName);
			objTalk.SetActive(true);

			if(CheckStartFadeBox()) delay = cCurTalk.fDelay+fixedFadeTime;
			else delay = cCurTalk.fDelay;
			
			if(delay > 0){
				bDirection = true;
				Invoke("LoadTalkAndBtn", delay+fixedFadeTime+questMsgTime);
				Invoke("LoadIndicator", delay+fixedFadeTime+questMsgTime);
				Invoke("CheckBG", delay);
			}else{
				LoadTalkAndBtn();
				LoadIndicator();
				CheckBG();
			}
			break;
		case 5:
			ePageType = PageType.Indi;
			GameObject tempObj = Instantiate((GameObject)AssetMgr.Instance.AssetLoad("Prefabs/UI/Tutorial/" + cInfo.lstTalks[page].sName + ".prefab", typeof(GameObject)));
			RectTransform rectTr = tempObj.GetComponent<RectTransform>();

            cCurTalk = null;

            TutorialEventPopup eventPopup = tempObj.GetComponent<TutorialEventPopup>();
			eventPopup.closeEvent = TouchMe;
			eventPopup.SetPopup();

			rectTr.SetParent(btnParent);
			rectTr.localScale = Vector3.one;
			rectTr.localPosition = Vector3.zero;
			rectTr.sizeDelta = Vector2.zero;
			break;
		case 6:
			ePageType = PageType.Indi;
			GameObject tempObj2 = Instantiate ((GameObject)AssetMgr.Instance.AssetLoad ("Prefabs/UI/Tutorial/" + cInfo.lstTalks [page].sName + ".prefab", typeof(GameObject)));
			RectTransform rectTr2 = tempObj2.GetComponent<RectTransform> ();

			skipButton.SetActive (false);
			cCurTalk = null;

			rectTr2.SetParent(btnParent);
			rectTr2.localScale = Vector3.one;
			rectTr2.localPosition = Vector3.zero;
			rectTr2.sizeDelta = Vector2.zero;
			break;
		}
	}

	bool CheckStartFadeBox(){

		if ((!imgTalkFadeBox.enabled || imgTalkFadeBox.color.a == 0) && cCurTalk.bBGOnOff) {
			return true;
		}

		return false;
	}

	bool CheckEndFadeBox(){
		if (imgTalkFadeBox.enabled && !cCurTalk.bBGOnOff) {
			return true;
		}
		
		return false;
	}

	void CheckBG(){
		if (ePopupType == TalkPopupType.Quest && page == 0 && cDirectionInfo.u1DirectionPos == 1) {
			//퀘스트 수락 메시지
			GameObject tempQuestMsg = GameObject.Instantiate (AssetMgr.Instance.AssetLoad ("Prefabs/UI/Common/Pref_Quest_Msg.prefab", typeof(GameObject))) as GameObject;
			tempQuestMsg.transform.SetParent(transform);
			tempQuestMsg.transform.localScale = Vector2.one;
			tempQuestMsg.transform.localPosition = Vector2.zero;
			tempQuestMsg.GetComponent<QuestMsgPopup> ().SetData (QuestInfoMgr.Instance.GetQuestInfo (cDirectionInfo.u2QuestID), false);
		}

        if(cCurTalk.sButtonID != "0"){
            if(cCurTalk.fDelay > 0){
                bBGDriection = true;
				if(gameObject.activeSelf) StartCoroutine (CheckBGDirection ());
            } else {
                bBGDriection = false;
            }
        } 
        
		if (!bChangeBG) {
			if (CheckStartFadeBox () && cCurTalk.bBGOnOff) {
				imgTalkFadeBox.enabled = true;
				imgTalkFadeBox.color = new Color (0, 0, 0, 0);
				LeanTween.alpha (imgTalkFadeBox.GetComponent<RectTransform> (), 160f / 255f, fixedFadeTime);
            
				if (!bBGDriection) {
					bBGDriection = true;
					if (gameObject.activeSelf)
						StartCoroutine (CheckBGDirection ());
				}
			}
		}

		if (cCurTalk.sBGName != "0" && cCurTalk.sBGName != "End") {
			imgTalkBG.enabled = true;
			imgTalkBG.color = new Color (255, 255, 255, 0);
			imgTalkBG.sprite = AtlasMgr.Instance.GetSprite ("Sprites/TutorialBG/" + cCurTalk.sBGName + "." + cCurTalk.sBGName);
			LeanTween.alpha (imgTalkBG.GetComponent<RectTransform> (), 1.0f, fixedFadeTime);
			if (!bBGDriection) {
				bBGDriection = true;
				if(gameObject.activeSelf) StartCoroutine (CheckBGDirection ());
			}
		}
	}

//	void CheckEndBG(){
//		if(cCurTalk == null) return;
//
//		bool bFadeBox = CheckEndFadeBox ();
//
//		if(cCurTalk.sBGName == "End" && bFadeBox){
//			LeanTween.cancel (imgTalkBG.gameObject);
//			LeanTween.alpha (imgTalkBG.GetComponent<RectTransform>(), 0.0f, fixedFadeTime);
//			LeanTween.cancel (imgTalkFadeBox.gameObject);
//			LeanTween.alpha (imgTalkFadeBox.GetComponent<RectTransform> (), 0.0f, fixedFadeTime);
//
//			if (!bBGDriection) {
//				bBGDriection = true;
//				StartCoroutine (CheckBGDirection (bFadeBox));
//			}
//		}
//	}

	void DeleteObject(){

		for (int i=0; i<objIndicator.transform.childCount; i++) {
			Destroy(objIndicator.transform.GetChild(i).gameObject);
		}

		for (int i=0; i<btnParent.childCount; i++) {
			Destroy(btnParent.GetChild(i).gameObject);
		}

		BaseBtn = null;
		BaseToggle = null;
	}

	void LoadStory(){
		cCurTalk = null;

		touchSpd = 1.0f;

		objStory.SetActive(true);
		skipButton.SetActive(true);

		TutorialTextData tData = TutorialInfoMgr.Instance.GetStoryInfo(cInfo.lstTalks[page].sName);

		txtStoryText.rectTransform.anchoredPosition = new Vector2 (txtStoryText.rectTransform.anchoredPosition.x, 0f);
		txtStoryText.text = TextManager.Instance.GetText(tData.text);

		imgStoryBG.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite ("Sprites/Tutorial/" + tData.bg + "." + tData.bg);

		imgStoryBG.localScale = Vector3.one * 2f;

		storyY = txtStoryText.preferredHeight + 440f;

		FadeIn(1.0f);
	}

	void DelayNext(float time, bool fadeOut){
		bDirection = true;
		if(fadeOut) FadeEffectMgr.Instance.FadeOut(time);
		StartCoroutine(CheckDelayNext(time));
	}

	void FadeIn(float time){
		bDirection = true;
		FadeEffectMgr.Instance.FadeIn(time);
		StartCoroutine(CheckDirection(time));
	}

	void FadeOut(float time){
		bDirection = true;
		FadeEffectMgr.Instance.FadeOut(time);
		StartCoroutine(CheckDirection(time));
	}

	IEnumerator CheckDirection(float time){
		yield return new WaitForSeconds(time);
		bDirection = false;
	}

	IEnumerator CheckDelayNext(float time){
		yield return new WaitForSeconds(time);
		bDirection = false;
		if(bTalkSkip) NextPage();
		else TouchMe();

		bTalkSkip = false;
	}

	IEnumerator CheckBGDirection(){
		yield return new WaitForSeconds(fixedFadeTime);
		bBGDriection = false;
	}

	IEnumerator LoadBtn(){
        
        yield return new WaitForEndOfFrame();
        
        while (bDirection || bBGDriection)
        {
            yield return new WaitForEndOfFrame();
        }
        
		if (objIndicator.transform.childCount > 0) {
			imgTalkFadeBox.enabled = false;
			imgTalkFadeBox.color = new Color32 (0, 0, 0, 0);
		} else {
			if (cCurTalk.bBGOnOff) {
				imgTalkFadeBox.enabled = true;
				imgTalkFadeBox.color = new Color32 (0, 0, 0, 160);
			} else {
				imgTalkFadeBox.enabled = true;
				imgTalkFadeBox.color = new Color32 (0, 0, 0, 1);
			}
		}

		TutorialButton[] btns = GameObject.FindObjectsOfType(typeof(TutorialButton)) as TutorialButton[];
		if (btns.Length <= 0) {
			objTouch.SetActive(true);
			yield break;
		}

		for (int i=0; i<btns.Length; i++) {
			string newBtnID = cCurTalk.sButtonID;
			if(newBtnID == "Character_1"){
				if(Legion.Instance.SelectedCrew.acLocation[0] == null){
					newBtnID = "Character_2";
					if(Legion.Instance.SelectedCrew.acLocation[1] == null){
						newBtnID = "Character_3";
					}
				}
			}
			if(btns[i].id == newBtnID){
				Vector3 pos = btns[i].transform.root.position - btns[i].transform.position;
				pos.z = 0f;
				Vector3 localScale = btns[i].transform.localScale;
				float scale = transform.lossyScale.x/btns[i].transform.root.lossyScale.x;
				Vector2 size = btns[i].GetComponent<RectTransform>().sizeDelta;

				GameObject temp = Instantiate(btns[i].gameObject) as GameObject;
				float max = size.x > size.y ? size.x : size.y;
//                temp.GetComponent<RectTransform>().anchorMin = Vector2.one*0.5f;
//                temp.GetComponent<RectTransform>().anchorMax = Vector2.one*0.5f;

                temp.transform.SetParent (btnParent);
				temp.transform.localScale = localScale;
				temp.transform.position = btnParent.root.position -(pos*scale);
				temp.GetComponent<RectTransform>().sizeDelta = size;

                if (cCurTalk.eButtonType != ButtonType.None){
					GameObject tempIndi = Instantiate(BtnIndic);
					tempIndi.transform.SetParent(temp.transform);
					tempIndi.transform.localScale = Vector3.one;
					tempIndi.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

					switch(cCurTalk.eButtonType){
					case ButtonType.Round:
						tempIndi.GetComponent<RectTransform>().sizeDelta = new Vector2(max*1.15f,max*1.15f);
						tempIndi.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_01_renew.btn_indic_round");
						tempIndi.GetComponent<Image>().type = Image.Type.Simple;
						break;
					default:
						tempIndi.GetComponent<RectTransform> ().sizeDelta = size;
						tempIndi.GetComponent<Image> ().sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_01_renew.btn_indic_roundrect");
						break;
					}
				}

                if(btns[i].id == "crewbtn")
                {
                    btns[i].gameObject.SetActive(false);
                }

				if(temp.GetComponent<Button>() != null){
                    BaseBtn = btns[i].GetComponent<Button>();
                    temp.GetComponent<Button>().onClick = BaseBtn.onClick;
					temp.GetComponent<Button>().interactable = true;
					temp.GetComponent<Button>().onClick.AddListener(() => { TouchMe(); }); 
				}else if(temp.GetComponent<Toggle>() != null){
					BaseToggle = btns[i].GetComponent<Toggle>();
					temp.GetComponent<Toggle>().onValueChanged = BaseToggle.onValueChanged;
					temp.GetComponent<Toggle>().onValueChanged.AddListener((on) => { TouchMe(on); });
					//Vector2 apos = temp.GetComponent<RectTransform>().anchoredPosition;
					//temp.GetComponent<RectTransform>().anchoredPosition = new Vector2(apos.x+size.x/2f, apos.y);
				}
				yield break;
			}
		}

		objTouch.SetActive(true);
	}

	string GetSpeechs(){
		string ch1Name = "";
		string ch2Name = "";
		string ch3Name = "";

		if(Legion.Instance.cBestCrew != null){
			if(Legion.Instance.cBestCrew.acLocation[0] != null) ch1Name = Legion.Instance.cBestCrew.acLocation[0].sName;

			if(Legion.Instance.cBestCrew.acLocation[1] != null) ch2Name = Legion.Instance.cBestCrew.acLocation[1].sName;
			else if(cCurTalk.u2OtherClassID1 != 0) ch2Name = TextManager.Instance.GetText(TutorialInfoMgr.Instance.GetTalkCharInfo(cCurTalk.u2OtherClassID1).sName);

			if(Legion.Instance.cBestCrew.acLocation[2] != null) ch3Name = Legion.Instance.cBestCrew.acLocation[2].sName;
				else if(cCurTalk.u2OtherClassID2 != 0) ch3Name = TextManager.Instance.GetText(TutorialInfoMgr.Instance.GetTalkCharInfo(cCurTalk.u2OtherClassID2).sName);
		}

		savedSpeech = SpeechCommandSplit(string.Format(TextManager.Instance.GetText(cCurTalk.lstSpeechs[talkIndex]), Legion.Instance.sName, ch1Name, ch2Name, ch3Name));
		textLength = savedSpeech.Length;

		return savedSpeech;
	}

	void CheckBtnException(){
		if (cCurTalk.sButtonID == "Dispath_Crew_2") {
			if (Legion.Instance.SelectedCrew.u1Index != 1) {
				cCurTalk.sButtonID = "Dispath_Crew_1";
			} else {
				if (Legion.Instance.acCrews [1].u1Count <= 0) {
					for (int i = 2; i < Legion.Instance.acCrews.Length; i++) {
						if (Legion.Instance.acCrews [i].u1Count > 0) {
							cCurTalk.sButtonID = "Dispath_Crew_"+(i+1).ToString();
						}
					}
				}
			}
		}
	}

	void LoadTalkAndBtn(){
		if (cCurTalk.sButtonID != "0") {
			objTouch.SetActive (false);
			bSkip = false;
			CheckBtnException ();
			StartCoroutine ("LoadBtn");
			SoundManager.Instance.PlayEff (SndIndi);
		}

		skipButton.SetActive (bSkip);

		if (cCurTalk.u1TalkCount > 0) {
			textIndex = 0;
			if(cCurTalk.u2ClassID == 0){
				objTextBox.SetActive(true);
				txtBoxText.rectTransform.anchoredPosition = Vector2.zero;

				txtBoxText.text = GetSpeechs();
				float width = txtBoxText.preferredWidth;
				txtBoxText.text = "";
				//txtBoxText.rectTransform.anchoredPosition = new Vector2(-width/2f, 0f);
				AnimTextBoxIn();
                if(bDirection) StartCoroutine(CheckDirection(dirTime));
			}else{
				for (int i=0; i<objTalkChar.Length; i++) {
					if((int)cCurTalk.u1Pos == i+1){
						objTalkChar[i].SetActive(true);
						if(cCurTalk.u2ClassID < CrewCharStartPos){
							TalkCharacter tempChar = TutorialInfoMgr.Instance.GetTalkCharInfo(cCurTalk.u2ClassID);
							acTalkUI[i]._Name.text = TextManager.Instance.GetText(tempChar.sName);
							acTalkUI[i]._Illust.sprite = AtlasMgr.Instance.GetSprite("Sprites/Tutorial/ch_" + tempChar.u2ImageID.ToString("000")+".ch_" + tempChar.u2ImageID.ToString("000"));
						}else{
							int crewIdx = cCurTalk.u2ClassID - CrewCharStartPos;
							if(Legion.Instance.cBestCrew != null){
								if(Legion.Instance.cBestCrew.acLocation[crewIdx] != null){
									Character tempChar = Legion.Instance.cBestCrew.acLocation[crewIdx];
									acTalkUI[i]._Name.text = tempChar.sName;
									acTalkUI[i]._Illust.sprite = AtlasMgr.Instance.GetSprite("Sprites/Tutorial/class_"+tempChar.cClass.u2ID.ToString("000")+".class_"+tempChar.cClass.u2ID.ToString("000"));
								}else{
									if(cCurTalk.u2SubClassID > 0){
										TalkCharacter tempChar = TutorialInfoMgr.Instance.GetTalkCharInfo(cCurTalk.u2SubClassID);
										acTalkUI[i]._Name.text = TextManager.Instance.GetText(tempChar.sName);
										acTalkUI[i]._Illust.sprite = AtlasMgr.Instance.GetSprite("Sprites/Tutorial/class_"+tempChar.u2ImageID.ToString("000")+".class_"+tempChar.u2ImageID.ToString("000"));
									}else if(cCurTalk.u2OtherClassID1 > 0){
										TalkCharacter tempChar = TutorialInfoMgr.Instance.GetTalkCharInfo(cCurTalk.u2OtherClassID1);
										acTalkUI[i]._Name.text = TextManager.Instance.GetText(tempChar.sName);
										acTalkUI[i]._Illust.sprite = AtlasMgr.Instance.GetSprite("Sprites/Tutorial/class_"+tempChar.u2ImageID.ToString("000")+".class_"+tempChar.u2ImageID.ToString("000"));
									}else if(cCurTalk.u2OtherClassID2 > 0){
										TalkCharacter tempChar = TutorialInfoMgr.Instance.GetTalkCharInfo(cCurTalk.u2OtherClassID2);
										acTalkUI[i]._Name.text = TextManager.Instance.GetText(tempChar.sName);
										acTalkUI[i]._Illust.sprite = AtlasMgr.Instance.GetSprite("Sprites/Tutorial/class_"+tempChar.u2ImageID.ToString("000")+".class_"+tempChar.u2ImageID.ToString("000"));
									}
								}
							}
						}
						GetSpeechs();
						acTalkUI[i]._Speech.text = "";

						AnimTextBoxIn();

						switch(cCurTalk.eAnimType){
						case AnimationType.In: case AnimationType.InOut:
							AnimIllustIn(true);
							break;
						case AnimationType.FixIn: case AnimationType.FixInOut:
							AnimIllustIn(false);
							break;
						case AnimationType.None:
							LeanTween.cancel(acTalkUI[i]._Illust.gameObject);
							acTalkUI[i]._Illust.rectTransform.anchoredPosition = acTalkUI[i]._firstPt;
							acTalkUI[i]._Illust.color = Color.white;
							break;
						}
					}
				}
			}
		}else{
			bDirection = false;
		}

		//if(bDirection) bDirection = false;
	}

	void LoadIndicator(){
		objIndicator.SetActive(true);
		imgTalkFadeBox.enabled = false;
		imgTalkFadeBox.color = new Color32 (0, 0, 0, 0);

		string newIndiName = cInfo.lstTalks[page].sName;
		if(newIndiName == "Equip_indi_1"){
			if(Legion.Instance.SelectedCrew.acLocation[0] == null){
				newIndiName = "Equip_indi_1_2";
				if(Legion.Instance.SelectedCrew.acLocation[1] == null){
					newIndiName = "Equip_indi_1_3";
				}
			}
		}

		GameObject tempObj = Instantiate((GameObject)AssetMgr.Instance.AssetLoad("Prefabs/UI/Tutorial/"+newIndiName+ ".prefab", typeof(GameObject)));
		RectTransform rectTr = tempObj.GetComponent<RectTransform>();

		rectTr.SetParent(objIndicator.transform);
		rectTr.localScale = Vector3.one;
		rectTr.localPosition = Vector3.zero;
		rectTr.sizeDelta = Vector2.zero;
	}

	void EndTutorial(){
		if (bCall)return;
		bCall = true;

		if (ePopupType == TalkPopupType.Notice || ePopupType == TalkPopupType.QuestComplete) {
			LeanTween.alpha (imgTalkFadeBox.GetComponent<RectTransform>(), 0f, fixedFadeTime);
			bBGDriection = true;
			LeanTween.alpha (imgTalkBG.GetComponent<RectTransform>(), 0.0f, fixedFadeTime);
			
			LeanTween.value(gameObject, 1, 0, fixedFadeTime).setOnUpdate((float alpha)=>{cg.alpha = alpha;});
			
			StartCoroutine (EndTutorialCoroutine (fixedFadeTime));

			PopupManager.Instance.bUseNotice = false;
			return;
		}else if (ePopupType == TalkPopupType.Tutorial){
			Legion.Instance.cTutorial.EndTutorial (cInfo);
			PopupManager.Instance.bUseNotice = false;
		}else {
			if(cDirectionInfo.u1DirectionPos == 1)
			{
				/*
				 * 2016. 12. 27 jy
				 * 퀘스트메뉴를 로비로 빼냄
				QuestScene questScene = Scene.GetCurrent() as QuestScene;
				if(questScene != null) questScene.ShowSelectedQuestWithDelay(fixedFadeTime);
				*/
				LobbyScene scene = Scene.GetCurrent() as LobbyScene;
				if(scene != null) scene.ShowSelectedQuestWithDelay(fixedFadeTime);
				//back button error
				PopupManager.Instance.bUseNotice = false;

			}else if(cDirectionInfo.u1DirectionPos == 3){
				GameObject tempQuestMsg = GameObject.Instantiate (AssetMgr.Instance.AssetLoad ("Prefabs/UI/Common/Pref_Quest_Msg.prefab", typeof(GameObject))) as GameObject;
				tempQuestMsg.transform.SetParent(PopupManager.Instance.GetManagerTransform());
				tempQuestMsg.transform.localScale = Vector2.one;
				tempQuestMsg.transform.localPosition = Vector2.zero;
				tempQuestMsg.GetComponent<QuestMsgPopup> ().SetData (QuestInfoMgr.Instance.GetQuestInfo (cDirectionInfo.u2QuestID), true);

				LeanTween.alpha (imgTalkFadeBox.GetComponent<RectTransform>(), 0f, fixedFadeTime);
				bBGDriection = true;
				LeanTween.alpha (imgTalkBG.GetComponent<RectTransform>(), 0.0f, fixedFadeTime);
				
				LeanTween.value(gameObject, 1, 0, fixedFadeTime).setOnUpdate((float alpha)=>{cg.alpha = alpha;});

				StartCoroutine (EndTutorialCoroutine (fixedFadeTime));
				PopupManager.Instance.bUseNotice = false;

				if(rewardPopup != null){
					rewardPopup.ShowMe (2.3f);
					rewardPopup = null;
				}
				return;
			}
		}

		if (cCurTalk != null && cCurTalk.sButtonID == "0") {
			LeanTween.alpha (imgTalkFadeBox.GetComponent<RectTransform>(), 0f, fixedFadeTime);
			bBGDriection = true;
			LeanTween.alpha (imgTalkBG.GetComponent<RectTransform>(), 0.0f, fixedFadeTime);

			LeanTween.value(gameObject, 1, 0, fixedFadeTime).setOnUpdate((float alpha)=>{cg.alpha = alpha;});

			StartCoroutine (EndTutorialCoroutine (fixedFadeTime));
		}else{
            DeleteObject();
			gameObject.SetActive (false);
			imgTalkFadeBox.enabled = false;
		}
	}

	IEnumerator EndTutorialCoroutine(float time){
		yield return new WaitForSeconds(time);
		gameObject.SetActive(false);
		bBGDriection = false;
		imgTalkFadeBox.enabled = false;

        switch(u1TypeVal)
        {
            case 1:
                break;

            case 2:
                break;

            case 3:
                break;

            case 4:
                //((UI_Forge_Smithing_Result_Effect)objVal).CloseStarGradeAni();
                break;
        }

		if (ePopupType == TalkPopupType.QuestComplete) {
			PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("mark_quest_complete"), TextManager.Instance.GetText("popup_quest_complete_shotcut"), GoToQuestScene, null);
		}
	}

	void GoToQuestScene(object[] param){
		Legion.Instance.AwayBattle ();
		FadeEffectMgr.Instance.QuickChangeScene(MENU.QUEST, 0, null);// (MENU.HERO_GUILD, (int)POPUP_HERO_GUILD.QUEST, null);
	}

	public void TouchDown(){
		touchSpd = 4.2f;
	}

	public void TouchUp(){
		touchSpd = 1.0f;
	}

	public void TouchMe(){
		if(ePageType == PageType.Story) return;
		if(bDirection || bBGDriection) return;

        if (BaseBtn != null)
        {
            BaseBtn.onClick.RemoveListener(() => { TouchMe(); });
        }
        else if (BaseToggle != null)
        {
            BaseToggle.onValueChanged.RemoveListener((on) => { TouchMe(on); });
        }

		if (cCurTalk != null) {
			if(cCurTalk.u1TalkCount > 0){
				if(textIndex < textLength-1){
					if(cCurTalk.u2ClassID == 0){
						txtBoxText.text = SpeechCommand(savedSpeech);
					}else{
						acTalkUI[cCurTalk.u1Pos-1]._Speech.text = SpeechCommand(savedSpeech);
					}
					textIndex = textLength-1;
				}else{
					if (talkIndex < cCurTalk.u1TalkCount - 1) {
						talkIndex++;
						textIndex = 0;

						txtBoxText.rectTransform.anchoredPosition = Vector2.zero;
						txtBoxText.text = GetSpeechs();
						float width = txtBoxText.preferredWidth;
						txtBoxText.text = "";
						//txtBoxText.rectTransform.anchoredPosition = new Vector2(-width/2f, 0f);
					} else {
						NextPage ();
					}
				}
			}else{
				NextPage ();
			}
		} else {
			NextPage ();
		}
	}

	public void TouchMe(bool bOn){
		if(bOn == true) TouchMe();
	}

	public void TouchSkip(){
		if(bDirection || bBGDriection) return;
        
		TouchYes ();

//		SkipPopup.SetActive(true);
//		if (ePopupType == TalkPopupType.Quest) {
//			skipTitle.text = TextManager.Instance.GetText("popup_title_talk_skip");
//			skipMsg.text = TextManager.Instance.GetText("popup_desc_talk_skip");
//		} else if (ePopupType == TalkPopupType.Tutorial) {
//			if (isBasicTutorial ()) {
//				skipTitle.text = TextManager.Instance.GetText ("popup_title_tutorial_skip");
//				skipMsg.text = TextManager.Instance.GetText ("대화를 스킵 하시겠습니까?");
//			}else{
//				skipTitle.text = TextManager.Instance.GetText ("popup_title_tutorial_skip");
//				skipMsg.text = TextManager.Instance.GetText ("popup_desc_tutorial_skip");
//			}
//		}
	}

	public void TouchYes(){
//		SkipPopup.SetActive (false);

		if (ePageType == PageType.Story)
		{
			NextPage ();
		}
		else if (ePageType == PageType.Talk)
		{
			if (ePopupType == TalkPopupType.Tutorial && isBasicTutorial()) 
			{
				SkipToButtonPage ();
			}
			else EndTutorial();
		}
		else EndTutorial();
	}

	bool isBasicTutorial(){
		if (Legion.Instance.cTutorial.au1Step [0] != Server.ConstDef.LastTutorialStep) {
			return true;
		}
		if (cInfo.u1TutorialType == 3 || cInfo.u1TutorialType == 5 || cInfo.u1TutorialType == 8 || cInfo.u1TutorialType == 9) return true;

		return false;
	}

	void SkipToButtonPage(){
		for (int i = page; i < cInfo.lstTalks.Count; i++) {
			if (cInfo.lstTalks [i].u1Type == 6) {
				page = i - 1;
				imgTalkBG.enabled = false;
				imgTalkFadeBox.enabled = false;
				bTalkSkip = true;
				NextPage();
				return;
			}else if (cInfo.lstTalks [i].u1Type != 3 && TutorialInfoMgr.Instance.GetTalkInfo (cInfo.lstTalks [i].sName).sButtonID != "0") {
				page = i - 1;
				imgTalkBG.enabled = false;
				imgTalkFadeBox.enabled = TutorialInfoMgr.Instance.GetTalkInfo (cInfo.lstTalks [i].sName).bBGOnOff;
				bTalkSkip = true;
				NextPage();
				return;
			}
		}
		EndTutorial();
	}

	public void TouchCancel(){
		SkipPopup.SetActive(false);
	}

	void FixedUpdate(){
		if (ePageType == PageType.Story)
		{
			txtStoryText.rectTransform.anchoredPosition += Vector2.up*storySpd*touchSpd;

			float per = (storyY - txtStoryText.rectTransform.anchoredPosition.y)/storyY;

			imgStoryBG.localScale = Vector3.one * (1f + per);

			if(txtStoryText.rectTransform.anchoredPosition.y > storyY)
				NextPage();

			SndDelay += Time.fixedDeltaTime;
			if (SndDelay >= 7f) {
				//story sound
				PlayStorysound();
			}
		}
		else if (ePageType == PageType.Talk) 
		{
			if (cCurTalk != null && cCurTalk.lstSpeechs.Count > 0)
			{
				if (textIndex < textLength - 1) 
				{
					if (!bDirection) {
						textIndex++;
						if (textIndex > textLength)
							textIndex = textLength;
					}
					
					if (cCurTalk.u2ClassID == 0) 
					{
						txtBoxText.text =  SpeechCommand(savedSpeech.Remove(textIndex));
					}
					else
					{
						acTalkUI [cCurTalk.u1Pos - 1]._Speech.text = SpeechCommand(savedSpeech.Remove(textIndex));
					}
				}
				else 
				{
					if (cCurTalk.u2ClassID == 0)
						txtBoxText.text = SpeechCommand(savedSpeech);
					else
						acTalkUI [cCurTalk.u1Pos - 1]._Speech.text = SpeechCommand(savedSpeech);
				}
			}
		}
	}

	void PlayStorysound()
	{
		string[] soundlist = new string[5]{"AmbMumbleMan1_nonloop","AmbMumbleMan2_nonloop","AmbMumbleWoman1_nonloop","AmbMumbleWoman2_nonloop",""};
		string soundfilename = soundlist [Random.Range (0, soundlist.Length)];
		if(soundfilename != ""){
			SoundManager.Instance.PlayEff ("Sound/UI/03. Tutorial/"+soundfilename);
		}
		SndDelay = 0;
	}

	private string SpeechCommand(string speech)
	{
		if( startCommand == null || endCommand == null )
			return speech;

		return (startCommand + speech + endCommand);
	}
	// 2016. 7. 13 jy 
	// 커맨드 분리 추후 개선해야 함
	private string SpeechCommandSplit(string speech)
	{
		startCommand = null;
		endCommand = null;

		int startIdx = speech.IndexOf('<');
		if(startIdx == -1)
			return speech;

		int endIdx = speech.IndexOf('>') + 1;
		int commandLength = endIdx - startIdx;

		startCommand = speech.Substring(startIdx, commandLength);
		speech = speech.Remove(startIdx, commandLength);

		startIdx = speech.IndexOf("</"); 
		if(startIdx == -1)
			return speech;
		
		endIdx = speech.IndexOf(">", startIdx) + 1;
		commandLength = endIdx - startIdx;
		endCommand = speech.Substring(startIdx, commandLength);
		speech = speech.Remove(startIdx, commandLength);

		return speech;
	}


	// 16. 7. 13 jy
	// 대화 커맨드 계선 준비
	/*
	struct CommandTalk
	{
		public int nStartIndex;
		public int nEndIndex;
		public string startCommand;
		public string endCommand;
	}
	private Dictionary<int, CommandTalk> commands = new Dictionary<int, CommandTalk>();

	private void CheckTalkCommand()
	{
		commands.Clear();
		int commandIndex = 0;
		while(true)
		{
			int startIdx = savedSpeech.IndexOf('<');
			if(startIdx == -1)
				break;

			CommandTalk command;
			int endIdx = savedSpeech.IndexOf('>') + 1;
			int commandLength = endIdx - startIdx;

			command.nStartIndex = startIdx;
			command.startCommand = savedSpeech.Substring(startIdx, commandLength);
			savedSpeech = savedSpeech.Remove(startIdx, commandLength);

			startIdx = savedSpeech.IndexOf("</"); 
			if(startIdx == -1)
				break;

			command.nEndIndex = startIdx;
			endIdx = savedSpeech.IndexOf(">", startIdx) + 1;
			commandLength = endIdx - startIdx;
			command.endCommand = savedSpeech.Substring(startIdx, commandLength);
			savedSpeech = savedSpeech.Remove(startIdx, commandLength);

			commands.Add(commandIndex++, command);
		}
	}
	*/
}

