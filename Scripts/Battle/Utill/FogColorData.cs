using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class FogInfo
{
	public string name;
	public Color color;
	public FogMode mode;
	public float density;
	public float start;
	public float end;
}


public class FogColorData : ScriptableObject {
	
	public FogInfo[] Fogs;
	
	public void AddInfo()
	{
		for(int i=0; i<Fogs.Length; i++)
		{        
			StageInfoMgr.Instance.AddFogInfo(i+1, Fogs[i]);
		}
	}
}
