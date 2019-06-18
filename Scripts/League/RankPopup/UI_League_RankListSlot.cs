using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class UI_League_RankListSlot : MonoBehaviour
{
    [SerializeField] Text txtRank;
    [SerializeField] Text txtChangeRankValue;
    [SerializeField] Text txtCrewName;
    [SerializeField] Text txtWinPoint;
    [SerializeField] Text txtDrawPoint;
    [SerializeField] Text txtLosePoint;
    [SerializeField] Text txtRankPoint;

    [SerializeField] Image imgChangeRankIcon;

    private Sprite[] _changeRankIconSprite;
    private Color[] _topRankTextColor;
    private LeagueLegendRank.RankInfo _rankInfo;

    StringBuilder tempStringBuilder;

    void Awake()
    {
        tempStringBuilder = new StringBuilder();
        _changeRankIconSprite = new Sprite[2];
        _changeRankIconSprite[0] = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.arrow_up_2");
        _changeRankIconSprite[1] = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.arrow_down_2");
        _topRankTextColor = new Color[2];
        _topRankTextColor[0] = Color.white;
        ColorUtility.TryParseHtmlString("FFDB8FFF", out _topRankTextColor[1]);
        imgChangeRankIcon.gameObject.SetActive(false);
        txtChangeRankValue.text = "-";
    }

    public void SetData(LeagueLegendRank.RankInfo _info)
    {
        _rankInfo = _info;
        txtRank.text = _rankInfo.u4Rank.ToString();
        txtCrewName.text = _rankInfo.strLegionName;
        txtRankPoint.text = _rankInfo.u4Point.ToString();
        txtWinPoint.text = _rankInfo.u2Win.ToString();
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append("/").Append(_rankInfo.u2Draw).Append("/");
        txtDrawPoint.text = tempStringBuilder.ToString();
        txtLosePoint.text = _rankInfo.u2Lose.ToString();
        if(Legion.Instance.sName == _rankInfo.strLegionName)
            GetComponent<Image>().enabled = true;
        else
            GetComponent<Image>().enabled = false;
    }
}
