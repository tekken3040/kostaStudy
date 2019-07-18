using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionListBtn : MonoBehaviour
{
    [SerializeField] GameObject Edge;

    public void OnEdgeActive(bool isActive)
    {
        Edge.SetActive(isActive);
    }
}
