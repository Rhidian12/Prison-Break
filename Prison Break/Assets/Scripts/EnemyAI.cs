using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Vector3 m_PatrolTarget;
    [SerializeField] private float m_TimeToNoticePlayer;
    [SerializeField] private GameObject m_Player;

    private Rigidbody m_Rigidbody;
    private NavMeshAgent m_NavmeshAgent;
    private Blackboard m_Blackboard;
    private BehaviourTree m_BehaviourTree;
    private BaseWeapon m_Weapon;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_NavmeshAgent = GetComponent<NavMeshAgent>();
        m_Weapon = GetComponentInChildren<BaseWeapon>();

        m_Blackboard = new Blackboard();

        /* General Usage */
        m_Blackboard.AddData("Rigidbody", m_Rigidbody);
        m_Blackboard.AddData("NavMeshAgent", m_NavmeshAgent);
        m_Blackboard.AddData("Player", m_Player);
        m_Blackboard.AddData("Weapon", m_Weapon);

        /* Check if player is in FOV */
        m_Blackboard.AddData("DetectionRadiusFOV", 10f);
        m_Blackboard.AddData("DetectionAngleXFOV", 89f);
        m_Blackboard.AddData("DetectionAngleYFOV", 135f); /* Apparently a human has a FOV of 135 degrees */
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

        /* Rotate Towards Player */
        m_Blackboard.AddData("MaxAngleToMiss", 2f);

        /* Attack */

        m_BehaviourTree = new BehaviourTree(m_Blackboard,
            new BehaviourSequence(new List<IBehaviour>
            {
                new BehaviourConditional(EnemyBehaviours.IsPlayerAlive), /* only do something if the player is alive */
                new BehaviourAction(EnemyBehaviours.CheckIfPlayerIsInFOV), /* Check FOV for player */
                new BehaviourAction(EnemyBehaviours.Notice), /* Try to notice the player */
                new BehaviourAction(EnemyBehaviours.Reload), /* Reload weapon if necessary */

                new BehaviourSelector(new List<IBehaviour> /* Decide what Sub-Tree to Execute */
                {
                    new BehaviourSequence(new List<IBehaviour> /* ATTACK SUB-TREE */
                    {
                        new BehaviourConditional(EnemyBehaviours.IsPlayerNoticed), /* Check if player has been noticed */
                        new BehaviourSequence(new List<IBehaviour> /* if player has been noticed, attack player */
                        {
                            new BehaviourAction(EnemyBehaviours.RotateTowardsPlayer), /* First, rotate towards the player */
                            new BehaviourConditional(EnemyBehaviours.IsAimingAtPlayer), /* Next, check if we're aiming at the player */
                            new BehaviourAction(EnemyBehaviours.Attack) /* if we are aiming at the player, fire */
                        })
                    }),

                    new BehaviourSequence(new List<IBehaviour> /* PATROL SUB-TREE */
                    {
                        new BehaviourInvertedConditional(EnemyBehaviours.IsPlayerInFOV),
                        new BehaviourAction(EnemyBehaviours.Patrol)
                    })
                })
            })
        );
    }

    // Update is called once per frame
    void Update()
    {
        m_NavmeshAgent.speed = 0f;
        m_NavmeshAgent.angularSpeed = 0f;

        m_BehaviourTree.Update();
    }
}
