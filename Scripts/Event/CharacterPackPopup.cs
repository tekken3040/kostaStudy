using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class CharacterPackPopup : BaseEventPopup
{
    public Text[] _txtBuyPrice;
    public Button[] _btnBuy;
    public Image _imgPopupBG;

    private int _nGroupIndex;
    List<EventPackageInfo> characterPackList;

    void OnEnable()
    {
        _nGroupIndex = 0;
    }

    public override bool SetPopup(bool isStrongPopup = false)
    {
        base.SetPopup(isStrongPopup);
        characterPackList = EventInfoMgr.Instance.GetOpenEventListByEventType(8);

        int count = EventInfoMgr.Instance.GetSaleEventListByEventType(8).Count;
        
        // 정렬
        characterPackList.Sort(delegate (EventPackageInfo info1, EventPackageInfo info2)
        {
            if (info1.u1EventGroupIdx > info2.u1EventGroupIdx) return 1;
            else if (info1.u1EventGroupIdx < info2.u1EventGroupIdx) return -1;
            else return 0;
        });

        if(CheckNextInfo() == true)
        {
            SetPopup();
        }
        else
        {
            //PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_buy_disable"), TextManager.Instance.GetText("event_goods_no_sales"), null);
            base.OnClickClosePopup();
            return false;
        }

        return true;
    }

    private void SetPopup()
    {
        StringBuilder tempString = new StringBuilder();
        tempString.Append("Sprites/").Append(TextManager.Instance.GetImagePath());
        tempString.Append("_Class_").Append(_nGroupIndex.ToString()).Append(".png");
        _imgPopupBG.sprite = AssetMgr.Instance.AssetLoad(tempString.ToString(), typeof(Sprite)) as Sprite;

        for (int i = 0; i < characterPackList.Count; ++i)
        {
            EventPackageInfo packageInfo = characterPackList[i];
            if (packageInfo.u1EventGroupIdx == _nGroupIndex)
            {
                int btnSlotIdx = packageInfo.u1EventGroupOrder - 1;
                _txtBuyPrice[btnSlotIdx].text = GetEventPackagePriceString(packageInfo);
                if(EventInfoMgr.Instance.CheckBuyPossible(packageInfo.u2ID) == 1)
                {
                    _btnBuy[btnSlotIdx].interactable = true;
                    _btnBuy[btnSlotIdx].onClick.RemoveAllListeners();
                    _btnBuy[btnSlotIdx].onClick.AddListener(() => OnClickBuyBtn(packageInfo));
                }
                else
                {
                    _btnBuy[btnSlotIdx].interactable = false;
                }   
            }
        }
    }

    private bool CheckNextInfo()
    {
        bool isEnabled = false;
        if (characterPackList.Count > 0)
        {
            int changeGroupIdx = _nGroupIndex;
            for (int i = 0; i < characterPackList.Count; ++i)
            {
                if (characterPackList[i].u1EventGroupIdx > _nGroupIndex)
                {
                    changeGroupIdx = characterPackList[i].u1EventGroupIdx;
                    break;
                }
            }
            // 이전 그룹과 같다면 팝업을 노출하지 않는다
            if (changeGroupIdx == _nGroupIndex)
                return false;
            else
                _nGroupIndex = changeGroupIdx;

            for (int i = 0; i < characterPackList.Count; ++i)
            {
                if (characterPackList[i].u1EventGroupIdx == _nGroupIndex)
                {
                    if (EventInfoMgr.Instance.CheckBuyPossible(characterPackList[i].u2ID) == 1)
                        isEnabled = true;
                }
            }
        }
        return isEnabled;
    }

    public override void OnClickClosePopup()
    {
        if(CheckNextInfo ())
        {
            SetPopup();
        }
        else
        {
            base.OnClickClosePopup();
        }
    }

    protected override void BuyRefiesh()
    {
        base.BuyRefiesh();
        _btnBuy[_cBuyPackageInfo.u1EventGroupOrder - 1].interactable = false;
    }

    protected override void SetBeforeBuyNotice()
    {
        StringBuilder tempString = new StringBuilder();
        tempString.Append(TextManager.Instance.GetText("mark_goods_warning_1")).Append("\n");
        tempString.Append(TextManager.Instance.GetText("mark_goods_warning_3"));

        _txtBeforeBuyNotice.text = tempString.ToString();
    }
}