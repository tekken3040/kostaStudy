using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class SocialMailTab : MonoBehaviour
{
    public SocialPanel _socialPanel;
    public GameObject BtnGroupMailNormal;
    public GameObject BtnGroupMailGoods;
    public GameObject objPrefMainNormalList;
    public GameObject objPrefMailGoodsList;
    public GameObject objPrefMailGachaList;
    public GameObject objPrefMailNoticeList;
    public GameObject ScrollList;
    public GameObject EmptyMailText;
    public GameObject PrefGachaResult;
    public GameObject PrefGachaResultEffect;
    public GameObject[] objAlram;
    public Toggle[] _mailMenu;

    List<UInt16> recvCnt = new List<UInt16>();
    Int64 u8TimeTick = 0;

    public void OnEnable()
    {
        _mailMenu[0].isOn = true;
        if(!_socialPanel._recvMailList)
        {
            RequestMailList();
            _socialPanel._recvMailList = true;
        }

        else
        {
            ReciveMailList(Server.ERROR_ID.NONE);
        }
    }

    public void OnDisable()
    {
        Byte tempu1MailExist = Legion.Instance.u1MailExist;
        bool bBailExist = false;
        for(int i=0; i<SocialInfo.Instance.u2MailCount; i++)
        {
            if(SocialInfo.Instance.dicMailList[(UInt16)i].bCheckedMail)
                bBailExist = false;
            else
            {
                bBailExist = true;
                break;
            }
        }
        if(bBailExist)
            Legion.Instance.u1MailExist = tempu1MailExist;
        else
            Legion.Instance.u1MailExist = 0;
    }
    public void RequestMailList()
    {
        PopupManager.Instance.ShowLoadingPopup(1);
        Server.ServerMgr.Instance.RequestMailList(ReciveMailList);
    }

    public void ReciveMailList(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
			PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.MAIL_LIST, err), Server.ServerMgr.Instance.CallClear);
		}
        else
        {
            for(int i = 0; i < _mailMenu.Length; ++i)
            {
                if (_mailMenu[i] == null)
                    continue;

                if(i == 0)
                {
                    _mailMenu[i].isOn = true;
                }
                else
                {
                    _mailMenu[i].isOn = false;
                }
            }
            
            OnClickMailMenu(0);
        }
    }

    public void ClearList()
    {
        if(ScrollList == null)
        {
            return;
        }

        for(int i=0; i<ScrollList.transform.childCount; i++)
        {
            GameObject.Destroy(ScrollList.transform.GetChild(i).gameObject);
        }
    }

    public void OnClickMailMenu(int _menu)
    {
        ClearList();
        if(SocialInfo.Instance.u2MailCount == 0)
        {
            for(int i=0; i<objAlram.Length; i++)
                objAlram[i].SetActive(false);
            //for(int i=0; i<_socialPanel.Alrams.Length; i++)
            _socialPanel.Alrams[1].SetActive(false);
            _socialPanel.Alrams[4].SetActive(false);
            EmptyMailText.SetActive(true);
            return;
        }
        for(int i=0; i<objAlram.Length; i++)
            objAlram[i].SetActive(false);
        for(int i=0; i<SocialInfo.Instance.u2MailCount; i++)
        {
            if(!SocialInfo.Instance.dicMailList[(UInt16)i].bCheckedMail)
            {
                if(SocialInfo.Instance.dicMailList[(UInt16)i].u1MailType == 1 && objAlram[0].activeSelf)
                    continue;
                else if(SocialInfo.Instance.dicMailList[(UInt16)i].u1MailType == 2 && objAlram[1].activeSelf)
                    continue;
                else if(SocialInfo.Instance.dicMailList[(UInt16)i].u1MailType == 3 && objAlram[2].activeSelf)
                    continue;
                else if(SocialInfo.Instance.dicMailList[(UInt16)i].u1MailType == 4 && objAlram[3].activeSelf)
                    continue;

                switch(SocialInfo.Instance.dicMailList[(UInt16)i].u1MailType)
                {
                    case 1:
                        objAlram[0].SetActive(true);
                        continue;
                    case 2:
                        objAlram[1].SetActive(true);
                        continue;
                    case 3:
                        objAlram[2].SetActive(true);
                        continue;
                    case 4:
                        objAlram[3].SetActive(true);
                        continue;
                }
            }
        }
        for(int i=0; i<SocialInfo.Instance.u2MailCount; i++)
        {
            if(SocialInfo.Instance.dicMailList[(UInt16)i].bCheckedMail)
            {
                _socialPanel.Alrams[1].SetActive(false);
                _socialPanel.Alrams[4].SetActive(false);
            }
            else
            {
                _socialPanel.Alrams[1].SetActive(true);
                _socialPanel.Alrams[4].SetActive(true);
                break;
            }
        }
        for(int i=0; i<SocialInfo.Instance.u2MailCount; i++)
        {
            if((_menu + 1) == SocialInfo.Instance.dicMailList[(UInt16)i].u1MailType)
            {
                if(SocialInfo.Instance.dicMailList[(UInt16)i].bCheckedMail)
                {
                    EmptyMailText.SetActive(true);
                }
                else
                {
                    EmptyMailText.SetActive(false);
                    break;
                }
            }
            else
            {
                EmptyMailText.SetActive(true);
            }
        }
        if(!_mailMenu[_menu].isOn)
            return;

        switch(_menu)
        {
            case 0:
                BtnGroupMailNormal.SetActive(true);
                BtnGroupMailGoods.SetActive(false);
                _mailMenu[0].interactable = false;
                _mailMenu[1].interactable = true;
                _mailMenu[2].interactable = true;
                _mailMenu[3].interactable = true;
                InitMailNormalList();
                if(SocialInfo.Instance.u2MailCount == 0)
                    BtnGroupMailNormal.transform.GetChild(0).GetComponent<Button>().interactable = false;
                else
                for(int i=0; i<SocialInfo.Instance.u2MailCount; i++)
                {
                    if(SocialInfo.Instance.dicMailList[(UInt16)i].u1MailType == 1)
                    {
                        if(!SocialInfo.Instance.dicMailList[(UInt16)i].bCheckedMail)
                        {
                            BtnGroupMailNormal.transform.GetChild(0).GetComponent<Button>().interactable = true;
                            break;
                        }
                        else
                        {
                            BtnGroupMailNormal.transform.GetChild(0).GetComponent<Button>().interactable = false;
                        }   
                    }
                    BtnGroupMailNormal.transform.GetChild(0).GetComponent<Button>().interactable = false;
                }
                
                break;

            case 1:
                BtnGroupMailNormal.SetActive(false);
                BtnGroupMailGoods.SetActive(true);
                _mailMenu[0].interactable = true;
                _mailMenu[1].interactable = false;
                _mailMenu[2].interactable = true;
                _mailMenu[3].interactable = true;
                InitMailGoodsList();
                if(SocialInfo.Instance.u2MailCount == 0)
                    BtnGroupMailGoods.transform.GetChild(0).GetComponent<Button>().interactable = false;
                for(int i=0; i<SocialInfo.Instance.u2MailCount; i++)
                {
                    if(SocialInfo.Instance.dicMailList[(UInt16)i].u1MailType == 2)
                    {
                        if(!SocialInfo.Instance.dicMailList[(UInt16)i].bCheckedMail)
                        {
                            BtnGroupMailGoods.transform.GetChild(0).GetComponent<Button>().interactable = true;
                            break;
                        }
                        else
                        {
                            BtnGroupMailGoods.transform.GetChild(0).GetComponent<Button>().interactable = false;
                        }   
                    }
                    BtnGroupMailGoods.transform.GetChild(0).GetComponent<Button>().interactable = false;
                }
                break;

            case 2:
                BtnGroupMailNormal.SetActive(false);
                BtnGroupMailGoods.SetActive(false);
                _mailMenu[0].interactable = true;
                _mailMenu[1].interactable = true;
                _mailMenu[2].interactable = false;
                _mailMenu[3].interactable = true;
                InitMailGachaList();
                break;

            case 3:
                BtnGroupMailNormal.SetActive(false);
                BtnGroupMailGoods.SetActive(false);
                _mailMenu[0].interactable = true;
                _mailMenu[1].interactable = true;
                _mailMenu[2].interactable = true;
                _mailMenu[3].interactable = false;
                InitMailNoticeList();
                break;
        }
    }

    public void RefreashMailList(Byte u1MailType)
    {
        for(int i=0; i<SocialInfo.Instance.u2MailCount; i++)
        {
            if(SocialInfo.Instance.dicMailList[(UInt16)i].bCheckedMail)
            {
                _socialPanel.Alrams[1].SetActive(false);
                _socialPanel.Alrams[4].SetActive(false);
                continue;
            }
            else
            {
                _socialPanel.Alrams[1].SetActive(true);
                _socialPanel.Alrams[4].SetActive(true);
                break;
            }
        }
        
        for(int i=0; i<SocialInfo.Instance.u2MailCount; i++)
        {
            if(SocialInfo.Instance.dicMailList[(UInt16)i].bCheckedMail)
            {
                EmptyMailText.SetActive(true);
                objAlram[u1MailType-1].SetActive(false);
                if(u1MailType == 1)
                    BtnGroupMailNormal.transform.GetChild(0).GetComponent<Button>().interactable = false;
                else if(u1MailType == 2)
                    BtnGroupMailGoods.transform.GetChild(0).GetComponent<Button>().interactable = false;
                continue;
            }
            else if(SocialInfo.Instance.dicMailList[(UInt16)i].u1MailType == u1MailType)
            {
                EmptyMailText.SetActive(false);
                objAlram[u1MailType-1].SetActive(true);
                if(u1MailType == 1)
                    BtnGroupMailNormal.transform.GetChild(0).GetComponent<Button>().interactable = true;
                else if(u1MailType == 2)
                    BtnGroupMailGoods.transform.GetChild(0).GetComponent<Button>().interactable = true;
                break;
            }
        }
    }

    //일반 우편=====================================================
    public void InitMailNormalList()
    {
        if(SocialInfo.Instance.u2MailCount == 0)
            EmptyMailText.SetActive(true);
        else
        {
            RefreashMailList(1);
        }
        for(int i=0; i<SocialInfo.Instance.u2MailCount; i++)
        {
            if(SocialInfo.Instance.dicMailList[(UInt16)i].bCheckedMail)
                continue;
            u8TimeTick = SocialInfo.Instance.dicMailList[(UInt16)i].dtExpireTime.Ticks - Legion.Instance.ServerTime.Ticks;
            if(u8TimeTick <= 0)
                continue;
            if(SocialInfo.Instance.dicMailList[(UInt16)i].u1MailType == 1)
            {
                GameObject objMailNormal = Instantiate(objPrefMainNormalList);
                objMailNormal.transform.SetParent(ScrollList.transform);
                objMailNormal.transform.localPosition = Vector3.zero;
                objMailNormal.transform.localScale = Vector3.one;
                objMailNormal.GetComponent<SocialMailNormalSlot>().SetData(SocialInfo.Instance.dicMailList[(UInt16)i], (UInt16)i, this.GetComponent<SocialMailTab>());
            }
        }
    }
    
    public void OnClickReciveAllNormalMail()
    {
		if(!Legion.Instance.CheckEmptyInven())
		{
			return;
		}

        PopupManager.Instance.ShowLoadingPopup(1);
        recvCnt.Clear();
        for(int i=0; i<SocialInfo.Instance.u2MailCount; i++)
        {
            if(SocialInfo.Instance.dicMailList[(UInt16)i].bCheckedMail)
                continue;

            if(SocialInfo.Instance.dicMailList[(UInt16)i].u1MailType == 1)
            {
                UInt16 _mailSN = SocialInfo.Instance.dicMailList[(UInt16)i].u2MailSN;
                recvCnt.Add(_mailSN);
            } 
        }
        if(recvCnt.Count == 0)
            return;
        UInt16[] mailSN = new UInt16[recvCnt.Count];
        for(int i=0; i<recvCnt.Count; i++)
        {
            mailSN[i] = recvCnt[i];
        }
        Server.ServerMgr.Instance.RequestGetItemInMail(mailSN, SuccessReciveAllNormalMail);
    }

    public void SuccessReciveAllNormalMail(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.MAIL_GET, err), Server.ServerMgr.Instance.CallClear);
        }

        else if(err == Server.ERROR_ID.NONE)
        {
            Legion.Instance.u1MailExist = 0;
            objAlram[0].SetActive(false);
            BtnGroupMailNormal.transform.GetChild(0).GetComponent<Button>().interactable = false;
            EmptyMailText.SetActive(true);
            for(int i=0; i<SocialInfo.Instance.dicMailList.Count; i++)
            {
                if(SocialInfo.Instance.dicMailList[(UInt16)i].u1MailType == 1)
                    SocialInfo.Instance.dicMailList[(UInt16)i].bCheckedMail = true;
            }
            for(int i=0; i<SocialInfo.Instance.dicMailList.Count; i++)
            {
                if(SocialInfo.Instance.dicMailList[(UInt16)i].bCheckedMail)
                {
                    _socialPanel.Alrams[1].SetActive(false);
                    _socialPanel.Alrams[4].SetActive(false);
                }
                else
                {
                    _socialPanel.Alrams[1].SetActive(true);
                    _socialPanel.Alrams[4].SetActive(true);
                    break;
                }
            }
            for(int i=0; i<SocialInfo.Instance.dicMailList.Count; i++)
            {
                if(SocialInfo.Instance.dicMailList[(UInt16)i].u1MailType == 1)
                {
                    switch(SocialInfo.Instance.dicMailList[(UInt16)i].u1ItemType)
                    {
                        case (Byte)GoodsType.NONE:
                            break;

                        case (Byte)GoodsType.MATERIAL:
                            Legion.Instance.cInventory.AddItem(0, SocialInfo.Instance.dicMailList[(UInt16)i].u2ItemID, SocialInfo.Instance.dicMailList[(UInt16)i].u4Count);
                            break;

                        case (Byte)GoodsType.CONSUME:
                            Legion.Instance.cInventory.AddItem(0, SocialInfo.Instance.dicMailList[(UInt16)i].u2ItemID, SocialInfo.Instance.dicMailList[(UInt16)i].u4Count);
                            break;

                        case (Byte)ItemInfo.ITEM_TYPE.RUNE:
                            break;
						case (Byte)GoodsType.EVENT_ITEM:
							Legion.Instance.AddGoods(new Goods(SocialInfo.Instance.dicMailList[(UInt16)i].u1ItemType,
																SocialInfo.Instance.dicMailList[(UInt16)i].u2ItemID,
																SocialInfo.Instance.dicMailList[(UInt16)i].u4Count));
							break;
                        case (Byte)GoodsType.EQUIP_GOODS:
                            break;
                    }
                }
            }
            for(int i=0; i<SocialInfo.Instance.dicMailGetItem.Count; i++)
            {
                switch(SocialInfo.Instance.dicMailGetItem[(UInt16)i].u1ItemType)
                {
                    case (Byte)GoodsType.NONE:
                        break;

                    case (Byte)GoodsType.EQUIP:
					
//                        Legion.Instance.cInventory.AddEquipment(0, SocialInfo.Instance.dicMailGetItem[(UInt16)i].u2ItemID, SocialInfo.Instance.dicMailGetItem[(UInt16)i].u1SkillSlot, SocialInfo.Instance.dicMailGetItem[(UInt16)i].u4Stat, "", "", SocialInfo.Instance.dicMailGetItem[(UInt16)i].u2ModelID, SocialInfo.Instance.dicMailGetItem[(UInt16)i].u1SmithingLevel);
						EquipmentInfo equipInfo = EquipmentInfoMgr.Instance.GetInfo(SocialInfo.Instance.dicMailGetItem[(UInt16)i].u2ItemID);

						Legion.Instance.cInventory.AddEquipment(
						0, 0, equipInfo.u2ID, SocialInfo.Instance.dicMailGetItem[(UInt16)i].u2Level, 0, SocialInfo.Instance.dicMailGetItem[(UInt16)i].u1SkillSlot,
						SocialInfo.Instance.dicMailGetItem[(UInt16)i].u4Stat, 0, "", Legion.Instance.sName,equipInfo.u2ModelID, true,
						SocialInfo.Instance.dicMailGetItem[(UInt16)i].u1SmithingLevel, 0, 0, SocialInfo.Instance.dicMailGetItem[(UInt16)i].u1Completeness);

						Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.EquipLevel, equipInfo.u2ID, (Byte)equipInfo.u1PosID, 0, 0, SocialInfo.Instance.dicMailGetItem[(UInt16)i].u2Level);

						Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.GetEquip, equipInfo.u2ID, (Byte)equipInfo.u1PosID, 
																	SocialInfo.Instance.dicMailGetItem[(UInt16)i].u1SmithingLevel, (Byte)equipInfo.u2ClassID, 1);
					break;

                    case (Byte)GoodsType.MATERIAL:
                        //Legion.Instance.cInventory.AddItem(0, SocialInfo.Instance.dicMailGetItem[(UInt16)i].u2ItemID, SocialInfo.Instance.dicMailGetItem[(UInt16)i].u4Count);
                        break;

                    case (Byte)GoodsType.CONSUME:
                        //Legion.Instance.cInventory.AddItem(0, SocialInfo.Instance.dicMailGetItem[(UInt16)i].u2ItemID, SocialInfo.Instance.dicMailGetItem[(UInt16)i].u4Count);
                        break;

                    case (Byte)ItemInfo.ITEM_TYPE.RUNE:
                        break;

                    case (Byte)GoodsType.EQUIP_GOODS:
                        Legion.Instance.AddGoods(new Goods(SocialInfo.Instance.dicMailGetItem[(UInt16)i].u1ItemType,
																SocialInfo.Instance.dicMailGetItem[(UInt16)i].u2ItemID,
																SocialInfo.Instance.dicMailGetItem[(UInt16)i].u4Count));
                        break;
                }
            }
            for(int i=0; i<ScrollList.transform.GetChildCount(); i++)
            {
                ScrollList.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
    //일반 우편End==================================================

    //재화 우편=====================================================
    public void InitMailGoodsList()
    {
        if(SocialInfo.Instance.u2MailCount == 0)
            EmptyMailText.SetActive(true);
        else
        {
            RefreashMailList(2);
        }
        for(int i=0; i<SocialInfo.Instance.u2MailCount; i++)
        {
            if(SocialInfo.Instance.dicMailList[(UInt16)i].bCheckedMail)
                continue;
            u8TimeTick = SocialInfo.Instance.dicMailList[(UInt16)i].dtExpireTime.Ticks - Legion.Instance.ServerTime.Ticks;
            if(u8TimeTick <= 0)
                continue;
            if(SocialInfo.Instance.dicMailList[(UInt16)i].u1MailType == 2)
            {
                GameObject objMailGoods = Instantiate(objPrefMailGoodsList);
				objMailGoods.transform.SetParent(ScrollList.transform);
                objMailGoods.transform.localPosition = Vector3.zero;
                objMailGoods.transform.localScale = Vector3.one;
                objMailGoods.GetComponent<SocialMailGoodsSlot>().SetData(SocialInfo.Instance.dicMailList[(UInt16)i], (UInt16)i, this.GetComponent<SocialMailTab>());
            }
        }
    }

    public void OnClickReciveAllGoodsMail()
    {
		if(!Legion.Instance.CheckEmptyInven())
		{
			return;
		}

        PopupManager.Instance.ShowLoadingPopup(1);
        recvCnt.Clear();

		bool isOverGoods = false;
		Dictionary<Byte, Goods> dicGoods = new Dictionary<Byte, Goods>();
        for(int i=0; i<SocialInfo.Instance.u2MailCount; i++)
        {
            if(SocialInfo.Instance.dicMailList[(UInt16)i].bCheckedMail)
                continue;
			
            if(SocialInfo.Instance.dicMailList[(UInt16)i].u1MailType == 2)
            {
				// 2016. 10. 17 jy 재화 오버 막기
				Goods mailGoods = new Goods(SocialInfo.Instance.dicMailList[(UInt16)i].u1ItemType,
					SocialInfo.Instance.dicMailList[(UInt16)i].u2ItemID,
					SocialInfo.Instance.dicMailList[(UInt16)i].u4Count);

				if(dicGoods.ContainsKey(mailGoods.u1Type))
				{
					dicGoods[mailGoods.u1Type].u4Count += mailGoods.u4Count;
					if(Legion.Instance.CheckGoodsLimitExcessx(mailGoods) == true)
					{
						dicGoods[mailGoods.u1Type].u4Count -= mailGoods.u4Count;
						continue;
					}
				}
				else
				{
					if(Legion.Instance.CheckGoodsLimitExcessx(mailGoods.u1Type) == true)
						continue;
					else
						dicGoods.Add(mailGoods.u1Type, mailGoods);
				}
				/*
				if(Legion.Instance.CheckGoodsLimitExcessx(SocialInfo.Instance.dicMailList[(UInt16)i].u1ItemType) == false)
				{
					if(dicGoods.ContainsKey(mailGoods.u1Type))
					{
						mailGoods.u4Count += dicGoods[mailGoods.u1Type].u4Count;
						if(Legion.Instance.CheckGoodsLimitExcessx(mailGoods, false) == false)
						{
							dicGoods[mailGoods.u1Type].u4Count = SocialInfo.Instance.dicMailList[(UInt16)i].u4Count;
						}
						else
						{
							isOverGoods = true;
							continue;
						}
					}
					else
						dicGoods.Add(mailGoods.u1Type, new Goods(mailGoods.u1Type,SocialInfo.Instance.dicMailList[(UInt16)i].u2ItemID,SocialInfo.Instance.dicMailList[(UInt16)i].u4Count));
				}
				else
				{
					isOverGoods = true;
					continue;
				}
				*/
                UInt16 _mailSN = SocialInfo.Instance.dicMailList[(UInt16)i].u2MailSN;
                recvCnt.Add(_mailSN);
            } 
        }

		if(isOverGoods == true)
		{
			PopupManager.Instance.CloseLoadingPopup();
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), 
				TextManager.Instance.GetText("popup_maxium_goods_post"), null);
			return;
		}

        if(recvCnt.Count == 0)
		{
			PopupManager.Instance.CloseLoadingPopup();
            return;
		}

        UInt16[] mailSN = new UInt16[recvCnt.Count];
        for(int i=0; i<recvCnt.Count; i++)
        {
            mailSN[i] = recvCnt[i];
        }
        Server.ServerMgr.Instance.RequestGetItemInMail(mailSN, SuccessReciveAllGoodsMail);
    }

    public void SuccessReciveAllGoodsMail(Server.ERROR_ID err)
    {
        PopupManager.Instance.CloseLoadingPopup();
        if(err != Server.ERROR_ID.NONE)
        {
            PopupManager.Instance.CloseLoadingPopup ();
			PopupManager.Instance.ShowOKPopup (TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError (Server.MSGs.MAIL_GET, err), Server.ServerMgr.Instance.CallClear);
        }
        else if(err == Server.ERROR_ID.NONE)
        {
            objAlram[1].SetActive(false);
            BtnGroupMailGoods.transform.GetChild(0).GetComponent<Button>().interactable = false;
            EmptyMailText.SetActive(true);

			// 2016. 09. 11 jy 재화 모두 받기시 실화가 받아지지 않음 코드 수정
			/*
			 * 이전 코드
            for(int i=0; i<SocialInfo.Instance.dicMailList.Count; i++)
            {
                if(SocialInfo.Instance.dicMailList[(UInt16)i].u1MailType == 2)
                    SocialInfo.Instance.dicMailList[(UInt16)i].bCheckedMail = true;
            }
            for(int i=0; i<SocialInfo.Instance.dicMailList.Count; i++)
            {
                if(SocialInfo.Instance.dicMailList[(UInt16)i].bCheckedMail)
                {
                    _socialPanel.Alrams[1].SetActive(false);
                    _socialPanel.Alrams[4].SetActive(false);
                }
                else
                {
                    _socialPanel.Alrams[1].SetActive(true);
                    _socialPanel.Alrams[4].SetActive(true);
                    break;
                }
            }

            for(int i=0; i<SocialInfo.Instance.u2ItemCount; i++)
            {
                if(SocialInfo.Instance.dicMailGetItem[(UInt16)i].u1ItemType == 1)
                    Legion.Instance.AddGoods(SocialInfo.Instance.dicMailGetItem[(UInt16)i].u1ItemType, SocialInfo.Instance.dicMailGetItem[(UInt16)i].u4Count);
                else if(SocialInfo.Instance.dicMailGetItem[(UInt16)i].u1ItemType == 2)
                    Legion.Instance.AddGoods(SocialInfo.Instance.dicMailGetItem[(UInt16)i].u1ItemType, SocialInfo.Instance.dicMailGetItem[(UInt16)i].u4Count);
                else if(SocialInfo.Instance.dicMailGetItem[(UInt16)i].u1ItemType == 3)
                    Legion.Instance.AddGoods(SocialInfo.Instance.dicMailGetItem[(UInt16)i].u1ItemType, SocialInfo.Instance.dicMailGetItem[(UInt16)i].u4Count);
            }
            */
			int mailCount = SocialInfo.Instance.dicMailList.Count;
			/*
			for(int i = 0; i < mailCount; ++i)
			{
				if(SocialInfo.Instance.dicMailList[(UInt16)i].u1MailType != 2)
					continue;

				MailList mailList = SocialInfo.Instance.dicMailList[(UInt16)i];
				mailList.bCheckedMail = true;
				Legion.Instance.AddGoods(mailList.u1ItemType, mailList.u4Count);
			}
			*/
			for(int i=0; i < mailCount; ++i)
			{
				if(SocialInfo.Instance.dicMailList[(UInt16)i].u1MailType != 2)
					continue;
				
				for(int j = 0; j < recvCnt.Count; ++j)
				{
					if(SocialInfo.Instance.dicMailList[(UInt16)i].u2MailSN == recvCnt[j])
					{
						SocialInfo.Instance.dicMailList[(UInt16)i].bCheckedMail = true;
						Legion.Instance.AddGoods(SocialInfo.Instance.dicMailList[(UInt16)i].u1ItemType,
							SocialInfo.Instance.dicMailList[(UInt16)i].u4Count);
						break;
					}
				}
				
				if(SocialInfo.Instance.dicMailList[(UInt16)i].bCheckedMail)
				{
					_socialPanel.Alrams[1].SetActive(false);
					_socialPanel.Alrams[4].SetActive(false);
				}
				else
				{
					_socialPanel.Alrams[1].SetActive(true);
					_socialPanel.Alrams[4].SetActive(true);
				}
			}

			OnClickMailMenu(1);
			/*
            for(int i=0; i<ScrollList.transform.GetChildCount(); i++)
            {
                ScrollList.transform.GetChild(i).gameObject.SetActive(false);
            }
            */
        }
    }
    //재화 우편End==================================================

    //뽑기 우편=====================================================
    public void InitMailGachaList()
    {
        if(SocialInfo.Instance.u2MailCount == 0)
            EmptyMailText.SetActive(true);
        else
        {
            RefreashMailList(3);
        }
        for(int i=0; i<SocialInfo.Instance.u2MailCount; i++)
        {
            if(SocialInfo.Instance.dicMailList[(UInt16)i].bCheckedMail)
                continue;
            u8TimeTick = SocialInfo.Instance.dicMailList[(UInt16)i].dtExpireTime.Ticks - Legion.Instance.ServerTime.Ticks;
            if(u8TimeTick <= 0)
                continue;
            if(SocialInfo.Instance.dicMailList[(UInt16)i].u1MailType == 3)
            {
                GameObject objMailEvent = Instantiate(objPrefMailGachaList);
                objMailEvent.transform.SetParent(ScrollList.transform);
                objMailEvent.transform.localPosition = Vector3.zero;
                objMailEvent.transform.localScale = Vector3.one;
                objMailEvent.GetComponent<SocialMailEventSlot>().SetData(SocialInfo.Instance.dicMailList[(UInt16)i], (UInt16)i, this.GetComponent<SocialMailTab>());
            }
        }
    }

    public void InitGachaResult(UInt16[] u2SlotNum, UInt16[] _itemID, UInt32[] _cnt)
    {
        GameObject objGachaResult = Instantiate(PrefGachaResult);
		RectTransform rectTr = objGachaResult.GetComponent<RectTransform>();
		rectTr.SetParent(_socialPanel.transform);
		rectTr.localPosition = Vector3.zero;
		rectTr.localScale = Vector3.one;
		rectTr.sizeDelta = Vector2.zero;

        GameObject objGachaResultEffect = Instantiate(PrefGachaResultEffect);
		rectTr = objGachaResultEffect.GetComponent<RectTransform>();
		rectTr.SetParent(_socialPanel.transform);
		rectTr.localPosition = Vector3.zero;
		rectTr.localScale = Vector3.one;
		rectTr.sizeDelta = Vector2.zero;

        SocialMailGachaResult gachResult = objGachaResult.GetComponent<SocialMailGachaResult>();;
        gachResult.SetInfo(u2SlotNum, objGachaResultEffect);
        gachResult.popupEquipment.SetActive(false);

        PopupManager.Instance.showLoading = true;
		//DebugMgr.LogError (Legion.Instance.cInventory.dicInventory.ContainsKey (u2SlotNum [0])+":"+u2SlotNum [0]);
        if (u2SlotNum.Length == 1 && Legion.Instance.cInventory.dicInventory[u2SlotNum[0]].cItemInfo.ItemType == ItemInfo.ITEM_TYPE.EQUIPMENT)
        {    
            objGachaResultEffect.GetComponent<UI_Shop_Gacha_Result_Effect>().SetData((EquipmentItem)Legion.Instance.cInventory.dicInventory[u2SlotNum[0]], 
                ((EquipmentItem)Legion.Instance.cInventory.dicInventory[u2SlotNum[0]]).u1SmithingLevel, gachResult.popupEquipment);

            PopupManager.Instance.AddPopup(objGachaResult, gachResult.Close);
        }
        else if(u2SlotNum.Length > 1 && Legion.Instance.cInventory.dicInventory[u2SlotNum[0]].cItemInfo.ItemType == ItemInfo.ITEM_TYPE.EQUIPMENT)
        {
            UI_Shop_Gacha_Result_Effect gachResultEffect = objGachaResultEffect.GetComponent<UI_Shop_Gacha_Result_Effect>();
            List<EquipmentItem> tempEquipment = new List<EquipmentItem>();
            for(int i=0; i<u2SlotNum.Length; i++)
                tempEquipment.Add((EquipmentItem)Legion.Instance.cInventory.dicInventory[u2SlotNum[i]]);
            gachResultEffect.SetData(tempEquipment.ToArray(), gachResult.gameObject);
            gachResultEffect.Btn_Retry.SetActive(false);

            PopupManager.Instance.AddPopup(objGachaResult, gachResult.Close);
            PopupManager.Instance.AddPopup(objGachaResultEffect, gachResultEffect.OnClickTouch);
        }
        else if(Legion.Instance.cInventory.dicInventory[u2SlotNum[0]].cItemInfo.ItemType == ItemInfo.ITEM_TYPE.MATERIAL)
        {
            UI_Shop_Gacha_Result_Effect gachResultEffect = objGachaResultEffect.GetComponent<UI_Shop_Gacha_Result_Effect>();
            List<MaterialItem> tempMaterial = new List<MaterialItem>();
            for(int i=0; i<u2SlotNum.Length; i++)
            {
                //tempMaterial.Add((MaterialItem)Legion.Instance.cInventory.dicInventory[u2SlotNum[i]]);
                tempMaterial.Add(new MaterialItem(_itemID[i], (UInt16)_cnt[i]));
                tempMaterial[i].u2SlotNum = u2SlotNum[i];
            }
            gachResultEffect.SetData(tempMaterial.ToArray(), objGachaResult);
            PopupManager.Instance.AddPopup(objGachaResult, gachResult.Close);
            PopupManager.Instance.AddPopup(objGachaResultEffect, gachResultEffect.OnClickTouch);
        }
		else if(Legion.Instance.cInventory.dicInventory[u2SlotNum[0]].cItemInfo.ItemType == ItemInfo.ITEM_TYPE.CONSUMABLE)
		{
			UI_Shop_Gacha_Result_Effect gachResultEffect = objGachaResultEffect.GetComponent<UI_Shop_Gacha_Result_Effect>();
			List<ConsumableItem> tempConsumable = new List<ConsumableItem>();
			for(int i=0; i<u2SlotNum.Length; i++)
			{
				//tempMaterial.Add((MaterialItem)Legion.Instance.cInventory.dicInventory[u2SlotNum[i]]);
				tempConsumable.Add(new ConsumableItem(_itemID[i], (UInt16)_cnt[i]));
				tempConsumable[i].u2SlotNum = u2SlotNum[i];
			}
			gachResultEffect.SetData(tempConsumable.ToArray(), objGachaResult);
            PopupManager.Instance.AddPopup(objGachaResult, gachResult.Close);
			PopupManager.Instance.AddPopup(objGachaResultEffect, gachResultEffect.OnClickTouch);
		}
        //objGachaResultEffect.SetActive(false);
    }
    //뽑기 우편End==================================================

    //알림 우편=====================================================
    public void InitMailNoticeList()
    {
        if(SocialInfo.Instance.u2MailCount == 0)
            EmptyMailText.SetActive(true);
        else
        {
            RefreashMailList(4);
        }
        for(int i=0; i<SocialInfo.Instance.u2MailCount; i++)
        {
            if(SocialInfo.Instance.dicMailList[(UInt16)i].bCheckedMail)
                continue;
            u8TimeTick = SocialInfo.Instance.dicMailList[(UInt16)i].dtExpireTime.Ticks - Legion.Instance.ServerTime.Ticks;
            if(u8TimeTick <= 0)
                continue;
            if(SocialInfo.Instance.dicMailList[(UInt16)i].u1MailType == 4)
            {
                GameObject objMailNotice = Instantiate(objPrefMailNoticeList);
                objMailNotice.transform.parent = ScrollList.transform;
                objMailNotice.transform.localPosition = Vector3.zero;
                objMailNotice.transform.localScale = Vector3.one;
                objMailNotice.GetComponent<SocialMailNoticeSlot>().SetData(SocialInfo.Instance.dicMailList[(UInt16)i], (UInt16)i, this.GetComponent<SocialMailTab>());
            }
        }
    }
    //알림 우편End==================================================
}