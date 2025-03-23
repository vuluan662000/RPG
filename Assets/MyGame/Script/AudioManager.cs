using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    public AudioClip backGround;

    private void Start()
    {
        musicSource.clip = backGround;
        musicSource.Play();
    }
}
