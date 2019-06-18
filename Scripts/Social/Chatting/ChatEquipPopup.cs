using UnityEngine;
using UnityEngine.UI;
using System;

public class ChatEquipPopup : MonoBehaviour
{
    private const int MAX_SLOT_COUNT = 3;
    public Image imgItemIcon;
    public Image imgItemGrade;

    public Text txtTier;
    public Text txtItemName;
    public Gradient cItemNameColor;

    public Text txtCreatorName;
    
    public Text[] aStatusName;
    public Text[] aStatusPoint;
    public Text[] aSkillInfo;
    public Image[] SkillIcon;

    //public Button btnShortcut;
    //private LobbyScene lobbyScene;

    public void Awake()
    {
        cItemNameColor = txtItemName.GetComponent<Gradient>();
    }

    public void SetEquipPopup(ChatEquipInfo equipinfo)
    {
        EquipmentInfo info = EquipmentInfoMgr.Instance.GetInfo(equipinfo.equipID);
        ForgeInfo forgeInfo = ForgeInfoMgr.Instance.GetList()[(equipinfo.smithyLV - 1)];
        // 아이콘 셋팅
        imgItemIcon.sprite = AtlasMgr.Instance.GetSprite(string.Format("Sprites/Item/{0}", info.cModel.sImagePath));
        imgItemIcon.SetNativeSize();
        imgItemGrade.sprite = AtlasMgr.Instance.GetSprite(string.Format("Sprites/Common/common_02_renew.grade_{0}", forgeInfo.u2ID));
        // 장비 티어 넣기
        txtTier.text = TextManager.Instance.GetText(string.Format("forge_level_{0}", equipinfo.smithyLV));
        // 장비 이름 셋팅
        txtItemName.text = TextManager.Instance.GetText(info.sName);
        UIManager.Instance.SetGradientFromElement(cItemNameColor, info.u1Element);
        // 장비 제작자 이름 넣기
        if (equipinfo.creatorName == "" || equipinfo.creatorName == null)
        {
            txtCreatorName.gameObject.SetActive(false);
        }
        else
        {
            txtCreatorName.gameObject.SetActive(true);
            txtCreatorName.text = string.Format("by {0}", equipinfo.creatorName);
        }

        for (int i = 0; i < MAX_SLOT_COUNT; ++i)
        {
            if (i >= equipinfo.statusPonits.Length || i >= info.acStatAddInfo.Length)
            {
                aStatusName[i].gameObject.SetActive(false);
                aStatusPoint[i].gameObject.SetActive(false);
            }
            else
            {
                aStatusName[i].gameObject.SetActive(true);
                aStatusPoint[i].gameObject.SetActive(true);

                aStatusName[i].text = Status.GetStatText(info.acStatAddInfo[i].u1StatType);
                aStatusPoint[i].text = equipinfo.statusPonits[i].ToString();
            }

            if (i < forgeInfo.cSmithingInfo.GetSkillCount())
            {
                aSkillInfo[i].gameObject.SetActive(true);
                SkillInfo skill = SkillInfoMgr.Instance.GetInfoBySlot(info.u2ClassID, equipinfo.skillSlot[i]);

                aSkillInfo[i].text = string.Format("{0} + {1}",TextManager.Instance.GetText(skill.sName), forgeInfo.cSmithingInfo.u1SkillInitLevel);
                SkillIcon[i].sprite = AtlasMgr.Instance.GetSprite(String.Format("Sprites/Skill/Atlas_SkillIcon_{0}.{1}", info.u2ClassID, skill.u2ID));
            }
            else
            {
                aSkillInfo[i].gameObject.SetActive(false);
            }
        }
        
        // 로비 여부를 확인하여 바로가기 버튼을 활성화 시킨다
        //LobbyScene scene = Scene.GetCurrent() as LobbyScene;
        //if(scene != null)
        //{
        //    lobbyScene = scene;
        //    btnShortcut.interactable = true;
        //}
        //else
        //{
        //    lobbyScene = null;
        //    btnShortcut.interactable = false;
        //}
    }

    //public void OnClickShortcut()
    //{
    //    if(lobbyScene != null)
    //    {
    //        PopupManager.Instance.CloseMainChatWindow();
    //        lobbyScene.OpenCreateEquipWindow();
    //        OnClickClose();
    //    }
    //}

    public void OnClickClose()
    {
        //lobbyScene = null;
        PopupManager.Instance.RemovePopup(this.gameObject);
        this.gameObject.SetActive(false);
    }
}
