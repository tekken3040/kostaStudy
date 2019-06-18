using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class TowerPage : ChapterPage 
{
	public TowerStageInfoWindow m_cTowerStageInfoWindow;
	public RectTransform StageContent;
	public RectTransform StartFloor;
	public RectTransform LastFloor;

	public Dictionary<int, TowerStageSlot> dicTowerSlotInfo  = new Dictionary<int, TowerStageSlot>();
	private float m_fFloorSizeY;
	private float m_fTargetFloorY;
	private bool  m_isFloorMove = false;

	public void Awke()
	{
		//dicTowerSlotInfo
		StartFloor.localScale = Vector3.one;
		StartFloor.anchoredPosition3D = Vector3.zero;
	}

	public override void SetStage(UInt16 chapterID)
	{
		m_chapterID = chapterID;
		// 이미 FloorSlot을 생성했다면
		if( dicTowerSlotInfo.Count != 0 )
		{
			// 탑의 스테이지 정보를 새로고친다
			RefreshTowerStageInfo(chapterID);
			return;
		}

		ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[chapterID];
		m_nDirectionType = chapterInfo.m_nDirectionType;
		m_BackGround.sprite = AssetMgr.Instance.AssetLoad("Sprites/Campaign/" + chapterInfo.m_strBackGroundName + ".png" , typeof(Sprite)) as Sprite;
		SetMapDifficultColor(m_BackGround);

		GameObject floorItem = AssetMgr.Instance.AssetLoad("Prefabs/UI/Campaign/TowerFloor.prefab", typeof(GameObject)) as GameObject;
		m_fFloorSizeY = floorItem.GetComponent<RectTransform>().sizeDelta.y;
		float fStartFloor = StartFloor.sizeDelta.y;

		int index = 0;
		int stageCount = chapterInfo.lstStageID.Count;
		for(; index < stageCount; ++index)
		{
			StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[chapterInfo.lstStageID[index]]; 
			TowerStageSlot stageSlotInfo;
			if(index == stageCount - 1)
			{
				// 마지막층 셋팅
				LastFloor.localScale = Vector3.one;
				LastFloor.anchoredPosition3D = new Vector3(0f, (fStartFloor + (index * m_fFloorSizeY)), 0f);

				stageSlotInfo = LastFloor.GetComponent<TowerStageSlot>();
			}
			else
			{
				GameObject instFloor = Instantiate(floorItem);
				instFloor.name = "Floor" + index.ToString();

				RectTransform rectTransform = instFloor.GetComponent<RectTransform>();
				rectTransform.SetParent(StageContent);
				rectTransform.localScale = Vector3.one;
				rectTransform.anchoredPosition3D = new Vector3(0f, (fStartFloor + (index * m_fFloorSizeY)), 0f);

				stageSlotInfo = instFloor.GetComponent<TowerStageSlot>();
			}
			stageSlotInfo.m_cParentPage = this;
			stageSlotInfo.SetSlot(stageInfo, index);
			dicTowerSlotInfo.Add(index ,stageSlotInfo);
		}
			
		StageContent.sizeDelta = new Vector2(StageContent.sizeDelta.x ,LastFloor.sizeDelta.y + fStartFloor + ( index * m_fFloorSizeY));
		StageContent.anchoredPosition3D = new Vector3(StageContent.anchoredPosition3D.x, (StageContent.sizeDelta.y * 0.5f), 0f);
		//CheckLastClearFloor();
	}

	// 탑 스테이지의 정보를 교체한다
	private void RefreshTowerStageInfo(UInt16 chapterID)
	{
		ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[chapterID];
		m_nDirectionType = chapterInfo.m_nDirectionType;
		m_BackGround.sprite = AssetMgr.Instance.AssetLoad("Sprites/Campaign/" + chapterInfo.m_strBackGroundName + ".png" , typeof(Sprite)) as Sprite;
		SetMapDifficultColor(m_BackGround);

		int count = dicTowerSlotInfo.Count;
		for(int i = 0; i < count; ++i )
		{
			dicTowerSlotInfo[i].SetSlot(chapterInfo.lstStageID[i], i);
		}
		StageContent.anchoredPosition3D = new Vector3(StageContent.anchoredPosition3D.x, (StageContent.sizeDelta.y * 0.5f), 0f);
		//m_cTowerStageInfoWindow.HideWindow();
		//CheckLastClearFloor();
	}

	// 마지막으로 클리어한 층 확인하여 해당층으로 이동
	public void CheckLastClearFloor()
	{
		StopCoroutine("LastFloorMoving");
		m_fTargetFloorY = 0;
		int i = 0;
		UInt16 stageID = 0;
		for(; i < dicTowerSlotInfo.Count; ++i)
		{
			stageID = dicTowerSlotInfo[i].m_u2StageID;
			// 현재 스테이지가 클리어 되었는지
			if( StageInfoMgr.Instance.IsClear(stageID, Legion.Instance.SelectedDifficult))
			{
				// 마지막 스테이지가 아니라면
				if( StageInfoMgr.Instance.dicStageData[stageID].IsLastStageInChapter == false )
					continue;
			}
			// 현재 클리어 한층이라도 클리어 했다면 
			if( i > 0 )
			{
				m_fTargetFloorY =  (StageContent.sizeDelta.y * 0.5f) - (i * m_fFloorSizeY);
				//contentPosY = (int)(StartFloor.sizeDelta.y + ((i-1) * m_fFloorSizeY));
			}
			else
			{
				if(m_cTowerStageInfoWindow != null)
					m_cTowerStageInfoWindow.FadeInWindow();
			}

			break;
		}
		m_cTowerStageInfoWindow.SetInfo(stageID);
		//StageContent.anchoredPosition3D = new Vector3(StageContent.anchoredPosition3D.x, (StageContent.sizeDelta.y * 0.5f) - contentPosY , 0f);
		StageContent.anchoredPosition3D = new Vector3(StageContent.anchoredPosition3D.x, (StageContent.sizeDelta.y * 0.5f), 0f);
		if(i > 0)
			StartLastFloorMoving();
	}

	public void StartLastFloorMoving()
	{
		StartCoroutine("LastFloorMoving");
	}

	private IEnumerator LastFloorMoving()
	{
		float time = 0f;
		Vector3 targetPos = new Vector3(StageContent.anchoredPosition3D.x, m_fTargetFloorY, 0f);
		while(true)
		{
			if(Vector3.Distance(targetPos, StageContent.anchoredPosition3D) >= 0.1f)
			{
				StageContent.anchoredPosition3D = Vector3.Lerp(StageContent.anchoredPosition3D ,targetPos ,time * 0.2f);
				time += Time.deltaTime;

				yield return null;
			}
			else
			{
				m_isFloorMove = false;
				if(m_cTowerStageInfoWindow != null)
					m_cTowerStageInfoWindow.FadeInWindow();
				yield break;
			}
		}
	}

	protected override void SetMapDifficultColor(Image mapImage)
	{
		if(m_BackGround == null)
			return;

		Color mapColor;
		switch(Legion.Instance.SelectedDifficult)
		{
		case 2:
			mapColor = new Color32(237, 245, 102, 255);
			break;
		case 3:
			mapColor = new Color32(245, 123, 102, 255);
			break;
		default:
			mapColor = new Color32(123, 227, 255, 255);
			break;
		}
		mapImage.color = mapColor;
	}
}