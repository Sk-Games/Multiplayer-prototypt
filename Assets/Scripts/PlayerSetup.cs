using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
//using UnityStandardAssets.Characters.FirstPerson;
using StarterAssets;

public class PlayerSetup : MonoBehaviourPunCallbacks
{
    public GameObject[] localPlayerItems;
    public GameObject[] remotePlayerItems;

    public GameObject playerCanvas;
    public GameObject cameraHolder;

    public GameObject virtualCam;
    public CharacterController cc;


    // Start is called before the first frame update
    void Start()
    {
        if (photonView.IsMine)
        {
            foreach (GameObject g in localPlayerItems)
            {
                g.SetActive(true);
            }
            foreach (GameObject g in remotePlayerItems)
            {
                g.SetActive(false);
            }
            //GetComponent<RigidbodyFirstPersonController>().enabled = true;
            //GetComponent<PlayerMovementController>().enabled = true;
            GetComponent<FirstPersonController>().enabled = true;
            GetComponent<BasicRigidBodyPush>().enabled = true;
            GetComponent<StarterAssetsInputs>().enabled = true;
            playerCanvas.SetActive(true);
            cameraHolder.SetActive(true);
            virtualCam.SetActive(true);
            GetComponent<CharacterController>().enabled = true;

        }
        else
        {
            foreach (GameObject g in localPlayerItems)
            {
                g.SetActive(false);
            }
            foreach (GameObject g in remotePlayerItems)
            {
                g.SetActive(true);
            }
            //GetComponent<RigidbodyFirstPersonController>().enabled = false;
            //GetComponent<PlayerMovementController>().enabled = false;
            //GetComponent<FirstPersonController>().enabled = false;
            GetComponent<BasicRigidBodyPush>().enabled = false;
            GetComponent<StarterAssetsInputs>().enabled = false;
            virtualCam.SetActive(false);
            playerCanvas.SetActive(false);
            cameraHolder.SetActive(false);
            GetComponent<CharacterController>().enabled = false;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
