using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections.Generic;

public class SocialMailNoticeSlot : MonoBehaviour
{
    public Image _element;
    public Image _grade;
    public Image _icon;
    public Text _count;
    public Text _itemName;
    public Text _timeLeft;

    MailList _MailGetItem;
    UInt16 _keyDic;
    Int64 timeTicks;
    EquipmentItem _itemInfo;
    SocialMailTab _mailTab;

    public void SetData(MailList _Mail, UInt16 _key, SocialMailTab tab)
    {
        _MailGetItem = new MailList();
        _MailGetItem = _Mail;
        _keyDic = _key;
        _mailTab = tab;

        if(_MailGetItem.u2Slot != 0)
        {
            for(int i=0; i<Legion.Instance.cInventory.lstInShop.Count; i++)
            {
                if(Legion.Instance.cInventory.lstInShop.ContainsKey(_MailGetItem.u2Slot))
                    _itemInfo = (EquipmentItem)Legion.Instance.cInventory.lstInShop[_MailGetItem.u2Slot];
            }

            _element.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + _itemInfo.GetEquipmentInfo().u1Element);

            UInt16 gradeID = ForgeInfoMgr.Instance.GetList()[Mathf.Clamp(_itemInfo.u1SmithingLevel,1,Server.ConstDef.MaxForgeLevel)-1].u2ID;
            _grade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + gradeID);

            UInt16 modelID = _itemInfo.u2ModelID;
            if(modelID == 0) modelID = _itemInfo.GetEquipmentInfo().u2ModelID;
            ModelInfo modelInfo = ModelInfoMgr.Instance.GetInfo(modelID);
            _icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/" + modelInfo.sImagePath);

            _count.text = _itemInfo.cLevel.u2Level.ToString();
            _itemName.text = TextManager.Instance.GetText(_itemInfo.cItemInfo.sName);
			timeTicks = SocialInfo.Instance.dicMailList[_keyDic].dtExpireTime.Ticks - Legion.Instance.ServerTime.Ticks;//DateTime.Now.Ticks;
            TimeSpan timespan = new TimeSpan(timeTicks);
            if(timespan.Days > 0)
                _timeLeft.text = string.Format(TextManager.Instance.GetText("mark_mail_time_left_day"), timespan.Days);
            else
                _timeLeft.text = string.Format(TextManager.Instance.GetText("mark_mail_time_left_hour"), timespan.Hours, timespan.Minutes);
        }
    }

    public void OnClickReciveItem()
    {
        timeTicks = SocialInfo.Instance.dicMailList[_keyDic].dtExpireTime.Ticks - Legion.Instance.ServerTime.Ticks;
        if(timeTicks  <= 0)
        {
            PopupManager.Instance.ShowOKPopup("만료", "만료된 우편입니다.", null);
            this.gameObject.SetActive(false);
            return;
        }
		if(!Legion.Instance.CheckEmptyInven())
		{
			return;
		}
        PopupManager.Instance.ShowLoadingPopup(1);
        UInt16[] u2MailSN = new UInt16[1];
        u2MailSN[0] = _MailGetItem.u2MailSN;
        Server.ServerMgr.Instance.RequestGetItemInMail(u2MailSN, SuccessRecvItem);
    }

    public void SuccessRecvItem(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.MAIL_GET, err), Server.ServerMgr.Instance.CallClear);
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            UInt32[] stats = new UInt32[Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType * 2];
            for (int i = 0; i < Server.ConstDef.EquipStatPointType; i++)
			{
                stats[i + Server.ConstDef.SkillOfEquip] = Convert.ToUInt16(_itemInfo.GetComponent<StatusComponent>().EquipBase.GetStat(_itemInfo.GetComponent<StatusComponent>().au1StatType[i]));
			}
            for(int i=0; i<Server.ConstDef.SkillOfEquip; i++)
            {
                stats[i] = Convert.ToUInt16(_itemInfo.GetComponent<StatusComponent>().points[i]);
            }
            for (int i = 0; i < Server.ConstDef.EquipStatPointType; i++)
			{
                stats[i + Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType] = Convert.ToUInt16(_itemInfo.GetComponent<StatusComponent>().points[i]);
			}
            if(Legion.Instance.cInventory.dicInventory.ContainsKey(_itemInfo.u2SlotNum))
                Legion.Instance.cInventory.dicInventory.Remove(_itemInfo.u2SlotNum);
            Legion.Instance.cInventory.AddEquipment(_itemInfo.u2SlotNum, 0, _itemInfo.cItemInfo.u2ID, _itemInfo.cLevel.u2Level, _itemInfo.cLevel.u8Exp, 
                _itemInfo.skillSlots, stats, _itemInfo.GetComponent<StatusComponent>().ResetCount,
                _itemInfo.itemName, _itemInfo.createrName, _itemInfo.u2ModelID, _itemInfo.isNew, _itemInfo.u1SmithingLevel, _itemInfo.u2UnsetStatPoint, _itemInfo.u2StatPointExp, _itemInfo.u1Completeness);
            Legion.Instance.cInventory.lstInShop.Remove(_itemInfo.u2SlotNum);
            SocialInfo.Instance.dicMailList[_keyDic].bCheckedMail = true;
            _mailTab.RefreashMailList(4);
            this.gameObject.SetActive(false);
        }
    }
}
