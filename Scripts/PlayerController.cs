using Assets.Scripts;
using Assets.Scripts.Networking;
using Assets.Scripts.Networking.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0F;
    public float gravity = 20.0F;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 85.0f;

    private CharacterController controller;
    private Vector3 direction;
    public float updateRate = 1f;
    private Timer updateTimer;
    private DClient client;
    private Animator animator;
    private Camera playerCamera;

    private float rotationX = 0;

    void Start()
    {
        client = DClient.GetClient();
        controller = GetComponent<CharacterController>();
        //animator = GetComponentInChildren<Animator>();
        playerCamera = GetComponentInChildren<Camera>();
        direction = Vector3.zero;
        updateTimer = new Timer(.1f);

    }

    // Update is called once per frame
    void Update()
    {

        Cursor.visible = false;
        if (controller.isGrounded)
        {
            float xInput = Input.GetAxis("Horizontal");
            float yInput = Input.GetAxis("Vertical");
            //animator.SetFloat("speed", Mathf.Abs(xInput) + Mathf.Abs(yInput));
            direction = new Vector3(xInput, 0, yInput);
            direction = transform.TransformDirection(direction);
            direction *= speed;
         }
         direction.y -= gravity * Time.deltaTime;
         controller.Move(direction * Time.deltaTime);

        rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
        rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);


        if (updateTimer.CheckFinished(false))
        {
            updateTimer.NextTime();
            Vector3 vel = controller.velocity;
            Vector3 pos = controller.transform.position;
            Vector3 rot = controller.transform.eulerAngles;

            client.Write(new PositionPacket(pos.x, pos.y, pos.z)); //send current position to the server
            client.Write(new RotationPacket(rot.x, rot.y, rot.z));

        }
    }
}
