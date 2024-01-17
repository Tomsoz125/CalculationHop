using System.Collections;
using UnityEngine; // TODO: IN HOOP CHECK AND RESET BUTTON IF STUCK
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public Transform groundCheck;
    public Transform topCheck;
    public LayerMask ground;
    public Camera mainCam;
    public float force = 5f;

    const float groundRadius = .2f;
    public const float forceMultiplier = 1.9f;
    private bool grounded;
    private Rigidbody2D rb;
    public bool isDragging = false;
    private GameObject currentHoop;
    private GameObject lastHoop;
    private int stillFor = 0;
    private Vector3 lastLocation;

    private Vector3 forceApplied;

    private Vector3 initialPosition;


    public UnityEvent<CircleCollider2D, GameObject> onLandEvent;
    public UnityEvent<CircleCollider2D, GameObject> onJumpEvent;
    public UnityEvent<CircleCollider2D, GameObject> onDragEvent;
    public EdgeCollision edgeCollision;
    public HoopController hoopController;
    private Vector2 direction;


    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        initialPosition = gameObject.transform.position;

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

        StartCoroutine(StuckCheck());
    }

    void OnMouseOver() {
        if (Input.GetMouseButtonDown(0)) {
            if (gameObject.tag == "Moveable") {
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
                    onLandEvent.Invoke(GetComponent<CircleCollider2D>(), colliders[i].gameObject);
                }
            }
        }
    }

    void Update() {
        lastLocation = gameObject.transform.position;
        if (stillFor > 3) {
            this.transform.position = lastHoop == null ? initialPosition : lastHoop.transform.position;
            stillFor = 0;
        }
    
        if (isDragging) {
            if (Input.GetMouseButtonUp(0)) {
                isDragging = false;
                Move();
            }
        }

        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 pos = transform.position;

        direction = mouse - pos;
    }

    public void Move() {
        rb.isKinematic = false;

        rb.velocity = direction * getForce();

        onJumpEvent.Invoke(rb.GetComponent<CircleCollider2D>(), currentHoop);
        grounded = false;
        lastHoop = currentHoop;
        currentHoop = null;
    }

    public Vector3 getForce() {
        return (Camera.main.ScreenToWorldPoint(Input.mousePosition) - gameObject.transform.position).normalized * 75;
    }

    public void SetCurrentHoop(GameObject hoop) {
        currentHoop = hoop;
    }

    public GameObject GetLastHoop() {
        return lastHoop;
    } 

    IEnumerator StuckCheck() {
        while (true) {
            yield return new WaitForSeconds(1f);
            if (currentHoop == null) {
                if (lastLocation == gameObject.transform.position) {
                    stillFor += 1;
                } else {
                    stillFor = 0;
                }
            }
        }
    }
}
