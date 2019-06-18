using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
public class EquipRoomSlotData
{
	public int index;
	public EquipTrainingInfo equipTrainingInfo;
}

//병기 훈련 리스트 슬롯
public class EquipRoomSlot : MonoBehaviour, ISlot<EquipRoomSlotData>{

	public delegate void OnClickSlot(UInt16 roomID);
	
	public Button button;
	public OnClickSlot onClickSlot;
	public GameObject roomLock;
	public GameObject alramMark;
	public Text roomName;
    public Text remainTime;
	
	private EquipRoomSlotData slotData;
	private bool locked;
	
	public void InitSlot(EquipRoomSlotData equipRoomData)
	{
		slotData = equipRoomData;
        CheckLock();
        
        if (slotData.equipTrainingInfo.timeType > 0)
            StartCoroutine("CheckTime");
        else
        {
            remainTime.gameObject.SetActive(false);
            alramMark.SetActive(false);
        }
	}
    
    public void RefreshSlot()
    {
        CheckLock();
        
        if(slotData.equipTrainingInfo.timeType > 0)
        {
            StopCoroutine("CheckTime");
            StartCoroutine("CheckTime");
        }
        else
        {
            StopCoroutine("CheckTime");
            remainTime.gameObject.SetActive(false);
            alramMark.SetActive(false);
        }
    }    
    
    void OnEnable()
    {
        if(slotData == null)
            return;        
        
        RefreshSlot();
    }
    
    public void CheckLock()
    {
        roomName.text = string.Format(TextManager.Instance.GetText("popup_title_tra_equip"), slotData.index + 1);        		
		string imagePath = "Sprites/Quest/Quest_02.weapon_"+((int)(slotData.index/2)%5 + 1);
        
		locked = (Legion.Instance.equipTrainingRoom[slotData.index] > 0) ? false : true;
        
        if(locked)
        {
		  roomLock.SetActive(true);		
		  imagePath += "_dis";
//          button.interactable = false;
		  button.interactable = true;
          roomName.color = Color.gray;
        }
        else
        {
          roomLock.SetActive(false);		
          button.interactable = true;
          roomName.color = Color.white;  
        }
		
		button.image.sprite = AtlasMgr.Instance.GetSprite(imagePath);				
    }
	
	string GetUnlockAchieve(){
		return TextManager.Instance.GetText(QuestInfoMgr.Instance.GetAchieveNameByRewardID (slotData.equipTrainingInfo.u2ID));
	}

	public void OnClickEvent()
	{
		if (locked) {
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("open_terms_title"), GetUnlockAchieve () + "\n" + TextManager.Instance.GetText("open_terms_desc"), null);
			return;
		}

		if(onClickSlot != null)
			onClickSlot(slotData.equipTrainingInfo.u2ID);
	}
    
    private IEnumerator CheckTime()
    {
        while(true)
        {                        
            remainTime.gameObject.SetActive(true);
            
            TimeSpan timeSpan = slotData.equipTrainingInfo.doneTime - Legion.Instance.ServerTime;
			
			if(timeSpan.Ticks > 0)
			{
                int hour = (int)(timeSpan.TotalSeconds / 3600);
                int min = (int)((timeSpan.TotalSeconds % 3600) / 60);
                int sec = (int)((timeSpan.TotalSeconds % 3600) % 60);                
                
				remainTime.text = string.Format("{0:00}:{1:00}:{2:00}", hour, min, sec);
                
                if(alramMark.activeSelf)
                    alramMark.SetActive(false);
			}
			else
			{                
				remainTime.gameObject.SetActive(false);
                alramMark.SetActive(true);
			}
            
            yield return new WaitForSeconds(1f);   
        }
    }    
}
