using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class RecomEventItemSlot : EventItemSlot 
{
	public Image _imgItemIcon2;
	private bool _bFirstMaterialType = false;
	public bool IsFirstMaterialType { get {return _bFirstMaterialType; } }

	public override void SetEventItem(int index, Goods info)
	{
        if (_objPlusIcon != null)
        {
            if (index == 0)
                _objPlusIcon.SetActive(false);
            else
                _objPlusIcon.SetActive(true);
        }

		_imgItemIcon.sprite = GetItemICon(info);
		_imgItemIcon.SetNativeSize();
		_imgItemIcon.transform.localPosition = Vector3.zero;
		switch((GoodsType)info.u1Type)
		{
		case GoodsType.GOLD: case GoodsType.CASH: case GoodsType.KEY:
			_imgItemIcon.GetComponent<RectTransform>().sizeDelta *= 0.5f;
			break;
		}

		_txtItemInfo.color = GetTextColor(info.u1Type);
		string itmeName = "";
		if (info.u1Type == (byte)GoodsType.CONSUME) 
			itmeName = TextManager.Instance.GetText (ItemInfoMgr.Instance.GetConsumableItemInfo (info.u2ID).sName);
		else if(info.u1Type == (byte)GoodsType.MATERIAL)
			_bFirstMaterialType = true;
		else 
			itmeName = Legion.Instance.GetConsumeString(info.u1Type);

		if(_bFirstMaterialType == false)
			_txtItemInfo.text = itmeName + " "+ info.u4Count;
	}

	public void SetSecondEventItem(Goods info)
	{
		_imgItemIcon2.gameObject.SetActive(true);
		_imgItemIcon2.sprite = GetItemICon(info); //AtlasMgr.Instance.GetGoodsIcon(info);
		_imgItemIcon2.SetNativeSize();
		switch((GoodsType)info.u1Type)
		{
		case GoodsType.GOLD: case GoodsType.CASH: case GoodsType.KEY:
			_imgItemIcon2.GetComponent<RectTransform>().sizeDelta *= 0.5f;
			break;
		}

		// 이미지 위치 수정
		_imgItemIcon.transform.localPosition = new Vector3(20, 10, 0);
		_imgItemIcon2.transform.localPosition = new Vector3(-10, -10, 0);

		// 2016. 10. 21 jy 
		// 첫번째 아이템이 재료 아이템이면 두번째에서는 아이템이름을 넣지 않는다
		if(_bFirstMaterialType == true && (GoodsType)info.u1Type == GoodsType.MATERIAL)
			return;

		_txtItemInfo.color = GetTextColor(info.u1Type);
		string itmeName;
		if (info.u1Type == (byte)GoodsType.CONSUME) 
			itmeName = TextManager.Instance.GetText (ItemInfoMgr.Instance.GetConsumableItemInfo (info.u2ID).sName);
		else if(info.u1Type == (byte)GoodsType.MATERIAL)
			return;
		else 
			itmeName = Legion.Instance.GetConsumeString(info.u1Type);
		_txtItemInfo.text += "\n"+ itmeName +" "+ info.u4Count;
	}
}
