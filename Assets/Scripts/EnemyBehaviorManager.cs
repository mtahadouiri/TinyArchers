using UnityEngine;
using UnityEngine.AI;
public class EnemyBehaviorManager : MonoBehaviour
{

    const int MaxDistance = 3;
    int layerMask = 1 << 10;
    public bool climbingSequence;
    Vector3 yOffsetClimbPosition = new Vector3(0, 3, 0);

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<Enemy>().canRayCast)
        {
            RaycastHit hit;
            RaycastHit hitLeft;
            RaycastHit hitRight;
            Vector3 origin = new Vector3(transform.position.x, 1, transform.position.z);
            if (Physics.Raycast(origin, transform.TransformDirection(Vector3.forward), out hit, MaxDistance, layerMask))
            {
                Debug.DrawRay(origin, transform.TransformDirection(Vector3.forward) * hit.distance, Color.green);
                if (!climbingSequence)
                {
                    Debug.Log("Climbing");
                    ClimbOtherEnemy(hit.collider.gameObject);
                }
            }
            if (Physics.Raycast(origin, transform.TransformDirection(new Vector3(1, 0, 2)), out hitLeft, MaxDistance, layerMask))
            {
                Debug.DrawRay(origin, transform.TransformDirection(new Vector3(1, 0, 2)) * hitLeft.distance, Color.green);
                if (!climbingSequence)
                {
                    Debug.Log("Climbing");
                    ClimbOtherEnemy(hitLeft.collider.gameObject);
                }
            }
            if (Physics.Raycast(origin, transform.TransformDirection(-1, 0, 2), out hitRight, MaxDistance, layerMask))
            {
                Debug.DrawRay(origin, transform.TransformDirection(-1, 0, 2) * hitRight.distance, Color.green);
                if (!climbingSequence)
                {
                    Debug.Log("Climbing");
                    ClimbOtherEnemy(hitRight.collider.gameObject);
                }
            }
            else
            {
                Debug.DrawRay(origin, transform.TransformDirection(Vector3.forward) * MaxDistance, Color.red);
                Debug.DrawRay(origin, transform.TransformDirection(new Vector3(1, 0, 2)).normalized * MaxDistance, Color.red);
                Debug.DrawRay(origin, transform.TransformDirection(new Vector3(-1, 0, 2)).normalized * MaxDistance, Color.red);
            }
        }
    }

    void ClimbOtherEnemy(GameObject enemy)
    {
        climbingSequence = !climbingSequence;
        NavMeshAgent navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        gameObject.GetComponent<Enemy>().animator.SetBool("walking", false);
        navMeshAgent.isStopped = true;
        navMeshAgent.enabled = false;
        transform.position = enemy.transform.position + yOffsetClimbPosition;
        climbingSequence = !climbingSequence;
    }
}
