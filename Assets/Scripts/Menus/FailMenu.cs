using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FailMenu : MonoBehaviour, IDataPersistence
{
    public LevelSelector levelSelector;
    private int level;

    private Dictionary<string, int> highScores;
    
    void Start() {
        level = PlayerPrefs.GetInt("end");

        PlayerPrefs.SetInt("score", 0);
        PlayerPrefs.SetInt("end", 0);

        highScores = DataPersistenceManager.instance.GetHighScores(level);
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

    public void LoadData(GameData data) {}

    public void SaveData(ref GameData data) {
        //data.scores.Add(level, score);
    }
}
