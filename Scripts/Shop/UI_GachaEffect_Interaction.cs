using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UI_GachaEffect_Interaction : MonoBehaviour
{
    [SerializeField] UI_Shop_Gacha_Result_Effect cParent;

    public void CallCharacterTalk()
    {
        cParent.CallCharacterTalk();
    }
}
