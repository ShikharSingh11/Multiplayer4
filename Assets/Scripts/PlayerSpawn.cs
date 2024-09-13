using UnityEngine;
using Fusion;
using System.Collections.Generic;
using TMPro;
public class PlayerSpawn : SimulationBehaviour, IPlayerJoined
{
    public GameObject playerPrefab;
    public List<Transform> spawnPositions;
    public TMP_InputField inputField;
    public string pName;
    public GameObject uiCanvas;
    PlayerManager playerSpawned;
    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            int count = spawnPositions.Count;
            int randomIndex = Random.Range(0, count);
            Debug.Log(randomIndex);
            Transform spawnPosition = spawnPositions[randomIndex];
            Debug.Log(spawnPosition.position);
            playerSpawned = Runner.Spawn(playerPrefab, new Vector3(spawnPosition.position.x, spawnPosition.position.y, spawnPosition.position.z), spawnPosition.rotation, player).GetComponent<PlayerManager>();
            
            
            playerSpawned.controller.enabled = false;
            playerSpawned.transform.position = spawnPosition.position;
            playerSpawned.transform.rotation = spawnPosition.rotation;
            playerSpawned.controller.enabled = true;
            
            
            playerSpawned.SetPLayerName(name);
        }   
    }

    public void PlayerJoin()
    {
        pName = inputField.text;
        name = pName;
    }

    public void SaveName()
    {
        pName = inputField.text;
        uiCanvas.SetActive(false);
    }
}
