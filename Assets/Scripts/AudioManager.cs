using UnityEngine;

/// <summary>
/// Простая версия AudioManager для проверки компиляции
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Range(0f, 1f)]
    public float MusicVolume = 0.7f;
    
    [Range(0f, 1f)]
    public float SFXVolume = 1f;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void PlaySFX(string clipName, float volume = 1f)
    {
        Debug.Log($"AudioManager: Playing SFX {clipName} at volume {volume}");
    }
    
    public void PlayMusic(string clipName, bool fadeIn = true)
    {
        Debug.Log($"AudioManager: Playing music {clipName}");
    }
    
    public void OnTankFire()
    {
        PlaySFX("tank_fire");
    }
    
    public void OnBulletHit()
    {
        PlaySFX("bullet_hit");
    }
    
    public void OnBulletBounce()
    {
        PlaySFX("bullet_bounce", 0.7f);
    }
    
    public void OnTankDestroyed()
    {
        PlaySFX("tank_destroyed");
    }
    
    public void OnButtonClick()
    {
        PlaySFX("button_click", 0.8f);
    }
}