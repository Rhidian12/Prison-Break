using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : EnemyBehaviour
{
    [SerializeField] private Vector3 m_StartLocation = Vector3.zero;
    [SerializeField] private Vector3 m_EndLocation = Vector3.zero;
    [SerializeField] private float m_PatrolSpeed = 300f;
    [SerializeField] private float m_RotationSpeed = 150f;

    private bool m_ShouldGoToEnd = true;
    private float m_DetectionRadius = 2f;
    private Rigidbody m_Rigidbody;
    private NavMeshAgent m_NavmeshAgent;

    void Start()
    {
        m_DetectionRadius *= m_DetectionRadius; /* Square the detection radius */

        m_Rigidbody = GetComponent<Rigidbody>();
        m_NavmeshAgent = GetComponent<NavMeshAgent>();

        m_NavmeshAgent.speed = m_PatrolSpeed;
        m_NavmeshAgent.angularSpeed = m_RotationSpeed;
    }

    void Update()
    {
        Vector3 target = Vector3.zero;
        if (m_ShouldGoToEnd)
        {
            target = m_EndLocation;
            m_NavmeshAgent.destination = m_EndLocation;
        }
        else
        {
            m_NavmeshAgent.destination = m_StartLocation;
            target = m_StartLocation;
        }

        if (Vector3.SqrMagnitude(target - m_Rigidbody.position) <= m_DetectionRadius)
            m_ShouldGoToEnd = !m_ShouldGoToEnd; /* Go to the other target */
    }
}
