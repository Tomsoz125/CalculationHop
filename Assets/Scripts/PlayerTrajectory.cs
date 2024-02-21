using UnityEngine;

public class PlayerTrajectory : MonoBehaviour
{
    // PRIVATE SERIALIZED VARIABLES
    [Header("Ball Configuration")]
    [SerializeField] private PlayerController playerController;

    [Header("Line Configuration")]
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private int numberPoints;


    // PRIVATE VARIABLES
    private GameObject[] points;
    private Vector2 direction;

    // CONSTANTS
    const float groundRadius = .5f;

    // Called before the first frame update.
    void Start() {
        points = new GameObject[numberPoints];
        for (int i = 0; i < numberPoints; i++) {
            points[i] = Instantiate(pointPrefab, transform.position, Quaternion.identity);
            points[i].SetActive(false);
        }
    }

    // Called every frame
    void Update() {
        // Transforms a point in screen space to a point in game space.
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // Gets the position of the current object.
        Vector2 pos = transform.position;

        // Gets the direction that the ball will be fired.
        direction = mouse - pos;

        // Loops through the points
        for (int i = 0; i < points.Length; i++) {
            if (!playerController.isDragging || playerController.currentHoop == null) {
                // Hides all points if the player isnt dragging and isnt in a hoop.
                points[i].SetActive(false);
                // Continue skips the rest of the code but continues the loop.
                continue;
            }
            
            // Gets the position for the specified point.
            Vector2 pointPos = PointPosition(i * 0.1f);

            // Sets the position of the current point and enables it.
            points[i].transform.position = pointPos;
            points[i].SetActive(true);
        }
    }

    // Gets the position of a specified point.
    Vector2 PointPosition(float t) {
        // Gets the applied force from the PlayerController.
        Vector2 force = playerController.getForce();

        // Checks the direction of the hoop to see if its left or right and adjusts the force accordingly.
        if (direction.x > 0) {
            force.x *= -1;
        }

        // Some sort of physics that I got off the internet.
        Vector2 currentPosition = (Vector2) transform.position + (direction.normalized * force * t) + 0.5f * Physics2D.gravity * (t*t);

        currentPosition.y -= 1.5f;

        return currentPosition;
    }

    // Disables the points to hide the line
    public void HideLine() {
        // Loops through all the points.
        for (int i = 0; i < points.Length; i++) {
            // Disables the individual point
            points[i].SetActive(false);
        }
    }

    // Checks if there is a collision at a specified point.
    public bool CheckForCollision(Vector3 position) {
        if (playerController == null) return true;

        // Overlaps a circle at the point and collects the colliders it finds.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, groundRadius, playerController.ground);

        // Returns true if there were any colliders detected.
        return colliders.Length > 0;
    }
}
