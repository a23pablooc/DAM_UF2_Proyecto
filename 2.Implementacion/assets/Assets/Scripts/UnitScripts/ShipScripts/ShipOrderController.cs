using UnityEngine;
using UnityEngine.AI;

namespace UnitScripts.ShipScripts
{
    public class ShipOrderController : MonoBehaviour
    {
        private NavMeshAgent _agent;

        private void Start()
        {
            _agent = GetComponent<NavMeshAgent>();
        }

        public void SetDestination(Vector3 destination)
        {
            _agent.SetDestination(destination);
        }
    }
}