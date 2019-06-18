using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuickQuestButton : MonoBehaviour {

	public Text txtAccept;
	public Text txtName;
	public Text txtProg;
	public GameObject imgMain;
	public GameObject imgQuest;
	public GameObject imgCheck;

	public void SetButton(){        
		if (Legion.Instance.cQuest.u2IngQuest > 0) { 
			imgQuest.SetActive (false);
			imgMain.SetActive (true);

			string iconName = "main";

			if(Legion.Instance.cQuest.CurrentQuest().u1MainType > 1) iconName = "sub";

			imgMain.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_quest_"+iconName);

			txtName.text = TextManager.Instance.GetText (Legion.Instance.cQuest.CurrentQuest ().sName);
			txtProg.text = TextManager.Instance.GetText (Legion.Instance.cQuest.CurrentQuest ().sSummary)+"  "+Legion.Instance.cQuest.u4QuestCount + "/" + Legion.Instance.cQuest.CurrentUserQuest().u4MaxCount;
			if (Legion.Instance.cQuest.isClearQuest()) {
				imgCheck.SetActive(true);
				float maxWidth = txtName.preferredWidth;
				if (maxWidth < txtProg.preferredWidth)
					maxWidth = txtProg.preferredWidth;
				
				txtProg.GetComponent<Outline> ().effectColor = new Color32 (0, 238, 21, 128);
			} else {
				imgCheck.SetActive(false);
			}
		} else {
			imgMain.SetActive (false);
			imgCheck.SetActive(false);
			if (Legion.Instance.cQuest.IsHaveOpenQuest ()) {
				imgQuest.SetActive (true);
				txtAccept.text = TextManager.Instance.GetText ("mark_quest_acceptable");
			} else {
				gameObject.SetActive (false);
			}
		}
	}
}
