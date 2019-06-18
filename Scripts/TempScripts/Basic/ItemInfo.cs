
using System;
using System.Collections.Generic;
using System.Text;

public class ItemInfo
{
    public UInt16 u2ID;
    public string sName;
	public string sDescription;

	public enum ITEM_ORDER
	{
		ACCESSORY_2 = 1,
		ACCESSORY_1 = 2,
		SHOES = 3,
		GLOVE = 4,
		SHOULDER = 5,
		HELMET = 6,
		PANTS = 7,
		CHEST = 8,
		WEAPON_L = 9,
		WEAPON_R = 10,
		ALL = 11,
		EQUIPMENT = 12,
		CONSUMABLE = 13,
		MATERIAL = 14,
		RUNE = 15,
		SWEEP = 16,
		DESIGN = 17,
		EVENT_GOODS = 18,
    }
	public ITEM_ORDER eOrder;

	public enum ITEM_TYPE
	{
		NONE = 0,
		EQUIPMENT = 1,
		MATERIAL = 2,
		CONSUMABLE = 3,
		RUNE = 4
	}

	public ITEM_TYPE ItemType
	{
		get
		{
			ITEM_TYPE ret = ITEM_TYPE.NONE;
			if(10000 <= u2ID && u2ID < 30000)
			{
				ret = ITEM_TYPE.EQUIPMENT;
			}
			else if(2000 < u2ID && u2ID <= 4000)
			{
				ret = ITEM_TYPE.MATERIAL;
			}
			else if(58000 < u2ID && u2ID <= 58200)
			{
				ret = ITEM_TYPE.CONSUMABLE;
			}
			return ret;
		}
	}

	public ItemInfo()
    {
    }
    
	public ItemInfo(UInt16 id, string name)
    {
        u2ID = id;
        sName = name;
    }

	

}