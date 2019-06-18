using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_ItemEquipSlot : MonoBehaviour
{
	public Image _ItemIcon;
	public Image _Grade;
	public Image _Element;
	public Text _EquipLevel;
	public Text _EquipCompleteness;
	public Image _imgStarGrade;

	public void SetData(AchieveItem itemInfo)
	{
		EquipmentInfo equipInfo = EquipmentInfoMgr.Instance.GetInfo(itemInfo.cAchieveReward.u2ID);
		ForgeInfo forgeInfo = ForgeInfoMgr.Instance.GetList()[Mathf.Clamp(itemInfo.u1SmithingLevel,1,Server.ConstDef.MaxForgeLevel)-1];
		_ItemIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Item/" + equipInfo.cModel.sImagePath);
		if(forgeInfo != null)
			_imgStarGrade.sprite = _Grade.sprite =  AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + forgeInfo.u2ID);
		else
			AtlasMgr.Instance.GetGoodsGrade(itemInfo.cAchieveReward);
		_Element.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + equipInfo.u1Element);
		_EquipLevel.text = itemInfo.u2Level.ToString();
		_EquipCompleteness.text = itemInfo.u1Completeness.ToString();
	}
}
