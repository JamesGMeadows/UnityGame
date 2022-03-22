using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipment : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject equippedPrefab;
    public GameObject hand;

    private Animator animator;
    private PlayerController controller;

    private Timer updateTimer;
    private GameObject equipped;
    public bool IsHitting = false;
    void Start()
    {
        controller = GetComponent<PlayerController>();
        var pos = hand.transform.position; //+ new Vector3(0, 2.3f, 0) + transform.forward * 2;
        equipped = Instantiate(equippedPrefab, pos, Quaternion.identity) as GameObject;
        equipped.transform.SetParent(hand.transform);
        animator = GetComponentInChildren<Animator>();

        updateTimer = new Timer(.151f);
    }

    // Update is called once per frame
    Vector3 originalPos;
    void Update()
    {

        if (updateTimer.CheckFinished(false))
        {
            if (Input.GetMouseButtonDown(0))
            {
                updateTimer.NextTime();
                animator.SetTrigger("Hit");
                IsHitting = true;
            }

        }
    }

}
