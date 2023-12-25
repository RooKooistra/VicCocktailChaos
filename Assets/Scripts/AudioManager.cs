using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {  get; private set; }

    private const string PLAYER_PREFS_SFX_VOLUME = "SoundEffectsVolume";
    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";

    [SerializeField] private AudioClipRefsSO audioClipRefsSO;

    private AudioSource audioSource;

    private float sfxVolume = 1f;
    private float musicVolume = 0.5f;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"There is more than one {Instance.name}! {transform}  -  {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        audioSource = GetComponent<AudioSource>();

        musicVolume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, musicVolume);
        sfxVolume = PlayerPrefs.GetFloat(PLAYER_PREFS_SFX_VOLUME, sfxVolume);

        audioSource.volume = musicVolume;
    }
    private void Start()
    {
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;
        DeliveryManager.Instance.OnRecipeFail += DeliveryManager_OnRecipeFail;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        //Player.Instance.OnPickedSomething += Player_OnPickedSomething1;
        BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
    }

    private void OnDestroy()
    {
        DeliveryManager.Instance.OnRecipeCompleted -= DeliveryManager_OnRecipeCompleted;
        DeliveryManager.Instance.OnRecipeFail -= DeliveryManager_OnRecipeFail;
        CuttingCounter.OnAnyCut -= CuttingCounter_OnAnyCut;
        // Player.Instance.OnPickedSomething -= Player_OnPickedSomething1;
        BaseCounter.OnAnyObjectPlacedHere -= BaseCounter_OnAnyObjectPlacedHere;
        TrashCounter.OnAnyObjectTrashed -= TrashCounter_OnAnyObjectTrashed;
    }

    private void TrashCounter_OnAnyObjectTrashed(Transform audioTransform)
    {
        PlaySound(audioClipRefsSO.trash, audioTransform.position);
    }

    private void BaseCounter_OnAnyObjectPlacedHere(Transform audioTransform)
    {
        PlaySound(audioClipRefsSO.objectDrop, audioTransform.position);
    }

    private void Player_OnPickedSomething1(Transform audioTransform)
    {
        PlaySound(audioClipRefsSO.objectPickup, audioTransform.position);
    }

    private void CuttingCounter_OnAnyCut(Transform audioTransform)
    {
        PlaySound(audioClipRefsSO.chop, audioTransform.position);
    }

    private void DeliveryManager_OnRecipeFail(Transform audioTransform)
    {
        PlaySound(audioClipRefsSO.deliveryFail, audioTransform.position);
    }

    private void DeliveryManager_OnRecipeCompleted(List<RecipeSO> notUsed, Transform audioTransform)
    {
        PlaySound(audioClipRefsSO.deliverySuccess, audioTransform.position);
    }

    private void PlaySound(AudioClip audioClip, Vector3 AudioPosition, float volumeMultiplyer = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, AudioPosition, volumeMultiplyer * sfxVolume);
    }

    private void PlaySound(AudioClip[] audioClipArray, Vector3 AudioPosition, float volumeMultiplyer = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClipArray[Random.Range(0,audioClipArray.Length)], AudioPosition, volumeMultiplyer * sfxVolume);
    }

    public void PlayFootstepSound(Vector3 playerPosition, float volumeMultiplyer = 1f)
    {
        PlaySound(audioClipRefsSO.footstep, playerPosition, volumeMultiplyer * sfxVolume);
    }

    public void ChangeSFXVolume()
    {
        sfxVolume += 0.1f;
        if (sfxVolume > 1f) sfxVolume = 0;

        PlayerPrefs.SetFloat(PLAYER_PREFS_SFX_VOLUME, sfxVolume);
    }

    public void ChangeMusicVolume()
    {
        musicVolume += 0.1f;
        if (musicVolume > 1f) musicVolume = 0;

        audioSource.volume = musicVolume;

        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, musicVolume);
    }

    public float GetSFXVolume() 
    { 
        return sfxVolume; 
    }

    public float GetMusicVolume()
    {
        return musicVolume;
    }
}
