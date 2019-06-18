using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class TowerStageSlot : MonoBehaviour 
{
	public Button	button;
	public Image	selectIcon;
	public Image	lockIcon;
	public Image[]	floorNum;
	public Image	clearIcon;
	public Image	crewFlogIcon;
	public Image	crewIndexIcon;

	[HideInInspector]
	public UInt16 m_u2StageID;
	public TowerPage m_cParentPage;

	private int m_iStageIndex = -1;
	public bool m_bStageLock = true;

	public void SetSlot(UInt16 stageID, int index)
	{
		m_u2StageID = stageID;
		StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[stageID];
		if(stageInfo != null)
			SetSlot(stageInfo, index);
	}

	public void SetSlot(StageInfo stageInfo, int index)
	{
		m_u2StageID = stageInfo.u2ID;
		// 현재 들어온 스테이지 인덱스가 기존과 다르다면
		if( m_iStageIndex != index)
		{
			// 인덱스 셋팅
			m_iStageIndex = index;
			SetFloorIndex();
		}

		ChapterInfo chapterInfo = stageInfo.chapterInfo;
		// 받아온 정보로 현 스테이지가 오픈 여부를 판단
		m_bStageLock = !StageInfoMgr.Instance.IsOpen(m_u2StageID, Legion.Instance.SelectedDifficult);
		lockIcon.gameObject.SetActive(m_bStageLock);
		crewFlogIcon.gameObject.SetActive(false);
		crewIndexIcon.gameObject.SetActive(false);

		StopCoroutine("SelectedIconEffect");
		// 스테이지가 락이 걸려 있다면
		if( m_bStageLock )
		{
			// 클리어 / 선택 아이콘 비활성화
			clearIcon.gameObject.SetActive(false);
			selectIcon.gameObject.SetActive(false);
			this.button.enabled = false;
		}
		else
		{
			this.button.enabled = true;
			// 클리어 여부를 확인한다
			bool isClear = (StageInfoMgr.Instance.IsClear(m_u2StageID, Legion.Instance.SelectedDifficult));
			// 클리어 하지않았다면 현 스테이지를 클리어 해야 하는층
			clearIcon.gameObject.SetActive(isClear);
			// 마지막 스테이지이고 클리어가 된 상태라면 무조건 선택 이미지를 띄운다
			if(stageInfo.IsLastStageInChapter == true && isClear == true )
			{
				//OnTowerStageClick();
				selectIcon.gameObject.SetActive(true);
				//OnTowerStageClick();
				StartCoroutine("SelectedIconEffect");
				// 현재 스테이지의 파견 정보가 있다면 파견아이콘 셋팅
				for(int i=0; i<Legion.Instance.acCrews.Length; i++)
				{
					if(Legion.Instance.acCrews[i].DispatchStage != null &&
						Legion.Instance.acCrews[i].DispatchStage.u2ID == m_u2StageID &&
						Legion.Instance.acCrews[i].StageDifficulty == Legion.Instance.SelectedDifficult)
					{
						crewFlogIcon.gameObject.SetActive(true);
                        crewFlogIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/flag_01.division_" + (i + 1));
                        crewIndexIcon.gameObject.SetActive(true);
                        if(i < 3)
						    crewIndexIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/flag_01.num_g_" + (i + 1));
                        else
                            crewIndexIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/flag_01.num_s_" + (i + 1));
                        crewIndexIcon.SetNativeSize();
					}
				}
			}
			else
			{
				selectIcon.gameObject.SetActive(!isClear);
				if(isClear == false)
				{
					//OnTowerStageClick();
					StartCoroutine("SelectedIconEffect");
				}
			}
		}

	}

	private void SetFloorIndex()
	{
		int[] arrFloorIndex = CutIndexNumber(m_iStageIndex + 1);
		if(floorNum.Length < arrFloorIndex.Length)
		{
			DebugMgr.LogError("FLOOR INDEX OVER");
		}
		else
		{
			// 분할된 인덱스 정보로 탑의 충수 뿌리기
			for(int i = 0; i < arrFloorIndex.Length; ++i)
			{
				floorNum[i].gameObject.SetActive(true);
				floorNum[i].sprite = AtlasMgr.Instance.GetSprite("Sprites/Campaign/tower_01_renew.tower_01_Number_"+ arrFloorIndex[i].ToString());
			}
		}

		// 셋팅된 층수를 정렬한다
		float y = floorNum[0].GetComponent<RectTransform>().anchoredPosition3D.y;
		if((m_iStageIndex + 1) > 9)
		{
			float imageRadius = floorNum[0].GetComponent<RectTransform>().sizeDelta.x * 0.5f;
			floorNum[1].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(-imageRadius, y, 0);
			floorNum[0].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(imageRadius, y, 0);
		}
		else
		{
			floorNum[0].GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, y, 0);
			floorNum[1].gameObject.SetActive(false);
		}
	}
		
	public void OnTowerStageClick()
	{
		// 현재 스테이지가 락이 걸려 있다면 진행하지 않는다
		if( m_bStageLock )
			return;
		
		if( m_cParentPage != null)
			m_cParentPage.OnClickStageButton(m_iStageIndex);
		else
			DebugMgr.LogError("m_cParentPage == null");
	}

	// 인덱스의 자릿수를 전부 한자릿수로 만들어 반환 한다
	private int[] CutIndexNumber(int index)
	{
		int len = index.ToString().Length;

		int[] cutIndex = new int[len];
		if(index > 9)
		{
			for(int i = len - 1; i >= 0; --i)
			{
				if(i == 0)
					cutIndex[i] = index % 10;
				else
					cutIndex[i] = index / 10;
			}	
		}
		else
			cutIndex[0] = index;

		return cutIndex;
	}

	// 현재 선택 아이콘 연출
	private IEnumerator SelectedIconEffect()
	{
		float targetAlpha = 0.5f;
		float alphaValue = 0;
		while(true)
		{
			alphaValue = selectIcon.color.a;
			if(targetAlpha < 1f)
			{
				alphaValue -= 0.01f;
				if(alphaValue <= targetAlpha)
					targetAlpha = 1f;
			}
			else
			{
				alphaValue += 0.01f;
				if(alphaValue >= targetAlpha)
					targetAlpha = 0.5f;
			}

			selectIcon.color = new Color(255, 0, 0, alphaValue);
			yield return null;
		}
	}
}
