using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_SeadZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
            GameManager.instance.GameEnded();
    }
}
