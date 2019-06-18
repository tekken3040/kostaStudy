using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class UI_Result_Gacha_ItemWindow : MonoBehaviour
{
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

    public UI_Button_CharacterInfo_Equipment_StateInfo _specializeBtn;
    [SerializeField] RectTransform _trNameGroup;
    [SerializeField] RectTransform _trStarGroup;
    //[SerializeField] GameObject Pref_Star;
    [SerializeField] GameObject starPos;

    private GameObject gradeEffect;
                                        
    private UI_Shop_Gacha_Result_Effect gachaResultPanel;

    public void SetItem(ShopItem shopItem)
    {
        StringBuilder tempString = new StringBuilder();
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
        
		itemTier.text = TextManager.Instance.GetText(tempString.Append("forge_level_").Append(shopItem.cEquipInfo.u1SmithingLevel).ToString());
		UIManager.Instance.SetGradientFromTier(itemTier.GetComponent<Gradient>(), shopItem.cEquipInfo.u1SmithingLevel);
        for(int i=0; i<starPos.transform.childCount; i++)
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
            tempString.Remove(0, tempString.Length);
            elementIcon.sprite = AtlasMgr.Instance.GetSprite (tempString.Append("Sprites/Common/common_02_renew.element_").Append(equipInfo.u1Element).ToString());
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
            tempString.Remove(0, tempString.Length);
            _imgClassIcon.sprite = AtlasMgr.Instance.GetSprite(tempString.Append("Sprites/Common/common_class.common_class_").Append(equipItem.GetEquipmentInfo().u2ClassID).ToString());
            _imgClassIcon.enabled = true;
            _imgClassIcon.transform.localScale = new Vector3(1.3f, 1.3f, 1f);
            _imgClassIcon.SetNativeSize();
        }
        else
        {
            className.text = TextManager.Instance.GetText("equip_common");
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
            tempString.Remove(0, tempString.Length);
            accSprite.sprite = AssetMgr.Instance.AssetLoad(tempString.Append("Sprites/Item/Accessory/acc_").Append(equipInfo.u2ModelID).Append(".png").ToString(), typeof(Sprite)) as Sprite;
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
        if(gradeEffect != null)
            Destroy(gradeEffect);
        gradeEffect = Instantiate(AssetMgr.Instance.AssetLoad(string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}.prefab", shopItem.cEquipInfo.u1SmithingLevel), typeof(GameObject))) as GameObject;
        gradeEffect.transform.SetParent(popupObject.transform);
        gradeEffect.transform.name = "WeaponEffect";
        gradeEffect.transform.localScale = new Vector3(1f, 1f, 1f);
        gradeEffect.transform.localPosition = itemPos.parent.localPosition + Vector3.up * 50f;
        gradeEffect.transform.localPosition = new Vector3(gradeEffect.transform.localPosition.x, gradeEffect.transform.localPosition.y, 0);

		UInt32[] tempStat = new UInt32[Server.ConstDef.SkillOfEquip + (Server.ConstDef.EquipStatPointType * 2)];
		
		//for(int i=0; i<shopItem.cEquipInfo.u2ArrBaseStat.Length; i++)
		for(int i=0; i<shopItem.cEquipInfo.u4ArrBaseStat.Length; i++)
        {
			tempStat[i + Server.ConstDef.SkillOfEquip] = shopItem.cEquipInfo.u4ArrBaseStat[i];
        }

        equipItem.statusComponent.LoadStatusEquipment(tempStat, equipItem.GetEquipmentInfo().acStatAddInfo, 0);
        equipItem.GetComponent<LevelComponent>().Set(shopItem.cEquipInfo.u2Level, shopItem.cEquipInfo.u8Exp);
        
        //스탯 정보 세팅
		for(int i=0; i<Server.ConstDef.EquipStatPointType; i++)
		{
			byte statType = equipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType;
			statName[i].text = Status.GetStatText(statType);
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

                tempString.Remove(0, tempString.Length);
                skillIcon[i].sprite = AtlasMgr.Instance.GetSprite(tempString.Append("Sprites/Skill/Atlas_SkillIcon_").Append(equipItem.GetEquipmentInfo().u2ClassID).Append(".").Append(skillInfo.u2ID).ToString());
                skillName[i].text = TextManager.Instance.GetText(skillInfo.sName);
                tempString.Remove(0, tempString.Length);
                skillElement[i].sprite = AtlasMgr.Instance.GetSprite(tempString.Append("Sprites/Common/common_02_renew.common_02_skill_element_").Append(skillInfo.u1Element).ToString());
                tempString.Remove(0, tempString.Length);
                skillLevel[i].text = tempString.Append("+ ").Append(shopItem.cEquipInfo.u1ArrSkillPoint[i]).ToString();
				_btnSkillInfo[i].SetData(skillInfo);
			}
		}
    }

    public void SetItem(EquipmentItem equipmentItem)
    {
        EquipmentItem equipItem = equipmentItem;
		EquipmentInfo equipInfo = equipItem.GetEquipmentInfo();

        ForgeInfo forgeInfo = ForgeInfoMgr.Instance.GetList()[equipItem.u1SmithingLevel-1];

		//아이템 정보
		//itemName.text = string.Format("{0} {1}", TextManager.Instance.GetText(equipInfo.sName), TextManager.Instance.GetText(equipInfo.sName));
        itemName.text = TextManager.Instance.GetText(equipInfo.sName);
		itemName.color = EquipmentItem.equipElementColors[equipInfo.u1Element];
        UIManager.Instance.SetGradientFromElement(itemName.GetComponent<Gradient>(), equipItem.GetEquipmentInfo().u1Element);
        _specializeBtn.SetData((Byte)(equipInfo.u1Specialize+2));
        Color tempColor;
        ColorUtility.TryParseHtmlString(equipItem.GetEquipmentInfo().GetHexColor((Byte)(equipItem.GetEquipmentInfo().u1Specialize+2)), out tempColor);
        _specializeBtn.transform.parent.GetComponent<Image>().color = tempColor;
        
		itemTier.text = TextManager.Instance.GetText( "forge_level_" + equipItem.u1SmithingLevel);
		UIManager.Instance.SetGradientFromTier(itemTier.GetComponent<Gradient>(), equipItem.u1SmithingLevel);
        for(int i=0; i<starPos.transform.childCount; i++)
            starPos.transform.GetChild(i).gameObject.SetActive(false);
        for(int i=0; i<equipItem.u1Completeness; i++)
        {
            starPos.transform.GetChild(i).gameObject.SetActive(true);
            UIManager.Instance.SetGradientFromTier(starPos.transform.GetChild(i).GetComponent<Gradient>(), equipItem.u1SmithingLevel);
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
            className.text = TextManager.Instance.GetText("equip_common");
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
        if(gradeEffect != null)
            Destroy(gradeEffect);
        gradeEffect = Instantiate(AssetMgr.Instance.AssetLoad(string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}.prefab", equipItem.u1SmithingLevel), typeof(GameObject))) as GameObject;
        gradeEffect.transform.SetParent(popupObject.transform);
        gradeEffect.transform.name = "WeaponEffect";
        gradeEffect.transform.localScale = new Vector3(1f, 1f, 1f);
        gradeEffect.transform.localPosition = itemPos.parent.localPosition + Vector3.up * 50f;
        gradeEffect.transform.localPosition = new Vector3(gradeEffect.transform.localPosition.x, gradeEffect.transform.localPosition.y, 0);
        
        //스탯 정보 세팅
		for(int i=0; i<Server.ConstDef.EquipStatPointType; i++)
		{
			byte statType = equipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType;
			statName[i].text = Status.GetStatText(statType);
            statValue[i].text = equipItem.statusComponent.FINAL_STATUS.GetStat(statType).ToString();
			_btnStatInfo[i].SetData( equipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType);
		}

        //스킬 정보 세팅
		for(int i=0; i<Server.ConstDef.SkillOfEquip; i++)
		{
			if(equipItem.GetPoints()[i] == 0)
			{
                skillObject[i].SetActive(false);
			}
			else
			{				
                skillObject[i].SetActive(true);
                
				SkillInfo skillInfo = SkillInfoMgr.Instance.GetInfoBySlot(equipItem.GetEquipmentInfo().u2ClassID, equipItem.skillSlots[i]);
                skillIcon[i].sprite = AtlasMgr.Instance.GetSprite("Sprites/Skill/Atlas_SkillIcon_" + equipItem.GetEquipmentInfo().u2ClassID + "." + skillInfo.u2ID);
                skillName[i].text = TextManager.Instance.GetText(skillInfo.sName);
                skillElement[i].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.common_02_skill_element_" + skillInfo.u1Element);
                skillLevel[i].text = equipItem.GetPoints()[i].ToString();
				_btnSkillInfo[i].SetData(skillInfo);
			}
		}
    }

    public void Close()
    {
        PopupManager.Instance.RemovePopup(gameObject);
        gameObject.SetActive(false);
    }
}
