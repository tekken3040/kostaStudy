using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

// 방 목록 생성 
public class TrainingRoomList : MonoBehaviour {

    public enum RoomType
    {
        Character,
        Equipment
    }
    
    public RoomType roomType;

    public ScrollRect scrollRect;

    private bool init = false;
    private GameObject slotObject;
	private List<CharRoomSlot> lstCharRoom; //= new List<CharRoomSlot>();
	private List<EquipRoomSlot> lstEquipRoom; //= new List<EquipRoomSlot>();
    private TrainingWindow trainingWindow;
    private TrainingProgressWindow trainingProgressWindow;
    private UInt16 selectedRoomID = 0;
    
    public void Init()
    {
        if(roomType == RoomType.Character)
            InitChar();
        else
            InitEquip();
    }
    
    private void InitChar()
    {
        if(init)
        {
            scrollRect.StopMovement();
            scrollRect.horizontalNormalizedPosition = 0f;            
            return;
        }
		else
		{
			lstCharRoom = new List<CharRoomSlot>();
		}
        
        if(slotObject == null)
            slotObject = AssetMgr.Instance.AssetLoad("Prefabs/UI/Quest/CharRoomSlot.prefab", typeof(GameObject)) as GameObject;
        
        Dictionary<UInt16, CharTrainingInfo> dicCharTraining = QuestInfoMgr.Instance.GetCharTrainingInfo();
        
        int index = 0;
        foreach(CharTrainingInfo info in dicCharTraining.Values)
        {
            CharRoomSlot charRoomSlot = Instantiate(slotObject).GetComponent<CharRoomSlot>();
            charRoomSlot.transform.SetParent(scrollRect.content);
            charRoomSlot.transform.localPosition = Vector3.zero;
            charRoomSlot.transform.localScale = Vector3.one;
            
            CharRoomSlotData charRoomSlotData = new CharRoomSlotData();
            charRoomSlotData.index = index;
            charRoomSlotData.charTrainingInfo = info;
            
			charRoomSlot.InitSlot(charRoomSlotData);
            charRoomSlot.onClickSlot = OnClickCharacter;
            
            lstCharRoom.Add(charRoomSlot);
            
            index++;
        }
        
        init = true;
    }
    
    private void InitEquip()
    {
        if(init)
        {
            scrollRect.StopMovement();
            scrollRect.horizontalNormalizedPosition = 0f;            
            return;
        }
		else
		{
			lstEquipRoom = new List<EquipRoomSlot>();
		}
        
        if(slotObject == null)
            slotObject = AssetMgr.Instance.AssetLoad("Prefabs/UI/Quest/EquipRoomSlot.prefab", typeof(GameObject)) as GameObject;
        
        int index = 0;
		Dictionary<UInt16, EquipTrainingInfo> dicEquipTraining = QuestInfoMgr.Instance.GetEquipTrainingInfo();
        foreach(EquipTrainingInfo info in dicEquipTraining.Values)
        {
            EquipRoomSlot equipRoomSlot = Instantiate(slotObject).GetComponent<EquipRoomSlot>();
            equipRoomSlot.transform.SetParent(scrollRect.content);
            equipRoomSlot.transform.localPosition = Vector3.zero;
            equipRoomSlot.transform.localScale = Vector3.one;
            
            EquipRoomSlotData equipRoomSlotData = new EquipRoomSlotData();
            equipRoomSlotData.index = index;
            equipRoomSlotData.equipTrainingInfo = info;
            
            equipRoomSlot.InitSlot(equipRoomSlotData);
            equipRoomSlot.onClickSlot = OnClickEquip;
            
            lstEquipRoom.Add(equipRoomSlot);
            
            index++;
        }
        
        init = true;        
    }
    
    public void RefreshSlot()
    {
        if(roomType == RoomType.Character)
        {
            for(int i=0; i<lstCharRoom.Count; i++)
            {
                lstCharRoom[i].RefreshSlot();
            }
        }
        else
		{
            for(int i=0; i<lstEquipRoom.Count; i++)
            {
                lstEquipRoom[i].RefreshSlot();
            }
        }        
    }
    
    public void OnClickCharacter(UInt16 id)
    {
        selectedRoomID = id;
        
        Dictionary<UInt16, CharTrainingInfo> dicCharTraining = QuestInfoMgr.Instance.GetCharTrainingInfo();
        
        if(dicCharTraining[id].timeType > 0)
        {
            TrainingInfo info = QuestInfoMgr.Instance.GetCharTrainingInfo()[id];
            TimeSpan timeSpan = info.doneTime - Legion.Instance.ServerTime;
            
            // 진행 중
            if(timeSpan.Ticks > 0)
            {
                OpenProgressWindow(id);    
            }
            // 완료 된 경우
            else
            {
                PopupManager.Instance.ShowLoadingPopup(1);
				Server.ServerMgr.Instance.CharEndTraining((Byte)(id - Server.ConstDef.BaseCharTrainingID), info, AckCharTrainingFinish);
            }
        }
        else
        {
            OpenWindow(id);
        }
    }
    
    public void OnClickEquip(UInt16 id)
    {
        selectedRoomID = id;
        
        Dictionary<UInt16, EquipTrainingInfo> dicEquipTraining = QuestInfoMgr.Instance.GetEquipTrainingInfo();
        
        if(dicEquipTraining[id].timeType > 0)
        {
            TrainingInfo info = QuestInfoMgr.Instance.GetEquipTrainingInfo()[id];
            TimeSpan timeSpan = info.doneTime - Legion.Instance.ServerTime;
            
            if(timeSpan.Ticks > 0)
            {
                OpenProgressWindow(id);    
            }
            else
            {
                PopupManager.Instance.ShowLoadingPopup(1);
                Server.ServerMgr.Instance.EquipEndTraining((Byte)(id - Server.ConstDef.BaseEquipTrainingID), AckCharTrainingFinish);
            }            
        }
        else
        {
            OpenWindow(id);
        }
    }
    
    // 진행 중 팝업
    private void OpenProgressWindow(UInt16 id)
    {
        if(trainingProgressWindow == null)
        {
            GameObject windowPref = AssetMgr.Instance.AssetLoad("Prefabs/UI/Quest/TrainingProgressWindow.prefab", typeof(GameObject)) as GameObject;
            trainingProgressWindow = Instantiate(windowPref).GetComponent<TrainingProgressWindow>();
            
            RectTransform rectTransform = trainingProgressWindow.GetComponent<RectTransform>();
            
            rectTransform.SetParent(transform.parent);
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.sizeDelta = Vector3.zero;
            rectTransform.localScale = Vector3.one; 
        }
        
        trainingProgressWindow.gameObject.SetActive(true);
        trainingProgressWindow.roomListObject = gameObject;
        trainingProgressWindow.SetWindow(id, (roomType == RoomType.Character) ? false : true);
        
		PopupManager.Instance.AddPopup(trainingProgressWindow.gameObject, trainingProgressWindow.OnClickClose);
    }
    
    // 훈련 창 오픈
    private void OpenWindow(UInt16 id)
    {
        if(trainingWindow == null)
        {
            GameObject windowPref = AssetMgr.Instance.AssetLoad("Prefabs/UI/Quest/TrainingWindow.prefab", typeof(GameObject)) as GameObject;
            trainingWindow = Instantiate(windowPref).GetComponent<TrainingWindow>();
            
            RectTransform rectTransform = trainingWindow.GetComponent<RectTransform>();

			if (roomType == RoomType.Character)
				trainingWindow.onClose = OnClickCharacter;
			else
				trainingWindow.onClose = OnClickEquip;
            
            rectTransform.SetParent(transform.parent);
            rectTransform.anchoredPosition3D = Vector3.zero;
            rectTransform.sizeDelta = Vector3.zero;
            rectTransform.localScale = Vector3.one; 
        }
        
        trainingWindow.gameObject.SetActive(true);    
        trainingWindow.roomListObject = gameObject;  
        trainingWindow.SetWindow(id, (roomType == RoomType.Character) ? false : true);  

		PopupManager.Instance.AddPopup(trainingWindow.gameObject, trainingWindow.OnClickClose);
    }
    
    // 완료 결과 처리
    private void AckCharTrainingFinish(Server.ERROR_ID err)
	{
        DebugMgr.Log(err);
        
		PopupManager.Instance.CloseLoadingPopup();
		
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CHAR_END_TRAINING, err), Server.ServerMgr.Instance.CallClear);
		}
		else if(err == Server.ERROR_ID.NONE)
		{            
            OpenProgressWindow(selectedRoomID);
            RefreshSlot();

            PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.CharLevel);
            PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.Training);
            PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.EquipLevel);
        }        
    }    
}
