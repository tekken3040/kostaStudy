using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

//장비 클릭시 장비 정보를 보여주는 팝업
public class ItemEquipWindow : MonoBehaviour {

	private const int UNLOCK_LV = 10;
    
    public InventoryPanel inventoryPanel;
    public ItemSellResult sellResult;

	public Transform itemPos;
	public Image accImage;
    public SpriteRenderer accImage2;
	public Image elementIcon;
    public Image elememtBG;
	public RectTransform _trNameGroup;
    public RectTransform _trStarGroup;
	public Text tierText;
	public Text itemNameText;
	public Text itemMakerText;
	public Text levelText;
	public Text expText;
	public Image expGague;

	//Point
	public Text statPointText;
    public Text statPointProgressText;
    public GameObject statPointEffect;
	
	//Stat
    public RectTransform[] statObjects;
	public Text[] statNames;
	public Text[] statTexts;
	public Button[] statButtons;
    public Button[] statButtons10;
	public UI_Button_CharacterInfo_Equipment_StatInfo[] _btnStatInfo;

	//Skill
	public RectTransform[] skillObjs;
	public GameObject[] noneSkillObjs;
	public Image[] skillIcons;
	public Image[] skillElements;
    public Text[] skillLevels;
	public Button[] skillButtons;
	public UI_Button_CharacterInfo_Equipment_SkillInfo[] _btnSkillInfo;

    public EquipSellWindow equipSellWindow;
	
	private int statPoint;
	private int statPointOrigin;
    private int statPointProgress;
	private EquipmentItem equipItem;
	private UInt32[] tempStatus; // 스탯을 투자한 임시 스탯 정보
	private byte[] skillSlots;
	private Byte[] statType;
    private UI_PointBuyPopup pointBuyPopup;
    
    public Button[] buttons; // Auto, Reset, Sell, Auction
    public Text[] buttonTexts;
    public GameObject remainEffect;
    //public GameObject PrefFusionPanel;
    //public GameObject PrefChangeLookPanel;
    public UI_Button_CharacterInfo_Equipment_StateInfo _specializeBtn;
    public Text _equipmentClassName;
    [SerializeField] Image _imgClassIcon;
    //public GameObject Pref_Star;
    public GameObject starPos;

    private GameObject pointingEffect;
    private GameObject gradeEffect;
    private GameObject resetPopup;
    
    private List<GameObject> listEffects = new List<GameObject>();
    StringBuilder tempStringBuilder;
    EquipmentItem _cEquipmentItem;
    //leanTween 애니메이션 최대값이 있으므로 루프는 한번만 호출해준다
    void Start()
    {
        //for(int i=0; i<effects.Length; i++)
        //{
        //    LeanTween.rotate(effects[i].GetComponent<RectTransform>(), -360f, 1f).setLoopType(LeanTweenType.easeInElastic).setLoopCount(0);
        //}   
    }

	public void SetWindow(EquipmentItem equipItem)
	{      
        if(pointingEffect == null)
            pointingEffect = AssetMgr.Instance.AssetLoad("Prefabs/UI_Effects/StatusPoint02.prefab", typeof(GameObject)) as GameObject;
        tempStringBuilder = new StringBuilder();
		this.equipItem = equipItem;
		tempStatus = equipItem.statusComponent.points;
        
        for(int i=0; i<tempStatus.Length; i++)
            DebugMgr.Log(tempStatus[i]);
        _cEquipmentItem = equipItem;
		skillSlots = equipItem.skillSlots;
		EquipmentInfo equipInfo = equipItem.GetEquipmentInfo();
		equipItem.GetComponent<StatusComponent>().CountingStatPointEquip(equipItem.cLevel.u2Level);

		//아이템 정보
		levelText.text = equipItem.cLevel.u2Level.ToString();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        expText.text = tempStringBuilder.Append("EXP  ").Append(ConvertExpValue(equipItem.cLevel.u8Exp)).Append("/").Append(ConvertExpValue(equipItem.cLevel.u8NextExp)).ToString();
		expGague.fillAmount = (float)((float)equipItem.cLevel.u8Exp / (float)equipItem.cLevel.u8NextExp);

        _specializeBtn.SetData((Byte)(equipInfo.u1Specialize+2));
        Color tempColor;
        ColorUtility.TryParseHtmlString(equipItem.GetEquipmentInfo().GetHexColor((Byte)(equipItem.GetEquipmentInfo().u1Specialize+2)), out tempColor);
        _specializeBtn.transform.parent.GetComponent<Image>().color = tempColor;
        if(equipInfo.u2ClassID <= ClassInfo.LAST_CLASS_ID)
        {
            _equipmentClassName.text = TextManager.Instance.GetText(ClassInfoMgr.Instance.GetInfo(equipInfo.u2ClassID).sName);

            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            _imgClassIcon.sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.Append("Sprites/Common/common_class.common_class_").Append(equipInfo.u2ClassID).ToString());
            _imgClassIcon.enabled = true;
            _imgClassIcon.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
            _imgClassIcon.SetNativeSize();
        }
        else
        {
            _equipmentClassName.text = TextManager.Instance.GetText("equip_common");
            _imgClassIcon.enabled = false;
        }
        
        for(int i=0; i<starPos.transform.childCount; i++)
            starPos.transform.GetChild(i).gameObject.SetActive(false);
        for(int i=0; i<this.equipItem.u1Completeness; i++)
        {
            starPos.transform.GetChild(i).gameObject.SetActive(true);
            UIManager.Instance.SetGradientFromTier(starPos.transform.GetChild(i).GetComponent<Gradient>(), this.equipItem.u1SmithingLevel);
        }
        starPos.GetComponent<GridLayoutGroup>().SetLayoutHorizontal();

        int smithingLevel = equipItem.u1SmithingLevel;
        
        if(smithingLevel < 1)
            smithingLevel = 1;

        //장비 등급 정보 표시
        ForgeInfo forgeInfo = ForgeInfoMgr.Instance.GetList()[smithingLevel-1];

        //tierText.text = "<" + TextManager.Instance.GetText("forge_level_" + equipItem.u1SmithingLevel) + ">";
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tierText.text = TextManager.Instance.GetText(tempStringBuilder.Append("forge_level_").Append(equipItem.u1SmithingLevel).ToString());
        //tierText.text = TextManager.Instance.GetText("forge_level_" + equipItem.u1SmithingLevel);
		UIManager.Instance.SetGradientFromTier(tierText.GetComponent<Gradient>(), equipItem.u1SmithingLevel);
		if(equipItem.itemName != "")
			itemNameText.text = equipItem.itemName + " " + TextManager.Instance.GetText(equipInfo.sName);
		else
			itemNameText.text = TextManager.Instance.GetText(equipInfo.sName);
        itemNameText.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
		UIManager.Instance.SetGradientFromElement(itemNameText.GetComponent<Gradient>(), equipInfo.u1Element);
		//UIManager.Instance.SetSizeTextGroup(_trNameGroup, 18);
        UIManager.Instance.SetSizeTextGroup(_trStarGroup, 18);
//		itemNameText.color = EquipmentItem.equipElementColors[equipInfo.u1Element];
		itemMakerText.text = (string.IsNullOrEmpty(equipItem.createrName) == true) ? "" : string.Format("By {0}", equipItem.createrName);

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        elememtBG.sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.Append("Sprites/Common/common_02_renew.element_").Append(equipInfo.u1Element).ToString());
        
        //속성 표시
        if(equipInfo.u1Element == 0)
            elementIcon.gameObject.SetActive(false);
        else
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            elementIcon.gameObject.SetActive(true);            
			elementIcon.sprite = AtlasMgr.Instance.GetSprite (tempStringBuilder.Append("Sprites/Common/common_02_renew.element_icon_").Append(equipInfo.u1Element).ToString());
            elementIcon.SetNativeSize();
        } 

		if(equipItem.attached.hero != null)
		{
			//장착 중 판매 불가
            buttons[2].interactable = false;
			buttonTexts[2].GetComponent<Gradient>().StartColor = new Color32(190, 190, 190, 255);
            buttonTexts[2].GetComponent<Gradient>().EndColor = new Color32(102, 102, 102, 255);
			buttons[3].interactable = false;
			buttonTexts[3].GetComponent<Gradient>().StartColor = new Color32(190, 190, 190, 255);
            buttonTexts[3].GetComponent<Gradient>().EndColor = new Color32(102, 102, 102, 255);
		}
		else
		{
            buttons[2].interactable = true;
			buttonTexts[2].GetComponent<Gradient>().StartColor = Color.white;
            buttonTexts[2].GetComponent<Gradient>().EndColor = new Color32(41, 41, 41, 255);
			buttons[3].interactable = true;
			buttonTexts[3].GetComponent<Gradient>().StartColor = Color.white;
            buttonTexts[3].GetComponent<Gradient>().EndColor = new Color32(41, 41, 41, 255);
		}
				
		//statPoint = equipItem.u1StatPoint;
        statPoint = equipItem.statusComponent.UNSET_STATPOINT;
		statPointOrigin = statPoint;
        statPointProgress = equipItem.statusComponent.STATPOINT_EXP;

        //스탯 정보 설정
		SetStatButtons();

        // 장비 모델 정보 (악세사리는 아이콘)
		if(itemPos.transform.childCount != 0)
			DestroyImmediate(itemPos.transform.GetChild(0).gameObject);
            
        if(gradeEffect != null)
            Destroy(gradeEffect);            

		if(equipInfo.u1PosID == EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 || equipInfo.u1PosID == EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
		{
			accImage2.gameObject.SetActive(true);
			itemPos.gameObject.SetActive(false);
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            accImage2.sprite = AssetMgr.Instance.AssetLoad(tempStringBuilder.Append("Sprites/Item/Accessory/acc_").Append(equipInfo.u2ModelID).Append(".png").ToString(), typeof(Sprite)) as Sprite;
		}
		else
		{
			accImage2.gameObject.SetActive(false);
			itemPos.gameObject.SetActive(true);
						
			equipItem.InitViewModelObject();
			equipItem.cObject.transform.SetParent(itemPos);
			equipItem.cObject.transform.localPosition = Vector3.zero;
			equipItem.cObject.transform.localScale = Vector3.one;
			equipItem.cObject.GetComponent<EquipmentObject>().SetLayer( LayerMask.NameToLayer("UI") );            
		}
        
        //아이템 등급 이펙트
        //if(equipItem.u1SmithingLevel > 1)
        //{
            gradeEffect = Instantiate(AssetMgr.Instance.AssetLoad(string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}.prefab", equipItem.u1SmithingLevel), typeof(GameObject))) as GameObject;
            gradeEffect.transform.SetParent(transform);
            gradeEffect.transform.name = "WeaponEffect";
            gradeEffect.transform.localScale = new Vector3(1f, 1f, 1f);
            gradeEffect.transform.localPosition = new Vector3(0f, 115f, 150f);
        //}        
               
        PopupManager.Instance.AddPopup(gameObject, OnClickClose);
	}

	public void SetStatButtons()
	{
        //남은 스탯 포인트
        statPointText.text = statPoint.ToString();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append("(");
        tempStringBuilder.Append((statPointProgress/StatusComponent.MAX_STATEXP_PROGRESS).ToString());
        tempStringBuilder.Append(".");
        if((statPointProgress%StatusComponent.MAX_STATEXP_PROGRESS) >= 10)
            tempStringBuilder.Append((statPointProgress%StatusComponent.MAX_STATEXP_PROGRESS).ToString().Remove(1));
        else
            tempStringBuilder.Append((statPointProgress%StatusComponent.MAX_STATEXP_PROGRESS).ToString());
        tempStringBuilder.Append("%)");
        statPointProgressText.text = tempStringBuilder.ToString();
        if(statPoint > 0)
        {
            statPointEffect.SetActive(true);
            for(int i=0; i<Server.ConstDef.SkillOfEquip; i++)
            {
                statButtons[i].gameObject.SetActive(true);
                skillButtons[i].gameObject.SetActive(true);
                if(statPoint > 19)
                    statButtons10[i].gameObject.SetActive(true);
                else
                    statButtons10[i].gameObject.SetActive(false);
            }
        }
        else
        {
            statPointEffect.SetActive(false);
            for(int i=0; i<Server.ConstDef.SkillOfEquip; i++)
            {
                statButtons[i].gameObject.SetActive(false);
                skillButtons[i].gameObject.SetActive(false);
                statButtons10[i].gameObject.SetActive(false);
            }
        }

        // 스탯 포인트 정보
		statType = new Byte[Server.ConstDef.EquipStatPointType];
		for(int i=0; i<statType.Length; i++)
		{
			statType[i] = equipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType;
			statNames[i].text = Status.GetStatText(statType[i]);
			statTexts[i].text = equipItem.cStatus.GetStat(statType[i], tempStatus[i + Server.ConstDef.SkillOfEquip]).ToString();
			_btnStatInfo[i].SetData(equipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType);
		}

        // 스킬 정보
		for(int i=0; i<Server.ConstDef.SkillOfEquip; i++)
		{
			if(skillSlots[i] != 0)
			{
				SkillInfo skillInfo = SkillInfoMgr.Instance.GetInfoBySlot(equipItem.GetEquipmentInfo().u2ClassID, skillSlots[i]);

                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                skillIcons[i].sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.Append("Sprites/Skill/Atlas_SkillIcon_").Append(equipItem.GetEquipmentInfo().u2ClassID).Append(".").Append(skillInfo.u2ID).ToString());

                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                skillElements[i].sprite = AtlasMgr.Instance.GetSprite(tempStringBuilder.Append("Sprites/Common/common_02_renew.common_02_skill_element_").Append(skillInfo.u1Element).ToString());

                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                skillLevels[i].text = tempStringBuilder.Append("+ ").Append(tempStatus[i]).ToString();

				_btnSkillInfo[i].SetData(skillInfo);
				skillObjs[i].gameObject.SetActive(true);
				noneSkillObjs[i].SetActive(false);
			}
            else
            {
                // 스킬 정보가 없으면 보여주지 않음
                skillObjs[i].gameObject.SetActive(false);
				noneSkillObjs[i].SetActive(true);
            }
		}        
        
        //초기화 비용
		tempEquipResetPrice = ((int)LegionInfoMgr.Instance.cEquipStatResetGoods.u4Count + ((int)LegionInfoMgr.Instance.equipResetUpgrade * equipItem.GetComponent<StatusComponent>().ResetCount));
	}

	//index : 3,4,5 스탯 클릭 처리
	public void OnClickStatUp(int index)
	{
        //요구 포인트
		int needPoint = EquipmentInfoMgr.Instance.statPointPerLevel;

		if(statPoint < needPoint)
			return;
        
        statPoint -= needPoint;
        statPointText.text = statPoint.ToString();
        tempStatus[index] += (UInt32)EquipmentInfoMgr.Instance.statPointPerLevel;
		statTexts[index-Server.ConstDef.SkillOfEquip].text = equipItem.cStatus.GetStat(statType[index-Server.ConstDef.SkillOfEquip], tempStatus[index]).ToString();
        ShowEffect(statObjects[index-Server.ConstDef.SkillOfEquip]);
        
        SetStatButtons();
	}

	//index 0, 1, 2 스킬 클릭 처리
	public void OnClickSkillUp(int index)
	{
        //스킬 요구 포인트
		int needPoint = EquipmentInfoMgr.Instance.skillPointPerLevel;

		if(statPoint < needPoint)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_desc_no_pt"), string.Format(TextManager.Instance.GetText("popup_desc_stat_pt_lack"), needPoint), null);
			return;
		}        

        statPoint -= needPoint;            
        statPointText.text = statPoint.ToString();
        tempStatus[index] += 1;

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        skillLevels[index].text = tempStringBuilder.Append("+ ").Append(tempStatus[index]).ToString();
        ShowEffect(skillObjs[index]); 
        
        SetStatButtons();
	}
    
    private void ShowEffect(RectTransform rect)
    {
        GameObject instEffect = Instantiate(pointingEffect);
        instEffect.transform.SetParent(transform.parent);
        instEffect.transform.localPosition = rect.anchoredPosition3D + Vector3.down * 20f;
        instEffect.transform.localScale = Vector3.one;
        
        listEffects.Add(instEffect);
    }

    GameObject FusionPanel;
    public void OnClickFusion()
    {
        if(equipItem.cObject != null)
        {
            GameObject.Destroy(equipItem.cObject);
            equipItem.cObject = null;
        }
        if(FusionPanel != null)
            FusionPanel.SetActive(true);
        else
        {
            FusionPanel = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Equipment/Pref_UI_Panel_Fusion_Detail.prefab", typeof(GameObject)) as GameObject);
            RectTransform rtTr = FusionPanel.GetComponent<RectTransform>();
            rtTr.SetParent(Scene.GetCurrent().transform);
            rtTr.localPosition = new Vector3(0f, 0f, -1500f);
            rtTr.sizeDelta = Vector2.zero;
            rtTr.localScale = Vector3.one;
            FusionPanel.GetComponent<UI_Panel_Forge_Fusion_Module>().SetData(equipItem, 1);
        }
        OnClickClose();
    }

    GameObject ChangeLookPanel;
    public void OnClickChangeLook()
    {
        if(equipItem.cObject != null)
        {
            GameObject.Destroy(equipItem.cObject);
            equipItem.cObject = null;
        }
        if(ChangeLookPanel != null)
            ChangeLookPanel.SetActive(true);
        else
        {
            ChangeLookPanel = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Equipment/Pref_UI_Panel_Change_Look_Detail_module.prefab", typeof(GameObject)) as GameObject);
            ChangeLookPanel.transform.SetParent(Scene.GetCurrent().transform);
            ChangeLookPanel.transform.localPosition = new Vector3(0f, 0f, -1500f);
            ChangeLookPanel.transform.localScale = Vector3.one;
            ChangeLookPanel.GetComponent<UI_Panel_Forge_ChangeLook_Detail_module>().SetData(equipItem, 1);
        }
        OnClickClose();
    }
    // 스탯 자동 처리
	public void OnClickAuto()
	{
		int index = Server.ConstDef.SkillOfEquip;

        Dictionary<int, int> upIndex = new Dictionary<int, int>();

        //스탯포인트가 남아있으면 순서대로 하나씩 올려준다
		while(statPoint > 0)
		{
            //스킬 찍는 부분
			if(index < Server.ConstDef.SkillOfEquip)
			{
				if(skillSlots[index] != 0 && statPoint > EquipmentInfoMgr.Instance.skillPointPerLevel)
				{
					// int unlockSlot = 1 + (equipItem.cLevel.u2Level / UNLOCK_LV);

					// if(unlockSlot > index)
					// {
						tempStatus[index] += 1;
						statPoint -= EquipmentInfoMgr.Instance.skillPointPerLevel;
                        
                        if(upIndex.ContainsKey(index) == false)
                        {
                            upIndex.Add(index, index);
                            ShowEffect(skillObjs[index]);
                        }      
					// }
				}
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
				skillLevels[index].text = tempStringBuilder.Append("+ ").Append(tempStatus[index]).ToString();
			}
            //스탯 찍는 부분
			else
			{
				tempStatus[index] += (UInt16)EquipmentInfoMgr.Instance.statPointPerLevel;
				statPoint -= EquipmentInfoMgr.Instance.statPointPerLevel;
				statTexts[index-Server.ConstDef.SkillOfEquip].text = equipItem.cStatus.GetStat(statType[index-Server.ConstDef.SkillOfEquip], tempStatus[index]).ToString();
                
                if(upIndex.ContainsKey(index) == false)
                {
                    upIndex.Add(index, index);
                    ShowEffect(statObjects[index-Server.ConstDef.SkillOfEquip]);
                }    
			}

			index++;

			if(index >= tempStatus.Length)
				index = 0;

			statPointText.text = statPoint.ToString();
		}
        
        SetStatButtons();
        
        // PopupManager.Instance.ShowLoadingPopup(1);
		// Server.ServerMgr.Instance.PointEquipmentStatus(equipItem, tempStatus, AckStatUp);        
	}
    
    
    int tempEquipResetPrice;
	//스탯 초기화 버튼 팝업
    public void OnClickReset()
	{
        if(resetPopup == null)
        {
            resetPopup = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Skill/Pref_UI_Reset.prefab", typeof(GameObject))) as GameObject;
            resetPopup.transform.SetParent(PopupManager.Instance._objPopupManager.transform);
            resetPopup.transform.localScale = Vector3.one;
            resetPopup.transform.localPosition = Vector3.zero;
            resetPopup.GetComponent<YesNoPopup>().Show(TextManager.Instance.GetText("popup_title_stat_reset_char"), tempEquipResetPrice.ToString(), RequestReset, null);
            resetPopup.GetComponent<StatResetPopup>().Set(TextManager.Instance.GetText("popup_title_stat_reset_char"), TextManager.Instance.GetText("popup_desc_stat_reset_char"));
        }
        
        PopupManager.Instance.AddPopup(resetPopup, resetPopup.GetComponent<YesNoPopup>().OnClickNoWithDest);        
		//PopupManager.Instance.ShowYesNoPopup("초기화", "", RequestResetEquipItemStatus, null);	
	}
    
    //스탯 초기화 통신 요청
    public void RequestReset(object[] param)
    {
        if(!Legion.Instance.CheckEnoughGoods(2, tempEquipResetPrice))
        {
			PopupManager.Instance.ShowChargePopup(2);            
            return;
        }            			                    
        
	    PopupManager.Instance.ShowLoadingPopup(1);
		Server.ServerMgr.Instance.ResetEquipmentStatus(equipItem, AckResetStat);        
    }

    //스탯 포인트 결과 처리 
	private void AckStatUp(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			//DebugMgr.Log(err);
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EQUIP_STAT_POINT, err), Server.ServerMgr.Instance.CallClear);
            statPoint = statPointOrigin;
            statPointText.text = statPoint.ToString();
            tempStatus = equipItem.GetPoints();
            SetStatButtons();
		}
		else
		{
            //장비 스탯 포인트 정보에 투자한 스탯을 넣어준다
			equipItem.GetComponent<StatusComponent>().points = tempStatus;
			equipItem.GetComponent<StatusComponent>().CountingStatPointEquip(equipItem.cLevel.u2Level);
			equipItem.GetComponent<StatusComponent>().SetByLevelEquip(equipItem.cLevel);
            
            //statPoint = equipItem.u1StatPoint;
            //statPoint = equipItem.statusComponent.UNSET_STATPOINT;
            
			Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.EquipStatPoint, 0, 0, 0, 0, (uint)(statPointOrigin - statPoint));
            
			statPointOrigin = statPoint;
            statPointText.text = statPoint.ToString(); 
            equipItem.statusComponent.UNSET_STATPOINT = (UInt16)statPoint;
//            SetStatButtons();    
            
            inventoryPanel.RefreshSlot();
            
            PopupManager.Instance.RemovePopup(gameObject);
            gameObject.SetActive(false);
            RemoveEffects();
            PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.EquipStatPoint);
        }
	}

    //스탯 초기화 통신 결과 처리
	private void AckResetStat(Server.ERROR_ID err)
	{
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			//DebugMgr.Log(err);
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EQUIP_STAT_RESET, err), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
            //초기화 비용 처리
            Legion.Instance.SubGoods((int)LegionInfoMgr.Instance.cEquipStatResetGoods.u1Type, tempEquipResetPrice);
            
            //모든 스탯을 0으로 초기화
			for(int i=0; i<tempStatus.Length; i++)
				tempStatus[i] = 0;

			equipItem.GetComponent<StatusComponent>().points = tempStatus;
			equipItem.GetComponent<StatusComponent>().CountingStatPointEquip(equipItem.cLevel.u2Level);
			equipItem.GetComponent<StatusComponent>().SetByLevelEquip(equipItem.cLevel);

			//statPoint = equipItem.u1StatPoint;
            statPoint = equipItem.statusComponent.UNSET_STATPOINT;
			statPointOrigin = statPoint;
			statPointText.text = equipItem.u1StatPoint.ToString();

			SetStatButtons();
            
            inventoryPanel.RefreshSlot();
		}
	}

	public void OnClickClose()
	{
		if(statPoint < statPointOrigin)
		{
			PopupManager.Instance.ShowLoadingPopup(1);

			UInt16[] tmp = new UInt16[tempStatus.Length];
			
			for(int i=0; i<tempStatus.Length; i++){
				tmp[i] = (UInt16)tempStatus[i];
                DebugMgr.Log("tempStatus " + i + " " + tmp[i]);
			}	      
            
            for(int i=0; i<equipItem.GetPoints().Length; i++)
            {
				DebugMgr.Log("getPoints " + i + " " + equipItem.GetPoints()[i]);
            }           
             
			Server.ServerMgr.Instance.PointEquipmentStatus(equipItem, tmp, AckStatUp);
		}
		else
        {
            inventoryPanel.RefreshSlot();
            PopupManager.Instance.RemovePopup(gameObject);
		    gameObject.SetActive(false);
            RemoveEffects();         
        }
	}
    
    private void RemoveEffects()
    {
        for(int i=0; i<listEffects.Count; i++)
        {
            if(listEffects[i] != null)
                Destroy(listEffects[i]);
        }
        
        listEffects.Clear();
    }

    private bool regiest = false;

    public void OpenSellWindow()
    {
        if(_cEquipmentItem.u1RoomNum != 0)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_sell"), TextManager.Instance.GetText("mark_training_sell_wrong_desc"), null);
            return;
        }
        else if(_cEquipmentItem.u1SmithingLevel >2)
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_sell"), TextManager.Instance.GetText("popup_3tier_over_sell"), null);
        regiest = false;
        equipSellWindow.gameObject.SetActive(true);
        equipSellWindow.SetSell(equipItem);
    }
    
    public void OpenRegistWindow()
    {
        if(_cEquipmentItem.u1RoomNum != 0)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_sell"), TextManager.Instance.GetText("mark_training_sell_wrong_desc"), null);
            return;
        }
        regiest = true;
        equipSellWindow.gameObject.SetActive(true);
        equipSellWindow.SetRegist(equipItem);
    }
    
    //장비 판매 처리
    public void OnClickSell()
    {
        //상점 등록일 경우
        if(regiest)
        {
            if(Legion.Instance.cInventory.lstInShop.Count >= ShopInfoMgr.Instance.maxEquipRegist)
            {
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_sell_resect"), TextManager.Instance.GetText("popup_desc_sell_resect"), null);
                return;
            }
            PopupManager.Instance.ShowLoadingPopup(1);
            Server.ServerMgr.Instance.ShopResister((Item)equipItem, AckRegist);            
        }
        //일반 판매일 경우
        else
        {
			if(Legion.Instance.CheckGoodsLimitExcessx(1) == true)
			{
				Legion.Instance.ShowGoodsOverMessage(1);
				return;
			}

            PopupManager.Instance.ShowLoadingPopup(1);
            Server.ServerMgr.Instance.InvenSellItem(equipItem.u2SlotNum, 1, AckSell);
        }
    }
    
    //판매 결과 처리
    public void AckSell(Server.ERROR_ID err)
    {
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			//DebugMgr.Log(err);
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.ITEM_SELL, err), Server.ServerMgr.Instance.CallClear);
			return;
		}
		else
		{
            equipSellWindow.gameObject.SetActive(false);
            gameObject.SetActive(false);
			sellResult.gameObject.SetActive(true);
			string itemName = "";
			if(equipItem.itemName != "")
				itemName = equipItem.itemName + " " + TextManager.Instance.GetText(equipItem.GetEquipmentInfo().sName);
			else
				itemName = TextManager.Instance.GetText(equipItem.GetEquipmentInfo().sName);
			sellResult.SetText(string.Format("{0}{1}", itemName, TextManager.Instance.GetText("popup_desc_sell_result")));
            Legion.Instance.cInventory.RemoveEquip(equipItem.u2SlotNum);
            inventoryPanel.RefreshSlot();

			EquipmentInfo equipmentItem = equipItem.GetEquipmentInfo();
			UInt32 sellPrice = (UInt16)(equipmentItem.cSellGoods.u4Count * (equipItem.u1Completeness * 0.5f) * equipItem.u1SmithingLevel * equipItem.cLevel.u2Level);
			Goods sellGood = new Goods(equipmentItem.cSellGoods.u1Type, equipmentItem.cSellGoods.u2ID, sellPrice);
			Legion.Instance.AddGoods(sellGood);
        }        
    }
    
    //장비 등록 결과 처리
    public void AckRegist(Server.ERROR_ID err)
    {
		PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			//DebugMgr.Log(err);
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.SHOP_REGISTER, err), Server.ServerMgr.Instance.CallClear);
			return;
		}
		else
		{
            equipSellWindow.gameObject.SetActive(false);
            gameObject.SetActive(false);
            equipItem.registedInShop = 1;
            Legion.Instance.cInventory.lstInShop.Add(equipItem.u2SlotNum, equipItem);
            Legion.Instance.cInventory.RemoveEquip(equipItem.u2SlotNum);
            inventoryPanel.RefreshSlot();
            sellResult.gameObject.SetActive(true);
            sellResult.SetText(TextManager.Instance.GetText("popup_desc_referral_sell_result"));
        }        
    }
    
    //포인트 구매 클릭 처리
    public void OnClickAddButton()
    {
        if(equipItem.GetComponent<StatusComponent>().BuyPoint >= EquipmentInfoMgr.Instance.LIMIT_EQUIP_STATPOINT)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_desc_over_limit"), TextManager.Instance.GetText("popup_desc_pt_nobuy"), null);
            return;
        }
        
        if(pointBuyPopup == null)
        {
            GameObject popupObject = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Skill/Pref_UI_PointBuy.prefab", typeof(GameObject))) as GameObject;
            RectTransform rect = popupObject.GetComponent<RectTransform>();
            rect.SetParent(PopupManager.Instance._objPopupManager.transform);
            rect.anchoredPosition3D = Vector3.zero;
            rect.sizeDelta = Vector2.zero;
            rect.localScale = Vector3.one;
            popupObject.transform.localPosition = new Vector3(0f, 0f, -1000f);            
            pointBuyPopup = popupObject.GetComponent<UI_PointBuyPopup>();
        }
        
        pointBuyPopup.gameObject.SetActive(true);
        PopupManager.Instance.AddPopup(pointBuyPopup.gameObject, pointBuyPopup.OnClickNoWithDest);
        pointBuyPopup.Show(TextManager.Instance.GetText("popup_title_stat_buy_equip"), TextManager.Instance.GetText("popup_desc_stat_buy_equip"), RequestBuyPoint, null);
        pointBuyPopup.SetBuyPointPopup(UI_PointBuyPopup.BuyType.Equip, equipItem.GetComponent<StatusComponent>().BuyPoint);
    }
    
    Byte buyPoint = 0;
    int pointPrice = 0;
    
    // 구매 팝업에서 구매 버튼 클릭시
    public void RequestBuyPoint(object[] param)
    {
        buyPoint = (Byte)param[0];
        pointPrice = (int)param[1];
        
        if(!Legion.Instance.CheckEnoughGoods(2, pointPrice))
        {
			PopupManager.Instance.ShowChargePopup(2);
            return;
        }
  
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.BuyEquipmentStatPoint(equipItem, buyPoint, AckBuyPoint);
    }
    
    //구매 결과 처리
    public void AckBuyPoint(Server.ERROR_ID err)
    {
    	PopupManager.Instance.CloseLoadingPopup();

		if(err != Server.ERROR_ID.NONE)
		{
			//DebugMgr.Log(err);
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EQUIP_BUY_STAT_POINT, err), Server.ServerMgr.Instance.CallClear);
			return;
		}
		else
		{
            equipItem.GetComponent<StatusComponent>().BuyPoint += buyPoint;
            statPoint += buyPoint;
            statPointOrigin = statPoint;
            statPointText.text = statPoint.ToString();            
			Legion.Instance.SubGoods(EquipmentInfoMgr.Instance.cEquipStatGoods.u1Type, pointPrice);
            buyPoint = 0;
            inventoryPanel.RefreshSlot();
            SetStatButtons();
        }
    }

    public string ConvertExpValue(UInt64 u8Exp)
    {
        string strConvertedExp = "0";

        if(u8Exp < 1000)
            return (strConvertedExp = u8Exp.ToString());

        int tempExp = (int)(Math.Log(u8Exp)/Math.Log(1000));
        strConvertedExp = String.Format("{0:F2}{1}", u8Exp/Math.Pow(1000, tempExp), "KMB".ToCharArray()[tempExp-1]);

        return strConvertedExp;
    }
}
