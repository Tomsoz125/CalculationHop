using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public Transform groundCheck;
    public Transform topCheck;
    public LayerMask ground;
    public Camera mainCam;

    const float groundRadius = .2f;
    private bool grounded;
    private Rigidbody2D rb;
    private bool isDragging = false;

    private Vector3 forceApplied;


    public UnityEvent<CircleCollider2D> onLandEvent;
    public UnityEvent<CircleCollider2D> onJumpEvent;
    public EdgeCollision edgeCollision;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();

        if (onLandEvent == null) {
            onLandEvent = new UnityEvent<CircleCollider2D>();
        }
        if (onJumpEvent == null) {
            onJumpEvent = new UnityEvent<CircleCollider2D>();
        }
        onLandEvent.RemoveAllListeners();
        onJumpEvent.RemoveAllListeners();
        onLandEvent.AddListener(edgeCollision.OnLand);
        onJumpEvent.AddListener(edgeCollision.OnFall);
    }

    void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) {
            if (gameObject.tag == "Moveable") {
                Debug.Log("Moveable");
                isDragging = true;
                rb.isKinematic = true;
            }
        }
    }

    void FixedUpdate() {
        bool wasGrounded = grounded;
        grounded = false;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundRadius, ground);
        for (int i = 0; i < colliders.Length; i++) {
            if (colliders[i].gameObject != gameObject) {
                grounded = true;
                if (!wasGrounded) {
                    onLandEvent.Invoke(GetComponent<CircleCollider2D>());
                    Debug.Log("landed");
                }
            }
        }
    }

    void Update() {
        if (isDragging) {
            if (Input.GetMouseButtonUp(0)) {
                Debug.Log("Up");
                isDragging = false;
                Move(1, Input.mousePosition, gameObject.transform.position);
            }
        }
    }

    public void Move(float power, Vector2 clickPos1, Vector2 clickPos2) {
        rb.isKinematic = false;

        forceApplied = getForce() * -1;
        rb.AddForce(forceApplied);
    }

    public Vector3 getForce() {
        return (Camera.main.ScreenToWorldPoint(Input.mousePosition) - gameObject.transform.position).normalized * 2500;
    }
}
