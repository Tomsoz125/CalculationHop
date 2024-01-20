using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelector : MonoBehaviour, IDataPersistence
{
    public GameObject levelObject;
    public DataPersistenceManager dataPersistenceManager;
    public List<GameObject> levelButtons = new List<GameObject>();
    public Dictionary<int, int> scenes = new Dictionary<int, int>();
    private Dictionary<int, int> scores;

    void Awake() {
        dataPersistenceManager.LoadGame(PlayerPrefs.GetString("name"));

        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string[] scenePathSplit = scenePath.Split("/");
            string name = scenePathSplit[scenePathSplit.Length - 1].Split(".")[0];

            if (!name.Contains("Level")) continue;

            int levelNo = int.Parse(name.Replace("Level", ""));

            Vector2 position;
            if (levelButtons.Count == 0) {
                position = new Vector2(-250, 150);
            } else {
                position = levelButtons.Last().transform.position;
                if (position.x >= 250) {
                    position.y -= 200;
                    position.x = -250;
                } else {
                    position.x += 250;
                }
            }

            GameObject levelObj = Instantiate(levelObject);
            levelObj.transform.SetParent(transform, false);
            levelObj.transform.localPosition = position;
            levelObj.name = "Level" + levelNo;
            Button buttonObj = levelObj.GetComponent<Button>();
            if (buttonObj == null) {
                Destroy(levelObj);
                Debug.LogError("Level object " + levelNo + " doesn't have a button component on it and was removed!");
                continue;
            }

            buttonObj.onClick.AddListener(delegate {SelectLevel(levelObj);});

            scenes.Add(levelNo, i);
        }
    }

    public void SelectLevel(GameObject btn) {
        int levelNo = int.Parse(btn.name.Replace("Level", ""));
        int sceneNo = scenes.GetValueOrDefault(levelNo, 1);
        SceneManager.LoadScene(sceneNo);
    }

    public void LoadData(GameData data) {
        scores = data.scores;
    }

    public void SaveData(ref GameData data) {}
}
