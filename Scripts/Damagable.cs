using Assets.Scripts.Networking.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damagable : MonoBehaviour
{

    private float lastHealth;
    private NetworkEntity network;
    private Slider slider;
    private bool dead = false;
    void Start()
    {
        slider = GetComponentInChildren<Slider>();
        network = GetComponent<NetworkEntity>();
    }

    public NetworkEntity GetNetwork()
    {
        return network;
    }

    void Update()
    {
        if (slider != null)
        {
            slider.value = GetCurrentHealth() / 100;
        }
        // Debug.Log(GetMaxHealth());
        CheckUpdate();
    }

    public bool playDamageAnimation = false;
    public bool playDeathAnimation = false;
    private void CheckUpdate()
    {
        float currentHealth = GetCurrentHealth();
        if (lastHealth != currentHealth)
        {
            Debug.Log("Health changed: " + currentHealth);
            lastHealth = currentHealth;
            playDamageAnimation = true;
        }
        if (currentHealth <= 0 && lastHealth != 0)
        {
            if (!dead)
            {
                Die();
            }
        }
    }

    public void Die()
    {
        dead = true;
        playDeathAnimation = true;
    }

    public bool isDead()
    {
        return dead;
    }

    public float GetCurrentHealth()
    {
        
        float health;
        network.GetStatus().TryGetValue(EntityStatus.CURRENT_HEALTH, out health);
        return health;
    }

    public float GetMaxHealth()
    {
        float health;
        network.GetStatus().TryGetValue(EntityStatus.MAX_HEALTH, out health);
        return health;
    }

}
