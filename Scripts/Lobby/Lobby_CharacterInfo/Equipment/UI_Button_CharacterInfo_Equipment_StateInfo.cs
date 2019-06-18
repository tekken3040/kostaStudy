using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class UI_Button_CharacterInfo_Equipment_StateInfo : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] RectTransform _infoPopup;
	public Vector3 subPosition;
    public Byte _u1StateType;
    Transform _parant;
    Text _stateName;

    public void SetData(Byte stateType)
	{
		_u1StateType = stateType;
        _parant = this.transform.parent;
        _stateName = _parant.FindChild("Text").GetComponent<Text>();

        switch(_u1StateType)
        {
            case (Byte)ClassInfo.ATTACK_ELEMENT.PHYSICAL:
                _stateName.text = TextManager.Instance.GetText("phy");
                break;

            case (Byte)ClassInfo.ATTACK_ELEMENT.MAGICAL:
                _stateName.text = TextManager.Instance.GetText("mag");
                break;

            case (Byte)ClassInfo.ATTACK_ELEMENT.OFFENSIVE:
                _stateName.text = TextManager.Instance.GetText("equip_atk");
                break;

            case (Byte)ClassInfo.ATTACK_ELEMENT.DEFENSIVE:
                _stateName.text = TextManager.Instance.GetText("equip_def");
                break;

            case (Byte)ClassInfo.ATTACK_ELEMENT.BALANCE:
                _stateName.text = TextManager.Instance.GetText("equip_bal");
                break;

            case (Byte)ClassInfo.ATTACK_ELEMENT.SPECIALIZE:
                _stateName.text = TextManager.Instance.GetText("equip_tal");
                break;
        }
	}

    #region IPointerDownHandler implementation
	public void OnPointerDown (PointerEventData eventData)
	{
		if(_infoPopup != null)
		{
			_infoPopup.GetComponent<UI_SubPanel_CharacterInfo_Equipment_StateInfo>().SetData(_u1StateType);
			_infoPopup.gameObject.SetActive(true);
			_infoPopup.transform.position = transform.position;
			_infoPopup.localPosition += subPosition;
			Vector3 popPos = _infoPopup.anchoredPosition3D;
			popPos.y += _infoPopup.sizeDelta.y;
			_infoPopup.anchoredPosition3D = popPos;
		}
		else
			DebugMgr.LogError("_infoPopup null");
	}
	#endregion

	#region IPointerUpHandler implementation

	public void OnPointerUp (PointerEventData eventData)
	{
        if(_infoPopup != null)
		    _infoPopup.gameObject.SetActive(false);
	}

	#endregion
}
