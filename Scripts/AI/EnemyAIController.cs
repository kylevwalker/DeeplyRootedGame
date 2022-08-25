using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIController : MonoBehaviour
{
    [Header("Connected Game Objects")]
    private GameObject gameManager;
    private GameObject player;
    private GameObject playerCamera;
    private NavMeshAgent enemyNavMeshAgent;

    [Header("Story Components")]
    public GameObject[] resettableShortcuts;

    private void Start()
    {
        gameManager = FindObjectOfType<GameControlsManager>().gameObject;
        player = gameManager.GetComponent<GameControlsManager>().player;
        playerCamera = player.GetComponentInChildren<Camera>().gameObject;
        enemyNavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        enemyNavMeshAgent.SetDestination(player.transform.position);
    }
}
