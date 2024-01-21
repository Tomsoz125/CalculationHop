using System.Collections; // TODO: MAKE THE BALL STAY RENDERED AT THE BOTTOM OF THE HOOP WHEN IT ROTATES
using System.Collections.Generic; // TODO: ADD HOOP PULL BACK
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
    private int hoopId;

    public PlayerController playerController;
    public TextMeshProUGUI scoreText;
    public PlayerTrajectory trajectories;
    public Quaternion rot;

    




    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        
        hoopId = int.Parse(gameObject.name.Replace("Hoop", ""));
    }

    void FixedUpdate() {
        if (!isDragging || !currentHoop) return;

        mousePos = Input.mousePosition;
        mousePos.z = -20;
        objectPos = Camera.main.WorldToScreenPoint(currentHoop.transform.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;
        angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        angle += 90;
        rot = Quaternion.Euler(0, 0, angle);
        currentHoop.transform.rotation = rot;
    }

    public void OnBallDrag(CircleCollider2D playerColliderL, GameObject currentHoopL) {
        currentHoop = currentHoopL;
        player = playerColliderL;
        isDragging = true;
    }

    public void OnBallJump(CircleCollider2D playerCollider, GameObject gameObject) {
        isDragging = false;
        trajectories.HideLine();
        if (currentHoop != null) currentHoop.transform.rotation = Quaternion.Euler(0,0,0);
        player = null;
        currentHoop = null;
    }

    void OnTriggerEnter2D(Collider2D ball) {
        if (hoopId == 0) playerController.SetCurrentHoop(gameObject);
        if (playerController.hoops.Contains(hoopId)) return;

        playerController.SetCurrentHoop(gameObject);
        if (playerController.GetLastHoop() != null && playerController.GetLastHoop() != gameObject) {
            playerController.score += 1;
            playerController.hoops.Add(hoopId);
            scoreText.SetText(playerController.score.ToString());
        }
    }

    void OnTriggerExit2D(Collider2D ball) {
        if (!playerController.grounded) {
            playerController.lastHoop = playerController.currentHoop;
            playerController.currentHoop = null;
        }
    }
}
