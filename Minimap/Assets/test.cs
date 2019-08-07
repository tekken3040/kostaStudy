using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
       MapObject[] mapObjects= GameObject.FindObjectsOfType<MapObject>() ;
        print("mapObjects.Length" +mapObjects.Length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
