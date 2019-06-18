using System;
using System.Collections.Generic;
using System.Text;

public class SkillInfoMgr : Singleton<SkillInfoMgr>
{
	private Dictionary<UInt16, SkillInfo> dicData;
	
	private bool loadedInfo=false;
	readonly public float KnockBackTime = 0.5f;
	
	public bool LoadedInfo
	{
		get { return loadedInfo; }
		set { loadedInfo = value; }
	}
	
	public void AddInfo(string[] cols)
	{
		if (cols == null)
		{
			loadedInfo = true;
			return;
		}
		SkillInfo info = new SkillInfo();
		info.Set(cols);
		dicData.Add(info.u2ID, info);
	}
	
	public SkillInfo GetInfo(UInt16 id)
	{
		SkillInfo ret;
		dicData.TryGetValue(id, out ret);
		return ret;
	}

	public SkillInfo GetInfoBySlot(UInt16 classId, Byte slotId)
	{
		foreach (SkillInfo info in dicData.Values) {
			if (info.u2ClassID == classId &&
			    info.u1SlotNum == slotId) return info;
		}

		return null;
	}

	public List<SkillInfo> GetInfoListByClass(UInt16 classId)
	{
		List<SkillInfo> lst = new List<SkillInfo>();

		foreach (SkillInfo info in dicData.Values) {
			if (info.u2ClassID == classId) lst.Add(info);
		}
		
		return lst;
	}

	public void Init()
	{
		dicData = new Dictionary<UInt16, SkillInfo>();
		DataMgr.Instance.LoadTable(this.AddInfo, "Skill");
	}
}