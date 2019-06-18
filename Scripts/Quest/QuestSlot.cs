using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class QuestSlot : MonoBehaviour {
	public Image _imgIcon;
	public Image _imgIconBG;
	
	public Button _btnSlot;
    public Button _btnAccepBtn;
	
	public Text _txtName;
	public Text _txtType;
	public Text _txtState;
    public Text _txtAccepBtnText;

	public GameObject _objState;

	public GameObject _objIng;
	public GameObject _objComplete;
	
	public UserQuest cInfo;
	
	public void SetSlot(UserQuest info, bool bNew){
		_objComplete.SetActive(false);
		_objIng.SetActive(false);

		cInfo = info;

		QuestInfo tInfo = info.GetInfo();

		string iconName = "main";

		_imgIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_quest_"+iconName);
		_imgIcon.SetNativeSize();

		if (info.GetInfo ().u1MainType == 1) {
			_txtType.text = string.Format ("<color=#ff6000>{0} </color>", TextManager.Instance.GetText ("mark_direction_quest_main"));
		} else if (info.GetInfo ().u1MainType == 2) {
			_txtType.text = string.Format ("<color=#f4a611>{0} </color>", TextManager.Instance.GetText ("mark_direction_quest_sub"));
			iconName = "sub";
		}

		_txtName.text = TextManager.Instance.GetText(tInfo.sName);
		if (bNew) {
			_objState.SetActive (true);
			_txtState.text = TextManager.Instance.GetText("mark_quest_new");
		} else {
            RefreshSlot();
        }

		_btnSlot.gameObject.AddComponent<TutorialButton>().id = "Quest"+cInfo.u2ID;
	}

	public void SetRewarded(){
		_txtState.text = TextManager.Instance.GetText("mark_icon_quest_done");
	}

	public void SetAccept(){
		_objIng.SetActive(true);
		_objState.SetActive (true);
		_txtState.text = TextManager.Instance.GetText("mark_quest_ing");

        // 빠른 수락 및 바로가기 버튼 활성화
        _btnAccepBtn.gameObject.SetActive(true);
        _txtAccepBtnText.text = TextManager.Instance.GetText("popup_btn_quest_shortcut");// "바로가기";
    }

	public void SetGiveUp(){
		_objIng.SetActive(false);
		_objState.SetActive (false);
		_txtState.text = "";

        _btnAccepBtn.gameObject.SetActive(true);
        _txtAccepBtnText.text = TextManager.Instance.GetText("popup_btn_quest_ok");// "바로가기";
    }
	
	public void DestroyMe(){
		Destroy (gameObject);
	}

    public void RefreshSlot()
    {
        if (Legion.Instance.cQuest.u2IngQuest == cInfo.u2ID)
        {
            if (cInfo.isClear())
            {
                _objComplete.SetActive(true);
                _objState.SetActive(true);
                _txtState.text = TextManager.Instance.GetText("mark_quest_complete2");

                // 빠른 수락 및 바로가기 버튼 숨김
                _btnAccepBtn.gameObject.SetActive(false);
            }
            else
            {
                SetAccept();
            }
        }
        else if (Legion.Instance.cQuest.u2IngQuest == 0)
        {
            _btnAccepBtn.gameObject.SetActive(true);
            _txtAccepBtnText.text = TextManager.Instance.GetText("popup_btn_quest_ok");// "바로가기";
        }
        else
        {
            _btnAccepBtn.gameObject.SetActive(false);
        }
    }

}
