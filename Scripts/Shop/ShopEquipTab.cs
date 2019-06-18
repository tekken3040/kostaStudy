using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections.Generic;

//장비 상점
public class ShopEquipTab : MonoBehaviour {
	
	private const int SHOP_TYPE = 2;
	
	public Text refreshtimeText;
    public RectTransform content;
	
	[Header("Item Info")]
	public ShopInfoWindow shopInfoWindow;
    public RefreshWindow refreshWindow;
    public GameObject equipInfoObject;
	public Transform itemPos;
	public Image accImage;
    public SpriteRenderer accSprite;
	public Text itemName;
    public Text _txtTier;
	public Text itemLevel;
	public Text itemPrice;
    public Image iconPrice;
    public Text itemElement;
    public Text remainPoint;
	public Text[] statName;
	public Text[] statPoint;
	public UI_Button_CharacterInfo_Equipment_StatInfo[] _btnStatInfo;
    public GameObject[] skillObject;
    public Image[] skillIcon;
    public Image[] skillElement;
	public Text[] skillName;
	public Text[] skillPoint;
    public Color[] RGB_Element;
	public UI_Button_CharacterInfo_Equipment_SkillInfo[] _btnSkillInfo;
    public GameObject leftArrow;
    public GameObject rightArrow;
    public UI_Button_CharacterInfo_Equipment_StateInfo _specializeBtn;
    public Text _equipmentClassName;
    [SerializeField] Image _imgClassIcon;
    public Text _equipmentCreaterName;
    //[SerializeField] GameObject Pref_Star;
    [SerializeField] GameObject starPos;

    [SerializeField] RectTransform _trNameGroup;
    [SerializeField] RectTransform _trStarGroup;

	private int selectIndex = -1;
	private DateTime resetTime;
	private GameObject shopItemSlot;
	private List<ShopEquipSlot> lstShopSlot = new List<ShopEquipSlot>();
	private bool init = false;
	private bool timeChecking = false;
    private GameObject gradeEffect;
	private StringBuilder tempStringbuilder;

	public void Init()
	{
		if(init)
			return;

		PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.RequsetShopList((Byte)SHOP_TYPE, 0, AckShopAutoRefresh);
	}

	public void OnEnable()
	{
		if(!timeChecking && init)
			StartCoroutine(CheckRefreshTime());
            
        ShopInfoMgr.Instance.equipCheck = false;            
	}

	public void OnDisable()
	{
		timeChecking = false;
	}

	//아이템 목록 생성
	public void SetShopList()
	{
		if(shopItemSlot == null)
			shopItemSlot = AssetMgr.Instance.AssetLoad("Prefabs/UI/Shop/Shop_EquipSlot.prefab", typeof(GameObject)) as GameObject;
		
		Shop shopData = ShopInfoMgr.Instance.ShopInfoEquip;

		if(shopData == null)
			return;
		
		int index = 0;
        int soldOutCount = 0;
        
        bool infoInit = false;
		
		for(int i=0; i<shopData.lstShopItem.Count; i++)
		{		
            EquipmentItem equipItem = new EquipmentItem(shopData.lstShopItem[i].u2ItemID);

			// 2016. 09. 11 jy 
			// 거래소의 장비를 정렬할때 현재 유저의 오픈된 캐릭터의 장비만을 보여준다
            //if(Legion.Instance.charAvailable[equipItem.GetEquipmentInfo().u2ClassID - 1] == 0)
            // 2016.09.12 정환
            //거래소 서버 테스트를 위해 임시로 적용 해제
			//if(Legion.Instance.charAvailable[equipItem.GetEquipmentInfo().u2ClassID - 1] != 1)
            //    continue;
			
			ShopEquipSlot shopSlot = null;
			
			ShopSlotData shopSlotData = new ShopSlotData();
			shopSlotData.index = index;
			shopSlotData.shopItem = shopData.lstShopItem[i];
			
			if(lstShopSlot.Count > index)
			{
				shopSlot = lstShopSlot[index];
			}
			else
			{
				GameObject item = Instantiate(shopItemSlot) as GameObject;
				RectTransform itemRect = item.GetComponent<RectTransform>();
				itemRect.SetParent(content);
				itemRect.localPosition = Vector3.zero;
				itemRect.localScale = Vector3.one;
				shopSlot = item.GetComponent<ShopEquipSlot>();
				
				lstShopSlot.Add(shopSlot);
			}
			
			shopSlot.InitSlot(shopSlotData);
			shopSlot.onClickSlot = OnClickItemSlot;	
            
            if(!infoInit && shopData.lstShopItem[i].u1SoldOut != 1)
            {
				//OnClickItemSlot(i);
                ItemSlotSet(i);
                //SetItemInfo();
                infoInit = true;
            }		
            
            if(shopData.lstShopItem[i].u1SoldOut == 1)
                soldOutCount++;
			
			index++;
		}               
        
        // 초기화가 되지 않았으면 장비 정보를 보여주지 않음
        if(!infoInit)
        {
           equipInfoObject.gameObject.SetActive(false);
           
           //등급 이펙트가 남아 있으면 제거
           if(gradeEffect != null)
               Destroy(gradeEffect); 
        }
		
		int lstCount = lstShopSlot.Count;
		
		for(int i=0; i<lstCount; i++)
		{
			if(i >= index)
				lstShopSlot[i].gameObject.SetActive(false);
			else
				lstShopSlot[i].gameObject.SetActive(true);
		}        
        
        leftArrow.SetActive(false);
        
        //아이템이 2개 이상일 때만 화살표 표시
        if(lstShopSlot.Count > 1)
            rightArrow.SetActive(true);
        else
            rightArrow.SetActive(false);            
                
		init = true;
	}

	//갱신 시간 체크
	private IEnumerator CheckRefreshTime()
	{
		timeChecking = true;

		while(true)
		{
			TimeSpan timeSpan = resetTime - Legion.Instance.ServerTime;
			
			if(timeSpan.Ticks > 0)
			{
				refreshtimeText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
			}
			else 
			{
				PopupManager.Instance.ShowLoadingPopup(1);
				Server.ServerMgr.Instance.RequsetShopList(SHOP_TYPE, 0, AckShopAutoRefresh);
				timeChecking = false;
				yield break;
			}
			
			yield return new WaitForSeconds(1f);
		}
	}

	//아이템 상세 정보
	public void SetItemInfo()
	{
		if(selectIndex < 0)
			return;
		
        ShopItem shopItem = lstShopSlot[selectIndex].slotData.shopItem;

        if(shopItem.u1SoldOut == 1)
            return;
        tempStringbuilder = new StringBuilder();
        equipInfoObject.SetActive(true);

		EquipmentItem equipItem = new EquipmentItem(shopItem.u2ItemID);
		EquipmentInfo equipInfo = equipItem.GetEquipmentInfo();
		itemName.text = string.Format("{0} {1}", shopItem.cEquipInfo.strItemName, TextManager.Instance.GetText(equipInfo.sName));
		itemName.color = EquipmentItem.equipElementColors[equipInfo.u1Element];
        UIManager.Instance.SetGradientFromElement(itemName.GetComponent<Gradient>(), equipItem.GetEquipmentInfo().u1Element);
		itemLevel.text = "LV." + shopItem.cEquipInfo.u2Level.ToString();
		itemPrice.text = shopItem.u4Price.ToString();      
        Goods tempGoods = new Goods(shopItem.u1PriceType, shopItem.u2ItemID, shopItem.u4Count);
        iconPrice.sprite = AtlasMgr.Instance.GetGoodsIcon(tempGoods);
        _specializeBtn.SetData((Byte)(equipInfo.u1Specialize+2));
        Color tempColor;
        ColorUtility.TryParseHtmlString(equipItem.GetEquipmentInfo().GetHexColor((Byte)(equipItem.GetEquipmentInfo().u1Specialize+2)), out tempColor);
        _specializeBtn.transform.parent.GetComponent<Image>().color = tempColor;
        if(equipInfo.u2ClassID <= ClassInfo.LAST_CLASS_ID)
        {
            _equipmentClassName.text = TextManager.Instance.GetText(ClassInfoMgr.Instance.GetInfo(equipInfo.u2ClassID).sName);
            tempStringbuilder.Remove(0, tempStringbuilder.Length);
            _imgClassIcon.sprite = AtlasMgr.Instance.GetSprite(tempStringbuilder.Append("Sprites/Common/common_class.common_class_").Append(equipInfo.u2ClassID).ToString());
            _imgClassIcon.enabled = true;
            _imgClassIcon.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
            _imgClassIcon.SetNativeSize();
        }
        else
        {
            _equipmentClassName.text = TextManager.Instance.GetText("equip_common");
            _imgClassIcon.enabled = false;
        }
        tempStringbuilder.Remove(0, tempStringbuilder.Length);
        if(shopItem.cEquipInfo.strCreater != "")
        {
            tempStringbuilder.Append("By. ").Append(shopItem.cEquipInfo.strCreater);
            _equipmentCreaterName.text = tempStringbuilder.ToString();
        }
        else
            _equipmentCreaterName.text = tempStringbuilder.ToString();
        //_txtTier.text = "<" + TextManager.Instance.GetText( "forge_level_" + shopItem.cEquipInfo.u1SmithingLevel) + ">";
        tempStringbuilder.Remove(0, tempStringbuilder.Length);
        _txtTier.text = TextManager.Instance.GetText(tempStringbuilder.Append("forge_level_").Append(shopItem.cEquipInfo.u1SmithingLevel).ToString());
		UIManager.Instance.SetGradientFromTier(_txtTier.GetComponent<Gradient>(), shopItem.cEquipInfo.u1SmithingLevel);
        for(int i=0; i<starPos.transform.childCount; i++)
            starPos.transform.GetChild(i).gameObject.SetActive(false);
        for(int i=0; i<shopItem.cEquipInfo.u1Completeness; i++)
        {
            starPos.transform.GetChild(i).gameObject.SetActive(true);
            UIManager.Instance.SetGradientFromTier(starPos.transform.GetChild(i).GetComponent<Gradient>(), shopItem.cEquipInfo.u1SmithingLevel);
        }
        starPos.GetComponent<GridLayoutGroup>().SetLayoutHorizontal();
        itemName.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
        // 속성 정보
        if(equipInfo.u1Element == 1)
            itemElement.gameObject.SetActive(false);
        else
        {
            itemElement.gameObject.SetActive(true);  
            itemElement.color = RGB_Element[equipInfo.u1Element-2];     
            itemElement.text = TextManager.Instance.GetText("element_" + (equipInfo.u1Element));     
            //itemElement.SetNativeSize();
        }   
	
        //UIManager.Instance.SetSizeTextGroup(_trNameGroup, 18);
        UIManager.Instance.SetSizeTextGroup(_trStarGroup, 18);

        // 스탯 정보
		Status equipStatus = new Status();
		/*
		equipStatus.Set(equipInfo.acStatAddInfo[0].u1StatType, shopItem.cEquipInfo.u2ArrBaseStat[0]);
		equipStatus.Set(equipInfo.acStatAddInfo[1].u1StatType, shopItem.cEquipInfo.u2ArrBaseStat[1]);
		equipStatus.Set(equipInfo.acStatAddInfo[2].u1StatType, shopItem.cEquipInfo.u2ArrBaseStat[2]);
		*/
		for(int i = 0; i < Server.ConstDef.EquipStatPointType; ++i)
		{
			equipStatus.Set(equipInfo.acStatAddInfo[i].u1StatType, shopItem.cEquipInfo.u4ArrBaseStat[i]);
		}
		equipStatus.Add(equipInfo.cStatus.cBasic);
		equipStatus.Add(equipInfo.acStatAddInfo[0].u1StatType, shopItem.cEquipInfo.u2ArrStatsPoint[0]);
		equipStatus.Add(equipInfo.acStatAddInfo[1].u1StatType, shopItem.cEquipInfo.u2ArrStatsPoint[1]);
		equipStatus.Add(equipInfo.acStatAddInfo[2].u1StatType, shopItem.cEquipInfo.u2ArrStatsPoint[2]);
		equipStatus.Add(equipInfo.cStatus.cLevelUp, (shopItem.cEquipInfo.u2Level - 1));
        
        // 찍은 포인트 만큼 스탯 추가
		for(int i=0; i<Server.ConstDef.EquipStatPointType; i++)
		{
			statName[i].text = Status.GetStatText(equipInfo.acStatAddInfo[i].u1StatType);		
			statPoint[i].text = equipStatus.GetStat(equipInfo.acStatAddInfo[i].u1StatType).ToString();
			_btnStatInfo[i].SetData(equipInfo.acStatAddInfo[i].u1StatType);
		}

        // 스킬 정보
		for(int i=0; i<Server.ConstDef.SkillOfEquip; i++)
		{
			if(shopItem.cEquipInfo.u1ArrSkillSlots[i] == 0)
			{
				skillObject[i].gameObject.SetActive(false);
			}
			else
			{
                skillObject[i].gameObject.SetActive(true);
                
				SkillInfo skillInfo = SkillInfoMgr.Instance.GetInfoBySlot(equipInfo.u2ClassID, shopItem.cEquipInfo.u1ArrSkillSlots[i]);
                skillIcon[i].sprite = AtlasMgr.Instance.GetSprite (String.Format ("Sprites/Skill/Atlas_SkillIcon_{0}.{1}", skillInfo.u2ClassID, skillInfo.u2ID));

                tempStringbuilder.Remove(0, tempStringbuilder.Length);
                skillElement[i].sprite = AtlasMgr.Instance.GetSprite(tempStringbuilder.Append("Sprites/Common/common_02_renew.common_02_skill_element_").Append(skillInfo.u1Element).ToString());
				skillName[i].text = TextManager.Instance.GetText(skillInfo.sName);

                tempStringbuilder.Remove(0, tempStringbuilder.Length);
                skillPoint[i].text = tempStringbuilder.Append("+ ").Append(shopItem.cEquipInfo.u1ArrSkillPoint[i]).ToString();
				_btnSkillInfo[i].SetData(skillInfo);
			}
		}
        
        // 남은 스탯 포인트 계산
        UInt16 useStatPoint = 0;
		
		for(int i=0; i<shopItem.cEquipInfo.u2ArrStatsPoint.Length; i++)
		{
			useStatPoint += shopItem.cEquipInfo.u2ArrStatsPoint[i];
		}
        
        for(int i=0; i<shopItem.cEquipInfo.u1ArrSkillPoint.Length; i++)
        {
            useStatPoint += (UInt16)(shopItem.cEquipInfo.u1ArrSkillPoint[i] * EquipmentInfoMgr.Instance.skillPointPerLevel);
        }

		int calStat = (shopItem.cEquipInfo.u2Level - 1 + shopItem.cEquipInfo.u1BuyPoint) - useStatPoint;

		if(calStat < 0)
			calStat = 0;
        
        remainPoint.text = calStat.ToString();        
        
        // 이전 모델이 있으면 제거
		if(itemPos.transform.childCount != 0)
			DestroyImmediate(itemPos.transform.GetChild(0).gameObject);

        if(gradeEffect != null)
            Destroy(gradeEffect);  

		equipItem.u2ModelID = shopItem.cEquipInfo.u2ModelID;

		if(equipInfo.u1PosID == EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 || equipInfo.u1PosID == EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
		{
			accSprite.gameObject.SetActive(true);
			itemPos.gameObject.SetActive(false);

            tempStringbuilder.Remove(0, tempStringbuilder.Length);
            accSprite.sprite = AssetMgr.Instance.AssetLoad(tempStringbuilder.Append("Sprites/Item/Accessory/acc_").Append(equipInfo.u2ModelID).Append(".png").ToString(), typeof(Sprite)) as Sprite;
			// accImage.sprite = AssetMgr.Instance.AssetLoad("Sprites/Item/Accessory/acc_" + equipInfo.u2ModelID, typeof(Sprite)) as Sprite;
			// accImage.SetNativeSize();
		}
		else
		{
            // 모델 생성
			accSprite.gameObject.SetActive(false);
			itemPos.gameObject.SetActive(true);

			equipItem.InitViewModelObject();
			equipItem.cObject.transform.SetParent(itemPos);
			equipItem.cObject.transform.localPosition = Vector3.zero;
			equipItem.cObject.transform.localScale = Vector3.one;
			equipItem.cObject.GetComponent<EquipmentObject>().SetLayer( LayerMask.NameToLayer("UI") );
		}
        
        // 등급 이펙트
        // if(shopItem.cEquipInfo.u1SmithingLevel > 1)
        // {
            gradeEffect = Instantiate(AssetMgr.Instance.AssetLoad(string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}.prefab", shopItem.cEquipInfo.u1SmithingLevel), typeof(GameObject))) as GameObject;
            gradeEffect.transform.SetParent(transform);
            gradeEffect.transform.name = "WeaponEffect";
            gradeEffect.transform.localScale = new Vector3(1f, 1f, 1f);
            //gradeEffect.transform.localPosition = itemPos.localPosition + new Vector3(50f, 150f, 150f);
            gradeEffect.transform.localPosition = new Vector3(-100f, 0f, 0f);
        // }              
	}
	
    // 아이템 클릭 처리
	public void OnClickItemSlot(int index)
	{
		// 같은 아이템을 클릭햇으면 리턴
		if(selectIndex == index)
			return;

		ItemSlotSet(index);
	}
    
    public void ItemSlotSet(int index)
    {
        // 2016. 07. 26 jy
		// 이전에 선택되어 있던 아이템 선택 아이콘을 끄고 선택된 아이템 아이콘을 켠다
		ShopEquipSlot slot;
		if(selectIndex >= 0 && selectIndex < lstShopSlot.Count)
		{
			slot = lstShopSlot[selectIndex];
			slot.SelectedIconEnabled(false);
		}
		if(index >= 0 && index < lstShopSlot.Count)
		{
			slot = lstShopSlot[index];
			slot.SelectedIconEnabled(true);
		}
			
        selectIndex = index;

        // 화살표 온오프 처리
        if(selectIndex == 0)
            leftArrow.SetActive(false);        
        else if(selectIndex >= lstShopSlot.Count-1)
            rightArrow.SetActive(false);
        else
        {
            leftArrow.SetActive(true);
            rightArrow.SetActive(true);
        }    
           
           
        if(selectIndex <= 0)
            leftArrow.SetActive(false);
        else               
            leftArrow.SetActive(true);
            
        if(selectIndex >= lstShopSlot.Count-1)
            rightArrow.SetActive(false);
        else
            rightArrow.SetActive(true);

        SetItemInfo();
    }

    public void OnClickLeft()
    {
        if(selectIndex < 1)
            return;
        
		int selectedIndex = selectIndex;
        // 품절 처리 인 경우 건너 뜀
        for(int i = selectIndex-1; i>=0; i--)
        { 
            if(lstShopSlot[i].slotData.shopItem.u1SoldOut != 1)
            {
				selectedIndex = i;
                break;
            }
        }
        
		if(selectedIndex <= 0)
        {
			selectedIndex = 0;
            leftArrow.SetActive(false);
        }
        
		if(selectedIndex < lstShopSlot.Count-1)
            rightArrow.SetActive(true);
        
		OnClickItemSlot(selectedIndex);
        //SetItemInfo();        
    }
    
    public void OnClickRight()
    {
        if(selectIndex > lstShopSlot.Count-1)
            return;
        
		int selectedIndex = selectIndex;
        // 품절 처리 인 경우 건너 뜀
        for(int i=selectIndex+1; i<lstShopSlot.Count; i++)
        {
            if(lstShopSlot[i].slotData.shopItem.u1SoldOut != 1)
            {
				selectedIndex = i;
                break;
            }
        }
       
		if(selectedIndex >= lstShopSlot.Count-1)
        {
			selectedIndex = lstShopSlot.Count-1;
            rightArrow.SetActive(false);
        }
        
		if(selectedIndex > 0)
            leftArrow.SetActive(true);

		OnClickItemSlot(selectedIndex);
        //SetItemInfo();          
    }
	
    // 구입 클릭시 팝업 처리
    public void OnClilckBuyButton()
    {
        shopInfoWindow.gameObject.SetActive(true);
        itemPos.gameObject.SetActive(false);
        shopInfoWindow.onClickBuy = OnClickBuy;
        shopInfoWindow.SetInfo(ShopInfoWindow.ShopType.Equip, lstShopSlot[selectIndex].slotData.shopItem, TextManager.Instance.GetText("popup_title_shop_buy_common"));
    }
    
    // 구매 요청
	public void OnClickBuy()
	{
		if(Legion.Instance.cInventory.IsInvenFull())
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_buy_equip_disable"), TextManager.Instance.GetText("popup_desc_inven_full"), null);
			return;
		}

		ShopItem shopItem = lstShopSlot[selectIndex].slotData.shopItem;
		if(!Legion.Instance.CheckEnoughGoods(shopItem.u1PriceType, shopItem.u4Price * shopItem.u4Count))
		{
			PopupManager.Instance.ShowChargePopup(shopItem.u1PriceType);
			return;
		}
		
		RequestBuy();
	}

	private void RequestBuy()
	{
		PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.ShopBuyItem(SHOP_TYPE, (Byte)(selectIndex+1), AckShopBuy);
	}
    
    // 갱신 클릭 처리
	public void OnClickRefresh()
	{
        byte priceType = ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u1Type;
		UInt32 price = (UInt32)(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u4Count
		                        + (ShopInfoMgr.Instance.ShopInfoEquip.u1RenewCount * ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2AddValue));
		
		if(price > ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2MaxValue)
			price = ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2MaxValue;

        itemPos.gameObject.SetActive(false);
        refreshWindow.gameObject.SetActive(true);
        refreshWindow.SetInfo(priceType, (int)price);
        refreshWindow.onClickRefresh = Refresh;
	}
   
    // 갱신 요청
    private void Refresh()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.RequsetShopList(SHOP_TYPE, 1, AckShopInstantRefresh);
    }    

	//즉시 갱신
	private void AckShopInstantRefresh(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();
		
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.SHOP_LIST, err), Server.ServerMgr.Instance.CallClear);			
		}
		else if(err == Server.ERROR_ID.NONE)
		{	
            //상점 리스트 갱신
			SetShopList();
			
            //갱신 비용 처리
			string priceType = Legion.Instance.GetConsumeString(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u1Type);
			UInt32 price = (UInt32)(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u4Count
			                        + (ShopInfoMgr.Instance.ShopInfoEquip.u1RenewCount * ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2AddValue));
			
			if(price > ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2MaxValue)
				price = ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2MaxValue;

			Legion.Instance.cInventory.EquipSort();

			if(ShopInfoMgr.Instance.ShopInfoEquip.u1RenewCount != 0)
			{
				Legion.Instance.SubGoods(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u1Type, ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u4Count +
			   	                      	(ShopInfoMgr.Instance.ShopInfoEquip.u1RenewCount-1) * ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2AddValue);
			}
						
            //시간 정보 갱신                        
			TimeCount();
			
			resetTime = ShopInfoMgr.Instance.ShopInfoEquip.leftTime;

			if(!timeChecking)
				StartCoroutine(CheckRefreshTime());
                
            refreshWindow.OnClickClose();    
		}
	}

	//시간에 맞춰 갱신
	private void AckShopAutoRefresh(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();
		
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.SHOP_LIST, err), Server.ServerMgr.Instance.CallClear);
		}
		else if(err == Server.ERROR_ID.NONE)
		{	
			SetShopList();
			
			string priceType = Legion.Instance.GetConsumeString(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u1Type);
			UInt32 price = (UInt32)(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].cRenewGoods.u4Count
			                        + (ShopInfoMgr.Instance.ShopInfoEquip.u1RenewCount * ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2AddValue));
			
			if(price > ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2MaxValue)
				price = ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u2MaxValue;
			
			Legion.Instance.cInventory.EquipSort();

			TimeCount();

			resetTime = ShopInfoMgr.Instance.ShopInfoEquip.leftTime;

			if(!timeChecking)
				StartCoroutine(CheckRefreshTime());
                
            refreshWindow.OnClickClose();    
            PopupManager.Instance.RemovePopup(refreshWindow.gameObject);
		}
	}
	
	private void AckShopBuy(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();
		
		if(err == Server.ERROR_ID.SOLDOUT)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.SHOP_BUY, err), Server.ServerMgr.Instance.CallClear);
			lstShopSlot[selectIndex].slotData.shopItem.u1SoldOut = 1;
			lstShopSlot[selectIndex].CheckSoldOut();
			shopInfoWindow.OnClickClose();
		}
		else if(err == Server.ERROR_ID.NONE)
		{
			//품절 처리
			lstShopSlot[selectIndex].slotData.shopItem.u1SoldOut = 1;
			lstShopSlot[selectIndex].CheckSoldOut();

			//장비 아이템 추가
			ShopItem shopItem = lstShopSlot[selectIndex].slotData.shopItem;

			UInt32[] au2Stats = new UInt32[Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType * 2];

			for(int i=0; i<Server.ConstDef.SkillOfEquip; i++)
				au2Stats[i] = shopItem.cEquipInfo.u1ArrSkillPoint[i];

			for(int i=0; i<Server.ConstDef.EquipStatPointType; i++)
			{
				au2Stats[Server.ConstDef.SkillOfEquip + i] = shopItem.cEquipInfo.u4ArrBaseStat[i];//u2ArrBaseStat[i];
				au2Stats[Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType + i] = shopItem.cEquipInfo.u2ArrStatsPoint[i];
			}

			Legion.Instance.cInventory.AddEquipment(0, 0, shopItem.u2ItemID, shopItem.cEquipInfo.u2Level, shopItem.cEquipInfo.u8Exp, shopItem.cEquipInfo.u1ArrSkillSlots,
			                                        au2Stats, 0, shopItem.cEquipInfo.strItemName, shopItem.cEquipInfo.strCreater, shopItem.cEquipInfo.u2ModelID, true, shopItem.cEquipInfo.u1SmithingLevel, shopItem.cEquipInfo.u2TotalStatPoint, shopItem.cEquipInfo.u2StatPointExp, shopItem.cEquipInfo.u1Completeness);

			EquipmentInfo eInfo = EquipmentInfoMgr.Instance.GetInfo (shopItem.u2ItemID);

			Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.GetEquip, eInfo.u2ID, (Byte)eInfo.u1PosID, shopItem.cEquipInfo.u1SmithingLevel, (Byte)eInfo.u2ClassID, 1);
			//재화 소모
			byte priceType = lstShopSlot[selectIndex].slotData.shopItem.u1PriceType;
			UInt32 price = lstShopSlot[selectIndex].slotData.shopItem.u4Price * lstShopSlot[selectIndex].slotData.shopItem.u4Count;
            
            shopInfoWindow.OnClickClose();            
            SetShopList();
            
            string title = TextManager.Instance.GetText("popup_title_shop_buy_common_result");
            string itemName = shopInfoWindow.itemName.text +TextManager.Instance.GetText("popup_desc_shop_buy_common_result");
            PopupManager.Instance.ShowOKPopup(title, itemName, null);
		}
		else
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.SHOP_BUY, err), Server.ServerMgr.Instance.CallClear);
		}
	}

	public void OnClickClose()
	{
		//infoWindow.SetActive(false);
	}

    //다음 갱신 시간 정보 세팅
	public void TimeCount()
	{
		int length = ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u1ArrResetTime.Length;
		
		for(int i=0; i<length; i++)
		{
			if(Legion.Instance.ServerTime < Legion.Instance.ServerTime.Date.AddHours(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u1ArrResetTime[i]))
			{
				ShopInfoMgr.Instance.ShopInfoEquip.leftTime = Legion.Instance.ServerTime.Date.AddHours(ShopInfoMgr.Instance.dicShopData[SHOP_TYPE].u1ArrResetTime[i]);
				break;
			}
		}
	}
}
