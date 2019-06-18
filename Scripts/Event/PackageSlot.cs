using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class PackageSlot : MonoBehaviour {

	enum PackageSlotType{
		Recommend = 6,
		Limit = 7,
		Promot = 3,
		Month = 5
	}

	EventPackageInfo info;
	public Text textTitle;
	public Text textPeriod;
	public Text textReward;

	public Text btnText;
	public Button btnBuy;
	public GameObject rewardIndicator;
	public GameObject rewardedMark;

	public RectTransform _trListGrid;
	public GameObject _objNewProduct;
	public GameObject _objSaleBG;
	public Text _txtDiscountRate;
	public Text _txtBasePrice;

	public int promotIndex = -1;

	public void SetSlot (EventPackageInfo tInfo, int idx = 0, uint lv = 0) {
		info = tInfo;
		
		switch ((PackageSlotType)tInfo.u1EventType) {
		case PackageSlotType.Recommend:
			{
				if(_trListGrid.childCount > 0)
					return;
				
				textTitle.text = TextManager.Instance.GetText (info.sName);
				int cut = 0;
				for(int i = 0; i < info.acPackageRewards.Length; ++i)
				{
					if(_trListGrid.childCount >= 2)
						break;
					
					if(info.acPackageRewards[i].u1Type == 0)
						continue;

					GameObject eventItemSlot = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/pref_RecomEventItemSlot.prefab", typeof(GameObject))) as GameObject;
					eventItemSlot.transform.SetParent(_trListGrid);
					eventItemSlot.transform.localPosition = Vector3.zero;
					eventItemSlot.transform.localScale = Vector3.one;
					RecomEventItemSlot slot = eventItemSlot.GetComponent<RecomEventItemSlot>();
					slot.SetEventItem(cut++, info.acPackageRewards[i]);
					// 재로템은 별도로 장비 텍스트를 받는다
					if(slot.IsFirstMaterialType == true)
						slot._txtItemInfo.text = TextManager.Instance.GetText(info.sEvnetNotice);
					
					if( i < info.acPackageRewards.Length )
					{
						for(i += 1; i < info.acPackageRewards.Length; ++i)
						{
							if(info.acPackageRewards[i].u1Type == 0)
								continue;

							slot.SetSecondEventItem(info.acPackageRewards[i]);
							if(slot.IsFirstMaterialType == false && (GoodsType)info.acPackageRewards[i].u1Type == GoodsType.MATERIAL)
								slot._txtItemInfo.text = TextManager.Instance.GetText(info.sEvnetNotice);
							
							break;
						}
					}
				}

				SetBtnText();
				if (EventInfoMgr.Instance.dicEventBuy [tInfo.u2ID].u8BuyEnd == 0)
				{
					string messgae = "";
					if (info.cNeedMinPeriod.u1Type > 0)
						messgae = Legion.Instance.GetConsumeString (info.cNeedMinPeriod.u1Type) + info.cNeedMinPeriod.u4Count;
					if (info.cNeedMinPeriod.u1Type > 0 || info.cNeedMaxPeriod.u1Type > 0)
						messgae += " ~ ";
					if (info.cNeedMaxPeriod.u1Type > 0)
						messgae += info.cNeedMaxPeriod.u4Count;
					if(messgae != "") 
						messgae += TextManager.Instance.GetText("mark_event_buynow");//"\n구매가능";

					textPeriod.text = messgae;
				}
				else
				{
					textPeriod.text = "";
					if (EventInfoMgr.Instance.dicEventBuy [tInfo.u2ID].u8BuyBegin != 0)
						textPeriod.text = EventInfoMgr.Instance.dicEventBuy [tInfo.u2ID].dtBuyBegin.ToString ("yyyy.MM.dd");
					if (EventInfoMgr.Instance.dicEventBuy [tInfo.u2ID].u8BuyBegin != 0 || EventInfoMgr.Instance.dicEventBuy [tInfo.u2ID].u8BuyEnd != 0)
						textPeriod.text += " ~ ";
					if (EventInfoMgr.Instance.dicEventBuy [tInfo.u2ID].u8BuyEnd != 0)
						textPeriod.text += EventInfoMgr.Instance.dicEventBuy [tInfo.u2ID].dtBuyEnd.ToString ("yyyy.MM.dd");
				}
			}
			break;
		case PackageSlotType.Limit:
			{
				if(_trListGrid.childCount > 0)
					return;

				textTitle.text = TextManager.Instance.GetText (info.sName);
				int cut = 0;
				for(int i = 0; i < info.acPackageRewards.Length; ++i)
				{
					if(info.acPackageRewards[i].u1Type == 0)
						continue;

					GameObject eventItemSlot = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_EventItemSlot.prefab", typeof(GameObject))) as GameObject;
					eventItemSlot.transform.SetParent(_trListGrid);
					eventItemSlot.transform.localPosition = Vector3.zero;
					eventItemSlot.transform.localScale = Vector3.one;
					eventItemSlot.GetComponent<EventItemSlot>().SetEventItem(cut++, info.acPackageRewards[i]);
				}
				SetBtnText();
				_objNewProduct.SetActive(info.isNewProduct);
				if(info.sDiscountRate != "")
				{
					_objSaleBG.SetActive(true);
					//_txtBasePrice.text = "$ " + info.sCost;
					_txtDiscountRate.text = info.sDiscountRate + "%";
				}
				else
					_objSaleBG.SetActive(false);
			}
			if (EventInfoMgr.Instance.dicEventBuy[tInfo.u2ID].u8BuyEnd == 0)
			{
				string messgae = "";
				if (info.cNeedMinPeriod.u1Type > 0)
					messgae = Legion.Instance.GetConsumeString (info.cNeedMinPeriod.u1Type) + info.cNeedMinPeriod.u4Count;
				if (info.cNeedMinPeriod.u1Type > 0 || info.cNeedMaxPeriod.u1Type > 0)
					messgae += " ~ ";
				if (info.cNeedMaxPeriod.u1Type > 0)
					messgae += info.cNeedMaxPeriod.u4Count;
				if(messgae != "") 
					messgae += TextManager.Instance.GetText("mark_event_buynow");

				textPeriod.text = messgae;
			}
			else
			{
				textPeriod.text = "";
				if (EventInfoMgr.Instance.dicEventBuy [tInfo.u2ID].u8BuyBegin != 0)
					textPeriod.text = EventInfoMgr.Instance.dicEventBuy [tInfo.u2ID].dtBuyBegin.ToString ("yyyy.MM.dd");
				if (EventInfoMgr.Instance.dicEventBuy [tInfo.u2ID].u8BuyBegin != 0 || EventInfoMgr.Instance.dicEventBuy [tInfo.u2ID].u8BuyEnd != 0)
					textPeriod.text += " ~ ";
				if (EventInfoMgr.Instance.dicEventBuy [tInfo.u2ID].u8BuyEnd != 0)
					textPeriod.text += EventInfoMgr.Instance.dicEventBuy [tInfo.u2ID].dtBuyEnd.ToString ("yyyy.MM.dd");
			}
			break;
		case PackageSlotType.Promot:
			textTitle.text = TextManager.Instance.GetText("mark_even_grow_level") +lv ;//"레벨";
			promotIndex = idx;
			textReward.text = info.acPackageRewards [idx].u4Count.ToString();
			btnText.text = TextManager.Instance.GetText("btn_even_grow_get");//"수령";
			break;
		case PackageSlotType.Month:			
			textTitle.text = TextManager.Instance.GetText("mark_event_30day");
			UInt32 rewardCount = info.acPackageRewards[0].u4Count;
			textReward.text = string.Format(TextManager.Instance.GetText("mark_event_30day_desc_1"), rewardCount, rewardCount * info.u2KeepDay, info.cRewardOnce.u4Count);
			textPeriod.text = string.Format(TextManager.Instance.GetText("mark_event_30day_desc_2"), rewardCount);
			break;
		}
	}

	public void DisableButton(){
		btnBuy.interactable = false;
		btnText.GetComponent<Gradient> ().enabled = false;
		btnText.color = Color.gray;
	}

	public void SetRewarded(){
		btnBuy.interactable = false;
		btnText.GetComponent<Gradient> ().enabled = false;
		btnText.color = Color.gray;
		if(rewardIndicator != null)
			rewardIndicator.SetActive (false);
		if(rewardedMark != null)
			rewardedMark.SetActive (true);
	}

	void SetBtnText(){
		if (info.cNeedGoods.u1Type == (byte)GoodsType.PURCHASE) {
			//#if UNITY_ANDROID
			//btnText.text = TextManager.Instance.GetText("btn_event_grow_buy")+" ￦"+info.cNeedGoods.u4Count;
			//#else
			btnText.text = /*TextManager.Instance.GetText("btn_event_grow_buy")+*/"$" + info.iOSPrice;
			//#endif
		} else {
			btnText.text =  /*TextManager.Instance.GetText("btn_event_grow_buy")+*/info.cNeedGoods.u4Count+" "+ Legion.Instance.GetConsumeString (info.cNeedGoods.u1Type);
		}
	}
}
