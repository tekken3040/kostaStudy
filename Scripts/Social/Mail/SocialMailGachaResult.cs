using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class SocialMailGachaResult : MonoBehaviour
{
    public Image backArea;
    public Image fadeBox;
    public Image overBox;
    public GameObject popupEquipment;
    public GameObject popupGoods;
    public RectTransform itemPos;
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
    public Animator animator;

    public Image goodsGrade;
    public Image goodsIcon;
    public Text goodsCnt;
    public Text goodsName;
    
    public UI_Button_CharacterInfo_Equipment_StateInfo _specializeBtn;
    [SerializeField] RectTransform _trNameGroup;
    [SerializeField] RectTransform _trStarGroup;
    //[SerializeField] GameObject Pref_Star;
    [SerializeField] GameObject starPos;

    private bool showResult = false;
    private UInt16[] _slotNum;
    private GameObject gradeEffect;
    
    private GameObject effects;
    private readonly string effectPath = "Prefabs/UI_Effects/Ani_Eff_UI_StorePick"; 
    private Byte currentIndex = 0;

    private GameObject resultEffectObj;
    GameObject _objEquipModel;

    public void SetInfo(UInt16[] u2SlotNum, GameObject _obj, bool showEffect = true)
    {
        //등급 이펙트가 남아 있으면 제거
        if(gradeEffect != null)
            Destroy(gradeEffect);         

        resultEffectObj = _obj;
        backArea.gameObject.SetActive(false);
        popupEquipment.SetActive(false);
        popupGoods.SetActive(false);
        fadeBox.gameObject.SetActive(true);
        fadeBox.color = new Color(0f, 0f, 0f, 0f);
        
        if(effects != null)
            effects.gameObject.SetActive(false);
        
        StartCoroutine(ShowResult(u2SlotNum, currentIndex, showEffect));  
        currentIndex++;

        PopupManager.Instance.AddPopup(gameObject, Close);
        PopupManager.Instance.showLoading = true;             
    }

    public void SetResultEffect()
    {
        resultEffectObj.GetComponent<CanvasGroup>().blocksRaycasts = true;
        resultEffectObj.SetActive(true);
    }

    private IEnumerator ShowResult(UInt16[] u2SlotNum, Byte _index, bool showEffect = true)
    {
        //다시 뽑기 할 경우에는 연출을 보여주지 않는다
        if(showEffect)
        {
            resultEffectObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
            overBox.gameObject.SetActive(true);
            
            this._slotNum = u2SlotNum;
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
        }                
            
        if(showEffect)
        {
            yield return new WaitForSeconds(0.1f);
            //popupEquipment.SetActive(true);
            //animator.Play("Default");
        }
        else
        {
            //popupEquipment.SetActive(true);
            //animator.Play("Popup_Show");
            LeanTween.scale(this.gameObject, new Vector3(1.2f, 1.2f, 1f), 0.02f).setOnComplete(
                LeanTween.scale(this.gameObject, new Vector3(1f, 1f, 1f), 0.03f).setDelay(0.02f).onCompleteObject);
        }
           
        SetItem(Legion.Instance.cInventory.dicInventory[_slotNum[_index]]);
        
        //if(showEffect)
        //    yield return new WaitForSeconds(0.5f);
            
        overBox.gameObject.SetActive(false);
        if(showEffect)
            SetResultEffect();
        Time.timeScale = 1f;            
        
        fadeBox.gameObject.SetActive(false);
        backArea.gameObject.SetActive(true);
        PopupManager.Instance.showLoading = false;
    } 

    public void SetItem(Item _Item)
    {
        DebugMgr.Log(_Item.cItemInfo.u2ID);
        StringBuilder tempString = new StringBuilder();
        if(_Item.cItemInfo.ItemType == ItemInfo.ITEM_TYPE.EQUIPMENT)
        {
            popupEquipment.SetActive(false);
            EquipmentItem equipItem = new EquipmentItem();       
            equipItem = (EquipmentItem)_Item;
		    EquipmentInfo equipInfo = equipItem.GetEquipmentInfo();

            ForgeInfo forgeInfo = ForgeInfoMgr.Instance.GetList()[equipItem.u1SmithingLevel-1];

		    //아이템 정보
            itemName.text = string.Format("{0} {1}", equipItem.itemName, TextManager.Instance.GetText(equipInfo.sName));
		    itemName.color = EquipmentItem.equipElementColors[equipInfo.u1Element];
            UIManager.Instance.SetGradientFromElement(itemName.GetComponent<Gradient>(), equipItem.GetEquipmentInfo().u1Element);
            _specializeBtn.SetData((Byte)(equipInfo.u1Specialize+2));
            Color tempColor;
            ColorUtility.TryParseHtmlString(equipItem.GetEquipmentInfo().GetHexColor((Byte)(equipItem.GetEquipmentInfo().u1Specialize+2)), out tempColor);
            _specializeBtn.transform.parent.GetComponent<Image>().color = tempColor;
            //itemTier.text = "<" + TextManager.Instance.GetText( "forge_level_" + equipItem.u1SmithingLevel) + ">";

            tempString.Remove(0, tempString.Length);
            itemTier.text = TextManager.Instance.GetText(tempString.Append("forge_level_").Append(equipItem.u1SmithingLevel).ToString());
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
                tempString.Remove(0, tempString.Length);
                accSprite.sprite = AssetMgr.Instance.AssetLoad(tempString.Append("Sprites/Item/Accessory/acc_").Append(equipInfo.u2ModelID).Append(".png").ToString(), typeof(Sprite)) as Sprite;
		    }
            // 모델 생성
		    else
		    {
		    	accSprite.gameObject.SetActive(false);
		    	itemPos.gameObject.SetActive(true);

                if(equipItem.cObject != null)
			        _objEquipModel = Instantiate(equipItem.cObject);
                else
                {
                    equipItem.InitViewModelObject();
                    _objEquipModel = equipItem.cObject;
                }
		    	//equipItem.InitViewModelObject();
		    	_objEquipModel.transform.SetParent(itemPos);
		    	_objEquipModel.transform.localPosition = Vector3.zero;
		    	_objEquipModel.transform.localScale = Vector3.one;
		    	_objEquipModel.GetComponent<EquipmentObject>().SetLayer( LayerMask.NameToLayer("UI") );
		    }        
            
            //등급 이펙트 생성 
            if(gradeEffect != null)
                Destroy(gradeEffect);
            gradeEffect = Instantiate(AssetMgr.Instance.AssetLoad(string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}.prefab", equipItem.u1SmithingLevel), typeof(GameObject))) as GameObject;
            gradeEffect.transform.SetParent(popupEquipment.transform);
            gradeEffect.transform.name = "WeaponEffect";
            gradeEffect.transform.localScale = new Vector3(1f, 1f, 1f);
            gradeEffect.transform.localPosition = itemPos.parent.localPosition + Vector3.up * 50f;
		    
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
		    	if(equipItem.skillSlots[i] == 0)
		    	{
                    skillObject[i].SetActive(false);
		    	}
		    	else
		    	{				
                    skillObject[i].SetActive(true);
                    
		    		SkillInfo skillInfo = SkillInfoMgr.Instance.GetInfoBySlot(equipItem.GetEquipmentInfo().u2ClassID, equipItem.skillSlots[i]);

                    tempString.Remove(0, tempString.Length);
                    skillIcon[i].sprite = AtlasMgr.Instance.GetSprite(tempString.Append("Sprites/Skill/Atlas_SkillIcon_").Append(equipItem.GetEquipmentInfo().u2ClassID).Append(".").Append( skillInfo.u2ID).ToString());
                    skillName[i].text = TextManager.Instance.GetText(skillInfo.sName);
                    tempString.Remove(0, tempString.Length);
                    skillElement[i].sprite = AtlasMgr.Instance.GetSprite(tempString.Append("Sprites/Common/common_02_renew.common_02_skill_element_").Append(skillInfo.u1Element).ToString());
                    tempString.Remove(0, tempString.Length);
                    skillLevel[i].text = tempString.Append("+ ").Append(equipItem.statusComponent.points[i]).ToString();
		    		_btnSkillInfo[i].SetData(skillInfo);
		    	}
		    }
        }
        else
        {
            popupGoods.SetActive(false);
            tempString.Remove(0, tempString.Length);
            goodsGrade.sprite = AtlasMgr.Instance.GetSprite(tempString.Append("Sprites/Common/common_02_renew.grade_").Append(ItemInfoMgr.Instance.GetItemGrade(_Item.cItemInfo.u2ID)).ToString());
            goodsName.text = TextManager.Instance.GetText(Legion.Instance.cInventory.dicInventory[_Item.u2SlotNum].cItemInfo.sName);
            if(_Item.cItemInfo.ItemType == ItemInfo.ITEM_TYPE.CONSUMABLE)
            {
                goodsCnt.text = ((ConsumableItem)Legion.Instance.cInventory.dicInventory[_Item.u2SlotNum]).u2Count.ToString();
                tempString.Remove(0, tempString.Length);
                goodsIcon.sprite = AtlasMgr.Instance.GetSprite(tempString.Append("Sprites/Item/item_01.").Append(ItemInfoMgr.Instance.GetConsumableItemInfo(_Item.cItemInfo.u2ID).u2ID).ToString());
            }

            else if(_Item.cItemInfo.ItemType == ItemInfo.ITEM_TYPE.MATERIAL)
            {
                goodsCnt.text = ((MaterialItem)Legion.Instance.cInventory.dicInventory[_Item.u2SlotNum]).u2Count.ToString();
                tempString.Remove(0, tempString.Length);
                goodsIcon.sprite = AtlasMgr.Instance.GetSprite(tempString.Append("Sprites/Item/item_01.").Append(ItemInfoMgr.Instance.GetMaterialItemInfo(_Item.cItemInfo.u2ID).u2IconID).ToString());
            }

            else if(_Item.cItemInfo.ItemType == ItemInfo.ITEM_TYPE.RUNE)
            {
                goodsCnt.text = "1";
                tempString.Remove(0, tempString.Length);
                goodsIcon.sprite = AtlasMgr.Instance.GetSprite(tempString.Append("Sprites/Item/rune_01.").Append(ItemInfoMgr.Instance.GetRuneInfo(_Item.cItemInfo.u2ID).u2ID).ToString());
            }
        }
        
        showResult = true;
    }

    
    public void Close()
    {
        //if(currentIndex < _slotNum.Length)
        //{
        //    StartCoroutine(ShowResult(_slotNum, currentIndex, false));
        //    currentIndex++;
        //}
        //else
        //{
            PopupManager.Instance.RemovePopup(gameObject);
            PopupManager.Instance.showLoading = false;
            Destroy(gameObject);
        //}
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
