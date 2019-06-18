using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class StrongRecommendPopup : MonoBehaviour
{
    enum StrongRecommendToggle
    {
        GrowthPopup = 0,
        CharacterPopup,
        EquipPackPopup,
        UpbringingPopup,
        Pack30DayPopup,
        MAX_POPUP,
    }

    public RectTransform _popupParent;
    public Image[] _tabBtnImageList;    // 탑 버튼 리스트
    private int _nCutBtnIdx;         // 현재 버튼 인덱스
    public Sprite[] imgBtnImage;

    protected GameObject[] PopupObjList = new GameObject[(int)StrongRecommendToggle.MAX_POPUP];
    
    void OnEnable()
    {
        _nCutBtnIdx = -1;
        bool isBuy = false;
        for (int i = 0; i < (int)StrongRecommendToggle.MAX_POPUP; ++i)
        {
            if(IsBuyPackage(i) == true)
            {
                OnClickTabButton(i);
                isBuy = true;
                break;
            }
        }
        if (isBuy == false)
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_buy_disable"), TextManager.Instance.GetText("event_goods_no_sales"), null);
            OnClickClose();
        }
    }
    
    public void OnClickTabButton(int selsectBtnIdx)
    {
        if (_nCutBtnIdx == selsectBtnIdx)
            return;

        if(IsBuyPackage(selsectBtnIdx) == false)
        {
            if(selsectBtnIdx == (int)StrongRecommendToggle.CharacterPopup && Legion.Instance.sName == "")
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_buy_disable"), TextManager.Instance.GetText("popup_desc_char_pkg_not_crew"), null);
            else
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_buy_disable"), TextManager.Instance.GetText("event_goods_no_sales"), null);
            return;
        }

        bool isEventPageOpen = true;
        if (selsectBtnIdx >= 0 && selsectBtnIdx < (int)StrongRecommendToggle.MAX_POPUP)
        {
            if (PopupObjList[selsectBtnIdx] == null)
                isEventPageOpen = CreatePopup(selsectBtnIdx);
            else
                ChangeTabButtonImgae(selsectBtnIdx, true);
            //PopupObjList[selsectBtnIdx].SetActive(true);
        }
        
        if (isEventPageOpen)
        {
            ChangeTabButtonImgae(selsectBtnIdx, true);
        }
        else
        {
            PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_buy_disable"), TextManager.Instance.GetText("event_goods_no_sales"), null);
            return;
        }

        if (_nCutBtnIdx >= 0)
        {
            if (PopupObjList[_nCutBtnIdx] != null)
                ChangeTabButtonImgae(_nCutBtnIdx, false);
                //PopupObjList[_nCutToggleIdx].SetActive(false);
        }

        _nCutBtnIdx = selsectBtnIdx;
    }

    protected void ChangeTabButtonImgae(int btnIdx, bool isEnable)
    {
        PopupObjList[btnIdx].SetActive(isEnable);
        if (isEnable == true)
            _tabBtnImageList[btnIdx].sprite = imgBtnImage[0];
        else
            _tabBtnImageList[btnIdx].sprite = imgBtnImage[1];
    }

    protected bool IsBuyPackage(int selectIndex)
    {
        int eventCount = 0;
        switch ((StrongRecommendToggle)selectIndex)
        {
            case StrongRecommendToggle.GrowthPopup:
                eventCount = EventInfoMgr.Instance.GetSaleEventListByEventType((Byte)PackagePopup.PackageMenu.Recommend).Count;
                break;
            case StrongRecommendToggle.CharacterPopup:
                eventCount = EventInfoMgr.Instance.GetSaleEventListByEventType((Byte)PackagePopup.PackageMenu.Character).Count;
                break;
            case StrongRecommendToggle.EquipPackPopup:
                eventCount = EventInfoMgr.Instance.GetSaleEventListByEventType((Byte)PackagePopup.PackageMenu.EquipPack).Count;
                break;
            case StrongRecommendToggle.UpbringingPopup:
                eventCount = EventInfoMgr.Instance.GetEventListByEventType((Byte)PackagePopup.PackageMenu.Promot).Count;
                break;
            case StrongRecommendToggle.Pack30DayPopup:
                eventCount = EventInfoMgr.Instance.GetEventListByEventType((Byte)PackagePopup.PackageMenu.Month).Count;
                break;
        }

        if (eventCount <= 0)
            return false;

        return true;
    }

    protected bool CreatePopup(int selectIndex)
    {
        int eventCount = 0;
        string prefabPath = null;
        switch((StrongRecommendToggle)selectIndex)
        {
            case StrongRecommendToggle.GrowthPopup:
                eventCount = EventInfoMgr.Instance.GetEventListByEventType((Byte)PackagePopup.PackageMenu.Recommend).Count;
                prefabPath = "Prefabs/UI/Event/Pref_GrowthPackPopup.prefab";
                break;
            case StrongRecommendToggle.CharacterPopup:
                eventCount = EventInfoMgr.Instance.GetEventListByEventType((Byte)PackagePopup.PackageMenu.Character).Count;
                prefabPath = "Prefabs/UI/Event/Pref_CharacterPackPopup.prefab";
                break;
            case StrongRecommendToggle.EquipPackPopup:
                eventCount = EventInfoMgr.Instance.GetEventListByEventType((Byte)PackagePopup.PackageMenu.EquipPack).Count;
                prefabPath = "Prefabs/UI/Event/Pref_EquipPackPopup.prefab";
                break;
            case StrongRecommendToggle.UpbringingPopup:
                eventCount = EventInfoMgr.Instance.GetEventListByEventType((Byte)PackagePopup.PackageMenu.Promot).Count;
                prefabPath = "Prefabs/UI/Event/Pref_UpbringingRewardPopup.prefab";
                break;
            case StrongRecommendToggle.Pack30DayPopup:
                eventCount = EventInfoMgr.Instance.GetEventListByEventType((Byte)PackagePopup.PackageMenu.Month).Count;
                prefabPath = "Prefabs/UI/Event/Pref_30DayPackPopup.prefab";
                break;
        }
        
        if(eventCount <= 0)
            return false;

        if (prefabPath == null)
        {
            DebugMgr.LogError("이벤트 프리팹의 경로가 존재하지 않습니다");
            return false;
        }

        GameObject popupObj = Instantiate(AssetMgr.Instance.AssetLoad(prefabPath, typeof(GameObject))) as GameObject;
        RectTransform rtTr = popupObj.GetComponent<RectTransform>();
        rtTr.SetParent(_popupParent);
        rtTr.anchoredPosition3D = Vector3.zero;
        rtTr.localScale = Vector3.one;
        rtTr.sizeDelta = Vector3.zero;

        bool isSetting = popupObj.GetComponent<BaseEventPopup>().SetPopup(true);
        PopupObjList[selectIndex] = popupObj;
        //ChangeTabButtonImgae(selectIndex, true); //PopupObjList[selectIndex].SetActive(true);

        return isSetting;
    }

    public void OnClickClose()
    {
        PopupManager.Instance.RemovePopup(this.gameObject);
        Destroy(this.gameObject);
    }

    public void OnClickComingSoon()
    {
        PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("title_notice"), TextManager.Instance.GetText("recomm_goods_ready"), null);
    }
}
