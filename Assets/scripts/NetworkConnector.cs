using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using System;

public class NetworkConnector : PunBehaviour
{

    // Use this for initialization
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings("test");
    }

    public override void OnConnectedToPhoton()
    {
        base.OnConnectedToPhoton();
        Debug.Log("On connected to photon");

    }
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("On connected to master");

        PhotonNetwork.JoinOrCreateRoom("testroom", new RoomOptions(), TypedLobby.Default);

    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("On joined lobby");
        PhotonNetwork.JoinOrCreateRoom("testroom", new RoomOptions(), TypedLobby.Default);
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        Debug.Log("Created Room");

        SpawnPlayer(Vector3.zero, Quaternion.identity);

    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined Room");
        Debug.Log("Is Master: " + PhotonNetwork.isMasterClient);
    }

    private void SpawnPlayer(Vector3 pos, Quaternion rotation)
    {
        Debug.Log("Spawn player");

        PhotonNetwork.Instantiate("Player", pos, rotation, 0);

    }

    private GameObject SpawnClient(Vector3 pos, Quaternion rotation)
    {
        var obj = PhotonNetwork.Instantiate("Player", pos, rotation, 0);
        return obj;
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        base.OnPhotonPlayerConnected(newPlayer);
        var obj = SpawnClient(new Vector3(6.51f, 0f, 0), Quaternion.identity);
        obj.transform.Rotate(0, 180f, 0);
        var pv = obj.GetPhotonView();
        if (pv)
        {
            pv.TransferOwnership(newPlayer);
        }

        //SpawnPlayer(new Vector3(6.51f, 0, 0), Quaternion.identity);
    }


}
