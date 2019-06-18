using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class BossRushSlot : MonoBehaviour
{
    [SerializeField] Image imgBossIcon;
    [SerializeField] Text txtName;
    [SerializeField] Text txtStageNum;
    [SerializeField] GameObject objHighLight;
    [SerializeField] GameObject objDisable;
    [SerializeField] GameObject objLock;
    [SerializeField] Image imgStageEmblem;
    [SerializeField] Text txtStageText;
    [SerializeField] Text txtRetry;
    [SerializeField] GameObject objRetry;
    [SerializeField] GameObject objFailSubText;
    [SerializeField] Color[] OpenColor;
    Byte u1RetryCnt = 0;
    EventDungeonStageInfo _stageInfo;
    public EventDungeonStageInfo GetStageInfo
    {
        get
        {
            return _stageInfo;
        }
    }
    StringBuilder tempStringBuilder;

    public void SetData(EventDungeonStageInfo stageInfo, int idx)
    {
        tempStringBuilder = new StringBuilder();
        _stageInfo = stageInfo;
        imgBossIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Event/boss-rush-icon." + _stageInfo.au2StageID[idx]);
        txtName.text = TextManager.Instance.GetText(StageInfoMgr.Instance.dicStageData[_stageInfo.au2StageID[idx]].sName);
        txtStageNum.text = ((stageInfo.u1OpenType - (Byte)WEEK_DAY.MONDAY)*5 + (idx+1)).ToString();
        int index = Legion.Instance.cEvent.openStageIds.FindIndex(cs => cs.u2EventID == _stageInfo.u2EventID && cs.u2StageID == _stageInfo.au2StageID[idx]);

        if(index >= 0)
        {
            switch(Legion.Instance.cEvent.openStageIds[index].u1Closed)
            {
                case 0:
                    objHighLight.SetActive(true);
                    objDisable.SetActive(false);
                    objLock.SetActive(false);
                    imgStageEmblem.gameObject.SetActive(false);
                    this.GetComponent<Button>().interactable = true;
                    objRetry.SetActive(false);
                    objFailSubText.SetActive(false);
                    break;

                case 1:
                    objHighLight.SetActive(false);
                    objDisable.SetActive(true);
                    objLock.SetActive(false);
                    imgStageEmblem.gameObject.SetActive(true);
                    imgStageEmblem.sprite = AtlasMgr.Instance.GetSprite("Sprites/Event/event_02.emblem_fail");
                    txtStageText.text = TextManager.Instance.GetText("event_bossrush_stage_fail");
                    txtStageText.gameObject.GetComponent<Outline>().effectColor = OpenColor[1];
                    this.GetComponent<Button>().interactable = true;
                    objRetry.SetActive(true);
                    objFailSubText.SetActive(true);
                    int openCost = 0;
                    openCost = EventInfoMgr.Instance.lstBossRush[0].u2OpenCount + (Legion.Instance.cEvent.openStageIds[index].u1PlayCount*EventInfoMgr.Instance.lstBossRush[0].u1OpenRetryUpCount);
                    if(openCost > EventInfoMgr.Instance.lstBossRush[0].u1OpenMaxCount)
                        openCost = EventInfoMgr.Instance.lstBossRush[0].u1OpenMaxCount;
                    txtRetry.text = openCost.ToString();
                    break;

                case 2:
                    objHighLight.SetActive(false);
                    objDisable.SetActive(true);
                    objLock.SetActive(false);
                    imgStageEmblem.gameObject.SetActive(true);
                    imgStageEmblem.sprite = AtlasMgr.Instance.GetSprite("Sprites/Event/event_02.emblem_complete");
                    txtStageText.text = TextManager.Instance.GetText("event_bossrush_stage_success");
                    txtStageText.gameObject.GetComponent<Outline>().effectColor = OpenColor[0];
                    this.GetComponent<Button>().interactable = false;
                    objRetry.SetActive(false);
                    objFailSubText.SetActive(false);
                    break;
            }
        }
        else
        {
            if(_stageInfo.u1OpenType == StageInfoMgr.Instance.OpenBossRush)
            {
                objHighLight.SetActive(true);
                objDisable.SetActive(false);
                objLock.SetActive(false);
                imgStageEmblem.gameObject.SetActive(false);
                this.GetComponent<Button>().interactable = true;
                objRetry.SetActive(false);
                objFailSubText.SetActive(false);
            }
            else if(_stageInfo.u1OpenType < StageInfoMgr.Instance.OpenBossRush)
            {
                objHighLight.SetActive(false);
                objDisable.SetActive(true);
                objLock.SetActive(false);
                imgStageEmblem.gameObject.SetActive(true);
                imgStageEmblem.sprite = AtlasMgr.Instance.GetSprite("Sprites/Event/event_02.emblem_fail");
                txtStageText.text = TextManager.Instance.GetText("event_bossrush_stage_fail");
                txtStageText.gameObject.GetComponent<Outline>().effectColor = OpenColor[1];
                this.GetComponent<Button>().interactable = true;
                objRetry.SetActive(true);
                objFailSubText.SetActive(true);
                int openCost = 0;
                if(index >= 0)
                    openCost = EventInfoMgr.Instance.lstBossRush[0].u2OpenCount + (Legion.Instance.cEvent.openStageIds[index].u1PlayCount*EventInfoMgr.Instance.lstBossRush[0].u1OpenRetryUpCount);
                else
                    openCost = EventInfoMgr.Instance.lstBossRush[0].u2OpenCount;
                if(openCost > EventInfoMgr.Instance.lstBossRush[0].u1OpenMaxCount)
                    openCost = EventInfoMgr.Instance.lstBossRush[0].u1OpenMaxCount;
                txtRetry.text = openCost.ToString();
            }
            else
            {
                objHighLight.SetActive(false);
                objDisable.SetActive(true);
                objLock.SetActive(true);
                imgStageEmblem.gameObject.SetActive(false);
                this.GetComponent<Button>().interactable = false;
                objRetry.SetActive(false);
                objFailSubText.SetActive(false);
            }
        }
    }
}
