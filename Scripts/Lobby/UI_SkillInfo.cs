using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UI_SkillInfo : MonoBehaviour {
	ushort u2ClassID = 1;
	UInt16 u2SkillPoint;

	Transform LineParent;
	Transform IconParent;

	List<SkillInfo> lstSkills = new List<SkillInfo>();
	Dictionary<byte, bool> dicLocked = new Dictionary<byte, bool>();
	List<LearnedSkill> lstLearnInfo = new List<LearnedSkill>();
	List<LearnedSkill> lstChangedInfo = new List<LearnedSkill>();

	Hero cSelectedHero;
	SkillComponent cHeroSkills;

	GameObject objTree;

	Image ElementBG;
	Image ElementBorder;
	Image ElementIcon;

	class SkillIcon{
		public RectTransform tr;
		public Image Icon;
		private GameObject Circle;
		private Image ElementBorder;
		private Image Element;
		private Text Level;
		private GameObject Use;
		private Text SkillType;

		Color colorActive = new Color32(255,175,175,255);
		Color colorPassive = new Color32(150,210,255,255);

		public SkillIcon(GameObject pObj){
			tr = pObj.GetComponent<RectTransform>();
			Icon = pObj.GetComponent<Image> ();
			Circle = pObj.transform.FindChild ("Circle").gameObject;
			ElementBorder = pObj.transform.FindChild ("ElementBorder").GetComponent<Image> ();
			Element = pObj.transform.FindChild ("SkillElement").GetComponent<Image> ();
			Level = pObj.transform.FindChild ("Level").GetComponent<Text> ();
			Use = pObj.transform.FindChild ("Use").gameObject;
			SkillType = pObj.transform.FindChild ("Type").GetComponent<Text> ();
		}

		public void SetIcon(UInt16 classID, UInt16 skillID, Byte u1Type){
			Icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Skill/Atlas_SkillIcon_"+classID+"."+skillID);
			if (u1Type == 1) {
				SkillType.text = TextManager.Instance.GetText ("type_active");
				SkillType.color = colorActive;
			}else if (u1Type == 2) {
				SkillType.text = TextManager.Instance.GetText ("type_passive");
				SkillType.color = colorPassive;
			}
		}

		public void SetElement(Byte ele){
			Element.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.element_" + ele);
			ElementBorder.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.common_02_skill_element_"+ele);
		}

		public void SetLevel(UInt16 level){
			if (level > 0)
				Level.text = level.ToString ();
			else
				Level.text = "";
		}

		public void SetUse(bool b){
			Use.SetActive (b);
		}

		public void ActiveCircle(bool b){
			Circle.SetActive (b);
		}
	}

	Dictionary<Byte, SkillIcon> lstIcons = new Dictionary<Byte, SkillIcon>();
	Dictionary<string, Image> dicLines = new Dictionary<string, Image>();

	GameObject objIcon;
	GameObject objLock;
	GameObject objLine;

	float page2X = -365f;

	Text TextHeroSP;
	GameObject PointMass;

	//Info
	RectTransform InfoObject;
	Image ImgSkillIcon;
	Image ImgSkillEle;
	Image ImageEleIcon;
	Text TextSkillLevel;
	Text TextSkillName;
	Text TextSkillType;
	Text TextSkillLimit;
	Text TextSkillComment;

	GameObject BtnUse;
	GameObject BtnUnuse;
	GameObject BtnGoLearn;
	GameObject BtnGoUpgrade;

	//Use
	Transform UseObject;

	List<UI_SkillSelectIcon> lstSelectSkills = new List<UI_SkillSelectIcon>();

	Byte u1BeforeSlot = 0;

	enum SkillWindowState{
		Idle,
		Select,
		Setting,
		Learn,
		Upgrade
	}

    StringBuilder tempStringBuilder; //스트링 출력용 스트링 빌더

	SkillWindowState eState = SkillWindowState.Idle;

	LearnedSkill cCurLearnedSkill;
	SkillInfo cCurSkillInfo;

	Byte Element;
	Byte u1UnlockIdx;

	Vector3 IconModify = new Vector3(-10,7,0);
	List<GameObject> lstLockIcons = new List<GameObject>();

	GameObject objPointBuyPop;
	GameObject objRecommandPop;
	GameObject objResetPop;
	GameObject objSlotBuyPop;
	GameObject objLearnPop;
	GameObject objUpgradePop;

	Byte u1AddPoint;
	int addPrice;

	GameObject objPtBuyEff;
	GameObject objLvUpEff;
	GameObject objActiveEff;
	GameObject objUnlockEff;

	public GameObject HavePointEff;

	bool bSetting = false;

	Color colorlimitRed = new Color32(230,45,45,255);
	Color colorlimitGreen = new Color32(40,200,0,255);

	GameObject curPopup;
    //캐릭터 세부정보 패널
    public GameObject objCharInfoPanel;

	Dictionary<Byte, Byte> EquipSkillPoint = new Dictionary<byte, byte>();

	void Awake(){
		ElementBG = transform.GetComponent<Image>();
		LineParent = transform.FindChild("Panel").FindChild("ScrollObject").FindChild("Lines");
		IconParent = transform.FindChild("Panel").FindChild("ScrollObject").FindChild("Icons");

		ElementBorder = transform.FindChild("Panel").FindChild("ScrollObject").FindChild("ElementBorder").GetComponent<Image>();
		ElementIcon = transform.FindChild("Panel").FindChild("ScrollObject").FindChild("Element").GetComponent<Image>();

		objIcon = (GameObject)AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Skill/Pref_UI_Main_SkillIcon.prefab", typeof(GameObject));
		objLock = (GameObject)AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Skill/Pref_UI_Main_SkillLock.prefab", typeof(GameObject));
		objLine = (GameObject)AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Skill/Pref_UI_Main_SkillLine.prefab", typeof(GameObject));

		objPointBuyPop = (GameObject)AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Skill/Pref_UI_PointBuy.prefab", typeof(GameObject));
		objRecommandPop = (GameObject)AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Skill/Pref_UI_RecSkill.prefab", typeof(GameObject));
		objResetPop = (GameObject)AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Skill/Pref_UI_Reset.prefab", typeof(GameObject));
		objSlotBuyPop = (GameObject)AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Skill/Pref_UI_SlotBuy.prefab", typeof(GameObject));
		objLearnPop = (GameObject)AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Skill/Pref_UI_SkillLearn.prefab", typeof(GameObject));
		objUpgradePop = (GameObject)AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Skill/Pref_UI_SkillUpgrade.prefab", typeof(GameObject));

		objPtBuyEff = (GameObject)AssetMgr.Instance.AssetLoad("Prefabs/UI_Effects/Eff_UI_Battle_levelup.prefab", typeof(GameObject));
		objLvUpEff = (GameObject)AssetMgr.Instance.AssetLoad("Prefabs/UI_Effects/Eff_UI_Skill_level_up.prefab", typeof(GameObject));
		objActiveEff = (GameObject)AssetMgr.Instance.AssetLoad("Prefabs/UI_Effects/Eff_UI_Skill_Point.prefab", typeof(GameObject));
		objUnlockEff = (GameObject)AssetMgr.Instance.AssetLoad("Prefabs/UI_Effects/UI_Eff_Stage_Map.prefab", typeof(GameObject));

        PointMass = transform.FindChild("PointLabel").gameObject;
        TextHeroSP = PointMass.transform.FindChild("TextPoint").GetComponent<Text>();
		
		//Info
		InfoObject = transform.FindChild ("SkillInfo").GetComponent<RectTransform>();
		ImgSkillIcon = InfoObject.FindChild("Icon").GetComponent<Image>();
		ImgSkillEle = InfoObject.FindChild("SkillElement").GetComponent<Image>();
		ImageEleIcon = InfoObject.FindChild("ElementIcon").GetComponent<Image>();
		TextSkillLevel = InfoObject.FindChild("TextLevel").GetComponent<Text>();
		TextSkillName = InfoObject.FindChild("TextName").GetComponent<Text>();
		TextSkillType = InfoObject.FindChild("TextType").GetComponent<Text>();
		TextSkillLimit = InfoObject.FindChild("TextLimit").GetComponent<Text>();
		TextSkillComment = InfoObject.FindChild("TextComment").GetComponent<Text>();

		BtnUse = InfoObject.FindChild ("BtnUse").gameObject;
		BtnUnuse = InfoObject.FindChild ("BtnUnuse").gameObject;
		BtnGoLearn = InfoObject.FindChild ("BtnLearn").gameObject;
		BtnGoUpgrade = InfoObject.FindChild ("BtnUpgrade").gameObject;

		InfoObject.gameObject.SetActive(false);
	
		//Use
		UseObject = transform.FindChild ("UseSkill");

		for (int i=0; i<6; i++) {
			Transform temp = UseObject.FindChild("Actives").FindChild((i+1).ToString());
			temp.gameObject.AddComponent<TutorialButton>().id = "Active"+(i+1);
			lstSelectSkills.Add(temp.gameObject.AddComponent<UI_SkillSelectIcon>());
		}
		for (int i=0; i<6; i++) {
			Transform temp = UseObject.FindChild("Passives").FindChild((i+1).ToString());
			temp.gameObject.AddComponent<TutorialButton>().id = "Passive"+(i+1);
			lstSelectSkills.Add(temp.gameObject.AddComponent<UI_SkillSelectIcon>());
		}
	}

    void OnEnable()
    {
        tempStringBuilder = new StringBuilder();
    }

	public void SetHero(Hero cHero){
		cHero.cObject.SetActive(false);

		EquipSkillPoint.Clear ();

		if (cSelectedHero == cHero) {
			EquipSkillPoint = Legion.Instance.cInventory.GetEquipSkillPoint(cHero);
			SetElement(true);
			CheckPoint();
			return;
		}

		InitData(true);
        lstLearnInfo = new List<LearnedSkill>();
		lstSkills.Clear();

		cSelectedHero = cHero;
		cHeroSkills = cSelectedHero.GetComponent<SkillComponent>();
		lstLearnInfo = cHeroSkills.lstLearnInfo;

		u2ClassID = cSelectedHero.cClass.u2ID;
		lstSkills.AddRange(cSelectedHero.cClass.acActiveSkills);
		lstSkills.AddRange(cSelectedHero.cClass.acPassiveSkills);

		bSetting = false;

		EquipSkillPoint = Legion.Instance.cInventory.GetEquipSkillPoint(cHero);
		SetElement(false);
		ViewSkills();
		ViewUseSkills();
		CheckPoint();
		SetUse();
        

//		foreach (KeyValuePair<Byte, Byte> tmp in EquipSkillPoint) {
//			DebugMgr.LogError(tmp.Key+"/"+tmp.Value);
//		}
	}

	void InitData(bool bInit){
		Destroy(objTree);
		
		lstChangedInfo.Clear();
		lstIcons.Clear();
		dicLocked.Clear();
		
		for(int i=0; i<lstSelectSkills.Count; i++){
			lstSelectSkills[i].UnUse();
			if(bInit)lstSelectSkills[i].GetButton().onClick.RemoveAllListeners();
		}

		for (int i=0; i<LineParent.childCount; i++) {
			Destroy(LineParent.GetChild(i).gameObject);
		}

		dicLines.Clear();
        u1BeforeSlot = 0;
	}

	void SetElement(bool equal){
		Byte before = Element;
		Element = cSelectedHero.GetHeroElement();
        //		if (equal) {
        //			if (before != Element) {
        //
        //			}
        //		}
        if (equal == true && before != Element)
        {
            for (int i = 0; i < lstSkills.Count; i++)
            {
                if (lstSkills[i].u1Element == 5)
                    lstIcons[lstSkills[i].u1SlotNum].SetElement(Element);
                else
                    lstIcons[lstSkills[i].u1SlotNum].SetElement(lstSkills[i].u1Element);
            }
        }
        ElementBG.sprite = AtlasMgr.Instance.GetSprite("Sprites/SkillBG/BG_Element_" + Element + ".BG_Element_" + Element);
        ElementBorder.sprite = AtlasMgr.Instance.GetSprite("Sprites/SkillBG/skill_01.skill_ele_" + Element);
        ElementIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_icon_" + Element);
        ElementIcon.SetNativeSize();
	}

//	void ChangeElementSKillSlot(){
//		foreach (KeyValuePair< in lstIcons) {
//			if (info.u1Element == 5) {
//				tempIcon.transform.FindChild ("SkillElement").GetComponent<Image> ().sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.element_" + Element);
//				tempIcon.transform.FindChild ("ElementBorder").GetComponent<Image> ().sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.common_02_skill_element_" + Element);
//			}
//		}
//
//		ViewUseSkills ();
//	}

	void ViewUseSkills(){
		for (int i=0; i<SkillInfo.MAX_USE_SLOT; i++) {
			int cnt = i+1;
			bool bLock = cHeroSkills.CheckLockSlot((Byte)i);

			LearnedSkill skill = lstLearnInfo.Find(cs => cs.u1UseIndex == (i+1));
//			DebugMgr.LogError(cnt+":"+bLock);
			if(skill != null){
				SkillInfo info = lstSkills.Find(cs => cs.u1SlotNum == skill.u1SlotNum);
				lstSelectSkills[i].Init(bLock, (byte)(i+1), u2ClassID, info.u2ID, skill.u2Level, info.u1Element == 5 ? Element : info.u1Element);
			}else{
				lstSelectSkills[i].Init(bLock, (byte)(i+1), u2ClassID, 0, 0, 0);
			}
			lstSelectSkills[i].GetButton().onClick.AddListener(() => { ClickUseSkill((byte)cnt); }); 
		}
	}

	void ViewSkills(){
		for (int i=0; i<lstLockIcons.Count; i++) {
			Destroy(lstLockIcons[i]);
		}
		lstLockIcons.Clear();

		objTree = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Skill/Pref_UI_Main_SkillTree"+cSelectedHero.cClass.u1SkillTree + ".prefab", typeof(GameObject))) as GameObject;
		objTree.transform.SetParent(IconParent);
		objTree.transform.localScale = Vector3.one;
		objTree.transform.localPosition = Vector3.zero;
		objTree.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

		for(int i=0; i<lstSkills.Count; i++){
			int cnt = i;
			bool bLock = true;
			int idx = lstLearnInfo.FindIndex(cs => cs.u1SlotNum == lstSkills[i].u1SlotNum);
			if(idx > -1){
				bLock = false;
				CreateIcon(lstSkills[i], lstSkills[cnt].u1SlotNum, lstLearnInfo[idx].u2Level, false);
			}else{
				if(lstSkills[cnt].bOpen){
					bLock = false;
				}else{
					for(int j=0; j<lstSkills[i].au1LinkSlot.Length; j++){
						idx = lstLearnInfo.FindIndex(cs => cs.u1SlotNum == lstSkills[i].au1LinkSlot[j]);
						if (idx > -1) {
							if (!lstIcons.ContainsKey (lstSkills [i].u1SlotNum)) {
								bLock = false;
							}
						}
					}
				}
				CreateIcon (lstSkills [i], lstSkills [cnt].u1SlotNum, 0, true);
			}
			dicLocked.Add(lstSkills [cnt].u1SlotNum, bLock);
		}

		DrawLine();

		bSetting = true;
	}

	void CreateIcon(SkillInfo info, Byte slotNum, UInt16 level, bool bGray){
		GameObject tempIcon = Instantiate(objIcon) as GameObject;
		tempIcon.transform.SetParent(objTree.transform.FindChild("Skill"+info.u1SlotNum));
		tempIcon.gameObject.AddComponent<TutorialButton>().id = "Skill_"+info.u1SlotNum;

		SkillIcon si = new SkillIcon (tempIcon);
		si.SetIcon (u2ClassID, info.u2ID, info.u1ActWay);

		if (info.u1Element == 5) {
			si.SetElement (Element);
		} else {
			si.SetElement (info.u1Element);
		}
		
		si.SetLevel(level);

		int index = lstLearnInfo.FindIndex (cs => cs.u1SlotNum == slotNum);

		if (index > -1) {
			if (lstLearnInfo [index].u1UseIndex > 0) {
				si.SetUse (true);
			}
		}

		if (bGray) {
			AtlasMgr.Instance.SetGrayScale (si.Icon);
			if(bSetting){
				GameObject tempObj = Instantiate(objUnlockEff) as GameObject;
				tempObj.transform.parent = tempIcon.transform;
				tempObj.transform.localScale = Vector3.one;
				tempObj.transform.localPosition = Vector3.zero;
				Destroy(tempObj, 2f);
			}
		}

		tempIcon.transform.localScale = Vector3.one;
		tempIcon.transform.localPosition = IconModify;
		
		lstIcons.Add(info.u1SlotNum, si);
		
		tempIcon.GetComponent<Button>().onClick.AddListener(() => { SetCurSkillInfo(slotNum); }); 
	}

	void DrawLine(){
		for (int i=0; i<lstSkills.Count; i++) {
			for(int j=0; j<lstSkills[i].au1LinkSlot.Length; j++){
				if(lstSkills[i].au1LinkSlot[j] > 0){
					string lineKey = "";
					if(lstSkills[i].u1SlotNum < lstSkills[i].au1LinkSlot[j]) lineKey = lstSkills[i].u1SlotNum+"-"+lstSkills[i].au1LinkSlot[j];
					else lineKey = lstSkills[i].au1LinkSlot[j]+"-"+lstSkills[i].u1SlotNum;

					if(!dicLines.ContainsKey(lineKey)){
						Vector3 linePos = objTree.transform.FindChild("Skill"+lstSkills[i].u1SlotNum).localPosition+IconModify;
						Vector3 targetPos = objTree.transform.FindChild("Skill"+lstSkills[i].au1LinkSlot[j]).localPosition+IconModify;

						GameObject tempLine = Instantiate(objLine) as GameObject;
						Transform activeLine = tempLine.transform.FindChild("Active");
						tempLine.transform.SetParent(LineParent);
						tempLine.transform.localScale = Vector3.one;

						Image lineImg = activeLine.GetComponent<Image>();

						if(lstLearnInfo.FindIndex(cs => cs.u1SlotNum == lstSkills[i].u1SlotNum) > -1
						   || lstLearnInfo.FindIndex(cs => cs.u1SlotNum == lstSkills[i].au1LinkSlot[j]) > -1){
							lineImg.enabled = true;
						}

						Vector3 dir;
						Quaternion rot;
						if(lstSkills[i].u1SlotNum < lstSkills[i].au1LinkSlot[j]){
							dir = targetPos - linePos;
							if(dir.x == 0) rot = Quaternion.Euler(0,0,-90f);
							else rot = Quaternion.LookRotation(dir, Vector3.up);
							tempLine.transform.localPosition = linePos;
						}else{
							dir = linePos - targetPos;
							if(dir.x == 0) rot = Quaternion.Euler(0,0,-90f);
							else rot = Quaternion.LookRotation(dir, Vector3.up);
							tempLine.transform.localPosition = targetPos;
						}

						tempLine.transform.localRotation = new Quaternion(0,0,rot.z,rot.w);
						RectTransform rt = tempLine.GetComponent<RectTransform>();
						rt.sizeDelta = new Vector2(Vector3.Magnitude(dir), rt.sizeDelta.y);
						RectTransform rt2 = activeLine.GetComponent<RectTransform>();
						rt2.sizeDelta = new Vector2(Vector3.Magnitude(dir), rt2.sizeDelta.y);

						dicLines.Add(lineKey, lineImg);
					}
				}
			}
		}
	}

	bool CheckNeedLevel(Byte SlotNum){
		if (lstSkills.Find (cs => cs.u1SlotNum == SlotNum).u2NeedLevel > cSelectedHero.cLevel.u2Level)
			return false;

		return true;
	}

	public void SetCurSkillInfo(Byte SlotNum){
		int slotIndex = lstLearnInfo.FindIndex (cs => cs.u1SlotNum == SlotNum);
		UInt16 Level = 0;
		DisableSettingEff();

		if (slotIndex < 0) {
			BtnGoUpgrade.SetActive (false);
			BtnGoLearn.SetActive (true);

			bool bLock = dicLocked [SlotNum];

			if(cHeroSkills.GetTotalPoint() <= 0 || !CheckNeedLevel(SlotNum) || bLock == true){
				SetButtonActive(BtnGoLearn, false);
			}else{
				SetButtonActive(BtnGoLearn, true);
			}

			BtnUse.SetActive(false);
			BtnUnuse.SetActive(false);
			TextSkillLevel.text = "";

			//InfoObject.localPosition = new Vector3(-(InfoObject.sizeDelta.x*0.5f) ,InfoObject.localPosition.y, InfoObject.localPosition.z);
			InfoObject.anchoredPosition3D = new Vector3(-(InfoObject.sizeDelta.x*0.5f) ,InfoObject.localPosition.y, InfoObject.localPosition.z);
			UseObject.gameObject.SetActive(false);
		}else{
			BtnGoLearn.SetActive (false);
			BtnGoUpgrade.SetActive (true);

			Level = lstLearnInfo.Find (cs => cs.u1SlotNum == SlotNum).u2Level;

			if(cHeroSkills.GetTotalPoint() <= 0)
			{
				SetButtonActive(BtnGoUpgrade, false);
			}
			else
			{
				if(Level >= EquipmentInfoMgr.Instance.skillMaxPoint)
					SetButtonActive(BtnGoUpgrade, false);					
				else
					SetButtonActive(BtnGoUpgrade, true);
			}

			if (lstLearnInfo[slotIndex].u1UseIndex > 0){
				BtnUse.SetActive(false);
				BtnUnuse.SetActive(true);
				UseObject.gameObject.SetActive(true);
				lstSelectSkills[lstLearnInfo[slotIndex].u1UseIndex-1].SetSelectEff(true);
				//InfoObject.localPosition = new Vector3(640, InfoObject.localPosition.y, InfoObject.localPosition.z);
				InfoObject.anchoredPosition3D = new Vector3(-(InfoObject.sizeDelta.x * 0.5f) + page2X ,InfoObject.localPosition.y, InfoObject.localPosition.z);
			}else{
				BtnUse.SetActive(true);
				BtnUnuse.SetActive(false);
				//InfoObject.localPosition = new Vector3(-(InfoObject.sizeDelta.x*0.5f) ,InfoObject.localPosition.y, InfoObject.localPosition.z);
				InfoObject.anchoredPosition3D = new Vector3(-(InfoObject.sizeDelta.x * 0.5f) ,InfoObject.localPosition.y, InfoObject.localPosition.z);
				UseObject.gameObject.SetActive(false);
			}
		}
		SetUseInfo(SlotNum, Level);
	}

	void SetUseInfo(Byte SlotNum, UInt16 Level){
		InfoObject.gameObject.SetActive(true);
		SkillInfo info = lstSkills.Find (cs => cs.u1SlotNum == SlotNum);
		ImgSkillIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Skill/Atlas_SkillIcon_"+u2ClassID+"."+info.u2ID);
		ImgSkillEle.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.common_02_skill_element_"+(info.u1Element == 5 ? Element : info.u1Element));
		ImageEleIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_"+(info.u1Element == 5 ? Element : info.u1Element));
		TextSkillName.text = TextManager.Instance.GetText(info.sName);
		TextSkillName.color = EquipmentItem.equipElementColors[(info.u1Element == 5 ? Element : info.u1Element)-1];
		TextSkillType.text = info.u1ActWay == 1 ? TextManager.Instance.GetText("mark_skill_active") : TextManager.Instance.GetText("mark_skill_passive");

		if (Level > 0) {
			TextSkillLevel.text = Level.ToString ();
			string equipLv = "";
			if (EquipSkillPoint.ContainsKey (SlotNum)) {
				equipLv = "(+"+EquipSkillPoint[SlotNum].ToString()+")";
			}

			TextSkillLimit.text = "Lv."+Level.ToString ()+equipLv;
		} else {
			TextSkillLevel.text = "";
			tempStringBuilder.Remove (0, tempStringBuilder.Length);
			tempStringBuilder.Append (TextManager.Instance.GetText ("mark_skill_need_lv_char")).Append (" ");
			tempStringBuilder.Append (info.u2NeedLevel.ToString ());
			TextSkillLimit.text = tempStringBuilder.ToString ();

			if (cSelectedHero.cLevel.u2Level < info.u2NeedLevel) {
				TextSkillLimit.color = colorlimitRed;
			} else {
				TextSkillLimit.color = colorlimitGreen;
			}
		}
		
		SetSkillInfoByTreeSlotNum(SlotNum);

		WriteSkillInfo (info, Level);
	}

	void WriteSkillInfo(SkillInfo info, UInt16 Level){

		if (EquipSkillPoint.ContainsKey (info.u1SlotNum)) {
			Level += EquipSkillPoint[info.u1SlotNum];
		}
		
		TextSkillComment.text = info.GetSkillDescToLevel (Level);
	}

	public void SetSkillInfoByTreeSlotNum(Byte SlotNum){
		if(u1BeforeSlot > 0) lstIcons[u1BeforeSlot].tr.localScale = Vector3.one;
		lstIcons[SlotNum].tr.localScale = Vector3.one*1.3f;

		u1BeforeSlot = SlotNum;
		cCurLearnedSkill = lstLearnInfo.Find (cs => cs.u1SlotNum == u1BeforeSlot);
		cCurSkillInfo = lstSkills.Find(cs => cs.u1SlotNum == SlotNum);
	}

	public void SetSetting(){
		eState = SkillWindowState.Setting;
		UseObject.gameObject.SetActive(true);
		//InfoObject.localPosition = new Vector3(640+page2X ,InfoObject.localPosition.y, InfoObject.localPosition.z);
		InfoObject.anchoredPosition3D = new Vector3(-(InfoObject.sizeDelta.x * 0.5f) ,InfoObject.localPosition.y, InfoObject.localPosition.z);
		BtnUse.SetActive(false);
		BtnGoLearn.SetActive(false);
		BtnGoUpgrade.SetActive(false);

		int start = 0;
		int end = lstSelectSkills.Count/2;
		if (cCurSkillInfo.u1ActWay == 2) {
			start = end;
			end = lstSelectSkills.Count;
		}

		for (int i=start; i<end; i++) {
			lstSelectSkills[i].SetSelectEff(true);
		}
	}

	public void ClickUnuseCurrent(){
		LearnedSkill temp = lstChangedInfo.Find(cs => cs.u1UseIndex == cCurLearnedSkill.u1UseIndex);

		lstSelectSkills[cCurLearnedSkill.u1UseIndex-1].UnUse();
		lstIcons [cCurLearnedSkill.u1SlotNum].SetUse (false);
		cCurLearnedSkill.u1UseIndex = 0;

		int idx = lstChangedInfo.FindIndex (cs => cs.u1SlotNum == cCurLearnedSkill.u1SlotNum);
		if(idx < 0)
			lstChangedInfo.Add(cCurLearnedSkill);
		else
			lstChangedInfo[idx].u1UseIndex = 0;

		SetUse();
	}

	public void SetUpgrade(bool bUpgrade){
		if(cHeroSkills.GetTotalPoint() <= 0 || !CheckNeedLevel(cCurSkillInfo.u1SlotNum)) return;

		bool bLock = dicLocked [cCurSkillInfo.u1SlotNum];

		if (bLock)
			return;

		if(bUpgrade){
			if(cCurLearnedSkill.u2Level >= EquipmentInfoMgr.Instance.skillMaxPoint) return;
		}

		DisableSettingEff();

		//UseObject.gameObject.SetActive(false);
		InfoObject.gameObject.SetActive(true);

		if (bUpgrade) {
			curPopup = Instantiate (objUpgradePop) as GameObject;
            //curPopup.transform.SetParent(transform.parent.parent);
            curPopup.transform.SetParent(PopupManager.Instance._objPopupManager.transform);
            curPopup.transform.localScale = Vector3.one;
			curPopup.transform.localPosition = Vector3.zero;

			UI_SkillLearnPopup tempScript = curPopup.GetComponent<UI_SkillLearnPopup>();
			Byte Ele = (cCurSkillInfo.u1Element == 5 ? Element : cCurSkillInfo.u1Element);

			eState = SkillWindowState.Upgrade;
			tempScript.Show(TextManager.Instance.GetText("popup_title_upgrade_skill"), TextManager.Instance.GetText(cCurSkillInfo.sName), TextManager.Instance.GetText("popup_btn_skill_upgrade"), UpgradeSkill, null);
			tempScript.SetLearnPopup(u2ClassID, cCurSkillInfo.u2ID, Ele, cCurLearnedSkill.u2Level, cCurSkillInfo.GetSkillDescToLevel(cCurLearnedSkill.u2Level), cCurSkillInfo.GetSkillDescToLevel((ushort)(cCurLearnedSkill.u2Level+1)));
		} else {
			curPopup = Instantiate (objLearnPop) as GameObject;
            //curPopup.transform.SetParent(transform.parent.parent);
            curPopup.transform.SetParent(PopupManager.Instance._objPopupManager.transform);

            curPopup.transform.localScale = Vector3.one;
			curPopup.transform.localPosition = Vector3.zero;

			UI_SkillLearnPopup tempScript = curPopup.GetComponent<UI_SkillLearnPopup>();
			Byte Ele = (cCurSkillInfo.u1Element == 5 ? Element : cCurSkillInfo.u1Element);

			eState = SkillWindowState.Learn;
			tempScript.Show(TextManager.Instance.GetText("popup_title_active_skill"), TextManager.Instance.GetText(cCurSkillInfo.sName), TextManager.Instance.GetText("popup_btn_skill_active"), ActivateSkill, null);
			tempScript.SetLearnPopup(u2ClassID, cCurSkillInfo.u2ID, Ele, 0, "", "");
		}

		PopupManager.Instance.AddPopup(curPopup, CloseCurrentPopup);
		
//		BtnUse.SetActive(false);
//		BtnUnuse.SetActive(false);
//		BtnGoLearn.SetActive(false);
//		BtnGoUpgrade.SetActive(false);
	}

	void CloseCurrentPopup(){
		if (curPopup == null)
			return;

		PopupManager.Instance.RemovePopup(curPopup);
		Destroy(curPopup);
	}

	void CheckPoint(){
		if (cHeroSkills.GetTotalPoint () <= 0) {
			//HavePointEff.SetActive (false);
            SkillPointEffEnable(false);
            foreach (SkillIcon temp in lstIcons.Values){
					temp.ActiveCircle(false);
			}

			if(cCurSkillInfo != null){
				if(BtnGoLearn.activeSelf) SetButtonActive(BtnGoLearn, false);
				else if(BtnGoUpgrade.activeSelf) SetButtonActive(BtnGoUpgrade, false);
			}
		} else {
			//HavePointEff.SetActive (true);
            SkillPointEffEnable(true);
            foreach (KeyValuePair<byte, SkillIcon> temp in lstIcons) {
				if (CheckNeedLevel(temp.Key)) {
					if(lstLearnInfo.FindIndex (cs => cs.u1SlotNum == temp.Key) < 0 && !dicLocked[temp.Key]){
						temp.Value.ActiveCircle(true);
					}
				}
			}

			if(cCurSkillInfo != null){
				if(BtnGoLearn.activeSelf){
					if(CheckNeedLevel(cCurSkillInfo.u1SlotNum)) SetButtonActive(BtnGoLearn, true);
				}else if(BtnGoUpgrade.activeSelf){
					if(cCurLearnedSkill.u2Level >= EquipmentInfoMgr.Instance.skillMaxPoint) SetButtonActive(BtnGoUpgrade, false);
					else SetButtonActive(BtnGoUpgrade, true);
				}
			}
		}
		TextHeroSP.text = cHeroSkills.GetTotalPoint().ToString();
        objCharInfoPanel.GetComponent<UI_Panel_CharacterInfo>().SetToggleBtn();
	}

	void SetButtonActive(GameObject btnObj, bool bActive){
		Button btn = btnObj.GetComponent<Button>();
		ColorBlock cb = btn.colors;
		
		if(bActive){
			btnObj.transform.GetChild(0).GetComponent<Text>().color = Color.white;
			cb.normalColor = Color.white;
			cb.highlightedColor = Color.white;
		}else{
			btnObj.transform.GetChild(0).GetComponent<Text>().color = Color.gray;
			cb.normalColor = Color.gray;
			cb.highlightedColor = Color.gray;
		}
		
		btn.colors = cb;
	}

	public void SetUse(){
		DisableSettingEff();

		eState = SkillWindowState.Idle;
		UseObject.gameObject.SetActive(true);
		InfoObject.gameObject.SetActive(false);
	}
    //정환 코드 추가
	public void ClickUseSkill(Byte SlotNum){
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
		if (eState != SkillWindowState.Setting) {
			LearnedSkill skill = lstLearnInfo.Find (cs => cs.u1UseIndex == SlotNum);
			if (skill != null) {
				DisableSettingEff();
				SetCurSkillInfo(skill.u1SlotNum);
				//SetUseSkillInfo (skill.u1SlotNum);
				lstSelectSkills[SlotNum-1].SetSelectEff(true);
			}else{
				if(!lstSelectSkills[SlotNum-1].bLock) return;

				u1UnlockIdx = SlotNum;
                object[] yesEventParam = new object[1];
			    yesEventParam[0] = SlotNum;

				int idx = GetUnlockPriceIndex(SlotNum);

				GameObject pop = Instantiate (objSlotBuyPop) as GameObject;
				pop.transform.SetParent(PopupManager.Instance._objPopupManager.transform);
				pop.transform.localScale = Vector3.one;
				pop.transform.localPosition = Vector3.zero;

				EventDisCountinfo disInfo = EventInfoMgr.Instance.GetDiscountEventinfo (DISCOUNT_ITEM.OPENSKILL);
				UInt32 price = LegionInfoMgr.Instance.acSkillOpenGoods [idx].u4Count;
				if (disInfo != null) {
					pop.GetComponent<StatResetPopup>().SetDiscount((uint)LegionInfoMgr.Instance.acSkillOpenGoods[idx].u4Count, disInfo.u1DiscountRate);
					price = (uint)(LegionInfoMgr.Instance.acSkillOpenGoods[idx].u4Count * disInfo.discountRate);
				}

				pop.GetComponent<YesNoPopup>().Show(TextManager.Instance.GetText("popup_title_slot_buy_skill"), price.ToString(), RequestSelectSlotUnlock, yesEventParam);
                return;
			}
		} else {
			int idx = SlotNum-1;
			if(lstSelectSkills[idx].bLock) return;

			if (cCurLearnedSkill != null) {
				if(cCurSkillInfo.u1ActWay == 1 && idx > (lstSelectSkills.Count/2)-1) return;
				else if(cCurSkillInfo.u1ActWay == 2 && idx < (lstSelectSkills.Count/2)) return;

				LearnedSkill beforeSkill = lstLearnInfo.Find(cs => cs.u1UseIndex == SlotNum);

				if(beforeSkill != null){
					beforeSkill.u1UseIndex = 0;
					lstIcons [beforeSkill.u1SlotNum].SetUse (false);

					int sidx = lstChangedInfo.FindIndex(cs => cs.u1SlotNum == beforeSkill.u1SlotNum);

					if(sidx < 0)
						lstChangedInfo.Add(beforeSkill);
					else
						lstChangedInfo[sidx].u1UseIndex = 0;
				}

				lstSelectSkills[idx].Change(cCurSkillInfo.u2ID, cCurSkillInfo.u1Element == 5 ? Element : cCurSkillInfo.u1Element, cCurLearnedSkill.u2Level);

				cCurLearnedSkill.u1UseIndex = SlotNum;
				lstIcons [cCurLearnedSkill.u1SlotNum].SetUse (true);

				int sidx2 = lstChangedInfo.FindIndex(cs => cs.u1SlotNum == cCurLearnedSkill.u1SlotNum) ;
				
				if(sidx2 < 0)
					lstChangedInfo.Add(cCurLearnedSkill);
				else
					lstChangedInfo[sidx2].u1UseIndex = SlotNum;

				eState = SkillWindowState.Idle;
				DisableSettingEff();
				InfoObject.gameObject.SetActive(false);
			}
		}
	}

	int GetUnlockPriceIndex(Byte SlotNum){
		switch(SlotNum)
		{
		case 4:
			return 0;
		case 5:
			return 1;
		case 6:
			return 2;
		case 10:
			return 3;
		case 11:
			return 4;
		case 12:
			return 5;
		}

		return 0;
	}

    void RequestSelectSlotUnlock(object[] param)
    {
        Byte _slotNum = Convert.ToByte(param [0]);

		int idx = GetUnlockPriceIndex(_slotNum);

		EventDisCountinfo disInfo = EventInfoMgr.Instance.GetDiscountEventinfo (DISCOUNT_ITEM.OPENSKILL);
		UInt32 price = LegionInfoMgr.Instance.acSkillOpenGoods [idx].u4Count;
		if (disInfo != null) {
			price = (uint)(LegionInfoMgr.Instance.acSkillOpenGoods[idx].u4Count * disInfo.discountRate);
		}

		if(!Legion.Instance.CheckEnoughGoods((int)LegionInfoMgr.Instance.acSkillOpenGoods[idx].u1Type, (int)price))
		{
			PopupManager.Instance.ShowChargePopup(LegionInfoMgr.Instance.acSkillOpenGoods[idx].u1Type);
			return;
		}

        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.OpenSkillSelectSlot(cSelectedHero, _slotNum, ResultSelectSlotUnlock);
    }

	void ResultSelectSlotUnlock(Server.ERROR_ID err){
		PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CHAR_SKILL_OPEN, err) + TextManager.Instance.GetText("popup_desc_server_error_critical"), Server.ServerMgr.Instance.ApplicationShutdown);
            //PopupManager.Instance.ShowOKPopup ("", TextManager.Instance.GetError(((int)err).ToString()), emptyMethod);
			return;
        }

		else if (err == Server.ERROR_ID.NONE)
        {
			int idx = GetUnlockPriceIndex(u1UnlockIdx);

			EventDisCountinfo disInfo = EventInfoMgr.Instance.GetDiscountEventinfo (DISCOUNT_ITEM.OPENSKILL);
			UInt32 price = LegionInfoMgr.Instance.acSkillOpenGoods [idx].u4Count;
			if (disInfo != null) {
				price = (uint)(LegionInfoMgr.Instance.acSkillOpenGoods[idx].u4Count * disInfo.discountRate);
			}

			lstSelectSkills[u1UnlockIdx-1].UnLock();
		}
	}
    //정환 코드 수정 끝
	void DisableSettingEff(){
		for (int i=0; i<lstSelectSkills.Count; i++) {
			lstSelectSkills[i].SetSelectEff(false);
		}
	}

	public void SetUseSkillInfo(Byte SlotNum){
		InfoObject.localPosition = new Vector3(640+page2X ,InfoObject.localPosition.y, InfoObject.localPosition.z);

		BtnGoLearn.SetActive (false);
		BtnGoUpgrade.SetActive (true);
		BtnUse.SetActive(false);
		BtnUnuse.SetActive(true);
		UInt16 Level = lstLearnInfo.Find(cs => cs.u1SlotNum == SlotNum).u2Level;
		TextSkillLevel.text = Level.ToString();
		string equipLv = "";
		if (EquipSkillPoint.ContainsKey (SlotNum)) {
			equipLv = "(+"+EquipSkillPoint[SlotNum].ToString()+")";
		}

		TextSkillLimit.text = string.Format ("<color=#298F01>Lv.{0} </color><color=#35FFBA>{1}</color>", Level.ToString (), equipLv);
		
		SetUseInfo(SlotNum, Level);
	}

	public void ActivateSkill(object[] param){
		if (cCurLearnedSkill == null) {

			bool bLock = dicLocked [u1BeforeSlot];

			if (bLock)
				return;

			if(cHeroSkills.GetTotalPoint() < 1){
				//ErrorPopup
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_active_skill"), TextManager.Instance.GetText("popup_desc_skill_pt_lack"), null);
				DebugMgr.LogError("Not Enough SkillPt");
				return;
			}

			if(cSelectedHero.cLevel.u2Level < cCurSkillInfo.u2NeedLevel){
				//ErrorPopup
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_active_skill"), TextManager.Instance.GetText("popup_desc_skill_lv_lack"), null);
				DebugMgr.LogError("Not Enough Level");
				return;
			}

			LearnedSkill temp = new LearnedSkill();
			temp.u1SlotNum = u1BeforeSlot;
			temp.u2Level = 1;
			temp.u1UseIndex = 0;
			lstLearnInfo.Add(temp);
			cHeroSkills.LeanSkill(temp);

			lstChangedInfo.Add(temp);

			AtlasMgr.Instance.SetDefaultShader(lstIcons[u1BeforeSlot].Icon);
			lstIcons[u1BeforeSlot].SetLevel(1);
			lstIcons[u1BeforeSlot].ActiveCircle(false);

			GameObject tempObj = Instantiate(objActiveEff) as GameObject;
			tempObj.transform.SetParent(lstIcons[u1BeforeSlot].tr);
			tempObj.transform.localScale = Vector3.one;
			tempObj.transform.localPosition = Vector3.zero;
			
			AddIcon();
			AddTreeLine();
			
			SetUse();

			cHeroSkills.SubSkillPoint();
			TextHeroSP.GetComponent<Animator>().Play("Ani_UI_Main_SkillUgrade",0,0.0f);

			CheckPoint();

			Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.SkillUpgrade, 0, 0, 0, 0, 1);
			//Server.ServerMgr.Instance.ChangeSkill(cSelectedHero, lstLearnInfo, ResultActive);
		}
	}

	public void UpgradeSkill(object[] param){
		if (cCurLearnedSkill != null) {

			if(cHeroSkills.GetTotalPoint() < 1){
				//ErrorPopup
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_upgrade_skill"), TextManager.Instance.GetText("popup_desc_skill_pt_lack"), null);
				DebugMgr.LogError("Not Enough SkillPt");
				return;
			}

			cCurLearnedSkill.u2Level++;
			lstIcons[u1BeforeSlot].SetLevel(cCurLearnedSkill.u2Level);

			if(cCurLearnedSkill.u1UseIndex > 0)
				lstSelectSkills[cCurLearnedSkill.u1UseIndex-1].Change(cCurSkillInfo.u2ID, cCurSkillInfo.u1Element == 5 ? Element : cCurSkillInfo.u1Element, cCurLearnedSkill.u2Level);

			GameObject temp = Instantiate(objLvUpEff) as GameObject;
			temp.transform.SetParent(lstIcons[u1BeforeSlot].tr.FindChild("SkillElement"));
			temp.transform.localScale = Vector3.one;
			temp.transform.localPosition = Vector3.zero;

			TextSkillLevel.text = cCurLearnedSkill.u2Level.ToString();
			TextSkillLevel.GetComponent<Animator>().Play("Ani_UI_Main_SkillUgrade",0,0.0f);

			WriteSkillInfo(cCurSkillInfo, cCurLearnedSkill.u2Level);
			
			if(lstChangedInfo.FindIndex(cs => cs.u1SlotNum == u1BeforeSlot) < 0)
				lstChangedInfo.Add(cCurLearnedSkill);

			cHeroSkills.SubSkillPoint();

			TextHeroSP.GetComponent<Animator>().Play("Ani_UI_Main_SkillUgrade",0,0.0f);

			CheckPoint();

			Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.SkillUpgrade, 0, 0, 0, 0, 1);
		}
	}

	void ResultChange(Server.ERROR_ID err){

		PopupManager.Instance.CloseLoadingPopup();
		if (err == Server.ERROR_ID.NONE) {
            objCharInfoPanel.GetComponent<UI_Panel_CharacterInfo>().SetToggleBtn();
			lstChangedInfo.Clear ();
            Legion.Instance.eCharState = Legion.ChangeCharInfo.CHANGED;
            PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.SkillUpgrade);
		} else {
            Legion.Instance.eCharState = Legion.ChangeCharInfo.ERROR;
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CHAR_SKILL_CHANGE, err)+TextManager.Instance.GetText("popup_desc_server_error_critical"), Server.ServerMgr.Instance.ApplicationShutdown);
			//PopupManager.Instance.ShowOKPopup ("", TextManager.Instance.GetError(((int)err).ToString()), emptyMethod);
		}
	}

	public void SubmitSkillSetting()
    {
		if(lstChangedInfo.Count == 0)
        {
            Legion.Instance.eCharState = Legion.ChangeCharInfo.CHANGED;
            return;
        } 

		PopupManager.Instance.ShowLoadingPopup (1);
		Server.ServerMgr.Instance.ChangeSkill(new Byte[1]{cSelectedHero.u1Index}, new List<LearnedSkill>[1]{lstChangedInfo}, ResultChange);
	}

	void AddIcon(){
		for(int i=0; i<lstSkills.Count; i++){
			for(int j=0; j<lstSkills[i].au1LinkSlot.Length; j++){
				Byte cnt = lstSkills[i].u1SlotNum;

				if(cCurSkillInfo.u1SlotNum == lstSkills[i].au1LinkSlot[j]){
					if (dicLocked[lstSkills [i].u1SlotNum] == true) {
						dicLocked [lstSkills [i].u1SlotNum] = false;
//					if (!lstIcons.ContainsKey (lstSkills [i].u1SlotNum)) {
//						CreateIcon (lstSkills.Find (cs => cs.u1SlotNum == lstSkills [i].u1SlotNum), cnt, 0, true);
					}
				}
			}
		}
	}

	void AddTreeLine(){
		for(int i=0; i<lstSkills.Count; i++){
			for(int j=0; j<lstSkills[i].au1LinkSlot.Length; j++){
				string lineKey = "";
				if(cCurSkillInfo.u1SlotNum < lstSkills[i].au1LinkSlot[j]) lineKey = cCurSkillInfo.u1SlotNum+"-"+lstSkills[i].au1LinkSlot[j];
				else lineKey = lstSkills[i].au1LinkSlot[j]+"-"+cCurSkillInfo.u1SlotNum;
				
				if(dicLines.ContainsKey(lineKey)){
					if(lstLearnInfo.FindIndex(cs => cs.u1SlotNum == cCurSkillInfo.u1SlotNum) > -1
					   || lstLearnInfo.FindIndex(cs => cs.u1SlotNum == lstSkills[i].au1LinkSlot[j]) > -1){
						dicLines[lineKey].enabled = true;
					}
				}
			}
		}
	}

    //정환 코드 수정
    int skillResetCountStack;
	public void ResetSkill(){
		
		if(lstChangedInfo.Count > 0)
		{
			PopupManager.Instance.ShowLoadingPopup (1);
			Server.ServerMgr.Instance.ChangeSkill(new Byte[1]{cSelectedHero.u1Index}, new List<LearnedSkill>[1]{lstChangedInfo}, ResultChange);
		}

        object[] yesEventParam = new object[1];
		
		if(!cHeroSkills.CheckResetPossible()) return;

		skillResetCountStack = (((int)cHeroSkills.ResetCount * (int)LegionInfoMgr.Instance.skillResetUpgrade) + (int)LegionInfoMgr.Instance.cSkillResetGoods.u4Count);

        if(skillResetCountStack > LegionInfoMgr.Instance.skillResetPriceMax)
            skillResetCountStack = (int)LegionInfoMgr.Instance.skillResetPriceMax;

		curPopup = Instantiate (objResetPop) as GameObject;

		EventDisCountinfo disInfo = EventInfoMgr.Instance.GetDiscountEventinfo (DISCOUNT_ITEM.RESETSKILL);
		if (disInfo != null) {
			curPopup.GetComponent<StatResetPopup>().SetDiscount((uint)skillResetCountStack, disInfo.u1DiscountRate);
			skillResetCountStack = (int)(skillResetCountStack * disInfo.discountRate);
		}

		yesEventParam[0] = skillResetCountStack;

		curPopup.transform.SetParent(PopupManager.Instance._objPopupManager.transform);
		curPopup.transform.localScale = Vector3.one;
		curPopup.transform.localPosition = Vector3.zero;
		curPopup.GetComponent<YesNoPopup>().Show(TextManager.Instance.GetText("popup_title_stat_rest_skill"), skillResetCountStack.ToString(), RequestResetSkill, yesEventParam);


		PopupManager.Instance.AddPopup(curPopup, CloseCurrentPopup);
		
		//		BtnUse.SetActive(false);
		//		BtnUnuse.SetActive(false);
		//		BtnGoLearn.SetActive(false);
		//		BtnGoUpgrade.SetActive(false);
	}

	public void RequestResetSkill(object[] param)
	{
		int tempPrice = Convert.ToInt16(param[0]);

		if(!Legion.Instance.CheckEnoughGoods((int)LegionInfoMgr.Instance.cSkillResetGoods.u1Type, skillResetCountStack))
		{
			PopupManager.Instance.ShowChargePopup(LegionInfoMgr.Instance.cSkillResetGoods.u1Type);
			return;
		}
		PopupManager.Instance.ShowLoadingPopup (1);
		Server.ServerMgr.Instance.ResetSkill (cSelectedHero, ResultReset);
	}

	//정환 코드 수정
	void ResultReset(Server.ERROR_ID err){
		PopupManager.Instance.CloseLoadingPopup();
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CHAR_SKILL_RESET, err)+TextManager.Instance.GetText("popup_desc_server_error_critical"), Server.ServerMgr.Instance.ApplicationShutdown);
			//PopupManager.Instance.ShowOKPopup ("", TextManager.Instance.GetError(((int)err).ToString()), emptyMethod);
			return;
		}
		else if (err == Server.ERROR_ID.NONE) {
			InitData(false);
			lstLearnInfo.Clear();
			cHeroSkills.lstLearnInfo = cHeroSkills.GetInitSkill ();
			lstLearnInfo = cHeroSkills.lstLearnInfo;

			bSetting = false;
			
			ViewSkills();
			ViewUseSkills();
			CheckPoint();
			SetUse();
		}
	}
    //정환 코드 수정 끝

	public void AutoSelect(){
		tempStringBuilder.Remove(0, tempStringBuilder.Length);
		tempStringBuilder.Append(TextManager.Instance.GetText("popup_desc_setting_auto_skill"));

		curPopup = Instantiate (objRecommandPop) as GameObject;
		curPopup.transform.parent = PopupManager.Instance._objPopupManager.transform;
		curPopup.transform.localScale = Vector3.one;
		curPopup.transform.localPosition = Vector3.zero;
		curPopup.GetComponent<YesNoPopup>().Show(TextManager.Instance.GetText("popup_title_setting_auto_skill"), tempStringBuilder.ToString(), RequestAutoSelect, null);

		PopupManager.Instance.AddPopup(curPopup, CloseCurrentPopup);
	}

	public void RequestAutoSelect(object[] param){
		for (int i=0; i<lstLearnInfo.Count; i++) {
			if(lstLearnInfo[i].u1UseIndex > 0){
				lstLearnInfo[i].u1UseIndex = 0;
				lstIcons [lstLearnInfo[i].u1SlotNum].SetUse (false);

				int idx = lstChangedInfo.FindIndex(cs => cs.u1SlotNum == lstLearnInfo[i].u1SlotNum);
				if(idx < 0)
					lstChangedInfo.Add(lstLearnInfo[i]);
				else 
					lstChangedInfo[idx].u1UseIndex = 0;
			}
		}

		lstLearnInfo.Sort( delegate(LearnedSkill x, LearnedSkill y){
			int diff = y.u2Level.CompareTo(x.u2Level);
			if(diff != 0) return diff;
			else return y.u1SlotNum.CompareTo(x.u1SlotNum);
		});

		Byte ActiveIdx = 0;
		Byte PassiveIdx = 6;
		for (int i=0; i<lstLearnInfo.Count; i++) {
			if(lstLearnInfo[i].u2Level > 0){
				SkillInfo tInfo = lstSkills.Find(cs => cs.u1SlotNum == lstLearnInfo[i].u1SlotNum);
				if(tInfo.u1ActWay == 1){
					if(ActiveIdx > 5) continue;

					bool bLock = cHeroSkills.CheckLockSlot(ActiveIdx);

					if(bLock) continue;

					ActiveIdx++;
					Byte before = lstLearnInfo[i].u1UseIndex;
					lstLearnInfo[i].u1UseIndex = ActiveIdx;
					lstIcons [lstLearnInfo[i].u1SlotNum].SetUse (true);
					lstSelectSkills[ActiveIdx-1].Change(tInfo.u2ID, tInfo.u1Element == 5 ? Element : tInfo.u1Element, lstLearnInfo[i].u2Level);
					if(before != ActiveIdx){
						int idx = lstChangedInfo.FindIndex(cs => cs.u1SlotNum == lstLearnInfo[i].u1SlotNum);
						if(idx < 0)
							lstChangedInfo.Add(lstLearnInfo[i]);
						else 
							lstChangedInfo[idx].u1UseIndex = ActiveIdx;
					}
				}else{
					if(PassiveIdx > 11) continue;

					bool bLock = cHeroSkills.CheckLockSlot(PassiveIdx);
					
					if(bLock) continue;

					PassiveIdx++;
					Byte before = lstLearnInfo[i].u1UseIndex;
					lstLearnInfo[i].u1UseIndex = PassiveIdx;
					lstIcons [lstLearnInfo[i].u1SlotNum].SetUse (true);
					lstSelectSkills[PassiveIdx-1].Change(tInfo.u2ID, tInfo.u1Element == 5 ? Element : tInfo.u1Element, lstLearnInfo[i].u2Level);
					if(before != PassiveIdx){
						int idx = lstChangedInfo.FindIndex(cs => cs.u1SlotNum == lstLearnInfo[i].u1SlotNum);
						if(idx < 0)
							lstChangedInfo.Add(lstLearnInfo[i]);
						else 
							lstChangedInfo[idx].u1UseIndex = PassiveIdx;
					}
				}
			}
		}

		SetUse ();
	}

	public void BuySkillPoint(){
        if (cHeroSkills.AddSkillPoint < EquipmentInfoMgr.Instance.LIMIT_SKILLPOINT) {
			tempStringBuilder.Remove (0, tempStringBuilder.Length);
			tempStringBuilder.Append (TextManager.Instance.GetText("popup_desc_stat_buy_skill"));
			curPopup = Instantiate (objPointBuyPop) as GameObject;
			curPopup.transform.parent = PopupManager.Instance._objPopupManager.transform;
			curPopup.transform.localScale = Vector3.one;
			curPopup.transform.localPosition = Vector3.zero;
			curPopup.GetComponent<UI_PointBuyPopup> ().Show (TextManager.Instance.GetText("popup_title_stat_buy_skill"), tempStringBuilder.ToString (), RequestBuyPoint, null);
			curPopup.GetComponent<UI_PointBuyPopup> ().SetBuyPointPopup (UI_PointBuyPopup.BuyType.Skill, cHeroSkills.AddSkillPoint);

			PopupManager.Instance.AddPopup(curPopup, CloseCurrentPopup);
		} else {
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_stat_buy_skill"), TextManager.Instance.GetText("popup_desc_skill_pt_nobuy"), null);
		}
	}

	public void RequestBuyPoint(object[] param){
		if(!Legion.Instance.CheckEnoughGoods(EquipmentInfoMgr.Instance.cSkillPointGoods.u1Type, (int)param[1]))
		{
			PopupManager.Instance.ShowChargePopup(EquipmentInfoMgr.Instance.cSkillPointGoods.u1Type);
			return;
		}
		PopupManager.Instance.ShowLoadingPopup (1);
		u1AddPoint = (Byte)param[0];
		addPrice = (int)param[1];
		Server.ServerMgr.Instance.BuyCharacterSkillPoint (cSelectedHero, u1AddPoint, ResultBuyPoint);
	}

	void ResultBuyPoint(Server.ERROR_ID err){
		PopupManager.Instance.CloseLoadingPopup();
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(Server.MSGs.CHAR_BUY_SKILL_POINT, err)+TextManager.Instance.GetText("popup_desc_server_error_critical")/*"\n치명적인 오류가 발생하여 앱을 종료합니다."*/, Server.ServerMgr.Instance.ApplicationShutdown);
			//PopupManager.Instance.ShowOKPopup ("", TextManager.Instance.GetError(((int)err).ToString()), emptyMethod);
			return;
		}
		else if (err == Server.ERROR_ID.NONE) {
			Legion.Instance.SubGoods((int)EquipmentInfoMgr.Instance.cSkillPointGoods.u1Type, addPrice);
			AddPoint();
            objCharInfoPanel.GetComponent<UI_Panel_CharacterInfo>().SetToggleBtn();
		}
	}

	void AddPoint(){
		GameObject temp = Instantiate(objPtBuyEff) as GameObject;
		temp.transform.parent = TextHeroSP.transform;
		temp.transform.localScale = Vector3.one;
		temp.transform.localPosition = Vector3.zero;

		TextHeroSP.GetComponent<Animator>().Play("Ani_UI_Main_SkillUgrade",0,0.0f);
		u1AddPoint = 0;
		addPrice = 0;
		CheckPoint();
	}

    public void SkillPointEffEnable(bool isEnable)
    {
        HavePointEff.SetActive(isEnable);
    }

    public void SkillPointEffAutoEnable()
    {
        if( cHeroSkills.GetTotalPoint() <= 0)
            SkillPointEffEnable(false);
        else
            SkillPointEffEnable(true);
    }
}
