using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UI_ListElement_Forge_Smithing_MaterialShortcut : MonoBehaviour {
	[SerializeField] Image _imgBossFrame;
	[SerializeField] Image _imgElementalIcon;
	[SerializeField] Text _txtMain;
	[SerializeField] Text _txtSub;
	[SerializeField] Text _txtStageNum;
	[SerializeField] Image _imgDivision;
	[SerializeField] Image _imgAdvento;
	[SerializeField] Button _btnGoTo;
	[SerializeField] RectTransform _trBossParents;
	[SerializeField] RectTransform _trNormalParents;
	[SerializeField] RectTransform _trStageIcon;

	private byte u1Difficulty;
    MaterialItemInfo _cMaterialItemInfo;
    int locationIdx;

	enum ShortCutPos{
		Stage,
		League,
		Advento,
		BossRush
	}

	ShortCutPos eShorcutPos;
    
    public void SetData(MaterialItemInfo materialInfo, int locationIndex)
	{
		eShorcutPos = ShortCutPos.Stage;
        locationIdx = locationIndex;
        _cMaterialItemInfo = materialInfo;
        MaterialItemInfo.Location _cLocationInfo = _cMaterialItemInfo.acLocation[locationIndex];

        string title = "";
		if(_cLocationInfo.eMenuID == MENU.CAMPAIGN)
		{
			string title2 = "";
			switch((POPUP_CAMPAIGN)_cLocationInfo.u2SubMenuID)
			{
			// 스테이지
			case POPUP_CAMPAIGN.STAGE_SELECT_EASY:
			case POPUP_CAMPAIGN.STAGE_INFO_EASY:
				title = TextManager.Instance.GetText("stage");
				u1Difficulty = 1;
				break;
			case POPUP_CAMPAIGN.STAGE_SELECT_NORMAL:
			case POPUP_CAMPAIGN.STAGE_INFO_NORMAL:
				title = TextManager.Instance.GetText("stage");
				u1Difficulty = 2;
				break;
			case POPUP_CAMPAIGN.STAGE_SELECT_HELL:
			case POPUP_CAMPAIGN.STAGE_INFO_HELL:
				title = TextManager.Instance.GetText("stage");
				u1Difficulty = 3;
				break;
			// 시련의 탑
			case POPUP_CAMPAIGN.TOWER_SELECT_EASY:
			case POPUP_CAMPAIGN.TOWER_INFO_EASY:
				title = TextManager.Instance.GetText("tower");
				u1Difficulty = 1;
				break;
			case POPUP_CAMPAIGN.TOWER_SELECT_NORMAL:
			case POPUP_CAMPAIGN.TOWER_INFO_NORMAL:
				title = TextManager.Instance.GetText("tower");
				u1Difficulty = 2;
				break;
			case POPUP_CAMPAIGN.TOWER_SELECT_HELL:
			case POPUP_CAMPAIGN.TOWER_INFO_HELL:
				title = TextManager.Instance.GetText("tower");
				u1Difficulty = 3;
				break;
			// 탐색의 숲
			case POPUP_CAMPAIGN.FOREST_SELECT_EASY:
			case POPUP_CAMPAIGN.FOREST_INFO_EASY:
				title = TextManager.Instance.GetText("forest");
				u1Difficulty = 1;
				break;
			case POPUP_CAMPAIGN.FOREST_SELECT_NORMAL:
			case POPUP_CAMPAIGN.FOREST_INFO_NORMAL:
				title = TextManager.Instance.GetText("forest");
				u1Difficulty = 2;
				break;
			case POPUP_CAMPAIGN.FOREST_SELECT_HELL:
			case POPUP_CAMPAIGN.FOREST_INFO_HELL:
				title = TextManager.Instance.GetText("forest");
				u1Difficulty = 3;
				break;
			default:
				DebugMgr.LogError("잘못된 유형의 값" + _cLocationInfo.u2SubMenuID);
				break;
			}

			switch (u1Difficulty)
			{
			case 1: 
				title2 = TextManager.Instance.GetText("btn_diffi_easy"); 
				break;
			case 2: 
				title2 = TextManager.Instance.GetText("btn_diffi_normal"); 
				break;
			case 3: 
				title2 = TextManager.Instance.GetText("btn_diffi_hell"); 
				break;
			}
			_txtMain.text = title + " [" + title2 + "]";
			// 16 .7 .14 JY
			// 아이콘 셋팅
			SetStageIcon();
			//DebugMgr.Log("Path : " + "Sprites/Campaign/" + stageInfo.stageMiniIconPath);
			//_btnGoTo.onClick.AddListener( () => OnClickEvent_Stage() );
		}
	}

	public void SetStageIcon()
	{
		StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[(UInt16)_cMaterialItemInfo.acLocation[locationIdx].aoMenuParam[2]];
		// 스테이지 이름 셋팅
		_txtSub.text = TextManager.Instance.GetText(stageInfo.sName);
		// 스테이지 속성 셋팅
		if( stageInfo.IsOpen(u1Difficulty) == false )
		{
			_imgElementalIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Forge/Forge_grade_01.Shortcut_Elemental_1");
			_btnGoTo.interactable = false;
		}
		else
		{
			_imgElementalIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Forge/Forge_grade_01.Shortcut_Elemental_"+GetElementalType(stageInfo.u1RecommandElement));
			_btnGoTo.interactable = true;
		}

		// 스테이지 넘버 셋팅
		_txtStageNum.text = stageInfo.chapterInfo.u1Number.ToString() + "-" + stageInfo.u1StageNum.ToString();

		// 스테이지 아이콘 셋팅
		// 보스가 없거나 탐색의 숲이라면 
		if (stageInfo.u1BossType == 0 || stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.FOREST)
		{
			_trStageIcon.SetParent(_trNormalParents);
			_trBossParents.gameObject.SetActive(false);
			_trNormalParents.gameObject.SetActive(true);
		}
		else
		{
			_imgBossFrame.sprite = AtlasMgr.Instance.GetSprite("Sprites/Forge/Forge_grade_01.Shortcut_BossFrame_"+stageInfo.u1BossType.ToString());
			_trStageIcon.SetParent(_trBossParents);
			_trBossParents.gameObject.SetActive(true);
			_trNormalParents.gameObject.SetActive(false);
		}
		_trStageIcon.localPosition = Vector3.zero;
	}

	protected string GetElementalType(Byte recommandElement)
	{
		string result = "1";
		switch(recommandElement)
		{
		case 2:	// 추천속성:불속성 -> 스테이지속성:바람
			result = "4";
			break;
		case 3:	// 추천속성:물속성 -> 스테이지속성:불
			result = "2";
			break;
		case 4:	// 추천속성:바람속성 -> 스테이지속성:물
			result = "3";
			break;
		}

		return result;
	}

	public void OnClickEvent_Stage()
	{
		switch (eShorcutPos) {
		case ShortCutPos.Stage:
			ClickForStage ();
			break;
		case ShortCutPos.League:
			FadeEffectMgr.Instance.QuickChangeScene (MENU.LEAGUE, 0, null);
			break;
		case ShortCutPos.Advento:
			FadeEffectMgr.Instance.QuickChangeScene (MENU.MAIN, (int)POPUP_MAIN.ADVENTO, null);
			break;
		case ShortCutPos.BossRush:
			FadeEffectMgr.Instance.QuickChangeScene (MENU.BOSS_RUSH, 0, null);
			break;
		}
	}

	void ClickForStage()
	{
		UInt16 stageID = (UInt16)_cMaterialItemInfo.acLocation[locationIdx].aoMenuParam[2];
		StageInfo stageInfo  = StageInfoMgr.Instance.dicStageData[stageID];
		// 2016. 08. 23 jy 
		// 탐색의 숲이라면 선택된 스테이지가 오픈되는 날인지를 확인한다
		if(stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.FOREST)
		{
			//StageInfoMgr.Instance.forest.au1OpenElement[(int)Legion.Instance.ServerTime.DayOfWeek];
			if(StageInfoMgr.Instance.OpenForestElement != StageInfo.Forest.ELEMENT_ALL)
			{
				if(stageInfo.u1ForestElement != StageInfoMgr.Instance.OpenForestElement)
				{
					PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_warnning"), TextManager.Instance.GetText("popup_shortcut_forest_dayclose"), null);
					return;
				}
			}
		}

		if(stageInfo.IsOpen(u1Difficulty) == false)
		{
			string title = TextManager.Instance.GetText("popup_title_not_clear");
			string msg = TextManager.Instance.GetText("popup_desc_not_clear");
			PopupManager.Instance.ShowOKPopup(title, msg, null);
			return;
		}

		// 캠페인 스테이지 일때만 반복전투 아이템을 셋팅을 한다
		if (stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.STAGE)
			SetRepatTargetItem();
		StageInfoMgr.Instance.LastPlayStage = stageID;
		GameManager.Instance.ReservePopupCampaign((POPUP_CAMPAIGN)_cMaterialItemInfo.acLocation[locationIdx].u2SubMenuID, _cMaterialItemInfo.acLocation[locationIdx].aoMenuParam);
		StartCoroutine(gotoStage());
	}

	IEnumerator gotoStage()
	{
		FadeEffectMgr.Instance.FadeOut ();
		yield return new WaitForSeconds (FadeEffectMgr.GLOBAL_FADE_TIME);
		AssetMgr.Instance.SceneLoad("SelectStageScene");
	}

    protected void SetRepatTargetItem()
    {
        UInt16 ownCount = 0;
        Item item = null;
        UInt16 invenSlotNum = 0;
        if (Legion.Instance.cInventory.dicItemKey.TryGetValue(_cMaterialItemInfo.u2ID, out invenSlotNum))
        {
            if (Legion.Instance.cInventory.dicInventory.TryGetValue(invenSlotNum, out item))
                ownCount = ((MaterialItem)item).u2Count;
        }

        Goods materialGoods = new Goods((Byte)GoodsType.MATERIAL, _cMaterialItemInfo.u2ID, StageInfoMgr.Instance.u4CurTargetItemCount);
        // 필요한 재료 갯수보다 많이 가지고 있다면 필요갯수를 0으로 만든다
        if (materialGoods.u4Count <= ownCount)
            materialGoods.u4Count = 1;

        StageInfoMgr.Instance.RepeatTargetItem = materialGoods;
        StageInfoMgr.Instance.u4CurTargetItemCount = 0;
    }


	public void SetData(int divisionIdx)
	{
		eShorcutPos = ShortCutPos.League;

		_txtMain.text = TextManager.Instance.GetText("popup_league_desc_league_title");
		_txtSub.text = String.Format(TextManager.Instance.GetText("league_desc_season_reward"), TextManager.Instance.GetText("mark_division_"+divisionIdx));

		_imgDivision.enabled = true;
		_imgDivision.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/flag_01.btn_Division_"+divisionIdx);
		_imgDivision.SetNativeSize ();

		_trBossParents.gameObject.SetActive(false);
		_trNormalParents.gameObject.SetActive(false);
	}

	public void SetDataForBossRush()
	{
		eShorcutPos = ShortCutPos.BossRush;

		_txtMain.text = TextManager.Instance.GetText("event_bossrush_title");
		_txtSub.text = TextManager.Instance.GetText("event_bossrush_popup_reward");

		_imgDivision.enabled = true;
		_imgDivision.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_01_renew.bossrush_icon");

		_trBossParents.gameObject.SetActive(false);
		_trNormalParents.gameObject.SetActive(false);

		if(!EventInfoMgr.Instance.dicEventReward.ContainsKey(EventInfoMgr.Instance.lstBossRush[0].u2EventID))
		{
			_btnGoTo.interactable = false;
		}
		else if(StageInfoMgr.Instance.OpenBossRush == 0)
		{
			_btnGoTo.interactable = false;
		}
		else
		{
			_btnGoTo.interactable = true;
		}
	}

	public void SetData(UInt16 stageID, EventDungeonShopInfo eventInfo)
	{
		eShorcutPos = ShortCutPos.Advento;

		_txtMain.text = TextManager.Instance.GetText(eventInfo.sTitle);
		_txtSub.text = TextManager.Instance.GetText(StageInfoMgr.Instance.dicStageData[stageID].sName);

		_imgAdvento.enabled = true;

		_trBossParents.gameObject.SetActive(false);
		_trNormalParents.gameObject.SetActive(false);

		if (Legion.Instance.cEvent.dicDungeonOpenInfo.ContainsKey (eventInfo.u2EventID)) {

			if (EventInfoMgr.Instance.CheckOpen (Legion.Instance.cEvent.dicDungeonOpenInfo [eventInfo.u2EventID])) {
				_btnGoTo.interactable = true;
			} else {
				_btnGoTo.interactable = false;
			}
		} else {
			_btnGoTo.interactable = false;
		}
	}
}
