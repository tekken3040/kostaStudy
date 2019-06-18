
using System;
using System.Collections.Generic;
using System.Text;

public class Item : ZObject, IComparable<Item>
{
	public ItemInfo cItemInfo;
	public UInt16 u2SlotNum;
	public bool isNew;
    public Byte u1RoomNum;
    public Byte u1SeatNum;    

	#region IComparable implementation
	public int CompareTo (Item other)
	{
        if((int)cItemInfo.eOrder < 11 && (int)other.cItemInfo.eOrder < 11)
        {
            if(isNew && !other.isNew)
                return -1;
            else if(!isNew && other.isNew)
                return 1;    
            
            EquipmentItem eqiupItem = (EquipmentItem)this;
            EquipmentItem otherItem = (EquipmentItem)other;
            
            if(eqiupItem.attached.hero == null && otherItem.attached.hero != null)
                return 1;
            else if(eqiupItem.attached.hero != null && otherItem.attached.hero == null)
                return -1;
            
            if((int)cItemInfo.eOrder > (int)other.cItemInfo.eOrder)
                return -1;
            if((int)cItemInfo.eOrder < (int)other.cItemInfo.eOrder)
                return 1;
            else
                return 0;            
        }
        else if((int)cItemInfo.eOrder < 11 && (int)other.cItemInfo.eOrder > 11)
        {            
            return -1;
        }
        else if((int)cItemInfo.eOrder > 11 && (int)other.cItemInfo.eOrder > 11)
        {
            if(isNew && !other.isNew)
                return -1;
            else if(!isNew && other.isNew)
                return 1;                
            
            if((int)cItemInfo.eOrder < (int)other.cItemInfo.eOrder)
                return -1;
            if((int)cItemInfo.eOrder > (int)other.cItemInfo.eOrder)
                return 1;
            else
                return 0;
        }          
        return 0;    
	}
	#endregion
}