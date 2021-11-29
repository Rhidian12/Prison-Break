using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Vector3 m_EndPosition;
    [SerializeField] private float m_Speed;

    private bool m_ShouldGoToEnd = false;
    private BoxCollider m_BoxCollider;
    private Rigidbody m_Rigidbody;

    private void Start()
    {
        m_BoxCollider = GetComponent<BoxCollider>();
        m_Rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_ShouldGoToEnd)
        {
            Vector3 toEnd = (m_EndPosition - transform.position).normalized;

            transform.position += toEnd * m_Speed;

            if (Vector3.SqrMagnitude(m_EndPosition - transform.position) <= 10f)
            {
                m_ShouldGoToEnd = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            m_ShouldGoToEnd = true;
            m_BoxCollider.isTrigger = false;
            m_Rigidbody.detectCollisions = false;
        }
    }
}