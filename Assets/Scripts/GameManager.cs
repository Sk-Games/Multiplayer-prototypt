using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    GameObject playerPrefab;


    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            int randomNumber = Random.Range(-20,80);
            PhotonNetwork.Instantiate(playerPrefab.name,new Vector3(randomNumber,0f,randomNumber),Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
