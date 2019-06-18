using UnityEngine;
using UnityEngine.UI;
using System;

public class GuildRewardPopup : MonoBehaviour
{
    public Text txtReward;

    public UI_League_Reward_ItemSlot[] _ItemSlots;

    GuildLeague cLeague;
    GuildBattleInfoPopup cParent;

    bool next = false;

    private void OnEnable()
    {
        PopupManager.Instance.AddPopup(this.gameObject, OnClickClose);
    }

    public void SetData(GuildLeague _league, Byte _type, GuildBattleInfoPopup _parent, bool nextReward = false)
    {
        cLeague = _league;
        cParent = _parent;
        next = nextReward;

        if(_type == 1)
            txtReward.text = TextManager.Instance.GetText("btn_guild_battle_rank_reward");
        else
            txtReward.text = TextManager.Instance.GetText("btn_guild_battle_division_reward");
        for (int i = 0; i < _ItemSlots.Length; i++)
			_ItemSlots[i].gameObject.SetActive (false);

        for(int i=0; i<cLeague.gReward.Length; i++)
        {
            if(cLeague.gReward[i].u1Type != 0)
            {
                _ItemSlots[i].gameObject.SetActive(true);
                _ItemSlots[i].SetData(cLeague.gReward[i]);
            }
        }


    }

    public void OnClickClose()
    {
        PopupManager.Instance.RemovePopup(this.gameObject);
        if(next)
            cParent.ShowNextReward();
        else
        {
            cParent.btn_Reward.interactable = false;
            this.gameObject.SetActive(false);
        }
        
    }
}
