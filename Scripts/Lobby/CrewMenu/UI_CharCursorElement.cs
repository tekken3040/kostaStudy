using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_CharCursorElement : MonoBehaviour
{
    public GameObject HeroImg;
    public GameObject HeroElementalProperty;
    public GameObject HeroLevel;
    public GameObject Pref_Effect;

    public Vector3[] HeroImg_Pos;

    public Hero cHero;

    public void OnEnable()
    {
        GameObject tempEff = Instantiate(Pref_Effect);
        tempEff.transform.SetParent(this.transform.GetChild(0).transform);
        tempEff.transform.SetParent(this.transform);
        tempEff.transform.localScale = Vector3.one;
        tempEff.transform.localPosition = Vector3.zero;
    }

    public void SetData(Hero inHero)
    {
        cHero = inHero;

        HeroImg.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Hero/hero_icon." + cHero.cClass.u2ID);
        HeroImg.GetComponent<Image>().SetNativeSize();
        //HeroImg.transform.localPosition = HeroImg_Pos[cHero.cClass.u2ID -1];
        HeroElementalProperty.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + (cHero.acEquips[6].GetEquipmentInfo().u1Element));
        HeroLevel.GetComponent<Text>().text = cHero.cLevel.u2Level.ToString();
    }
}
