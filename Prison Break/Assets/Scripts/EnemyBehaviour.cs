using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyBehaviours
{
    public static bool IsPlayerInFOV(Blackboard blackboard)
    {
        int detectionRadius = blackboard.GetData<int>("DetectionRadiusFOV");
        int detectionAngleX = blackboard.GetData<int>("DetectionAngleXFOV");
        int detectionAngleY = blackboard.GetData<int>("DetectionAngleYFOV");
        Rigidbody rigidbodyPlayer = blackboard.GetData<Rigidbody>("RigidbodyPlayer");

        blackboard.ChangeData("HasPlayerBeenSpotted", false);

        for (int x = -detectionAngleX; x < detectionAngleX; ++x)
        {
            for (int y = -detectionAngleY; y < detectionAngleY; ++y)
            {
                /* Raycast everything except enemies */
                int layerMask = ~LayerMask.GetMask("Enemy");

                Vector3 direction = Quaternion.AngleAxis(x, Vector3.right) * rigidbodyPlayer.transform.forward;
                direction = Quaternion.AngleAxis(y, Vector3.up) * direction;

                if (Physics.Raycast(rigidbodyPlayer.position, direction.normalized, out RaycastHit raycastHit, detectionRadius, layerMask))
                {
                    /* Check if the raycast hit the player */

                    if (raycastHit.collider.gameObject.layer.ToString().Equals("Player"))
                    {
                        blackboard.ChangeData("HasPlayerBeenSpotted", true);
                        return true;
                    }
                }
            }
        }

        return false;
    }
}