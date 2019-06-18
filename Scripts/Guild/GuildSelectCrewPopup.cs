using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GuildSelectCrewPopup : MonoBehaviour
{
    [SerializeField] GameObject[] CrewSlot;
	[SerializeField] Text CrewName;

    private void OnEnable()
    {
        PopupManager.Instance.AddPopup(this.gameObject, OnClickClose);
        Init();
    }

    public void Init()
    {
		CrewName.text = Legion.Instance.sName;
        for(int i=0; i<Legion.MAX_CREW_OF_LEGION; i++)
        {
            CrewSlot[i].GetComponent<GuildCrewSlot>().SetData((i+1), this);
        }
    }

    public void OnClickClose()
    {
		if (_cParent != null) {
			_cParent.SetGuildCrew ();
			_cParent = null;
		}
		if (_cParent2 != null) {
			_cParent2.SetGuildCrew ();
			_cParent2 = null;
		}
        PopupManager.Instance.RemovePopup(this.gameObject);
        this.gameObject.SetActive(false);
    }

    GuildPanel _cParent;
    public void SetCrewData(GuildPanel parent)
    {
        _cParent = parent;
    }
    GuildNoPanel _cParent2;
    public void SetCrewData(GuildNoPanel parent)
    {
        _cParent2 = parent;
    }
}
