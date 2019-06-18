using UnityEngine;
using System;
using System.Collections;

public class UI_CharSlot : MonoBehaviour
{
    public Byte u1Pos;

    public Hero cHero;

    //public void OnEnable()
    //{
    //    if(Legion.Instance.cBestCrew.acLocation[u1Pos] == null)
    //        cHero = null;
    //    else
    //        cHero = (Hero)(Legion.Instance.cBestCrew.acLocation[u1Pos]);
    //    DebugMgr.LogError("enable : "+u1Pos);
    //    DebugMgr.LogError("enable : "+Legion.Instance.cBestCrew.acLocation[u1Pos]);
    //}

    public void SetData(Byte u1CrewIdx)
    {
        if(Legion.Instance.acCrews[u1CrewIdx].acLocation[u1Pos] == null)
            cHero = null;
        else
            cHero = (Hero)(Legion.Instance.acCrews[u1CrewIdx].acLocation[u1Pos]);
    }
}
