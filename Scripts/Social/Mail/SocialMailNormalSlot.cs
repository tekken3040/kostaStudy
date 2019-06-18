using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class SocialMailNormalSlot : MonoBehaviour
{
    public GameObject _bg;
    public GameObject _element;
    public GameObject _cntBG;
    public Image _grade;
    public Image _icon;
    public Text _count;
    public Text _itemName;
    public Text _timeLeft;
    SocialMailTab _mailTab;

    MailList _MailGetItem;
    UInt16 _keyDic;
    Int64 timeTicks;
    StringBuilder tempStringBuilder;

    public Vector3[] _countPos;

    public void SetData(MailList _Mail, UInt16 _key, SocialMailTab tab)
    {
        tempStringBuilder = new StringBuilder();
        _MailGetItem = new MailList();
        _MailGetItem = _Mail;
        _keyDic = _key;
        _mailTab = tab;
		timeTicks = SocialInfo.Instance.dicMailList[_keyDic].dtExpireTime.Ticks - Legion.Instance.ServerTime.Ticks;//DateTime.Now.Ticks;
        TimeSpan timespan = new TimeSpan(timeTicks);
        if(timespan.Days > 0)
            _timeLeft.text = string.Format(TextManager.Instance.GetText("mark_mail_time_left_day"), timespan.Days);
        else
            _timeLeft.text = string.Format(TextManager.Instance.GetText("mark_mail_time_left_hour"), timespan.Hours, timespan.Minutes);

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(_MailGetItem.strMainTitle).Append(" ");
        switch(_MailGetItem.u1ItemType)
        {
            case (Byte)GoodsType.NONE:
                break;

            case (Byte)GoodsType.EQUIP:
                UInt16 modelID = EquipmentInfoMgr.Instance.GetInfo(_MailGetItem.u2ItemID).u2ModelID;
                if(modelID == 0) modelID = EquipmentInfoMgr.Instance.GetInfo(_MailGetItem.u2ItemID).u2ModelID;
                ModelInfo modelInfo = ModelInfoMgr.Instance.GetInfo(modelID);
                UInt16 gradeID = ForgeInfoMgr.Instance.GetList()[Mathf.Clamp(_MailGetItem.u1Grade,1,Server.ConstDef.MaxForgeLevel)-1].u2ID;
                _grade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + gradeID);
                _bg.transform.localScale = new Vector3(-Vector3.one.x, Vector3.one.y, Vector3.one.z);
                tempStringBuilder.Append(TextManager.Instance.GetText(EquipmentInfoMgr.Instance.GetInfo(_MailGetItem.u2ItemID).sName));
                _itemName.text = tempStringBuilder.ToString();
                _element.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + EquipmentInfoMgr.Instance.GetInfo(_MailGetItem.u2ItemID).u1Element);
                _count.text = _MailGetItem.u2Level.ToString();
                _icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/" + modelInfo.sImagePath);
                _cntBG.SetActive(false);
                _element.SetActive(true);
                _count.gameObject.transform.localPosition = _countPos[0];
                break;

            case (Byte)GoodsType.MATERIAL:
                _grade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(_MailGetItem.u2ItemID));
                _bg.transform.localScale = Vector3.one;
                tempStringBuilder.Append(TextManager.Instance.GetText(ItemInfoMgr.Instance.GetMaterialItemInfo(_MailGetItem.u2ItemID).sName));
                _itemName.text = tempStringBuilder.ToString();
                _count.text = _MailGetItem.u4Count.ToString();
				_icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + ItemInfoMgr.Instance.GetMaterialItemInfo(_MailGetItem.u2ItemID).u2IconID);
                _cntBG.SetActive(true);
                _element.SetActive(false);
                _count.gameObject.transform.localPosition = _countPos[1];
                break;

            case (Byte)GoodsType.CONSUME:
                _grade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(_MailGetItem.u2ItemID));
                _bg.transform.localScale = Vector3.one;
                tempStringBuilder.Append(TextManager.Instance.GetText(ItemInfoMgr.Instance.GetConsumableItemInfo(_MailGetItem.u2ItemID).sName));
                _itemName.text = tempStringBuilder.ToString();
                _count.text = _MailGetItem.u4Count.ToString();
                _icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + _MailGetItem.u2ItemID);
                _cntBG.SetActive(true);
                _element.SetActive(false);
                _count.gameObject.transform.localPosition = _countPos[1];
                break;

            case (Byte)ItemInfo.ITEM_TYPE.RUNE:
                _grade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(_MailGetItem.u1ItemType));
                _bg.transform.localScale = Vector3.one;
                tempStringBuilder.Append(TextManager.Instance.GetText(ItemInfoMgr.Instance.GetRuneInfo(_MailGetItem.u2ItemID).sName));
                _itemName.text = tempStringBuilder.ToString();
                _count.text = _MailGetItem.u4Count.ToString();
                _icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/rune_01." + _MailGetItem.u2ItemID);
                break;

			case (Byte)GoodsType.EVENT_ITEM:
				_grade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(_MailGetItem.u1ItemType));
				_bg.transform.localScale = Vector3.one;
				tempStringBuilder.Append(TextManager.Instance.GetText(EventInfoMgr.Instance.dicMarbleGoods[_MailGetItem.u2ItemID].sName));
				_itemName.text = tempStringBuilder.ToString();
				_count.text = _MailGetItem.u4Count.ToString();
				_icon.sprite = AtlasMgr.Instance.GetGoodsIcon(new Goods(_MailGetItem.u1ItemType,_MailGetItem.u2ItemID, _MailGetItem.u4Count));
				break;
            case (Byte)GoodsType.CHARACTER_PACKAGE:
                _grade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_4570");
                _bg.transform.localScale = Vector3.one;
                tempStringBuilder.Append(TextManager.Instance.GetText(ClassInfoMgr.Instance.GetInfo(EventInfoMgr.Instance.dicClassGoods[_MailGetItem.u2ItemID].u2ClassID).sName));
                _itemName.text = tempStringBuilder.ToString();
                _count.text = _MailGetItem.u4Count.ToString();
                _icon.sprite = AtlasMgr.Instance.GetGoodsIcon(new Goods(_MailGetItem.u1ItemType, _MailGetItem.u2ItemID, _MailGetItem.u4Count));
                break;
            case (Byte)GoodsType.CHARACTER_AVAILABLE:
                _grade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_4570");
                _bg.transform.localScale = Vector3.one;
                tempStringBuilder.Append(TextManager.Instance.GetText(ClassInfoMgr.Instance.GetInfo(_MailGetItem.u2ItemID).sName));
                _itemName.text = tempStringBuilder.ToString();
                _count.text = _MailGetItem.u4Count.ToString();
                _icon.sprite = AtlasMgr.Instance.GetSprite(string.Format("Sprites/Hero/hero_icon.{0}", _MailGetItem.u2ItemID));
                break;
            case (Byte)GoodsType.EQUIP_GOODS:
                ClassGoodsEquipInfo ginfo = EventInfoMgr.Instance.dicClassGoodsEquip[_MailGetItem.u2ItemID];
                EquipmentInfo equipInfo = EquipmentInfoMgr.Instance.GetInfo(ginfo.u2Equip);
                UInt16 modelID2 = equipInfo.u2ModelID;
                if(modelID2 == 0) modelID2 = equipInfo.u2ModelID;
                ModelInfo modelInfo2 = ModelInfoMgr.Instance.GetInfo(modelID2);
                UInt16 gradeID2 = ForgeInfoMgr.Instance.GetList()[Mathf.Clamp(ginfo.u1SmithingLevel,1,Server.ConstDef.MaxForgeLevel)-1].u2ID;
                _grade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + gradeID2);
                _bg.transform.localScale = new Vector3(-Vector3.one.x, Vector3.one.y, Vector3.one.z);
                tempStringBuilder.Append(TextManager.Instance.GetText(equipInfo.sName));
                _itemName.text = tempStringBuilder.ToString();
                _element.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + equipInfo.u1Element);
                _count.text = ginfo.u2Level.ToString();
                _icon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/" + modelInfo2.sImagePath);
                _cntBG.SetActive(false);
                _element.SetActive(true);
                _count.gameObject.transform.localPosition = _countPos[0];
                break;
        }
    }

    public void OnClickReciveItem()
    {
        timeTicks = SocialInfo.Instance.dicMailList[_keyDic].dtExpireTime.Ticks - Legion.Instance.ServerTime.Ticks;
        if(timeTicks  <= 0)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("mail_limit_over_title"),TextManager.Instance.GetText("mail_limit_over_desc"), null);
            this.gameObject.SetActive(false);
            return;
        }
        // 현재 우편을 받아도 이상이 없을지 체크
        if (Legion.Instance.CheckGoodsLimitExcessx(_MailGetItem.u1ItemType))
        {
            Legion.Instance.ShowGoodsOverMessage(_MailGetItem.u1ItemType);
            return;
        }
		if(!Legion.Instance.CheckEmptyInven())
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
            switch(_MailGetItem.u1ItemType)
            {
                case (Byte)GoodsType.NONE:
                    break;

                case (Byte)GoodsType.EQUIP:
                    MailGetItem _GetItem = new MailGetItem();
                    _GetItem.u4Stat = new UInt32[Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType * 2];
                    _GetItem.u1ItemType = SocialInfo.Instance.dicMailGetItem[0].u1ItemType;
                    _GetItem.u2ItemID = SocialInfo.Instance.dicMailGetItem[0].u2ItemID;
                    _GetItem.u4Count = SocialInfo.Instance.dicMailGetItem[0].u4Count;
                    _GetItem.u1SmithingLevel = SocialInfo.Instance.dicMailGetItem[0].u1SmithingLevel;
                    _GetItem.u2ModelID = SocialInfo.Instance.dicMailGetItem[0].u2ModelID;
                    _GetItem.u2Level = SocialInfo.Instance.dicMailGetItem[0].u2Level;
                    _GetItem.u1Completeness = SocialInfo.Instance.dicMailGetItem[0].u1Completeness;
                    _GetItem.u4Stat = SocialInfo.Instance.dicMailGetItem[0].u4Stat;
                    //for(int i=0; i<Server.ConstDef.SkillOfEquip; i++)
                    //{
                    //    _GetItem.u4Stat[i] = SocialInfo.Instance.dicMailGetItem[0].u4Stat[i];
                    //}
                    _GetItem.u1SkillCount = SocialInfo.Instance.dicMailGetItem[0].u1SkillCount;
                    _GetItem.u1SkillSlot = new Byte[/*_GetItem.u1SkillCount*/Server.ConstDef.SkillOfEquip];
                    for(int i=0; i<_GetItem.u1SkillCount; i++)
                    {
                        _GetItem.u1SkillSlot[i] = SocialInfo.Instance.dicMailGetItem[0].u1SkillSlot[i];
                    }
                    
					EquipmentInfo equipInfo = EquipmentInfoMgr.Instance.GetInfo(_GetItem.u2ItemID);

					Legion.Instance.cInventory.AddEquipment(
					0, 0, _GetItem.u2ItemID, _GetItem.u2Level, 0, _GetItem.u1SkillSlot, _GetItem.u4Stat, 0, "", Legion.Instance.sName,equipInfo.u2ModelID, true,
					_GetItem.u1SmithingLevel, 0, 0, _GetItem.u1Completeness);

					Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.EquipLevel, equipInfo.u2ID, (Byte)equipInfo.u1PosID, 0, 0, _GetItem.u2Level);

					Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.GetEquip, equipInfo.u2ID, (Byte)equipInfo.u1PosID, 
																_GetItem.u1SmithingLevel, (Byte)equipInfo.u2ClassID, 1);
                    break;

                case (Byte)GoodsType.MATERIAL:
                    Legion.Instance.cInventory.AddItem(0, _MailGetItem.u2ItemID, _MailGetItem.u4Count);
                    break;

                case (Byte)GoodsType.CONSUME:
                    Legion.Instance.cInventory.AddItem(0, _MailGetItem.u2ItemID, _MailGetItem.u4Count);
                    break;

                case (Byte)ItemInfo.ITEM_TYPE.RUNE:
                    
                    break;
				case (Byte)GoodsType.EVENT_ITEM:
                case (Byte)GoodsType.CHARACTER_PACKAGE:
                case (Byte)GoodsType.CHARACTER_AVAILABLE:
                    Legion.Instance.AddGoods(new Goods(_MailGetItem.u1ItemType, _MailGetItem.u2ItemID, _MailGetItem.u4Count));
					break;
                case (Byte)GoodsType.EQUIP_GOODS:
                    Legion.Instance.AddGoods(new Goods(_MailGetItem.u1ItemType, _MailGetItem.u2ItemID, _MailGetItem.u4Count));
                    break;
            }
            SocialInfo.Instance.dicMailList[_keyDic].bCheckedMail = true;
            _mailTab.RefreashMailList(1);
            this.gameObject.SetActive(false);
        }
    }

    public MailList GetMailItem()
    {
        return _MailGetItem;
    }
}
