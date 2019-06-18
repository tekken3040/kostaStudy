using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
public class CharRoomSlotData
{
	public int index;
	public CharTrainingInfo charTrainingInfo;
}

// 수련의방 목록 슬롯
public class CharRoomSlot : MonoBehaviour, ISlot<CharRoomSlotData>{

	public delegate void OnClickSlot(UInt16 roomID);
	
	public Button button;
	public OnClickSlot onClickSlot;
	public GameObject roomLock;
	public GameObject alramMark;
	public Text roomName;
    public Text remainTime;
	
	private CharRoomSlotData slotData;
	private bool locked;
	
	public void InitSlot(CharRoomSlotData charRoomData)
	{
		slotData = charRoomData;
        CheckLock();
        
        if (slotData.charTrainingInfo.timeType > 0)
            StartCoroutine("CheckTime");
        else
        {
            alramMark.SetActive(false);
            remainTime.gameObject.SetActive(false);
        }
	}
    
    public void RefreshSlot()
    {
        CheckLock();
        
        if(slotData.charTrainingInfo.timeType > 0)
        {
            StopCoroutine("CheckTime");
            StartCoroutine("CheckTime");
        }
        else
        {
            StopCoroutine("CheckTime");
            alramMark.SetActive(false);
            remainTime.gameObject.SetActive(false);
        }
    }
    
    void OnEnable()
    {
        if(slotData == null)
            return;
            
        RefreshSlot();
    }
    
    // 열렸는지 닫혔는지 처리
    public void CheckLock()
    {
        roomName.text = string.Format(TextManager.Instance.GetText("popup_title_tra_char"), slotData.index + 1);        		
		//string imagePath = "Sprites/Quest/Quest_01.training_"+(slotData.index%10 + 1);
		string imagePath = "Sprites/Quest/Quest_02.weapon_"+((int)(slotData.index/2)%5 + 1);
        
		locked = (Legion.Instance.charTrainingRoom[slotData.index] > 0) ? false : true;
        
        if(locked)
        {
		  roomLock.SetActive(true);		
		  imagePath += "_dis";
//			button.interactable = false;
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
		return TextManager.Instance.GetText(QuestInfoMgr.Instance.GetAchieveNameByRewardID (slotData.charTrainingInfo.u2ID));
	}
	
	public void OnClickEvent()
	{
		if (locked) {
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("open_terms_title"), GetUnlockAchieve () + "\n" +TextManager.Instance.GetText("open_terms_desc"), null);
			return;
		}

		if(onClickSlot != null)
			onClickSlot(slotData.charTrainingInfo.u2ID);
	}
    
    // 남은 시간 처리
    private IEnumerator CheckTime()
    {
        while(true)
        {                        
            remainTime.gameObject.SetActive(true);
            
            TimeSpan timeSpan = slotData.charTrainingInfo.doneTime - Legion.Instance.ServerTime;
			
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
                
                if(slotData.charTrainingInfo.timeType != 0)
                    alramMark.SetActive(true);
			}
            
            yield return new WaitForSeconds(1f);						   
        }
    }
}
