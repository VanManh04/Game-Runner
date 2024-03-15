using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform[] leverlPart;
    [SerializeField] private Vector3 nextPartPosition;

    [SerializeField] private float distanceToSpawn;
    [SerializeField] private float distanceToDelete;
    [SerializeField] private Transform player;

    void Update()
    {
        GeneratePlatform();
        DeletePlatform();
    }

    private void GeneratePlatform()
    {
        if (Vector2.Distance(player.transform.position,nextPartPosition)<distanceToSpawn)
        {
            Transform part = leverlPart[Random.Range(0, leverlPart.Length)];

            Vector2 newPosition = new Vector2(nextPartPosition.x - part.Find("StartPoint").position.x, 0);

            Transform newPast = Instantiate(part, newPosition, transform.rotation, transform);

            nextPartPosition = newPast.Find("EndPoint").position;
        }
    }

    private void DeletePlatform()
    {
        if (transform.childCount > 0)
        {
            Transform partToDelete = transform.GetChild(0);

            if (Vector2.Distance(player.transform.position, partToDelete.transform.position) > distanceToDelete)
            {
                Destroy(partToDelete.gameObject);
            }
        }
    }
}
