using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class EdgeCollision : MonoBehaviour
{
    public UnityEvent OnFailEvent;
    public PlayerController playerController;

    void Awake () 
    {
        if (OnFailEvent == null) {
            OnFailEvent = new UnityEvent();
        }
        OnFailEvent.RemoveAllListeners();

      AddCollider();
    }

    void AddCollider () 
    {
      if (Camera.main==null) {Debug.LogError("Camera.main not found, failed to create edge colliders"); return;}

      var cam = Camera.main;
      if (!cam.orthographic) {Debug.LogError("Camera.main is not Orthographic, failed to create edge colliders"); return;}

      var bottomLeft = (Vector2)cam.ScreenToWorldPoint(new Vector3(0, -500, cam.nearClipPlane));
      var topLeft = (Vector2)cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight, cam.nearClipPlane));
      var topRight = (Vector2)cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, cam.nearClipPlane));
      var bottomRight = (Vector2)cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, -500, cam.nearClipPlane));

      var edge = GetComponent<EdgeCollider2D>() == null ? gameObject.AddComponent<EdgeCollider2D>() : GetComponent<EdgeCollider2D>();

      var edgePoints = new [] {bottomLeft,topLeft,topRight,bottomRight, bottomLeft};
      edge.points = edgePoints;
    }

    public void OnLand(CircleCollider2D playerCollider, GameObject gameObject) {
        if (playerCollider.transform.position.y > -5) return;

        if (playerController.score < 1) {
            playerController.transform.position = playerController.lastHoop == null ? playerController.initialPosition : playerController.lastHoop.transform.position;
        } else {
            PlayerPrefs.SetInt("end", int.Parse(SceneManager.GetActiveScene().name.Replace("Level", "")));
            PlayerPrefs.SetInt("score", playerController.score);
            SceneManager.LoadScene(0);
        }
        OnFailEvent.Invoke();
    }
}
