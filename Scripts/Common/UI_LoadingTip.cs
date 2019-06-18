using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class UI_LoadingTip : MonoBehaviour {
	public Image ClassImg;
	public Text TipText;

	bool bInit = false;

	List<KeyValuePair<UInt16, TalkCharacter>> LoadingChars;

	void SetData(){
		LoadingChars = TutorialInfoMgr.Instance.GetLoadingChars(1);
		bInit = true;
	}

	public void SetTip(){
		if (!bInit)
			SetData ();
        if(TextManager.Instance.eLanguage == TextManager.LANGUAGE_TYPE.KOREAN)
        {
            int rand = UnityEngine.Random.Range (1, 10);
            TipText.transform.parent.gameObject.SetActive(false);
			ClassImg.sprite = Resources.Load("Sprites/TutorialBG/loadding_tip_"+rand, typeof(Sprite)) as Sprite;
		    ClassImg.SetNativeSize ();
        }
        else
        {
            int rand = UnityEngine.Random.Range (0, LoadingChars.Count);
            TipText.transform.parent.gameObject.SetActive(true);
		    ClassImg.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_class.common_class_"+LoadingChars[rand].Value.u2ImageID);
		    ClassImg.SetNativeSize ();
		    TipText.text = TextManager.Instance.GetText(LoadingChars[rand].Value.sDescription);
        }
	}
}
