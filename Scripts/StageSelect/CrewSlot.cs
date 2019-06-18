using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class CrewSlot : MonoBehaviour
{

    public Button button;
    public Image slotBG;
    public Image flagBG;
    public Image flagNumber;
    public Text timeText;
    public Text crewText;
    public GameObject alram;
    public int index;
    public GameObject questMark;
    SelectStageScene selectStageScene;
    private UInt16 stageID;

    void Awake()
    {
        stageID = 0;
        selectStageScene = Scene.GetCurrent() as SelectStageScene;
        if (selectStageScene != null)
        {
            selectStageScene.refreshDispatchInfo += Init;
        }

        Init();
    }

    public void Init()
    {
        SetSlot();

        if (gameObject.activeInHierarchy == true)
        {
            StopCoroutine("CheckDispatchTime");
            StartCoroutine("CheckDispatchTime");
        }
    }

    public void Init(UInt16 stageID)
    {
        this.stageID = stageID;
        Init();
    }

    public void SetSlot()
    {
        //크루 튜토리얼이 진행되지 않은 경우 잠금
        if (Legion.Instance.sName == "")
        {
            button.interactable = false;
            slotBG.gameObject.SetActive(true);
            SetQuestMarkEnable(false);
            return;
        }

        Crew crew = Legion.Instance.acCrews[index];
        if (crew.abLocks[0]) // lock
        {
            button.interactable = false;
            slotBG.gameObject.SetActive(true);
            flagBG.gameObject.SetActive(false);
            flagNumber.gameObject.SetActive(false);
            timeText.gameObject.SetActive(false);
            crewText.color = Color.white;

            SetQuestMarkEnable(false);
        }
        else
        {
            slotBG.gameObject.SetActive(false);
            flagBG.gameObject.SetActive(true);
            flagNumber.gameObject.SetActive(true);

            if (index < 3)
                flagNumber.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/flag_01.num_g_" + (index + 1));
            else
                flagNumber.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/flag_01.num_s_" + (index + 1));
            flagNumber.SetNativeSize();

            // 현재 스테이지가 파견인지 체크하여 Icon을 비/활성화 한다
            if (crew.DispatchStage == null)
            {
                flagBG.color = Color.white;
                flagNumber.color = Color.white;
                crewText.color = Color.white;

                if (stageID == 0)
                {
                    timeText.gameObject.SetActive(false);
                    button.interactable = true;
                    SetQuestMarkEnable(false);
                }
                else
                {
                    if (crew.u1Index == Legion.Instance.cBestCrew.u1Index)
                    {
                        button.interactable = false;
                        timeText.gameObject.SetActive(true);
                        timeText.text = TextManager.Instance.GetText("btn_main_crew_dispatch");
                        SetQuestMarkEnable(false);
                    }
                    else
                    {
                        button.interactable = true;
                        timeText.gameObject.SetActive(false);
                        SetQuestMarkEnable(CheckDispatchQuest());
                    }
                }
            }
            else
            {
                SetQuestMarkEnable(false);
                TimeSpan timeSpan = crew.DispatchTime - Legion.Instance.ServerTime;

                timeText.gameObject.SetActive(true);
                flagBG.color = Color.gray;
                flagNumber.color = Color.gray;
                crewText.color = Color.gray;

                if (alram != null)
                    alram.SetActive(false);

                if (timeSpan.Ticks <= 0)
                {
                    timeText.text = TextManager.Instance.GetText("popup_title_dispatch_done");

                    if (alram != null)
                        alram.SetActive(true);
                }
            }
        }
    }

    //파견 시간 갱신
    private IEnumerator CheckDispatchTime()
    {
        Crew crew = Legion.Instance.acCrews[index];

        if (crew.DispatchStage != null)
        {
            //selectStageScene.RefreshCrewList();
            while (true)
            {
                TimeSpan timeSpan = crew.DispatchTime - Legion.Instance.ServerTime;

                if (timeSpan.Ticks > 0)
                {
                    int hour = (int)(timeSpan.TotalSeconds / 3600);
                    int min = (int)((timeSpan.TotalSeconds % 3600) / 60);
                    int sec = (int)((timeSpan.TotalSeconds % 3600) % 60);

                    timeText.text = string.Format("{0:00}:{1:00}:{2:00}", hour, min, sec);
                }
                else
                {
                    //DebugMgr.LogError("!!");
                    timeText.text = TextManager.Instance.GetText("popup_title_dispatch_done");
                    yield break;
                }

                yield return new WaitForSeconds(1f);
            }
        }
    }

    void OnDestroy()
    {
        SelectStageScene selectStageScene = Scene.GetCurrent() as SelectStageScene;
        if (selectStageScene != null)
        {
            selectStageScene.refreshDispatchInfo -= Init;
        }
    }

    private bool CheckDispatchQuest()
    {
        if (stageID == 0 || Legion.Instance.cQuest.u2IngQuest <= 0)
            return false;

        // 퀘스트 타입이 파견인지 확인
        QuestInfo questInfo = Legion.Instance.cQuest.CurrentQuest();
        if (questInfo.u1QuestType != 18)
            return false;

        // 입력된 스테이지 정보가 존재하는지 확인
        StageInfo stageInfo;
        StageInfoMgr.Instance.dicStageData.TryGetValue(stageID, out stageInfo);
        if (stageInfo == null)
            return false;

        // 파견 스테이지가 같지 않다면
        if (questInfo.u2QuestTypeID != stageID)
            return false;

        Byte bossType = stageInfo.u1BossType;
        if (bossType < 2) bossType = 1;
        if ((questInfo.u1Delemiter1 == 0 || questInfo.u1Delemiter1 == Legion.Instance.SelectedDifficult)
         && (questInfo.u1Delemiter2 == 0 || questInfo.u1Delemiter2 == bossType)
         && (questInfo.u1Delemiter3 == 0 || questInfo.u1Delemiter3 == stageInfo.GetActInfo().u1Number))
        {
            return true;
        }

        return false;
    }

    private void SetQuestMarkEnable(bool isEnable)
    {
        if (questMark != null)
            questMark.SetActive(isEnable);
    }
}