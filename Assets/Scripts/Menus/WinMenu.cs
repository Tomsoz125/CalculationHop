using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using TMPro;

public class WinMenu : MonoBehaviour, IDataPersistence
{
    // PRIVATE SERIALIZED VARIABLES
    [SerializeField] private TMP_Text starCount;
    [Header("Level Selector Import")]
    [SerializeField] private LevelSelector levelSelector;

    [Header("Leaderboard Information")]
    [SerializeField] private GameObject leaderboard;
    [SerializeField] private GameObject leaderboardText;

    // PRIVATE VARIABLES
    private Dictionary<string, int> highScores;
    private int level;
    private int score;
    private int stars = 0;
    

    // Called before the first frame update
    void Start() {
        // Gets the level and the score from the local storage.
        level = PlayerPrefs.GetInt("win");
        score = PlayerPrefs.GetInt("score");

        // Unsets the level and score to ensure that the main menu doesn't loop back to win when it shouldn't
        PlayerPrefs.SetInt("score", 0);
        PlayerPrefs.SetInt("win", 0);

        // Saves all of the data to the storage file
        DataPersistenceManager.instance.SaveGame();

        // Gets a list of all high scores from the DPM
        highScores = DataPersistenceManager.instance.GetHighScores(level);

        // Displays leaderboard
        DisplayLeaderboard();
    }

    public void Update() {
        starCount.text = stars.ToString();
    }

    // Creates functionality for the BackButton
    public void BackButton(GameObject mainMenu) {
        // Disables the win screen and enables the main menu
        gameObject.SetActive(false);
        mainMenu.SetActive(true);
    }

    // Creates functionality for the NextButton
    public void NextButton() {
        // Uses the Level Selector's map to get the scene number of the next level.
        int sceneNo = levelSelector.scenes.GetValueOrDefault(level + 1, 1);

        // Loads the next level
        SceneManager.LoadScene(sceneNo);
    }

    // Required for data persistence
    public void LoadData(GameData data) {
        stars = data.stars;
    }

    // Called when the DPM tries to save data
    public void SaveData(ref GameData data) {
        // Only store the score if it's higher than what is already stored
        if (data.scores.ContainsKey(level)) {
            if (data.scores[level] < score) return;
            data.scores.Remove(level);
        }
        data.scores.Add(level, score);

        // If they haven't already completed this level before then add it to their completed levels
        if (!data.completedLevels.ContainsKey(level)) {
            data.completedLevels.Add(level, true);
        }
    }

    // Function that displays the leaderboard
    public void DisplayLeaderboard() {
        // Ensure that high scores are set
        if (highScores == null) return;

        // Creates a list of texts for the list
        List<GameObject> texts = new List<GameObject>();
        // Loop 5 times
        for (int i = 0; i < 5; i++) {
            // Creates the text for the leaderboard
            GameObject text = Instantiate(leaderboardText);
            // Sets the parent object and sets it to use relative positioning
            text.transform.SetParent(leaderboard.transform, false);
            // Sets the name of the text for easier debugging
            text.name = "Leaderboard #" + (i + 1);
            // Checks if there is a high score for this spot
            if (i + 1 > highScores.Count) {
                // Sets the text of the leaderboard text to the text in the editor
                text.GetComponent<TMP_Text>().text = text.GetComponent<TMP_Text>().text.Replace("{pos}", (i + 1).ToString()).Replace("{name}", "None").Replace("{score}", "0");
            } else {
                // Gets the score object
                KeyValuePair<string, int> score = highScores.ElementAt(i);
                // Sets the text of the leaderboard text to the text in the editor
                text.GetComponent<TMP_Text>().text = text.GetComponent<TMP_Text>().text = text.GetComponent<TMP_Text>().text.Replace("{pos}", (i + 1).ToString()).Replace("{name}", score.Key).Replace("{score}", score.Value.ToString());
            }

            // If it isn't the first text then set it to 80px below the last one
            if (texts.Count > 0) {
                text.transform.localPosition = new Vector3(text.transform.localPosition.x, texts[i - 1].transform.localPosition.y - 80, text.transform.localPosition.z);
            }
            texts.Add(text);
        }
    }
}
