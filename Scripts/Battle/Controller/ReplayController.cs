using UnityEngine;
using System.Collections;
using System;

public class ReplayController : MonoBehaviour {

	UInt16[] au2UserInfo;
	UInt16 u2Field;

	UInt16 u2Seed;
	byte[] abRandom = new byte[5];
	BattleEvent[] acEvent;

	// Use this for initialization
	void Start () {
		u2Seed = 123;

		for(int i=0; i<10; i++){
			u2Seed = (ushort)(RandomGen(u2Seed));
			DebugMgr.Log("result="+u2Seed );
		}
	}

	UInt16 RandomGen(UInt16 seed) {
		UInt16 result = 0;
		byte add = 0;

		for(int i=0; i<abRandom.Length; i++){
			add = (byte)(seed >> i);
			DebugMgr.Log(add);
			if(result + add > UInt16.MaxValue) result = 0;
			result += add;
		}

		return result;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
