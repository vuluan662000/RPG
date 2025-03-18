using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXManager : MonoBehaviour
{
    public VisualEffect footStep;
    public ParticleSystem blade01;
    public void PlayeFootStepVFX()
    {
        footStep.Play();
    }
    public void StopFootStepVFX()
    {
        footStep.Stop();
    }   
    public void PlayyBlade01VFX()
    {
        blade01.Play();
    }    
}
