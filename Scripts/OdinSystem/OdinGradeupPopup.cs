using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class OdinGradeupPopup : MonoBehaviour
{
    public Image[] aImgWindow;
    public Text txtNextGradeName;
    public Image ImgOdinGradeIcon;

    public GameObject objHighGradeDeco;

    public GameObject objParticleEffect;

    private Vector3 iconDefaultScale = new Vector3(1.8f, 1.8f, 1.8f);

    public void SetPopup(Byte currentLv, Byte nextLv)
    {
        // vip 등급이 11보다 작으면 0 아니라면 11
        Sprite window = null;
        if (nextLv < 11)
        {
            objHighGradeDeco.SetActive(false);
            window = AtlasMgr.Instance.GetSprite("Sprites/Common/common_05_renew.odin_GradeUpWindow_0");
        }
        else
        {
            objHighGradeDeco.SetActive(true);
            window = AtlasMgr.Instance.GetSprite("Sprites/Common/common_05_renew.odin_GradeUpWindow_1");
        }

        for(int i = 0;i < aImgWindow.Length; ++i)
        {
            aImgWindow[i].sprite = window;
            aImgWindow[i].SetNativeSize();
        }

        txtNextGradeName.text = string.Format(TextManager.Instance.GetText("odin_advance_finish"),
            TextManager.Instance.GetText(string.Format("odin_name_{0}", nextLv)));

        objParticleEffect.SetActive(false);
        ImgOdinGradeIcon.rectTransform.localScale = iconDefaultScale;
        ImgOdinGradeIcon.sprite = AtlasMgr.Instance.GetSprite(string.Format("Sprites/Common/otm_ambulram.Odin_Grade_icon_{0}", currentLv));
        ImgOdinGradeIcon.SetNativeSize();

        this.transform.localScale = Vector3.one * 1.3f;
        this.gameObject.SetActive(true);
        StartCoroutine(StartEffect(nextLv));
    }

    public IEnumerator StartEffect(Byte nextLv)
    {
        LeanTween.scale(this.gameObject, Vector3.one, 0.1f);
        // 대기
        yield return new WaitForSeconds(0.3f);

        // 파티클 연출 
        objParticleEffect.SetActive(true);
        yield return new WaitForSeconds(2.1f);
        
        // 다음 등급 아이콘 셋팅
        ImgOdinGradeIcon.sprite = AtlasMgr.Instance.GetSprite(string.Format("Sprites/Common/otm_ambulram.Odin_Grade_icon_{0}", nextLv));
        ImgOdinGradeIcon.SetNativeSize();
        ImgOdinGradeIcon.rectTransform.localScale = Vector3.one * 5f;

        // 임펙트!~
        LeanTween.alpha(ImgOdinGradeIcon.rectTransform, 1f, 0.15f);
        LeanTween.scale(ImgOdinGradeIcon.rectTransform, iconDefaultScale, 0.2f);

        objParticleEffect.SetActive(false);
    }

    public void OnClickClose()
    {
        objParticleEffect.SetActive(false);
        ImgOdinGradeIcon.rectTransform.localScale = iconDefaultScale;

        this.gameObject.SetActive(false);
        PopupManager.Instance.ShowOKPopup(TextManager.Instance.GetText("odin_advance_check_title"), TextManager.Instance.GetText("odin_advance_check_desc"), null);
    }
}
