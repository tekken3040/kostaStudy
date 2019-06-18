using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class GachaResult : MonoBehaviour
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
	private GameObject gradeEffect;

	private GameObject effects;
	private readonly string effectPath = "Prefabs/UI_Effects/Ani_Eff_UI_StorePick"; 
	private Byte currentIndex = 0;

	GameObject _objEquipModel;

	public GameObject PrefGachaResultEffect;

	public void SetInfo(ShopItem item)
	{     
		backArea.gameObject.SetActive(false);
		popupEquipment.SetActive(false);
		popupGoods.SetActive(false);
		fadeBox.gameObject.SetActive(true);
		fadeBox.color = new Color(0f, 0f, 0f, 0f);

		if(effects != null)
			effects.gameObject.SetActive(false);

		StartCoroutine(ShowResult(item));  
		currentIndex++;

		PopupManager.Instance.AddPopup(gameObject, Close);
		PopupManager.Instance.showLoading = true;             
	}

	private IEnumerator ShowResult(ShopItem item)
	{
		overBox.gameObject.SetActive(true);

		showResult = false;

		LeanTween.alpha(fadeBox.rectTransform, 1f, 1f);

		UnityEngine.Object effectPrefab = null;                

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

		yield return new WaitForSeconds(0.1f);

		SetItem(item);
		if(item.u1Type == (Byte)GoodsType.EQUIP) popupEquipment.SetActive (true);
		else popupGoods.SetActive (true);

		//if(showEffect)
		//    yield return new WaitForSeconds(0.5f);

		overBox.gameObject.SetActive(false);

		Time.timeScale = 1f;            

		fadeBox.gameObject.SetActive(false);
		backArea.gameObject.SetActive(true);
		PopupManager.Instance.showLoading = false;
	} 

	public void SetItem(ShopItem _Item)
	{
		Goods _goodsInfo = new Goods (_Item.u1Type, _Item.u2ItemID, _Item.u4Count);
		if(_Item.u1Type == (Byte)GoodsType.EQUIP)
		{
			EquipmentItem equipItem = new EquipmentItem(_Item.u2ItemID);       
			equipItem.u1Completeness = _Item.cEquipInfo.u1Completeness;
			EquipmentInfo equipInfo = equipItem.GetEquipmentInfo();

			ForgeInfo forgeInfo = ForgeInfoMgr.Instance.GetList()[_Item.cEquipInfo.u1SmithingLevel-1];

			//아이템 정보
			itemName.text = string.Format("{0} {1}", _Item.cEquipInfo.strItemName, TextManager.Instance.GetText(equipInfo.sName));
			itemName.color = EquipmentItem.equipElementColors[equipInfo.u1Element];
			UIManager.Instance.SetGradientFromElement(itemName.GetComponent<Gradient>(), equipItem.GetEquipmentInfo().u1Element);
			_specializeBtn.SetData((Byte)(equipInfo.u1Specialize+2));
			Color tempColor;
			ColorUtility.TryParseHtmlString(equipItem.GetEquipmentInfo().GetHexColor((Byte)(equipItem.GetEquipmentInfo().u1Specialize+2)), out tempColor);
			_specializeBtn.transform.parent.GetComponent<Image>().color = tempColor;
			//itemTier.text = "<" + TextManager.Instance.GetText( "forge_level_" + _Item.cEquipInfo.u1SmithingLevel) + ">";
			itemTier.text = TextManager.Instance.GetText( "forge_level_" + _Item.cEquipInfo.u1SmithingLevel);
			UIManager.Instance.SetGradientFromTier(itemTier.GetComponent<Gradient>(), _Item.cEquipInfo.u1SmithingLevel);
			for(int i=0; i<starPos.transform.GetChildCount(); i++)
				starPos.transform.GetChild(i).gameObject.SetActive(false);
			for(int i=0; i<_Item.cEquipInfo.u1Completeness; i++)
			{
				starPos.transform.GetChild(i).gameObject.SetActive(true);
				UIManager.Instance.SetGradientFromTier(starPos.transform.GetChild(i).GetComponent<Gradient>(), _Item.cEquipInfo.u1SmithingLevel);
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
			// if(_Item.cEquipInfo.u1SmithingLevel > 1)
			// {
			gradeEffect = Instantiate(AssetMgr.Instance.AssetLoad(string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}.prefab", _Item.cEquipInfo.u1SmithingLevel), typeof(GameObject))) as GameObject;
			gradeEffect.transform.SetParent(popupEquipment.transform);
			gradeEffect.transform.name = "WeaponEffect";
			gradeEffect.transform.localScale = new Vector3(1f, 1f, 1f);
			gradeEffect.transform.localPosition = itemPos.parent.localPosition + Vector3.up * 50f;
			gradeEffect.transform.localPosition = new Vector3(gradeEffect.transform.localPosition.x, gradeEffect.transform.localPosition.y, 0);
			// }
			UInt32[] tempStat = new UInt32[Server.ConstDef.SkillOfEquip + (Server.ConstDef.EquipStatPointType * 2)];
			//_Item.cEquipInfo.u2ArrBaseStat.Length
			//for(int i=0; i<_Item.cEquipInfo.u2ArrBaseStat.Length; i++)
			for(int i=0; i<_Item.cEquipInfo.u4ArrBaseStat.Length; i++)
			{
				tempStat[i + Server.ConstDef.SkillOfEquip] = _Item.cEquipInfo.u4ArrBaseStat[i];//u2ArrBaseStat[i];
			}

			equipItem.statusComponent.LoadStatusEquipment(tempStat, equipItem.GetEquipmentInfo().acStatAddInfo, 0);
			equipItem.GetComponent<LevelComponent>().Set(_Item.cEquipInfo.u2Level, _Item.cEquipInfo.u8Exp);

			//스탯 정보 세팅
			for(int i=0; i<Server.ConstDef.EquipStatPointType; i++)
			{
				byte statType = equipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType;
				statName[i].text = Status.GetStatText(statType);
				//statValue[i].text = equipItem.cStatus.GetStat(statType, _Item.cEquipInfo.u2ArrBaseStat[i]).ToString();
				statValue[i].text = equipItem.statusComponent.FINAL_STATUS.GetStat(statType).ToString();
				_btnStatInfo[i].SetData( equipItem.GetEquipmentInfo().acStatAddInfo[i].u1StatType);
			}

			//스킬 정보 세팅
			for(int i=0; i<Server.ConstDef.SkillOfEquip; i++)
			{
				if(_Item.cEquipInfo.u1ArrSkillSlots[i] == 0)
				{
					skillObject[i].SetActive(false);
				}
				else
				{				
					skillObject[i].SetActive(true);

					SkillInfo skillInfo = SkillInfoMgr.Instance.GetInfoBySlot(equipItem.GetEquipmentInfo().u2ClassID, _Item.cEquipInfo.u1ArrSkillSlots[i]);
					skillIcon[i].sprite = AtlasMgr.Instance.GetSprite("Sprites/Skill/Atlas_SkillIcon_" + equipItem.GetEquipmentInfo().u2ClassID + "." + skillInfo.u2ID);
					skillName[i].text = TextManager.Instance.GetText(skillInfo.sName);
					skillElement[i].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.common_02_skill_element_" + skillInfo.u1Element);
					skillLevel[i].text = _Item.cEquipInfo.u1ArrSkillPoint[i].ToString();
					_btnSkillInfo[i].SetData(skillInfo);
				}
			}
		}
		else
		{
			goodsGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(_goodsInfo.u2ID));
			goodsName.text = Legion.Instance.GetGoodsName (_goodsInfo);
			if(_Item.u1Type == (Byte)GoodsType.CONSUME)
			{
				goodsCnt.text = _goodsInfo.u4Count.ToString();
				goodsIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + ItemInfoMgr.Instance.GetConsumableItemInfo(_goodsInfo.u2ID).u2ID);
			}

			else if(_Item.u1Type == (Byte)GoodsType.MATERIAL)
			{
				goodsCnt.text = _goodsInfo.u4Count.ToString();
				goodsIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + ItemInfoMgr.Instance.GetMaterialItemInfo(_goodsInfo.u2ID).u2IconID);
			}
		}

		showResult = true;
	}


	public void Close()
	{
		PopupManager.Instance.RemovePopup(gameObject);
		PopupManager.Instance.showLoading = false;
		Destroy(gameObject);
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
