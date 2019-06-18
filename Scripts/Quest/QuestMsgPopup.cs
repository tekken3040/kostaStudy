using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;

public class QuestMsgPopup : MonoBehaviour
{
    public GameObject Effect;
    public GameObject TextUp;
    public GameObject TextDown;
    public GameObject Line;

    public Color[] RGB_Quest;

    QuestInfo questInfo;
    StringBuilder tempStringBuilder;

	float fadeDelay = 1.2f;

    void Awake()
    {
        tempStringBuilder = new StringBuilder();
    }

    public void SetData(QuestInfo info, bool bClear)
    {
        questInfo = info;

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        if(!bClear)
        {
            if(questInfo.u1MainType > 1)
                TextUp.GetComponent<Text>().text = TextManager.Instance.GetText("mark_direction_quest_sub");
            else
                TextUp.GetComponent<Text>().text = TextManager.Instance.GetText("mark_direction_quest_main");
            TextUp.GetComponent<Gradient>().StartColor = RGB_Quest[0];
            TextUp.GetComponent<Gradient>().EndColor = RGB_Quest[1];

			TextDown.GetComponent<Text>().text = TextManager.Instance.GetText(questInfo.sName);
            TextDown.GetComponent<Gradient>().StartColor = RGB_Quest[0];
            TextDown.GetComponent<Gradient>().EndColor = RGB_Quest[1];

			GetComponent<Animator>().Play("BattleMsg");
			SoundManager.Instance.PlayEff ("Sound/UI/04. Main/UI_Button_Guild_1");
        }
        else
        {
			TextUp.GetComponent<Text>().text = TextManager.Instance.GetText(questInfo.sName);
            TextUp.GetComponent<Gradient>().StartColor = RGB_Quest[2];
            TextUp.GetComponent<Gradient>().EndColor = RGB_Quest[3];

            TextDown.GetComponent<Text>().text = TextManager.Instance.GetText("mark_direction_quest_done");
            TextDown.GetComponent<Gradient>().StartColor = RGB_Quest[2];
            TextDown.GetComponent<Gradient>().EndColor = RGB_Quest[3];

            Effect.gameObject.SetActive(true);

			fadeDelay = 1.0f;

			GetComponent<Animator>().Play("QuestEnd");
			SoundManager.Instance.PlayEff ("Sound/UI/10. Guild/UI_Guild_Direc_1");
        }
        StartCoroutine(TextAlphaAni());
    }

    IEnumerator TextAlphaAni()
    {
        Color tempStart = TextUp.GetComponent<Gradient>().StartColor;
        Color tempEnd = TextUp.GetComponent<Gradient>().EndColor;
		yield return new WaitForSeconds(fadeDelay);
        for(int i=0; i<21; i++)
        {
            yield return new WaitForSeconds(0.03f);
            TextUp.GetComponent<Gradient>().StartColor = new Color(tempStart.r, tempStart.g, tempStart.b, tempStart.a-(i*0.05f));
            TextUp.GetComponent<Gradient>().EndColor = new Color(tempEnd.r, tempEnd.g, tempEnd.b, tempEnd.a-(i*0.05f));
            TextDown.GetComponent<Gradient>().StartColor = new Color(tempStart.r, tempStart.g, tempStart.b, tempStart.a-(i*0.05f));
            TextDown.GetComponent<Gradient>().EndColor = new Color(tempEnd.r, tempEnd.g, tempEnd.b, tempEnd.a-(i*0.05f));
            TextUp.GetComponent<Text>().SetAllDirty();
            TextDown.GetComponent<Text>().SetAllDirty();
            Line.GetComponent<Image>().color = new Color(1, 1, 1, 1-(0.05f*i));
        }

		Destroy(gameObject);
    }
}
