using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class UpbringingRewardSlot : MonoBehaviour 
{
	public Text _txtRewardLevel;
	public Text _txtRewardInfo;
	public Text _txtBtnText;
	public Image _imgRewardIcon;
	public Button _btnGetReward;
    public GameObject _objCheckImg;
    //public GameObject _objDisabledImg;

    protected int _nSlotIndex;
	public int SlotIndex { get {return _nSlotIndex; }}

	public void SetSlot(EventPackageInfo eventInfo, int slotIdx)
	{
		_nSlotIndex = slotIdx;
		StringBuilder tempString = new StringBuilder();
		// 지급 레벨 설정
		tempString.Append(Legion.Instance.GetConsumeString(eventInfo.u1PensionNeedType)).Append(" ");
		tempString.Append((slotIdx + 1) * eventInfo.u4PensionNeedMin);
		_txtRewardLevel.text = tempString.ToString();

		// 보상 아이템 셋팅
		Goods rewardGood = eventInfo.acPackageRewards[slotIdx];
		tempString.Remove(0, tempString.Length);
		tempString.Append(Legion.Instance.GetConsumeString(rewardGood.u1Type)).Append(" ");
		tempString.Append(rewardGood.u4Count);
		_txtRewardInfo.text = tempString.ToString();

		//// 아이템에 따른 텍스트 컬러 변경
		//Gradient cInfoTextGradient = _txtRewardInfo.GetComponent<Gradient>();
		//if(cInfoTextGradient != null)
		//	cInfoTextGradient.EndColor = GetTextColor(rewardGood.u1Type);

		// 아이템 아이콘 셋팅
		_imgRewardIcon.sprite = GetItemICon(rewardGood);
		_imgRewardIcon.GetComponent<RectTransform>().sizeDelta *= 0.8f;
	}

	public void DisableButton()
	{
		_objCheckImg.SetActive(false);
		_btnGetReward.interactable = false;
        //_objDisabledImg.SetActive(true);
        SetSlotColor(false);
        _txtBtnText.text = TextManager.Instance.GetText("btn_time_reward_get");
    }

	public void SetRewardDone(bool isDone)
	{
		if(isDone == true)	
		{
			_btnGetReward.interactable = false;
			_objCheckImg.SetActive(true);
            //_objDisabledImg.SetActive(true);
            SetSlotColor(false);
			_txtBtnText.text = TextManager.Instance.GetText("btn_tiem_reward_done");
		}
		else
		{
			_btnGetReward.interactable = true;
			_objCheckImg.SetActive(false);
            //_objDisabledImg.SetActive(false);
            SetSlotColor(true);
            _txtBtnText.text = TextManager.Instance.GetText("btn_time_reward_get");
		}
	}

    // 비활성화 이미지를 넣을 으면 배경과 이질감이 생겨 아이콘 및 텍스트 색상을 변경한다
    private void SetSlotColor(bool isEanble)
    {
        if (isEanble == true)
        {
            _txtRewardLevel.color = Color.white;
            _txtRewardInfo.color = Color.white;
            _imgRewardIcon.color = Color.white;
        }
        else
        {
            _txtRewardLevel.color = Color.gray;
            _txtRewardInfo.color = Color.gray;
            _imgRewardIcon.color = Color.gray;
        }
    }
    /*
	protected virtual Color GetTextColor(Byte itmeType)
	{
		Color color;
		switch((GoodsType)itmeType)
		{
		case GoodsType.GOLD: case GoodsType.KEY:
			color = new Color32(255, 180, 100, 255);
			break;
		case GoodsType.CASH:
			color = new Color32(255, 100, 100, 255);
			break;
		default:
			color = new Color32(101, 218, 209, 255);
			break;
		}
		return color;
	}
    
	protected Color SetColor(int r, int g, int b, int a = 255)
	{
		Color resultColor = new Color(
			(r == 0) ? r : (float)Math.Round( (double)r / 255, 3), 
			(g == 0) ? g : (float)Math.Round( (double)g / 255, 3),
			(b == 0) ? b : (float)Math.Round( (double)b / 255, 3),
			(a == 0) ? a : (float)Math.Round( (double)a / 255, 3)
		);

		return resultColor;
	}
    */
    protected Sprite GetItemICon(Goods good)
	{
		switch((GoodsType)good.u1Type)
		{
		case GoodsType.GOLD:
			return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.gold_3");
		case GoodsType.CASH:
			return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.cash_3");
		case GoodsType.KEY:
			return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.key_3");
		case GoodsType.LEAGUE_KEY:
			return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.key_3");
		case GoodsType.FRIENDSHIP_POINT:
			return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.friend_3");
		case GoodsType.MATERIAL:
			return AtlasMgr.Instance.GetSprite("Sprites/Item/Item_01." + ItemInfoMgr.Instance.GetMaterialItemInfo(good.u2ID).u2IconID);
		case GoodsType.CONSUME:
			if(good.u2ID >= 58006 && good.u2ID <= 58009)
				return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02." + good.u2ID);
			else
				return AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + good.u2ID);
		case GoodsType.EQUIP_COUPON:
			return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.equip_drawing");
		case GoodsType.MATERIAL_COUPON:
			return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.stuff_drawing");
		}

		return null;
	}
}
