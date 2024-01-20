using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;

public class DataPersistenceManager : MonoBehaviour {
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
        DontDestroyOnLoad(gameObject.transform.parent);

        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);
    }

    private void Start() {
        dataPersistenceObjects = FindAllDataPersistenceObjects();        
    }

    public void NewGame(string name) {
        gameData = new GameData(name);
    }

    public void LoadGame(string name) {
        gameData = dataHandler.Load();

        if (gameData == null) {
            NewGame(name);
        }

        foreach (IDataPersistence dataPersistence in dataPersistenceObjects) {
            dataPersistence.LoadData(gameData);
        }
    }

    public void SaveGame() {
        //if (dataPersistenceObjects == null) dataPersistenceObjects = FindAllDataPersistenceObjects();
        foreach (IDataPersistence dataPersistence in dataPersistenceObjects) {
            dataPersistence.SaveData(ref gameData);
        }

        dataHandler.Save(gameData);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects() {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}