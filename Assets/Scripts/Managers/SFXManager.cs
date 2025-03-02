using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager Instance;

    [Header("Sound Effects")]
    [Tooltip("Volume of sound effects (0-1)")]
    [Range(0f, 1f)]
    public float sfxVolume = 0.7f;

    [Header("Turret Selection")]
    [Tooltip("Sound effects to play when turret is selected (random)")]
    public AudioClip[] turretSelectionSounds;
    
    [Tooltip("Volume multiplier for turret selection sounds")]
    [Range(0f, 2f)]
    public float turretSelectionVolume = 1.0f;

    [Header("Block Placement")]
    [Tooltip("Sound effects to play when block is placed (random)")]
    public AudioClip[] blockPlacementSounds;
    
    [Tooltip("Volume multiplier for block placement sounds")]
    [Range(0f, 2f)]
    public float blockPlacementVolume = 1.0f;

    [Header("Turret Upgrade")]
    [Tooltip("Sound effects to play when turret is upgraded (random)")]
    public AudioClip[] turretUpgradeSounds;
    
    [Tooltip("Volume multiplier for turret upgrade sounds")]
    [Range(0f, 2f)]
    public float turretUpgradeVolume = 1.0f;

    //audio source pool for playing multiple sounds simultaneously
    private List<AudioSource> audioSourcePool = new List<AudioSource>();
    private int maxAudioSources = 5; //maximum number of simultaneous sounds

    private void Awake()
    {
        //singleton pattern
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //initialize audio source pool
        InitializeAudioSourcePool();
    }

    private void InitializeAudioSourcePool()
    {
        //create initial pool of audio sources
        for (int i = 0; i < maxAudioSources; i++)
        {
            CreateNewAudioSource();
        }
    }

    private AudioSource CreateNewAudioSource()
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSourcePool.Add(audioSource);
        return audioSource;
    }

    private AudioSource GetAvailableAudioSource()
    {
        //find an available audio source that's not playing
        foreach (AudioSource source in audioSourcePool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        //if all sources are playing, return the oldest one
        return audioSourcePool[0];
    }
    public void PlayTurretSelectionSound()
    {
        if (turretSelectionSounds == null || turretSelectionSounds.Length == 0)
        {
            Debug.LogWarning("No turret selection sounds assigned to SFXManager");
            return;
        }

        //select random sound
        AudioClip soundToPlay = turretSelectionSounds[Random.Range(0, turretSelectionSounds.Length)];
        PlaySoundEffect(soundToPlay, turretSelectionVolume);
    }
    public void PlayBlockPlacementSound()
    {
        if (blockPlacementSounds == null || blockPlacementSounds.Length == 0)
        {
            Debug.LogWarning("No block placement sounds assigned to SFXManager");
            return;
        }

        //select random sound
        AudioClip soundToPlay = blockPlacementSounds[Random.Range(0, blockPlacementSounds.Length)];
        PlaySoundEffect(soundToPlay, blockPlacementVolume);
    }
    public void PlayTurretUpgradeSound()
    {
        if (turretUpgradeSounds == null || turretUpgradeSounds.Length == 0)
        {
            Debug.LogWarning("No turret upgrade sounds assigned to SFXManager");
            return;
        }

        //select random sound
        AudioClip soundToPlay = turretUpgradeSounds[Random.Range(0, turretUpgradeSounds.Length)];
        PlaySoundEffect(soundToPlay, turretUpgradeVolume);
    }

    public void PlaySoundEffect(AudioClip clip, float volumeMultiplier = 1.0f)
    {
        if (clip == null) return;

        AudioSource audioSource = GetAvailableAudioSource();
        audioSource.clip = clip;
        audioSource.volume = sfxVolume * volumeMultiplier;
        audioSource.Play();
    }
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }
} 