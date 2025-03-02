using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Background Music")]
    [Tooltip("List of background music tracks to play randomly")]
    public AudioClip[] backgroundMusicTracks;
    
    [Tooltip("Should the music loop after playing?")]
    public bool loopMusic = true;
    
    [Tooltip("Volume of the background music (0-1)")]
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    
    [Tooltip("Should music tracks crossfade when changing?")]
    public bool crossfadeMusic = true;
    
    [Tooltip("Duration of crossfade in seconds")]
    [Range(0f, 5f)]
    public float crossfadeDuration = 1.5f;
    
    [Tooltip("Should a new random track be selected when the current one finishes?")]
    public bool autoChangeTrack = true;
    
    [Header("Scene Handling")]
    [Tooltip("Should the AudioManager persist across scenes?")]
    public bool persistAcrossScenes = false;
    
    [Tooltip("Should music change when scenes change (even if persisting)?")]
    public bool changeMusicOnSceneChange = true;
    
    private AudioSource musicSource;
    private AudioSource secondMusicSource; //for crossfading
    private int currentTrackIndex = -1;
    private bool isSecondSourcePlaying = false;
    private string currentSceneName;

    private void Awake()
    {
        //record the current scene
        currentSceneName = SceneManager.GetActiveScene().name;
        
        //check if this is the primary AudioManager or a duplicate
        if (Instance != null && Instance != this)
        {
            //if we want to change music for each scene, let the existing manager know
            if (changeMusicOnSceneChange)
            {
                Instance.OnSceneChanged(currentSceneName);
            }
            
            //destroy this duplicate AudioManager
            Destroy(gameObject);
            return;
        }
        
        //this is the first AudioManager we've encountered
        Instance = this;
        
        //only mark DontDestroyOnLoad if we want persistence
        if (persistAcrossScenes)
        {
            DontDestroyOnLoad(gameObject);
            
            //register for scene change events
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        //create audio sources
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = loopMusic;
        musicSource.volume = musicVolume;
        musicSource.playOnAwake = false;
        
        secondMusicSource = gameObject.AddComponent<AudioSource>();
        secondMusicSource.loop = loopMusic;
        secondMusicSource.volume = 0f;
        secondMusicSource.playOnAwake = false;
    }

    private void OnDestroy()
    {
        //unregister from scene events when destroyed
        if (persistAcrossScenes && Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    //called when a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (changeMusicOnSceneChange && scene.name != currentSceneName)
        {
            OnSceneChanged(scene.name);
        }
    }
    
    //handle scene change logic
    private void OnSceneChanged(string newSceneName)
    {
        currentSceneName = newSceneName;
        
        //play a new random track for the new scene
        if (backgroundMusicTracks.Length > 0)
        {
            PlayRandomBackgroundMusic();
        }
    }

    private void Start()
    {
        //start playing a random track if we have any
        if (backgroundMusicTracks.Length > 0)
        {
            PlayRandomBackgroundMusic();
        }
    }
    
    private void Update()
    {
        //if auto-change is enabled and the current track has finished playing
        if (autoChangeTrack && !loopMusic && musicSource.isPlaying == false && secondMusicSource.isPlaying == false)
        {
            PlayRandomBackgroundMusic();
        }
    }
    public void PlayRandomBackgroundMusic()
    {
        if (backgroundMusicTracks.Length == 0)
        {
            Debug.LogWarning("No background music tracks assigned to AudioManager");
            return;
        }
        
        //select a random track (different from the current one if possible)
        int newTrackIndex = currentTrackIndex;
        if (backgroundMusicTracks.Length > 1)
        {
            while (newTrackIndex == currentTrackIndex)
            {
                newTrackIndex = Random.Range(0, backgroundMusicTracks.Length);
            }
        }
        else
        {
            newTrackIndex = 0;
        }
        
        currentTrackIndex = newTrackIndex;
        AudioClip trackToPlay = backgroundMusicTracks[currentTrackIndex];
        
        //play the track (with crossfade if enabled)
        if (crossfadeMusic && (musicSource.isPlaying || secondMusicSource.isPlaying))
        {
            StartCoroutine(CrossfadeTracks(trackToPlay));
        }
        else
        {
            musicSource.clip = trackToPlay;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }
    private IEnumerator CrossfadeTracks(AudioClip newTrack)
    {
        //determine which source is currently playing and which will play the new track
        AudioSource sourceToFadeOut = isSecondSourcePlaying ? secondMusicSource : musicSource;
        AudioSource sourceToFadeIn = isSecondSourcePlaying ? musicSource : secondMusicSource;
        
        //set up the new track
        sourceToFadeIn.clip = newTrack;
        sourceToFadeIn.volume = 0f;
        sourceToFadeIn.Play();
        
        //crossfade
        float timer = 0f;
        while (timer < crossfadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / crossfadeDuration;
            
            sourceToFadeOut.volume = musicVolume * (1f - t);
            sourceToFadeIn.volume = musicVolume * t;
            
            yield return null;
        }
        
        //ensure final volumes are set correctly
        sourceToFadeOut.Stop();
        sourceToFadeOut.volume = 0f;
        sourceToFadeIn.volume = musicVolume;
        
        //toggle which source is playing
        isSecondSourcePlaying = !isSecondSourcePlaying;
    }
    
    public void ChangeBackgroundMusic(int trackIndex)
    {
        if (trackIndex < 0 || trackIndex >= backgroundMusicTracks.Length)
        {
            Debug.LogWarning($"Track index {trackIndex} is out of range");
            return;
        }
        
        currentTrackIndex = trackIndex;
        AudioClip trackToPlay = backgroundMusicTracks[currentTrackIndex];
        
        if (crossfadeMusic && (musicSource.isPlaying || secondMusicSource.isPlaying))
        {
            StartCoroutine(CrossfadeTracks(trackToPlay));
        }
        else
        {
            musicSource.clip = trackToPlay;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        
        if (isSecondSourcePlaying)
        {
            secondMusicSource.volume = musicVolume;
        }
        else
        {
            musicSource.volume = musicVolume;
        }
    }
    
    public void PauseMusic()
    {
        if (isSecondSourcePlaying)
        {
            secondMusicSource.Pause();
        }
        else
        {
            musicSource.Pause();
        }
    }
    
    public void ResumeMusic()
    {
        if (isSecondSourcePlaying)
        {
            secondMusicSource.UnPause();
        }
        else
        {
            musicSource.UnPause();
        }
    }
} 