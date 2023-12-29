using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    private const string IS_WALKING = "IsWalking";

    private Animator animator;


    // components
    [SerializeField] private Player player;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;

        animator.SetBool(IS_WALKING, player.IsWalking());
    }
}
