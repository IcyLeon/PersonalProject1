using Cinemachine;
using JetBrains.Rider.Unity.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float Speed, SlowMultiplier, TurnMultiplier, ZoomMultiplier;
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    private CinemachineFramingTransposer cinemachineFramingTransposer;
    private float MaxCameraDistance = 5f;
    private float CameraDistance;
    private Rigidbody rb;
    private bool isGrounded;
    private Vector3 InputDirection;

    public event Action OnElementalSkillHold;
    public event Action OnElementalSkillTrigger;
    public event Action OnElementalBurstTrigger;
    public event Action OnChargeTrigger;
    public delegate void OnMouseScroll(float inputValue);
    public OnMouseScroll onMouseScroll;


    private Coroutine CameraZoomOffsetCoroutine, CameraPosOffsetCoroutine;
    public CinemachineVirtualCamera GetVirtualCamera()
    {
        return virtualCamera;
    }


    // Start is called before the first frame update
    void Start()
    {
        CameraDistance = MaxCameraDistance;
        CharacterManager.GetInstance().SetPlayerController(this);
        GameObject obj = Instantiate(CharacterManager.GetInstance().GetCharacterList()[0].gameObject, transform);
        InventoryManager.GetInstance().SetCurrentEquipCharacter(obj.GetComponent<Characters>());
        cinemachineFramingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        rb = InventoryManager.GetInstance().GetCurrentEquipCharacter().gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        //rb = InventoryManager.GetInstance().GetCurrentEquipCharacter().gameObject.GetComponent<Rigidbody>();
        GatherInput();
        UpdateControls();
        UpdateGrounded();
        UpdateCamera();

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(300f * Vector3.up);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Dash();
        }
    }

    private IEnumerator UpdateCameraOffsetPosition(float ModifierX, float ModifierY, float Speed)
    {
        while (cinemachineFramingTransposer.m_ScreenX != ModifierX)
        {
            if (cinemachineFramingTransposer.m_ScreenX != ModifierX)
                cinemachineFramingTransposer.m_ScreenX = Mathf.Lerp(cinemachineFramingTransposer.m_ScreenX, ModifierX, Time.deltaTime * Speed);

            yield return null;
        }
        while (cinemachineFramingTransposer.m_ScreenX != ModifierX)
        {
            if (cinemachineFramingTransposer.m_ScreenY != ModifierY)
                cinemachineFramingTransposer.m_ScreenY = Mathf.Lerp(cinemachineFramingTransposer.m_ScreenY, ModifierY, Time.deltaTime * Speed);

            yield return null;
        }
        CameraPosOffsetCoroutine = null;
    }

    private IEnumerator UpdateCameraZoomPosition(float CameraDistance, float Speed)
    {
        while (cinemachineFramingTransposer.m_CameraDistance != CameraDistance)
        {
            cinemachineFramingTransposer.m_CameraDistance = Mathf.Lerp(cinemachineFramingTransposer.m_CameraDistance, CameraDistance, Time.deltaTime * Speed);
            yield return null;
        }
        CameraZoomOffsetCoroutine = null;
    }

    public void CameraOffsetPositionAnim(float ModifierX, float ModifierY, float Speed)
    {
        if (CameraPosOffsetCoroutine != null)
            StopCoroutine(CameraPosOffsetCoroutine);

        CameraPosOffsetCoroutine = StartCoroutine(UpdateCameraOffsetPosition(ModifierX, ModifierY, Speed));
    }

    public void CameraZoomOffsetAnim(float CameraDistance, float Speed)
    {
        if (CameraZoomOffsetCoroutine != null)
            StopCoroutine(CameraZoomOffsetCoroutine);

        CameraZoomOffsetCoroutine = StartCoroutine(UpdateCameraZoomPosition(CameraDistance, Speed));
    }


    void UpdateCamera()
    {
        if (GetCharacterRB() == null)
            return;

        virtualCamera.Follow = GetCharacterRB().transform;
        virtualCamera.LookAt = GetCharacterRB().transform;
    }

    void UpdateGrounded()
    {
        isGrounded = Physics.Raycast(rb.position, Vector3.down, (rb.transform.GetComponent<CapsuleCollider>().height / 2) + 0.05f);
    }

    public Vector3 GetRayPosition3D(Vector3 origin, Vector3 direction, float maxdistance)
    {
        if (Physics.Raycast(origin, direction.normalized, out RaycastHit hit, maxdistance))
        {
            return hit.point;
        }
        return origin + direction.normalized * maxdistance;
    }

    void UpdateControls()
    {
        if (Input.GetKey(KeyCode.E))
            OnElementalSkillHold?.Invoke();
        else if (Input.GetKeyUp(KeyCode.E))
            OnElementalSkillTrigger?.Invoke();
        else if(Input.GetKey(KeyCode.Q))
            OnElementalBurstTrigger?.Invoke();
        else if(Input.GetMouseButton(0))
            OnChargeTrigger?.Invoke();
        else
        {
            CameraOffsetPositionAnim(0.5f, 0.5f, 10f); // return to default
            CameraZoomOffsetAnim(5f, 10f);
        }

    }

    void GatherInput()
    {
        float HorizontalInput = Input.GetAxisRaw("Horizontal");
        float VerticalInput = Input.GetAxisRaw("Vertical");
        float MouseInput = -Input.GetAxisRaw("Mouse ScrollWheel");

        InputDirection = (GetVirtualCamera().transform.forward * VerticalInput) + (GetVirtualCamera().transform.right * HorizontalInput);
        InputDirection.y = 0;
        InputDirection.Normalize();

        CameraDistance += MouseInput * ZoomMultiplier;
        CameraDistance = Mathf.Clamp(CameraDistance, 2f, 5f);

        if (InputDirection == Vector3.zero || !isGrounded)
            return;

        Quaternion m_Rotation = Quaternion.LookRotation(InputDirection);
        rb.rotation = Quaternion.Slerp(rb.rotation, m_Rotation, Time.deltaTime * 360f * TurnMultiplier);

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        UpdatePhysicsMovement();
    }

    void UpdatePhysicsMovement()
    {
        if (InputDirection == Vector3.zero || !isGrounded)
            return;

        rb.AddForce((InputDirection * Speed) - GetHorizontalVelocity() * SlowMultiplier, ForceMode.VelocityChange);
    }

    private void Dash()
    {
        if (!isGrounded)
            return;

        Vector3 forward = rb.transform.forward;
        forward.y = 0;
        forward.Normalize();

        rb.AddForce((forward * Speed * 1.5f) - GetHorizontalVelocity() * SlowMultiplier, ForceMode.VelocityChange);
    }

    private Vector3 GetHorizontalVelocity()
    {
        Vector3 vel = rb.velocity;
        vel.y = 0;
        return vel;
    }

    public Rigidbody GetCharacterRB()
    {
        return rb;
    }

    void UpdateDash()
    {

    }
}
