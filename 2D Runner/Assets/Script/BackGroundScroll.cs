using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScroll : MonoBehaviour
{
    [SerializeField] CanvasRenderer canvasRenderer;
    [SerializeField] Material material;
    [SerializeField] float fScrollSpeed = 0.5f;

    Vector2 textureOffset;
    bool bPlay = true;

    public void SetPlay(bool _bPlay = true)
    {
        bPlay = _bPlay;
    }

    void FixedUpdate()
    {
        if (!bPlay)
            return;
        textureOffset = new Vector2(Time.deltaTime * fScrollSpeed, 0);
        material.mainTextureOffset += textureOffset;
    }
}
