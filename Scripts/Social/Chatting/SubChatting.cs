using UnityEngine;
using UnityEngine.UI;

public class SubChatting : MonoBehaviour
{
    public Button btnOpenMainChat;          // 메인 채팅창 오픈 버튼

    public RectTransform rtTrSubChat;       // 서브 채팅창 트렌트폼
    public RectTransform chatTextParent;    // 채팅 텍스트 부모;

    public Text txtChileText;
    private int nTxtChildCount;
    public int TxtChildCount { get { return nTxtChildCount; } }

    private float fBaseSubChatSizeX;

    private void Awake()
    {
        fBaseSubChatSizeX = rtTrSubChat.sizeDelta.x;
        nTxtChildCount = chatTextParent.childCount;
        if(nTxtChildCount == 1)
        {
            txtChileText = chatTextParent.GetChild(0).GetComponent<Text>();
        }
    }

    public void AddSubChattingMsg(string msg)
    {
        if (nTxtChildCount == 1)
        {
            txtChileText.text = msg;

            if (fBaseSubChatSizeX < txtChileText.preferredWidth + 20)
                rtTrSubChat.sizeDelta = new Vector2(txtChileText.preferredWidth + 20, rtTrSubChat.sizeDelta.y);
            else
                rtTrSubChat.sizeDelta = new Vector2(fBaseSubChatSizeX, rtTrSubChat.sizeDelta.y);
        }
        else
        {
            txtChileText = chatTextParent.GetChild(0).GetComponent<Text>();

            txtChileText.text = msg;
            txtChileText.transform.SetAsLastSibling();
        }
        txtChileText.gameObject.SetActive(true);
    }

    public void AddSubChattingMsg(BaseMessage msg)
    {
        if (nTxtChildCount == 1)
        {
            txtChileText.text = msg.GetSubChatMsg();

            if (fBaseSubChatSizeX < txtChileText.preferredWidth + 20)
                rtTrSubChat.sizeDelta = new Vector2(txtChileText.preferredWidth + 20, rtTrSubChat.sizeDelta.y);
            else
                rtTrSubChat.sizeDelta = new Vector2(fBaseSubChatSizeX, rtTrSubChat.sizeDelta.y);
        }
        else
        {
            txtChileText = chatTextParent.GetChild(0).GetComponent<Text>();

            txtChileText.text = msg.Message;
            txtChileText.transform.SetAsLastSibling();
        }

        txtChileText.gameObject.SetActive(true);
    }
}

