using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource uiSource;

    [SerializeField] private AudioClip defaultMusic; // Музыка по умолчанию для первой сцены
    [SerializeField] private AudioClip loadingMusic;   // Музыка загрузочного экрана

    [Header("Mixer Snapshots")]
    [SerializeField] private AudioMixerSnapshot normalSnapshot;   // Состояние нормального звука
    [SerializeField] private AudioMixerSnapshot loadingSnapshot;  // Snapshot, где звук сцены выключен
    [SerializeField] private float snapshotTransitionTime = 0.5f;   // Время перехода

    [Header("Mini-game")]
    [SerializeField] private AudioClip miniGameMusic;   // Музыка для мини-игры

    private AudioClip _previousMusic;                   // Сохраняем, что играло раньше
    private bool _isInMiniGame;

    private void Awake()
    {
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
    }

    private void Start()
    {
        LoadVolumeSettings();

        if (defaultMusic != null)
        {
            PlayMusic(defaultMusic);
        }
    }

    public static AudioManager GetInstance()
    {
        return Instance;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.isPlaying && musicSource.clip == clip) return;
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    /// <summary>
    /// Проигрывает музыку загрузочного экрана.
    /// </summary>
    public void PlayLoadingMusic()
    {
        if (loadingMusic != null)
        {
            PlayMusic(loadingMusic);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayUI(AudioClip clip)
    {
        uiSource.PlayOneShot(clip);
    }

    public void SetVolume(string parameter, float volume)
    {
        if (volume == 0f)
        {
            audioMixer.SetFloat(parameter, -80f);
        }
        else
        {
            audioMixer.SetFloat(parameter, Mathf.Log10(volume) * 20);
        }
    }

    private void LoadVolumeSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float uiVolume = PlayerPrefs.GetFloat("UIVolume", 1f);

        SetVolume("Volume_Music", musicVolume);
        SetVolume("Volume_SFX", sfxVolume);
        SetVolume("Volume_UI", uiVolume);
    }

    /// <summary>
    /// Смена музыки в зависимости от названия сцены.
    /// Этот метод вызывается из загрузочного экрана после его закрытия.
    /// </summary>
    public void ChangeMusicForScene(string sceneName)
    {
        AudioClip newMusic = null;

        switch (sceneName)
        {
            case "MainMenu":
                newMusic = Resources.Load<AudioClip>("Music/MainMenuMusic");
                break;
            case "StartDay":
                newMusic = Resources.Load<AudioClip>("Music/StartDayMusic");
                break;
            case "MainRoom":
                newMusic = Resources.Load<AudioClip>("Music/MainRoomMusic");
                break;
            case "Forge":
                newMusic = Resources.Load<AudioClip>("Music/ForgeMusic");
                break;
            case "Storage":
                newMusic = Resources.Load<AudioClip>("Music/StorageMusic");
                break;
        }

        if (newMusic != null)
        {
            PlayMusic(newMusic);
        }
    }

    /// <summary>
    /// Переключает mixer на snapshot для загрузочного экрана, где звуки сцены выключены.
    /// </summary>
    public void ActivateLoadingSnapshot()
    {
        // Принудительно устанавливаем громкость для SFX и UI на -80 дБ независимо от пользовательских настроек
        audioMixer.SetFloat("Volume_SFX", -80f);
        audioMixer.SetFloat("Volume_UI", -80f);

        // Затем переходим на snapshot загрузочного экрана
        if (loadingSnapshot != null)
        {
            loadingSnapshot.TransitionTo(snapshotTransitionTime);
        }
    }

    /// <summary>
    /// Возвращает mixer в нормальное состояние.
    /// </summary>
    public void ActivateNormalSnapshot()
    {
        // Считываем настройки звука из PlayerPrefs
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float uiVolume = PlayerPrefs.GetFloat("UIVolume", 1f);
        SetVolume("Volume_SFX", sfxVolume);
        SetVolume("Volume_UI", uiVolume);

        if (normalSnapshot != null)
        {
            normalSnapshot.TransitionTo(snapshotTransitionTime);
        }
    }

    public void StartMiniGameMusic()
    {
        if (_isInMiniGame || miniGameMusic == null) return;

        _previousMusic = musicSource.clip;  // запоминаем текущий трек
        _isInMiniGame = true;
        PlayMusic(miniGameMusic);
    }

    public void StopMiniGameMusic()
    {
        if (!_isInMiniGame) return;

        _isInMiniGame = false;
        // Если предыдущий трек был, вернём его; иначе ничего не меняем
        if (_previousMusic != null)
            PlayMusic(_previousMusic);
    }
    public void ReloadVolumeSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float uiVolume = PlayerPrefs.GetFloat("UIVolume", 1f);

        SetVolume("Volume_Music", musicVolume);
        SetVolume("Volume_SFX", sfxVolume);
        SetVolume("Volume_UI", uiVolume);
    }
}
