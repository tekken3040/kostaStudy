using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

[System.Serializable]
public class SeatInfo
{
    public GameObject slot;
    public Image portrait;
    public Image grade;
    public Image element;
    public Text level;    
    public GameObject levelUpMark;    
    public bool full;

	public SeatInfo(Transform tr){
		slot = tr.gameObject;
		portrait = tr.FindChild ("Portrait").GetComponent<Image> ();
		grade = tr.FindChild ("Grade").GetComponent<Image> ();
		element = tr.FindChild ("Element").GetComponent<Image> ();
		level = tr.FindChild ("Level").GetComponent<Text> ();
		levelUpMark = tr.FindChild ("Up").gameObject;
	}
}

// 병기훈련, 수련의방 진행 중을 보여주는 창
public class TrainingProgressWindow : MonoBehaviour {

    public GameObject roomListObject;
    public Text title;
	public GameObject[] seats;
    SeatInfo[] seatInfos;
    public Image timeGague;    
    public Text timeText;
    public Text remainTime;
	public Text _textExpTitle;
    public Text exp;
    public GameObject btnCacel;
    public GameObject btnDone;
    public GameObject btnOk;
    
    public GameObject doneWindow;
    public GameObject cancelWindow;
    public Text donePrice;
    
    private UInt16 roomID;
    private bool isEquip;
    private float remainPer;
    
    // 장비인지 캐릭터인지에 따라 정보 설정
    public void SetWindow(UInt16 roomID, bool isEquip)
    {
		seatInfos = new SeatInfo[seats.Length];
		for (int i = 0; i < seats.Length; i++) {
			seatInfos [i] = new SeatInfo (seats [i].transform);
		}

        this.isEquip = isEquip;
        this.roomID = roomID;
        
        TrainingInfo info = null;
        
        bool trainingDone = false;
        for(int i=0; i<seatInfos.Length; i++)
		{
            seatInfos[i].full = false;
		}
        if(!isEquip)
        {
            info = QuestInfoMgr.Instance.GetCharTrainingInfo()[roomID];
			int roomIdx = roomID - Server.ConstDef.BaseCharTrainingID;
            // 영웅 정보 설정
            for(int i=0; i<Legion.Instance.acHeros.Count; i++)
            {
                if(Legion.Instance.acHeros[i].u1RoomNum == roomIdx)
                {
                    Hero hero = Legion.Instance.acHeros[i];
                    Byte seatNum = (Byte)(hero.u1SeatNum - 1);

                    seatInfos[seatNum].slot.SetActive(true);
                    seatInfos[seatNum].portrait.sprite = AtlasMgr.Instance.GetSprite("Sprites/Hero/hero_icon." + hero.cClass.u2ID);
					seatInfos[seatNum].element.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + EquipmentInfoMgr.Instance.GetInfo(hero.acEquips[6].cItemInfo.u2ID).u1Element);
                    seatInfos[seatNum].level.text = hero.cLevel.u2Level.ToString();
                    seatInfos[seatNum].levelUpMark.gameObject.SetActive(false);
                    seatInfos[seatNum].portrait.SetNativeSize();
                    seatInfos[seatNum].full = true;
                }
            }
            /*
            // 채워진 영웅에 따라 슬롯 온 오프
            for(int i=0; i<seatInfos.Length; i++)
                seatInfos[i].slot.gameObject.SetActive(seatInfos[i].full);
            
            // 골드 경험치 정보 (선택한 시간에 따라 달라짐)
            gold.text = info.arrRewardGoods[info.timeType-1].u4Count.ToString();
            exp.text = info.arrExp[info.timeType-1].ToString();
            
            // 남은 시간을 확인해 완료인지 진행중인지 판단
            TimeSpan timeSpan = info.doneTime - Legion.Instance.ServerTime;
			
			trainingDone = (timeSpan.Ticks > 0) ? false : true;
			*/
            title.text = string.Format(TextManager.Instance.GetText("popup_title_tra_char"), roomIdx);
			_textExpTitle.text = TextManager.Instance.GetText("popup_mark_tra_char_exp");

        }
        else
        {
            info = QuestInfoMgr.Instance.GetEquipTrainingInfo()[roomID];
            int roomIdx = roomID - Server.ConstDef.BaseEquipTrainingID;
            Legion.Instance.cInventory.EquipSort();
            // 장비 정보 설정
            for(int i=0; i<Legion.Instance.cInventory.lstSortedEquipment.Count; i++)
            {
                if(Legion.Instance.cInventory.lstSortedEquipment[i].u1RoomNum == roomIdx)
                {
                    EquipmentItem equipmentItem = Legion.Instance.cInventory.lstSortedEquipment[i];
                    ModelInfo modelInfo = ModelInfoMgr.Instance.GetInfo(equipmentItem.GetEquipmentInfo().u2ModelID);
					ForgeInfo forgeInfo = ForgeInfoMgr.Instance.GetList () [equipmentItem.u1SmithingLevel - 1];

                    Byte seatNum = (Byte)(equipmentItem.u1SeatNum - 1);
                    if (modelInfo != null)
                    {
                        string imagePath = "Sprites/Item/" + modelInfo.sImagePath;                                    
                        seatInfos[seatNum].portrait.sprite = AtlasMgr.Instance.GetSprite(imagePath);
                    }
                    seatInfos[seatNum].portrait.transform.localScale = Vector3.one;                                               
                    seatInfos[seatNum].slot.SetActive(true);
					seatInfos[seatNum].element.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + equipmentItem.GetEquipmentInfo().u1Element);
                    seatInfos[seatNum].level.text = equipmentItem.cLevel.u2Level.ToString();
                    seatInfos[seatNum].levelUpMark.gameObject.SetActive(false);
					seatInfos[seatNum].grade.sprite = AtlasMgr.Instance.GetSprite(" Sprites/Common/common_02_renew.grade_" + forgeInfo.u2ID);
                    seatInfos[seatNum].portrait.SetNativeSize();
                    seatInfos[seatNum].full = true;
                }                
            }
            title.text = string.Format(TextManager.Instance.GetText("popup_title_tra_equip"), roomIdx);
			_textExpTitle.text = TextManager.Instance.GetText("popup_mark_tra_equip_exp");
        }

		int nCount = 0;
		for(int i=0; i<seatInfos.Length; i++)
		{
			seatInfos[i].slot.gameObject.SetActive(seatInfos[i].full);
			if(seatInfos[i].full == true)
				++nCount;
		}
		exp.text = info.arrExp[info.timeType-1].ToString();

		TimeSpan timeSpan = info.doneTime - Legion.Instance.ServerTime;
		trainingDone = (timeSpan.Ticks > 0) ? false : true;
        
        // 완료 된 경우
        if(trainingDone)
        {
            btnCacel.gameObject.SetActive(false);
            btnDone.gameObject.SetActive(false);
            btnOk.gameObject.SetActive(true);
            timeText.gameObject.SetActive(false);
            timeGague.fillAmount = 1f;
            remainTime.text = TextManager.Instance.GetText("mark_icon_quest_done");
            
            if(!isEquip)
            {
                bool isLavelUp = false;
                info = QuestInfoMgr.Instance.GetCharTrainingInfo()[roomID];
                int roomNum = info.u2ID - Server.ConstDef.BaseCharTrainingID;                
                for(int i=0; i<Legion.Instance.acHeros.Count; i++)
                {
                    Hero hero = Legion.Instance.acHeros[i];
                    // 경험치 추가
                    if(hero.u1RoomNum == roomNum)
                    {
                        if (hero.GetComponent<LevelComponent>().AddExp(info.arrExp[info.timeType - 1]) > 0)
                        {
                            seatInfos[hero.u1SeatNum - 1].levelUpMark.gameObject.SetActive(true);
                            isLavelUp = true;
                        }

						// 캐릭터가 착용한 아이템의 경험치를 올린다
						for(int j = 0; j < hero.acEquips.Length; ++j)
						{
                            if (hero.acEquips[j].GetComponent<LevelComponent>().AddExp(info.arrExp[info.timeType - 1]) > 0)
                            {
                                isLavelUp = true;
                            }
						}
                        seatInfos[hero.u1SeatNum - 1].level.text = hero.cLevel.u2Level.ToString();
                        hero.u1RoomNum = 0;
                        hero.u1SeatNum = 0;
                    }
                }
                
                info.timeType = 0;
                info.doneTime = Legion.Instance.ServerTime;

                QuestScene questScene = Scene.GetCurrent() as QuestScene;
                // 느낌표 갱신
                if(questScene != null)
					questScene.CheckCharTrainingAlram();
            }
            else
            {
                info = QuestInfoMgr.Instance.GetEquipTrainingInfo()[roomID];
                int roomNum = info.u2ID - Server.ConstDef.BaseEquipTrainingID;                
                foreach(Item item in Legion.Instance.cInventory.dicInventory.Values)
                {
                    if(item.cItemInfo.ItemType == ItemInfo.ITEM_TYPE.EQUIPMENT)
                    {
                        if(item.u1RoomNum == info.u2ID - Server.ConstDef.BaseEquipTrainingID)
                        {
                            // 경험치 추가
                            EquipmentItem equipmentItem = (EquipmentItem)item;
                            if(equipmentItem.GetComponent<LevelComponent>().AddExp(info.arrExp[info.timeType - 1]) > 0)
                            {
                                seatInfos[item.u1SeatNum-1].levelUpMark.gameObject.SetActive(true);
                            }
                            
                            seatInfos[item.u1SeatNum  - 1].level.text = equipmentItem.cLevel.u2Level.ToString();
                            item.u1RoomNum = 0;
                            item.u1SeatNum = 0;
                        }                            
                    }
                }
                
                info.timeType = 0;
                info.doneTime = Legion.Instance.ServerTime;
                // 
                //QuestScene questScene = Scene.GetCurrent() as QuestScene;
                //if(questScene != null)
                //    questScene.CheckEquipTrainingAlram();
            }
        }
        else
        {
            // 완료 중이 아닐 경우 남은 시간 표시
            timeText.gameObject.SetActive(true);
            btnCacel.gameObject.SetActive(true);
            btnDone.gameObject.SetActive(true);
            btnOk.gameObject.SetActive(false);
            StartCoroutine(CheckTime());    
        }
    }
    
    private IEnumerator CheckTime()
    {
        while(true)
        {
            TrainingInfo info = null;
            int trainingTime = 0;
            if(!isEquip)
            {
                info = QuestInfoMgr.Instance.GetCharTrainingInfo()[roomID];                
                trainingTime = info.arrTrainingTime[info.timeType - 1];
            }
            else
            {
                info = QuestInfoMgr.Instance.GetEquipTrainingInfo()[roomID];
                trainingTime = info.arrTrainingTime[info.timeType - 1];
            }
                        
            TimeSpan timeSpan = info.doneTime - Legion.Instance.ServerTime;
			if(timeSpan.Ticks > 0)
			{
                int hour = (int)(timeSpan.TotalSeconds / 3600);
                int min = (int)((timeSpan.TotalSeconds % 3600) / 60);
                int sec = (int)((timeSpan.TotalSeconds % 3600) % 60);                
                
				remainTime.text = string.Format("{0:00}:{1:00}:{2:00}", hour, min, sec);                                
                
                remainPer = ((float)timeSpan.TotalMinutes / (float)(trainingTime));
                timeGague.fillAmount = 1f - remainPer;
			}
			else
			{
				remainTime.gameObject.SetActive(false);
			}
			
			yield return new WaitForSeconds(1f);   
        }
    }
    
    // 취소창 오픈
    public void OpenCancel()
    {
        cancelWindow.SetActive(true);
    }
    
    // 즉시 완료 창 오픈
    public void OpenDone()
    {
        doneWindow.SetActive(true);
        donePrice.text = GetPrice().ToString();
    }
    
    // 남은 시간에 따라 즉시 완료 가격이 달라진다
    private int GetPrice()
    {
        int price = 0;
        
        TrainingInfo info = null;
        
        if(!isEquip)
        {
            info = QuestInfoMgr.Instance.GetCharTrainingInfo()[roomID];
            float time = info.arrTrainingTime[info.timeType - 1] / 60f;
            price = Mathf.FloorToInt(info.doneGoods.u4Count * time * remainPer);
        }
        else
        {
            info = QuestInfoMgr.Instance.GetEquipTrainingInfo()[roomID];
            float time = info.arrTrainingTime[info.timeType - 1] / 60f;
            price = Mathf.FloorToInt(info.doneGoods.u4Count * time * remainPer);
        }
        
        return price;
    }
    
    public void CloseCancel()
    {
        cancelWindow.SetActive(false);
    }
    
    public void CloseDone()
    {
        doneWindow.SetActive(false);
    }
    
    public void OnClickClose()
    {
        gameObject.SetActive(false);
		PopupManager.Instance.RemovePopup(gameObject);

        //if (!isEquip)
        //{
        //    PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.Training);
        //    //Legion.Instance.cQuest.CheckEndDirection(AchievementTypeData.Training);
        //}
        //else
        //{
        //    PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.EqTraining);
        //    //Legion.Instance.cQuest.CheckEndDirection(AchievementTypeData.EqTraining);
        //}
    }
    
    // 즉시 완료 요청
    public void OnClickDone()
    {
        TrainingInfo info = null;
        
        if(!isEquip)
            info = QuestInfoMgr.Instance.GetCharTrainingInfo()[roomID];
        else
            info = QuestInfoMgr.Instance.GetEquipTrainingInfo()[roomID];
        
        // 캐쉬 부족시 불가능
        if(!Legion.Instance.CheckEnoughGoods(info.doneGoods.u1Type, GetPrice()))
        {
			PopupManager.Instance.ShowChargePopup(info.doneGoods.u1Type);            
            return;
        }
        
        PopupManager.Instance.ShowLoadingPopup(1);       
        if(!isEquip)
            Server.ServerMgr.Instance.CharFinishTraining((Byte)(roomID - Server.ConstDef.BaseCharTrainingID), (UInt32)GetPrice(), QuestInfoMgr.Instance.GetCharTrainingInfo()[roomID], AckFinishTraining);
        else
            Server.ServerMgr.Instance.EquipFinishTraining((Byte)(roomID - Server.ConstDef.BaseEquipTrainingID), (UInt32)GetPrice(), QuestInfoMgr.Instance.GetEquipTrainingInfo()[roomID], AckFinishTraining);
            
        CloseDone();
        OnClickClose();    
    }
    
    // 취소 요청
    public void OnClickCancel()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        
        if(!isEquip)
            Server.ServerMgr.Instance.CharCancelTraining((Byte)(roomID - Server.ConstDef.BaseCharTrainingID), AckCancelTraining);
        else
            Server.ServerMgr.Instance.EquipCancelTraining((Byte)(roomID - Server.ConstDef.BaseEquipTrainingID), AckCancelTraining);
    }
    
    private void AckCancelTraining(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        
        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EQUIP_CANCEL_TRAINING, err), Server.ServerMgr.Instance.CallClear);   
        }
        else
        {
            TrainingInfo info = null;
            
            if(!isEquip)
            {
                // 해당 시간, 방, 자리 정보 초기화
                info = QuestInfoMgr.Instance.GetCharTrainingInfo()[roomID];
                info.timeType = 0;
                info.doneTime = Legion.Instance.ServerTime;
                for(int i=0; i<Legion.Instance.acHeros.Count; i++)
                {
                    Hero hero = Legion.Instance.acHeros[i];
                    
                    if(hero.u1RoomNum == info.u2ID - Server.ConstDef.BaseCharTrainingID)
                    {
                        hero.u1RoomNum = 0;
                        hero.u1SeatNum = 0;
                    }
                }
            }
            else
            {
                info = QuestInfoMgr.Instance.GetEquipTrainingInfo()[roomID];
                info.timeType = 0;
                info.doneTime = Legion.Instance.ServerTime;
                
                foreach(Item item in Legion.Instance.cInventory.dicInventory.Values)
                {
                    if(item.u1RoomNum == info.u2ID - Server.ConstDef.BaseEquipTrainingID)
                    {
                        item.u1RoomNum = 0;
                        item.u1SeatNum = 0;
                    }
                }
            }
            
            CloseCancel();
            OnClickClose();
            roomListObject.GetComponent<TrainingRoomList>().RefreshSlot();            
        }
    }
    
    // 훈련 결과 처리
    private void AckFinishTraining(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();

        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CHAR_FINISH_TRAINING, err), Server.ServerMgr.Instance.CallClear);   
        }
        else
        {
            TrainingInfo info = null;
            if(!isEquip)
            {
                info = QuestInfoMgr.Instance.GetCharTrainingInfo()[roomID];
                //int roomNum = info.u2ID - Server.ConstDef.BaseCharTrainingID;;
            }
            else
            {
                info = QuestInfoMgr.Instance.GetEquipTrainingInfo()[roomID];
                //int roomNum = info.u2ID - Server.ConstDef.BaseEquipTrainingID;
            }

			// 2016. 10. 18 jy
			// 재화 소모 보상 없어짐
			//Legion.Instance.AddGoods(info.arrRewardGoods[info.timeType-1]);
			info.doneTime = Legion.Instance.ServerTime;
            gameObject.SetActive(true);
                        
            SetWindow(roomID, isEquip); //경험치는 여기서 줌
            roomListObject.GetComponent<TrainingRoomList>().RefreshSlot(); // 룸 리스트 갱신

            PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.CharLevel);
            PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.Training);
            PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.EquipLevel);
        }
    }    
}
