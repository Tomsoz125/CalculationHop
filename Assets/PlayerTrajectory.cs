using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerTrajectory : MonoBehaviour
{
    public float _vel = .5f;
    public float _mass = -9.81f;
    public float _force = 500f;
    public LayerMask ground;

    const float groundRadius = .2f;

    public void Trajectory(Quaternion dir) {
        LineRenderer lr = gameObject.GetComponent<LineRenderer>();
        if (lr == null) {
            Debug.LogError("Line Renderer not found!");
            return;
        }

        Vector3[] positions = Plot(dir.).ToArray();
        lr.positionCount = positions.Length;
        lr.SetPositions(positions);
    }

    public Vector3[] Plot(float dir) {
        float maxDuration = 5f;
        float timeStepInterval = 0.1f;
        int maxSteps = (int)(maxDuration / timeStepInterval);
        Vector3[] lineRendererPoints = new Vector3[maxSteps];
        Vector3 directionVector = transform.up;
        Vector3 launchPosition = transform.position + transform.up;
        _vel = _force / _mass * Time.deltaTime;

        for (int i = 0; i < maxSteps; ++i) {
            Vector3 calculatedPosition = launchPosition + directionVector * _vel * i * timeStepInterval;
            calculatedPosition.y += Physics2D.gravity.y/2 * Mathf.Pow(i * timeStepInterval, 2);

            lineRendererPoints.Append(calculatedPosition);

            if (CheckForCollision(calculatedPosition)) break;
        }

        Debug.Log(lineRendererPoints.);
        return lineRendererPoints;
    }

    public bool CheckForCollision(Vector3 position) {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, groundRadius, ground);

        return colliders.Length > 0;;
    }
}
