using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DownloadPopup : YesNoPopup {
	public GameObject popupObject;
	public GameObject progressObject;
	public RectTransform totalProgressBar;
	public Text currentSizeText;
	public Text currentPerText;

	public GameObject okBtn;
	public GameObject noBtn;
	public GameObject compBtn;

	UnityEngine.Events.UnityAction lastEvent;

	// Use this for initialization
	public void UpdateInfo(float size, string per, string prog) {
		totalProgressBar.sizeDelta = new Vector2 (size, totalProgressBar.sizeDelta.y);
		currentSizeText.text = prog;
		currentPerText.text = per;
	}

	public void SetCompleteEvent(UnityEngine.Events.UnityAction evt){
		compBtn.GetComponent<Button> ().onClick.AddListener (evt);
		lastEvent = evt;
	}

	public void OnClickComp()
	{
		if (lastEvent != null) {
			compBtn.GetComponent<Button> ().onClick.RemoveListener (lastEvent);
			lastEvent = null;
		}
		gameObject.SetActive (false);
	}
}
