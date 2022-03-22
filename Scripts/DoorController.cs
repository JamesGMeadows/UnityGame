using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    // Start is called before the first frame update
    private Damagable damagable;
    void Start()
    {
        damagable = GetComponent<Damagable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (damagable.playDeathAnimation)
        {
            gameObject.SetActive(false);
            damagable.playDamageAnimation = false;
        }
    }
}
