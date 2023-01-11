using UnityEngine.Audio;
using System; 
using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{
    public Sound[] sounds;

    void Awake(){
        foreach(Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }
    void Start() {
        if(gameObject.name == "MusicPlayer")
        {
            DontDestroyOnLoad(gameObject);
            play("AmbientMusic");
        }
    }
    

    public void play(string name)
    {
        print(name);
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }
    public void stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Stop();
    }
}


[System.Serializable]
public class Sound 
{
    public string name;
    public AudioClip clip;
    [Range(0, 1)]
    public float volume;
    [Range(.1f, 3)]
    public float pitch;
    [HideInInspector]
    public AudioSource source;
    public bool loop;
}
