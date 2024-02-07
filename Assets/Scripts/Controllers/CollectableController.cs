using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : MonoBehaviour, IDataPersistence
{
    public PlayerController playerController;

    public void Awake() {
        DataPersistenceManager.instance.AddDataPersistenceObject(this);
    }

    private void OnTriggerEnter2D() {
        gameObject.SetActive(false);
        
        playerController.starCount += 1;
        DataPersistenceManager.instance.SaveGame();
    }

    public void LoadData(GameData data) {
        playerController.starCount = data.stars;
    }

    public void SaveData(ref GameData data) {
        data.stars = playerController.starCount;
    }
}
