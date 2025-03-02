using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    private AudioSource musicSource;
    private AudioSource secondMusicSource; // For crossfading
    private int currentTrackIndex = -1;
    private bool isSecondSourcePlaying = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Create audio sources
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.loop = loopMusic;
        musicSource.volume = musicVolume;
        musicSource.playOnAwake = false;
        
        secondMusicSource = gameObject.AddComponent<AudioSource>();
        secondMusicSource.loop = loopMusic;
        secondMusicSource.volume = 0f;
        secondMusicSource.playOnAwake = false;
    }

    private void Start()
    {
        // Start playing a random track if we have any
        if (backgroundMusicTracks.Length > 0)
        {
            PlayRandomBackgroundMusic();
        }
    }
    
    private void Update()
    {
        // If auto-change is enabled and the current track has finished playing
        if (autoChangeTrack && !loopMusic && musicSource.isPlaying == false && secondMusicSource.isPlaying == false)
        {
            PlayRandomBackgroundMusic();
        }
    }
    
    /// <summary>
    /// Plays a random background music track from the available tracks
    /// </summary>
    public void PlayRandomBackgroundMusic()
    {
        if (backgroundMusicTracks.Length == 0)
        {
            Debug.LogWarning("No background music tracks assigned to AudioManager");
            return;
        }
        
        // Select a random track (different from the current one if possible)
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
        
        // Play the track (with crossfade if enabled)
        if (crossfadeMusic && musicSource.isPlaying)
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
    
    /// <summary>
    /// Crossfades between two music tracks
    /// </summary>
    private IEnumerator CrossfadeTracks(AudioClip newTrack)
    {
        // Determine which source is currently playing and which will play the new track
        AudioSource sourceToFadeOut = isSecondSourcePlaying ? secondMusicSource : musicSource;
        AudioSource sourceToFadeIn = isSecondSourcePlaying ? musicSource : secondMusicSource;
        
        // Set up the new track
        sourceToFadeIn.clip = newTrack;
        sourceToFadeIn.volume = 0f;
        sourceToFadeIn.Play();
        
        // Crossfade
        float timer = 0f;
        while (timer < crossfadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / crossfadeDuration;
            
            sourceToFadeOut.volume = musicVolume * (1f - t);
            sourceToFadeIn.volume = musicVolume * t;
            
            yield return null;
        }
        
        // Ensure final volumes are set correctly
        sourceToFadeOut.Stop();
        sourceToFadeOut.volume = 0f;
        sourceToFadeIn.volume = musicVolume;
        
        // Toggle which source is playing
        isSecondSourcePlaying = !isSecondSourcePlaying;
    }
    
    /// <summary>
    /// Changes to a specific track by index
    /// </summary>
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
    
    /// <summary>
    /// Sets the music volume
    /// </summary>
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
    
    /// <summary>
    /// Pauses the currently playing music
    /// </summary>
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
    
    /// <summary>
    /// Resumes the paused music
    /// </summary>
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