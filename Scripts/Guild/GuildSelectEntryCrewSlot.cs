using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class GuildSelectEntryCrewSlot : MonoBehaviour
{
    [SerializeField] Text txtCrewName;
    [SerializeField] Text txtCrewPower;

    [SerializeField] Button btnSelect;

    [SerializeField] GameObject objCrewTitle;
    [SerializeField] GameObject objMyCrewTitle;

    [SerializeField] Text[] txtCharLevel;
    [SerializeField] Image[] imgClassIcon;
    [SerializeField] Image[] imgElementIcon;
    [SerializeField] GameObject[] objCharSlot;

    StringBuilder tempStringBuilder;
    GuildMember _member;
    GuildSelectEntryCrewPopup _parent;
    int _index = 0;

    private void OnEnable()
    {
        tempStringBuilder = new StringBuilder();
        for(int i=0; i<objCharSlot.Length; i++)
        {
            objCharSlot[i].SetActive(false);
        }
    }

    public void SetData(GuildMember member, int idx, GuildSelectEntryCrewPopup parent)
    {
        _member = member;
        _parent = parent;
        _index = idx;
        txtCrewName.text = _member.strLegionName;
        txtCrewPower.text = _member.u8Power.ToString();

        if(_member.strLegionName == Legion.Instance.sName)
        {
            objCrewTitle.SetActive(false);
            objMyCrewTitle.SetActive(true);
        }
        else
        {
            objCrewTitle.SetActive(true);
            objMyCrewTitle.SetActive(false);
        }

        for(int i=0; i<_member.u2ClassID.Length; i++)
        {
            if(_member.u2ClassID[i] == 0)
                continue;
            objCharSlot[i].SetActive(true);
            txtCharLevel[i].text = _member.u2Level[i].ToString();
            imgClassIcon[i].sprite = AtlasMgr.Instance.GetSprite("Sprites/Hero/hero_icon." + _member.u2ClassID[i]);
            imgClassIcon[i].SetNativeSize();
            imgElementIcon[i].sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + _member.u1Element[i]);
        }

        if(_parent._parent.MASTER_ENTRY)
        {
            if(GuildInfoMgr.Instance.dicMasterEntry.ContainsKey((Byte)_index))
            {
                if(GuildInfoMgr.Instance.dicMasterEntry[(Byte)_index].strLegionName == _member.strLegionName)
                {
                    btnSelect.interactable = false;
                }
            }
        }
        else
        {
            if(GuildInfoMgr.Instance.dicUserEntry.ContainsKey((Byte)_index))
            {
                if(GuildInfoMgr.Instance.dicUserEntry[(Byte)_index].strLegionName == Legion.Instance.sName && _member.strLegionName == Legion.Instance.sName)
                {
                    btnSelect.interactable = false;
                }
            }
        }
    }

    public void OnClickSelect()
    {
        if(_parent._parent.MASTER_ENTRY)
            MasterEntry();
        else
            UserEntry();
        
        _parent.OnClickClose();
    }

    private void UserEntry()
    {
        if(GuildInfoMgr.Instance.dicUserEntry.ContainsKey((Byte)_index))
        {
            if(GuildInfoMgr.Instance.dicUserEntry[(Byte)_index].strLegionName == Legion.Instance.sName)
            {
                //자신의 대표 크루가 포함되어야 한다는 경고문
                PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("popup_title_guild_crew_change_reject"), TextManager.Instance.GetText("desc_guild_crew_change_reject"), null);
                return;
            }
            else
            {
                for(int i=0; i<3; i++)
                {
                    if(GuildInfoMgr.Instance.dicUserEntry.ContainsKey((Byte)(i+1)))
                    {
                        if (GuildInfoMgr.Instance.dicUserEntry[(Byte)(i+1)].strLegionName == _member.strLegionName)
                        {
                            GuildInfoMgr.Instance.dicUserEntry.Remove((Byte)(i+1));
                            break;
                        }
                    }
                }
            }
        }
        else
        {
            if(_member.strLegionName == Legion.Instance.sName)
            {
                for(int i=0; i<3; i++)
                {
                    if(GuildInfoMgr.Instance.dicUserEntry.ContainsKey((Byte)(i+1)))
                    {
                        if(GuildInfoMgr.Instance.dicUserEntry[(Byte)(i+1)].strLegionName == Legion.Instance.sName && ((i+1) != _index))
                        {
                            GuildInfoMgr.Instance.dicUserEntry.Remove((Byte)(i+1));
                            break;
                        }
                    }
                }
            }
            else
            {
                for(int i=0; i<3; i++)
                {
                    if(GuildInfoMgr.Instance.dicUserEntry.ContainsKey((Byte)(i+1)))
                    {
                        if (GuildInfoMgr.Instance.dicUserEntry[(Byte)(i+1)].strLegionName == _member.strLegionName)
                        {
                            GuildInfoMgr.Instance.dicUserEntry.Remove((Byte)(i+1));
                            break;
                        }
                    }
                }
            }
        }

        GuildInfoMgr.Instance.bDirty = true;
        if(GuildInfoMgr.Instance.dicUserEntry.ContainsKey((Byte)_index))
            GuildInfoMgr.Instance.dicUserEntry.Remove((Byte)_index);
        GuildInfoMgr.Instance.dicUserEntry.Add((Byte)_index, _member);
    }

    private void MasterEntry()
    {
        if(GuildInfoMgr.Instance.dicMasterEntry.ContainsKey((Byte)_index))
        {
            for(int i=0; i<3; i++)
            {
                if(GuildInfoMgr.Instance.dicMasterEntry.ContainsKey((Byte)(i+1)))
                {
                    if (GuildInfoMgr.Instance.dicMasterEntry[(Byte)(i+1)].strLegionName == _member.strLegionName)
                    {
                        GuildInfoMgr.Instance.dicMasterEntry.Remove((Byte)(i+1));
                        break;
                    }
                }
            }
        }
        else
        {
            for(int i=0; i<3; i++)
            {
                if(GuildInfoMgr.Instance.dicMasterEntry.ContainsKey((Byte)(i+1)))
                {
                    if (GuildInfoMgr.Instance.dicMasterEntry[(Byte)(i+1)].strLegionName == _member.strLegionName)
                    {
                        GuildInfoMgr.Instance.dicMasterEntry.Remove((Byte)(i+1));
                        break;
                    }
                }
            }
        }

        GuildInfoMgr.Instance.bDirty = true;
        if(GuildInfoMgr.Instance.dicMasterEntry.ContainsKey((Byte)_index))
            GuildInfoMgr.Instance.dicMasterEntry.Remove((Byte)_index);
        GuildInfoMgr.Instance.dicMasterEntry.Add((Byte)_index, _member);
    }
}
