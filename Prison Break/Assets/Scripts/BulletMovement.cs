using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMovement : MonoBehaviour
{
    [SerializeField] private float m_Speed;

    private Vector3 m_Velocity;
    private Rigidbody m_Rigidbody;

    public Vector3 Velocity
    {
        get => m_Velocity;
        set
        {
            m_Velocity = value.sqrMagnitude <= 1f ? value : value.normalized;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        m_Rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.tag.Equals("Bullet"))
            Destroy(gameObject);
    }

    void FixedUpdate()
    {
        m_Rigidbody.velocity = m_Velocity * m_Speed * Time.fixedDeltaTime;
    }
}
