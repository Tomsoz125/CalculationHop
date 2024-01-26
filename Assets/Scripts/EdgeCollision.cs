using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class EdgeCollision : MonoBehaviour
{ 
    // PRIVATE SERIALIZED VARIABLES
    [Header("Events")]
    [SerializeField] private UnityEvent OnFailEvent;

    [Header("Ball Configuration")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PhysicsMaterial2D bounce;

    // Called when an enabled script instance is called.
    void Awake() {
        // Initialize the fail event.
        if (OnFailEvent == null) {
            OnFailEvent = new UnityEvent();
        }
        OnFailEvent.RemoveAllListeners();

        // Generates the EdgeCollider2D on the camera.
        AddCollider();
    }

    // Adds the EdgeCollider2D to the camera.
    void AddCollider () {
        // Checks if camera exists
        if (Camera.main==null) {Debug.LogError("Camera.main not found, failed to create edge colliders"); return;}

        var cam = Camera.main;
        // Camera needs to be orthographic for it to work for some reason. 
        if (!cam.orthographic) {Debug.LogError("Camera.main is not Orthographic, failed to create edge colliders"); return;}

        // Finds the positions of the corners to create the collider. -500 is used to make the bottom lower than the bottom of the camera
        // so the ball can physically fall of the bottom of the cam.
        var bottomLeft = (Vector2)cam.ScreenToWorldPoint(new Vector3(0, -500, cam.nearClipPlane));
        var topLeft = (Vector2)cam.ScreenToWorldPoint(new Vector3(0, cam.pixelHeight, cam.nearClipPlane));
        var topRight = (Vector2)cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, cam.nearClipPlane));
        var bottomRight = (Vector2)cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, -500, cam.nearClipPlane));

        // Gets or creates an EdgeCollider2D.
        var edge = GetComponent<EdgeCollider2D>() == null ? gameObject.AddComponent<EdgeCollider2D>() : GetComponent<EdgeCollider2D>();

        // Sets the points of the collider to the corners of the camera.
        var edgePoints = new [] {bottomLeft, topLeft, topRight, bottomRight, bottomLeft};
        edge.sharedMaterial = bounce;
        edge.points = edgePoints;
    }

    // Called when the ball lands
    public void OnLand(CircleCollider2D playerCollider, GameObject gameObject) {
        // If they have landed within the camera then do nothing.
        if (playerCollider.transform.position.y > -5) return;

        if (playerController.score < 1) {
            // If they failed the first jump dont bother going to the fail screen and just reset them.
            playerController.transform.position = playerController.lastHoop == null ? playerController.initialPosition : playerController.lastHoop.transform.position;
        } else {
            // Sets the level number and the score to be accessed in the fail menu.
            PlayerPrefs.SetInt("end", int.Parse(SceneManager.GetActiveScene().name.Replace("Level", "")));
            PlayerPrefs.SetInt("score", playerController.score);

            // Loads the menus scene.
            SceneManager.LoadScene(0);
        }

        // Calls the on fail event because they have failed.
        OnFailEvent.Invoke();
    }
}
