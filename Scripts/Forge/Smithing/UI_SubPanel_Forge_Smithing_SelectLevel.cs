using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UI_SubPanel_Forge_Smithing_SelectLevel : MonoBehaviour {
	[SerializeField] RectTransform _trSelectLevelListParent;
	ForgeInfo _cForgeInfo;
	UI_Panel_Forge_Smithing_Detail _cParent;
	Color32 selectLevelDisableColor = new Color32(58,74,98,255);
	Color32 selectLevelEnableColor = new Color32(114,124,136,255);
	Color32 selectLevelPressedColor = new Color32(255,255,255,255);
	public void SetData(ForgeInfo forgeInfo, UI_Panel_Forge_Smithing_Detail parent)
	{
		_cForgeInfo = forgeInfo;
		_cParent = parent;
		GameObject listElement = null;
		RectTransform trListElement = null;

		// 2016. 10. 23 jy
		for(int i = _trSelectLevelListParent.childCount; i < Legion.Instance.u1ForgeLevel; i++)
		{
			listElement = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Equipment/Pref_UI_ListElement_Forge_Smithing_Level.prefab", typeof(GameObject))) as GameObject;
			trListElement = listElement.GetComponent<RectTransform>();
			trListElement.SetParent(_trSelectLevelListParent);
			trListElement.localScale = Vector3.one;
			trListElement.localPosition = Vector3.zero;
			int level= i+1;
			listElement.GetComponent<Button>().onClick.AddListener( () => OnClickLevel(level) );
            Image icon = trListElement.FindChild("Image").FindChild("Icon").GetComponent<Image>();
            icon.sprite = AtlasMgr.Instance.GetSprite(string.Format("Sprites/Forge/Forge_01.Forge_01_LevelIcon_{0:00}", level));
            icon.SetNativeSize();
            trListElement.FindChild("Label").GetComponent<Text>().text = TextManager.Instance.GetText(ForgeInfoMgr.Instance.GetList()[i].sName);
		}

		for(int i=0; i<_trSelectLevelListParent.childCount; i++)
		{
			trListElement = _trSelectLevelListParent.GetChild(i).GetComponent<RectTransform>();
			Text txtName = trListElement.FindChild("Label").GetComponent<Text>();
			GameObject objImage = trListElement.FindChild("Image").gameObject;
			GameObject objLockIcon = trListElement.FindChild("Lock").gameObject;
			if(i < Legion.Instance.u1ForgeLevel)
			{
				txtName.color = selectLevelEnableColor;
				objLockIcon.SetActive(false);
				objImage.SetActive(true);
				trListElement.GetComponent<Button>().enabled = true;
			}
			else
			{
				txtName.color = selectLevelDisableColor;
				objLockIcon.SetActive(true);
				objImage.SetActive(false);
				trListElement.GetComponent<Button>().enabled = false;
			}
		}
        //임시 대장간 레벨 제한
        //if(forgeInfo.u1Level > 5)
        //    OnClickLevel(5);
        //else
		    OnClickLevel(forgeInfo.u1Level);
		//_trSelectLevelListParent.sizeDelta = new Vector2(426, 71*_trSelectLevelListParent.childCount);
		
	}

	public Byte _u1SelectedLevel=1;
	public void OnClickLevel(int level)
	{
		_trSelectLevelListParent.GetChild(_u1SelectedLevel-1).FindChild("Select").gameObject.SetActive(false);
		_trSelectLevelListParent.GetChild(level-1).FindChild("Select").gameObject.SetActive(true);
		_trSelectLevelListParent.GetChild(_u1SelectedLevel-1).FindChild("Label").GetComponent<Text>().color = selectLevelEnableColor;
		_trSelectLevelListParent.GetChild(level-1).FindChild("Label").GetComponent<Text>().color = selectLevelPressedColor;

//		_trSelectLevelListParent.GetChild(_nSelectedLevel-1).FindChild("Label").GetComponent<Text>().color = selectLevelEnableColor;
//		_trSelectLevelListParent.GetChild(level-1).FindChild("Label").GetComponent<Text>().color = selectLevelPressedColor;
		_u1SelectedLevel = (Byte)level;
	}

	public void OnClickConfirm()
	{
		_cParent.ChangeLevel(_u1SelectedLevel);
		gameObject.SetActive(false);
	}
}
