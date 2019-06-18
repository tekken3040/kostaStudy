using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum RuneType{
	AD = 1,
	AP = 2,
	DEF = 3,
	REG = 4,
	COOL = 5
}

public class RuneComponent {
	Crew cCrew;
	RuneInfo[] acRunes;

	int runeTypeCount = 2;
	Dictionary<int, int[]> dicRuneEffect = new Dictionary<int, int[]>();

	public RuneComponent()
	{
		acRunes = new RuneInfo[5];
	}
	public RuneComponent(Crew tCrew)
	{
		cCrew = tCrew;
		acRunes = new RuneInfo[5];
	}

	public RuneComponent(Crew tCrew, UInt16[] tRuneIds)
	{
		cCrew = tCrew;
		acRunes = new RuneInfo[5];
		for (int i=0; i<acRunes.Length; i++) {
			if(tRuneIds[i] > 0){
				RuneInfo temp = ItemInfoMgr.Instance.GetRuneInfo(tRuneIds[i]);
				acRunes[i] = temp;
			}else{
				acRunes[i] = null;
			}
		}
	}

	public void SetRunes(UInt16[] tRuneIds)
	{
		for (int i=0; i<acRunes.Length; i++) {
			if(tRuneIds[i] > 0){
				RuneInfo temp = ItemInfoMgr.Instance.GetRuneInfo(tRuneIds[i]);
				acRunes[i] = temp;
			}else{
				acRunes[i] = null;
			}
		}
	}

	public void PushRune(int u1Slot, UInt16 u2ID){

		if(acRunes[u1Slot] == null)
		{
			RuneInfo tempInfo = ItemInfoMgr.Instance.GetRuneInfo(u2ID);
			acRunes[u1Slot] = tempInfo;
		}

		if(acRunes[u1Slot].u2ID == u2ID) return;

		RuneInfo temp = ItemInfoMgr.Instance.GetRuneInfo(u2ID);
		acRunes[u1Slot] = temp;
	}

	public void PopRune(int u1Slot){
		acRunes[u1Slot] = null;
	}

	public UInt16 GetUsingRuneCnt(UInt16 u2ID){
		UInt16 usingCnt = 0;
		for (int i=0; i<acRunes.Length; i++) {
			if(acRunes[i] != null){
				if(acRunes[i].u2ID == u2ID) usingCnt++;
			}
		}

		return usingCnt;
	}

	public RuneInfo[] GetRuneInfo()
	{
		return acRunes;
	}

	public int GetTotalEffVal(byte u1Type, byte u1Increase){
		int key = runeTypeCount * (u1Type - 1) + u1Increase;

		if(dicRuneEffect.ContainsKey(key)) return dicRuneEffect[key][2];
		
		return 0;
	}

	public int GetTotalEffVal(RuneType eType, byte u1Increase){
		byte u1Type = (byte)eType;
		int key = runeTypeCount * (u1Type - 1) + u1Increase;

		if(dicRuneEffect.ContainsKey(key)) return dicRuneEffect[key][2];

		return 0;
	}

	public void InitTotalEffVal(){
		for (int i=0; i<acRunes.Length; i++)
		{
			if (acRunes [i] != null)
			{
				int key = runeTypeCount * (acRunes [i].u1Type - 1) + acRunes [i].u1IncreaseType;
				int[] value = new int[3];
			
				if (!dicRuneEffect.ContainsKey (key))
					dicRuneEffect.Add (key, value);
			
				value = dicRuneEffect [key];
			
				value [0] = acRunes [i].u1Type;
				value [1] = acRunes [i].u1IncreaseType;
				value [2] += acRunes [i].u2EffVal;
			
				if (value [2] >= acRunes [i].u2MaxEffVal)
					value [2] = acRunes [i].u2MaxEffVal;
			
				dicRuneEffect [key] = value;
			}
		}
	}
}

public class RuneItem {
	public RuneInfo cInfo;

	UInt32 u4Count;

	public RuneItem()
	{
		
	}
	public RuneItem(UInt16 id)
	{
		cInfo = ItemInfoMgr.Instance.GetRuneInfo(id);
	}
	public RuneItem(UInt16 id, UInt32 count)
	{
		cInfo = ItemInfoMgr.Instance.GetRuneInfo(id);
		u4Count = count;
	}

	public void AddCount(UInt32 cnt){
		u4Count += cnt;
	}

	public void SubCount(UInt32 cnt){
		u4Count -= cnt;
	}

	public int GetCount(bool bRemain){
		UInt16 usingCnt = 0;
		if (bRemain) {
			for(int i=0; i<Legion.Instance.acCrews.Length; i++){
				usingCnt += Legion.Instance.acCrews[i].cRuneComponent.GetUsingRuneCnt(cInfo.u2ID);
			}
		}

		return (int)(u4Count - usingCnt);
	}
}
