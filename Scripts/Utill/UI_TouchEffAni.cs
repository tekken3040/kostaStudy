using UnityEngine;
using System.Collections;

public class UI_TouchEffAni : MonoBehaviour
{
    public void OnEnable()
    {
        LeanTween.alpha(this.GetComponent<RectTransform>(), 1, 0.2f).setOnComplete(
            (LeanTween.alpha(this.GetComponent<RectTransform>(), 0, 0.2f).setDelay(0.2f).onCompleteObject));
        LeanTween.scale(this.GetComponent<RectTransform>(), new Vector3(0.5f, 0.5f, 1f), 0.2f).setOnComplete(
            (LeanTween.scale(this.GetComponent<RectTransform>(), new Vector3(1f, 1f, 1f), 0.2f)).setDelay(0.2f).onCompleteObject);
    }
}
