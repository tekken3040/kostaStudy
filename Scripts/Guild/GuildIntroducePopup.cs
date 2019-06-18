using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class GuildIntroducePopup : MonoBehaviour
{
    [SerializeField] Text txtInfoTitle;
    [SerializeField] Text txtInfoSubTitle;
    [SerializeField] Text txtInfoContent;

    [SerializeField] Button btnPrev;
    [SerializeField] Button btnNext;

    private Byte u1Page = 0;
    private StringBuilder tempStringBuilder;

    private void OnEnable()
    {
        tempStringBuilder = new StringBuilder();
        u1Page = 0;

        RefreshPageButton();
    }

    public void RefreshPageButton()
    {
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        switch(u1Page)
        {
            case 0:
                btnPrev.interactable = false;
                btnNext.interactable = true;

                txtInfoTitle.text = TextManager.Instance.GetText("popup_guild_desc_guildinfo_title_1");
                txtInfoSubTitle.text = TextManager.Instance.GetText("popup_guild_desc_guildinfo_subtitle_1");
                txtInfoContent.text = TextManager.Instance.GetText("popup_guild_desc_guildinfo_desc_1");
                break;

            case 1:
                btnPrev.interactable = true;
                btnNext.interactable = true;

                txtInfoTitle.text = TextManager.Instance.GetText("popup_guild_desc_guildinfo_title_2");
                txtInfoSubTitle.text = TextManager.Instance.GetText("popup_guild_desc_guildinfo_subtitle_2");
                txtInfoContent.text = TextManager.Instance.GetText("popup_guild_desc_guildinfo_desc_2");
                break;

            case 2:
                btnPrev.interactable = true;
                btnNext.interactable = true;
                
                txtInfoTitle.text = TextManager.Instance.GetText("popup_guild_desc_guildinfo_title_3");
                txtInfoSubTitle.text = TextManager.Instance.GetText("popup_guild_desc_guildinfo_subtitle_3");
                txtInfoContent.text = TextManager.Instance.GetText("popup_guild_desc_guildinfo_desc_3");
                break;

            case 3:
                btnPrev.interactable = true;
                btnNext.interactable = true;

                txtInfoTitle.text = TextManager.Instance.GetText("popup_guild_desc_guildinfo_title_4");
                txtInfoSubTitle.text = TextManager.Instance.GetText("popup_guild_desc_guildinfo_subtitle_4");
                txtInfoContent.text = TextManager.Instance.GetText("popup_guild_desc_guildinfo_desc_4");
                break;

            case 4:
                btnPrev.interactable = true;
                btnNext.interactable = false;

                txtInfoTitle.text = TextManager.Instance.GetText("popup_guild_desc_guildinfo_title_5");
                txtInfoSubTitle.text = TextManager.Instance.GetText("popup_guild_desc_guildinfo_subtitle_5");
                txtInfoContent.text = TextManager.Instance.GetText("popup_guild_desc_guildinfo_desc_5");
                break;
        }
    }

    public void OnClickPrev()
    {
        if(u1Page > 0)
            u1Page--;

        RefreshPageButton();
    }

    public void OnClickNext()
    {
        if(u1Page <4)
            u1Page++;

        RefreshPageButton();
    }
}
