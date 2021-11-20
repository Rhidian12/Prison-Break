using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Vector3 m_PatrolTarget;
    [SerializeField] private float m_TimeToNoticePlayer;

    private Rigidbody m_Rigidbody;
    private NavMeshAgent m_NavmeshAgent;
    Blackboard m_Blackboard;
    BehaviourTree m_BehaviourTree;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_NavmeshAgent = GetComponent<NavMeshAgent>();

        m_Blackboard = new Blackboard();

        /* General Usage */
        m_Blackboard.AddData("Rigidbody", m_Rigidbody);
        m_Blackboard.AddData("NavMeshAgent", m_NavmeshAgent);

        /* Check if player is in FOV */
        m_Blackboard.AddData("DetectionRadiusFOV", 10f);
        m_Blackboard.AddData("DetectionAngleXFOV", 89f / 2f);
        m_Blackboard.AddData("DetectionAngleYFOV", 135f / 2f); /* Apparently a human has a FOV of 135 degrees */
        m_Blackboard.AddData("HasPlayerBeenSpotted", false);

        /* Patrol */
        m_Blackboard.AddData("PatrolSpeed", 3f);
        m_Blackboard.AddData("RotationSpeed", 75f);
        m_Blackboard.AddData("ShouldGoToEnd", true);
        m_Blackboard.AddData("PatrolDetectionRange", 4f);
        m_Blackboard.AddData("PatrolStart", m_Rigidbody.position);
        m_Blackboard.AddData("PatrolEnd", m_PatrolTarget);

        /* Notice */
        m_Blackboard.AddData("TimeToNoticePlayer", m_TimeToNoticePlayer);
        m_Blackboard.AddData("TimeToNoticePlayerTimer", 0f);
        m_Blackboard.AddData("HasPlayerBeenNoticed", false);

        m_BehaviourTree = new BehaviourTree(m_Blackboard,
            new BehaviourSelector(new List<IBehaviour>
                {
                    new BehaviourSequence(new List<IBehaviour>
                    {
                        new BehaviourAction(EnemyBehaviours.CheckIfPlayerIsInFOV), /* Check FOV for player */
                        new BehaviourSelector(new List<IBehaviour>
                        {
                            new BehaviourSequence(new List<IBehaviour> /* If player is in FOV, notice him after some time */
                            {
                                new BehaviourConditional(EnemyBehaviours.IsPlayerInFOV),
                                new BehaviourAction(EnemyBehaviours.Notice)
                            }),
                            new BehaviourSequence(new List<IBehaviour> /* If player is not in FOV, patrol */
                            {
                                new BehaviourInvertedConditional(EnemyBehaviours.IsPlayerInFOV),
                                new BehaviourAction(EnemyBehaviours.Patrol)
                            })
                        })
                    })
                })
            );
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(m_Rigidbody.position, transform.forward, Color.red, 0.1f, false);

        m_BehaviourTree.Update();
    }
}
