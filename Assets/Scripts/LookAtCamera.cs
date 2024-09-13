using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class LookAtCamera : NetworkBehaviour
{
    
    public Transform cameraTransorm;
    PlayerManager playerManager;

    

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            cameraTransorm = FindObjectOfType<PlayerManager>().cam.transform;
        }
    }


    public override void FixedUpdateNetwork()
    {
        transform.LookAt(cameraTransorm);
    }
}
