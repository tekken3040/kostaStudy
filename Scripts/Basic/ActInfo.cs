using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ActInfo {
	public enum ACT_TYPE
	{
		STAGE = 1,
		TOWER = 2,
		FOREST = 3
	}
	
	//Data
	public UInt16 u2ID;
	public string strName;
	public ActInfo.ACT_TYPE u1Mode;
	public Byte u1Number;
	public Byte u1OpenCondition;
	public Byte u1ConditionValue;
	public string strImagePath;
	
    //서버에서 관리되는 데이터

	public List<UInt16> lstChapterID =  new List<UInt16>();
	public Byte openState;
    public int ActIndex = 0;
    public ActInfo prevAct = null;
    public ActInfo nextAct = null;
	 
	public UInt16 Set(string[] cols)
	{
		lstChapterID.Clear();
		
		UInt16 idx = 0;
		
		u2ID = Convert.ToUInt16(cols[idx++]);
		idx++;
		strName = cols[idx++];
		u1Mode = (ActInfo.ACT_TYPE)Convert.ToByte(cols[idx++]);
		u1Number = Convert.ToByte(cols[idx++]);
		u1OpenCondition = Convert.ToByte(cols[idx++]);
		u1ConditionValue = Convert.ToByte(cols[idx++]);
		idx++;
		strImagePath = cols[idx++].Replace("/r", "");

		return u2ID;
	}

    public void SetLink(StageInfo[] prevStage)
    {
        ChapterInfo last = null;
        foreach (UInt16 chapterID in lstChapterID)
        {
            ChapterInfo info = StageInfoMgr.Instance.dicChapterData[chapterID];
            if (last != null)
            {
                info.prevChapter = last;
                last.nextChapter = info;
            }
            info.SetLink(prevStage);
            last = info;
        }
        if (last != null) last.IsLastChapterInAct = true;
    }
	
    // 열리는 조건에 따른 액트 열림 체크
	public bool CheckActOpen()
	{
		switch(u1OpenCondition)
		{
            //항상 열림
			case 1:
				return true;
				
            //레벨에 따라
			case 2:
			{
				if(Legion.Instance.TopLevel >= u1ConditionValue)
					return true;
				else
					return false;
			}
					
            //이전 액트가 클리어 됬을 경우        
			case 3:
			{
				if(prevAct.openState > 1)
					return true;
				else 
					return false;
			}
			
            //열리지 않음
			case 4:
				return false;
				
			default:
				return false;
		}
	}
}
