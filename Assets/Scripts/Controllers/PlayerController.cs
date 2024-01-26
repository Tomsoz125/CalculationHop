using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerController : MonoBehaviour, IDataPersistence
{
    public Transform groundCheck;
    public Transform topCheck;
    public LayerMask ground;
    public Camera mainCam;
    public float force = 5f;

    [SerializeField] private TMP_Text starCounter;
 
    const float groundRadius = .2f;
    public const float forceMultiplier = 1.9f;
    public bool grounded;
    private Rigidbody2D rb;
    public bool isDragging = false;
    public GameObject currentHoop;
    public GameObject lastHoop;
    private int stillFor = 0;
    public Vector3 lastLocation;
    public int starCount = 0;

    public Vector3 initialPosition;


    public UnityEvent<CircleCollider2D, GameObject> onLandEvent;
    public UnityEvent<CircleCollider2D, GameObject> onJumpEvent;
    public UnityEvent<CircleCollider2D, GameObject> onDragEvent;
    public CalculationController calcs;
    public EdgeCollision edgeCollision;
    public HoopController hoopController;
    private Vector2 direction;

    public int score = 0;

    public List<int> hoops = new List<int>();


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
            if (gameObject.tag == "Moveable" && calcs.canJump) {
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
                    if (calcs.canJump) {
                        calcs.canJump = false;
                        calcs.Init();
                    }
                }
            }
        }
    }

    void Update() {
        starCounter.text = starCount.ToString();

        lastLocation = gameObject.transform.position;
        if (stillFor > 3) {
            transform.position = lastHoop == null ? initialPosition : lastHoop.transform.position;
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

        Vector2 force = getForce();
        if (direction.x > 0) {
            force.x *= -1;
        }
        rb.velocity = direction * force;

        onJumpEvent.Invoke(rb.GetComponent<CircleCollider2D>(), currentHoop);
        grounded = false;
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

    public void LoadData(GameData data) {
        starCount = data.stars;
    }

    public void SaveData(ref GameData data) {
        data.stars = starCount;
    }
}
