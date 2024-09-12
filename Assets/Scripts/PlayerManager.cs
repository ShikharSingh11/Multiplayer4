using UnityEngine;
using Fusion;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using System.Collections;
using TMPro;
public class PlayerManager : NetworkBehaviour
{
    public CharacterController controller;
    public float moveSpeed = 5f;
    public float turnSpeed = 10f;
    public float raycastRange = 10f;
    public int maxHealth = 100;
    public int damage = 10;

    public Camera cam;
    public GameObject cameraPrefab;
    [SerializeField] LineRenderer line;
    [SerializeField] private GameObject gun;
    public int killCount=0;
    public TMP_Text killCountText;
    public LayerMask mask;
    public TMP_Text textToDisplay;
    [Networked, OnChangedRender(nameof(OnNameChanged))] public string NetworkedPlayerName {  get; set; }
    [Networked] public string killCountTMP {  get; set; }


    
    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            GameObject cameraObject = Instantiate(cameraPrefab);
            cam = cameraObject.GetComponent<Camera>();
            cam.GetComponent<CameraController>().playerBody = transform;
            UpdateKillCountUI();
            NetworkedPlayerName = Runner.GetComponent<PlayerSpawn>().name;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            HandleMovement();
        }
        
    }

    private void Update(){
        if(HasStateAuthority){
            if(Input.GetButtonDown("Fire1")){
                Shoot();
            }
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
                    if(enemy.NetworkedHealth <= 0){
                        killCount++;
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
    IEnumerator DiasableLine(float time){
        yield return new WaitForSeconds(time);
        line.enabled = false;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_ConfigureName(string name)
    {
        NetworkedPlayerName = name;
    }
    void UpdateKillCountUI(){
        if(killCountText != null){
            killCountText.text = "Kills : " + killCount.ToString();
        }
    }
}
