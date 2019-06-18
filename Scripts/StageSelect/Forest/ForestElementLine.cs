using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class ForestElementLine : MonoBehaviour 
{
	public const int MAX_FOREST_STAGE_COUNT = 7;

	public RectTransform _rtGoingStageEffect;
	private int m_iGoingStageIndex = -1;

	public StageSlot[] m_arrStageSlot;
	public GameObject[] m_arrLockImageObj;
	public RectTransform trNewMark;
	public RectTransform trCloud;
	private Vector3 m_vBaseCloudPos;
	private bool m_isCloudMove = false;

	private List<UInt16> m_lstStageID = new List<UInt16>();

	void Awake()
	{
		trCloud.gameObject.SetActive(true);
		m_vBaseCloudPos = trCloud.anchoredPosition3D;
	}

	public void SetElementLine(ChapterInfo chapterInfo, Byte openElement, int elementIndex)
	{
		_rtGoingStageEffect.gameObject.SetActive(false);
		m_lstStageID.Clear();
		m_isCloudMove = false;
		trCloud.anchoredPosition3D = m_vBaseCloudPos;
		Byte lineElement =  (Byte)(StageInfo.Forest.BASE_ELEMENT + elementIndex);
		int stageIDCount = chapterInfo.lstStageID.Count;
		for(int i = 0; i < stageIDCount; ++i)
		{
			StageInfo stageInfo;
			StageInfoMgr.Instance.dicStageData.TryGetValue(chapterInfo.lstStageID[i], out stageInfo);
			if(stageInfo.u1ForestElement == lineElement)
				m_lstStageID.Add(chapterInfo.lstStageID[i]);
		}

		SetForestStage(openElement, chapterInfo.u1Number);
		/*
		if(openElement != StageInfo.Forest.ELEMENT_ALL)
		{
			if(openElement == lineElement)
				SetActiveLane_Forest(true, openElement);
			else
				SetActiveLane_Forest(false, openElement);
		}
		else
			SetActiveLane_Forest(true, openElement);
		*/
	}

	private void SetForestStage(Byte openElement, Byte chapterIndex)
	{
		Byte selectedDifficult = Legion.Instance.SelectedDifficult;
		trNewMark.gameObject.SetActive(false);
		m_iGoingStageIndex = -1;
		for(int i = 0; i < MAX_FOREST_STAGE_COUNT; ++i)
		{
			if(i < m_lstStageID.Count)
			{
				m_arrStageSlot[i].gameObject.SetActive(true);
				StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[m_lstStageID[i]];
				m_arrStageSlot[i].SetSlotForest(stageInfo, i, openElement, chapterIndex);
				if(m_arrStageSlot[i].stageLock == false && stageInfo.IsClear(selectedDifficult) == false)
				{
					trNewMark.gameObject.SetActive(true);
					trNewMark.anchoredPosition3D = m_arrStageSlot[i].GetComponent<RectTransform>().anchoredPosition3D + new Vector3(-42f, 35f, 0f);//new Vector3(-62f, 33f, -50f);                                    
					m_iGoingStageIndex = i;
				}
			}
			else
			{
				m_arrStageSlot[i].gameObject.SetActive(false);
			}
		}
	}

	private void SetActiveLane_Forest(bool enabled, Byte openElement)
	{
		Byte selectedDifficult = Legion.Instance.SelectedDifficult;
		trNewMark.gameObject.SetActive(false);
		for(int i=0; i<MAX_FOREST_STAGE_COUNT; i++)
		{
			if(i < m_lstStageID.Count)
			{			
				m_arrLockImageObj[i].SetActive(false);
				AtlasMgr.Instance.SetDefaultShader(m_arrStageSlot[i].GetComponent<Image>());
				//StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[m_lstStageID[i]]; 
				if (m_arrStageSlot[i].stageLock == false && StageInfoMgr.Instance.IsClear(m_arrStageSlot[i].stageID, selectedDifficult) == false)
				{
					trNewMark.gameObject.SetActive(true);
					trNewMark.anchoredPosition3D = m_arrStageSlot[i].GetComponent<RectTransform>().anchoredPosition3D + new Vector3(-42f, 15f, 0f);                                                                  
				}
			}
			else
			{
				m_arrLockImageObj[i].SetActive(true);
			}
		}
	}

	public int GetStageIndex(UInt16 stageID)
	{
		for(int i = 0; i < m_arrStageSlot.Length; ++i)
		{
			if(m_arrStageSlot[i].stageID == stageID)
				return i;
		}

		return -1;
	}

	public void SetOpenLine(bool isOpen)
	{
		if(isOpen == true)
			StartCoroutine("LineCloudHide");
		else
		{
			trCloud.GetComponent<Image>().color = Color.white;
			trCloud.anchoredPosition3D = m_vBaseCloudPos;
			trCloud.gameObject.SetActive(true);
		}
	}

	public IEnumerator LineCloudHide()
	{
		if(trCloud == null || m_isCloudMove == true)
			yield break;

		m_isCloudMove = true;

		Color color = Color.white;
		Image cloudImage = trCloud.GetComponent<Image>();
		trCloud.gameObject.SetActive(true);
		Vector3 pos = trCloud.anchoredPosition3D;
		float targetX = trCloud.anchoredPosition3D.x + trCloud.sizeDelta.x;
		float time = 0f;
		while(true)
		{
			pos = trCloud.anchoredPosition3D;
			if(targetX > pos.x)
			{
				pos.x += time * 20f;
				trCloud.anchoredPosition3D = pos;
				if(color.a > 0)
				{
					color.a -= Time.deltaTime * 0.8f;
					cloudImage.color = color;
				}
				else
					break;
			}
			else
				break;
	
			time += Time.deltaTime;
			yield return null;
		}
		SetGoingStageEffect();
		trCloud.gameObject.SetActive(false);
		m_isCloudMove = false;
	}

	protected virtual void SetGoingStageEffect()
	{
		if(_rtGoingStageEffect == null)
			return;

		if(m_iGoingStageIndex < 0)
		{
			_rtGoingStageEffect.gameObject.SetActive(false);
			return;
		}

		RectTransform rectTr = m_arrStageSlot[m_iGoingStageIndex].GetComponent<RectTransform>();
		Vector3 currentPos = rectTr.anchoredPosition3D;
		currentPos.z = -15;

		_rtGoingStageEffect.anchoredPosition3D = currentPos;
		_rtGoingStageEffect.gameObject.SetActive(true);
	}
}

