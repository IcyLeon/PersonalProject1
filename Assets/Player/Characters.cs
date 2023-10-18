using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Characters : MonoBehaviour
{
    protected float AimSpeed = 20f;
    private int Level;
    // Start is called before the first frame update
    protected List<Artifacts> EquippedArtifacts = new List<Artifacts>();
    private PlayerController playerController;
    [SerializeField] PlayersSO playersSO;

    public PlayersSO GetPlayersSO()
    {
        return playersSO;
    }

    public void Attack()
    {

    }

    public int GetLevel()
    {
        return Level;
    }
    protected virtual void Start()
    {
        playerController = CharacterManager.GetInstance().GetPlayerController();
        playerController.OnElementalSkillHold += ElementalSkillHold;
        playerController.OnElementalBurstTrigger += ElementalBurstTrigger;
        playerController.OnElementalSkillTrigger += ElementalSkillTrigger;
    }

    public PlayerController GetPlayerController()
    {
        return playerController;
    }
    // Update is called once per frame
    protected virtual void Update()
    {

    }

    public CinemachineVirtualCamera GetVirtualCamera()
    {
        if (playerController == null)
            return null;

        return playerController.GetVirtualCamera();
    }

    protected void UpdateCameraOffsetPosition(float ModifierX, float ModifierY, float Speed)
    {
        if (playerController == null)
            return;

        playerController.CameraOffsetPositionAnim(ModifierX, ModifierY, Speed);
    }

    protected void UpdateCameraZoomOffset(float CameraDistance, float Speed)
    {
        if (playerController == null)
            return;

        playerController.CameraZoomOffsetAnim(CameraDistance, Speed);
    }

    public Vector3 GetRayPosition3D(Vector3 origin, Vector3 direction, float maxdistance)
    {
        if (playerController == null)
            return Vector3.zero;

        return playerController.GetRayPosition3D(origin, direction, maxdistance);
    }

    protected void UpdateCameraAim()
    {
        UpdateCameraOffsetPosition(0.28f, 0.5f, AimSpeed);
        UpdateCameraZoomOffset(3f, AimSpeed);
    }

    protected virtual void ElementalSkillTrigger()
    {
    }

    protected virtual void ElementalSkillHold()
    {

    }
    protected virtual void ElementalBurstTrigger()
    {

    }
    public List<Artifacts> GetEquippedArtifactsList()
    {
        return EquippedArtifacts;
    }

    public Artifacts CheckIfArtifactTypeExist(ArtifactType artifacttype)
    {
        for (int i = 0; i < EquippedArtifacts.Count; i++)
        {
            if (EquippedArtifacts[i].GetArtifactType() == artifacttype)
            {
                return EquippedArtifacts[i];
            }
        }
        return null;
    }
}
