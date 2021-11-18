using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyNotice : EnemyBehaviour
{
    [SerializeField] private float m_DetectionRadius = 0f;
    [SerializeField] private int m_DetectionAngleX = 0;
    [SerializeField] private int m_DetectionAngleY = 0;
    [SerializeField] private float m_TimeToNoticePlayer = 0f;
    [SerializeField] private GameObject m_Player;

    private NavMeshAgent m_NavmeshAgent = null;
    private Rigidbody m_Rigidbody = null;
    private bool m_IsPlayerSpotted = false;
    private bool m_IsPlayerNoticed = false;
    private float m_TimeToNoticePlayerTimer = 0f;

    public bool HasPlayerBeenNoticed
    {
        get => m_IsPlayerNoticed;
    }

    // Start is called before the first frame update
    public EnemyNotice(Rigidbody rigidbody, NavMeshAgent agent)
    {
        m_Rigidbody = rigidbody;
        m_NavmeshAgent = agent;
    }

    public override void Initialize() {}

    // Update is called once per frame
    public override void Update()
    {
        /* did our raycast see the player? */
        if (m_IsPlayerSpotted)
        {
            /* increment notice timer */
            m_TimeToNoticePlayerTimer += Time.deltaTime;

            /* if the notice timer is bigger than the time we need to notice a player, we've officially noticed the player */
            if (m_TimeToNoticePlayerTimer >= m_TimeToNoticePlayer)
                m_IsPlayerNoticed = true;
        }
        else
        {
            /* reset the player notice timer */
            m_TimeToNoticePlayerTimer = 0f;
            m_IsPlayerNoticed = false;
        }
    }

    public override void FixedUpdate()
    {
        m_IsPlayerSpotted = false;

        for (int x = -m_DetectionAngleX; x < m_DetectionAngleX; ++x)
        {
            for (int y = -m_DetectionAngleY; y < m_DetectionAngleY; ++y)
            {
                int layerMask = ~LayerMask.GetMask("Enemy");

                Vector3 direction = Quaternion.AngleAxis(x, Vector3.right) * m_Rigidbody.transform.forward;
                direction = Quaternion.AngleAxis(y, Vector3.up) * direction;

                if (Physics.Raycast(m_Rigidbody.position, direction.normalized, out RaycastHit raycastHit, m_DetectionRadius, layerMask))
                {
                    /* Check if the raycast hit the player */

                    if (raycastHit.collider.gameObject.layer.ToString().Equals("Player"))
                    {
                        m_IsPlayerSpotted = true;
                        return; /* no point in wasting time on raycasts if we've already found the player */
                    }
                }
            }
        }
    }
}
