using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXManager : MonoBehaviour
{
    public VisualEffect footStep;
    public ParticleSystem blade01;
    public VisualEffect blink;
    public VisualEffect healthVFX;
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
    public void PlayHealthVFX()
    {
        healthVFX.Play(); 
    }
    public void PlayBlinkVFX()
    {
        blink.Play();
    }
}
