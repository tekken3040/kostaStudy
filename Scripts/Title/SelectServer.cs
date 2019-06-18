using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class SelectServer : MonoBehaviour
{
    [SerializeField] Text txtServerName;
    [SerializeField] Text txtCrewName;
    [SerializeField] Text txtNew;
    [SerializeField] Text txtRecentServerName;
    [SerializeField] GameObject objList;
    [SerializeField] GameObject objTab;
    [SerializeField] GameObject objPopup;
    [SerializeField] GameObject objBtnPopupClose;
    [SerializeField] TitleScene _title;
    string strServerCode;
    string strCrewName;
    UInt16 u2ServerID;
    StringBuilder tempStringBuilder;
    void OnEnable()
    {
        Init();
    }
    public string SetServerCode(string code)
    {
        strServerCode = code;

        return strServerCode;
    }
    public string SetCrewName(string name)
    {
        strCrewName = name;

        return strCrewName;
    }
    public UInt16 SERVERID
    {
        set
        {
            u2ServerID = value;
        }
        get
        {
            return u2ServerID;
        }
    }
    public void Init()
    {
        //if(Legion.Instance.u2LastLoginServer == 0)
        //{
        //    OnClickSelectServer();
        //    return;
        //}
        tempStringBuilder = new StringBuilder();
        objTab.SetActive(true);
        if(Legion.Instance.u2LastLoginServer == 0)
        {
            txtCrewName.text = TextManager.Instance.GetText("default_legion_name");
            strCrewName = txtCrewName.text;
            for(int i=0; i<Legion.Instance.u1ServerCount; i++)
            {
                if(Legion.Instance.lstServerGroup[i].u2ServerID == Legion.Instance.u2RecommendServerID)
                {
                    if(Legion.Instance.lstServerGroup[i].u1Congestion != 3 && Legion.Instance.lstServerGroup[i].u1State == 1)
                    {
                        u2ServerID = Legion.Instance.lstServerGroup[i].u2ServerID;
                        tempStringBuilder/*.Append(Legion.Instance.lstServerGroup[i].u2ServerID).Append(". ")*/.Append(TextManager.Instance.GetText(Legion.Instance.lstServerGroup[i].strServerNameCode));
                        txtServerName.text = tempStringBuilder.ToString();
                        txtRecentServerName.text = tempStringBuilder.ToString();
                        strServerCode = Legion.Instance.lstServerGroup[i].strServerNameCode;
                        if(Legion.Instance.lstServerGroup[i].u1New == 1)
                            txtNew.gameObject.SetActive(true);
                        else
                            txtNew.gameObject.SetActive(false);
                        Legion.Instance.u2LastLoginServer = Legion.Instance.lstServerGroup[i].u2ServerID;
                        _title.OnSelectedServer();
                        break;
                    }
                }
                else
                {
                    if(TextManager.Instance.GetText(Legion.Instance.lstServerGroup[i].strServerNameCode) == "")
                    {
                        u2ServerID = Legion.Instance.lstServerGroup[i].u2ServerID;
                        tempStringBuilder/*.Append(Legion.Instance.lstServerGroup[i].u2ServerID).Append(". ")*/.Append(Legion.Instance.lstServerGroup[i].strServerNameCode);
                        txtServerName.text = tempStringBuilder.ToString();
                        txtRecentServerName.text = tempStringBuilder.ToString();
                    }
                    else
                    {
                        u2ServerID = Legion.Instance.lstServerGroup[i].u2ServerID;
                        tempStringBuilder/*.Append(Legion.Instance.lstServerGroup[i].u2ServerID).Append(". ")*/.Append(TextManager.Instance.GetText(Legion.Instance.lstServerGroup[i].strServerNameCode));
                        txtServerName.text = tempStringBuilder.ToString();
                        txtRecentServerName.text = tempStringBuilder.ToString();
                    }
                    strServerCode = Legion.Instance.lstServerGroup[i].strServerNameCode;
                    if(Legion.Instance.lstServerGroup[i].u1New == 1)
                        txtNew.gameObject.SetActive(true);
                    else
                        txtNew.gameObject.SetActive(false);
                    Legion.Instance.u2LastLoginServer = Legion.Instance.lstServerGroup[i].u2ServerID;
                    _title.OnSelectedServer();
                    break;
                }
            }
        }
        else
        {
            for(int i=0; i<Legion.Instance.u1ServerCount; i++)
            {
                tempStringBuilder.Remove(0, tempStringBuilder.Length);
                if(Legion.Instance.u2LastLoginServer == Legion.Instance.lstServerGroup[i].u2ServerID)
                {
                    if(Legion.Instance.lstServerGroup[i].strLegionName != "")
                        txtCrewName.text = Legion.Instance.lstServerGroup[i].strLegionName;
                    else
                        txtCrewName.text = TextManager.Instance.GetText("default_legion_name");
                    strCrewName = txtCrewName.text;
                    if(TextManager.Instance.GetText(Legion.Instance.lstServerGroup[i].strServerNameCode) == "")
                    {
                        u2ServerID = Legion.Instance.lstServerGroup[i].u2ServerID;
                        tempStringBuilder/*.Append(Legion.Instance.lstServerGroup[i].u2ServerID).Append(". ")*/.Append(Legion.Instance.lstServerGroup[i].strServerNameCode);
                        txtServerName.text = tempStringBuilder.ToString();
                        txtRecentServerName.text = tempStringBuilder.ToString();
                    }
                    else
                    {
                        u2ServerID = Legion.Instance.lstServerGroup[i].u2ServerID;
                        tempStringBuilder/*.Append(Legion.Instance.lstServerGroup[i].u2ServerID).Append(". ")*/.Append(TextManager.Instance.GetText(Legion.Instance.lstServerGroup[i].strServerNameCode));
                        txtServerName.text = tempStringBuilder.ToString();
                        txtRecentServerName.text = tempStringBuilder.ToString();
                    }
                    strServerCode = Legion.Instance.lstServerGroup[i].strServerNameCode;
                    if(Legion.Instance.lstServerGroup[i].u1New == 1)
                        txtNew.gameObject.SetActive(true);
                    else
                        txtNew.gameObject.SetActive(false);
                    break;
                }
            }
        }
    }

    public void RefreashList()
    {
		for(int i=0; i<objList.transform.childCount; i++)
        {
            objList.transform.GetChild(i).GetComponent<ServerListSlot>().SetData(Legion.Instance.lstServerGroup[i], GetComponent<SelectServer>());
        }
    }

    public void OnClickSelectServer()
    {
        objPopup.SetActive(true);
        PopupManager.Instance.AddPopup(objPopup, OnClickCloseServerPopup);
        
        for(int i=0; i<Legion.Instance.u1ServerCount; i++)
        {
            GameObject _slot = Instantiate(AssetMgr.Instance.AssetLoad("Prefabs/UI/Main/ServerSlot.prefab", typeof(GameObject)) as GameObject);
            _slot.transform.SetParent(objList.transform);
            _slot.transform.localPosition = Vector3.zero;
            _slot.transform.localScale = Vector3.one;
            _slot.GetComponent<ServerListSlot>().SetData(Legion.Instance.lstServerGroup[i], GetComponent<SelectServer>());
        }

        if(Legion.Instance.u2LastLoginServer == 0)
        {
            for(int i=0; i<objList.transform.childCount; i++)
            {
                if(objList.transform.GetChild(i).GetComponent<ServerListSlot>().GetServerData.u1Congestion != 3 &&
                    objList.transform.GetChild(i).GetComponent<ServerListSlot>().GetServerData.u1State == 1)
                    objList.transform.GetChild(i).GetComponent<ServerListSlot>().OnClickSlot();
                break;
            }
        }
    }

    public void OnClickCloseServerPopup()
    {
        for(int i=0; i<objList.transform.childCount; i++)
            Destroy(objList.transform.GetChild(i).gameObject);
        _title.OnSelectedServer();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        
        tempStringBuilder.Append(u2ServerID).Append(". ").Append(TextManager.Instance.GetText(strServerCode));
        txtServerName.text = tempStringBuilder.ToString();
        if(strCrewName != "")
            txtCrewName.text = strCrewName;
        else
            txtCrewName.text = TextManager.Instance.GetText("default_legion_name");
        PopupManager.Instance.RemovePopup(objPopup);
        objPopup.SetActive(false);
    }
}
