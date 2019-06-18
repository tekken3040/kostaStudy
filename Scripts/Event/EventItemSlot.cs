using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class EventItemSlot : MonoBehaviour 
{
	public GameObject _objPlusIcon;
	public Image _imgItemIcon;
	public Text _txtItemInfo;

	public virtual void SetEventItem(int index, Goods info)
	{
		if(index == 0)
			_objPlusIcon.SetActive(false);
		else
			_objPlusIcon.SetActive(true);

		_imgItemIcon.sprite = GetItemICon(info);
		_imgItemIcon.SetNativeSize();
		_imgItemIcon.transform.localPosition = Vector3.zero;

		switch((GoodsType)info.u1Type)
		{
		case GoodsType.GOLD: case GoodsType.CASH: case GoodsType.KEY:
			_imgItemIcon.GetComponent<RectTransform>().sizeDelta *= 0.5f;
			break;
		}

		_txtItemInfo.color = GetTextColor(info.u1Type);
		string itmeName;
		if (info.u1Type == (byte)GoodsType.CONSUME) 
			itmeName = TextManager.Instance.GetText (ItemInfoMgr.Instance.GetConsumableItemInfo (info.u2ID).sName);
		else if(info.u1Type == (byte)GoodsType.MATERIAL)
			itmeName = TextManager.Instance.GetText (ItemInfoMgr.Instance.GetMaterialItemInfo (info.u2ID).sName);
		else 
			itmeName = Legion.Instance.GetConsumeString(info.u1Type);
		_txtItemInfo.text = itmeName + "\n" + info.u4Count;
	}

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
    protected virtual Sprite GetItemICon(Goods good)
    {
        switch ((GoodsType)good.u1Type)
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
                return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.forge_stuff_1");//AtlasMgr.Instance.GetSprite("Sprites/Item/Item_01." + ItemInfoMgr.Instance.GetMaterialItemInfo(good.u2ID).u2IconID);
            case GoodsType.CONSUME:
                if (good.u2ID == 58004)
                    return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02." + good.u2ID);
                if (good.u2ID >= 58001 && good.u2ID <= 58017)
                    return AtlasMgr.Instance.GetSprite("Sprites/Shop/shop_02.exp_potion_5");
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
