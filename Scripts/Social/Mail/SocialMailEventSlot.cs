using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class SocialMailEventSlot : MonoBehaviour
{
    public Image _itemIcon;
    public Text _itemName;
    public Text _timeLeft;

    MailList _MailGetItem;
    UInt16 _keyDic;
    Int64 timeTicks;

    ShopGoodInfo _goodsInfo;
    SocialMailTab _mailTab;

    public void SetData(MailList _Mail, UInt16 _key, SocialMailTab tab)
    {
        StringBuilder tempString = new StringBuilder();
        _goodsInfo = new ShopGoodInfo();
        _MailGetItem = new MailList();
        _MailGetItem = _Mail;
        _goodsInfo = ShopInfoMgr.Instance.dicShopGoodData[_MailGetItem.u2ItemID];
        _keyDic = _key;
        _mailTab = tab;
        //if (_MailGetItem.u2MailTitleCode == 0)
        tempString.Append(_MailGetItem.strMainTitle).Append(" ");
        _itemName.text = tempString.Append(TextManager.Instance.GetText(_goodsInfo.title)).ToString();

		timeTicks = SocialInfo.Instance.dicMailList[_keyDic].dtExpireTime.Ticks - Legion.Instance.ServerTime.Ticks; // DateTime.Now.Ticks;
        TimeSpan timespan = new TimeSpan(timeTicks);
        if(timespan.Days > 0)
            _timeLeft.text = string.Format(TextManager.Instance.GetText("mark_mail_time_left_day"), timespan.Days);
        else
            _timeLeft.text = string.Format(TextManager.Instance.GetText("mark_mail_time_left_hour"), timespan.Hours, timespan.Minutes);
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
        //슬롯이 없으면 뽑기 불가능
		if(!Legion.Instance.CheckEmptyInven())
        {
            //PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("btn_shop_main_gacha"), TextManager.Instance.GetText("popup_desc_full_inven"), null);
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
            MailGetItem[] _GetItem = new MailGetItem[SocialInfo.Instance.u2ItemCount];
            UInt16[] getItemSlotNums = new UInt16[_GetItem.Length];
            UInt16[] getItemIDs = new UInt16[_GetItem.Length];
            UInt32[] getItemCount = new UInt32[_GetItem.Length];

            for(int i=0; i<_GetItem.Length; i++)
            {
                _GetItem[i] = SocialInfo.Instance.dicMailGetItem[(UInt16)i];
            }
            for(int i=0; i<_GetItem.Length; i++)
            {
                if(_GetItem[i].u1ItemType == 10)
                {
			    	EquipmentInfo equipInfo = EquipmentInfoMgr.Instance.GetInfo(_GetItem[i].u2ItemID);
                    getItemSlotNums[i] = Legion.Instance.cInventory.AddEquipment(
			    		0, 0, _GetItem[i].u2ItemID, _GetItem[i].u2Level, 0, _GetItem[i].u1SkillSlot, _GetItem[i].u4Stat, 0, "", 
                        Legion.Instance.sName,equipInfo.u2ModelID, true, _GetItem[i].u1SmithingLevel, 0, 0, _GetItem[i].u1Completeness);
                    getItemIDs[i] = _GetItem[i].u2ItemID;
                    getItemCount[i] = _GetItem[i].u4Count;
                }
                else
                {
                    getItemSlotNums[i] = Legion.Instance.cInventory.AddItem(0, _GetItem[i].u2ItemID, _GetItem[i].u4Count);
                    getItemIDs[i] = _GetItem[i].u2ItemID;
                    getItemCount[i] = _GetItem[i].u4Count;
                }
            }

            _mailTab.InitGachaResult(getItemSlotNums, getItemIDs, getItemCount);
            SocialInfo.Instance.dicMailList[_keyDic].bCheckedMail = true;
            _mailTab.RefreashMailList(3);
            this.gameObject.SetActive(false);
        }
    }
}
