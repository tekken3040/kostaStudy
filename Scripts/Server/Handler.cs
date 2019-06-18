using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;

namespace Server
{
	class AccountHandler
	{
		struct BagItem
		{
			public UInt16 itemid;
			public UInt16 count;
			public Byte inShop;
            public Byte isNew;
		}

		// 2016. 10. 16 벤 팝업 띄움
		static void ShowUserBanPopup(BinaryReader brIn)
		{
			PopupManager.Instance.CloseLoadingPopup();
			Byte banCode = brIn.ReadByte();			// 벤 코드
			string banReason = brIn.ReadString();	// 벤 이유
			UInt64 endTime = brIn.ReadUInt64();		// 벤 타임

			if(endTime != 0)
			{
				DateTime banEndTime = DateTime.FromBinary((Int64)endTime);
				DateTime serverTime = DateTime.FromBinary((Int64)brIn.ReadUInt64());
				banEndTime = banEndTime.Add(DateTime.Now - serverTime);

				banReason = string.Format(TextManager.Instance.GetText("bantime"), banReason, banEndTime.Year, banEndTime.Month, banEndTime.Day);
			}
			else
			{
				banReason = string.Format(TextManager.Instance.GetText("bantime0"), banReason);
			}

			PopupManager.Instance.ShowBigOKPopup(TextManager.Instance.GetText("title_notice"), banReason, ServerMgr.Instance.ApplicationShutdown);
		}

		public static ERROR_ID Create(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION) 
			{
				PopupManager.Instance.CloseLoadingPopup();
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
			}
			if (err == ERROR_ID.NONE)
			{
				ServerMgr.id = brIn.ReadString();
#if UNITY_EDITOR
                DebugMgr.LogError (ServerMgr.id);
#endif
            }

			callBack(err);
			return err;
		}
		public static ERROR_ID LogIn(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (ServerMgr.bConnectToServer)
			{
                if (err == ERROR_ID.NONE)
                {
                    err = LoginComplete(brIn);
                }
                else if (err == ERROR_ID.LOGIN_BLOCK_FUNCTION)  // 기능 벤
                {
                    Byte banCode = brIn.ReadByte();         // 벤 코드
                    string banReason = brIn.ReadString();   // 벤 이유
                    DateTime banCloseTime = DateTime.FromBinary((Int64)brIn.ReadUInt64());  // 벤 종료 시간
                    DateTime serverTime = DateTime.FromBinary((Int64)brIn.ReadUInt64());    // 서버 시간
                    err = LoginComplete(brIn);
                }
                else
                {
                    if (err == ERROR_ID.REQUEST_DUPLICATION)
                    {
                        PopupManager.Instance.CloseLoadingPopup();
                        PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
                    }
                    else if (err == ERROR_ID.LOGIN_BLOCK)
                    {
                        ShowUserBanPopup(brIn);
                        return err;
                    }
                }
				/*
				if (err == ERROR_ID.REQUEST_DUPLICATION) 
				{
					PopupManager.Instance.CloseLoadingPopup();
					PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
				}
				else if(err == ERROR_ID.NONE_SESSION_NOSAVE || 
					err == ERROR_ID.NETWORK_ANSWER_FAILED || 
					err == ERROR_ID.NETWORK_ANSWER_DELAYED)
				{
					PopupManager.Instance.CloseLoadingPopup();
					PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("...클라를 종료 합니다 Login"), Server.ServerMgr.Instance.ApplicationShutdown);
				}
				else if( err == ERROR_ID.LOGIN_BLOCK ||
					err == ERROR_ID.LOGIN_BLOCK_FUNCTION)
				{
					ShowUserBanPopup(brIn);
					return err;
				}
				else if (err == ERROR_ID.NONE)
                    //err = ReadUserInfo(brIn);
                    err = LoginComplete(brIn);
				*/
			}
			else
			{
				Legion.Instance.SetUserData();
			}
			callBack(err);
			return err;
		}

        static ERROR_ID LoginComplete(BinaryReader brIn)
        {
            ServerMgr.sessionID = brIn.ReadString();
            Legion.Instance.u2LastLoginServer = brIn.ReadUInt16();
            if(Legion.Instance.u2LastLoginServer == 0)
                Legion.Instance.u2RecommendServerID = brIn.ReadUInt16();
            Legion.Instance.u1ServerCount = brIn.ReadByte();
            Legion.Instance.lstServerGroup = new List<Legion.ServerGroup>();
            for(int i=0; i<Legion.Instance.u1ServerCount; i++)
            {
                Legion.ServerGroup serverGroup = new Legion.ServerGroup();
                serverGroup.u2ServerID = brIn.ReadUInt16();
                serverGroup.strServerNameCode = brIn.ReadString();
                serverGroup.u1State = brIn.ReadByte();
                serverGroup.u2Port = brIn.ReadUInt16();
                serverGroup.u1CharCreated = brIn.ReadByte();
                if(serverGroup.u1CharCreated == 1)
                {
                    serverGroup.strLegionName = brIn.ReadString();
                    if(serverGroup.strLegionName == "")
                        serverGroup.strLegionName = TextManager.Instance.GetText("default_legion_name");
                }
                else
                    serverGroup.strLegionName = TextManager.Instance.GetText("default_legion_name");
                serverGroup.u1Congestion = brIn.ReadByte();
                serverGroup.u1New = brIn.ReadByte();

                Legion.Instance.lstServerGroup.Add(serverGroup);
            }

            return ERROR_ID.NONE;
        }

		public static ERROR_ID ReadUserInfo(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			int i;
			int j;
			Byte heroIndex = 0;
            DateTime tempTime;
			//ServerMgr.sessionID = brIn.ReadString();
            tempTime = DateTime.FromBinary((Int64)brIn.ReadUInt64());
			Legion.Instance.sName = brIn.ReadString();
			Legion.Instance.u2Level = brIn.ReadUInt16();
			Legion.Instance.Gold = brIn.ReadUInt32();
			Legion.Instance.Cash = brIn.ReadUInt32();
			Legion.Instance.Energy = brIn.ReadUInt16();
			Legion.Instance.nextEnergyChargeTime = Legion.Instance.ServerTime.AddSeconds(brIn.ReadUInt16());
			Legion.Instance.LeagueKey = brIn.ReadUInt16();
			Legion.Instance.nextLeagueEnergyChargeTime = Legion.Instance.ServerTime.AddSeconds(brIn.ReadUInt16());
			Legion.Instance.u1VIPLevel = brIn.ReadByte();
			Legion.Instance.u4VIPPoint = brIn.ReadUInt32(); 
			Legion.Instance.FriendShipPoint = brIn.ReadUInt16();
			Legion.Instance.u1ForgeLevel = brIn.ReadByte();
			Legion.Instance.charAvailable = brIn.ReadBytes(ConstDef.MaxCharClass);
			Legion.Instance.charTrainingRoom = brIn.ReadBytes(ConstDef.MaxCharTrainingRoom);
			// 2016. 10. 20 jy
			// 병기의 방 통합으로 인한 주석 처리
			//Legion.Instance.equipTrainingRoom = brIn.ReadBytes(ConstDef.MaxEquipTrainingRoom);

			AssetMgr.Instance.InitDownloadList();

			for (i = 0; i < Legion.Instance.charAvailable.Length; i++) {
				if (Legion.Instance.charAvailable[i] == 1) {
					AssetMgr.Instance.AddDivisionDownload (1, i+1);
				}
			}
            
			Byte tutorialCount = brIn.ReadByte();
			Legion.Instance.cTutorial.au1Step = new Byte[tutorialCount];
            Legion.Instance.SetServerTime(tempTime);
//			DebugMgr.LogError (tutorialCount);
            
			for (j = 0; j < tutorialCount; j++)
			{
				Legion.Instance.cTutorial.au1Step[j] = brIn.ReadByte();
//				DebugMgr.LogError (j+" : "+Legion.Instance.cTutorial.au1Step[j]);
			}

            // 2017.01.25 jy 기능 열림 여부 추가
            LegionInfoMgr.Instance.u2ContentOpenCode = brIn.ReadUInt16();
            
            ShopInfoMgr.Instance.lastShopTime = DateTime.FromBinary((Int64)brIn.ReadUInt64());

			//Legion.Instance.cTutorial.au1Step[0] = 8;
            
//			Legion.Instance.cTutorial.au1Step[0] = Server.ConstDef.LastTutorialStep;
//			Legion.Instance.cTutorial.au1Step[1] = 0;

			// BLACK MARK 암시장 
			// 16.6.23 jy
			// 이전에는 상점에 일정 상태에서만 타임이 넘어왔지만 현재 무조건 타임을 받음
			Legion.Instance.u1BlackMarketOpen = brIn.ReadByte();
			//if(Legion.Instance.u1BlackMarketOpen == 3 || Legion.Instance.u1BlackMarketOpen == 4)			
			Legion.Instance.BlackMarketLeftTime = Legion.Instance.ServerTime.AddSeconds(brIn.ReadUInt32());

			Legion.Instance.cInventory.lstInShop = new Dictionary<ushort, EquipmentItem>();

			UInt16 u2BagCount = brIn.ReadUInt16();
			Dictionary<UInt16, BagItem> bag = new Dictionary<UInt16, BagItem>(u2BagCount);
			for (i = 0; i < u2BagCount; i++)
			{
				BagItem item = new BagItem();
				UInt16 slot = brIn.ReadUInt16();
				item.itemid = brIn.ReadUInt16();
				item.count = brIn.ReadUInt16();
				item.isNew = brIn.ReadByte();
				//if(item.itemid/58000 == 1)
                //    DebugMgr.LogError(item.itemid);
				bag.Add(slot, item);
			}

			UInt16 u2EquipCount = brIn.ReadUInt16();
			for (i = 0; i < u2EquipCount; i++)
			{
				UInt16 slot = brIn.ReadUInt16();
				BagItem item = bag[slot];
                item.inShop = brIn.ReadByte();
				
				string itemName = brIn.ReadString();
				string createrName = brIn.ReadString();
				Byte smithingLevel = brIn.ReadByte();
				UInt16 modelID = brIn.ReadUInt16();
				var level = (Byte)brIn.ReadUInt16();
				var exp = brIn.ReadUInt64(); // 16.05.21 jy 경험치 변수 타입 변경 brIn.Uint32() -> 64;(UInt64)
                Byte u1Completeness = brIn.ReadByte();
				Byte[] slots = new Byte[ConstDef.SkillOfEquip];
				UInt32[] stats = new UInt32[ConstDef.SkillOfEquip + ConstDef.EquipStatPointType * 2];
				for (j = 0; j < ConstDef.EquipStatPointType; j++)
				{
					stats[j + ConstDef.SkillOfEquip] = brIn.ReadUInt32();//brIn.ReadUInt16();
				}
				
				//Byte buyEquipPoint = brIn.ReadByte();
                //UInt16 vipEquipPoint = brIn.ReadUInt16();
				
				for (j = 0; j < ConstDef.SkillOfEquip; j++)
				{
					slots[j] = brIn.ReadByte();
					stats[j] = brIn.ReadUInt16();
				}
				for (j = 0; j < ConstDef.EquipStatPointType; j++)
				{
					stats[j + ConstDef.SkillOfEquip + ConstDef.EquipStatPointType] = brIn.ReadUInt16();
				}
                UInt16 u2UnsetStatPoint = brIn.ReadUInt16();
                UInt16 u2StatPointExp = brIn.ReadUInt16();
                
                //if(item.inShop == 1)
                //{
                //    EquipmentItem _item = new EquipmentItem(item.itemid);
		        //    _item.u2SlotNum = slot;
		        //    _item.registedInShop = item.inShop;
		        //    _item.itemName = itemName;
		        //    _item.createrName = createrName;
		        //    _item.u2ModelID = modelID;
		        //    _item.GetComponent<StatusComponent>().LoadStatusEquipment(stats, _item.GetEquipmentInfo().acStatAddInfo, 0);
		        //    _item.GetComponent<StatusComponent>().LoadStatus(stats,
		        //                                                    _item.GetEquipmentInfo().acStatAddInfo[0].u1StatType,
		        //                                                    _item.GetEquipmentInfo().acStatAddInfo[1].u1StatType,
		        //                                                    _item.GetEquipmentInfo().acStatAddInfo[2].u1StatType,
		        //                                                    brIn.ReadByte());
                //
		        //    _item.GetComponent<LevelComponent>().Set(level, exp);
		        //    _item.skillSlots = slots;
                //    _item.isNew = false;
		        //    _item.u1SmithingLevel = smithingLevel;
                //    Legion.Instance.cInventory.lstInShop.Add(_item);
                //}

                UInt16 equipSlot = Legion.Instance.cInventory.AddEquipment(slot, item.inShop, item.itemid, level, exp, slots,
				                                                       stats, 0, itemName, createrName, modelID, Convert.ToBoolean(item.isNew), smithingLevel, u2UnsetStatPoint, u2StatPointExp, u1Completeness);


                Item addedItem = Legion.Instance.cInventory.dicInventory[equipSlot];
				//addedItem.GetComponent<StatusComponent>().BuyPoint = buyEquipPoint;
                //addedItem.GetComponent<StatusComponent>().VIP_STATPOINT = vipEquipPoint;

				if(equipSlot == 0)
				{
					PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), "Equipment Assign Failed Slot", (x) => Application.Quit());
					DebugMgr.LogError("Error Slot : " + slot);
				}
				/*
				 * // 2016. 10. 20 jy 병기방 통합으로 인하여 사라짐
                Byte equipTrainingRoom = brIn.ReadByte();
				if(equipTrainingRoom > 0)
				{
					QuestInfoMgr.Instance.GetEquipTrainingInfo()[(UInt16)(ConstDef.BaseEquipTrainingID + equipTrainingRoom)].timeType = brIn.ReadByte();
					addedItem.u1RoomNum = equipTrainingRoom;
					addedItem.u1SeatNum = brIn.ReadByte(); 
					QuestInfoMgr.Instance.GetEquipTrainingInfo()[(UInt16)(ConstDef.BaseEquipTrainingID + equipTrainingRoom)].doneTime = Legion.Instance.ServerTime.AddSeconds(brIn.ReadUInt32());                    
				}
				*/

                if(item.inShop == 1)
                {
                    EquipmentItem _item = new EquipmentItem();
                    _item = (EquipmentItem)Legion.Instance.cInventory.dicInventory[slot];
                    Legion.Instance.cInventory.lstInShop.Add(_item.u2SlotNum, _item);
                    Legion.Instance.cInventory.dicInventory.Remove(slot);
                }
                
				bag.Remove(slot);
			}
            
			Legion.Instance.cInventory.initInventory = false;
            
			foreach (KeyValuePair<UInt16, BagItem> kvp in bag)
			{
				Legion.Instance.cInventory.AddItem(kvp.Key, kvp.Value.itemid, kvp.Value.count);
			}
            
			Legion.Instance.cInventory.initInventory = true;
            
			bag.Clear();

            UInt16 EventBag = brIn.ReadUInt16();
            for (i = 0; i < EventBag; i++)
            {
               UInt16 itemID = brIn.ReadUInt16();
               UInt32 itemCount = brIn.ReadUInt32();
               Goods tGoods = new Goods((Byte)GoodsType.EVENT_ITEM, itemID, itemCount);
               EventInfoMgr.Instance.dicMarbleBag.Add(tGoods.u2ID, tGoods);
            }

            //if (EventBag != 0)
            //{
            //    EventInfoMgr.Instance.OnLoginCheckMarble = true;
            //}

            // 2017.01.25 jy
            // 서버에서 룬 정보 데이터 보는 정보 삭제 요청
            //Legion.Instance.runeList = brIn.ReadBytes(ConstDef.SizeOfRuneBuffer);
			//Legion.Instance.SetRuneventory();

			Byte u1CrewCount = brIn.ReadByte();
			for (i = 0; i < u1CrewCount; i++)
			{
				Legion.Instance.acCrews[i] = new Crew();
				Legion.Instance.acCrews[i].u1Index = (Byte)(i + 1);
				Byte u1Unlock = brIn.ReadByte();
				if (u1Unlock % 2 == 1) Legion.Instance.acCrews[i].abLocks[0] = false;
				u1Unlock /= 2;
				if (u1Unlock % 2 == 1) Legion.Instance.acCrews[i].abLocks[1] = false;
				u1Unlock /= 2;
				if (u1Unlock % 2 == 1) Legion.Instance.acCrews[i].abLocks[2] = false;
				var selectStageID = brIn.ReadUInt16();

				if(selectStageID > 0 )
				{
					//if(StageInfoMgr.Instance.dicStageData.ContainsKey(selectStageID))
					{
						var selectStageDifficulty = brIn.ReadByte();
						Int64 s8ServerTime = brIn.ReadInt64();
						StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[selectStageID];
						Legion.Instance.acCrews[i].Dispatch(stageInfo, selectStageDifficulty, DateTime.FromBinary(s8ServerTime));
						// 서버에서 잠김 상태 
						// 16.06.16 jy 파견 보상 정보 저장

						Byte u1StageRewardCount = brIn.ReadByte();
						for(j = 0; j < u1StageRewardCount; ++j )
						{
							Byte u1DispatchItemIdx =  brIn.ReadByte();
							Int32 s4DispatchItemCount = brIn.ReadInt32();
							if( stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.TOWER )
								Legion.Instance.acCrews[i].DispatchRewaerd.AddTowerDispatchRewardByIndex(u1DispatchItemIdx, s4DispatchItemCount);
							else
								Legion.Instance.acCrews[i].DispatchRewaerd.AddNewRewardByIndex(u1DispatchItemIdx, s4DispatchItemCount);
						}
					}
				}

				//리그 정보
//				var divisionIndex = brIn.ReadByte();
//				var legendState = brIn.ReadByte();
//				var leagueDispatch = brIn.ReadByte();
//				Legion.Instance.acCrews[i].SetDivision(divisionIndex);
//				Legion.Instance.acCrews[i].SetLegendState(legendState);
//				Legion.Instance.acCrews[i].LeagueDispatch(leagueDispatch);
//				/////////
//				UInt16[] crewRunes = new UInt16[Server.ConstDef.MaxCrewRune];
//
//				for(j=0; j<crewRunes.Length; j++)
//				{
//					crewRunes[j] = brIn.ReadUInt16();
//				}
//
//				Legion.Instance.acCrews[i].SetRunes(crewRunes);
			}
			Byte u1SelectedCrewIndex = brIn.ReadByte();
			if (u1SelectedCrewIndex > 0)
			{
				Legion.Instance.cBestCrew = Legion.Instance.acCrews[u1SelectedCrewIndex - 1];
			}
			Hero[] heroes = new Hero[ConstDef.MaxHeroBelongTo + 1];
			bool[] heroskillcall = new bool[ConstDef.MaxHeroBelongTo + 1];
			UInt16[] heroskillslotopen = new UInt16[ConstDef.MaxHeroBelongTo + 1];
			Byte[] heroskillreset = new Byte[ConstDef.MaxHeroBelongTo + 1];
			Byte[] heroskillbuypoint = new Byte[ConstDef.MaxHeroBelongTo + 1];
            UInt16[] heroskillvippoint = new UInt16[ConstDef.MaxHeroBelongTo + 1];

			Byte u1HeroCount = brIn.ReadByte();
			for (i = 0; i < u1HeroCount; i++)
			{
				heroIndex = brIn.ReadByte();
				Byte madeByUser = brIn.ReadByte();
				string name = brIn.ReadString();
				UInt16 classID = brIn.ReadUInt16();

				AssetMgr.Instance.AddDivisionDownload (1, classID);

				Hero hero = new Hero(heroIndex, classID, name, madeByUser);
				UInt16[] equips = new UInt16[Hero.MAX_EQUIP_OF_CHAR];
				var level = (Byte)brIn.ReadUInt16();
				var exp = brIn.ReadUInt64(); //(UInt32)brIn.ReadUInt32()
				Byte crewIndex = brIn.ReadByte();
				Byte crewpos = brIn.ReadByte();
				if (crewIndex > 0)
				{
					Legion.Instance.acCrews[crewIndex - 1].Fill(hero, crewpos);
				}

				Byte[] shape = brIn.ReadBytes(ConstDef.LengthOfShape);
				hero.u1SelectedHair = shape[0];
				hero.u1SelectedHairColor = shape[1];
				hero.u1SelectedFace = shape[2];
                
				Byte buyStatPoint = brIn.ReadByte();
                UInt16 vipPoint = brIn.ReadUInt16();
                
				hero.GetComponent<StatusComponent>().VIP_STATPOINT = vipPoint;
				hero.GetComponent<StatusComponent>().setBuyStatPoint(buyStatPoint);
                
				UInt32[] stats = new UInt32[ConstDef.CharStatPointType];
				for (j = 0; j < ConstDef.CharStatPointType; j++)
				{
					stats[j] = brIn.ReadUInt16();
				}
				for (j = 0; j < Hero.MAX_EQUIP_OF_CHAR; j++)
				{
					equips[j] = brIn.ReadUInt16();
				}
				hero.GetComponent<StatusComponent>().LoadStatus(stats, brIn.ReadByte());
				hero.GetComponent<LevelComponent>().Set(level, exp);
				hero.Wear(equips);
				heroes[heroIndex] = hero;
				heroskillbuypoint[heroIndex] = brIn.ReadByte();
                heroskillvippoint[heroIndex] = brIn.ReadUInt16();
				heroskillslotopen[heroIndex] = brIn.ReadUInt16();
				heroskillreset[heroIndex] = brIn.ReadByte();
				Byte trainingRoom = brIn.ReadByte();
				if(trainingRoom > 0)
				{
					QuestInfoMgr.Instance.GetCharTrainingInfo()[(UInt16)(ConstDef.BaseCharTrainingID + trainingRoom)].timeType = brIn.ReadByte();
					hero.u1RoomNum = trainingRoom;
					hero.u1SeatNum = brIn.ReadByte();
					QuestInfoMgr.Instance.GetCharTrainingInfo()[(UInt16)(ConstDef.BaseCharTrainingID + trainingRoom)].doneTime = Legion.Instance.ServerTime.AddSeconds(brIn.ReadUInt32());                    
				}
			}

			List<LearnedSkill> lstLearnInfo = null;
			UInt16 u2HeroSkillCount = brIn.ReadUInt16();
			Byte prevheroIndex = 0;
			for (i = 0; i < u2HeroSkillCount; i++)
			{
				heroIndex = brIn.ReadByte();
				if (prevheroIndex != heroIndex)
				{
					if (lstLearnInfo != null)
					{
						heroes[prevheroIndex].GetComponent<SkillComponent>().LoadSkill(heroskillslotopen[prevheroIndex], lstLearnInfo, heroskillreset[prevheroIndex], heroskillbuypoint[prevheroIndex], heroskillvippoint[prevheroIndex]);
						heroskillcall[prevheroIndex] = true;
					}
					lstLearnInfo = new List<LearnedSkill>();
					prevheroIndex = heroIndex;
				}
				LearnedSkill temp = new LearnedSkill();
				//temp.u1SlotNum = 1;
				//temp.u2Level = 1;
				//temp.u1UseIndex = 1;
				temp.u1SlotNum = brIn.ReadByte();
				temp.u2Level = (Byte)brIn.ReadUInt16();
				temp.u1UseIndex = brIn.ReadByte();
				lstLearnInfo.Add(temp);
			}
			if (lstLearnInfo != null)
			{
				heroes[heroIndex].GetComponent<SkillComponent>().LoadSkill(heroskillslotopen[heroIndex], lstLearnInfo, heroskillreset[heroIndex], heroskillbuypoint[heroIndex], heroskillvippoint[prevheroIndex]);
				heroskillcall[heroIndex] = true;
			}

			foreach (Hero hero in heroes)
			{
				if (hero != null)
				{
					if (!heroskillcall[hero.u1Index]) 
						hero.GetComponent<SkillComponent>().LoadSkill(heroskillslotopen[hero.u1Index], null, heroskillreset[hero.u1Index], heroskillbuypoint[hero.u1Index], heroskillvippoint[hero.u1Index]);
					Legion.Instance.acHeros.Add(hero);
				}
			}

			//리그 정보
			Byte LeagueDivision = brIn.ReadByte();
			LeagueMatchList tempList = new LeagueMatchList();
			tempList.u1DivisionIndex = LeagueDivision;

			if(LeagueDivision > 0)
            {
				tempList.u4MyRank = brIn.ReadUInt32();
				for(int lcIdx=0; lcIdx<Legion.Instance.au1LeagueCharIndex.Length; lcIdx++)
                {
					Legion.Instance.au1LeagueCharIndex[lcIdx] = brIn.ReadByte();
                    if(Legion.Instance.au1LeagueCharIndex[lcIdx] != 0)
                    {
                        //Legion.Instance.cLeagueCrew.acLocation[lcIdx] = Legion.Instance.acHeros[(Legion.Instance.au1LeagueCharIndex[lcIdx]-1)];
                        Legion.Instance.cLeagueCrew.acLocation[lcIdx] = Legion.Instance.GetHero(Legion.Instance.au1LeagueCharIndex[lcIdx]);
                        //Legion.Instance.acHeros[(Legion.Instance.au1LeagueCharIndex[lcIdx]-1)].bAssignedLeagueCrew = true;
                        Legion.Instance.GetHero(Legion.Instance.au1LeagueCharIndex[lcIdx]).bAssignedLeagueCrew = true;
                    }
				}
				tempList.u4MyPoint = (ushort)brIn.ReadUInt32();
				Legion.Instance.u2LeagueWin = brIn.ReadUInt16();
				Legion.Instance.u2LeagueDraw = brIn.ReadUInt16();
				Legion.Instance.u2LeagueLose = brIn.ReadUInt16();
				Legion.Instance.u1LeagueReward = brIn.ReadByte();
				Legion.Instance.u2SeasonNum = brIn.ReadUInt16();
			}

			UI_League.Instance.cLeagueMatchList = tempList;
            

            //길드 정보
            GuildInfoMgr.Instance.u8GuildSN = brIn.ReadUInt64();
            GuildInfoMgr.Instance.u1GuildCrewIndex = brIn.ReadByte();
            if(GuildInfoMgr.Instance.u1GuildCrewIndex == 0)
                GuildInfoMgr.Instance.u1GuildCrewIndex = Legion.Instance.cBestCrew.u1Index;

			Legion.Instance.checkLoginAchievement = brIn.ReadByte();
            // 최초 접속일때 리셋 팝업
            if (Legion.Instance.checkLoginAchievement == 1)
            {
                // 출석 보상 셋팅
                Legion.Instance.AddLoginPopupStep(Legion.LoginPopupStep.LOGIN_REWARED);
                Legion.Instance.bTowerResetPopup = true;
                // 30일 패키지 할인 구매 여부 확인후 구매 안되었다면 셋팅
                Legion.Instance.AddLoginPopupStep(Legion.LoginPopupStep.EVENT_30DAY_PACK);
                // 오늘 하루 보지 않기 목록 삭제
                EventInfoMgr.Instance.DeleteADPref();
            }
            else
            {
                EventInfoMgr.Instance.LoadADPref();
            }
            // 네이버 까페 스탭 셋팅
			//기능막음 2017.08.08 jc
//			if (Application.systemLanguage == SystemLanguage.Korean) {
//				Legion.Instance.AddLoginPopupStep(Legion.LoginPopupStep.NAVER_CAFE);
//			}

            // 푸쉬 셋팅
            Legion.Instance.SetPushSetting(brIn.ReadByte());

            Legion.Instance.u1MailExist = brIn.ReadByte();
            Legion.Instance.u1FriendExist = brIn.ReadByte();
			//Legion.Instance.u1RankRewad = brIn.ReadByte();
			RankInfoMgr.Instance.u1RankRewardCount = brIn.ReadByte();

			RankInfoMgr.Instance.dicRankRewardData.Clear();
			if(RankInfoMgr.Instance.u1RankRewardCount > 0)
			{
				Legion.Instance.u1RankRewad = 1;
				for(int k = 0; k < RankInfoMgr.Instance.u1RankRewardCount; ++k)
				{
					RankReward _rankReward = new RankReward();
					_rankReward.u1RankType = brIn.ReadByte();
					_rankReward.u4Rank = brIn.ReadUInt32();
					_rankReward.u1RewardIndex = brIn.ReadByte();

					RankInfoMgr.Instance.dicRankRewardData.Add((UInt16)k, _rankReward);
				}
			}
			else
				Legion.Instance.u1RankRewad = 0;
            
            Legion.Instance.u1VIPUpgrade = brIn.ReadByte();

			AssetMgr.Instance.ShowDownLoadPopup();

            //Legion.Instance.CreateEnemyCrew();
            callBack(err);

			return err;
		}

		public static ERROR_ID LoginInfoMore(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (ServerMgr.bConnectToServer)
			{
				/*
				if (err == ERROR_ID.REQUEST_DUPLICATION)
				{
					PopupManager.Instance.CloseLoadingPopup();
					PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
				}
				else if(err == ERROR_ID.NONE_SESSION_NOSAVE || 
					err == ERROR_ID.NETWORK_ANSWER_FAILED || 
					err == ERROR_ID.NETWORK_ANSWER_DELAYED)
				{
					PopupManager.Instance.CloseLoadingPopup();
					PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("...클라를 종료 합니다 LoginInfoMore"), Server.ServerMgr.Instance.ApplicationShutdown);
				}
				else if( err == ERROR_ID.LOGIN_BLOCK || err == ERROR_ID.LOGIN_BLOCK_FUNCTION)
				{
					ShowUserBanPopup(brIn);
					return err;
				}
				if (err == ERROR_ID.NONE) err = ReadUserMoreInfo(brIn);
				*/
				if (err == ERROR_ID.NONE) 
					err = ReadUserMoreInfo(brIn);
				else 
				{
					if( err == ERROR_ID.LOGIN_BLOCK || err == ERROR_ID.LOGIN_BLOCK_FUNCTION)
					{
						ShowUserBanPopup(brIn);
						return err;
					}
					else 
					{
						PopupManager.Instance.CloseLoadingPopup();
						PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
					}
				}
				Legion.Instance.bLoaded = true;
                Legion.Instance.ConnectGameService();
			}
			callBack(err);
			return err;
		}

        public static ERROR_ID Logout(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if (ServerMgr.bConnectToServer)
			{
				if (err == ERROR_ID.REQUEST_DUPLICATION) 
				{
					PopupManager.Instance.CloseLoadingPopup();

					PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
				}
                if(err == ERROR_ID.NONE)
                {
                    //AssetMgr.Instance.SceneLoad("TitleScene");
                }
			}
			callBack(err);

            return err;
        }

        public static ERROR_ID Quit(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if (ServerMgr.bConnectToServer)
			{
				if (err == ERROR_ID.REQUEST_DUPLICATION) 
				{
					PopupManager.Instance.CloseLoadingPopup();

					PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
				}
                if(err == ERROR_ID.NONE)
                {
                    //PlayerPrefs.DeleteAll();
                    //AssetMgr.Instance.SceneLoad("TitleScene");
                }
			}
			callBack(err);

            return err;
        }

		static ERROR_ID ReadUserMoreInfo(BinaryReader brIn)
		{
			byte chapterCount = brIn.ReadByte();
			
			for(int i=1; i<chapterCount+1; i++)
			{
				//Byte difficulty = brIn.ReadByte();
				
				UInt16 chapterID = (UInt16)(Server.ConstDef.BaseChapterID + i);

				if (StageInfoMgr.Instance.dicChapterData.ContainsKey (chapterID)) {
					for (int j = 0; j < Server.ConstDef.MaxDifficult; j++) {								
						StageInfoMgr.Instance.dicChapterData [chapterID].repeatCount [j] = brIn.ReadByte ();
						StageInfoMgr.Instance.dicChapterData [chapterID].starCount [j] = brIn.ReadByte ();
						StageInfoMgr.Instance.dicChapterData [chapterID].repeatType [j] = brIn.ReadByte ();

//						DebugMgr.Log (StageInfoMgr.Instance.dicChapterData [chapterID].repeatCount [j]);
//						DebugMgr.Log (StageInfoMgr.Instance.dicChapterData [chapterID].starCount [j]);
					}
				} else {
					brIn.BaseStream.Position += 3 * Server.ConstDef.MaxDifficult;
				}
			}

			byte[] stageClear = brIn.ReadBytes(ConstDef.SizeOfStageBuffer);
			
			for(int i=1; i<stageClear.Length+1; i++)
			{
				UInt16 stageID = (UInt16)(Server.ConstDef.BaseStageID + i);
				if(StageInfoMgr.Instance.dicStageData.ContainsKey(stageID))
					StageInfoMgr.Instance.dicStageData[stageID].clearState = stageClear[i];
			}

			Legion.Instance.acEquipDesign = new System.Collections.BitArray( brIn.ReadBytes(ConstDef.SizeOfEquipDesignBuffer) );
			Legion.Instance.acEquipDesignNew = new System.Collections.BitArray( brIn.ReadBytes(ConstDef.SizeOfEquipDesignBuffer) );
            Legion.Instance.acEquipDesignMake = new System.Collections.BitArray( brIn.ReadBytes(ConstDef.SizeOfEquipDesignBuffer) );
			Legion.Instance.acLookDesign = new System.Collections.BitArray( brIn.ReadBytes(ConstDef.SizeOfLookDesignBuffer) );
			Legion.Instance.acLookDesignNew = new System.Collections.BitArray( brIn.ReadBytes(ConstDef.SizeOfLookDesignBuffer) );
            Legion.Instance.acMakeNewBinary = new System.Collections.BitArray( brIn.ReadBytes(ConstDef.SizeOfMakeNewBinaryBuffer) );
            Legion.Instance.cQuest.InitList();

			UInt16 questID = brIn.ReadUInt16();
			if(QuestInfoMgr.Instance.GetQuestList().ContainsKey(questID) == true)
				Legion.Instance.cQuest.u2IngQuest = questID;
			
			if(questID != 0)
				Legion.Instance.cQuest.u4QuestCount = brIn.ReadUInt16();

			System.Collections.BitArray questDone = new System.Collections.BitArray( brIn.ReadBytes(ConstDef.SizeOfQuestDoneBuffer) );
			for(int i=0; i<questDone.Count; i++)
			{
				if(questDone.Get(i))
				{
					UInt16 u2ID = (UInt16)(ConstDef.BaseQuestID+i);
					if (QuestInfoMgr.Instance.GetQuestList ().ContainsKey (u2ID) == false)
						continue;
					
					Legion.Instance.cQuest.dicQuests[u2ID].bRewarded = true;
				}
			}

			System.Collections.BitArray achievementDone = new System.Collections.BitArray( brIn.ReadBytes(ConstDef.SizeOfAchievementDoneBuffer) );
			for(int i=0; i<achievementDone.Count; i++)
			{
				if(achievementDone.Get(i))
				{
					UInt16 u2ID = (UInt16)(ConstDef.BaseAchievementID+i);
					if (Legion.Instance.cQuest.dicAchievements.ContainsKey(u2ID))
                		Legion.Instance.cQuest.dicAchievements[u2ID].bRewarded = true;
				}
			}

			Legion.Instance.cQuest.abBuffer = brIn.ReadBytes(ConstDef.SizeOfAchievementBoolBuffer);
			Legion.Instance.cQuest.au1Buffer = brIn.ReadBytes(ConstDef.SizeOfAchievementU1Buffer);
			Legion.Instance.cQuest.au2Buffer = brIn.ReadBytes(ConstDef.SizeOfAchievementU2Buffer);
			Legion.Instance.cQuest.au4Buffer = brIn.ReadBytes(ConstDef.SizeOfAchievementU4Buffer);

			foreach (UserAchievement data in Legion.Instance.cQuest.dicAchievements.Values) {
				data.CheckOpen();
			}

            /* 외형 도안은 따로 없어서 삭제(제작 도안이랑 같이 사용)
			Legion.Instance.lookDesign = new System.Collections.BitArray( brIn.ReadBytes(ConstDef.SizeOfLookDesignBuffer) );
			*/
            //Legion.Instance.CheckCrewSlotUnlock();

			/// 16.06.20 jy
			/// 탐색의 숲 입장 가능 횟수 맵별 -> 난이도 별로 수정 [임시]
			//Byte[] state = brIn.ReadBytes(ConstDef.SizeOfForestStageBuffer * ConstDef.SizeOfForestStateBuffer);
			Byte[] ticketData = brIn.ReadBytes(ConstDef.SizeOfForestStateBuffer);

			StageInfoMgr.Instance.forestTicketData.u1TicketCount = new byte[ConstDef.MaxDifficult]; 
			StageInfoMgr.Instance.forestTicketData.au1ChargedTicketCount = new byte[ConstDef.MaxDifficult]; 

			for(int i = 0; i< ConstDef.MaxDifficult; ++i)
			{
				StageInfoMgr.Instance.forestTicketData.u1TicketCount[i] = ticketData[i];
				StageInfoMgr.Instance.forestTicketData.au1ChargedTicketCount[i] = ticketData[i + 3];
			}
			/*
			//Byte[] state = brIn.ReadBytes(ConstDef.SizeOfForestStageBuffer * ConstDef.SizeOfForestStateBuffer);

			int id = 1;
			for(int stateIdx=4; stateIdx<state.Length; stateIdx+=4)
			{
				UInt16 stageID = (UInt16)(Server.ConstDef.BaseForestStageID + id);

				StageInfo stageInfo=null;
				if(StageInfoMgr.Instance.dicStageData.TryGetValue(stageID, out stageInfo))
				{
					//stageInfo.forestData.u1TicketCount = state[stateIdx];
					/stageInfo.forestData.au1ChargedTicketCount = new byte[ConstDef.MaxDifficult];

					for(int j=0; j<ConstDef.MaxDifficult; j++)
					{
						stageInfo.forestData.au1ChargedTicketCount[j] = state[stateIdx+j+1];
					}

//					if(stageID == 6515)
//						DebugMgr.LogError("D");
				}
				id++;

			}
			*/
            Legion.Instance.StopConnectTimeCount();

			Byte adCount = brIn.ReadByte();
			Legion.Instance.adRemainCount = new byte[adCount];
			Legion.Instance.adLeftTime = new ushort[adCount];
			for (int i = 0; i < adCount; i++) 
			{
				Legion.Instance.adRemainCount[i] = brIn.ReadByte();
				Legion.Instance.adLeftTime[i] = brIn.ReadUInt16();
			}

            EventInfoMgr.Instance.u1EventCount = brIn.ReadByte();
            for(int i=0; i<EventInfoMgr.Instance.u1EventCount; i++)
            {
				EventReward _eventReward = new EventReward();
				_eventReward.u2EventID = brIn.ReadUInt16();
                _eventReward.u1EventType = brIn.ReadByte();
				_eventReward.u1RewardIndex = brIn.ReadByte();
				_eventReward.u4RecordValue = brIn.ReadUInt32();
                _eventReward.u8EventBegin = brIn.ReadInt64();
                if (_eventReward.u8EventBegin != 0)
                    _eventReward.dtEventBegin = DateTime.FromBinary(_eventReward.u8EventBegin);
                _eventReward.u8EventEnd = brIn.ReadInt64();
                if (_eventReward.u8EventEnd != 0)
                    _eventReward.dtEventEnd = DateTime.FromBinary(_eventReward.u8EventEnd);

				if (EventInfoMgr.Instance.dicEventReward.ContainsKey (_eventReward.u2EventID))
					DebugMgr.LogError ("EventID Already");
				else
					EventInfoMgr.Instance.dicEventReward.Add(_eventReward.u2EventID, _eventReward);

                if(_eventReward.eventType == (Byte)EVENT_TYPE.BUFF_EXP)
                    EventInfoMgr.Instance.lstExpBuffEvent.Add(_eventReward);
                else if(_eventReward.eventType == (Byte)EVENT_TYPE.BUFF_GOLD)
                    EventInfoMgr.Instance.lstGoldBuffEvent.Add(_eventReward);

                if(_eventReward.eventType == (Byte)EVENT_TYPE.BOSSRUSH)
                {
                    EventInfoMgr.Instance.u1BossRushRewardIdx = _eventReward.u1RewardIndex;
                    EventInfoMgr.Instance.u1BossRushProgress = (Byte)_eventReward.u4RecordValue;
                }
                
            }

            for (int i=0; i<EventInfoMgr.Instance.lstTimeEventID.Count; i++)
            {
                if(EventInfoMgr.Instance.dicEventReward.ContainsKey(EventInfoMgr.Instance.lstTimeEventID[i]))
                {
                    //Legion.Instance.tsPrevConnectTime = TimeSpan.FromSeconds(Legion.Instance.timeDist);
                    Legion.Instance.tsConnectTime = TimeSpan.FromSeconds(EventInfoMgr.Instance.dicEventReward[EventInfoMgr.Instance.lstTimeEventID[i]].u4RecordValue);
                    if (EventInfoMgr.Instance.dicEventReward[EventInfoMgr.Instance.lstTimeEventID[i]].u1RewardIndex == 5)
                        EventInfoMgr.Instance.OnLastReward = true;
                    break;
                }
                else
                {
                    Legion.Instance.tsConnectTime = TimeSpan.FromSeconds(0f);
                }
            }
            
            Legion.Instance.ConnectTimeCount();
            EventInfoMgr.Instance.u1EventGoodsCount = brIn.ReadByte();
            for(int i=0; i<EventInfoMgr.Instance.u1EventGoodsCount; i++)
            {
                EventBuy _eventBuy = new EventBuy();
                _eventBuy.u2EventID = brIn.ReadUInt16();
				_eventBuy.u1EventBuyCnt = brIn.ReadByte();
                _eventBuy.u8BuyBegin = brIn.ReadInt64();
                _eventBuy.u8BuyEnd = brIn.ReadInt64();
                // 시작시간이 0 이면 상시 오픈
                if (_eventBuy.u8BuyBegin != 0)
                {
                    _eventBuy.dtBuyBegin = DateTime.FromBinary(_eventBuy.u8BuyBegin);
                    // 아직 오픈날 전이라면 다음 이벤트로
                    if (DateTime.Compare(Legion.Instance.ServerTime, _eventBuy.dtBuyBegin) <= 0)
                        continue;
                }
                // 종료 시간이 0이면 닫히지 않음
                if (_eventBuy.u8BuyEnd != 0)
                {
                    _eventBuy.dtBuyEnd = DateTime.FromBinary(_eventBuy.u8BuyEnd);
                    // 끝난 이벤트라면 다음 이벤트로
                    if (DateTime.Compare(Legion.Instance.ServerTime, _eventBuy.dtBuyEnd) > 0)
                        continue;
                }

                if (EventInfoMgr.Instance.dicEventBuy.ContainsKey(_eventBuy.u2EventID))
                    EventInfoMgr.Instance.dicEventBuy[_eventBuy.u2EventID] = _eventBuy;
                else
                    EventInfoMgr.Instance.dicEventBuy.Add(_eventBuy.u2EventID, _eventBuy);
            }
            // 비용할인 셋팅
            EventInfoMgr.Instance.SetDisCountEventList();

            // 2017.02.02 jy
            // 불필요한 이벤트 공지를 걸러낸다
            // 이벤트 공지가 구매목록 보다 먼저 셋팅하기 때문에 
            // 구매 목록을 받은 이후 체크 한다
            if (Legion.Instance.cTutorial.au1Step[4] == ConstDef.LastTutorialStep)
                SocialInfo.Instance.SFluousEventNoticeFilter();

            // 월간 이벤트 아이템 셋팅
            int EventGoodsRewardCount = brIn.ReadByte();
            for(int i = 0; i < EventGoodsRewardCount; ++i)
            {
                UInt16 eventID = brIn.ReadUInt16();
                Byte itemCount = brIn.ReadByte();
                
                if (EventInfoMgr.Instance.dicEventPackage.ContainsKey(eventID) == true)
                {
                    for (int j = 0; j < itemCount; ++j)
                    {
                        Goods rewardGoods = new Goods(
                            brIn.ReadByte(),
                            brIn.ReadUInt16(),
                            brIn.ReadUInt32());
                        EventInfoMgr.Instance.dicEventPackage[eventID].acPackageRewards[j] = rewardGoods;
                    }
                    //Legion.Instance.AddLoginPopupStep(Legion.LoginPopupStep.EVENT_MONTHLY_PACK);
                }
                else
                {
                    // 이벤트 정보가 없어도 다음 데이터를 세팅 하도록 예외 처리
                    DebugMgr.LogError("월간 이벤트 정보가 셋팅되어 있지 않음");
                    for (int j = 0; j < itemCount; ++j)
                    {
                        Goods rewardGoods = new Goods(
                            brIn.ReadByte(),
                            brIn.ReadUInt16(),
                            brIn.ReadUInt32());
                    }
                }
            }

			// 탐색의 숲 오픈된 길을 서버에서 받는다
			StageInfoMgr.Instance.OpenForestElement = brIn.ReadByte();
			StageInfoMgr.Instance.CalculateChapterOpen();
            StageInfoMgr.Instance.OpenBossRush = brIn.ReadByte();

			UInt16 BoughtEventItemCount = brIn.ReadUInt16();

			for (int i = 0; i < BoughtEventItemCount; i++)
            {
				EventItemBuyCountInfo tInfo = new EventItemBuyCountInfo ();
                tInfo.u2EventID = brIn.ReadUInt16(); // 첫 구매 1+1 이벤트 방식 변경 준비
                tInfo.u2ShopID = brIn.ReadUInt16();
				tInfo.u2ShopGroupID = brIn.ReadUInt16();
				tInfo.u4BuyCount = brIn.ReadUInt32();
				Legion.Instance.cEvent.lstItemBuyHistory.Add (tInfo);
			}

			UInt16 EventDungeonCount = brIn.ReadByte();

			for (int i = 0; i < EventDungeonCount; i++) {
				UInt16 tEventID =  brIn.ReadUInt16();
				Byte tEventType =  brIn.ReadByte();
				if (Legion.Instance.cEvent.dicDungeonOpenInfo.ContainsKey(tEventID)) {
					DebugMgr.LogError (tEventID + " is Exist");
				} else {
					EventDungeonOpenInfo tInfo = new EventDungeonOpenInfo ();
					tInfo.u2EventID = tEventID;
					tInfo.u1DayScheduleType = tEventType;
					if (tEventType == 2) {
						tInfo.u1DayInWeek = brIn.ReadByte ();
					}else if (tEventType == 3) {
						tInfo.u1OpenDayCount = brIn.ReadByte ();
						tInfo.au1OpenDays = new Byte[tInfo.u1OpenDayCount];
						for (int j = 0; j < tInfo.au1OpenDays.Length; j++) {
							tInfo.au1OpenDays[j] = brIn.ReadByte ();
						}
					}else if (tEventType == 4) {
						tInfo.u1DayTurm = brIn.ReadByte ();
					}
					tInfo.u1TimeScheduleType = brIn.ReadByte ();
					if (tInfo.u1TimeScheduleType == 1) {
						tInfo.u1OpenTimeCount = brIn.ReadByte ();
						tInfo.adtOpenTime = new DateTime[tInfo.u1OpenTimeCount];
						tInfo.au2OpeningMinute = new UInt16[tInfo.u1OpenTimeCount];
						for (int j = 0; j < tInfo.adtOpenTime.Length; j++) {
							Int64 u8Time = brIn.ReadInt64 ();
							if (u8Time != 0) tInfo.adtOpenTime[j] = DateTime.FromBinary(u8Time);
							tInfo.au2OpeningMinute[j] = brIn.ReadUInt16();
						}
					}

					Legion.Instance.cEvent.dicDungeonOpenInfo.Add (tInfo.u2EventID, tInfo);
				}
			}

			Byte openDungeonCount = brIn.ReadByte();
			for (int i = 0; i < openDungeonCount; i++) {
				UInt16 tEventId = brIn.ReadUInt16();
				UInt16 tStageId = brIn.ReadUInt16();
                Byte tClosed = brIn.ReadByte();
                Byte tPlayCount = brIn.ReadByte();
				Legion.Instance.cEvent.AddOpenStage (tEventId, tStageId, tClosed, tPlayCount);
			}

            //Legion.Instance.cEvent.u1OXtotalReward = brIn.ReadByte();
            //if(Legion.Instance.cEvent.u1OXtotalReward < Legion.Instance.cEvent.MAX_OXQUESTION_CNT)
            //{
            //    Legion.Instance.cEvent.u1OXquestion = brIn.ReadByte();
            //    Legion.Instance.cEvent.u2OXlefttime = brIn.ReadUInt16();
            //    Legion.Instance.cEvent.u1OXanswer = brIn.ReadByte();
            //    if(Legion.Instance.cEvent.u2OXlefttime != 9998 && Legion.Instance.cEvent.u2OXlefttime != 9999 && Legion.Instance.cEvent.u2OXlefttime != 0)
            //    {
            //        EventInfoMgr.Instance.StartCoroutine("OxTimer");
            //        Legion.Instance.cEvent.u1TodayOxDone = 2;
            //    }
            //    else if(Legion.Instance.cEvent.u2OXlefttime == 9998 || Legion.Instance.cEvent.u2OXlefttime == 9999)
            //    {
            //        Legion.Instance.cEvent.u1TodayOxDone = 1;
            //    }
            //    // 문제를 풀고 보상을 받지 않은 상태이거나 퀴즈를 풀지 않은 상테
            //    if(Legion.Instance.cEvent.u2OXlefttime >= 0 && Legion.Instance.cEvent.u2OXlefttime != 9999)
            //    {
            //        Legion.Instance.AddLoginPopupStep(Legion.LoginPopupStep.OX_EVENT);
            //    }
            //}

            UInt16 oxEventID = brIn.ReadUInt16();
            Legion.Instance.cEvent.u2OXEventID = oxEventID;
            if (oxEventID != 0)
            {
                Legion.Instance.cEvent.u1OXtotalReward = brIn.ReadByte();
                Legion.Instance.cEvent.u1OXquestion = brIn.ReadByte();
                Legion.Instance.cEvent.u2OXlefttime = brIn.ReadUInt16();
                Legion.Instance.cEvent.u1OXanswer = brIn.ReadByte();

                // 문제를 푸는 타입이라면 문제의 틀림여부를 체크하여 틀렷었다면 다시 풀기까지 남은 시간을 체크하게 한다
                if (EventInfoMgr.Instance.IsOXQuestion())
                {
                    if (Legion.Instance.cEvent.u2OXlefttime != 9998 && Legion.Instance.cEvent.u2OXlefttime != 9999 && Legion.Instance.cEvent.u2OXlefttime != 0)
                    {
                        EventInfoMgr.Instance.StartCoroutine("OxTimer");
                        Legion.Instance.cEvent.u1TodayOxDone = 2;
                    }
                    else if (Legion.Instance.cEvent.u2OXlefttime == 9998 || Legion.Instance.cEvent.u2OXlefttime == 9999)
                    {
                        Legion.Instance.cEvent.u1TodayOxDone = 1;
                    }
                }
                // 문제를 풀고 보상을 받지 않은 상태이거나 퀴즈를 풀지 않은 상테
                if (Legion.Instance.cEvent.u2OXlefttime >= 0 && Legion.Instance.cEvent.u2OXlefttime != 9999)
                {
                    Legion.Instance.AddLoginPopupStep(Legion.LoginPopupStep.OX_EVENT);
                }
            }

            //#ODIN [서버에서 진행중인 오딘 임무 정보 받기]
            Legion.Instance.AddLoginPopupStep(Legion.LoginPopupStep.ODIN_PAGE);
            Legion.Instance.cQuest.userOdinMissionList.Clear();
            Byte missionCount = brIn.ReadByte();
            for (Byte i = 0; i < missionCount; ++i)
            {
                Legion.Instance.cQuest.OdinMissionSeting(brIn.ReadUInt16(), brIn.ReadUInt32());
            }

            return ERROR_ID.NONE;
		}

		public static ERROR_ID SetName(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION) 
			{
				err = ERROR_ID.NONE;
			}
            //if (err == ERROR_ID.LEGION_NAME_DUPLICATE)
            //{
            //    err = ERROR_ID.NONE;
            //}
			if (err == ERROR_ID.NONE)
            {
                Legion.Instance.sName = (string)obj1;
            }
			callBack(err);
            
			return err;
		}

		public static ERROR_ID SelectCrew(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION) 
			{
				err = ERROR_ID.NONE;
			}
			Legion.Instance.cBestCrew = (Crew)obj1;
			callBack(err);
			return err;
		}

		public static ERROR_ID UpdateTutorial(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION) 
			{
				err = ERROR_ID.NONE;
			}

			callBack(err);
			return err;
		}
        
        public static ERROR_ID LegionMark(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION) 
			{
				err = ERROR_ID.NONE;
			}
            if (err == ERROR_ID.NONE)
            {
				switch(Convert.ToInt16(obj1)) //Legion.Instance.u1MarkType;
				{
				case 1: // 암시장 상태
					{
						Legion.Instance.u1BlackMarketOpen = brIn.ReadByte();
						Legion.Instance.BlackMarketLeftTime = Legion.Instance.ServerTime.AddSeconds(brIn.ReadUInt32());
					}
					break;
				case 2: // 이벤트 정보
					{
						EventInfoMgr.Instance.u1EventCount = brIn.ReadByte();
						for(int i=0; i<EventInfoMgr.Instance.u1EventCount; i++)
						{

							EventReward _eventReward = new EventReward();
							_eventReward.u2EventID = brIn.ReadUInt16();
							_eventReward.u1RewardIndex = brIn.ReadByte();
							_eventReward.u4RecordValue = brIn.ReadUInt32();
							if (EventInfoMgr.Instance.dicEventReward.ContainsKey (_eventReward.u2EventID)) {
								EventInfoMgr.Instance.dicEventReward.Remove(_eventReward.u2EventID);
							}
							EventInfoMgr.Instance.dicEventReward.Add (_eventReward.u2EventID, _eventReward);
						}
						//Legion.Instance.tsConnectTime = TimeSpan.FromSeconds(EventInfoMgr.Instance.dicEventReward[EventInfoMgr.TIME_EVENT_ID].u4RecordValue);
						//DebugMgr.LogError(EventInfoMgr.Instance.dicEventReward[EventInfoMgr.TIME_EVENT_ID].u4RecordValue);
						//Legion.Instance.ConnectTimeCount();
					}
					break;
				case 3:	// 소셜 정보
					{
						Legion.Instance.u1MailExist = brIn.ReadByte();
						Legion.Instance.u1FriendExist = brIn.ReadByte();
					}
					break;
				case 5: // 리뷰 보상
					{
						Byte itemType = brIn.ReadByte();
						// 이미 받았다면 아이템 타임 0
						if(itemType != 0)
						{
							Goods ReviewGoods = new Goods(itemType, brIn.ReadUInt16(), brIn.ReadUInt32());
							Legion.Instance.AddGoods(ReviewGoods);
						}
					}
					break;
				}
            }
			if (callBack != null)
				callBack(err);
			return err;
		}
	}

	class CharacterHandler
	{
		public static ERROR_ID Create(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION) 
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE)
            {
                Byte missionCount = brIn.ReadByte();
                if (missionCount > 0)
                {
                    for (int i = 0; i < missionCount; ++i)
                    {
                        Legion.Instance.cQuest.OdinMissionSeting(brIn.ReadUInt16(), brIn.ReadUInt32());
                    }
                }

                if (obj1 != null) {
					Hero hero = (Hero)obj1;
					Legion.Instance.AddNewHero (hero);
					hero.MakeBasicEquipAndWear ();
					Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.CreateChar, 0, 0, 0, 0, 1);
				}
            }
			callBack(err);
			return err;
		}
		public static ERROR_ID Retire(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION) 
			{
				PopupManager.Instance.CloseLoadingPopup();
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
				return err;
			}
			if (err == ERROR_ID.NONE)
			{
                EquipmentItem tempEquip;
                for(int i=0; i<((Hero)obj1).acEquips.Length; i++)
                {
                    if(((Hero)obj1).acEquips[i] != null)
                    {
                        tempEquip = ((Hero)obj1).acEquips[i];
                        tempEquip.Detach();
                        Legion.Instance.cInventory.RemoveEquip(tempEquip.u2SlotNum);
                    }
                }
				Legion.Instance.RemoveHero((Hero)obj1);
				Byte u1Count = brIn.ReadByte();
				for (int i = 0; i < u1Count; i++)
				{
					Byte itemType = brIn.ReadByte();
					UInt16 itemid = brIn.ReadUInt16();
					UInt32 count = brIn.ReadUInt32();
					Legion.Instance.cInventory.AddItem(0, itemid, count);
				}
			}
			callBack(err);
			return err;
		}
		public static ERROR_ID ResetStat(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE) {
				((Hero)obj1).GetComponent<StatusComponent> ().ResetStatus ();

				Legion.Instance.Cash = brIn.ReadUInt32 ();
				Legion.Instance.effectEventID = brIn.ReadUInt16 ();
			}
			callBack(err);
			EventInfoMgr.Instance.CheckChangeEvent (Legion.Instance.effectEventID);
			return err;
		}
		public static ERROR_ID PointStat(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE)
			{
				Hero[] datas = (Hero[])obj1;
                UInt16[][] datas2 = (UInt16[][])obj2;
                for (int i = 0; i < datas.Length; i++)
                {
                    datas[i].GetComponent<StatusComponent>().DoPointingStatus(datas2[i]);
                }
			}
			else
			{
				Hero[] datas = (Hero[])obj1;
                for(int i=0; i<datas.Length; i++)
				    datas[i].GetComponent<StatusComponent>().UndoPointingStatus();
			}
			callBack(err);
			return err;
		}
		public static ERROR_ID ChangeEquip(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION) 
			{
				err = ERROR_ID.NONE;
			}
            if (err != ERROR_ID.NONE)
            {
                ((Hero)obj1).UndoChangingEquip();
            }
            callBack(err);
			return err;
		}
        public static ERROR_ID ToggleStatusPoint(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE)
			{
				
			}

			callBack(err);
			return err;
		}
        public static ERROR_ID BuyStatusPoint(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE)
			{
				((Hero)obj1).GetComponent<StatusComponent>().addBuyStatPoint((Byte)obj2);
			}
			
			callBack(err);
			return err;
		}
        
        public static ERROR_ID CharBeginTraining(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
            if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE)
			{

			}
			
			callBack(err);
			return err;
        }
        
        public static ERROR_ID CharEndTraining(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
            if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE)
			{
				TrainingInfo info = (TrainingInfo)obj1;
				Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.Training, 0, info.u1Step, info.timeType, 0, 1);
			}
			
			callBack(err);
			return err;
        }
        
        public static ERROR_ID CharCancelTraining(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
            if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE)
			{

			}
			
			callBack(err);
			return err;
        }
        
        public static ERROR_ID CharFinishTraining(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
            if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE)
			{
               	TrainingInfo info = (TrainingInfo)obj1;
               	Legion.Instance.SubGoods(info.doneGoods.u1Type, brIn.ReadUInt32());
				Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.Training, 0, info.u1Step, info.timeType, 0, 1);
			}
			
			callBack(err);
			return err;
        }
        
        public static ERROR_ID CharOpenTrainingSeat(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
            if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE)
			{

			}
			
			callBack(err);
			return err;
        }
	}

	class CrewHandler
	{
		public static ERROR_ID Open(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION) 
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE)
			{
				((Crew)obj1).UnLock();
				if (Legion.Instance.cBestCrew == null) Legion.Instance.cBestCrew = (Crew)obj1;

				Legion.Instance.Gold = brIn.ReadUInt32 ();
				Legion.Instance.Cash = brIn.ReadUInt32 ();
				Legion.Instance.effectEventID = brIn.ReadUInt16 ();
			}
			callBack(err);
			EventInfoMgr.Instance.CheckChangeEvent (Legion.Instance.effectEventID);
			return err;
		}
		public static ERROR_ID OpenSlot(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION) 
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE)
			{
				((Crew)obj1).abLocks[(Byte)obj2] = false;

				Legion.Instance.Gold = brIn.ReadUInt32 ();
				Legion.Instance.Cash = brIn.ReadUInt32 ();
				Legion.Instance.effectEventID = brIn.ReadUInt16 ();

			}
			callBack(err);
			EventInfoMgr.Instance.CheckChangeEvent (Legion.Instance.effectEventID);
			return err;
		}
		public static ERROR_ID Change(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION) 
			{
				err = ERROR_ID.NONE;
			}
			if(err == ERROR_ID.NONE)
			{
				((Crew)obj1).DoChanging();
			}
			else
			{
				((Crew)obj1).UndoChanging();
			}
			callBack(err);
			return err;
		}
	}

	class ItemHandler
	{
		public static ERROR_ID ResetStat(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE) ((EquipmentItem)obj1).GetComponent<StatusComponent>().ResetStatus();
			callBack(err);
			return err;
		}
		public static ERROR_ID PointStat(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION) 
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE)
			{
//				((EquipmentItem)obj1).GetComponent<StatusComponent>().DoPointingStatus((UInt16[])obj2);
			}
			else
			{
//				((EquipmentItem)obj1).GetComponent<StatusComponent>().UndoPointingStatus();
			}
			callBack(err);
			return err;
		}

		public static ERROR_ID UseItem(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}

			if(err == ERROR_ID.NONE)
			{

			}

			callBack(err);
			return err;
		}

		public static ERROR_ID SellItem(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			
			if(err == ERROR_ID.NONE)
			{
				
			}
			
			callBack(err);
			return err;
		}
		
		public static ERROR_ID BuyEquipmentStatPoint(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			
			if(err == ERROR_ID.NONE)
			{
				
			}
			
			callBack(err);
			return err;
		}

        public static ERROR_ID EquipBeginTraining(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
            if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE)
			{

			}
			
			callBack(err);
			return err;
        }
        
        public static ERROR_ID EquipEndTraining(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
            if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE)
			{
				Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.EqTraining, 0, 0, 0, 0, 1);
			}
			
			callBack(err);
			return err;
        }
        
        public static ERROR_ID EquipCancelTraining(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
            if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE)
			{

			}
			
			callBack(err);
			return err;
        }
        
        public static ERROR_ID EquipFinishTraining(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
            if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE)
			{
               TrainingInfo info = (TrainingInfo)obj1;
               Legion.Instance.SubGoods(info.doneGoods.u1Type, brIn.ReadUInt32());
				Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.EqTraining, 0, 0, 0, 0, 1);
			}
			
			callBack(err);
			return err;
        }
        
        public static ERROR_ID EquipOpenTrainingSeat(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
            if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE)
			{

			}
			
			callBack(err);
			return err;
        }        
        
        public static ERROR_ID EquipCheckSlot(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE)
			{

			}
            else
            {
                DebugMgr.Log(err);
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetError(MSGs.EQUIP_CHECK_SLOT, err), Server.ServerMgr.Instance.CallClear);                
            }
			
			callBack(err);
			return err;            
        }
	}

	class SkillHandler
	{
		public static ERROR_ID Open(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE) {
				((Hero)obj1).GetComponent<SkillComponent> ().OpenSelectSlot ((Byte)obj2);

				Legion.Instance.Cash = brIn.ReadUInt32 ();
				Legion.Instance.effectEventID = brIn.ReadUInt16 ();
			}
			callBack(err);
			EventInfoMgr.Instance.CheckChangeEvent (Legion.Instance.effectEventID);
			return err;
		}
		public static ERROR_ID Reset(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE) {
				((Hero)obj1).GetComponent<SkillComponent> ().Reset ();

				Legion.Instance.Cash = brIn.ReadUInt32 ();
				Legion.Instance.effectEventID = brIn.ReadUInt16 ();
			}
			callBack(err);
			EventInfoMgr.Instance.CheckChangeEvent (Legion.Instance.effectEventID);
			return err;
		}
		public static ERROR_ID Change(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE)
			{

			}
	
			callBack(err);
			return err;
		}
		public static ERROR_ID BuySkillPoint(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE)
			{
				((Hero)obj1).GetComponent<SkillComponent>().AddBuyPoint((Byte)obj2);
			}
			
			callBack(err);
			return err;
		}
	}

	class StageHandler
	{
		public class Stage
		{
			public StageInfo info;
			public Byte difficulty;

			public Stage(StageInfo _info, Byte _difficulty)
			{
				info = _info;
				difficulty = _difficulty;
			}
		}

		public static ERROR_ID Start(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION) 
			{
				PopupManager.Instance.CloseLoadingPopup();
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
			}
			if (err == ERROR_ID.NONE) {
				Stage stage = (Stage)obj2;
				if (ServerMgr.bConnectToServer)
                {
					Legion.Instance.cReward = new Reward (stage.info, stage.difficulty);
					Byte u1Count = brIn.ReadByte ();
					for (Byte i = 0; i < u1Count; i++) {
						Byte u1Index = brIn.ReadByte ();
						Int32 u4Count = brIn.ReadInt32 ();
						Legion.Instance.cReward.AddNewRewardByIndex (u1Index, u4Count);
					}

					Legion.Instance.Energy = brIn.ReadUInt16 ();

					if (Legion.Instance.Energy < LegionInfoMgr.Instance.keyTime.MAX_COUNT)
						Legion.Instance.nextEnergyChargeTime = Legion.Instance.ServerTime.AddSeconds (brIn.ReadUInt16 ());      
					
					if (stage.info.u2ChapterID > 0)
                    {
                        ChapterInfo chapterInfo = null;
                        StageInfoMgr.Instance.dicChapterData.TryGetValue(stage.info.u2ChapterID, out chapterInfo);
                        if (chapterInfo != null)
                        {
                            StageInfo stageInfo = null;
                            StageInfoMgr.Instance.dicStageData.TryGetValue(Legion.Instance.u2SelectStageID, out stageInfo);
                            if (stageInfo != null)
                            {
                                if (stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.FOREST)
                                    StageInfoMgr.Instance.UseForestTicket(Legion.Instance.u2SelectStageID);
                            }
                            else
                            {
                                err = ERROR_ID.REQUEST_DUPLICATION;
                            }
                        }
                        else
                        {
                            err = ERROR_ID.REQUEST_DUPLICATION;
                        }
					}
                }
                else {
					Legion.Instance.cReward = new Reward (stage.info, stage.difficulty);
					Legion.Instance.cReward.AddNewRewardByIndex (0, 1);
				}
			} else {
				PopupManager.Instance.CloseLoadingPopup();
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
			}
			callBack(err);
			return err;
		}
		public static ERROR_ID Result(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION) 
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE)
			{// 결과화면에서 애니 여부에 따라서 추가 처리를 달리한다.
				DebugMgr.Log("핸들러 진입 성공");
                
				StageInfo stageInfo = StageInfoMgr.Instance.dicStageData[Legion.Instance.cReward.stageID];
                if ((Byte)obj2 > 0)
                {
                    StageInfoMgr.Instance.u8AddedExp = brIn.ReadUInt64();
                    StageInfoMgr.Instance.u4TotalGold = brIn.ReadUInt32();
                    StageInfoMgr.Instance.u1EventIDCount = brIn.ReadByte();
                    StageInfoMgr.Instance.u4PrevTotalGold = Legion.Instance.Gold;
                    StageInfoMgr.Instance.arrEventID = new UInt16[StageInfoMgr.Instance.u1EventIDCount];

                    for (int i = 0; i < StageInfoMgr.Instance.u1EventIDCount; i++)
                        StageInfoMgr.Instance.arrEventID[i] = brIn.ReadUInt16();
                    Legion.Instance.cReward.PutIntoBag((Crew)obj1);

                    //#ODIN [스테이지 클리어 오딘 포인트 증가하기]
                    if (stageInfo.chapterInfo != null)
                    {
                        Legion.Instance.AddGoods(stageInfo.chapterInfo.cPlayPayBack);
                    }
                    //#ODIN [이벤트 던전 클리어시 오딘 포인트 증가하기]
                    else
                    {
                        Legion.Instance.AddGoods(EventInfoMgr.Instance.GetEventDungeonClearPoint(stageInfo.u2ID));
                    }
                }
				if (stageInfo.actInfo != null) {
					if ((Byte)obj2 > 0)
	                {
						StageInfoMgr.Instance.UpdateStageClearByClear (Legion.Instance.cReward.stageID, Legion.Instance.cReward.difficulty, (Byte)obj2);
						StageInfoMgr.Instance.UpdateClearPoint (Legion.Instance.cReward.stageID);
						StageInfoMgr.Instance.UpdateStarPoint (Legion.Instance.cReward.stageID);

						//if (stageInfo.u1ForestElement == 0) 
						// 2016. 09. 21 jy 
						// 일일 업적에서 스테이지 클리어는 업적은 스테이지 일때만 한다
						if(stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.STAGE)
						{
							if (stageInfo.u1BossType > 0)
								Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.StageClear, Legion.Instance.cReward.stageID, Legion.Instance.SelectedDifficult, 2, stageInfo.actInfo.u1Number, 1);
							else
								Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.StageClear, Legion.Instance.cReward.stageID, Legion.Instance.SelectedDifficult, 1, stageInfo.actInfo.u1Number, 1);
						}
						else if(stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.TOWER)
						{
							Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.StageClearMode1, Legion.Instance.cReward.stageID, Legion.Instance.SelectedDifficult, 0, 0, 1);
						}
						else if(stageInfo.actInfo.u1Mode == ActInfo.ACT_TYPE.FOREST)
						{
							Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.StageClearMode2, Legion.Instance.cReward.stageID, Legion.Instance.SelectedDifficult, 0, 0, 1);
						}
					}
					StageInfoMgr.Instance.LastPlayStage = Legion.Instance.cReward.stageID;  
				}

				if ((Byte)obj2 > 0) {
					if (stageInfo.u2ChapterID > 0) {
						//몬스터관련 퀘스트 업적 정보 업데이트
						Dictionary<ushort, uint> dicMonsterCount = new Dictionary<ushort, uint> ();
						for (int a = 0; a < stageInfo.acPhases.Length; a++) {
							FieldInfo cCurrentBattleField = stageInfo.acPhases [a].getField ();
							for (int iGroup = 0; iGroup < cCurrentBattleField.acMonsterGroup.Length; iGroup++) {
								for (Byte i = 0; i < cCurrentBattleField.acMonsterGroup [iGroup].u1SubGroupNum; i++) {
									ushort monID = cCurrentBattleField.acMonsterGroup [iGroup].acMonsterInfo [i].u2MonsterID;
									if (!dicMonsterCount.ContainsKey (monID))
										dicMonsterCount.Add (monID, (uint)cCurrentBattleField.acMonsterGroup [iGroup].acMonsterInfo [i].u1SpawnCount);
									else
										dicMonsterCount [monID] += (uint)cCurrentBattleField.acMonsterGroup [iGroup].acMonsterInfo [i].u1SpawnCount;
								}
							}
						}

						foreach (KeyValuePair<ushort, uint> info in dicMonsterCount) {
							ClassInfo temp = ClassInfoMgr.Instance.GetInfo (info.Key);
							if (temp != null)
								Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.KillMonster, info.Key, temp.u1Element, temp.u1MonsterType, Legion.Instance.cReward.difficulty, info.Value);
						}
					}
				}
			}
			callBack(err);
			return err;
		}
		public static ERROR_ID Dispatch(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				PopupManager.Instance.CloseLoadingPopup();
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
			}
			if (err == ERROR_ID.NONE)
			{
				Stage stage = (Stage)obj2;
				Int64 s8ServerTime = brIn.ReadInt64();
				((Crew)obj1).Dispatch(stage.info, stage.difficulty, DateTime.FromBinary(s8ServerTime));

				Byte u1RewardCount = brIn.ReadByte();
				for(int i = 0; i < u1RewardCount; ++i)
				{
					Byte u1ItemIndex = brIn.ReadByte();
					Int32 s4ItemCount = brIn.ReadInt32();

					if(stage.info.actInfo.u1Mode == ActInfo.ACT_TYPE.TOWER)
						((Crew)obj1).DispatchRewaerd.AddTowerDispatchRewardByIndex(u1ItemIndex, s4ItemCount);
					else
						((Crew)obj1).DispatchRewaerd.AddNewRewardByIndex(u1ItemIndex, s4ItemCount);
						
				}

                Legion.Instance.Energy = brIn.ReadUInt16();

				//StageInfo stageInfo = null;
				//StageInfoMgr.Instance.dicStageData.TryGetValue(stage.info.u2ID, out stageInfo);
				if (stage.info.actInfo.u1Mode == ActInfo.ACT_TYPE.FOREST)
				{
					StageInfoMgr.Instance.UseForestTicket(stage.info.u2ID);
				}

				if (Legion.Instance.Energy < LegionInfoMgr.Instance.keyTime.MAX_COUNT)
					Legion.Instance.nextEnergyChargeTime = Legion.Instance.ServerTime.AddSeconds(brIn.ReadUInt16());
			}
			callBack(err);
			return err;
		}
		public static ERROR_ID CancelDispatch(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				err = ERROR_ID.NONE;
			}
			if (err == ERROR_ID.NONE)
			{
				((Crew)obj1).ClearDispatch();
			}
			callBack(err);
			return err;
		}
		public static ERROR_ID GetDispatchResult(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				PopupManager.Instance.CloseLoadingPopup();
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
			}
			if (err == ERROR_ID.NONE)
			{
				if (ServerMgr.bConnectToServer)
				{
					Legion.Instance.cReward = null;

					Byte u1Count = brIn.ReadByte();
					
					Legion.Instance.cReward = new Reward(((Crew)obj1).DispatchStage, ((Crew)obj1).StageDifficulty);
					for (Byte i = 0; i < u1Count; i++)
					{
						Byte u1Index = brIn.ReadByte();
						Int32 u4Count = brIn.ReadInt32();
						if(((Crew)obj1).DispatchStage.actInfo.u1Mode != ActInfo.ACT_TYPE.TOWER)
							Legion.Instance.cReward.AddNewRewardByIndex(u1Index, u4Count);
						else
							Legion.Instance.cReward.AddTowerDispatchRewardByIndex(u1Index, u4Count);
					}
                    StageInfoMgr.Instance.u8AddedExp = brIn.ReadUInt64();
                    StageInfoMgr.Instance.u4TotalGold = brIn.ReadUInt32();
                    StageInfoMgr.Instance.u1EventIDCount = brIn.ReadByte();
                    StageInfoMgr.Instance.u4PrevTotalGold = Legion.Instance.Gold;
                    StageInfoMgr.Instance.arrEventID = new UInt16[StageInfoMgr.Instance.u1EventIDCount];
                    for(int i=0; i<StageInfoMgr.Instance.u1EventIDCount; i++)
                        StageInfoMgr.Instance.arrEventID[i] = brIn.ReadUInt16();
					Legion.Instance.cReward.PutIntoBag((Crew)obj1);
					StageInfoMgr.Instance.UpdateClearPoint(Legion.Instance.cReward.stageID);
					Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.Dispatch, Legion.Instance.cReward.stageID, ((Crew)obj1).StageDifficulty, 0, 0, 1);
				}
				else
				{
					Legion.Instance.cReward = new Reward(((Crew)obj1).DispatchStage, ((Crew)obj1).StageDifficulty);
					Legion.Instance.cReward.AddNewRewardByIndex(0, 1);
					StageInfoMgr.Instance.UpdateClearPoint(Legion.Instance.cReward.stageID);
					//((Crew)obj1).ClearDispatch();
				}
			}
			callBack(err);
			return err;
		}
		public static ERROR_ID FinishDispatch(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				PopupManager.Instance.CloseLoadingPopup();
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
			}
			if (err == ERROR_ID.NONE)
			{
				if (ServerMgr.bConnectToServer)
				{
					Legion.Instance.cReward = null;

					Byte u1Count = brIn.ReadByte();
                    Crew crew = (Crew)obj1;
                    Legion.Instance.cReward = new Reward(crew.DispatchStage, crew.StageDifficulty);
					for (Byte i = 0; i < u1Count; i++)
					{
						Byte u1Index = brIn.ReadByte();
						Int32 u4Count = brIn.ReadInt32();
						// 2016. 07 . 21 jy
						// 탑의 파견 보상은 스테이지 보상이 아닌 챕터 보상으로 따로 처리한다
						if(((Crew)obj1).DispatchStage.actInfo.u1Mode != ActInfo.ACT_TYPE.TOWER)
							Legion.Instance.cReward.AddNewRewardByIndex(u1Index, u4Count);
						else
							Legion.Instance.cReward.AddTowerDispatchRewardByIndex(u1Index, u4Count);
					}
                    StageInfoMgr.Instance.u8AddedExp = brIn.ReadUInt64();
                    StageInfoMgr.Instance.u4TotalGold = brIn.ReadUInt32();
                    StageInfoMgr.Instance.u1EventIDCount = brIn.ReadByte();
                    StageInfoMgr.Instance.u4PrevTotalGold = Legion.Instance.Gold;
                    StageInfoMgr.Instance.arrEventID = new UInt16[StageInfoMgr.Instance.u1EventIDCount];
                    for(int i=0; i<StageInfoMgr.Instance.u1EventIDCount; i++)
                        StageInfoMgr.Instance.arrEventID[i] = brIn.ReadUInt16();
					Legion.Instance.cReward.PutIntoBag((Crew)obj1);
					StageInfoMgr.Instance.UpdateClearPoint(Legion.Instance.cReward.stageID);
					Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.Dispatch, Legion.Instance.cReward.stageID, ((Crew)obj1).StageDifficulty, 0, 0, 1);
                    //Legion.Instance.cQuest.CheckEndDirection (AchievementTypeData.Dispatch);
                    UInt32 constCount = brIn.ReadUInt32();
#if UNITY_EDITOR
                    // 빠른 완료 재화 차감
                    DebugMgr.LogError(string.Format("빠른 완료 재화 차감 : {0}", constCount));
#endif
                    Legion.Instance.SubGoods(crew.DispatchStage.GetChapterInfo().cReturnGoods.u1Type, constCount);
                }
				else
				{
					Legion.Instance.cReward = new Reward(((Crew)obj1).DispatchStage, ((Crew)obj1).StageDifficulty);
					Legion.Instance.cReward.AddNewRewardByIndex(0, 1);
					StageInfoMgr.Instance.UpdateClearPoint(Legion.Instance.cReward.stageID);
					//((Crew)obj1).ClearDispatch();
				}
			}
			callBack(err);
			return err;
		}
		public static ERROR_ID Sweep(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				PopupManager.Instance.CloseLoadingPopup();
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
			}
			if (err == ERROR_ID.NONE)
			{
				Stage stage = (Stage)obj1;
				if (ServerMgr.bConnectToServer)
				{
					Legion.Instance.cReward = new Reward(stage.info, stage.difficulty);
					Byte u1Count = brIn.ReadByte();
					for (Byte i = 0; i < u1Count; i++)
					{
						Byte u1Index = brIn.ReadByte();
						Int32 u4Count = brIn.ReadInt32();
						Legion.Instance.cReward.AddNewRewardByIndex(u1Index, u4Count);
					}
					// 2016. 10. 19 jy
					// 인벤토리에 보상 아이템을 받기전에 소탕권을 차감한다
					ConsumableItem ticket = (ConsumableItem)Legion.Instance.cInventory.dicInventory[Legion.Instance.cInventory.dicItemKey[Server.ConstDef.TICKET_SWEEP]];
					ticket.u2Count -= 1;

					if(ticket.u2Count == 0)
						Legion.Instance.cInventory.RemoveItem(ticket.cItemInfo.u2ID);

					Legion.Instance.cReward.PutIntoBag(Legion.Instance.cBestCrew);
					StageInfoMgr.Instance.UpdateClearPoint(Legion.Instance.cReward.stageID);
					Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.Sweep, stage.info.u2ID, stage.difficulty, 0, 0, 1);

                    Legion.Instance.Energy = brIn.ReadUInt16();
                    
                    UInt16 newServerTime = brIn.ReadUInt16();
                    
                    if(Legion.Instance.Energy < LegionInfoMgr.Instance.keyTime.MAX_COUNT)
                        //Legion.Instance.nextEnergyChargeTime = Legion.Instance.ServerTime.AddSeconds(brIn.ReadUInt16()); 
                        Legion.Instance.nextEnergyChargeTime = Legion.Instance.ServerTime.AddSeconds(newServerTime); 

                    StageInfoMgr.Instance.u4TotalGold = brIn.ReadUInt32();
                    StageInfoMgr.Instance.u1EventIDCount = brIn.ReadByte();
                    StageInfoMgr.Instance.u4PrevTotalGold = Legion.Instance.Gold;
                    StageInfoMgr.Instance.arrEventID = new UInt16[StageInfoMgr.Instance.u1EventIDCount];

                    for (int i = 0; i < StageInfoMgr.Instance.u1EventIDCount; i++)
                        StageInfoMgr.Instance.arrEventID[i] = brIn.ReadUInt16();

					if(stage.info.actInfo.u1Mode == ActInfo.ACT_TYPE.FOREST)
					{
						StageInfoMgr.Instance.UseForestTicket(stage.info.u2ID);
					}
				}
				else
				{
					Legion.Instance.cReward = new Reward(stage.info, stage.difficulty);
					Legion.Instance.cReward.AddNewRewardByIndex(0, 1);
					StageInfoMgr.Instance.UpdateClearPoint(Legion.Instance.cReward.stageID);
				}
			}
			callBack(err);
			return err;
		}
		
		public static ERROR_ID RepeatReward(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				PopupManager.Instance.CloseLoadingPopup();
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
			}
			if (err == ERROR_ID.NONE)
			{
				StageInfoMgr.Instance.repeatReward = brIn.ReadByte();	
			}
			callBack(err);
			return err;
		}
		
		public static ERROR_ID StarReward(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				PopupManager.Instance.CloseLoadingPopup();
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
			}
			if (err == ERROR_ID.NONE)
			{
					
			}
			callBack(err);
			return err;
		}

		public static ERROR_ID BuyTicket_Forest(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if (err == ERROR_ID.REQUEST_DUPLICATION)
			{
				PopupManager.Instance.CloseLoadingPopup();
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
			}
			if (err == ERROR_ID.NONE)
			{
				//UInt16 stageID = (UInt16)obj1;
				Byte difficult = Legion.Instance.SelectedDifficult;

				StageInfoMgr.Instance.ResetForestTicket();//(stageID);
			}
			callBack(err);
			return err;
		}
	}

	class ShopHandler
	{
		public static ERROR_ID ShopList(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(ServerMgr.bConnectToServer)
            {
	            if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					PopupManager.Instance.CloseLoadingPopup();
					PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
				}
				
				if(err == ERROR_ID.NONE)
				{
					Shop shopData = new Shop();

					shopData.u1ShopType = brIn.ReadByte();
					shopData.u1RenewCount = brIn.ReadByte();
					
					shopData.leftTime = Legion.Instance.ServerTime.AddSeconds(brIn.ReadUInt32());

					shopData.lstShopItem = new List<ShopItem>();

					Byte u1ItemCount = brIn.ReadByte();
				
					for (int i = 0; i < u1ItemCount; i++)
					{
						ShopItem shopItem = new ShopItem();
						shopItem.u2ItemID = brIn.ReadUInt16();
						shopItem.u4Count = brIn.ReadByte();
						shopItem.u1PriceType = brIn.ReadByte();
						shopItem.u4Price = brIn.ReadUInt32();
						shopItem.u1SoldOut = brIn.ReadByte();
						shopItem.cEquipInfo = null;

						if(shopItem.u1SoldOut == 1)
						{
							shopData.lstShopItem.Add(shopItem);
							continue;
						}							

						if(shopItem.u2ItemID >= 10000 && shopItem.u2ItemID < 30000)
						{
							shopItem.cEquipInfo = new ShopItemEquip();
							shopItem.cEquipInfo.strItemName = brIn.ReadString();
							shopItem.cEquipInfo.strCreater = brIn.ReadString();
							shopItem.cEquipInfo.u1SmithingLevel = brIn.ReadByte();
							shopItem.cEquipInfo.u2ModelID = brIn.ReadUInt16();
							shopItem.cEquipInfo.u2Level = (Byte)brIn.ReadUInt16();
							shopItem.cEquipInfo.u8Exp = brIn.ReadUInt64(); //(UInt64)brIn.ReadUInt32();
                            shopItem.cEquipInfo.u1Completeness = brIn.ReadByte();
							//shopItem.cEquipInfo.u2ArrBaseStat = new UInt16[Server.ConstDef.EquipStatPointType];
							shopItem.cEquipInfo.u4ArrBaseStat = new UInt32[Server.ConstDef.EquipStatPointType];
							
							for(int j=0; j<Server.ConstDef.EquipStatPointType; j++)
								shopItem.cEquipInfo.u4ArrBaseStat[j] = brIn.ReadUInt32();//brIn.ReadUInt16();

							//shopItem.cEquipInfo.u1BuyPoint = brIn.ReadByte();
                            //shopItem.cEquipInfo.u1VipPoint = brIn.ReadUInt16();

							shopItem.cEquipInfo.u1ArrSkillSlots = new Byte[Server.ConstDef.SkillOfEquip];
							shopItem.cEquipInfo.u1ArrSkillPoint = new UInt16[Server.ConstDef.SkillOfEquip];
							for(int j=0; j<Server.ConstDef.SkillOfEquip; j++)
							{
								shopItem.cEquipInfo.u1ArrSkillSlots[j] = brIn.ReadByte();
								shopItem.cEquipInfo.u1ArrSkillPoint[j] = brIn.ReadUInt16();
							}
	
							shopItem.cEquipInfo.u2ArrStatsPoint = new UInt16[Server.ConstDef.EquipStatPointType];
							for(int j=0; j<Server.ConstDef.EquipStatPointType; j++)
								shopItem.cEquipInfo.u2ArrStatsPoint[j] = brIn.ReadUInt16();
                            shopItem.cEquipInfo.u2TotalStatPoint = brIn.ReadUInt16();
                            shopItem.cEquipInfo.u2StatPointExp = brIn.ReadUInt16();
						}

						shopData.lstShopItem.Add(shopItem);
					}

					switch(shopData.u1ShopType)
					{
					case 1:
						ShopInfoMgr.Instance.SetShopItem(shopData);
						break;

					case 2:
						ShopInfoMgr.Instance.SetShopEquip(shopData);
						break;

					case 3:
						ShopInfoMgr.Instance.SetShopBlack(shopData);
						break;
					}
				}
				else
				{
					DebugMgr.LogError(err.ToString());
					PopupManager.Instance.CloseLoadingPopup();
					PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
				}
			}
				
			callBack(err);
			return err;
		}

		public static ERROR_ID ShopBuy(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(ServerMgr.bConnectToServer)
			{
                if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					//err = ERROR_ID.NONE;
                    PopupManager.Instance.CloseLoadingPopup();
					PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
				}
				
				if(err == ERROR_ID.NONE)
				{
					PopupManager.Instance.CloseLoadingPopup();
					Legion.Instance.Gold = brIn.ReadUInt32 ();
					Legion.Instance.Cash = brIn.ReadUInt32 ();
					Legion.Instance.effectEventID = brIn.ReadUInt16 ();
				}
			}
			
			callBack(err);
			EventInfoMgr.Instance.CheckChangeEvent (Legion.Instance.effectEventID);
			return err;
		}

		public static ERROR_ID ShopReigister(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				
				if(err == ERROR_ID.NONE)
				{

				}
			}
			
			callBack(err);
			return err;
		}
		
		public static ERROR_ID ShopFixShop(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				
				if (err == ERROR_ID.NONE) {
					Byte itemCount = brIn.ReadByte ();
					ShopGoodInfo shopInfo = ShopInfoMgr.Instance.dicShopGoodData [(UInt16)obj1];
					
					ShopInfoMgr.Instance.lstFixItem.Clear ();
					
					if (itemCount == 0) {
						
					} else {
						for (int i = 0; i < itemCount; i++) {						
							ShopItem shopItem = new ShopItem ();
							shopItem.u1Type = brIn.ReadByte ();
							shopItem.u2ItemID = brIn.ReadUInt16 ();
							shopItem.u4Count = brIn.ReadUInt32 ();
							shopItem.cEquipInfo = null;						
		
							if (shopItem.u2ItemID >= 10000 && shopItem.u2ItemID < 30000) {
								shopItem.cEquipInfo = new ShopItemEquip ();
								
								shopItem.cEquipInfo.u1SmithingLevel = brIn.ReadByte ();
								shopItem.cEquipInfo.u2ModelID = brIn.ReadUInt16 ();
								shopItem.cEquipInfo.u2Level = (Byte)brIn.ReadUInt16 ();
								shopItem.cEquipInfo.u8Exp = 0;
								shopItem.cEquipInfo.u1Completeness = brIn.ReadByte ();
		
								//shopItem.cEquipInfo.u2ArrBaseStat = new UInt16[Server.ConstDef.EquipStatPointType];
								shopItem.cEquipInfo.u4ArrBaseStat = new UInt32[Server.ConstDef.EquipStatPointType];
								
								for (int j = 0; j < Server.ConstDef.EquipStatPointType; j++)
									shopItem.cEquipInfo.u4ArrBaseStat [j] = brIn.ReadUInt32 ();//brIn.ReadUInt16();
		
								shopItem.cEquipInfo.u1BuyPoint = 0;
								shopItem.cEquipInfo.u1VipPoint = 0;
		
								shopItem.cEquipInfo.u1ArrSkillSlots = new Byte[Server.ConstDef.SkillOfEquip];
								shopItem.cEquipInfo.u1ArrSkillPoint = new UInt16[Server.ConstDef.SkillOfEquip];
								
								Byte skillCount = brIn.ReadByte ();
								ForgeInfo smithingForgeInfo = ForgeInfoMgr.Instance.GetList () [(shopItem.cEquipInfo.u1SmithingLevel - 1)];
								for (int j = 0; j < skillCount; j++) {
									shopItem.cEquipInfo.u1ArrSkillSlots [j] = brIn.ReadByte ();
									if (shopItem.cEquipInfo.u1ArrSkillSlots [j] > 0)
										shopItem.cEquipInfo.u1ArrSkillPoint [j] = smithingForgeInfo.cSmithingInfo.u1SkillInitLevel;
								}
		
								shopItem.cEquipInfo.u2ArrStatsPoint = new UInt16[Server.ConstDef.EquipStatPointType];
								for (int j = 0; j < Server.ConstDef.EquipStatPointType; j++)
									shopItem.cEquipInfo.u2ArrStatsPoint [j] = 0;
                                        
								//장비 아이템 추가
								UInt32[] au2Stats = new UInt32[Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType * 2];

								for (int j = 0; j < Server.ConstDef.SkillOfEquip; j++)
									au2Stats [j] = shopItem.cEquipInfo.u1ArrSkillPoint [j];

								for (int j = 0; j < Server.ConstDef.EquipStatPointType; j++) {
									au2Stats [Server.ConstDef.SkillOfEquip + j] = shopItem.cEquipInfo.u4ArrBaseStat [j];//u2ArrBaseStat[j];
									au2Stats [Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType + j] = shopItem.cEquipInfo.u2ArrStatsPoint [j];
								}

								Legion.Instance.cInventory.AddEquipment (0, 0, shopItem.u2ItemID, shopItem.cEquipInfo.u2Level, shopItem.cEquipInfo.u8Exp, shopItem.cEquipInfo.u1ArrSkillSlots,
									au2Stats, 0, shopItem.cEquipInfo.strItemName, shopItem.cEquipInfo.strCreater, shopItem.cEquipInfo.u2ModelID, true, shopItem.cEquipInfo.u1SmithingLevel, 0, 0, shopItem.cEquipInfo.u1Completeness);
                                
								EquipmentInfo eInfo = EquipmentInfoMgr.Instance.GetInfo (shopItem.u2ItemID);

								Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.EquipLevel, eInfo.u2ID, (Byte)eInfo.u1PosID, 0, 0, shopItem.cEquipInfo.u2Level);
								Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.GetEquip, eInfo.u2ID, (Byte)eInfo.u1PosID, shopItem.cEquipInfo.u1SmithingLevel, (Byte)eInfo.u2ClassID, 1);
								Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.Lottery, 0, shopInfo.u1Type, 0, 0, 1);
							}
							
							ShopInfoMgr.Instance.lstFixItem.Add (shopItem);	                                                      
						}
					}

                    if((GoodsType)shopInfo.cBuyGoods.u1Type == GoodsType.CASH)
					    Legion.Instance.bCheckCash = true;

                    Legion.Instance.ServerCash = brIn.ReadUInt32 ();
					Legion.Instance.effectEventID = brIn.ReadUInt16 ();

					if (shopInfo.u1Type == 8 || shopInfo.u1Type == 9) {
						Legion.Instance.cEvent.AddBuyCount (shopInfo, 1, Legion.Instance.effectEventID);
					}
				}
			}
			
			callBack(err);
			EventInfoMgr.Instance.CheckChangeEvent (Legion.Instance.effectEventID);
			return err;
		}		
	}

    class LeagueHandler
    {
        public static ERROR_ID LeagueMatch(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				
				if(err == ERROR_ID.NONE)
				{
					LeagueMatch leagueMatch = new LeagueMatch();

                    leagueMatch.strLegionName = brIn.ReadString();
                    leagueMatch.u4Point = brIn.ReadUInt32();
                    leagueMatch.u2Win = brIn.ReadUInt16();
                    leagueMatch.u2Draw = brIn.ReadUInt16();
                    leagueMatch.u2Lose = brIn.ReadUInt16();
                    leagueMatch.lstLeagueCharInfo = new List<LeagueCharInfo>();
                    leagueMatch.u1CharCount = brIn.ReadByte();
                    
                    for(int i=0; i<leagueMatch.u1CharCount; i++)
                    {
                        LeagueCharInfo leagueCharInfo = new LeagueCharInfo();

                        leagueCharInfo.u1CharIndex = brIn.ReadByte();
                        leagueCharInfo.strCharName = brIn.ReadString();
                        leagueCharInfo.u2ClassID = brIn.ReadUInt16();
                        leagueCharInfo.u2Level = brIn.ReadUInt16();
                        leagueCharInfo.u1CrewPos = brIn.ReadByte();
                        leagueCharInfo.u1Shape = new Byte[Server.ConstDef.LengthOfShape];
                        for(int j=0; j<Server.ConstDef.LengthOfShape; j++)
                            leagueCharInfo.u1Shape[j] = brIn.ReadByte();
                        leagueCharInfo.u2HPPoint = brIn.ReadUInt16();
                        leagueCharInfo.u2StrPoint = brIn.ReadUInt16();
                        leagueCharInfo.u2IntPoint = brIn.ReadUInt16();
                        leagueCharInfo.u2DefPoint = brIn.ReadUInt16();
                        leagueCharInfo.u2ResPoint = brIn.ReadUInt16();
                        leagueCharInfo.u2AgiPoint = brIn.ReadUInt16();
                        leagueCharInfo.u2CriPoint = brIn.ReadUInt16();
                        leagueCharInfo.u2EquipItemSlot = new UInt16[Hero.MAX_EQUIP_OF_CHAR];
                        for(int j=0; j<Hero.MAX_EQUIP_OF_CHAR; j++)
                            leagueCharInfo.u2EquipItemSlot[j] = brIn.ReadUInt16();
                        //leagueMatch.lstLeagueCharInfo.Add(leagueCharInfo);
                        
                        leagueMatch.lstLeagueCharInfo.Add(leagueCharInfo);
                    }
					leagueMatch.dicLeagueCharEquipInfo = new Dictionary<ushort, LeagueCharEquipInfo>();
					leagueMatch.u2EquipCount = brIn.ReadUInt16();
                    //DebugMgr.LogError("Equip");
                    int charCnt = 0;
                    for(int j=0; j<leagueMatch.u2EquipCount; j++)
                    {
                        LeagueCharEquipInfo leagueCharEquipInfo = new LeagueCharEquipInfo();

                        leagueCharEquipInfo.u2Slot = brIn.ReadUInt16();
                        leagueCharEquipInfo.u2ItemID = brIn.ReadUInt16();
                        leagueCharEquipInfo.strItemName = brIn.ReadString();
                        leagueCharEquipInfo.u1SmithingLevel = brIn.ReadByte();
                        leagueCharEquipInfo.u2ModelID = brIn.ReadUInt16();
                        //DebugMgr.LogError("ModelID " + leagueCharEquipInfo.u2ModelID);
                        leagueCharEquipInfo.u2Level = brIn.ReadUInt16();
                        leagueCharEquipInfo.u1Completeness = brIn.ReadByte();
                        leagueCharEquipInfo.u4Stat = new UInt32[ConstDef.SkillOfEquip + ConstDef.EquipStatPointType * 2];
                        leagueCharEquipInfo.u1SkillSlot = new Byte[Server.ConstDef.SkillOfEquip];
                        for(int k=0; k<Server.ConstDef.EquipStatPointType; k++)
                        {
                            leagueCharEquipInfo.u4Stat[k + ConstDef.SkillOfEquip] = brIn.ReadUInt32();
                        }
                        for(int k=0; k<Server.ConstDef.SkillOfEquip; k++)
                        {
                            leagueCharEquipInfo.u1SkillSlot[k] = brIn.ReadByte();
                            leagueCharEquipInfo.u4Stat[k] = brIn.ReadUInt16();
                        }
                        for(int k=0; k<Server.ConstDef.EquipStatPointType; k++)
                        {
                            leagueCharEquipInfo.u4Stat[k + ConstDef.SkillOfEquip + ConstDef.EquipStatPointType] = brIn.ReadUInt16();
                        }

						leagueMatch.dicLeagueCharEquipInfo.Add(leagueCharEquipInfo.u2Slot, leagueCharEquipInfo);
                        if((j+1)%10 == 0)
                            charCnt++;
                    }

                    leagueMatch.u2CharSkillCount = brIn.ReadUInt16();
                    leagueMatch.lstLeagueCharSkillInfo = new List<LeagueCharSkillInfo>();
                    for(int j=0; j<leagueMatch.u2CharSkillCount; j++)
                    {
                        LeagueCharSkillInfo leagueCharSkillInfo = new LeagueCharSkillInfo();

                        leagueCharSkillInfo.u1CharIndex = brIn.ReadByte();
                        leagueCharSkillInfo.u1SkillSlot = brIn.ReadByte();
                        leagueCharSkillInfo.u2Level = brIn.ReadUInt16();
                        leagueCharSkillInfo.u1SelectSlot = brIn.ReadByte();

                        leagueMatch.lstLeagueCharSkillInfo.Add(leagueCharSkillInfo);
                    }

                    UI_League.Instance.SetLeagueMatchData(leagueMatch);
				}
			}
			
			callBack(err);
			return err;
        }

        public static ERROR_ID LeagueMatchResult(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				
				if(err == ERROR_ID.NONE)
				{
					Legion.Instance.u2LastBattleResultPoint = brIn.ReadInt16();

					Legion.Instance.cQuest.UpdateAchieveCnt ((Byte)AchievementTypeData.LeagueMatch, 0, (Byte)(UI_League.Instance.u1SelectEnemyCrewRevenge + 1), Legion.Instance.GetDivision, 0, 1);
                    UI_League.Instance.u1SelectEnemyCrewRevenge = 0;
                }
			}
			
			callBack(err);
			return err;
        }

        public static ERROR_ID LeagueMatchStart(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				
				if(err == ERROR_ID.NONE)
				{
                    LeagueMatchStart _matchStart = new LeagueMatchStart();
                    _matchStart.u2FieldID = brIn.ReadUInt16();
                    _matchStart.u1DayOrNight = brIn.ReadByte();
                    _matchStart.u1SkyBox = brIn.ReadByte();
                    _matchStart.u2Key = brIn.ReadUInt16();
                    _matchStart.u2KeyLeftTime = brIn.ReadUInt16();
                    Legion.Instance.LeagueKey = _matchStart.u2Key;
                    if (Legion.Instance.LeagueKey < LegionInfoMgr.Instance.leagueKeyTime.MAX_COUNT)
						Legion.Instance.nextLeagueEnergyChargeTime = Legion.Instance.ServerTime.AddSeconds (_matchStart.u2KeyLeftTime);
                    UI_League.Instance.SetLeagueMatchStart(_matchStart);


				}
			}
			
			callBack(err);
			return err;
        }

		public static ERROR_ID LeagueRevengeMsg(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{

				}
			}

			callBack(err);
			return err;
		}

        public static ERROR_ID LeagueSetCrew(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				
				if(err == ERROR_ID.NONE)
				{
                    
				}
                else
                {
                    Legion.Instance.cLeagueCrew.UndoChanging();
                }
			}
			
			callBack(err);
			return err;
        }

        public static ERROR_ID LeagueReward(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				
				if(err == ERROR_ID.NONE)
				{
					LeagueReward leagueReward = new LeagueReward();

                    leagueReward.u1LastDivision = brIn.ReadByte();
                    leagueReward.u1DivisionRank = brIn.ReadByte();
                    if(leagueReward.u1DivisionRank > 0)
                    {
                        leagueReward.u1DivRwdCount = brIn.ReadByte();
                        leagueReward.lstDivRwdItem = new List<LeagueReward.DivRwdItem>();
                        for(int i=0; i<leagueReward.u1DivRwdCount; i++)
                        {
                            LeagueReward.DivRwdItem divRwdItem = new LeagueReward.DivRwdItem();
                            divRwdItem.u1DivRwdType = brIn.ReadByte();
                            divRwdItem.u2DivRwdID = brIn.ReadUInt16();
                            divRwdItem.u4DivRwdNumber = brIn.ReadUInt32();

                            leagueReward.lstDivRwdItem.Add(divRwdItem);
                        }
                    }
                    leagueReward.u1PromotionCount = brIn.ReadByte();
                    leagueReward.lstProRwdItem = new List<LeagueReward.PromotionRwditem>();
                    for(int i=0; i<leagueReward.u1PromotionCount; i++)
                    {
                        LeagueReward.PromotionRwditem promotionRwdItem = new LeagueReward.PromotionRwditem();
                        promotionRwdItem.u1Division = brIn.ReadByte();
                        promotionRwdItem.u1ProRwdCount = brIn.ReadByte();
                        promotionRwdItem.lstProRwdItem = new List<LeagueReward.DivRwdItem>();
                        for(int j=0; j<promotionRwdItem.u1ProRwdCount; j++)
                        {
                            LeagueReward.DivRwdItem proRwdItem = new LeagueReward.DivRwdItem();
                            proRwdItem.u1DivRwdType = brIn.ReadByte();
                            proRwdItem.u2DivRwdID = brIn.ReadUInt16();
                            proRwdItem.u4DivRwdNumber = brIn.ReadUInt32();

                            promotionRwdItem.lstProRwdItem.Add(proRwdItem);
                        }
                        leagueReward.lstProRwdItem.Add(promotionRwdItem);
                    }

                    UI_League.Instance.SetLeagueReward(leagueReward);
				}
			}
			
			callBack(err);
			return err;
        } 

        public static ERROR_ID LeagueInfo(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				
				if(err == ERROR_ID.NONE)
				{
					LeagueInfomation leagueInfomation = new LeagueInfomation();

                    leagueInfomation.u1CrewCount = brIn.ReadByte();
                    leagueInfomation.lstLeagueCrewInfo = new List<LeagueCrewInfo>();

                    for(int i=0; i<leagueInfomation.u1CrewCount; i++)
                    {
                        LeagueCrewInfo leagueCrewInfo = new LeagueCrewInfo();

                        leagueCrewInfo.u1CrewIndex = brIn.ReadByte();
                        leagueCrewInfo.u1DivisionIndex = brIn.ReadByte();
                        leagueCrewInfo.u2GroupNo = brIn.ReadUInt16();
                        leagueCrewInfo.u1State = brIn.ReadByte();
                        leagueCrewInfo.u1Rank = brIn.ReadByte();
                        leagueCrewInfo.u1Legend = brIn.ReadByte();
                        leagueCrewInfo.u2LegendRank = brIn.ReadUInt16();
                        leagueCrewInfo.u1Reward = brIn.ReadByte();
                        leagueCrewInfo.u2Point = brIn.ReadUInt16();
                        leagueCrewInfo.u2Win = brIn.ReadUInt16();
                        leagueCrewInfo.u2Lose = brIn.ReadUInt16();
                        leagueCrewInfo.u1LeftMatchKey = brIn.ReadByte();
                        leagueCrewInfo.u1LeagueDispatch = brIn.ReadByte();
                        leagueCrewInfo.u8DispatchTime = brIn.ReadInt64();
                        leagueCrewInfo.dtDispatchTime = DateTime.FromBinary(leagueCrewInfo.u8DispatchTime);

                        leagueInfomation.lstLeagueCrewInfo.Add(leagueCrewInfo);
                        //Legion.Instance.acCrews[leagueCrewInfo.u1CrewIndex-1].SetDivision(leagueCrewInfo.u1DivisionIndex); //2016.11.22 jc
                    }
                    UI_League.Instance.SetLeagueInfomation(leagueInfomation);
				}
			}
			
			callBack(err);
			return err;
        }

        public static ERROR_ID LeagueGroup(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				
				if(err == ERROR_ID.NONE)
				{
					LeagueGroup leagueGroup = new LeagueGroup();

                    leagueGroup.u1State = brIn.ReadByte();
                    leagueGroup.u1CrewCount = brIn.ReadByte();
                    leagueGroup.lstLeagueCrewListInfo = new List<LeagueCrewListInfo>();

                    for(int i=0; i<leagueGroup.u1CrewCount; i++)
                    {
                        LeagueCrewListInfo leagueCrewListInfo = new LeagueCrewListInfo();

                        leagueCrewListInfo.strLegionName = brIn.ReadString();
                        leagueCrewListInfo.u1CrewIndex = brIn.ReadByte();
                        leagueCrewListInfo.u2Win = brIn.ReadUInt16();
                        leagueCrewListInfo.u2Lose = brIn.ReadUInt16();
                        leagueCrewListInfo.u2Point = brIn.ReadUInt16();
                        leagueCrewListInfo.u1Legend = brIn.ReadByte();

                        leagueGroup.lstLeagueCrewListInfo.Add(leagueCrewListInfo);
                    }
                    UI_League.Instance.SetLeagueGroup(leagueGroup);
				}
			}
			
			callBack(err);
			return err;
        }

        public static ERROR_ID LeagueGroups(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				
				if(err == ERROR_ID.NONE)
				{
					LeagueGroups leagueGroups = new LeagueGroups();

                    leagueGroups.u1GroupCount = brIn.ReadByte();

                    leagueGroups.lstLeagueGroupInfo = new List<LeagueGroupsInfo>();
                    for(int i=0; i<leagueGroups.u1GroupCount; i++)
                    {
                        LeagueGroupsInfo leagueGroupsInfo = new LeagueGroupsInfo();

                        leagueGroupsInfo.u2GroupID = brIn.ReadUInt16();
                        leagueGroupsInfo.u1CrewCount = brIn.ReadByte();

                        leagueGroupsInfo.lstLeagueGroupsCrewInfo = new List<LeagueGroupsCrewInfo>();
                        for(int j=0; j<leagueGroupsInfo.u1CrewCount; j++)
                        {
                            LeagueGroupsCrewInfo leagueGroupsCrewInfo = new LeagueGroupsCrewInfo();

                            leagueGroupsCrewInfo.strLegionName = brIn.ReadString();
                            leagueGroupsCrewInfo.u1CrewIndex = brIn.ReadByte();
                            leagueGroupsCrewInfo.u2Win = brIn.ReadUInt16();
                            leagueGroupsCrewInfo.u2Lose = brIn.ReadUInt16();
                            leagueGroupsCrewInfo.u2Point = brIn.ReadUInt16();
                            leagueGroupsCrewInfo.u1Legend = brIn.ReadByte();

                            leagueGroupsInfo.lstLeagueGroupsCrewInfo.Add(leagueGroupsCrewInfo);
                        }

                        leagueGroups.lstLeagueGroupInfo.Add(leagueGroupsInfo);
                    }

                    UI_League.Instance.SetLeagueGroups(leagueGroups);
				}
			}
			
			callBack(err);
			return err;
        }

		public static ERROR_ID SetCrewRune(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				
				if(err == ERROR_ID.NONE)
				{

				}
			}
			
			callBack(err);
			return err;
		}

        public static ERROR_ID LeagueLegend(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				
				if(err == ERROR_ID.NONE)
				{
					LeagueLegendRank leagueLegend = new LeagueLegendRank();

                    leagueLegend.u8CloseTime = brIn.ReadInt64();
                    DateTime tempTime = DateTime.FromBinary(leagueLegend.u8CloseTime);
                    leagueLegend.tsLeftTime = tempTime - Legion.Instance.ServerTime;
                    leagueLegend.u1Count = brIn.ReadByte();
                    leagueLegend.sRankInfo = new LeagueLegendRank.RankInfo[leagueLegend.u1Count];
                    for(int i=0; i<leagueLegend.u1Count; i++)
                    {
                        leagueLegend.sRankInfo[i].u4Rank = brIn.ReadUInt32();
                        leagueLegend.sRankInfo[i].strLegionName = brIn.ReadString();
                        leagueLegend.sRankInfo[i].u4Point = brIn.ReadUInt32();
                        leagueLegend.sRankInfo[i].u2Win = brIn.ReadUInt16();
                        leagueLegend.sRankInfo[i].u2Draw = brIn.ReadUInt16();
                        leagueLegend.sRankInfo[i].u2Lose = brIn.ReadUInt16();
                    }
                    UI_League.Instance.SetLeagueLegend(leagueLegend);
				}
			}
			
			callBack(err);
			return err;
        }

        public static ERROR_ID LeagueMatchList(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				
				if(err == ERROR_ID.NONE)
				{
					if (UI_League.Instance.cLeagueMatchList.u4MyRank != 0) {
						UI_League.Instance.u1BeforeRank = UI_League.Instance.cLeagueMatchList.u4MyRank;
					}
                    LeagueMatchList tempList = new LeagueMatchList();
                    tempList.u1DivisionIndex = brIn.ReadByte();

					if (UI_League.Instance.cLeagueMatchList.u1DivisionIndex > 0) {
						if (UI_League.Instance.cLeagueMatchList.u1DivisionIndex < tempList.u1DivisionIndex) {
							UI_League.Instance.u1Prom = 1;
						} else if (UI_League.Instance.cLeagueMatchList.u1DivisionIndex > tempList.u1DivisionIndex) {
							UI_League.Instance.u1Prom = 2;
						} else {
							UI_League.Instance.u1Prom = 0;
						}
					}

                    tempList.u1LastCheckDicisionIndex = brIn.ReadByte();
					if(tempList.u1LastCheckDicisionIndex == 0)
					{
						tempList.u1PrevDivisionIndex = brIn.ReadByte();
						tempList.u4PrevMyRank = brIn.ReadUInt32();
					}

                    tempList.u4MyPoint = brIn.ReadUInt32();
                    //tempList.u1Reward = brIn.ReadByte();
					Legion.Instance.u1LeagueReward = brIn.ReadByte();
                    tempList.u4MyRank = brIn.ReadUInt32();

					if (UI_League.Instance.u1Prom != 0) {
						UI_League.Instance.u1BeforeRank = tempList.u4MyRank;
					}

                    tempList.u1Count = brIn.ReadByte();
                    tempList.sListSlotData = new LeagueMatchList.ListSlotData[tempList.u1Count];
                    List<LeagueMatchList.ListSlotData> lstSlots = new List<global::LeagueMatchList.ListSlotData>();
                    for(int i=0; i<tempList.u1Count; i++)
                    {
                        tempList.sListSlotData[i].u4Rank = brIn.ReadUInt32();
                        tempList.sListSlotData[i].u8UserSN = brIn.ReadUInt64();
                        tempList.sListSlotData[i].u2ClassID = brIn.ReadUInt16();
                        tempList.sListSlotData[i].strLegionName = brIn.ReadString();
                        tempList.sListSlotData[i].u4Point = brIn.ReadUInt32();
                        tempList.sListSlotData[i].u8Time = brIn.ReadInt64();
                        tempList.sListSlotData[i].dtTime = DateTime.FromBinary(tempList.sListSlotData[i].u8Time);
                        tempList.sListSlotData[i].u1Revenge = brIn.ReadByte();
                        tempList.sListSlotData[i].strRevengeMessage = brIn.ReadString();
                        if(tempList.sListSlotData[i].u1Revenge == 2 || tempList.sListSlotData[i].u1Revenge == 3)
                        {
                            UI_League.Instance.RevengeCrew = tempList.sListSlotData[i];
                        }
                        //if(tempList.sListSlotData[i].u1Revenge != 3)
                            lstSlots.Add(tempList.sListSlotData[i]);
                    }
                    //UI_League.Instance.cLeagueMatchList = tempList;
                    lstSlots.Sort
                    (
                        delegate(LeagueMatchList.ListSlotData x, LeagueMatchList.ListSlotData y) 
                        {
                            int compare = 0;

                            if(compare == 0)
                                compare = x.u4Rank.CompareTo(y.u4Rank);

                            return compare;
                        }
                    );
                    tempList.sListSlotData = lstSlots.ToArray();
                    UI_League.Instance.cLeagueMatchList = tempList;
					Legion.Instance.u1LeagueKeyBuyCount = brIn.ReadByte();
                    Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.LeagueDivision, 0, Legion.Instance.GetDivision, 0, 0, 1);
                }
            }
			
			callBack(err);
			return err;
        }

        public static ERROR_ID LeagueBuyKey(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				
				if(err == ERROR_ID.NONE)
				{
					Legion.Instance.LeagueKey = brIn.ReadUInt16();
					UInt16 leftTime = brIn.ReadUInt16(); 
					if (Legion.Instance.LeagueKey < LegionInfoMgr.Instance.leagueKeyTime.MAX_COUNT)
					Legion.Instance.nextLeagueEnergyChargeTime = Legion.Instance.ServerTime.AddSeconds (leftTime);
                }
            }
			
			callBack(err);
			return err;
        }

        public static ERROR_ID LeagueDivisionCheck(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				
				if(err == ERROR_ID.NONE)
				{
                    
                }
            }
			
			callBack(err);
			return err;
        }
    }

	class ForgeHandler
	{
		public static ERROR_ID ForgeRune(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object objEquipID, System.Object aobjSkillSlots)
		{
			if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				
				if(err == ERROR_ID.NONE)
				{
					Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.MakeRune, 0, 0, 0, 0, 1);
				}
			}
			
			callBack(err);
			return err;
		}

		public static ERROR_ID SmithingSuccess(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			DebugMgr.Log("SmithingSuccess In Handler");
			if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
//					err = ERROR_ID.NONE;

						PopupManager.Instance.CloseLoadingPopup();
						
						PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
				}
				
				if(err == ERROR_ID.NONE)
				{
					DebugMgr.Log("SmithingSuccess In Handler No Err");
					UI_Panel_Forge_Smithing_Detail smithingDetail = (UI_Panel_Forge_Smithing_Detail)obj1;

					// Create EquipmentItem at Server Data
					UInt16 equipID = smithingDetail._cEquipInfo.u2ID;
					// [스킬부여포인트3개, 초기스탯(랜덤)3개(3가지 스테이터스 고정), 스탯부여포인트 3개] 
					UInt32[] stats = new UInt32[ConstDef.SkillOfEquip + (ConstDef.EquipStatPointType * 2)];
                    Byte u1Completeness = brIn.ReadByte();
					stats[ConstDef.SkillOfEquip] = brIn.ReadUInt32();//brIn.ReadUInt16();
					stats[ConstDef.SkillOfEquip + 1] = brIn.ReadUInt32();//brIn.ReadUInt16();
					stats[ConstDef.SkillOfEquip + 2] = brIn.ReadUInt32();//brIn.ReadUInt16();
					stats[ConstDef.SkillOfEquip + 3] = 0;
					stats[ConstDef.SkillOfEquip + 4] = 0;
					stats[ConstDef.SkillOfEquip + 5] = 0;

					Byte skillCount = brIn.ReadByte();
					Byte[] skillSlots = new Byte[Server.ConstDef.CharStatPointType];
					for(int i=0; i<skillCount; i++)
					{
						skillSlots[i] = brIn.ReadByte();
                        if(skillSlots[i] > 0)
                            stats[i] = smithingDetail._cForgeInfo.cSmithingInfo.u1SkillInitLevel;
					}
					EquipmentInfo equipInfo = EquipmentInfoMgr.Instance.GetInfo(equipID);

					UInt16 invenSlotNum = Legion.Instance.cInventory.AddEquipment(
						0, 0, equipID, 1, 0, skillSlots, stats, 0, "", Legion.Instance.sName,equipInfo.u2ModelID, true, smithingDetail._cForgeInfo.u1Level, 0, 0, u1Completeness);
					smithingDetail._u2LastSlotNum = invenSlotNum;
                    Legion.Instance.cInventory.dicInventory[invenSlotNum].isNew = true;
					Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.GetEquip,equipInfo.u2ID,(Byte)equipInfo.u1PosID,smithingDetail._cForgeInfo.u1Level,(Byte)equipInfo.u2ClassID,1);
					Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.MakeEquip,equipInfo.u2ID,(Byte)equipInfo.u1PosID,smithingDetail._cForgeInfo.u1Level,(Byte)equipInfo.u2ClassID,1);
					Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.CreateEquipGrade, 0, u1Completeness, smithingDetail._cForgeInfo.u1Level, (Byte)equipInfo.u1PosID, 1);

					Legion.Instance.Gold = brIn.ReadUInt32();
					Legion.Instance.effectEventID = brIn.ReadUInt16();
                }
			}
			else
			{
				UI_Panel_Forge_Smithing_Detail smithingDetail = (UI_Panel_Forge_Smithing_Detail)obj1;
				UInt16 equipID = smithingDetail._cEquipInfo.u2ID;

				UInt16 modelID = EquipmentInfoMgr.Instance.GetInfo(equipID).cModel.u2ID;
				Byte[] slots = new Byte[Server.ConstDef.SkillOfEquip];
				UInt32[] stats = new UInt32[Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType * 2];
				
				for(int i=0; i<stats.Length; i++)
				{
					stats[i] = 0;
				}
				stats[0] = (UInt16)(UnityEngine.Random.Range((int)EquipmentInfoMgr.Instance.GetInfo(equipID).acStatAddInfo[0].u2BaseStatMin,
				                                          (int)EquipmentInfoMgr.Instance.GetInfo(equipID).acStatAddInfo[0].u2BaseStatMax)
					+ ((int)EquipmentInfoMgr.Instance.GetInfo(equipID).acStatAddInfo[0].u2AddStatMaxForgeLevel * Legion.Instance.u1ForgeLevel));
				stats[1] = (UInt16)(UnityEngine.Random.Range((int)EquipmentInfoMgr.Instance.GetInfo(equipID).acStatAddInfo[1].u2BaseStatMin,
				                                          (int)EquipmentInfoMgr.Instance.GetInfo(equipID).acStatAddInfo[1].u2BaseStatMax)
				                                                   + ((int)EquipmentInfoMgr.Instance.GetInfo(equipID).acStatAddInfo[1].u2AddStatMaxForgeLevel * Legion.Instance.u1ForgeLevel));
				stats[2] = (UInt16)(UnityEngine.Random.Range((int)EquipmentInfoMgr.Instance.GetInfo(equipID).acStatAddInfo[2].u2BaseStatMin,
				                                          (int)EquipmentInfoMgr.Instance.GetInfo(equipID).acStatAddInfo[2].u2BaseStatMax)
				                                                   + ((int)EquipmentInfoMgr.Instance.GetInfo(equipID).acStatAddInfo[2].u2AddStatMaxForgeLevel * Legion.Instance.u1ForgeLevel));

				Byte[] skillSlots = new byte[Server.ConstDef.SkillOfEquip];
				for(int i=0; i<smithingDetail._cForgeInfo.cSmithingInfo.u1RandomSkillCount; i++)
				{
					skillSlots[i] = (Byte)UnityEngine.Random.Range(1, 21);
				}
				for(int i=smithingDetail._cForgeInfo.cSmithingInfo.u1RandomSkillCount; i<(smithingDetail._cForgeInfo.cSmithingInfo.u1RandomSkillCount+smithingDetail._cForgeInfo.cSmithingInfo.u1SelectSkillCount); i++)
				{
					skillSlots[i] = smithingDetail._au1SelectedSkill[(i-smithingDetail._cForgeInfo.cSmithingInfo.u1RandomSkillCount)];
				}
				EquipmentInfo equipInfo = EquipmentInfoMgr.Instance.GetInfo(equipID);
				UInt16 invenSlotNum = Legion.Instance.cInventory.AddEquipment(
					0, 0, equipID, 1, 0, skillSlots, stats, 0, "", Legion.Instance.sName,equipInfo.u2ModelID, true, smithingDetail._cForgeInfo.u1Level);
				smithingDetail._u2LastSlotNum = invenSlotNum;
//				UI_Panel_Forge_Smithing_Result smithingResult = (UI_Panel_Forge_Smithing_Result)obj2;
//				smithingResult.SetData((EquipmentItem)Legion.Instance.cInventory.dicInventory[invenSlotNum], smthingInfo._cForgeInfo.u1Level);
				Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.GetEquip,equipInfo.u2ID,(Byte)equipInfo.u1PosID,smithingDetail._cForgeInfo.u1Level,(Byte)equipInfo.u2ClassID,1);
				Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.MakeEquip,equipInfo.u2ID,(Byte)equipInfo.u1PosID,smithingDetail._cForgeInfo.u1Level,(Byte)equipInfo.u2ClassID,1);
				Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.CreateEquipGrade, 0, 1, smithingDetail._cForgeInfo.u1Level, (Byte)equipInfo.u1PosID, 1);
			}

			callBack(err);
			EventInfoMgr.Instance.CheckChangeEvent (Legion.Instance.effectEventID);
			return err;
		}

		public static ERROR_ID CheckDesignSuccess(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(err == ERROR_ID.REQUEST_DUPLICATION)
			{
				PopupManager.Instance.CloseLoadingPopup();
				
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
			}
			
			if(err == ERROR_ID.NONE)
			{
			
			}

			callBack(err);
			return err;
		}

		public static ERROR_ID ChangeEquipName(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{

			if(err == ERROR_ID.REQUEST_DUPLICATION)
			{
				PopupManager.Instance.CloseLoadingPopup();
				
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
			}
			
			if(err == ERROR_ID.NONE)
			{
				EquipmentItem equipItem = (EquipmentItem)obj1;
				String name = (String)obj2;

				equipItem.itemName = name;
			}


			callBack(err);
			return err;
		}

		public static ERROR_ID Fusion(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{

			if(err == ERROR_ID.REQUEST_DUPLICATION)
			{
				PopupManager.Instance.CloseLoadingPopup();
				
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
			}
			
			if(err == ERROR_ID.NONE)
			{
				EquipmentItem baseEquipItem = (EquipmentItem)obj1;
				UInt16[] materialInvenSlotNums = (UInt16[])obj2;
				UInt64 addExp = 0;
                UInt16 addStatExp = 0;
				EquipmentItem equipItem = null;
				for(int i=0; i<materialInvenSlotNums.Length; i++)
				{
					equipItem = (EquipmentItem)Legion.Instance.cInventory.dicInventory[materialInvenSlotNums[i]];
                    if(equipItem.cLevel.u2Level>1)
					    addExp += ClassInfoMgr.Instance.GetAccExp((Byte)(equipItem.cLevel.u2Level-1));
					addExp += equipItem.cLevel.u8Exp;
                    addExp += ForgeInfoMgr.Instance.GetInfo(ForgeInfoMgr.Instance.GetIDs()[equipItem.u1SmithingLevel-1]).u4FusionExp;

                    int tempSmithingLvl = baseEquipItem.u1SmithingLevel - equipItem.u1SmithingLevel;
                    if (tempSmithingLvl > 0)
                        addStatExp += (UInt16)(EquipmentInfoMgr.Instance.u2BaseStatPointExp + tempSmithingLvl*(EquipmentInfoMgr.Instance.MinusStatPointExp));
                    else if(tempSmithingLvl < 0)
                        addStatExp += (UInt16)(EquipmentInfoMgr.Instance.u2BaseStatPointExp + tempSmithingLvl*(EquipmentInfoMgr.Instance.PlusStatPointExp)*-1);
                    else
                        addStatExp += EquipmentInfoMgr.Instance.u2BaseStatPointExp;
//					for(int expIdx=1; expIdx<=equipItem.cLevel.u2Level; expIdx++)
//					{
//						addExp += ClassInfoMgr.Instance.GetNextExp((Byte)expIdx);
//					}
					Legion.Instance.cInventory.dicInventory.Remove(equipItem.u2SlotNum);
				}
				
				DebugMgr.Log("Add EXP : " + addExp);

				baseEquipItem.GetComponent<LevelComponent>().AddExp(addExp);
                baseEquipItem.statusComponent.StatPointExpUp(addStatExp, baseEquipItem.cLevel.u2Level);

				DebugMgr.Log(baseEquipItem.cLevel.u2Level + "  " + baseEquipItem.cLevel.u8Exp);

				Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.FuseEquip, 0, (Byte)baseEquipItem.GetEquipmentInfo().u1PosID, 0,0,(uint)materialInvenSlotNums.Length);

				Legion.Instance.Gold = brIn.ReadUInt32 ();
				Legion.Instance.effectEventID = brIn.ReadUInt16 ();
			}

			
			callBack(err);
			EventInfoMgr.Instance.CheckChangeEvent (Legion.Instance.effectEventID);
			return err;
		}

		public static ERROR_ID ChangeLook(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					PopupManager.Instance.CloseLoadingPopup();
					
					PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
				}
				
				if(err == ERROR_ID.NONE)
				{
					UInt16 invenSlotNum = (UInt16)obj1;
					UInt16 modelID = (UInt16)obj2;
                    UInt16 oldModel;
					EquipmentItem equipItem = (EquipmentItem)Legion.Instance.cInventory.dicInventory[invenSlotNum];
                    oldModel = equipItem.u2ModelID;
					equipItem.u2ModelID = modelID;
					if (equipItem.attached.hero != null) {
						if (equipItem.attached.hero.cObject != null)
							equipItem.attached.hero.cObject.GetComponent<HeroObject> ().ChangeEquipModel (oldModel, equipItem);
					}
					Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.ChangeLook, 0, (Byte)equipItem.GetEquipmentInfo().u1PosID, (Byte)equipItem.GetEquipmentInfo().u2ClassID, 0, 1);

					Legion.Instance.Gold = brIn.ReadUInt32 ();
					Legion.Instance.effectEventID = brIn.ReadUInt16 ();
				}
			}
			else
			{
				UInt16 invenSlotNum = (UInt16)obj1;
				UInt16 modelID = (UInt16)obj2;

				EquipmentItem equipItem = (EquipmentItem)Legion.Instance.cInventory.dicInventory[invenSlotNum];

				if(modelID == 0)
				{
					int modelIdx = (UInt16)UnityEngine.Random.Range(0, (Legion.Instance.u1ForgeLevel+1));

//					equipItem.u2ModelID = equipItem.GetEquipmentInfo().au2EquipModelID[modelIdx];
				}
				else
				{
					equipItem.u2ModelID = modelID;
				}
			}

			callBack(err);
			EventInfoMgr.Instance.CheckChangeEvent (Legion.Instance.effectEventID);
			return err;
		}

		public static ERROR_ID UpgradeForge(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			
			if(err == ERROR_ID.REQUEST_DUPLICATION)
			{
				PopupManager.Instance.CloseLoadingPopup();
				
				PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), TextManager.Instance.GetText("popup_desc_server_error"), Server.ServerMgr.Instance.ApplicationShutdown);
			}
			
			if(err == ERROR_ID.NONE)
			{
                ObscuredPrefs.SetInt("SelectedForgeLevel", Legion.Instance.u1ForgeLevel);
                Legion.Instance.u1ForgeLevel++;
                Legion.Instance.cQuest.UpdateAchieveCnt(AchievementTypeData.ForgeLevel,0,(byte)(Legion.Instance.u1ForgeLevel-1),0,0,1);

                Legion.Instance.Gold = brIn.ReadUInt32();
				Legion.Instance.effectEventID = brIn.ReadUInt16 ();
            }
			
			callBack(err);
			EventInfoMgr.Instance.CheckChangeEvent (Legion.Instance.effectEventID);
			return err;
		}
	}

	class QuestHandler
	{
		public static ERROR_ID QuestAccept(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				
				if(err == ERROR_ID.NONE)
				{

				}
			}
			
			callBack(err);
			return err;
		}

		public static ERROR_ID QuestCancel(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				
				if(err == ERROR_ID.NONE)
				{
					
				}
			}
			
			callBack(err);
			return err;
		}

		public static ERROR_ID QuestComplete(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				
				if(err == ERROR_ID.NONE)
				{
					
				}
			}
			
			callBack(err);
			return err;
		}

		public static ERROR_ID AchievementReward(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				
				if(err == ERROR_ID.NONE)
				{
					Byte itemCount = brIn.ReadByte();
					for(int i = 0; i < itemCount; ++i)
					{
						AchieveItem achieveItem = new AchieveItem();
						achieveItem.cAchieveReward = new Goods(brIn.ReadByte(), brIn.ReadUInt16(), brIn.ReadUInt32());
						if(achieveItem.cAchieveReward.isEquip())
						{
							if(EquipmentInfoMgr.Instance.GetInfo(achieveItem.cAchieveReward.u2ID).ItemType == ItemInfo.ITEM_TYPE.EQUIPMENT)
							{
								achieveItem.u1SmithingLevel = brIn.ReadByte();
								achieveItem.u2ModelID = brIn.ReadUInt16();
								achieveItem.u2Level = brIn.ReadUInt16();
								achieveItem.u1Completeness = brIn.ReadByte();
								achieveItem.u4Stat = new UInt32[Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType * 2];
								for(int j=0; j<Server.ConstDef.SkillOfEquip; j++)
								{
									achieveItem.u4Stat[Server.ConstDef.SkillOfEquip+j] = brIn.ReadUInt32();//brIn.ReadUInt16();
								}
								achieveItem.u1SkillCount = brIn.ReadByte();
								achieveItem.u1SkillSlot = new Byte[Server.ConstDef.SkillOfEquip];
								for(int j=0; j<achieveItem.u1SkillCount; j++)
								{
									achieveItem.u1SkillSlot[j] = brIn.ReadByte();
                                    if(achieveItem.u1SkillSlot[j] > 0)
                                        achieveItem.u4Stat[j] = ForgeInfoMgr.Instance.GetInfo(ForgeInfoMgr.Instance.GetIDs()[achieveItem.u1SmithingLevel-1]).cSmithingInfo.u1SkillInitLevel;
								}
								Legion.Instance.cInventory.AddEquipment (
									0, 0, achieveItem.cAchieveReward.u2ID, achieveItem.u2Level, 0, achieveItem.u1SkillSlot, achieveItem.u4Stat, 0, "", 
									Legion.Instance.sName, achieveItem.u2ModelID, true, achieveItem.u1SmithingLevel, 0, 0, achieveItem.u1Completeness);
							}
						}
						QuestInfoMgr.Instance.listAchieveItemData.Add(achieveItem);
					}
				}
			}
			callBack(err);
			return err;
		}
        //#ODIN [오딘 임무 보상 받기]
        public static ERROR_ID MissionReward(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            PopupManager.Instance.CloseLoadingPopup();
            if (ServerMgr.bConnectToServer)
            {
                if (err == ERROR_ID.REQUEST_DUPLICATION)
                {
                    err = ERROR_ID.NONE;
                }
                if (err == ERROR_ID.NONE)
                {
                    Legion.Instance.cQuest.receiveOdinMissionList.Clear();
                    Byte missionCount = brIn.ReadByte();    // 임수 정보 갯수
                    for (int i = 0; i < missionCount; ++i)
                    {
                        Legion.Instance.cQuest.receiveOdinMissionList.Enqueue(new UserOdinMission(brIn.ReadUInt16(), brIn.ReadUInt32()));
                    }
                }
            }
            callBack(err);
            return err;
        }
        //#ODIN [오딘 임무 미루기]
        public static ERROR_ID MissionPushback(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            PopupManager.Instance.CloseLoadingPopup();
            if (ServerMgr.bConnectToServer)
            {
                if (err == ERROR_ID.REQUEST_DUPLICATION)
                {
                    err = ERROR_ID.NONE;
                }

                if (err == ERROR_ID.NONE)
                {
                    Legion.Instance.cQuest.receiveOdinMissionList.Clear();
                    Legion.Instance.cQuest.receiveOdinMissionList.Enqueue(new UserOdinMission(brIn.ReadUInt16(), brIn.ReadUInt32()));
                }
            }
            callBack(err);
            return err;
        }
    }
    
	class OptionHandler
	{
		public static ERROR_ID OptionPush(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				
				if(err == ERROR_ID.NONE)
				{
					Legion.Instance.pushSetting = 1;
				}
			}
			
			callBack(err);
			return err;
		}

		public static ERROR_ID OptionAccount(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
					
				}
			}

			callBack(err);
			return err;
		}

        public static ERROR_ID OptionSet(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				else if(err == ERROR_ID.NONE)
				{
					//Legion.Instance.pushSetting = brIn.ReadByte();
				}
			}
			
			callBack(err);
			return err;
		}
    }    

	class Cheat
	{
		public static ERROR_ID EtcCommand(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
				}
			}

			callBack(err);
			return err;
		}
	}

    class SocialHandler
    {
        public static ERROR_ID ReciveFriendList(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
                    SocialInfo.Instance.u1FriendCount = brIn.ReadByte();
					uint newCnt = 0;
                    SocialInfo.Instance.dicFriendCount.Clear();
                    for (int i=0; i<SocialInfo.Instance.u1FriendCount; i++)
                    {
                        FriendCount _friendCount = new FriendCount();
                        _friendCount.u8UserSN = brIn.ReadUInt64();
                        _friendCount.strLegionName = brIn.ReadString();
                        _friendCount.u2MainCharClassID = brIn.ReadUInt16();
                        _friendCount.u1MainCharElement = brIn.ReadByte();
                        _friendCount.u8LastLoginTime = brIn.ReadInt64();
                        _friendCount.dtLastLoginTime = DateTime.FromBinary(_friendCount.u8LastLoginTime);
                        _friendCount.u2Level = brIn.ReadUInt16();
                        _friendCount.u1Point = brIn.ReadByte();
                        _friendCount.u1SendPoint = Convert.ToByte(_friendCount.u1Point & 0x01);
                        _friendCount.u1RecvPoint = Convert.ToByte(_friendCount.u1Point & 0x02);
                        _friendCount.u1New = brIn.ReadByte();
						if (_friendCount.u1New == 1)
							newCnt++;
                        _friendCount.bDeleted = false;
                        SocialInfo.Instance.dicFriendCount.Add((UInt16)i, _friendCount);
                    }

					if (newCnt > 0) {
						Legion.Instance.cQuest.UpdateAchieveCnt (AchievementTypeData.Friend, 0, 0, 0, 0, newCnt);
					}

                    SocialInfo.Instance.u1FriendRequestCount = brIn.ReadByte();
                    SocialInfo.Instance.dicFriendRequestCount.Clear();
                    for (int i=0; i<SocialInfo.Instance.u1FriendRequestCount; i++)
                    {
                        FriendRequestCount _friendRequestCount = new FriendRequestCount();
                        _friendRequestCount.u8UserSN = brIn.ReadUInt64();
                        _friendRequestCount.strLegionName = brIn.ReadString();
                        _friendRequestCount.u2MainCharClassID = brIn.ReadUInt16();
                        _friendRequestCount.u1MainCharElement = brIn.ReadByte();
                        _friendRequestCount.u8LastLoginTime = brIn.ReadInt64();
                        _friendRequestCount.dtLastLoginTime = DateTime.FromBinary(_friendRequestCount.u8LastLoginTime);
                        _friendRequestCount.u2Level = brIn.ReadUInt16();
                        _friendRequestCount.bDeleted = false;
                        SocialInfo.Instance.dicFriendRequestCount.Add((UInt16)i, _friendRequestCount);
                    }

                    SocialInfo.Instance.u1FriendInviteCount = brIn.ReadByte();
                    SocialInfo.Instance.dicFriendInviteCount.Clear();
                    for (int i=0; i<SocialInfo.Instance.u1FriendInviteCount; i++)
                    {
                        FriendInviteCount _friendInviteCount = new FriendInviteCount();
                        _friendInviteCount.u8UserSN = brIn.ReadUInt64();
                        _friendInviteCount.strLegionName = brIn.ReadString();
                        _friendInviteCount.u2MainCharClassID = brIn.ReadUInt16();
                        _friendInviteCount.u1MainCharElement = brIn.ReadByte();
                        _friendInviteCount.u8LastLoginTime = brIn.ReadInt64();
                        _friendInviteCount.dtLastLoginTime = DateTime.FromBinary(_friendInviteCount.u8LastLoginTime);
                        _friendInviteCount.u2Level = brIn.ReadUInt16();
                        _friendInviteCount.u1New = brIn.ReadByte();
                        _friendInviteCount.bAccept = false;
                        _friendInviteCount.bDeleted = false;
                        SocialInfo.Instance.dicFriendInviteCount.Add((UInt16)i, _friendInviteCount);
                    }
				}
			}

			callBack(err);
			return err;
        }

        public static ERROR_ID FriendNewCheck(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
				}
			}

			callBack(err);
			return err;
        }

        public static ERROR_ID FriendInvite(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
				}
			}

			callBack(err);
			return err;
        }

        public static ERROR_ID FriendInviteCancel(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
				}
			}

			callBack(err);
			return err;
        }

        public static ERROR_ID FriendInviteConfirm(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				if(err == ERROR_ID.NONE)
				{
				}
			}

			callBack(err);
			return err;
        }

        public static ERROR_ID FriendRecommend(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
                    SocialInfo.Instance.u1FriendRecommendCount = brIn.ReadByte();
                    for(int i=0; i<SocialInfo.Instance.u1FriendRecommendCount; i++)
                    {
                        FriendRecommend _friendRecommend = new FriendRecommend();
                        _friendRecommend.u8UserSN = brIn.ReadUInt64();
                        _friendRecommend.strLegionName = brIn.ReadString();
                        _friendRecommend.u2MainCharClassID = brIn.ReadUInt16();
                        _friendRecommend.u1MainCharElement = brIn.ReadByte();
                        _friendRecommend.u8LastLoginTime = brIn.ReadInt64();
                        _friendRecommend.dtLastLoginTime = DateTime.FromBinary(_friendRecommend.u8LastLoginTime);
                        _friendRecommend.u2Level = brIn.ReadUInt16();
                        SocialInfo.Instance.dicFriendRecommend.Add((UInt16)i, _friendRecommend);
                    }
				}
			}

			callBack(err);
			return err;
        }

        public static ERROR_ID FriendDrop(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
				}
			}

			callBack(err);
			return err;
        }

        public static ERROR_ID SendFriendshipPoint(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
				}
			}

			callBack(err);
			return err;
        }

        public static ERROR_ID ReciveFriendshipPoint(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
				}
			}

			callBack(err);
			return err;
        }

        public static ERROR_ID ReciveMailList(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
                    SocialInfo.Instance.dicMailList.Clear();
                    SocialInfo.Instance.u2MailCount = brIn.ReadUInt16();
                    for(int i=0; i<SocialInfo.Instance.u2MailCount; i++)
                    {
                        MailList _mailList = new MailList();
                        _mailList.u2MailSN = brIn.ReadUInt16();
                        _mailList.u2MailTitleCode = brIn.ReadUInt16();

                        if(_mailList.u2MailTitleCode == 0)
                            _mailList.strMainTitle = brIn.ReadString();
						else if(_mailList.u2MailTitleCode > 0)
							_mailList.strMainTitle = TextManager.Instance.GetMailTitle(_mailList.u2MailTitleCode, brIn.ReadString());	
						
                        _mailList.u8ExpireTime = brIn.ReadInt64();
                        _mailList.dtExpireTime = DateTime.FromBinary(_mailList.u8ExpireTime);
                        _mailList.u1New = brIn.ReadByte();
                        _mailList.u1MailType = brIn.ReadByte();

                        if((_mailList.u1MailType == 1) || (_mailList.u1MailType == 2) || (_mailList.u1MailType == 3))
                        {
                            _mailList.u1ItemType = brIn.ReadByte();
                            _mailList.u2ItemID = brIn.ReadUInt16();
                            _mailList.u4Count = brIn.ReadUInt32();
                            _mailList.u1Grade = brIn.ReadByte();
                            _mailList.u2Level = brIn.ReadUInt16();
                        }

                        if(_mailList.u1MailType == 4)
                        {
                            _mailList.u2Slot = brIn.ReadUInt16();
                            if(_mailList.u2Slot == 0)
                            {
                                _mailList.u2MailContentCode = brIn.ReadUInt16();
                                if(_mailList.u2MailContentCode == 0)
                                    _mailList.strMailContent = brIn.ReadString();
                            }
                        }
                        _mailList.bCheckedMail = false;
                        SocialInfo.Instance.dicMailList.Add((UInt16)i, _mailList);
                    }
                }
            }

            callBack(err);
			return err;
        }

        public static ERROR_ID GetItemInMail(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
                    SocialInfo.Instance.dicMailGetItem.Clear();
                    //SocialInfo.Instance.u1ItemCount = 0;

                    SocialInfo.Instance.u2ItemCount = brIn.ReadUInt16();
                    if(SocialInfo.Instance.u2ItemCount != 0)
                    {
                        for(int i=0; i<SocialInfo.Instance.u2ItemCount; i++)
                        {
                            MailGetItem _mailGetItem = new MailGetItem();
                            _mailGetItem.u2MailSN = brIn.ReadUInt16();
                            _mailGetItem.u1ItemType = brIn.ReadByte();
                            _mailGetItem.u2ItemID = brIn.ReadUInt16();
                            _mailGetItem.u4Count = brIn.ReadUInt32();
                            if(_mailGetItem.u1ItemType == 10)
                            {
                                if(EquipmentInfoMgr.Instance.GetInfo(_mailGetItem.u2ItemID).ItemType == ItemInfo.ITEM_TYPE.EQUIPMENT)
                                {
                                    _mailGetItem.u1SmithingLevel = brIn.ReadByte();
                                    _mailGetItem.u2ModelID = brIn.ReadUInt16();
                                    _mailGetItem.u2Level = brIn.ReadUInt16();
                                    _mailGetItem.u1Completeness = brIn.ReadByte();
                                    _mailGetItem.u4Stat = new UInt32[Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType * 2];
                                    for(int j=0; j<Server.ConstDef.SkillOfEquip; j++)
                                    {
										_mailGetItem.u4Stat[Server.ConstDef.SkillOfEquip+j] = brIn.ReadUInt32();//brIn.ReadUInt16();
                                    }
                                    _mailGetItem.u1SkillCount = brIn.ReadByte();
                                    _mailGetItem.u1SkillSlot = new Byte[Server.ConstDef.SkillOfEquip];
                                    for(int j=0; j<_mailGetItem.u1SkillCount; j++)
                                    {
                                        _mailGetItem.u1SkillSlot[j] = brIn.ReadByte();
                                        if(_mailGetItem.u1SkillSlot[j] > 0)
                                            _mailGetItem.u4Stat[j] = ForgeInfoMgr.Instance.GetInfo(ForgeInfoMgr.Instance.GetIDs()[_mailGetItem.u1SmithingLevel-1]).cSmithingInfo.u1SkillInitLevel;
                                    }
                                }
                            }
                            SocialInfo.Instance.dicMailGetItem.Add((UInt16)i, _mailGetItem);
                        }
                    }
                    
				}
			}

			callBack(err);
			return err;
        }

        public static ERROR_ID ReadMail(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
                }
            }

            callBack(err);
			return err;
        }

        public static ERROR_ID GetNotice(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
                    SocialInfo.Instance.dicNotice.Clear();
                    //SocialInfo.Instance.dicNoticeEvent.Clear();
                    SocialInfo.Instance.lstNoticeEventList.Clear();

                    SocialInfo.Instance.u1NoticeCount = brIn.ReadByte();

                    for(int i=0; i<SocialInfo.Instance.u1NoticeCount; i++)
                    {
                        SocialNotice _notice = new SocialNotice();
                        _notice.u2NoticeSN = brIn.ReadUInt16();
                        _notice.strTitle = brIn.ReadString();
                        _notice.u8ShowDay = brIn.ReadInt64();
                        _notice.dtShowDay = DateTime.FromBinary(_notice.u8ShowDay);
                        _notice.strContent = brIn.ReadString();
                        _notice.strImageURL = brIn.ReadString();
                        _notice.u1ShowTime = brIn.ReadByte();
                        _notice.strLinkURL = brIn.ReadString();
                        SocialInfo.Instance.dicNotice.Add((UInt16)i, _notice);
                    }

                    SocialInfo.Instance.u1EventPopupCount = brIn.ReadByte();
                    for(int i=0; i<SocialInfo.Instance.u1EventPopupCount; i++)
                    {
                        SocialNoticeEvent _noticeEvent = new SocialNoticeEvent();
                        _noticeEvent.u2EventPopupSN = brIn.ReadUInt16();
						//_noticeEvent.strListImageURL = brIn.ReadString(); // 서버 프로토콜 변경시 삭제
                        _noticeEvent.strImageURL = brIn.ReadString();
                        _noticeEvent.u1ShowTime = brIn.ReadByte();
						//_noticeEvent.strLinkURL = brIn.ReadString(); // 서버 프로토콜 변경시 삭제
                        _noticeEvent.u1SizeType = brIn.ReadByte();

						// 2016. 12. 22 jy
						// 서버 프로토콜 변경시 추가 로직
						_noticeEvent.u1PopupNo = brIn.ReadByte();
						_noticeEvent.u1ButtonCount = brIn.ReadByte();
						_noticeEvent.arrsNoticeEventInfo = new NoticeEventInfo[_noticeEvent.u1ButtonCount];
						for(int j = 0; j < _noticeEvent.u1ButtonCount; ++j)
						{
                            _noticeEvent.arrsNoticeEventInfo[j].u2EventID = brIn.ReadUInt16();
                            _noticeEvent.arrsNoticeEventInfo[j].strLinkURL = brIn.ReadString();
						}
                        SocialInfo.Instance.lstNoticeEventList.Add(_noticeEvent);
                    }
                }
            }

            callBack(err);
			return err;
        }

        public static ERROR_ID RequestCoupon(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}
				
				if(err == ERROR_ID.NONE)
				{
				}
			}
			
			callBack(err);
			return err;
		}
    }

    class RankHandler
    {
        public static ERROR_ID GetRankInfo(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
                    RankInfoMgr.Instance.u1RankListCount = brIn.ReadByte();

                    for(int i=0; i<RankInfoMgr.Instance.u1RankListCount; i++)
                    {
                        RankListInfo _rankListInfo = new RankListInfo();

                        _rankListInfo.u1RankType = brIn.ReadByte();
                        _rankListInfo.strLegionName = brIn.ReadString();
                        _rankListInfo.u2CharClassID = new UInt16[Crew.MAX_CHAR_IN_CREW];
                        if(_rankListInfo.u1RankType == 1)
                        {
                            _rankListInfo.u2CharClassID[0] = brIn.ReadUInt16();
                            _rankListInfo.strCharName = brIn.ReadString();
                        }

                        if(_rankListInfo.u1RankType == 2)
                        {
                            _rankListInfo.u1CrewIndex = brIn.ReadByte();
                            _rankListInfo.u2CharClassID[0] = brIn.ReadUInt16();
                            _rankListInfo.u2CharClassID[1] = brIn.ReadUInt16();
                            _rankListInfo.u2CharClassID[2] = brIn.ReadUInt16();
                        }
                        _rankListInfo.u4Value = brIn.ReadUInt32();
                        if(_rankListInfo.u1RankType == 5)
                            _rankListInfo.u4MakingCount = brIn.ReadUInt32();
                        if(_rankListInfo.u1RankType == 6 || _rankListInfo.u1RankType == 7 || _rankListInfo.u1RankType == 8)
                        {
                            _rankListInfo.u1Difficulty = brIn.ReadByte();
                            _rankListInfo.u8ClearTime = brIn.ReadInt64();
                            _rankListInfo.dtClearTime = DateTime.FromBinary(_rankListInfo.u8ClearTime);
                        }
                        _rankListInfo.u8MyRank = brIn.ReadUInt64();

                        RankInfoMgr.Instance.dicRankListData.Add((UInt16)(_rankListInfo.u1RankType-1), _rankListInfo);
                    }
                }
            }

            callBack(err);
			return err;
        }

        public static ERROR_ID GetRankList(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
                    RankInfoMgr.Instance.u1RankListDetailCount = brIn.ReadByte();

                    for(int i=0; i<RankInfoMgr.Instance.u1RankListDetailCount; i++)
                    {
                        RankListDetail _rankListDetailInfo = new RankListDetail();
                        _rankListDetailInfo.u1RankType = RankInfoMgr.Instance.u1RankType;
                        _rankListDetailInfo.u4Rank= brIn.ReadUInt32();
                        _rankListDetailInfo.strLegionName = brIn.ReadString();
                        _rankListDetailInfo.u2CharClassID = new UInt16[Crew.MAX_CHAR_IN_CREW];

                        if(_rankListDetailInfo.u1RankType == 1)
                        {
                            _rankListDetailInfo.u2CharClassID[0] = brIn.ReadUInt16();
                            _rankListDetailInfo.strCharName = brIn.ReadString();
                        }

                        if(_rankListDetailInfo.u1RankType == 2)
                        {
                            _rankListDetailInfo.u1CrewIndex = brIn.ReadByte();
                            _rankListDetailInfo.u2CharClassID[0] = brIn.ReadUInt16();
                            _rankListDetailInfo.u2CharClassID[1] = brIn.ReadUInt16();
                            _rankListDetailInfo.u2CharClassID[2] = brIn.ReadUInt16();
                        }
                        _rankListDetailInfo.u4Value = brIn.ReadUInt32();
                        if(_rankListDetailInfo.u1RankType == 5)
                            _rankListDetailInfo.u4MakingCount = brIn.ReadUInt32();

                        if(_rankListDetailInfo.u1RankType == 6 || _rankListDetailInfo.u1RankType == 7 || _rankListDetailInfo.u1RankType == 8)
                        {
                            _rankListDetailInfo.u1Difficulty = brIn.ReadByte();
                            _rankListDetailInfo.u8ClearTime = brIn.ReadInt64();
                            _rankListDetailInfo.dtClearTime = DateTime.FromBinary(_rankListDetailInfo.u8ClearTime);
                        }

                        RankInfoMgr.Instance.dicRankListDetailData.Add((UInt16)i, _rankListDetailInfo);
                    }
                    RankListDetail _rankListDetail = new RankListDetail();
                    _rankListDetail.u1RankType = RankInfoMgr.Instance.u1RankType;
                    if(RankInfoMgr.Instance.u1RankType == 2)
                    {
                        RankInfoMgr.Instance.u1MyCount = brIn.ReadByte();
                        RankInfoMgr.Instance.dicMyCrewRankData.Clear();
                        for(int i=0; i<RankInfoMgr.Instance.u1MyCount; i++)
                        {
                            MyCrewRank _myCrewRank = new MyCrewRank();
                            _myCrewRank.u1CrewIndex = brIn.ReadByte();
                            _myCrewRank.u8MyRank = brIn.ReadUInt64();
                            _myCrewRank.u4MyPower = brIn.ReadUInt32();

                            RankInfoMgr.Instance.dicMyCrewRankData.Add((UInt16)i, _myCrewRank);
                        }
                    }
                    else
                    {
                        _rankListDetail.u8MyRank = brIn.ReadUInt64();
                        if(_rankListDetail.u8MyRank != 0)
                        {
                            _rankListDetail.u4MyValue = brIn.ReadUInt32();
                            if(_rankListDetail.u1RankType == 5)
                                _rankListDetail.u4MyMakingCount = brIn.ReadUInt32();
                            if(_rankListDetail.u1RankType == 6 || _rankListDetail.u1RankType == 7 || _rankListDetail.u1RankType == 8)
                            {
                                _rankListDetail.u1Difficulty = brIn.ReadByte();
                                _rankListDetail.u8MyClearTime = brIn.ReadInt64();
                                _rankListDetail.dtMyClearTime = DateTime.FromBinary(_rankListDetail.u8MyClearTime);
                            }
                        }
                    }
                    RankInfoMgr.Instance.dicRankListDetailData.Add((UInt16)RankInfoMgr.Instance.dicRankListDetailData.Count, _rankListDetail);
                }
            }

            callBack(err);
			return err;
        }

        public static ERROR_ID GetRankReward(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
                    RankInfoMgr.Instance.u1RankRewardCount = brIn.ReadByte();
                    RankInfoMgr.Instance.dicRankRewardData.Clear();
                    for(int i=0; i<RankInfoMgr.Instance.u1RankRewardCount; i++)
                    {
                        RankReward _rankReward = new RankReward();
                        _rankReward.u1RankType = brIn.ReadByte();
                        _rankReward.u4Rank = brIn.ReadUInt32();
                        _rankReward.u1RewardIndex = brIn.ReadByte();

                        RankInfoMgr.Instance.dicRankRewardData.Add((UInt16)i, _rankReward);
                    }
                    Legion.Instance.u1RankRewad = 0;
                }
            }

            callBack(err);
			return err;
        }
    }

    class EventHandler
    {
        public static ERROR_ID RecvEventGoodsBuy(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
                    EventInfoMgr.Instance.u1EventGoodsItemCount = brIn.ReadByte();
                    for(int i=0; i<EventInfoMgr.Instance.u1EventGoodsItemCount; i++)
                    {
                        EventGoodsBuy _eventGoodsBuy = new EventGoodsBuy();
                        _eventGoodsBuy.u1ItemType = brIn.ReadByte();
                        _eventGoodsBuy.u2ItemID = brIn.ReadUInt16();
                        _eventGoodsBuy.u4Count = brIn.ReadUInt32();
                        if(_eventGoodsBuy.u1ItemType == (Byte)GoodsType.EQUIP)
                        {
                            _eventGoodsBuy.u1SmithingLevel = brIn.ReadByte();
                            _eventGoodsBuy.u2ModelID = brIn.ReadUInt16();
                            _eventGoodsBuy.u2Level = brIn.ReadUInt16();
                            _eventGoodsBuy.u1Completeness = brIn.ReadByte();

							_eventGoodsBuy.u4Stat = new UInt32[Server.ConstDef.SkillOfEquip + Server.ConstDef.EquipStatPointType * 2];
                            for(int j=0; j<Server.ConstDef.SkillOfEquip; j++)
								_eventGoodsBuy.u4Stat[j] = brIn.ReadUInt32();//brIn.ReadUInt16();
                            _eventGoodsBuy.u1SkillCount = brIn.ReadByte();
                            _eventGoodsBuy.u1SkillSlot = new Byte[Server.ConstDef.SkillOfEquip];
                            for(int j=0; j<_eventGoodsBuy.u1SkillCount; j++)
                                _eventGoodsBuy.u1SkillSlot[j] = brIn.ReadByte();
                        }

                        EventInfoMgr.Instance.lstEventGoodsBuy.Add(_eventGoodsBuy);
                    }
                }
            }

            callBack(err);
			return err;
        }

        public static ERROR_ID RecvEventGoodsReward(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
                    EventInfoMgr.Instance.sEventGoodsReward.u1LastRewardIndex = brIn.ReadByte();
                    EventInfoMgr.Instance.sEventGoodsReward.u4RecordValue = brIn.ReadUInt32();

                    for (int i=0; i<EventInfoMgr.Instance.lstTimeEventID.Count; i++)
                    {
                        if(Convert.ToUInt16(obj1) == EventInfoMgr.Instance.lstTimeEventID[i])
                        {
                            EventInfoMgr.Instance.dicEventReward.Remove(EventInfoMgr.Instance.lstTimeEventID[i]);
                            EventReward _eventReward = new EventReward();
                            _eventReward.u2EventID = EventInfoMgr.Instance.lstTimeEventID[i];
                            _eventReward.u1RewardIndex = EventInfoMgr.Instance.sEventGoodsReward.u1LastRewardIndex;
                            _eventReward.u4RecordValue = EventInfoMgr.Instance.sEventGoodsReward.u4RecordValue;
                            EventInfoMgr.Instance.dicEventReward.Add(EventInfoMgr.Instance.lstTimeEventID[i], _eventReward);
                            break;
                        }
                    }
                }
            }

            callBack(err);
			return err;
        }

		public static ERROR_ID UseEventItem(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
					
				}
			}

			callBack(err);
			return err;
		}


		public static ERROR_ID RecvAdReward(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
					Byte Remain = brIn.ReadByte();
					UInt16 LeftTime = brIn.ReadUInt16();

					Byte pos = Convert.ToByte (obj1);

					Legion.Instance.adRemainCount [pos - 1] = Remain;
					Legion.Instance.adLeftTime [pos - 1] = LeftTime;

					Legion.Instance.AddGoods (EventInfoMgr.Instance.dicAdReward [pos].cReward);

					PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_ad") , EventInfoMgr.Instance.dicAdReward [pos].cReward.GetGoodsString()+"\n"+ TextManager.Instance.GetText("mark_event_get_goods"), null);
				}
			}

			callBack(err);
			return err;
		}

        public static ERROR_ID EventReload(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
		{
			if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
					//EventInfoMgr.Instance.dicEventReward.Clear();
                    
                    List<UInt16> lstKeys = new List<UInt16>();
                    foreach (KeyValuePair<UInt16, EventReward> item in EventInfoMgr.Instance.dicEventReward)
                    {
                        if(item.Value.eventType == (Byte)EVENT_TYPE.DISCOUNT)
                        {
                            lstKeys.Add(item.Key);
                        }
                        else if(item.Value.eventType == (Byte)EVENT_TYPE.FIRSTPAYMENT)
                        {
                            lstKeys.Add(item.Key);
                        }
                        else if(item.Value.eventType == (Byte)EVENT_TYPE.ADDITIONALREWARD)
                        {
                            lstKeys.Add(item.Key);
                        }
                        else if(item.Value.eventType == (Byte)EVENT_TYPE.BUFF_EXP)
                        {
                            EventInfoMgr.Instance.u4ExpBoostPer = 0;
                            lstKeys.Add(item.Key);
                        }
                        else if(item.Value.eventType == (Byte)EVENT_TYPE.BUFF_GOLD)
                        {
                            EventInfoMgr.Instance.u4GoldBoostPer = 0;
                            lstKeys.Add(item.Key);
                        }
                    }

                    EventInfoMgr.Instance.lstGoldBuffEvent.Clear();
                    EventInfoMgr.Instance.lstExpBuffEvent.Clear();

                    EventInfoMgr.Instance.u1EventCount -= (Byte)lstKeys.Count;
                    for(int i=0; i<lstKeys.Count; i++)
                    {
                            EventInfoMgr.Instance.dicEventReward.Remove(lstKeys[i]);
                    }
                    
                    Byte tempCnt = brIn.ReadByte();
                    for(int i=0; i<tempCnt; i++)
                    {
                        EventReward tempReward = new EventReward();
                        tempReward.u2EventID = brIn.ReadUInt16();
                        tempReward.u1EventType = brIn.ReadByte();
                        tempReward.u1RewardIndex = brIn.ReadByte();
                        tempReward.u4RecordValue = brIn.ReadUInt32();
                        tempReward.u8EventBegin = brIn.ReadInt64();
                        if(tempReward.u8EventBegin != 0)
                            tempReward.dtEventBegin = DateTime.FromBinary(tempReward.u8EventBegin);
                        tempReward.u8EventEnd = brIn.ReadInt64();
                        if(tempReward.u8EventEnd != 0)
                            tempReward.dtEventEnd = DateTime.FromBinary(tempReward.u8EventEnd);
                        if(tempReward.eventType == (Byte)EVENT_TYPE.BUFF_EXP)
                        {
                            EventInfoMgr.Instance.u4ExpBoostPer = tempReward.recordValue;
                            EventInfoMgr.Instance.lstExpBuffEvent.Add(tempReward);
                        }
                        if(tempReward.eventType == (Byte)EVENT_TYPE.BUFF_GOLD)
                        {
                            EventInfoMgr.Instance.u4GoldBoostPer = tempReward.recordValue;
                            EventInfoMgr.Instance.lstGoldBuffEvent.Add(tempReward);
                        }
                    
                        EventInfoMgr.Instance.dicEventReward.Add(tempReward.u2EventID, tempReward);
                    }
                    EventInfoMgr.Instance.u1EventCount += tempCnt;
                    StageInfoMgr.Instance.bReloadEvent = true;
				}
			}

			callBack(err);
			return err;
		}

        public static ERROR_ID EventOxQuizz(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
                    Legion.Instance.cEvent.u1TodayOxDone = brIn.ReadByte();
                    if(Legion.Instance.cEvent.u1TodayOxDone == 1)
                    {
                        Legion.Instance.cEvent.u2OXlefttime = 9998;
                        if(Legion.Instance.cEvent.u1OXanswer < EventOxReward.MAX_DAY)
                        {
                            Legion.Instance.cEvent.u1OXanswer++;
                            //Legion.Instance.cEvent.u1OXquestion++;
                        }
                        EventInfoMgr.Instance.StopCoroutine("OxTimer");
                        Legion.Instance.AddLoginPopupStep(Legion.LoginPopupStep.OX_EVENT);
                    }
                    else
                    {
                        Legion.Instance.cEvent.u2OXlefttime = (UInt16)(EventInfoMgr.Instance.GetOXQuestion(Legion.Instance.cEvent.u1OXquestion).u1Retry * 60);
                        EventInfoMgr.Instance.StartCoroutine("OxTimer");
                    }
                }
            }

            callBack(err);
			return err;
        }
    }

    class GuildHandler
    {
        public static ERROR_ID GuildCreate(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
					GuildInfoMgr.Instance.u8GuildSN = brIn.ReadUInt64();
                }
            }

            callBack(err);
			return err;
        }

        public static ERROR_ID GuildInfo(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
					GuildInfoMgr.Instance.bGuildMaster = false;

                    GuildInfoMgr.Instance.cGuildMemberInfo.strGuildName = brIn.ReadString();
                    GuildInfoMgr.Instance.cGuildMemberInfo.u1Public = brIn.ReadByte();
                    GuildInfoMgr.Instance.cGuildMemberInfo.u8GuildPower = brIn.ReadUInt64();
                    GuildInfoMgr.Instance.cGuildMemberInfo.u8Rank = brIn.ReadUInt64();
                    GuildInfoMgr.Instance.cGuildMemberInfo.u2Score = brIn.ReadUInt16();
                    GuildInfoMgr.Instance.cGuildMemberInfo.u8LastRank = brIn.ReadUInt64();
                    GuildInfoMgr.Instance.cGuildMemberInfo.u2LastScore = brIn.ReadUInt16();
                    GuildInfoMgr.Instance.cGuildMemberInfo.u1DailyCheckCount = brIn.ReadByte();
                    GuildInfoMgr.Instance.cGuildMemberInfo.u1LastDailyCheckCount = brIn.ReadByte();
                    GuildInfoMgr.Instance.cGuildMemberInfo.u1GuildKey = brIn.ReadByte();
                    GuildInfoMgr.Instance.GuildKey = GuildInfoMgr.Instance.cGuildMemberInfo.u1GuildKey;
                    GuildInfoMgr.Instance.cGuildMemberInfo.u2GuildKeyLeftTime = brIn.ReadUInt16();
                    GuildInfoMgr.Instance.dtGuildKeyChargeTime = Legion.Instance.ServerTime.Add(TimeSpan.FromSeconds(GuildInfoMgr.Instance.cGuildMemberInfo.u2GuildKeyLeftTime));
                    GuildInfoMgr.Instance.cGuildMemberInfo.u1RewardFlag = brIn.ReadByte();
                    GuildInfoMgr.Instance.cGuildMemberInfo.dtRewardBeginTime = DateTime.FromBinary(brIn.ReadInt64());					GuildInfoMgr.Instance.cGuildMemberInfo.dtRewardEndTime = DateTime.FromBinary(brIn.ReadInt64());                    GuildInfoMgr.Instance.cGuildMemberInfo.u1MemberCount = brIn.ReadByte();
                    GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember = new List<GuildMember>();
                    for(int i=0; i<GuildInfoMgr.Instance.cGuildMemberInfo.u1MemberCount; i++)
                    {
                        GuildMember tempMember = new GuildMember();
						tempMember.bMember = true;
                        tempMember.u8UserSN = brIn.ReadUInt64();
                        tempMember.strLegionName = brIn.ReadString();
                        tempMember.u8Power = brIn.ReadUInt64();
                        if(tempMember.strLegionName == Legion.Instance.sName)
                            tempMember.u8Power = Legion.Instance.acCrews[GuildInfoMgr.Instance.u1GuildCrewIndex-1].u4Power;
                        tempMember.u2ClassID = new UInt16[Crew.MAX_CHAR_IN_CREW];
                        tempMember.u2Level = new UInt16[Crew.MAX_CHAR_IN_CREW];
                        tempMember.u1Element = new Byte[Crew.MAX_CHAR_IN_CREW];
                        for(int j=0; j<Crew.MAX_CHAR_IN_CREW; j++)
                        {
                            tempMember.u2ClassID[j] = brIn.ReadUInt16();
                            tempMember.u2Level[j] = brIn.ReadUInt16();
                            tempMember.u1Element[j] = brIn.ReadByte();
                        }
                        tempMember.u2LeagueCount = brIn.ReadUInt16();
                        tempMember.u1Option = brIn.ReadByte();
                        tempMember.u8LastLogin = brIn.ReadInt64();
                        if(tempMember.strLegionName == Legion.Instance.sName)
                        {
                            if((tempMember.u1Option & 0x10) != 0)
                            {
                                GuildInfoMgr.Instance.bGuildMaster = true;
                            }
                        }
						tempMember.dtJoinDate = DateTime.FromBinary((Int64)brIn.ReadUInt64());
                        if(tempMember.strLegionName == Legion.Instance.sName)
                            GuildInfoMgr.Instance.MyGuildInfo = tempMember;
                        GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember.Add(tempMember);
                    }
                    GuildInfoMgr.Instance.cGuildMemberInfo.u8GuildSN = brIn.ReadUInt64();
                    GuildInfoMgr.Instance.u8GuildSN = GuildInfoMgr.Instance.cGuildMemberInfo.u8GuildSN;
                }
            }

            callBack(err);
			return err;
        }

        public static ERROR_ID GuildMark(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
					
                }
            }

            callBack(err);
			return err;
        }

        public static ERROR_ID GuildSetMember(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
					if ((byte)obj1 == 1) {
						GuildMember tempMember = GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember.Find (cs => cs.u8UserSN == (ulong)obj2);
						tempMember.bMember = true;
						tempMember.u8UserSN = brIn.ReadUInt64 ();
						tempMember.strLegionName = brIn.ReadString ();
						tempMember.u8Power = brIn.ReadUInt64 ();
						tempMember.u2ClassID = new UInt16[Crew.MAX_CHAR_IN_CREW];
						tempMember.u2Level = new UInt16[Crew.MAX_CHAR_IN_CREW];
						tempMember.u1Element = new Byte[Crew.MAX_CHAR_IN_CREW];
						for (int j = 0; j < Crew.MAX_CHAR_IN_CREW; j++) {
							tempMember.u2ClassID [j] = brIn.ReadUInt16 ();
							tempMember.u2Level [j] = brIn.ReadUInt16 ();
							tempMember.u1Element [j] = brIn.ReadByte ();
						}
						tempMember.u2LeagueCount = brIn.ReadUInt16 ();
						tempMember.u1Option = brIn.ReadByte ();
						tempMember.u8LastLogin = brIn.ReadInt64 ();
						//tempMember.dtJoinDate = DateTime.FromBinary((Int64)brIn.ReadUInt64());
					} else if ((byte)obj1 == 5) {
						GuildInfoMgr.Instance.cGuildDetailCrew = new Crew ();

						GuildMatchCrew tempCrew = new GuildMatchCrew ();
						tempCrew.u8UserSN = (ulong)obj2;
						tempCrew.u1CharCount = brIn.ReadByte();
						tempCrew.lstMatchCrew = new List<GuildMatchCrewChar> ();

						for(int i=0; i<tempCrew.u1CharCount; i++)
						{
							GuildMatchCrewChar guildCharInfo = new GuildMatchCrewChar();

							guildCharInfo.u1CharIndex = brIn.ReadByte();
							guildCharInfo.strCharName = brIn.ReadString();
							guildCharInfo.u2ClassID = brIn.ReadUInt16();
							guildCharInfo.u2Level = brIn.ReadUInt16();
							guildCharInfo.u1CrewPos = brIn.ReadByte();
							guildCharInfo.u1Shape = new Byte[Server.ConstDef.LengthOfShape];
							for(int j=0; j<Server.ConstDef.LengthOfShape; j++)
								guildCharInfo.u1Shape[j] = brIn.ReadByte();

							guildCharInfo.u2Stats = new ushort[7];
							
							guildCharInfo.u2Stats[0] = brIn.ReadUInt16();
							guildCharInfo.u2Stats[1] = brIn.ReadUInt16();
							guildCharInfo.u2Stats[2] = brIn.ReadUInt16();
							guildCharInfo.u2Stats[3] = brIn.ReadUInt16();
							guildCharInfo.u2Stats[4] = brIn.ReadUInt16();
							guildCharInfo.u2Stats[5] = brIn.ReadUInt16();
							guildCharInfo.u2Stats[6] = brIn.ReadUInt16();

							guildCharInfo.lstCrewEquipment = new List<GuildMatchCrewEquipment>();

							for(int j=0; j<Hero.MAX_EQUIP_OF_CHAR; j++)
							{
								GuildMatchCrewEquipment guildCharEquipInfo = new GuildMatchCrewEquipment();

								guildCharEquipInfo.u2EquipmentSlot = brIn.ReadUInt16();
								guildCharEquipInfo.u2ItemID = brIn.ReadUInt16();
								guildCharEquipInfo.strItemName = brIn.ReadString();
								guildCharEquipInfo.u1SmithingLevel = brIn.ReadByte();
								guildCharEquipInfo.u2ModelId = brIn.ReadUInt16();
								guildCharEquipInfo.u2Level = brIn.ReadUInt16();
								guildCharEquipInfo.u1Completeness = brIn.ReadByte();
								guildCharEquipInfo.u4Stats = new UInt32[ConstDef.SkillOfEquip + ConstDef.EquipStatPointType * 2];
								guildCharEquipInfo.u1SkillSlot = new Byte[Server.ConstDef.SkillOfEquip];
								for(int k=0; k<Server.ConstDef.EquipStatPointType; k++)
								{
									guildCharEquipInfo.u4Stats[k + ConstDef.SkillOfEquip] = brIn.ReadUInt32();
								}
								for(int k=0; k<Server.ConstDef.SkillOfEquip; k++)
								{
									guildCharEquipInfo.u1SkillSlot[k] = brIn.ReadByte();
									guildCharEquipInfo.u4Stats[k] = brIn.ReadUInt16();
								}
								for(int k=0; k<Server.ConstDef.EquipStatPointType; k++)
								{
									guildCharEquipInfo.u4Stats[k + ConstDef.SkillOfEquip + ConstDef.EquipStatPointType] = brIn.ReadUInt16();
								}

								guildCharInfo.lstCrewEquipment.Add(guildCharEquipInfo);
							}

							guildCharInfo.u2SkillCount = brIn.ReadUInt16();
							guildCharInfo.lstCrewSkill = new List<GuildMatchCrewSkill>();
							for(int j=0; j<guildCharInfo.u2SkillCount; j++)
							{
								GuildMatchCrewSkill guildCharSkillInfo = new GuildMatchCrewSkill();

								guildCharSkillInfo.u1SkillSlot = brIn.ReadByte();
								guildCharSkillInfo.u2Level = brIn.ReadUInt16();
								guildCharSkillInfo.u1SelectSlot = brIn.ReadByte();

								guildCharInfo.lstCrewSkill.Add(guildCharSkillInfo);
							}

							tempCrew.lstMatchCrew.Add(guildCharInfo);

							GuildInfoMgr.Instance.SetCrewData (guildCharInfo, 0, true, true);
						}

						GuildInfoMgr.Instance.cGuildDetailData = tempCrew;
					}
                }
            }

            callBack(err);
			return err;
        }

        public static ERROR_ID GuildSetCrew(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
                }
            }

            callBack(err);
			return err;
        }

        public static ERROR_ID GuildSearch(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
					GuildInfoMgr.Instance.dicGuildList = new Dictionary<byte, GuildList> ();
                    
                    GuildInfoMgr.Instance.u1GuildListCount = brIn.ReadByte();
                    for(int i=0; i<GuildInfoMgr.Instance.u1GuildListCount; i++)
                    {
						GuildList tempList = new GuildList();
                        tempList.u8GuildSN = brIn.ReadUInt64();
                        tempList.strGuildName = brIn.ReadString();
                        tempList.u1MemberCount = brIn.ReadByte();
                        tempList.u2Score = brIn.ReadUInt16();
						tempList.u1Public = brIn.ReadByte();
                        tempList.u1Request = brIn.ReadByte();

                        GuildInfoMgr.Instance.dicGuildList.Add((Byte)i, tempList);
                    }
                }
            }

            callBack(err);
			return err;
        }

        public static ERROR_ID GuildRequestJoinList(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
                    GuildInfoMgr.Instance.u1JoinUserCount = brIn.ReadByte();
                    for(int i=0; i<GuildInfoMgr.Instance.u1JoinUserCount; i++)
                    {
//                        GuildJoinList tempList = new GuildJoinList();
//                        tempList.u8UserSN = brIn.ReadUInt64();
//						tempList.strLegionName = brIn.ReadString();
//						tempList.u8Power = brIn.ReadUInt64();
//
//						GuildInfoMgr.Instance.lstGuildJoinList.Add(tempList);

						GuildMember tempMember = new GuildMember();
						tempMember.bMember = false;
						tempMember.u8UserSN = brIn.ReadUInt64();
						tempMember.strLegionName = brIn.ReadString();
						tempMember.u8Power = brIn.ReadUInt64();
						tempMember.u2ClassID = new UInt16[Crew.MAX_CHAR_IN_CREW];
						tempMember.u2Level = new UInt16[Crew.MAX_CHAR_IN_CREW];
						tempMember.u1Element = new Byte[Crew.MAX_CHAR_IN_CREW];

						int alreadyIdx = GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember.FindIndex (cs => cs.u8UserSN == tempMember.u8UserSN && cs.bMember == true);

						if (alreadyIdx > -1) {
							GuildInfoMgr.Instance.cGuildMemberInfo.u1MemberCount--;
							GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember [alreadyIdx].bMember = false;
							byte decIndex = GuildInfoMgr.Instance.dicUserEntry.FirstOrDefault (cs => cs.Value.u8UserSN == tempMember.u8UserSN).Key;
							if (decIndex > 0) {
								GuildInfoMgr.Instance.dicUserEntry.Remove (decIndex);
							}

							byte mdecIndex = GuildInfoMgr.Instance.dicMasterEntry.FirstOrDefault (cs => cs.Value.u8UserSN == tempMember.u8UserSN).Key;
							if (mdecIndex > 0) {
								GuildInfoMgr.Instance.dicMasterEntry.Remove (mdecIndex);
							}
						} else {
							if(GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember.FindIndex(cs => cs.u8UserSN == tempMember.u8UserSN) < 0)
								GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember.Add (tempMember);
						}
                    }
                }
            }

            callBack(err);
			return err;
        }

        public static ERROR_ID GuildRequestJoin(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
                    GuildInfoMgr.Instance.u8GuildSN = brIn.ReadUInt64();
                }
            }

            callBack(err);
			return err;
        }

        public static ERROR_ID GuildMatchInfo(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
					GuildInfoMgr.Instance.lstGuildMatchList = new List<GuildMatchList> ();
                    GuildInfoMgr.Instance.u1GuildMatchListCount = brIn.ReadByte();

                    for(int i=0; i<GuildInfoMgr.Instance.u1GuildMatchListCount; i++)
                    {
                        GuildMatchList tempMatchList = new GuildMatchList();
                        tempMatchList.u8GuildSN = brIn.ReadUInt64();
                        tempMatchList.strGuildName = brIn.ReadString();
                        tempMatchList.u8Power = brIn.ReadUInt64();
                        tempMatchList.u2Score = brIn.ReadUInt16();

                        GuildInfoMgr.Instance.lstGuildMatchList.Add(tempMatchList);
                    }
                }
            }

            callBack(err);
			return err;
        }

        public static ERROR_ID GuildMatch(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
                    GuildInfoMgr.Instance.cGuildMatchData = new GuildMatchData();
                    GuildInfoMgr.Instance.cGuildMatchData.u1UserDeckCnt = brIn.ReadByte();
                    GuildInfoMgr.Instance.cGuildMatchData.lstGuildCrew = new List<GuildMatchCrew>();
                    for(int i=0; i<GuildInfoMgr.Instance.cGuildMatchData.u1UserDeckCnt; i++)
                    {
                        GuildMatchCrew tempCrew = new GuildMatchCrew();
                        tempCrew.u8UserSN = brIn.ReadUInt64();
                        tempCrew.u1CrewIndex = brIn.ReadByte();
                        tempCrew.u1CharCount = brIn.ReadByte();
                        tempCrew.lstMatchCrew = new List<GuildMatchCrewChar>();
                        for(int j=0; j<tempCrew.u1CharCount; j++)
                        {
                            GuildMatchCrewChar tempChar = new GuildMatchCrewChar();
                            tempChar.u1CharIndex = brIn.ReadByte();
                            tempChar.strCharName = brIn.ReadString();
                            tempChar.u2ClassID = brIn.ReadUInt16();
                            tempChar.u2Level = brIn.ReadUInt16();
                            tempChar.u1CrewPos = brIn.ReadByte();
                            tempChar.u1Shape = new Byte[ConstDef.LengthOfShape];
                            for(int k=0; k<ConstDef.LengthOfShape; k++)
                                tempChar.u1Shape[k] = brIn.ReadByte();
                            tempChar.u2Stats = new UInt16[ConstDef.CharStatPointType];
                            for(int k=0; k<ConstDef.CharStatPointType; k++)
                                tempChar.u2Stats[k] = brIn.ReadUInt16();
                            tempChar.lstCrewEquipment = new List<GuildMatchCrewEquipment>();
                            for(int k=0; k<ConstDef.MaxItemSlot; k++)
                            {
                                GuildMatchCrewEquipment tempEquip = new GuildMatchCrewEquipment();
                                tempEquip.u2EquipmentSlot = brIn.ReadUInt16();
                                tempEquip.u2ItemID = brIn.ReadUInt16();
                                tempEquip.strItemName = brIn.ReadString();
                                tempEquip.u1SmithingLevel = brIn.ReadByte();
                                tempEquip.u2ModelId = brIn.ReadUInt16();
                                tempEquip.u2Level = brIn.ReadUInt16();
                                tempEquip.u1Completeness = brIn.ReadByte();
                                tempEquip.u4Stats = new UInt32[ConstDef.SkillOfEquip + ConstDef.EquipStatPointType * 2];
                                for(int l=0; l<ConstDef.EquipStatPointType; l++)
                                    tempEquip.u4Stats[l + ConstDef.SkillOfEquip] = brIn.ReadUInt32();
                                tempEquip.u1SkillSlot = new Byte[ConstDef.SkillOfEquip];
                                tempEquip.u2SkillPoint = new UInt16[ConstDef.SkillOfEquip];
                                tempEquip.u2StatPoint = new UInt16[ConstDef.SkillOfEquip];
                                for(int l=0; l<ConstDef.EquipStatPointType; l++)
                                {
                                    tempEquip.u1SkillSlot[l] = brIn.ReadByte();
                                    tempEquip.u4Stats[l] = brIn.ReadUInt16();
                                }
                                for(int l=0; l<ConstDef.EquipStatPointType; l++)
                                    tempEquip.u4Stats[l + ConstDef.SkillOfEquip + ConstDef.EquipStatPointType] = brIn.ReadUInt16();

                                tempChar.lstCrewEquipment.Add(tempEquip);
                            }
                            tempChar.u2SkillCount = brIn.ReadUInt16();
                            tempChar.lstCrewSkill = new List<GuildMatchCrewSkill>();
                            for(int k=0; k<tempChar.u2SkillCount; k++)
                            {
                                GuildMatchCrewSkill tempSkill = new GuildMatchCrewSkill();
                                tempSkill.u1SkillSlot = brIn.ReadByte();
                                tempSkill.u2Level = brIn.ReadUInt16();
                                tempSkill.u1SelectSlot = brIn.ReadByte();

                                tempChar.lstCrewSkill.Add(tempSkill);
                            }
                            tempCrew.lstMatchCrew.Add(tempChar);
                        }
                        GuildInfoMgr.Instance.cGuildMatchData.lstGuildCrew.Add(tempCrew);
                    }

                    GuildInfoMgr.Instance.cGuildMatchData.strMatchingGuildName = brIn.ReadString();
                    GuildInfoMgr.Instance.cGuildMatchData.u8MatchingGuildPower = brIn.ReadUInt64();
                    GuildInfoMgr.Instance.cGuildMatchData.u1MatchingDeckCount = brIn.ReadByte();
                    GuildInfoMgr.Instance.cGuildMatchData.lstEnemyCrew = new List<GuildMatchCrew>();

                    for(int i=0; i<GuildInfoMgr.Instance.cGuildMatchData.u1MatchingDeckCount; i++)
                    {
                        GuildMatchCrew tempCrew = new GuildMatchCrew();
                        tempCrew.u8UserSN = brIn.ReadUInt64();
                        tempCrew.u1CrewIndex = brIn.ReadByte();
                        tempCrew.u1CharCount = brIn.ReadByte();
                        tempCrew.lstMatchCrew = new List<GuildMatchCrewChar>();
                        for(int j=0; j<tempCrew.u1CharCount; j++)
                        {
                            GuildMatchCrewChar tempChar = new GuildMatchCrewChar();
                            tempChar.u1CharIndex = brIn.ReadByte();
                            tempChar.strCharName = brIn.ReadString();
                            tempChar.u2ClassID = brIn.ReadUInt16();
                            tempChar.u2Level = brIn.ReadUInt16();
                            tempChar.u1CrewPos = brIn.ReadByte();
                            tempChar.u1Shape = new Byte[ConstDef.LengthOfShape];
                            for(int k=0; k<ConstDef.LengthOfShape; k++)
                                tempChar.u1Shape[k] = brIn.ReadByte();
                            tempChar.u2Stats = new UInt16[ConstDef.CharStatPointType];
                            for(int k=0; k<ConstDef.CharStatPointType; k++)
                                tempChar.u2Stats[k] = brIn.ReadUInt16();
                            tempChar.lstCrewEquipment = new List<GuildMatchCrewEquipment>();
                            for(int k=0; k<ConstDef.MaxItemSlot; k++)
                            {
                                GuildMatchCrewEquipment tempEquip = new GuildMatchCrewEquipment();
                                tempEquip.u2EquipmentSlot = brIn.ReadUInt16();
                                tempEquip.u2ItemID = brIn.ReadUInt16();
                                tempEquip.strItemName = brIn.ReadString();
                                tempEquip.u1SmithingLevel = brIn.ReadByte();
                                tempEquip.u2ModelId = brIn.ReadUInt16();
                                tempEquip.u2Level = brIn.ReadUInt16();
                                tempEquip.u1Completeness = brIn.ReadByte();
                                tempEquip.u4Stats = new UInt32[ConstDef.SkillOfEquip + ConstDef.EquipStatPointType * 2];
                                for(int l=0; l<ConstDef.EquipStatPointType; l++)
                                    tempEquip.u4Stats[l + ConstDef.SkillOfEquip] = brIn.ReadUInt32();
                                tempEquip.u1SkillSlot = new Byte[ConstDef.SkillOfEquip];
                                tempEquip.u2SkillPoint = new UInt16[ConstDef.SkillOfEquip];
                                tempEquip.u2StatPoint = new UInt16[ConstDef.SkillOfEquip];
                                for(int l=0; l<ConstDef.EquipStatPointType; l++)
                                {
                                    tempEquip.u1SkillSlot[l] = brIn.ReadByte();
                                    tempEquip.u4Stats[l] = brIn.ReadUInt16();
                                }
                                for(int l=0; l<ConstDef.EquipStatPointType; l++)
                                    tempEquip.u4Stats[l + ConstDef.SkillOfEquip + ConstDef.EquipStatPointType] = brIn.ReadUInt16();

                                tempChar.lstCrewEquipment.Add(tempEquip);
                            }
                            tempChar.u2SkillCount = brIn.ReadUInt16();
                            tempChar.lstCrewSkill = new List<GuildMatchCrewSkill>();
                            for(int k=0; k<tempChar.u2SkillCount; k++)
                            {
                                GuildMatchCrewSkill tempSkill = new GuildMatchCrewSkill();
                                tempSkill.u1SkillSlot = brIn.ReadByte();
                                tempSkill.u2Level = brIn.ReadUInt16();
                                tempSkill.u1SelectSlot = brIn.ReadByte();

                                tempChar.lstCrewSkill.Add(tempSkill);
                            }
                            tempCrew.lstMatchCrew.Add(tempChar);
                        }
                        GuildInfoMgr.Instance.cGuildMatchData.lstEnemyCrew.Add(tempCrew);
                    }

                    GuildInfoMgr.Instance.CreateMatchCrews();
                }
            }

            callBack(err);
			return err;
        }

        public static ERROR_ID GuildStartMatch(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
                    GuildInfoMgr.Instance.cGuildMemberInfo.u1GuildKey = brIn.ReadByte();
                    GuildInfoMgr.Instance.cGuildMemberInfo.u2GuildKeyLeftTime = brIn.ReadUInt16();
                }
            }

            callBack(err);
			return err;
        }

        public static ERROR_ID GuildMatchResult(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
                    Int16 u2DeltaPoint = brIn.ReadInt16();
					Legion.Instance.u2LastBattleResultPoint = u2DeltaPoint;
					GuildInfoMgr.Instance.cGuildMemberInfo.u2Score = (UInt16)(GuildInfoMgr.Instance.cGuildMemberInfo.u2Score + u2DeltaPoint);
                }
            }

            callBack(err);
			return err;
        }

        public static ERROR_ID GuildRankInfo(ERROR_ID err, OnResponse callBack, BinaryReader brIn, System.Object obj1, System.Object obj2)
        {
            if(ServerMgr.bConnectToServer)
			{
				if(err == ERROR_ID.REQUEST_DUPLICATION)
				{
					err = ERROR_ID.NONE;
				}

				if(err == ERROR_ID.NONE)
				{
                    if(((Byte)obj1) == (Byte)GuildInfoMgr.GUILD_RANK_TYPE.RankList)
                    {
                        GuildInfoMgr.Instance.u1GuildRankCount = brIn.ReadByte();
                        GuildInfoMgr.Instance.lstGuildRank = new List<GuildRankList>();
                        for(int i=0; i<GuildInfoMgr.Instance.u1GuildRankCount; i++)
                        {
                            GuildRankList tempList = new GuildRankList();
                            tempList.u8GuildSN = brIn.ReadUInt64();
                            tempList.strGuildName = brIn.ReadString();
                            tempList.u8Rank = brIn.ReadUInt64();
                            tempList.u2Score = brIn.ReadUInt16();
                            tempList.u8Power = brIn.ReadUInt64();

                            GuildInfoMgr.Instance.lstGuildRank.Add(tempList);
                        }
                    }
                    else
                    {
                        GuildInfoMgr.Instance.cGuildRankInfo = new GuildRankInfo();
                        GuildInfoMgr.Instance.cGuildRankInfo.strName = brIn.ReadString();
                        GuildInfoMgr.Instance.cGuildRankInfo.u8GuildPower = brIn.ReadUInt64();
                        GuildInfoMgr.Instance.cGuildRankInfo.u1DeckCount = brIn.ReadByte();
                        GuildInfoMgr.Instance.cGuildRankInfo.lstGuildCrew = new List<GuildMatchCrew>();

                        for(int i=0; i<GuildInfoMgr.Instance.cGuildRankInfo.u1DeckCount; i++)
                        {
                            GuildMatchCrew tempCrew = new GuildMatchCrew();
                            tempCrew.u8UserSN = brIn.ReadUInt64();
                            tempCrew.u1CrewIndex = brIn.ReadByte();
                            tempCrew.u1CharCount = brIn.ReadByte();
                            tempCrew.lstMatchCrew = new List<GuildMatchCrewChar>();
                            for(int j=0; j<tempCrew.u1CharCount; j++)
                            {
                                GuildMatchCrewChar tempChar = new GuildMatchCrewChar();
                                tempChar.u1CharIndex = brIn.ReadByte();
                                tempChar.strCharName = brIn.ReadString();
                                tempChar.u2ClassID = brIn.ReadUInt16();
                                tempChar.u2Level = brIn.ReadUInt16();
                                tempChar.u1CrewPos = brIn.ReadByte();
                                tempChar.u1Shape = new Byte[ConstDef.LengthOfShape];
                                for(int k=0; k<ConstDef.LengthOfShape; k++)
                                    tempChar.u1Shape[k] = brIn.ReadByte();
                                tempChar.u2Stats = new UInt16[ConstDef.CharStatPointType];
                                for(int k=0; k<ConstDef.CharStatPointType; k++)
                                    tempChar.u2Stats[k] = brIn.ReadUInt16();
                                tempChar.lstCrewEquipment = new List<GuildMatchCrewEquipment>();
                                for(int k=0; k<ConstDef.MaxItemSlot; k++)
                                {
                                    GuildMatchCrewEquipment tempEquip = new GuildMatchCrewEquipment();
                                    tempEquip.u2EquipmentSlot = brIn.ReadUInt16();
                                    tempEquip.u2ItemID = brIn.ReadUInt16();
                                    tempEquip.strItemName = brIn.ReadString();
                                    tempEquip.u1SmithingLevel = brIn.ReadByte();
                                    tempEquip.u2ModelId = brIn.ReadUInt16();
                                    tempEquip.u2Level = brIn.ReadUInt16();
                                    tempEquip.u1Completeness = brIn.ReadByte();
                                    tempEquip.u4Stats = new UInt32[ConstDef.SkillOfEquip + ConstDef.EquipStatPointType * 2];
                                    for(int l=0; l<ConstDef.EquipStatPointType; l++)
                                        tempEquip.u4Stats[l + ConstDef.SkillOfEquip] = brIn.ReadUInt32();
                                    tempEquip.u1SkillSlot = new Byte[ConstDef.SkillOfEquip];
                                    tempEquip.u2SkillPoint = new UInt16[ConstDef.SkillOfEquip];
                                    tempEquip.u2StatPoint = new UInt16[ConstDef.SkillOfEquip];
                                    for(int l=0; l<ConstDef.EquipStatPointType; l++)
                                    {
                                        tempEquip.u1SkillSlot[l] = brIn.ReadByte();
                                        tempEquip.u4Stats[l] = brIn.ReadUInt16();
                                    }
                                    for(int l=0; l<ConstDef.EquipStatPointType; l++)
                                        tempEquip.u4Stats[l + ConstDef.SkillOfEquip + ConstDef.EquipStatPointType] = brIn.ReadUInt16();

                                    tempChar.lstCrewEquipment.Add(tempEquip);
                                }
                                tempChar.u2SkillCount = brIn.ReadUInt16();
                                tempChar.lstCrewSkill = new List<GuildMatchCrewSkill>();
                                for(int k=0; k<tempChar.u2SkillCount; k++)
                                {
                                    GuildMatchCrewSkill tempSkill = new GuildMatchCrewSkill();
                                    tempSkill.u1SkillSlot = brIn.ReadByte();
                                    tempSkill.u2Level = brIn.ReadUInt16();
                                    tempSkill.u1SelectSlot = brIn.ReadByte();

                                    tempChar.lstCrewSkill.Add(tempSkill);
                                }
                                tempCrew.lstMatchCrew.Add(tempChar);
                            }
                            GuildInfoMgr.Instance.cGuildRankInfo.lstGuildCrew.Add(tempCrew);
                        }
                    }
                }
            }
            callBack(err);
			return err;
        }
    }
}
