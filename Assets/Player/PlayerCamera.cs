using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] PlayerController player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (player == null || player.GetCharacterRB() == null)
            return;


        virtualCamera.Follow = player.GetCharacterRB().transform;
        virtualCamera.LookAt = player.GetCharacterRB().transform;
    }
}
