using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class EquipPackagePopup : BaseEventPopup
{
    public Text _txtPackageNotice;
    private Dictionary<UInt16, EquipPackSlot> _packSlotList = new Dictionary<UInt16, EquipPackSlot>();
    public override bool SetPopup(bool isStrongPopup = false)
    {
        base.SetPopup(isStrongPopup);
        if (_trSlotParent.childCount > 0)
            return false;

        // 슬롯 오브젝트의 정보가 없다면 불러온다
        if (_objSlot == null)
        {
            _objSlot = AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_EquipPackSlot.prefab", typeof(GameObject)) as GameObject;
        }
        
        List<EventPackageInfo> lstEventList = EventInfoMgr.Instance.GetOpenEventListByEventType(9);
        for (int i = 0; i < lstEventList.Count; ++i)
        {
            int checkBuyPossibleType = EventInfoMgr.Instance.CheckBuyPossible(lstEventList[i].u2ID);
            if (checkBuyPossibleType == 0)
                continue;

            GameObject addSlot = Instantiate(_objSlot) as GameObject;
            RectTransform rtTr = addSlot.GetComponent<RectTransform>();
            rtTr.SetParent(_trSlotParent);
            rtTr.anchoredPosition3D = Vector3.zero;
            rtTr.localScale = Vector3.one;

            EquipPackSlot slotScript = addSlot.GetComponent<EquipPackSlot>();
            slotScript.SetSlot(lstEventList[i]);

            if (checkBuyPossibleType == 2)
                slotScript._btnBuyBtn.interactable = false;
            else
            {
                EventPackageInfo info = (lstEventList[i]);
                slotScript._btnBuyBtn.interactable = true;
                slotScript._btnBuyBtn.onClick.AddListener(() => OnClickBuyBtn(info));//lstEventList[i], slotCout));
            }
            _packSlotList.Add(lstEventList[i].u2ID, slotScript);
        }

        Vector2 sizeDelta = _trSlotParent.GetComponent<RectTransform>().sizeDelta;
        _trSlotParent.GetComponent<RectTransform>().sizeDelta.Set(
            _objSlot.GetComponent<RectTransform>().sizeDelta.x * lstEventList.Count + 10, sizeDelta.y);

        // 상품 소개
        _txtPackageNotice.text = string.Format(TextManager.Instance.GetText("event_equip_goods_pack_desc"), GetEventPackagePriceString(lstEventList[0]));

        return true;
    }

    protected override void BuyRefiesh()
    {
        if(_cBuyPackageInfo != null)
        {
            int checkBuyPossibleType = EventInfoMgr.Instance.CheckBuyPossible(_cBuyPackageInfo.u2ID);
            if(checkBuyPossibleType != 1)
                _packSlotList[_cBuyPackageInfo.u2ID]._btnBuyBtn.interactable = false;
        }
    }
}
