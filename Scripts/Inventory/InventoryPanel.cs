using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class InventoryPanel : MonoBehaviour {

	public const int CATEGORY_COUNT = 12;
	public ScrollRect classScroll;
	public ScrollRect itemScroll;
	public RectTransform classContent;
	public RectTransform itemContent;
	public ItemEquipWindow itemEquipWindow;
    public ItemInfoWindow itemInfoWindow;
    
	public Toggle[] categoryToggle;
    public GameObject[] toggleAlram;
    public Image toggleConsumeBack;
    public Image toggleConsumeIcon;
    public Image toggleMetarialBack;
    public Image toggleMetarialIcon;

	public Text itemCount;
    public GameObject _emptyList;

	private UInt16 selectedClass;
	private int tabIndex = 0;
	private GameObject classItem;
	private GameObject invenItem;	
	private List<InvenClassSlot> lstClass = new List<InvenClassSlot>();
	private List<InvenItemSlot> lstItem = new List<InvenItemSlot>();
    private List<Item> lstInvenItem = new List<Item>();
    private List<UInt16> lstNewItems = new List<UInt16>();
    private bool init = false;

	public void Init()
	{
        if(!init)
		  SetClassList();
        //SetNewItemList();
        classScroll.StopMovement();
        classScroll.verticalNormalizedPosition = 1f;

        categoryToggle[1].isOn = true;
        init = true;
        
        tabIndex = 1;
        OnClickClass(0, 0);
        RefreshClassAlram();
        
        Legion.Instance.cTutorial.CheckTutorial(MENU.INVENTORY);
        PopupManager.Instance.AddPopup(gameObject, OnClickClose);
	}

    public void SetNewItemList()
    {
        Legion.Instance.cInventory.AllInvenSort();
        List<Item> inventoryData = Legion.Instance.cInventory.lstSortedAllInventory;
        for(int i=0; i<inventoryData.Count; i++)
        {
            if(inventoryData[i].isNew)
                lstNewItems.Add(inventoryData[i].u2SlotNum);
        }
        //for(int i=0; i<lstNewItems.Count; i++)
        //    Legion.Instance.cInventory.dicInventory[lstNewItems[i]].isNew = false;
    }

    // 클래스 목록 생성
	public void SetClassList()
	{		
		Dictionary<ushort, ClassInfo> classData = ClassInfoMgr.Instance.GetClassListInfo();
		
		GameObject classItem = AssetMgr.Instance.AssetLoad("Prefabs/UI/Inventory/Inven_ClassSlot.prefab",
		                                                   typeof(GameObject)) as GameObject;

		int index = 1;
        
        InvenClassSlot allSlot = classContent.GetChild(0).GetComponent<InvenClassSlot>();
        allSlot.onClickClass = OnClickClass;
        lstClass.Add(allSlot);
		
		foreach(ushort key in classData.Keys)
		{			
			ushort id = key;
			
			if(classData[id].u1MonsterType != 0)
				continue;

            if (Legion.Instance.charAvailable[id - 1] == 0)
                continue;
            if (Legion.Instance.charAvailable[id - 1] == 2)
                continue;
			string className = classData[id].sName;
			
			GameObject item = Instantiate(classItem) as GameObject;
			RectTransform itemRect = item.GetComponent<RectTransform>();
			itemRect.SetParent(classContent);
			itemRect.localPosition = Vector3.zero;
			itemRect.localScale = Vector3.one;
			
			InvenClassData invenClassData = new InvenClassData();
			invenClassData.id = id;
			invenClassData.index = index;
			
			InvenClassSlot invenCharacterSlot = item.GetComponent<InvenClassSlot>();
			invenCharacterSlot.InitSlot(invenClassData);
			invenCharacterSlot.onClickClass = OnClickClass;
			invenCharacterSlot.SelectButton(false);
			
			lstClass.Add(invenCharacterSlot);
			
			index++;
		} 
	}
	
    //클래스 클릭 처리
	public void OnClickClass(int index, UInt16 id)
	{
        if(tabIndex > 9 && selectedClass == 0)
        {
            tabIndex = 1;
            categoryToggle[1].isOn = true;
        }        
        
		selectedClass = id;

        if(selectedClass != 0 && (tabIndex == 1 || tabIndex == 2))
        {
            tabIndex = 3;
            categoryToggle[3].isOn = true;
        }
        
        for(int i=0; i<lstClass.Count; i++)
        {
            if(index == i)
                lstClass[i].SelectButton(true);
            else
                lstClass[i].SelectButton(false);
        }

		SetItemList(selectedClass, tabIndex);
        
        // 아이템 확인 처리할(느낌표) 아이템 목록을 서버로 보냄
        //List<UInt16> slots = new List<UInt16>();
        //
        //for(int i=0; i<lstItem.Count; i++)
        //{
        //    if(lstItem[i].gameObject.activeSelf && lstItem[i].data.item.isNew)
        //        slots.Add(lstItem[i].data.item.u2SlotNum);
        //}
        //
        //if(slots.Count > 0)
        //{
        //    PopupManager.Instance.ShowLoadingPopup(1);
        //    Server.ServerMgr.Instance.EquipCheckSlot(slots.ToArray(), AckCheckSlot);
        //}
        //else
        //{
        //    RefreshClassAlram();            
        //}
        
        //전체보기 경우에만 소모품, 재료들을 보여주는 처리
        if(id != 0)
        {
            categoryToggle[1].enabled = false;
            categoryToggle[2].enabled = false;
            toggleConsumeBack.color = Color.gray;
            toggleMetarialBack.color = Color.gray;
            toggleConsumeIcon.color = Color.gray;
            toggleMetarialIcon.color = Color.gray;
        }
        else
        {
            categoryToggle[1].enabled = true;
            categoryToggle[2].enabled = true;
            toggleConsumeBack.color = Color.white;
            toggleMetarialBack.color = Color.white;
            toggleConsumeIcon.color = Color.white;
            toggleMetarialIcon.color = Color.white;            
        }              
        
                          
	}
    
    // 확인 처리 결과
    private void AckCheckSlot(Server.ERROR_ID err)
    {
        DebugMgr.Log(err);
        PopupManager.Instance.CloseLoadingPopup();

        if (err == Server.ERROR_ID.NONE)
        {
            for(int i=0; i<lstItem.Count;)
            {
                if (lstItem[i].data != null && lstItem[i].data.item != null)
                {
                    // 느낌표가 떠있는 아이템의 정보를 갱신
                    if (lstItem[i].gameObject.activeSelf && lstItem[i].data.item.isNew)
                        lstItem[i].data.item.isNew = false;

                    if (Legion.Instance.cInventory.dicInventory.ContainsKey(lstItem[i].data.item.u2SlotNum))
                    {
                        if (Legion.Instance.cInventory.dicInventory[lstItem[i].data.item.u2SlotNum].isNew)
                            Legion.Instance.cInventory.dicInventory[lstItem[i].data.item.u2SlotNum].isNew = false;

                        ++i;
                    }
                    else
                    {
                        lstItem.RemoveAt(i);
                    }
                }
                else
                {
                    lstItem.RemoveAt(i);
                }
                //lstNewItems.Clear();
            }
        }             
        
        RefreshClassAlram();
    }

    // 아이템 목록 생성
	public void SetItemList(UInt16 classID, int category)
	{
		itemScroll.verticalNormalizedPosition = 1;
		
		if(invenItem == null)
			invenItem = AssetMgr.Instance.AssetLoad("Prefabs/UI/Inventory/Inven_ItemSlot.prefab",
			                                       typeof(GameObject)) as GameObject;												

		Legion.Instance.cInventory.AllInvenSort();
		//List<Item> inventoryData = Legion.Instance.cInventory.lstSortedInventory;
		List<Item> inventoryData = Legion.Instance.cInventory.lstSortedAllInventory;
		int index = 0;
		//if(classID == 0)
        //    category = 3;
		for(int i=0; i<inventoryData.Count; i++)
		{
            //선텍한 카테고리 아이템이 아니면 스킵
			switch(category)
			{
                case 1: if(inventoryData[i].cItemInfo.eOrder != ItemInfo.ITEM_ORDER.MATERIAL) continue; break;
                case 2: if(inventoryData[i].cItemInfo.eOrder != ItemInfo.ITEM_ORDER.CONSUMABLE) continue; break;
				case 3: if(inventoryData[i].cItemInfo.eOrder != ItemInfo.ITEM_ORDER.WEAPON_R) continue; break;				
				case 4: if(inventoryData[i].cItemInfo.eOrder != ItemInfo.ITEM_ORDER.WEAPON_L) continue; break;				
				case 5: if(inventoryData[i].cItemInfo.eOrder != ItemInfo.ITEM_ORDER.HELMET) continue; break;				
				case 6: if(inventoryData[i].cItemInfo.eOrder != ItemInfo.ITEM_ORDER.CHEST) continue; break;				
				case 7: if(inventoryData[i].cItemInfo.eOrder != ItemInfo.ITEM_ORDER.SHOULDER) continue; break;
				case 8: if(inventoryData[i].cItemInfo.eOrder != ItemInfo.ITEM_ORDER.GLOVE) continue; break;
				case 9: if(inventoryData[i].cItemInfo.eOrder != ItemInfo.ITEM_ORDER.PANTS) continue; break;				
				case 10: if(inventoryData[i].cItemInfo.eOrder != ItemInfo.ITEM_ORDER.SHOES) continue; break;				
				case 11: if(inventoryData[i].cItemInfo.eOrder != ItemInfo.ITEM_ORDER.ACCESSORY_1 && inventoryData[i].cItemInfo.eOrder != ItemInfo.ITEM_ORDER.ACCESSORY_2) continue; break;				
			}
			
            //장비 아이템일 경우
			if(inventoryData[i].cItemInfo.ItemType == ItemInfo.ITEM_TYPE.EQUIPMENT)
			{
				EquipmentItem item = (EquipmentItem)inventoryData[i];
				EquipmentInfo info = item.GetEquipmentInfo();
				                               
                // 악세사리가 아니고 선택한 클래스 아이템이 아니면 스킵                               
				if((classID > 0 && info.u2ClassID <= 10) && info.u2ClassID != classID)
					continue;

                // 장비 상점에 등록되도 스킵
				if(item.registedInShop != 0)
					continue;

                if(item.attached.hero != null)
                    continue;
			}
            else
            {
                if(classID != 0)
                {
                    continue;
                }
            }
            
            if(inventoryData[i].isNew)
            {
                DebugMgr.Log("new");
            }
	
			InvenItemSlot invenItemSlot = null;
			
			InvenItemData invenItemData = new InvenItemData();
			invenItemData.index = index;
			invenItemData.item = inventoryData[i];
			
            //한번 생성한 프리팹을 재활용한다, 프리팹이 부족할경우 더 생성
			if(lstItem.Count > index)
			{
				invenItemSlot = lstItem[index];
			}
			else
			{
				GameObject item = Instantiate(invenItem) as GameObject;
				RectTransform itemRect = item.GetComponent<RectTransform>();
				itemRect.SetParent(itemContent);
				itemRect.localPosition = Vector3.zero;
				itemRect.localScale = Vector3.one;
				invenItemSlot = item.GetComponent<InvenItemSlot>();

				lstItem.Add(invenItemSlot);
			}
			
			invenItemSlot.InitSlot(invenItemData);
			invenItemSlot.onClickItem = OnClickItem;
            
            if(index == 0)
                invenItemSlot.gameObject.AddComponent<TutorialButton>().id = "Inven_item";
			
//			DebugMgr.Log(inventoryData[i].u2SlotNum);
			
			index++;
		}        
        
		int lstCount = lstItem.Count;
		
		for(int i=0; i<lstCount; i++)
		{
			if(i >= index)
				lstItem[i].gameObject.SetActive(false);
			else
				lstItem[i].gameObject.SetActive(true);
		}

        for(int i=0; i<lstCount; i++)
        {
            if(i >= index)
                _emptyList.SetActive(true);
            else
            {
                _emptyList.SetActive(false);
                break;
            }
        }
		//itemCount.text = Legion.Instance.cInventory.lstSortedInventory.Count + " / " + LegionInfoMgr.Instance.SizeOfBag;
        //itemCount.text = Legion.Instance.cInventory.lstSortedAllInventory.Count + " / " + LegionInfoMgr.Instance.GetMaxInvenSize();
        lstInvenItem.Clear();
        for(int i=0; i<Legion.Instance.cInventory.lstSortedAllInventory.Count; i++)
        {
            if(Legion.Instance.cInventory.lstSortedAllInventory[i].cItemInfo.ItemType == ItemInfo.ITEM_TYPE.EQUIPMENT && 
                ((EquipmentItem)Legion.Instance.cInventory.lstSortedAllInventory[i]).attached.hero != null)
                continue;
            lstInvenItem.Add(Legion.Instance.cInventory.lstSortedAllInventory[i]);
        }
        
        itemCount.text = lstInvenItem.Count + " / " + LegionInfoMgr.Instance.SizeOfBag;

        // 아이템 확인 처리할(느낌표) 아이템 목록을 서버로 보냄
        List<UInt16> slots = new List<UInt16>();
        
        for(int i=0; i<lstItem.Count; i++)
        {
            if(lstItem[i].gameObject.activeSelf && lstItem[i].data.item.isNew)
                slots.Add(lstItem[i].data.item.u2SlotNum);
        }
        
        if(slots.Count > 0)
        {
            PopupManager.Instance.ShowLoadingPopup(1);
            Server.ServerMgr.Instance.EquipCheckSlot(slots.ToArray(), AckCheckSlot);
        }
        else
        {
            RefreshClassAlram();            
        }
	}
	
    //느낌표 갱신 처리
    public void RefreshClassAlram()
    {
		//List<Item> inventoryData = Legion.Instance.cInventory.lstSortedInventory;
        List<Item> inventoryData = Legion.Instance.cInventory.lstSortedAllInventory;

        bool all = false;
        
        for(int i=0; i<toggleAlram.Length; i++)
        {
            toggleAlram[i].SetActive(false);
        }

        for(int i=0; i<lstClass.Count; i++)
        {
            bool showAlram = false;
            
            for(int j=0; j<inventoryData.Count; j++)
            {
                //장비일 경우 (악세사리 제외)
                if(inventoryData[j].cItemInfo.ItemType == ItemInfo.ITEM_TYPE.EQUIPMENT)
                {
                    EquipmentItem equip = (EquipmentItem)inventoryData[j];
                    
                    if(equip.attached.hero != null)
                        continue;

                    else if((equip.GetEquipmentInfo().u2ClassID == lstClass[i].id && equip.GetEquipmentInfo().u2ClassID <= 10) || lstClass[i].id == 0)
                    {
                        //새로운 아이템이거나 스탯포인트가 있으면 느낌표 띄워줌
                        if(inventoryData[j].isNew == true /*|| ((EquipmentItem)inventoryData[j]).u1StatPoint > 0*/)
                        {
                            showAlram = true;
                            
                            if(selectedClass == lstClass[i].id)
                            {
                                //해당 카테고리에 느낌표 띄워줌
                                switch (inventoryData[j].cItemInfo.eOrder)
                                {
                                    case ItemInfo.ITEM_ORDER.WEAPON_R: toggleAlram[1].SetActive(true); break;
                                    case ItemInfo.ITEM_ORDER.WEAPON_L: toggleAlram[2].SetActive(true); break;
                                    case ItemInfo.ITEM_ORDER.HELMET: toggleAlram[3].SetActive(true); break;
                                    case ItemInfo.ITEM_ORDER.CHEST: toggleAlram[4].SetActive(true); break;
                                    case ItemInfo.ITEM_ORDER.SHOULDER: toggleAlram[5].SetActive(true); break;
                                    case ItemInfo.ITEM_ORDER.GLOVE: toggleAlram[6].SetActive(true); break;
                                    case ItemInfo.ITEM_ORDER.PANTS: toggleAlram[7].SetActive(true); break;
                                    case ItemInfo.ITEM_ORDER.SHOES: toggleAlram[8].SetActive(true); break;
                                }
                                
                                //느낌표가 하나라도 있으면 전체에도 느낌표 보여줌
                                toggleAlram[0].SetActive(true);
                            }
                        }                        
                    }
                    //악세사리일 경우
                    else
                    {
                        if(inventoryData[j].cItemInfo.eOrder == ItemInfo.ITEM_ORDER.ACCESSORY_1 || inventoryData[j].cItemInfo.eOrder == ItemInfo.ITEM_ORDER.ACCESSORY_2)
                        {
                            if(inventoryData[j].isNew == true /*|| ((EquipmentItem)inventoryData[j]).u1StatPoint > 0*/)
                            {
                                toggleAlram[9].SetActive(true);
                                showAlram = true;
                                toggleAlram[0].SetActive(true);
                            }
                        }
                    }                    
                }
                // 소모품일 경우 
                else if(inventoryData[j].cItemInfo.ItemType == ItemInfo.ITEM_TYPE.CONSUMABLE)
                {
                    if(lstClass[i].id == 0 && inventoryData[j].isNew == true)
                    {
                        showAlram = true;
                        toggleAlram[10].SetActive(true);
                        toggleAlram[0].SetActive(true);
                        break;
                    }                    
                }          
                //재료일 경우      
                else if(inventoryData[j].cItemInfo.ItemType == ItemInfo.ITEM_TYPE.MATERIAL)
                {
                    if(lstClass[i].id == 0 && inventoryData[j].isNew == true)
                    {
                        showAlram = true;
                        toggleAlram[11].SetActive(true);
                        toggleAlram[0].SetActive(true);
                        break;
                    }                             
                }
                
            }            
            
            //느낌표가 하나라도 있으면 해당 클래스 카테고리에도 느낌표 표시 
            if(!all && showAlram)
                all = true;
            
            lstClass[i].RefreshAlram(showAlram);
        }  

        //느낌표가 하나라도 있으면 전체 카테고리에도 느낌표 표시
        lstClass[0].RefreshAlram(all);
    }    
    
    //아이템 클릭했을 경우 정보창을 보여준다
	public void OnClickItem(Item item)
	{				
		switch(item.cItemInfo.ItemType)
		{
			case ItemInfo.ITEM_TYPE.EQUIPMENT:
			itemEquipWindow.gameObject.SetActive(true);
			itemEquipWindow.SetWindow((EquipmentItem)item);
			
			break;
			
			case ItemInfo.ITEM_TYPE.CONSUMABLE:			
			case ItemInfo.ITEM_TYPE.MATERIAL:
            itemInfoWindow.gameObject.SetActive(true);
            itemInfoWindow.SetInfo(item);
			break;
		}
	}
	
	public void ChangeCategory(int index)
	{
		if(tabIndex == index)
			return;
			
		tabIndex = index;
		
		SetItemList(selectedClass, index);
	}
	
    //판매나 스탯사용등 갱신이 이루어질때 아이템 목록을 갱신해준다
	public void RefreshSlot()
	{
		SetItemList(selectedClass, tabIndex);
        RefreshClassAlram();
	}
	
	public void OnClickClose()
	{
        PopupManager.Instance.RemovePopup(gameObject);
        StartCoroutine(Fade());
        //if(lstNewItems.Count > 0)
        //List<UInt16> slots = new List<UInt16>();
        //
        //for(int i=0; i<lstItem.Count; i++)
        //{
        //    if(lstItem[i].gameObject.activeSelf && lstItem[i].data.item.isNew)
        //        slots.Add(lstItem[i].data.item.u2SlotNum);
        //}
        //
        //if(slots.Count > 0)
        //{
        //    PopupManager.Instance.ShowLoadingPopup(1);
        //    Server.ServerMgr.Instance.EquipCheckSlot(slots.ToArray(), ClearNewItem);
        //}
        //
        //else
        //{
        //    ClearNewItem(Server.ERROR_ID.NONE);
        //}
	}
    
    public void ClearNewItem(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EQUIP_CHECK_SLOT, err), Server.ServerMgr.Instance.CallClear);
            return;
        }
        else
        {
            //for(int i=0; i<lstItem.Count; i++)
            //{
            //    // 느낌표가 떠있는 아이템의 정보를 갱신
            //    if(Legion.Instance.cInventory.dicInventory[lstItem[i].data.item.u2SlotNum].isNew)
            //        Legion.Instance.cInventory.dicInventory[lstItem[i].data.item.u2SlotNum].isNew = false;
            //    //lstNewItems.Clear();
            //}
            PopupManager.Instance.RemovePopup(gameObject);
            StartCoroutine(Fade());
        }
    }

    IEnumerator Fade()
    {
        FadeEffectMgr.Instance.FadeOut();
        yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
		// 2017. 01. 02 jy
		// Base씬을 상속받는 모든 씬에서 사용 가능하도록 변경
		Scene.GetCurrent().CloseInventory();
        //GameObject.FindObjectOfType<LobbyScene>().CloseInventory();
        gameObject.SetActive(false);		
    }
    
	public void Open(POPUP_INVENTORY openPopupType)
	{
        
	}    
}
