using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections.Generic;

public class SocialMailGoodsSlot : MonoBehaviour
{
    public Image _goodsIcon;
    public Text _itemName;
    public Text _timeLeft;
    public Sprite[] _goodsSprite;
    MailList _MailGetItem;
    UInt16 _keyDic;
    StringBuilder tempStringBuilder;
    Int64 timeTicks;
    SocialMailTab _mailTab;

    public void SetData(MailList _Mail, UInt16 _key, SocialMailTab tab)
    {
        tempStringBuilder = new StringBuilder();
        //_MailGetItem = new MailList();
        _MailGetItem = _Mail;
        _keyDic = _key;
        _mailTab = tab;

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
		tempStringBuilder.Append(_MailGetItem.strMainTitle);
		
        switch(_MailGetItem.u1ItemType)
        {
            case (Byte)GoodsType.GOLD:
                _goodsIcon.sprite = _goodsSprite[0];
                _goodsIcon.SetNativeSize();
                tempStringBuilder.Append(" ").Append(TextManager.Instance.GetText("mark_gold")).Append(" ");
                break;

            case (Byte)GoodsType.CASH:
                _goodsIcon.sprite = _goodsSprite[1];
                _goodsIcon.SetNativeSize();
                tempStringBuilder.Append(" ").Append(TextManager.Instance.GetText("mark_cash")).Append(" ");
                break;

            case (Byte)GoodsType.KEY:
                _goodsIcon.sprite = _goodsSprite[2];
                _goodsIcon.SetNativeSize();
                tempStringBuilder.Append(" ").Append(TextManager.Instance.GetText("mark_key")).Append(" ");
                break;

            case (Byte)GoodsType.LEAGUE_KEY:
                _goodsIcon.sprite = _goodsSprite[3];
                _goodsIcon.SetNativeSize();
                tempStringBuilder.Append(" ").Append(TextManager.Instance.GetText("mark_leaguekey")).Append(" ");
                break;

            case (Byte)GoodsType.FRIENDSHIP_POINT:
                _goodsIcon.sprite = _goodsSprite[4];
                _goodsIcon.SetNativeSize();
                tempStringBuilder.Append(" ").Append(TextManager.Instance.GetText("mark_friendshippoint")).Append(" ");
                break;
        }
        
        tempStringBuilder.Append(_MailGetItem.u4Count.ToString());
        _itemName.text = tempStringBuilder.ToString();
		timeTicks = SocialInfo.Instance.dicMailList[_keyDic].dtExpireTime.Ticks - Legion.Instance.ServerTime.Ticks;//DateTime.Now.Ticks;
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
		if(!Legion.Instance.CheckEmptyInven())
		{
			return;
		}
		if(Legion.Instance.CheckGoodsLimitExcessx(_MailGetItem.u1ItemType) == true)
		{
			Legion.Instance.ShowGoodsOverMessage(_MailGetItem.u1ItemType);
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
            //if(_MailGetItem.u1ItemType == 1)
            //    Legion.Instance.AddGoods(_MailGetItem.u1ItemType, _MailGetItem.u4Count);
            //else if(_MailGetItem.u1ItemType == 2)
            //    Legion.Instance.AddGoods(_MailGetItem.u1ItemType, _MailGetItem.u4Count);
            //else if(_MailGetItem.u1ItemType == 3)
            //    Legion.Instance.AddGoods(_MailGetItem.u1ItemType, _MailGetItem.u4Count);
            //else if(_MailGetItem.u1ItemType == 4)
                Legion.Instance.AddGoods(_MailGetItem.u1ItemType, _MailGetItem.u4Count);

            SocialInfo.Instance.dicMailList[_keyDic].bCheckedMail = true;
            _mailTab.RefreashMailList(2);
            this.gameObject.SetActive(false);
        }
    }

    public MailList GetMailItem()
    {
        return _MailGetItem;
    }
}
