using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject levelSelector;
    public GameObject failMenu;

    string playerName;

    void Awake() {
        Debug.Log(PlayerPrefs.GetInt("end"));
        if (PlayerPrefs.GetInt("end") > 0) {
            gameObject.SetActive(false);
            failMenu.SetActive(true);
        }
    }

    public void SelectLevel(GameObject textInput) {
        if (playerName == null || playerName == "") {
            int placeholderId = 0;
            for (int i = 0; i < textInput.transform.GetChild(0).childCount; i++) {
                if (textInput.transform.GetChild(0).GetChild(i).name == "Placeholder") {
                    placeholderId = i;
                    break;
                }
            }
            textInput.transform.GetChild(0).GetChild(placeholderId).GetComponent<TMP_Text>().text = "You must enter a name!";
            return;
        }

        PlayerPrefs.SetString("playerName", playerName);
        DataPersistenceManager.instance.LoadGame(playerName);
        if (!DataPersistenceManager.instance.HasGameData()) {
            DataPersistenceManager.instance.NewGame(playerName);
            DataPersistenceManager.instance.SaveGame();
        }

        gameObject.SetActive(false);
        levelSelector.SetActive(true);
    }

    public void QuitGame() {
        Debug.Log("QUIT!");
        Application.Quit();
    }

    public void SelectName(GameObject nameObject) {
        int placeholderId = 0;
        for (int i = 0; i < nameObject.transform.GetChild(0).childCount; i++) {
            if (nameObject.transform.GetChild(0).GetChild(i).name == "Placeholder") {
                placeholderId = i;
                break;
            }
        }
        nameObject.transform.GetChild(0).GetChild(placeholderId).gameObject.SetActive(false);
    }

    public void DeselectName(GameObject nameObject) {
        if (nameObject.transform.GetComponent<TMP_InputField>().text == null || nameObject.transform.GetComponent<TMP_InputField>().text == "") {
            int placeholderId = 0;
            for (int i = 0; i < nameObject.transform.GetChild(0).childCount; i++) {
                if (nameObject.transform.GetChild(0).GetChild(i).name == "Placeholder") {
                    placeholderId = i;
                    break;
                }
            }
            nameObject.transform.GetChild(0).GetChild(placeholderId).gameObject.SetActive(true);
        }
    }

    public void ChangeName(GameObject inputField) {
        TMP_InputField input = inputField.GetComponent<TMP_InputField>();
        if (input == null) {
            Debug.LogError("Couldn't find input field component on name input!");
            return;
        }

        playerName = input.text;
    }
}
