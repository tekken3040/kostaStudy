using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

// 상점 목록 슬롯
public class ShopSlot : MonoBehaviour, ISlot<UInt16> {

    public delegate void OnClickSlot(UInt16 id);
    public OnClickSlot onClickSlot;
    
    public Image slotImage;
    public Text shopValue;
	public Text smallShopValue;
    
    public Image shopIcon; 
    
    public GameObject equipObject;
    public Text equipTitle;
    public Text equipDesc;
    public Text gachaCount;
    
    public Image priceIcon;
    public Text priceType;
    public Text priceValue;

	public Image priceBG;
	public Text smallPriceText;

    private UInt16 shopID;
    
    private bool init = false;
    
    private Vector3 startPos;

	public GameObject bonusObj;
	public Text bonusTxt;
    
	public GameObject disObj;
	public DiscountUI disScript;

	public GameObject soldOutMark;

    public void Awake()
    {
        startPos = shopIcon.rectTransform.anchoredPosition3D;
    }

    public void InitSlot(UInt16 id)
    {
        shopID = id;
        ShopGoodInfo info = ShopInfoMgr.Instance.dicShopGoodData[shopID];
        if (info.u1Type == 8 && EventInfoMgr.Instance.dicEventReward.Count > 0)
            SetCashEvent();

        if (info.u1Grade == 99) {
			smallShopValue.enabled = true;
			shopValue.enabled = false;
			smallShopValue.text = TextManager.Instance.GetText (info.title);
			UIManager.Instance.SetGradientFromTier (smallShopValue.GetComponent<Gradient> (), Convert.ToByte (info.itemGrade));
		} else {
			int add_value = 0;
			add_value = LegionInfoMgr.Instance.GetAddVipValue (info) + info.GetBuyBonus ();
			if (add_value == 0) {
				shopValue.text = TextManager.Instance.GetText (info.title);
			} else {
				if ((add_value % 1000) == 0 && (add_value / 1000) > 1)
					shopValue.text = TextManager.Instance.GetText (info.title) + "\n(+" + (add_value / 1000) + "k)";
				else
					shopValue.text = TextManager.Instance.GetText (info.title) + "\n(+" + add_value + ")";
			}
		}
		 
		//천원샵
		if (info.u1Grade == 99) {
			equipObject.SetActive (false);

			slotImage.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Shop/shop_01.shop_grade_" + info.u1Grade);
			shopIcon.GetComponent<RectTransform> ().localScale = Vector3.one;

			if (info.u1Type == 9)
				shopIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Shop/" + info.imagePath);
		}else{
			// 장비 뽑기 일 경우 처리
			if (info.cShopItem.u1Type == 10) {         
				equipObject.SetActive (true);            
				equipTitle.text = TextManager.Instance.GetText (info.itemLevel);
				equipDesc.text = TextManager.Instance.GetText (info.itemGrade);   
				if (string.IsNullOrEmpty (info.gachaCount) == true) {
					gachaCount.text = "";
				} else
					gachaCount.text = TextManager.Instance.GetText (info.gachaCount); 
                
				slotImage.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Shop/shop_01.shop_grade_" + info.u1Grade);

				float size = 1f;

				if (info.u1Type == 9) {
					shopIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Shop/" + info.imagePath);
					size = 0.7f;
				} else
					shopIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Shop/shop_01." + info.imagePath);
				
				shopIcon.rectTransform.anchoredPosition3D = startPos + Vector3.up * 25f;  
				shopIcon.GetComponent<RectTransform> ().localScale = Vector3.one * size;
			} else {
				equipObject.SetActive (false);

				if (info.u1Type == 9)
					shopIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Shop/" + info.imagePath);
				else
					shopIcon.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Shop/shop_02." + info.imagePath);

				shopIcon.GetComponent<RectTransform> ().localScale = Vector3.one * 0.7f;
			}
		}
        
//		priceBG;
//		smallPriceText;
//		smallPriceDesc;
		shopIcon.SetNativeSize();
        // 캐쉬 아이템 처리
        if((GoodsType)info.cBuyGoods.u1Type == GoodsType.PURCHASE)
        {
			if (info.u1Grade == 99) {
				priceBG.enabled = false;
				priceIcon.gameObject.SetActive (false);
				priceType.gameObject.SetActive (false);
				priceValue.enabled = false;
				smallPriceText.enabled = true;
#if UNITY_ANDROID || UNITY_EDITOR
				if (TextManager.Instance.eLanguage == TextManager.LANGUAGE_TYPE.KOREAN) {
					smallPriceText.text = "￦ "+info.cBuyGoods.u4Count.ToString ("n0");
				} else {
					smallPriceText.text = "$ "+info.iOSPrice;
				}
#elif UNITY_IOS
				smallPriceText.text = "$ "+info.iOSPrice;
#endif

			} else {
				priceIcon.gameObject.SetActive (false);
				priceType.gameObject.SetActive (true);
#if UNITY_ANDROID || UNITY_EDITOR
				if (TextManager.Instance.eLanguage == TextManager.LANGUAGE_TYPE.KOREAN) {
					priceType.text = "￦";
					priceValue.text = info.cBuyGoods.u4Count.ToString ("n0");
				} else {
					priceType.text = "$";
					priceValue.text = info.iOSPrice;
				}
#elif UNITY_IOS
	            priceType.text = "$";
	            priceValue.text = info.iOSPrice;
#endif
			}
        }
        else
        {
            priceIcon.gameObject.SetActive(true);
            priceIcon.sprite = AtlasMgr.Instance.GetGoodsIcon(info.cBuyGoods);
            priceType.gameObject.SetActive(false);
			priceValue.text = SetDisCountInfo().ToString("n0");

        }

		//Event Item Buy Limit
		CheckBuyPossible(info);
    }

	uint SetDisCountInfo(){
		ShopGoodInfo info = ShopInfoMgr.Instance.dicShopGoodData[shopID];

		uint price = info.cBuyGoods.u4Count;
		Byte disRate = 0;
		if (info.u1Type < 5) {
			EventDisCountinfo disInfo = EventInfoMgr.Instance.GetDiscountEventinfo (DISCOUNT_ITEM.LOTTERY);
			if (disInfo != null) {
				price = (uint)(info.cBuyGoods.u4Count * disInfo.discountRate);
				disRate = disInfo.u1DiscountRate;
			}
		} else if (info.u1Type == 6) {
			EventDisCountinfo disInfo = EventInfoMgr.Instance.GetDiscountEventinfo (DISCOUNT_ITEM.BUYGOLD);
			if (disInfo != null){
				price = (uint)(info.cBuyGoods.u4Count * disInfo.discountRate);
				disRate = disInfo.u1DiscountRate;
			}
		} else if (info.u1Type == 7) {
			EventDisCountinfo disInfo = EventInfoMgr.Instance.GetDiscountEventinfo (DISCOUNT_ITEM.BUYKEY);
			if (disInfo != null){
				price = (uint)(info.cBuyGoods.u4Count * disInfo.discountRate);
				disRate = disInfo.u1DiscountRate;
			}
		}

		if (price != info.cBuyGoods.u4Count) {
			disObj.SetActive (true);
			disScript.SetData (info.cBuyGoods.u4Count, disRate);
		} else {
			disObj.SetActive (false);
		}

		return price;
	}

	void CheckBuyPossible(ShopGoodInfo info){

		bool bActivate = true;
		EventItemBuyCountInfo buyInfo = Legion.Instance.cEvent.lstItemBuyHistory.Find (cs => cs.u2ShopID == info.u2ID);

		if (buyInfo == null)
			return;

		if (!EventInfoMgr.Instance.dicFixshopEventID.ContainsKey (info.u2ID)) {
			return;
		}

		if (info.u1BuyOverlap == 2) {
			bActivate = false;
		} else if (info.u1BuyOverlap == 3) {
			List<KeyValuePair<UInt16, ShopGoodInfo>> temp = ShopInfoMgr.Instance.dicShopGoodData.Where (cs => cs.Value.u2GroupIndex == info.u2GroupIndex).ToList();

			for (int i = 0; i < temp.Count; i++)
			{
				if (EventInfoMgr.Instance.dicFixshopEventID.ContainsKey (temp [i].Value.u2ID)) 
				{
					if (Legion.Instance.cEvent.lstItemBuyHistory.FindIndex (cs => cs.u2ShopID == temp [i].Value.u2ID) > -1) 
					{
						bActivate = false;
					}
				}
			}
		} else if (info.u1BuyOverlap == 1) {
			if (buyInfo.u4BuyCount >= info.u2BuyRestriction) {
				bActivate = false;
			}
		}

		if (!bActivate) {
			DebugMgr.LogError (info.u2ID);
			GetComponent<Button> ().interactable = false;
			soldOutMark.SetActive (true);
		} else {
			soldOutMark.SetActive (false);
		}
	}

    public void refleshSlot()
    {      
        ShopGoodInfo info = ShopInfoMgr.Instance.dicShopGoodData[shopID];
        if (info.u1Type == 8 && EventInfoMgr.Instance.dicEventReward.Count > 0)
            SetCashEvent();

        int add_value = 0;
		add_value = LegionInfoMgr.Instance.GetAddVipValue(info) + info.GetBuyBonus();
        if (add_value == 0)
        {
            shopValue.text = TextManager.Instance.GetText(info.title);
        }
        else
        {
            if((add_value % 1000) == 0 && (add_value / 1000) > 1)
                shopValue.text = TextManager.Instance.GetText(info.title) + "\n(+" + (add_value/1000) + "k)";
            else
                shopValue.text = TextManager.Instance.GetText(info.title) + "\n(+" + add_value + ")";
        }

		CheckBuyPossible(info);

        if ((GoodsType)info.cBuyGoods.u1Type == GoodsType.PURCHASE)
        {
            if (info.u1Grade == 99)
            {
                priceBG.enabled = false;
                priceIcon.gameObject.SetActive(false);
                priceType.gameObject.SetActive(false);
                priceValue.enabled = false;
                smallPriceText.enabled = true;
#if UNITY_ANDROID || UNITY_EDITOR
                if (TextManager.Instance.eLanguage == TextManager.LANGUAGE_TYPE.KOREAN)
                {
                    smallPriceText.text = "￦ " + info.cBuyGoods.u4Count.ToString("n0");
                }
                else
                {
                    smallPriceText.text = "$ " + info.iOSPrice;
                }
#elif UNITY_IOS
				smallPriceText.text = "$ "+info.iOSPrice;
#endif

            }
            else
            {
                priceIcon.gameObject.SetActive(false);
                priceType.gameObject.SetActive(true);
#if UNITY_ANDROID || UNITY_EDITOR
                if (TextManager.Instance.eLanguage == TextManager.LANGUAGE_TYPE.KOREAN)
                {
                    priceType.text = "￦";
                    priceValue.text = info.cBuyGoods.u4Count.ToString("n0");
                }
                else
                {
                    priceType.text = "$";
                    priceValue.text = info.iOSPrice;
                }
#elif UNITY_IOS
	            priceType.text = "$";
	            priceValue.text = info.iOSPrice;
#endif
            }
        }
        else
        {
            priceValue.text = SetDisCountInfo().ToString("n0");
        }
		//if(
    }

    public void OnClick()
    {
        if(onClickSlot != null)
            onClickSlot(shopID);
    }

    private void SetCashEvent()
    {
        Byte u1Add = 0;
        EventReward eventInfo;
        if (EventInfoMgr.Instance.dicEventReward.FirstOrDefault(cs => cs.Value.u1EventType == (Byte)EVENT_TYPE.FIRSTPAYMENT).Key > 0)
        {
            eventInfo = EventInfoMgr.Instance.dicEventReward.FirstOrDefault(cs => cs.Value.u1EventType == (Byte)EVENT_TYPE.FIRSTPAYMENT).Value;

            bool isFirstBuy = false;
            int buyListCount = Legion.Instance.cEvent.lstItemBuyHistory.Count;
            for (int i = 0; i < buyListCount; ++i)
            {
                if (Legion.Instance.cEvent.lstItemBuyHistory[i].u2ShopID == shopID &&
                    Legion.Instance.cEvent.lstItemBuyHistory[i].u2EventID == eventInfo.u2EventID)
                {
                    isFirstBuy = true;
                    break;
                }
            }

            if (isFirstBuy == false)
            {
                u1Add = eventInfo.u1RewardIndex;
                bonusTxt.text = string.Format(TextManager.Instance.GetText("mark_goods_event_add_cash_first"), u1Add);
                // 첫구매를 한적이 없으므로 활성화
                bonusObj.SetActive(true);
                return;
            }
        }

        if (u1Add == 0)
        {
            if (EventInfoMgr.Instance.dicEventReward.FirstOrDefault(cs => cs.Value.u1EventType == (Byte)EVENT_TYPE.ADDITIONALREWARD).Key > 0)
            {
                eventInfo = EventInfoMgr.Instance.dicEventReward.FirstOrDefault(cs => cs.Value.u1EventType == (Byte)EVENT_TYPE.ADDITIONALREWARD).Value;
                
                bool isAddCash = true;
                // 이벤트가 0시작이 0이 아니라면 시간이 셋팅되어 있음
                if (eventInfo.u8EventBegin != 0)
                {
                    // 이벤트가 시작되었는지 확인한다
                    if (DateTime.Compare(eventInfo.dtEventBegin, Legion.Instance.ServerTime) > 0)
                        isAddCash = false;
                }

                if (isAddCash == true)
                {
                    if (eventInfo.u8EventEnd != 0)
                    {
                        // 이벤트가 종료되었는지 확인한다
                        if (DateTime.Compare(eventInfo.dtEventEnd, Legion.Instance.ServerTime) <= 0)
                            isAddCash = false;
                    }
                }

                if(isAddCash == true)
                {
                    u1Add = eventInfo.u1RewardIndex;
                    bonusTxt.text = string.Format(TextManager.Instance.GetText("mark_goods_event_add_cash"), u1Add);
                    bonusObj.SetActive(true);
                    return;
                }
            }
        }

        bonusObj.SetActive(false);
    }
}
