using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class TowerTab : ChapterTab
{
	public const int TOWER_INIT_DAY = 1;

	public override void Awake()
	{
		base.Awake();
		if(stageInfoWindow != null)
			stageInfoWindow.onClickDispatch = OnClickDispatch;
	}

	void Start()
	{
		// 처음으로 시련의탑 탭의 활성화 되었을때 확인하기 위하여 스타트에서 체크
		if(Legion.Instance.bTowerResetPopup)
		{
			Legion.Instance.bTowerResetPopup = false;
			// 스크립트 생성후 활성화 상태에서만 한번만 체크하도록 스타트 함수에 넣는다
			if (Legion.Instance.ServerTime.Day == TOWER_INIT_DAY) 
				PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("poup_title_tower_rest"), TextManager.Instance.GetText ("popup_tower_reset"), null);
		}
	}

	public override void SetChapterPage (UInt16 actID)
	{
		// 마지막 스테이지 클리어 여부를 체크하여 팝을 띄운다
		CheckLastFloorClearPopup();

		// 초기화 된적이 없다면 챕터 페이지를 생성한다
		if (init == false) 
		{
			this.actID = actID;
			ActInfo actInfo = StageInfoMgr.Instance.dicActData [actID];
			maxPage = actInfo.lstChapterID.Count;

			// 생성할 챕터의 프리팹을 로드한다
			GameObject pageItem = AssetMgr.Instance.AssetLoad ("Prefabs/UI/Campaign/TowerPage.prefab", typeof(GameObject)) as GameObject;
			RectTransform pageTransform = pageItem.GetComponent<RectTransform>();
			pageSizeX = Legion.BaseScreenSizeX;
			pageSizeY = Legion.BaseScreenSizeY;

			// 최소 페이지 만큼 생성하고 챕터가 그보다 적으면 챕터 수만큼 생성
			int makePage = (maxPage < MIN_PAGE) ? maxPage : MIN_PAGE;
			for (int i = 0; i < makePage; ++i)
			{            
				GameObject instPage = Instantiate (pageItem);
				instPage.name = "TowerPage" + i.ToString ();

				pageTransform = instPage.GetComponent<RectTransform> ();
				pageTransform.SetParent (pageContent);
				pageTransform.SetSiblingIndex(i);
				pageTransform.localScale = Vector3.one;
				pageTransform.anchoredPosition3D = new Vector3 (i * pageSizeX, 0f, 0f);
				pageTransform.sizeDelta = Vector2.zero;

				TowerPage towerPage = instPage.GetComponent<TowerPage> ();
				towerPage.onClickStage = OnClickStage;
				towerPage.m_cTowerStageInfoWindow = stageInfoWindow as TowerStageInfoWindow;

				instPage.SetActive(false);
				// 생성된 타운씬 페이지를 스크립트를 저장한다
				m_ChapterList.Add(towerPage);
			}
			SetDropDownList();
			init = true;
		}
		RefreshAct (actID);
	}

	// 스테이지 클릭 처리
	protected override void OnClickStage(int index)
	{
		TowerPage towerPage =  m_ChapterList[m_nCurrent] as TowerPage;
		if( towerPage == null )
		{
			DebugMgr.LogError("OnClickStage (towerPage Info = null)");
			return;
		}
		//해당 스테이지에 파견중이 크루가 있으면 파견 정보창을 띄워 준다
		for (int i = 0; i < Legion.Instance.acCrews.Length; i++) 
		{
			// 2016. 07.04 jy 
			// 스테이지 클릭시 파견 여부가 ID로만 체크하여 난이도가 변경되어도 같은 스테이지는 파견이 보내져 있어서
			// 파견 보내진 스테이지와 현재 선택된 스테이지 난이도도 체크하도록 함
			if (Legion.Instance.acCrews [i].DispatchStage != null &&
				Legion.Instance.acCrews [i].DispatchStage.u2ID == towerPage.dicTowerSlotInfo[index].m_u2StageID &&
				Legion.Instance.acCrews [i].StageDifficulty == Legion.Instance.SelectedDifficult) 
			{
				Legion.Instance.u2SelectStageID = towerPage.dicTowerSlotInfo [index].m_u2StageID;
				OnClickDispatch (i);
				return;
			}
		}

		try
		{
			stageInfoWindow.SetInfo (towerPage.dicTowerSlotInfo[index].m_u2StageID);
		}
		catch
		{
			DebugMgr.LogError(index);
		}
	}

	//파견 정보창
	public override void OnClickDispatch(int crewIdx)
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
			transform.anchoredPosition3D = Vector3.zero;//new Vector3(0f, 0f, -150f);
			transform.sizeDelta = Vector2.zero;

			dispatchWindow = windowPref.GetComponent<DispatchWindow>();
			dispatchWindow.chapterTab = this;
		}		

		dispatchWindow.gameObject.SetActive(false);

		//파견중이 아니면 파견 팝업
		if(Legion.Instance.acCrews[crewIdx].DispatchStage == null)
		{
			// 시련의 탑은 중복 파견을 막는다
			for(int i = 0; i < Legion.Instance.acCrews.Length; ++i)
			{
				if(Legion.Instance.acCrews[i].DispatchStage == null)
					continue;
				
				if(Legion.Instance.acCrews[i].DispatchStage.u2ID == Legion.Instance.SelectedStage.u2ID)
					return;
			}
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
					return;
				
				dispatchCrewIdx = crewIdx;
				PopupManager.Instance.ShowLoadingPopup(1);
				Server.ServerMgr.Instance.GetDispatchResult(Legion.Instance.acCrews[crewIdx], AckDispatchResult);	
			}					
		}
	}

	private void CheckLastFloorClearPopup()
	{
		// 스테이지가 마지막 스테이지고 클리어 했다면 최종층 클리어 팝업을 띄운다
		if (Legion.Instance.bTowerClearPopup) 
		{
			Legion.Instance.bTowerClearPopup = false;
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_tower_clear"), TextManager.Instance.GetText ("popup_tower_clear"), null);
		}
	}

	protected override IEnumerator CampaignOpenEffect()
	{
		TopMenuAniContorller.Play("OutStageTopMenu");

		HideRewardBtn();
		GameObject cloudEffect = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI_Effects/Eff_UI_Cloud_FadeIn.prefab", typeof(GameObject))) as GameObject;
		RectTransform transform = cloudEffect.GetComponent<RectTransform>();
		transform.SetParent(popupParent);
		transform.localScale = Vector3.one;
		transform.localPosition = Vector3.zero;

		yield return new WaitForSeconds(0.8f);

		m_cFedeEffectText.StartTextEffect();

		yield return new WaitForSeconds(1f);

		TopMenuAniContorller.Play("SelectStageTopMenu");
		StartCoroutine("PageEffect");
	}

	protected override IEnumerator PageMoveComplete()
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

	protected override void HideRewardBtn()
	{
		if(SideBtnCanvas != null)
			SideBtnCanvas.alpha = 0;
		((TowerStageInfoWindow)stageInfoWindow).HideWindow();
	}

	protected override IEnumerator PageEffect()
	{
		//StartCoroutine("ActNameFadeIn");
		((TowerStageInfoWindow)stageInfoWindow).HideWindow();
		((TowerPage)m_ChapterList[m_nCurrent]).CheckLastClearFloor();

		while(true)
		{
			if(SideBtnCanvas.alpha < 1)
			{
				SideBtnCanvas.alpha += Time.deltaTime * 2f;// * 0.01f;
			}
			else
			{
				SideBtnCanvas.alpha = 1f;
				yield break;
			}
			yield return null;
		}
	}

	// 파견 요청 통신 처리 
	public override void AckRequestDispatch(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		DebugMgr.Log(err);
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.STAGE_DISPATCH, err), Server.ServerMgr.Instance.CallClear);
			return;
		}
		else
		{			
			dispatchWindow.gameObject.SetActive(false);

			SelectStageScene selectStageScene = Scene.GetCurrent() as SelectStageScene;
			if(selectStageScene != null)
				selectStageScene.RefreshDispatchInfo();                     

			RefreshChapter();    
		}
	}	
}