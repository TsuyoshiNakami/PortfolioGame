using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locomotion : MonoBehaviour
{

    //十字キーのみで操作(上下矢印キー＝前後，左右矢印キー＝回転)
    //CharacterControllerが必要

    public float speed = 6.0F;          //歩行速度
    public float jumpSpeed = 8.0F;      //ジャンプ力
    public float gravity = 20.0F;       //重力の大きさ
    public float rotateSpeed = 3.0F;    //回転速度

    bool isJumping;
    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    //private float h, v;

    Vector3 velocity = Vector3.zero;
    Animator animator;

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

    }//Start()

    // Update is called once per frame
    void Update()
    {

            //Debug.Log(controller.isGrounded);

        Vector3 cameraDirection = Camera.main.transform.forward;
        cameraDirection.y = 0f;
        Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, cameraDirection);
        // Convert joystick input in Worldspace coordinates
        if (controller.isGrounded)
        {
            velocity = Vector3.zero;
            //moveDirection = transform.TransformDirection(moveDirection);
            if (Input.GetButtonDown("Jump"))
            {
                isJumping = true;
                velocity.y += jumpSpeed;
            }

        }
            moveDirection = referentialShift * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        velocity.y += gravity + Time.deltaTime;
            controller.Move(Vector3.up * -velocity.y * Time.deltaTime);
        if (moveDirection != Vector3.zero)
        {
            controller.Move(transform.forward * speed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(moveDirection);
            animator.SetInteger("AnimIndex", 3);
        } else
        {

            animator.SetInteger("AnimIndex", 0);
        }


    }//Update()

}