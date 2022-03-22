using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieController : MonoBehaviour
{
    // Start is called before the first frame update
    private NavMeshAgent nav;
    private GameObject target;
    private Damagable damagable;
    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        target = GameObject.FindGameObjectWithTag("Player");
        damagable = GetComponent<Damagable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
            return;
        }
        if (damagable.isDead())
        {
            return;
        }
        nav.SetDestination(target.transform.position);
    }
}
