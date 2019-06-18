using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ParticleColorChanger : MonoBehaviour {
	ParticleSystem[] particles;

	public Color32[] colors;
	// Use this for initialization
	void Awake () {
		particles = transform.GetComponentsInChildren<ParticleSystem>();
	}
	
	public void SetColor(Color32 color){
		for(int i=0; i<particles.Length; i++){
			particles[i].startColor = color;
		}
	}

	public void SetColorByIndex(int idx){
		for(int i=0; i<particles.Length; i++){
			particles[i].startColor = colors[idx];
		}
	}
}
