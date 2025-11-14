using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{ 
    private NavMeshAgent agent;
    [SerializeField] private GameObject playerObj;
    
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // NavMeshPlus 2D setup
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // Force enemy onto the NavMesh plane (usually z = 0)
        var pos = transform.position;
        transform.position = new Vector3(pos.x, pos.y, 0f);
    }

    void Start()
    {
        if (!agent.isOnNavMesh)
        {
            Debug.LogError("Enemy is NOT on NavMesh at Start. Check Z position and NavMesh baking.");
        }
    }

    void Update()
    {
        if (agent == null || playerObj == null) return;

        agent.SetDestination(playerObj.transform.position);
    }
}