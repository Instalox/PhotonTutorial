using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using System;

public class Player : PunBehaviour
{
    public GameObject MissilePrefab;
    public Transform GunPosition;

    TextMesh HealthText;

    public float Speed = 1f;

    public float Horizontal { get; set; }
    public float Vertical { get; set; }

    [SerializeField]
    private float _health = 100f;

    public float Health
    {
        get { return _health; }
        set
        {
            _health = value;
            HealthText.text = "HP: " + _health;
        }
    }

    void Start()
    {
        HealthText = GetComponentInChildren<TextMesh>();
        HealthText.text = "HP: " + Health;
    }

    void Update()
    {
        if (!photonView.isMine)
            return;

        //Movement
        Horizontal = Input.GetAxis("Horizontal") * Time.deltaTime;
        Vertical = Input.GetAxis("Vertical") * Time.deltaTime;

        transform.Translate(new Vector3(Horizontal * Speed, Vertical * Speed, 0f));

        //Fire missile
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Fire(GunPosition.position, transform.rotation);
        }

    }

    public void ApplyDamage(float dmg)
    {
        if (!photonView.isMine)
            return;

        //Send an rpc to let everyone know this player has taken damage.
        photonView.RPC("TakeDamage", PhotonTargets.All, dmg);
    }

    [PunRPC]
    private void TakeDamage(float dmg)
    {
        Health -= dmg;
        if (Health <= 0f)
        {
            //tell everyone this player has died.
            if (photonView.isMine)
                photonView.RPC("Die", PhotonTargets.All);
        }
    }

    [PunRPC]
    private void Die()
    {
        if (photonView.isMine)
            PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    private void FireMissile(Vector3 pos, Quaternion rotation)
    {
        if (!photonView.isMine)
            return;
        
        var missile = PhotonNetwork.Instantiate("Missile", pos, rotation, 0);
        var missileComponent = missile.GetComponent<Missile>();
        if (missileComponent)
            missileComponent.OwnerID = photonView.viewID;

    }

    private void Fire(Vector3 pos, Quaternion rotation)
    {
        photonView.RPC("FireMissile", PhotonTargets.All, pos, rotation);
    }


}
