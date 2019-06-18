using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class QuestCompleteMsg : MonoBehaviour {
	public GameObject popup;
	public GameObject eff;
	RectTransform popTrans;
	public Image imgIcon;
	public Text txtTitle;
	public Text txtDesc;

	List<KeyValuePair<int, ushort>> clearList = new List<KeyValuePair<int, ushort>>();

	float fTick = 3.0f;
	float fTime = 0.0f;

	void Awake(){
		popTrans = popup.GetComponent<RectTransform>();
	}

	void OnEnable(){
		fTime = fTick;
	}

	public void AddList (int type, ushort id) {
		if (type != 1) {
			if (clearList.Count > 4)
				return;
            if (clearList.FindIndex (cs => cs.Key == 1) > -1)
				return;

			AchievementInfo info = QuestInfoMgr.Instance.GetAchieveInfo(id);
			if (EventInfoMgr.Instance.GetOpenMarbleGameInfo (info.u2EventID) == null)
				return;
		}

		if (type == 1 && clearList.FindIndex (cs => cs.Key == 1) > -1) {
			return;
		}

		gameObject.SetActive(true);
		clearList.Add(new KeyValuePair<int, ushort>(type, id));
	}

	void Update(){
		fTime += Time.deltaTime;
		if (fTime > fTick) {
			fTime = 0.0f;
			if(clearList.Count <= 0) gameObject.SetActive(false);

			if (clearList.Count > 0) {
				if(clearList[0].Key == 1){
					QuestInfo info = QuestInfoMgr.Instance.GetQuestInfo(clearList[0].Value);
					imgIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/" + info.sIconImage);
					if (Legion.Instance.cQuest.isClearQuest ()) {
						txtTitle.text = TextManager.Instance.GetText ("mark_quest_complete");
						txtDesc.text = TextManager.Instance.GetText (info.sName);
						Invoke ("ShowEffect", 0.3f);
					} else {
						txtTitle.text = TextManager.Instance.GetText (info.sName);
						txtDesc.text = TextManager.Instance.GetText (info.sSummary)+"("+Legion.Instance.cQuest.u4QuestCount+"/"+Legion.Instance.cQuest.CurrentUserQuest().u4MaxCount+")";
					}
                    popup.GetComponent<Image>().color = Color.white;
                    fTick = 4f;
				}else{
					AchievementInfo info = QuestInfoMgr.Instance.GetAchieveInfo(clearList[0].Value);
					txtTitle.text = TextManager.Instance.GetText("mark_achieve_complete");
					string category = "";
					switch(info.u1PeriodType){
					case 1: category = TextManager.Instance.GetText("mark_achieve"); break;
					case 2: category = TextManager.Instance.GetText("mark_achieve_daily"); break;
					case 3: category = TextManager.Instance.GetText("mark_achieve_weekly"); break;
					}

					if (info.u2EventID > 0) {
						popup.GetComponent<Image> ().color = Color.red;
					} else {
						popup.GetComponent<Image> ().color = Color.white;
					}

					txtDesc.text = category+TextManager.Instance.GetText(info.sName);
					imgIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/"+info.sIconImage);
					eff.SetActive (false);
					fTick = 3.0f;
				}
				imgIcon.SetNativeSize();
				LeanTween.move(popTrans, Vector2.up*-60f,0.3f);
				LeanTween.move(popTrans, Vector2.up*60f,0.7f).setDelay(fTick-1f);
				clearList.RemoveAt(0);
			}
		}
	}

	void ShowEffect(){
		eff.SetActive (true);
		Invoke("HideEffect", 1.5f);
	}

	void HideEffect(){
		eff.SetActive (false);
	}
}
