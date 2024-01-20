using UnityEditor.Experimental.GraphView;
using UnityEditor.Search;
using UnityEngine;

public class PlayerTrajectory : MonoBehaviour
{
    public PlayerController playerController;

    public GameObject pointPrefab;
    const float groundRadius = .5f;
    public GameObject[] points;
    public int numberPoints;
    
    private Vector2 direction;

    void Start() {
        points = new GameObject[numberPoints];
        for (int i = 0; i < numberPoints; i++) {
            points[i] = Instantiate(pointPrefab, transform.position, Quaternion.identity);
            points[i].SetActive(false);
        }
    }

    void Update() {
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pos = transform.position;

        direction = mouse - pos;

        for (int i = 0; i < points.Length; i++) {
            if (!playerController.isDragging || playerController.currentHoop == null) {
                points[i].SetActive(false);
                continue;
            }
            
            Vector2 pointPos = PointPosition(i * 0.1f);

            points[i].transform.position = pointPos;
            points[i].SetActive(true);
        }
    }

    Vector2 PointPosition(float t) {
        Vector2 force = playerController.getForce();
        if (direction.x > 0) {
            force.x *= -1;
        }

        Vector2 currentPosition = (Vector2) transform.position + (direction.normalized * force * t) + 0.5f * Physics2D.gravity * (t*t);

        currentPosition.y -= 1.5f;

        return currentPosition;
    }

    public void HideLine() {
        for (int i = 0; i < points.Length; i++) {
            points[i].SetActive(false);
        }
    }

    public bool CheckForCollision(Vector3 position) {
        if (playerController == null) return true;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, groundRadius, playerController.ground);

        return colliders.Length > 0;
    }
}
