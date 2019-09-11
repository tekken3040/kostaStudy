using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRWebViewScript : MonoBehaviour
{
    private WebViewObject webViewObject;

    private void Start()
    {
        string strUrl = "http://www.google.com";

        webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
        webViewObject.Init((msg) => {
            Debug.Log(string.Format("CallFromJS[{0}]", msg));
        });

        webViewObject.LoadURL(strUrl);
        webViewObject.SetVisibility(true);
    }
}
