using System.Collections; // TODO: MAKE THE BALL STAY RENDERED AT THE BOTTOM OF THE HOOP WHEN IT ROTATES
using System.Collections.Generic; // TODOl ADD HOOP PULL BACK
using TMPro;
using UnityEngine;

public class HoopController : MonoBehaviour
{
    private float distance;
    private Rigidbody2D rb;
    private bool isDragging = false;

    private GameObject currentHoop;

    private Vector3 mousePos;
    private float angle;
    private Vector3 objectPos;
    private CircleCollider2D player;

    public PlayerController playerController;
    public TextMeshProUGUI scoreText;
    public int score = 0;
    public int perfectStreak = 0;
    public PlayerTrajectory trajectories;




    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        if (!isDragging) return;

        Debug.Log("drag");
        mousePos = Input.mousePosition;
        mousePos.z = -20;
        objectPos = Camera.main.WorldToScreenPoint(currentHoop.transform.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;
        angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        angle += 90;
        currentHoop.transform.rotation = Quaternion.Euler(0, 0, angle);

        trajectories.Trajectory(Quaternion.Euler(0, 0, angle));
    }

    public void OnBallDrag(CircleCollider2D playerColliderL, GameObject currentHoopL) {
        currentHoop = currentHoopL;
        player = playerColliderL;
        isDragging = true;
    }

    public void OnBallJump(CircleCollider2D playerCollider, GameObject gameObject) {
        isDragging = false;
        player = null;
        currentHoop = null;
    }

    void OnTriggerEnter2D(Collider2D ball) {
        playerController.SetCurrentHoop(gameObject);
        if (playerController.GetLastHoop() != null && playerController.GetLastHoop() != gameObject) {
            score += 1;
            scoreText.SetText(score.ToString());
        }
    }
}
