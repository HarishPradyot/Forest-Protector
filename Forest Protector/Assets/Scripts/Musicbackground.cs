using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Musicbackground : MonoBehaviour
{
     public AudioSource AudioSource;
    private float musicvolume = 1f;

    // Start is called before the first frame update
    void Start()
    {
      AudioSource.Play();  
    }

    // Update is called once per frame
    void Update()
    {
        AudioSource.volume = musicvolume;
    }
    public void updateVolume(float volume){
        musicvolume= volume;
    }
}
