using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Text;

public class ActSlotData
{
	public int index;
	public ActInfo actInfo;
}

public class ActSlot : MonoBehaviour, ISlot<ActSlotData>{
	
	public delegate void OnClickAct(UInt16 actID);
	
	public Button button;
	public OnClickAct onClickAct;
	public GameObject actLock;
	public GameObject alramMark;
	public GameObject questMark;
	public Text actName;
	public GameObject helpMark;
	public Color lockTextColor;
	
	private ActSlotData slotData;
	private bool locked;
	
	public void InitSlot(ActSlotData actData)
	{
        StringBuilder tempString = new StringBuilder();
		slotData = actData;
		
		locked = !actData.actInfo.CheckActOpen();
		actLock.SetActive(locked);
		actName.text = TextManager.Instance.GetText(actData.actInfo.strName);
		
		tempString.Append("Sprites/Campaign/").Append(actData.actInfo.strImagePath).ToString();

        if (locked == true)
        {
            tempString.Append("off");
            actName.color = lockTextColor;
        }
        else
            tempString.Append("on");
		
		button.image.sprite = AtlasMgr.Instance.GetSprite(tempString.ToString());
		button.interactable = !locked;		
		            
        alramMark.SetActive(false);                
        
		// 2016. 6. 29 jy
		// 탑 모드는 별과 반복 보상이 없으므로 탑 모드가 아닐때만 검사한다
		if( slotData.actInfo.u1Mode != ActInfo.ACT_TYPE.TOWER)
		{
	        // 별 or 반복 보상 수령 가능시 느낌표 처리
	        for(int i=0; i<actData.actInfo.lstChapterID.Count; i++)
	        {
	            if(StageInfoMgr.Instance.dicChapterData[actData.actInfo.lstChapterID[i]].RewardEnable())
	            {
	                alramMark.SetActive(true);
	                break;
	            }
	        }
		}
        tempString.Remove(0, tempString.Length);
        gameObject.AddComponent<TutorialButton>().id = tempString.Append("Act").Append(actData.actInfo.u1Number).ToString();

		questMark.SetActive(false);

		if(Legion.Instance.cQuest.CheckQuestAlarm(MENU.CAMPAIGN, 0)){
			bool bActive = Legion.Instance.cQuest.CheckQuestRelationInAct(slotData.actInfo);
			questMark.SetActive(bActive);
		}
	}
	
	public void OnClickEvent()
	{
		if(locked)
			return;

		if(onClickAct != null)
			onClickAct(slotData.actInfo.u2ID);
	}
}
