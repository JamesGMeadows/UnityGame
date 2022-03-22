using Assets.Scripts.Networking.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{

    public bool open = false;

    private float openZRot;
    private bool isOpen = false;
    public bool inSwing = false;
    private Damagable damagable;
    private NetworkEntity network;
    void Start()
    {
        openZRot = (inSwing ? -90 : 90);
        network = GetComponent<NetworkEntity>();
        damagable = GetComponent<Damagable>();
    }


    void Update()
    {
        if (damagable.isDead())
        {
            open = true;
        }
        InteractDoor();
    }

    public void InteractDoor()
    {
        Vector3 rot = transform.rotation.eulerAngles;
        bool changed = false;

        if (isOpen)
        {
            if (!open)
            {
                transform.rotation = Quaternion.Euler(rot.x, rot.y, -openZRot);
                changed = true;
                isOpen = false;
            }
        }
        else
        {
            if (open)
            {
                transform.rotation = Quaternion.Euler(rot.x, rot.y, openZRot);
                changed = true;
                isOpen = true;
            }
        }
        if (changed)
        {
            network.UpdateStatus(EntityStatus.DOOR_STATE, isOpen ? 1 : 0);
        }
    }
}
