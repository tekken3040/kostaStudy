using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;

public class UI_GoodsInfo : MonoBehaviour
{
    public GameObject _gold;
    public GameObject _cash;
    public GameObject _key;

    StringBuilder tempStringBuilder;

    void Awake()
    {
        tempStringBuilder = new StringBuilder();
    }

    public void OnEnable()
    {
        SetGoodsInfo();
    }

    public void SetGoodsInfo()
    {
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(string.Format("{0:N0}", Legion.Instance.Gold));
        _gold.transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(string.Format("{0:N0}", Legion.Instance.Cash));
        _cash.transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(string.Format("{0:N0}", Legion.Instance.Energy));
        _key.transform.GetChild(2).GetComponent<Text>().text = tempStringBuilder.ToString();
    }
}
