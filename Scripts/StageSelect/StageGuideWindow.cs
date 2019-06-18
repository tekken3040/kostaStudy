using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class StageGuideWindow : MonoBehaviour {

	public Text txtGuide;

	public Text txtBossName;
	public Text txtBossEle;
	public Image imgBossIcon;
	public Image imgBossEle;

	public Image imgTier;
	public Text txtTier;
	public Text txtTierNum;
	public Text txtTierNeed;

	public Transform trBossSkills;
	public GameObject objBossSkill;
	public Transform trUserSkills;
	public GameObject objUserSkill;

	public GameObject objCondiDesc;
	public RectTransform trCondiBG;
	public Text txtCondiDesc;

	GuideInfo guideInfo;
    int _totalUasedStatPoint;                       //총 사용 스텟포인트
    List<Hero> lstHero;                             //스텟 투자한 영웅 리스트
    List<UInt16[]> lstAddPoint;                     //스텟 리스트
    UInt16[] _statusAddPoint;                       //투자한 스텟 포인트(0 == HP, 1 == STR, 2 == INT, 3 == DEF, 4 == RES, 5 == AGI, 6 == CRI)
    Hero cHero;
	List<UInt16> RecomCondis = new List<UInt16>();

	int needPartsLevel = 1;
	int needPartsCount = 1;

	public void SetPopup (StageInfo info) {
		for (int i = 0; i < trBossSkills.childCount; i++) {
			Destroy (trBossSkills.GetChild (i).gameObject);
		}

		guideInfo = StageInfoMgr.Instance.GetGuideInfo (info.au2GuideID [Legion.Instance.SelectedDifficult - 1]);
		txtGuide.text = TextManager.Instance.GetText( guideInfo.sDesc );
		ClassInfo bossInfo = info.GetBoss ();
		if (bossInfo != null) {
			txtBossName.text = TextManager.Instance.GetText (bossInfo.sName);
			txtBossEle.text = TextManager.Instance.GetText ("element_" + bossInfo.u1Element);
			imgBossEle.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.element_icon_" + bossInfo.u1Element);
			imgBossEle.SetNativeSize ();
		}

        imgBossIcon.enabled = true;
        if (info.stageMiniIconPath != "0") {
			imgBossIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Campaign/" + info.stageMiniIconPath + "_on");
		} else {		
            imgBossIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Campaign/stage_icon_mini_03.Stage_Default_icon");
        }

		needPartsLevel = ForgeInfoMgr.Instance.GetInfo(info.smithID[Legion.Instance.SelectedDifficult-1]).u1Level;
		needPartsCount = info.recommendEquipPartsCountKey [Legion.Instance.SelectedDifficult - 1];

		imgTier.sprite = AtlasMgr.Instance.GetSprite("Sprites/Forge/Forge_01.Forge_01_LevelIcon_"+needPartsLevel.ToString("00"));
        imgTier.SetNativeSize();
		txtTier.text = TextManager.Instance.GetText ("forge_level_2_" + needPartsLevel);
		txtTierNum.text = needPartsLevel.ToString ();
		txtTierNeed.text = TextManager.Instance.GetText (info.recommendEquipPartsCountKey [Legion.Instance.SelectedDifficult - 1]+"part");

		for (int i = 0; i < bossInfo.acActiveSkills.Count; i++) {
			if (bossInfo.acActiveSkills [i].cBuff != null) {
				GameObject temp = Instantiate (objBossSkill) as GameObject;
				temp.AddComponent<GuideSkillIcon> ().SetIcon (bossInfo.acActiveSkills [i].cBuff, false);
				temp.transform.SetParent (trBossSkills);
				temp.transform.localScale = Vector3.one;
				temp.transform.localPosition = Vector3.zero;

				RectTransform pos = temp.GetComponent<RectTransform> ();
				string desc = TextManager.Instance.GetText(bossInfo.acActiveSkills [i].cBuff.sDescription);
				temp.GetComponent<Button>().onClick.AddListener(() => { SetConditionDesc(pos, desc); }); 
			}
			if (bossInfo.acActiveSkills [i].cDebuff != null) {
				GameObject temp = Instantiate (objBossSkill) as GameObject;
				temp.AddComponent<GuideSkillIcon> ().SetIcon (bossInfo.acActiveSkills [i].cDebuff, false);
				temp.transform.SetParent (trBossSkills);
				temp.transform.localScale = Vector3.one;
				temp.transform.localPosition = Vector3.zero;

				RectTransform pos = temp.GetComponent<RectTransform> ();
				string desc = TextManager.Instance.GetText(bossInfo.acActiveSkills [i].cDebuff.sDescription);
				temp.GetComponent<Button>().onClick.AddListener(() => { SetConditionDesc(pos, desc); }); 
			}
		}
		for (int i = 0; i < bossInfo.acPassiveSkills.Count; i++) {
			if (bossInfo.acPassiveSkills [i].cBuff != null) {
				GameObject temp = Instantiate (objBossSkill) as GameObject;
				temp.AddComponent<GuideSkillIcon> ().SetIcon (bossInfo.acPassiveSkills [i].cBuff, false);
				temp.transform.SetParent (trBossSkills);
				temp.transform.localScale = Vector3.one;
				temp.transform.localPosition = Vector3.zero;

				RectTransform pos = temp.GetComponent<RectTransform> ();
				string desc = TextManager.Instance.GetText(bossInfo.acPassiveSkills [i].cBuff.sDescription);
				temp.GetComponent<Button>().onClick.AddListener(() => { SetConditionDesc(pos, desc); }); 
			}
			if (bossInfo.acPassiveSkills [i].cDebuff != null) {
				GameObject temp = Instantiate (objBossSkill) as GameObject;
				temp.AddComponent<GuideSkillIcon> ().SetIcon (bossInfo.acPassiveSkills [i].cDebuff, false);
				temp.transform.SetParent (trBossSkills);
				temp.transform.localScale = Vector3.one;
				temp.transform.localPosition = Vector3.zero;

				RectTransform pos = temp.GetComponent<RectTransform> ();
				string desc = TextManager.Instance.GetText(bossInfo.acPassiveSkills [i].cDebuff.sDescription);
				temp.GetComponent<Button>().onClick.AddListener(() => { SetConditionDesc(pos, desc); }); 
			}
		}

		SetUserSkill ();
	}

	bool bAlreadySkillSetting = true;

	public void SetUserSkill(){
		for (int i = 0; i < trUserSkills.childCount; i++) {
			Destroy (trUserSkills.GetChild (i).gameObject);
		}

		List<UInt16> skillConditions = new List<UInt16> ();

		for(int i=0; i<Legion.Instance.SelectedCrew.acLocation.Length; i++){
			if(Legion.Instance.SelectedCrew.acLocation [i] != null){
				Hero hero = (Hero)Legion.Instance.SelectedCrew.acLocation [i];
				SkillComponent temp = hero.GetComponent<SkillComponent> ();
				for (int j=0; j<temp.lstLearnInfo.Count; j++) {
					if (temp.lstLearnInfo [j].u1UseIndex == 0 || temp.lstLearnInfo [j].u1UseIndex > 6)
						continue;
					
					if (temp.dicSkillInfo [temp.lstLearnInfo [j].u1SlotNum].cSkill.cInfo.cBuff != null) {
						if (!skillConditions.Contains (temp.dicSkillInfo [temp.lstLearnInfo [j].u1SlotNum].cSkill.cInfo.cBuff.u2Group)) {
							skillConditions.Add (temp.dicSkillInfo [temp.lstLearnInfo [j].u1SlotNum].cSkill.cInfo.cBuff.u2Group);
						}
					}
					if (temp.dicSkillInfo [temp.lstLearnInfo [j].u1SlotNum].cSkill.cInfo.cDebuff != null) {
						if (!skillConditions.Contains (temp.dicSkillInfo [temp.lstLearnInfo [j].u1SlotNum].cSkill.cInfo.cDebuff.u2Group)) {
							skillConditions.Add (temp.dicSkillInfo [temp.lstLearnInfo [j].u1SlotNum].cSkill.cInfo.cDebuff.u2Group);
						}
					}
				}
			}
		}

		RecomCondis.Clear();

		for (int i = 0; i < guideInfo.au2RecomSkill.Length; i++) {
			if (guideInfo.au2RecomSkill [i] > 0) {
				RecomCondis.Add (guideInfo.au2RecomSkill [i]);

				ConditionInfo cInfo = ConditionInfoMgr.Instance.GetInfo (guideInfo.au2RecomSkill [i]);

				GameObject temp = Instantiate (objUserSkill) as GameObject;
				temp.transform.SetParent (trUserSkills);
				temp.transform.localScale = Vector3.one;
				temp.transform.localPosition = Vector3.zero;
				if (skillConditions.FindIndex (cs => cs == guideInfo.au2RecomSkill [i]) < 0) {
					bAlreadySkillSetting = false;
					temp.AddComponent<GuideSkillIcon> ().SetIcon (cInfo, true, false);
					RectTransform pos = temp.GetComponent<RectTransform> ();
					string desc = TextManager.Instance.GetText(cInfo.sDescription);
					temp.GetComponent<Button>().onClick.AddListener(() => { SetConditionDesc(pos, desc); }); 
				} else {
					temp.AddComponent<GuideSkillIcon> ().SetIcon (cInfo, true, true);
					RectTransform pos = temp.GetComponent<RectTransform> ();
					string desc = TextManager.Instance.GetText(cInfo.sDescription);
					temp.GetComponent<Button>().onClick.AddListener(() => { SetConditionDesc(pos, desc); }); 
				}
			}
		}
	}

	public void OnClickClose()
	{
		gameObject.SetActive (false);
	}

	void SetConditionDesc(RectTransform pos, string text)
	{
		objCondiDesc.SetActive (true);
		trCondiBG.position = pos.position;
		trCondiBG.anchoredPosition += new Vector2 (0, -80f);
		txtCondiDesc.text = text;
	}

	public void OnClickDesc()
	{
		objCondiDesc.SetActive (false);
	}

	public void OnClickGoToSkill()
	{
		FadeEffectMgr.Instance.QuickChangeScene(MENU.CHARACTER_INFO, (int)POPUP_CHARACTER_INFO.SKILL);
	}

	public void OnClickAutoStat()
	{
		if (!CheckRemainStat ()) {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_guide_stat_title"), TextManager.Instance.GetText("popup_guide_stat_enough"),null);
			return;
		}

        object[] yesEventParam = new object[1];
		PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("popup_guide_stat_title"), TextManager.Instance.GetText("popup_desc_stat_auto_char"),
            AddAutoStatus, yesEventParam);
	}

	bool CheckRemainStat()
	{
		for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
		{
			if(Legion.Instance.cBestCrew.acLocation[i] == null)
				continue;
            ((Hero)Legion.Instance.cBestCrew.acLocation [i]).GetComponent<StatusComponent>().CheckHaveStatPoint(((Hero)Legion.Instance.cBestCrew.acLocation [i]).GetComponent<LevelComponent>().cLevel.u2Level);
			Byte remain = ((Hero)Legion.Instance.cBestCrew.acLocation [i]).GetComponent<StatusComponent> ().STAT_POINT;

			DebugMgr.LogError (remain);

			if(remain > 0)
			{
				return true;
			}
		}

		return false;
	}
    
    public void AddAutoStatus(object[] param)
    {
        int SelectedAutoStat = 0;
        lstHero = new List<Hero>();
        lstAddPoint = new List<UInt16[]>();
        _statusAddPoint = new UInt16[Server.ConstDef.CharStatPointType];

        _totalUasedStatPoint = 0;
        for(int i=0; i<Crew.MAX_CHAR_IN_CREW; i++)
        {
            if(Legion.Instance.cBestCrew.acLocation[i] == null)
                continue;
            cHero = (Hero)Legion.Instance.cBestCrew.acLocation[i];

			StatusComponent sComp = cHero.GetComponent<StatusComponent> ();

            if(sComp.STAT_POINT == 0)
                continue;
            _statusAddPoint = new UInt16[Server.ConstDef.CharStatPointType];
            for (int j=sComp.STAT_POINT; j>0;)
            {
                for(int k=0; k<ClassInfo.MAX_CHARACTER_AUTO_STATUS; k++)
                {
                    if(j>0)
                        j--;
                    else
                        break;
                    SelectedAutoStat = (int)(cHero.cClass.au1AutoStat[k]);
                    _statusAddPoint[SelectedAutoStat-1]++;
                    
                    sComp.STAT_POINT--;
                    sComp.USE_POINT++;
                    _totalUasedStatPoint++;
                }
            }
            lstHero.Add(cHero);
            lstAddPoint.Add(_statusAddPoint);
        }
        PopupManager.Instance.ShowLoadingPopup(1);
        StartCoroutine(RequestAddStatPoint());
    }
    IEnumerator RequestAddStatPoint()
    {
        yield return new WaitForEndOfFrame();
        Server.ServerMgr.Instance.PointCharacterStatus(lstHero.ToArray(), lstAddPoint.ToArray(), RequestSendStatPoint);
    }

    public void RequestSendStatPoint(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CHAR_STAT_POINT, err), Server.ServerMgr.Instance.CallClear);
			return;
        }

        else if(err == Server.ERROR_ID.NONE)
        {
			Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.CharStatPoint, 0, 0, 0, 0, (UInt16)_totalUasedStatPoint);
        }
    }

	Dictionary<Byte, List<LearnedSkill>> lstAutoChangedSkill;

	public void OnClickAutoSkill()
	{
		if (bAlreadySkillSetting) {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_guide_skill_title"), TextManager.Instance.GetText("popup_guide_skill_enough"), null);
			return;
		}

		bool bNeed = false;

		for (int i = 0; i<Legion.Instance.SelectedCrew.acLocation.Length; i++) {
			if (Legion.Instance.SelectedCrew.acLocation [i] != null) {
				if (CheckRecomSkill ((Hero)Legion.Instance.SelectedCrew.acLocation [i])) {
					bNeed = true;
				}
			}
		}

		if (bNeed) {
			PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("popup_guide_skill_title"), TextManager.Instance.GetText("popup_guide_skill"), SubmitSkillSetting, null);
		} else {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_guide_skill_title"), TextManager.Instance.GetText("popup_guide_skill_enough"), null);
		}
	}

	bool CheckRecomSkill(Hero hero)
	{
		SkillComponent cHeroSkills = hero.GetComponent<SkillComponent>();
		List<LearnedSkill> lstLearnInfo = new List<LearnedSkill>();
		//List<LearnedSkill> lstChangedInfo = new List<LearnedSkill>();

		lstLearnInfo = cHeroSkills.lstLearnInfo;

		//bool result = false;

		Byte ActiveIdx = 0;

		for (int i=0; i<lstLearnInfo.Count; i++) {
			if(lstLearnInfo[i].u2Level > 0){
				SkillInfo tInfo = SkillInfoMgr.Instance.GetInfoBySlot(hero.cClass.u2ID, lstLearnInfo[i].u1SlotNum);
				if(tInfo.u1ActWay == 1 && CheckStageCondition(tInfo)){
					if(ActiveIdx > 5) continue;

					bool bLock = cHeroSkills.CheckLockSlot(ActiveIdx);

					if(bLock) continue;

					Byte before = lstLearnInfo[i].u1UseIndex;

					ActiveIdx++;

					if (before == 0) {
						return true;
					}

					if(before != ActiveIdx){
						return true;
					}
				}
			}
		}

		return false;
	}

	bool CheckStageCondition(SkillInfo sInfo)
	{
		if (sInfo.cBuff != null) {
			if (RecomCondis.FindIndex (cs => cs == sInfo.cBuff.u2Group) > -1) {
				return true;
			}
		}
		if (sInfo.cDebuff != null) {
			if (RecomCondis.FindIndex (cs => cs == sInfo.cDebuff.u2Group) > -1) {
				return true;
			}
		}

		return false;
	}

	void SkillAutoSetting(Hero hero)
	{
		SkillComponent cHeroSkills = hero.GetComponent<SkillComponent>();
		List<LearnedSkill> lstLearnInfo = new List<LearnedSkill>();
		List<LearnedSkill> lstChangedInfo = new List<LearnedSkill>();

		lstLearnInfo = cHeroSkills.lstLearnInfo;

		lstLearnInfo.Sort( delegate(LearnedSkill x, LearnedSkill y){
			int diff = y.u2Level.CompareTo(x.u2Level);
			if(diff != 0) return diff;
			else return y.u1SlotNum.CompareTo(x.u1SlotNum);
		});

		Byte ActiveIdx = 0;

		for (int i=0; i<lstLearnInfo.Count; i++) {
			if(lstLearnInfo[i].u2Level > 0){
				SkillInfo tInfo = SkillInfoMgr.Instance.GetInfoBySlot(hero.cClass.u2ID, lstLearnInfo[i].u1SlotNum);
				if(tInfo.u1ActWay == 1 && CheckStageCondition(tInfo)){
					if(ActiveIdx > 5) continue;

					bool bLock = cHeroSkills.CheckLockSlot(ActiveIdx);

					if(bLock) continue;

					Byte before = lstLearnInfo[i].u1UseIndex;

					ActiveIdx++;

					if(before != ActiveIdx){
						int idx = lstLearnInfo.FindIndex(cs => cs.u1UseIndex == ActiveIdx);
						lstLearnInfo[i].u1UseIndex = ActiveIdx;

						if (idx > -1) {
							lstLearnInfo [idx].u1UseIndex = before;

							int idx2 = lstChangedInfo.FindIndex(cs => cs.u1SlotNum == lstLearnInfo[idx].u1SlotNum);
							if(idx2 < 0)
								lstChangedInfo.Add(lstLearnInfo [idx]);
							else 
								lstChangedInfo[idx2].u1UseIndex = before;
						}

						int idx3 = lstChangedInfo.FindIndex(cs => cs.u1SlotNum == lstLearnInfo[i].u1SlotNum);
						if(idx3 < 0)
							lstChangedInfo.Add(lstLearnInfo[i]);
						else 
							lstChangedInfo[idx3].u1UseIndex = ActiveIdx;
					}
				}
			}
		}

		if (lstChangedInfo.Count > 0) {
			lstAutoChangedSkill.Add (hero.u1Index, lstChangedInfo);
		}
	}

	void SubmitSkillSetting(object[] param)
	{
		PopupManager.Instance.ShowLoadingPopup (1);

		lstAutoChangedSkill = new Dictionary<Byte, List<LearnedSkill>>();

		for (int i = 0; i<Legion.Instance.SelectedCrew.acLocation.Length; i++) {
			if (Legion.Instance.SelectedCrew.acLocation [i] != null) {
				SkillAutoSetting ((Hero)Legion.Instance.SelectedCrew.acLocation [i]);
			}
		}

		int idx = 0;
		Byte[] heros = new Byte[lstAutoChangedSkill.Count];

		foreach (Byte heroIndex in lstAutoChangedSkill.Keys) {
			heros [idx++] = heroIndex;
		}

		idx = 0;
		List<LearnedSkill>[] skills = new List<LearnedSkill>[lstAutoChangedSkill.Count];

		foreach (List<LearnedSkill> changedSkill in lstAutoChangedSkill.Values) {
			skills [idx++] = changedSkill;
		}
		Server.ServerMgr.Instance.ChangeSkill(heros, skills, ResultChange);
	}

	void ResultChange(Server.ERROR_ID err){

		PopupManager.Instance.CloseLoadingPopup();
		if (err == Server.ERROR_ID.NONE) {
			SetUserSkill ();
		} else {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CHAR_SKILL_CHANGE, err)+TextManager.Instance.GetText("popup_desc_server_error_critical"), Server.ServerMgr.Instance.ApplicationShutdown);
		}
	}

	int equipNeedHeroIndex = -1;
	int needEquipIndex = -1;

	public void OnClickAutoCraft()
	{
		for (int i = 0; i<Legion.Instance.SelectedCrew.acLocation.Length; i++) {
			if (Legion.Instance.SelectedCrew.acLocation [i] != null) {
				Hero hero = (Hero)Legion.Instance.SelectedCrew.acLocation [i];
				needEquipIndex = CheckNeedCraft (hero);
				if (needEquipIndex > 0) {
					equipNeedHeroIndex = i;
					PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("popup_guide_equip_title"), 
						String.Format(TextManager.Instance.GetText("popup_guide_equip"), hero.sName,  TextManager.Instance.GetText(((EquipmentInfo)hero.acEquips[needEquipIndex-1].cItemInfo).EquipTypeKey())),
						GoToAutoCraft, null);
					return;
				}
			}
		}

		PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_guide_equip_title"), 
			TextManager.Instance.GetText("popup_guide_equip_enough"), null);
	}

	void GoToAutoCraft(object[] param)
	{
		Legion.Instance.equipNeedHeroIndex = equipNeedHeroIndex;
		Legion.Instance.equipShortCut = needEquipIndex;
		FadeEffectMgr.Instance.QuickChangeScene(MENU.CHARACTER_INFO, (int)POPUP_CHARACTER_INFO.EQUIP_CREATE);
	}

	int CheckNeedCraft(Hero hero)
	{
		int have = 0;
		
		for (int i = 0; i < hero.acEquips.Length; i++) {
			if (hero.acEquips [i].u1SmithingLevel >= needPartsLevel) {
				have++;
			}
		}

		if (have < needPartsCount) {
			
			for (int i = 0; i < guideInfo.au2NeedParts.Length; i++) {
				if (guideInfo.au2NeedParts [i] > 0) {
					if (hero.acEquips [guideInfo.au2NeedParts [i] - 1].u1SmithingLevel < needPartsLevel) {
						return guideInfo.au2NeedParts [i];
					}
				}
			}
		}

		return 0;
	}
}

public class GuideSkillIcon : MonoBehaviour
{
	Image imgIcon;
	Text txtCondi;

	public void SetIcon(ConditionInfo info, bool bHero, bool bActive = false)
	{
		imgIcon = transform.GetComponent<Image> ();
		txtCondi = transform.FindChild ("TextName").GetComponent<Text> ();
		if (bHero) {
			imgIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Campaign/stage_icon_mini_03.hero_" + info.u2Group);
			if (!bActive)
				AtlasMgr.Instance.SetGrayScale (imgIcon);
		}else{
			imgIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Campaign/stage_icon_mini_03.boss_" + info.u2Group);
		}
		txtCondi.text = TextManager.Instance.GetText (info.sName);
	}
}
