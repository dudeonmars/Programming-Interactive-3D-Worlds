using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCNavigation : MonoBehaviour
{
    private NavMeshAgent npcNavAgent;

    [SerializeField]
    private GameObject playerObject;

    private LineRenderer lineToPlayer;

    [SerializeField]
    private float updatePathInterval = 0.5f; 

    private float pathUpdateTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        npcNavAgent = transform.GetComponent<NavMeshAgent>();

        lineToPlayer = GetComponent<LineRenderer>();
        lineToPlayer.startWidth = 0.2f;
        lineToPlayer.endWidth = 0.2f;
        lineToPlayer.positionCount = 0;

        // If player object is not assigned, try to find it by tag
        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerObject == null)
        {
            return;
        }

        // Update path to player at intervals
        pathUpdateTimer += Time.deltaTime;
        if (pathUpdateTimer >= updatePathInterval)
        {
            NavigateToPlayer();
            pathUpdateTimer = 0f;
        }

        // Draw path if NPC has a path
        if (npcNavAgent.hasPath)
        {
            DrawPath();
        }
    }

    private void NavigateToPlayer()
    {
        if (playerObject != null)
        {
            npcNavAgent.SetDestination(playerObject.transform.position);
        }
    }

    private void DrawPath()
    {
        int pathLength = npcNavAgent.path.corners.Length;

        lineToPlayer.positionCount = pathLength;

        lineToPlayer.SetPosition(0, transform.position);

        if (pathLength < 2)
        {
            return;
        }

        for (int i = 1; i < pathLength; i++)
        {
            Vector3 pointPos = npcNavAgent.path.corners[i];
            lineToPlayer.SetPosition(i, pointPos);
        }
    }
}
