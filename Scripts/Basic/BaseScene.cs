using UnityEngine;
using System.Collections;

//현재 씬 클래스를 얻어온다
public static class Scene
{
	public static BaseScene GetCurrent()
	{
		return GameObject.FindObjectOfType<BaseScene>();
	}
}

// 씬들은 이걸 상속 받음 (인게임 제외)
public abstract class BaseScene : MonoBehaviour {
    
    public GameObject mainPanel;
    
    public InventoryPanel inventoryPanel;
    public ShopPanel shopPanel;   
    //public EventPanel eventPanel;
    
    //상점 버튼 선택
    //상점은 어느 씬에서든 항상 열 수 있다
    public void OnClickShop()
    {
		LobbyScene scene = Scene.GetCurrent() as LobbyScene;
		if(scene != null){
			if (scene._objAlramIcon [(int)LobbyScene.LobbyAlram.SHOP].activeSelf) {
				PopupManager.Instance.SetNoticePopup (MENU.SHOP);
			}
		}

		StartCoroutine(ShowShop(true));
    }

    public IEnumerator ShowShop(bool fade, int index = -1)
    {
        yield return StartCoroutine(OpenShop(fade));        
        
        switch(index)
        {
            case 0: shopPanel.Open(POPUP_SHOP.GOLD); break;
            case 1: shopPanel.Open(POPUP_SHOP.CASH); break;
            case 2: shopPanel.Open(POPUP_SHOP.ENERGY); break;
        }
        Legion.Instance.googleAnalytics.LogEvent(new EventHitBuilder().SetEventCategory("Shop").SetEventAction("Open").SetEventLabel("ShopOpen"));
        if(mainPanel != null)
            mainPanel.SetActive(false);
    }

	public void CloseShop()
	{
        LobbyScene scene = Scene.GetCurrent() as LobbyScene;
        if(scene != null)
        {
            //scene._mainMenu.SetActive(true);
        }
        FadeEffectMgr.Instance.FadeIn();
        Legion.Instance.googleAnalytics.LogEvent(new EventHitBuilder().SetEventCategory("Shop").SetEventAction("Close").SetEventLabel("ShopClose"));
        if(mainPanel != null)
            mainPanel.SetActive(true);
        RefreshAlram();
	}    
    
    public void OpenEventPanel()
    {
        //if(eventPanel == null)
        //{
        //    UnityEngine.Object eventObj = null;
        //    AssetMgr.Instance.AssetLoadAsync((x) => eventObj = x, "Prefabs/UI/Event/Pref_Monthly_login.prefab", typeof(GameObject));
        //    GameObject eventObject = Instantiate(eventObj) as GameObject;
        //    RectTransform rectTransform = eventObject.GetComponent<RectTransform>();
        //
        //    rectTransform.SetParent(transform);
		//	rectTransform.localScale = Vector3.one;
		//	rectTransform.anchoredPosition3D = Vector3.zero;
		//	rectTransform.sizeDelta = Vector2.zero;
        //
        //    eventPanel = eventObject.GetComponent<EventPanel>();
        //    //eventPanel
        //}
    }

    public IEnumerator OpenInventory(bool fade)
    {
        LobbyScene scene = Scene.GetCurrent() as LobbyScene;
        if(scene != null)
        {
            //scene._mainMenu.SetActive(false);
        }
        if(fade)
        {
            FadeEffectMgr.Instance.FadeOut();
            yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
        }        
        
		if(inventoryPanel == null)
		{
			ResourceRequest req = Resources.LoadAsync("Prefabs/UI/Inventory/InventoryPanel", typeof(GameObject));
			//UnityEngine.Object inventory = null;                                   
            
			//yield return StartCoroutine(AssetMgr.Instance.AssetLoadAsync((x) => inventory = x, "Prefabs/UI/Inventory/InventoryPanel.prefab", typeof(GameObject)));
			yield return req;
			GameObject invenObject = Instantiate(req.asset) as GameObject;
			RectTransform rectTransform = invenObject.GetComponent<RectTransform>();
						
			rectTransform.SetParent(transform);
			rectTransform.localScale = Vector3.one;
			rectTransform.anchoredPosition3D = Vector3.zero;
			rectTransform.sizeDelta = Vector2.zero;
			
            FadeEffectMgr.Instance.FadeIn();
			inventoryPanel = invenObject.GetComponent<InventoryPanel>();
            inventoryPanel.Init();
		}
		else
		{
            FadeEffectMgr.Instance.FadeIn();
			inventoryPanel.gameObject.SetActive(true);
            inventoryPanel.Init();
			
		}     
        
        if(shopPanel != null && shopPanel.gameObject.activeSelf)
        {
            shopPanel.gameObject.SetActive(false);
            PopupManager.Instance.RemovePopup(shopPanel.gameObject);
        }
        Legion.Instance.googleAnalytics.LogEvent(new EventHitBuilder().SetEventCategory("Inven").SetEventAction("Open").SetEventLabel("InvenOpen"));
        if(mainPanel != null)
            mainPanel.SetActive(false);
    }
    
    public IEnumerator OpenShop(bool fade)
    {
        LobbyScene scene = Scene.GetCurrent() as LobbyScene;
        if(scene != null)
        {
            //scene._mainMenu.SetActive(false);
        }
        if(fade)
        {
            FadeEffectMgr.Instance.FadeOut();
            yield return new WaitForSeconds(FadeEffectMgr.GLOBAL_FADE_TIME);
        }
        
		if(shopPanel == null)
		{
			ResourceRequest req = Resources.LoadAsync("Prefabs/UI/Shop/ShopPanel", typeof(GameObject));
			//UnityEngine.Object shop = null;
            
			//yield return StartCoroutine(AssetMgr.Instance.AssetLoadAsync((x) => shop = x, "Prefabs/UI/Shop/ShopPanel.prefab", typeof(GameObject)));
			yield return req;
			GameObject shopObject = Instantiate(req.asset) as GameObject;
			RectTransform rectTransform = shopObject.GetComponent<RectTransform>();                                   
						
			rectTransform.SetParent(transform);
			rectTransform.localScale = Vector3.one;
			rectTransform.anchoredPosition3D = Vector3.zero;
			rectTransform.sizeDelta = Vector2.zero;
			
            FadeEffectMgr.Instance.FadeIn();
			shopPanel = shopObject.GetComponent<ShopPanel>();
            shopPanel.Init();
		}
		else
		{
            FadeEffectMgr.Instance.FadeIn();
			shopPanel.gameObject.SetActive(true);
            shopPanel.Init();			
		}        
        
        if(inventoryPanel != null && inventoryPanel.gameObject.activeSelf)
        {
            inventoryPanel.gameObject.SetActive(false);
            PopupManager.Instance.RemovePopup(inventoryPanel.gameObject);
        }
        Legion.Instance.googleAnalytics.LogEvent(new EventHitBuilder().SetEventCategory("Shop").SetEventAction("Open").SetEventLabel("ShopOpen"));
        if(mainPanel != null)
            mainPanel.SetActive(false);
		shopPanel.transform.SetAsLastSibling();
    }

	public void CloseInventory()
	{
        LobbyScene scene = Scene.GetCurrent() as LobbyScene;
        if(scene != null)
        {
            //scene._mainMenu.SetActive(true);
        }
		FadeEffectMgr.Instance.FadeIn();
        if(mainPanel != null)
		    mainPanel.SetActive(true);
		RefreshAlram();
	}
    
    public abstract IEnumerator CheckReservedPopup();
    public abstract void RefreshAlram();
}
