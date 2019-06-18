using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleAnimation : MonoBehaviour
{
    public GameObject LeftCharacter;
    public GameObject RightCharacter;
    public GameObject TitleName;
    public GameObject PressButton;
    public GameObject TouchArea;
    public Vector3[] v3LeftChar;
    public Vector3[] v3RightChar;

    public void Start()
    {
        //StartCoroutine(IdleAnimation());
    }

    public void IdleAnimation()
    {
        //yield return new WaitForSeconds(5f);
        this.GetComponent<Animator>().enabled = false;
        TouchArea.GetComponent<Button>().interactable = true;
        StartCoroutine(CharacterIdleAni());
        StartCoroutine(TitleIdleAni());
        StartCoroutine("PressButtonAni");
        //StartCoroutine(PressedAni());
    }

    IEnumerator CharacterIdleAni()
    {
        LeanTween.moveLocal(LeftCharacter.gameObject, v3LeftChar[1], 8f);
        LeanTween.moveLocal(RightCharacter.gameObject, v3RightChar[1], 8f);
        yield return new WaitForSeconds(8f);
        LeanTween.moveLocal(LeftCharacter.gameObject, v3LeftChar[0], 8f);
        LeanTween.moveLocal(RightCharacter.gameObject, v3RightChar[0], 8f);
        yield return new WaitForSeconds(8f);
        StartCoroutine(CharacterIdleAni());
    }

    IEnumerator TitleIdleAni()
    {
        Object[] sprites = Resources.LoadAll("Sprites/Title/logo_03", typeof(Sprite));        
        
        yield return new WaitForSeconds(5f);
        for(int i=1; i<8; i++)
        {
            TitleName.GetComponent<Image>().sprite =  sprites[i] as Sprite;
            yield return new WaitForSeconds(0.1f);
        }
        TitleName.GetComponent<Image>().sprite = sprites[0] as Sprite;
        yield return new WaitForSeconds(0.1f);

        StartCoroutine(TitleIdleAni());
    }

    IEnumerator PressButtonAni()
    {
        //yield return new WaitForSeconds(0.15f);
        LeanTween.alpha(PressButton.GetComponent<RectTransform>(), 0, 0.15f);
        yield return new WaitForSeconds(0.3f);
        LeanTween.alpha(PressButton.GetComponent<RectTransform>(), 1, 0.15f);
        yield return new WaitForSeconds(0.3f);
        StartCoroutine("PressButtonAni");
    }

    public void PressAnimation()
    {
        StopCoroutine("PressButtonAni");
        StartCoroutine(PressedAni());
    }

    IEnumerator PressedAni()
    {
        LeanTween.alpha(PressButton.GetComponent<RectTransform>(), 0, 0.035f);
        yield return new WaitForSeconds(0.05f);
        LeanTween.alpha(PressButton.GetComponent<RectTransform>(), 1, 0.035f);
        yield return new WaitForSeconds(0.05f);
        StartCoroutine(PressedAni());
    }
}
