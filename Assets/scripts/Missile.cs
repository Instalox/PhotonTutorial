using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
public class Missile : PunBehaviour
{
    private Rigidbody rb;

    [SerializeField]
    private float Force = 1f;

    public int OwnerID;

    [SerializeField]
    private float Damage = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.right * Force, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!photonView.isMine)
            return;

        //Grab the photonview of the object we hit so we can pass the ID out to everyone else.          
        PhotonView pv = collision.gameObject.GetComponent<PhotonView>();
        if (pv)
        {
            Explode(pv.viewID);
        }
    }

    private void Explode(int viewID)
    {
        if (photonView.isMine)
            photonView.RPC("ExplodeRPC", PhotonTargets.All, viewID);
    }

    [PunRPC]
    void ExplodeRPC(int viewID)
    {
        //find the photon view and apply damage
        var playerHit = PhotonView.Find(viewID);
        if (playerHit)
        {
            //now grab the player component
            var PlayerComp = playerHit.GetComponent<Player>();
            if (PlayerComp)
            {
                //apply the damage
                PlayerComp.ApplyDamage(Damage);
            }
        }
        //destroy over the network
        if (photonView.isMine)
            PhotonNetwork.Destroy(gameObject);
    }
}
