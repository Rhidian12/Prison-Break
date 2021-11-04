using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : EnemyBehaviour
{
    [SerializeField] private Vector3 m_StartLocation = Vector3.zero;
    [SerializeField] private Vector3 m_EndLocation = Vector3.zero;
    [SerializeField] private float m_PatrolSpeed = 300f;
    [SerializeField] private float m_RotationSpeed = 150f;

    private bool m_ShouldGoToEnd = true;
    private float m_DetectionRadius = 10f;
    private Rigidbody m_Rigidbody;
    private Vector2 m_CurrentRotation = new Vector2(45f, 45f);

    void Start()
    {
        m_DetectionRadius *= m_DetectionRadius; /* Square the detection radius */

        m_Rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 target = Vector3.zero;
        if (m_ShouldGoToEnd)
            target = m_EndLocation;
        else
            target = m_StartLocation;

        if (Vector3.SqrMagnitude(target - m_Rigidbody.position) <= m_DetectionRadius)
        {
            /* Go to the other target */
            m_ShouldGoToEnd = !m_ShouldGoToEnd;
        }

        /* Rotate towards the target */
        float angleToMove = Vector3.Angle(m_Rigidbody.transform.forward, target);
    }

    private void FixedUpdate()
    {
        /* Move to the target */
        Vector3 target = Vector3.zero;
        if (m_ShouldGoToEnd)
            target = m_EndLocation;
        else
            target = m_StartLocation;

        Vector3 velocity = Vector3.zero;
        velocity += (target - transform.position).normalized;
        velocity *= m_PatrolSpeed * Time.fixedDeltaTime;
        velocity.y = m_Rigidbody.velocity.y;

        m_Rigidbody.velocity = velocity;
    }
}
