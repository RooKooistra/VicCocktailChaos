using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance {  get; private set; }

    [SerializeField] private AudioClipRefsSO audioClipRefsSO;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"There is more than one {Instance.name}! {transform}  -  {Instance}");
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;
        DeliveryManager.Instance.OnRecipeFail += DeliveryManager_OnRecipeFail;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        Player.Instance.OnPickedSomething += Player_OnPickedSomething1;
        BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
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

    private void PlaySound(AudioClip audioClip, Vector3 AudioPosition, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, AudioPosition, volume);
    }

    private void PlaySound(AudioClip[] audioClipArray, Vector3 AudioPosition, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClipArray[Random.Range(0,audioClipArray.Length)], AudioPosition, volume);
    }

    public void PlayFootstepSound(Vector3 playerPosition, float volume = 1f)
    {
        PlaySound(audioClipRefsSO.footstep, playerPosition, volume);
    }
}
