using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;

public class GrowthPackSlot : MonoBehaviour 
{
    //public Image _imgSlotBG;		// 슬롯 BG 
    public Text _packageTitle;      // 패키지 타이틀
    public Image _packageIcon;      // 패키지 아이콘
	public GameObject _objDisableImg;	// 비활성화 이미지
    public Text _txtPackConfig;	    // 패키지 구성품
    public Button _btnBuyBtn;       // 구매 버튼 이미지
    public Text _txtBuyPrice;		// 구매 가격

    //public GrowthPackItemSlot[] _arrEventItemSlots;

	public void SetSlot(EventPackageInfo eventInfo, int slotIdx, byte u1EventGroupIdx)
	{
        StringBuilder tempString = new StringBuilder();
		_packageTitle.text = TextManager.Instance.GetText(eventInfo.sName);
        _packageIcon.sprite = AtlasMgr.Instance.GetSprite(tempString.Append("Sprites/").Append(eventInfo.sBgImagePath).ToString());
        _packageIcon.SetNativeSize();

        if (u1EventGroupIdx != 0)
            _packageIcon.GetComponent<RectTransform>().sizeDelta *= 0.7f;
        else
            _packageIcon.GetComponent<RectTransform>().sizeDelta *= 0.8f;

        // 패키지 내용물 셋팅
        tempString.Remove(0, tempString.Length);
        if(u1EventGroupIdx != 0)
            tempString.Append("<color=").Append(eventInfo.sEvnetNotice).Append(">").Append(GetPackageConfig(eventInfo)).Append("</color>");
        else
            tempString.Append(GetPackageConfig(eventInfo));

        _txtPackConfig.text = tempString.ToString();
        // 가격 셋팅
        tempString.Remove(0, tempString.Length);
        _txtBuyPrice.text = tempString.Append(TextManager.Instance.GetText("btn_event_grow_buy")).Append(" ").Append(GetEventPackagePriceString(eventInfo)).ToString();
    }

	public void DisableButton()
	{
		_btnBuyBtn.interactable = false;
		_objDisableImg.SetActive(true);
	}

    private string GetPackageConfig(EventPackageInfo info)
    {
        StringBuilder tempString = new StringBuilder();
        int rewardCount = info.acPackageRewards.Length;
        for (int i = 0; i < rewardCount; ++i)
        {
            if (info.acPackageRewards[i].u1Type == 0)
                continue;

            tempString.Append(Legion.Instance.GetGoodsName(info.acPackageRewards[i]));
            if ((i + 1) >= rewardCount)
                tempString.Append("x").Append(info.acPackageRewards[i].u4Count.ToString("n0")).Append("\n");
            else
            {
                if (info.acPackageRewards[(i + 1)].u1Type == 0 ||
                   info.acPackageRewards[i].u1Type != info.acPackageRewards[(i + 1)].u1Type ||
                   info.acPackageRewards[i].u4Count != info.acPackageRewards[(i + 1)].u4Count)
                {
                    tempString.Append("x").Append(info.acPackageRewards[i].u4Count.ToString("n0")).Append("\n");
                }
                else
                {
                    tempString.Append("/");
                }
            }
        }

        return tempString.ToString();
    }
    /**/

    /*
	/// <summary>
	/// 패키지 아이템 슬롯 셋팅
	/// </summary>
	private void SetPackageItemSlot(EventPackageInfo info)
	{
        for(int i = 0; i < _arrEventItemSlots.Length; ++i)
        {
            _arrEventItemSlots[i].gameObject.SetActive(false);
        }

		int setItemCount = 0;
        int rewardCount = info.acPackageRewards.Length;
        for (int i = 0; i < rewardCount; ++i)
		{
            // 준비된 슬롯보다 세팅된 겠수할 아이템 겟수가 크다면 종료
            if (_arrEventItemSlots.Length < setItemCount)
                break;

            // 아이템 타입이 0이면 빈 정보
            if (info.acPackageRewards[i].u1Type == 0)
                continue;

            // 인덱스가 0보다 크면
            if( i > 0 )
            {
                // 이전 상품과 자신의 상품타입이 재료라면 다음 아이템으로 넘김
                if (info.acPackageRewards[i - 1].u1Type == (byte)GoodsType.MATERIAL &&
                    info.acPackageRewards[i].u1Type == (byte)GoodsType.MATERIAL)
                    continue;
            }

            _arrEventItemSlots[setItemCount].SetEventItem(setItemCount, info.acPackageRewards[i]);
            if ((i+1) < info.acPackageRewards.Length)
            {
                if (info.acPackageRewards[i + 1].u1Type == (byte)GoodsType.MATERIAL &&
                    info.acPackageRewards[i].u1Type == (byte)GoodsType.MATERIAL)
                {
                    _arrEventItemSlots[setItemCount]._txtItemInfo.text = TextManager.Instance.GetText(info.sEvnetNotice);
                }
            }
            
			++setItemCount;
		}
	}
    */
    protected string GetEventPackagePriceString(EventPackageInfo packageInfo)
	{
		StringBuilder tempString = new StringBuilder();
		#if UNITY_ANDROID || UNITY_EDITOR
		if(TextManager.Instance.eLanguage == TextManager.LANGUAGE_TYPE.KOREAN)
			tempString.Append("￦").Append(packageInfo.cNeedGoods.u4Count.ToString("n0"));
		else
			tempString.Append("$").Append(packageInfo.iOSPrice.ToString());
		#elif UNITY_IOS
		tempString.Append("$").Append(packageInfo.iOSPrice.ToString());
		#endif
		return tempString.ToString();
	}
}
