using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private bool isEnd = false;
    [SerializeField] private GameObject starPrefab;

    public PlayerController playerController;
    public TextMeshProUGUI scoreText;
    public PlayerTrajectory trajectories;
    public Quaternion rot;



    void Awake() {
        rb = GetComponent<Rigidbody2D>();
        
        if (gameObject.name == "HoopEnd") isEnd = true;
        else hoopId = int.Parse(gameObject.name.Replace("Hoop", ""));

        if (Random.value > 0.5) {
            if (hoopId == 0) return;

            GameObject star = Instantiate(starPrefab);
            star.name = "Star" + hoopId;
            star.transform.SetParent(gameObject.transform, false);
            star.transform.localPosition = new Vector2(0, 1);
            star.GetComponent<CollectableController>().playerController = playerController;
        }
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
        for (int i = 0; i < gameObject.transform.childCount; i++) {
            Transform child = gameObject.transform.GetChild(i);
            if (child.name.StartsWith("Object")) {
                child.gameObject.SetActive(false);
            }
            if (child.name.EndsWith("_0")) {
                SpriteRenderer renderer = child.GetComponent<SpriteRenderer>();
                renderer.color = new Color32(140, 140, 140, 255);
            }
        }

        playerController.SetCurrentHoop(gameObject);
        if (isEnd) {
            PlayerPrefs.SetInt("win", int.Parse(SceneManager.GetActiveScene().name.Replace("Level", "")));
            PlayerPrefs.SetInt("score", playerController.score);

            SceneManager.LoadScene(0);
            return;
        }
        if (playerController.hoops.Contains(hoopId)) return;
        if (hoopId == 0) playerController.hoops.Add(0);
        
        if (playerController.GetLastHoop() != null && playerController.GetLastHoop() != gameObject) {
            playerController.score += 1;
            playerController.hoops.Add(hoopId);
            scoreText.SetText(playerController.score.ToString());
        }
    }

    void OnTriggerExit2D(Collider2D ball) {
        if (playerController.lastHoop == gameObject) return;
        if (playerController.lastHoop != null) {
            playerController.lastHoop.SetActive(false);
        }
        playerController.lastHoop = playerController.currentHoop;
        playerController.currentHoop = null;
    }
}
