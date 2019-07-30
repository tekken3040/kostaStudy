using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Manager : Singleton<Manager>
{
    [SerializeField] Transform moleGroup;
    [SerializeField] HammerAni hammerAni;
    [SerializeField] UI canvasUI;
    [SerializeField] ParticleSystem particleSystem;

    private MoleAni[] aniMoles;
    private Queue<byte> spawnQ;
    private System.Random random;
    private bool isOver = false;
    public bool IsOver
    {
        set{ isOver = true; }
    }
    private int activeCnt = 0;
    public int ActiveCnt
    {
        get{ return activeCnt; }
        set{ activeCnt = value; }
    }
    private EventSystem eventSystem;
    RaycastHit hit;
    Ray ray;

    private void Awake()
    {
        aniMoles = new MoleAni[moleGroup.childCount];
        for(int i=0; i<moleGroup.childCount; i++)
            aniMoles[i] = moleGroup.GetChild(i).GetComponent<MoleAni>();
        spawnQ = new Queue<byte>();
        random = new System.Random();
        eventSystem = EventSystem.current;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            OnPointerClick();
        }
    }

    public void OnPointerClick()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray.origin, ray.direction, out hit))
        {
            if(hit.transform.parent != null && hit.transform.parent.CompareTag("Mole"))
            {
                if(activeCnt > 0)
                    activeCnt--;
                aniMoles[hit.transform.parent.GetSiblingIndex()].HitMole();
                Vector3 _pos = new Vector3(hit.transform.position.x, 1.5f, hit.transform.position.z);
                hammerAni.OnRequestHit(_pos);
                canvasUI.AddScore();
                particleSystem.Stop();
                particleSystem.transform.localPosition = new Vector3(hit.transform.position.x, 0, hit.transform.position.z); ;
                particleSystem.Play();
            }
        }
        else
            return;
    }

    public void InitMethod()
    {
        isOver = false;
        activeCnt = 0;
        StartCoroutine(AddQueueSpawn());
        StartCoroutine(SpawnMoles());
    }

    public void ResetMoles()
    {
        StopAllCoroutines();
        for(int i=0; i<aniMoles.Length; i++)
        {
            aniMoles[i].HitMole();
        }
    }

    private IEnumerator AddQueueSpawn()
    {
        while(true)
        {
            if(isOver)
                break;
            yield return new WaitForSeconds(0.2f);
            byte u1RandQ = (byte)random.Next(0, 9);
            if(spawnQ.Count > 9)
                continue;
            if(spawnQ.Contains(u1RandQ))
                continue;
            spawnQ.Enqueue(u1RandQ);
        }
    }

    private IEnumerator SpawnMoles()
    {
        while(true)
        {
            if(isOver)
                break;
            yield return new WaitForSeconds(0.5f);
            
            if(activeCnt >= 3)
                continue;
            aniMoles[spawnQ.Dequeue()].ShowMole();
            //spawnQ.Dequeue();
            activeCnt++;
        }
    }
}
