using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class UI_Status_UseExpPotion : MonoBehaviour
{
    public GameObject StatusPanel;                          //스테이터스창
    public GameObject SlotList;                             //포션 슬롯 리스트
    public GameObject PotionList;                           //포션 리스트
    public GameObject ArrowBg;                              //경험치 상승 화살표
    public GameObject Item_Name;                            //포션 이름
    public GameObject Item_Info;                            //포션 설명
    //public GameObject SlotElement;                          //슬롯 프리펩
    //public GameObject PotionElement;                        //포션 프리펩
    public GameObject LevelupEff;                           //레벨업 이펙트
    public GameObject LevelupEff2;                          //레벨업 이펙트
    public GameObject Btn_Use;                              //사용버튼

    public ConsumableItem _selectedConsumableItem;          //선택된 포션
    public UInt16 u2SelectedItemID;
    public ConsumableItem _prevConsumableItem;              //이전에 선택된 포션
    Dictionary<UInt16, ConsumableItem> dicConsumableItem;
    List<ExpPotion> lstExpPotion;

    StringBuilder tempStringBuilder;
    Hero cHero;

    public Hero GetHero
    {
        get
        {
            return cHero;
        }
    }

    void Awake()
    {
        dicConsumableItem = new Dictionary<ushort, ConsumableItem>();
        tempStringBuilder = new StringBuilder();
    }

    public void OnEnable()
    {
        effect_lvlup = false;
        //StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().CheckOpenPotionPopup(true);
        SetPotionList();
        //StopAllCoroutines();
    }
    public void RefreashLevelUpEff()
    {
        effect_lvlup = false;
        LevelupEff.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        LevelupEff.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        LevelupEff.transform.GetChild(0).gameObject.SetActive(false);
        LevelupEff2.SetActive(false);
        StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().cHero.GetComponent<LevelComponent>().bLevelup = false;
    }

    //선택 물약 변경 체크
    Goods _goods;
    public void ChangeConsumableItems()
    {
        if(_selectedConsumableItem != _prevConsumableItem)
        {
            if(_itemCnt > 0)
            {
                _goods = new Goods((Byte)GoodsType.CONSUME, _prevConsumableItem.cItemInfo.u2ID, _itemCnt);
                if(Legion.Instance.CheckEnoughGoods(_goods))
                {
                    PopupManager.Instance.ShowLoadingPopup(1);
                    //Btn_Use.GetComponent<Button>().interactable = false;
                    Server.ServerMgr.Instance.InvenUseItem(_prevConsumableItem.u2SlotNum, _itemCnt, StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().cHero.u1Index, UsePotion);
                }
                else
                {
					PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetErrorText("popup_title_nocost", "", false), TextManager.Instance.GetText(ItemInfoMgr.Instance.GetConsumableItemInfo(_prevConsumableItem.cItemInfo.u2ID).sName) + TextManager.Instance.GetErrorText("popup_desc_nocost", "", false), null);
		        	return;
                }
            }
            else
                _prevConsumableItem = _selectedConsumableItem;
        }
        //else
        //{
        //    _prevConsumableItem = _selectedConsumableItem;
        //}
    }
    //사용버튼 클릭
    public UInt16 _itemCnt = 0;
    public int transformIndex = 0;
    public void OnClickUse()
    {
        if(_selectedConsumableItem == null) return;

        if(cHero.GetComponent<LevelComponent>().cLevel.u2Level >= Server.ConstDef.MaxHeroLevel)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("btn_potion"), TextManager.Instance.GetText("char_max_level"), null);
            return;
        }
        //if(_selectedConsumableItem != _prevConsumableItem)
        //{
        //    _goods = new Goods((Byte)GoodsType.CONSUME, _prevConsumableItem.cItemInfo.u2ID, _itemCnt);
        //    if(Legion.Instance.CheckEnoughGoods(_goods))
        //    {
        //        Btn_Use.GetComponent<Button>().interactable = false;
        //        Server.ServerMgr.Instance.InvenUseItem(_prevConsumableItem.u2SlotNum, _itemCnt, StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().cHero.u1Index, UsePotion);
        //    }
        //    else
        //    {
        //        PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetErrorText("popup_title_nocost"), TextManager.Instance.GetText(ItemInfoMgr.Instance.GetConsumableItemInfo(_prevConsumableItem.cItemInfo.u2ID).sName) + TextManager.Instance.GetErrorText("popup_desc_nocost"), null);
		//    	return;
        //    }
        //    //_itemCnt = 0;
        //}

        _itemCnt++;
        StartCoroutine(DelayRefreash());
        //RefreshPotions();
        PotionList.transform.GetChild(transformIndex).GetComponent<UI_ExpPotion>().DecreaseCount();
    }
    public void EmptyConsumableItems()
    {
        if(_itemCnt > 0)
        {
            _goods = new Goods((Byte)GoodsType.CONSUME, _selectedConsumableItem.cItemInfo.u2ID, _itemCnt);
            if(Legion.Instance.CheckEnoughGoods(_goods))
            {
                PopupManager.Instance.ShowLoadingPopup(1);
                Server.ServerMgr.Instance.InvenUseItem(_selectedConsumableItem.u2SlotNum, _itemCnt, StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().cHero.u1Index, UsePotion);
            }
            else
            {
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetErrorText("popup_title_nocost"), TextManager.Instance.GetText(ItemInfoMgr.Instance.GetConsumableItemInfo(_selectedConsumableItem.cItemInfo.u2ID).sName) + TextManager.Instance.GetErrorText("popup_desc_nocost"), null);
		    	return;
            }
        }
    }
    //포션 사용
    UInt16 prevLvl;
    public void UsePotion(Server.ERROR_ID err)
    {
        //Btn_Use.GetComponent<Button>().interactable = true;
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.ITEM_USE, err), Server.ServerMgr.Instance.CallClear);
			return;
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            Legion.Instance.SubGoods(_goods);
            _itemCnt = 0;
            _prevConsumableItem = _selectedConsumableItem;
            RefreshPotions();
        }
    }
    IEnumerator DelayRefreash()
    {
        yield return new WaitForEndOfFrame();
        //RefreshPotions();
        LevelUpCheck();
    }
    public void LevelUpCheck()
    {
        prevLvl = StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().cHero.GetComponent<LevelComponent>().cLevel.u2Level;
        StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().cHero.GetComponent<LevelComponent>().AddExp(ItemInfoMgr.Instance.GetConsumableItemInfo(u2SelectedItemID).u4Exp);
        StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().SetCharacterInfo(StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().cHero);
        if(prevLvl < StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().cHero.GetComponent<LevelComponent>().cLevel.u2Level)
            if (!effect_lvlup)
                StartCoroutine(ShowLevelUpEffect());
    }
    public void RefreshPotions()
    {
        //for(int i=0; i<PotionList.transform.childCount; i++)
        //{
        //    PotionList.transform.GetChild(i).GetComponent<UI_ExpPotion>().RefreshPotionData();
        //}
        int tempSlotCnt = (3 - (PotionList.transform.childCount%3));
        if(PotionList.transform.childCount%3 == 0)
        {
            for(int i=0; i<SlotList.transform.childCount-PotionList.transform.childCount; i++)
            {
                Destroy(SlotList.transform.GetChild(i).gameObject);
            }
        }
        //else
        //{
        //    for(int i=0; i<SlotList.transform.childCount-(PotionList.transform.childCount+tempSlotCnt); i++)
        //    {
        //        Destroy(SlotList.transform.GetChild(i).gameObject);
        //    }
        //}
        
        //for(int i=0; i<SlotList.transform.childCount - PotionList.transform.childCount; i++)
        //    Destroy(SlotList.transform.GetChild(i).gameObject);
        StartCoroutine(CheckPotion());
    }
    IEnumerator CheckPotion()
    {
        yield return new WaitForEndOfFrame();
        if(PotionList.transform.childCount == 0)
            OnClickClose();
    }
    bool effect_lvlup = false;
    IEnumerator ShowLevelUpEffect()
    {
        effect_lvlup = true;
        //LevelupEff.GetComponent<Image>().color = Color.white;
        LeanTween.alpha(LevelupEff.GetComponent<RectTransform>(), 1f, 0.02f);
        //LeanTween.scale(LevelupEff, new Vector3(1.1f, 1.1f), 0.1f);
        LeanTween.scale(LevelupEff, Vector3.one, 0.05f).setDelay(0.02f);
        yield return new WaitForSeconds(0.2f);
        LevelupEff.transform.GetChild(0).gameObject.SetActive(true);
        LevelupEff2.SetActive(true);
        LeanTween.alpha(LevelupEff.GetComponent<RectTransform>(), 0f, 0.8f).setDelay(1f);
        yield return new WaitForSeconds(1.8f);
        LevelupEff.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
        LevelupEff.transform.localScale = new Vector3(1.5f, 1.5f, 1f);
        LevelupEff.transform.GetChild(0).gameObject.SetActive(false);
        LevelupEff2.SetActive(false);
        StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().cHero.GetComponent<LevelComponent>().bLevelup = false;

        effect_lvlup = false;
    }
    //물약 사용 검사
    public void CheckUsePotion(bool BtnBack)
    {
        if(_itemCnt != 0)
        {
            //Btn_Use.GetComponent<Button>().interactable = false;
            _goods = new Goods((Byte)GoodsType.CONSUME, _prevConsumableItem.cItemInfo.u2ID, _itemCnt);
            if(Legion.Instance.CheckEnoughGoods(_goods))
            {
                PopupManager.Instance.ShowLoadingPopup(1);
                Server.ServerMgr.Instance.InvenUseItem(_selectedConsumableItem.u2SlotNum, _itemCnt, StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().cHero.u1Index, RecvCheckUsePotion);
            }
            else
            {
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetErrorText("popup_title_nocost", "", false), TextManager.Instance.GetText(ItemInfoMgr.Instance.GetConsumableItemInfo(_prevConsumableItem.cItemInfo.u2ID).sName) + TextManager.Instance.GetErrorText("popup_desc_nocost", "", false), null);
		        return;
            }
        }
        else
        {
            Close();
            if(BtnBack)
                StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().CallBackBtnBack();
            else
                StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().CallBackSwitchMenu();
        }
    }
    public void RecvCheckUsePotion(Server.ERROR_ID err)
    {
        //Btn_Use.GetComponent<Button>().interactable = true;
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.ITEM_USE, err), Server.ServerMgr.Instance.CallClear);
			return;
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            Legion.Instance.SubGoods(_goods);
            _itemCnt = 0;
            _prevConsumableItem = null;
            _selectedConsumableItem = null;
            Close();
            StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().CallBackBtnBack();
        }
    }
    //닫기 버튼 클릭
    public void OnClickClose()
    {
        if(_itemCnt != 0)
        {
            //Btn_Use.GetComponent<Button>().interactable = false;
            _goods = new Goods((Byte)GoodsType.CONSUME, _prevConsumableItem.cItemInfo.u2ID, _itemCnt);
            if(Legion.Instance.CheckEnoughGoods(_goods))
            {
                PopupManager.Instance.ShowLoadingPopup(1);
                Server.ServerMgr.Instance.InvenUseItem(_selectedConsumableItem.u2SlotNum, _itemCnt, StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().cHero.u1Index, RecvUsePotion);
            }
            else
            {
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetErrorText("popup_title_nocost", "", false), TextManager.Instance.GetText(ItemInfoMgr.Instance.GetConsumableItemInfo(_prevConsumableItem.cItemInfo.u2ID).sName) + TextManager.Instance.GetErrorText("popup_desc_nocost", "", false), null);
		        return;
            }
        }
        else
            Close();
    }
    public void RecvUsePotion(Server.ERROR_ID err)
    {
        //Btn_Use.GetComponent<Button>().interactable = true;
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.ITEM_USE, err), Server.ServerMgr.Instance.CallClear);
			return;
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            Legion.Instance.SubGoods(_goods);
            _itemCnt = 0;
            _prevConsumableItem = null;
            _selectedConsumableItem = null;
            Close();
        }
    }
    public void Close()
    {
        for(int i=0; i<PotionList.transform.childCount; i++)
            GameObject.Destroy(PotionList.transform.GetChild(i).gameObject);
        for(int i=0; i<SlotList.transform.childCount; i++)
            GameObject.Destroy(SlotList.transform.GetChild(i).gameObject);

        //ArrowBg.gameObject.SetActive(false);
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        Item_Name.GetComponent<Text>().text = tempStringBuilder.ToString();
        Item_Info.GetComponent<Text>().text = tempStringBuilder.ToString();
        for(int i=0; i<lstExpPotion.Count; i++)
        {
            if(lstExpPotion[i]._consumableItem.u2Count != 0)
            {
                StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().PotionUseBtnEnable(true);
                break;
            }
            else
            {
                StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().PotionUseBtnEnable(false);
            }
        }
        dicConsumableItem.Clear();
        lstExpPotion.Clear();
        StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().CheckOpenPotionPopup(false);
        //RefreashLevelUpEff();
        if(StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().Status_Point.text != "0")
            StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().StatPointEffEnable(true);
        StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().Btn_UsePotion.transform.GetChild(1).gameObject.SetActive(false);
        this.gameObject.SetActive(false);
        PopupManager.Instance.RemovePopup(gameObject);

        PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.CharLevel);
    }
    //포션 리스트 생성
    public void SetPotionList()
    {
		dicConsumableItem = new Dictionary<ushort, ConsumableItem> ();
        //int emptySlot = Legion.Instance.cInventory.FindEmptySlot();
        int emptySlot = Legion.Instance.cInventory.dicInventory.Count;
        lstExpPotion = new List<ExpPotion>();
        cHero = StatusPanel.GetComponent<UI_Panel_CharacterInfo_Status>().cHero;
        for(int i=0; i<PotionList.transform.childCount; i++)
            GameObject.Destroy(PotionList.transform.GetChild(i).gameObject);
        for(int i=0; i<SlotList.transform.childCount; i++)
            GameObject.Destroy(SlotList.transform.GetChild(i).gameObject);

        //ArrowBg.gameObject.SetActive(false);
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        Item_Name.GetComponent<Text>().text = tempStringBuilder.ToString();
        Item_Info.GetComponent<Text>().text = tempStringBuilder.ToString();

        for(int i=1; i<=emptySlot; i++)
        {
            if(!Legion.Instance.cInventory.dicInventory.ContainsKey((ushort)i))
            {
                emptySlot++;
                //i++;
                continue;
            }
            if(((Legion.Instance.cInventory.dicInventory[(UInt16)i].cItemInfo.u2ID/58000) == 1) && 
                (ItemInfoMgr.Instance.GetConsumableItemInfo(((ConsumableItem)Legion.Instance.cInventory.dicInventory[(UInt16)i]).cItemInfo.u2ID).u4Exp != 0))
            {
                ExpPotion _expPotion = new ExpPotion();
                dicConsumableItem.Add(Legion.Instance.cInventory.dicInventory[(UInt16)i].cItemInfo.u2ID, (ConsumableItem)Legion.Instance.cInventory.dicInventory[(UInt16)i]);
                _expPotion.u2ID = Legion.Instance.cInventory.dicInventory[(UInt16)i].cItemInfo.u2ID;
                _expPotion._consumableItem = (ConsumableItem)Legion.Instance.cInventory.dicInventory[(UInt16)i];
                lstExpPotion.Add(_expPotion);
            }
        }
        //New 체크
        List<UInt16> slots = new List<UInt16>();
        for(int i=0; i<lstExpPotion.Count; i++)
        {
            if(lstExpPotion[i]._consumableItem.isNew)
                slots.Add(lstExpPotion[i]._consumableItem.u2SlotNum);
        }
        
        if(slots.Count > 0)
        {
            PopupManager.Instance.ShowLoadingPopup(1);
            Server.ServerMgr.Instance.EquipCheckSlot(slots.ToArray(), ConfirmNewPotion);
        }
        for(int i=0; i<dicConsumableItem.Count; i++)
        {
            GameObject _potionElement = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Status/Potion_.prefab", typeof(GameObject)) as GameObject);
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append("PotionList_").Append(i.ToString());
            _potionElement.GetComponent<RectTransform>().SetParent(PotionList.GetComponent<RectTransform>());
            _potionElement.GetComponent<RectTransform>().name = tempStringBuilder.ToString();
            _potionElement.GetComponent<RectTransform>().localScale = Vector3.one;
            _potionElement.GetComponent<RectTransform>().localPosition = Vector3.zero;
            _potionElement.GetComponent<UI_ExpPotion>().SetExpPotionData(lstExpPotion[i].u2ID, lstExpPotion[i]._consumableItem);
        }
        if(PotionList.transform.childCount%3 == 0)
        {
            for(int i=0; i<PotionList.transform.childCount; i++)
            {
                GameObject _slotElement = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Status/Slot_.prefab", typeof(GameObject)) as GameObject);
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append("SlotList_").Append(i.ToString());
                _slotElement.GetComponent<RectTransform>().SetParent(SlotList.GetComponent<RectTransform>());
                _slotElement.GetComponent<RectTransform>().name = tempStringBuilder.ToString();
                _slotElement.GetComponent<RectTransform>().localScale = Vector3.one;
                _slotElement.GetComponent<RectTransform>().localPosition = Vector3.zero;
            }
        }
        else
        {
            int tempSlotCnt = (3 - (PotionList.transform.childCount%3));
            for(int i=0; i<PotionList.transform.childCount+tempSlotCnt; i++)
            {
                GameObject _slotElement = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Status/Slot_.prefab", typeof(GameObject)) as GameObject);
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                tempStringBuilder.Append("SlotList_").Append(i.ToString());
                _slotElement.GetComponent<RectTransform>().SetParent(SlotList.GetComponent<RectTransform>());
                _slotElement.GetComponent<RectTransform>().name = tempStringBuilder.ToString();
                _slotElement.GetComponent<RectTransform>().localScale = Vector3.one;
                _slotElement.GetComponent<RectTransform>().localPosition = Vector3.zero;
            }
        }
    }

    public void ConfirmNewPotion(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();

        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.EQUIP_CHECK_SLOT, err), Server.ServerMgr.Instance.CallClear);
			return;
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            for(int i=0; i<lstExpPotion.Count; i++)
            {
                lstExpPotion[i]._consumableItem.isNew = false;
                Legion.Instance.cInventory.dicInventory[lstExpPotion[i]._consumableItem.u2SlotNum].isNew = false;
            }
        }
    }
}
//포션 정보 클래스
public class ExpPotion
{
    public UInt16 u2ID;                         //포션 아이디
    public ConsumableItem _consumableItem;      //포션 
}
