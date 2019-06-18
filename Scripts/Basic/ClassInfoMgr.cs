using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;
class ClassInfoMgr : Singleton<ClassInfoMgr>
{
	private Dictionary<UInt16, AttackModelInfo> dicAttackData;
	private Dictionary<UInt16, AttackSetInfo> dicAttackSetData;
	private Dictionary<UInt16, SkillAIInfo> dicSkillAIData;
	private Dictionary<UInt16, ClassInfo> dicData;
	private Dictionary<UInt16, PrecedenceInfo> dicPrecedenceData;
	private Dictionary<UInt16, UInt64> dicLevelUpData;
	private Dictionary<UInt16, UInt64> dicAccLevelUpData;
    private Dictionary<UInt16, Double> dicLevelCostData;
	private Dictionary<string, float> dicAnimLengthData;
	private bool loadedInfo = false;
	public bool LoadedInfo
	{
		get { return loadedInfo; }
		set { loadedInfo = value; }
	}

	public void AddPrecedenceInfo(string[] cols)
	{
		if (cols == null) return;
		
		PrecedenceInfo info = new PrecedenceInfo();
		info.Set(cols);
		dicPrecedenceData.Add(info.u2ID, info);
	}

	public PrecedenceInfo GetPrecedenceInfo(UInt16 id)
	{
		PrecedenceInfo ret;
		dicPrecedenceData.TryGetValue(id, out ret);
		return ret;
	}

	public void AddAttackInfo(string[] cols)
	{
		if (cols == null) return;
		AttackModelInfo info = new AttackModelInfo();
		info.Set(cols);
		dicAttackData.Add(info.u2ID, info);
	}

	public AttackModelInfo GetAttackInfo(UInt16 id)
	{
		AttackModelInfo ret;
		dicAttackData.TryGetValue(id, out ret);
		return ret;
	}

	public void AddAttackSetInfo(string[] cols)
	{
		if (cols == null) return;
		
		AttackSetInfo info = new AttackSetInfo();
		info.Set(cols);
		dicAttackSetData.Add(info.u2ID, info);
	}

	public AttackSetInfo GetAttackSetInfo(UInt16 id)
	{
		AttackSetInfo ret;
		dicAttackSetData.TryGetValue(id, out ret);
		return ret;
	}

	public void AddSkillAIInfo(string[] cols)
	{
		if (cols == null) return;

		SkillAIInfo info = new SkillAIInfo();
		info.Set(cols);
		dicSkillAIData.Add(info.u2ID, info);
	}

	public void AddLevelUpInfo(string[] cols)
	{
		if (cols == null) return;
		dicLevelUpData.Add(Convert.ToByte(cols[0]), Convert.ToUInt64(cols[1]));
		dicAccLevelUpData.Add(Convert.ToByte(cols[0]), Convert.ToUInt64(cols[2]));
        dicLevelCostData.Add(Convert.ToByte(cols[0]), Convert.ToDouble(cols[3]));
	}

	public UInt64 GetAccExp(UInt16 level)
	{
        if(level < 1)
            level = 1;
		return dicAccLevelUpData[level];
	}
	public UInt64 GetNextExp(UInt16 level)
	{
		return dicLevelUpData[level];
	}
    public Double GetCostFacter(UInt16 level)
    {
        return dicLevelCostData[level];
    }

	public SkillAIInfo GetSkillAIInfo(UInt16 id)
	{
		SkillAIInfo ret;
		dicSkillAIData.TryGetValue(id, out ret);
		return ret;
	}

	// 미사일 관련 프리팹은 Resources/Missile
	public GameObject GetMissileObject(AttackModelInfo attackModelInfo)
	{
		string resPath = "Effects/Missile/" + attackModelInfo.sMissileModelName + ".prefab";
		GameObject missileObj = GameObject.Instantiate(AssetMgr.Instance.AssetLoad(resPath, typeof(GameObject)) as GameObject) as GameObject;
		return missileObj;
	}
	public GameObject GetHitObject(AttackModelInfo attackModelInfo)
	{
		string resPath = "Effects/Hit/" + attackModelInfo.sMissileHitModelName + ".prefab";
		GameObject hitObj = GameObject.Instantiate(AssetMgr.Instance.AssetLoad(resPath, typeof(GameObject)) as GameObject) as GameObject;
		return hitObj;
	}
	public GameObject GetCrushObject(AttackModelInfo attackModelInfo)
	{
		string resPath = "Effects/Crush/" + attackModelInfo.sMissileCrushModelName + ".prefab";
		GameObject crushObj = GameObject.Instantiate(AssetMgr.Instance.AssetLoad(resPath, typeof(GameObject)) as GameObject) as GameObject;
		return crushObj;
	}

	public void AddClassInfo(string[] cols)
	{
		if (cols == null) return;

		ClassInfo info = new ClassInfo();
		info.Set(cols);
		dicData.Add(info.u2ID, info);
	}
	public void AddMonsterInfo(string[] cols)
	{
		if (cols == null)
		{
			loadedInfo = true;
			return;
		}
		ClassInfo info = new ClassInfo();
		info.SetMonster(cols);
		dicData.Add(info.u2ID, info);
	}

	private void AddFaceInfo(string[] cols)
	{
		if (cols == null) return;
		FaceInfo faceInfo = new FaceInfo();
		faceInfo.Set(cols);

		UInt16 classID = faceInfo.u2ClassID;
		ClassInfo classInfo;
		if (dicData.TryGetValue(classID, out classInfo))
		{
			// DebugMgr.Log("addFaceInfo : " + classInfo.u2ID + "_" + faceInfo.u1Element);
			if(faceInfo.u1Element == 1)
			{
				classInfo.AddFace(faceInfo);
			}
			else
			{
				classInfo.AddHair(faceInfo);
			}
		}
	}

	private void AddHairColor(string[] cols)
	{
		if (cols == null) return;
		HairColorInfo hairColorInfo = new HairColorInfo();
		hairColorInfo.Set(cols);

		UInt16 classID = hairColorInfo.u2ClassID;
		ClassInfo classInfo;
		if (dicData.TryGetValue(classID, out classInfo))
		{
			// DebugMgr.Log("addFaceInfo : " + classInfo.u2ID + "_" + faceInfo.u1Element);
			classInfo.AddHairColor(hairColorInfo);
		}

	}

	public ClassInfo GetInfo(UInt16 id)
	{
		ClassInfo ret;
		dicData.TryGetValue(id, out ret);
		return ret;
	}

	public List<KeyValuePair<UInt16, ClassInfo>> GetSkillGroup(UInt16 id)
	{
		return dicData.Where(cs => cs.Value.u2SkillGroup == id).ToList();
	}

	public Dictionary<UInt16, ClassInfo> GetClassListInfo()
	{
		return dicData;
	}

	public int GetCount()
	{
		return dicData.Count;
	}

	public void AddAnimLengthInfo(string[] cols)
	{
		if (cols == null) return;
		dicAnimLengthData.Add(cols[0], (float)Convert.ToDouble(cols[1]));
	}

	public float GetAnimLength(string animName)
	{
		if (dicAnimLengthData.ContainsKey (animName)) {
			return dicAnimLengthData[animName];
		}

		DebugMgr.LogError ("can't find aniName"); 

		return 2.0f;
	}

	public void Init()
	{
		dicLevelUpData = new Dictionary<UInt16, UInt64>();
		dicAccLevelUpData = new Dictionary<UInt16, UInt64>();
        dicLevelCostData = new Dictionary<UInt16, Double>();
		DataMgr.Instance.LoadTable(this.AddLevelUpInfo, "ClassLevelUp");
		dicAttackData = new Dictionary<UInt16, AttackModelInfo>();
		DataMgr.Instance.LoadTable(this.AddAttackInfo, "Attack");
		dicAttackSetData = new Dictionary<UInt16, AttackSetInfo>();
		DataMgr.Instance.LoadTable(this.AddAttackSetInfo, "AttackSet");

		dicSkillAIData = new Dictionary<UInt16, SkillAIInfo>();
		DataMgr.Instance.LoadTable(this.AddSkillAIInfo, "SkillAI");

		dicPrecedenceData = new Dictionary<UInt16, PrecedenceInfo>();
		DataMgr.Instance.LoadTable(this.AddPrecedenceInfo, "Precedence");

		dicData = new Dictionary<UInt16, ClassInfo>();
		DataMgr.Instance.LoadTable(this.AddClassInfo, "Class");
		DataMgr.Instance.LoadTable(this.AddMonsterInfo, "Monster");
		DataMgr.Instance.LoadTable(this.AddFaceInfo, "Face");
		DataMgr.Instance.LoadTable(this.AddHairColor, "HairColor");

		dicAnimLengthData = new Dictionary<string, float> ();
		DataMgr.Instance.LoadTable (this.AddAnimLengthInfo, "AnimationLength");
	}

}

