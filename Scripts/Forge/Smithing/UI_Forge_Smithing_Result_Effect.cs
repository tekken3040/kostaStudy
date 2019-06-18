using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UI_Forge_Smithing_Result_Effect : MonoBehaviour
{
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

    GameObject _objEquipModel;
	GameObject _objEquipEffect;

	EquipmentItem _cEquipItem;
    UI_Forge_Smithing_module _cParent2;

    public void SetData(EquipmentItem equipItem, Byte forgeLevel, UI_Forge_Smithing_module _parent)
	{
        TouchArea.interactable = false;
		_cEquipItem = equipItem;
        _cParent2 = _parent;
		//_txtTier.text = "<" + TextManager.Instance.GetText("forge_level_" + forgeLevel) + ">";
		_txtTier.text = TextManager.Instance.GetText("forge_level_" + forgeLevel);
		UIManager.Instance.SetGradientFromTier(_txtTier.GetComponent<Gradient>(), forgeLevel);
		_txtEquipmentName.text = TextManager.Instance.GetText( equipItem.GetEquipmentInfo().sName);
        _txtEquipmentName.GetComponent<ContentSizeFitter>().SetLayoutHorizontal();
		UIManager.Instance.SetGradientFromElement(	_txtEquipmentName.GetComponent<Gradient>(), equipItem.GetEquipmentInfo().u1Element );
		UIManager.Instance.SetSizeTextGroup(_trNameGroup, 18);
        _txtEquipmentName_Top.text = TextManager.Instance.GetText("equip_grade_" + _cEquipItem.u1Completeness + "star");
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
            UIManager.Instance.SetGradientFromTier(starPos.transform.GetChild(i).GetComponent<Gradient>(), _cEquipItem.u1SmithingLevel);
        }
        starPos.GetComponent<GridLayoutGroup>().SetLayoutHorizontal();

		if(equipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_1 && equipItem.GetEquipmentInfo().u1PosID != EquipmentInfo.EQUIPMENT_POS.ACCESSORY_2)
		{
			_trEquipModelParent.gameObject.SetActive(true);
			//equipItem.InitViewModelObject();
			//equipItem.cObject.GetComponent<EquipmentObject>().SetLayer( LayerMask.NameToLayer("UI") );
			//equipItem.cObject.transform.SetParent( _trEquipModelParent );
			//_trEquipModelParent.GetComponent<RotateCharacter>().characterTransform = equipItem.cObject.transform;
			//equipItem.cObject.transform.localScale = Vector3.one;
			//equipItem.cObject.transform.localPosition = new Vector3(0f, -150f, 0f);
			_objEquipModel = Instantiate(equipItem.cObject);
			_objEquipModel.GetComponent<EquipmentObject>().SetLayer( LayerMask.NameToLayer("UI") );
			_objEquipModel.transform.SetParent( _trEquipModelParent );
			_trEquipModelParent.GetComponent<RotateCharacter>().characterTransform = _objEquipModel.transform;
			_objEquipModel.transform.localScale = Vector3.one;
			_objEquipModel.transform.localPosition = new Vector3(0f, -150f, -400f);

			_imgAccessory.gameObject.SetActive(false);
			_imgAccessoryUI.gameObject.SetActive(false);
		}
		else
		{
			_trEquipModelParent.gameObject.SetActive(false);
			_imgAccessory.sprite = AssetMgr.Instance.AssetLoad("Sprites/Item/Accessory/acc_" + equipItem.GetEquipmentInfo().u2ModelID + ".png", typeof(Sprite)) as Sprite;
			//_imgAccessory.SetNativeSize();
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

    public void OnClickTouch()
    {
        //_cParent._scriptSmithingResult.gameObject.SetActive(true);
        //Destroy(this.gameObject);
        TouchArea.interactable = false;
        StartCoroutine(TouchAniPlay());
    }

    IEnumerator TouchAniPlay()
    {
        this.GetComponent<Animator>().enabled = false;
        LeanTween.scale(_txtEquipmentName_Top.transform.parent.GetComponent<RectTransform>(), Vector2.zero, 0.1f);
        yield return new WaitForSeconds(0.5f);

        _cParent2._scriptSmithingResult.gameObject.SetActive(true);
        Destroy(this.gameObject);

        PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.MakeEquip);
        PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.GetEquip);
        PopupManager.Instance.OdinMissionClearPopup.SetClearPopup(Legion.Instance.cQuest.u2ClearOdinMissionID, AchievementTypeData.CreateEquipGrade);
    }

    public void CallCharacterTalk()
    {
        TouchArea.interactable = true;
        if(Legion.Instance.cTutorial.au1Step [7] < Server.ConstDef.LastTutorialStep)
            StartCoroutine(TutorialEffectEnd());
        //if(Legion.Instance.cTutorial.au1Step [7] >= Server.ConstDef.LastTutorialStep)
        //    PopupManager.Instance.SetNoticePopup (MENU.FORGE, 4, _cEquipItem.u1Completeness, this);
        //else
        //    StartCoroutine(TutorialEffectEnd());
    }

    IEnumerator TutorialEffectEnd()
    {
        yield return new WaitForSeconds(1f);
        OnClickTouch();
    }

    public void CloseStarGradeAni()
    {
        this.GetComponent<Animator>().enabled = false;
        LeanTween.scale(_txtEquipmentName_Top.transform.parent.GetComponent<RectTransform>(), Vector2.zero, 0.1f);
    }
}
