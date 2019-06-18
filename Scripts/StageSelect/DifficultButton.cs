using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DifficultButton : MonoBehaviour {
	
	public Button button;
	public Text diffcultText;

	// 2016. 08. 05 jy
	// 난이도 원버튼 셋팅
	public void SetDifficultButton(int nDifficult)
	{
		string sDifficult;
		switch(nDifficult)
		{
		case 2:
			sDifficult = "normal";
			break;
		case 3:
			sDifficult = "hell";
			break;
		default:
			sDifficult = "easy";
			break;
		}
		button.image.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_04_renew.Btn_Campaign_difficulty_"+nDifficult.ToString());
		diffcultText.text = TextManager.Instance.GetText("btn_diffi_"+sDifficult);
	}
	
	public void SetButton(int type)
	{
		switch(type)
		{
			case 0:
//			button.image.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02.common_02_button_02");
			button.interactable = true;
            diffcultText.color = Color.white;
			break;
			
			case 1:
//			button.image.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02.common_02_button_03");
			button.interactable = true;
            diffcultText.color = Color.white;
			break;
			
			case 2:
			button.interactable = false;
//            diffcultText.color = new Color32(199, 199, 199, 255);
			break;
		}
	}
}
