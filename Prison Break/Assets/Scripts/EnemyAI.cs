using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
        Patrolling,
        NoticedPlayer,
        Attacking
    }

    [SerializeField] private State m_EnemyState;

    private Dictionary<string, EnemyBehaviour> m_EnemyBehaviours;
    private Rigidbody m_Rigidbody;
    private NavMeshAgent m_NavmeshAgent;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_NavmeshAgent = GetComponent<NavMeshAgent>();

        EnemyPatrol patrol = new EnemyPatrol(m_Rigidbody, m_NavmeshAgent);
        EnemyNotice notice = new EnemyNotice(m_Rigidbody, m_NavmeshAgent);

        m_EnemyBehaviours.Add("patrol", patrol);
        m_EnemyBehaviours.Add("notice", notice);

        /*
            Notice  =>   Patrol
         */

        foreach (KeyValuePair<string, EnemyBehaviour> behaviour in m_EnemyBehaviours)
            behaviour.Value.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        /* Trust me, this disgusts me more than it does you */
        switch (m_EnemyState)
        {
            case State.NoticedPlayer:
                {
                    var notice = m_EnemyBehaviours["notice"];

                    notice.FixedUpdate();
                    notice.Update();

                    if (((EnemyNotice)notice).HasPlayerBeenNoticed)
                        m_EnemyState = State.Attacking;
                    break;
                }
            case State.Patrolling:
                {
                    var patrol = m_EnemyBehaviours["patrol"];

                    patrol.FixedUpdate();
                    patrol.Update();
                    break;
                }
        }
    }
}
