using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float m_Speed;

    private Vector3 m_DesiredVelocity;
    private Rigidbody m_RigidBody;

    public Vector3 DesiredVelocity
    {
        get => m_DesiredVelocity;
        set
        {
            m_DesiredVelocity = value.sqrMagnitude <= 1f ? value : value.normalized;
        }
    }

    public Rigidbody Rigidbody
    {
        get => m_RigidBody;
    }

    private void Awake()
    {
        m_RigidBody = gameObject.GetComponentInParent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Vector3 newVelocity = Vector3.zero;
        newVelocity += m_DesiredVelocity;
        newVelocity *= m_Speed * Time.fixedDeltaTime;
        newVelocity.y = m_RigidBody.velocity.y;

        m_RigidBody.velocity = newVelocity;

        m_RigidBody.angularVelocity = Vector3.zero;
    }
}