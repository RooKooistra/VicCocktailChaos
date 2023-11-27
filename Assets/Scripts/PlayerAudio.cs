using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{ 
    private Player player;
    private float footstepTimer;

    [SerializeField] private float footstepTimerMax = 0.2f;
    [SerializeField] private float footstepVolume = 1f;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        footstepTimer -= Time.deltaTime;
        if (footstepTimer > 0f) return;

        footstepTimer = footstepTimerMax;

        if (!player.IsWalking()) return;
        AudioManager.Instance.PlayFootstepSound(transform.position, footstepVolume);
    }
}
