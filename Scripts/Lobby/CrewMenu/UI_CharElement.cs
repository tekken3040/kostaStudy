using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;

public class UI_CharElement : MonoBehaviour
{
    public GameObject HeroImg;
    public GameObject HeroElementalProperty;
    public GameObject HeroLevel;
    public GameObject NoticeImg;
    GameObject _lobby;
    GameObject _crew;
    public Vector3[] HeroImg_Pos;

    public Hero cHero;
    StringBuilder tempStringBuilder;

    Crew tempSelectedCrew;

    void Awake()
    {
        tempStringBuilder = new StringBuilder();
        //_lobby = GameObject.Find("LobbyScene");
        //_crew = GameObject.Find("Pref_UI_Main_CrewMenu");
        //_lobby = (Scene.GetCurrent() as LobbyScene).gameObject;
        //_crew = _lobby.GetComponent<LobbyScene>()._CrewMenu;
    }

    public void SetData(Hero inHero, LobbyScene lobby, UI_CrewMenu crewMenu)
    {
        _lobby = lobby.gameObject;
        _crew = crewMenu.gameObject;
        cHero = inHero;

        HeroImg.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Hero/hero_icon." + cHero.cClass.u2ID);
        HeroImg.GetComponent<Image>().SetNativeSize();
        //HeroImg.transform.localPosition = HeroImg_Pos[cHero.cClass.u2ID -1];
        HeroElementalProperty.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + (cHero.acEquips[6].GetEquipmentInfo().u1Element));
        HeroLevel.GetComponent<Text>().text = cHero.cLevel.u2Level.ToString();
        if(cHero.GetComponent<StatusComponent>().CheckHaveStatPoint(cHero.cLevel.u2Level))
			this.transform.GetChild(3).gameObject.SetActive(true);//.GetComponent<Image>().enabled = true;
        else if(cHero.GetComponent<SkillComponent>().SkillPoint > 0)
			this.transform.GetChild(3).gameObject.SetActive(true);//.GetComponent<Image>().enabled = true;
        else
        {
            for(int i=0; i<cHero.acEquips.Length; i++)
            {
                if(cHero.acEquips[i].GetComponent<StatusComponent>().CheckHaveEquipStatPoint(cHero.acEquips[i].cLevel.u2Level))
                {
					this.transform.GetChild(3).gameObject.SetActive(true);//GetComponent<Image>().enabled = true;
                    break;
                }

                else
					this.transform.GetChild(3).gameObject.SetActive(false);//.GetComponent<Image>().enabled = false;
            }
        }
        //12.19코드 추가
        if(cHero.u1AssignedCrew != 0)
            this.gameObject.SetActive(false);
        else
            this.gameObject.SetActive(true);
    }

    public void OnClickCharElement()
    {
		if (AssetMgr.Instance.CheckDivisionDownload (1, cHero.cClass.u2ID))
			return;
        //_lobby.GetComponent<LobbyScene>().infoHero = cHero;
        //_lobby.GetComponent<LobbyScene>()._characterInfo.GetComponent<UI_Panel_CharacterInfo>().SetData(_lobby.GetComponent<LobbyScene>().infoHero, _lobby.GetComponent<LobbyScene>());
        //_lobby.GetComponent<LobbyScene>().CallCharacterInfoPopup();
        if(Legion.Instance.cTutorial.au1Step[4] == 200)
        {
            PopupManager.Instance.ShowLoadingPopup(1);

            if(_crew.GetComponent<UI_CrewMenu>().GetSelectedCrew().bDirty)
            {
                _crew.GetComponent<UI_CrewMenu>().GetSelectedCrew().CallServer(CallInfo);
                for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
                    Legion.Instance.v3MainCharRotation[i] = Vector3.zero;
            }
            else if(_crew.GetComponent<UI_CrewMenu>().GetPrevSelectedCrew() != _crew.GetComponent<UI_CrewMenu>().GetSelectedCrew())
            {
                if(_crew.GetComponent<UI_CrewMenu>().GetSelectedCrew().u1Count != 0)
                {
                    _crew.GetComponent<UI_CrewMenu>().GetSelectedCrew().CallServer(CallInfo);
                    for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
                        Legion.Instance.v3MainCharRotation[i] = Vector3.zero;
                }
                else
                    CallInfo(Server.ERROR_ID.NONE);
            }
            else
            {
                CallInfo(Server.ERROR_ID.NONE);
            }
        }
    }

    public void CallInfo(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CREW_CHANGE, err), Server.ServerMgr.Instance.CallClear);
			return;
		}

		else if(err == Server.ERROR_ID.NONE) 
		{
            tempSelectedCrew = _crew.GetComponent<UI_CrewMenu>().GetSelectedCrew();
			if (tempSelectedCrew.DispatchStage != null) {
				SaveCrewSuccess(Server.ERROR_ID.NONE);
				return;
			}

			PopupManager.Instance.ShowLoadingPopup(1);
				
			if(tempSelectedCrew.u1Count != 0)
                Server.ServerMgr.Instance.SelectCrew(tempSelectedCrew, SaveCrewSuccess);
            else
                SaveCrewSuccess(Server.ERROR_ID.NONE);
        }
    }

    public void SaveCrewSuccess(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CREW_SELECT, err), Server.ServerMgr.Instance.CallClear);
			return;
		}

		else if(err == Server.ERROR_ID.NONE) 
		{
            Legion.Instance.bCharInfoToCrew = true;
			_lobby.GetComponent<LobbyScene>().StartCoroutine(_lobby.GetComponent<LobbyScene>().OnClickCharacterInfo(3, 0, cHero, 1));
            _lobby.GetComponent<LobbyScene>().prevScreen = 1;
            _crew.SetActive(false);
		}
    }

    public void CheckNoticeTrue()
    {
        this.transform.GetChild(3).gameObject.SetActive(true);
    }
    public void CheckNoticeFalse()
    {
        this.transform.GetChild(3).gameObject.SetActive(false);
    }

	public void RefreshHeroInfo()
	{
		HeroElementalProperty.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + (cHero.acEquips[6].GetEquipmentInfo().u1Element));
		HeroLevel.GetComponent<Text>().text = cHero.cLevel.u2Level.ToString();
		if(cHero.GetComponent<StatusComponent>().CheckHaveStatPoint(cHero.cLevel.u2Level))
			this.transform.GetChild(3).gameObject.SetActive(true);//.GetComponent<Image>().enabled = true;
		else if(cHero.GetComponent<SkillComponent>().SkillPoint > 0)
			this.transform.GetChild(3).gameObject.SetActive(true);//.GetComponent<Image>().enabled = true;
		else
		{
			for(int i=0; i<cHero.acEquips.Length; i++)
			{
				if(cHero.acEquips[i].GetComponent<StatusComponent>().CheckHaveEquipStatPoint(cHero.acEquips[i].cLevel.u2Level))
				{
					this.transform.GetChild(3).gameObject.SetActive(true);//GetComponent<Image>().enabled = true;
					break;
				}

				else
					this.transform.GetChild(3).gameObject.SetActive(false);//.GetComponent<Image>().enabled = false;
			}
		}
	}
}
