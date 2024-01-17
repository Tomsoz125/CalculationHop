using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerTrajectory : MonoBehaviour
{
    public float _velX = .5f;
    public float _velY = .5f;

    const float groundRadius = .5f;

    private PlayerController pc;

    public void Trajectory(Quaternion angle, Vector3 force, PlayerController pcL) {
        pc = pcL;

        LineRenderer lr = gameObject.GetComponent<LineRenderer>();
        if (lr == null) {
            Debug.LogError("Line Renderer not found!");
            return;
        }

        Vector3[] positions = Plot(angle, force);
        lr.positionCount = positions.Count();
        lr.SetPositions(positions);
    }

    public Vector3[] Plot(Quaternion angle, Vector3 force) {
        // List<Vector3> points = new List<Vector3>();
        // float maxDuration = 5f;
        // float timeStepInterval = 0.1f;
        // int maxSteps = (int)(maxDuration / timeStepInterval);
        // // Vector2 directionVector = angle;
        // Vector3 launchPosition = angle*transform.position;
        // float velocity = (force.x + force.y) / gameObject.GetComponent<Rigidbody2D>().mass * Time.fixedDeltaTime;

        // float velocityX = force.x / gameObject.GetComponent<Rigidbody2D>().mass * Time.fixedDeltaTime;
        // float velocityY = force.y / gameObject.GetComponent<Rigidbody2D>().mass * Time.fixedDeltaTime;

        // for (int i = 0; i < maxSteps; ++i) {
        //     // if (i == 0) {
        //     //     points.Add(transform.position);
        //     //     continue;
        //     // }

        //     Vector3 calculatedPositionX = transform.position + launchPosition * velocityX * timeStepInterval;
        //     Vector3 calculatedPositionY = transform.position + launchPosition * velocityY * timeStepInterval;
        //     Vector3 calculatedPosition = new Vector3(calculatedPositionX.x*-1, calculatedPositionY.y*-1); // TODO: VERY CLOSE TO GETTING THIS TO WORK!!!! :):) IT JUST OVERPREDICTS THE VELOCITY HUGELY

        //     calculatedPosition.y += Physics2D.gravity.y/2 * Mathf.Pow(i * timeStepInterval, 2);
        //     Debug.Log(calculatedPosition);

        //     points.Add(calculatedPosition);

        //     if (CheckForCollision(calculatedPosition)) break;
        // }
        
        // // for (int i = 0; i < points.Count; i++) {
        // //     Debug.Log(i + ": " + points.ToArray()[i]);
        // // }

        // return points.ToArray();


        List<Vector3> points = new List<Vector3>();

        float maxDuration = 5f;
        float timeStepInterval = 0.1f;
        int maxSteps = (int)(maxDuration / timeStepInterval);

        // -------------------------
        // X AXIS
        // -------------------------

        float launchPositionX = transform.position.x;
        _velX = force.x / gameObject.GetComponent<Rigidbody2D>().mass * Time.deltaTime;

        // -------------------------
        // Y AXIS
        // -------------------------

        float launchPositionY = transform.position.y;
        _velY = force.y / gameObject.GetComponent<Rigidbody2D>().mass * Time.deltaTime;
        Debug.Log("X: " + _velX + "   Y: " + _velY);



        for (int i = 0; i < maxSteps; ++i) {
            float calculatedPositionX = (float)((launchPositionX * _velX * i * timeStepInterval) + -1.03);
            float calculatedPositionY = (float)((launchPositionY * _velY * i * timeStepInterval) + -1.67);

            Vector3 calculatedPosition = new Vector3(calculatedPositionX, calculatedPositionY);
            calculatedPosition.y += Physics2D.gravity.y/2 * Mathf.Pow(i * timeStepInterval, 2);

            points.Add(angle * calculatedPosition);

            if (CheckForCollision(calculatedPosition)) break;
        }

        return points.ToArray();
    }

    public bool CheckForCollision(Vector3 position) {
        if (pc == null) return true;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, groundRadius, pc.ground);

        return colliders.Length > 0;
    }

    public void HideLine() {
        LineRenderer lr = gameObject.GetComponent<LineRenderer>();
        if (lr == null) {
            Debug.LogError("Line Renderer not found!");
            return;
        }
        lr.SetPositions(new List<Vector3>().ToArray());
    }
}
