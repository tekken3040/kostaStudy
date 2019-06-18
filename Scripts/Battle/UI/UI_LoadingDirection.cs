using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class UI_LoadingDirection : MonoBehaviour {
	public GameObject Object1;
	public Text Name1;
	public Text Comment1;
	public Image Illust1;
	public GameObject Object2;
	public Text Name2;
	public Text Comment2;
	public Image Illust2;

	int[] randomIdx;
	int idx = 0;

	int length = 0;

	List<KeyValuePair<UInt16, TalkCharacter>> LoadingChars;

	void Start(){
		LoadingChars = TutorialInfoMgr.Instance.GetLoadingChars(0);
		length = LoadingChars.Count;
		randomIdx = new int[length];

		for (int i=0; i<length; i++) randomIdx[i] = i;
		
		
		for (int i = length - 1; i > 0; i--) {
			int r = UnityEngine.Random.Range(0,i+1);
			int tmp = randomIdx[i];
			randomIdx[i] = randomIdx[r];
			randomIdx[r] = tmp;
		}

		if (UnityEngine.Random.Range (0, 2) == 0) {
			ViewLeft ();
		} else {
			ViewRight();
		}
	}

	void ViewLeft(){
		TalkCharacter tempChar = LoadingChars[randomIdx[idx]].Value;
		idx++;
		if(idx > length -1) idx = 0;
		Object1.SetActive(true);
		Object2.SetActive(false);

		Name1.text = TextManager.Instance.GetText(tempChar.sName);
		Comment1.text = TextManager.Instance.GetText(tempChar.sDescription);
		Illust1.sprite = AtlasMgr.Instance.GetSprite("Sprites/Tutorial/ch_"+tempChar.u2ImageID.ToString("000")+".ch_"+tempChar.u2ImageID.ToString("000"));
	}

	void ViewRight(){
		TalkCharacter tempChar = LoadingChars[randomIdx[idx]].Value;
		idx++;
		if(idx > length -1) idx = 0;
		Object2.SetActive(true);
		Object1.SetActive(false);

		Name2.text = TextManager.Instance.GetText(tempChar.sName);
		Comment2.text = TextManager.Instance.GetText(tempChar.sDescription);
		Illust2.sprite = AtlasMgr.Instance.GetSprite("Sprites/Tutorial/ch_"+tempChar.u2ImageID.ToString("000")+".ch_"+tempChar.u2ImageID.ToString("000"));
	}
}
