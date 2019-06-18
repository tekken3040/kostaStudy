using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UI_Result_Equipment_ItemSlot : MonoBehaviour
{
    [SerializeField] Image _grade;
    [SerializeField] Image _icon;
    [SerializeField] Image _element;
    [SerializeField] Text _txtLvl;

    [SerializeField] Text _txtName;
    [SerializeField] Text _txtStarGrade;
    [SerializeField] Text _txtStarCount;
    [SerializeField] Text _txtTier;
    [SerializeField] Text _txtClassName;
    [SerializeField] UI_Button_CharacterInfo_Equipment_StateInfo _specializeBtn;
    [SerializeField] GameObject starPos;
    [SerializeField] RectTransform _trNameGroup;
    [SerializeField] RectTransform _trStarGroup;
    [SerializeField] Button _windowButton;

    UI_Result_Gacha_ItemWindow _itemWindow;
    EquipmentItem _cEquipmentItem;
    ShopItem _cShopItem;

    public void SetData(ShopItem _shopItem, UI_Result_Gacha_ItemWindow _window)
    {
        _itemWindow = _window;
        _cEquipmentItem = new EquipmentItem(_shopItem.u2ItemID);
        _cEquipmentItem.u1Completeness = _shopItem.cEquipInfo.u1Completeness;
        _cEquipmentItem.u1SmithingLevel = _shopItem.cEquipInfo.u1SmithingLevel;
        _cShopItem = _shopItem;
        _txtStarGrade.transform.parent.gameObject.SetActive(false);
        starPos.SetActive(false);

        ModelInfo modelInfo = ModelInfoMgr.Instance.GetInfo(_cEquipmentItem.GetEquipmentInfo().u2ModelID);
        string imagePath = "Sprites/Item/" + modelInfo.sImagePath;
        _icon.sprite = AtlasMgr.Instance.GetSprite(imagePath);
        _icon.SetNativeSize();

        int smithingLevel = _cEquipmentItem.u1SmithingLevel;
            
        if(smithingLevel < 1)
            smithingLevel = 1;       
            
        ForgeInfo forgeInfo = ForgeInfoMgr.Instance.GetList()[smithingLevel-1];    
        _grade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + forgeInfo.u2ID);
        _grade.SetNativeSize();

        _element.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + _cEquipmentItem.GetEquipmentInfo().u1Element);

        _txtLvl.text = _cEquipmentItem.cLevel.u2Level.ToString();
        _txtName.text = TextManager.Instance.GetText(_cEquipmentItem.GetEquipmentInfo().sName);

        _txtTier.text = TextManager.Instance.GetText("forge_level_" + smithingLevel);
		UIManager.Instance.SetGradientFromTier(_txtTier.GetComponent<Gradient>(), (Byte)smithingLevel);

        _txtName.text = TextManager.Instance.GetText( _cEquipmentItem.GetEquipmentInfo().sName);
        _txtName.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
		UIManager.Instance.SetGradientFromElement(	_txtName.GetComponent<Gradient>(), _cEquipmentItem.GetEquipmentInfo().u1Element );
		
        _txtStarGrade.text = TextManager.Instance.GetText("equip_grade_" + _cEquipmentItem.u1Completeness + "star");
        _specializeBtn.SetData((Byte)(_cEquipmentItem.GetEquipmentInfo().u1Specialize+2));
        Color tempColor;
        ColorUtility.TryParseHtmlString(_cEquipmentItem.GetEquipmentInfo().GetHexColor((Byte)(_cEquipmentItem.GetEquipmentInfo().u1Specialize+2)), out tempColor);
        _specializeBtn.transform.parent.GetComponent<Image>().color = tempColor;
        if(_cEquipmentItem.GetEquipmentInfo().u2ClassID <= ClassInfo.LAST_CLASS_ID)
        {
            _txtClassName.text = TextManager.Instance.GetText(ClassInfoMgr.Instance.GetInfo(_cEquipmentItem.GetEquipmentInfo().u2ClassID).sName);
        }
        else
        {
            _txtClassName.text = TextManager.Instance.GetText("equip_common");
        }

        for(int i=0; i<starPos.transform.GetChildCount(); i++)
            starPos.transform.GetChild(i).gameObject.SetActive(false);
        for(int i=0; i<_cEquipmentItem.u1Completeness; i++)
        {
            starPos.transform.GetChild(i).gameObject.SetActive(true);
            UIManager.Instance.SetGradientFromTier(starPos.transform.GetChild(i).GetComponent<Gradient>(), _cEquipmentItem.u1SmithingLevel);
        }
        starPos.GetComponent<GridLayoutGroup>().SetLayoutHorizontal();

        _txtStarCount.text = _cEquipmentItem.u1Completeness.ToString();
    }

    public void SetData(EquipmentItem _equipmentItem, UI_Result_Gacha_ItemWindow _window)
    {
        _itemWindow = _window;
        _cEquipmentItem = _equipmentItem;
        _txtStarGrade.transform.parent.gameObject.SetActive(false);
        starPos.SetActive(false);

        ModelInfo modelInfo = ModelInfoMgr.Instance.GetInfo(_cEquipmentItem.GetEquipmentInfo().u2ModelID);
        string imagePath = "Sprites/Item/" + modelInfo.sImagePath;
        _icon.sprite = AtlasMgr.Instance.GetSprite(imagePath);
        _icon.SetNativeSize();

        int smithingLevel = _cEquipmentItem.u1SmithingLevel;
            
        if(smithingLevel < 1)
            smithingLevel = 1;       
            
        ForgeInfo forgeInfo = ForgeInfoMgr.Instance.GetList()[smithingLevel-1];    
        _grade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + forgeInfo.u2ID);
        _grade.SetNativeSize();

        _element.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + _cEquipmentItem.GetEquipmentInfo().u1Element);

        _txtLvl.text = _cEquipmentItem.cLevel.u2Level.ToString();
        _txtName.text = TextManager.Instance.GetText(_cEquipmentItem.GetEquipmentInfo().sName);

        _txtTier.text = TextManager.Instance.GetText("forge_level_" + smithingLevel);
		UIManager.Instance.SetGradientFromTier(_txtTier.GetComponent<Gradient>(), (Byte)smithingLevel);

        _txtName.text = TextManager.Instance.GetText( _cEquipmentItem.GetEquipmentInfo().sName);
        _txtName.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
		UIManager.Instance.SetGradientFromElement(	_txtName.GetComponent<Gradient>(), _cEquipmentItem.GetEquipmentInfo().u1Element );
		
        _txtStarGrade.text = TextManager.Instance.GetText("equip_grade_" + _cEquipmentItem.u1Completeness + "star");
        _specializeBtn.SetData((Byte)(_cEquipmentItem.GetEquipmentInfo().u1Specialize+2));
        Color tempColor;
        ColorUtility.TryParseHtmlString(_cEquipmentItem.GetEquipmentInfo().GetHexColor((Byte)(_cEquipmentItem.GetEquipmentInfo().u1Specialize+2)), out tempColor);
        _specializeBtn.transform.parent.GetComponent<Image>().color = tempColor;
        if(_cEquipmentItem.GetEquipmentInfo().u2ClassID <= ClassInfo.LAST_CLASS_ID)
        {
            _txtClassName.text = TextManager.Instance.GetText(ClassInfoMgr.Instance.GetInfo(_cEquipmentItem.GetEquipmentInfo().u2ClassID).sName);
        }
        else
        {
            _txtClassName.text = TextManager.Instance.GetText("equip_common");
        }

        for(int i=0; i<starPos.transform.GetChildCount(); i++)
            starPos.transform.GetChild(i).gameObject.SetActive(false);
        for(int i=0; i<_cEquipmentItem.u1Completeness; i++)
        {
            starPos.transform.GetChild(i).gameObject.SetActive(true);
            UIManager.Instance.SetGradientFromTier(starPos.transform.GetChild(i).GetComponent<Gradient>(), _cEquipmentItem.u1SmithingLevel);
        }
        starPos.GetComponent<GridLayoutGroup>().SetLayoutHorizontal();

        _txtStarCount.text = _cEquipmentItem.u1Completeness.ToString();
    }

    public void OnClickItem()
    {
        PopupManager.Instance.AddPopup(_itemWindow.gameObject, _itemWindow.Close);
        _itemWindow.gameObject.SetActive(true);
        if(_cShopItem != null)
            _itemWindow.SetItem(_cShopItem);
        else
            _itemWindow.SetItem(_cEquipmentItem);
    }

    public void StartAnimations()
    {
        this.GetComponent<Animator>().enabled = true;
    }

    public void StartAnimations2()
    {
        _txtStarGrade.transform.parent.gameObject.SetActive(true);
        starPos.SetActive(true);
        LeanTween.scale(starPos, Vector3.one, 0.3f);
        LeanTween.scale(_txtStarGrade.transform.parent.gameObject, Vector3.one, 0.3f);
        _windowButton.interactable = true;
    }
}
