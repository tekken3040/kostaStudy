using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class WeeklyRewardSlot : MonoBehaviour
{
    //public Image BGImage;
    public Image Icon;
    public Image Disable;
    public Image Check;

    public Text _txtRewardDay;
    public Text _txtRewardName;
	public Text _txtRewardCount;
    public Text VipLv;
    public Text VipBonus;
	public Text _txtRewardCondition;
    DailyCheckReward _dailyCheckReward;
    StringBuilder tempStringBuilder;
    bool _checkSlot = false;

	public void SetData(DailyCheckReward _reward, bool _check, int day, int slotIdx)
    {
		Disable.enabled = false;
		Check.enabled = false;

		tempStringBuilder = new StringBuilder();

		// 날짜 셋팅
		tempStringBuilder.Append(slotIdx.ToString()).Append(TextManager.Instance.GetText("popup_title_reward_day"));
		_txtRewardDay.text = tempStringBuilder.ToString();

		//tempStringBuilder.Append(TextManager.Instance.GetText("btn_vip_info"));
        _dailyCheckReward = _reward;
        _checkSlot = _check;

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
		if(VipLv != null)
		{
			if(_reward.u4MultipleConditionCount > 0)
			{
				VipLv.transform.parent.gameObject.SetActive(true);
				tempStringBuilder.Append(TextManager.Instance.GetText("btn_vip_info")).Append(" ").Append(_reward.u4MultipleConditionCount);
		       	VipLv.text = tempStringBuilder.ToString();
        
				tempStringBuilder.Remove(0, tempStringBuilder.Length);
				tempStringBuilder.Append("x").Append(_reward.u1Multiple);
				VipBonus.text = tempStringBuilder.ToString();
			}
			else
			{
				VipLv.transform.parent.gameObject.SetActive(false);
			}
		}
		// 보상 아이콘 셋팅
		switch(_dailyCheckReward.u1RewardItemType)
		{
		case (Byte)GoodsType.GOLD:
			//Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_Gold");
			Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.gold_3");
			break;
		case (Byte)GoodsType.CASH:
			//Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_Cash");
			Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.cash_3");
			break;
		case (Byte)GoodsType.KEY:
			//Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_Key");
			Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.key_3");
			break;
		case (Byte)GoodsType.LEAGUE_KEY:
			//Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_Key");
			Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.key_3");
			break;
		case (Byte)GoodsType.FRIENDSHIP_POINT:
			//Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.icon_friendship");
			Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.friend_3");
			break;
		case (Byte)GoodsType.MATERIAL:
			Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + ItemInfoMgr.Instance.GetMaterialItemInfo(_dailyCheckReward.u2RewardItemID).u2IconID);
			break;
		case (Byte)GoodsType.CONSUME:
			//Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + _dailyCheckReward.u2RewardItemID);
			if(_dailyCheckReward.u2RewardItemID >= 58006 && _dailyCheckReward.u2RewardItemID <= 58009)
				Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02." + _dailyCheckReward.u2RewardItemID);
			else
				Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + _dailyCheckReward.u2RewardItemID);
			break;

		case (Byte)GoodsType.EQUIP_COUPON:
			//Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_01." + ShopInfoMgr.Instance.dicShopGoodData[_dailyCheckReward.u2RewardItemID].imagePath);
			Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.equip_drawing");
			break;
		case (Byte)GoodsType.MATERIAL_COUPON:
			//Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_01." + ShopInfoMgr.Instance.dicShopGoodData[_dailyCheckReward.u2RewardItemID].imagePath);
			Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.stuff_drawing");
			break;
		}
		Icon.SetNativeSize();
		Icon.GetComponent<RectTransform>().sizeDelta *= 0.4f;

        if(_checkSlot)
        {
			_txtRewardCondition.text = TextManager.Instance.GetText("mark_attend_done");
            if(day == _dailyCheckReward.u1Day && Legion.Instance.u1RecvLoginReward == 1)
            {
                Disable.color = new Color(0, 0, 0, 0);
                Check.color = new Color(1, 1, 1, 0);
                //Check.transform.localScale = new Vector3(1.75f, 1.75f, 1f);
                Check.transform.localScale = new Vector3(7f, 7f, 1f);
                Disable.enabled = true;
                Check.enabled = true;
                StartCoroutine(CheckAni());
            }
			else
			{
	            Disable.enabled = true;
	            Check.enabled = true;
			}
        }
        else
        {
			_txtRewardCondition.text = TextManager.Instance.GetText("mark_attend");
            Disable.enabled = false;
            Check.enabled = false;
        }

		// 아이템 정보 넣기
		tempStringBuilder.Remove(0, tempStringBuilder.Length);
		//switch(_dailyCheckReward.u1RewardItemType)
		//{
		//case (Byte)GoodsType.MATERIAL:
		//	tempStringBuilder.Append(TextManager.Instance.GetText(ItemInfoMgr.Instance.GetMaterialItemInfo(_dailyCheckReward.u2RewardItemID).sName));
		//	break;
		//case (Byte)GoodsType.EQUIP:
		//	tempStringBuilder.Append(TextManager.Instance.GetText(EquipmentInfoMgr.Instance.GetInfo(_dailyCheckReward.u2RewardItemID).sName));
		//	break;
		//case (Byte)GoodsType.CONSUME:
		//	tempStringBuilder.Append(TextManager.Instance.GetText(ItemInfoMgr.Instance.GetConsumableItemInfo(_dailyCheckReward.u2RewardItemID).sName));
		//	break;
		//case (Byte)GoodsType.MATERIAL_COUPON:
		//case (Byte)GoodsType.EQUIP_COUPON :
		//	tempStringBuilder.Append(TextManager.Instance.GetText(ShopInfoMgr.Instance.dicShopGoodData[_dailyCheckReward.u2RewardItemID].title));
		//	break;
		//default:
		//	tempStringBuilder.Append(Legion.Instance.GetConsumeString(_dailyCheckReward.u1RewardItemType));
		//	break;
		//}
		if(_dailyCheckReward.u1RewardItemType == (Byte)GoodsType.MATERIAL_COUPON || 
			_dailyCheckReward.u1RewardItemType == (Byte)GoodsType.EQUIP_COUPON)
		{
			tempStringBuilder.Append(TextManager.Instance.GetText(ShopInfoMgr.Instance.dicShopGoodData[_dailyCheckReward.u2RewardItemID].title));
		}
        else if(_dailyCheckReward.u1RewardItemType == (Byte)GoodsType.CASH)
        {
            tempStringBuilder.Append(TextManager.Instance.GetText("mark_cash"));
        }
        else if(_dailyCheckReward.u1RewardItemType == (Byte)GoodsType.GOLD)
        {
            tempStringBuilder.Append(TextManager.Instance.GetText("mark_gold"));
        }
        else if(_dailyCheckReward.u1RewardItemType == (Byte)GoodsType.KEY)
        {
            tempStringBuilder.Append(TextManager.Instance.GetText("mark_key"));
        }
        else if(_dailyCheckReward.u1RewardItemType == (Byte)GoodsType.LEAGUE_KEY)
        {
            tempStringBuilder.Append(TextManager.Instance.GetText("mark_leaguekey"));
        }
        else if(_dailyCheckReward.u1RewardItemType == (Byte)GoodsType.FRIENDSHIP_POINT)
        {
            tempStringBuilder.Append(TextManager.Instance.GetText("mark_friendshippoint"));
        }
		Color gradientColor = GetTextColor(_dailyCheckReward.u1RewardItemType);
        
		//if(slotIdx == 7)
		//{
		//	tempStringBuilder.Append("x ").Append(_dailyCheckReward.u4RewardItemCount.ToString());
		//	_txtRewardCount.gameObject.SetActive(false);
		//}
		//else
		//{
			_txtRewardCount.gameObject.SetActive(true);
			_txtRewardCount.text = _dailyCheckReward.u4RewardItemCount.ToString();
			_txtRewardCount.GetComponent<Gradient>().EndColor = GetTextColor(_dailyCheckReward.u1RewardItemType);
		//}

		_txtRewardName.GetComponent<Gradient>().EndColor = gradientColor;
		_txtRewardName.text = "";

		/*
		if(_dailyCheckReward.u1RewardItemType == (Byte)GoodsType.MATERIAL_COUPON)
		{
			tempStringBuilder.Append(TextManager.Instance.GetText("mark_material_gacha"));
			//Value.text = TextManager.Instance.GetText("mark_material_gacha");
		}
        else if(_dailyCheckReward.u1RewardItemType == (Byte)GoodsType.EQUIP_COUPON)
		{
			tempStringBuilder.Append(TextManager.Instance.GetText("mark_equipment_gacha"));
			//_txtRewardName.text = TextManager.Instance.GetText("mark_equipment_gacha");
		}
        else
		{
			tempStringBuilder.Append(ItemInfoMgr);
			//_txtRewardName.text = _dailyCheckReward.u4RewardItemCount.ToString();
		}
		*/
    }

    IEnumerator CheckAni()
    {
        yield return new WaitForSeconds(1f);
        this.transform.SetAsLastSibling();
		Check.color = new Color(1, 1, 1, 1);
        //LeanTween.scale(Check.GetComponent<RectTransform>(), new Vector3(7f, 7f, 7f), 0.1f).setOnComplete(
        //LeanTween.scale(Check.GetComponent<RectTransform>(), new Vector3(1f, 1f, 1f), 0.1f).setDelay(0.1f).onComplete);
        LeanTween.scale(Check.GetComponent<RectTransform>(), new Vector3(6f, 6f, 1f), 0.075f).
        setOnComplete(LeanTween.scale(Check.GetComponent<RectTransform>(), new Vector3(4.5f, 4.5f, 1f), 0.075f).setDelay(0.075f).onComplete).
        setOnComplete(LeanTween.scale(Check.GetComponent<RectTransform>(), new Vector3(2.5f, 2.5f, 1f), 0.075f).setDelay(0.075f).onComplete).
        setOnComplete(LeanTween.scale(Check.GetComponent<RectTransform>(), new Vector3(1f, 1f, 1f), 0.075f).setDelay(0.075f).onComplete);
        LeanTween.alpha(Disable.GetComponent<RectTransform>(), 0.5f, 0.2f);
    }

	protected virtual Color GetTextColor(Byte itmeType)
	{
		Color color;
		switch((GoodsType)itmeType)
		{
		case GoodsType.GOLD: case GoodsType.KEY:
			color = new Color32(253, 212, 85, 255);
			break;
		case GoodsType.CASH:
			color = new Color32(255, 159, 206, 255);
			break;
		default:
			color = new Color32(142, 245, 237, 255);
			break;
		}
		return color;
	}
    /*
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
}
