using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;

public class ServerListSlot : MonoBehaviour
{
    [SerializeField] Text txtState;
    [SerializeField] Text txtServerName;
    [SerializeField] Image imgBGGradient;
    [SerializeField] GameObject objDeny;
    [SerializeField] GameObject objSelected;
    [SerializeField] GameObject objSelect;
    [SerializeField] Button btnSelect;
    [SerializeField] Text txtNew;
    [SerializeField] Color[] congestionColor;
    private Legion.ServerGroup _serverData;
    SelectServer _cParent;
    StringBuilder tempStringBuilder;
    public Legion.ServerGroup GetServerData
    {
        get
        {
            return _serverData;
        }
    }

    public void SetData(Legion.ServerGroup _group, SelectServer _parent)
    {
        tempStringBuilder = new StringBuilder();
        _serverData = _group;
        _cParent = _parent;
        btnSelect.interactable = true;
        imgBGGradient.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_05_renew.gradient_white");
        switch(_serverData.u1Congestion)
        {
            case 1:
                txtState.text = TextManager.Instance.GetText("mark_server_good");
                txtState.color = congestionColor[0];
                objDeny.SetActive(false);
                objSelected.SetActive(false);
                objSelect.SetActive(true);
                break;
            case 2:
                txtState.text = TextManager.Instance.GetText("mark_server_rush");
                txtState.color = congestionColor[1];
                objDeny.SetActive(false);
                objSelected.SetActive(false);
                objSelect.SetActive(true);
                break;
            case 3:
                txtState.text = TextManager.Instance.GetText("mark_server_full");
                txtState.color = congestionColor[2];
                btnSelect.interactable = false;
                objDeny.SetActive(true);
                objSelected.SetActive(false);
                objSelect.SetActive(false);
                imgBGGradient.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_05_renew.gradient_red");
                break;
        }
        
        if(_serverData.u1New == 1)
            txtNew.gameObject.SetActive(true);
        else
            txtNew.gameObject.SetActive(false);
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder/*.Append(_serverData.u2ServerID).Append(". ")*/.Append(TextManager.Instance.GetText(_serverData.strServerNameCode));
        if(TextManager.Instance.GetText(_serverData.strServerNameCode) == null)
            txtServerName.text = _serverData.strServerNameCode;
        else
            txtServerName.text = tempStringBuilder.ToString();

        if(_serverData.u1State != 1)
        {
            objDeny.SetActive(true);
            objSelected.SetActive(false);
            objSelect.SetActive(false);
            btnSelect.interactable = false;
            imgBGGradient.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_05_renew.gradient_red");
        }

        else if(Legion.Instance.u2LastLoginServer == _serverData.u2ServerID)
        {
            objDeny.SetActive(false);
            objSelected.SetActive(true);
            objSelect.SetActive(false);
            btnSelect.interactable = false;
            imgBGGradient.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_05_renew.gradient_green");
        }
    }

    public void OnClickSlot()
    {
        Legion.Instance.u2LastLoginServer = _serverData.u2ServerID;
        //Server.ServerMgr.id = Legion.Instance.u2LastLoginServer.ToString();

        //2017.01.06 수정
        //_cParent.Init();
        //_cParent.OnClickCloseServerPopup();
        _cParent.SetServerCode(_serverData.strServerNameCode);
        _cParent.SetCrewName(_serverData.strLegionName);
        _cParent.SERVERID = _serverData.u2ServerID;
        _cParent.RefreashList();
        _cParent.OnClickCloseServerPopup();
        //objDeny.SetActive(false);
        //objSelected.SetActive(true);
        //objSelect.SetActive(false);
        //btnSelect.interactable = false;
        //imgBGGradient.sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_05_renew.gradient_green");
    }
}
