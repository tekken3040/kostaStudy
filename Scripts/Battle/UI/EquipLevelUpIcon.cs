using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EquipLevelUpIcon : MonoBehaviour {

	public Image _Grade;
	public Image _Icon;
	public Image _Element;
	public Text _Level;
	public RectTransform LevelUp;

	public void SetItem(EquipmentItem item){
		//_Grade.sprite = 
		byte ele = item.GetEquipmentInfo ().u1Element;
		if(ele == 0) ele = 1;

		_Element.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.element_" + ele);
        
        ModelInfo modelInfo = ModelInfoMgr.Instance.GetInfo(item.GetEquipmentInfo().u2ModelID);      
        if(modelInfo != null)
        {
			_Icon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Item/" + modelInfo.sImagePath);

			_Icon.SetNativeSize();
        }

		int smithingLevel = item.u1SmithingLevel;

		if(smithingLevel < 1)
			smithingLevel = 1;       

		ForgeInfo forgeInfo = ForgeInfoMgr.Instance.GetList()[smithingLevel-1]; 
		if(forgeInfo != null)
		{
			_Grade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + forgeInfo.u2ID);
		}
		_Level.text = item.cLevel.u2Level.ToString();
	}
}
