using UnityEngine;
using System.Collections;

public class VFX {

	private ParticleSystem[] aParticles;
	Transform cTrans;
	
	public VFX(Transform obj){
		cTrans = obj;
		aParticles = cTrans.GetComponentsInChildren<ParticleSystem>();
	}

	public void SetEmit(bool emit){
		foreach (ParticleSystem particle in aParticles) {
			particle.enableEmission = emit;
		}
	}

	public void SetPlay(bool play){
		foreach (ParticleSystem particle in aParticles) {
			if(particle == null) continue;
			if(play) particle.Play();
			else particle.Pause();
		}
	}

	public GameObject gameObject{
		get{ return cTrans.gameObject; }
	}

}
