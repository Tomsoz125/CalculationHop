using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : MonoBehaviour
{
    public PlayerController playerController;

    private void OnTriggerEnter2D() {
        gameObject.SetActive(false);
        
        playerController.starCount += 1;
    }
}
