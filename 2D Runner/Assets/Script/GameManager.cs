using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] BackGroundScroll _sky;
    [SerializeField] BackGroundScroll _platform;
    [SerializeField] TokoControl tokoControl;

    [SerializeField] Button btn_Start;
    [SerializeField] Button btn_GameOver;
    [SerializeField] Text txtScore;
    [SerializeField] GameObject[] traps;

    [SerializeField] Vector3 trapStartPos;

    bool bPlayerControl = false;
    UInt32 u4Score = 0; 

    void FixedUpdate()
    {
        if (!bPlayerControl)
            return;

        if(tokoControl.GetDead())
        {
            _sky.SetPlay(false);
            _platform.SetPlay(false);
            bPlayerControl = false;
            btn_GameOver.gameObject.SetActive(true);
            DisableTraps();
        }
        u4Score += Convert.ToUInt32(Time.deltaTime * 100);
        txtScore.text = (u4Score / 100).ToString();
    }

    public void OnClickStart()
    {
        bPlayerControl = true;
        txtScore.text = "0";
        u4Score = 0;
        btn_Start.gameObject.SetActive(false);
    }

    public void OnClickGameOver()
    {
        btn_Start.gameObject.SetActive(true);
        btn_GameOver.gameObject.SetActive(false);
        _sky.SetPlay(true);
        _platform.SetPlay(true);
    }

    void DisableTraps()
    {
        for (int i = 0; i < traps.Length; i++)
            traps[i].SetActive(false);
    }

    void EnableTraps()
    {
        for (int i = 0; i < traps.Length; i++)
            traps[i].SetActive(true);
    }
}
