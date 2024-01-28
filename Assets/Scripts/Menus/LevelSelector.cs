using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class LevelSelector : MonoBehaviour, IDataPersistence
{
    public GameObject levelObject;
    public List<GameObject> levelButtons = new List<GameObject>();
    public Dictionary<int, int> scenes = new Dictionary<int, int>();
    private GameData data;

    async void Awake() {
        DataPersistenceManager.instance.AddDataPersistenceObject(this);
        DataPersistenceManager.instance.LoadGame(PlayerPrefs.GetString("playerName"));
        while (data == null) {
            await Task.Delay(25);
        }
        Debug.Log(data);

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

            if (data.completedLevels.ContainsKey(levelNo)) {
                levelObj.GetComponent<Image>().color = new Color32(40, 255, 0, 255);
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
        Debug.Log(data);
        this.data = data;
    }

    public void SaveData(ref GameData data) {}
}
