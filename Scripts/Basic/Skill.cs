using System;
using System.Collections.Generic;
using System.Text;


public class SkillNode
{
	public class Skill
	{
		public SkillInfo cInfo;
		public UInt16 u2Level;
	}

	public Skill cSkill;

	public bool bUnlocked; // false: Not Learn, true: Learn
	public Byte u1Active; // 0: Not Active, 1~6: UseSkill SlotNum


	public SkillNode(SkillInfo a)
	{
		cSkill = new Skill();
		cSkill.cInfo = a;
		bUnlocked = false;
	}

	public void Set(LearnedSkill cCodes)
	{
		cSkill.u2Level = cCodes.u2Level;
		bUnlocked = true;
		u1Active = cCodes.u1SlotNum;
	}

	public bool IsSkillActive
	{
		get	{return u1Active > 0;}
	}

	public bool IsActiveSkill
	{
		get
		{
			if (!bUnlocked) return false;
			return cSkill.cInfo.u1ActWay == 1;
		}
	}

	public void Unlock()
	{
		bUnlocked = true;
	}

	public void Select(Byte u1Slot)
	{
		if (!bUnlocked) return;
		u1Active = u1Slot;
	}

	public void Unactivate()
	{
		if (!bUnlocked) return;
		u1Active = 0;
	}

	public void Upgrade()
	{
		if (!bUnlocked) return;
		if(cSkill.u2Level < 99) cSkill.u2Level++;
	}
}

public class LearnedSkill
{
	public Byte u1SlotNum;
	public UInt16 u2Level;
	public Byte u1UseIndex;
}

public class SkillComponent : ZComponent
{
	public ClassInfo cClassInfo;
	public Dictionary<Byte, SkillNode> dicSkillInfo = new Dictionary<Byte, SkillNode>();
	public List<LearnedSkill> lstLearnInfo = new List<LearnedSkill> ();

	List<LearnedSkill> lstLearnedSkill;

	public string SelectSlot;

	public UInt16 SkillPoint;
	public Byte AddSkillPoint;
    public UInt16 VIPSkillPoint;
    public Byte ResetCount;

	bool bLoad = false;

    public void Reset()
    {
        ResetCount++;
		lstLearnInfo.Clear();
		//SkillPoint = (ushort)lstLearnedSkill.Count;
		SkillPoint = 0;
		SkillPoint += (ushort)((((Character)owner).cLevel.u2Level-1)*EquipmentInfoMgr.Instance.lvPerSkillPoint);
		SkillPoint += AddSkillPoint;
        SkillPoint += VIPSkillPoint;
    }

	public void SkillPointUp(UInt16 Level, Byte upPoint)
	{
		UInt16 SkillPt = 0;
		for (int i=0; i<lstLearnInfo.Count; i++)
			SkillPt += lstLearnInfo[i].u2Level;
	
		SkillPt += SkillPoint;

		Byte basicSkillPt = (Byte)lstLearnedSkill.Count;

		if (SkillPt < (Level - 1) * EquipmentInfoMgr.Instance.lvPerSkillPoint + AddSkillPoint + VIPSkillPoint + basicSkillPt)
        {
            for (int i=0; i<upPoint; i++) {
                {
                    SkillPoint += (ushort)(1*EquipmentInfoMgr.Instance.lvPerSkillPoint);
					VIPSkillPoint += (ushort)(LegionInfoMgr.Instance.GetCurrentVIPInfo().u1LvUpAddSkillPt);
                    SkillPoint += (ushort)(LegionInfoMgr.Instance.GetCurrentVIPInfo().u1LvUpAddSkillPt);
                }
            }
        }
	}

	public bool CheckLockSlot(Byte idx){
		if(SelectSlot.Substring(SelectSlot.Length-(idx+1), 1) == "1") return false;

		return true;
	}

	public void AddBuyPoint(Byte Pt){
		AddSkillPoint += Pt;
		SkillPoint += Pt;
	}

	public void SubSkillPoint(){
		if(SkillPoint > 0) SkillPoint--;
	}

	public UInt16 GetTotalPoint(){
		return SkillPoint;
	}

	public bool CheckResetPossible()
	{
		int usedPt = 0;
		for (int i = 0; i < lstLearnInfo.Count; i++) {
			usedPt += lstLearnInfo [i].u2Level;
		}
		foreach (SkillNode node in dicSkillInfo.Values) {
			if (node.cSkill.cInfo.u1UsedSlot > 0) {
				usedPt--;
			}
		}
		if (usedPt > 0)
			return true;
		
		return false;
	}

    public void OpenSelectSlot(Byte u1SelectSlot)
    {
		SelectSlot = SelectSlot.Remove(SkillInfo.MAX_USE_SLOT-u1SelectSlot,1);
		SelectSlot = SelectSlot.Insert(SkillInfo.MAX_USE_SLOT-u1SelectSlot,"1");
    }

	public override void init(Object param)
	{
		cClassInfo = ((Character)owner).cClass;
		SelectSlot = "000111000111";

		lstLearnInfo = new List<LearnedSkill>();
		lstLearnedSkill = new List<LearnedSkill> ();
		ResetCount = 0;
		SkillPoint = 0;

		if ((Character)owner is Hero) {
			for (int i=0; i<cClassInfo.acActiveSkills.Count; i++) {
				SkillInfo info = cClassInfo.acActiveSkills [i];
				SkillNode node = new SkillNode (info);
				dicSkillInfo.Add (info.u1SlotNum, node);
			}
			for (int i=0; i<cClassInfo.acPassiveSkills.Count; i++) {
				SkillInfo info = cClassInfo.acPassiveSkills [i];
				SkillNode node = new SkillNode (info);
				dicSkillInfo.Add (info.u1SlotNum, node);
			}
		}

		SetInitSkill ();
	}

	void SetInitSkill()
	{
		foreach (SkillNode node in dicSkillInfo.Values) {
			if (node.cSkill.cInfo.u1UsedSlot > 0) {
				LearnedSkill temp = new LearnedSkill ();
				temp.u1SlotNum = node.cSkill.cInfo.u1SlotNum;
				temp.u2Level = 1;
				temp.u1UseIndex = node.cSkill.cInfo.u1UsedSlot;
				lstLearnedSkill.Add (temp);
			}
		}
	}

	public List<LearnedSkill> GetInitSkill()
	{
		List<LearnedSkill> tempLearnedSkill = new List<LearnedSkill> ();

		foreach (SkillNode node in dicSkillInfo.Values) {
			if (node.cSkill.cInfo.u1UsedSlot > 0) {
				LearnedSkill temp = new LearnedSkill ();
				temp.u1SlotNum = node.cSkill.cInfo.u1SlotNum;
				temp.u2Level = 1;
				temp.u1UseIndex = node.cSkill.cInfo.u1UsedSlot;
				tempLearnedSkill.Add (temp);
			}
		}

		return tempLearnedSkill;
	}

	public void GetBattleSkill(out List<BattleSkill> lstcSkills)
	{
		if (owner is Hero) {
			Dictionary<Byte, Byte> EquipSkillPoint = new Dictionary<byte, byte> ();
			EquipSkillPoint = Legion.Instance.cInventory.GetEquipSkillPoint ((Hero)owner);

			Byte u1ActiveIndex = 0;
			lstcSkills = new List<BattleSkill> ();

			if (lstLearnInfo == null)
				return;

			lstLearnInfo.Sort (delegate(LearnedSkill x, LearnedSkill y) {
				return x.u1UseIndex.CompareTo (y.u1UseIndex);
			});

			for (int i = 0; i < lstLearnInfo.Count; i++) {
				if (lstLearnInfo [i].u1UseIndex > 0) {
					Byte Equipbonus = 0;
					if (EquipSkillPoint.ContainsKey (lstLearnInfo [i].u1SlotNum)) {
						Equipbonus = EquipSkillPoint [lstLearnInfo [i].u1SlotNum];
					}
						
					BattleSkill bs = new BattleSkill (dicSkillInfo [lstLearnInfo [i].u1SlotNum].cSkill, (UInt16)(Equipbonus+lstLearnInfo [i].u2Level));

					if (dicSkillInfo [lstLearnInfo [i].u1SlotNum].IsActiveSkill)
						bs.u1ActiveSkillIndex = u1ActiveIndex++;
					lstcSkills.Add (bs);
				}
			}
		} else {
			Byte u1ActiveIndex = 0;
			lstcSkills = new List<BattleSkill> ();

			if (lstLearnInfo == null)
				return;

			lstLearnInfo.Sort (delegate(LearnedSkill x, LearnedSkill y) {
				return x.u1UseIndex.CompareTo (y.u1UseIndex);
			});

			for (int i = 0; i < lstLearnInfo.Count; i++) {
				if (lstLearnInfo[i].u1UseIndex > 0)
				{
					BattleSkill bs = new BattleSkill(dicSkillInfo[lstLearnInfo[i].u1SlotNum].cSkill, 0);
					if (dicSkillInfo[lstLearnInfo[i].u1SlotNum].IsActiveSkill) bs.u1ActiveSkillIndex = u1ActiveIndex++;
					lstcSkills.Add(bs);
				}
			}
		}
	}

    public void LoadSkill(UInt16 u2SelectSlotOpenBits, List<LearnedSkill> _lstLearnInfo, Byte u1ResetCount, Byte u1BuyPoint, UInt16 u1VIPPoint)
	{
		SkillPoint = 0;
		UInt16 classID = ((Character)owner).cClass.u2ID;
		SelectSlot = Convert.ToString(u2SelectSlotOpenBits,2);
		AddSkillPoint = u1BuyPoint;
        VIPSkillPoint = u1VIPPoint;

		if (SelectSlot.Length < SkillInfo.MAX_USE_SLOT) {
			while(SelectSlot.Length < SkillInfo.MAX_USE_SLOT){
				SelectSlot = SelectSlot.Insert(0,"0");
			}
		}

		SkillPoint += (ushort)lstLearnedSkill.Count;

		if (_lstLearnInfo == null) {
			lstLearnInfo = new List<LearnedSkill> ();
		} else {
			lstLearnInfo = _lstLearnInfo;
		}

        ResetCount = u1ResetCount;
		SkillPoint += (ushort)((((Character)owner).cLevel.u2Level-1)*EquipmentInfoMgr.Instance.lvPerSkillPoint);
		SkillPoint += AddSkillPoint;
        SkillPoint += VIPSkillPoint;

		LoadHeroSkill(cClassInfo.acActiveSkills);
		LoadHeroSkill(cClassInfo.acPassiveSkills);
	}

	public void LeanSkill(LearnedSkill learn){
		dicSkillInfo[learn.u1SlotNum].Set(learn);
	}

	void LoadHeroSkill(List<SkillInfo> lstSkills){
		for (int i=0; i<lstSkills.Count; i++) {
			int idx = lstLearnInfo.FindIndex(cs => cs.u1SlotNum == lstSkills[i].u1SlotNum);
			if(idx > -1){
				dicSkillInfo[lstSkills[i].u1SlotNum].Set(lstLearnInfo[idx]);
				if(SkillPoint > 0) SkillPoint -= lstLearnInfo[idx].u2Level;
			}
		}
	}

	public Status GetPassiveStatus(){
		Dictionary<Byte, Byte> EquipSkillPoint = new Dictionary<byte, byte>();
		EquipSkillPoint = Legion.Instance.cInventory.GetEquipSkillPoint((Hero)owner);

		//Status result = new Status ();
        Status tempStatus = ((Hero)owner).cFinalStatus;
        Status result = new Status();
		foreach (LearnedSkill node in lstLearnInfo)
		{
			if (node.u1UseIndex > 0)
			{
				SkillInfo info = dicSkillInfo [node.u1SlotNum].cSkill.cInfo;
				if (info.u1ActWay == 2 && info.u1ActSituation == 1)
                {
					//result.Add (info.cPassive);
                    tempStatus.AddPercentDetail (info.cPassive);
					//ushort lv = dicSkillInfo [node.u1SlotNum].cSkill.u2Level;
                    ushort lv = node.u2Level;
					if (EquipSkillPoint.ContainsKey (node.u1SlotNum))
                    {
						lv += EquipSkillPoint [node.u1SlotNum];
					}
					//result.Add (info.cPassiveUp, lv);
                    result.Add(tempStatus.AddPercentDetail (info.cPassiveUp, lv));
				}
			}
		}
		return result;
	}

	//for Monster
	public void LoadAllSkill()
	{	
		LoadMonsterSkill (cClassInfo.acActiveSkills);
		LoadMonsterSkill (cClassInfo.acPassiveSkills);
	}

	void LoadMonsterSkill(List<SkillInfo> lstSkills){
		for (Byte i = 0; i < lstSkills.Count; i++) {
			SkillNode node = new SkillNode(lstSkills[i]);
			
			LearnedSkill temp = new LearnedSkill();
			temp.u1SlotNum = (Byte)(i+1);
			temp.u2Level = 1;
			temp.u1UseIndex = (Byte)(i+1);
			node.Set(temp);

			dicSkillInfo.Add ((Byte)(i+1), node);
			
			lstLearnInfo.Add(temp);
		}
	}

	public void Activate(Byte u1SlotNum)
	{
		LearnedSkill temp = new LearnedSkill ();
		temp.u2Level = 1;
		temp.u1SlotNum = u1SlotNum;
		temp.u1UseIndex = 0;
		lstLearnInfo.Add (temp);
	}

	public void Select(Byte u1SlotNum, Byte u1SelectSlot)
	{
		int before = lstLearnInfo.FindIndex (cs => cs.u1UseIndex == u1SelectSlot);
		if (before > -1) {
			lstLearnInfo[before].u1UseIndex = 0;

		}

		lstLearnInfo.Find(cs => cs.u1SlotNum == u1SlotNum).u1UseIndex = u1SelectSlot;
	}

	public void Upgrade(Byte u1SlotNum)
	{

	}

    public bool CheckHaveSkillPoint(Byte _skillPoint)
    {
        bool _havePoint = false;

        if(_skillPoint > 0)
            _havePoint = true;
        else
            _havePoint = false;

        return _havePoint;
    }
}