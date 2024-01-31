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
    // PRIVATE SERIALIZED VARIABLES
    [Header("Menus")]
    [SerializeField] private GameObject levelSelector;
    [SerializeField] private GameObject failMenu;
    [SerializeField] private GameObject winMenu;

    // PRIVATE VARIABLES
    private string playerName;

    // Called when an enabled script instance is called.
    void Awake() {
        // If there is a failed level stored then switch to the fail menu.
        if (PlayerPrefs.GetInt("end") > 0) {
            gameObject.SetActive(false);
            failMenu.SetActive(true);
        }
        // If there is a won level stored then switch to the win menu.
        if (PlayerPrefs.GetInt("win") > 0) {
            gameObject.SetActive(false);
            winMenu.SetActive(true);
        }
    }

    // Defines functionality for the play button
    public void SelectLevel(GameObject textInput) {
        // The player must have entered a name
        if (playerName == null || playerName == "") {
            int placeholderId = 0;
            // Loop to get the child id of the placeholder text, its not the best workaround but for some reason just getting index 0 doesn't work
            // so this is here as a failsafe just in case the ids change often.
            for (int i = 0; i < textInput.transform.GetChild(0).childCount; i++) {
                if (textInput.transform.GetChild(0).GetChild(i).name == "Placeholder") {
                    placeholderId = i;
                    break;
                }
            }
            // Set the placeholder text to tell the user that they must have a name
            textInput.transform.GetChild(0).GetChild(placeholderId).GetComponent<TMP_Text>().text = "You must enter a name!";
            return;
        }

        // Set the name as a public variable
        PlayerPrefs.SetString("playerName", playerName);
        // Ask the DPM to load save data for that name
        DataPersistenceManager.instance.LoadGame(playerName);
        // If they don't have save data then create a new save file for that player
        if (!DataPersistenceManager.instance.HasGameData()) {
            DataPersistenceManager.instance.NewGame(playerName);
            DataPersistenceManager.instance.SaveGame();
        }

        // Disables the main menu and enables the level selector
        gameObject.SetActive(false);
        levelSelector.SetActive(true);
    }

    // Defines functionality for the quit button
    public void QuitGame() {
        // Log quit because Application.Quit() doesn't have any effect in the editor
        Debug.Log("QUIT!");
        Application.Quit();
    }

    // Called whenever the name input is selected
    public void SelectName(GameObject nameObject) {
        int placeholderId = 0;
        // Loop to get the child id of the placeholder text, its not the best workaround but for some reason just getting index 0 doesn't work
        // so this is here as a failsafe just in case the ids change often.
        for (int i = 0; i < nameObject.transform.GetChild(0).childCount; i++) {
            if (nameObject.transform.GetChild(0).GetChild(i).name == "Placeholder") {
                placeholderId = i;
                break;
            }
        }
        // Sets the placeholder text to be invisible just to tell the user that they have it selected
        nameObject.transform.GetChild(0).GetChild(placeholderId).gameObject.SetActive(false);
    }

    // Called whenever the name input is deselected
    public void DeselectName(GameObject nameObject) {
        // Check if there isn't a name written in the input
        if (nameObject.transform.GetComponent<TMP_InputField>().text == null || nameObject.transform.GetComponent<TMP_InputField>().text == "") {
            int placeholderId = 0;
            // Loop to get the child id of the placeholder text, its not the best workaround but for some reason just getting index 0 doesn't work
            // so this is here as a failsafe just in case the ids change often.
            for (int i = 0; i < nameObject.transform.GetChild(0).childCount; i++) {
                if (nameObject.transform.GetChild(0).GetChild(i).name == "Placeholder") {
                    placeholderId = i;
                    break;
                }
            }
            // Sets the placeholder to be visible
            nameObject.transform.GetChild(0).GetChild(placeholderId).gameObject.SetActive(true);
        }
    }

    // Called whenever the value of the name input is changed
    public void ChangeName(GameObject inputField) {
        // Get the input field and ensure it's not null
        TMP_InputField input = inputField.GetComponent<TMP_InputField>();
        if (input == null) {
            Debug.LogError("Couldn't find input field component on name input!");
            return;
        }

        // Sets the playerName variable to the input text
        playerName = input.text;
    }
}
