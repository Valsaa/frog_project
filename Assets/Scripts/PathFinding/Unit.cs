using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit : MonoBehaviour
{
    const float minPathUpdateTime = 0.2f;
    const float pathUpdateMoveThreshold = 0.5f;

    public Transform target;
    public float speed = 20;
    public float turnSpeed = 3;
    public float turnDst = 5;
    public float stoppingDst = 10;

    private Path path;
    private PathRequest lastRequest;

    private Vector3 nextPoint;
    private float distanceFromPoint = 0;
    private int pathIndex = 0;
    private bool followingPath = false;

    void Start()
    {
        StartCoroutine(UpdatePath());
    }

    public void OnPathFound(PathRequest result)
    {
        Debug.Log("OnPathFound");
        if (result.pipelineState == PathState.Calculated)
        {
            path = new Path(result.path, result.pathStart, turnDst, stoppingDst);
            pathIndex = 0;
            followingPath = true;
            transform.LookAt(path.lookPoints[0]);
            nextPoint = path.lookPoints[0];
        }
    }

    IEnumerator UpdatePath()
    {
        if (Time.timeSinceLevelLoad < 0.3f)
        {
            yield return new WaitForSeconds(0.3f);            
        }
        SearchPath();

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target.transform.position;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if ((target.transform.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                SearchPath();
                targetPosOld = target.transform.position;
            }            
        }
    }


    void Update()
    {
        FollowPath();
    }

    private void FollowPath()
    {
        if (path != null && followingPath)
        {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);

            // Check if the next line has been crossed
            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
            {
                if (pathIndex == path.finishLineIndex) followingPath = false;
                else pathIndex++;

                nextPoint = path.lookPoints[pathIndex];
            }

            if (followingPath)
            {
                float speedPercent = 1;
                if (pathIndex >= path.slowDownIndex && stoppingDst > 0)
                {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDst);
                    if (speedPercent < 0.01f)
                    {
                        followingPath = false;
                    }
                }

                Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
                transform.Translate(Vector3.forward * speed * speedPercent * Time.deltaTime, Space.Self);
            }
            else
            {
                path = null;
            }
        }        
    }

    private void SearchPath()
    {
        lastRequest = new PathRequest(transform.position, target.position, OnPathFound);       
        PathRequestManager.RequestPath(lastRequest);
    }

    void OnDrawGizmos()
    {
        if (path != null)
        {
            path.DrawWithGizmos();

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(nextPoint + Vector3.up, 0.5f);
            
            
            Vector3 direction = path.lookPoints[path.lookPoints.Length - 2] - path.lookPoints[path.lookPoints.Length - 1];
            Gizmos.DrawLine(path.lookPoints[path.lookPoints.Length - 1] + Vector3.up + Vector3.up+ Vector3.up, path.lookPoints[path.lookPoints.Length - 1] + (direction.normalized * distanceFromPoint) + Vector3.up + Vector3.up + Vector3.up);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(path.lookPoints[path.lookPoints.Length - 1] + Vector3.up + Vector3.up, path.lookPoints[path.lookPoints.Length - 1] + (direction.normalized * stoppingDst) + Vector3.up + Vector3.up);
        }
    }
}
