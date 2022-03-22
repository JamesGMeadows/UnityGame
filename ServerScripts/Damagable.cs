using Assets.Scripts.Networking.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damagable : MonoBehaviour
{

    public float maxHealth;
    public float currentHealth;
    public bool dead = false;
    private NetworkEntity network;
    void Start()
    {
        currentHealth = maxHealth;
        network = GetComponent<NetworkEntity>();
    }

    public NetworkEntity GetNetwork()
    {
        return network;
    }


    void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
        UpdateStatus();
    }

    public bool isDead()
    {
        return dead;
    }

    public void Die()
    {
        dead = true;
        network.UpdateStatus(EntityStatus.DEATH, 1);
    }

    public void Damage(float amount)
    {
        currentHealth -= amount;
        network.UpdateStatus(EntityStatus.CURRENT_HEALTH, currentHealth);
        Debug.Log("DAMAGED ENTITY: " + currentHealth);
    }

    //updates the status of the NetworkEntity. If it changed, it will be sent to the client
    private void UpdateStatus()
    {
        network.UpdateStatus(EntityStatus.CURRENT_HEALTH, currentHealth);
        network.UpdateStatus(EntityStatus.MAX_HEALTH, maxHealth);
        network.UpdateStatus(EntityStatus.DEATH, dead ? 1 : 0);

        //network.UpdateStatus(EntityStatus.CURRENT_HEALTH, currentHealth + 1);
    }
}
