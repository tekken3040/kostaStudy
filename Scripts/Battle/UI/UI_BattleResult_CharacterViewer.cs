using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class UI_BattleResult_CharacterViewer : MonoBehaviour {
	Text _txtEXP;
	Text _txtLv;

	Image _Element;

	public GameObject _UnderLevelUp;
	GameObject _LevelUp;
	GameObject _EquipUp;
	GameObject _SkillUnlock;

	Image _ExpGage;

	GameObject[] _objClearRewardElement;
	GameObject[] _objStageRewardElement;

	bool bEndAwake = false;
	bool bSet = false;
	bool bLvUp = false;
	bool bEquipUp = false;

	GameObject _EquipLvUpObj;
	GameObject _SkillUnlockObj;

	List<GameObject> _lstEquipLvUp = new List<GameObject>();

	float spd = 1.0f;

	float tick = 0.6f;
	float LvUpDelay = 1.5f;
	float time = -1.5f;

	int idx = 0;

	LevelComponent cLevelComp;

	public Animator anim;

	void Awake()
	{
		_EquipLvUpObj = (GameObject)AssetMgr.Instance.AssetLoad("Prefabs/UI/Battle/Result/EquipLvUp.prefab", typeof(GameObject));
		_SkillUnlockObj = (GameObject)AssetMgr.Instance.AssetLoad("Prefabs/UI/Battle/Result/SkillUnlock.prefab", typeof(GameObject));
		_Element = transform.FindChild("Lvl_Exp").FindChild("Levels").FindChild("Lvl_Element").GetComponent<Image>();
		_txtLv = transform.FindChild("Lvl_Exp").FindChild("Levels").FindChild("Text").GetComponent<Text>();
		_txtEXP = transform.FindChild("Lvl_Exp").FindChild("Exps").FindChild("Exp_Value").GetComponent<Text>();

		_ExpGage = transform.FindChild("Lvl_Exp").FindChild("Exps").FindChild("Exp_Gague").GetComponent<Image>();
		
		_LevelUp = transform.FindChild("LvUp").gameObject;
		_EquipUp = transform.FindChild("EquipUp").gameObject;
		_SkillUnlock = transform.FindChild("SkillUnlock").gameObject;

		bEndAwake = true;
//		_txtEXP.enabled = false;
	}

	UInt32 rewardExp;
	public bool SetData(Hero hero, uint u4Exp)
	{
		if(!bEndAwake) Awake();

		float from = (float)((float)hero.cLevel.u8Exp / (float)hero.cLevel.u8NextExp);

		_Element.sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.element_" + hero.GetHeroElement());
		rewardExp = u4Exp;

		cLevelComp = hero.GetComponent<LevelComponent> ();

		_txtLv.text = cLevelComp.cLevel.u2Level.ToString ();
		_txtEXP.text = cLevelComp.cLevel.u8Exp.ToString ()+"/"+cLevelComp.cLevel.u8NextExp.ToString ();

		_ExpGage.fillAmount = (float)((float)hero.cLevel.u8Exp / (float)hero.cLevel.u8NextExp);

		UInt16 beforeLv = cLevelComp.cLevel.u2Level;

		if (hero.GetComponent<LevelComponent> ().AddExp (rewardExp) > 0) {
			_LevelUp.SetActive(true);
			bLvUp = true;

			List<SkillNode> tmpList = hero.GetComponent<SkillComponent>().dicSkillInfo.Values.ToList<SkillNode>().
				FindAll( (x) => x.cSkill.cInfo.u2NeedLevel > beforeLv && x.cSkill.cInfo.u2NeedLevel <= cLevelComp.cLevel.u2Level);

			if(tmpList.Count > 0){
				_SkillUnlock.SetActive(true);

				for(int i=0; i<tmpList.Count; i++){
					GameObject tempIcon = Instantiate(_SkillUnlockObj) as GameObject;
					tempIcon.transform.SetParent(transform);
					tempIcon.transform.localScale = Vector3.one*2.5f;
					tempIcon.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;

					SkillInfo info = tmpList[i].cSkill.cInfo;
					Byte ele = info.u1Element == (Byte)5 ? (Byte)1 : info.u1Element;

					tempIcon.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Skill/Atlas_SkillIcon_"+hero.cClass.u2ID+"."+info.u2ID);
					tempIcon.transform.FindChild ("SkillElement").GetComponent<Image> ().sprite = AtlasMgr.Instance.GetSprite ("Sprites/Common/common_02_renew.element_"+ele);
					tempIcon.transform.FindChild("ElementBorder").GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.common_02_skill_element_"+ele);
					tempIcon.SetActive(false);
					_lstEquipLvUp.Add(tempIcon);
				}
			}
		}

		float to = (float) ((float)hero.cLevel.u8Exp / (float)hero.cLevel.u8NextExp);

		if (from > to)
			from = 0f;
		
		LeanTween.value(_ExpGage.gameObject, from, to, 1f).setOnUpdate((float amount)=>{_ExpGage.fillAmount = amount;}).setDelay(LvUpDelay);

		for(int j=0; j<hero.acEquips.Length; j++)
		{
			if(hero.acEquips[j] == null)
				continue;
			
			if(hero.acEquips[j].cLevel.u2Level < Server.ConstDef.MaxHeroLevel)
			{
				if(hero.acEquips[j].GetComponent<LevelComponent>().AddExp(rewardExp) > 0){
					bEquipUp = true;

					GameObject temp = Instantiate(_EquipLvUpObj) as GameObject;
					temp.transform.SetParent(transform);
					temp.transform.localScale = Vector3.one*2.5f;
					temp.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
					temp.GetComponent<EquipLevelUpIcon>().SetItem(hero.acEquips[j]);
					temp.SetActive(false);
					_lstEquipLvUp.Add(temp);
				}
			}
		}
		if (bEquipUp) {
			_EquipUp.SetActive (true);
		}

		bSet = true;

		if (bLvUp || bEquipUp) {
			return true;
		}

		return false;
	}

	void FixedUpdate(){
		if (bSet) {
			if(LvUpDelay > 0){
				LvUpDelay -= Time.fixedDeltaTime;
				if(LvUpDelay <= 0){
					_txtLv.text = cLevelComp.cLevel.u2Level.ToString ();
					_txtEXP.text = cLevelComp.cLevel.u8Exp.ToString ()+"/"+cLevelComp.cLevel.u8NextExp.ToString ();
					if(bLvUp){
						_UnderLevelUp.SetActive(true);
						bLvUp = false;
						anim.enabled = true;
					}else if (bEquipUp) {
						anim.enabled = true;
					}else{
						time = -1f;
					}
				}
			}else{
				if(_lstEquipLvUp.Count > 0){
					if(time >= tick*idx){
						Destroy(_lstEquipLvUp[idx], 1.2f);
						_lstEquipLvUp[idx].SetActive(true);
						int newIdx = idx;
						LeanTween.scale(_lstEquipLvUp[newIdx], Vector3.one*0.8f, 0.15f);
						LeanTween.scale(_lstEquipLvUp[newIdx], Vector3.one, 0.05f).setDelay(0.15f);
						LeanTween.alpha(_lstEquipLvUp[newIdx].transform.FindChild("Txt").GetComponent<RectTransform>(), 1.0f, 0.05f).setDelay(0.3f);
						LeanTween.moveLocalX(_lstEquipLvUp[newIdx].transform.FindChild("Txt").gameObject, 50f, 0.2f).setDelay(0.3f).setEase(LeanTweenType.easeInBack);
						LeanTween.moveLocalY(_lstEquipLvUp[newIdx], 150f, 0.5f).setDelay(0.5f);
						LeanTween.value(_lstEquipLvUp[newIdx], 1f, 0f, 0.3f).setDelay(0.7f).setOnUpdate((float alpha)=>{_lstEquipLvUp[newIdx].GetComponent<CanvasGroup>().alpha = alpha;});


						if(idx < _lstEquipLvUp.Count-1) idx++;
						else bSet = false;
						//spd += tick/2f;
					}
					time += Time.fixedDeltaTime*spd;
				}else{
					bSet = false;
				}
			}
		}
	}
}
