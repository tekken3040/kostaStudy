using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
// ChapterTab과 ForestTab이 구조가 비슷하고
// ChapterTab이 ChapterPage를 사용하는걸 그대로 사용하기 위해 ForestPage도 ChapterPage를 상속받음
// ChapterTab이 ChapterPage 함수 사용하는것 처럼 ForestTab이 ForestPage에 있는 함수를 사용하지만,
// 내부적으로는 길을 판단 후 처리하는 과정을 한번 더 거치도록 구현되어있음
public class ForestPage : ChapterPage 
{
	public ForestElementLine[] _arrElementLine;
	public GameObject[] _arrObjLineBG;
	private Byte openElement;

	public override void SetStage(UInt16 chapterID)
	{
		ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[chapterID];
		openElement = StageInfoMgr.Instance.OpenForestElement;//StageInfoMgr.Instance.forest.au1OpenElement[(int)Legion.Instance.ServerTime.DayOfWeek];
		m_nDirectionType = chapterInfo.m_nDirectionType;

		m_BackGround.sprite = AssetMgr.Instance.AssetLoad("Sprites/Background/" + chapterInfo.m_strBackGroundName + ".png" , typeof(Sprite)) as Sprite;
		SetMapDifficultColor(m_BackGround);

		for(int i = 0; i < StageInfo.Forest.MAX_LANE; ++i)
		{
			_arrElementLine[i].SetElementLine(chapterInfo, openElement, i);
		}
		/*
		// UI상 속성별 라인 하나씩 ChapterPage 를 가지고 있음.
		for(int laneIdx=0; laneIdx<StageInfo.Forest.MAX_LANE; laneIdx++)
		{
			ChapterPage chapterPage = _objLane[laneIdx].GetComponent<ChapterPage>();
			List<UInt16> lstStage = new List<UInt16>();
			for(int i=0; i<chapterInfo.lstStageID.Count; i++)
			{
				StageInfo stageInfo;
				StageInfoMgr.Instance.dicStageData.TryGetValue(chapterInfo.lstStageID[i], out stageInfo);
				if(stageInfo.u1ForestElement == StageInfo.Forest.BASE_ELEMENT+laneIdx)
					lstStage.Add(chapterInfo.lstStageID[i]);
			}

			chapterPage.onClickStage = base.onClickStage; // StageSlot 클릭시 발생하는 이벤트
			_objLane[laneIdx].GetComponent<ChapterPage>().SetForestStage(lstStage, openElement);

			if(openElement != StageInfo.Forest.ELEMENT_ALL)
			{
				if(openElement == StageInfo.Forest.BASE_ELEMENT+laneIdx)
				{
					chapterPage.SetActiveLane_Forest(true);
				}
				else
				{
					chapterPage.SetActiveLane_Forest(false);
				}
			}
			else
			{
				chapterPage.SetActiveLane_Forest(true);
			}
		}
		*/
	}
	public void InitElementLineCloud()
	{
		for(int i = 0; i < StageInfo.Forest.MAX_LANE; ++i)
		{
			_arrElementLine[i].SetOpenLine(false);
		}
	}

	public void SetElementLineBG(Byte openElement)
	{
		this.openElement = openElement;
		int element = openElement - StageInfo.Forest.BASE_ELEMENT;
		for(int i = 0; i < StageInfo.Forest.MAX_LANE; ++i)
		{
			if(openElement != StageInfo.Forest.ELEMENT_ALL)
			{
				if(element == i)
				{
					_arrElementLine[i].SetOpenLine(true);
					_arrObjLineBG[i].SetActive(true);
				}
				else
				{
					_arrElementLine[i].SetOpenLine(false);
					_arrObjLineBG[i].SetActive(false);
				}
			}
			else
			{
				_arrElementLine[i].SetOpenLine(true);
				_arrObjLineBG[i].SetActive(true);
			}
		}
	}

	public override void ShowMark(bool show)
	{
		for(int i = 0; i < StageInfo.Forest.MAX_LANE; ++i)
		{
			if(openElement != StageInfo.Forest.ELEMENT_ALL)
			{
				if((int)(openElement - StageInfo.Forest.BASE_ELEMENT) == i)
				{
					_arrElementLine[i].trNewMark.gameObject.SetActive(show);
					break;
				}
			}
			else
			{
				_arrElementLine[i].trNewMark.gameObject.SetActive(show);
			}
		}
	}

	public bool OpenForestStageInfo(UInt16 stageID)
	{
		int lineIndex = 10;
		for(int i = 0; i < _arrElementLine.Length; ++i)
		{
			int nStageIndex = _arrElementLine[i].GetStageIndex(stageID);
			if(nStageIndex != -1)
			{
				OnClickStageButton(lineIndex + nStageIndex);
				return true;
			}
			lineIndex += 10;
		}

		return false;
	}
}
