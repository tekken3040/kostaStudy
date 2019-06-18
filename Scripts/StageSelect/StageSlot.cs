using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

//스테이지 슬롯
public class StageSlot : MonoBehaviour {

    public const float NOMAL_SLOT_SCALE = 0.85f;
    public const float MIDDLE_SLOT_SCALE = 0.97f;
    public const float BOSS_SLOT_SCALE = 1.1f;

    public Button button;
	public Image slotBG;
    public Image slotImg;
    public Image stageIcon;
	public Image stageMiniIllust;
	public Image bossMark;
	public Image stageNumBg;
	public GameObject[] stars;
	public Image flagImage;
	public Image flagNumber;
	public GameObject questMark;
	public Text stageName;
    public GameObject m_ImgQuestMark;

	public Vector3 starPos1;
	public Vector3[] starPos2;
	public Vector3[] starPos3; 
	
	[HideInInspector]
	public UInt16 stageID;
	
	private int stageIndex = 0;
    private Vector2 m_NamePos;
	private Vector2 m_EdgePos;
	public bool stageLock;
	private bool isGoingStage = false;

    void Awake()
    {
        m_NamePos = stageName.GetComponent<RectTransform>().anchoredPosition;
		m_EdgePos = slotBG.GetComponent<RectTransform>().anchoredPosition;
    }
	public void SetSlot(UInt16 stageID, int index)
	{
		this.stageID = stageID;
		StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[stageID];
		if(stageInfo.u1ForestElement == 0)
		{
			SetSlot(stageInfo, index);
		}
	}

	public void SetSlot(StageInfo stageInfo, int index)
	{
		this.stageID = stageInfo.u2ID;
		stageIndex = index;		

//		StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[stageID];
		ChapterInfo chapterInfo = stageInfo.chapterInfo;
		Byte difficult = Legion.Instance.SelectedDifficult;
		stageLock = true;

		questMark.SetActive(false);
        m_ImgQuestMark.SetActive(false);
        stageName.gameObject.SetActive(true);
        stageName.text = chapterInfo.u1Number.ToString() + "-" + stageInfo.u1StageNum;
		stageLock = !StageInfoMgr.Instance.IsOpen(stageID, difficult);

        stageName.GetComponent<RectTransform>().anchoredPosition = m_NamePos;

		// 해상도에 따른 슬롯 위치 조정용
		Vector2 ratio = Legion.Instance.ScreenRatio;
		GetComponent<RectTransform>().anchoredPosition = new Vector2(stageInfo.stagePosX * ratio.x, stageInfo.stagePosY * ratio.y);

		Byte ConcernEle = 1;
		switch (stageInfo.u1RecommandElement)
		{
		case 2:
			ConcernEle = 4;
			break;
		case 3:
			ConcernEle = 2;
			break;
		case 4:
			ConcernEle = 3;
			break;
		}

        slotImg.sprite = AtlasMgr.Instance.GetSprite("Sprites/Campaign/Campaign_03.stageslot_" + ConcernEle);

        //스테이지 오픈 여부에 따라서 표시가 달라진다
        if (stageInfo.u1BossType == 0)
        {
			slotBG.sprite = AtlasMgr.Instance.GetSprite("Sprites/Campaign/Campaign_03.stageslot_0_"+Legion.Instance.SelectedDifficult);		
			stageIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Campaign/Campaign_03.stageslot_element_"+ConcernEle);
            bossMark.gameObject.SetActive(false);
			stageMiniIllust.enabled = false;
            GetComponent<RectTransform>().localScale = new Vector3(NOMAL_SLOT_SCALE, NOMAL_SLOT_SCALE, 1.0f);

			stageNumBg.sprite = AtlasMgr.Instance.GetSprite("Sprites/Campaign/Campaign_03.stageslot_name_bg");
			stageNumBg.SetNativeSize ();
			stageNumBg.GetComponent<RectTransform>().anchoredPosition = new Vector2(m_NamePos.x, m_NamePos.y);
			stageName.GetComponent<RectTransform>().anchoredPosition = new Vector2(m_NamePos.x, m_NamePos.y);

			if(Legion.Instance.SelectedDifficult == 3){
				stageNumBg.GetComponent<RectTransform>().anchoredPosition = new Vector2(m_NamePos.x, m_NamePos.y - 7.0f);
				stageName.GetComponent<RectTransform>().anchoredPosition = new Vector2(m_NamePos.x, m_NamePos.y - 7.0f);
			}
			slotBG.GetComponent<RectTransform> ().anchoredPosition = m_EdgePos;
        }
		else
		{
            stageIcon.gameObject.SetActive(true);
			bossMark.gameObject.SetActive(true);
			stageMiniIllust.enabled = true;
			bossMark.sprite =  AtlasMgr.Instance.GetSprite("Sprites/Campaign/Campaign_03.stageslot_boss_" + stageInfo.u1BossType);
			bossMark.SetNativeSize();
			slotBG.sprite = AtlasMgr.Instance.GetSprite("Sprites/Campaign/Campaign_03.stageslot_boss_" + stageInfo.u1BossType +"_"+ConcernEle);
           
			stageMiniIllust.sprite = AtlasMgr.Instance.GetSprite("Sprites/Campaign/" + stageInfo.stageMiniIconPath + "_on");
			stageIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Campaign/Campaign_03.stageslot_element_"+ConcernEle);

			if (stageInfo.u1BossType == 1)
			{
				GetComponent<RectTransform>().localScale = new Vector3(MIDDLE_SLOT_SCALE, MIDDLE_SLOT_SCALE, 1.0f);
				stageName.GetComponent<RectTransform>().localScale = new Vector3(NOMAL_SLOT_SCALE, NOMAL_SLOT_SCALE, 1.0f);
				stageNumBg.sprite = AtlasMgr.Instance.GetSprite("Sprites/Campaign/Campaign_03.stageslot_name_bg2");
				stageNumBg.GetComponent<RectTransform> ().localScale = new Vector3 (1f/ MIDDLE_SLOT_SCALE, 1f/ MIDDLE_SLOT_SCALE, 1.0f);
				stageNumBg.SetNativeSize ();
				stageNumBg.GetComponent<RectTransform>().anchoredPosition = new Vector2(m_NamePos.x, m_NamePos.y - 50.0f);
				stageName.GetComponent<RectTransform>().anchoredPosition = new Vector2(m_NamePos.x, m_NamePos.y - 50.0f);
				slotBG.GetComponent<RectTransform> ().anchoredPosition = m_EdgePos;
			}
			else
			{
				GetComponent<RectTransform>().localScale = new Vector3(BOSS_SLOT_SCALE, BOSS_SLOT_SCALE, 1.0f);
				stageNumBg.enabled = false;
				stageName.gameObject.SetActive(false);
				slotBG.GetComponent<RectTransform> ().anchoredPosition = new Vector2(m_EdgePos.x + 8, m_EdgePos.y);
			}
		}

        if (stageLock)
		{
			button.interactable = false;
			AtlasMgr.Instance.SetGrayScale(stageIcon);
			AtlasMgr.Instance.SetGrayScale(slotBG);
			AtlasMgr.Instance.SetGrayScale(bossMark);
			AtlasMgr.Instance.SetGrayScale(stageMiniIllust);
		}
		else
		{
			button.interactable = true;
			AtlasMgr.Instance.SetDefaultShader(stageIcon);
			AtlasMgr.Instance.SetDefaultShader(slotBG);
			AtlasMgr.Instance.SetDefaultShader(bossMark);
			AtlasMgr.Instance.SetDefaultShader(stageMiniIllust);
		}
		slotBG.SetNativeSize();


        //별 표시 처리
		if(!StageInfoMgr.Instance.IsClear(stageID, Legion.Instance.SelectedDifficult))
		{
			for(int i=0; i<stars.Length; i++)
			{
				stars[i].SetActive(false);
			}	
		}
		else
		{
			int star = StageInfoMgr.Instance.GetStageClearStar(stageID);
			for(int i=0; i<stars.Length; i++)
			{
				if(i < star)
					stars[i].SetActive(true);
				else
					stars[i].SetActive(false);
			}
		}
		SetStarScalePos(stageInfo.u1BossType , StageInfoMgr.Instance.GetStageClearStar(stageID));

		SetDispatchICon();

		stageName.text = chapterInfo.u1Number.ToString() + "-" + stageInfo.u1StageNum;
		if (gameObject.GetComponent<TutorialButton> () == null) {
			gameObject.AddComponent<TutorialButton> ().id = "Stage" + chapterInfo.u1Number + "_" + stageInfo.u1StageNum;
		}
        
        if (m_ImgQuestMark.activeSelf == false)
        {
			if(Legion.Instance.cQuest.CheckQuestAlarm(MENU.CAMPAIGN, 0))
            {
				bool bActive = Legion.Instance.cQuest.CheckQuestRelationInStage (stageInfo);
				setQuestMarkScale (stageInfo.u1BossType, bActive);
            }// if(Legion.Instance.cQuest.CheckQuestAlarm(MENU.CAMPAIGN, 0)) end
        }
//        setQuestMarkScale(stageInfo.u1BossType,true);
	}

	public void SetSlotForest(StageInfo stageInfo, int index, Byte openElement, Byte chapterIndex)
	{
		this.stageID = stageInfo.u2ID;
		stageIndex = index;		

//		StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[stageID];
		ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[stageInfo.u2ChapterID];
		stageLock = true;

		questMark.SetActive(false);
        if(m_ImgQuestMark != null) m_ImgQuestMark.SetActive(false);

		stageLock = !StageInfoMgr.Instance.IsOpenForest(stageID, Legion.Instance.SelectedDifficult, openElement);
		stageIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Campaign/" + stageInfo.stageMiniIconPath + "_on");

		int slotIndex = ((chapterIndex - 1 ) * ForestElementLine.MAX_FOREST_STAGE_COUNT) + stageInfo.u1StageNum;
		stageName.text = slotIndex.ToString();//stageInfo.u1StageNum.ToString();
		//스테이지 오픈 여부에 따라서 표시가 달라진다
		if(stageLock)
		{
			button.interactable = false;
			if(stageInfo.u1BossType == 0)
				slotBG.sprite = AtlasMgr.Instance.GetSprite("Sprites/Campaign/Forest_001.forest_slot_disable");
			else
				slotBG.sprite = AtlasMgr.Instance.GetSprite("Sprites/Campaign/Forest_001.forest_slot_disable");

			AtlasMgr.Instance.SetGrayScale(stageIcon);		
		}
		else
		{
			button.interactable = true;
			if(stageInfo.u1BossType == 0)
				slotBG.sprite = AtlasMgr.Instance.GetSprite("Sprites/Campaign/Forest_001.forest_slot_" + stageInfo.u1ForestElement);
			else
				slotBG.sprite = AtlasMgr.Instance.GetSprite("Sprites/Campaign/Forest_001.forest_slot_" + stageInfo.u1ForestElement);				

			AtlasMgr.Instance.SetDefaultShader(stageIcon);
		}

		//별 표시 처리
		if(!stageInfo.IsClear(Legion.Instance.SelectedDifficult))
		{
			for(int i=0; i<stars.Length; i++)
			{
				stars[i].SetActive(false);
			}	
		}
		else
		{
			int star = StageInfoMgr.Instance.GetStageClearStar(stageID);
			for(int i=0; i<stars.Length; i++)
			{
				if(i < star)
					stars[i].SetActive(true);
				else
					stars[i].SetActive(false);
				
				if(stageLock)
					AtlasMgr.Instance.SetGrayScale(stars[i].GetComponent<Image>());
				else
					AtlasMgr.Instance.SetDefaultShader(stars[i].GetComponent<Image>());
			}

			SetStarScalePos(stageInfo.u1BossType , StageInfoMgr.Instance.GetStageClearStar(stageID));
		}

		SetDispatchICon();

		if (gameObject.GetComponent<TutorialButton> () == null) {
			gameObject.AddComponent<TutorialButton> ().id = "Stage" + chapterInfo.u1Number + "_" + stageInfo.u1StageNum;
		}

		if(!m_ImgQuestMark.activeSelf){
			if(Legion.Instance.cQuest.CheckQuestAlarm(MENU.CAMPAIGN, 0)){
				QuestInfo tempQuest = Legion.Instance.cQuest.CurrentQuest();
				if(index == 9 && tempQuest.u1QuestType == 20){
					if(chapterInfo.GetActInfo().u1Number == tempQuest.u1Delemiter1){
						if(tempQuest.u2QuestTypeID == chapterInfo.u2ID){
							if(tempQuest.u1Delemiter2 > 0){
								if(tempQuest.u1Delemiter2 == Legion.Instance.SelectedDifficult){
									m_ImgQuestMark.SetActive(true);
									return;
								}
							}else{
								m_ImgQuestMark.SetActive(true);
								return;
							}
						}
					}
				}
                else if(tempQuest.u1QuestType == 15)
                {
                    bool isChapterID = false;
                    ChapterInfo checkChapertInfo = null;
                    StageInfoMgr.Instance.dicChapterData.TryGetValue(tempQuest.u2QuestTypeID, out checkChapertInfo);
                    if(checkChapertInfo != null)
                    {
                        if (chapterInfo.u2ID != tempQuest.u2QuestTypeID)
                            return;

                        isChapterID = true;
                    }

					for(int i = 0; i < stageInfo.acPhases.Length; i++)
                    {
                        FieldInfo fieldInfo = StageInfoMgr.Instance.GetFieldInfo(stageInfo.acPhases[i].u2FieldID);
                        if (fieldInfo == null)
                            continue;

                        for (int j = 0; j < fieldInfo.acMonsterGroup.Length; j++)
                        {
							for(int k=0; k< fieldInfo.acMonsterGroup[j].acMonsterInfo.Length; k++)
                            {
								ClassInfo monsterInfo = ClassInfoMgr.Instance.GetInfo(fieldInfo.acMonsterGroup[j].acMonsterInfo[k].u2MonsterID);
                                if (monsterInfo == null)
                                    continue;

                                if (isChapterID == false  && tempQuest.u2QuestTypeID > 0 && monsterInfo.u2ID != tempQuest.u2QuestTypeID)
                                    continue;

                                if (tempQuest.u1Delemiter1 > 0 && monsterInfo.u1Element != tempQuest.u1Delemiter1)
                                    continue;

                                if (tempQuest.u1Delemiter2 > 0 && monsterInfo.u1MonsterType != tempQuest.u1Delemiter2)
                                    continue;

                                if (tempQuest.u1Delemiter3 > 0 && Legion.Instance.SelectedDifficult != tempQuest.u1Delemiter3)
                                    continue;

                                m_ImgQuestMark.SetActive(true);
                                return;
							}
						}
					}
				}else if(tempQuest.u1QuestType == 29){
					if(tempQuest.u2QuestTypeID > 0){
						if(stageInfo.CheckRewardInStage(tempQuest.u2QuestTypeID) > 0)
						{
							m_ImgQuestMark.SetActive (true);
							return;
						}
					}
				}else{
					Byte bossType = stageInfo.u1BossType;
					if(bossType < 2) bossType = 1;
					else if(bossType == 3) bossType = 2;
					if(tempQuest.u2QuestTypeID > 0){
						if(stageID == tempQuest.u2QuestTypeID){
							if((tempQuest.u1Delemiter1 == 0 || tempQuest.u1Delemiter1 == bossType)
								&& (tempQuest.u1Delemiter2 == 0 || tempQuest.u1Delemiter2 == Legion.Instance.SelectedDifficult)
								&& (tempQuest.u1Delemiter3 == 0 ||tempQuest.u1Delemiter3 == chapterInfo.GetActInfo().u1Number))
							{
								m_ImgQuestMark.SetActive(true);
								return;
							}
						}
					}else{
						if((tempQuest.u1Delemiter1 == 0 || tempQuest.u1Delemiter1 == bossType)
							&& (tempQuest.u1Delemiter2 == 0 || tempQuest.u1Delemiter2 == Legion.Instance.SelectedDifficult)
							&& (tempQuest.u1Delemiter3 == 0 ||tempQuest.u1Delemiter3 == chapterInfo.GetActInfo().u1Number))
						{
							m_ImgQuestMark.SetActive(true);
							return;
						}
					}
				}
			}
		}
	}

    public void setNewMarkActive(bool _bActive)
    {
        
    }

	private void setQuestMarkScale(Byte _nBossType ,bool _bActive )
	{
		//0.65 //0.75 //0.85
		float _scaleTo = 0.0f;
		switch (_nBossType)
		{
		case 0:
			m_ImgQuestMark.GetComponent<RectTransform>().localScale = new Vector3(1.0f / NOMAL_SLOT_SCALE, 1.0f / NOMAL_SLOT_SCALE, 1.0f);
			_scaleTo = (1.0f / 0.65f) * 1.3f;
			break;
		case 1:
			m_ImgQuestMark.GetComponent<RectTransform>().localScale = new Vector3(1.0f / MIDDLE_SLOT_SCALE, 1.0f / MIDDLE_SLOT_SCALE, 1.0f);
			_scaleTo = (1.0f / 0.75f) * 1.3f;
			break;
		case 2:
			m_ImgQuestMark.GetComponent<RectTransform>().localScale = new Vector3(1.0f / BOSS_SLOT_SCALE, 1.0f / BOSS_SLOT_SCALE, 1.0f);
			_scaleTo = (1.0f / 0.85f) * 1.3f;
			break;
		}

        LeanTween.cancel(m_ImgQuestMark);
        LeanTween.scale(m_ImgQuestMark.GetComponent<RectTransform>(), new Vector3(_scaleTo, _scaleTo, _scaleTo), 0.5f).setLoopPingPong(0);  
		m_ImgQuestMark.SetActive(_bActive);
	}

	private void SetStarScalePos(byte bossType , int count)
	{
		float scale = 0.0f;
		float fgap = 0.0f;
		float ygap = 0.0f;
		switch (bossType)
		{
		case 0: //normally
			scale = 1.0f / NOMAL_SLOT_SCALE;
			fgap = 7.0f;
			ygap = 10f;
			break;
		case 1: //Mid Boss
			scale = 1.0f / MIDDLE_SLOT_SCALE;
			fgap = 4.0f;
			ygap = 16f;
			break;
		case 2: //boss
			scale = 1.0f / BOSS_SLOT_SCALE;
			fgap = 0.0f;
			ygap = 20f;
			break;
		default://혹시하는 사태에 대비해 남겨둠
			scale = 1.0f;
			fgap = 0.0f;
			break;
		}

		for (int i = 0; i < stars.Length; i++)
		{
			stars[i].GetComponent<RectTransform>().localScale = new Vector3(scale, scale, 1);
		}

		switch(count)
		{
		case 1:
			stars[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(starPos1.x, starPos1.y+ygap);
			break;
		case 2:
			stars[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(starPos2[0].x -fgap , starPos2[0].y+ygap);
			stars[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(starPos2[1].x +fgap , starPos2[1].y+ygap);
			break;
		case 3:
			stars[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(starPos3[0].x -fgap , starPos3[0].y+ygap);
			stars[1].GetComponent<RectTransform>().anchoredPosition = new Vector2(starPos3[1].x, starPos3[1].y+ygap);
			stars[2].GetComponent<RectTransform>().anchoredPosition = new Vector2(starPos3[2].x +fgap , starPos3[2].y+ygap);
			break;
		}
	}

	public void SetDispatchICon()
	{
        int disPatchCrewIdx = Legion.Instance.CheckDispatch(stageID);
        if(disPatchCrewIdx >= 0)
        {
            flagImage.gameObject.SetActive(true);
            flagNumber.gameObject.SetActive(true);
            flagImage.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/flag_01.division_" + (disPatchCrewIdx + 1));
            flagNumber.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/flag_01.num_s_" + (disPatchCrewIdx + 1));
            flagNumber.SetNativeSize();
        }
        else
        {
            flagImage.gameObject.SetActive(false);
            flagNumber.gameObject.SetActive(false);
        }

        //flagImage.gameObject.SetActive(false);
        //flagNumber.gameObject.SetActive(false);
        //for (int i=0; i<Legion.Instance.acCrews.Length; i++)
		//{
        //    if (Legion.Instance.acCrews[i].DispatchStage != null
        //        && Legion.Instance.acCrews[i].DispatchStage.u2ID == stageID
        //        && Legion.Instance.acCrews[i].StageDifficulty == Legion.Instance.SelectedDifficult)
		//	{
		//		flagImage.gameObject.SetActive(true);
		//		flagNumber.gameObject.SetActive(true);
		//		flagImage.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/flag_01.division_" +(i + 1));
		//		flagNumber.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/flag_01.num_s_" + (i + 1));
		//		flagNumber.SetNativeSize();
		//	}
		//}
	}
}
