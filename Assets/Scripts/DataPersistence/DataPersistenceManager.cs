using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class DataPersistenceManager : MonoBehaviour {
    [Header("Debugging")]
    [SerializeField] private bool initializeDataIfNull = false;
    [SerializeField] private string defaultName = "Tom";

    [Header("File Storage Config")]
    [SerializeField] private string fileName;
    [SerializeField] private bool useEncryption;

    private GameData gameData;
    public static DataPersistenceManager instance { get; private set; }
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    private void Awake() {
        if (instance != null) {
            Debug.LogError("Found more than one Data Persistence Manager in the scene!");
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        dataPersistenceObjects = FindAllDataPersistenceObjects();

        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
        dataPersistenceObjects = FindAllDataPersistenceObjects();
    }  
    public void OnSceneUnloaded(Scene scene) {
        SaveGame();
    } 

    public void NewGame(string name) {
        if (name == null || name == "") name = defaultName;
        Debug.Log("Created a new game for \"" + name + "\".");
        gameData = new GameData(name);
    }

    public void LoadGame(string name) {
        if (name == null || name == "") name = defaultName;
        gameData = dataHandler.Load(name);

        Debug.Log("Loading \"" + name + "\"'s game.");

        if (gameData == null && initializeDataIfNull) {
            NewGame(name);
        }

        if (gameData == null) {
            return;
        }

        Debug.Log("Loaded \"" + name + "\"'s game.");

        Debug.Log(dataPersistenceObjects.Count);
        foreach (IDataPersistence dataPersistence in dataPersistenceObjects) {
            Debug.Log("hello");
            dataPersistence.LoadData(gameData);
        }
    }

    public void SaveGame() {
        Debug.Log("tried to save :( " + gameData.name);
        if (gameData == null) return;

        foreach (IDataPersistence dataPersistence in dataPersistenceObjects) {
            dataPersistence.SaveData(ref gameData);
        }

        dataHandler.Save(gameData, gameData.name);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects() {
        Debug.Log("hello1");
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    public void AddDataPersistenceObject(IDataPersistence obj) {
        dataPersistenceObjects.Add(obj);
    }

    public bool HasGameData() {
        return gameData != null;
    }

    public Dictionary<string, int> GetHighScores(int level) {
        Dictionary<string, int> scores = new Dictionary<string, int>();
        GameData[] data = dataHandler.LoadAll();
        foreach (GameData d in data) {
            if (level < 0) {
                scores.Add(d.name, d.scores.Count);
            } else {
                if (d.scores.ContainsKey(level)) {
                    scores.Add(d.name, d.scores.GetValueOrDefault(level, -1));
                }
            }
        }

        return new Dictionary<string, int>(from entry in scores orderby entry.Value descending select entry);
    }
}