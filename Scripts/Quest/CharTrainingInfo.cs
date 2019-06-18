using System;

public class TrainingInfo
{   
    public UInt16 u2ID;
    public Byte u1Step;
    public UInt16[] arrTrainingTime;
    //public UInt32[] arrExp;
	public UInt64[] arrExp;
    public Goods[] arrRewardGoods;
    
    public byte u1SeatCount;
    public byte u1SeatLockCount;
    public Goods[] arrUnlockGoods;
    
    public Goods doneGoods;

	public Goods[] arrTrainingCost;	// 훈련 비용
    
    //ServerData
    public Byte timeType = 0;
    public DateTime doneTime = DateTime.Now;

	public byte GetOpenedSlotCount(){
		return (byte)(u1SeatCount - u1SeatLockCount);
	}
}

public class CharTrainingInfo : TrainingInfo{
        
    const int MAX_TYPE = 7;
    const int MAX_UNLOCK_MATERIAL = 4;        
    
    public UInt16 Set(string[] cols)
	{        
        UInt16 idx = 0;
        u2ID = Convert.ToUInt16(cols[idx++]);
        idx++;
        u1Step = Convert.ToByte(cols[idx++]);
        
        arrTrainingTime = new UInt16[MAX_TYPE];
        arrExp = new UInt64[MAX_TYPE];
        arrRewardGoods = new Goods[MAX_TYPE];
        
        for(int i=0; i<MAX_TYPE; i++)
        {
            arrTrainingTime[i] = Convert.ToUInt16(cols[idx++]);
            arrExp[i] = Convert.ToUInt32(cols[idx++]);      
            arrRewardGoods[i] = new Goods(cols, ref idx);
        }
        
        u1SeatCount = Convert.ToByte(cols[idx++]);
        u1SeatLockCount = Convert.ToByte(cols[idx++]);
        
        arrUnlockGoods = new Goods[MAX_UNLOCK_MATERIAL];
        for(int i=0; i<MAX_UNLOCK_MATERIAL; i++)
        {
            arrUnlockGoods[i] = new Goods(cols, ref idx);
        }
        
        doneGoods = new Goods(cols, ref idx);

		++idx;// 초기 오픈 값

		// 훈련 비용 값 저장
		arrTrainingCost = new Goods[MAX_TYPE];
		for(int i = 0; i < MAX_TYPE; ++i)
		{
			arrTrainingCost[i] = new Goods(cols, ref idx);
		}
        
        return u2ID;
    }
}
