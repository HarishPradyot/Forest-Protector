using UnityEngine;
using UnityEngine.UI;

public class audiochange : MonoBehaviour
{

    private string FirstPlay = "FirstPlay";
    private string SoundEffectsPref = "SoundEffectsPref";
    private int firstPlayInt;
    public Slider soundEffectsSlider;
    private float soundEffectsFloat;
    public AudioSource[] soundEffectsAudio;
    public void  SaveSoundSettings()
    {
        PlayerPrefs.SetFloat(SoundEffectsPref, soundEffectsSlider.value);
    }
    void OnApplicationFocus(bool inFocus)
    {
        if (!inFocus)
        {
            SaveSoundSettings();
        }
    }
    public void UpdateSound()
    {
        for (int i = 0; i < soundEffectsAudio.Length; i++)
        {
            soundEffectsAudio[i].volume = soundEffectsSlider.value;
        }
    }

    // Reference to Audio Source component
    private AudioSource audioSrc;

    // Music volume variable that will be modified
    // by dragging slider knob
    private float musicVolume = 1f;

    // Use this for initialization
    void Start()
    {

        // Assign Audio Source component to control it
        audioSrc = GetComponent<AudioSource>();
        firstPlayInt = PlayerPrefs.GetInt(FirstPlay);
        if (!PlayerPrefs.HasKey(SoundEffectsPref))
        {
            soundEffectsFloat = .75f;
            if(soundEffectsSlider){
                soundEffectsSlider.value = soundEffectsFloat;
            }
            PlayerPrefs.SetFloat(SoundEffectsPref, soundEffectsFloat);
            PlayerPrefs.SetInt(FirstPlay, -1);
        }
        else
        {
            soundEffectsFloat = PlayerPrefs.GetFloat(SoundEffectsPref);
            if(soundEffectsSlider){
                soundEffectsSlider.value = soundEffectsFloat;
            }
           
        }
    }

    // Update is called once per frame
    void Update()
    {

        // Setting volume option of Audio Source to be equal to musicVolume
        // audioSrc.volume = musicVolume;
        if(soundEffectsSlider){
            UpdateSound();
        }
    }

    // Method that is called by slider game object
    // This method takes vol value passed by slider
    // and sets it as musicValue
    public void SetVolume(float vol)
    {
        musicVolume = vol;
       
    }
}