using UnityEngine;
using System.Collections;

public class PopupScaleAnimation : MonoBehaviour
{
    Vector3 ScaleUP = new Vector3(1.2f, 1.2f, 1.2f);
    public void OnEnable()
    {
        StartCoroutine(ScaleAnimation());
    }

    IEnumerator ScaleAnimation()
    {
        LeanTween.scale(this.gameObject, ScaleUP, 0.1f);
        yield return new WaitForSeconds(0.1f);
        LeanTween.scale(this.gameObject, Vector3.one, 0.1f);
    }
}
