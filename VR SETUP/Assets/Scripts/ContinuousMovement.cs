using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class ContinuousMovement : MonoBehaviour
{
    public float speed = 1;
    //agar dapat memilih source input yang ingin di dengarkan contohnya left hand
    public XRNode inputSource;
    public float gravity = -9.81f;
    //Untuk memberikan masking layer mana yang dibutuhkan untuk berinteraksi
    public LayerMask groundLayer;
    public float additionalHeight = 0.2f;

    private float fallingSpeed;
    private XRRig rig;
    private Vector2 inputAxis;
    //Untuk menggerakan VR Rig
    private CharacterController character;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
        rig = GetComponent<XRRig>();
    }

    // Update is called once per frame
    void Update()
    {
        //mengetahui input yang diberikan oleh source berupa vector2 kemudian dimasukan ke variable inputAxis yang sudah dibuat dalam private
        InputDevice device = InputDevices.GetDeviceAtXRNode(inputSource);
        device.TryGetFeatureValue(CommonUsages.primary2DAxis, out inputAxis);
    }

    //dimasukan kedalam fixedupdate supaya gerakan di lakukan setiap unity update physics
    private void FixedUpdate()
    {
        CapsuleFollowHeadset();

        //Untuk bergerak sesuai dengan arah camera vr
        Quaternion headYaw = Quaternion.Euler(0, rig.cameraGameObject.transform.eulerAngles.y, 0);
        //Untuk bergerak sesuai dengan input
        Vector3 direction = headYaw * new Vector3(inputAxis.x, 0, inputAxis.y);
        character.Move(direction * Time.fixedDeltaTime * speed);

        //gravity
        bool isGrounded = CheckIfGrounded();
        if (isGrounded)
            fallingSpeed = 0;
        else
            //function ini aktif terus maka dibutuhkan isGrounded agar tidak ada fallingspeed jika sudah di tanah
            fallingSpeed += gravity * Time.fixedDeltaTime;
        
        character.Move(Vector3.up * fallingSpeed * Time.fixedDeltaTime);
    }

    //Agar karakter mengikuti headset saat headset bergerak
    void CapsuleFollowHeadset()
    {
        character.height = rig.cameraInRigSpaceHeight + additionalHeight;
        Vector3 capsuleCenter = transform.InverseTransformPoint(rig.cameraGameObject.transform.position);
        character.center = new Vector3(capsuleCenter.x, character.height/2 + character.skinWidth , capsuleCenter.z);
    }

    bool CheckIfGrounded()
    {
        //tell us if on ground
        Vector3 rayStart = transform.TransformPoint(character.center);
        float rayLength = character.center.y + 0.01f;
        bool hasHit = Physics.SphereCast(rayStart, character.radius, Vector3.down, out RaycastHit hitInfo, rayLength, groundLayer);
        return hasHit;
    }
}
