using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using Unity.VisualScripting;
using UnityEngine;

public class MovingScreenController : MonoBehaviour
{
    [Header("Camera Coordinates")]
    [SerializeField] private int cameraY = 0;
    [SerializeField] private int cameraZ = -10;

    [Header("")]
    [SerializeField] private PlayerController playerController;

    void Update() {
        if (playerController.transform.position.y <= cameraY) {
            if (transform.position.y != cameraY) transform.position = new Vector3(transform.position.x, 0, cameraZ);
            return;
        }

        transform.position = new Vector3(transform.position.x, playerController.transform.position.y, cameraZ);
    }
}
