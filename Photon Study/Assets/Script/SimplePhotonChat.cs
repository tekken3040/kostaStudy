using UnityEngine;
using Server;
using Photon;
using System.IO;
using System.Text;

public class SimplePhotonChat : MonoBehaviour
{
    [SerializeField] GameObject prefabCard;
    [SerializeField] GameObject parentPanel;

    private void Start()
    {
        ServerMgr.Instance.InitConnect();
        //Test();
    }

    public void OnClickCreateCardBtn()
    {
        CARD_INIT card = new CARD_INIT();
        card.u1Count = 1;
        card.bAura = false;
        card.frameName = "CardDesign";
        card.imgName = "AnimatedArmor";
        card.callback = CreateCard;
        ServerMgr.Instance.RequestCreateCard(MSGs.CREATE_CARD, card);
    }

    public void CreateCard(Server.ERROR_ID err)
    {
        GameObject card_01 = Photon.Pun.PhotonNetwork.InstantiateSceneObject(prefabCard.name, parentPanel.transform.position, Quaternion.Euler(0, 0, 0));
        card_01.transform.position = Vector3.zero;
    }

    public void Test()
    {
        MemoryStream m = new MemoryStream(10000);
        BinaryWriter bwOut = new BinaryWriter(m, Encoding.Unicode);
        bwOut.Write(1);
        bwOut.Write(false);
        bwOut.Write("CardDesign");
        bwOut.Write("AnimatedArmor");
        for(int i=0; i<m.ToArray().Length; i++)
            Debug.Log(m.ToArray().GetValue(i).ToString());
    }
}
