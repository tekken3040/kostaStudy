using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System;

// 스테이지 목록
public class ChapterPage : MonoBehaviour 
{
	protected const int MAX_STAGE_COUNT = 10;
	public delegate void OnClickStage(int index);	
	public OnClickStage onClickStage;

	public CanvasGroup StageSlotsCanvas;
	public StageSlot[] stageSlots;
	public RectTransform mark;
	public RectTransform bossMark;
	public RectTransform _rtGoingStageEffect;

	public Image m_BackGround;
    protected int m_nDirectionType;
    public int Direction { get{ return m_nDirectionType; } }
	protected UInt16 m_chapterID;
	public UInt16 u2ID { get { return m_chapterID; } }
	public int m_iGoingStageIndex = -1;

	public virtual void SetStage(UInt16 chapterID)
	{
		Byte difficult = Legion.Instance.SelectedDifficult;
		m_chapterID = chapterID;
		ChapterInfo chapterInfo = StageInfoMgr.Instance.dicChapterData[chapterID];
        m_nDirectionType = chapterInfo.m_nDirectionType;
		m_iGoingStageIndex = -1;

        //m_BackGround.sprite = AssetMgr.Instance.AssetLoad("Sprites/Campaign/" + chapterInfo.m_strBackGroundName + ".png" , typeof(Sprite)) as Sprite;
        SetStageRandomBG();
        SetMapDifficultColor(m_BackGround);
		ShowMark(false);
		bossMark.gameObject.SetActive(false);
		_rtGoingStageEffect.gameObject.SetActive(false);
		for(int i=0; i<MAX_STAGE_COUNT; i++)
		{
			//stageLines[i].SetActive(false);
			if(i < chapterInfo.lstStageID.Count)
			{			
				stageSlots[i].gameObject.SetActive(true);
				StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[chapterInfo.lstStageID[i]]; 
				stageSlots[i].SetSlot(stageInfo.u2ID, i);
                
                //마지막 여린 스테이지일 경우 이펙트를 띄워준다 
                if (stageSlots[i].stageLock == false)
                {
					bool isStageClear = stageInfo.IsClear(difficult);
					ShowMark(!isStageClear);
                    setCurrenMarkScale(stageInfo);

					// 현재 스테이지 클리어 되지 않았다면
					// 인덱스만 저장한다 스테이지 오픈 열출로 인하여 이펙트는 나중에 활성화
					if(isStageClear == false)
						m_iGoingStageIndex = i;
					//mark.GetComponent<RectTransform>().anchoredPosition3D = stageSlots[i].GetComponent<RectTransform>().anchoredPosition3D + new Vector3(0f, 0f, -50f);
                }
					
                //보스 스테이지일 경우 보스 마크를 띄워준다
				if (stageInfo.u1BossType == 2)
				{
					Vector2 ratio = Legion.Instance.ScreenRatio;
					bossMark.gameObject.SetActive(true);					
					int star = StageInfoMgr.Instance.GetStageClearStar(stageInfo.u2ID);					
					bossMark.anchoredPosition = new Vector3((stageInfo.stagePosX*ratio.x)+8, stageInfo.stagePosY * ratio.y, 0.0f);
				}
			}
			else
			{
				stageSlots[i].gameObject.SetActive(false);
			}
		}
	}

    void setCurrenMarkScale(StageInfo _stageInfo)
    {
        float _fGapX = 0.0f;
        float _fGapY = 0.0f;
        switch (_stageInfo.u1BossType)
        {
            case 0:
                _fGapX = 11.0f; //11
                _fGapY = 100.0f; // 76
                break;
            case 1:
                _fGapX = 15.0f; //15
                _fGapY = 113.0f;// 87
                break;
            case 2:
                _fGapX = 6.0f;  //6
                _fGapY = 128.0f; // 99
                break;
        }
		Vector2 ratio = Legion.Instance.ScreenRatio;
		mark.anchoredPosition3D = new Vector3((_stageInfo.stagePosX * ratio.x) + _fGapX, (_stageInfo.stagePosY * ratio.y) + _fGapY, -1.0f);
    }

    public virtual void ShowMark(bool show)
    {
		if(mark == null)
			return;

        mark.gameObject.SetActive(show);
    }
	
	public void OnClickStageButton(int index)
	{		
		if(!Legion.Instance.CheckEmptyInven())
		{
			return;
		}
		if(onClickStage != null)
			onClickStage(index);
	}

	// 2016. 08. 09 jy 
	// 난이도에 따라 배경이미지의 색상을 추가한다
	protected virtual void SetMapDifficultColor(Image mapImage)
	{
		if(m_BackGround == null)
			return;
		
		Color mapColor;
		switch(Legion.Instance.SelectedDifficult)
		{
		case 2:
            mapColor = new Color32(87, 97, 167, 255);
			break;
		case 3:
			mapColor = new Color32(163, 62, 77, 255);
			break;
		default:
			mapColor = Color.white;
			break;
		}
		mapImage.color = mapColor;
	}

	// 스테이지 슬롯을 숨긴다
	public void HideStageSlot()
	{
		if(StageSlotsCanvas == null )
			return;

		StageSlotsCanvas.alpha = 0;
	}

	// 스테이지 슬롯을 FadeIn 시킬 함수
	public void StartSlotFadeIn()
	{
		if( StageSlotsCanvas == null )
			return;
		
		StartCoroutine("StageSlotFadeIn");
	}

	// 스테이지 슬롯을 FadeIn 시킬 코루틴 함수
	private IEnumerator StageSlotFadeIn()
	{
		while(true)
		{
			if(StageSlotsCanvas.alpha < 1)
			{
				StageSlotsCanvas.alpha += Time.deltaTime * 3f;
			}
			else
			{
				StageSlotsCanvas.alpha = 1f;
				SetGoingStageEffect();
				yield break;
			}
			yield return null;
		}
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

        RectTransform rectTr = stageSlots[m_iGoingStageIndex].GetComponent<RectTransform>();
        _rtGoingStageEffect.SetParent(rectTr);
        _rtGoingStageEffect.anchoredPosition3D = new Vector3(0, 0, -20);
        _rtGoingStageEffect.localScale = Vector3.one;

        _rtGoingStageEffect.gameObject.SetActive(true);
	}

    protected void SetStageRandomBG()
    {
        int randomValue = UnityEngine.Random.Range(0, 27);
        StringBuilder bgName = new StringBuilder();
        bgName.Append("Sprites/TutorialBG/").Append("questbg_").Append(randomValue.ToString()).Append(".png");
        m_BackGround.sprite = AssetMgr.Instance.AssetLoad(bgName.ToString(), typeof(Sprite)) as Sprite;
    }
}
