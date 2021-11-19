using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public sealed class EnemyBehaviours
{
    public static BehaviourState CheckIfPlayerIsInFOV(Blackboard blackboard)
    {
        int detectionRadius = blackboard.GetData<int>("DetectionRadiusFOV");
        int detectionAngleX = blackboard.GetData<int>("DetectionAngleXFOV");
        int detectionAngleY = blackboard.GetData<int>("DetectionAngleYFOV");
        Rigidbody rigidbody = blackboard.GetData<Rigidbody>("Rigidbody");

        blackboard.ChangeData("HasPlayerBeenSpotted", false);

        for (int x = -detectionAngleX; x < detectionAngleX; ++x)
        {
            for (int y = -detectionAngleY; y < detectionAngleY; ++y)
            {
                /* Raycast everything except enemies */
                int layerMask = ~LayerMask.GetMask("Enemy");

                Vector3 direction = Quaternion.AngleAxis(x, Vector3.right) * rigidbody.transform.forward;
                direction = Quaternion.AngleAxis(y, Vector3.up) * direction;

                if (Physics.Raycast(rigidbody.position, direction.normalized, out RaycastHit raycastHit, detectionRadius, layerMask))
                {
                    /* Check if the raycast hit the player */

                    if (raycastHit.collider.gameObject.layer.ToString().Equals("Player"))
                    {
                        blackboard.ChangeData("HasPlayerBeenSpotted", true);
                        return BehaviourState.Success;
                    }
                }
            }
        }

        return BehaviourState.Success;
    }

    public static bool IsPlayerInFOV(Blackboard blackboard)
    {
        return blackboard.GetData<bool>("HasPlayerBeenSpotted");
    }

    public static BehaviourState Patrol(Blackboard blackboard)
    {
        Rigidbody rigidbody = blackboard.GetData<Rigidbody>("Rigidbody");
        NavMeshAgent navMeshAgent = blackboard.GetData<NavMeshAgent>("NavMeshAgent");
        float patrolSpeed = blackboard.GetData<float>("PatrolSpeed");
        float rotationSpeed = blackboard.GetData<float>("RotationSpeed");
        float patrolDetectionRange = blackboard.GetData<float>("PatrolDetectionRange");
        bool shouldGoToEnd = blackboard.GetData<bool>("ShouldGoToEnd");
        Vector3 endTarget = blackboard.GetData<Vector3>("PatrolEnd");
        Vector3 startTarget = blackboard.GetData<Vector3>("PatrolStart");

        navMeshAgent.speed = patrolSpeed;
        navMeshAgent.angularSpeed = rotationSpeed;

        Vector3 target = Vector3.zero;
        if (shouldGoToEnd)
            target = endTarget;
        else
            target = startTarget;

        if (Vector3.SqrMagnitude(target - rigidbody.position) <= patrolDetectionRange)
            shouldGoToEnd = !shouldGoToEnd; /* Go to the other target */

        if (shouldGoToEnd)
            target = endTarget;
        else
            target = startTarget;

        navMeshAgent.destination = target;

        return BehaviourState.Success;
    }

    public static BehaviourState Notice(Blackboard blackboard)
    {
        bool isPlayerSpotted = blackboard.GetData<bool>("HasPlayerBeenSpotted");
        bool isPlayerNoticed = blackboard.GetData<bool>("HasPlayerBeenNoticed");
        float timeToNoticePlayer = blackboard.GetData<float>("TimeToNoticePlayer");
        float timeToNoticePlayerTimer = blackboard.GetData<float>("TimeToNoticePlayerTimer");


        /* did our raycast see the player? */
        if (isPlayerSpotted)
        {
            /* increment notice timer */
            timeToNoticePlayerTimer += Time.deltaTime;

            /* if the notice timer is bigger than the time we need to notice a player, we've officially noticed the player */
            if (timeToNoticePlayerTimer >= timeToNoticePlayer)
                isPlayerNoticed = true;
        }
        else
        {
            /* reset the player notice timer */
            timeToNoticePlayerTimer = 0f;
            isPlayerNoticed = false;
        }

        return BehaviourState.Success;
    }
}