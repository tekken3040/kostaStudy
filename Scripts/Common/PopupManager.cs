using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class PopupManager : Singleton<PopupManager> {
	string reviewURL;
	string imgURL;

	protected PopupManager(){}
	public GameObject _objPopupManager;
    public GameObject particleCanvas;
    public bool showLoading = false; // 로딩중에 백버튼을 막기위해 사용      
    private GameObject effectPrefab;

    private ChattingManager cChatMgr;
	private OdinMissionClearPopup cOdinMissionClearPopup;
    public OdinMissionClearPopup OdinMissionClearPopup { get { return cOdinMissionClearPopup; } }

	private void checkCanvas()
	{
        //if(GameObject.Find("Pref_UI_ParticleCanvas") == null)
        //{
        //    particleCanvas = Instantiate((GameObject)AssetMgr.Instance.AssetLoad("Prefabs/UI/Common/Pref_UI_ParticleCanvas", typeof(GameObject)));
        //    particleCanvas.transform.SetAsLastSibling();
        //    particleCanvas.transform.name = "Pref_UI_ParticleCanvas";
        //    particleCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
        //    DontDestroyOnLoad(particleCanvas);
        //}

		if(GameObject.Find("UI_PopupManager") == null)
		{
			//GameObject resCanvas = Resources.Load("Prefabs/UI/Title/UI_PopupManager") as GameObject;
			GameObject resCanvas = (GameObject)Resources.Load("Prefabs/UI/Common/UI_PopupManager", typeof(GameObject));
			_objPopupManager = Instantiate(resCanvas) as GameObject;
			_objPopupManager.transform.SetAsLastSibling();
			_objPopupManager.transform.name = "UI_PopupManager";
			DontDestroyOnLoad(_objPopupManager);

			objLoadingPopup = _objPopupManager.transform.FindChild("LoadingPopup").gameObject;
			objOKPopup = _objPopupManager.transform.FindChild("OKPopup").gameObject;
			objBigOKPopup = _objPopupManager.transform.FindChild("BigOkPopup").gameObject;

			objYesNoPopup =  _objPopupManager.transform.FindChild("YesNoPopup").gameObject;
			objEventPopup =  _objPopupManager.transform.FindChild("EventPopup").gameObject;
			objEffCamera = _objPopupManager.transform.FindChild("Eff_Camera").gameObject;

            objOptionpopup = _objPopupManager.transform.FindChild("OptionPopup").gameObject;
			objDownloadPopup = _objPopupManager.transform.FindChild("DownloadPopup").GetComponent<DownloadPopup>();

            // #CHATTING
            Transform mainChat = _objPopupManager.transform.FindChild("Pref_ChatWindow");
            if(mainChat != null)
            { 
                cChatMgr = mainChat.GetComponent<ChattingManager>();
            }

            cOdinMissionClearPopup = _objPopupManager.transform.FindChild("OdinMissionClearPopup").GetComponent<OdinMissionClearPopup>();
        }
	}

	public void SetReviewURL(string _url){
		reviewURL = _url;
	}

	public void SetImageURL(string _url){
		imgURL = _url;
	}

	public string GetReviewURL(){
		return reviewURL;
	}

	public string GetImageURL(){
		if (string.IsNullOrEmpty (imgURL))
			return TitleScene.rootURL;
		
		return imgURL;
	}

	public void RemoveManager(){
		if (GameObject.Find ("UI_PopupManager") != null) {
			Destroy (_objPopupManager);
		}
		Destroy (this.gameObject);
		Destroy (this);
	}

	public Transform GetManagerTransform(){
		if (_objPopupManager == null) {
			checkCanvas();
		}
		return _objPopupManager.transform;
	}

    public GameObject _touchEffect;
    public GameObject _touchModule;
    public Vector3 mouseToScreenPos;

	List<string> lstUserStep = new List<string>();

	public void AddUserStep(string stepName)
	{
		if (lstUserStep.Count < 15) {
			lstUserStep.Add (stepName);
		} else {
			lstUserStep.RemoveAt(0);
			lstUserStep.Add (stepName);
		}
	}

	string GetUserStepString()
	{
		string result = Server.ServerMgr.id + "/Gold:"+Legion.Instance.Gold+"/Cash:"+Legion.Instance.Cash+"/Key:"+Legion.Instance.Energy;
		for (int i = 0; i < lstUserStep.Count; i++) {
			result += " -> " + lstUserStep [i];
		}

		return result;
	}

    public void Awake()
    {
        _touchEffect = Resources.Load("Prefabs/UI_Effects/UI_Eff_Touch", typeof(GameObject)) as GameObject;
    }
    public void Update()
    {
        //if(_touchModule == null)
        //{
        //    _touchModule = GameObject.Find("EventSystem");
        //    particleCanvas.GetComponent<Canvas>().worldCamera = Camera.main;
        //}

        //if(_touchModule.GetComponent<TouchInputModule>())
		if (Input.GetMouseButtonDown (0)) {   
			//GameObject tempEff = Instantiate(_touchEffect) as GameObject;
			//tempEff.transform.SetParent(particleCanvas.transform);
			//tempEff.transform.localScale = Vector3.one;
			//mouseToScreenPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
			//tempEff.transform.position = new Vector3(mouseToScreenPos.x, mouseToScreenPos.y, 50f);
			//tempEff.gameObject.layer = LayerMask.NameToLayer("UI");
			//StartCoroutine(TouchAni());
		} 
		else if (Input.GetMouseButtonUp (0)) 
		{
//			if (EventSystem.current != null) {
//				if (EventSystem.current.currentSelectedGameObject == null) {
//					DebugMgr.LogError ("currentSelectedGameObject is Null");
//				} else {
//					AddUserStep (EventSystem.current.currentSelectedGameObject.name);
//
//					//DebugMgr.LogError (GetUserStepString ());
//				}
//			}
		}
        else if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(Legion.Instance.cTutorial.bIng || showLoading)
                return;

			if (bUseNotice) {
				TutorialPopup.TouchYes ();
				return;
			}
            
            //팝업이 있을 경우
            if(dicPopup.Count > 0)
            {       
				int lastIndex = dicPopup.Count - 1;
				if(dicPopup[lastIndex].popupObject == null)
                {             
					dicPopup.RemoveAt(lastIndex);
					DebugMgr.LogError("Remove");
                    return;
                }
                
				CloseMethod closeMethod = dicPopup[lastIndex].popupCallback;
                
				if(closeMethod == null)
                {
					dicPopup.RemoveAt(lastIndex);
                    DebugMgr.LogError("Close Delegate None");
                    return;   
                }
				//DebugMgr.Log("Close Method : " +  dicPopup.Keys.Last<GameObject>().transform.name);
                closeMethod();
            }
            else
            {
                BattleUIMgr battleUI = FindObjectOfType<BattleUIMgr>() as BattleUIMgr;
                
                //인게임
                if(battleUI != null)
                {
                    battleUI.OnClickPause();                    
                }
                //아웃게임
				else if(Scene.GetCurrent() as LobbyScene != null)
                {                    
                    PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("popup_title_application_quit"), TextManager.Instance.GetText("popup_desc_application_quit"), (x) => Application.Quit(), null);                    
                }
				else if(Scene.GetCurrent() as TitleScene != null)
				{                    
					PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("popup_title_application_quit"), TextManager.Instance.GetText("popup_desc_application_quit"), (x) => Application.Quit(), null);                    
				}
            }
        }
    }
    IEnumerator TouchAni()
    {
        if(_objPopupManager == null)
            yield break;
        
        if(effectPrefab == null)
            effectPrefab = Resources.Load("Prefabs/UI_Effects/Pref_UI_TouchEff", typeof(GameObject)) as GameObject;
            
        GameObject tempEff = Instantiate(effectPrefab) as GameObject;
        tempEff.transform.SetParent(_objPopupManager.transform);
        tempEff.transform.localScale = new Vector3(0, 0, 1f);
        tempEff.transform.position = Input.mousePosition;
        tempEff.gameObject.layer = LayerMask.NameToLayer("UI");
        yield return new WaitForSeconds(0.1f);
        GameObject tempEff2 = Instantiate(tempEff) as GameObject;
        tempEff2.transform.SetParent(_objPopupManager.transform);
        tempEff2.transform.localScale = new Vector3(0, 0, 1f);
        tempEff2.transform.position = tempEff.transform.position;
        tempEff2.gameObject.layer = LayerMask.NameToLayer("UI");
    }
    
    public delegate void CloseMethod();

	class PopupClass
	{
		public GameObject popupObject;
		public CloseMethod popupCallback;

		public PopupClass(GameObject obj, CloseMethod cMet){
			popupObject = obj;
			popupCallback = cMet;
		}
	}

	List<PopupClass> dicPopup = new List<PopupClass>();
    public void AddPopup(GameObject popupObject, CloseMethod closeMethod)
    {
		if(dicPopup.FindIndex(cs => cs.popupObject == popupObject) < 0)
        {
			PopupClass temp = new PopupClass (popupObject, closeMethod);
			dicPopup.Add(temp);
        }
    }
    
    public void RemovePopup(GameObject popupObject)
    {
		int idx = dicPopup.FindIndex (cs => cs.popupObject == popupObject);
		if(idx >= 0)
        {
			dicPopup.RemoveAt (idx);
        }
    }
    
    public void ClearPopup()
    {
        dicPopup.Clear();
    }
    
//	public GameObject _objPopupManager;
//	
//	private void checkFadeObj()
//	{
//		if(_Canvas.transform.FindChild("Fade") == null)
//		{
//			GameObject resFade = Resources.Load("Effects/UI/Fade") as GameObject;
//			_objFade = Instantiate(resFade) as GameObject;
//			_objFade.transform.parent = _Canvas.transform;
//			_objFade.GetComponent<RectTransform>().localPosition = Vector2.zero;
//			_objFade.GetComponent<RectTransform>().localScale = Vector2.one;
//			_objFade.transform.name = "Fade";
//			DontDestroyOnLoad(_objFade);
//		}
//	}

	public delegate void OnClickEvent(object[] param);

	TalkPopup TutorialPopup;
	QuestCompleteMsg QuestMsg;
	public GameObject objTutorialEnd;
	public GameObject objLoadingPopup;
	public GameObject objOKPopup;
	public GameObject objBigOKPopup;
	public GameObject objYesNoPopup;
	public GameObject objEventPopup;
    public GameObject objOptionpopup;
	public DownloadPopup objDownloadPopup;
	public GameObject ErrorPopup;
    public ItemInfoPopup itemInfoPopup;
	public GameObject objEffCamera;
	private GameObject[] popupList = new GameObject[10];
	public bool bUseNotice = false;
	public void registerPopup(GameObject currentPopup, int zOrder)
	{
		//DebugMgr.Log("registerPopup");
		if(popupList[zOrder] != null && popupList[zOrder] != currentPopup) {
			GameObject prevPopup = popupList[zOrder];
			//DebugMgr.Log("PrevPopupName : " + prevPopup.transform.name);
			prevPopup.GetComponent<Popup>().Close();
		}
		popupList[zOrder] = currentPopup;
	}

	public void SetDownloadPopup(string downloadSize){
		GetManagerTransform();
		objDownloadPopup.gameObject.SetActive (true);
		objDownloadPopup.compBtn.SetActive (false);
		objDownloadPopup.noBtn.SetActive (AssetMgr.Instance.bCanCancel);
		objDownloadPopup.okBtn.SetActive (true);
		objDownloadPopup.Show (TextManager.Instance.GetText("popup_btn_data_download"), downloadSize + TextManager.Instance.GetText("popup_add_download"), StartDownload, null, CancelDownload, null);
	}

	void StartDownload(object[] param){
		objDownloadPopup.popupObject.SetActive (false);
		objDownloadPopup.progressObject.SetActive (true);
		AssetMgr.Instance.StartFileDownload ();
	}

	public void UpdateDownload(float size, string per, string prog){
		objDownloadPopup.UpdateInfo (size, per, prog);
	}

	public void AddDownloadCompleteEvent(UnityEngine.Events.UnityAction evt){
		objDownloadPopup.SetCompleteEvent (evt);
	}

	public void SetDownloadComplete(){
		objDownloadPopup.popupObject.SetActive (true);
		objDownloadPopup.progressObject.SetActive (false);
		objDownloadPopup.compBtn.SetActive (true);
		objDownloadPopup.noBtn.SetActive (false);
		objDownloadPopup.okBtn.SetActive (false);
		objDownloadPopup.Show (TextManager.Instance.GetText("popup_btn_data_download"), TextManager.Instance.GetText("popup_download_done"), TextManager.Instance.GetText("btn_ok"), CancelDownload, null);
	}

	void CancelDownload(object[] param){
		AssetMgr.Instance.StopDownload ();
		objDownloadPopup.gameObject.SetActive (false);
	}

	void CreateTutorPopup(){
		GameObject tempObj = (GameObject)Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Tutorial/TalkMass.prefab", typeof(GameObject)));

		tempObj.transform.SetParent (_objPopupManager.transform);
		tempObj.transform.localScale = Vector3.one;
		tempObj.transform.localPosition = Vector3.zero;
		tempObj.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
		tempObj.transform.SetSiblingIndex (2);
		TutorialPopup = tempObj.GetComponent<TalkPopup>();
	}

	public void SetTutorialPopup(TutorialInfo info){
		GetManagerTransform();

		if (TutorialPopup == null) CreateTutorPopup ();

		TutorialPopup.gameObject.SetActive(true);
		TutorialPopup.SetPopup (info);
	}

	public bool SetNoticePopup(MENU menu, byte u1Type = 1, ushort u2Star = 0, object obj = null){
		if (Legion.Instance.cTutorial.au1Step [0] == Server.ConstDef.LastTutorialStep && !Legion.Instance.cTutorial.bIng) {
			GetManagerTransform ();

			if (TutorialPopup == null) CreateTutorPopup ();

			TutorialPopup.gameObject.SetActive (true);
			TutorialPopup.SetPopup (menu, u1Type, u2Star, obj);
			return true;
		}

		return false;
	}

	public bool SetQuestCompletePopup(byte _type){
		if (Legion.Instance.cTutorial.au1Step [0] == Server.ConstDef.LastTutorialStep && !Legion.Instance.cTutorial.bIng) {
			GetManagerTransform ();

			if (TutorialPopup == null) CreateTutorPopup ();

			TutorialPopup.gameObject.SetActive (true);
			TutorialPopup.SetPopup (_type);

			bUseNotice = true;
			return true;
		}

		return false;
	}

	public void SetQuestDirectionPopup(QuestDirection info, GameObject rewardPop){
		GetManagerTransform();

		if (TutorialPopup == null) CreateTutorPopup ();

		TutorialPopup.gameObject.SetActive(true);
		TutorialPopup.SetPopup (info, rewardPop);

		bUseNotice = true;
	}

	public void TutorialNextPage()
	{
		TutorialPopup.TouchMe ();
	}

	private void ShowPopup()
	{
		checkCanvas();
	}

	public void ShowLoadingPopup(int type)
	{
		checkCanvas();
		objLoadingPopup.SetActive(true);

		for(int i=0; i<objLoadingPopup.transform.childCount; i++)
		{
			if(type != (i+1))
			{
				objLoadingPopup.transform.GetChild(i).gameObject.SetActive(false);
			}
		}

		objLoadingPopup.transform.FindChild("AnimType_" + type).gameObject.SetActive(true);
		objLoadingPopup.GetComponent<Animator>().Play("LoadingType"+type);

		//objLoadingPopup.transform.FindChild("MSG").gameObject.SetActive(false);
        objLoadingPopup.transform.FindChild("MSG").gameObject.SetActive(true);
		objLoadingPopup.transform.FindChild("MSG").GetComponent<Text>().text = TextManager.Instance.GetText("data_loading_desc");
        
        showLoading = true;
	}

	public void ShowLoadingPopup(int type, string msg)
	{
		checkCanvas();
		objLoadingPopup.SetActive(true);

		for(int i=0; i<objLoadingPopup.transform.childCount; i++)
		{
			if(type != (i+1))
			{
				objLoadingPopup.transform.GetChild(i).gameObject.SetActive(false);
			}
		}

		objLoadingPopup.transform.FindChild("AnimType_" + type).gameObject.SetActive(true);
		objLoadingPopup.GetComponent<Animator>().Play("LoadingType"+type);

		objLoadingPopup.transform.FindChild("MSG").gameObject.SetActive(true);
		objLoadingPopup.transform.FindChild("MSG").GetComponent<Text>().text = msg;
        
        showLoading = true;
	}

	public void CloseLoadingPopup()
	{
		checkCanvas();
		objLoadingPopup.SetActive(false);
        
        showLoading = false;
	}

	public void ShowErrorPopup(string title, string content)
	{
		checkCanvas();
	}

	public void ShowOKPopup(string title, string msg, OnClickEvent okEvent)
	{
		checkCanvas();
		objOKPopup.SetActive(true);
		object[] param = new object[0];
		objOKPopup.GetComponent<OKPopup>().Init(title, msg, okEvent, param);
        AddPopup(objOKPopup, objOKPopup.GetComponent<OKPopup>().OnClickOK);
	}

	public void ShowBigOKPopup(string title, string msg, OnClickEvent okEvent, object[] param = null)
	{
		checkCanvas();
		objBigOKPopup.SetActive(true);
		objBigOKPopup.GetComponent<OKPopup>().Init(title, msg, okEvent, param);
		AddPopup(objBigOKPopup, objBigOKPopup.GetComponent<OKPopup>().OnClickOK);
	}

	public void ShowChargePopup(Byte u1GoodsType)
	{
        string msg = string.Format("{0}"+TextManager.Instance.GetText("popup_desc_nocost"), Legion.Instance.GetConsumeString(u1GoodsType));
        // 우정 포인트 부족시 팝업만 띄움
        if (u1GoodsType == (Byte)GoodsType.FRIENDSHIP_POINT)
        {
            ShowOKPopup(TextManager.Instance.GetText("popup_title_nocost"), msg, null);
            return;
        }

        checkCanvas();
		objYesNoPopup.SetActive(true);

		objYesNoPopup.GetComponent<YesNoPopup>().Show(TextManager.Instance.GetText("popup_title_nocost"), msg, TextManager.Instance.GetText("btn_popup_shotcut_charge"), GoToGoodsCharge, new object[1]{u1GoodsType});
		AddPopup(objYesNoPopup, objYesNoPopup.GetComponent<YesNoPopup>().OnClickNo);
	}

    public void ShowOptionPopup()
    {
        checkCanvas();
        objOptionpopup.SetActive(true);
        AddPopup(objOptionpopup, objOptionpopup.GetComponent<OptionPopup>().OnClickClose);
    }

//	public void ShowOKPopup(string title, string content)
//	{
//		checkCanvas();
//		objOKPopup.GetComponent<OKPopup>().Show(title, content);
//	}

	public void ShowYesNoPopup(string title, string content, OnClickEvent yesEvent, object[] yesEventParam)
	{
		checkCanvas();
		objYesNoPopup.GetComponent<YesNoPopup>().Show(title, content, yesEvent, yesEventParam);
        AddPopup(objYesNoPopup, objYesNoPopup.GetComponent<YesNoPopup>().OnClickNo);
	}

	public void ShowYesNoPopup(string title, string content, string btnText, OnClickEvent yesEvent, object[] yesEventParam)
	{
		checkCanvas();
		objYesNoPopup.GetComponent<YesNoPopup>().Show(title, content, btnText, yesEvent, yesEventParam);
        AddPopup(objYesNoPopup, objYesNoPopup.GetComponent<YesNoPopup>().OnClickNo);
	}
	
	public void ShowYesNoPopup(string title, string content, OnClickEvent yesEvent, object[] yesEventParam, OnClickEvent noEvent, object[] noEventParam)
	{
		checkCanvas();
		objYesNoPopup.GetComponent<YesNoPopup>().Show(title, content, yesEvent, yesEventParam, noEvent, noEventParam);
        AddPopup(objYesNoPopup, objYesNoPopup.GetComponent<YesNoPopup>().OnClickNo);
	}

	public void ShowYesNoPopup(string title, string content, string btnText, OnClickEvent yesEvent, object[] yesEventParam, OnClickEvent noEvent, object[] noEventParam)
	{
		checkCanvas();
		objYesNoPopup.GetComponent<YesNoPopup>().Show(title, content, btnText, yesEvent, yesEventParam, noEvent, noEventParam);
		AddPopup(objYesNoPopup, objYesNoPopup.GetComponent<YesNoPopup>().OnClickNo);
	}

	public void ShowEventPopup(string url, PopupManager.OnClickEvent clickEvent, object[] clickEventParam)
	{
		checkCanvas();
		objEventPopup.SetActive(true);
		objEventPopup.GetComponent<EventPopup>().Init(url, clickEvent, clickEventParam);
        AddPopup(objEventPopup, objEventPopup.GetComponent<EventPopup>().OnClickClose);
	}

	public void GoToGoodsCharge(object[] param)
	{
		
		Byte gType = Convert.ToByte (param [0]);
		int index = -1;
		switch((GoodsType)gType){
		case GoodsType.GOLD:
			index = 0;
			break;
		case GoodsType.CASH:
			index = 1;
			break;
		case GoodsType.KEY:
			index = 2;
			break;
		}

		if (index < 0)
			return;

        int popupCnt = dicPopup.Count;

        for(int i=0; i<popupCnt; i++)
        {
			if (i >= dicPopup.Count)
				break;
			
			GameObject obj = dicPopup [i].popupObject;
			if (obj != null) {
				if (obj.name == "ShopInfoWindow") {
					obj.SetActive (false);
					dicPopup.RemoveAt (i);
					i--;
					break;
				} else if (obj.name == "Pref_UI_Panel_Shop_Gacha_Result_Effect") {
					Destroy (obj);
					dicPopup.RemoveAt (i);
					i--;
					break;
				} else if (obj.name == "GachaEquipResult") {
					Destroy (obj);
					dicPopup.RemoveAt (i);
					i--;
					break;
				}
			} else {
				dicPopup.RemoveAt (i);
				i--;
			}
        }

		StartCoroutine(OpenShop(index));
	}

	private IEnumerator OpenShop(int index)
	{
		if (Scene.GetCurrent () != null) {
			yield return Scene.GetCurrent ().StartCoroutine (Scene.GetCurrent ().ShowShop (true, index));
		} else {
			switch(index){
			case 0:
				FadeEffectMgr.Instance.QuickChangeScene (MENU.MAIN, (int)POPUP_MAIN.GOLD);
				break;
			case 1:
				FadeEffectMgr.Instance.QuickChangeScene (MENU.MAIN, (int)POPUP_MAIN.CASH);
				break;
			case 2:
				FadeEffectMgr.Instance.QuickChangeScene (MENU.MAIN, (int)POPUP_MAIN.ENERGY);
				break;
			}

		}
	}
    
    public void ShowItemInfo(byte itemType, ushort itemID, Vector3 position, float scale)
    {
        if(showLoading)
            return;
            
        if(itemInfoPopup == null)
        {
            GameObject temp = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Common/ItemInfoPopup.prefab", typeof(GameObject))) as GameObject;
            temp.transform.SetParent(_objPopupManager.transform);
            temp.transform.localScale = Vector3.one;
            temp.transform.localPosition = Vector3.zero;
            temp.transform.SetSiblingIndex(0);
			temp.GetComponent<RectTransform> ().anchoredPosition = Vector2.zero;
			temp.GetComponent<RectTransform> ().sizeDelta = Vector2.zero;
            itemInfoPopup = temp.GetComponent<ItemInfoPopup>();
			itemInfoPopup.gameObject.SetActive (false);
        }
		if (itemType == (byte)GoodsType.CASH ||
		    itemType == (byte)GoodsType.GOLD ||
		    itemType == (byte)GoodsType.KEY ||
			itemType == (byte)GoodsType.LEAGUE_KEY ||
			itemType == (byte)GoodsType.FRIENDSHIP_POINT ||
			itemType == (byte)GoodsType.TRAINING_ROOM ||
			itemType == (byte)GoodsType.EQUIP_TRAINING||
            itemType == (byte)GoodsType.EQUIP_GOODS ||
			itemType == (byte)GoodsType.EQUIP_COUPON ||
			itemType == (byte)GoodsType.MATERIAL_COUPON ||
            itemType == (byte)GoodsType.RANDOM_REWARD)
        {
			itemInfoPopup.gameObject.SetActive (true);
			itemInfoPopup.SetInfo (itemType, itemID, position, scale);
		} 
        else 
        {
			// 16.6.14 jy
			// itemID가 0 일때 최초 클릭시 팝업창이 오픈되어 예외처리 함
			if(itemID == 0) return;
			
			switch (ItemInfoMgr.Instance.GetItemType (itemID)) {
			case ItemInfo.ITEM_ORDER.CONSUMABLE:					
			case ItemInfo.ITEM_ORDER.MATERIAL:
			case ItemInfo.ITEM_ORDER.EQUIPMENT:
			case ItemInfo.ITEM_ORDER.DESIGN:
			case ItemInfo.ITEM_ORDER.EVENT_GOODS:
				itemInfoPopup.gameObject.SetActive (true);
				itemInfoPopup.SetInfo (itemType, itemID, position, scale);
				break;
			}     
		}
    }

	public void HideItemInfo()
	{
		if (itemInfoPopup != null) {
			itemInfoPopup.gameObject.SetActive (false);
		}
	}

	public void ShowQuestComp(int type, ushort id){
		if (QuestMsg == null) {
			GameObject tempObj = (GameObject)Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Common/QuestComplete.prefab", typeof(GameObject)));
			RectTransform rectTr = tempObj.GetComponent<RectTransform>();
			rectTr.SetParent (_objPopupManager.transform);
			rectTr.localScale = Vector3.one;
			rectTr.localPosition = Vector3.zero;
			rectTr.sizeDelta = Vector2.zero;
			rectTr.SetSiblingIndex (4);
			QuestMsg = tempObj.GetComponent<QuestCompleteMsg>();
		}

		QuestMsg.AddList(type, id);
	}

	public void ShowTutorialEnd(byte stepNum)
	{
		checkCanvas();
		objEffCamera.SetActive(true);
		if (objTutorialEnd == null) {
			objTutorialEnd = (GameObject)Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Tutorial/TutorialEnd.prefab", typeof(GameObject)));

			objTutorialEnd.transform.SetParent (_objPopupManager.transform);
			objTutorialEnd.transform.localScale = Vector3.one;
			objTutorialEnd.transform.localPosition = Vector3.zero;
		}

		objTutorialEnd.SetActive(true);
		objTutorialEnd.transform.GetChild(0).gameObject.SetActive(true);
        switch(stepNum)
        {
		case 1:
            objTutorialEnd.transform.GetChild(1).gameObject.SetActive(true);
            LeanTween.alpha(objTutorialEnd.transform.GetChild(1).GetComponent<RectTransform>(), 1f, 0.1f);
            break;

		case 2: case 3:  case 4: case 5: case 6: case 7: case 8: case 9:
			objTutorialEnd.transform.GetChild(stepNum).gameObject.SetActive(true);
			LeanTween.alpha(objTutorialEnd.transform.GetChild(stepNum).GetComponent<RectTransform>(), 1f, 0.1f);
			LeanTween.alpha(objTutorialEnd.transform.GetChild(stepNum).GetChild(0).GetComponent<RectTransform>(), 1f, 0.1f);
            break;
        }
		//LeanTween.alpha(objTutorialEnd.GetComponent<RectTransform>(), 1f, 0.1f);
		StartCoroutine( autoOffTutorialEnd(stepNum) );
	}

	IEnumerator autoOffTutorialEnd(byte stepNum)
	{
		yield return new WaitForSeconds(1.3f);
		//LeanTween.alpha(objTutorialEnd.GetComponent<RectTransform>(), 0f, 0.5f);
        switch(stepNum)
        {
		case 1: 
            LeanTween.alpha(objTutorialEnd.transform.GetChild(1).GetComponent<RectTransform>(), 0f, 0.5f);
            break;

		case 2: case 3:  case 4: case 5: case 6: case 7: case 8: case 9:
			LeanTween.alpha(objTutorialEnd.transform.GetChild(stepNum).GetComponent<RectTransform>(), 0f, 0.5f);
			LeanTween.alpha(objTutorialEnd.transform.GetChild(stepNum).GetChild(0).GetComponent<RectTransform>(), 0f, 0.5f);
            break;
        }
		yield return new WaitForSeconds(0.6f);
		objTutorialEnd.SetActive(false);
		objTutorialEnd.transform.GetChild(0).gameObject.SetActive(false);
        switch(stepNum)
        {
            case 0: case 1:
                objTutorialEnd.transform.GetChild(1).gameObject.SetActive(false);
                break;
            case 2:
                objTutorialEnd.transform.GetChild(2).gameObject.SetActive(false);
                break;
            case 3:
                objTutorialEnd.transform.GetChild(3).gameObject.SetActive(false);
                break;
            case 4:
                objTutorialEnd.transform.GetChild(4).gameObject.SetActive(false);
                break;
        }
		objEffCamera.SetActive(false);
	}

    public IEnumerator EndLastTutorialPopup()
    {
        //        ShowTutorialEnd(1);
        //        yield return new WaitForSeconds(1f);

        VipInfo vipInfo = LegionInfoMgr.Instance.GetVipInfo(1);
        Legion.Instance.AddGoods(new Goods((Byte)GoodsType.CHARACTER_AVAILABLE, vipInfo.u2OpenClassID, 1));

        // OkPopup이 비활성화 될때까지 대기한다
        while (true)
        {
            if (objOKPopup.activeSelf == false)
                break;

            yield return null;
        }

		Legion.Instance.cTutorial.CheckTutorial(MENU.MAIN);

//        LobbyScene lobbyScene = Scene.GetCurrent() as LobbyScene;
//        if (EventInfoMgr.Instance.u1EventCount != 0)
//        {
//            Legion.Instance.AddLoginPopupStep(Legion.LoginPopupStep.LOGIN_REWARED);
//            if(lobbyScene != null)
//                lobbyScene.CheckLoginPopupStep();
//        }
    }

    // 쿠폰 버튼 비/활성화
    public void SetCouponBtnActive(string value)
    {
        if(string.IsNullOrEmpty(value))
        {
            return;
        }

        checkCanvas();
        if(objOptionpopup == null)
        {
            return;
        }
        objOptionpopup.GetComponent<OptionPopup>().SetCouponBtnActive(value == "0" ? true : false);
    }

    public bool IsOpenOdinMissionClearPopup()
    {
        if (cOdinMissionClearPopup == null)
        {
            checkCanvas();
        }

        return cOdinMissionClearPopup.gameObject.activeSelf;
    }

    protected bool IsChatting()
    {
        if(cChatMgr == null)
        {
            checkCanvas();
        }

        return cChatMgr != null;
    }
    public void OpenChattingType(ChattingManager.ChattingTabType tabType)
    {
        if (!IsChatting())
        {
            return;
        }

        cChatMgr.OpenMainChattingWindow(tabType);
    }
    // 채팅 가능 여부
    public bool IsChattingActive()
    {
		//기능막음 2017.08.08 jc
		return false;

        if(cChatMgr != null)
        {
            return cChatMgr.ChattingActive;
        }

        return IsChatting();
    }
    // 서브 채팅창 셋팅
    public void SetSubChtting(SubChatting cSubChatting, ChattingManager.ChattingTabType tabType = ChattingManager.ChattingTabType.NormalTab)
    {
        cChatMgr.SetSubChattingUI(cSubChatting, tabType);
    }

    // 채팅 연결 끊기
    public void ChattingDisconnect()
    {
        if (cChatMgr == null)
        {
            return;
        }
        cChatMgr.CloseMainChattingWindow();
        cChatMgr.OnDisconnect();
    }
    public void GuildChatConnect()
    {
        if (cChatMgr == null)
        {
            return;
        }
        cChatMgr.RequestGuildChannel();
    }
    // 길드 채팅 연결 끊기
    public void GuildChatDisconnect()
    {
        if(cChatMgr == null)
        {
            return;
        }
        cChatMgr.GuildChannelDisconnected();
    }
}
