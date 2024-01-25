using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableController : MonoBehaviour
{
    private void OnTriggerEnter2D() {
        gameObject.SetActive(false);
        // TODO: SAVE STARS
    }
}
