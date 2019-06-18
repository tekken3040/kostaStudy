using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

public class RankRewardRecvWindow : MonoBehaviour
{
    public Text _subTitle;
    public Text _rank;

    public GameObject _equipIcon;
    public Image _equipGrade;
    public Image _equipElement;
    public Image _equipImg;
    public Text _equipCnt;
    public GameObject _equipIcon2;
    public Image _equipGrade2;
    public Image _equipElement2;
    public Image _equipImg2;
    public Text _equipCnt2;

    public GameObject _itemIcon;
    public Image _itemGrade;
    public Image _itemImg;
    public Text _itemCnt;
    public GameObject _itemIcon2;
    public Image _itemGrade2;
    public Image _itemImg2;
    public Text _itemCnt2;

    public GameObject _goodsIcon;
    public Image _goodImg;
    public Text _goodsCnt;
    public GameObject _goodsIcon2;
    public Image _goodImg2;
    public Text _goodsCnt2;

    public Sprite[] _goodsSprite;
    public Vector2[] _goodsPos;         //보상 갯수에 따른 위치(0:동일 두개, 1:각각, 2:하나)
    public Vector2[] _itemPos;          //보상 갯수에 따른 위치(0,1:동일 두개, 2:각각, 3:하나)
    RankReward _rewardInfo;
    StringBuilder tempStringBuilder;

    public void SetData(RankReward _reward, UInt16 _index)
    {
        _equipIcon.SetActive(false);
        _itemIcon.SetActive(false);
        _goodsIcon.SetActive(false);
        _equipIcon2.SetActive(false);
        _itemIcon2.SetActive(false);
        _goodsIcon2.SetActive(false);
        _rewardInfo = new RankReward();
        _rewardInfo = _reward;
        SetItemImg(RankInfoMgr.Instance.dicRankData[_rewardInfo.u1RankType].u1RewardType1[_rewardInfo.u1RewardIndex-1], 
            RankInfoMgr.Instance.dicRankData[_rewardInfo.u1RankType].u1RewardType2[_rewardInfo.u1RewardIndex-1]);
        tempStringBuilder = new StringBuilder();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(RankInfoMgr.Instance.dicRankRewardData[_index].u4Rank.ToString()).Append(TextManager.Instance.GetText("mark_rank"));
        _rank.text = tempStringBuilder.ToString();
        switch(_rewardInfo.u1RankType-1)
        {
            case 0:
                _subTitle.text = TextManager.Instance.GetText("mark_user_power");
                break;

            case 1:
                _subTitle.text = TextManager.Instance.GetText("mark_crew_power");
                break;

            case 2:
                _subTitle.text = TextManager.Instance.GetText("mark_total_gold");
                break;

            case 3:
                _subTitle.text = TextManager.Instance.GetText("mark_weekly_used_cash");
                break;

            case 4:
                _subTitle.text = TextManager.Instance.GetText("mark_weekly_crafting");
                break;

            case 5:
                _subTitle.text = TextManager.Instance.GetText("mark_weekly_campaign_clear");
                break;

            case 6:
                _subTitle.text = TextManager.Instance.GetText("mark_weekly_forest_clear");
                break;

            case 7:
                _subTitle.text = TextManager.Instance.GetText("mark_weekly_tower_clear");
                break;
        }
    }

    public void SetItemImg(Byte _type, Byte _type2)
    {
        UInt16 _index = (UInt16)(_rewardInfo.u1RewardIndex-1);
        UInt16 _typeRank = (UInt16)_rewardInfo.u1RankType;
        //EquipmentInfo _info = EquipmentInfoMgr.Instance.GetInfo(RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardID1[_index]);
        //UInt16 modelID = _info.u2ModelID;
        //EquipmentInfo _info2 = EquipmentInfoMgr.Instance.GetInfo(RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardID2[_index]);
        //UInt16 modelID2 = _info.u2ModelID;
        UInt16 u2ID1 = RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardID1[_index];
        UInt16 u2ID2 = RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardID2[_index];
        SetIconPosition(_type, _type2);
        switch(_type)
        {
            case (Byte)GoodsType.NONE:
                _equipIcon.SetActive(false);
                _itemIcon.SetActive(false);
                _goodsIcon.SetActive(false);
                break;
        
            case (Byte)GoodsType.EQUIP:
                //if(modelID == 0) modelID = _info.u2ModelID;
                //ModelInfo modelInfo = ModelInfoMgr.Instance.GetInfo(modelID);
                //UInt16 gradeID = ForgeInfoMgr.Instance.GetList()[Mathf.Clamp(ItemInfoMgr.Instance.GetItemGrade(_info.u2ID),1,Server.ConstDef.MaxForgeLevel)-1].u2ID;
                //_equipGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + gradeID);
                //_equipElement.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + _info.u1Element);
                //_equipCnt.text = RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount1[_index].ToString();
                //_equipImg.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/" + modelInfo.sImagePath);
                break;
        
            case (Byte)GoodsType.MATERIAL:
                _itemGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(u2ID1));
                _itemCnt.text = RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount1[_index].ToString();
				_itemImg.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + ItemInfoMgr.Instance.GetMaterialItemInfo(u2ID1).u2IconID);
                Legion.Instance.cInventory.AddItem(0, u2ID1, RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount1[_index]);
                _itemIcon.SetActive(true);
                break;
        
            case (Byte)GoodsType.CONSUME:
                _itemGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(u2ID1));
                _itemCnt.text = RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount1[_index].ToString();
				_itemImg.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + ItemInfoMgr.Instance.GetConsumableItemInfo(u2ID1).u2ID);
                Legion.Instance.cInventory.AddItem(0, u2ID1, RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount1[_index]);
                _itemIcon.SetActive(true);
                break;

            case (Byte)GoodsType.GOLD:
                _goodImg.sprite = _goodsSprite[0];
                _goodImg.SetNativeSize();
                _goodsCnt.text = RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount1[_index].ToString();
                Legion.Instance.AddGoods((int)GoodsType.GOLD, RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount1[_index]);
                _goodsIcon.SetActive(true);
                break;

            case (Byte)GoodsType.CASH:
                _goodImg.sprite = _goodsSprite[1];
                _goodImg.SetNativeSize();
                _goodsCnt.text = RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount1[_index].ToString();
                Legion.Instance.AddGoods((int)GoodsType.CASH, RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount1[_index]);
                _goodsIcon.SetActive(true);
                break;

            case (Byte)GoodsType.KEY:
                _goodImg.sprite = _goodsSprite[2];
                _goodImg.SetNativeSize();
                _goodsCnt.text = RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount1[_index].ToString();
                Legion.Instance.AddGoods((int)GoodsType.KEY, RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount1[_index]);
                _goodsIcon.SetActive(true);
                break;

            case (Byte)GoodsType.LEAGUE_KEY:
                _goodImg.sprite = _goodsSprite[3];
                _goodImg.SetNativeSize();
                _goodsCnt.text = RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount1[_index].ToString();
                Legion.Instance.AddGoods((int)GoodsType.LEAGUE_KEY, RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount1[_index]);
                _goodsIcon.SetActive(true);
                break;

            case (Byte)GoodsType.FRIENDSHIP_POINT:
                _goodImg.sprite = _goodsSprite[4];
                _goodImg.SetNativeSize();
                _goodsCnt.text = RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount1[_index].ToString();
                Legion.Instance.AddGoods((int)GoodsType.FRIENDSHIP_POINT, RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount1[_index]);
                _goodsIcon.SetActive(true);
                break;
        }

        switch(_type2)
        {
            case (Byte)GoodsType.NONE:
                _equipIcon2.SetActive(false);
                _itemIcon2.SetActive(false);
                _goodsIcon2.SetActive(false);
                break;
        
            case (Byte)GoodsType.EQUIP:
                //if(modelID == 0) modelID2 = _info2.u2ModelID;
                //ModelInfo modelInfo2 = ModelInfoMgr.Instance.GetInfo(modelID);
                //UInt16 gradeID2 = ForgeInfoMgr.Instance.GetList()[Mathf.Clamp(ItemInfoMgr.Instance.GetItemGrade(_info2.u2ID),1,Server.ConstDef.MaxForgeLevel)-1].u2ID;
                //_equipGrade2.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + gradeID2);
                //_equipElement2.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + _info2.u1Element);
                //_equipCnt2.text = RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount2[_index].ToString();
                //_equipImg2.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/" + modelInfo2.sImagePath);
                break;
        
            case (Byte)GoodsType.MATERIAL:
                _itemGrade2.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(u2ID2));
                _itemCnt2.text = RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount2[_index].ToString();
				_itemImg2.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + ItemInfoMgr.Instance.GetMaterialItemInfo(u2ID2).u2IconID);
                Legion.Instance.cInventory.AddItem(0, u2ID2, RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount2[_index]);
                _itemIcon2.SetActive(true);
                break;
        
            case (Byte)GoodsType.CONSUME:
                _itemGrade2.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetItemGrade(u2ID2));
                _itemCnt2.text = RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount2[_index].ToString();
				_itemImg2.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + ItemInfoMgr.Instance.GetConsumableItemInfo(u2ID2).u2ID);
                Legion.Instance.cInventory.AddItem(0, u2ID2, RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount2[_index]);
                _itemIcon2.SetActive(true);
                break;

            case (Byte)GoodsType.GOLD:
                _goodImg2.sprite = _goodsSprite[0];
                _goodImg2.SetNativeSize();
                _goodsCnt2.text = RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount2[_index].ToString();
                Legion.Instance.AddGoods((int)GoodsType.GOLD, RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount2[_index]);
                _goodsIcon2.SetActive(true);
                break;

            case (Byte)GoodsType.CASH:
                _goodImg2.sprite = _goodsSprite[1];
                _goodImg2.SetNativeSize();
                _goodsCnt2.text = RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount2[_index].ToString();
                Legion.Instance.AddGoods((int)GoodsType.CASH, RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount2[_index]);
                _goodsIcon2.SetActive(true);
                break;

            case (Byte)GoodsType.KEY:
                _goodImg2.sprite = _goodsSprite[2];
                _goodImg2.SetNativeSize();
                _goodsCnt2.text = RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount2[_index].ToString();
                Legion.Instance.AddGoods((int)GoodsType.KEY, RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount2[_index]);
                _goodsIcon2.SetActive(true);
                break;

            case (Byte)GoodsType.LEAGUE_KEY:
                _goodImg2.sprite = _goodsSprite[3];
                _goodImg2.SetNativeSize();
                _goodsCnt2.text = RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount2[_index].ToString();
                Legion.Instance.AddGoods((int)GoodsType.LEAGUE_KEY, RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount2[_index]);
                _goodsIcon2.SetActive(true);
                break;

            case (Byte)GoodsType.FRIENDSHIP_POINT:
                _goodImg2.sprite = _goodsSprite[4];
                _goodImg2.SetNativeSize();
                _goodsCnt2.text = RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount2[_index].ToString();
                Legion.Instance.AddGoods((int)GoodsType.FRIENDSHIP_POINT, RankInfoMgr.Instance.dicRankData[_typeRank].u2RewardCount2[_index]);
                _goodsIcon2.SetActive(true);
                break;
        }
    }

    public void SetIconPosition(Byte _type, Byte _type2)
    {
        if(_type == (Byte)GoodsType.NONE)
        {
            _equipIcon.SetActive(false);
            _itemIcon.SetActive(false);
            _goodsIcon.SetActive(false);
        }

        if(_type2 == (Byte)GoodsType.NONE)
        {
            _equipIcon2.SetActive(false);
            _itemIcon2.SetActive(false);
            _goodsIcon2.SetActive(false);
        }

        if((_type == (Byte)GoodsType.GOLD || _type == (Byte)GoodsType.CASH || _type == (Byte)GoodsType.KEY || _type == (Byte)GoodsType.LEAGUE_KEY || _type == (Byte)GoodsType.FRIENDSHIP_POINT)
            && (_type2 == (Byte)GoodsType.GOLD || _type2 == (Byte)GoodsType.CASH || _type2 == (Byte)GoodsType.KEY || _type2 == (Byte)GoodsType.LEAGUE_KEY || _type2 == (Byte)GoodsType.FRIENDSHIP_POINT))
        {
            _goodsIcon.SetActive(true);
            _goodsIcon2.SetActive(true);
            _goodsIcon.transform.localPosition = _goodsPos[0];
            _goodsIcon2.transform.localPosition = _goodsPos[1];
        }

        else if((_type == (Byte)GoodsType.GOLD || _type == (Byte)GoodsType.CASH || _type == (Byte)GoodsType.KEY || _type == (Byte)GoodsType.LEAGUE_KEY || _type == (Byte)GoodsType.FRIENDSHIP_POINT)
            && (_type2 == (Byte)GoodsType.EQUIP || _type2 == (Byte)GoodsType.MATERIAL || _type2 == (Byte)GoodsType.CONSUME))
        {
            _goodsIcon.SetActive(true);
            _goodsIcon.transform.localPosition = _goodsPos[1];
            if(_type2 == (Byte)GoodsType.EQUIP)
            {
                _equipIcon2.SetActive(true);
                _equipIcon2.transform.localPosition = _itemPos[2];
            }
            else if(_type2 == (Byte)GoodsType.MATERIAL || _type2 == (Byte)GoodsType.CONSUME)
            {
                _itemIcon2.SetActive(true);
                _itemIcon2.transform.localPosition = _itemPos[2];
            }
        }

        else if((_type == (Byte)GoodsType.EQUIP || _type == (Byte)GoodsType.MATERIAL || _type == (Byte)GoodsType.CONSUME)
            && (_type2 == (Byte)GoodsType.GOLD || _type2 == (Byte)GoodsType.CASH || _type2 == (Byte)GoodsType.KEY || _type2 == (Byte)GoodsType.LEAGUE_KEY || _type2 == (Byte)GoodsType.FRIENDSHIP_POINT))
        {
            _goodsIcon2.SetActive(true);
            _goodsIcon2.transform.localPosition = _goodsPos[1];
            if(_type == (Byte)GoodsType.EQUIP)
            {
                _equipIcon.SetActive(true);
                _equipIcon.transform.localPosition = _itemPos[2];
            }
            else if(_type == (Byte)GoodsType.MATERIAL || _type == (Byte)GoodsType.CONSUME)
            {
                _itemIcon.SetActive(true);
                _itemIcon.transform.localPosition = _itemPos[2];
            }
        }
        
        else if((_type == (Byte)GoodsType.EQUIP || _type == (Byte)GoodsType.MATERIAL || _type == (Byte)GoodsType.CONSUME)
            && (_type2 == (Byte)GoodsType.EQUIP || _type2 == (Byte)GoodsType.MATERIAL || _type2 == (Byte)GoodsType.CONSUME))
        {
            if(_type == (Byte)GoodsType.MATERIAL || _type == (Byte)GoodsType.CONSUME)
            {
                _itemIcon.SetActive(true);
                _itemIcon.transform.localPosition = _itemPos[0];
            }
            else if(_type == (Byte)GoodsType.EQUIP)
            {
                _equipIcon.SetActive(true);
                _equipIcon.transform.localPosition = _itemPos[0];
            }

            if(_type2 == (Byte)GoodsType.MATERIAL || _type2 == (Byte)GoodsType.CONSUME)
            {
                _itemIcon2.SetActive(true);
                _itemIcon2.transform.localPosition = _itemPos[1];
            }
            else if(_type2 == (Byte)GoodsType.EQUIP)
            {
                _equipIcon2.SetActive(true);
                _equipIcon2.transform.localPosition = _itemPos[1];
            }
        }

        else if((_type == (Byte)GoodsType.GOLD || _type == (Byte)GoodsType.CASH || _type == (Byte)GoodsType.KEY || _type == (Byte)GoodsType.LEAGUE_KEY || _type == (Byte)GoodsType.FRIENDSHIP_POINT)
            && _type2 == (Byte)GoodsType.NONE)
        {
            _goodsIcon.SetActive(true);
            _goodsIcon.transform.localPosition = _goodsPos[2];
        }

        else if((_type == (Byte)GoodsType.EQUIP || _type == (Byte)GoodsType.MATERIAL || _type == (Byte)GoodsType.CONSUME)
            && _type2 == (Byte)GoodsType.NONE)
        {
            if(_type == (Byte)GoodsType.MATERIAL || _type == (Byte)GoodsType.CONSUME)
            {
                _itemIcon.SetActive(true);
                _itemIcon.transform.localPosition = _itemPos[3];
            }
            else if(_type == (Byte)GoodsType.EQUIP)
            {
                _equipIcon.SetActive(true);
                _equipIcon.transform.localPosition = _itemPos[3];
            }
        }

        else if((_type2 == (Byte)GoodsType.GOLD || _type2 == (Byte)GoodsType.CASH || _type2 == (Byte)GoodsType.KEY || _type2 == (Byte)GoodsType.LEAGUE_KEY || _type2 == (Byte)GoodsType.FRIENDSHIP_POINT)
            && _type == (Byte)GoodsType.NONE)
        {
            _goodsIcon2.SetActive(true);
            _goodsIcon2.transform.localPosition = _goodsPos[2];
        }

        else if((_type2 == (Byte)GoodsType.EQUIP || _type2 == (Byte)GoodsType.MATERIAL || _type2 == (Byte)GoodsType.CONSUME)
            && _type == (Byte)GoodsType.NONE)
        {
            if(_type2 == (Byte)GoodsType.MATERIAL || _type2 == (Byte)GoodsType.CONSUME)
            {
                _itemIcon2.SetActive(true);
                _itemIcon2.transform.localPosition = _itemPos[3];
            }
            else if(_type2 == (Byte)GoodsType.EQUIP)
            {
                _equipIcon2.SetActive(true);
                _equipIcon2.transform.localPosition = _itemPos[3];
            }
        }
    }
}
