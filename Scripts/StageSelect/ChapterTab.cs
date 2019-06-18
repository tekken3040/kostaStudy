using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

// 챕터와 스테이지 정보를 보여준다.
public class ChapterTab : MonoBehaviour {

	public const int MIN_PAGE = 2;

	public RectTransform popupParent;
	public RectTransform pageContent;

	public GameObject leftArrow;
	public GameObject rightArrow;
	public DifficultButton difficultButton;
	public Animator TopMenuAniContorller;
	public CanvasGroup SideBtnCanvas;
	public FadeEffectText m_cFedeEffectText;

    // 반복 보상
    public RepeatReward cRepeatReward;
    public RepeatReward cStarReward;

	protected UInt16 actID;
	protected int currentPage;
	//protected int openPage;
	protected int maxPage;

	protected bool init = false;
		
	public StageInfoWindow stageInfoWindow;
	public StageGuideWindow stageGuideWindow;
	protected DispatchWindow dispatchWindow;
	private SweepWindow sweepWindow;
	protected int dispatchCrewIdx;
	protected float pageSizeX;
	protected float pageSizeY;

	protected List<ChapterPage> m_ChapterList;
	protected int m_nCurrent;
	protected Vector3 m_vCurrentStartPos;
	protected Vector3 m_vOtherDestPos;
    protected float m_fTime;
	protected bool lerp;
	protected List<Vector3> m_RewardOriginalPosList;

	public Dropdown _ChapterDropdown;
	private int _iDropdownListIndex;

	public virtual void Awake()
	{
		m_nCurrent = 0;
		m_fTime = 0.0f;
		pageSizeX = 0f;
		pageSizeY = 0f;

		m_ChapterList = new List<ChapterPage>();
		m_RewardOriginalPosList = new List<Vector3>();
		if(cRepeatReward != null)
			m_RewardOriginalPosList.Add(cRepeatReward.GetComponent<RectTransform>().anchoredPosition3D);
		if(cStarReward != null)
			m_RewardOriginalPosList.Add(cStarReward.GetComponent<RectTransform>().anchoredPosition3D);
	}

	// 페이지 생성
	public virtual void SetChapterPage(UInt16 actID)
	{
		bool bInit = init;
		if(init == false)
		{
			//페이지 생성 - 스테이지 목록
			this.actID = actID;
			ActInfo actInfo = StageInfoMgr.Instance.dicActData[actID];
			maxPage = actInfo.lstChapterID.Count;

			GameObject pageItem = AssetMgr.Instance.AssetLoad("Prefabs/UI/Campaign/Page.prefab", typeof(GameObject)) as GameObject;
			RectTransform pageTransform = pageItem.GetComponent<RectTransform>();
			pageSizeX = Legion.BaseScreenSizeX;
			pageSizeY = Legion.BaseScreenSizeY;

			// 최소 페이지 만큼 생성하고 챕터가 그보다 적으면 챕터 수만큼 생성
			int makePage = (maxPage < MIN_PAGE) ? maxPage : MIN_PAGE;
			for(int i = 0; i < makePage; ++i)
			{
				GameObject instPage = Instantiate(pageItem);
				instPage.name = "Page" + i.ToString();

				pageTransform = instPage.GetComponent<RectTransform>();
				pageTransform.SetParent(pageContent);
				pageTransform.SetSiblingIndex(i);
				pageTransform.localScale = Vector3.one;
				pageTransform.sizeDelta = Vector2.zero;
				pageTransform.anchoredPosition3D = new Vector3(i * pageSizeX, 0f, 0f);

				ChapterPage chapterPage = instPage.GetComponent<ChapterPage>();
				chapterPage.onClickStage = OnClickStage;

				instPage.SetActive(false);
				m_ChapterList.Add(chapterPage);
			}
			init = true;
			SetDropDownList();
		}
		RefreshAct(actID, bInit);
	}

    // 액트 정보 설정
	protected virtual void RefreshAct(UInt16 actID, bool bInit = false)
	{
		this.actID = actID;
		ActInfo actInfo = StageInfoMgr.Instance.dicActData[actID];
		maxPage = actInfo.lstChapterID.Count;
		currentPage = 0;

		CheckDifficult();
		if (!Legion.Instance.cTutorial.bIng)
			CheckLastChapter();

        SetPage(actInfo.lstChapterID[currentPage]);

		// 캠페인 오픈 효과
		StartCoroutine("CampaignOpenEffect");
	}
    
	protected virtual void CheckLastChapter()
    {
        ActInfo actInfo = StageInfoMgr.Instance.dicActData[actID];
        
        UInt16 lastChapter = actInfo.lstChapterID[0];
        UInt16 lastStage = StageInfoMgr.Instance.dicChapterData[lastChapter].lstStageID[0];
        
        currentPage = 0;
		for(int i = 0; i < maxPage; ++i)
		{
			ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[actInfo.lstChapterID[i]];
			int stageCount = chapterInfo.lstStageID.Count;
			for(int j = 0; j < stageCount; ++j)
			{
				int star = StageInfoMgr.Instance.GetStageClearStar(chapterInfo.lstStageID[j]);
				if(star > 0)
				{
					lastChapter = chapterInfo.u2ID;
					lastStage = chapterInfo.lstStageID[j];
					currentPage = i;
				}
				else
					break;
			}
		}
		// 챕터의 마지막 스테이지이고 최종 챕터가 아닐 경우 다음 챕터를 보여줌 
        if(StageInfoMgr.Instance.dicStageData[lastStage].IsLastStageInChapter
         && StageInfoMgr.Instance.GetStageClearStar(lastStage) > 0
         && StageInfoMgr.Instance.dicChapterData[lastChapter].u1Number < maxPage)
        {
            currentPage = StageInfoMgr.Instance.dicChapterData[lastChapter].u1Number;
        }
    }
	
    // 챕터 정보 갱신 
	protected virtual void RefreshChapter()
	{
		if(currentPage == 0)
			leftArrow.SetActive(false);
		else
			leftArrow.SetActive(true);

		if(currentPage >= maxPage-1)
			rightArrow.SetActive(false);
		else
			rightArrow.SetActive(true);
		
		if(init == false)
			return;

		UInt16 chapterID = StageInfoMgr.Instance.dicActData[actID].lstChapterID[currentPage];
		m_ChapterList[m_nCurrent].gameObject.SetActive(true);
		m_ChapterList[m_nCurrent].SetStage(StageInfoMgr.Instance.dicActData[actID].lstChapterID[currentPage]);
		SetChapterName(chapterID);

        // 반복보상, 별보상 갱신
        SetChapterRewardInfo();
        //SetRepeatRewradInfo();
		//SetStarRewardInfo();
	}
	
    // 화살표 클릭 처리
	//TODO::moonshine chapterpage
	public void OnClickNext()
	{
        if(currentPage >= maxPage - 1 || lerp) 
			return;

		currentPage++;

		m_nCurrent = m_nCurrent == 0 ? 1 : 0;
        RefreshChapter();
		SetMoveDirection(true);

		lerp = true;
		StartCoroutine("PageChange"); // 업데이트 대신 코루틴으로 대체
	}

	public void OnClickPrev()
	{
        if(currentPage < 1 || lerp) 
			return;   

		currentPage--;

		m_nCurrent = m_nCurrent == 0 ? 1: 0;
        RefreshChapter();
		SetMoveDirection(false);

		lerp = true;
		StartCoroutine("PageChange"); // 업데이트 대신 코루틴으로 대체
	}

	// moonshine 16.06.27
	protected virtual void SetMoveDirection(bool _bNext)
	{
		int nType;
		if (_bNext)
			nType = m_ChapterList[m_nCurrent].Direction;
		else
		{
			nType = m_ChapterList[m_nCurrent == 0 ? 1 : 0].Direction;
			//방향반전
			switch (nType)
			{
				case 1: nType = 2;  break;// top
				case 2: nType = 1;  break;// bottom
				case 3: nType = 4;  break;// left 
				case 4: nType = 3;  break;// right
			}
		}

		// 파티클 연출 생성
		CreateMoveCloudEffect(nType);
		switch(nType)
		{
			case 1: // top
				{
					m_vCurrentStartPos.Set(0, -pageSizeY , 0);
					m_vOtherDestPos.Set(0, pageSizeY , 0);
				}
				break;	
			case 2: // bottom
				{
					m_vCurrentStartPos.Set(0, pageSizeY , 0);
					m_vOtherDestPos.Set(0, -pageSizeY , 0);
				}
				break;
			case 3: // left 
				{
                    m_vCurrentStartPos.Set( -pageSizeX ,0, 0);
					m_vOtherDestPos.Set( pageSizeX ,0, 0);
				}
				break;
			case 4: // right
				{
					m_vCurrentStartPos.Set(pageSizeX ,0, 0);
					m_vOtherDestPos.Set(-pageSizeX ,0, 0);
				}
				break;
		}

        m_ChapterList[m_nCurrent].GetComponent<RectTransform>().anchoredPosition = m_vCurrentStartPos;
	}

    //해당 챕터 페이지로 설정(바로가기 할때 사용)
    public virtual void SetPage(UInt16 chapterID)
    {   
        lerp = false;
        currentPage = 0;
        
		int page = StageInfoMgr.Instance.dicActData[actID].lstChapterID.FindIndex((x) => x == chapterID);       
        currentPage = page;
		RefreshChapter();
    }

	// 2016. 7. 6 jy
	// 페이지 변경
	protected virtual IEnumerator PageChange()
	{
		StartPageMove();
		m_fTime = 0.0f;
		while(true)
		{
			m_fTime += Time.deltaTime;
			for (int i = 0; i < 2; i++)
			{
				if (i == m_nCurrent)
					m_ChapterList[i].GetComponent<RectTransform>().anchoredPosition = Vector3.Slerp(m_vCurrentStartPos, Vector3.zero, m_fTime * 3.3f);                    
				else
					m_ChapterList[i].GetComponent<RectTransform>().anchoredPosition = Vector3.Slerp(Vector3.zero, m_vOtherDestPos, m_fTime * 3.3f);
			}
			if (Vector3.Distance(m_ChapterList[m_nCurrent].GetComponent<RectTransform>().anchoredPosition, Vector3.zero) < 1.0f)
			{
				lerp = false;
				for (int i = 0; i < 2; i++)
				{
					if (i == m_nCurrent)
						m_ChapterList[i].GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
					else
						m_ChapterList[i].GetComponent<RectTransform>().anchoredPosition = m_vOtherDestPos;
				}
				StartCoroutine("PageMoveComplete");
				yield break;
			}
			yield return null;
		}
	}
    
    //열린 난이도 확인
	protected virtual void CheckDifficult()
    {
		Legion.Instance.SelectedDifficult = (Byte)Server.ConstDef.MaxDifficult;
		while(!StageInfoMgr.Instance.CheckUnlockDifficult(Legion.Instance.SelectedDifficult, actID))
        {
            Legion.Instance.SelectedDifficult -= 1;            
        }
		difficultButton.SetDifficultButton(Legion.Instance.SelectedDifficult);
    }

    //챕터나 액트가 바뀌었을 경우 난이도 정보를 갱신
	public virtual void RefreshDifficult(int difficult)
	{
        if(!init)
            return;

		if(StageInfoMgr.Instance.CheckUnlockDifficult(difficult, actID))
		{
			Legion.Instance.SelectedDifficult = (Byte)difficult;
			difficultButton.SetDifficultButton(difficult);
			RefreshChapter();
		}
	}
	// 2016. 08. 05 jy
	// 난이도 변경 원버튼 방식 변경
	public void OnClickDifficult()
	{
		// 난이도 버튼 클릭시 현재 선택된 난이도를 받아
		// 최대 난이도가 아니면 난이도를 증가하고 최대 난이도면 최소 난이도로 변경
		int difficult = Legion.Instance.SelectedDifficult;
		if(difficult < Server.ConstDef.MaxDifficult)
			++difficult;
		else
			difficult = Server.ConstDef.MinDifficult;

		// 현재 변경할 난이도가 오픈되어 있는지 확인한다
		bool isUnLock = StageInfoMgr.Instance.CheckUnlockDifficult(difficult, actID);
		if(isUnLock == false && difficult == 2)
		{
			// 어려움 난이도가 오픈되지 않았다면 메시지 창을 띄운다
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetText("popup_desc_no_diff_desc"), null);
			return;
		}
		else if(isUnLock == false)
		{
			// 헬 난이도가 오픈되지 안았다면 최하 난이도로 변경한다
			difficult = Server.ConstDef.MinDifficult;
		}

		Legion.Instance.SelectedDifficult = (Byte)difficult;
		difficultButton.SetDifficultButton(difficult);

		//2016. 09. 02 jy
		// 난이도 변경시 해당 난이도 클리어 할 스테이지 페이지로 이동 
		if (!Legion.Instance.cTutorial.bIng)
			CheckLastChapter();
		
		CreateMoveCloudEffect();
		HideRewardBtn();

		RefreshChapter();

		StopCoroutine("PageEffect");
		StartCoroutine("PageEffect");
	}

    // 스테이지 클릭 처리
	protected virtual void OnClickStage(int index)
	{
        //해당 스테이지에 파견중이 크루가 있으면 파견 정보창을 띄워 준다
        // 2016. 07.04 jy 
        // 스테이지 클릭시 파견 여부가 ID로만 체크하여 난이도가 변경되어도 같은 스테이지는 파견이 보내져 있어서
        // 파견 보내진 스테이지와 현재 선택된 스테이지 난이도도 체크하도록 함
        int disPatchCrewIdx = Legion.Instance.CheckDispatch(m_ChapterList[m_nCurrent].stageSlots[index].stageID);
        if(disPatchCrewIdx >= 0)
        {
            Legion.Instance.u2SelectStageID = m_ChapterList[m_nCurrent].stageSlots[index].stageID;
            OnClickDispatch(disPatchCrewIdx);
            return;
        }
        
		//for(int i=0; i<Legion.Instance.acCrews.Length; i++)
		//{
		//	if(Legion.Instance.acCrews[i].DispatchStage != null && 
		//		Legion.Instance.acCrews[i].DispatchStage.u2ID == m_ChapterList[m_nCurrent].stageSlots[index].stageID &&
		//		Legion.Instance.acCrews[i].StageDifficulty == Legion.Instance.SelectedDifficult)
		//	{
		//		Legion.Instance.u2SelectStageID = m_ChapterList[m_nCurrent].stageSlots[index].stageID;
		//		OnClickDispatch(i);
		//		return;
		//	}
		//}
		
        // 스테이지 정보창을 띄워 준다
		if(stageInfoWindow == null)
		{
			GameObject windowPref = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Campaign/StageInfoWindow.prefab", typeof(GameObject))) as GameObject;
			RectTransform transform = windowPref.GetComponent<RectTransform>();
			transform.SetParent(popupParent);
			transform.localScale = Vector3.one;
			transform.anchoredPosition3D = new Vector3(0f, 0f, -200f);
			transform.sizeDelta = Vector2.zero;
			transform.SetSiblingIndex(0);

			stageInfoWindow = windowPref.GetComponent<StageInfoWindow>();
			stageInfoWindow.onClickDispatch = OnClickDispatch;
			stageInfoWindow.onClickSweep = OnClickSweep;
			stageInfoWindow.onClickGuide = OnClickGuide;
		}
		
		stageInfoWindow.gameObject.SetActive(true);
		stageInfoWindow.SetInfo(m_ChapterList[m_nCurrent].stageSlots[index].stageID);
	}

	public void OnClickGuide()
	{
		StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[Legion.Instance.SelectedStage.u2ID];

		if(stageGuideWindow == null)
		{
			GameObject windowPref = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Campaign/StageGuideWindow.prefab", typeof(GameObject))) as GameObject;
			RectTransform transform = windowPref.GetComponent<RectTransform>();
			transform.SetParent(popupParent);
			transform.localScale = Vector3.one;
			transform.anchoredPosition3D = new Vector3(0f, 0f, -250f);
			transform.sizeDelta = Vector2.zero;

			stageGuideWindow = windowPref.GetComponent<StageGuideWindow> ();
		}		

		stageGuideWindow.SetPopup (stageInfo);
		stageGuideWindow.gameObject.SetActive(true);

		PopupManager.Instance.AddPopup (stageGuideWindow.gameObject, CloseGuide);
	}

	void CloseGuide()
	{
		stageGuideWindow.gameObject.SetActive(false);
		PopupManager.Instance.RemovePopup (stageGuideWindow.gameObject);
	}
    
    //바로가기 스테이지 정보창 처리
    public virtual void OpenStageInfo(UInt16 stageID)
    {
		for(int i=0; i<m_ChapterList[m_nCurrent].stageSlots.Length; i++)
        {
			if(m_ChapterList[m_nCurrent].stageSlots[i].stageID == stageID)
            {
                OnClickStage(i);
                break;
            }    
		}
    }
	
    //파견 정보창
	public virtual void OnClickDispatch(int crewIdx)
	{
        if(Legion.Instance.acCrews[crewIdx].u1Count == 0)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_stage_mark_dispatch"), TextManager.Instance.GetText("popup_char_set"), null);
            return;
        }
		if(dispatchWindow == null)
		{
			GameObject windowPref = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Campaign/DispatchWindow.prefab", typeof(GameObject))) as GameObject;
			RectTransform transform = windowPref.GetComponent<RectTransform>();
			transform.SetParent(popupParent);
			transform.localScale = Vector3.one;
			transform.anchoredPosition3D = new Vector3(0f, 0f, -150f);
			transform.sizeDelta = Vector2.zero;

			dispatchWindow = windowPref.GetComponent<DispatchWindow>();
			dispatchWindow.chapterTab = this;
		}		
		
        dispatchWindow.gameObject.SetActive(false);
        dispatchWindow.CloseCancelPopup();
        dispatchWindow.CloseDonePopup();

        //파견중이 아니면 파견 팝업
        if (Legion.Instance.acCrews[crewIdx].DispatchStage == null)
		{
			dispatchWindow.gameObject.SetActive(true);
			dispatchWindow.SetDispatchWindow(Legion.Instance.SelectedStage.u2ID, crewIdx);
		}
		else
		{
			TimeSpan timeSpan = Legion.Instance.acCrews[crewIdx].DispatchTime - Legion.Instance.ServerTime;
			
            //파견중이고 시간이 남았으면 파견 진행 정보를 보여줌
			if(timeSpan.Ticks > 0)
			{
				dispatchWindow.gameObject.SetActive(true);
				dispatchWindow.SetDispatchWindow(Legion.Instance.SelectedStage.u2ID, crewIdx);
			}
            // 완료 처리
			else
			{				
				if(!Legion.Instance.CheckEmptyInven())
				{
					return;
				}

				// 탑은 파견 보상은 챕터 정보에서 읽으며 따로 처리한다
				if(Legion.Instance.acCrews[crewIdx].DispatchStage.actInfo.u1Mode == ActInfo.ACT_TYPE.TOWER)
				{
					if( CheckTowerDispatchOverReward(Legion.Instance.acCrews[crewIdx]) == true)
						return;
				}
				else
				{
					// 2016. 10. 25 jy
					// 보상받기전 현재 금액이 최대치 인지 확인
					if(Legion.Instance.CheckGoodsLimitExcessx(Legion.Instance.SelectedStage.acGetGoods[Legion.Instance.SelectedDifficult -1].u1Type) == true)
					{
						Legion.Instance.ShowGoodsOverMessage(Legion.Instance.SelectedStage.acGetGoods[Legion.Instance.SelectedDifficult -1].u1Type);
						return;
					}
				}
				dispatchCrewIdx = crewIdx;
                PopupManager.Instance.ShowLoadingPopup(1);
				Server.ServerMgr.Instance.GetDispatchResult(Legion.Instance.acCrews[crewIdx], AckDispatchResult);	
			}					
		}
	}

	// 탑은 파견 보상은 챕터 정보에서 읽으며 따로 처리한다
	protected bool CheckTowerDispatchOverReward(Crew crew)
	{
		int rewardIndex = 0;
		// 모드가 쉬움이 아니라면 
		if(crew.StageDifficulty != 1)
			rewardIndex = (crew.StageDifficulty - 1) * 3;

		for(int i = rewardIndex; i < rewardIndex + 3; ++i)
		{
			if(Legion.Instance.CheckGoodsLimitExcessx(crew.DispatchStage.chapterInfo.acTowerRewards[i].u1Type))
				return true;
		}

		return false;
	}

	
    // 소탕 처리
	public virtual void OnClickSweep()
	{        
        // 별 3 클리어 아니면 불가능
       	if(StageInfoMgr.Instance.GetStageClearStar(Legion.Instance.u2SelectStageID) < 3)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_stage_clear"), TextManager.Instance.GetText("popup_desc_stage_need_star"), null);
            return; 
		}        
        
        StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[Legion.Instance.SelectedStage.u2ID];
        ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[stageInfo.u2ChapterID];
        
        //키 없으면 불가능 
        if(!Legion.Instance.CheckEnoughGoods(chapterInfo.GetConsumeGoods()))
		{
			PopupManager.Instance.ShowChargePopup(chapterInfo.GetConsumeGoods().u1Type);
			return;
		}
		        
		if(Legion.Instance.cInventory.dicItemKey.ContainsKey(Server.ConstDef.TICKET_SWEEP))
		{
			if(!Legion.Instance.CheckEmptyInven())
				return;

			// 소탕후 재화 확인
			if(Legion.Instance.CheckGoodsLimitExcessx(stageInfo.acGetGoods[Legion.Instance.SelectedDifficult-1].u1Type) == true)
			{
				Legion.Instance.ShowGoodsOverMessage(stageInfo.acGetGoods[Legion.Instance.SelectedDifficult-1].u1Type);
				return;
			}

			PopupManager.Instance.ShowLoadingPopup(1);
			Server.ServerMgr.Instance.SweepStage(Legion.Instance.SelectedStage, Legion.Instance.SelectedDifficult, AckSweepResult);
		}
        //소탕권 없으면 불가능
		else
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_dispatch_ticket_no"), TextManager.Instance.GetText("popup_desc_dispatch_ticket_no"), null);
		}
	}
	
    // 파견 요청
	public void RequestDispatch(StageInfo stageInfo, Crew crew, Byte stageDiffect)
	{
		if(stageInfo.GetActInfo().u1Mode == ActInfo.ACT_TYPE.FOREST)
		{
			if(StageInfoMgr.Instance.GetForestTicket() <= 0)
			{
				// 충전하시겠습니까?(팝업)
				if (stageInfoWindow == null) {
					OnClickStage ((StageInfoMgr.Instance.OpenForestElement-1)*10 + (stageInfo.u1StageNum-1));
					stageInfoWindow.gameObject.SetActive (false);
				}
				((ForestStageInfoWindow)stageInfoWindow).OnClick_ChargeTicket();
				return;
			}
		}
		// 2016. 09. 13jy 
		// 시련의 탑은 별을 체크하지 않는다
		if(stageInfo.actInfo.u1Mode != ActInfo.ACT_TYPE.TOWER)
		{
	        // 별 3 클리어 아니면 불가능
			if(StageInfoMgr.Instance.GetStageClearStar(stageInfo.u2ID) < 3)
			{
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_stage_clear"), TextManager.Instance.GetText("popup_desc_stage_need_star"), null);
	            return; 
			}
		}
        
        ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[stageInfo.u2ChapterID];
        
        // 키 없으면 불가능
        if(!Legion.Instance.CheckEnoughGoods(chapterInfo.GetConsumeGoods()))
		{
			PopupManager.Instance.ShowChargePopup(chapterInfo.GetConsumeGoods().u1Type);
			return;
		}
		
        // 선택된 크루는 불가능
        if(crew == Legion.Instance.cBestCrew)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_dispatch_wrong"), TextManager.Instance.GetText("popup_desc_dispatch_wrong"), null);
		}
		else
		{
			PopupManager.Instance.ShowLoadingPopup(1);
			Server.ServerMgr.Instance.DispatchStage(crew, stageInfo, stageDiffect, AckRequestDispatch);
		}
	}
	
	public void DispatchCancel(Crew crew)
	{
		PopupManager.Instance.ShowLoadingPopup(1);	
		dispatchCrewIdx = crew.u1Index - 1;
		Server.ServerMgr.Instance.CancelDispatchStage(crew, AckCancelDispatch);
	}
	
	public void DispatchDone(Crew crew, UInt32 doneCostCount)
	{
		if(!Legion.Instance.CheckEmptyInven())
			return;

		if(crew.DispatchStage.actInfo.u1Mode == ActInfo.ACT_TYPE.TOWER)
		{
			// 탑은 파견 보상은 챕터 정보에서 읽으며 따로 처리한다
			if( CheckTowerDispatchOverReward(crew) == true)
				return;
		}
		else
		{
			// 재화 오버 체크
			if(Legion.Instance.CheckGoodsLimitExcessx(crew.DispatchStage.acGetGoods[crew.StageDifficulty-1].u1Type) == true)
			{
				Legion.Instance.ShowGoodsOverMessage(crew.DispatchStage.acGetGoods[crew.StageDifficulty-1].u1Type);
				return;
			}
				
		}
		PopupManager.Instance.ShowLoadingPopup(1);	
		dispatchCrewIdx = crew.u1Index - 1;
		Server.ServerMgr.Instance.FinishDispatchStage(crew, doneCostCount,  AckFinishDispatch);
	}

    public void SetChapterRewardInfo()
    {
        UInt16 chapterID = StageInfoMgr.Instance.dicActData[actID].lstChapterID[currentPage];
        Byte difficult = Legion.Instance.SelectedDifficult;

        // 반복보상 셋팅
        if (cRepeatReward != null)
            cRepeatReward.SetRewardInfo(chapterID, difficult);

        // 별 보상 셋팅
        if (cStarReward != null)
            cStarReward.SetRewardInfo(chapterID, difficult);
    }

    /*
    // 반복 보상 정보
    public void SetRepeatRewradInfo()
	{
		if(cRepeatReward == null)
			return;
    
        UInt16 chapterID = StageInfoMgr.Instance.dicActData[actID].lstChapterID[currentPage];
        Byte difficult = Legion.Instance.SelectedDifficult;
    
        cRepeatReward.SetRewardInfo(chapterID, difficult);
    }
    
	// 별 보상 정보
	public virtual void SetStarRewardInfo()
	{
		if(cStarReward == null)
			return;
    
        UInt16 chapterID = StageInfoMgr.Instance.dicActData[actID].lstChapterID[currentPage];
        Byte difficult = Legion.Instance.SelectedDifficult;
    
        cStarReward.SetRewardInfo(chapterID, difficult);
    }
    
    // 반복 보상 수령 클릭
	public void OnClickRepeatReward()
	{
		ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[StageInfoMgr.Instance.dicActData[actID].lstChapterID[currentPage]];
		Goods goods;
		// 초기 보상 수령이라면 
		if(chapterInfo.repeatType[Legion.Instance.SelectedDifficult-1] == 0)
			goods = chapterInfo.acFirstRewards[Legion.Instance.SelectedDifficult-1];
		else		
			goods = chapterInfo.acRepeatRewards[chapterInfo.repeatType[Legion.Instance.SelectedDifficult-1] - 1][Legion.Instance.SelectedDifficult-1];
		//보상을 받을시 재화 초과 여부를 체크하여 초과 하면 재화를 받지 않는다
		if(Legion.Instance.CheckGoodsLimitExcessx(goods.u1Type) == true)
		{
			Legion.Instance.ShowGoodsOverMessage(goods.u1Type);
			return;
		}

		PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.StageRepeatReward(StageInfoMgr.Instance.dicActData[actID].lstChapterID[currentPage], Legion.Instance.SelectedDifficult, AckRepeatReward);	
	}
		
    // 별 보상 수령 클릭
	public void OnClickStarReward()
	{
		ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[StageInfoMgr.Instance.dicActData[actID].lstChapterID[currentPage]];

		//보상을 받을시 재화 초과 여부를 체크하여 초과 하면 재화를 받지 않는다
		if(Legion.Instance.CheckGoodsLimitExcessx(chapterInfo.acStarRewards[Legion.Instance.SelectedDifficult-1].u1Type) == true)
		{
			Legion.Instance.ShowGoodsOverMessage(chapterInfo.acStarRewards[Legion.Instance.SelectedDifficult-1].u1Type);
			return;
		}

        PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.StageStarReward(chapterInfo.u2ID, Legion.Instance.SelectedDifficult, AckStarReward);
	}
    */
    protected void SetDropDownList()
	{
		if(_ChapterDropdown == null)
			return;
		
		if( _ChapterDropdown.options.Count > 0)
			_ChapterDropdown.ClearOptions();

		List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
		ActInfo actInfo = StageInfoMgr.Instance.dicActData[actID];
		for(UInt16 i = 0; i < actInfo.lstChapterID.Count; ++i)
		{
			ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[actInfo.lstChapterID[i]];
			options.Add(new Dropdown.OptionData(TextManager.Instance.GetText(chapterInfo.strShortcutName)));
		}
		_ChapterDropdown.options = options;
	}

	public void ChangeShortCutChapter(Int32 value)
	{
		if(currentPage == _ChapterDropdown.value)
			return;
		
		bool isNext = false;
		if(_iDropdownListIndex < _ChapterDropdown.value)
			isNext = true;

		currentPage = _ChapterDropdown.value;
		m_nCurrent = m_nCurrent == 0 ? 1: 0;
		RefreshChapter();
		SetMoveDirection(isNext);

		lerp = true;
		StartCoroutine("PageChange"); // 업데이트 대신 코루틴으로 대체
	}

	// =============================================== 서버 처리 함수 =================================================================//

	// 파견 통신 처리
	protected virtual void AckSweepResult(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.STAGE_SWEEP,err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
			StartCoroutine(SweepResult());
		}
	}

    IEnumerator SweepResult()
    {
		if(StageInfoMgr.Instance.u1EventIDCount > 0)
        {
            for(int i=0; i<StageInfoMgr.Instance.arrEventID.Length; i++)
            {
                if(!EventInfoMgr.Instance.dicEventReward.ContainsKey(StageInfoMgr.Instance.arrEventID[i]))
                {
                    StageInfoMgr.Instance.bReloadEvent = false;
                    PopupManager.Instance.ShowLoadingPopup(1);
                    Server.ServerMgr.Instance.RequestEventReload(EventReloadResult);
                    break;
                }
                else
                {
                    StageInfoMgr.Instance.bReloadEvent = true;
                    continue;
                }
            }
        }
        else
            StageInfoMgr.Instance.bReloadEvent = true;
		
        while(!StageInfoMgr.Instance.bReloadEvent)
        {
            yield return new WaitForEndOfFrame();
        }

		if(stageInfoWindow != null)
			stageInfoWindow.RefreshTicket();

		if(sweepWindow == null)
		{
			GameObject windowPref = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Campaign/SweepWindow.prefab", typeof(GameObject))) as GameObject;
			windowPref.transform.SetParent(popupParent);
			windowPref.transform.localScale = Vector3.one;
			windowPref.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, 0f, -150f);
			windowPref.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
			sweepWindow = windowPref.GetComponent<SweepWindow>();
		}

		sweepWindow.gameObject.SetActive(true);
		sweepWindow.chapterTab = this;
		sweepWindow.SetInfo(Legion.Instance.SelectedStage.u2ID, Legion.Instance.cBestCrew.u1Index-1);
		RefreshChapter();

		SelectStageScene selectStageScene = Scene.GetCurrent() as SelectStageScene;
		if(selectStageScene != null)
			selectStageScene.RefreshRewardInfo();

		//ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[Legion.Instance.SelectedStage.u2ChapterID];        
		//Legion.Instance.SubGoods(chapterInfo.acPlayGoods[Legion.Instance.selectedDifficult-1]);
    }

	// 파견 요청 통신 처리 
	public virtual void AckRequestDispatch(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		DebugMgr.Log(err);

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.STAGE_DISPATCH, err), Server.ServerMgr.Instance.CallClear);
			return;
		}
		else
		{			
			dispatchWindow.gameObject.SetActive(false);

			if(stageInfoWindow != null)
                stageInfoWindow.OnClickClose();					

			SelectStageScene selectStageScene = Scene.GetCurrent() as SelectStageScene;
			if(selectStageScene != null)
				selectStageScene.RefreshDispatchInfo();                

			RefreshChapter();

			//ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[Legion.Instance.SelectedStage.u2ChapterID];        
            //
			//Legion.Instance.SubGoods(chapterInfo.acPlayGoods[Legion.Instance.SelectedDifficult-1]);            
		}
	}	

	// 파견 결과 통신 처리
	protected void AckDispatchResult(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.STAGE_DISPATCH_RESULT, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
			DispatchResult();
		}
	}

    void DispatchResult()
    {
        //파견 보상 처리 
		StageInfo stageInfo = Legion.Instance.acCrews[dispatchCrewIdx].DispatchStage;
        int disPatchDifficult = Legion.Instance.acCrews[dispatchCrewIdx].StageDifficulty;
        //StageInfoMgr.Instance.u4PrevTotalGold = Legion.Instance.Gold;

        if (stageInfo.acGetGoods[Legion.Instance.SelectedDifficult-1].u1Type == (Byte)GoodsType.GOLD)
        {
            if(StageInfoMgr.Instance.u4TotalGold != 0)
                Legion.Instance.Gold = StageInfoMgr.Instance.u4TotalGold;
            else
                Legion.Instance.AddGoods(stageInfo.acGetGoods[Legion.Instance.SelectedDifficult-1]);
        }
        else
            Legion.Instance.AddGoods(stageInfo.acGetGoods[Legion.Instance.SelectedDifficult-1]);
		//경험치 보상은 결과창에서 처리
		dispatchWindow.gameObject.SetActive(true);			
		dispatchWindow.SetDispatchResult(Legion.Instance.acCrews[dispatchCrewIdx].DispatchStage.u2ID, dispatchCrewIdx);						
		Legion.Instance.acCrews[dispatchCrewIdx].ClearDispatch();

		SelectStageScene selectStageScene = Scene.GetCurrent() as SelectStageScene;

		if(selectStageScene != null)
		{
			selectStageScene.RefreshDispatchInfo();
			selectStageScene.RefreshRewardInfo();
		}            

		RefreshChapter();
        PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.Dispatch);

		if(StageInfoMgr.Instance.u1EventIDCount > 0)
		{
			for(int i=0; i<StageInfoMgr.Instance.arrEventID.Length; i++)
			{
				if(!EventInfoMgr.Instance.dicEventReward.ContainsKey(StageInfoMgr.Instance.arrEventID[i]))
				{
					StageInfoMgr.Instance.bReloadEvent = false;
					PopupManager.Instance.ShowLoadingPopup(1);
					Server.ServerMgr.Instance.RequestEventReload(EventReloadResult);
					break;
				}
				else
				{
					StageInfoMgr.Instance.bReloadEvent = true;
					continue;
				}
			}
		}
		else
			StageInfoMgr.Instance.bReloadEvent = true;
    }

    protected void EventReloadResult(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.STAGE_DISPATCH_RESULT, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
        }
    }

	//파견 취소 통신 처리
	public void AckCancelDispatch(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		//DebugMgr.Log(err);

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.STAGE_DISPATCH_CANCEL, err), Server.ServerMgr.Instance.CallClear);
			return;
		}
		else
		{
			dispatchWindow.gameObject.SetActive(false);
			Legion.Instance.acCrews[dispatchCrewIdx].ClearDispatch();   

			SelectStageScene selectStageScene = Scene.GetCurrent() as SelectStageScene;

			if(selectStageScene != null)
			{
				selectStageScene.RefreshDispatchInfo();
			}                          

			RefreshChapter();
		}
	}	

	// 파견 즉시 완료 통신 처리
	public void AckFinishDispatch(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		DebugMgr.Log(err);

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.STAGE_DISPATCH_FINISH, err), Server.ServerMgr.Instance.CallClear);
			return;
		}
		else
		{
			StartCoroutine(DispatchFinishResult());
		}
	}

    IEnumerator DispatchFinishResult()
    {
		if(StageInfoMgr.Instance.u1EventIDCount > 0)
		{
			for(int i=0; i<StageInfoMgr.Instance.arrEventID.Length; i++)
			{
				if(!EventInfoMgr.Instance.dicEventReward.ContainsKey(StageInfoMgr.Instance.arrEventID[i]))
				{
					StageInfoMgr.Instance.bReloadEvent = false;
					PopupManager.Instance.ShowLoadingPopup(1);
					Server.ServerMgr.Instance.RequestEventReload(EventReloadResult);
					break;
				}
				else
				{
					StageInfoMgr.Instance.bReloadEvent = true;
					continue;
				}
			}
		}
		else
			StageInfoMgr.Instance.bReloadEvent = true;

        while(!StageInfoMgr.Instance.bReloadEvent)
        {
            yield return new WaitForEndOfFrame();
        }

        //파견 보상 처리 
		StageInfo stageInfo = Legion.Instance.acCrews[dispatchCrewIdx].DispatchStage;	
		
		// 2016. 7. 14 jy 시련의 탑 보상을 넣는다
		// 기존의 보상코드는 else쪽이지만 스테이지에서 직접 받아는지 확인하고 코드 통일화가 필요함?
		/*
		if(stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.TOWER)
		{
			RewardItem[] rewardItems = Legion.Instance.cReward.GetReward();
			for(int i = 0; i < rewardItems.Length; ++i)
			{
				Legion.Instance.AddGoods(rewardItems[i].cRewards);
			}
		}
		else*/
		//{
		//	Legion.Instance.AddGoods(stageInfo.acGetGoods[Legion.Instance.SelectedDifficult-1]);
		//}
        //StageInfoMgr.Instance.u4PrevTotalGold = Legion.Instance.Gold;
        if(stageInfo.acGetGoods[Legion.Instance.SelectedDifficult-1].u1Type == (Byte)GoodsType.GOLD)
        {
            if(StageInfoMgr.Instance.u4TotalGold != 0)
                Legion.Instance.Gold = StageInfoMgr.Instance.u4TotalGold;
            else
                Legion.Instance.AddGoods(stageInfo.acGetGoods[Legion.Instance.SelectedDifficult-1]);
        }
        else
            Legion.Instance.AddGoods(stageInfo.acGetGoods[Legion.Instance.SelectedDifficult-1]);
		//경험치 보상은 결과창에서 처리
		dispatchWindow.gameObject.SetActive(true);			
		dispatchWindow.SetDispatchResult(stageInfo.u2ID, dispatchCrewIdx);
		Legion.Instance.acCrews[dispatchCrewIdx].ClearDispatch();	

		SelectStageScene selectStageScene = Scene.GetCurrent() as SelectStageScene;

		if(selectStageScene != null)
		{
			selectStageScene.RefreshDispatchInfo();
			selectStageScene.RefreshRewardInfo();
		}                    	

		RefreshChapter();
        PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.Dispatch);
    }
	/*
    // 반복 보상 수령 통신 처리
	private void AckRepeatReward(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		DebugMgr.Log(err);

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.STAGE_REPEATREWARD, err), Server.ServerMgr.Instance.CallClear);
			return;
		}
		else
		{
			if(StageInfoMgr.Instance.repeatReward == 100)
			{
				DebugMgr.Log("Error");
				return;
			}
			else
			{
				ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[StageInfoMgr.Instance.dicActData[actID].lstChapterID[currentPage]];
				Goods goods;
                // 초기 보상 수령
				if(StageInfoMgr.Instance.repeatReward == 0)
                {
					goods = chapterInfo.acFirstRewards[Legion.Instance.SelectedDifficult-1];
					Legion.Instance.AddGoods(goods);
                    chapterInfo.repeatType[Legion.Instance.SelectedDifficult-1] = 1;
                }
                // 그 이후 보상 수령
				else		
                {
					goods = chapterInfo.acRepeatRewards[StageInfoMgr.Instance.repeatReward-1][Legion.Instance.SelectedDifficult-1];
					Legion.Instance.AddGoods(goods);
                    chapterInfo.repeatType[Legion.Instance.SelectedDifficult-1] += 1;
                    
                    if(chapterInfo.repeatType[Legion.Instance.SelectedDifficult-1] > ChapterInfo.MAX_REWARD_ROTATION)
                        chapterInfo.repeatType[Legion.Instance.SelectedDifficult-1] = 1;
                }
                    
                chapterInfo.repeatCount[Legion.Instance.SelectedDifficult-1] -= chapterInfo.u1RepeatCount;
                
                repeatResult.gameObject.SetActive(true);
				repeatResult.SetInfo(goods);
                //repeatResult.SetInfo(repeatRewardIcon.sprite, repeatRewardGrade.sprite, repeatRewardCount.text);
               
                // 보상 결과 처리 팝업
                SetRepeatRewradInfo();
                
                StageInfoMgr.Instance.repeatReward = 100;
                
                SelectStageScene selectStageScene = Scene.GetCurrent() as SelectStageScene;
                
                if(selectStageScene != null)
                {
                    selectStageScene.RefreshRewardInfo();
                }
			}
		}
	}
	
    // 별 보상 수령 통신 처리 
	private void AckStarReward(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		DebugMgr.Log(err);

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.STAGE_STARREWARD, err), Server.ServerMgr.Instance.CallClear);
			return;
		}
		else
		{
			ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[StageInfoMgr.Instance.dicActData[actID].lstChapterID[currentPage]];
			Byte difficult = Legion.Instance.SelectedDifficult;
			Goods reward = null;
			reward = chapterInfo.acStarRewards[Legion.Instance.SelectedDifficult-1];

			Legion.Instance.AddGoods(reward);
			chapterInfo.starCount[Legion.Instance.SelectedDifficult-1] = 0; // 보상 수령시 챕터의 별 카운트를 0으로 해준다

            repeatResult.gameObject.SetActive(true);
			repeatResult.SetInfo(reward);
            //repeatResult.SetInfo(starRewardIcon.sprite, starRewardGrade.sprite, starRewardCount.text);
            
            SetStarRewardInfo();
            
            SelectStageScene selectStageScene = Scene.GetCurrent() as SelectStageScene;
            if(selectStageScene != null)
            {
                selectStageScene.RefreshRewardInfo();
            }
		}
	}
    */
	// =========================================== 서버 처리 함수 end ==================================================//

	// ============================================= 연출 함수 ========================================================= //
	protected virtual IEnumerator CampaignOpenEffect()
	{
		TopMenuAniContorller.Play("OutStageTopMenu");

		HideRewardBtn();
		GameObject cloudEffect = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI_Effects/Eff_UI_Cloud_FadeIn.prefab", typeof(GameObject))) as GameObject;
		RectTransform transform = cloudEffect.GetComponent<RectTransform>();
		transform.SetParent(popupParent);
		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;

		yield return new WaitForSeconds(0.8f);

		// 2016. 08. 31 jy
		// 바로가기 / 다시시작 / 다음 스테이지 에서는 챕터 이름이 연출이 안뜨도록 예외 처리함
		// 바로가기 / 다시시작 / 다음 스테이지 진행시 스테이지 정보창이 먼저 생성되어 활성화 되어 있음
		if(stageInfoWindow == null || stageInfoWindow.gameObject.activeSelf == false)
			m_cFedeEffectText.StartTextEffect();

		yield return new WaitForSeconds(1f);

		TopMenuAniContorller.Play("SelectStageTopMenu");
		StartCoroutine("PageEffect");
	}

	public void CreateMoveCloudEffect(int directionType = 0)
	{
		GameObject cloudEffect = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI_Effects/Eff_UI_Cloud_Move.prefab", typeof(GameObject))) as GameObject;
		if(cloudEffect == null)
			return;

		// 2초후 삭제되도록 한다
		Destroy(cloudEffect, 2f);
		Transform transform = cloudEffect.transform;
		transform.SetParent(popupParent);
		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;

		// 흘러가는 방향을 셋팅한다
		switch(directionType)
		{
		case 0:
			transform.Rotate(0, 0, 45f);
			break;
		case 1:
			transform.Rotate(0, 0, 90f);
			break;
		case 2:
			transform.Rotate(0, 0, 270f);
			break;
		case 3:
			transform.Rotate(0, 0, 0);
			break;
		case 4:
			transform.Rotate(0, 0, 180f);
			break;
		}
	}

	// 2016. 08. 09 jy
	// 페이지 이동 시작시 셋팅할 것을 넣음 
	protected virtual void StartPageMove()
	{
		TopMenuAniContorller.Play("OutStageTopMenu");
		HideRewardBtn();
	}
	// 2016. 08. 09 jy
	// 페이지 이동 완료시 셋팅할 것을 넣음
	protected virtual IEnumerator PageMoveComplete()
	{
		// 이동이 끝난후에는 보여주는 페이지를 제외하고 비활성화 한다
		for(int i = 0; i < m_ChapterList.Count; ++i)
		{
			if(m_nCurrent == i)
				m_ChapterList[i].gameObject.SetActive(true);
			else
				m_ChapterList[i].gameObject.SetActive(false);
		}
			
		m_cFedeEffectText.StartTextEffect();

		yield return new WaitForSeconds(1f);

		TopMenuAniContorller.Play("InStageTopMenu");
		StartCoroutine("PageEffect");
	}

	protected virtual void HideRewardBtn()
	{
		m_ChapterList[m_nCurrent].HideStageSlot();
		if(SideBtnCanvas != null)
			SideBtnCanvas.alpha = 0;

		if(m_RewardOriginalPosList.Count > 0)
		{
			int count = 0;
			RectTransform[] transform = new RectTransform[2];
			if(cRepeatReward != null)
				transform[count++] = cRepeatReward.GetComponent<RectTransform>();
			if(cStarReward != null)
				transform[count++] = cStarReward.GetComponent<RectTransform>();
			
			Vector3[] targetPos = new Vector3[count];
			for( int i = 0; i < count; ++i)
			{
				transform[i].anchoredPosition3D = new Vector3(transform[i].anchoredPosition3D.x, -transform[i].sizeDelta.y * 3, transform[i].anchoredPosition3D.z);
			}
		}
	}

	protected virtual IEnumerator PageEffect()
	{
		RectTransform[] transform = new RectTransform[2];
		if(m_RewardOriginalPosList.Count > 0)
		{
			int index = 0;
			if(cRepeatReward != null)
				transform[index++] = cRepeatReward.GetComponent<RectTransform>();
			if(cStarReward != null)
				transform[index] = cStarReward.GetComponent<RectTransform>();
		}

		bool isMoveEnd = false;
		float time = 0f;
		float deltaTime;
		while(true)
		{
			deltaTime = Time.deltaTime;
			if(SideBtnCanvas.alpha < 1)
				SideBtnCanvas.alpha += deltaTime * 2f;// * 0.01f;
			
			for(int i = 0 ; i < m_RewardOriginalPosList.Count; ++i)
			{
				if( Vector3.Distance(m_RewardOriginalPosList[i], transform[i].anchoredPosition3D) > 0.1f)
					transform[i].anchoredPosition3D = Vector3.Lerp(transform[i].anchoredPosition3D , m_RewardOriginalPosList[i], time * 1f);// * 1.7f);
				else
				{
					m_ChapterList[m_nCurrent].StartSlotFadeIn();
					isMoveEnd = true;
				}
			}
			time += deltaTime;

			if(isMoveEnd)
			{
				SideBtnCanvas.alpha = 1f;
				yield break;
			}
			else
				yield return null;
		}
	}

	// 쳅터 이름 셋팅
	protected virtual void SetChapterName(UInt16 chapterID)
	{
		ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[chapterID];

		if(chapterInfo == null || _ChapterDropdown == null)
			return;

		string chapterName = TextManager.Instance.GetText(chapterInfo.strName);
		_ChapterDropdown.value = chapterInfo.u1Number - 1;
		_ChapterDropdown.captionText.text = chapterName;
		m_cFedeEffectText.SetText(chapterName);
	}

	// ============================================= 연출 함수 End ========================================================= //

	/*
	// 2016. 08 .11 jy
	// 현재 스와이프 처리에 사용하는 변수 및 함수 전부 사용하지 않음
	// 추후 제거
	#region 스와이프 처리           
	protected List<Vector3> pos;
	public ScrollRect scrollRect;
	protected Vector3 lerpTarget;


	private bool fastSwipe = false;
	private bool fastSwipeTimer = false;
	private int fastSwipeCounter = 0;
	private int fastSwipeTarget = 30;
	private int FastSwipeThreshold = 200;
	private Vector3 dragBeginPostion;


	public virtual void OnClickLeft()
	{
		if(currentPage < 1)
		{
			lerpTarget = pos[currentPage];
			lerp = true;
			return;
		}

		lerpTarget = pos[currentPage-1];
		SetLeft();
		lerp = true;
	}

	public virtual void OnClickRight()
	{
		if(currentPage >= maxPage - 1)
		{
			lerpTarget = pos[currentPage];
			lerp = true;            
			return;
		}

		lerpTarget = pos[currentPage+1];
		SetRight();
		lerp = true;
	}

	protected virtual void SetLeft()
	{
		if(currentPage < 1)
			return;        

		currentPage--;

		if(currentPage == 0)

			leftArrow.GetComponent<Button>().interactable = false;
		else if(currentPage < maxPage)

			rightArrow.GetComponent<Button>().interactable = true;	  


		//lstPage[currentPage % MIN_PAGE].SetStage(StageInfoMgr.Instance.dicActData[actID].lstChapterID[currentPage]);
		//lstPage[currentPage % MIN_PAGE].ShowMark((openPage == currentPage) ? true : false);

		//if(currentPage >= 1 && currentPage <= paging)
		//{
		//맨 마지막 페이지를 앞으로 보내는 처리(4개를 돌려쓰기 때문)
		//	Vector3 vec = new Vector3(pageSizeX * (currentPage-1), 0f, 0f);
		//lstPage[(currentPage+3) % MIN_PAGE].GetComponent<RectTransform>().anchoredPosition = vec;
		//lstPage[(currentPage+3) % MIN_PAGE].SetStage(StageInfoMgr.Instance.dicActData[actID].lstChapterID[currentPage-1]);
		lstPage[(currentPage+3) % MIN_PAGE].ShowMark(false);
		//    paging = currentPage;
		//}


		RefreshChapter();
	}

	protected virtual void SetRight()
	{
		if(currentPage >= maxPage - 1)
			return;        

		currentPage++;
		if(currentPage >= maxPage - 1)
			rightArrow.SetActive(false);
		else if(currentPage > 0)
			leftArrow.SetActive(true);

		//lstPage[currentPage % MIN_PAGE].SetStage(StageInfoMgr.Instance.dicActData[actID].lstChapterID[currentPage]);
		//lstPage[currentPage % MIN_PAGE].ShowMark((openPage == currentPage) ? true : false);
		//if(currentPage >= (MIN_PAGE - 2) && currentPage < maxPage - 2)
		//{
		//	맨 앞 페이지를 뒤로 보내는 처리
		//	Vector3 vec = new Vector3(pageSizeX * (currentPage+2), 0f, 0f);
		//	lstPage[(currentPage - 2) % MIN_PAGE].GetComponent<RectTransform>().anchoredPosition = vec;
		//	lstPage[(currentPage - 2) % MIN_PAGE].SetStage(StageInfoMgr.Instance.dicActData[actID].lstChapterID[currentPage + MIN_PAGE - 2]);
		//	lstPage[(currentPage - 2) % MIN_PAGE].ShowMark(false);
		//  paging = currentPage;
		//}
		RefreshChapter();
	}


	public void OnBeginDrag()
	{
		return;
		fastSwipeCounter = 0;
		fastSwipeTimer = true;
		dragBeginPostion = pageContent.localPosition;
	}

	public void OnDrag()
	{
		return;
		lerp = false;
		fastSwipeCounter = 0;
		fastSwipeTimer = true;
		dragBeginPostion = pageContent.localPosition;
	}

	public void OnEndDrag()
	{
		return;
		//빠르게 스와이프 할경우 처리
		fastSwipe = false;
		fastSwipeTimer = false;

		if (fastSwipeCounter <= fastSwipeTarget)
		{                        
			if (Math.Abs(dragBeginPostion.x - pageContent.localPosition.x) > FastSwipeThreshold)
			{
				fastSwipe = true;
			}
		}     

		if(fastSwipe)
		{
			if (dragBeginPostion.x - pageContent.localPosition.x > 0)
			{
				OnClickRight();
			}
			else
			{
				OnClickLeft();
			}            
		}
		else
		{
			lerp = true;
			lerpTarget = FindClosestFrom(pageContent.anchoredPosition, pos);    
		}                        
	}

	//스와이프로 스크롤시 손을 땐 지점에서 가까운 페이지를 검색
	private Vector3 FindClosestFrom(Vector3 start, List<Vector3> positions)
	{
		Vector3 closest = Vector3.zero;
		float distance = Mathf.Infinity;

		int closePage = 0;

		for(int i=0; i<positions.Count; i++)
		{
			if (Vector3.Distance(start, positions[i]) < distance)
			{
				distance = Vector3.Distance(start, positions[i]);
				closest = positions[i];
				closePage = i;
			}
		}

		if(closePage > currentPage)
			SetRight();
		else if(closePage < currentPage)
			SetLeft(); 

		return closest;
	}

	#endregion
	*/
}
