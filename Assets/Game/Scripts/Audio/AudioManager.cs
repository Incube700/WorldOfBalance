using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/// <summary>
/// Менеджер звуковых эффектов и музыки для WorldOfBalance.
/// Обеспечивает централизованное управление всеми аудио ресурсами игры.
/// </summary>
public class AudioManager : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        
        [Header("Music Clips")]
        [SerializeField] private AudioClip mainMenuMusic;
        [SerializeField] private AudioClip gameplayMusic;
        [SerializeField] private AudioClip gameOverMusic;
        
        [Header("SFX Clips")]
        [SerializeField] private AudioClip tankFireSound;
        [SerializeField] private AudioClip bulletHitSound;
        [SerializeField] private AudioClip bulletBounceSound;
        [SerializeField] private AudioClip tankDestroyedSound;
        [SerializeField] private AudioClip buttonClickSound;
        
        [Header("Volume Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float musicVolume = 0.7f;
        [Range(0f, 1f)]
        [SerializeField] private float sfxVolume = 1f;
        
        // Audio clips dictionary for easy access
        private Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();
        
        // Singleton pattern
        public static AudioManager Instance { get; private set; }
        
        // Properties
        public float MusicVolume
        {
            get => musicVolume;
            set
            {
                musicVolume = Mathf.Clamp01(value);
                if (musicSource != null)
                    musicSource.volume = musicVolume;
            }
        }
        
        public float SFXVolume
        {
            get => sfxVolume;
            set
            {
                sfxVolume = Mathf.Clamp01(value);
                if (sfxSource != null)
                    sfxSource.volume = sfxVolume;
            }
        }
        
        void Awake()
        {
            // Singleton setup
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudio();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        void InitializeAudio()
        {
            // Create audio sources if not assigned
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }
            
            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
            
            // Set initial volumes
            musicSource.volume = musicVolume;
            sfxSource.volume = sfxVolume;
            
            // Populate audio clips dictionary
            PopulateAudioDictionary();
            
            Debug.Log("AudioManager initialized successfully");
        }
        
        void PopulateAudioDictionary()
        {
            // Music clips
            if (mainMenuMusic != null) audioClips["mainmenu_music"] = mainMenuMusic;
            if (gameplayMusic != null) audioClips["gameplay_music"] = gameplayMusic;
            if (gameOverMusic != null) audioClips["gameover_music"] = gameOverMusic;
            
            // SFX clips
            if (tankFireSound != null) audioClips["tank_fire"] = tankFireSound;
            if (bulletHitSound != null) audioClips["bullet_hit"] = bulletHitSound;
            if (bulletBounceSound != null) audioClips["bullet_bounce"] = bulletBounceSound;
            if (tankDestroyedSound != null) audioClips["tank_destroyed"] = tankDestroyedSound;
            if (buttonClickSound != null) audioClips["button_click"] = buttonClickSound;
        }
        
        /// <summary>
        /// Воспроизводит звуковой эффект по имени
        /// </summary>
        /// <param name="clipName">Имя аудиоклипа</param>
        /// <param name="volume">Громкость (по умолчанию 1.0)</param>
        public void PlaySFX(string clipName, float volume = 1f)
        {
            if (audioClips.TryGetValue(clipName.ToLower(), out AudioClip clip))
            {
                sfxSource.PlayOneShot(clip, volume * sfxVolume);
            }
            else
            {
                Debug.LogWarning($"AudioManager: SFX clip '{clipName}' not found!");
            }
        }
        
        /// <summary>
        /// Воспроизводит музыку по имени
        /// </summary>
        /// <param name="clipName">Имя музыкального клипа</param>
        /// <param name="fadeIn">Плавное появление музыки</param>
        public void PlayMusic(string clipName, bool fadeIn = true)
        {
            if (audioClips.TryGetValue(clipName.ToLower(), out AudioClip clip))
            {
                if (musicSource.clip == clip && musicSource.isPlaying)
                    return; // Already playing this music
                
                if (fadeIn && musicSource.isPlaying)
                {
                    StartCoroutine(CrossfadeMusic(clip));
                }
                else
                {
                    musicSource.clip = clip;
                    musicSource.Play();
                }
            }
            else
            {
                Debug.LogWarning($"AudioManager: Music clip '{clipName}' not found!");
            }
        }
        
        /// <summary>
        /// Останавливает музыку
        /// </summary>
        /// <param name="fadeOut">Плавное исчезновение</param>
        public void StopMusic(bool fadeOut = true)
        {
            if (fadeOut)
            {
                StartCoroutine(FadeOutMusic());
            }
            else
            {
                musicSource.Stop();
            }
        }
        
        /// <summary>
        /// Ставит музыку на паузу
        /// </summary>
        public void PauseMusic()
        {
            musicSource.Pause();
        }
        
        /// <summary>
        /// Возобновляет музыку
        /// </summary>
        public void ResumeMusic()
        {
            musicSource.UnPause();
        }
        
        /// <summary>
        /// Воспроизводит звук выстрела танка
        /// </summary>
        public void OnTankFire()
        {
            PlaySFX("tank_fire");
        }
        
        /// <summary>
        /// Воспроизводит звук попадания снаряда
        /// </summary>
        public void OnBulletHit()
        {
            PlaySFX("bullet_hit");
        }
        
        /// <summary>
        /// Воспроизводит звук рикошета снаряда
        /// </summary>
        public void OnBulletBounce()
        {
            PlaySFX("bullet_bounce", 0.7f);
        }
        
        /// <summary>
        /// Воспроизводит звук уничтожения танка
        /// </summary>
        public void OnTankDestroyed()
        {
            PlaySFX("tank_destroyed");
        }
        
        /// <summary>
        /// Воспроизводит звук нажатия кнопки
        /// </summary>
        public void OnButtonClick()
        {
            PlaySFX("button_click", 0.8f);
        }
        
        IEnumerator CrossfadeMusic(AudioClip newClip)
        {
            float fadeDuration = 1f;
            float startVolume = musicSource.volume;
            
            // Fade out current music
            for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
                yield return null;
            }
            
            // Switch to new clip
            musicSource.clip = newClip;
            musicSource.Play();
            
            // Fade in new music
            for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
            {
                musicSource.volume = Mathf.Lerp(0, musicVolume, t / fadeDuration);
                yield return null;
            }
            
            musicSource.volume = musicVolume;
        }
        
        IEnumerator FadeOutMusic()
        {
            float fadeDuration = 1f;
            float startVolume = musicSource.volume;
            
            for (float t = 0; t < fadeDuration; t += Time.unscaledDeltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
                yield return null;
            }
            
            musicSource.Stop();
            musicSource.volume = musicVolume;
        }
    }