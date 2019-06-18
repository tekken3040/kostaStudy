using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;

public class EquipPackSlot : EventItemSlot
{
    public Text _txtTitle;
    public Image _imgPackImage;
    public Text _txtPackPrice;
    public Text _txtPackDetail;
    public Button _btnBuyBtn;
    public GameObject _objDisableImg;

    public void SetSlot(EventPackageInfo eventInfo)
    {
        StringBuilder tempString = new StringBuilder();
        _txtTitle.text = TextManager.Instance.GetText(eventInfo.sName);
        _txtPackPrice.text = tempString.Append(TextManager.Instance.GetText("btn_event_grow_buy")).Append(" ").Append(GetEventPackagePriceString(eventInfo)).ToString();

        ClassGoodsEquipInfo equipGoodsInfo = EventInfoMgr.Instance.dicClassGoodsEquip[eventInfo.acPackageRewards[0].u2ID];
        EquipmentInfo equipInfo = EquipmentInfoMgr.Instance.GetInfo(equipGoodsInfo.u2Equip);

        tempString.Remove(0, tempString.Length);
        _imgPackImage.sprite = AtlasMgr.Instance.GetSprite(tempString.Append("Sprites/Event/event_02.EquipPackIcon_").Append(equipInfo.u2ClassID).ToString());
        _txtPackDetail.text = string.Format(TextManager.Instance.GetText("pkg_mark_star_tier"), equipGoodsInfo.u1StarLevel, equipGoodsInfo.u1SmithingLevel);
    }

    protected string GetEventPackagePriceString(EventPackageInfo packageInfo)
    {
        StringBuilder tempString = new StringBuilder();
#if UNITY_ANDROID || UNITY_EDITOR
        if (TextManager.Instance.eLanguage == TextManager.LANGUAGE_TYPE.KOREAN)
            tempString.Append("￦").Append(packageInfo.cNeedGoods.u4Count.ToString("n0"));
        else
            tempString.Append("$").Append(packageInfo.iOSPrice.ToString());
#elif UNITY_IOS
		tempString.Append("$").Append(packageInfo.iOSPrice.ToString());
#endif
        return tempString.ToString();
    }
}
