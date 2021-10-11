using System;
using System.Collections;
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
        //Debug.Log($"{_parent.gameObject.name} killed {collision2D.collider.gameObject.name}");
        //collision2D.collider.enabled = false;
        collision2D.otherCollider.enabled = false;
        StartCoroutine(ActivateCollider(collision2D.otherCollider));
        OnJumpPlayerHead?.Invoke(new JumpPlayerHeadArgs(collision2D.collider));
    }

    private IEnumerator ActivateCollider(Behaviour c)
    {
        yield return new WaitForSeconds(0.2f);
        c.enabled = true;
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