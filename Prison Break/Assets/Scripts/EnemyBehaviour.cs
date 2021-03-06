using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public sealed class EnemyBehaviours
{
    public static BehaviourState CheckIfPlayerIsInFOV(Blackboard blackboard)
    {
        float detectionRadius = blackboard.GetData<float>("DetectionRadiusFOV");
        float detectionAngleX = blackboard.GetData<float>("DetectionAngleXFOV");
        float detectionAngleY = blackboard.GetData<float>("DetectionAngleYFOV");
        Rigidbody rigidbody = blackboard.GetData<Rigidbody>("Rigidbody");
        GameObject player = blackboard.GetData<GameObject>("Player");

        blackboard.ChangeData("HasPlayerBeenSpotted", false);

        Vector3 toPlayer = player.transform.position - rigidbody.position;

        Debug.DrawRay(rigidbody.position, toPlayer, Color.red, .1f, false);

        float angle = Vector3.Angle(rigidbody.transform.forward, toPlayer.normalized);
        if (angle <= detectionAngleY /*&& angle <= detectionAngleX*/)
        {
            /* Raycast everything except enemies */
            int layerMask = ~LayerMask.GetMask("Enemy");

            if (Physics.Raycast(rigidbody.position, toPlayer.normalized, out RaycastHit raycastHit, detectionRadius, layerMask))
            {
                /* Check if the raycast hit the player */
                if (raycastHit.collider.gameObject.CompareTag("Player"))
                {
                    blackboard.ChangeData("HasPlayerBeenSpotted", true);
                    return BehaviourState.Success;
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

        blackboard.ChangeData("ShouldGoToEnd", shouldGoToEnd);

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

        blackboard.ChangeData("TimeToNoticePlayerTimer", timeToNoticePlayerTimer);
        blackboard.ChangeData("HasPlayerBeenNoticed", isPlayerNoticed);

        return BehaviourState.Success;
    }

    public static bool IsPlayerNoticed(Blackboard blackboard)
    {
        return blackboard.GetData<bool>("HasPlayerBeenNoticed");
    }

    public static BehaviourState RotateTowardsPlayer(Blackboard blackboard)
    {
        if (!IsPlayerNoticed(blackboard))
            return BehaviourState.Failure;

        GameObject player = blackboard.GetData<GameObject>("Player");
        Rigidbody rigidbody = blackboard.GetData<Rigidbody>("Rigidbody");

        Vector3 toPlayer = player.transform.position - rigidbody.position;

        float angleDifference = Vector3.SignedAngle(rigidbody.transform.forward, toPlayer.normalized, Vector3.up);

        if (angleDifference >= 180f)
            angleDifference = 360f - angleDifference;

        rigidbody.transform.Rotate(new Vector3(0f, angleDifference * Time.deltaTime * rigidbody.maxAngularVelocity, 0f), Space.Self);

        if (IsAimingAtPlayer(blackboard))
            return BehaviourState.Success;
        else
            return BehaviourState.Running;
    }

    public static bool IsAimingAtPlayer(Blackboard blackboard)
    {
        GameObject player = blackboard.GetData<GameObject>("Player");
        Rigidbody rigidbody = blackboard.GetData<Rigidbody>("Rigidbody");
        float maxAngleToMiss = blackboard.GetData<float>("MaxAngleToMiss");

        Vector3 toPlayer = player.transform.position - rigidbody.position;

        return Vector3.Angle(rigidbody.transform.forward, toPlayer.normalized) <= maxAngleToMiss;
    }

    public static BehaviourState Attack(Blackboard blackboard)
    {
        BaseWeapon weapon = blackboard.GetData<BaseWeapon>("Weapon");

        /* I'll do this if I have time */
        /* make sure we can miss a shot AKA NO PERFECT ACCURACY */

        if (weapon.CanFire())
            weapon.FireBullet(~LayerMask.GetMask("Enemy"));

        return BehaviourState.Success;
    }

    public static bool IsPlayerAlive(Blackboard blackboard)
    {
        GameObject player = blackboard.GetData<GameObject>("Player");

        if (player == null)
            return false;

        return player.GetComponent<HealthScript>().GetMaxHealth > 0f;
    }
    
    public static BehaviourState Reload(Blackboard blackboard)
    {
        BaseWeapon weapon = blackboard.GetData<BaseWeapon>("Weapon");

        if (weapon.GetClips.Count > 0)
            if (weapon.GetClips[0].AmountOfRemainingBullets <= 0)
                weapon.Reload();

        return BehaviourState.Success;
    }
}