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
        ServerMgr.Instance.RequestCreateCard(MSGs.CREATE_CARD, card, CreateCard);
    }

    public void CreateCard(Server.ERROR_ID err)
    {
        GameObject card_01 = Photon.Pun.PhotonNetwork.Instantiate("Prefabs/"+prefabCard.name, parentPanel.transform.position, Quaternion.Euler(0, 0, 0));
        card_01.transform.SetParent(parentPanel.transform);
        card_01.transform.localPosition = Vector3.zero;
    }
}
