using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;

public class TrainingSlot : MonoBehaviour{

    public delegate void OnClickSlot(int selectedIndex, int itemIndex, int slotIndex);
    public OnClickSlot onClickSlot; 
    
    public Image portrait;
    public Image grade;
    public Image element;
    public Text level;
    
    public TrainingWindow trainingWindow;
    public ScrollRect scrollRect;
    public static GameObject dragObject;

    public GameObject _starObj;
    public Text _txtStarCnt;
	public Image _imgStarGrade;
      
    private int itemIndex;
    private int slotIndex;
    //private bool pressed = false;
    private float time;    
    private Vector2 canvasSize;
    
    //슬롯 정보 세팅
    public void SetSlot(int itemIndex, int slotIndex, bool isEquip)
    {    
        this.slotIndex = slotIndex;
        this.itemIndex = itemIndex;
        
        if(!isEquip)
        {
            Hero hero = Legion.Instance.acHeros[itemIndex];
            
            portrait.sprite = AtlasMgr.Instance.GetSprite("Sprites/Hero/hero_icon." + hero.cClass.u2ID);
			element.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + EquipmentInfoMgr.Instance.GetInfo(hero.acEquips[6].cItemInfo.u2ID).u1Element);
            level.text = hero.cLevel.u2Level.ToString();
            _starObj.SetActive(false);             
        }
        else
        {
            EquipmentItem equipmentItem = Legion.Instance.cInventory.lstSortedEquipment[itemIndex];
            	
            ModelInfo modelInfo = ModelInfoMgr.Instance.GetInfo(equipmentItem.u2ModelID);
                
            _starObj.SetActive(true);
            _txtStarCnt.text = equipmentItem.u1Completeness.ToString();
            if(modelInfo != null)
            {
                string imagePath = "Sprites/Item/" + modelInfo.sImagePath;                            
                portrait.sprite = AtlasMgr.Instance.GetSprite(imagePath);
                portrait.transform.localScale = Vector3.one;
                portrait.SetNativeSize();
            }            

			int smithingLevel = equipmentItem.u1SmithingLevel;

			if(smithingLevel < 1)
				smithingLevel = 1;       

			ForgeInfo forgeInfo = ForgeInfoMgr.Instance.GetList()[smithingLevel-1];            
			_imgStarGrade.sprite = grade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + forgeInfo.u2ID);

			element.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + equipmentItem.GetEquipmentInfo().u1Element);
            level.text = equipmentItem.cLevel.u2Level.ToString();
        }
        
        portrait.SetNativeSize();
        canvasSize = transform.root.GetComponent<RectTransform>().sizeDelta;
    }

	public void SetEquipSlot(EquipmentItem cEquipItem)
	{
		ModelInfo modelInfo = ModelInfoMgr.Instance.GetInfo(cEquipItem.u2ModelID);

		_starObj.SetActive(true);
		_txtStarCnt.text = cEquipItem.u1Completeness.ToString();
		if(modelInfo != null)
		{
			string imagePath = "Sprites/Item/" + modelInfo.sImagePath;                            
			portrait.sprite = AtlasMgr.Instance.GetSprite(imagePath);
			portrait.transform.localScale = Vector3.one;
			portrait.SetNativeSize();
		}            

		int smithingLevel = cEquipItem.u1SmithingLevel;
		if(smithingLevel < 1)
			smithingLevel = 1;       

		ForgeInfo forgeInfo = ForgeInfoMgr.Instance.GetList()[smithingLevel-1];            
		_imgStarGrade.sprite = grade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + forgeInfo.u2ID);

		element.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + cEquipItem.GetEquipmentInfo().u1Element);
		level.text = cEquipItem.cLevel.u2Level.ToString();
	}
    
    public void SetIndex(int itemIndex, int slotIndex)
    {
        this.itemIndex = itemIndex;
        this.slotIndex = slotIndex;
    }

    public int GetItemIndex()
    {
        return itemIndex;
    }
    
    public int GetSlotIndex()
    {
        return slotIndex;
    }
    
    public void OnClick()
    {
        if(onClickSlot != null)
			onClickSlot(slotIndex, itemIndex, slotIndex);
    }
}
