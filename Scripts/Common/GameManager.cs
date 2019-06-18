using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum MENU
{
	NONE = 0,
	TITLE = 1000, // Scene
	MAIN = 1001, // Scene
	INVENTORY = 1002, // Prefab
	FORGE = 1003, // Scene
	HERO_GUILD = 1004, // Scene
	SHOP = 1005, // Prefab
	LEAGUE = 1006, // Scene
	CAMPAIGN = 1007, // Scene
	CREW = 1008, // Prefab
	CREATE_CHARACTER = 1009, // Scene
	CHARACTER_INFO = 1010, // Prefab
	SOCIAL = 1011,
	BATTLE = 1012,
	BATTLE_RESULT = 1013,
	QUEST = 1014,	// Prefab
	BOSS_RUSH = 1015,	// Scene
}

public enum POPUP_MAIN
{
	// Range 10~19
	NONE = 0,
	GOLD = 10,
	CASH = 11,
	ENERGY = 12,
	CHAT = 13,
	OPTION = 14,
	QUEST = 15,
	ADVENTO = 16,
	ODIN = 17,
}

public enum POPUP_INVENTORY
{
	// Range 20~29
	NONE = 0,
	SELECT_CATEGORY = 20,
}

public enum POPUP_FORGE
{
	//Range 30~39
	NONE = 0,
	MAIN = 30,
	SMITH = 31,
	FUSION = 32,
	RUNE = 33,
	CHANGE_LOOK = 34,
	UPGRADE = 35,
}

public enum POPUP_HERO_GUILD
{
	//Range 40~49
	NONE = 0,
	DAILY_ACHIEVEMENT = 40,
	WEEKLY_ACHIEVEMENT = 41,
	ACHIEVEMENT = 42,
	QUEST = 43,
	QUEST_SELECT = 44,
	TRAINING_HERO = 45,
	TRAINING_EQUIP = 46
}

public enum POPUP_QUEST
{
	NONE = 0
}

public enum POPUP_SHOP
{
	//Range 50~59
	NONE = 0,
	GOLD = 50,
	CASH = 51,
	ENERGY = 52,
	NORMAL = 53,
	EQUIPMENT = 54,
	RANDOM = 55,
	BLACK = 56,
	THOUSAND = 57
}

public enum POPUP_LEAGUE
{
	//Range 60~69
	NONE = 0,
}

public enum POPUP_CAMPAIGN
{
	//Range 70~89
	NONE = 0,
	STAGE_SELECT_EASY = 71,
	STAGE_SELECT_NORMAL = 72,
	STAGE_SELECT_HELL = 73,
	STAGE_INFO_EASY = 74,
	STAGE_INFO_NORMAL = 75,
	STAGE_INFO_HELL = 76,

	TOWER_SELECT_EASY = 77,
	TOWER_SELECT_NORMAL = 78,
	TOWER_SELECT_HELL = 79,
	TOWER_INFO_EASY = 80,
	TOWER_INFO_NORMAL = 81,
	TOWER_INFO_HELL = 82,

	FOREST_SELECT_EASY = 83,
	FOREST_SELECT_NORMAL = 84,
	FOREST_SELECT_HELL = 85,
	FOREST_INFO_EASY = 86,
	FOREST_INFO_NORMAL = 87,
	FOREST_INFO_HELL = 88,
}

public enum POPUP_CREW
{
	//Range 90~99
	NONE = 0,
}

public enum POPUP_CREATE_CHARACTER
{
	//Range 100~109
	NONE = 0,
	CLASS = 100,
	HAIR = 101,
	HAIR_COLOR = 102,
	FACE = 103,
}

public enum POPUP_CHARACTER_INFO
{
	//Range 110~129
	NONE = 0,
	STATUS = 110,
	STATUS_USE_POTION = 111,
	STATUS_RESET_POINT = 112,
	STATUS_BUY_POINT = 113,
	STATUS_AUTO = 114,
	STATUS_RETIRE = 115,
	EQUIPMENT = 116,
	EQUIPMENT_AUTO = 117,
	EQUIPMENT_SELECT = 118,
	EQUIPMENT_AUTO_STATUS = 119,
	SKILL = 120,
	SKILL_AUTO = 121,
	SKILL_SELECT_SLOT = 122,
	SKILL_BUY_SLOT = 123,
	SKILL_BUY_POINT = 124,
	SKILL_ACTIVE = 125,
	SKILL_RESET_POINT = 126,
	SKILL_UPGRADE = 127,
	EQUIP_CREATE_UPGRADE = 128,
    EQUIP_CREATE = 129,
}


public enum POPUP_SOCIAL
{
	//Range 120~129
	NONE = 0,
	FRIEND = 120,
	MAILBOX = 121,
	NOTICE = 122,
	RANK = 123,
}
public class GameManager : Singleton<GameManager> { 
	Dictionary<MENU, ReservedPopup> dicReservedPopup;
	public class ReservedPopup
	{
		public MENU menu;
		public int popup;
		public object[] param;
		public ReservedPopup()
		{
			menu = MENU.NONE;
			popup = 0;
			param = null;
		}

		public void Copy(ReservedPopup origin)
		{
			menu = origin.menu;
			popup = origin.popup;
			param = origin.param;
		}

		public POPUP_MAIN GetReservedPopupMain()
		{
			return (POPUP_MAIN)popup;
		}
		public POPUP_INVENTORY GetReservedPopupInventory()
		{
			return (POPUP_INVENTORY)popup;
		}
		public POPUP_FORGE GetReservedPopupForge()
		{
			return (POPUP_FORGE)popup;
		}
		public POPUP_HERO_GUILD GetReservedPopupHeroGuild()
		{
			return (POPUP_HERO_GUILD)popup;
		}
		public POPUP_QUEST GetReservedPopupQuest()
		{
			return (POPUP_QUEST)popup;
		}
		public POPUP_SHOP GetReservedPopupShop()
		{
			return (POPUP_SHOP)popup;
		}
		public POPUP_LEAGUE GetReservedPopupLeague()
		{
			return (POPUP_LEAGUE)popup;
		}
		public POPUP_CAMPAIGN GetReservedPopupCampaign()
		{
			return (POPUP_CAMPAIGN)popup;
		}
		public POPUP_CREW GetReservedPopupCrew()
		{
			return (POPUP_CREW)popup;
		}
		public POPUP_CREATE_CHARACTER GetReservedPopupCreateCharacter()
		{
			return (POPUP_CREATE_CHARACTER)popup;
		}
		public POPUP_CHARACTER_INFO GetReservedPopupCharacterInfo()
		{
			return (POPUP_CHARACTER_INFO)popup;
		}
		public POPUP_SOCIAL GetReservedPopupSocial()
		{
			return (POPUP_SOCIAL)popup;
		}
	}

	void ReservePopup(MENU menu, int popupIdx, object[] popupParam)
	{
		ReservedPopup reservedPopup = new ReservedPopup();

		reservedPopup.menu = menu;
		reservedPopup.popup = popupIdx;
		reservedPopup.param = popupParam;

		if(dicReservedPopup == null)
			dicReservedPopup = new Dictionary<MENU, ReservedPopup>();

		if(dicReservedPopup.ContainsKey(menu) == false)
			dicReservedPopup.Add(menu, reservedPopup);
		else
			dicReservedPopup[menu] = reservedPopup;
	}
	
	public void ReservePopupMain(POPUP_MAIN popup, object[] popupParam)
	{
		ReservePopup(MENU.MAIN, (int)popup, popupParam);
	}
	public void ReservePopupInventory(POPUP_INVENTORY popup, object[] popupParam)
	{
		ReservePopup(MENU.INVENTORY, (int)popup, popupParam);
	}
	public void ReservePopupForge(POPUP_FORGE popup, object[] popupParam)
	{
		ReservePopup(MENU.FORGE, (int)popup, popupParam);
	}
	public void ReservePopupHeroGuild(POPUP_HERO_GUILD popup, object[] popupParam)
	{
		ReservePopup(MENU.HERO_GUILD, (int)popup, popupParam);
	}
	public void ReservePopupQuest(POPUP_QUEST popup, object[] popupParam)
	{
		ReservePopup(MENU.QUEST, (int)popup, popupParam);
	}
	public void ReservePopupShop(POPUP_SHOP popup, object[] popupParam)
	{
		ReservePopup(MENU.SHOP, (int)popup, popupParam);
	}
	public void ReservePopupLeague(POPUP_LEAGUE popup, object[] popupParam)
	{
		ReservePopup(MENU.LEAGUE, (int)popup, popupParam);
	}
	public void ReservePopupCampaign(POPUP_CAMPAIGN popup, object[] popupParam)
	{
		ReservePopup(MENU.CAMPAIGN, (int)popup, popupParam);
	}
	public void ReservePopupCrew(POPUP_CREW popup, object[] popupParam)
	{
		ReservePopup(MENU.CREW, (int)popup, popupParam);
	}
	public void ReservePopupCreateCharacter(POPUP_CREATE_CHARACTER popup, object[] popupParam)
	{
		ReservePopup(MENU.CREATE_CHARACTER, (int)popup, popupParam);
	}
	public void ReservePopupCharacterInfo(POPUP_CHARACTER_INFO popup, object[] popupParam)
	{
		ReservePopup(MENU.CHARACTER_INFO, (int)popup, popupParam);
	}
	public void ReservePopupSocial(POPUP_SOCIAL popup, object[] popupParam)
	{
		ReservePopup(MENU.SOCIAL, (int)popup, popupParam);
	}
//	public bool CheckOpendMenu(MENU currentMenu)
//	{
//		bool ret = false;
//		if(currentMenu == MENU.MAIN && Application.loadedLevelName == "LobbyScene")
//			ret = true;
//		if(currentMenu == MENU.INVENTORY)
//
//		return ret;
//	}
	public ReservedPopup GetReservedPopup(MENU menu)
	{
		ReservedPopup reservedPopup = null;
		if(menu != MENU.NONE)
		{
			if(dicReservedPopup != null && dicReservedPopup.TryGetValue(menu, out reservedPopup))
			{
				DebugMgr.Log(menu + " included");
				ReservedPopup retPopup = new ReservedPopup();
				retPopup.Copy(reservedPopup);
				dicReservedPopup.Remove(menu);
				return retPopup;
			}
		}
		return null;
	}

	public int GetReserveCount()
	{
		if (dicReservedPopup == null)
			return 0;
		
		return dicReservedPopup.Count;
	}

}