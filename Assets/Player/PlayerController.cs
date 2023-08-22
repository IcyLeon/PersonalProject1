using JetBrains.Rider.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float CameraSensitivity;
    [SerializeField] float Speed;
    [SerializeField] Camera camera;
    private float CurrentAngle;
    private InventoryManager inventoryManager;
    private Vector3 direction;
    private Rigidbody rb;

    private void Awake()
    {
        inventoryManager = new InventoryManager();
    }

    // Start is called before the first frame update
    void Start()
    {
        CharacterManager.GetInstance().SetPlayerController(this);
        GameObject obj = Instantiate(CharacterManager.GetInstance().GetCharacterList()[0].gameObject, transform);
        inventoryManager.SetCurrentEquipCharacter(obj.GetComponent<Characters>());
        rb = inventoryManager.GetCurrentEquipCharacter().gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        rb = inventoryManager.GetCurrentEquipCharacter().gameObject.GetComponent<Rigidbody>();
        //if (currentequipCharacter != null)
        //{
        //    currentequipCharacter.Attack();
        //}
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float HorizontalInput = Input.GetAxis("Horizontal");
        float VerticalInput = Input.GetAxis("Vertical");
        Vector3 InputDirection = (camera.transform.forward * VerticalInput) + (camera.transform.right * HorizontalInput);
        InputDirection.y = 0;
        InputDirection.Normalize();

        if (InputDirection == Vector3.zero)
            return;

        direction = Vector3.Slerp(direction, InputDirection, Time.deltaTime * 20);
        Quaternion m_Rotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(m_Rotation);
        rb.AddForce(InputDirection * Speed - GetVelocity(), ForceMode.VelocityChange);

        //rb.MovePosition(rb.position + InputDirection * Speed * Time.deltaTime);
    }

    private Vector3 GetVelocity()
    {
        Vector3 vel = rb.velocity;
        vel.y = 0;
        return vel;
    }

    private void Jump()
    {

    }
    public Rigidbody GetCharacterRB()
    {
        return rb;
    }

    public InventoryManager GetInventory()
    {
        return inventoryManager;
    }

    void UpdateDash()
    {

    }
}
