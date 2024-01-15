using Unity.VisualScripting; // TODO: PLAYER CAN COLLIDE WITH CAMERA FIX IT
using UnityEngine; // TODO: IN HOOP CHECK AND RESET BUTTON IF STUCK
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public Transform groundCheck;
    public Transform topCheck;
    public LayerMask ground;
    public Camera mainCam;

    const float groundRadius = .2f;
    const float forceMultiplier = 1.9f;
    private bool grounded;
    private Rigidbody2D rb;
    private bool isDragging = false;
    private GameObject currentHoop;

    private Vector3 forceApplied;


    public UnityEvent<CircleCollider2D, GameObject> onLandEvent;
    public UnityEvent<CircleCollider2D, GameObject> onJumpEvent;
    public UnityEvent<CircleCollider2D, GameObject> onDragEvent;
    public EdgeCollision edgeCollision;
    public HoopController hoopController;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();

        if (onLandEvent == null) {
            onLandEvent = new UnityEvent<CircleCollider2D, GameObject>();
        }
        if (onJumpEvent == null) {
            onJumpEvent = new UnityEvent<CircleCollider2D, GameObject>();
        }
        if (onDragEvent == null) {
            onJumpEvent = new UnityEvent<CircleCollider2D, GameObject>();
        }
        onLandEvent.RemoveAllListeners();
        onDragEvent.RemoveAllListeners();
        onJumpEvent.RemoveAllListeners();
        onLandEvent.AddListener(edgeCollision.OnLand);
        onDragEvent.AddListener(hoopController.OnBallDrag);
        onJumpEvent.AddListener(hoopController.OnBallJump);
    }

    void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) {
            if (gameObject.tag == "Moveable") {
                Debug.Log("Moveable");
                isDragging = true;
                onDragEvent.Invoke(gameObject.GetComponent<CircleCollider2D>(), currentHoop);
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
                    if (colliders[i].gameObject.tag != "HoopMain") {
                        currentHoop = colliders[i].gameObject.transform.parent.gameObject;
                        Debug.Log(currentHoop.name);
                    } else {
                        currentHoop = colliders[i].gameObject;
                    }
                    
                    onLandEvent.Invoke(GetComponent<CircleCollider2D>(), colliders[i].gameObject);
                    // rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationZ;
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

        forceApplied = getForce() * forceMultiplier * -1;
        rb.AddForce(forceApplied);
        onJumpEvent.Invoke(rb.GetComponent<CircleCollider2D>(), currentHoop);
        grounded = false;
    }

    public Vector3 getForce() {
        return (Camera.main.ScreenToWorldPoint(Input.mousePosition) - gameObject.transform.position).normalized * 2500;
    }
}
