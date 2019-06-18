using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class RankRewardInfoSlot : MonoBehaviour
{
    public Text _rank;
    public Text _reward;
    StringBuilder tempStringBuilder;

    public void Awake()
    {
        tempStringBuilder = new StringBuilder();
    }

    public void SetData(Byte _type, int _num)
    {
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        if(_num <3)
            tempStringBuilder.Append(RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u2RankBase[_num].ToString());
        else
        {
            tempStringBuilder.Append((RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u2RankBase[_num-1]+1));
            tempStringBuilder.Append("~");
            tempStringBuilder.Append((RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u2RankBase[_num]));
        }
        tempStringBuilder.Append(TextManager.Instance.GetText("mark_rank"));
        _rank.text = tempStringBuilder.ToString();

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        Byte itemType;
        itemType = RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u1RewardType1[_num];
        switch(itemType)
        {
            case (Byte)GoodsType.NONE:
                break;
        
            case (Byte)GoodsType.EQUIP:
                break;
        
            case (Byte)GoodsType.MATERIAL:
                tempStringBuilder.Append(
                    TextManager.Instance.GetText(ItemInfoMgr.Instance.GetMaterialItemInfo(
                        RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u2RewardID1[_num]).sName)).Append(" ");
                tempStringBuilder.Append(RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u2RewardCount1[_num]);
                //_reward.text = tempStringBuilder.ToString();
                break;
        
            case (Byte)GoodsType.CONSUME:
                tempStringBuilder.Append(
                    TextManager.Instance.GetText(ItemInfoMgr.Instance.GetConsumableItemInfo(
                        RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u2RewardID1[_num]).sName)).Append(" ");
                tempStringBuilder.Append(RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u2RewardCount1[_num]);
                //_reward.text = tempStringBuilder.ToString();
                break;

            case (Byte)GoodsType.GOLD:
                tempStringBuilder.Append(TextManager.Instance.GetText("mark_gold")).Append(" ").Append(
                    RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u2RewardCount1[_num]);
                //_reward.text = tempStringBuilder.ToString();
                break;

            case (Byte)GoodsType.CASH:
                tempStringBuilder.Append(TextManager.Instance.GetText("mark_cash")).Append(" ").Append(
                    RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u2RewardCount1[_num]);
                //_reward.text = tempStringBuilder.ToString();
                break;

            case (Byte)GoodsType.KEY:
                tempStringBuilder.Append(TextManager.Instance.GetText("mark_key")).Append(" ").Append(
                    RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u2RewardCount1[_num]);
                //_reward.text = tempStringBuilder.ToString();
                break;

            case (Byte)GoodsType.LEAGUE_KEY:
                tempStringBuilder.Append(TextManager.Instance.GetText("mark_leaguekey")).Append(" ").Append(
                    RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u2RewardCount1[_num]);
                //_reward.text = tempStringBuilder.ToString();
                break;

            case (Byte)GoodsType.FRIENDSHIP_POINT:
                tempStringBuilder.Append(TextManager.Instance.GetText("mark_friendshippoint")).Append(" ").Append(
                    RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u2RewardCount1[_num]);
                //_reward.text = tempStringBuilder.ToString();
                break;
        }

        //tempStringBuilder.Remove(0, tempStringBuilder.Length);
        itemType = RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u1RewardType2[_num];
        switch(itemType)
        {
            case (Byte)GoodsType.NONE:
                break;
        
            case (Byte)GoodsType.EQUIP:
                break;
        
            case (Byte)GoodsType.MATERIAL:
                tempStringBuilder.Append(" / ").Append(
                    TextManager.Instance.GetText(ItemInfoMgr.Instance.GetMaterialItemInfo(
                        RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u2RewardID2[_num]).sName)).Append(" ");
                tempStringBuilder.Append(RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u2RewardCount2[_num]);
                _reward.text = tempStringBuilder.ToString();
                break;
        
            case (Byte)GoodsType.CONSUME:
                tempStringBuilder.Append(" / ").Append(
                    TextManager.Instance.GetText(ItemInfoMgr.Instance.GetConsumableItemInfo(
                        RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u2RewardID2[_num]).sName)).Append(" ");
                tempStringBuilder.Append(RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u2RewardCount2[_num]);
                _reward.text = tempStringBuilder.ToString();
                break;

            case (Byte)GoodsType.GOLD:
                tempStringBuilder.Append(" / ").Append(TextManager.Instance.GetText("mark_gold")).Append(" ").Append(
                    RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u2RewardCount2[_num]);
                _reward.text = tempStringBuilder.ToString();
                break;

            case (Byte)GoodsType.CASH:
                tempStringBuilder.Append(" / ").Append(TextManager.Instance.GetText("mark_cash")).Append(" ").Append(
                    RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u2RewardCount2[_num]);
                _reward.text = tempStringBuilder.ToString();
                break;

            case (Byte)GoodsType.KEY:
                tempStringBuilder.Append(" / ").Append(TextManager.Instance.GetText("mark_key")).Append(" ").Append(
                    RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u2RewardCount2[_num]);
                _reward.text = tempStringBuilder.ToString();
                break;

            case (Byte)GoodsType.LEAGUE_KEY:
                tempStringBuilder.Append(" / ").Append(TextManager.Instance.GetText("mark_leaguekey")).Append(" ").Append(
                    RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u2RewardCount2[_num]);
                _reward.text = tempStringBuilder.ToString();
                break;

            case (Byte)GoodsType.FRIENDSHIP_POINT:
                tempStringBuilder.Append(" / ").Append(TextManager.Instance.GetText("mark_friendshippoint")).Append(" ").Append(
                    RankInfoMgr.Instance.dicRankData[(UInt16)(_type+1)].u2RewardCount2[_num]);
                _reward.text = tempStringBuilder.ToString();
                break;
        }
    }
}
