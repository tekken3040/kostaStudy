using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class GrowthPackagePopup : BaseEventPopup 
{
	[SerializeField]
	private int SLOT_SCROLL_ENABLE_MIN_COUNT; //
	private const int SLOT_BG_COUT = 3;

	public ScrollRect _SlotScroll;
    public Text _txtPackageNotice;

	private float SLOT_SPACING;	// 슬롯간의 간격
	private float _fSlotWidht;	// 슬롯의 넓이

    private Byte u1EventGroupIdx;
	private Dictionary<UInt16, GrowthPackSlot> _packSlotList = new Dictionary<UInt16, GrowthPackSlot>();

	void Awake()
	{
		SLOT_SPACING = _trSlotParent.GetComponent<GridLayoutGroup>().spacing.x;
        u1EventGroupIdx = 0;
    }

    public bool SetPopup(Byte u1GroupIdx, bool isStrongPopup = false)
    {
        u1EventGroupIdx = u1GroupIdx;
        return SetPopup(isStrongPopup);
    }

    public override bool SetPopup(bool isStrongPopup = false)
    {
        base.SetPopup(isStrongPopup);
        // 이미 셋팅 햇다면
		if(_trSlotParent.childCount > 0)
			return true;

		List<EventPackageInfo> lstEventList = EventInfoMgr.Instance.GetOpenEventListByEventType(6);
        if (lstEventList.Count <= 0)
            return false;

        if(_txtPackageNotice != null)
            _txtPackageNotice.gameObject.SetActive(!isStrongPopup);
        
        StringBuilder tempString = new StringBuilder();
        // 안내 문구 설정
        if (u1EventGroupIdx == 0)
        {
            tempString.Append("Sprites/").Append(TextManager.Instance.GetImagePath()).Append(".GrowthPack_title");
            _titleObj.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite(tempString.ToString());
            _txtPackageNotice.text = TextManager.Instance.GetText("event_grow_pack_title_desc");
        }
        else
        {
            tempString.Append("Sprites/").Append(TextManager.Instance.GetImagePath()).Append(".Material_Pack_title");
            _txtPackageNotice.text = TextManager.Instance.GetText("event_marterial_pack_desc");
        }
        Image titleImg = _titleObj.GetComponent<Image>();
        titleImg.sprite = AtlasMgr.Instance.GetSprite(tempString.ToString());
        titleImg.SetNativeSize();

        // 슬롯 오브젝트의 정보가 없다면 불러온다
        if (_objSlot == null)
        {
            _objSlot = AssetMgr.Instance.AssetLoad("Prefabs/UI/Event/Pref_GrowthPackSlot.prefab", typeof(GameObject)) as GameObject;
            _fSlotWidht = _objSlot.GetComponent<RectTransform>().sizeDelta.x;
        }

        int slotCout = 0;
        for ( int i = 0; i < lstEventList.Count; ++i )
		{
			if (EventInfoMgr.Instance.dicEventBuy.ContainsKey (lstEventList [i].u2ID) == false)
				continue;

            if (u1EventGroupIdx != lstEventList[i].u1EventGroupIdx)
                continue;

			int checkBuyPossibleType = EventInfoMgr.Instance.CheckBuyPossible (lstEventList [i].u2ID);
			if(checkBuyPossibleType == 0)
				continue;

			GameObject addSlot = Instantiate(_objSlot) as GameObject;
			RectTransform rtTr = addSlot.GetComponent<RectTransform>();
			rtTr.SetParent(_trSlotParent);
			rtTr.anchoredPosition3D = Vector3.zero;
			rtTr.localScale = Vector3.one;

			GrowthPackSlot slotScript = addSlot.GetComponent<GrowthPackSlot>();
			slotScript.SetSlot(lstEventList[i], (slotCout++ % SLOT_BG_COUT), u1EventGroupIdx);

			if(checkBuyPossibleType == 2)
				slotScript.DisableButton();
			else
			{
				EventPackageInfo info = (lstEventList [i]);
				slotScript._btnBuyBtn.onClick.AddListener (() => OnClickBuyBtn(info));//lstEventList[i], slotCout));
			}
			_packSlotList.Add(lstEventList[i].u2ID, slotScript);
		}

        if (_trSlotParent.childCount <= 0)
        {
            OnClickClosePopup();
            return false;
        }

        RefreshSlotParentSize();
        return true;
	}

	protected void RefreshSlotParentSize()
	{
		// 슬롯의 갯수를 얻어 온다
		int slotCount = _trSlotParent.childCount;
        Vector2 size = new Vector2(slotCount * (SLOT_SPACING + _fSlotWidht), _trSlotParent.sizeDelta.y);
        _trSlotParent.sizeDelta = size;

        if (slotCount <= SLOT_SCROLL_ENABLE_MIN_COUNT)
			_SlotScroll.enabled = false;
		else
			_SlotScroll.enabled = true;
	}

	// 구매후 새로 고침
	protected override void BuyRefiesh()
	{
        base.BuyRefiesh();

		_packSlotList[_cBuyPackageInfo.u2ID].DisableButton();
    }
}
