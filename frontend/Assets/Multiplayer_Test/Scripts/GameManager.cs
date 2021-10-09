using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] _playerPrefabs;
    private readonly Vector3 _startPosition = Vector3.up;

    private void Start()
    {
        foreach (var playerPrefab in _playerPrefabs)
        {
            //TODO: instanciar baseado na quantidade de players conectados 
            /*PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0f,1f,0f)
                , Quaternion.identity, 0);*/

            Instantiate(playerPrefab, _startPosition, Quaternion.identity);
        }
    }
}