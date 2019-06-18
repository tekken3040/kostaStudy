using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class GuildCrewDetailSlot : MonoBehaviour
{
	public Byte u1Pos;

	[SerializeField] GameObject _objHeroElement;
	[SerializeField] Image _imgHeroElementIcon;
	[SerializeField] Image _imgHeroIcon;
	[SerializeField] Image _imgBG;
	[SerializeField] Text _txtHeroLevel;

	public Hero cHero;
	StringBuilder tempStringBuilder;

	void Awake()
	{
		tempStringBuilder = new StringBuilder();
	}

	public void SetData(Byte u1HeroIdx, bool nullHero = false, bool bEnemy = false)
	{
		if(tempStringBuilder == null)
			tempStringBuilder = new StringBuilder();
		if (nullHero){
			cHero = null;
		} else {
			if (GuildInfoMgr.Instance.cGuildDetailCrew.acLocation [u1HeroIdx] != null)
				cHero = (Hero)GuildInfoMgr.Instance.cGuildDetailCrew.acLocation [u1HeroIdx];
			else
				cHero = null;
		}

		if(cHero == null)
		{
			_objHeroElement.SetActive(false);
			_imgHeroIcon.color = Color.clear;
			_txtHeroLevel.text = "";
			//if(_txtLeaderHero != null)
			//    _txtLeaderHero.gameObject.SetActive(false);
			_imgBG.color = new Color(1, 1, 1, 0.5f);
			if(GetComponent<Button>() != null)
				GetComponent<Button>().interactable = false;
		}
		else
		{
			_objHeroElement.SetActive(true);
			_imgHeroElementIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_icon_" + (cHero.acEquips[6].GetEquipmentInfo().u1Element));
			_imgHeroElementIcon.SetNativeSize();
			_imgHeroIcon.color = Color.white;
			_imgHeroIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/BattleField/league_04.char_large_" + (cHero.cClass.u2ID));
			_imgBG.color = Color.white;
			tempStringBuilder.Remove(0, tempStringBuilder.Length);
			tempStringBuilder.Append("Lv ").Append(cHero.cLevel.u2Level);
			_txtHeroLevel.text = tempStringBuilder.ToString();
			if(GetComponent<Button>() != null)
				GetComponent<Button>().interactable = true;
		}
	}
}
