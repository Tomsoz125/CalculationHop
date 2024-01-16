using System.Collections;
using UnityEngine; // TODO: IN HOOP CHECK AND RESET BUTTON IF STUCK
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public Transform groundCheck;
    public Transform topCheck;
    public LayerMask ground;
    public Camera mainCam;

    const float groundRadius = .2f;
    public const float forceMultiplier = 1.9f;
    private bool grounded;
    private Rigidbody2D rb;
    private bool isDragging = false;
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
                Debug.Log("Up");
                isDragging = false;
                Move();
            }
        }
    }

    public void Move() {
        rb.isKinematic = false;

        forceApplied = getForce() * forceMultiplier * -1;
        rb.AddForce(forceApplied);
        onJumpEvent.Invoke(rb.GetComponent<CircleCollider2D>(), currentHoop);
        grounded = false;
        lastHoop = currentHoop;
        currentHoop = null;
    }

    public Vector3 getForce() {
        return (Camera.main.ScreenToWorldPoint(Input.mousePosition) - gameObject.transform.position).normalized * 2500;
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
                Debug.Log("still?");
                Debug.Log(lastLocation);
                Debug.Log(gameObject.transform.position);
                if (lastLocation == gameObject.transform.position) {
                    Debug.Log("still!");
                    stillFor += 1;
                } else {
                    stillFor = 0;
                    Debug.Log("stillf");
                }
            }
        }
    }
}
