using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
public class UI_SubPanel_Forge_Smithing_MaterialShortcut : MonoBehaviour {
	[SerializeField] RectTransform _trNameGroup;
	[SerializeField] Image _imgGrade;
	[SerializeField] Image _imgItemIcon;
	[SerializeField] Text _txtItemName;
	[SerializeField] RectTransform _trListParent;
	MaterialItemInfo _cItemInfo;
	public void SetData(MaterialItemInfo itemInfo)
	{
		_cItemInfo = itemInfo;
		_imgGrade.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + itemInfo.u2Grade);
		_imgItemIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + itemInfo.u2IconID);
		_txtItemName.text = "<" + TextManager.Instance.GetText( itemInfo.sName ) + ">";
		for(int i=0; i<_trListParent.childCount; i++)
		{
			DestroyObject( _trListParent.GetChild(i).gameObject );
		}

		for(Byte i=0; i<itemInfo.u1GetLocationCount; i++)
		{
			GameObject listElement = Instantiate( AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Equipment/Pref_UI_ListElement_Forge_Smithing_Material_Shortcut.prefab", typeof(GameObject))) as GameObject;
			listElement.GetComponent<RectTransform>().SetParent( _trListParent );
			listElement.transform.localScale = Vector3.one;
			listElement.transform.localPosition = Vector3.zero;
            listElement.GetComponent<UI_ListElement_Forge_Smithing_MaterialShortcut>().SetData(itemInfo, i);
        }

		foreach (EventDungeonShopInfo tData in EventInfoMgr.Instance.dicDungeonShop.Values) {
			if (tData.u1UIType == 1){
				EventDungeonStageInfo dungeonInfo = EventInfoMgr.Instance.lstDungeonStage.Find (cs => cs.u2EventID == tData.u2EventID);
				List<UInt16> stages = dungeonInfo.IsRewardMaterialStages (itemInfo.u2ID);
				if (stages.Count > 0) {
					for (int i = 0; i < stages.Count; i++) {
						GameObject listElement = Instantiate (AssetMgr.Instance.AssetLoad ("Prefabs/UI/CharacterInfo/Equipment/Pref_UI_ListElement_Forge_Smithing_Material_Shortcut.prefab", typeof(GameObject))) as GameObject;
						listElement.GetComponent<RectTransform> ().SetParent (_trListParent);
						listElement.transform.localScale = Vector3.one;
						listElement.transform.localPosition = Vector3.zero;
						listElement.GetComponent<UI_ListElement_Forge_Smithing_MaterialShortcut> ().SetData (stages[i], tData);
					}
				}
			}
		}

		foreach (LeagueInfo tData in LeagueInfoMgr.Instance.dicLeagueData.Values) {
			if (tData.IsRewardMaterial (itemInfo.u2ID)) {
				GameObject listElement = Instantiate( AssetMgr.Instance.AssetLoad("Prefabs/UI/CharacterInfo/Equipment/Pref_UI_ListElement_Forge_Smithing_Material_Shortcut.prefab", typeof(GameObject))) as GameObject;
				listElement.GetComponent<RectTransform>().SetParent( _trListParent );
				listElement.transform.localScale = Vector3.one;
				listElement.transform.localPosition = Vector3.zero;
				listElement.GetComponent<UI_ListElement_Forge_Smithing_MaterialShortcut>().SetData(tData.u2LeagueID-4500);
			}
		}
			
		for (int i=0; i<EventInfoMgr.Instance.lstBossRush.Count; i++) {
			for (int j = 0; j <EventInfoMgr.Instance.lstBossRush [i].rewardGoods.Length; j++) {
				if (EventInfoMgr.Instance.lstBossRush [i].rewardGoods[j].u2ID == itemInfo.u2ID) {
					GameObject listElement = Instantiate (AssetMgr.Instance.AssetLoad ("Prefabs/UI/CharacterInfo/Equipment/Pref_UI_ListElement_Forge_Smithing_Material_Shortcut.prefab", typeof(GameObject))) as GameObject;
					listElement.GetComponent<RectTransform> ().SetParent (_trListParent);
					listElement.transform.localScale = Vector3.one;
					listElement.transform.localPosition = Vector3.zero;
					listElement.GetComponent<UI_ListElement_Forge_Smithing_MaterialShortcut> ().SetDataForBossRush ();
					break;
				}
			}
		}

		// 슬롯이 2개 보다 많으면 스크롤 영역의 크기를 키운다
		if(itemInfo.u1GetLocationCount > 2)
		{
			int count = itemInfo.u1GetLocationCount / 2;
			if(itemInfo.u1GetLocationCount % 2 != 0)
				++count;
			
			Vector2 size = _trListParent.sizeDelta;
			size.y = _trListParent.GetComponent<GridLayoutGroup>().cellSize.y * count;
			_trListParent.sizeDelta = size;
		}
		UIManager.Instance.SetSizeTextGroup(_trNameGroup);
	}
}
