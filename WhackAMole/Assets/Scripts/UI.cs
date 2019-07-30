using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] Image panelImage;
    [SerializeField] Button panelButton;
    [SerializeField] GameObject objStart;
    [SerializeField] GameObject objGameOver;
    [SerializeField] Text txtTime;
    [SerializeField] Text txtScore;

    private byte u1Time = 30;
    private UInt16 u2Score = 0;
    private const byte DefaultTime = 30;
    private const byte DefaultAddScore = 50;

    private bool isStart = false;

    public void OnClickButton()
    {
        if(isStart)
        {
            u1Time = DefaultTime;
            u2Score = 0;
            objGameOver.SetActive(false);
            objStart.SetActive(true);
            isStart = false;
        }
        else
        {
            OnGameStart();
            Manager.Instance.InitMethod();
        }
    }
    
    public void OnGameStart()
    {
        panelImage.enabled = false;
        panelButton.enabled = false;
        objStart.SetActive(false);
        isStart = true;
        StartCoroutine(TimeStart());
    }

    private IEnumerator TimeStart()
    {
        while(u1Time > 0)
        {
            yield return new WaitForSeconds(1f);
            txtTime.text = Convert.ToString(--u1Time);
        }

        panelImage.enabled = true;
        panelButton.enabled = true;
        objGameOver.SetActive(true);
        //Manager.Instance.IsOver = true;
        Manager.Instance.ResetMoles();
        StopAllCoroutines();
    }

    public void AddScore()
    {
        u2Score += DefaultAddScore;
        txtScore.text = u2Score.ToString();
    }
}
