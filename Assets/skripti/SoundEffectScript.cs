using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectScript : MonoBehaviour
{
    public AudioClip[] soundEffects;
    public AudioSource audioSource;
    
    public void Hover()
    {
        audioSource.PlayOneShot(soundEffects[0]);
    }

    public void Click()
    {
        audioSource.PlayOneShot(soundEffects[1]);
    }

    public void OnDice()
    {
        audioSource.loop = true;
        audioSource.clip = soundEffects[2];
        audioSource.Play();

    }
    public void PlayButton()
    {
        audioSource.PlayOneShot(soundEffects[3]);
    }
    public void CancelButton()
    {
        audioSource.PlayOneShot(soundEffects[4]);
    }
    public void NameField()
    {
        audioSource.PlayOneShot(soundEffects[5]);
    }
}
