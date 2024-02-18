using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;
using TMPro;

public class LevelSelector : MonoBehaviour, IDataPersistence
{
    // PRIVATE SERIALIZED VARIABLES
    [SerializeField] private GameObject levelObject;
    [SerializeField] private List<GameObject> levelButtons = new List<GameObject>();
    [SerializeField] private TMP_Text starCount;
    
    // PUBLIC VARIABLES
    public Dictionary<int, int> scenes = new Dictionary<int, int>();

    // PRIVATE VARIABLES
    private GameData data;

    // Called when an enabled script instance is called.
    async void Awake() {
        // Manually adds this file as an object that uses DPM because it isn't auto registered for some reason.
        DataPersistenceManager.instance.AddDataPersistenceObject(this);
        // Loads the game for the player name which is set in main menu.
        DataPersistenceManager.instance.LoadGame(PlayerPrefs.GetString("playerName"));
        // The data takes some time to get so this holds the code until data != null
        while (data == null) {
            // The function is async because this uses await
            await Task.Delay(25);
        }

        // Loops through all the scenes in the build settings
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
            // Gets the path of the scene
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            // Splits the path into an array
            string[] scenePathSplit = scenePath.Split("/");
            // Gets the name of the scene, there isn't a built in way to do this without loading the scene :(
            string name = scenePathSplit[scenePathSplit.Length - 1].Split(".")[0];

            // If it isn't a level then go to the next element in the loop
            if (!name.Contains("Level")) continue;

            // Gets the level number
            int levelNo = int.Parse(name.Replace("Level", ""));

            Vector2 position;
            if (levelButtons.Count == 0) {
                // If it's the first button then set the position to the initial position
                position = new Vector2(-250, 150);
            } else {
                // Gets the previous button's position
                position = levelButtons.Last().transform.localPosition;
                // Sets the position of the button
                if (position.x >= 250) {
                    position.y -= 200;
                    position.x = -250;
                } else {
                    position.x += 250;
                }
            }

            // Creates a new level button
            GameObject levelObj = Instantiate(levelObject);
            // Sets the parent object of the level button
            levelObj.transform.SetParent(transform, false);
            // Sets the position of the element to the location calculated above
            levelObj.transform.localPosition = position;
            // Sets the name of the element for debugging purposes
            levelObj.name = "Level" + levelNo;
            // Gets the button object that is on the element
            Button buttonObj = levelObj.GetComponent<Button>();
            // Ensures there is a button
            if (buttonObj == null) {
                Destroy(levelObj);
                Debug.LogError("Level object " + levelNo + " doesn't have a button component on it and was removed!");
                continue;
            }

            for (int c = 0; c < levelObj.transform.childCount; c++) {
                Transform child = levelObj.transform.GetChild(c);
                if (child.name.StartsWith("Text")) {
                    child.GetComponent<TMP_Text>().text = levelNo.ToString();
                }
            }

            // If the level has already been completed then change the colour
            if (data.completedLevels.ContainsKey(levelNo)) {
                levelObj.GetComponent<Image>().color = new Color32(40, 255, 0, 255);
            }

            // Adds an onclick listener to select the level.
            buttonObj.onClick.AddListener(delegate {SelectLevel(levelObj);});

            // Adds button to the level buttons
            levelButtons.Add(levelObj);

            // Add to the scene list
            scenes.Add(levelNo, i);
        }
    }

    public void Update() {
        starCount.text = data.stars.ToString();
    }

    // Defines functionality for selecting a level
    public void SelectLevel(GameObject btn) {
        // Gets the level number
        int levelNo = int.Parse(btn.name.Replace("Level", ""));
        // Uses the map to get the scene number
        int sceneNo = scenes.GetValueOrDefault(levelNo, 1);
        // Loads the scene
        SceneManager.LoadScene(sceneNo);
    }

    // Called when DPM loads data
    public void LoadData(GameData data) {
        // Sets the level data to the game data
        this.data = data;
    }

    // Required for the link with the DPM
    public void SaveData(ref GameData data) {}
}
