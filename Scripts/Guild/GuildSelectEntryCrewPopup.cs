using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class GuildSelectEntryCrewPopup : MonoBehaviour
{
    [SerializeField] Text txtCrewName;

    [SerializeField] GameObject objCrewList;

    private int slotIndex = 0;
    public int SlotIndex
    {
        set
        {
            slotIndex = value;
        }
        get
        {
            return slotIndex;
        }
    }
    public GuildPanel _parent;
    StringBuilder tempStringBuilder;

    private void OnEnable()
    {
        PopupManager.Instance.AddPopup(this.gameObject, OnClickClose);
        tempStringBuilder = new StringBuilder();
        StartCoroutine(DelayedInit());
    }

    IEnumerator DelayedInit()
    {
        if(objCrewList.transform.GetChildCount() != 0)
        {
            for(int i=0; i<objCrewList.transform.GetChildCount(); i++)
            {
                Destroy(objCrewList.transform.GetChild(i).gameObject);
            }
        }
        yield return new WaitForEndOfFrame();
        InitData();
    }

    private void InitData()
    {
        txtCrewName.text = Legion.Instance.sName;
        for(int i=0; i<GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember.Count; i++)
        {
            GameObject tempSlot = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Guild/GuildEntrySlot_.prefab", typeof(GameObject)) as GameObject);
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append("GuildEntrySlot_").Append(i);
            tempSlot.name = tempStringBuilder.ToString();
            tempSlot.transform.SetParent(objCrewList.transform);
            tempSlot.transform.localPosition = Vector3.zero;
            tempSlot.transform.localScale = Vector3.one;
            tempSlot.GetComponent<GuildSelectEntryCrewSlot>().SetData(GuildInfoMgr.Instance.cGuildMemberInfo.lstGuildMember[i], SlotIndex, this);
        }
    }

    public void OnClickClose()
    {
        if(_parent != null)
        {
            if(_parent.MASTER_ENTRY)
                _parent.OnClickGuildEntryToggle();
            else
                _parent.OnClickEntryToggle();
        }
        PopupManager.Instance.RemovePopup(this.gameObject);
        this.gameObject.SetActive(false);
    }
}
