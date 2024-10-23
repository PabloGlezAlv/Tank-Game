
using UnityEngine;
using UnityEngine.AI;

public enum TankType { Player, TankAI1 }

[RequireComponent(typeof(NavMeshAgent))]
public class TankMovement : MonoBehaviour
{
    [SerializeField]
    private TankType _type = TankType.Player;

    private NavMeshAgent agent;

    private bool startMovement = false;
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void MovePlayer(Vector3 point)
    {
        startMovement = true;
        agent.SetDestination(point);
    }

    void Update()
    {
        if (startMovement && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
            {
                OnDestinationReached();
            }
        }
    }

    void OnDestinationReached()
    {
        startMovement = false;
        GameManager.Instance.TankMoved(_type);
    }
}
