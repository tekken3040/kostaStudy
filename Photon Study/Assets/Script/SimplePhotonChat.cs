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
        PacketHeader.Set(bwOut, MSGs.CREATE_CARD);
        bwOut.Write(1);
        bwOut.Write(false);
        bwOut.Write("CardDesign");
        bwOut.Write("AnimatedArmor");
        BinaryReader br = new BinaryReader(m, Encoding.Unicode);
        br.BaseStream.Position = 0;
        br.ReadByte().ToString();

        CARD_INIT card = new CARD_INIT();
        card.u1Count = br.ReadByte();
        card.bAura = br.ReadBoolean();
        card.frameName = br.ReadString();
        card.imgName = br.ReadString();

        Debug.Log(card.u1Count);
        Debug.Log(card.bAura);
        Debug.Log(card.frameName);
        Debug.Log(card.imgName);
    }
}
