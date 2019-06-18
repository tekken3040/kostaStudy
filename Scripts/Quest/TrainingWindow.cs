using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
/*
[System.Serializable]
public class SelectedTrainingSlot
{
    public GameObject slot;
    public GameObject area;
    public Image portrait;
    public Image element;
	public Image grade;
    public Text name;
    public Text level;
    public Text exp;
    public Image expGague;
    public GameObject lockMark;
    public GameObject emptyMark;
    public GameObject levelupMark;
    public int seatType;
    public int slotIndex;

	public void Init(RectTransform tSlot){
		slot = tSlot.FindChild ("Slot").gameObject;
		area = tSlot.FindChild ("Area").gameObject;
		portrait = tSlot.FindChild ("Slot").FindChild ("Portrait").GetComponent<Image> ();
		element = tSlot.FindChild ("Slot").FindChild ("Element").GetComponent<Image> ();
		grade = tSlot.FindChild ("Slot").FindChild ("Grade").GetComponent<Image> ();
		name = tSlot.FindChild ("Slot").FindChild ("Name").GetComponent<Text> ();
		level = tSlot.FindChild ("Slot").FindChild ("Level").GetComponent<Text> ();
		exp = tSlot.FindChild ("Slot").FindChild ("Exp").GetComponent<Text> ();
		expGague = tSlot.FindChild ("Slot").FindChild ("ExpGague").GetComponent<Image> ();
		lockMark = tSlot.FindChild ("Lock").gameObject;
		emptyMark = tSlot.FindChild ("Empty").gameObject;
		levelupMark = tSlot.FindChild ("Slot").FindChild ("Up").gameObject;
	}
}
*/
// 훈련창
public class TrainingWindow : MonoBehaviour {
	public delegate void CloseWindow(UInt16 roomID);
	public CloseWindow onClose; 

    public GameObject roomListObject;
    public Text title;
    public Text[] timeText;
	//public RectTransform[] Slots;
    //public SelectedTrainingSlot[] selectedSlots;

	public RectTransform _selectSlotParents;
	protected List<SelectTrainingSlot> selectSlots = new List<SelectTrainingSlot>();
	protected GameObject _pefSelectSlot;

	public Text expTitleText;
    public Text gold;
    public Text exp;
    public ScrollRect scollRect;
    //public bool scrollNow = false;
    public Toggle timeToggle;
    public GameObject dragEffect;
    
    private bool isEquip;
    private UInt16 roomID;
    private int selectedTimeIndex = 0;
    private GameObject slotObject;
    private List<TrainingSlot> lstSlots = new List<TrainingSlot>();
    
    private List<Byte> lstChar;
    private List<UInt16> lstEquip;
    private List<Byte> lstSeat;

    private TrainingSeatUnlock seatUnlockWindow;
	private TrainingHeroEquipWindow _cHeroEquipWindow;
    
    private bool equipLimitLevel = false;
	private UInt32 _nTrainingCount;
    
    public GameObject SlotObject()
    {
        return slotObject;
    }
    
    public bool IsEquip
    {
        get { return isEquip; }
    }
    
    public void SetWindow(UInt16 id, bool isEquip)
    {
		_nTrainingCount = 0;
        this.isEquip = isEquip;
        
        if(slotObject == null)
            slotObject = AssetMgr.Instance.AssetLoad("Prefabs/UI/Quest/TrainingSlot.prefab", typeof(GameObject)) as GameObject;

		if(_pefSelectSlot == null)
			_pefSelectSlot = AssetMgr.Instance.AssetLoad("Prefabs/UI/Quest/SelectTrainingSlot.prefab", typeof(GameObject)) as GameObject;

		selectedTimeIndex = 0;
        timeToggle.isOn = true;
        ChangeTime(selectedTimeIndex);
        
        roomID = id;
        
        TrainingInfo info = null;
        // 룸 이름
        if(!isEquip)
        {
            info = QuestInfoMgr.Instance.GetCharTrainingInfo()[roomID];
            title.text = string.Format(TextManager.Instance.GetText("popup_title_tra_char"), roomID - Server.ConstDef.BaseCharTrainingID);
			expTitleText.text = TextManager.Instance.GetText("popup_mark_tra_char_exp");
        }
        else
        {
            info = QuestInfoMgr.Instance.GetEquipTrainingInfo()[roomID];
            title.text = string.Format(TextManager.Instance.GetText("popup_title_tra_equip"), roomID - Server.ConstDef.BaseEquipTrainingID);
			expTitleText.text = TextManager.Instance.GetText("popup_mark_tra_equip_exp");
        }
        
        // 시간 정보
        int tempTrainingTime = 0;
        for(int i=0; i<timeText.Length; i++)
        {
            tempTrainingTime = Legion.Instance.getSubVIPTrainingTime(info.arrTrainingTime[i] , isEquip);
            if(tempTrainingTime < 60)
            {
                timeText[i].text = tempTrainingTime + TextManager.Instance.GetText("mark_training_time_min");    
            }
            else
            {
                timeText[i].text = (tempTrainingTime/ 60) + TextManager.Instance.GetText("mark_training_time_hour")
                    +"\n"+ tempTrainingTime%60 + TextManager.Instance.GetText("mark_training_time_min");
            }            
        }
        
        byte[] trainingRoom;
        if(!isEquip)
            trainingRoom = Legion.Instance.charTrainingRoom;
        else
            trainingRoom = Legion.Instance.equipTrainingRoom;

		// 2016. 10. 19 jy
		// test 코드
		for(int i = 0; i < info.u1SeatCount; ++i)
		{
			if(selectSlots.Count <= i)
			{
				GameObject objSelectSlot = Instantiate(_pefSelectSlot) as GameObject;
				RectTransform rectRT = objSelectSlot.GetComponent<RectTransform>();
				rectRT.SetParent(_selectSlotParents);
				rectRT.localPosition = Vector3.zero;
				rectRT.localScale = Vector3.one;
				SelectTrainingSlot cSlot = objSelectSlot.GetComponent<SelectTrainingSlot>();
				cSlot.slotDrag.onDropSlot = OnClickSelected;
				cSlot.btnEquipOpen.onClick.AddListener(() => OpenEqiupWindow(cSlot.slotDrag.Index));
				cSlot.slotDrag.SetIndex(i, 0, 0);
				cSlot.slotDrag.trainingWindow = this;

				selectSlots.Add(cSlot);
			}

			if(i < trainingRoom[info.u1Step-1])
				selectSlots[i].SetSlotLock(false);
			else
				selectSlots[i].SetSlotLock(true);
		}

		/*
		selectedSlots = new SelectedTrainingSlot[info.u1SeatCount];
		for (int i = 0; i < Slots.Length; i++) {
			if (i < info.u1SeatCount) {
				Slots [i].gameObject.SetActive (true);
				selectedSlots [i] = new SelectedTrainingSlot ();
				selectedSlots [i].Init (Slots [i]);
			} else {
				Slots [i].gameObject.SetActive (false);
			}
		}
		 // 왼쪽 슬롯 목록 세팅
        for(int i=0; i<info.u1SeatCount; i++)
        {
            if(i < trainingRoom[info.u1Step-1])
            {
                selectedSlots[i].lockMark.SetActive(false);
                selectedSlots[i].emptyMark.SetActive(true);
                selectedSlots[i].seatType = 0;
            }
            else
            {
                selectedSlots[i].lockMark.SetActive(true);
                selectedSlots[i].emptyMark.SetActive(false);
                selectedSlots[i].seatType = -1;
            }
            
            selectedSlots[i].slot.SetActive(false);
            
            TrainingSelectedSlotDrag slotDrag = selectedSlots[i].area.GetComponent<TrainingSelectedSlotDrag>();            
            slotDrag.onDropSlot = OnClickSelected;
            slotDrag.SetIndex(i, 0, 0);
            slotDrag.trainingWindow = this;
        }
		*/
        // 골드 경험치
        gold.text = "0";
        exp.text = info.arrExp[selectedTimeIndex].ToString();
               
        if(!isEquip)
        {
            int index = 0;
            //Legion.Instance.SortAllHero();
            // 영웅 목록 생성
            for(int i=0; i<Legion.Instance.acHeros.Count; i++)
            //for(int i=0; i<Legion.Instance.lstSortedChar.Count; i++)
            {
				if(lstSlots.Count > i)
					lstSlots[i].gameObject.SetActive(false);
				
                if(Legion.Instance.acHeros[i].u1SeatNum != 0)
				{					
                //if(Legion.Instance.lstSortedChar[i].u1SeatNum != 0)
                    continue;
				}
                TrainingSlot trainingSlot = null;
                
				if(lstSlots.Count > index)
                {
					trainingSlot = lstSlots[index];
                }
                else
                {
                    GameObject item = Instantiate(slotObject) as GameObject;
                    RectTransform itemRect = item.GetComponent<RectTransform>();
                    itemRect.SetParent(scollRect.content);
                    itemRect.localPosition = Vector3.zero;
                    itemRect.localScale = Vector3.one;
                    trainingSlot = item.GetComponent<TrainingSlot>();
                    
                    lstSlots.Add(trainingSlot);
                }
                
				//2016. 08. 19jy 정렬로 인하여 인덱스 꼬임
				//int charIdx = Legion.Instance.lstSortedChar[i].u1Index-1;
				trainingSlot.SetSlot(i, index, false);
                trainingSlot.onClickSlot = OnClickSlot;
                trainingSlot.scrollRect = scollRect;
                trainingSlot.gameObject.SetActive(true);
                trainingSlot.trainingWindow = this;
                
                index++;
            }
        }
        else
        {
            int index = 0;
            
            Legion.Instance.cInventory.EquipSort();
            
            //장비 목록 생성
            for(int i=0; i<Legion.Instance.cInventory.lstSortedEquipment.Count; i++)
            {
				if(lstSlots.Count > i)
					lstSlots[i].gameObject.SetActive(false);
				
				if(Legion.Instance.cInventory.lstSortedEquipment[i].u1SeatNum != 0)
                    continue;
				                    
                TrainingSlot trainingSlot = null;
                              
				if(lstSlots.Count > index)
                {
                    trainingSlot = lstSlots[index];
                }
                else
                {
                    GameObject item = Instantiate(slotObject) as GameObject;
                    RectTransform itemRect = item.GetComponent<RectTransform>();
                    itemRect.SetParent(scollRect.content);
                    itemRect.localPosition = Vector3.zero;
                    itemRect.localScale = Vector3.one;
                    trainingSlot = item.GetComponent<TrainingSlot>();
                    
                    lstSlots.Add(trainingSlot);
                }
                
                int itemIdx = i;
				trainingSlot.SetSlot(itemIdx, index, true);
                trainingSlot.onClickSlot = OnClickSlot;
                trainingSlot.scrollRect = scollRect;
                trainingSlot.gameObject.SetActive(true);
                trainingSlot.trainingWindow = this;
                
                index++;
            }            
        }
    }
    
    // 시간 변경 처리
    public void ChangeTime(int index)
    {
        if(index == selectedTimeIndex)
            return;
        
        selectedTimeIndex = index;
        
        TrainingInfo info = null;
       
        if(!isEquip)
        {        
            info = QuestInfoMgr.Instance.GetCharTrainingInfo()[roomID];
			//*for(int i=0; i<selectedSlots.Length; i++)
			for(int i=0; i<selectSlots.Count; i++)
            {
                //* if(selectedSlots[i].seatType == 1)
				if(selectSlots[i].SeatType == 1)
                {
					selectSlots[i].SetHeroExp(info, selectedTimeIndex);
                    // 획득 예상 경험치 처리
                    /* **
                    Hero hero = Legion.Instance.acHeros[selectedSlots[i].area.GetComponent<TrainingSelectedSlotDrag>().GetItemIndex()];

					UInt16 level = hero.cLevel.u2Level;
                    UInt64 nowExp = hero.cLevel.u8Exp + info.arrExp[selectedTimeIndex]; 
                    UInt64 nextExp = ClassInfoMgr.Instance.GetNextExp(level);
                    
                    while(nowExp >= nextExp)
                    {                    
                        nowExp -= nextExp;
                        level++;
                        nextExp = ClassInfoMgr.Instance.GetNextExp(level);
                    }
                        
                    selectedSlots[i].level.text = level.ToString();
                    selectedSlots[i].exp.text = nowExp + "/" + nextExp;
					selectedSlots[i].expGague.fillAmount = (float)((float)nowExp / (float)nextExp);
                    
                    if(level > hero.cLevel.u2Level)
                        selectedSlots[i].levelupMark.SetActive(true);
                    else                                        
                        selectedSlots[i].levelupMark.SetActive(false);
                   	*/
                }
            }
  
        }
        else
        {
            info = QuestInfoMgr.Instance.GetEquipTrainingInfo()[roomID];
            //*for(int i=0; i<selectedSlots.Length; i++)
			for(int i=0; i<selectSlots.Count; i++)
            {
                //*if(selectedSlots[i].seatType == 1)
				if(selectSlots[i].SeatType == 1)
                {
					equipLimitLevel = selectSlots[i].SetEquipItemExp(info, selectedTimeIndex);
					/*
                    // 획득 예상 경험치 처리
                    EquipmentItem equipItem = Legion.Instance.cInventory.lstSortedEquipment[selectedSlots[i].area.GetComponent<TrainingSelectedSlotDrag>().GetItemIndex()];                    
                    
					UInt16 level = equipItem.cLevel.u2Level;
                    UInt64 nowExp = equipItem.cLevel.u8Exp + info.arrExp[selectedTimeIndex]; 
                    UInt64 nextExp = ClassInfoMgr.Instance.GetNextExp(level);
                    
                    while(nowExp >= nextExp)
                    {                    
                        nowExp -= nextExp;
                        level++;
                        nextExp = ClassInfoMgr.Instance.GetNextExp(level);
                    }
                        
                    selectedSlots[i].level.text = level.ToString();
                    selectedSlots[i].exp.text = nowExp + "/" + nextExp;
                    selectedSlots[i].expGague.fillAmount = (float)nowExp / (float)nextExp;
                    
                    if(level > equipItem.cLevel.u2Level)
                        selectedSlots[i].levelupMark.SetActive(true);
                    else                                        
                        selectedSlots[i].levelupMark.SetActive(false);         
                    
                    // 예상 레벨이 캐릭터 레벨보다 높아질 경우
                    if(level > Legion.Instance.TopLevel)
                        equipLimitLevel = true;
                    else
                        equipLimitLevel = false;
					*/
                }
            }            
        }
		SetTrainingCost();
        exp.text = info.arrExp[selectedTimeIndex].ToString();
    }
    
    // 드래그앤 드롭시
    private void OnClickSlot(int selectedIndex, int itemIndex, int slotIndex)
    {
        if(selectedIndex == -1)
        {
            // Hero hero = Legion.Instance.acHeros[itemIndex];               
            // object[] param = new object[2];
            // param[0] = hero.u1AssignedCrew;
            // param[1] = hero;
            
            // FadeEffectMgr.Instance.QuickChangeScene(MENU.CHARACTER_INFO, 0, param);
            return;
        }

        TrainingInfo info = null;
        if(!isEquip)
            info = QuestInfoMgr.Instance.GetCharTrainingInfo()[roomID];
        else
            info = QuestInfoMgr.Instance.GetEquipTrainingInfo()[roomID];

		int binIndex = -1;
		for(int i = 0; i < selectSlots.Count; ++i)
		{
			if(selectSlots[i].SeatType < 1)
			{
				binIndex = i;
				break;
			}
		}
		/**
		for (int i = 0; i < selectedSlots.Length; i++) {
			if (selectedSlots [i].seatType < 1) {
				binIndex = i;
				break;
			}
		}
		*/
		if(binIndex == -1)
			return;
       
        // 잠김 슬롯일 경우
        //*if(selectedSlots[binIndex].seatType == -1)
		if(selectSlots[binIndex].SeatType == -1)
        {
			SetUnlockWindow ();
            return;
        }
        else
        {
			++_nTrainingCount;
            // 드롭한 슬롯에 해당 정보를 세팅
            //*if(selectedSlots[binIndex].seatType != 0)
			if(selectSlots[binIndex].SeatType != 0)
                OnClickSelected(binIndex);

			selectSlots[binIndex].SlotIndex = slotIndex;
			selectSlots[binIndex].SetSlot(lstSlots[slotIndex]);
			selectSlots[binIndex].slotDrag.SetIndex(binIndex, itemIndex, slotIndex);                
			/*
            selectedSlots[binIndex].slot.gameObject.SetActive(true);
            selectedSlots[binIndex].lockMark.SetActive(false);
            selectedSlots[binIndex].emptyMark.SetActive(false);
            selectedSlots[binIndex].portrait.sprite = lstSlots[slotIndex].portrait.sprite;
            selectedSlots[binIndex].element.sprite = lstSlots[slotIndex].element.sprite;
            //selectedSlots[binIndex].level.text = lstSlots[slotIndex].level.text;
            selectedSlots[binIndex].portrait.SetNativeSize();
            selectedSlots[binIndex].slotIndex = slotIndex;                
            selectedSlots[binIndex].area.GetComponent<TrainingSelectedSlotDrag>().SetIndex(binIndex, itemIndex, slotIndex);                
            selectedSlots[binIndex].portrait.transform.localScale = Vector3.one;
            */
            if(!isEquip)
            {
				selectSlots[binIndex].SetHero(Legion.Instance.acHeros[itemIndex]);
				//*Hero hero = Legion.Instance.acHeros[itemIndex];
                //*selectedSlots[binIndex].name.text = hero.sName;
				//*selectedSlots[binIndex]._cHero = hero;
            }
            else
            {
                EquipmentItem equipItem = Legion.Instance.cInventory.lstSortedEquipment[itemIndex];
				ForgeInfo forgeInfo = ForgeInfoMgr.Instance.GetList () [equipItem.u1SmithingLevel - 1];

                //*selectedSlots[binIndex].name.text = equipItem.itemName + TextManager.Instance.GetText(equipItem.GetEquipmentInfo().sName);
				//*selectedSlots[binIndex].grade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + forgeInfo.u2ID);
				selectSlots[binIndex].txtName.text = equipItem.itemName + TextManager.Instance.GetText(equipItem.GetEquipmentInfo().sName);
				selectSlots[binIndex].imgGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + forgeInfo.u2ID);
            }
            
            lstSlots[slotIndex].gameObject.SetActive(false);
            //*selectedSlots[binIndex].seatType = 1;            
			selectSlots[binIndex].SeatType = 1;
        }
        
        int tempIndex = selectedTimeIndex;
        selectedTimeIndex = -1;
        ChangeTime(tempIndex);
    }
    
    // 선택된 영웅이나 장비를 꺼낼 경우
    public void OnClickSelected(int index)
    {
		/*
        if(selectedSlots[index].seatType == 1)
        {
            selectedSlots[index].seatType = 0;
            selectedSlots[index].slot.gameObject.SetActive(false);
            selectedSlots[index].emptyMark.SetActive(true);
			selectedSlots[index]._cHero = null;
            lstSlots[selectedSlots[index].slotIndex].gameObject.SetActive(true);

			if(_nTrainingCount > 0)
				--_nTrainingCount;

			SetTrainingCost();
        }
        */
        //else if(selectedSlots[index].seatType == -1)
		if(selectSlots[index].SeatType == 1)
		{
			selectSlots[index].SetEmpty();
			lstSlots[selectSlots[index].SlotIndex].gameObject.SetActive(true);
			if(_nTrainingCount > 0)
				--_nTrainingCount;
			
			SetTrainingCost();
		}
		else if(selectSlots[index].SeatType == -1)
			SetUnlockWindow ();
    }

	void SetUnlockWindow(){
		if(seatUnlockWindow == null)
		{
			GameObject windowPref = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Quest/TrainingSeatUnlock.prefab", typeof(GameObject))) as GameObject;
			windowPref.transform.SetParent(transform);
			windowPref.transform.localScale = Vector3.one;
			windowPref.transform.localPosition = Vector3.one;
			windowPref.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
			windowPref.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

			seatUnlockWindow = windowPref.GetComponent<TrainingSeatUnlock>();
		}

		seatUnlockWindow.gameObject.SetActive(true);
		seatUnlockWindow.SetWindow(roomID, isEquip);
		seatUnlockWindow.onClickUnlock = OnClickUnlock;
	}

	public void OpenEqiupWindow(int index)
	{
		if(_cHeroEquipWindow == null)
		{
			GameObject windowPref = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Quest/TrainingHeroEquipWindow.prefab", typeof(GameObject))) as GameObject;
			RectTransform tr = windowPref.GetComponent<RectTransform>();
			tr.SetParent(transform);
			tr.localScale = Vector3.one;
			tr.localPosition = Vector3.one;
			tr.anchoredPosition3D = Vector3.zero;
			tr.sizeDelta = Vector2.zero;

			_cHeroEquipWindow = windowPref.GetComponent<TrainingHeroEquipWindow>();
		}
		_cHeroEquipWindow.gameObject.SetActive(true);
		_cHeroEquipWindow.SetInfo(selectSlots[index].cHero, roomID, selectedTimeIndex);
	}
    
    public void OnClickClose()
    {
        gameObject.SetActive(false);
		PopupManager.Instance.RemovePopup(gameObject);
    }
    
    public void OnClickStart()
    {
        int selectedCount = 0;
		/* **
        for(int i=0; i<selectedSlots.Length; i++)
        {
            if(selectedSlots[i].seatType == 1)
                selectedCount++;
        }
        */
		for(int i=0; i<selectSlots.Count; i++)
		{
			if(selectSlots[i].SeatType == 1)
				selectedCount++;
		}

        if(selectedCount == 0)
        {
            string title = TextManager.Instance.GetText("popup_title_tra_equip_wrong");
            string msg = (isEquip) ? TextManager.Instance.GetText("popup_desc_tra_equip_wrong") : TextManager.Instance.GetText("popup_desc_tra_char_wrong");            
            PopupManager.Instance.ShowOKPopup(title, msg, null);
            
            return;
        }
			
		if(!isEquip)
		{
			// 훈련 비용이 되는지 체크한다
			Goods costGoods = QuestInfoMgr.Instance.GetCharTrainingInfo()[roomID].arrTrainingCost[selectedTimeIndex];
			if(Legion.Instance.CheckEnoughGoods(costGoods.u1Type, (costGoods.u4Count * _nTrainingCount)) == false)
			{
				PopupManager.Instance.ShowChargePopup(costGoods.u1Type);
				return;
			}
		}
		
        if(equipLimitLevel && isEquip) 
        {
            string title = TextManager.Instance.GetText("popup_title_lvup_limit_equip");
            string msg = TextManager.Instance.GetText("popup_desc_lvup_limit_equip");
            PopupManager.Instance.ShowYesNoPopup(title, msg, RequestTraining, null);
            return;
        }
        
        RequestTraining(null);
    }
    
    // 서버로 훈련 요청
    private void RequestTraining(object[] param)
    {
        if(!isEquip)
        {
            CharTrainingInfo info = QuestInfoMgr.Instance.GetCharTrainingInfo()[roomID];
            
            lstChar = new List<Byte>();
            lstSeat = new List<Byte>();
            
            for(int i=0; i<info.u1SeatCount; i++)
            {
                //*if(selectedSlots[i].seatType == 1)
				if(selectSlots[i].SeatType == 1)
                {
                    //Byte id = (Byte)selectedSlots[i].area.GetComponent<TrainingSelectedSlotDrag>().GetItemIndex();                    
					Byte id = (Byte)selectSlots[i].slotDrag.GetItemIndex();                    
					lstChar.Add(Legion.Instance.acHeros[id].u1Index);
                    lstSeat.Add((Byte)(i+1));
                }
            }
            
            PopupManager.Instance.ShowLoadingPopup(1);
            Server.ServerMgr.Instance.CharBeginTraining(info.u1Step, lstChar.ToArray(), lstSeat.ToArray(), (Byte)(selectedTimeIndex + 1), AckCharTrainingBegin);
        }
        else
        {
			// 서버 통합으로 병기훈련은 막음
			return;

            EquipTrainingInfo info = QuestInfoMgr.Instance.GetEquipTrainingInfo()[roomID];
            
            lstEquip = new List<UInt16>();
            lstSeat = new List<Byte>();
            
            Legion.Instance.cInventory.EquipSort();
            for(int i=0; i<info.u1SeatCount; i++)
            {
                //if(selectedSlots[i].seatType == 1)
				if(selectSlots[i].SeatType == 1)
                {
					//Byte id = (Byte)selectedSlots[i].area.GetComponent<TrainingSelectedSlotDrag>().GetItemIndex();                    
					Byte id = (Byte)selectSlots[i].slotDrag.GetItemIndex();
                    lstEquip.Add(Legion.Instance.cInventory.lstSortedEquipment[id].u2SlotNum);
                    lstSeat.Add((Byte)(i+1));
                }
            }
            
            PopupManager.Instance.ShowLoadingPopup(1);
            Server.ServerMgr.Instance.EquipBeginTraining(info.u1Step, lstEquip.ToArray(), lstSeat.ToArray(), (Byte)(selectedTimeIndex + 1), AckCharTrainingBegin);            
        }        
    }
    
    // 결과 처리
    private void AckCharTrainingBegin(Server.ERROR_ID err)
	{
        DebugMgr.Log(err);
        
		PopupManager.Instance.CloseLoadingPopup();
		
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EQUIP_BEGIN_TRAINING, err), Server.ServerMgr.Instance.CallClear);
		}
		else if(err == Server.ERROR_ID.NONE)
		{            
            if(!isEquip)
            {
				// 훈련 비용을 차감한다
				Goods costGoods = QuestInfoMgr.Instance.GetCharTrainingInfo()[roomID].arrTrainingCost[selectedTimeIndex];
				Legion.Instance.SubGoods(costGoods.u1Type, (costGoods.u4Count * _nTrainingCount));
	
                for(int i=0; i<lstChar.Count; i++)
                {
                    CharTrainingInfo info = QuestInfoMgr.Instance.GetCharTrainingInfo()[roomID];                    
                    info.timeType = (Byte)(selectedTimeIndex + 1);
                    info.doneTime = Legion.Instance.ServerTime.AddMinutes(
                        Legion.Instance.getSubVIPTrainingTime(info.arrTrainingTime[selectedTimeIndex] , isEquip));
//                    info.doneTime = Legion.Instance.ServerTime.AddMinutes(info.arrTrainingTime[selectedTimeIndex]);
                    
					Hero hero = Legion.Instance.acHeros.Find(cs => cs.u1Index == lstChar[i]);
                    hero.u1SeatNum = lstSeat[i];
                    hero.u1RoomNum = (Byte)(roomID - Server.ConstDef.BaseCharTrainingID);
                }
            }
            else
            {
                Legion.Instance.cInventory.Sort();
                for(int i=0; i<lstEquip.Count; i++)
                {
                    EquipTrainingInfo info = QuestInfoMgr.Instance.GetEquipTrainingInfo()[roomID];                    
                    info.timeType = (Byte)(selectedTimeIndex + 1);
                    info.doneTime = Legion.Instance.ServerTime.AddMinutes(
                        Legion.Instance.getSubVIPTrainingTime(info.arrTrainingTime[selectedTimeIndex] , isEquip));
                                            
                    EquipmentItem equipItem = (EquipmentItem)Legion.Instance.cInventory.dicInventory[lstEquip[i]];
                    equipItem.u1SeatNum = lstSeat[i];
                    equipItem.u1RoomNum = (Byte)(roomID - Server.ConstDef.BaseEquipTrainingID);
                }              
            }

            roomListObject.GetComponent<TrainingRoomList>().RefreshSlot();
            gameObject.SetActive(false);

			onClose (roomID);
        }        
    }
    
    public void OnClickUnlockButton(int index)
    {
        //if(selectedSlots[index].seatType == -1)
		if(selectSlots[index].SeatType == -1)
            OnClickSlot(index, 0, 0);
    }
    
    // 잠김 슬롯 클릭
    public void OnClickUnlock()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        if(!isEquip)
            Server.ServerMgr.Instance.CharOpenTrainingSeat((Byte)(roomID - Server.ConstDef.BaseCharTrainingID), AckSeatUnlock);
        else          
            Server.ServerMgr.Instance.EquipOpenTrainingSeat((Byte)(roomID - Server.ConstDef.BaseEquipTrainingID), AckSeatUnlock);
          
    }
    
    // 잠김 해제 요청
    private void AckSeatUnlock(Server.ERROR_ID err)
	{
        DebugMgr.Log(err);
        
		PopupManager.Instance.CloseLoadingPopup();
		
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EQUIP_OPEN_TRAINING_SEAT, err), Server.ServerMgr.Instance.CallClear);
		}
		else if(err == Server.ERROR_ID.NONE)
		{           
            //for(int i=0; i<selectedSlots.Length; i++)
			for(int i=0; i<selectSlots.Count; i++)
            {
                //if(selectedSlots[i].seatType == -1)
				if(selectSlots[i].SeatType == -1)
                {
					/* **
                    selectedSlots[i].lockMark.SetActive(false);
                    selectedSlots[i].emptyMark.SetActive(true);
                    selectedSlots[i].seatType = 0;
                    */
					selectSlots[i].SetEmpty();

                    if(!isEquip)
                    {
                        //룸 자리 갯수 증가 시킴
                        Legion.Instance.charTrainingRoom[roomID - Server.ConstDef.BaseCharTrainingID - 1] += 1;
                        CharTrainingInfo info = QuestInfoMgr.Instance.GetCharTrainingInfo()[roomID];    
                        
                        for(int j=0; j<info.arrUnlockGoods.Length; j++)
                        {
                            Legion.Instance.SubGoods(info.arrUnlockGoods[j]);
                        }                
                    }
                    else
                    {
                        //룸 자리 갯수 증가 시킴
                        Legion.Instance.equipTrainingRoom[roomID - Server.ConstDef.BaseEquipTrainingID - 1] += 1;
                        EquipTrainingInfo info = QuestInfoMgr.Instance.GetEquipTrainingInfo()[roomID];
                        
                        for(int j=0; j<info.arrUnlockGoods.Length; j++)
                        {
                            Legion.Instance.SubGoods(info.arrUnlockGoods[j]);
                        }                          
                    }
                    break;
                }
            }
        }
    }    
    
    public void ShowEffect(Transform trans)
    {
        GameObject instEffect = Instantiate(dragEffect) as GameObject;
        instEffect.transform.SetParent(trans);
        instEffect.transform.localScale = Vector3.one;
        instEffect.transform.localPosition = Vector3.zero;
    }

	protected void SetTrainingCost()
	{
		TrainingInfo info = null;
		if(!isEquip)
			info = QuestInfoMgr.Instance.GetCharTrainingInfo()[roomID];		
		else
			return;
		
		UInt32 cost = info.arrTrainingCost[selectedTimeIndex].u4Count * _nTrainingCount;
		// 골드 정보 갱신
		gold.text = cost.ToString();
	}
}
