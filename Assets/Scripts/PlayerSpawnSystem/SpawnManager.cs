using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject playerPrefab;
    public SpawnPoint[] spawnPoints;
    public float respawnDelay = 3f;
    
    [Header("Current Player Reference")]
    public GameObject currentPlayer;
    
    [Header("Orbs Spawn Settings")]
    [SerializeField] private List<OrbData> m_orbDatas;
    
    private void Start()
    {
        // If no player is assigned, try to find one in the scene
        if (currentPlayer == null)
        {
            currentPlayer = GameObject.FindGameObjectWithTag("Player");
        }
        
        
        spawnPoints = FindObjectsOfType<SpawnPoint>().ToArray();
        
        
        // If still no player and we have a prefab, spawn one
        if (currentPlayer == null && playerPrefab != null)
        {
            SpawnPlayer();
        }

    }
    
    public void PlayerDied()
    {
        StartCoroutine(ReloadLevel());
    }
    
    private IEnumerator ReloadLevel()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(respawnDelay);
        
        // Destroy the current player if it exists
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }
        
        // Spawn a new player
        ReLoadScene();
    }

    private void ReLoadScene()
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex);
    }
    
    private void SpawnPlayer()
    {
        Debug.Log("Spawning player");
        
        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab is not assigned!");
            return;
        }
        
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return;
        }
        
        // Choose a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)].transform;
        
        // Instantiate the player at the spawn point
        currentPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        
        // Get the PlayerHealth component and set up the death event
        PlayerHealth playerHealth = currentPlayer.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.onPlayerDeath += PlayerDied;
        }
        
        OrbManager orbManager = currentPlayer.GetComponent<OrbManager>();
        orbManager.SetOrbs(m_orbDatas);
    }
}
