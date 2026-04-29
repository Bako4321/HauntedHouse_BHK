using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointPatrol : MonoBehaviour
{
    public float minSpeed = 1.0f;
    public float maxSpeed = 4.0f;
    public float currentMoveSpeed = 1.0f;

    public Transform[] waypoints;

    private Rigidbody m_RigidBody;
    int m_CurrentWaypointIndex;

    void Start ()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        currentMoveSpeed = Random.Range(minSpeed, maxSpeed);
    }

    void FixedUpdate ()
    {
        Transform currentWaypoint = waypoints[m_CurrentWaypointIndex];
        Vector3 currentToTarget = currentWaypoint.position - m_RigidBody.position;

        if (currentToTarget.magnitude < 0.1f)
        {
            m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
            currentMoveSpeed = Random.Range(minSpeed, maxSpeed); 
        }
        Quaternion forwardRotation = Quaternion.LookRotation(currentToTarget);
        m_RigidBody.MoveRotation(forwardRotation);
        m_RigidBody.MovePosition(m_RigidBody.position + currentToTarget.normalized * currentMoveSpeed * Time.deltaTime);
    }
}