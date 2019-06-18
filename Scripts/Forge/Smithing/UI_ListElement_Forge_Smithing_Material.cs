using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class UI_ListElement_Forge_Smithing_Material : MonoBehaviour {
	[SerializeField] Image _imgMaterialGrade;
	[SerializeField] Image _imgMaterialIcon;
	[SerializeField] GameObject _objCheckMark;
	[SerializeField] Text _txtMaterialName;
	[SerializeField] Text _txtCount;
    public Button BtnDrop;
	public static Color32 lowerCount = new Color32(124, 114, 136, 255);

	public void SetDataSmithing(Goods materialItemInfo, UInt16 ownCount)
	{
		_imgMaterialIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + ItemInfoMgr.Instance.GetMaterialItemInfo(materialItemInfo.u2ID).u2IconID);

		UInt16 grade = ItemInfoMgr.Instance.GetMaterialItemInfo(materialItemInfo.u2ID).u2Grade;

		if(ownCount >= materialItemInfo.u4Count)
		{
			_txtMaterialName.color = Color.white;
			if(_txtCount != null)
				_txtCount.color = Color.white;
			_objCheckMark.SetActive(true);
		}
		else
		{
			_txtMaterialName.color = lowerCount;
			if(_txtCount != null)
				_txtCount.color = lowerCount;
			_objCheckMark.SetActive(false);
		}
		_imgMaterialGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_"+grade);
		if(_txtCount != null)
		{
			_txtMaterialName.text = TextManager.Instance.GetText(ItemInfoMgr.Instance.GetMaterialItemInfo(materialItemInfo.u2ID).sName);
			StringBuilder strBuilder = new StringBuilder();
			strBuilder.Append(ownCount);
			strBuilder.Append("/");
			strBuilder.Append(materialItemInfo.u4Count);
			_txtCount.text = strBuilder.ToString();
		}
		else
		{
			StringBuilder strBuilder = new StringBuilder();
			strBuilder.Append(TextManager.Instance.GetText( ItemInfoMgr.Instance.GetMaterialItemInfo(materialItemInfo.u2ID).sName));
			strBuilder.Append("    ");
			strBuilder.Append(ownCount);
			strBuilder.Append("/");
			strBuilder.Append(materialItemInfo.u4Count);
			_txtMaterialName.text = strBuilder.ToString();
		}
	}

	public void SetDataUpgrade(Goods materialItemInfo, UInt16 ownCount)
	{
		if(_txtCount != null)
			_txtCount.text = "";
		
		_imgMaterialIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + ItemInfoMgr.Instance.GetMaterialItemInfo(materialItemInfo.u2ID).u2IconID);

		UInt16 grade = ItemInfoMgr.Instance.GetMaterialItemInfo(materialItemInfo.u2ID).u2Grade;

		if(ownCount >= materialItemInfo.u4Count)
		{
			_txtMaterialName.color = Color.white;
			_txtCount.color = Color.white;
			_objCheckMark.SetActive(true);
		}
		else
		{
			_txtMaterialName.color = lowerCount;
			_txtCount.color = lowerCount;
			_objCheckMark.SetActive(false);
		}
		_imgMaterialGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_"+grade);

		_txtMaterialName.text = TextManager.Instance.GetText( ItemInfoMgr.Instance.GetMaterialItemInfo(materialItemInfo.u2ID).sName);

		StringBuilder strBuilder = new StringBuilder();
		strBuilder.Append(ownCount);
		strBuilder.Append("/");
		strBuilder.Append(materialItemInfo.u4Count);

		_txtCount.text = strBuilder.ToString();

	}
}
