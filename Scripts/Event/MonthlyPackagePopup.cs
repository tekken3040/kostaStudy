using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MonthlyPackagePopup : BaseEventPopup
{
    private Dictionary<UInt16, MonthlyPackSlot> _packSlotList = new Dictionary<UInt16, MonthlyPackSlot>();

    public override bool SetPopup(bool isStrongPopup)
    {
        base.SetPopup(isStrongPopup);
        if (_trSlotParent.childCount > 0)
            return false;

        // 슬롯 오브젝트의 정보가 없다면 불러온다
        if (_objSlot == null)
        {
            _objSlot = AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_MonthlyPackSlot.prefab", typeof(GameObject)) as GameObject;
        }

        List<EventPackageInfo> lstEventList = EventInfoMgr.Instance.GetSaleEventListByEventType((Byte)PackageMenu.MonthlyGoods);
        if (lstEventList.Count == 0)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_buy_disable"), TextManager.Instance.GetText("event_goods_no_sales"), null);
            OnClickClosePopup();
            return false;
        }

        for (int i = 0; i < lstEventList.Count; ++i)
        {
            //int checkBuyPossibleType = EventInfoMgr.Instance.CheckBuyPossible(lstEventList[i].u2ID);
            //if (checkBuyPossibleType == 0)
            //    continue;
            //
            GameObject addSlot = Instantiate(_objSlot) as GameObject;
            RectTransform rtTr = addSlot.GetComponent<RectTransform>();
            rtTr.SetParent(_trSlotParent);
            rtTr.anchoredPosition3D = Vector3.zero;
            rtTr.localScale = Vector3.one;

            MonthlyPackSlot slotScript = addSlot.GetComponent<MonthlyPackSlot>();
            slotScript.SetSlot(lstEventList[i]);

            //if (checkBuyPossibleType == 2)
            //    slotScript.DisableButton();
            //else
            //{
                EventPackageInfo info = (lstEventList[i]);
                slotScript._btnBuyBtn.onClick.AddListener(() => OnClickBuyBtn(info));
            //}
            _packSlotList.Add(lstEventList[i].u2ID, slotScript);
        }

        Vector2 sizeDelta = _trSlotParent.GetComponent<RectTransform>().sizeDelta;
        _trSlotParent.GetComponent<RectTransform>().sizeDelta.Set(
            _objSlot.GetComponent<RectTransform>().sizeDelta.x * lstEventList.Count + 10, sizeDelta.y);

        return true;
    }

    // 팝업을 닫는다
    public override void OnClickClosePopup()
    {
        Legion.Instance.SubLoginPopupStep(Legion.LoginPopupStep.EVENT_MONTHLY_PACK);
        Destroy(this.gameObject);
    }
}
