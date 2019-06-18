using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AccessDayRewardSlot : MonoBehaviour
{
    public Text txtRewardDay;
    public Text txtRewardName;
    public Text txtCharLv;
    public Text txtRewardDetailName;
    public GameObject objCheckMarkImg;
    public GameObject objSlotWhiteImg;
    public GameObject objParticleEffect;

    private PopupManager.OnClickEvent function;

    public void SetSlot(int idx, Goods goods, bool isCheck)
    {
        // 날짜 셋팅
        txtRewardDay.text = string.Format(TextManager.Instance.GetText("7days_holy_event_day"), idx);
        txtRewardName.text = Legion.Instance.GetGoodsName(goods);
        objCheckMarkImg.SetActive(isCheck);
        objSlotWhiteImg.SetActive(false);
        objParticleEffect.SetActive(false);

        switch ((GoodsType)goods.u1Type)
        {
            case GoodsType.CHARACTER_PACKAGE:
                {
                    txtCharLv.gameObject.SetActive(true);
                    txtRewardDetailName.gameObject.SetActive(true);

                    ClassGoodsInfo classGoodsInfo = null;
                    EventInfoMgr.Instance.dicClassGoods.TryGetValue(goods.u2ID, out classGoodsInfo);
                    if (classGoodsInfo != null)
                    {
                        txtCharLv.text = string.Format("LV {0}", classGoodsInfo.u2Level);
                        txtRewardDetailName.text = TextManager.Instance.GetText(ClassInfoMgr.Instance.GetClassListInfo()[classGoodsInfo.u2ClassID].sName);
                    }
                }
                break;
            case GoodsType.EQUIP_GOODS:
                {
                    txtCharLv.gameObject.SetActive(false);
                    txtRewardDetailName.gameObject.SetActive(false);
                }
                break;
            default:
                {
                    txtCharLv.gameObject.SetActive(false);
                    txtRewardDetailName.gameObject.SetActive(true);

                    txtRewardDetailName.text = goods.u4Count.ToString();
                }
                break;
        }
    }

    public void RewardCheckMarkEffect(PopupManager.OnClickEvent fun)
    {
        StartCoroutine("GetRewardEffect");
        if(fun != null)
        {
            function = fun;
        }
    }

    private IEnumerator GetRewardEffect()
    {
        SetEffectActive(false);
        RectTransform rtTf = objCheckMarkImg.GetComponent<RectTransform>();
        rtTf.localScale = new Vector3(10, 10);
        objCheckMarkImg.SetActive(true);
        LeanTween.scale(rtTf, Vector3.one, 0.2f);
        yield return new WaitForSeconds(0.25f);

        objSlotWhiteImg.SetActive(true);
        LeanTween.alpha(objSlotWhiteImg.GetComponent<RectTransform>(), 0, 0.6f);
        yield return new WaitForSeconds(0.6f);

        if (function != null)
            function(null);
    }

    public void SetEffectActive(bool isActive)
    {
        objParticleEffect.SetActive(isActive);
    }
}
