using UnityEngine;
using Fusion;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;
public class PlayerManager : NetworkBehaviour
{
    public CharacterController controller;
    public float moveSpeed = 5f;
    public float turnSpeed = 10f;
    public float raycastRange = 10f;
    public int maxHealth = 100;
    public int damage = 10;

    public Camera mainCamera;

    public Camera cam;
    public GameObject cameraPrefab;
    [SerializeField] LineRenderer line;
    [SerializeField] private GameObject gun;
    public LayerMask mask;
    public TMP_Text textToDisplay;
    public TMP_Text killCountText;
    public int killCount = 0;
    [Networked, OnChangedRender(nameof(OnNameChanged))] public string NetworkedPlayerName { get; set; }
    [Networked, OnChangedRender(nameof(OnKillCountChange))] public int NetworkedKillCount { get; set; }



    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            mainCamera = Camera.main;
            GameObject cameraObject = Instantiate(cameraPrefab);
            cam = cameraObject.GetComponent<Camera>();
            cam.GetComponent<CameraController>().playerBody = transform;
            Cursor.lockState = CursorLockMode.Locked;
        }
        textToDisplay.text = NetworkedPlayerName.ToString();
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority && Cursor.lockState == CursorLockMode.Locked)
        {
            HandleMovement();
        }
    }

    void OnKillCountChange()
    {
        if (HasStateAuthority)
        {
            killCountText.text = "Kills: " + NetworkedKillCount.ToString();
        }
    }
    private void Update()
    {
        if (HasStateAuthority)
        {
            if (Input.GetButtonDown("Fire1") && !IsPointerOverUIObject())
            {
                Cursor.lockState = CursorLockMode.Locked;
                Shoot();
            }
        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
        }

    }
    bool IsPointerOverUIObject()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    // public void ChangeCameraView(){
    //     cam.enabled = false;
    //     mainCamera.enabled = true;

    // }
    public void OnButtonClick()
    {
        StartCoroutine(HandleButtonClick());
    }

    IEnumerator HandleButtonClick()
    {
        if (HasStateAuthority)
        {
            cam.enabled = false;
            mainCamera.enabled = true;
            yield return null;

            Cursor.lockState = CursorLockMode.Locked;
        }
    }
    public void SetPLayerName(string name)
    {
        if (HasStateAuthority)
        {
            RPC_ConfigureName(name);
        }
    }
    void OnNameChanged()
    {
        textToDisplay.text = NetworkedPlayerName.ToString();
    }
    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        controller.Move(move * moveSpeed * Runner.DeltaTime);

        Vector3 lookDirection = new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection), turnSpeed * Runner.DeltaTime);
    }
    void HandleShooting()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
    }
    void Shoot()
    {
        Debug.Log("Shoot");
        RaycastHit hit;
        Vector3 rayOrigin = gun.transform.position;
        Vector3 rayDirection = cam.ScreenPointToRay(Input.mousePosition).direction;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, raycastRange, mask))
        {
            if (hit.collider.CompareTag("Player"))
            {
                line.SetPosition(1, hit.point);
                Health enemy = hit.collider.GetComponent<Health>();
                if (enemy != null && enemy != this)
                {
                    enemy.TakeDamageRpc(damage);
                    if (enemy.NetworkedHealth <= 0)
                    {
                        if (HasStateAuthority)
                        {
                            NetworkedKillCount++;
                        }
                    }
                }
            }
        }
        if (HasStateAuthority)
        {
            RPC_ShowLine(rayOrigin, rayDirection, hit.point);
        }
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    void RPC_ShowLine(Vector3 rayOrigin, Vector3 rayDirection, Vector3 hitPoint)
    {
        line.useWorldSpace = true;
        line.SetPosition(0, rayOrigin);
        line.SetPosition(1, hitPoint);
        line.startColor = Color.red;
        line.endColor = Color.red;
        line.enabled = true;
        StartCoroutine(DiasableLine(0.3f));
    }
    IEnumerator DiasableLine(float time)
    {
        yield return new WaitForSeconds(time);
        line.enabled = false;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_ConfigureName(string name)
    {
        NetworkedPlayerName = name;
    }



    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_ConfigureKillCount(int newKillcount)
    {
        NetworkedKillCount = newKillcount;
    }
}
