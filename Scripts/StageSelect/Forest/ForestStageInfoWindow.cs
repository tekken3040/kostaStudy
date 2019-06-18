using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ForestStageInfoWindow : StageInfoWindow {
	[SerializeField] Button _btnChargeTicket;
	[SerializeField] Image _imgChargeTicketButton;
	[SerializeField] Text _txtTicketCountLabel;
	[SerializeField] Text _txtTicketCount;

	[SerializeField] Button _btnSweep;
	[SerializeField] Text _txtSweep;

	[SerializeField] Button _btnStart;
	[SerializeField] Text _txtStart;

	Byte _u1Ticket=0;
	Byte _u1ChargedTicketCount=0;

	UInt16 _u2StageID = 0;
	public override void SetInfo(UInt16 stageID)
	{
		base.SetInfo(stageID);
        StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[stageID];
        UIManager.Instance.SetGradientFromElement(stageTitle.GetComponent<Gradient>(), stageInfo.u1ForestElement);
		_u2StageID = stageID;
		UpdateTicket();
	}

	public void UpdateTicket()
	{
		_u1Ticket = StageInfoMgr.Instance.GetForestTicket();//(_u2StageID);
		_u1ChargedTicketCount = StageInfoMgr.Instance.GetForestChargedTicketCount();//(_u2StageID);
		_txtTicketCount.text = _u1Ticket.ToString();

		if(_u1Ticket > 0)
		{
			DeactiveChargeTicket();
			ActiveSweap();
			ActiveStart();
		}
		else
		{
			ActiveChargeTicket();
			DeactiveSweap();
			DeactiveStart();
		}
	}

	void DeactiveChargeTicket()
	{
		_imgChargeTicketButton.sprite = AtlasMgr.Instance.GetSprite("Sprites/Campaign/forest_001.Forest_Repeat_Button_Off");
		_txtTicketCountLabel.GetComponent<Gradient>().StartColor = new Color32(190, 190, 190, 255);
		_txtTicketCountLabel.GetComponent<Gradient>().EndColor = new Color32(102, 102, 102, 255);
		_txtTicketCountLabel.GetComponent<Gradient>().ReDraw();
		_txtTicketCount.GetComponent<Gradient>().StartColor = new Color32(190, 190, 190, 255);
		_txtTicketCount.GetComponent<Gradient>().EndColor = new Color32(102, 102, 102, 255);
		_txtTicketCount.GetComponent<Gradient>().ReDraw();
		_btnChargeTicket.interactable = false;

	}

	void ActiveChargeTicket()
	{
		_imgChargeTicketButton.sprite = AtlasMgr.Instance.GetSprite("Sprites/Campaign/forest_001.Forest_Repeat_Button_On");
		_txtTicketCountLabel.GetComponent<Gradient>().StartColor = new Color32(255, 255, 255, 255);
		_txtTicketCountLabel.GetComponent<Gradient>().EndColor = new Color32(137, 163, 180, 255);
		_txtTicketCountLabel.GetComponent<Gradient>().ReDraw();
		_txtTicketCount.GetComponent<Gradient>().StartColor = new Color32(255, 255, 255, 255);
		_txtTicketCount.GetComponent<Gradient>().EndColor = new Color32(137, 163, 180, 255);
		_txtTicketCount.GetComponent<Gradient>().ReDraw();
		_btnChargeTicket.interactable = true;
	}

	void DeactiveSweap()
	{
		_btnSweep.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_01_renew.btn_blue_w126_off");
		_txtSweep.GetComponent<Gradient>().StartColor = new Color32(190, 190, 190, 255);
		_txtSweep.GetComponent<Gradient>().EndColor = new Color32(102, 102, 102, 255);
		_txtSweep.GetComponent<Gradient>().ReDraw();
		_btnSweep.interactable = false;
	}

	void ActiveSweap()
	{
		_btnSweep.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_01_renew.btn_blue_w126");
		_txtSweep.GetComponent<Gradient>().StartColor = new Color32(255, 255, 255, 255);
		_txtSweep.GetComponent<Gradient>().EndColor = new Color32(160, 160, 160, 255);
		_txtSweep.GetComponent<Gradient>().ReDraw();
		_btnSweep.interactable = true;
	}

	void DeactiveStart()
	{
		_btnStart.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_01_renew.btn_blue_w126_off");
		_txtStart.GetComponent<Gradient>().StartColor = new Color32(190, 190, 190, 255);
		_txtStart.GetComponent<Gradient>().EndColor = new Color32(102, 102, 102, 255);
		_txtStart.GetComponent<Gradient>().ReDraw();
		_btnStart.interactable = false;
	}

	void ActiveStart()
	{
		_btnStart.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_01_renew.btn_blue_w126");
		_txtStart.GetComponent<Gradient>().StartColor = new Color32(255, 255, 255, 255);
		_txtStart.GetComponent<Gradient>().EndColor = new Color32(137, 163, 180, 255);
		_txtStart.GetComponent<Gradient>().ReDraw();
		_btnStart.interactable = true;
	}

	public void OnClick_ChargeTicket()
	{
		if(StageInfoMgr.Instance.forest.u1ChargeCountMax <= _u1ChargedTicketCount)
		{
			// 충전회수 최대치 초과(팝업)
			return;
		}

		UInt32 needCost = 0;
		UInt32 basicCost = StageInfoMgr.Instance.forest.cChargeGoods.u4Count;
		UInt32 addCost = (UInt32)Mathf.Min(StageInfoMgr.Instance.forest.u4ChargeGoodsAddCost * _u1ChargedTicketCount, StageInfoMgr.Instance.forest.u4ChargeGoodsAddCostMax);
		needCost = basicCost + addCost;
		Goods needGoods = new Goods(
			StageInfoMgr.Instance.forest.cChargeGoods.u1Type, 
			StageInfoMgr.Instance.forest.cChargeGoods.u2ID,
			needCost);
			//StageInfoMgr.Instance.forest.cChargeGoods.u4Count);
		//needGoods.u4Count = ;

		if(!Legion.Instance.CheckEnoughGoods(needGoods))
		{
			// 충전 재화부족 재화 충전?팝업
			PopupManager.Instance.ShowChargePopup(needGoods.u1Type);
			return;
		}

		// 충전하시겠습니까?(팝업)
		PopupManager.Instance.ShowYesNoPopup(TextManager.Instance.GetText("popup_title_enter_reset"), string.Format(TextManager.Instance.GetText("popup_desc_enter_reset"), needCost), OnClick_ChargeTicketOK, null);
	}

	public void OnClick_ChargeTicketOK(object[] param)
	{
		#if UNITY_EDITOR
		// 2016. 7. 5 jy
		// 클라이언트 싱글화 전투 후 다음 스테이지로 넘어갈 수 있도록 값을 저장한다
		if( Server.ServerMgr.bConnectToServer == false )
		{
			DebugMgr.LogError("bConnectToServer false SingeClientProcess");
			Server.Message.SingeClientProcess(Server.MSGs.MODE_BUY_FOREST_TICKET, AckChargeTicket, (UInt16)_u2StageID, null);
			return;
		}	
		#endif

		// 2016. 10. 31 jy
		// 연속적으로 가능횟수 충전요청이 서버로 넘어가는 현상이 발생함
		// 충전 버튼 활성화 여부로 확인한다
		if(StageInfoMgr.Instance.GetForestTicket() > 0)
			return;

		PopupManager.Instance.ShowLoadingPopup (1);

		Server.ServerMgr.Instance.BuyTicket_Forest(_u2StageID, AckChargeTicket);
	}

	public void AckChargeTicket(Server.ERROR_ID errID)
	{
		PopupManager.Instance.CloseLoadingPopup ();
		_btnChargeTicket.interactable = true;
		if(errID != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.MODE_BUY_FOREST_TICKET, errID), Server.ServerMgr.Instance.CallClear);
		}
		else
		{
			UInt32 needCost = 0;
			UInt32 basicCost = StageInfoMgr.Instance.forest.cChargeGoods.u4Count;
			UInt32 addCost = (UInt32)Mathf.Min(StageInfoMgr.Instance.forest.u4ChargeGoodsAddCost * _u1ChargedTicketCount, StageInfoMgr.Instance.forest.u4ChargeGoodsAddCostMax);
			needCost = basicCost + addCost;
			Goods needGoods = new Goods(
				StageInfoMgr.Instance.forest.cChargeGoods.u1Type, 
				StageInfoMgr.Instance.forest.cChargeGoods.u2ID,
				needCost);
				//StageInfoMgr.Instance.forest.cChargeGoods.u4Count);
			//needGoods.u4Count = ;
			StageInfoMgr.Instance.AddForestChargedTicketCount();
			Legion.Instance.SubGoods(needGoods);
			//SetInfo(_u2StageID);
			UpdateTicket();
		}
	}
}
