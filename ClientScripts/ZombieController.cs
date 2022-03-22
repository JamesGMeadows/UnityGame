using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator anim;
    private NetworkEntity entity;
    private Damagable damagable;
    private SkinnedMeshRenderer skin;
    private Timer damageEffectTimer;
    private bool damageColor = false;
    void Start()
    {
        damagable = GetComponent<Damagable>();
        anim = GetComponent<Animator>();
        entity = GetComponent<NetworkEntity>();
        skin = GetComponentInChildren<SkinnedMeshRenderer>();
        damageEffectTimer = new Timer(.3f);
    }
 


    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("Speed", entity.GetVelocity());
        if (damagable.playDeathAnimation) //could use a listener system instead
        {
            anim.SetTrigger("Death");
            damagable.playDeathAnimation = false;
        }
        else if (damagable.playDamageAnimation)
        {
            skin.material.color = Color.red;
            damagable.playDamageAnimation = false;
            damageEffectTimer.NextTime();
            damageColor = true;
        }
        
        if (damageColor && damageEffectTimer.CheckFinished(false))
        {
            skin.material.color = Color.white;
            damageColor = false;
        }
    }
}
