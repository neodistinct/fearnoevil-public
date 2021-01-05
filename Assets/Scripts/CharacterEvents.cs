using System;
using UnityEngine;
using UnityEngine.AI;

public class CharacterEvents : MonoBehaviour
{
    [SerializeField]
    private GameObject attackPoint;

    private NavMeshAgent navMeshAgent;
    private float speed = 0;

    public void Awake()
    {
        navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
       
        speed = navMeshAgent.speed;
    }

    public void SetDamaging(int value)
    {
        if(attackPoint)
        {
            attackPoint.GetComponent<AttackProjectile>().isActive = Convert.ToBoolean(value);

            // Debug.Log("Set non-damaging:  " + value);
        }
        
    }

    public void SetMoving(int value)
    {
        if(navMeshAgent)
        {
            bool isMoving = Convert.ToBoolean(value);

            float newSpeed = isMoving ? speed : 0.1f;

            navMeshAgent.speed = newSpeed;
        }
    }
}
