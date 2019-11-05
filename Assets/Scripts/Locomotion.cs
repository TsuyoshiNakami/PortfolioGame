using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Locomotion : MonoBehaviour
{

    //十字キーのみで操作(上下矢印キー＝前後，左右矢印キー＝回転)
    //CharacterControllerが必要

    public float speed = 6.0F;          //歩行速度
    public float jumpSpeed = 8.0F;      //ジャンプ力
    public float gravity = 20.0F;       //重力の大きさ
    public float rotateSpeed = 3.0F;    //回転速度

    [SerializeField] float checkGroundLength;
    [SerializeField] bool isJumping;
    [SerializeField]float jumpTime;
    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    [SerializeField]bool isGrounded;
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
        jumpTime += Time.deltaTime;
        Vector3 cameraDirection = Camera.main.transform.forward;
        cameraDirection.y = 0f;
        Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, cameraDirection);
        // Convert joystick input in Worldspace coordinates
        isGrounded = CheckGrounded();
        if (isGrounded)
        {
            if(isJumping &&
                jumpTime >= 0.5f)
            {
                isJumping = false;
            }

            if (!isJumping)
            {
                velocity = Vector3.zero;
                //moveDirection = transform.TransformDirection(moveDirection);
                if (Input.GetButtonDown("Jump"))
                {
                    isJumping = true;
                    jumpTime = 0;
                    velocity.y += jumpSpeed;
                }
            }

        }
        moveDirection = referentialShift * new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        velocity.y -= gravity + Time.deltaTime;
        controller.Move(Vector3.up * velocity.y * Time.deltaTime);
        if (moveDirection != Vector3.zero)
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(moveDirection);
            animator.SetInteger("AnimIndex", 3);
        }
        else
        {

            animator.SetInteger("AnimIndex", 4);
        }


    }//Update()



    public bool CheckGrounded()
    {
        //CharacterController取得
        var controller = GetComponent<CharacterController>();
        //CharacterControlle.IsGroundedがtrueならRaycastを使わずに判定終了
        if (controller.isGrounded) { return true; }
        //放つ光線の初期位置と姿勢
        //若干身体にめり込ませた位置から発射しないと正しく判定できない時がある
        var ray = new Ray(this.transform.position + Vector3.up * 0.1f, Vector3.down);
        //探索距離
        var tolerance = checkGroundLength;
        //Raycastがhitするかどうかで判定
        //地面にのみ衝突するようにレイヤを指定する
        return Physics.Raycast(ray, tolerance, (int)LayerMask.NameToLayer("Ground"));
    }
}