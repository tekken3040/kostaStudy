using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

public class UI_EventDungeonBtn : MonoBehaviour {
	public Image imgBtnIcon;
	public Image imgBtnBorder;
	public Text txtDungeonName;
	public GameObject objNewIcon;

	EventDungeonShopInfo shopInfo;

	UInt16 eventId;

	public void Init(EventDungeonShopInfo info){
		shopInfo = info;
		txtDungeonName.text = TextManager.Instance.GetText (info.sMainBtnName);
		if (info.u1UIType != 1) {
			imgBtnIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Event/event_01." + shopInfo.sMainBtnImagePath);
		} else {
			imgBtnIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Event/" + shopInfo.sAdventoBtnImagePath);
		}
		imgBtnBorder.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Event/event_01.EventDungeonBtn");
	}

	public void OnClickThis(){
		LobbyScene lb = Scene.GetCurrent () as LobbyScene;
		if(lb != null){
			GameObject temp = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_UI_EventDungeonPopup.prefab", typeof(GameObject))) as GameObject;
			temp.GetComponent<UI_EventDungeonPopup> ().Set (shopInfo);

			temp.transform.SetParent (lb.transform);
			temp.transform.localScale = Vector3.one;
			temp.transform.localPosition = Vector3.zero;
			temp.GetComponent<RectTransform> ().anchoredPosition3D = Vector3.zero;
			temp.GetComponent<RectTransform> ().sizeDelta = Vector2.zero;

			PopupManager.Instance.AddPopup (temp, temp.GetComponent<UI_EventDungeonPopup> ().OnClickClose);
		}else{
			DebugMgr.LogError("Not Lobby");
		}
	}
}
