using Photon.Pun;
using UnityEngine;

namespace Project
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private PhotonView photonView;
        [SerializeField] private CharacterController characterController;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float rotationSpeed;

        void Update()
        {
            if (photonView.IsMine)
            {
                Movement();
            }
        }

        private void Movement()
        {
            if (Input.GetKey(KeyCode.W))
            {
                characterController.Move(transform.forward * Time.deltaTime * movementSpeed);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                characterController.Move(-transform.right * Time.deltaTime * movementSpeed);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                characterController.Move(-transform.forward * Time.deltaTime * movementSpeed);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                characterController.Move(transform.right * Time.deltaTime * movementSpeed);
            }
        }
    }
}
