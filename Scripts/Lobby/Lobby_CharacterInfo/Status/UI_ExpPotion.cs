using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;
using System.Collections;

public class UI_ExpPotion : MonoBehaviour
{
    public GameObject _grade;           //포션 등급
    public GameObject _itemImg;         //포션 이미지
    public GameObject _count;           //포션 수량
    public GameObject _selectImg;       //선택 활성화 이미지
    public GameObject _useEffect;       //사용 이펙트
    GameObject _infoItem;               //포션 정보
    GameObject _potionPopup;            //포션 사용 팝업
    ConsumableItem consumItem;          //포션 데이터
    StringBuilder tempStringBuilder;
    UInt16 prevConsumCnt;
    UInt16 potionCnt = 0;

    void Awake()
    {
        tempStringBuilder = new StringBuilder();
        _infoItem = GameObject.Find("Info_Bg");
        _potionPopup = GameObject.Find("Popup_EXP_Potion");
    }

    void Start()
    {
        for(int i=0; i<_infoItem.transform.childCount; i++)
        {
            _infoItem.transform.GetChild(i).gameObject.SetActive(false);
        }
        prevConsumCnt = consumItem.u2Count;
        _selectImg.gameObject.SetActive(false);
    }
    public ConsumableItem GetConsumableItem()
    {
        return consumItem;
    }
    //포션 선택
    public void OnClickItem()
    {
        for(int i=0; i<_infoItem.transform.childCount; i++)
        {
            _infoItem.transform.GetChild(i).gameObject.SetActive(true);
        }
        for(int i=0; i<this.transform.parent.childCount; i++)
        {
            this.transform.parent.GetChild(i).GetChild(6).gameObject.SetActive(false);
        }
        this.transform.GetChild(5).gameObject.SetActive(false);
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(TextManager.Instance.GetText(ItemInfoMgr.Instance.GetConsumableItemInfo(consumItem.cItemInfo.u2ID).sName));
        _infoItem.transform.GetChild(0).GetComponent<Text>().text = tempStringBuilder.ToString();

        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(TextManager.Instance.GetText(ItemInfoMgr.Instance.GetConsumableItemInfo(consumItem.cItemInfo.u2ID).sDescription));
        //switch(TextManager.Instance.eLanguage)
        //{
        //    case TextManager.LANGUAGE_TYPE.ENGLISH:
        //        tempStringBuilder.Append(TextManager.Instance.GetText(ItemInfoMgr.Instance.GetConsumableItemInfo(consumItem.cItemInfo.u2ID).sDescription));
        //        break;
        //
        //    case TextManager.LANGUAGE_TYPE.KOREAN:
        //        tempStringBuilder.Append(TextManager.Instance.GetText(ItemInfoMgr.Instance.GetConsumableItemInfo(consumItem.cItemInfo.u2ID).sDescription)).Insert(9, "\n\n");
        //        break;
        //}
        
        _infoItem.transform.GetChild(1).GetComponent<Text>().text = tempStringBuilder.ToString();

        _selectImg.gameObject.SetActive(true);
        _potionPopup.GetComponent<UI_Status_UseExpPotion>()._selectedConsumableItem = consumItem;
        _potionPopup.GetComponent<UI_Status_UseExpPotion>().u2SelectedItemID = consumItem.cItemInfo.u2ID;
        if(_potionPopup.GetComponent<UI_Status_UseExpPotion>()._prevConsumableItem == null)
            _potionPopup.GetComponent<UI_Status_UseExpPotion>()._prevConsumableItem = consumItem;
        _potionPopup.GetComponent<UI_Status_UseExpPotion>().transformIndex = this.transform.GetSiblingIndex();
        _potionPopup.GetComponent<UI_Status_UseExpPotion>().ChangeConsumableItems();
    }
    //포션 정보 생성
    public void SetExpPotionData(UInt16 u2ID, ConsumableItem _item)
    {
        consumItem = _item;
        _itemImg.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Item/item_01." + u2ID.ToString());
        _grade.GetComponent<Image>().sprite = AtlasMgr.Instance.GetSprite("Sprites/Common/common_02_renew.grade_" + ItemInfoMgr.Instance.GetConsumableItemInfo(consumItem.cItemInfo.u2ID).u2Grade);
        potionCnt = _item.u2Count;
        tempStringBuilder.Remove(0, tempStringBuilder.Length);
        tempStringBuilder.Append(_item.u2Count.ToString());
        _count.GetComponent<Text>().text = tempStringBuilder.ToString();
        if(consumItem.isNew)
            this.transform.GetChild(5).gameObject.SetActive(true);
        else
            this.transform.GetChild(5).gameObject.SetActive(false);
    }
    //포션 정보 새로고침
    public void RefreshPotionData()
    {
        if(consumItem.u2Count == 0)
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            _infoItem.transform.GetChild(0).GetComponent<Text>().text = tempStringBuilder.ToString();
            _infoItem.transform.GetChild(1).GetComponent<Text>().text = tempStringBuilder.ToString();
            _potionPopup.GetComponent<UI_Status_UseExpPotion>()._selectedConsumableItem = null;
            for(int i=0; i<_infoItem.transform.childCount; i++)
            {
                _infoItem.transform.GetChild(i).gameObject.SetActive(false);
            }
            //Legion.Instance.cInventory.RemoveItem(consumItem.cItemInfo.u2ID);
            GameObject.Destroy(this.gameObject);
        }
        else
        {
            if(!usingPotion && (prevConsumCnt > consumItem.u2Count))
                StartCoroutine(PlayUseEffect());
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(consumItem.u2Count.ToString());
            _count.GetComponent<Text>().text = tempStringBuilder.ToString();
            prevConsumCnt = consumItem.u2Count;
        }
    }
    public void DecreaseCount()
    {
        potionCnt--;
        if(potionCnt == 0)
        {
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            _infoItem.transform.GetChild(0).GetComponent<Text>().text = tempStringBuilder.ToString();
            _infoItem.transform.GetChild(1).GetComponent<Text>().text = tempStringBuilder.ToString();
            //_potionPopup.GetComponent<UI_Status_UseExpPotion>()._selectedConsumableItem = null;
            for(int i=0; i<_infoItem.transform.childCount; i++)
            {
                _infoItem.transform.GetChild(i).gameObject.SetActive(false);
            }
            _potionPopup.GetComponent<UI_Status_UseExpPotion>().EmptyConsumableItems();
            _potionPopup.GetComponent<UI_Status_UseExpPotion>()._selectedConsumableItem = null;
            //Legion.Instance.cInventory.RemoveItem(consumItem.cItemInfo.u2ID);
            GameObject.Destroy(this.gameObject);
        }
        else
        {
            if(!usingPotion && (prevConsumCnt > potionCnt))
                StartCoroutine(PlayUseEffect());
            tempStringBuilder.Remove(0, tempStringBuilder.Length);
            tempStringBuilder.Append(potionCnt.ToString());
            _count.GetComponent<Text>().text = tempStringBuilder.ToString();
            prevConsumCnt = potionCnt;
        }
    }
    bool usingPotion = false;
    IEnumerator PlayUseEffect()
    {
        usingPotion = true;
        /*
        _useEffect.SetActive(true);
        yield return new WaitForSeconds(6f);
        _useEffect.SetActive(false);
        */
        LeanTween.scale(this.GetComponent<RectTransform>(), new Vector2(1.2f, 1.2f), 0.1f).setOnComplete(
        LeanTween.scale(this.GetComponent<RectTransform>(), Vector2.one, 0.1f).setDelay(0.1f).onCompleteObject);
        yield return new WaitForSeconds(0.2f);
        usingPotion = false;
    }
}
