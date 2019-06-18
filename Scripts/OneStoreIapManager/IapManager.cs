using UnityEngine;
using System.Collections;
using System;
using IapResponse;
using IapVerifyReceipt;
using IapError;
using IgaworksUnityAOS;
using System.Collections.Generic;

public class IapManager : Singleton<IapManager> {

#if UNITY_ONESTORE

	//------------------------------------------------
	//
	// Variables
	//
	//------------------------------------------------

	private AndroidJavaClass unityPlayerClass = null;
	private AndroidJavaObject currentActivity = null;
	private AndroidJavaObject iapRequestAdapter = null;
    private bool _inited = false;
    public bool IsInited
    {
        get
        {
            return _inited;
        }
    }
    private string AppID = "OA00710909";

	public void Init () 
	{
        if(_inited)
            return;
         gameObject.name = "IapManager";
		//-----------------
		// Initialize
		//-----------------
		unityPlayerClass = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
		currentActivity = unityPlayerClass.GetStatic<AndroidJavaObject> ("currentActivity");
        DebugMgr.LogError("Init 원스토어");
		if (currentActivity != null) {
			// RequestAdapter를 초기화
			// ---------------------------------
			// 함수 parameter 정리
			// ---------------------------------
			// (1) 콜백을 받을 클래스 이름
			// (2) Activity Context
			// (3) debug 여부
	#if live
			iapRequestAdapter = new AndroidJavaObject("com.onestore.iap.unity.RequestAdapter", "IapManager", currentActivity, false); //Release
	#else
			iapRequestAdapter = new AndroidJavaObject("com.onestore.iap.unity.RequestAdapter", "IapManager", currentActivity, true); //Debug
	#endif
            _inited = true;
		}
	}
     
	void Destroy () 
	{
		if (unityPlayerClass != null)
			unityPlayerClass.Dispose ();
		if (currentActivity != null)
			currentActivity.Dispose ();
		if (iapRequestAdapter != null)
			iapRequestAdapter.Dispose ();
	}

	//------------------------------------------------
	//
	// Exit
	//
	//------------------------------------------------
	public void Exit () 
	{
		if (iapRequestAdapter != null) 
		{
			DebugMgr.LogError ("Exit called!!!");
			iapRequestAdapter.Call ("exit");
		}
	}

	//------------------------------------------------
	//
	// Command - Request
	//
	//------------------------------------------------

	public void RequestPurchaseHistory() 
	{
		// ---------------------------------
		// 함수 parameter 정리
		// ---------------------------------
		// (0) 메소드이름 : 구매내역 조회
		// ---------------------------------
		// (1) 필요시에는 UI 노출
		// (2) appId
		// (3) productIds
		// ----------------------------------
		string[] productIds = {"0910066902"};
		iapRequestAdapter.Call ("requestPurchaseHistory", false, AppID, productIds);
		//iapRequestAdapter.Call ("requestPurchaseHistory", true, AppID, productIds); // UI노출 없이 Background로만 진행
	}

	public void RequestProductInfo() 
	{
		// ---------------------------------
		// 함수 parameter 정리
		// ---------------------------------
		// (0) 메소드이름 : 상품정보 조회
		// ---------------------------------
		// (1) 필요시에는 UI 노출
		// (2) appId
		// ----------------------------------
		iapRequestAdapter.Call ("requestProductInfo", false, AppID);
		//iapRequestAdapter.Call ("requestProductInfo", true, AppID); // UI노출 없이 Background로만 진행
	}

	public void RequestCheckPurchasability() 
	{
		// ---------------------------------
		// 함수 parameter 정리
		// ---------------------------------
		// (0) 메소드이름 : 구매가능여부 조회
		// ---------------------------------
		// (1) 필요시에는 UI 노출
		// (2) appId
		// (3) productIds
		// ----------------------------------
		string[] productIds = {"0910066902"};
		iapRequestAdapter.Call ("requestCheckPurchasability", false, AppID, productIds);
		//iapRequestAdapter.Call ("requestCheckPurchasability", true, AppID, productIds); // UI노출 없이 Background로만 진행
	}

	public void RequestSubtractPoints() 
	{
		// ---------------------------------
		// 함수 parameter 정리
		// ---------------------------------
		// (0) 메소드이름 : 상품 속성변경 요청 
		// ---------------------------------
		// (1) 필요시에는 UI 노출
		// (2) action(아이템차감)
		// (3) appId
		// (4) productIds
		// ----------------------------------
		string[] productIds = {"0910066902"};
		iapRequestAdapter.Call ("requestChangeProductProperties", false, "subtract_points", AppID, productIds);
		//iapRequestAdapter.Call ("requestChangeProductProperties", true, "subtract_points", AppID, productIds); // UI노출 없이 Background로만 진행
	}

	public void RequestCancelSubscription() 
	{
		// ---------------------------------
		// 함수 parameter 정리
		// ---------------------------------
		// (0) 메소드이름 : 상품 속성변경 요청 
		// ---------------------------------
		// (1) 필요시에는 UI 노출
		// (2) action(자동결제 취소)
		// (3) appId
		// (4) productIds
		// ----------------------------------
		string[] productIds = {"0910066902"};
		iapRequestAdapter.Call ("requestChangeProductProperties", false, "cancel_subscription", AppID, productIds);
		//iapRequestAdapter.Call ("requestChangeProductProperties", true, "cancel_subscription", AppID, productIds); // UI노출 없이 Background로만 진행
	}


	//------------------------------------------------
	//
	// Command - Callback
	//
	//------------------------------------------------

	public void CommandResponse(string response) 
	{
		DebugMgr.LogError ("--------------------------------------------------------");
		DebugMgr.LogError ("[UNITY] CommandResponse >>> " + response);
		DebugMgr.LogError ("--------------------------------------------------------");

		// Parsing Json string to "Reponse" class
		Response data = JsonUtility.FromJson<Response> (response);
		DebugMgr.LogError (">>> " + data.ToString());
		DebugMgr.LogError ("--------------------------------------------------------");
	}

	public void CommandError(string message) 
	{
		DebugMgr.LogError ("--------------------------------------------------------");
		DebugMgr.LogError ("[UNITY] CommandError >>> " + message);
		DebugMgr.LogError ("--------------------------------------------------------");

		// Parsing Json string to "Error" class
		Error data = JsonUtility.FromJson<Error> (message);
		DebugMgr.LogError (">>> " + data.ToString());
		DebugMgr.LogError ("--------------------------------------------------------");
	}


	//------------------------------------------------
	//
	// Payment - Request
	//
	//------------------------------------------------
    ShopResult.OnResponse CallBackMethod;
	public void RequestPaymenet(string productID, string productName, string tID, string bpInfo, ShopResult.OnResponse CallBack)
	{
		// ---------------------------------
		// 함수 parameter 정리
		// ---------------------------------
		// (0) 메소드이름 : 구매요청
		// ---------------------------------
		// (1) appId
		// (2) productId
		// (3) proudtName
		// (4) tId(개발사 생성하는 ID값)
		// (5) bpInfo(캠페인 통계 등을 위해서 개발사가 자유롭게 사용하는 태그)
		// ----------------------------------
        Application.runInBackground = true;
        DebugMgr.LogError("원스토어 결제 호출 in");
        CallBackMethod = CallBack;
		iapRequestAdapter.Call ("requestPayment", AppID, productID, productName, tID, bpInfo);
        //iapRequestAdapter.Call ("requestPayment", "OA00679020", "0910024112", "UNITY결제", "TID_0123", "BPINFO_0123");
	}

	public void VerifyReceipt() 
	{
		// ---------------------------------
		// 함수 parameter 정리
		// ---------------------------------
		// (0) 메소드이름 : 구매요청
		// ---------------------------------
		// (1) appId
		// (2) txId
		// (3) signData
		// ----------------------------------
		//iapRequestAdapter.Call ("verifyReceipt", appId, txId, signData);
	}
     
	//------------------------------------------------
	//
	// Payment - Callback
	//
	//------------------------------------------------
    public Response cResultData;
	public void PaymentResponse(string response) 
	{
		DebugMgr.LogError ("--------------------------------------------------------");
		DebugMgr.LogError ("[UNITY] PaymentResponse >>> " + response);
		DebugMgr.LogError ("--------------------------------------------------------");

		// Parsing Json string to "Reponse" class
		Response data = JsonUtility.FromJson<Response> (response);
		DebugMgr.LogError (">>> " + data.ToString());
		DebugMgr.LogError ("--------------------------------------------------------");
        cResultData = new Response();
        cResultData = data;
        DebugMgr.LogError("원스토어 결제 호출 완료 콜백");
        //CallBackMethod(cResultData.result.receipt, cResultData.result.txid);
        //CallBackMethod(data.result.receipt, data.result.txid);
		// Try ReceiptVerification
		iapRequestAdapter.Call ("verifyReceipt", AppID, data.result.txid, data.result.receipt);
	}

	public void PaymentError(string message) 
	{
		DebugMgr.LogError ("--------------------------------------------------------");
		DebugMgr.LogError ("[UNITY] PaymentError >>> " + message);
		DebugMgr.LogError ("--------------------------------------------------------");

		// Parsing Json string to "Error" class
		Error data = JsonUtility.FromJson<Error> (message);
		DebugMgr.LogError (">>> " + data.ToString());
		DebugMgr.LogError ("--------------------------------------------------------");
        PopupManager.Instance.CloseLoadingPopup();
	}

	public void ReceiptVerificationResponse(string result) 
	{
		DebugMgr.LogError ("--------------------------------------------------------");
		DebugMgr.LogError ("[UNITY] ReceiptVerificationResponse >>> " + result);
		DebugMgr.LogError ("--------------------------------------------------------");

		// Parsing Json string to "VerifyReceipt" class
		VerifyReceipt data = JsonUtility.FromJson<VerifyReceipt> (result);
		DebugMgr.LogError (">>> " + data.ToString());
		DebugMgr.LogError ("--------------------------------------------------------");
        DebugMgr.LogError ("receipt = " + cResultData.result.receipt);
        DebugMgr.LogError ("txid = " + cResultData.result.txid);
		int resultCode = Convert.ToInt32 (data.detail);
		if (resultCode == 0) {
			List<IgaworksUnityPluginAOS.IgawCommerceItemModel> items = new List<IgaworksUnityPluginAOS.IgawCommerceItemModel>();
			for(int i=0; i<cResultData.result.product.Count; i++){
				IgaworksUnityPluginAOS.IgawCommerceItemModel item = new IgaworksUnityPluginAOS.IgawCommerceItemModel (
					                             cResultData.result.receipt,
					                             cResultData.result.product [i].id,
					                             cResultData.result.product [i].name,
					                             cResultData.result.product [i].price,
					                             1,
					                             IgaworksUnityPluginAOS.Currency.KR_KRW,
					                             "");

				items.Add (item);
			}
			IgaworksUnityPluginAOS.Adbrix.purchase (items);
            IgaworksUnityPluginAOS.Adbrix.retention ("Purchase");
			CallBackMethod (cResultData.result.receipt, cResultData.result.txid);
		} else {
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_payment_fail"), TextManager.Instance.GetText("popup_desc_paymen_fail"), null);
		}
	}

	public void ReceiptVerificationError(string message) 
	{
		DebugMgr.LogError ("--------------------------------------------------------");
		DebugMgr.LogError ("[UNITY] ReceiptVerificationError >>> " + message);
		DebugMgr.LogError ("--------------------------------------------------------");

		// Parsing Json string to "Error" class
		Error data = JsonUtility.FromJson<Error> (message);
		DebugMgr.LogError (">>> " + data.ToString());
		DebugMgr.LogError ("--------------------------------------------------------");
        PopupManager.Instance.CloseLoadingPopup();
	}

	// ------------------------------------------------------

#endif
}
