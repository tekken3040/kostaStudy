using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;

public class UI_League_PromotedPopup : MonoBehaviour
{
    [SerializeField]
    GameObject objProInfo;
    [SerializeField]
    Text txtProInfoTitle;
    [SerializeField]
    Text txtProInfoContent;
    [SerializeField]
    UI_DivisionMark cDivisionMark;

    [SerializeField]
    GameObject objProRwd;
    [SerializeField]
    Text txtProRwdTitle;
    [SerializeField]
    Text txtProRwdContent;
    // 연출 부
    [SerializeField]
    UI_DivisionMark cPromotedDivisionMark;
    [SerializeField]
    GameObject objPromotedEffect;

    //[SerializeField] Image imgDemotion;
    [SerializeField]
    UI_DivisionMark cDemotionDivisionMark;
    [SerializeField]
    GameObject objDemotionEffect;
    private StringBuilder tempStringBuilder = new StringBuilder();

    void OnEnable()
    {
        if (UI_League.Instance.u1Prom == 1)
            StartCoroutine("DiversionPromotedEffect"); // 승격 연출
        else
            StartCoroutine("DiversionDemotionEffect"); // 강등 연출
    }


    // 디비전 승격 연출
    private IEnumerator DiversionPromotedEffect()
    {
        byte myDivision = Legion.Instance.GetDivision;

        cPromotedDivisionMark.SetDivisionMark((byte)(myDivision - 1));
        yield return new WaitForSeconds(0.5f);
        objPromotedEffect.SetActive(true);

        yield return new WaitForSeconds(1.3f);
        cPromotedDivisionMark.SetDivisionMark(myDivision);

        yield return new WaitForSeconds(1.2f);
        objPromotedEffect.SetActive(false);
        // 팝업 셋팅
        SetPopupInfo(true);
    }

    // 디비전 강등 연출
    private IEnumerator DiversionDemotionEffect()
    {
        //tempStringBuilder.Remove(0, tempStringBuilder.Length);
        //tempStringBuilder.Append("Sprites/BattleField/league_06.division_").Append();
        cDemotionDivisionMark.SetDivisionMark(Legion.Instance.GetDivision);
        objDemotionEffect.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        objDemotionEffect.SetActive(false);

        SetPopupInfo(false);
    }

    private void SetPopupInfo(bool isDivisionUp)
    {
        byte myDivision = Legion.Instance.GetDivision;
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append("mark_division_").Append(myDivision);

        if (isDivisionUp == true) // 승격
        {
            txtProInfoTitle.text = TextManager.Instance.GetText("popup_title_league_divison_up");
            txtProInfoContent.text = string.Format(TextManager.Instance.GetText("popup_desc_league_divison_up"), TextManager.Instance.GetText(tempStringBuilder.ToString()));
        }
        else
        {
            txtProInfoTitle.text = TextManager.Instance.GetText("popup_title_league_division_down");
            txtProInfoContent.text = string.Format(TextManager.Instance.GetText("popup_desc_league_division_down"), TextManager.Instance.GetText(tempStringBuilder.ToString()));
        }

        cDivisionMark.SetDivisionMark(myDivision);
        //imgProInfoIcon.sprite = AtlasMgr.Instance.GetSprite("Sprites/BattleField/league_06.division_" + myDivision);
        //imgProInfoIcon.SetNativeSize();
        objProInfo.SetActive(true);
        StartCoroutine("PopupAutoClose");
    }

    public void OnClickClose()
    {
        StopCoroutine("PopupAutoClose");
        Server.ServerMgr.Instance.RequestLeagueDivisionCheck(Legion.Instance.GetDivision, RequestCheck);

        objProInfo.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void RequestCheck(Server.ERROR_ID err)
    {

    }

    private IEnumerator PopupAutoClose()
    {
        // 팝업이 닫히는데 딜레이를 준다
        yield return new WaitForSeconds(1.5f);

        OnClickClose();
    }
}
