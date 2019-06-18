using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UI_Shop_Gacha_Result_Effect : MonoBehaviour
{
    [SerializeField] GameObject _singleEquipPanel;
    [SerializeField] GameObject _multiEquipPanel;
    [SerializeField] GameObject _materialPanel;
    [SerializeField] Text _txtTier;
    [SerializeField] Text _txtEquipmentName;
    [SerializeField] Text _txtEquipmentName_Top;
    [SerializeField] Text _txtClassName;
    [SerializeField] RectTransform _trNameGroup;
    [SerializeField] RectTransform _trStarGroup;

    [SerializeField] Transform _trEquipModelParent;
	[SerializeField] SpriteRenderer _imgAccessory;
	[SerializeField] Image _imgAccessoryUI;
	[SerializeField] Transform _trEquipEffectParent;
    [SerializeField] UI_Button_CharacterInfo_Equipment_StateInfo _specializeBtn;
    [SerializeField] GameObject starPos;
    [SerializeField] Button TouchArea;
    [SerializeField] Image BG;

    //[SerializeField] GameObject Pref_EquipmentItemSlot;
    //[SerializeField] GameObject Pref_MaterialItemSlot;

    [SerializeField] GameObject _gridEquipment;
    [SerializeField] GameObject _gridMaterial;
    [SerializeField] UI_Result_Gacha_ItemWindow _equipItemWindow;
    [SerializeField] ItemInfoWindow _materialItemWindow;
    [SerializeField] Text _txtRetryPrice;
    [SerializeField] Text discountText;
    public GameObject Btn_Retry;
    [SerializeField] Button _retry;

    GameObject _objEquipModel;
	GameObject _objEquipEffect;
    GameObject _cParentObj;

	EquipmentItem _cEquipItem;
    EquipmentItem[] _cEquipItems;
    ShopItem[] _cShopItems;
    MaterialItem[] _cMaterialItems;
	ConsumableItem[] _cConsumableItem;

    Byte u1GachaType = 0;               //1 = 장비1개, 2 = 장비4개, 3 = 재료
    UInt16 _u2ShopID;

	public void SetData(EquipmentItem equipItem, Byte forgeLevel, GameObject _parentObj)
	{
        //PopupManager.Instance.AddPopup(this.gameObject, RemoveFromPopupManager);
        PopupManager.Instance.showLoading = true;
        if (Scene.GetCurrent().shopPanel != null && Scene.GetCurrent().shopPanel.gameObject.active)
        {
            _singleEquipPanel.SetActive(true);
            _multiEquipPanel.SetActive(false);
            _materialPanel.SetActive(false);
            _singleEquipPanel.GetComponent<Animator>().enabled = true;
        }
        else
            StartCoroutine(SingleEquipmentSlotAni());
		_cEquipItem = equipItem;
        u1GachaType = 1;
        _cParentObj = _parentObj;

        _txtTier.text = TextManager.Instance.GetText("forge_level_" + forgeLevel);
		UIManager.Instance.SetGradientFromTier(_txtTier.GetComponent<Gradient>(), forgeLevel);
		_txtEquipmentName.text = TextManager.Instance.GetText( equipItem.GetEquipmentInfo().sName);
        _txtEquipmentName.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
		UIManager.Instance.SetGradientFromElement(	_txtEquipmentName.GetComponent<Gradient>(), equipItem.GetEquipmentInfo().u1Element );
		UIManager.Instance.SetSizeTextGroup(_trNameGroup, 18);
        _txtEquipmentName_Top.text = TextManager.Instance.GetText("equip_grade_" + (_cEquipItem.u1Completeness) + "star");
        _specializeBtn.SetData((Byte)(_cEquipItem.GetEquipmentInfo().u1Specialize+2));
        Color tempColor;
        ColorUtility.TryParseHtmlString(_cEquipItem.GetEquipmentInfo().GetHexColor((Byte)(_cEquipItem.GetEquipmentInfo().u1Specialize+2)), out tempColor);
        _specializeBtn.transform.parent.GetComponent<Image>().color = tempColor;
        if(_cEquipItem.GetEquipmentInfo().u2ClassID <= ClassInfo.LAST_CLASS_ID)
        {
            _txtClassName.text = TextManager.Instance.GetText(ClassInfoMgr.Instance.GetInfo(_cEquipItem.GetEquipmentInfo().u2ClassID).sName);
        }
        else
        {
            _txtClassName.text = TextManager.Instance.GetText("equip_common");
        }

        for(int i=0; i<starPos.transform.GetChildCount(); i++)
            starPos.transform.GetChild(i).gameObject.SetActive(false);
        for(int i=0; i<_cEquipItem.u1Completeness; i++)
        {
            starPos.transform.GetChild(i).gameObject.SetActive(true);
            UIManager.Instance.SetGradientFromTier(starPos.transform.GetChild(i).GetComponent<Gradient>(), forgeLevel);
        }
        starPos.GetComponent<GridLayoutGroup>().SetLayoutHorizontal();

		if(_cEquipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 && _cEquipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
		{
			_trEquipModelParent.gameObject.SetActive(true);
            if(_cEquipItem.cObject != null)
			    _objEquipModel = Instantiate(_cEquipItem.cObject);
            else
            {
                _cEquipItem.InitViewModelObject();
                _objEquipModel = _cEquipItem.cObject;
            }
			_objEquipModel.GetComponent<EquipmentObject>().SetLayer( LayerMask.NameToLayer("UI") );
			_objEquipModel.transform.SetParent( _trEquipModelParent );
			_trEquipModelParent.GetComponent<RotateCharacter>().characterTransform = _objEquipModel.transform;
			_objEquipModel.transform.localScale = Vector3.one;
			_objEquipModel.transform.localPosition = new Vector3(0f, -150f, 0f);

			_imgAccessory.gameObject.SetActive(false);
			_imgAccessoryUI.gameObject.SetActive(false);
		}
		else
		{
			_trEquipModelParent.gameObject.SetActive(false);
			_imgAccessory.sprite = AssetMgr.Instance.AssetLoad("Sprites/Item/Accessory/acc_" + equipItem.GetEquipmentInfo().u2ModelID + ".png", typeof(Sprite)) as Sprite;
			_imgAccessory.gameObject.SetActive(true);
			_imgAccessoryUI.gameObject.SetActive(false);
		}

		if(_objEquipEffect != null) DestroyObject(_objEquipEffect);
		if(forgeLevel >= 1)
		{
			DebugMgr.Log("Asset : " +string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}", forgeLevel));
			_objEquipEffect = Instantiate(AssetMgr.Instance.AssetLoad(string.Format("Prefabs/Weapon_Effects/UI_Eff_Weapon_{0:D2}.prefab", forgeLevel), typeof(GameObject))) as GameObject;
			_objEquipEffect.transform.SetParent(_trEquipEffectParent);
			_objEquipEffect.transform.name = "WeaponEffect";
			_objEquipEffect.transform.localScale = Vector3.one;
			_objEquipEffect.transform.localPosition = Vector3.zero;
		}
	}
    IEnumerator SingleEquipmentSlotAni()
    {
        TouchArea.interactable = false;
        LeanTween.alpha(TouchArea.GetComponent<RectTransform>(), 1f, 1f);
        yield return new WaitForSeconds(7f);
        LeanTween.alpha(TouchArea.GetComponent<RectTransform>(), 0.8f, 1f);
        yield return new WaitForSeconds(0.5f);
        TouchArea.interactable = true;
        _singleEquipPanel.SetActive(true);
        _multiEquipPanel.SetActive(false);
        _materialPanel.SetActive(false);
        _singleEquipPanel.GetComponent<Animator>().enabled = true;
    }
    public void SetData(EquipmentItem[] equipmentItems, GameObject _parentObj)
	{
        //PopupManager.Instance.AddPopup(this.gameObject, RemoveFromPopupManager);
        PopupManager.Instance.showLoading = true;
        _singleEquipPanel.SetActive(false);
        _multiEquipPanel.SetActive(true);
        _materialPanel.SetActive(false);
        _cEquipItems = equipmentItems;
        _cParentObj = _parentObj;
        u1GachaType = 2;
        Btn_Retry.SetActive(false);
        TouchArea.interactable = false;
        for(int i=0; i<_cEquipItems.Length; i++)
        {
            GameObject _equipmentSlot = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Shop/Result_Equipment_ItemSlot.prefab", typeof(GameObject)) as GameObject);
            _equipmentSlot.GetComponent<RectTransform>().SetParent(_gridEquipment.transform);
            _equipmentSlot.transform.localScale = Vector3.one;
            _equipmentSlot.transform.localPosition = Vector3.zero;
            _equipmentSlot.GetComponent<UI_Result_Equipment_ItemSlot>().SetData(_cEquipItems[i], _equipItemWindow);
        }
        LeanTween.alpha(TouchArea.GetComponent<RectTransform>(), 1f, 1f);
        StartCoroutine("EquipmentSlotAni2");
    }
    IEnumerator EquipmentSlotAni2()
    {
        yield return new WaitForSeconds(7f);
        LeanTween.alpha(TouchArea.GetComponent<RectTransform>(), 0.8f, 1f);
        yield return new WaitForSeconds(0.5f);
        for(int i=0; i<_gridEquipment.transform.GetChildCount(); i++)
        {
            _gridEquipment.transform.GetChild(i).GetComponent<UI_Result_Equipment_ItemSlot>().StartAnimations();
        }
        yield return new WaitForSeconds(2f);
        for(int i=0; i<_gridEquipment.transform.GetChildCount(); i++)
        {
            _gridEquipment.transform.GetChild(i).GetComponent<UI_Result_Equipment_ItemSlot>().StartAnimations2();
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.6f);
        PopupManager.Instance.showLoading = false;
        TouchArea.interactable = true;
    }
    public void SetData(ShopItem[] shopItems, GameObject _parentObj)
	{
        //PopupManager.Instance.AddPopup(this.gameObject, RemoveFromPopupManager);
        PopupManager.Instance.showLoading = true;
        _singleEquipPanel.SetActive(false);
        _multiEquipPanel.SetActive(true);
        _materialPanel.SetActive(false);
        _cShopItems = shopItems;
        _cParentObj = _parentObj;
        u1GachaType = 2;
        _u2ShopID = Scene.GetCurrent().shopPanel.tabs[2].GetComponent<ShopTab>().GetGachaResult().GetShopID();
        _txtRetryPrice.text = ShopInfoMgr.Instance.getGachaDiscount(_u2ShopID).ToString();
        discountText.text = string.Format("{0}"+TextManager.Instance.GetText("popup_desc_gacha_discount"), ShopInfoMgr.Instance.dicShopGoodData[_u2ShopID].u1Discount + LegionInfoMgr.Instance.GetCurrentVIPInfo().u1VIPGachaPer);
        TouchArea.interactable = false;
        for(int i=0; i<_cShopItems.Length; i++)
        {
            GameObject _equipmentSlot = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Shop/Result_Equipment_ItemSlot.prefab", typeof(GameObject)) as GameObject);
            _equipmentSlot.GetComponent<RectTransform>().SetParent(_gridEquipment.transform);
            _equipmentSlot.transform.localScale = Vector3.one;
            _equipmentSlot.transform.localPosition = Vector3.zero;
            _equipmentSlot.GetComponent<UI_Result_Equipment_ItemSlot>().SetData(_cShopItems[i], _equipItemWindow);
        }

        StartCoroutine(EquipmentSlotAni());
    }

    IEnumerator EquipmentSlotAni()
    {
        //for(int i=0; i<_gridEquipment.transform.GetChildCount(); i++)
        //{
        //    LeanTween.value(_gridEquipment.transform.GetChild(i).gameObject, 0f, 1f, 0.5f).setOnUpdate((float val)=> {_gridEquipment.transform.GetChild(i).GetComponent<CanvasGroup>().alpha = val;});
        //    yield return new WaitForSeconds(0.2f);
        //}

        yield return new WaitForSeconds(1f);
        for(int i=0; i<_gridEquipment.transform.GetChildCount(); i++)
        {
            _gridEquipment.transform.GetChild(i).GetComponent<UI_Result_Equipment_ItemSlot>().StartAnimations();
        }
        yield return new WaitForSeconds(2f);
        for(int i=0; i<_gridEquipment.transform.GetChildCount(); i++)
        {
            _gridEquipment.transform.GetChild(i).GetComponent<UI_Result_Equipment_ItemSlot>().StartAnimations2();
            yield return new WaitForSeconds(0.5f);
        }
        LeanTween.value(Btn_Retry, 0f, 1f, 0.5f).setOnUpdate((float alpha) => {Btn_Retry.GetComponent<CanvasGroup>().alpha = alpha; });
        yield return new WaitForSeconds(0.6f);
        PopupManager.Instance.showLoading = false;
        _retry.interactable = true;
        TouchArea.interactable = false;
    }

    public void SetData(MaterialItem[] materialItems, GameObject _obj)
    {
        //PopupManager.Instance.AddPopup(this.gameObject, RemoveFromPopupManager);
        PopupManager.Instance.showLoading = true;
        _singleEquipPanel.SetActive(false);
        _multiEquipPanel.SetActive(false);
        _materialPanel.SetActive(false);
        BG.gameObject.SetActive(false);
        TouchArea.GetComponent<Image>().enabled = false;
        _cMaterialItems = materialItems;
        _cParentObj = _obj;
        u1GachaType = 3;

        StartCoroutine(MaterialAni());
    }

    IEnumerator MaterialAni()
    {
        yield return new WaitForSeconds(7f);
        _materialPanel.SetActive(true);
        for(int i=0; i<_cMaterialItems.Length; i++)
        {
            GameObject _materialSlot = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Shop/Result_Material_ItemSlot.prefab", typeof(GameObject)) as GameObject);
            _materialSlot.GetComponent<RectTransform>().SetParent(_gridMaterial.transform);
            _materialSlot.transform.localScale = Vector3.one;
            _materialSlot.transform.localPosition = Vector3.zero;
            _materialSlot.GetComponent<UI_Result_Material_ItemSlot>().SetData(_cMaterialItems[i], _materialItemWindow);
        }
    }

	public void SetData(ConsumableItem[] consumableItem, GameObject _obj)
	{
		//PopupManager.Instance.AddPopup(this.gameObject, RemoveFromPopupManager);
		PopupManager.Instance.showLoading = true;
		_singleEquipPanel.SetActive(false);
		_multiEquipPanel.SetActive(false);
		_materialPanel.SetActive(false);
		BG.gameObject.SetActive(false);
		TouchArea.GetComponent<Image>().enabled = false;
		_cConsumableItem = consumableItem;
		_cParentObj = _obj;
		u1GachaType = 3;

		StartCoroutine(ConsumableAni());
	}

    IEnumerator ConsumableAni()
    {
        yield return new WaitForSeconds(7f);
        _materialPanel.SetActive(true);
        for(int i=0; i<_cConsumableItem.Length; i++)
		{
			GameObject _materialSlot = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Shop/Result_Material_ItemSlot.prefab", typeof(GameObject)) as GameObject);
			_materialSlot.GetComponent<RectTransform>().SetParent(_gridMaterial.transform);
			_materialSlot.transform.localScale = Vector3.one;
			_materialSlot.transform.localPosition = Vector3.zero;
			_materialSlot.GetComponent<UI_Result_Material_ItemSlot>().SetData(_cConsumableItem[i], _materialItemWindow);
		}
    }

    public void OnClickTouch()
    {
        //_cParent._scriptSmithingResult.gameObject.SetActive(true);
        //Destroy(this.gameObject);
        TouchArea.interactable = false;
        switch(u1GachaType)
        {
            case 1:
                StartCoroutine(TouchAniPlay());
                break;

            case 2:
                StartCoroutine(TouchAniPlayEquipments());
                break;

            case 3:
                StartCoroutine(TouchAniPlayMaterial());
                break;
        }
    }

    //장비 1개 뽑기=====================================================
    IEnumerator TouchAniPlay()
    {
        _singleEquipPanel.GetComponent<Animator>().enabled = false;
        LeanTween.scale(_txtEquipmentName_Top.transform.parent.GetComponent<RectTransform>(), Vector2.zero, 0.1f);
        yield return new WaitForSeconds(0.5f);
        bDestroy = true;
        RemoveFromPopupManager();
        _cParentObj.SetActive(true);
        _cParentObj.GetComponent<CanvasGroup>().alpha = 1;
        //Destroy(this.gameObject);
    }

    public void CallCharacterTalk()
    {
        //PopupManager.Instance.showLoading = false;
        TouchArea.interactable = true;
        if(Legion.Instance.cTutorial.au1Step [8] >= Server.ConstDef.LastTutorialStep)
            PopupManager.Instance.SetNoticePopup (MENU.SHOP, 3, _cEquipItem.u1Completeness, this);
        else
            StartCoroutine(TutorialEffectEnd());
    }

    IEnumerator TutorialEffectEnd()
    {
        yield return new WaitForSeconds(1f);
        OnClickTouch();
    }
    //===================================================================

    //장비 2개 이상 뽑기=================================================
    IEnumerator TouchAniPlayEquipments()
    {
        //_multiEquipPanel.GetComponent<Animator>().enabled = false;
        yield return new WaitForSeconds(0.0f);

        SocialMailGachaResult smgr = null;
        if(_cParentObj != null)
            smgr = _cParentObj.GetComponent<SocialMailGachaResult>();
        // [ 예외처리 ]
        BaseScene scene = Scene.GetCurrent();
        if (scene != null)
        {
            if (scene.shopPanel != null && scene.shopPanel.gameObject.activeSelf)
            {
                ShopTab shopTab = scene.shopPanel.GetShopTab(2);
                if(shopTab != null)
                    shopTab.GetGachaResult().Close();
            }
        }

        if (smgr != null)
			smgr.Close();
        //_cParentObj.SetActive(true);
        //_cParentObj.GetComponent<CanvasGroup>().alpha = 1;
        bDestroy = true;
        RemoveFromPopupManager();
        //Destroy(this.gameObject);
    }
    //===================================================================
    
    //재료 뽑기==========================================================
    IEnumerator TouchAniPlayMaterial()
    {
        //_materialPanel.GetComponent<Animator>().enabled = false;
        yield return new WaitForSeconds(0.0f);
        if(_cParentObj.GetComponent<SocialMailGachaResult>() != null)
            _cParentObj.GetComponent<SocialMailGachaResult>().Close();
        bDestroy = true;
        RemoveFromPopupManager();
        //Destroy(this.gameObject);
    }
    //===================================================================

    public void OnClickRetryButton()
    {
        if(!Legion.Instance.CheckEnoughGoods(ShopInfoMgr.Instance.dicShopGoodData[_u2ShopID].cBuyGoods.u1Type, ShopInfoMgr.Instance.getGachaDiscount(_u2ShopID)))
        {
			PopupManager.Instance.ShowChargePopup(ShopInfoMgr.Instance.dicShopGoodData[_u2ShopID].cBuyGoods.u1Type);
            return;
        }
        Scene.GetCurrent().shopPanel.tabs[2].GetComponent<ShopTab>().GetGachaResult().OnClickGachaRetry();
        bDestroy = true;
        RemoveFromPopupManager();
        //Destroy(this.gameObject);
    }
    bool bDestroy = false;
    void RemoveFromPopupManager()
    {
        PopupManager.Instance.RemovePopup(this.gameObject);
        PopupManager.Instance.showLoading = false;
        if(bDestroy)
            Destroy(this.gameObject);
    }

    public void CloseStarGradeAni()
    {
        _singleEquipPanel.GetComponent<Animator>().enabled = false;
        LeanTween.scale(_txtEquipmentName_Top.transform.parent.GetComponent<RectTransform>(), Vector2.zero, 0.1f);
    }
}
