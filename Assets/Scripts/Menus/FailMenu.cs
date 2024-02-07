using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

public class FailMenu : MonoBehaviour, IDataPersistence
{
    public LevelSelector levelSelector;
    [SerializeField] private TMP_Text starCount;
    private int level;
    private int score;

    private Dictionary<string, int> highScores;

    public GameObject leaderboard;
    public GameObject leaderboardText;

    private int stars = 0;
    
    void Start() {
        level = PlayerPrefs.GetInt("end");
        score = PlayerPrefs.GetInt("score");

        PlayerPrefs.SetInt("score", 0);
        PlayerPrefs.SetInt("end", 0);

        DataPersistenceManager.instance.SaveGame();

        highScores = DataPersistenceManager.instance.GetHighScores(level);

        DisplayLeaderboard();
    }

    public void BackButton(GameObject mainMenu) {
        gameObject.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void RetryButton() {
        int levelNo = PlayerPrefs.GetInt("end");
        int sceneNo = levelSelector.scenes.GetValueOrDefault(levelNo, 1);

        SceneManager.LoadScene(sceneNo);
    }

    public void LoadData(GameData data) {
        stars = data.stars;
    }

    public void SaveData(ref GameData data) {
        if (data.scores.ContainsKey(level)) {
            if (data.scores[level] < score) return;
            data.scores.Remove(level);
        }

        data.scores.Add(level, score);
    }

    public void Update() {
        starCount.text = stars.ToString();
    }


    public void DisplayLeaderboard() {
        if (highScores == null) return;
        List<GameObject> texts = new List<GameObject>();
        for (int i = 0; i < 5; i++) {
            GameObject text = Instantiate(leaderboardText);
            text.transform.SetParent(leaderboard.transform, false);
            text.name = "Leaderboard #" + (i + 1);
            if (i + 1 > highScores.Count) {
                text.GetComponent<TMP_Text>().text = text.GetComponent<TMP_Text>().text.Replace("{pos}", (i + 1).ToString()).Replace("{name}", "None").Replace("{score}", "0");
            } else {
                KeyValuePair<string, int> score = highScores.ElementAt(i);
                text.GetComponent<TMP_Text>().text = text.GetComponent<TMP_Text>().text = text.GetComponent<TMP_Text>().text.Replace("{pos}", (i + 1).ToString()).Replace("{name}", score.Key).Replace("{score}", score.Value.ToString());
            }

            if (texts.Count > 0) {
                text.transform.localPosition = new Vector3(text.transform.localPosition.x, texts[i - 1].transform.localPosition.y - 80, text.transform.localPosition.z);
            }
            texts.Add(text);
        }
    }
}
