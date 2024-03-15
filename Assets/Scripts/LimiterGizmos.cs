using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimiterGizmos : MonoBehaviour
{
    [SerializeField] private Transform start;
    [SerializeField] private Transform end;
    [SerializeField] private Transform groundLevel1;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(start.position,new Vector2(start.position.x,start.position.y+1000));
        Gizmos.DrawLine(start.position,new Vector2(start.position.x,start.position.y-1000));

        Gizmos.DrawLine(end.position,new Vector2(end.position.x,end.position.y+1000));
        Gizmos.DrawLine(end.position,new Vector2(end.position.x,end.position.y-1000));

        Gizmos.DrawLine(groundLevel1.position,new Vector2(groundLevel1.position.x + 1000, groundLevel1.position.y));
        Gizmos.DrawLine(groundLevel1.position,new Vector2(groundLevel1.position.x-1000,groundLevel1.position.y));

    }
}
