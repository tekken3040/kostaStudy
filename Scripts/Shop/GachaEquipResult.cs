using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
public class GachaEquipResult : MonoBehaviour {

    public Image backArea;
    public Image fadeBox;
    public Image overBox;
    public GameObject popupObject;
    public RectTransform itemPos;
    public Image accImage;
    public SpriteRenderer accSprite;
    public Text itemName;
    public Text itemTier;
    public Text className;
    [SerializeField] Image _imgClassIcon;
    public GameObject elementLabel;
    public Image elementIcon;
    public Text[] statName;
    public Text[] statValue;
	public UI_Button_CharacterInfo_Equipment_StatInfo[] _btnStatInfo;
    public GameObject[] skillObject;
    public Image[] skillIcon;
    public Image[] skillElement;
    public Text[] skillName;
    public Text[] skillLevel;
	public UI_Button_CharacterInfo_Equipment_SkillInfo[] _btnSkillInfo;
    public GameObject retryObject;
    public Image priceIcon;
    public Text priceValue;
    public Text discountText;
    
    public GameObject nextButton;
    
    public Animator animator;

    public UI_Button_CharacterInfo_Equipment_StateInfo _specializeBtn;
    [SerializeField] RectTransform _trNameGroup;
    [SerializeField] RectTransform _trStarGroup;
    //[SerializeField] GameObject Pref_Star;
    [SerializeField] GameObject starPos;

    private bool showResult = false;
    private UInt16 shopID;
    private int retryPrice;
    private GameObject gradeEffect;
    
    private GameObject effects;
    private readonly string effectPath = "Prefabs/UI_Effects/Ani_Eff_UI_StorePick";                                         
    private UI_Shop_Gacha_Result_Effect gachaResultPanel;

    public void SetInfo(UInt16 shopID, bool showEffect = true)
    {
        //등급 이펙트가 남아 있으면 제거
        if(gradeEffect != null)
            Destroy(gradeEffect);         
        
        //Legion.Instance.cTutorial.CheckTutorial(5);
        
        backArea.gameObject.SetActive(false);
        popupObject.SetActive(false);
        fadeBox.gameObject.SetActive(true);
        fadeBox.color = new Color(0f, 0f, 0f, 0f);
        
        if(effects != null)
            effects.gameObject.SetActive(false);
        
        StartCoroutine(ShowResult(shopID, showEffect));            
        PopupManager.Instance.AddPopup(gameObject, Close);
        PopupManager.Instance.showLoading = true;             
    }
    
    public void SetResultEffect()
    {
        GameObject resultEffectObject = AssetMgr.Instance.AssetLoad("Prefabs/UI/Shop/Pref_UI_Panel_Shop_Gacha_Result_Effect.prefab", typeof(GameObject)) as GameObject;
        gachaResultPanel = Instantiate(resultEffectObject).GetComponent<UI_Shop_Gacha_Result_Effect>();
        gachaResultPanel.GetComponent<RectTransform>().SetParent(transform.parent.parent);
        gachaResultPanel.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0f, 0f, -400f);
        gachaResultPanel.GetComponent<RectTransform>().sizeDelta = Vector3.zero;
        gachaResultPanel.transform.localScale = Vector3.one;
        gachaResultPanel.transform.name = "Pref_UI_Panel_Shop_Gacha_Result_Effect";
        gachaResultPanel.gameObject.SetActive(true); 
        
        if(ShopInfoMgr.Instance.lstFixItem.Count == 1 && ShopInfoMgr.Instance.lstFixItem[0].u1Type == (Byte)GoodsType.EQUIP)
        {
            EquipmentItem equipItem = new EquipmentItem(ShopInfoMgr.Instance.lstFixItem[0].u2ItemID); 
            equipItem.u1Completeness = ShopInfoMgr.Instance.lstFixItem[0].cEquipInfo.u1Completeness;
            gachaResultPanel.SetData(equipItem, ShopInfoMgr.Instance.lstFixItem[0].cEquipInfo.u1SmithingLevel, popupObject);
        }
        
        else if(ShopInfoMgr.Instance.lstFixItem.Count > 1 && ShopInfoMgr.Instance.lstFixItem[0].u1Type == (Byte)GoodsType.EQUIP)
        {
            ShopItem[] tempShopItem = new ShopItem[ShopInfoMgr.Instance.lstFixItem.Count];
            for(int i=0; i<ShopInfoMgr.Instance.lstFixItem.Count; i++)
            {
                tempShopItem[i] = ShopInfoMgr.Instance.lstFixItem[i];
            }
        
            gachaResultPanel.SetData(tempShopItem, popupObject);
        }
        
        else if(ShopInfoMgr.Instance.lstFixItem.Count > 1 && ShopInfoMgr.Instance.lstFixItem[0].u1Type == (Byte)GoodsType.MATERIAL)
        {
            MaterialItem[] materialItem = new MaterialItem[ShopInfoMgr.Instance.lstFixItem.Count];
            for(int i=0; i<ShopInfoMgr.Instance.lstFixItem.Count; i++)
                materialItem[i] = new MaterialItem(ShopInfoMgr.Instance.lstFixItem[i].u2ItemID, (UInt16)ShopInfoMgr.Instance.lstFixItem[i].u4Count);
        
            gachaResultPanel.SetData(materialItem, null);
        }
    }

    private IEnumerator ShowResult(UInt16 shopID, bool showEffect = true)
    {
        //다시 뽑기 할 경우에는 연출을 보여주지 않는다
        if(showEffect)
        {
            overBox.gameObject.SetActive(true);
            
            this.shopID = shopID;
            showResult = false;
            
            LeanTween.alpha(fadeBox.rectTransform, 1f, 1f);
            
			ResourceRequest req = Resources.LoadAsync(effectPath, typeof(GameObject));
			yield return req;
                            
			effects = Instantiate(req.asset) as GameObject;
            effects.transform.SetParent(transform);                
            effects.transform.localPosition = new Vector3(0f, 0f, -500f);

            effects.transform.localScale = Vector3.one;
            effects.SetActive(false);                                
        
            yield return new WaitForSeconds(1.5f);
            effects.gameObject.SetActive(true);
            yield return new WaitForSeconds(5f);
            LeanTween.alpha(fadeBox.rectTransform, 0.5f, 1f);
            //yield return new WaitForSeconds(0.5f);
            //effects[1].gameObject.SetActive(true);
        }                
        
        // 1개 뽑기 일 경우
        if(ShopInfoMgr.Instance.lstFixItem.Count == 1)
        {
            retryObject.SetActive(true);
            nextButton.SetActive(false);
            discountText.text = string.Format("{0}"+TextManager.Instance.GetText("popup_desc_gacha_discount"), ShopInfoMgr.Instance.dicShopGoodData[shopID].u1Discount + LegionInfoMgr.Instance.GetCurrentVIPInfo().u1VIPGachaPer);
            //retryPrice = (int)((float)ShopInfoMgr.Instance.dicShopGoodData[shopID].cBuyGoods.u4Count * ((100f - (float)ShopInfoMgr.Instance.dicShopGoodData[shopID].u1Discount) / 100f));
            retryPrice = ShopInfoMgr.Instance.getGachaDiscount(shopID);
            priceValue.text = retryPrice.ToString();
            priceIcon.sprite = ShopInfoMgr.Instance.GetGoodsSprites(ShopInfoMgr.Instance.dicShopGoodData[shopID].cBuyGoods);
            priceIcon.SetNativeSize();

            if(ShopInfoMgr.Instance.dicShopGoodData[shopID].cBuyGoods.u1Type != (Byte)GoodsType.FRIENDSHIP_POINT)
                discountText.gameObject.SetActive(true);
            else
                discountText.gameObject.SetActive(false);

            if(showEffect)
            {
                yield return new WaitForSeconds(0.1f);
                //popupObject.SetActive(true);
                animator.Play("Default");
            }
            else
            {
                popupObject.SetActive(true);
                animator.Play("Popup_Show");
            }
            //if(!showResult)
                
            SetItem(ShopInfoMgr.Instance.lstFixItem[0]);      
            
            //if(showEffect)
            //    yield return new WaitForSeconds(0.5f);
                
            overBox.gameObject.SetActive(false);
            if(showEffect)
            {
                popupObject.SetActive(false);
                SetResultEffect();
            }
            Time.timeScale = 1f;            
            Legion.Instance.cTutorial.CheckTutorial(MENU.SHOP);
        }
        //2개 이상 뽑을 경우
        else
        {
            nextButton.SetActive(false);
            retryObject.SetActive(false);
            popupObject.SetActive(false);
            SetResultEffect();
            for(int i=0; i<ShopInfoMgr.Instance.lstFixItem.Count; i++)
            {                
                yield return new WaitForSeconds(0.1f);
                //popupObject.SetActive(true);
                
                
                //if(i == 0)
                //    animator.Play("Default");
                //else
                //    animator.Play("Popup_Show");
                    
                //SetItem(ShopInfoMgr.Instance.lstFixItem[i]);                
                showResult = true;
                if(i == ShopInfoMgr.Instance.lstFixItem.Count - 1)
                {
                    retryObject.SetActive(true);
                    nextButton.SetActive(false);
                    discountText.text = string.Format("{0}"+TextManager.Instance.GetText("popup_desc_gacha_discount"), ShopInfoMgr.Instance.dicShopGoodData[shopID].u1Discount + LegionInfoMgr.Instance.GetCurrentVIPInfo().u1VIPGachaPer);
                    //retryPrice = (int)((float)ShopInfoMgr.Instance.dicShopGoodData[shopID].cBuyGoods.u4Count * ((100f - (float)ShopInfoMgr.Instance.dicShopGoodData[shopID].u1Discount) / 100f));
                    retryPrice = ShopInfoMgr.Instance.getGachaDiscount(shopID);
                    priceValue.text = retryPrice.ToString();
                    priceIcon.sprite = ShopInfoMgr.Instance.GetGoodsSprites(ShopInfoMgr.Instance.dicShopGoodData[shopID].cBuyGoods);
                    priceIcon.SetNativeSize();

                    if(ShopInfoMgr.Instance.dicShopGoodData[shopID].cBuyGoods.u1Type != (Byte)GoodsType.FRIENDSHIP_POINT)
                        discountText.gameObject.SetActive(true);
                    else
                        discountText.gameObject.SetActive(false);
                }
                
                //yield return new WaitForSeconds(0.5f);
                overBox.gameObject.SetActive(false);
                Time.timeScale = 1f;
                
                //다음 아이템 정보를 보여주기 위해 대기
                while(showResult)
                {
                    yield return null;
                }
            }            
        }               
        
        fadeBox.gameObject.SetActive(false);
        backArea.gameObject.SetActive(true);
        //PopupManager.Instance.showLoading = false;
    } 
    
    public void SetItem(ShopItem shopItem)
    {
        DebugMgr.Log(shopItem.u2ItemID);
        
        EquipmentItem equipItem = new EquipmentItem(shopItem.u2ItemID);       
        equipItem.u1Completeness = shopItem.cEquipInfo.u1Completeness;
		EquipmentInfo equipInfo = equipItem.GetEquipmentInfo();

        ForgeInfo forgeInfo = ForgeInfoMgr.Instance.GetList()[shopItem.cEquipInfo.u1SmithingLevel-1];

		//아이템 정보
		itemName.text = string.Format("{0} {1}", shopItem.cEquipInfo.strItemName, TextManager.Instance.GetText(equipInfo.sName));
		itemName.color = EquipmentItem.equipElementColors[equipInfo.u1Element];
        UIManager.Instance.SetGradientFromElement(itemName.GetComponent<Gradient>(), equipItem.GetEquipmentInfo().u1Element);
        _specializeBtn.SetData((Byte)(equipInfo.u1Specialize+2));
        Color tempColor;
        ColorUtility.TryParseHtmlString(equipItem.GetEquipmentInfo().GetHexColor((Byte)(equipItem.GetEquipmentInfo().u1Specialize+2)), out tempColor);
        _specializeBtn.transform.parent.GetComponent<Image>().color = tempColor;
        //itemTier.text = "<" + TextManager.Instance.GetText( "forge_level_" + shopItem.cEquipInfo.u1SmithingLevel) + ">";
		itemTier.text = TextManager.Instance.GetText( "forge_level_" + shopItem.cEquipInfo.u1SmithingLevel);
		UIManager.Instance.SetGradientFromTier(itemTier.GetComponent<Gradient>(), shopItem.cEquipInfo.u1SmithingLevel);
        for(int i=0; i<starPos.transform.GetChildCount(); i++)
            starPos.transform.GetChild(i).gameObject.SetActive(false);
        for(int i=0; i<shopItem.cEquipInfo.u1Completeness; i++)
        {
            starPos.transform.GetChild(i).gameObject.SetActive(true);
            UIManager.Instance.SetGradientFromTier(starPos.transform.GetChild(i).GetComponent<Gradient>(), shopItem.cEquipInfo.u1SmithingLevel);
        }
        starPos.GetComponent<GridLayoutGroup>().SetLayoutHorizontal();
        itemName.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
        UIManager.Instance.SetSizeTextGroup(_trNameGroup, 18);
        UIManager.Instance.SetSizeTextGroup(_trStarGroup, 18);

        //속성이 있을경우 표시
        if(equipInfo.u1Element > 0)
        {				
            elementIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.element_" + equipInfo.u1Element);
            elementIcon.SetNativeSize();
            elementIcon.gameObject.SetActive(true);
            elementLabel.gameObject.SetActive(true);
        }
        else
        {
            elementIcon.gameObject.SetActive(false);
            elementLabel.gameObject.SetActive(false);
        }
        
        // 악세사리는 클래스 정보가 없음
        if(equipItem.GetEquipmentInfo().u2ClassID <= ClassInfo.LAST_CLASS_ID)
        {
            ClassInfo classInfo = ClassInfoMgr.Instance.GetInfo(Convert.ToUInt16(equipItem.GetEquipmentInfo().u2ClassID));
		    className.text = TextManager.Instance.GetText(classInfo.sName);
            _imgClassIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_class.common_class_" + equipItem.GetEquipmentInfo().u2ClassID);
            _imgClassIcon.enabled = true;
            _imgClassIcon.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
            _imgClassIcon.SetNativeSize();
        }
        else
        {
            className.text = "";
            _imgClassIcon.enabled = false;
        }
        
        // 이미 생성된 장비 모델이 있으면 제거
		if(itemPos.transform.childCount != 0)
			DestroyImmediate(itemPos.transform.GetChild(0).gameObject);

        // 악세사리는 스프라이트로 표시
		if(equipInfo.u1PosID == EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 || equipInfo.u1PosID == EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
		{
			accSprite.gameObject.SetActive(true);
			itemPos.gameObject.SetActive(false);

            accSprite.sprite = AssetMgr.Instance.AssetLoad("Sprites/Item/Accessory/acc_" + equipInfo.u2ModelID + ".png", typeof(Sprite)) as Sprite;
			// accImage.sprite = AssetMgr.Instance.AssetLoad("Sprites/Item/Accessory/acc_" + equipInfo.u2ModelID, typeof(Sprite)) as Sprite;
			// accImage.SetNativeSize();
		}
        // 모델 생성
		else
		{
			accSprite.gameObject.SetActive(false);
			itemPos.gameObject.SetActive(true);
						
			equipItem.InitViewModelObject();
			equipItem.cObject.transform.SetParent(itemPos);
			equipItem.cObject.transform.localPosition = Vector3.zero;
			equipItem.cObject.transform.localScale = Vector3.one;
			equipItem.cObject.GetComponent<EquipmentObject>().SetLayer( LayerMask.NameToLayer("UI") );
		}        
        
        //등급 이펙트 생성 
        // if(shopItem.cEquipInfo.u1SmithingLevel > 1)
        // {
            gradeEffect = Instantiate(AssetMgr.Instance.AssetLoad(string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}.prefab", shopItem.cEquipInfo.u1SmithingLevel), typeof(GameObject))) as GameObject;
            gradeEffect.transform.SetParent(popupObject.transform);
            gradeEffect.transform.name = "WeaponEffect";
            gradeEffect.transform.localScale = new Vector3(1f, 1f, 1f);
            gradeEffect.transform.localPosition = itemPos.parent.localPosition + Vector3.up * 50f;
            gradeEffect.transform.localPosition = new Vector3(gradeEffect.transform.localPosition.x, gradeEffect.transform.localPosition.y, 0);
        // }
		UInt32[] tempStat = new UInt32[Server.ConstDef.SkillOfEquip + (Server.ConstDef.EquipStatPointType * 2)];
		//shopItem.cEquipInfo.u2ArrBaseStat.Length
		//for(int i=0; i<shopItem.cEquipInfo.u2ArrBaseStat.Length; i++)
		for(int i=0; i<shopItem.cEquipInfo.u4ArrBaseStat.Length; i++)
        {
			tempStat[i + Server.ConstDef.SkillOfEquip] = shopItem.cEquipInfo.u4ArrBaseStat[i];//u2ArrBaseStat[i];
        }

        equipItem.statusComponent.LoadStatusEquipment(tempStat, equipItem.GetEquipmentInfo().acStatAddInfo, 0);
        equipItem.GetComponent<LevelComponent>().Set(shopItem.cEquipInfo.u2Level, shopItem.cEquipInfo.u8Exp);
        
        //스탯 정보 세팅
		for(int i=0; i<Server.ConstDef.EquipStatPointType; i++)
		{
			byte statType = equipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType;
			statName[i].text = Status.GetStatText(statType);
			//statValue[i].text = equipItem.cStatus.GetStat(statType, shopItem.cEquipInfo.u2ArrBaseStat[i]).ToString();
            statValue[i].text = equipItem.statusComponent.FINAL_STATUS.GetStat(statType).ToString();
			_btnStatInfo[i].SetData( equipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType);
		}

        //스킬 정보 세팅
		for(int i=0; i<Server.ConstDef.SkillOfEquip; i++)
		{
			if(shopItem.cEquipInfo.u1ArrSkillSlots[i] == 0)
			{
                skillObject[i].SetActive(false);
			}
			else
			{				
                skillObject[i].SetActive(true);
                
				SkillInfo skillInfo = SkillInfoMgr.Instance.GetInfoBySlot(equipItem.GetEquipmentInfo().u2ClassID, shopItem.cEquipInfo.u1ArrSkillSlots[i]);
                skillIcon[i].sprite = AtlasMgr.Instance.GetSprite("Sprites/Skill/Atlas_SkillIcon_" + equipItem.GetEquipmentInfo().u2ClassID + "." + skillInfo.u2ID);
                skillName[i].text = TextManager.Instance.GetText(skillInfo.sName);
                skillElement[i].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.common_02_skill_element_" + skillInfo.u1Element);
                skillLevel[i].text = "+ " + shopItem.cEquipInfo.u1ArrSkillPoint[i].ToString();
				_btnSkillInfo[i].SetData(skillInfo);
			}
		}
        //if(!showResult)
            //popupObject.SetActive(false);
        showResult = true;
    }
    
    public void OnClickNext()
    {
        if(gradeEffect != null)
            Destroy(gradeEffect);
             
        animator.Play("Popup_Hide");
    }
    
    // 다시 뽑기 할 경우에는 비용이 감소 된다 
    public void OnClickRetry()
    {
        if(!Legion.Instance.CheckEnoughGoods(ShopInfoMgr.Instance.dicShopGoodData[shopID].cBuyGoods.u1Type, retryPrice))
        {
			PopupManager.Instance.ShowChargePopup(ShopInfoMgr.Instance.dicShopGoodData[shopID].cBuyGoods.u1Type);
            return;
        }
        if(gradeEffect != null)
            Destroy(gradeEffect);        
        
        PopupManager.Instance.ShowLoadingPopup(1);
        if(ShopInfoMgr.Instance.dicShopGoodData[shopID].cBuyGoods.u1Type != (Byte)GoodsType.FRIENDSHIP_POINT)
	        Server.ServerMgr.Instance.ShopFixShop (shopID, 1, 255, 0, "", "", AckFixShop);  
        else
            Server.ServerMgr.Instance.ShopFixShop (shopID, 0, 255, 0, "", "", AckFixShop);  
        //PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("popup_title_gacha_again"), TextManager.Instance.GetText("popup_desc_gacha_again"), GachaRetry, null);
    }

    public void OnClickGachaRetry()
    {
        if(gradeEffect != null)
            Destroy(gradeEffect);        
        
        PopupManager.Instance.ShowLoadingPopup(1);
        if(ShopInfoMgr.Instance.dicShopGoodData[shopID].cBuyGoods.u1Type != (Byte)GoodsType.FRIENDSHIP_POINT)
	        Server.ServerMgr.Instance.ShopFixShop (shopID, 1, 255, 0, "", "", AckFixShop2);  
        else
            Server.ServerMgr.Instance.ShopFixShop (shopID, 0, 255, 0, "", "", AckFixShop2);  
        //PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("popup_title_gacha_again"), TextManager.Instance.GetText("popup_desc_gacha_again"), GachaRetry, null);
    }
    private void AckFixShop2(Server.ERROR_ID err)
    {
        DebugMgr.Log("AckFixShop " + err);

		PopupManager.Instance.CloseLoadingPopup();
		
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.SHOP_FIXSHOP, err), Server.ServerMgr.Instance.CallClear);
		}
		else
        {
            //int count = (int)((float)ShopInfoMgr.Instance.dicShopGoodData[shopID].cBuyGoods.u4Count * ((100f - (float)ShopInfoMgr.Instance.dicShopGoodData[shopID].u1Discount) / 100f));
            int count = ShopInfoMgr.Instance.getGachaDiscount(shopID);
            ShopGoodInfo shopGoodInfo = ShopInfoMgr.Instance.dicShopGoodData[shopID];
            Legion.Instance.SubGoods(shopGoodInfo.cBuyGoods.u1Type, count);
            
            SetInfo(shopID, false);
        }
    }
    // 다시 뽑기 요청
    private void GachaRetry(object[] param)
    {
        if(gradeEffect != null)
            Destroy(gradeEffect);        
        
        PopupManager.Instance.ShowLoadingPopup(1);
        if(ShopInfoMgr.Instance.dicShopGoodData[shopID].cBuyGoods.u1Type != (Byte)GoodsType.FRIENDSHIP_POINT)
	        Server.ServerMgr.Instance.ShopFixShop (shopID, 1, 255, 0, "", "", AckFixShop);    
        else
            Server.ServerMgr.Instance.ShopFixShop (shopID, 0, 255, 0, "", "", AckFixShop);      
    }
    
    
    // 다시 뽑기 결과 처리
    private void AckFixShop(Server.ERROR_ID err)
    {
        DebugMgr.Log("AckFixShop " + err);

		PopupManager.Instance.CloseLoadingPopup();
		
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.SHOP_FIXSHOP, err), Server.ServerMgr.Instance.CallClear);
		}
		else
        {
            //int count = (int)((float)ShopInfoMgr.Instance.dicShopGoodData[shopID].cBuyGoods.u4Count * ((100f - (float)ShopInfoMgr.Instance.dicShopGoodData[shopID].u1Discount) / 100f));
            int count = ShopInfoMgr.Instance.getGachaDiscount(shopID);
            ShopGoodInfo shopGoodInfo = ShopInfoMgr.Instance.dicShopGoodData[shopID];
            Legion.Instance.SubGoods(shopGoodInfo.cBuyGoods.u1Type, count);
            
            StartCoroutine(ResetInfo());
        }
    }
    
    IEnumerator ResetInfo()
    {
        animator.Play("Popup_Hide");

        while(showResult)
            yield return null;

        SetInfo(shopID, false);
    }
    
    public void OnClickClose()
    {
       if(showResult)
          showResult = false;
    }
    
    public void Close()
    {
        PopupManager.Instance.RemovePopup(gameObject);
        //gameObject.SetActive(false);
        Destroy(gameObject);
    }

    public UInt16 GetShopID()
    {
        return shopID;
    }
    
    // 누르고 있을 경우 연출 시간을 빠르게 함
    public void OnDown()
    {        
        Time.timeScale = 2f;   
    }
    
    public void OnUp()
    {
        Time.timeScale = 1f;
    }
}
