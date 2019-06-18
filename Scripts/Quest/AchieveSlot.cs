using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class AchieveSlot : MonoBehaviour {
	public Image _imgIconBG;
	public Image _imgIcon;

	public Button _btnReward;

	[SerializeField] GameObject _objIng;
	[SerializeField] GameObject _objReward;
	[SerializeField] GameObject _objClear;

	public Image _imgGauge;

	public Text _txtName;
	public Text _txtButton;
	public Text _txtGoTo;
	public Text _txtState;
	public Text _txtProg;

	public UserAchievement cInfo;
	AchievementInfo info;

	public void SetSlot(UserAchievement uinfo){
		cInfo = uinfo;
		info = uinfo.GetInfo();

		string bgName = "achieve";

		if(info.u1PeriodType > 1) bgName = "daily";

		_imgIconBG.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_03_renew.bg_icon_"+bgName);
		_imgIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/"+info.sIconImage);
		_imgIcon.SetNativeSize();

		_txtGoTo.text = TextManager.Instance.GetText("btn_achi_shortcut");
		if (uinfo.bRewarded)
        {
            _txtButton.text = TextManager.Instance.GetText("btn_achi_reward_get");

            _txtState.text = TextManager.Instance.GetText("mark_icon_quest_done");
            _txtButton.color = Color.gray;
            _txtGoTo.color = Color.gray;
            _objReward.SetActive(true);
            _objIng.SetActive(true);
            _objClear.SetActive(false);
            _txtProg.text = info.u4MaxCount + "/" + info.u4MaxCount;
            _imgGauge.fillAmount = 1f;
        }
		else if (uinfo.isClear() || info.cOpenGoods.u1Type == (Byte)GoodsType.VIP_LEVEL)
        {
            _txtButton.text = TextManager.Instance.GetText("btn_achi_reward_get");
            _txtButton.color = Color.white;
            _txtGoTo.color = Color.gray;
            _txtState.text = "";
            _objClear.SetActive(true);
            _objReward.SetActive(true);
            _objIng.SetActive(false);
            _txtProg.text = info.u4MaxCount + "/" + info.u4MaxCount;
            _imgGauge.fillAmount = 1f;
        }
        else 
        {
			_txtButton.text = TextManager.Instance.GetText("btn_achi_reward");
			_txtButton.color = Color.white;
			_txtGoTo.color = Color.white;
			_txtState.text = "";
			_objIng.SetActive (false);
			_objClear.SetActive (false);
			_objReward.SetActive (false);
			uint cnt = uinfo.GetCount ();
			_txtProg.text = cnt+"/"+info.u4MaxCount;
			_imgGauge.fillAmount = (float)cnt / (float)info.u4MaxCount;
		}

		_txtName.text = TextManager.Instance.GetText(info.sName);

		_btnReward.gameObject.AddComponent<TutorialButton>().id = "Reward"+info.u2ID;
	}

	public void ClickGoTo(){
		if(cInfo.bRewarded) return;

		if (_objClear.activeSelf) return;

		MENU shortCut = (MENU)info.u2ShortCut;
		int shortCutPopup = info.u2ShortCutDetail;

		StageInfoMgr.Instance.ShortCutChapter = -1;
		int selectedStage = -1;

		if (shortCut == MENU.CAMPAIGN && (info.u1AchievementType == 2 || info.u1AchievementType == 18 || info.u1AchievementType == 19) && info.u2AchievementTypeID != 0) {
			if (!StageInfoMgr.Instance.IsOpen (info.u2AchievementTypeID, info.u1Delemiter1)) {
				string title = TextManager.Instance.GetText ("popup_title_not_clear");
				string msg = TextManager.Instance.GetText ("popup_desc_not_clear");
				PopupManager.Instance.ShowOKPopup (title, msg, null);
				return;
			}

			selectedStage = info.u2AchievementTypeID;
			StageInfoMgr.Instance.ShortCutChapter = -1;
		} else if (info.u1AchievementType == 15 && info.u2AchievementTypeID != 0){
			StageInfoMgr.Instance.LastPlayStage = -1;
			foreach(ActInfo actInfo in StageInfoMgr.Instance.dicActData.Values){
				for(int a=0; a<actInfo.lstChapterID.Count; a++){
					ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData [actInfo.lstChapterID [a]];

					for(int b=0; b<chapterInfo.lstStageID.Count; b++){
						StageInfo stageInfo = StageInfoMgr.Instance.dicStageData [chapterInfo.lstStageID [b]];
						for(int i=0; i<stageInfo.acPhases.Length; i++){
							for(int j=0; j<StageInfoMgr.Instance.GetFieldInfo(stageInfo.acPhases[i].u2FieldID).acMonsterGroup.Length; j++){
								for(int k=0; k<StageInfoMgr.Instance.GetFieldInfo(stageInfo.acPhases[i].u2FieldID).acMonsterGroup[j].acMonsterInfo.Length; k++){
									ushort monID = StageInfoMgr.Instance.GetFieldInfo(stageInfo.acPhases[i].u2FieldID).acMonsterGroup[j].acMonsterInfo[k].u2MonsterID;
									if (info.u2AchievementTypeID == monID && StageInfoMgr.Instance.IsOpen (stageInfo.u2ID, info.u1Delemiter1)) {
										selectedStage = stageInfo.u2ID;
									}
								}
							}
						}
					}

				}
			}
		} else if (info.u1AchievementType == 29 && info.u2AchievementTypeID != 0){
			StageInfoMgr.Instance.LastPlayStage = -1;
			foreach(ActInfo actInfo in StageInfoMgr.Instance.dicActData.Values){
				for(int a=0; a<actInfo.lstChapterID.Count; a++){
					ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData [actInfo.lstChapterID [a]];

					for(int b=0; b<chapterInfo.lstStageID.Count; b++){
						StageInfo stageInfo = StageInfoMgr.Instance.dicStageData [chapterInfo.lstStageID [b]];
						if(stageInfo.CheckRewardInStage(info.u2AchievementTypeID) > 0)
						{
							selectedStage = stageInfo.u2ID;
						}
					}

				}
			}
		} else {
			StageInfoMgr.Instance.ShortCutChapter = 0;
		} 

		if (selectedStage == -1 && StageInfoMgr.Instance.ShortCutChapter == -1) {
			string title = TextManager.Instance.GetText ("popup_title_not_clear");
			string msg = TextManager.Instance.GetText ("popup_desc_not_clear");
			PopupManager.Instance.ShowOKPopup (title, msg, null);
			return;
		}

		StageInfoMgr.Instance.LastPlayStage = selectedStage;

		if (shortCutPopup == 71)
			shortCutPopup = 74;
		if (shortCutPopup == 72)
			shortCutPopup = 75;
		if (shortCutPopup == 73)
			shortCutPopup = 76;

		FadeEffectMgr.Instance.QuickChangeScene(shortCut, shortCutPopup);
	}

	public void DestroyMe(){
		if (cInfo.GetInfo ().u1PeriodType == 1) {
			Destroy (gameObject);
		}else{
			_objReward.SetActive (true);
			_objIng.SetActive (true);
			_objClear.SetActive (false);
			_txtState.text = TextManager.Instance.GetText("mark_icon_quest_done");
		}
	}
}
