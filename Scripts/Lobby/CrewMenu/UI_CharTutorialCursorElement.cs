using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_CharTutorialCursorElement : MonoBehaviour
{
    public GameObject HeroImg;
    public GameObject HeroElementalProperty;
    public GameObject HeroLevel;
    public GameObject Pref_Effect;
    public GameObject Pref_Indicate;

    public Vector3[] HeroImg_Pos;

    public Hero cHero;

    private Vector2 AniStartPos;
    private Vector2 AniEndPos;

    public void OnEnable()
    {
        //GameObject tempEff = Instantiate(Pref_Effect);
        //tempEff.transform.SetParent(this.transform.GetChild(0).transform);
        //tempEff.transform.SetParent(this.transform);
        //tempEff.transform.localScale = Vector3.one;
        //tempEff.transform.localPosition = Vector3.zero;
    }

    public void SetData(Hero inHero, Vector2 startPos, Vector2 endPos)
    {
        cHero = inHero;

        AniStartPos = startPos;
        AniEndPos = endPos;

        HeroImg.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Hero/hero_icon." + cHero.cClass.u2ID);
        HeroImg.GetComponent<Image>().SetNativeSize();
        //HeroImg.transform.localPosition = HeroImg_Pos[cHero.cClass.u2ID -1];
        HeroElementalProperty.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.element_" + (cHero.acEquips[6].GetEquipmentInfo().u1Element));
        HeroLevel.GetComponent<Text>().text = cHero.cLevel.u2Level.ToString();

        GameObject tempIndicate = Instantiate(Pref_Indicate);
        tempIndicate.transform.SetParent(this.transform);
        tempIndicate.transform.localPosition = Vector3.zero;
        tempIndicate.transform.localScale = Vector3.one;
        tempIndicate.GetComponent<Image>().type = Image.Type.Simple;
        tempIndicate.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_01_renew.btn_indic_round");
        tempIndicate.GetComponent<Image>().SetNativeSize();
        tempIndicate.transform.SetAsLastSibling();

        transform.position = AniStartPos;
        //transform.TransformPoint(AniStartPos);

        StartCoroutine(CheckState());
        //this.gameObject.SetActive(false);
    }

    IEnumerator CheckState()
    {
        //while(Legion.Instance.cTutorial.au1Step[4] == 3 && Legion.Instance.cTutorial.au1Step[4] != 200)
        while(true)
        {
            if(Legion.Instance.cTutorial.au1Step[4] == 3)
                break;
            else
                yield return null;
        }
        GetComponent<CanvasGroup>().alpha = 1;
        PlayIndicateAnimation();
    }

    public void PlayIndicateAnimation()
    {
        StartCoroutine(AnimationStart());
    }

    IEnumerator AnimationStart()
    {
        LeanTween.moveLocal(gameObject, AniEndPos, 2f);
        yield return new WaitForSeconds(2.5f);
        transform.position = AniStartPos;
        StartCoroutine(AnimationStart());
    }
}
