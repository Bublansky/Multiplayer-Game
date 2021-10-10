using System;
using UnityEngine;

public class PlayerFeet : MonoBehaviour
{
    [SerializeField] private Transform _parent;
    
    public Action<JumpPlayerHeadArgs> OnJumpPlayerHead;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerKill(collision);
        }
    }

    private void HandlePlayerKill(Collision2D collision2D)
    {
        Debug.Log($"{_parent.gameObject.name} killed {collision2D.collider.gameObject.name}");

        OnJumpPlayerHead?.Invoke(new JumpPlayerHeadArgs(collision2D.collider));
    }

    public readonly struct JumpPlayerHeadArgs
    {
        public readonly Collider2D Collider;

        public JumpPlayerHeadArgs(Collider2D collider)
        {
            Collider = collider;
        }
    }
}