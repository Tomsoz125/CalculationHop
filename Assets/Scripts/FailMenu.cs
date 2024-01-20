using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FailMenu : MonoBehaviour, IDataPersistence
{
    public LevelSelector levelSelector;
    public DataPersistenceManager dataPersistenceManager;

    private string playerName;
    private int score;
    private int level;
    
    void Awake() {
        score = PlayerPrefs.GetInt("score");
        playerName = PlayerPrefs.GetString("name");
        level = PlayerPrefs.GetInt("end");

        PlayerPrefs.SetInt("score", 0);
        PlayerPrefs.SetInt("end", 0);

        dataPersistenceManager.SaveGame();
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
        data.scores.Add(level, score);
    }
}
