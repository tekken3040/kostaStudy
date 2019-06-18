using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class RevengePopup : MonoBehaviour
{
    [SerializeField] Button btnClose;
    [SerializeField] Button btnRevenge;

    [SerializeField] Text txtCrewName;
    [SerializeField] Text txtRankPoint;
    [SerializeField] Text txtTalkMsg;

    [SerializeField] GameObject TalkBlank;

    [SerializeField] Image imgClass;

    [SerializeField] LeagueScene _leagueScene;
    StringBuilder tempStringBuilder;
    LeagueMatchList.ListSlotData _slotData;

    public void SetData(LeagueMatchList.ListSlotData _slot)
    {
        tempStringBuilder = new StringBuilder();
        _slotData = _slot;

        txtCrewName.text = _slotData.strLegionName;
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(_slotData.u4Point).Append(" pt");
        txtRankPoint.text = tempStringBuilder.ToString();
        if(_slotData.strRevengeMessage == "")
            txtTalkMsg.text = TextManager.Instance.GetText("massage_box_revenge");
        else
            txtTalkMsg.text = _slotData.strRevengeMessage;

        imgClass.sprite = AssetMgr.Instance.AssetLoad("Sprites/Tutorial/class_"+_slotData.u2ClassID.ToString("000")+".png", typeof(Sprite)) as Sprite;

        UI_League.Instance.RevengeCrew = new LeagueMatchList.ListSlotData();

		btnClose.gameObject.SetActive (false);
		btnRevenge.gameObject.SetActive (false);

		Direction ();
    }

	void Direction()
	{
		imgClass.rectTransform.anchoredPosition = new Vector2 (-370, 0);
		imgClass.color = new Color (1, 1, 1, 0);
		txtCrewName.enabled = false;
		txtRankPoint.enabled = false;
		TalkBlank.GetComponent<Image>().color = new Color (1, 1, 1, 0);
		txtTalkMsg.color = new Color (0, 0, 0, 0);

		StartCoroutine (StartDirection ());
	}

	IEnumerator StartDirection()
	{
		LeanTween.moveLocal(imgClass.gameObject, new Vector3(-270f,0,0), 0.2f);
		LeanTween.alpha(imgClass.rectTransform, 1.0f, 0.1f);

		yield return new WaitForSeconds (0.3f);
		txtCrewName.enabled = true;
		txtRankPoint.enabled = true;
		LeanTween.value (txtTalkMsg.gameObject, 0f, 1f, 0.2f).setDelay(0.4f).setOnUpdate((float alpha)=>{txtTalkMsg.color = new Color(0,0,0,alpha);});
		LeanTween.alpha(TalkBlank.GetComponent<RectTransform>(), 1.0f, 0.2f).setDelay(0.2f);

		yield return new WaitForSeconds (0.7f);

		btnClose.gameObject.SetActive (true);
		btnRevenge.gameObject.SetActive (true);
	}

    public void OnClickMatch()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestLeagueMatch(_slotData.u8UserSN, _slotData.u1Revenge, ReceiveMatchPlayer);
    }

    public void ReceiveMatchPlayer(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(((int)err).ToString()), Server.ServerMgr.Instance.CallClear);
			return;
        }
        else if(err == Server.ERROR_ID.NONE)
        {
            UI_League.Instance.CreateEnemyCrew();
            //StartLeague();
            _leagueScene.ShowBattleScreen();
            gameObject.SetActive(false);
        }
    }

    public void OnClickClose()
    {
        gameObject.SetActive(false);
    }
}
