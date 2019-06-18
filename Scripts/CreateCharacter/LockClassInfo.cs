using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class LockClassInfo : MonoBehaviour 
{
	public Image _imgClassIllustrate;
	public Text _textUnLockCondition;
	public Animator _openEffectAni;

	public void SetLockClassInfo(UInt16 classID)
	{
		ClassInfo classInfo = ClassInfoMgr.Instance.GetInfo(classID);
		if(classInfo == null)
		{
			DebugMgr.LogError("Lock ClassInfo null");
			return;
		}
		_imgClassIllustrate.sprite = AssetMgr.Instance.AssetLoad(string.Format("Sprites/CreateCharacter/Character_illustration_{0}.png",classID.ToString()), typeof(Sprite)) as Sprite;

		// 현재 잠긴 클래스중 vip 레벨로 오픈 가능 여부를 찾는다
		Byte openVipLevel = 0;
		Dictionary<Byte, VipInfo> vipData = LegionInfoMgr.Instance.dicVipData;
		string openCond;
		for(byte i = 0; i < vipData.Count; ++i)
		{
			if(vipData[i].u2OpenClassID != classID)
				continue;

			openVipLevel = vipData[i].u1Level;
			break;
		}

		if(openVipLevel > 0)
			openCond = string.Format(TextManager.Instance.GetText(classInfo.UnLockInfo), TextManager.Instance.GetText(string.Format("odin_name_{0}", openVipLevel)));
		else
			openCond = TextManager.Instance.GetText(classInfo.UnLockInfo);
	
		_textUnLockCondition.text = openCond;
		_openEffectAni.Play("OpenLockCharacterInfo");
	}
}
