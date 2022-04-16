using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private bool fight;
    private Transform[] allChildren;
    private NavMeshAgent[] navMeshAgents;

    private void Start()
    {
        allChildren = this.transform.GetComponentsInChildren<Transform>();
        fight = false;
    }

    public void Fight(Transform player)
    {
        navMeshAgents = this.transform.GetComponentsInChildren<NavMeshAgent>();
        foreach (NavMeshAgent child in navMeshAgents)
        {
            if (child.isOnNavMesh)
                child.SetDestination(player.position);
        }
    }
}
