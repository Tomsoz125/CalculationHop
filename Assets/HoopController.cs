using System.Collections; // TODO: MAKE THE BALL STAY RENDERED AT THE BOTTOM OF THE HOOP WHEN IT ROTATES
using System.Collections.Generic; // TODOl ADD HOOP PULL BACK
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
    }

    public void OnBallDrag(CircleCollider2D playerColliderL, GameObject currentHoopL) {
        currentHoop = currentHoopL;
        isDragging = true;
    }

    public void OnBallJump(CircleCollider2D playerCollider, GameObject gameObject) {
        isDragging = false;
        currentHoop = null;
    }
}
