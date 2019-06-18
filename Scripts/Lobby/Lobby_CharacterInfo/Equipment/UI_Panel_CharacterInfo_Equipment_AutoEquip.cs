using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class UI_Panel_CharacterInfo_Equipment_AutoEquip : MonoBehaviour {
	Color32 color_up = new Color32(79, 240, 200, 255);
	Color32 color_down = new Color32(240, 79, 79, 255);

	[SerializeField] Text[] _txtStatType;
	[SerializeField] Text[] _txtStatValue;
	[SerializeField] Image[] _imgStatArrow;
	[SerializeField] Button _btnAutoEquipConfirm;
	[SerializeField] Sprite _imgEnableButton;
	[SerializeField] Sprite _imgDisableButton;

	[SerializeField] Text _txtBtnAutoEquipConfirm;
	[SerializeField] UI_Panel_CharacterInfo_Equipment _cParent;
	Hero _cHero;

	// _au2ChangeEquipSlot : 인벤슬롯 번호를 저장하는 변수
	// 처음엔 착용중 장비의 인벤 슬롯번호 hero.acEquips[0~9].u2SlotNum
	// 기억하고 있다가 바뀔 장비가 있으면 슬롯번호 교체.
	// 교체버튼 눌렀을 때 hero.acEquips[0~9].u2SlotNum와 au2ChangeEquipSlot[0~9] 슬롯번호를 비교하여 바뀔 장비 판단하여
	// 서버에 전송
	UInt16[] _au2ChangeEquipSlot; 

	Status _cBaseStatus;
	Status _cDiffStatus;

	public void InitEquipList(Hero hero)
	{
		_cHero = hero;
		Legion.Instance.cInventory.EquipSort();
		List<EquipmentItem> _lstEquip = Legion.Instance.cInventory.lstSortedEquipment;
		_au2ChangeEquipSlot = new UInt16[(int)EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2];

		bool check = false;
		EquipmentItem equipItem = null;
		Byte posID = 0;

		_cBaseStatus = new Status();
		_cBaseStatus.Add(hero.cFinalStatus);
		// 현재 캐릭터 장비 스테이터스 총합
		// 슬롯번호 초기화
		for(int i=(int)EquipmentInfo.EQUIPMENT_POS.HELMET; i<(int)EquipmentInfo.EQUIPMENT_POS.END; i++)
		{
			_cBaseStatus.Add(hero.acEquips[i-1].cFinalStatus);
			_au2ChangeEquipSlot[i-1] = hero.acEquips[i-1].u2SlotNum;
		}

		for(int i=0; i<_lstEquip.Count; i++)
		{
			equipItem = _lstEquip[i];
			posID = (Byte)((Byte)equipItem.GetEquipmentInfo().u1PosID-1);

			if(equipItem.GetEquipmentInfo().u1PosID == EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 ||
				equipItem.GetEquipmentInfo().u1PosID == EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
			{
				if(equipItem.attached.hero != null) continue; // 장비가 현재 착용중(자신 또는 다른 영웅 모두 포함)이면 대상에서 제외
				if(equipItem.GetEquipmentInfo().u2ClassID != (UInt16)ClassInfo.COMMON_CLASS_ID) continue; // 클래스정보가 공용이 아니면 제외
				if(equipItem.cLevel.u2Level > hero.cLevel.u2Level) continue; // 장비레벨이 영웅레벨보다 높을 수 없음

				// _au2ChangeEquipSlot에 전투력 수치가 가장 높은 아이템의 슬롯번호가 최종적으로 저장됨
				if(equipItem.u2Power > ((EquipmentItem)Legion.Instance.cInventory.dicInventory[_au2ChangeEquipSlot[posID]]).u2Power)
				{
					_au2ChangeEquipSlot[posID] = equipItem.u2SlotNum;
					check = true;
				}
			}
			else
			{
				if(equipItem.attached.hero != null) continue;
				if(equipItem.GetEquipmentInfo().u2ClassID != hero.cClass.u2ID) continue;
				if(equipItem.cLevel.u2Level > hero.cLevel.u2Level) continue;
				if(equipItem.u2Power > ((EquipmentItem)Legion.Instance.cInventory.dicInventory[_au2ChangeEquipSlot[posID]]).u2Power)
				{
					_au2ChangeEquipSlot[posID] = equipItem.u2SlotNum;
					check = true;
				}
			}
		}

		_cDiffStatus = new Status();
		_cDiffStatus.Add(hero.cFinalStatus);
		for(int i=(int)EquipmentInfo.EQUIPMENT_POS.HELMET; i<(int)EquipmentInfo.EQUIPMENT_POS.END; i++)
		{
			_cDiffStatus.Add(((EquipmentItem)Legion.Instance.cInventory.dicInventory[_au2ChangeEquipSlot[i-1]]).cFinalStatus);
		}

		if(_cBaseStatus.u4Power > _cDiffStatus.u4Power) check = false;

		if(check)
		{
			int diffValue = 0;
			for(byte i=0; i<7; i++)
			{
				byte type=(byte)(i+1);
				_txtStatType[i].text = Status.GetStatText(type);
				diffValue = _cDiffStatus.GetStat(type) - _cBaseStatus.GetStat(type);
				// 테스트용
//				_txtStatValue[i].text = string.Format("{0}", _cDummyHero.cFinalStatus.GetStat(type));
//				_txtStatValue[i].text = string.Format("{0}>{1}", hero.cFinalStatus.GetStat(i), _cDummyHero.cFinalStatus.GetStat(type));
				if(diffValue > 0)
				{
					_txtStatValue[i].text = string.Format("{0}", _cDiffStatus.GetStat(type));
					// 테스트용
//					_txtStatValue[i].text = string.Format("{0} (+{1})", _cDummyHero.cFinalStatus.GetStat(type), diffValue);
					_txtStatValue[i].color = color_up;
					_imgStatArrow[i].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.arrow_up");
					_imgStatArrow[i].gameObject.SetActive(true);
					
					
				}
				else if(diffValue < 0)
				{
					_txtStatValue[i].text = string.Format("{0}", _cBaseStatus.GetStat(type));
//					_txtStatValue[i].text = string.Format("{0} ({1})", _cDummyHero.cFinalStatus.GetStat(type), diffValue);
					_txtStatValue[i].color = color_down;
					_imgStatArrow[i].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.arrow_down");
					_imgStatArrow[i].gameObject.SetActive(true);
				}
				else
				{
					_txtStatValue[i].text = string.Format("{0}", _cBaseStatus.GetStat(type));
					_txtStatValue[i].color = Color.white;
					_imgStatArrow[i].gameObject.SetActive(false);
				}
			}
		}
		else
		{
			for(byte i=0; i<7; i++)
			{
				byte type=(byte)(i+1);
				_txtStatValue[i].text = string.Format("{0}", _cBaseStatus.GetStat(type));
				_txtStatValue[i].color = Color.white;
				_imgStatArrow[i].gameObject.SetActive(false);
			}
		}

		if(check)
		{
			_btnAutoEquipConfirm.interactable = true;
			_btnAutoEquipConfirm.image.sprite = _imgEnableButton;
			_txtBtnAutoEquipConfirm.GetComponent<Gradient>().StartColor = new Color32(255,255,255,255);
			_txtBtnAutoEquipConfirm.GetComponent<Gradient>().EndColor = new Color32(137,163,180,255);
			_txtBtnAutoEquipConfirm.SetAllDirty();
		}
		else
		{
			_btnAutoEquipConfirm.interactable = false;
			_btnAutoEquipConfirm.image.sprite = _imgDisableButton;
			_txtBtnAutoEquipConfirm.GetComponent<Gradient>().StartColor = new Color32(190,190,190,255);
			_txtBtnAutoEquipConfirm.GetComponent<Gradient>().EndColor = new Color32(102,102,102,255);
			_txtBtnAutoEquipConfirm.SetAllDirty();
		}
	}
	UInt16 SeqNo = 0;
	public void OnClick_AutoEquip()
	{
        bool isChangeEquip = false;
		DebugMgr.Log ("Change Equip Auto");
        for (int i=0; i<_cHero.acEquips.Length; i++)
		{
			if(_cHero.acEquips[i].u2SlotNum != _au2ChangeEquipSlot[i])
			{
				_cHero.ChangeEquip( (EquipmentItem)Legion.Instance.cInventory.dicInventory[_au2ChangeEquipSlot[i]]);
                Legion.Instance.cInventory.dicInventory[_au2ChangeEquipSlot[i]].isNew = false;
                //_cHero.ChangeWear (_posID, _cDiffEquipItem.u2SlotNum);
                //_cHero.cObject.GetComponent<HeroObject> ().ChangeEquip (_cBaseEquipItem, _cDiffEquipItem);
                //_cHero.cObject.GetComponent<HeroObject> ().SetAnimations_UI();
                isChangeEquip = true;
            }
		}
		UInt16[] changedSlots;
		if(_cHero == null)
			changedSlots = null;
		
		else
			changedSlots = _cHero.GetChangingEquip();
		
		//if (false)
		if (isChangeEquip || changedSlots != null)
		{
			//PopupManager.Instance.ShowLoadingPopup(1);
			//SeqNo = Server.ServerMgr.Instance.WearHero(_cHero, changedSlots, OnResponseFromServer);
            OnResponseFromServer(Server.ERROR_ID.NONE);
		}
		
		else
		{
            gameObject.SetActive(false);
        }
	}

	//인벤토리 닫기 서버응답 받음
	public void OnResponseFromServer(Server.ERROR_ID err)
	{
		DebugMgr.Log ("Change Equip Auto Success");

		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), err.ToString(), Server.ServerMgr.Instance.CallClear);
			return;
		}
		
		else if(err == Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			_cHero.cObject.GetComponent<HeroObject> ().SetAnimations_UI();
            _cParent.OnClickAttach();
            SeqNo = 0;
			_cHero = null;
			_cParent.InitEquipSlots();
		}
		gameObject.SetActive(false);
	}

}
