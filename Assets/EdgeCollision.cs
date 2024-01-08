using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeCollision : MonoBehaviour
{
    void Awake () 
    {
      AddCollider();
    }

    void AddCollider () 
    {
      if (Camera.main==null) {Debug.LogError("Camera.main not found, failed to create edge colliders"); return;}

      var cam = Camera.main;
      if (!cam.orthographic) {Debug.LogError("Camera.main is not Orthographic, failed to create edge colliders"); return;}

      var bottomLeft = (Vector2)cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
      var topLeft = (Vector2)cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight, cam.nearClipPlane));
      var topRight = (Vector2)cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, cam.nearClipPlane));
      var bottomRight = (Vector2)cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, 0, cam.nearClipPlane));

      // add or use existing EdgeCollider2D

      while (GetComponents<EdgeCollider2D>().Length < 2) {
        gameObject.AddComponent<EdgeCollider2D>();
      }
      var edge = GetComponents<EdgeCollider2D>()[0];

      var edgePoints = new [] {bottomLeft,topLeft,topRight,bottomRight, bottomLeft};
      edge.points = edgePoints;
    }

    void OnTriggerEnter (Collider other) {
        Debug.Log("collider");
        if (other.transform.position.y > -4.53 && other.transform.position.y < 4.5) return;

        var ballCollider = other.GetComponent<CircleCollider2D>();
        if (ballCollider == null) {
            Debug.LogError("Couldn't find collider on ball");
            return;
        }


        ballCollider.enabled = false;
    }

    void OnTriggerExit (Collider other) {
        var ballCollider = other.GetComponent<CircleCollider2D>();
        if (ballCollider == null) {
            Debug.LogError("Couldn't find collider on ball");
            return;
        }

        //ballCollider.enabled = true;
    }

    public void OnLand(CircleCollider2D playerCollider) {
        Debug.Log("Land2");
        if (playerCollider.transform.position.y > -4.570165 && playerCollider.transform.position.y < 4.5) return;
        playerCollider.enabled = false;
    }
    public void OnFall(CircleCollider2D playerCollider) {Debug.Log("Fall2");
        if (playerCollider.transform.position.y > -4.570165 && playerCollider.transform.position.y < 4.5) return;
        playerCollider.enabled = true;
    }
}
