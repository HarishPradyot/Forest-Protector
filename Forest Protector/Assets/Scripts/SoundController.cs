using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    //Menu Audio Source
    [SerializeField]
    private AudioSource AudioSource;
    
    // Music - Menu Music and Sound - In Game Music
    [SerializeField]
    private Slider musicSlider, soundSlider;
    private float musicVolume=1f, soundVolume=0.5f;

    // Start is called before the first frame update
    void Start()
    {
        musicSlider.value=musicVolume;
        soundSlider.value=soundVolume;
        AudioSource.Play();  
    }

    // Update is called once per frame
    void Update()
    {
        AudioSource.volume=musicVolume;
    }
    public void updateVolume()
    {
        musicVolume=musicSlider.value;
    }
}
