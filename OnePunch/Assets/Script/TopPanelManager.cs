using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TopPanelManager : MonoBehaviour
{
    [SerializeField] Image imgPlayerHP, imgEnemyHP;
    [SerializeField] Image imgPlayerWins, imgEnemyWins;
    [SerializeField] Text txtRoundCnt;

    float fPlayerHP = 1f;
    float fEnemyHP = 1f;
    float parameterHP = 0.01f;

    public enum TARGET
    {
        NONE = 0,
        PLAYER,
        ENEMY,
    }

    public void SetHPImage(TARGET target, float fHP)
    {
        switch(target)
        {
            case TARGET.PLAYER:
                imgPlayerHP.fillAmount -= fHP * parameterHP;
                break;

            case TARGET.ENEMY:
                break;
        }
    }
}
