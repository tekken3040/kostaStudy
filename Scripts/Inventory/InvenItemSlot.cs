using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class InvenItemData
{
	public int index;
	public Item item;
}

//아이템 목록에 사용된다
public class InvenItemSlot : MonoBehaviour, ISlot<InvenItemData> {

	public delegate void OnClickItem(Item item);

	public OnClickItem onClickItem;
	public Image slotBG;
	public Image gradeBG;
	public Image elementBG;
    public Image countBG;
	public Text itemLevel;
	public Text itemCount;
	public Image itemIcon;
	public GameObject alramIcon;
    public GameObject equipIcon;
    public GameObject effectIcon;
	public InvenItemData data;
    public Text _txtStarCnt;
	public Image _imgStarGrade;
    EquipmentItem equipmentItem;

	public void InitSlot(InvenItemData itemData)
	{
		data = itemData;
		
		Item item = data.item;
		
        // 장비인자 아닌지에 따라 표시 방법이 다르다
		if(item.cItemInfo.ItemType == ItemInfo.ITEM_TYPE.EQUIPMENT)
		{
            _txtStarCnt.transform.parent.gameObject.SetActive(true);
            
//			slotBG.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
			slotBG.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_bg_right");
			slotBG.rectTransform.anchoredPosition = new Vector2(11.5f, -6f);
			elementBG.gameObject.SetActive(true);
			itemLevel.gameObject.SetActive(true);
			itemLevel.gameObject.SetActive(true);		
			itemCount.gameObject.SetActive(false);
            countBG.gameObject.SetActive(false);	
			
            equipmentItem = ((EquipmentItem)item);
            _txtStarCnt.text = equipmentItem.u1Completeness.ToString();
            // 남은 스탯포인트가 있는경우 느낌표 처리
            if(/*equipmentItem.GetComponent<StatusComponent>().STAT_POINT > 0 ||*/ equipmentItem.isNew)
                alramIcon.SetActive(true);
            else    
                alramIcon.SetActive(false);
            
			itemLevel.text = equipmentItem.cLevel.u2Level.ToString();
			                   
            ModelInfo modelInfo = ModelInfoMgr.Instance.GetInfo(equipmentItem.u2ModelID);      
            
            if(modelInfo != null)
            {    
                string imagePath = "Sprites/Item/" + modelInfo.sImagePath;
                itemIcon.sprite = AtlasMgr.Instance.GetSprite(imagePath);
                itemIcon.SetNativeSize();
            }
            
			elementBG.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + equipmentItem.GetEquipmentInfo().u1Element);
                          
            int smithingLevel = equipmentItem.u1SmithingLevel;
            
            if(smithingLevel < 1)
                smithingLevel = 1;       
                
            ForgeInfo forgeInfo = ForgeInfoMgr.Instance.GetList()[smithingLevel-1];            
			_imgStarGrade.sprite = gradeBG.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + forgeInfo.u2ID);
            gradeBG.SetNativeSize();
            
            if(equipmentItem.attached.hero != null)
                equipIcon.SetActive(true);
            else
                equipIcon.SetActive(false);
		}
		else
		{
			equipmentItem = null;
            _txtStarCnt.transform.parent.gameObject.SetActive(false);
//			slotBG.rectTransform.localScale = Vector3.one;
			slotBG.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_bg_left");
			slotBG.rectTransform.anchoredPosition = new Vector2(0f, -6f);
			elementBG.gameObject.SetActive(false);
			itemLevel.gameObject.SetActive(false);
			itemCount.gameObject.SetActive(true);
			countBG.gameObject.SetActive(true);
			alramIcon.SetActive(item.isNew);
            equipIcon.SetActive(false);
            effectIcon.SetActive(false);

			ushort iconID = item.cItemInfo.u2ID;

			switch(item.cItemInfo.ItemType)
			{
				case ItemInfo.ITEM_TYPE.CONSUMABLE:
					itemCount.text = ((ConsumableItem)item).u2Count.ToString();
					break;
				case ItemInfo.ITEM_TYPE.MATERIAL:
					iconID = ((MaterialItemInfo)item.cItemInfo).u2IconID;
					itemCount.text = ((MaterialItem)item).u2Count.ToString();
					break;
			}
			
			itemIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + iconID);
			gradeBG.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(item.cItemInfo.u2ID));
            gradeBG.SetNativeSize();
            itemIcon.SetNativeSize();
		}
	}
	
	public void OnClickEvent()
	{
        if(equipmentItem != null)
        {
            Legion.Instance.cInventory.dicInventory[equipmentItem.u2SlotNum].isNew = false;
            alramIcon.SetActive(false);
            //if(equipmentItem.GetComponent<StatusComponent>().STAT_POINT > 0)
            //    alramIcon.SetActive(true);
            //else    
            //    alramIcon.SetActive(false);
        }
        else
        {
            Legion.Instance.cInventory.dicInventory[data.item.u2SlotNum].isNew = false;
            alramIcon.SetActive(false);
        }
		if(onClickItem != null)
			onClickItem(data.item);
	}
}
