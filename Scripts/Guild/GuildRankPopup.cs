using UnityEngine;
using System;
using System.Collections;

public class GuildRankPopup : MonoBehaviour
{
    [SerializeField] GameObject _scrollList;
    [SerializeField] GameObject objDetailPopup;

    private void OnEnable()
    {
        PopupManager.Instance.AddPopup(this.gameObject, OnClickBack);
        InitList();
    }

    public void InitList()
    {
        for(int i=0; i<GuildInfoMgr.Instance.u1GuildRankCount; i++)
        {
            GameObject tempObj = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Guild/RankSlot_.prefab", typeof(GameObject)) as GameObject);
            tempObj.transform.SetParent(_scrollList.transform);
            tempObj.transform.localPosition = Vector3.zero;
            tempObj.transform.localScale = Vector3.one;
            tempObj.GetComponent<GuildRankSlot>().SetData(GuildInfoMgr.Instance.lstGuildRank[i], this);
        }
    }

    public void ShowDetail()
    {
        objDetailPopup.SetActive(true);
    }

    public void OnClickBack()
    {
        for(int i=0; i<_scrollList.transform.GetChildCount(); i++)
            Destroy(_scrollList.transform.GetChild(i).gameObject);
        if(objDetailPopup.active)
            objDetailPopup.SetActive(false);
        PopupManager.Instance.RemovePopup(this.gameObject);
        this.gameObject.SetActive(false);
    }
}
