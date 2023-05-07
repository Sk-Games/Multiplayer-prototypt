using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField userNameText;
    public TMP_InputField roomNameText;
    public TMP_InputField maxPlayer;

    public GameObject PlayerNamePanel;
    public GameObject LobbyPanel;
    public GameObject RoomCreatePanel;
    public GameObject ConnectingPanel;
    public GameObject RoomListPanel; //this panel shows all the list of room created

    private Dictionary<string, RoomInfo> roomListData; //use to store the name of the room and the info of that room

    public GameObject roomListPrefab;
    public GameObject roomListParent;

    private Dictionary<string, GameObject> roomListGameObject;
    private Dictionary<int, GameObject> PlayerListGameObject;

    [Header("Inside Room Panel")]
    public GameObject InsideRoomPanel;
    public GameObject playerListItemPrefab;
    public GameObject playerListItemParent;
    public GameObject playButton;



    #region UnityMethods
    void Start()
    {
        ActivateMyPanel(PlayerNamePanel.name);
        roomListData = new Dictionary<string, RoomInfo>(); // initialization of dictionary
        roomListGameObject = new Dictionary<string, GameObject>();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Update is called once per frame
    void Update()
    {
       // Debug.Log("Network Status : " + PhotonNetwork.NetworkClientState);   
    }
    #endregion




    #region UiMethods

    public void OnLoginClick()
    {
        string name = userNameText.text;
        if (!string.IsNullOrEmpty(name))
        {
            PhotonNetwork.LocalPlayer.NickName = name;
            PhotonNetwork.ConnectUsingSettings();
            ActivateMyPanel(ConnectingPanel.name);
        }
        else
        {
            Debug.Log("Empty name");
        }
    }

    public void OnClickRoomCreate()
    {
        string roomName = roomNameText.text;

        if (!string.IsNullOrEmpty(roomName))
        {
            roomName = roomName + Random.Range(0,1000);
        }
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = (byte)int.Parse(maxPlayer.text);
        PhotonNetwork.CreateRoom(roomName,roomOptions);
    }

    public void OnCancelClick()
    {
        ActivateMyPanel(LobbyPanel.name);
    }

    public void RoomListButtonClicked()  // this is done when the room list button is clicked
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }
        ActivateMyPanel(RoomListPanel.name);
    }

    public void BackFromRoomList()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }
        ActivateMyPanel(LobbyPanel.name);
    }

    public void BackFromPlayerList()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
        ActivateMyPanel(LobbyPanel.name);
    }

    #endregion




    #region PHOTON_CALLBACKS

    public override void OnConnected()
    {
        Debug.Log("Connected to Internet");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " is connected to Photon....");
        ActivateMyPanel(LobbyPanel.name);
    }

    public override void OnCreatedRoom()
    {
        //base.OnCreatedRoom();
        Debug.Log(PhotonNetwork.CurrentRoom.Name + " is created !");
    }

    public override void OnJoinedRoom()
    {
        // base.OnJoinedRoom();
        Debug.Log(PhotonNetwork.LocalPlayer.NickName + " Room Join ");
        ActivateMyPanel(InsideRoomPanel.name);



        if (PlayerListGameObject == null)
        {
            PlayerListGameObject = new Dictionary<int, GameObject>();
        }

        if (PhotonNetwork.IsMasterClient)
        {
            playButton.SetActive(true);
        }
        else
        {
            playButton.SetActive(false);
        }

        foreach(Player p in PhotonNetwork.PlayerList)
        {
            Debug.Log("hello");
            GameObject playerListItem = Instantiate(playerListItemPrefab);
            playerListItem.transform.SetParent(playerListItemParent.transform);
            
            //playerListItem.transform.localScale = Vector3.one;
            playerListItem.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = p.NickName;
            if (p.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                playerListItem.transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                playerListItem.transform.GetChild(1).gameObject.SetActive(false);
            }

            PlayerListGameObject.Add(p.ActorNumber,playerListItem);
        }

    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        GameObject playerListItem = Instantiate(playerListItemPrefab);
        playerListItem.transform.SetParent(playerListItemParent.transform);

        //playerListItem.transform.localScale = Vector3.one;
        playerListItem.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = newPlayer.NickName;
        if (newPlayer.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            playerListItem.transform.GetChild(1).gameObject.SetActive(true);
        }
        else
        {
            playerListItem.transform.GetChild(1).gameObject.SetActive(false);
        }

        PlayerListGameObject.Add(newPlayer.ActorNumber, playerListItem);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Destroy(PlayerListGameObject[otherPlayer.ActorNumber]);
        PlayerListGameObject.Remove(otherPlayer.ActorNumber);

        if (PhotonNetwork.IsMasterClient)
        {
            playButton.SetActive(true);
        }
        else
        {
            playButton.SetActive(false);
        }
    }


    public void onClickPlayButton()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("Game");
        }
    }


    public override void OnLeftRoom()
    {
        ActivateMyPanel(LobbyPanel.name);
        foreach (GameObject obj in PlayerListGameObject.Values)
        {
            Destroy(obj);

        }
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList) //room list has all list of rooms
    {
        //clear list
        
        //clearRoomList();
        Debug.Log("hell1");
        if (roomList.Count > 0)
        {
            Debug.Log("hell2");
        }
        //base.OnRoomListUpdate(roomList);
        foreach (RoomInfo rooms in roomList)
        {
            Debug.Log("hell");
            Debug.Log("Room Names: " + rooms.Name); //it will tell how many room lists are present
            if (!rooms.IsOpen || !rooms.IsVisible || rooms.RemovedFromList)  
            {
                Debug.Log("hello1");
                if (roomListData.ContainsKey(rooms.Name))
                {
                    roomListData.Remove(rooms.Name); //remove the rooms if already present in the list
                }
            }
            else
            {
                Debug.Log("hello2");
                if (roomListData.ContainsKey(rooms.Name))
                {
                    //update list
                    roomListData[rooms.Name] = rooms;
                }
                else
                {
                    roomListData.Add(rooms.Name, rooms);  // rooms will only be added only if it is not present in the list.
                }
            }
        }
        Debug.Log("hello");
        //generate list item
        foreach (RoomInfo roomItem in roomListData.Values)
        {
            Debug.Log("hello3");

            GameObject roomListItemObject = Instantiate(roomListPrefab,roomListParent.transform);
            roomListItemObject.transform.SetParent(roomListParent.transform);
            roomListItemObject.transform.localScale = Vector3.one; // use to scale
            // we have the follwing in one item
            // room name player number button to join
            roomListItemObject.transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = roomItem.Name;
            roomListItemObject.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = roomItem.PlayerCount + "/" + roomItem.MaxPlayers;
            roomListItemObject.transform.GetChild(2).gameObject.GetComponent<Button>().onClick.AddListener(() => RoomJoinFromList(roomItem.Name));
           
            roomListGameObject.Add(roomItem.Name, roomListItemObject);

        }


    }


    public override void OnLeftLobby()
    {
        clearRoomList();
        roomListData.Clear();
    }



    #endregion




    #region Public_Methods


    public void RoomJoinFromList(string roomName)
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.JoinRoom(roomName);
        }
    }


    public void clearRoomList()
    {
        if (roomListGameObject.Count>0)
        {
            foreach (var v in roomListGameObject.Values)
            {
                Destroy(v);
            }

            roomListGameObject.Clear();
        }
    }

    public void ActivateMyPanel(string panelName)
    {
        LobbyPanel.SetActive(panelName.Equals(LobbyPanel.name));
        PlayerNamePanel.SetActive(panelName.Equals(PlayerNamePanel.name));
        RoomCreatePanel.SetActive(panelName.Equals(RoomCreatePanel.name));
        ConnectingPanel.SetActive(panelName.Equals(ConnectingPanel.name));
        RoomListPanel.SetActive(panelName.Equals(RoomListPanel.name));
        InsideRoomPanel.SetActive(panelName.Equals(InsideRoomPanel.name));

    }

    #endregion
}
