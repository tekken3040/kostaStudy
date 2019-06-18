using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class UI_League_CharElement : MonoBehaviour
{
    public GameObject HeroImg;
    public GameObject HeroElementalProperty;
    public GameObject HeroLevel;
    public GameObject NoticeImg;
    public Hero cHero;

    StringBuilder tempStringBuilder;
    LeagueScene _leagueScene;

    void Awake()
    {
        tempStringBuilder = new StringBuilder();
    }

    public void SetData(Hero inHero)
    {
        _leagueScene = GameObject.Find("LeagueScene").GetComponent<LeagueScene>();
        cHero = inHero;

        HeroImg.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Hero/hero_icon." + cHero.cClass.u2ID);
        HeroImg.GetComponent<Image>().SetNativeSize();
        HeroElementalProperty.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + (cHero.acEquips[6].GetEquipmentInfo().u1Element));
        HeroLevel.GetComponent<Text>().text = cHero.cLevel.u2Level.ToString();
        if(cHero.GetComponent<StatusComponent>().CheckHaveStatPoint(cHero.cLevel.u2Level))
			this.transform.GetChild(3).gameObject.SetActive(true);
        else if(cHero.GetComponent<SkillComponent>().SkillPoint > 0)
			this.transform.GetChild(3).gameObject.SetActive(true);
        else
        {
            for(int i=0; i<cHero.acEquips.Length; i++)
            {
                if(cHero.acEquips[i].GetComponent<StatusComponent>().CheckHaveEquipStatPoint(cHero.acEquips[i].cLevel.u2Level))
                {
					this.transform.GetChild(3).gameObject.SetActive(true);
                    break;
                }

                else
					this.transform.GetChild(3).gameObject.SetActive(false);
            }
        }

        if(cHero.bAssignedLeagueCrew)
            this.gameObject.SetActive(false);
        else
            this.gameObject.SetActive(true);
    }

    public void OnClickCharElement()
    {
		if (AssetMgr.Instance.CheckDivisionDownload (1, cHero.cClass.u2ID))
			return;
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
            PopupManager.Instance.ShowLoadingPopup(1);
        }
    }

    public void OnClickGoInfo()
    {
        UI_League.Instance.u1SelectLeagueCharIndex = cHero.u1Index;
        Legion.Instance.bLeagueToCharInfo = true;
        _leagueScene.OnClickClose(1);
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
			this.transform.GetChild(3).gameObject.SetActive(true);
		else if(cHero.GetComponent<SkillComponent>().SkillPoint > 0)
			this.transform.GetChild(3).gameObject.SetActive(true);
		else
		{
			for(int i=0; i<cHero.acEquips.Length; i++)
			{
				if(cHero.acEquips[i].GetComponent<StatusComponent>().CheckHaveEquipStatPoint(cHero.acEquips[i].cLevel.u2Level))
				{
					this.transform.GetChild(3).gameObject.SetActive(true);
					break;
				}

				else
					this.transform.GetChild(3).gameObject.SetActive(false);
			}
		}
        if(cHero.bAssignedLeagueCrew)
            this.gameObject.SetActive(false);
        else
            this.gameObject.SetActive(true);
	}
}
