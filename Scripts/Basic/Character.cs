using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;

public class Character : ZObject
{
	public string sName;
	public Byte u1AssignedCrew;
    public bool bAssignedLeagueCrew = false;
	public int iIndexInCrew;
	public ClassInfo cClass;
	public Status cStatus
	{
        get { return GetComponent<StatusComponent>().STATUS; }
	}
	public Status cFinalStatus
	{
        get { return GetComponent<StatusComponent>().FINAL_STATUS; }
	}
	public Level cLevel
	{
		get { return GetComponent<LevelComponent>().cLevel; }
	}

	void init()
	{
		AddComponent<LevelComponent>(null);
		AddComponent<SkillComponent>(null);
		AddComponent<StatusComponent>(cClass.cStatus);
	}
	public Character(){}
    public Character(UInt16 classID, string name)
    {
        sName = name;
        cClass = ClassInfoMgr.Instance.GetInfo(classID);
		init();
	}

	public virtual void attachAnimator(GameObject cObject) { }
}