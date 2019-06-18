using UnityEngine;
using System.Collections;

public class Rotate_Skybox : MonoBehaviour
{
    private float skyRot = 0;
    public float rotValue;

	// Use this for initialization
	void Start ()
    {
	    
	}
	
	// Update is called once per frame
	void Update ()
    {
	    skyRot += rotValue * Time.deltaTime;
        skyRot %= 360;
        RenderSettings.skybox.SetFloat("_Rotation", skyRot);
	}
}
