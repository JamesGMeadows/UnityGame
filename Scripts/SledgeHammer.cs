using Assets.Scripts.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SledgeHammer : MonoBehaviour
{
    public AudioClip contact;
    private PlayerEquipment equipment;
    private DClient client;
    private AudioSource source;
    void Start()
    {
        equipment = GetComponentInParent<PlayerEquipment>();
        source = GetComponentInParent<AudioSource>();
        client = DClient.GetClient();
    }

    void Update()
    {
        //Debug.Log(collider.bounds.max);
    }

    void OnTriggerEnter(Collider other)
    {
        if (equipment.IsHitting)
        {
            equipment.IsHitting = false;
            Damagable victim = other.gameObject.GetComponentInParent<Damagable>();
            if (victim != null)
            {
                OnContact(victim);
            }
        }
    }

    public void OnContact(Damagable victim)
    {
        victim.playDamageAnimation = true;
        client.Write(new AttackEntityPacket(victim.GetNetwork().GetNetworkId(), 0));
        source.PlayOneShot(contact);
    }
}
