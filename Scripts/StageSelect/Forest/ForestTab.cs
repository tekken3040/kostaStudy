using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

// 챕터와 스테이지 정보를 보여준다.
// 함수 일부는 ChapterTab의 함ㅜ
public class ForestTab : ChapterTab 
{
	private Byte openElement;
	private int nDayOfOpenedMenu;

	public override void SetChapterPage(UInt16 actID)
	{
		openElement = StageInfoMgr.Instance.OpenForestElement;
		if(init == false)
		{
			//페이지 생성 - 스테이지 목록
			base.actID = actID;
			ActInfo actInfo = StageInfoMgr.Instance.dicActData[actID];
			maxPage = actInfo.lstChapterID.Count;

			GameObject pageItem = AssetMgr.Instance.AssetLoad("Prefabs/UI/Campaign/ForestPage.prefab", typeof(GameObject)) as GameObject;
			RectTransform pageTransform = pageItem.GetComponent<RectTransform>();
			pageSizeX = Legion.BaseScreenSizeX;
			pageSizeY = Legion.BaseScreenSizeY;

			// 최소 페이지 만큼 생성하고 챕터가 그보다 적으면 챕터 수만큼 생성
			int makePage = (maxPage < MIN_PAGE) ? maxPage : MIN_PAGE;
			for(int i=0; i <makePage; i++)
			{            
				GameObject instPage = Instantiate(pageItem);
				instPage.name = "ForestPage" + i.ToString();
				pageTransform = instPage.GetComponent<RectTransform>();
				pageTransform.SetParent(pageContent);
				pageTransform.SetSiblingIndex(i);
				pageTransform.localScale = Vector3.one;
				pageTransform.sizeDelta = Vector2.zero;
				pageTransform.anchoredPosition3D = new Vector3(i * pageSizeX, 0f, 0f);

				ForestPage forestPage = instPage.GetComponent<ForestPage>();
				forestPage.onClickStage = OnClickStage;

				instPage.SetActive(false);
				m_ChapterList.Add(forestPage);

			}
			SetDropDownList();
			init = true;
		}
		RefreshAct(actID);
	}

	protected override void CheckLastChapter()
	{
		ActInfo actInfo = StageInfoMgr.Instance.dicActData[actID];

		UInt16 lastChapter = actInfo.lstChapterID[0];
		UInt16 lastStage = StageInfoMgr.Instance.dicChapterData[lastChapter].lstStageID[0];

		currentPage = 0;
		//openPage = 0;

		List<UInt16> stageIDs = new List<UInt16>();

		// 최종 클리어 스테이지를 판별
		// 스테이지가 전부 열려있지 않은 경우는 열린 길의 스테이지중 최종 클리어된 챕터 페이지와 스테이지 위치를 저장

		// 스테이지가 전부 열려있는 경우 3개의 길 각각 위와같이 체크 후
		// 체크된 LastStage 3개(길3개)중 최종 위치를 저장
		if(openElement != StageInfo.Forest.ELEMENT_ALL)
		{	
			for(int j=0; j<maxPage; j++)
			{
				ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[actInfo.lstChapterID[j]];
				stageIDs = StageInfoMgr.Instance.GetForestStageIDList(actInfo.lstChapterID[j], openElement);
				for(int k=0; k<stageIDs.Count; k++)
				{                    
					int star = StageInfoMgr.Instance.GetStageClearStar(stageIDs[k]);
					if(star > 0)
					{
						lastChapter = chapterInfo.u2ID;
						lastStage = chapterInfo.lstStageID[k];
						currentPage = j;                    
					}
				}
			}
		}
		else
		{
			UInt16[] lastStageCountOrder = new UInt16[StageInfo.Forest.MAX_LANE];
			UInt16 lastStageCount = 0;

			UInt16[] lastChapters = new UInt16[StageInfo.Forest.MAX_LANE];
			UInt16[] lastStages = new UInt16[StageInfo.Forest.MAX_LANE];

			for(int j=0; j<maxPage; j++)
			{
				ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[actInfo.lstChapterID[j]];
				for(Byte element=StageInfo.Forest.BASE_ELEMENT; element<StageInfo.Forest.ELEMENT_ALL; element++)
				{
					Byte elementIdx = (Byte)(element - StageInfo.Forest.BASE_ELEMENT);
					stageIDs = StageInfoMgr.Instance.GetForestStageIDList(actInfo.lstChapterID[j], element);
					lastStageCount = 0;
					for(int k=0; k<stageIDs.Count; k++)
					{                    
						int star = StageInfoMgr.Instance.GetStageClearStar(stageIDs[k]);
						if(star > 0)
						{
//							lastChapters[element-StageInfo.Forest.BASE_ELEMENT] = chapterInfo.u2ID;
							lastStages[elementIdx] = chapterInfo.lstStageID[k];
							currentPage = j;
							lastStageCount = (UInt16)k;
							lastStageCountOrder[elementIdx]++;
						}
						else
							break;
					}
				}
			}			
			// 변수 재활용(Max값 비교용)
			lastStageCount = lastStageCountOrder[0];
			int selectLane = 0;
			for(int laneIdx=0; laneIdx<StageInfo.Forest.MAX_LANE; laneIdx++)
			{
				if(lastStageCountOrder[laneIdx] >= lastStageCount)
				{
					selectLane = laneIdx;
					lastStageCount = lastStageCountOrder[laneIdx];
				}
			}

			if(lastStages[selectLane] != 0)
			{
				lastStage = lastStages[selectLane];
				lastChapter = StageInfoMgr.Instance.dicStageData[lastStage].chapterInfo.u2ID;
			}
		}
		// }

		//DebugMgr.Log(StageInfoMgr.Instance.GetStageClearStar(lastStage));

		// 챕터의 마지막 스테이지이고 최종 챕터가 아닐 경우 다음 챕터를 보여줌 
		if(StageInfoMgr.Instance.dicStageData[lastStage].IsLastStageInChapter
			&& StageInfoMgr.Instance.GetStageClearStar(lastStage) > 0
			&& StageInfoMgr.Instance.dicChapterData[lastChapter].u1Number < maxPage)
		{
			currentPage = StageInfoMgr.Instance.dicChapterData[lastChapter].u1Number;
		}
	}

	protected override void OnClickStage(int index)
	{
		Byte laneIdx = (Byte)(index/10);
		int convertIdx = index - (laneIdx * 10);

		//해당 스테이지에 파견중이 크루가 있으면 파견 정보창을 띄워 준다
		for(int i=0; i<Legion.Instance.acCrews.Length; i++)
		{
			// 2016. 07.04 jy 
			// 스테이지 클릭시 파견 여부가 ID로만 체크하여 난이도가 변경되어도 같은 스테이지는 파견이 보내져 있어서
			// 파견 보내진 스테이지와 현재 선택된 스테이지 난이도도 체크하도록 함
			if(Legion.Instance.acCrews[i].DispatchStage != null && 
				//Legion.Instance.acCrews[i].DispatchStage.u2ID == ((ForestPage)m_ChapterList[currentPage % MIN_PAGE])._objLane[(laneIdx-1)].GetComponent<ChapterPage>().stageSlots[convertIdx].stageID &&
				Legion.Instance.acCrews[i].DispatchStage.u2ID == ((ForestPage)m_ChapterList[currentPage % MIN_PAGE])._arrElementLine[(laneIdx-1)].m_arrStageSlot[convertIdx].stageID &&
				Legion.Instance.acCrews[i].StageDifficulty == Legion.Instance.SelectedDifficult)
			{
				OnClickDispatch(i);
				return;
			}
		}

		// 스테이지 정보창을 띄워 준다
		if(stageInfoWindow == null)
		{
			GameObject windowPref = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Campaign/ForestStageInfoWindow.prefab", typeof(GameObject))) as GameObject;

			RectTransform rectTranstorm = windowPref.GetComponent<RectTransform>();
			rectTranstorm.SetParent(popupParent);
			rectTranstorm.localScale = Vector3.one;
			rectTranstorm.anchoredPosition3D = new Vector3(0f, 0f, -200f);
			rectTranstorm.sizeDelta = Vector2.zero;
			rectTranstorm.SetSiblingIndex(0);

			stageInfoWindow = windowPref.GetComponent<ForestStageInfoWindow>() as StageInfoWindow;
			stageInfoWindow.onClickDispatch = OnClickDispatch;
			stageInfoWindow.onClickSweep = this.OnClickSweep;	            		
		}

		stageInfoWindow.gameObject.SetActive(true);

		//stageInfoWindow.SetInfo(((ForestPage)lstPage[currentPage % MIN_PAGE])._objLane[(laneIdx-1)].GetComponent<ChapterPage>().stageSlots[convertIdx].stageID);
		stageInfoWindow.SetInfo(((ForestPage)m_ChapterList[m_nCurrent])._arrElementLine[(laneIdx-1)].m_arrStageSlot[convertIdx].stageID);
	}

	//바로가기 스테이지 정보창 처리
	public override void OpenStageInfo(UInt16 stageID)
	{
		for(int i=0; i<m_ChapterList.Count; i++)
		{
			if( ((ForestPage)m_ChapterList[i]).OpenForestStageInfo(stageID) == true )
				break;
		}
	}

	public override void OnClickSweep()
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
			{
				return;
			}
//			PopupManager.Instance.ShowLoadingPopup(1);
//			Server.ServerMgr.Instance.SweepStage(Legion.Instance.SelectedStage, Legion.Instance.selectedDifficult, AckSweepResult);
		}
		//소탕권 없으면 불가능
		else
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_dispatch_ticket_no"), TextManager.Instance.GetText("popup_desc_dispatch_ticket_no"), null);
			return;
		}

		UInt16 _u2StageID = Legion.Instance.u2SelectStageID;
		Byte _u1Ticket = StageInfoMgr.Instance.GetForestTicket();//(_u2StageID);
		Byte _u1ChargedTicketCount = StageInfoMgr.Instance.GetForestChargedTicketCount();//(_u2StageID);

		if(_u1Ticket > 0)
		{
			// 소탕후 재화 확인
			if(Legion.Instance.CheckGoodsLimitExcessx(stageInfo.acGetGoods[Legion.Instance.SelectedDifficult-1].u1Type) == true)
			{
				Legion.Instance.ShowGoodsOverMessage(stageInfo.acGetGoods[Legion.Instance.SelectedDifficult-1].u1Type);
				return;
			}

			PopupManager.Instance.ShowLoadingPopup(1);
			Server.ServerMgr.Instance.SweepStage(Legion.Instance.SelectedStage, Legion.Instance.SelectedDifficult, AckSweepResult);
		}
		else
		{
			// 충전하시겠습니까?(팝업)
			((ForestStageInfoWindow)stageInfoWindow).OnClick_ChargeTicket();
		}
	}

	// 파견 통신 처리
	protected override void AckSweepResult(Server.ERROR_ID err)
	{
		base.AckSweepResult(err);
		if(err != Server.ERROR_ID.NONE)
		{

		}
		else
		{
            if(stageInfoWindow != null)
				((ForestStageInfoWindow)stageInfoWindow).RefreshTicket();
			((ForestStageInfoWindow)stageInfoWindow).UpdateTicket();
		}
	}

	private IEnumerator CheckTime()
	{        
		while(true)
		{
			if(nDayOfOpenedMenu != Legion.Instance.ServerTime.Day)
			{
				SetOpenElement();
			}
			yield return new WaitForSeconds(1f);
		}
	}  

	private void SetOpenElement()
	{
		openElement = StageInfoMgr.Instance.OpenForestElement;
		// 2016. 08 .05 jy 
		// 탐색의숲 라인 BG를 이제는 탑에서 관리하지 않는다
		((ForestPage)m_ChapterList[m_nCurrent]).SetElementLineBG(openElement);
	}

	void NoticeElement()
	{
		if(openElement != StageInfo.Forest.ELEMENT_ALL)
		{
			PopupManager.Instance.ShowOKPopup(
				TextManager.Instance.GetText(""),
				TextManager.Instance.GetText("forest_open_line_element_" + openElement),
				EmptyMethod );
		}
		else
		{
			PopupManager.Instance.ShowOKPopup(
				TextManager.Instance.GetText(""),
				TextManager.Instance.GetText("forest_open_line_all"),
				EmptyMethod);
		}
	}

	void EmptyMethod(object[] param){}

	// ============================================= 연출 함수 ========================================================= //
	protected override IEnumerator CampaignOpenEffect()
	{
		TopMenuAniContorller.Play("OutStageTopMenu");
		((ForestPage)m_ChapterList[m_nCurrent]).InitElementLineCloud();
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

		yield return new WaitForSeconds(0.5f);

		SetOpenElement();
		// 2016. 09. 20 jy
		// 서버에서 오픈된 길을 받게 되며 직접 날짜 변경 여부를 체크할 필요가 없어짐
		//StartCoroutine("CheckTime");
	}

	protected override void StartPageMove()
	{
		base.StartPageMove();

		((ForestPage)m_ChapterList[m_nCurrent]).InitElementLineCloud();
	}

	protected override IEnumerator PageMoveComplete()
	{
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

		yield return new WaitForSeconds(0.5f);

		SetOpenElement();
	}

	protected override void SetChapterName(UInt16 chapterID)
	{
		ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[chapterID];
		if(chapterInfo == null || _ChapterDropdown == null)
			return;

		string chapterName = TextManager.Instance.GetText(chapterInfo.strName);
		string openLineText;
		if( openElement == 0 ) 
			openElement = StageInfoMgr.Instance.OpenForestElement;
				
		if(openElement != StageInfo.Forest.ELEMENT_ALL)
			openLineText = 	TextManager.Instance.GetText("forest_open_line_element_" + openElement);
		else
			openLineText = TextManager.Instance.GetText("forest_open_line_all");

		_ChapterDropdown.captionText.text = chapterName;
		m_cFedeEffectText.SetText(chapterName +"\n"+ openLineText);
	}
	// ============================================= 연출 함수 End ========================================================= //
}
