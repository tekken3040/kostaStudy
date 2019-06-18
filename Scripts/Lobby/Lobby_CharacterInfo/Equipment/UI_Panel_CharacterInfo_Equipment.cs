using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class UI_Panel_CharacterInfo_Equipment : MonoBehaviour {
	[SerializeField] RectTransform _trMenuGroup;
	[SerializeField] GameObject _objBackButton;
	[SerializeField] GameObject _objGoodsPanel;
	[SerializeField] GameObject _objPanel;
	[SerializeField] GameObject _objLevel;
    [SerializeField] GameObject _objCharacterinfo;
    [SerializeField] GameObject _objPowerValue;
	[SerializeField] UI_Slot_CharacterInfo_Equipment[] _acEquipSlots;
	[SerializeField] Text _txtLevel;
	[SerializeField] Image _imgElement;
	[SerializeField] Image _imgExpGauge;
	[SerializeField] Text _txtExp;

	[SerializeField] UI_SubPanel_CharacterInfo_Equipment_ItemList _panelItemList;
	[SerializeField] UI_SubPanel_CharacterInfo_Equipment_ItemInfo _panelItemInfo;
	[SerializeField] UI_Panel_CharacterInfo_Equipment_AutoEquip _panelAutoEquip;

    [SerializeField] UI_Panel_CharacterInfo _panelParent;
	// [SerializeField] UI_SubPanel_CharacterInfo_Equipment_ModelViewer _panelModelViewer;
	Hero _cHero;
    public GameObject _objMaterialPackageBtn;

    StringBuilder tempStringBuilder;
    public UI_SubPanel_CharacterInfo_Equipment_ItemList GetPanelItemList
    {
        get
        {
            return _panelItemList;
        }
    }
	int selectedPosIdx;
    public int SelectPosIdx
    {
        set
        {
            selectedPosIdx = value;
        }
        get
        {
            return selectedPosIdx;
        }
    }

	UInt16 SeqNo = 0;
	public void OnClickEquipSlot(int posID)
	{
		_trMenuGroup.SetAsFirstSibling();

		_cHero.cObject.SetActive(false);

		_panelItemList.SetData(_cHero.acEquips[(posID-1)], this);
		_panelItemList.gameObject.SetActive(true);

		_acEquipSlots[(posID-1)].transform.SetSiblingIndex(13);
		_acEquipSlots[selectedPosIdx].DeSelect();
		_acEquipSlots[(posID-1)].Select();
		_acEquipSlots[(posID-1)].disableAttachEffect();
        _acEquipSlots[(posID-1)]._objStatPointEffect.SetActive(false);
		selectedPosIdx = posID-1;

		_objBackButton.SetActive(false);
		//_objPanel.SetActive(false);
		_objLevel.SetActive(false);
        //_objGoodsPanel.SetActive(false);
        StopChangeEffect();

        PopupManager.Instance.AddPopup(_panelItemList.gameObject, _panelItemList.OnClickClose);
	}
//	public void ResetEquipSlotSibling()
//	{
//		_acEquipSlots[selectedPosIdx].transform.SetSiblingIndex(selectedPosIdx+2);
//	}
	public void OnClickAttach()
	{
        UInt32 prevPower = _cHero.u4Power;
        DebugMgr.Log("attach pos : " + selectedPosIdx + " slot num : " + _cHero.acEquips[selectedPosIdx].u2SlotNum);
		_acEquipSlots[selectedPosIdx].SetData(_cHero.acEquips[selectedPosIdx]);
        _objCharacterinfo.GetComponent<UI_Panel_CharacterInfo>().GetPanelStatus().SetCharacterInfo(_cHero);

        StartChangeEffect(prevPower, _cHero.u4Power);
    }

	public void ReloadPowerValue()
    {
        _objCharacterinfo.GetComponent<UI_Panel_CharacterInfo>().GetPanelStatus().SetCharacterInfo(_cHero);
    }

	public void OnClose_ItemList()
	{
		_trMenuGroup.SetAsLastSibling();
		_panelItemList.gameObject.SetActive(false);
		_objBackButton.SetActive(true);
		//_objPanel.SetActive(true);
		_objLevel.SetActive(true);
		//_objGoodsPanel.SetActive(true);
        _objCharacterinfo.GetComponent<UI_Panel_CharacterInfo>().SetToggleBtn();
        PopupManager.Instance.RemovePopup(_panelItemList.gameObject);
	}
	public void OnClickClose()
	{
		_trMenuGroup.SetAsLastSibling();
//		_objGoodsPanel.SetActive(true);        
		OnClickBack();
	}
	public bool OnClickBack()
	{
		bool ret=false;
		if(_panelItemList.gameObject.activeSelf)
		{
			_panelItemList.gameObject.SetActive(false);
			_acEquipSlots[selectedPosIdx].DeSelect();

			// 다른패널 끄기.
			_trMenuGroup.SetAsLastSibling();
			_panelItemInfo.gameObject.SetActive(false);
			_objBackButton.SetActive(true);
			//_objPanel.SetActive(true);
			_objLevel.SetActive(true);

//			_objGoodsPanel.SetActive(true);
			//_panelModelViewer.gameObject.SetActive(false);
            Legion.Instance.eCharState = Legion.ChangeCharInfo.CHANGED;
			ret = true;
		}
		else
		{
			UInt16[] changedSlots;
			if(_cHero == null)
				changedSlots = null;
			
			else
				changedSlots = _cHero.GetChangingEquip();
			
			//if (false)
			if (changedSlots != null)
			{
				PopupManager.Instance.ShowLoadingPopup(1);
				//_cHero.StartChangingEquip();
				SeqNo = Server.ServerMgr.Instance.WearHero(_cHero, changedSlots, OnResponseFromServer);
			}
			
			else
			{
                Legion.Instance.eCharState = Legion.ChangeCharInfo.CHANGED;
				gameObject.SetActive(false);
			}
			ret = false;
		}
		return ret;
	}
    public void CheckChangeEquip()
    {
        UInt16[] changedSlots;
		if(_cHero == null)
			changedSlots = null;
		
		else
			changedSlots = _cHero.GetChangingEquip();
		
		if (changedSlots != null)
		{
			PopupManager.Instance.ShowLoadingPopup(1);
			SeqNo = Server.ServerMgr.Instance.WearHero(_cHero, changedSlots, OnCheckEquip);
		}
        else
        {
            _panelParent.OpenInven();
        }
    }
    public void OnCheckEquip(Server.ERROR_ID err)
	{
        PopupManager.Instance.CloseLoadingPopup();
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), err.ToString(), Server.ServerMgr.Instance.CallClear);
			return;
		}
		
		else if(err == Server.ERROR_ID.NONE)
		{
			SeqNo = 0;
            _panelParent.OpenInven();
		}
	}
	public void SetData(Hero hero/*, UI_Panel_CharacterInfo parent */)
	{
        tempStringBuilder = new StringBuilder();
		_cHero = hero;

		if(_cHero.cObject == null)
		{
			_cHero.InitModelObject();
		}

		// 바꾸기전에 저장
//		_cHero.cObject.GetComponent<HeroObject>().SaveTransform();
		_cHero.cObject.SetActive(true);
		//_cHero.cObject.GetComponent<HeroObject>().SetLayer( LayerMask.NameToLayer("UI") );
        _cHero.cObject.GetComponent<HeroObject>().SetLayer( LayerMask.NameToLayer("BGMainMap") );
		// _cHero.cObject.transform.parent = transform;
//		_cHero.cObject.transform.localPosition = new Vector3(0f, -266f, -720f);
//		_cHero.cObject.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
//		_cHero.cObject.transform.localScale = new Vector3(500f, 500f, 500f);

		InitEquipSlots();

		_txtLevel.text = hero.cLevel.u2Level.ToString();
		_imgElement.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + hero.acEquips[(int)(EquipmentInfo.EQUIPMENT_POS.WEAPON_R-1)].GetEquipmentInfo().u1Element);
		_imgExpGauge.fillAmount = (float)((float)hero.cLevel.u8Exp / (float)hero.cLevel.u8NextExp);
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
		tempStringBuilder.Append("EXP ");
        tempStringBuilder.Append(ConvertExpValue(_cHero.cLevel.u8Exp)).Append(" / ").Append(ConvertExpValue(_cHero.cLevel.u8NextExp));
		_txtExp.text = tempStringBuilder.ToString();

        _cHero.StartChangingEquip();

        // 장비 재료 패키지 구매 버튼
        SetMaterialPackageBtn();

        _ChangPower.gameObject.SetActive(false);
    }


	public void InitEquipSlots()
	{
		if(_cHero == null)
			return;
		
		for(int i=0; i<_cHero.acEquips.Length; i++)
		{
			int posID = i+1;
//			DebugMgr.Log(_cHero.acEquips[i].cItemInfo.sName);

			_acEquipSlots[i].SetData(_cHero.acEquips[i]);
		}
	}

	void OnEnable()
	{
		_panelAutoEquip.gameObject.SetActive(false);
		_panelItemList.gameObject.SetActive(false);
		_panelItemInfo.gameObject.SetActive(false);
	}
	//인벤토리 닫기 서버응답 받음
	public void OnResponseFromServer(Server.ERROR_ID err)
	{
		if(err != Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
            Legion.Instance.eCharState = Legion.ChangeCharInfo.ERROR;
			PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_server_error"), err.ToString(), Server.ServerMgr.Instance.CallClear);
			return;
		}
		
		else if(err == Server.ERROR_ID.NONE)
		{
			PopupManager.Instance.CloseLoadingPopup();
			Legion.Instance.eCharState = Legion.ChangeCharInfo.CHANGED;
			SeqNo = 0;
			_cHero = null;
		}
	}

	public void OnClickAutoEquip()
	{
		_panelAutoEquip.InitEquipList(_cHero);
		_panelAutoEquip.gameObject.SetActive(true);
		_objLevel.SetActive(false);

        StopChangeEffect();

		PopupManager.Instance.AddPopup(_panelAutoEquip.gameObject, Close_AutoEquip);
	}

	public void Close_AutoEquip()
	{
		_panelAutoEquip.gameObject.SetActive(false);
		_objLevel.SetActive(true);
		PopupManager.Instance.RemovePopup(_panelAutoEquip.gameObject);
	}

    public string ConvertExpValue(UInt64 u8Exp)
    {
        string strConvertedExp = "0";

        if(u8Exp < 1000)
            return (strConvertedExp = u8Exp.ToString());

        int tempExp = (int)(Math.Log(u8Exp)/Math.Log(1000));
        strConvertedExp = String.Format("{0:F2}{1}", u8Exp/Math.Pow(1000, tempExp), "KMB".ToCharArray()[tempExp-1]);

        return strConvertedExp;
    }

    public void OpenSmithingPanel(int slotIndex)
    {
        OnClickEquipSlot(slotIndex);
        _panelItemList.OnClickSmithing();
    }

    public void SetMaterialPackageBtn()
    {
        bool isOpen = false;
        for (int i = 0; i < 3; ++i)
        {
            if (EventInfoMgr.Instance.CheckBuyPossible((UInt16)(9390 + i)) == 1)
            {
                isOpen = true;
                break;
            }
        }

        if (isOpen)
            _objMaterialPackageBtn.SetActive(true);
        else
            _objMaterialPackageBtn.SetActive(false);
    }

    public void OpenMaterialPackage()
    {
        LobbyScene lobbyScene = Scene.GetCurrent() as LobbyScene;
        if (lobbyScene != null)
        {
            lobbyScene._eventPanel.OnClickMenu(8);
        }
        else
            _objMaterialPackageBtn.SetActive(false);
    }

    public RectTransform _ChangPower;
    public Text _power;
    public Image _statusConditionIcon;
    private int checkCoroutine = 0;  // 0 안켜짐, 1 Up, 2 Down;
    private IEnumerator coroutine;
    private bool isStartChangePowerEffect = false;

    public void StartChangeEffect(UInt32 prev, UInt32 after)
    {
        coroutine = ChangeEquipHeroPowerEffect(prev, after);
        StartCoroutine(coroutine);
    }

    public void StopChangeEffect()
    {
        if (!isStartChangePowerEffect)
            return;
        
        StopCoroutine(coroutine);
        isStartChangePowerEffect = false;
        _ChangPower.gameObject.SetActive(false);
        _objPowerValue.SetActive(true);
    }

    public IEnumerator ChangeEquipHeroPowerEffect(UInt32 prev, UInt32 after)
    {
        if(prev == after)
        {
            yield break;
        }

        isStartChangePowerEffect = true;
        float fTime = 0.15f;
        //Vector3 basePos = _objPowerValue.GetComponent<RectTransform>().anchoredPosition3D;

        _power.text = prev.ToString();

        _ChangPower.anchoredPosition3D = new Vector3(0, 350);
        _ChangPower.localScale = new Vector3(1.5f, 1.5f);
        _ChangPower.gameObject.SetActive(true);
        _objPowerValue.SetActive(false);

        if (checkCoroutine == 1)
        {
            StopCoroutine("StatIconUpAni");
        }
        else if(checkCoroutine == 2)
        {
            StopCoroutine("StatIconDownAni");
        }

        if (prev < after)
        {
            _statusConditionIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.arrow_up");
            _statusConditionIcon.SetNativeSize();
            StartCoroutine("StatIconUpAni");
        }
        else
        {
            _statusConditionIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.arrow_down");
            _statusConditionIcon.SetNativeSize();
            StartCoroutine("StatIconDownAni");
        }

        long crewPowerGap = Math.Abs((long)prev - (long)after);
        int subNum = Math.Abs(crewPowerGap).ToString().Length - 1;
        if (subNum > 1)
            subNum = (int)Math.Pow(10, subNum);
        else
            subNum = 1;

        yield return new WaitForSeconds(0.2f);

        while (prev != after)
        {
            if (after > prev)
                prev += (UInt16)subNum;
            else
                prev -= (UInt16)subNum;

            crewPowerGap -= subNum;
            if (crewPowerGap < subNum && subNum != 1)
            {
                subNum = (int)(subNum * 0.1f);
            }

            _power.text = prev.ToString();
            yield return null;
        }
        _power.text = after.ToString();
        yield return new WaitForSeconds(1f);

        LeanTween.move(_ChangPower.GetComponent<RectTransform>(), _objPowerValue.GetComponent<RectTransform>().anchoredPosition3D, fTime);
        LeanTween.scale(_ChangPower, Vector3.one, fTime);
        yield return new WaitForSeconds(fTime);

        _ChangPower.gameObject.SetActive(false);
        _objPowerValue.SetActive(true);
        isStartChangePowerEffect = false;
    }
    
    private IEnumerator StatIconUpAni()
    {
        checkCoroutine = 1;
        _statusConditionIcon.rectTransform.anchoredPosition3D = Vector3.zero;
        while (true)
        {
            LeanTween.moveLocalY(_statusConditionIcon.gameObject, 7, 0.2f);
            yield return new WaitForSeconds(0.2f);
            LeanTween.moveLocalY(_statusConditionIcon.gameObject, 0, 0.2f);
            yield return new WaitForSeconds(0.2f);
        }
    }
    private IEnumerator StatIconDownAni()
    {
        checkCoroutine = 2;
        _statusConditionIcon.rectTransform.anchoredPosition3D = Vector3.zero;
        while (true)
        {
            LeanTween.moveLocalY(_statusConditionIcon.gameObject, -7, 0.2f);
            yield return new WaitForSeconds(0.2f);
            LeanTween.moveLocalY(_statusConditionIcon.gameObject, 0, 0.2f);
            yield return new WaitForSeconds(0.2f);
        }
    }
}
