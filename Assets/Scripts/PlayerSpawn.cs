using UnityEngine;
using Fusion;
using System.Collections.Generic;
using TMPro;
public class PlayerSpawn : SimulationBehaviour, IPlayerJoined
{
    public GameObject playerPrefab;
    public List<Transform> spawnPositions;
    public TMP_InputField inputField;
    public string name;
    public GameObject uiCanvas;
    public void PlayerJoined(PlayerRef player)
    {
        if (player == Runner.LocalPlayer)
        {
            int count = spawnPositions.Count;
            int randomIndex = Random.Range(0, count);
            Debug.Log(randomIndex);
            Transform spawnPosition = spawnPositions[randomIndex];
            Debug.Log(spawnPosition.position);
            Runner.Spawn(playerPrefab, new Vector3(spawnPosition.position.x, spawnPosition.position.y, spawnPosition.position.z), spawnPosition.rotation);

        }
    }

    public void SaveName()
    {
        name = inputField.text;
        uiCanvas.SetActive(false);
    }
}
