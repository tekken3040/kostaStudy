using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;

public class MarbleAchieveSlot : MonoBehaviour
{
    public GameObject objAchieveBtn;
    public GameObject objAchieveRewardBtn;

    public string[] AchieveTitleKey;
    public Text txtAchieveTitle;
    public Text txtAchieveCount;
    public GameObject objAchieveClearImg;

    public void SetAchieveSlot(UserAchievement achieveInfo)
    {
        StringBuilder tempString = new StringBuilder();
        objAchieveClearImg.SetActive(false);
        // 업적이 클리어 된 상태라면
        if (achieveInfo.isClear() == true)
        {
            txtAchieveTitle.text = TextManager.Instance.GetText(AchieveTitleKey[1]);
            objAchieveBtn.SetActive(false);
            objAchieveRewardBtn.SetActive(true);
            // 보상을 받은 상태
            if (achieveInfo.bRewarded == true)
                SetAchieveSlotClear();
            else
                objAchieveRewardBtn.GetComponent<Button>().interactable = true;

            tempString.Append(achieveInfo.GetInfo().acReward[0].u4Count);
            tempString.Append(TextManager.Instance.GetText("mark_goods_number_ea"));
        }
        else
        {
            txtAchieveTitle.text = TextManager.Instance.GetText(AchieveTitleKey[0]);
            objAchieveBtn.SetActive(true);

            objAchieveRewardBtn.SetActive(false);
            if (achieveInfo.GetInfo().u4MaxCount == 1)
            {
                tempString.Append(TextManager.Instance.GetText("event_marble_mission_count"));
                tempString.Replace("{0}", achieveInfo.GetInfo().u4MaxCount.ToString());
            }
            else
            {
                tempString.Append(achieveInfo.GetCount()).Append(" / ");
                tempString.Append(achieveInfo.GetInfo().u4MaxCount);
            }
        }

        txtAchieveCount.text = tempString.ToString();
    }

    public void SetAchieveSlotClear()
    {
        objAchieveRewardBtn.GetComponent<Button>().interactable = false;
        objAchieveClearImg.SetActive(true);
    }
}
