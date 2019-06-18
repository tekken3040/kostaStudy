using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityStandardAssets.ImageEffects;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine.SceneManagement;
using IgaworksUnityAOS;

public class OptionPopup : MonoBehaviour
{
    GameObject _mapCamera;
    public GameObject _sliderBGM;
    public GameObject _sliderEFFECT;
	public GameObject[] _toggleGraphic;
	public GameObject[] _togglePush;	//0.공지(상점, 암시장, 시간보상), 1.게임(파견, 트레이닝), 2.열쇠(열쇠 충전 완료 / 시간 별 열쇠 보상)
    public GameObject _toggleHelmet;
    public GameObject _toggleBattle;
    public GameObject _couponInput;
    public GameObject objChangeLanguagePopup;
    public GameObject objLanguageGroup;
    public Text strLanguageLabel;
    public Text strCoupon;
    public Text strError;
	public Text strClientVersion;
	public Text _txtUserNumver;
	public Button _btnDownload;

	public Button _btnFacebook;
	public Image _imgFacebook;
	public Image _imgFacebookCheck;
	public Button _btnGoogle;
	public Image _imgGoogle;
	public Image _imgGoogleCheck;
    public Button _btnNaver;
    public Image _imgNaver;
    public Image _imgNaverCheck;

    public GameObject _objCoupon;

    bool bEnable = false;
    bool bLanguageChanged = false;
    TextManager.LANGUAGE_TYPE eLanguageType;    
    //int _nClickPushToggleIdx = 0;
    private Byte u1PushSetting;

    public void Awake()
    {
        _sliderBGM.GetComponent<Slider>().value = ObscuredPrefs.GetFloat("VolumeBGM", 1f);
        _sliderEFFECT.GetComponent<Slider>().value = ObscuredPrefs.GetFloat("VolumeEFFECT", 1f);
        //this.gameObject.SetActive(false);
    }

	void Start()
	{
		#if UNITY_ANDROID
		IgaworksUnityPluginAOS.OnOpenNanooFanPage = mOnOpenNanooFanPage;
		#endif
		if (Application.systemLanguage != SystemLanguage.Korean) {
			_btnNaver.gameObject.SetActive (false);
		}
	}

    public void OnEnable()
    {
		_imgFacebookCheck.enabled = false;
		_imgGoogleCheck.enabled = false;
        _imgNaverCheck.enabled = false;
		//_imgGoogle.color = Color.white;
		//_imgFacebook.color = Color.white;
        //_imgNaver.color = Color.white;
        _imgFacebookCheck.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_05_renew.login_disable_inoption");
        _imgGoogleCheck.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_05_renew.login_disable_inoption");
        _imgNaverCheck.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_05_renew.login_disable_inoption");
        _imgFacebookCheck.SetNativeSize();
        _imgGoogleCheck.SetNativeSize();
        _imgNaverCheck.SetNativeSize();

		if (PlayerPrefs.GetInt ("SavedPublish", 0) == 2) {
			_btnFacebook.interactable = false;
			_btnGoogle.interactable = false;
            _btnNaver.interactable = false;
            //_imgGoogle.color = Color.gray;
            //_imgNaver.color = Color.gray;
            _imgFacebookCheck.enabled = true;
            _imgFacebookCheck.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_05_renew.login_use_inoption");
            _imgFacebookCheck.SetNativeSize();
		} else if (PlayerPrefs.GetInt ("SavedPublish", 0) == 3) {
			_btnFacebook.interactable = false;
			_btnGoogle.interactable = false;
            _btnNaver.interactable = false;
            //_imgFacebook.color = Color.gray;
            //_imgNaver.color = Color.gray;
            _imgGoogleCheck.enabled = true;
            _imgGoogleCheck.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_05_renew.login_use_inoption");
            _imgGoogleCheck.SetNativeSize();
		} else if (PlayerPrefs.GetInt("SavedPublish", 0) == 4) {
            _btnFacebook.interactable = false;
            _btnGoogle.interactable = false;
            _btnNaver.interactable = false;
            //_imgGoogle.color = Color.gray;
            //_imgFacebook.color = Color.gray;
            _imgNaverCheck.enabled = true;
            _imgNaverCheck.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_05_renew.login_use_inoption");
            _imgNaverCheck.SetNativeSize();
        }

        bEnable = true;
        strError.text = "";
        _mapCamera = GameObject.Find("MapCamera");
        eLanguageType = TextManager.Instance.eLanguage;
        u1PushSetting = Legion.Instance.pushSetting;
        // push 버튼 셋팅
        for (int i = 0; i < Legion.MAX_PUSH_EVENT_ID; ++i)
		{
			if(ObscuredPrefs.GetBool("pushActive" + i))
			{
				_togglePush[i].GetComponent<Toggle>().isOn = true;
				_togglePush[i].transform.GetChild(1).GetComponent<Text>().text = TextManager.Instance.GetText("popup_btn_option_on");
			}
			else
			{
				_togglePush[i].GetComponent<Toggle>().isOn = false;
				_togglePush[i].transform.GetChild(1).GetComponent<Text>().text = TextManager.Instance.GetText("popup_btn_option_off");
			}
		}
        
        if(ObscuredPrefs.GetBool("HelmetToggle", true))
        {
            _toggleHelmet.GetComponent<Toggle>().isOn = true;
            _toggleHelmet.transform.GetChild(1).GetComponent<Text>().text = TextManager.Instance.GetText("popup_btn_option_on");
        }
        else
        {
            _toggleHelmet.GetComponent<Toggle>().isOn = false;
            _toggleHelmet.transform.GetChild(1).GetComponent<Text>().text = TextManager.Instance.GetText("popup_btn_option_off");
        }
        if(ObscuredPrefs.GetBool("BattlePushToggle", true))
        {
            _toggleBattle.GetComponent<Toggle>().isOn = true;
            _toggleBattle.transform.GetChild(1).GetComponent<Text>().text = TextManager.Instance.GetText("popup_btn_option_on");
        }
        else
        {
            _toggleBattle.GetComponent<Toggle>().isOn = false;
            _toggleBattle.transform.GetChild(1).GetComponent<Text>().text = TextManager.Instance.GetText("popup_btn_option_off");
        }

		_toggleGraphic[ObscuredPrefs.GetInt("Graphic", 1)].GetComponent<Toggle>().isOn = true;

        objLanguageGroup.transform.GetChild(((int)eLanguageType) - 1).GetComponent<Toggle>().isOn = true;
        //ChangeLanguage(((int)eLanguageType) - 1);
		strClientVersion.text = Server.ServerMgr.clientVersionString;
		_txtUserNumver.text = ObscuredPrefs.GetString("guestID", "");

		_btnDownload.interactable = AssetMgr.Instance.CheckDownloadedAll ();
    }

	public void OnClickDownload()
	{
		AssetMgr.Instance.CheckDownloadAll ();
		OnClickClose ();
	}

    public void OnClickClose()
    {
        OnClickCloseCoupon();
        ObscuredPrefs.SetFloat("VolumeBGM", _sliderBGM.GetComponent<Slider>().value);

        float effectValue = _sliderEFFECT.GetComponent<Slider>().value;
        ObscuredPrefs.SetFloat("VolumeEFFECT", effectValue);
        if (effectValue > 0)
            SoundManager.Instance.SetMuteEff(false);

        LobbyScene scene = Scene.GetCurrent() as LobbyScene;
        if(scene != null)
		{
        	scene.OnCheckHelmet();
        }
        if (eLanguageType != TextManager.Instance.eLanguage)
        {
            PopupManager.Instance.ShowLoadingPopup(1);
            Server.ServerMgr.Instance.OptionSet(Legion.Instance.pushSetting, (Byte)TextManager.Instance.eLanguage, LobbyScecneLoad);
        }
        else if(u1PushSetting != Legion.Instance.pushSetting)
        {
            PopupManager.Instance.ShowLoadingPopup(1);
            Server.ServerMgr.Instance.OptionSet(Legion.Instance.pushSetting, (Byte)TextManager.Instance.eLanguage, OptionPushSet);
        }
        else
        {
            if (scene != null)
            {
                scene.CheckOptions();
            }
            bLanguageChanged = false;
            this.gameObject.SetActive(false);
        }

        PopupManager.Instance.RemovePopup(gameObject);
    }

    private void OptionPushSet(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if (err != Server.ERROR_ID.NONE)
        {
            TextManager.Instance.eLanguage = eLanguageType;
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.OPTION_SET, err), Server.ServerMgr.Instance.CallClear);
            return;
        }
        else if (err == Server.ERROR_ID.NONE)
        {
            bLanguageChanged = false;
            this.gameObject.SetActive(false);
        }
    }

    private void LobbyScecneLoad(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if (err != Server.ERROR_ID.NONE)
        {
            TextManager.Instance.eLanguage = eLanguageType;
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.OPTION_SET, err), Server.ServerMgr.Instance.CallClear);
            return;
        }
        else if (err == Server.ERROR_ID.NONE)
        {
            AssetMgr.Instance.SceneLoad("LobbyScene", false);
            bLanguageChanged = false;
            this.gameObject.SetActive(false);
        }
    }

    public void ChangeLanguageAccept(object[] parma)
    {
        OnClickCloseCoupon();
        ObscuredPrefs.SetFloat("VolumeBGM", _sliderBGM.GetComponent<Slider>().value);
        ObscuredPrefs.SetFloat("VolumeEFFECT", _sliderEFFECT.GetComponent<Slider>().value);
        LobbyScene scene = Scene.GetCurrent() as LobbyScene;
		if(scene != null)
		{
        	scene.OnCheckHelmet();
	        PopupManager.Instance.RemovePopup(gameObject);
	        //Legion.Instance.eGameStyle = GameStyle.ReloadLobby;
	        AssetMgr.Instance.SceneLoad("LobbyScene", false);
		}
        this.gameObject.SetActive(false);
    }

    public void OnSlideBGM()
    {
        SoundManager.Instance.GetcBGMPlayer().volume = _sliderBGM.GetComponent<Slider>().value;
    }

    public void OnSlideEFFECT()
    {
        SoundManager.Instance.GetcEffPlayer().volume = _sliderEFFECT.GetComponent<Slider>().value;
    }

	public void OnTogglePush(int pushToggleIdx)
	{
		//PopupManager.Instance.ShowLoadingPopup(1);
		//_nClickPushToggleIdx = pushToggleIdx;

		Byte hexCode = (Byte)(0x01 << pushToggleIdx + 1); //0x01 번은 토크사용 함으로 0x02 부터 확인한다
		Legion.Instance.pushSetting |= hexCode;
		if(_togglePush[pushToggleIdx].GetComponent<Toggle>().isOn == false)
			Legion.Instance.pushSetting -= hexCode;

        // 푸쉬값 클라에 저장
        ObscuredPrefs.SetInt("pushSetting", Legion.Instance.pushSetting);

        // 푸쉬값 UI 셋팅
        if (_togglePush[pushToggleIdx].GetComponent<Toggle>().isOn == true)
        {
            Legion.Instance.SetPushActive(pushToggleIdx, true);
            // 2017. 01. 06 jy 서버 푸시는 바로 On Off
            if (pushToggleIdx == 0)
            {
#if UNITY_IOS
                LiveOpsPluginIOS.LiveOpsSetRemotePushEnable(true);
#elif UNITY_ANDROID
                IgaworksUnityAOS.IgaworksUnityPluginAOS.LiveOps.enableService(true);
#endif
            }
            _togglePush[pushToggleIdx].transform.GetChild(1).GetComponent<Text>().text = TextManager.Instance.GetText("popup_btn_option_on");
        }
        else
        {
            Legion.Instance.SetPushActive(pushToggleIdx, false);
            // 2017. 01. 06 jy 서버 푸시는 바로 On Off
            if (pushToggleIdx == 0)
            {
#if UNITY_IOS
					LiveOpsPluginIOS.LiveOpsSetRemotePushEnable(false);
#elif UNITY_ANDROID
                IgaworksUnityAOS.IgaworksUnityPluginAOS.LiveOps.enableService(false);
#endif
            }
            _togglePush[pushToggleIdx].transform.GetChild(1).GetComponent<Text>().text = TextManager.Instance.GetText("popup_btn_option_off");
        }

        //Server.ServerMgr.Instance.OptionSet(Legion.Instance.pushSetting, (Byte)TextManager.Instance.eLanguage, RequestTogglePush);
	}
    /*
    // 2017. 05. 12 JY 푸쉬 셋팅은 서버 동기화는 옵션팝업 종료시 일괄 처리함
    public void RequestTogglePush(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();

        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.OPTION_SET, err), Server.ServerMgr.Instance.CallClear);
            return;
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            //if(_togglePush.GetComponent<Toggle>().isOn)
            if( _togglePush[_nClickPushToggleIdx].GetComponent<Toggle>().isOn == true )
            {
                Legion.Instance.SetPushActive(_nClickPushToggleIdx, true);
                if (_nClickPushToggleIdx == 0)
                {
#if UNITY_IOS
					LiveOpsPluginIOS.LiveOpsSetRemotePushEnable(true);
#elif UNITY_ANDROID
					IgaworksUnityAOS.IgaworksUnityPluginAOS.LiveOps.enableService(true);
#endif
                }
                _togglePush[_nClickPushToggleIdx].transform.GetChild(1).GetComponent<Text>().text = TextManager.Instance.GetText("popup_btn_option_on");
            }
            else
            {
                Legion.Instance.SetPushActive(_nClickPushToggleIdx, false);
                // 2017. 01. 06 jy 서버 푸시는 바로 On Off
                if (_nClickPushToggleIdx == 0)
                {
#if UNITY_IOS
					LiveOpsPluginIOS.LiveOpsSetRemotePushEnable(false);
#elif UNITY_ANDROID
					IgaworksUnityAOS.IgaworksUnityPluginAOS.LiveOps.enableService(false);
#endif
                }
                _togglePush[_nClickPushToggleIdx].transform.GetChild(1).GetComponent<Text>().text = TextManager.Instance.GetText("popup_btn_option_off");
            }
        }
    }
    */
    public void OnToggleHelmet()
    {
        if(bEnable)
        {
            //bEnable = false;
            //return;
        }
        if(_toggleHelmet.GetComponent<Toggle>().isOn)
        {
            ObscuredPrefs.SetBool("HelmetToggle", true);
            _toggleHelmet.transform.GetChild(1).GetComponent<Text>().text = TextManager.Instance.GetText("popup_btn_option_on");
        }
        else
        {
            ObscuredPrefs.SetBool("HelmetToggle", false);
            _toggleHelmet.transform.GetChild(1).GetComponent<Text>().text = TextManager.Instance.GetText("popup_btn_option_off");
        }
    }
    public void OnToggleBattlePush()
    {
        if(bEnable)
        {
            //bEnable = false;
            //return;
        }
        if(_toggleBattle.GetComponent<Toggle>().isOn)
        {
            ObscuredPrefs.SetBool("BattlePushToggle", true);
            _toggleBattle.transform.GetChild(1).GetComponent<Text>().text = TextManager.Instance.GetText("popup_btn_option_on");
        }
        else
        {
            ObscuredPrefs.SetBool("BattlePushToggle", false);
            _toggleBattle.transform.GetChild(1).GetComponent<Text>().text = TextManager.Instance.GetText("popup_btn_option_off");
        }
    }
	public void OnToggleGraphic(int idx)
    {
        if(bEnable)
        {
            //bEnable = false;
            //return;
        }

		ObscuredPrefs.SetInt("Graphic", idx);
		Legion.Instance.graphicGrade = idx;
    }

    public void OnClickLinkAccountFaceBook()
    {
		AccountManager.Instance.FBConnect();
		OnClickClose ();
    }

    public void OnClickLinkAccountGoogle()
    {
		#if (UNITY_ANDROID || (UNITY_IPHONE && !NO_GPGS))
		AccountManager.Instance.GPConnect();
		OnClickClose ();
		#endif
    }

    public void OnClickLinkAccountNaver()
    {
#if UNITY_ANDROID
        AccountManager.Instance.NaverConnect();
#elif UNITY_IOS

#endif
		OnClickClose();
    }

    public void OnClickCloseCoupon()
    {
        strError.text = "";
        strCoupon.text = "";
        PopupManager.Instance.RemovePopup(_couponInput);
        strError.gameObject.SetActive(false);
        _couponInput.SetActive(false);
    }

    public void OnClickInputCoupon()
    {
		if(!Legion.Instance.CheckEmptyInven())
		{
			return;
		}

        strError.text = "";
        PopupManager.Instance.AddPopup(_couponInput, OnClickCloseCoupon);
        _couponInput.SetActive(true);
    }

    public void RequestCouponReward()
    {
		if(!Legion.Instance.CheckEmptyInven())
		{
			return;
		}

        if(strCoupon.text == "")
        {
            //쿠폰 에러 입력값 없음
            strError.text = TextManager.Instance.GetErrorText("coupon_wrong_code");
            strError.gameObject.SetActive(true);
            return;
        }
        else
        {
            for(int i=0; i<strCoupon.text.Length; i++)
            {
                if(strCoupon.text.Substring(i, 1).Equals(" "))
                {
                    //쿠폰 에러 공백
                    strError.text = TextManager.Instance.GetErrorText("coupon_space_code");
                    strError.gameObject.SetActive(true);
                    return;
                }
            }
        }
        //Server.ServerMgr.Instance.ClearFirstJobError();
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestCoupon(strCoupon.text, RecvCouponReward);
    }
    IEnumerator CallClear()
    {
        yield return new WaitForEndOfFrame();
        Server.ServerMgr.Instance.ClearFirstJobError();
    }
    public void RecvCouponReward(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();

        if(err != Server.ERROR_ID.NONE)
        {
			//PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.SOCIAL_COUPON, err), Server.ServerMgr.Instance.CallClear);
			strError.text = TextManager.Instance.GetError(Server.MSGs.SOCIAL_COUPON, err);
            //Server.ServerMgr.Instance.ClearFirstJobError();
            strError.gameObject.SetActive(true);
            StartCoroutine("CallClear");
            return;
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_coupon"), TextManager.Instance.GetText("popup_desc_coupon"), CouponRewardPopup);
            Legion.Instance.u1MailExist = 1;
            //SocialInfo.Instance.u2MailCount = (UInt16)SocialInfo.Instance.dicMailList.Count;
            LobbyScene _lobby = Scene.GetCurrent() as LobbyScene;
            if(_lobby != null)
                _lobby._objAlramIcon[(int)LobbyScene.LobbyAlram.MAIL].SetActive(true);
            return;
        }
    }

    public void CouponRewardPopup(object[] param)
    {
        OnClickCloseCoupon();
    }

    public void OnClickChangeLanguage()
    {
        objChangeLanguagePopup.SetActive(true);
        PopupManager.Instance.AddPopup(objChangeLanguagePopup, OnCloseLanguagePopup);
    }

    public void OnCloseLanguagePopup()
    {
        if(!bLanguageChanged)
        {
            //PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("popup_title_language_change"), TextManager.Instance.GetText("popup_desc_language_change"), ChangeLanguageAccept, null);
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_language_change"), TextManager.Instance.GetText("popup_desc_language_change"), null);
            bLanguageChanged = true;
        }
        PopupManager.Instance.RemovePopup(objChangeLanguagePopup);
        objChangeLanguagePopup.SetActive(false);
    }

    public void ChangeLanguage(int _textType)
    {
        switch(_textType)
        {
            case 0:
                TextManager.Instance.SetLanguage("ENGLISH");
                break;

            case 1:
                TextManager.Instance.SetLanguage("KOREAN");
                break;

            case 2:
                TextManager.Instance.SetLanguage("JAPANESE");
                break;
        }
        
        //strLanguageLabel.GetComponent<LoadText>().TextLoad();
    }

    public void OnClickContact()
    {
		#if UNITY_ANDROID
		IgaworksUnityAOS.IgaworksUnityPluginAOS.Nanoo.openNanooFanPage (false);
		#else
		Application.OpenURL ("https://game.nanoo.so/Guardians/customer/inquiry_post?YIo0J="+SystemInfo.deviceModel+"&BZ2pT="+SystemInfo.operatingSystem+"&TOSoM="+Server.ServerMgr.id);
		#endif
        //Application.OpenURL ("https://play260.helpshift.com");

    }

	void mOnOpenNanooFanPage(string url){
		Application.OpenURL ("https://game.nanoo.so/Guardians/customer/inquiry_post?YIo0J="+SystemInfo.deviceModel+"&BZ2pT="+SystemInfo.operatingSystem+"&TOSoM="+Server.ServerMgr.id);
		// openNanooFanPage api가 호출되고 FanPageURL이 전달됩니다.
	}

    public void OnClickLogout()
    {
        PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("popup_title_logout"), TextManager.Instance.GetText("popup_desc_logout"), RequestLogout, null);
        return;
    }

    public void RequestLogout(object[] param)
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.Logout(LogoutSuccess);
    }

    public void LogoutSuccess(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();

        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.AUTH_LOGOUT, err), Server.ServerMgr.Instance.CallClear);
            return;
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            if (eLanguageType != TextManager.Instance.eLanguage || u1PushSetting != Legion.Instance.pushSetting)
            {
                PopupManager.Instance.ShowLoadingPopup(1);
                Server.ServerMgr.Instance.OptionSet(Legion.Instance.pushSetting, (Byte)TextManager.Instance.eLanguage, LogOutChangedSet);
            }

            else
            {
                OnClickClose();
                if(Legion.Instance.cInventory.dicInventory !=null)
                {
			    	Legion.Instance.InitUserData ();
                }
			    AccountManager.Instance.Logout ();
                //DataMgr.Instance.Init ();
                //#CHATTING
                ObscuredPrefs.DeleteKey("ChattingServerIdx");
			    PopupManager.Instance.ChattingDisconnect();

			    ObscuredPrefs.DeleteKey ("SavedPublish");
			    SceneManager.LoadScene("TitleScene");
            }
        }
    }

    public void LogOutChangedSet(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();

        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.AUTH_LOGOUT, err), Server.ServerMgr.Instance.CallClear);
            return;
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            OnClickCloseCoupon();
            ObscuredPrefs.SetFloat("VolumeBGM", _sliderBGM.GetComponent<Slider>().value);

            float effectValue = _sliderEFFECT.GetComponent<Slider>().value;
            ObscuredPrefs.SetFloat("VolumeEFFECT", effectValue);
            if (effectValue > 0)
                SoundManager.Instance.SetMuteEff(false);

            bLanguageChanged = false;

            if(Legion.Instance.cInventory.dicInventory !=null)
            {
				Legion.Instance.InitUserData ();
            }
			AccountManager.Instance.Logout ();
            //DataMgr.Instance.Init ();
            //#CHATTING
            ObscuredPrefs.DeleteKey("ChattingServerIdx");
			PopupManager.Instance.ChattingDisconnect();

			ObscuredPrefs.DeleteKey ("SavedPublish");
			SceneManager.LoadScene("TitleScene");

            this.gameObject.SetActive(false);

            PopupManager.Instance.RemovePopup(gameObject);
        }
    }

    public void OnClickResignAccount()
    {
        PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("popup_title_resign_account"), TextManager.Instance.GetText("popup_desc_resign_account"), RequestResignAccount, null);
        return;
    }

    public void RequestResignAccount(object[] param)
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.Quit(SuccessResignAccount);
    }

    public void SuccessResignAccount(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();

        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.AUTH_QUIT, err), Server.ServerMgr.Instance.CallClear);
            return;
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            OnClickClose();
            if(Legion.Instance.cInventory.dicInventory !=null)
            {
				Legion.Instance.InitUserData ();
            }
			AccountManager.Instance.Logout ();
			//DataMgr.Instance.Init ();
			ObscuredPrefs.DeleteKey ("guestID");
			ObscuredPrefs.DeleteKey ("SavedPublish");
			SceneManager.LoadScene("TitleScene");
        }
    }

	public void ShowPersonalTerms()
	{
		//Privacy Policy
		Application.OpenURL ("https://goo.gl/WpaokM");
	}

	public void ShowUseTerms()
	{
		Application.OpenURL ("https://goo.gl/XaZ9dJ");
	}

    // 쿠폰 활성화 여부
    public void SetCouponBtnActive(bool value)
    {
        if(_objCoupon == null)
        {
            return;
        }

        _objCoupon.SetActive(value);
    }
}


public class GraphicOption
{
	public static readonly bool[] attack_basic = new bool[4]{true, true, false, false};
	public static readonly bool[] attack_skill = new bool[4]{true, true, true, false};
	public static readonly bool[] attack_missile = new bool[4]{true, true, true, true};

	public static readonly bool[] damage_script = new bool[4]{true, false, false, false};
	public static readonly bool[] damage_basic = new bool[4]{true, true, true, true};

	public static readonly bool[] condition_hero = new bool[4]{true, true, false, false};
	public static readonly bool[] condition_mon = new bool[4]{true, false, false, false};
	public static readonly bool[] run = new bool[4]{true, true, false, false};

	public static readonly bool[] shadow = new bool[4]{true, false, false, false};
}