using System;
using UnityEngine;

namespace Pixel_Adventure_1.Assets.Scripts
{
    public class FallingPlatformController : MonoBehaviour
    {
        [SerializeField] private TargetJoint2D _targetJoint2D;
        [SerializeField] private PolygonCollider2D _polygonCollider2D;

        private float _fallingTime = .5f;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Invoke("Fall", _fallingTime);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.gameObject.CompareTag("GameOver"))
                Destroy(gameObject);
        }

        private void Fall()
        {
            _targetJoint2D.enabled = false;
            _polygonCollider2D.isTrigger = true;
        }
    }
}
