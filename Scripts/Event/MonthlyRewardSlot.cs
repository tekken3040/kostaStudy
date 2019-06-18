using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class MonthlyRewardSlot : MonoBehaviour
{
    //public Image BGImage;
    public Image Icon;
    public Image Disable;
    public Image Check;

    //public Text Number;
    public Text Value;
    public Text VipLv;
    public Text VipBonus;
    DailyCheckReward _dailyCheckReward;
    StringBuilder tempStringBuilder;
    bool _checkSlot = false;

    public void SetData(DailyCheckReward _reward, bool _check, int day)
    {
        Disable.enabled = false;
        Check.enabled = false;
        tempStringBuilder = new StringBuilder();
        //tempStringBuilder.Append(TextManager.Instance.GetText("btn_vip_info"));
        _dailyCheckReward = _reward;
        _checkSlot = _check;
        //tempStringBuilder.Remove(0, tempStringBuilder.Length);
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
		Icon.GetComponent<RectTransform>().sizeDelta *=0.4f;

        if(_checkSlot)
        {
            if(day == _dailyCheckReward.u1Day && Legion.Instance.u1RecvLoginReward == 1)
            {
                Disable.color = new Color(0, 0, 0, 0f);
                Check.color = new Color(1, 1, 1, 0);
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
            Disable.enabled = false;
            Check.enabled = false;
        }

		tempStringBuilder.Remove(0, tempStringBuilder.Length);
        Value.GetComponent<Text>().font = AssetMgr.Instance.AssetLoad("Fonts/NanumBarunGothicBold.ttf", typeof(Font)) as Font;
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
		tempStringBuilder.Append(" ").Append(_dailyCheckReward.u4RewardItemCount.ToString());
		Value.text = tempStringBuilder.ToString();
    }

    IEnumerator CheckAni()
    {
        yield return new WaitForSeconds(2.5f);
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
}
