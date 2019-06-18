using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UI_ListElement_Forge_Fusion_Material : MonoBehaviour {
	[SerializeField] Image _imgGrade;
	[SerializeField] Image _imgElement;
	[SerializeField] Image _imgItemIcon;
	[SerializeField] Text _txtLevel;
	[SerializeField] GameObject _objSelectIcon;
	[SerializeField] GameObject _objAttached;
	[SerializeField] Image _imgArrow;
    [SerializeField] GameObject _noticeIcon;
    [SerializeField] GameObject _starObj;
    [SerializeField] Text _starCnt;
	[SerializeField] Image _imgStarGrade;

	EquipmentItem _cEquipItem;
	EquipmentItem _cBaseEquipItem;

	public void SetData (EquipmentItem equipItem, EquipmentItem baseEquipItem, bool bLeague = false)
	{
		_cEquipItem = equipItem;
		_cBaseEquipItem = baseEquipItem;

		UInt16 gradeID = ForgeInfoMgr.Instance.GetList()[Mathf.Clamp(equipItem.u1SmithingLevel,1,Server.ConstDef.MaxForgeLevel)-1].u2ID;
		_imgStarGrade.sprite = _imgGrade.sprite = AtlasMgr.Instance.GetSprite( "Sprites/Common/common_02_renew.grade_" + gradeID );
		_imgElement.sprite = AtlasMgr.Instance.GetSprite( "Sprites/Common/common_02_renew.element_" + equipItem.GetEquipmentInfo().u1Element);
        _starCnt.text = equipItem.u1Completeness.ToString();
//		DebugMgr.Log("ModelID : " + equipItem.u2ModelID + " ModelInfo ID :" + equipItem.GetEquipmentInfo().u2ModelID);
//		DebugMgr.Log("ListElement PosID : " + equipItem.GetEquipmentInfo().u1PosID + " InvenSlot : " + equipItem.u2SlotNum); 
        int statPoint = equipItem.GetComponent<StatusComponent>().STAT_POINT;
//		DebugMgr.Log("StatPoint : " + statPoint);

        if(_noticeIcon != null)
        {
            if(_cEquipItem.isNew)
                _noticeIcon.SetActive(true);
            else
                _noticeIcon.SetActive(false);
        }
        if(!bLeague)
            Legion.Instance.cInventory.dicInventory[_cEquipItem.u2SlotNum].isNew = false;
        UInt16 modelID = equipItem.u2ModelID;
        if(modelID == 0) modelID = equipItem.GetEquipmentInfo().u2ModelID;
        
        ModelInfo modelInfo = ModelInfoMgr.Instance.GetInfo(modelID);
        
        if(modelInfo != null)
            _imgItemIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/" + modelInfo.sImagePath);

		_txtLevel.text = equipItem.cLevel.u2Level.ToString();

		if(_objAttached != null)
		{
			if(equipItem.attached.hero != null)
				_objAttached.SetActive(true);
			else
				_objAttached.SetActive(false);
		}
		if(baseEquipItem != null)
			SetDiff();
		DeSelect();
	}

	void SetDiff()
	{
		if(_cEquipItem.u2Power > _cBaseEquipItem.u2Power)
		{
			_imgArrow.sprite =  AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.arrow_up");
			_imgArrow.SetNativeSize();
			_imgArrow.gameObject.SetActive(true);
		}
		else if(_cEquipItem.u2Power < _cBaseEquipItem.u2Power)
		{
			_imgArrow.sprite =  AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.arrow_down");
			_imgArrow.SetNativeSize();
			_imgArrow.gameObject.SetActive(true);
		}
		else
		{
			_imgArrow.gameObject.SetActive(false);
		}
	}

	bool bCheck;
	public void Select()
	{
		bCheck = true;
		_objSelectIcon.SetActive(true);
	}

	public void DeSelect()
	{
		bCheck = false;
		_objSelectIcon.SetActive(false);
	}

	public void Toggle()
	{
		bCheck = !bCheck;
		_objSelectIcon.SetActive(bCheck);
	}

    public void OnClickSelect()
    {
        if(_noticeIcon != null)
            _noticeIcon.SetActive(false);

        Legion.Instance.cInventory.dicInventory[_cEquipItem.u2SlotNum].isNew = false;
    }
}
